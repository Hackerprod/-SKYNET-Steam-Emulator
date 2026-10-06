using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

// Compares the managed callback/struct layouts of steam_api.dll against expected/callbacks_<arch>.txt
// (generated from the real SDK headers by gen_expected.py). The expected file used depends on the
// bitness of THIS process, so run the x64 and the x86 build of the tool.
internal static class Program
{
    private sealed class Expected
    {
        public string Name;
        public int Id;
        public long Size;
        public List<KeyValuePair<string, long>> Fields = new List<KeyValuePair<string, long>>();
        public bool IsCallback => Id != -1;
    }

    private static int fieldsCompared, structsCompared;

    private static readonly BindingFlags AllInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static int Main(string[] args)
    {
        if (args.Length < 2 || args[0] != "verify")
        {
            Console.Error.WriteLine("usage: CallbackLayoutCheck verify <steam_api.dll> <toolDir containing expected/, aliases.txt, waivers.txt>");
            return 2;
        }

        string arch = IntPtr.Size == 8 ? "x64" : "x86";
        string dll = Path.GetFullPath(args[1]);
        string toolDir = Path.GetFullPath(args.Length > 2 ? args[2] : ".");

        var expected = LoadExpected(Path.Combine(toolDir, "expected", "callbacks_" + arch + ".txt"));
        var aliases = LoadAliases(Path.Combine(toolDir, "aliases.txt"));
        var waivers = LoadWaivers(Path.Combine(toolDir, "waivers.txt"));

        Assembly asm = Assembly.LoadFrom(dll);
        Type cbInterface = asm.GetType("SKYNET.Callback.ICallbackData", true);
        PropertyInfo idProp = cbInterface.GetProperty("CallbackType");
        PropertyInfo sizeProp = cbInterface.GetProperty("DataSize");

        Type[] allTypes;
        try { allTypes = asm.GetTypes(); }
        catch (ReflectionTypeLoadException ex) { allTypes = ex.Types.Where(t => t != null).ToArray(); }

        var callbackTypes = allTypes.Where(t => t.IsValueType && cbInterface.IsAssignableFrom(t)).ToList();
        var byName = new Dictionary<string, List<Type>>();
        foreach (var t in allTypes.Where(t => t.IsValueType && !t.IsEnum && !t.IsGenericType))
        {
            if (!byName.TryGetValue(t.Name, out var list)) byName[t.Name] = list = new List<Type>();
            list.Add(t);
        }

        var lines = new List<string>();
        var counts = new Dictionary<string, int>();
        var matched = new HashSet<Type>();

        void Report(string kind, string name, string text)
        {
            bool waived = waivers.TryGetValue(kind + " " + name, out string reason);
            if (kind == "NOTE" || kind == "OUR-ONLY") waived = false;
            lines.Add((waived ? "WAIVED " + kind : kind) + " " + name + ": " + text + (waived ? "  [" + reason + "]" : ""));
            string key = waived ? "WAIVED" : kind;
            counts[key] = counts.TryGetValue(key, out int c) ? c + 1 : 1;
        }

        // callback id -> managed types, for the fallback when the managed name differs from the header name
        var byId = new Dictionary<int, List<Type>>();
        foreach (var t in callbackTypes)
        {
            int id = ReadId(t, idProp);
            if (!byId.TryGetValue(id, out var list)) byId[id] = list = new List<Type>();
            list.Add(t);
        }

        foreach (var e in expected.OrderBy(x => x.Name, StringComparer.Ordinal))
        {
            var candidates = new List<Type>();
            if (byName.TryGetValue(e.Name, out var named)) candidates.AddRange(named.Where(t => !e.IsCallback || callbackTypes.Contains(t)));
            foreach (var a in aliases.Where(a => a.Value == e.Name))
                if (byName.TryGetValue(a.Key, out var aliased)) candidates.AddRange(aliased);

            if (candidates.Count == 0 && e.IsCallback && byId.TryGetValue(e.Id, out var sameId))
            {
                var renamed = sameId.Where(t => !expected.Any(o => o.IsCallback && o.Name == t.Name)).ToList();
                if (renamed.Count == 1)
                {
                    candidates.Add(renamed[0]);
                    Report("NOTE", e.Name, "implemented as " + renamed[0].Name + " (same callback id " + e.Id + ")");
                }
            }

            if (candidates.Count == 0)
            {
                if (e.IsCallback) Report("MISSING", e.Name, "id " + e.Id + ", size " + e.Size);
                continue;
            }

            foreach (var t in candidates.Distinct())
            {
                matched.Add(t);
                Compare(e, t, idProp, sizeProp, Report);
            }
        }

        foreach (var t in callbackTypes.Where(t => !matched.Contains(t)).OrderBy(t => t.Name, StringComparer.Ordinal))
            Report("OUR-ONLY", t.Name, "callback id " + ReadId(t, idProp) + ", size " + SafeSize(t));

        foreach (string l in lines) Console.WriteLine(l);

        int failures = new[] { "MISSING", "ID-DIFF", "SIZE-DIFF", "OFFSET-DIFF" }.Sum(k => counts.TryGetValue(k, out int c) ? c : 0);
        Console.WriteLine("[" + arch + "] expected " + expected.Count(e => e.IsCallback) + " callbacks + " + expected.Count(e => !e.IsCallback) + " structs; managed ICallbackData types " + callbackTypes.Count + "; compared " + structsCompared + " structs, " + fieldsCompared + " field offsets");
        Console.WriteLine("[" + arch + "] " + string.Join(" ", new[] { "MISSING", "ID-DIFF", "SIZE-DIFF", "OFFSET-DIFF", "WAIVED", "OUR-ONLY", "NOTE" }.Select(k => k + "=" + (counts.TryGetValue(k, out int c) ? c : 0))));
        Console.WriteLine(failures == 0 ? "OK [" + arch + "]: callback/struct layouts match the SDK 1.65 headers" : "FAIL [" + arch + "]: " + failures + " mismatches");
        return failures == 0 ? 0 : 1;
    }

    private static void Compare(Expected e, Type t, PropertyInfo idProp, PropertyInfo sizeProp, Action<string, string, string> report)
    {
        string name = e.Name;
        if (e.IsCallback)
        {
            int id = ReadId(t, idProp);
            if (id != e.Id) report("ID-DIFF", name, "expected " + e.Id + " got " + id);
            try
            {
                object inst = Activator.CreateInstance(t);
                int ds = (int)sizeProp.GetValue(inst);
                if (ds != SafeSize(t)) report("SIZE-DIFF", name, "DataSize " + ds + " != Marshal.SizeOf " + SafeSize(t));
            }
            catch (Exception) { }
        }

        structsCompared++;
        int size = SafeSize(t);
        if (size != e.Size) report("SIZE-DIFF", name, "expected " + e.Size + " got " + size + (t.Name != e.Name ? " (managed " + t.Name + ")" : ""));

        var fields = t.GetFields(AllInstance).OrderBy(f => f.MetadataToken).ToList();
        var pairs = new List<KeyValuePair<FieldInfo, KeyValuePair<string, long>>>();
        if (fields.Count == e.Fields.Count)
        {
            for (int i = 0; i < fields.Count; i++) pairs.Add(new KeyValuePair<FieldInfo, KeyValuePair<string, long>>(fields[i], e.Fields[i]));
        }
        else
        {
            var used = new HashSet<string>();
            foreach (var f in fields)
            {
                var m = e.Fields.FirstOrDefault(x => !used.Contains(x.Key) && Norm(x.Key) == Norm(f.Name));
                if (m.Key == null) continue;
                used.Add(m.Key);
                pairs.Add(new KeyValuePair<FieldInfo, KeyValuePair<string, long>>(f, m));
            }
            if (e.Fields.Count > 0 && fields.Count > 0 && pairs.Count < Math.Min(fields.Count, e.Fields.Count))
                report("NOTE", name, "field count " + fields.Count + " vs header " + e.Fields.Count + ", " + pairs.Count + " mapped by name");
        }

        foreach (var p in pairs)
        {
            long off;
            try { off = (long)Marshal.OffsetOf(t, p.Key.Name); }
            catch (Exception) { continue; }
            fieldsCompared++;
            if (off != p.Value.Value)
                report("OFFSET-DIFF", name, p.Key.Name + " (header " + p.Value.Key + ") expected " + p.Value.Value + " got " + off);
        }
    }

    private static int ReadId(Type t, PropertyInfo idProp)
    {
        try { return (int)(ulong)idProp.GetValue(Activator.CreateInstance(t)); }
        catch (Exception) { return -2; }
    }

    private static int SafeSize(Type t)
    {
        try { return Marshal.SizeOf(t); }
        catch (Exception) { return -1; }
    }

    private static string Norm(string name)
    {
        string s = name.StartsWith("m_", StringComparison.Ordinal) ? name.Substring(2) : name;
        s = Regex.Replace(s, "^[a-z]{1,4}(?=[A-Z0-9])", "");
        return s.ToLowerInvariant();
    }

    private static List<Expected> LoadExpected(string path)
    {
        var list = new List<Expected>();
        foreach (string line in File.ReadAllLines(path))
        {
            if (line.Length == 0 || line[0] == '#') continue;
            string[] p = line.Split('|');
            var e = new Expected { Name = p[0], Id = int.Parse(p[1]), Size = long.Parse(p[2]) };
            foreach (string f in p[3].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int eq = f.IndexOf('=');
                e.Fields.Add(new KeyValuePair<string, long>(f.Substring(0, eq), long.Parse(f.Substring(eq + 1))));
            }
            list.Add(e);
        }
        return list;
    }

    // aliases.txt: "ManagedName=HeaderName"
    private static Dictionary<string, string> LoadAliases(string path)
    {
        var d = new Dictionary<string, string>();
        if (!File.Exists(path)) return d;
        foreach (string raw in File.ReadAllLines(path))
        {
            string line = raw.Split('#')[0].Trim();
            int eq = line.IndexOf('=');
            if (eq > 0) d[line.Substring(0, eq).Trim()] = line.Substring(eq + 1).Trim();
        }
        return d;
    }

    // waivers.txt: "<KIND> <Name>: <reason>" for findings that are deliberately left
    private static Dictionary<string, string> LoadWaivers(string path)
    {
        var d = new Dictionary<string, string>();
        if (!File.Exists(path)) return d;
        foreach (string raw in File.ReadAllLines(path))
        {
            string line = raw.Trim();
            if (line.Length == 0 || line[0] == '#') continue;
            int colon = line.IndexOf(':');
            if (colon < 0) continue;
            d[line.Substring(0, colon).Trim()] = line.Substring(colon + 1).Trim();
        }
        return d;
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

internal static class Program
{
    private const BindingFlags Static = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

    private static int Main(string[] args)
    {
        if (args.Length < 3 || (args[0] != "generate" && args[0] != "verify" && args[0] != "verify-headers" && args[0] != "generate-new"))
        {
            Console.Error.WriteLine("usage: InterfaceLayoutCheck generate|verify <steam_api.dll> <goldenDir> [versionPrefix]");
            Console.Error.WriteLine("       InterfaceLayoutCheck verify-headers <steam_api.dll> <header-spec-dir> [versionPrefix]");
            return 2;
        }

        string mode = args[0];
        string dll = Path.GetFullPath(args[1]);
        string goldenDir = Path.GetFullPath(args[2]);
        string prefix = args.Length > 3 ? args[3] : "";

        Assembly asm = Assembly.LoadFrom(dll);
        Type interfaceManager = asm.GetType("SKYNET.Managers.InterfaceManager", true);
        interfaceManager.GetMethod("Initialize", Static).Invoke(null, null);

        MethodInfo resolveRuntime = interfaceManager.GetMethod("GetInterfaceMethods", Static);
        var typeMap = (System.Collections.IDictionary)interfaceManager.GetField("interfaceTypes", Static).GetValue(null);

        var versions = new List<string>();
        foreach (System.Collections.DictionaryEntry e in typeMap) versions.Add((string)e.Key);
        versions = versions.Where(v => v.StartsWith(prefix, StringComparison.Ordinal)).OrderBy(v => v, StringComparer.Ordinal).ToList();

        var actual = new Dictionary<string, string>();
        foreach (string version in versions)
        {
            actual[version] = Dump(version, resolveRuntime);
        }

        if (mode == "verify-headers") return VerifyHeaders(actual, goldenDir, prefix);

        if (mode == "generate-new")
        {
            int added = 0;
            foreach (var kv in actual)
            {
                string file = Path.Combine(goldenDir, kv.Key + ".txt");
                if (File.Exists(file)) continue;
                var text = string.Join("\r\n", Lines(kv.Value).Select(Normalize)) + "\r\n";
                File.WriteAllText(file, text);
                Console.WriteLine("ADDED    " + kv.Key);
                added++;
            }
            Console.WriteLine("generated " + added + " new golden files (run verify-headers first: goldens snapshot the current layout)");
            return 0;
        }

        if (mode == "generate")
        {
            Directory.CreateDirectory(goldenDir);
            foreach (var kv in actual) File.WriteAllText(Path.Combine(goldenDir, kv.Key + ".txt"), kv.Value);
            Console.WriteLine("generated " + actual.Count + " golden files in " + goldenDir);
            return 0;
        }

        int failures = 0;
        var goldenFiles = Directory.GetFiles(goldenDir, "*.txt")
            .Where(f => Path.GetFileNameWithoutExtension(f).StartsWith(prefix, StringComparison.Ordinal)).ToList();
        foreach (string file in goldenFiles)
        {
            string version = Path.GetFileNameWithoutExtension(file);
            if (!actual.ContainsKey(version))
            {
                Console.WriteLine("MISSING  " + version + " (golden exists, version no longer registered)");
                failures++;
            }
        }

        foreach (var kv in actual)
        {
            string file = Path.Combine(goldenDir, kv.Key + ".txt");
            if (!File.Exists(file))
            {
                Console.WriteLine("NEW      " + kv.Key + " (registered, no golden)");
                failures++;
                continue;
            }

            string[] expected = Lines(File.ReadAllText(file));
            string[] got = Lines(kv.Value);
            string diff = Compare(expected, got);
            if (diff == null) continue;
            Console.WriteLine("DIFF     " + kv.Key + ": " + diff);
            failures++;
        }

        Console.WriteLine(failures == 0
            ? "OK: " + actual.Count + " interface versions match golden"
            : "FAIL: " + failures + " mismatches");
        return failures == 0 ? 0 : 1;
    }

    private static string Dump(string version, MethodInfo resolveRuntime)
    {
        var sb = new System.Text.StringBuilder();
        try
        {
            var methods = (List<MethodInfo>)resolveRuntime.Invoke(null, new object[] { version });
            for (int i = 0; i < methods.Count; i++)
            {
                var m = methods[i];
                sb.Append(i).Append('|').Append(m.Name).Append('|').Append(TypeName(m.ReturnType)).Append('|')
                  .Append(string.Join(",", m.GetParameters().Select(p => TypeName(p.ParameterType)))).Append('\n');
            }
        }
        catch (Exception ex)
        {
            var inner = ex is TargetInvocationException && ex.InnerException != null ? ex.InnerException : ex;
            sb.Append("ERROR|").Append(inner.GetType().Name).Append('|').Append(inner.Message).Append('\n');
        }
        return sb.ToString();
    }

    // Compares the runtime slot order (names only) against specs generated from the real Valve headers.
    // Spec line: index|MethodName|paramCount|returnType|paramTypes ; '#' lines are comments, including
    // "# overload <cppName> slots a,b,c" (C++ overload group; the emulator may name its members freely).
    private static int VerifyHeaders(Dictionary<string, string> actual, string specDir, string prefix)
    {
        var runtime = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in actual) runtime[kv.Key] = kv.Value;

        int failures = 0, matched = 0, variantNotes = 0;
        var missing = new List<string>();
        foreach (string file in Directory.GetFiles(specDir, "*.txt").OrderBy(f => f, StringComparer.Ordinal))
        {
            string version = Path.GetFileNameWithoutExtension(file);
            if (!version.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) continue;
            string got;
            if (!runtime.TryGetValue(version, out got))
            {
                missing.Add(version);
                continue;
            }

            var specNames = new List<string>();
            var overloadCpp = new Dictionary<int, string>();
            foreach (string line in File.ReadAllLines(file))
            {
                if (line.Length == 0) continue;
                if (line[0] == '#')
                {
                    string[] t = line.Substring(1).Trim().Split(' ');
                    if (t.Length == 4 && t[0] == "overload" && t[2] == "slots")
                        foreach (string slot in t[3].Split(',')) overloadCpp[int.Parse(slot)] = t[1];
                    continue;
                }
                string[] p = line.Split('|');
                specNames.Add(p[1].StartsWith("~") ? "Destructor" : p[1]);
            }

            string[] rt = Lines(got).Select(l => Normalize(l).Split('|')[1]).ToArray();
            string diff = null;
            if (rt.Length > 0 && rt[0] == "ERROR") diff = "runtime layout error: " + Lines(got)[0];
            var variants = new List<string>();
            for (int i = 0; diff == null && i < Math.Min(specNames.Count, rt.Length); i++)
            {
                if (specNames[i] == rt[i]) continue;
                string r = Canon(rt[i]);
                string cpp;
                if (r.StartsWith(Canon(specNames[i]), StringComparison.Ordinal) ||
                    (overloadCpp.TryGetValue(i, out cpp) && r.StartsWith(Canon(cpp), StringComparison.Ordinal)))
                {
                    variants.Add(i + ":" + rt[i] + "~" + specNames[i]);
                    continue;
                }
                diff = "slot " + i + " header [" + specNames[i] + "] runtime [" + rt[i] + "]";
            }

            if (diff == null && specNames.Count != rt.Length)
                diff = "slot count header " + specNames.Count + " != runtime " + rt.Length +
                       (specNames.Count > rt.Length ? " (runtime lacks [" + specNames[rt.Length] + "])"
                                                    : " (runtime extra [" + rt[specNames.Count] + "])");
            if (diff != null)
            {
                Console.WriteLine("HDRDIFF  " + version + ": " + diff);
                failures++;
                continue;
            }

            matched++;
            if (variants.Count > 0)
            {
                variantNotes++;
                Console.WriteLine("NOTE     " + version + ": " + variants.Count + " emulator-named overload/variant slots (" + string.Join(", ", variants.Take(4)) + (variants.Count > 4 ? ", ..." : "") + ")");
            }
        }

        foreach (string v in missing) Console.WriteLine("MISSING  " + v + " (header spec exists, not registered at runtime)");
        Console.WriteLine("header-spec versions NOT registered at runtime: " + missing.Count);
        Console.WriteLine(failures == 0
            ? "OK: " + matched + " registered interface versions match header spec (" + variantNotes + " with emulator-named variant slots)"
            : "FAIL: " + failures + " registered versions differ from header spec (" + matched + " match)");
        return failures == 0 ? 0 : 1;
    }

    // Lower-cases and drops underscores so "GetQueryUGCResult_old" compares as an extension of "GetQueryUGCResult".
    private static string Canon(string name) => name.Replace("_", "").ToLowerInvariant();

    private static string TypeName(Type t) => t.FullName ?? t.Name;

    private static string[] Lines(string s) =>
        s.Replace("\r\n", "\n").Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

    // Variant methods in the unified class carry a "__Suffix" that the golden (original class) does not.
    private static string Normalize(string line)
    {
        string[] p = line.Split(new[] { '|' }, 4);
        if (p.Length < 4) return line;
        int cut = p[1].IndexOf("__", StringComparison.Ordinal);
        if (cut >= 0) p[1] = p[1].Substring(0, cut);
        return string.Join("|", p);
    }

    private static string Compare(string[] expected, string[] got)
    {
        if (expected.Length != got.Length) return "slot count " + expected.Length + " != " + got.Length;
        for (int i = 0; i < expected.Length; i++)
        {
            if (expected[i] != Normalize(got[i])) return "slot " + i + " expected [" + expected[i] + "] got [" + got[i] + "]";
        }
        return null;
    }
}

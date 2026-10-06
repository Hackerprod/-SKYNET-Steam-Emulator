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
        if (args.Length < 3 || (args[0] != "generate" && args[0] != "verify"))
        {
            Console.Error.WriteLine("usage: InterfaceLayoutCheck generate|verify <steam_api.dll> <goldenDir> [versionPrefix]");
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

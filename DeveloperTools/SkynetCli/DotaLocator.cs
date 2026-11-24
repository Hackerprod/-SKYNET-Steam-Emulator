using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace SKYNET.Cli;

internal static class DotaLocator
{
    public static string? SteamPath()
    {
        foreach (var hive in new[] { Registry.CurrentUser, Registry.LocalMachine })
        foreach (var key in new[] { @"Software\Valve\Steam", @"Software\WOW6432Node\Valve\Steam" })
        {
            using (var k = hive.OpenSubKey(key))
            {
                var v = k?.GetValue("SteamPath") as string ?? k?.GetValue("InstallPath") as string;
                if (!string.IsNullOrEmpty(v) && Directory.Exists(v)) return Path.GetFullPath(v);
            }
        }
        return null;
    }

    /// <summary>Directory containing dota2.exe (game\bin\win64), found via Steam's library list.</summary>
    public static string? Win64Dir()
    {
        var steam = SteamPath();
        if (steam == null) return null;
        var libs = new List<string> { steam };
        var vdf = Path.Combine(steam, "steamapps", "libraryfolders.vdf");
        if (File.Exists(vdf))
            foreach (Match m in Regex.Matches(File.ReadAllText(vdf), "\"path\"\\s+\"([^\"]+)\""))
                libs.Add(m.Groups[1].Value.Replace(@"\\", @"\"));
        foreach (var lib in libs)
        {
            var dir = Path.Combine(lib, "steamapps", "common", "dota 2 beta", "game", "bin", "win64");
            if (File.Exists(Path.Combine(dir, "dota2.exe"))) return dir;
        }
        return null;
    }
}

using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace SKYNET_server.Services;

/// <summary>
/// Finds where Dota 2 is installed from what Steam itself records: the Steam path in the registry and the
/// library folders listed in <c>steamapps\libraryfolders.vdf</c>. Used once, the first time no Dota folder
/// has been saved; the result is then stored in the single Dota path setting and not detected again.
/// </summary>
public static class DotaInstallDetector
{
    private const string DotaAppId = "570";
    private const string DefaultInstallDir = "dota 2 beta";

    public static string? Detect()
    {
        // Steam writes an uninstall entry for every installed game with its folder; it is the most direct source.
        foreach (var registered in FindRegisteredInstallLocations())
        {
            if (HasGameExecutable(registered))
            {
                return Path.GetFullPath(registered);
            }
        }

        foreach (var steamRoot in FindSteamRoots())
        {
            foreach (var library in FindLibraries(steamRoot))
            {
                var installDir = ReadInstallDir(library) ?? DefaultInstallDir;
                var candidate = Path.Combine(library, "steamapps", "common", installDir);
                if (HasGameExecutable(candidate))
                {
                    return Path.GetFullPath(candidate);
                }
            }
        }

        return null;
    }

    private static bool HasGameExecutable(string folder) =>
        !string.IsNullOrWhiteSpace(folder) && File.Exists(Path.Combine(folder, "game", "bin", "win64", "dota2.exe"));

    private static IEnumerable<string> FindRegisteredInstallLocations()
    {
        if (!OperatingSystem.IsWindows())
        {
            yield break;
        }

        const string uninstall = @"Microsoft\Windows\CurrentVersion\Uninstall\Steam App " + DotaAppId;
        yield return ReadRegistry(Registry.LocalMachine, @"SOFTWARE\" + uninstall, "InstallLocation");
        yield return ReadRegistry(Registry.LocalMachine, @"SOFTWARE\WOW6432Node\" + uninstall, "InstallLocation");
        yield return ReadRegistry(Registry.CurrentUser, @"Software\" + uninstall, "InstallLocation");
    }

    private static IEnumerable<string> FindSteamRoots()
    {
        var roots = new List<string>();
        if (OperatingSystem.IsWindows())
        {
            roots.Add(ReadRegistry(Registry.CurrentUser, @"Software\Valve\Steam", "SteamPath"));
            roots.Add(ReadRegistry(Registry.LocalMachine, @"SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath"));
            roots.Add(ReadRegistry(Registry.LocalMachine, @"SOFTWARE\Valve\Steam", "InstallPath"));
        }

        roots.Add(Environment.GetEnvironmentVariable("SteamPath") ?? string.Empty);
        return roots
            .Where(path => !string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
            .Select(path => Path.GetFullPath(path.Replace('/', Path.DirectorySeparatorChar)))
            .Distinct(StringComparer.OrdinalIgnoreCase);
    }

    private static string ReadRegistry(RegistryKey hive, string subKey, string name)
    {
        try
        {
            using var key = hive.OpenSubKey(subKey);
            return key?.GetValue(name) as string ?? string.Empty;
        }
        catch (Exception ex) when (ex is System.Security.SecurityException or UnauthorizedAccessException or IOException)
        {
            return string.Empty;
        }
    }

    // Libraries are the Steam folder itself plus every "path" in libraryfolders.vdf.
    private static IEnumerable<string> FindLibraries(string steamRoot)
    {
        yield return steamRoot;
        string text;
        try
        {
            var file = Path.Combine(steamRoot, "steamapps", "libraryfolders.vdf");
            if (!File.Exists(file))
            {
                yield break;
            }

            text = File.ReadAllText(file);
        }
        catch (IOException)
        {
            yield break;
        }

        foreach (Match match in Regex.Matches(text, @"""path""\s+""(?<p>[^""]+)"""))
        {
            var path = match.Groups["p"].Value.Replace("\\\\", "\\");
            if (Directory.Exists(path))
            {
                yield return path;
            }
        }
    }

    private static string? ReadInstallDir(string library)
    {
        try
        {
            var manifest = Path.Combine(library, "steamapps", $"appmanifest_{DotaAppId}.acf");
            if (!File.Exists(manifest))
            {
                return null;
            }

            var match = Regex.Match(File.ReadAllText(manifest), @"""installdir""\s+""(?<d>[^""]+)""");
            return match.Success ? match.Groups["d"].Value : null;
        }
        catch (IOException)
        {
            return null;
        }
    }
}

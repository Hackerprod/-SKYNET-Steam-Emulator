using Microsoft.Win32;

namespace SKYNET.Client.Services;

/// <summary>
/// Gives a launched game the environment Steam itself would provide.
///
/// Source 2 games (Dota 2) run a "trusted launch" self-check at startup: they load
/// the game's own signature manifest plus <c>&lt;SteamPath&gt;\steam.signatures</c> and only
/// report the process as VAC-capable ("BSecureAllowed") when every list verifies. The
/// Steam install directory comes from the <c>SteamPath</c> environment variable (or
/// <c>-steampath</c>) that Steam sets when it starts a game. Without it the Steam list
/// stays empty, the check never reaches its final stage and the client refuses to join
/// a server with "Valve Anti-Cheat was unable to verify that your machine is secure".
/// </summary>
internal static class SteamEnvironment
{
    private const string SteamPathVariable = "SteamPath";

    // Set to 1 to behave as if Steam were not installed (used to exercise the standalone fallback).
    private const string IgnoreInstalledSteamVariable = "SKYNET_IGNORE_INSTALLED_STEAM";

    // A Steam-less machine only needs the manifest to exist and to list at least one entry; the
    // listed files are never required to be present. The entry is a placeholder, not a real file.
    private const string StandaloneManifestName = "steam.signatures";
    private const string StandaloneManifestContent =
        "...\\skynet_standalone.dll~SHA1:0000000000000000000000000000000000000000;CRC:00000000\r\n";

    /// <summary>The Steam install directory, from the registry; null if Steam is not installed.</summary>
    public static string? ResolveSteamPath()
    {
        if (Environment.GetEnvironmentVariable(IgnoreInstalledSteamVariable) == "1")
            return null;

        var fromEnvironment = Environment.GetEnvironmentVariable(SteamPathVariable);
        if (!string.IsNullOrWhiteSpace(fromEnvironment) && Directory.Exists(fromEnvironment))
            return fromEnvironment;

        var candidates = new[]
        {
            ReadRegistry(RegistryHive.CurrentUser, RegistryView.Registry32, @"Software\Valve\Steam", "SteamPath"),
            ReadRegistry(RegistryHive.CurrentUser, RegistryView.Registry64, @"Software\Valve\Steam", "SteamPath"),
            ReadRegistry(RegistryHive.LocalMachine, RegistryView.Registry32, @"SOFTWARE\Valve\Steam", "InstallPath"),
            ReadRegistry(RegistryHive.LocalMachine, RegistryView.Registry64, @"SOFTWARE\Valve\Steam", "InstallPath"),
        };

        foreach (var candidate in candidates)
        {
            if (!string.IsNullOrWhiteSpace(candidate) && Directory.Exists(candidate))
                return candidate!;
        }

        return null;
    }

    /// <summary>
    /// Defines <c>SteamPath</c> for processes created while the scope is alive (they inherit the
    /// launcher's environment) and restores the previous value on dispose.
    /// </summary>
    public static IDisposable Apply(Action<string>? log = null)
    {
        var steamPath = ResolveSteamPath();
        if (steamPath == null)
        {
            steamPath = EnsureStandaloneSteamDirectory();
            if (steamPath == null)
            {
                log?.Invoke("No Steam install and the standalone Steam folder could not be created; SteamPath is not set.");
                return NoopScope.Instance;
            }

            log?.Invoke($"Steam not installed; using the standalone SteamPath: {steamPath}");
        }
        else
        {
            log?.Invoke($"SteamPath for the game: {steamPath}");
        }

        var previous = Environment.GetEnvironmentVariable(SteamPathVariable);
        Environment.SetEnvironmentVariable(SteamPathVariable, steamPath);
        return new RestoreScope(previous);
    }

    /// <summary>
    /// Creates (if needed) a launcher-owned folder holding a minimal steam.signatures so the game's
    /// trusted-launch check can complete on machines without a Steam client.
    /// </summary>
    private static string? EnsureStandaloneSteamDirectory()
    {
        try
        {
            var directory = Path.Combine(ConfigStore.RootDir, "steam");
            Directory.CreateDirectory(directory);
            var manifest = Path.Combine(directory, StandaloneManifestName);
            if (!File.Exists(manifest) || File.ReadAllText(manifest) != StandaloneManifestContent)
                File.WriteAllText(manifest, StandaloneManifestContent);

            return directory;
        }
        catch
        {
            return null;
        }
    }

    private static string? ReadRegistry(RegistryHive hive, RegistryView view, string subKey, string name)
    {
        try
        {
            using var baseKey = RegistryKey.OpenBaseKey(hive, view);
            using var key = baseKey.OpenSubKey(subKey);
            return key?.GetValue(name) as string;
        }
        catch
        {
            return null;
        }
    }

    private sealed class RestoreScope : IDisposable
    {
        private readonly string? _previous;

        public RestoreScope(string? previous) => _previous = previous;

        public void Dispose() => Environment.SetEnvironmentVariable(SteamPathVariable, _previous);
    }

    private sealed class NoopScope : IDisposable
    {
        public static readonly NoopScope Instance = new();

        public void Dispose()
        {
        }
    }
}

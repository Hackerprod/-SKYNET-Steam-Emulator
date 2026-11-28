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
///
/// ===========================================================================
/// VAC POPUP POST-MORTEM  (keep this: the symptom is easy to misdiagnose)
/// ===========================================================================
///
/// SYMPTOM
///   Creating a Dota lobby on a regional server (e.g. US West) and pressing START GAME
///   showed "Valve Anti-Cheat (VAC) - Valve Anti-Cheat was unable to verify that your
///   machine is secure". The dedicated server started and registered with the GC, but the
///   client never opened a connection to it, so the dedicated sat in
///   "Player 0 has connection state 1" for 180 s and then quit (PlayerFailedToConnect).
///   The same lobby with "Local Host" (lan=true) worked, because the client then uses its
///   own listen server and never takes the code path that shows the popup.
///
/// WHAT IT WAS NOT (each of these was investigated and ruled out)
///   - Not the GC: the popup text (#DOTA_VAC_Warning_Header_Solo) is built locally by
///     client.dll; the GC never sends CMsgDOTAPopup / VAC_NOT_VERIFIED / GCToClientVACReminder.
///   - Not -insecure: the engine flags and the check below are identical with and without it
///     (the launcher used to force -insecure, which only hid nothing). Do NOT add it back.
///   - Not a steam_api call: nothing in the emulated Steam API is consulted. dota2.exe reads
///     the SteamPath environment variable (or -steampath) directly at startup.
///   - Not the network: with a direct "+connect ip:port" the handshake, auth and match work;
///     the SDR/cert problem described in SteamHTTP.BuildSdrConfigJson was a separate bug.
///
/// ROOT CAUSE
///   client.dll calls the pre-join check client+0x22E82A0 from FINDMATCH_JOIN
///   (client+0x232F560, call at +0x232F611) when a lobby with a remote server goes to
///   SERVERSETUP. That check shows the VAC popup and aborts the join when
///   BSecureAllowed() is false. client.dll resolves BSecureAllowed (plus
///   GetTotalFilesLoaded / CountFilesNeedTrustCheck / CountFilesCompletedTrustCheck /
///   CountItemsToReport) with GetProcAddress from the main module: they are exports of
///   dota2.exe itself. In dota2.exe, BSecureAllowed returns true only if the global trust
///   object (dota2.exe+0x3D000) holds   [+0x24]==1, [+0x28]==1, [+0x2C] in {1,2},
///   [+0x30] in {1,2}, [+0x44]==9.   [+0x44] is a stage counter written by the
///   initialiser at dota2.exe+0x5050/+0x5644 (1..9; 9 = it hooked NtOpenFile,
///   LoadLibraryExW and PeekMessageW and installed its exception handler).
///   [+0x24..+0x30] are the verification status of four file lists:
///     obj+0xC0  the game's own manifest (dota.signatures, 98 files)      -> was OK
///     obj+0xC8  &lt;SteamPath&gt;\steam.signatures (Steam client files)       -> WAS EMPTY (status 2)
///     obj+0xD0  system files (system.signatures, %WINDIR%)             -> OK
///     obj+0xD8  foreign.signatures                                       -> OK
///   Our launcher did not define SteamPath, so list obj+0xC8 stayed empty, [+0x28] was 2
///   instead of 1 and the stage counter stopped at 1. Result: BSecureAllowed()==false for
///   every launch, the popup, and no connection.
///
/// FIX (this class)
///   Define SteamPath for the launched process only (it inherits the launcher environment):
///   the real Steam directory from the registry, or - on a machine without a Steam client -
///   a launcher-owned folder holding a minimal steam.signatures (see EnsureStandaloneSteamDirectory).
///   Verified results of the trust object after the fix: +0x28==1, +0x44==9, 37 entries in
///   list B with real Steam, 1 entry with the standalone folder.
///
/// WHAT THE CHECK ACCEPTS (measured, not assumed)
///   SteamPath -> real Steam dir ................ passes
///   SteamPath -> dir with only steam.signatures  passes (the listed DLLs need not exist)
///   SteamPath -> dir with a 1-line manifest ..... passes
///   SteamPath -> dir with a 0-byte manifest ..... FAILS (list stays empty)
///   SteamPath -> empty dir / unset ............... FAILS
///
/// HOW TO RE-VERIFY IF THE POPUP EVER COMES BACK
///   1. Launch the client through the launcher (never the exe directly) and read, in
///      dota2.exe, the dwords at module base + 0x3D000 + {0x24,0x28,0x2C,0x30,0x44}.
///      Healthy values: 1, 1, 1|2, 1|2, 9. If +0x44 is not 9 the initialiser stopped
///      early: 1 = the environment/list check failed (this bug), 2..8 = hooking a system
///      API failed (look for another injector/overlay hooking NtOpenFile, LoadLibraryExW
///      or PeekMessageW, or a missing ntdll/kernelbase/user32 export).
///   2. If +0x28 != 1, check that SteamPath is set in the game process environment and
///      that the folder it names contains a non-empty steam.signatures.
///   3. A game update can change this: re-check that BSecureAllowed is still exported by
///      dota2.exe and that client.dll still resolves it. Offsets above are for build 6989
///      (dota\steam.inf ClientVersion/ServerVersion 6989); the logic, not the addresses,
///      is what to look for.
///
/// RELATED
///   SKYNET_IGNORE_INSTALLED_STEAM=1 forces the standalone folder (used to test a Steam-less
///   machine). The dedicated server started by DotaDedicatedServerSupervisor does not need
///   SteamPath: the popup only exists on the client join path.
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
        if (HasUsableSteamSignatures(fromEnvironment))
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
            if (HasUsableSteamSignatures(candidate))
                return candidate!;
        }

        return null;
    }

    /// <summary>
    /// A directory only counts as a Steam install when it holds a non-empty steam.signatures: that file is
    /// what the game's trusted-launch check reads (see the post-mortem above), so a folder that merely exists
    /// is not enough. A partial Steam folder (bin, logs, a few DLLs, no manifest) used to be taken for the real
    /// one, the standalone fallback never kicked in and every join to a remote server showed the VAC popup.
    /// </summary>
    private static bool HasUsableSteamSignatures(string? directory)
    {
        if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
            return false;

        try
        {
            var manifest = new FileInfo(Path.Combine(directory, StandaloneManifestName));
            return manifest.Exists && manifest.Length > 0;
        }
        catch
        {
            return false;
        }
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

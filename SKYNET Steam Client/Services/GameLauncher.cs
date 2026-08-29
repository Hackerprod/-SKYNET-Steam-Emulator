using System.Diagnostics;
using System.Security.Cryptography;
using SKYNET.Client.Models;

namespace SKYNET.Client.Services;

public sealed class LaunchResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public Process? Process { get; set; }
    public bool UsedStaticImportRedirection { get; set; }

    public static LaunchResult Fail(string error) => new() { Success = false, Error = error };
    public static LaunchResult Ok(Process p, bool usedStaticImportRedirection) => new()
    {
        Success = true,
        Process = p,
        UsedStaticImportRedirection = usedStaticImportRedirection
    };
}

/// <summary>
/// Launches a game with the SKYNET emulator injected into the process at start,
/// with nothing written into the game folder. The game exe is created suspended,
/// the emulator DLL shipped in the launcher's payload folder is copied to an
/// isolated per-build shadow path and injected via
/// CreateRemoteThread(LoadLibraryW), then the process is resumed. Because the game
/// loads steam_api64.dll dynamically by bare name, the loader returns our
/// already-loaded module for its later LoadLibrary("steam_api64.dll"), so the game
/// uses our emulator without the original file ever being touched. See DllInjector.
/// The shadow path keeps Windows' loader lock away from the launcher's payload,
/// so rebuilding the client can refresh its bundled DLL while a launched game is
/// still running.
///
/// RecoverOrphans still runs on startup to clean up any DLL swap left by an older
/// version of this launcher.
/// </summary>
public sealed class GameLauncher
{
    private const string BackupSuffix = ".skynet-orig";
    private const string MarkerSuffix = ".skynet-injected";
    private const int PreviousPayloadShadowsToKeep = 3;

    private static string PayloadDll(GameArch arch)
    {
        var rel = arch == GameArch.X64
            ? Path.Combine("payload", "x64", "steam_api64.dll")
            : Path.Combine("payload", "x86", "steam_api.dll");
        return Path.Combine(AppContext.BaseDirectory, rel);
    }

    public event Action<GameEntry>? GameExited;

    public LaunchResult Launch(GameEntry game, AppConfig app, WebUser? user, string? extraArgs = null)
    {
        if (!game.ExeExists)
            return LaunchResult.Fail($"Executable not found:\n{game.ExecutablePath}");

        // The PE header is authoritative. A stale/manual architecture selection
        // must never make us inject an x86 DLL into an x64 executable (or vice versa).
        var detectedArch = PeArch.Detect(game.ExecutablePath);
        var arch = detectedArch != GameArch.Unknown ? detectedArch : game.Arch;
        if (arch == GameArch.Unknown)
            return LaunchResult.Fail("Could not determine game architecture (x86/x64).");

        var payload = PayloadDll(arch);
        if (!File.Exists(payload))
            return LaunchResult.Fail($"Emulator payload missing:\n{payload}");

        // The emulator resolves steam_api.ini / logs from the game process's own
        // exe folder (Common.GetPath uses MainModule), so the payload DLL can stay
        // in the launcher's payload directory while per-game config lives with the game.
        try
        {
            IniWriter.Write(game, app, user);
        }
        catch (Exception ex)
        {
            return LaunchResult.Fail($"Failed to write steam_api.ini:\n{ex.Message}");
        }

        var steamImportName = Path.GetFileName(payload);
        var hasStaticSteamImport = PeImports.ImportsModule(game.ExecutablePath, steamImportName);
        Process proc;
        try
        {
            var workDir = string.IsNullOrWhiteSpace(game.ExeFolder) ? Path.GetDirectoryName(game.ExecutablePath)! : game.ExeFolder;
            var insecureArg = !game.Ini.SecureNetworking &&
                !ContainsArgument(game.LaunchArguments, "-insecure") &&
                !ContainsArgument(extraArgs, "-insecure")
                    ? "-insecure"
                    : null;
            var args = string.Join(" ",
                new[] { game.LaunchArguments, insecureArg, extraArgs }
                    .Where(a => !string.IsNullOrWhiteSpace(a)));

            var injectablePayload = PrepareInjectablePayload(payload);
            proc = DllInjector.LaunchAndInject(
                game.ExecutablePath,
                injectablePayload,
                args,
                workDir,
                hasStaticSteamImport ? steamImportName : null);
            proc.EnableRaisingEvents = true;
            proc.Exited += (_, _) => GameExited?.Invoke(game);
            GameWindowActivator.BringToFrontWhenReady(proc);
        }
        catch (Exception ex)
        {
            return LaunchResult.Fail($"Failed to inject emulator into the game:\n{ex.Message}");
        }

        game.LastPlayedUtc = DateTimeOffset.UtcNow;
        return LaunchResult.Ok(proc, hasStaticSteamImport);
    }

    private static bool ContainsArgument(string? arguments, string expected)
    {
        if (string.IsNullOrWhiteSpace(arguments))
            return false;

        return arguments!
            .Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Any(argument => string.Equals(argument, expected, StringComparison.OrdinalIgnoreCase));
    }

    private static string PrepareInjectablePayload(string payload)
    {
        var payloadBytes = File.ReadAllBytes(payload);
        var hash = ComputePayloadHash(payloadBytes);
        var shadowRoot = Path.Combine(Path.GetTempPath(), "SKYNETSteamClient", "payload-shadow");
        var shadowDir = Path.Combine(shadowRoot, hash);
        var payloadFileName = Path.GetFileName(payload);
        var shadowPath = Path.Combine(shadowDir, payloadFileName);

        Directory.CreateDirectory(shadowDir);
        bool shadowMatches = false;
        try
        {
            shadowMatches = File.Exists(shadowPath) &&
                File.ReadAllBytes(shadowPath).SequenceEqual(payloadBytes);
        }
        catch
        {
            // A missing, unreadable, or partially replaced shadow must be rebuilt
            // from the payload shipped next to this launcher.
        }

        if (!shadowMatches)
        {
            File.WriteAllBytes(shadowPath, payloadBytes);
        }

        try
        {
            Directory.SetLastWriteTimeUtc(shadowDir, DateTime.UtcNow);
        }
        catch
        {
        }

        CleanupPayloadShadows(shadowRoot, hash, payloadFileName);
        return shadowPath;
    }

    private static string ComputePayloadHash(byte[] payloadBytes)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(payloadBytes);
        return BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLowerInvariant();
    }

    private static void CleanupPayloadShadows(string shadowRoot, string activeHash, string payloadFileName)
    {
        try
        {
            if (!Directory.Exists(shadowRoot))
            {
                return;
            }

            var shadows = Directory.GetDirectories(shadowRoot)
                .Where(directory => File.Exists(Path.Combine(directory, payloadFileName)))
                .OrderByDescending(directory =>
                    string.Equals(Path.GetFileName(directory), activeHash, StringComparison.OrdinalIgnoreCase)
                        ? DateTime.MaxValue
                        : Directory.GetLastWriteTimeUtc(directory))
                .ToArray();

            foreach (var directory in shadows.Skip(PreviousPayloadShadowsToKeep + 1))
            {
                TryDeleteShadow(directory);
            }
        }
        catch
        {
        }
    }

    private static void TryDeleteShadow(string directory)
    {
        try
        {
            Directory.Delete(directory, recursive: true);
        }
        catch
        {
        }
    }

    /// <summary>Restores the original DLL and removes our footprint. Safe to call twice.</summary>
    private static void TryRestore(string targetDll)
    {
        try
        {
            var backup = targetDll + BackupSuffix;
            var marker = targetDll + MarkerSuffix;
            if (!File.Exists(marker) && !File.Exists(backup)) return;

            if (File.Exists(targetDll)) File.Delete(targetDll);
            var cfg = targetDll + ".config";
            if (File.Exists(cfg)) File.Delete(cfg);

            if (File.Exists(backup)) File.Move(backup, targetDll);
            if (File.Exists(marker)) File.Delete(marker);
        }
        catch { /* best-effort; recovered on next start */ }
    }

    /// <summary>
    /// Restores any DLLs left injected by a previous run that crashed before exit.
    /// Call on startup (no game of ours is running then, so files are unlocked).
    /// </summary>
    public void RecoverOrphans(IEnumerable<GameEntry> games)
    {
        foreach (var game in games)
        {
            if (string.IsNullOrWhiteSpace(game.ExeFolder)) continue;
            foreach (var name in new[] { "steam_api64.dll", "steam_api.dll" })
            {
                var target = Path.Combine(game.ExeFolder, name);
                if (File.Exists(target + MarkerSuffix) || File.Exists(target + BackupSuffix))
                    TryRestore(target);
            }
        }
    }
}

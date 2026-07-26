using System.Windows;
using SKYNET.Client.Models;
using SKYNET.Client.Services;

namespace SKYNET.Client;

public partial class App : Application
{
    public static ConfigStore Store { get; } = new();
    public static ServerClient Server { get; } = new();
    public static GameLauncher Launcher { get; } = new();

    public App()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, ev) =>
            Crash("AppDomain", ev.ExceptionObject as Exception);
        DispatcherUnhandledException += (_, ev) =>
        {
            Crash("Dispatcher", ev.Exception);
            ev.Handled = false;
        };
    }

    private static void Crash(string src, Exception? ex)
    {
        try
        {
            Directory.CreateDirectory(ConfigStore.RootDir);
            File.AppendAllText(Path.Combine(ConfigStore.RootDir, "launcher.log"),
                $"{DateTimeOffset.Now:HH:mm:ss.fff}  CRASH[{src}]: {ex}{Environment.NewLine}");
        }
        catch { }
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Store.Load();
        Server.Configure(Store.Config.ServerUrl);
        Launcher.RecoverOrphans(Store.Config.Games);

        // Headless launch mode for automated testing:
        //   "SKYNET Steam Client.exe" --launch <gameId|exePath>
        // Launches the game (injected), waits for it to exit, restores, then quits.
        var launchRequest = ParseLaunchArgs(e.Args);
        if (launchRequest.Target != null)
        {
            RunHeadless(launchRequest.Target, launchRequest.ExtraArguments);
            return;
        }

        var win = new MainWindow();
        win.Show();
    }

    private static (string? Target, string? ExtraArguments) ParseLaunchArgs(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (string.Equals(args[i], "--launch", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                var separator = Array.FindIndex(
                    args,
                    i + 2,
                    argument => string.Equals(argument, "--", StringComparison.Ordinal));
                var extraArguments = separator >= 0
                    ? string.Join(" ", args.Skip(separator + 1).Select(QuoteProcessArgument))
                    : null;
                return (args[i + 1], extraArguments);
            }
        }
        return (null, null);
    }

    private static string QuoteProcessArgument(string value)
    {
        value ??= string.Empty;
        if (value.Length > 0 && !value.Any(character => char.IsWhiteSpace(character) || character == '"'))
        {
            return value;
        }

        var result = new System.Text.StringBuilder(value.Length + 2);
        result.Append('"');
        var backslashes = 0;
        foreach (var character in value)
        {
            if (character == '\\')
            {
                backslashes++;
                continue;
            }

            if (character == '"')
            {
                result.Append('\\', backslashes * 2 + 1);
                result.Append('"');
                backslashes = 0;
                continue;
            }

            result.Append('\\', backslashes);
            backslashes = 0;
            result.Append(character);
        }

        result.Append('\\', backslashes * 2);
        result.Append('"');
        return result.ToString();
    }

    private static void HeadlessLog(string msg)
    {
        try
        {
            Directory.CreateDirectory(ConfigStore.RootDir);
            File.AppendAllText(Path.Combine(ConfigStore.RootDir, "launcher.log"),
                $"{DateTimeOffset.Now:HH:mm:ss.fff}  {msg}{Environment.NewLine}");
        }
        catch { }
    }

    private async void RunHeadless(string target, string? extraArguments)
    {
        try
        {
            HeadlessLog($"--launch target: {target}");
            var game = ResolveGame(target);
            if (game == null)
            {
                HeadlessLog("game not found");
                Shutdown(2);
                return;
            }
            HeadlessLog($"resolved game: {game.Name} appId={game.AppId} arch={game.Arch} exe={game.ExecutablePath}");

            WebUser? user = null;
            try
            {
                var session = await Server.ResolveSessionAsync(Store.Config);
                user = session.User;
                HeadlessLog($"session: {session.Status} user={user?.DisplayName ?? "(none)"}");
            }
            catch (Exception ex) { HeadlessLog($"session error: {ex.Message}"); }

            var result = Launcher.Launch(game, Store.Config, user, extraArguments);
            Store.Save();
            if (!result.Success)
            {
                HeadlessLog($"launch FAILED: {result.Error}");
                Shutdown(1);
                return;
            }

            HeadlessLog($"launched pid={result.Process!.Id}; waiting for exit...");
            if (result.UsedStaticImportRedirection)
                HeadlessLog("static Steam API import redirected to the injected payload.");
            await Task.Run(() => result.Process.WaitForExit());
            HeadlessLog("game exited; original DLL restored.");
            Shutdown(0);
        }
        catch (Exception ex)
        {
            HeadlessLog($"UNHANDLED: {ex}");
            Shutdown(3);
        }
    }

    private static GameEntry? ResolveGame(string target)
    {
        var games = Store.Config.Games;
        var byId = games.FirstOrDefault(g => string.Equals(g.Id, target, StringComparison.OrdinalIgnoreCase));
        if (byId != null) return byId;

        var byPath = games.FirstOrDefault(g =>
            string.Equals(g.ExecutablePath, target, StringComparison.OrdinalIgnoreCase));
        if (byPath != null) return byPath;

        // Allow launching an arbitrary exe path not yet in the library.
        if (File.Exists(target))
            return new GameEntry
            {
                Name = Path.GetFileNameWithoutExtension(target),
                ExecutablePath = target,
                AppId = ReadSteamAppId(Path.GetDirectoryName(target)),
                Arch = PeArch.Detect(target)
            };

        return null;
    }

    private static uint ReadSteamAppId(string? directory)
    {
        if (string.IsNullOrWhiteSpace(directory))
            return 0;

        try
        {
            var path = Path.Combine(directory, "steam_appid.txt");
            return File.Exists(path) &&
                   uint.TryParse(File.ReadAllText(path).Trim(), out var appId)
                ? appId
                : 0;
        }
        catch
        {
            return 0;
        }
    }
}

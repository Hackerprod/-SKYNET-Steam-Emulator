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
        var launchTarget = ParseLaunchArg(e.Args);
        if (launchTarget != null)
        {
            RunHeadless(launchTarget);
            return;
        }

        var win = new MainWindow();
        win.Show();
    }

    private static string? ParseLaunchArg(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
            if (string.Equals(args[i], "--launch", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
                return args[i + 1];
        return null;
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

    private async void RunHeadless(string target)
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

            var result = Launcher.Launch(game, Store.Config, user);
            Store.Save();
            if (!result.Success)
            {
                HeadlessLog($"launch FAILED: {result.Error}");
                Shutdown(1);
                return;
            }

            HeadlessLog($"launched pid={result.Process!.Id}; waiting for exit...");
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
                Arch = PeArch.Detect(target)
            };

        return null;
    }
}

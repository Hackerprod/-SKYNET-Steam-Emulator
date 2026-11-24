namespace SKYNET.Cli.Capture;

/// <summary>
/// Entry point of skynet-hook.exe (the net472 build). EasyHook needs a .NET Framework process (it uses
/// Remoting) both to inject and to load the payload, so `skynet-cli capture` delegates to this exe,
/// which is also the assembly injected into dota2.exe.
/// </summary>
internal static class HookMain
{
    private static int Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "start") return CaptureCommand.Run(args.Skip(1).ToArray());
        Console.Error.WriteLine("skynet-hook.exe start [--out DIR] [--args \"...\"] [--dota DIR]   (use: skynet-cli capture start)");
        return 2;
    }
}

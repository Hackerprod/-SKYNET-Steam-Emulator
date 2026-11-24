namespace SKYNET.Gc.Capture;

/// <summary>
/// Entry point of skynet-gc-hook.exe (the net472 build). EasyHook needs a .NET Framework process (it uses
/// Remoting) both to inject and to load the payload, so `skynet-gc capture` delegates to this exe,
/// which is also the assembly injected into dota2.exe.
/// </summary>
internal static class HookMain
{
    private static int Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "start") return CaptureCommand.Run(args.Skip(1).ToArray());
        Console.Error.WriteLine("skynet-gc-hook.exe start [--out DIR] [--args \"...\"] [--dota DIR]   (use: skynet-gc capture start)");
        return 2;
    }
}

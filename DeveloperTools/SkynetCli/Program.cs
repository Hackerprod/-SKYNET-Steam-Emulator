using SKYNET.Cli.Capture;
using SKYNET.Cli.Ui;

namespace SKYNET.Cli;

internal static class Program
{
    private static int Main(string[] args)
    {
        var rest = args.Skip(1).ToArray();
        switch (args.Length > 0 ? args[0] : "")
        {
            case "capture" when rest.Length > 0 && rest[0] == "start": return CaptureCommand.Run(rest.Skip(1).ToArray());
            case "ui": return UiCommand.Run(rest);
        }
        Console.WriteLine("skynet-cli");
        Console.WriteLine("  capture start [--out DIR] [--args \"...\"] [--dota DIR]   launch Dota on real Steam with the GC hook");
        Console.WriteLine("  ui shot|click|key|type|console ...                        drive the game window (fractional coords)");
        return 1;
    }
}

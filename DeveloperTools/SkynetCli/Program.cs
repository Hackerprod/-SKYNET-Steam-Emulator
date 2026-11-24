using SKYNET.Cli.Analyze;
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
            case "capture" when rest.Length > 0 && rest[0] == "start": return CaptureProxy.Run(rest.Skip(1).ToArray());
            case "analyze": return AnalyzeCommand.Run(rest);
            case "decode": return DecodeCommand.Run(rest);
            case "ui": return UiCommand.Run(rest);
        }
        Console.WriteLine("skynet-cli");
        Console.WriteLine("  capture start [--out DIR] [--args \"...\"] [--dota DIR]   launch Dota on real Steam with the GC hook");
        Console.WriteLine("  analyze <capture-dir|gc.jsonl> [out] [--after N --window N --baseline DIR]   decode the flow");
        Console.WriteLine("  decode <gcMessageType> <body.bin> [client|server]                            decode one GC body to JSON");
        Console.WriteLine("  ui shot|click|key|type|console ...                        drive the game window (fractional coords)");
        return 1;
    }
}




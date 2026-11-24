using System.Text.Json;
using ProtoBuf;

namespace SKYNET.Gc.Analyze;

/// <summary>decode TYPE FILE [client|server]: decodes one GC message body (protobuf) to JSON.</summary>
internal static class DecodeCommand
{
    public static int Run(string[] args)
    {
        if (args.Length < 2 || !uint.TryParse(args[0], out var messageType))
        {
            Console.Error.WriteLine("Usage: skynet-gc decode <gcMessageType> <body.bin> [client|server]");
            return 2;
        }

        var direction = args.Length > 2 ? args[2] : "client";
        var root = FindRepoRoot(AppContext.BaseDirectory);
        var resolver = new GcTypeResolver(
            typeof(CMsgClientHello).Assembly,
            Path.Combine(root, "SKYNET server", "GC", "570", "contracts"));
        var type = resolver.ResolveProtoType(messageType, direction);
        if (type == null)
        {
            Console.Error.WriteLine($"No protobuf contract mapped for GC message {messageType} ({resolver.GetMessageName(messageType)}) / {direction}.");
            return 1;
        }

        using var stream = File.OpenRead(args[1]);
        var decoded = Serializer.NonGeneric.Deserialize(type, stream);
        Console.WriteLine(JsonSerializer.Serialize(ProtoJson.ToJsonObject(decoded, resolver), new JsonSerializerOptions { WriteIndented = true }));
        return 0;
    }

    private static string FindRepoRoot(string start)
    {
        for (var dir = new DirectoryInfo(start); dir != null; dir = dir.Parent)
            if (Directory.Exists(Path.Combine(dir.FullName, "SKYNET server", "GC"))) return dir.FullName;
        throw new InvalidOperationException("Could not locate the SKYNET repo root from " + start);
    }
}

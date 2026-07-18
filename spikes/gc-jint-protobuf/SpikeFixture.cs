using Jint;

namespace GcJintProtobufSpike;

/// <summary>
/// Shared Jint engine loaded exactly like JintGcEngine loads it in the server:
/// set __PROTO_DESCRIPTOR__, evaluate the committed dist/gc.js bundle
/// (protobufjs/light + long bundled in), then call the __proto_* bridge.
/// </summary>
public sealed class SpikeFixture : IDisposable
{
    public Engine Engine { get; }
    public string RepoRoot { get; }
    public string GcDir => Path.Combine(RepoRoot, "SKYNET server", "GC", "570");
    public List<string> LogLines { get; } = new();

    public SpikeFixture()
    {
        RepoRoot = FindRepoRoot();
        var descriptor = File.ReadAllText(Path.Combine(GcDir, "js", "proto-descriptor.json"));
        var bundle = File.ReadAllText(Path.Combine(GcDir, "js", "dist", "gc.js"));

        Engine = new Engine(options =>
        {
            options.LimitMemory(128 * 1024 * 1024);
            options.TimeoutInterval(TimeSpan.FromSeconds(30));
            options.LimitRecursion(512);
        });
        Engine.SetValue("log", new Action<string>(LogLines.Add));
        Engine.SetValue("__PROTO_DESCRIPTOR__", descriptor);
        Engine.Execute(bundle);
    }

    public string DecodeJson(string typeName, byte[] payload)
        => Engine.Invoke("__proto_decode_json", typeName, Convert.ToBase64String(payload)).AsString();

    public byte[] EncodeFromJson(string typeName, string objectJson)
        => Convert.FromBase64String(Engine.Invoke("__proto_encode_b64", typeName, objectJson).AsString());

    public byte[] Fixture(string relativePath)
        => File.ReadAllBytes(Path.Combine(GcDir, relativePath.Replace('/', Path.DirectorySeparatorChar)));

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, "SKYNET server", "GC", "570")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new InvalidOperationException("Repo root not found from " + AppContext.BaseDirectory);
    }

    public void Dispose() => Engine.Dispose();
}

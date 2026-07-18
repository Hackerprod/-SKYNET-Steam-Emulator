using Microsoft.Extensions.Logging.Abstractions;
using SKYNET_server.Models;
using SKYNET_server.Services;
using Xunit;

namespace SkynetServer.GcTests;

/// <summary>
/// Fase 1 gate: the JS engine loads the committed bundle, executes the noop
/// handler, hot-reloads on artifact touch, and never claims messages it does
/// not handle (so the router falls back to Lua/C#).
/// </summary>
[Collection("gc-serial")]
public class JintGcEngineTests
{
    private static JintGcEngine CreateEngine()
        => new(
            TestHost.Environment(),
            TestHost.EmptyConfiguration(),
            NullLogger<JintGcEngine>.Instance,
            new GameCoordinatorTraceService(),
            new GcClientVersionCache());

    private static GameCoordinatorContext Context(ulong steamId = 76561198000000001)
        => new() { AppId = 570, SteamId = steamId, AccountId = (uint)(steamId & 0xFFFFFFFF), PersonaName = "test" };

    [Fact]
    public void Bundle_Is_Available_For_570()
    {
        Assert.True(CreateEngine().IsAvailable(570));
        Assert.False(CreateEngine().IsAvailable(440));
    }

    [Fact]
    public void Dispatch_Noop_Returns_NotHandled_With_Reason()
    {
        var engine = CreateEngine();
        var response = engine.Exchange(
            Context(),
            new ApiGCExchangeRequest { AppId = 570, MessageType = 9999, BodyBase64 = "" },
            out var reason);

        Assert.False(response.Handled);
        Assert.Contains("noop", reason);
        Assert.Empty(response.Messages);
    }

    [Fact]
    public void HotReload_Survives_Bundle_Touch()
    {
        var engine = CreateEngine();
        var request = new ApiGCExchangeRequest { AppId = 570, MessageType = 9999, BodyBase64 = "" };
        var first = engine.Exchange(Context(), request, out _);
        Assert.False(first.Handled);

        // Touch the bundle: the engine must rebuild (atomic swap) and keep serving.
        var bundlePath = Path.Combine(TestHost.RepoRoot, "SKYNET server", "GC", "570", "js", "dist", "gc.js");
        var originalStamp = File.GetLastWriteTimeUtc(bundlePath);
        try
        {
            File.SetLastWriteTimeUtc(bundlePath, originalStamp.AddSeconds(2));
            var second = engine.Exchange(Context(), request, out var reason);
            Assert.False(second.Handled);
            Assert.Contains("noop", reason);
        }
        finally
        {
            File.SetLastWriteTimeUtc(bundlePath, originalStamp);
        }
    }

    [Fact]
    public void Concurrent_Dispatches_Are_Serialized_Safely()
    {
        var engine = CreateEngine();
        var request = new ApiGCExchangeRequest { AppId = 570, MessageType = 9999, BodyBase64 = "" };
        var errors = 0;

        Parallel.For(0, 32, i =>
        {
            var response = engine.Exchange(Context(), request, out _);
            if (response.Handled)
            {
                Interlocked.Increment(ref errors);
            }
        });

        Assert.Equal(0, errors);
    }

    [Fact]
    public void Tick_Does_Not_Throw_After_First_Exchange()
    {
        var engine = CreateEngine();
        engine.Exchange(Context(), new ApiGCExchangeRequest { AppId = 570, MessageType = 1, BodyBase64 = "" }, out _);
        engine.Tick();
    }
}

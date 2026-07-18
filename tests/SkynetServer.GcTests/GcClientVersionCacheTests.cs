using SKYNET_server.Services;
using Xunit;

namespace SkynetServer.GcTests;

/// <summary>
/// Fase B decision check: clientVersion is derived server-side from
/// CMsgClientHello field 1 - validated against a real nethook capture.
/// </summary>
[Collection("gc-serial")]
public class GcClientVersionCacheTests
{
    [Fact]
    public void Parses_Version_From_Real_ClientHello_Fixture()
    {
        var payload = File.ReadAllBytes(Path.Combine(
            TestHost.RepoRoot, "SKYNET server", "GC", "570", "fixtures", "nethook", "client_4006_0001.bin"));

        Assert.Equal(6851u, GcClientVersionCache.TryParseHelloVersion(payload));
    }

    [Fact]
    public void Empty_Or_Garbage_Payload_Yields_Null()
    {
        Assert.Null(GcClientVersionCache.TryParseHelloVersion(Array.Empty<byte>()));
        Assert.Null(GcClientVersionCache.TryParseHelloVersion(new byte[] { 0xFF, 0xFF, 0xFF }));
    }

    [Fact]
    public void Cache_Is_Per_App_And_Steamid()
    {
        var cache = new GcClientVersionCache();
        cache.Set(570, 1, 6851);
        Assert.Equal(6851u, cache.Get(570, 1));
        Assert.Null(cache.Get(570, 2));
        Assert.Null(cache.Get(730, 1));
    }
}

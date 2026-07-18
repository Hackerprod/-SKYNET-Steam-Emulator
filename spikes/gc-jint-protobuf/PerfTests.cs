using System.Diagnostics;
using System.Text.Json.Nodes;
using Xunit;
using Xunit.Abstractions;

namespace GcJintProtobufSpike;

/// <summary>
/// Gate 5: perf of the WORST realistic case under the Jint interpreter (no V8
/// JIT): the real SOCacheSubscribed capture with ~309 econ items, plus per-item
/// CSOEconItem decode+encode. Numbers land in the test output and the README.
/// </summary>
public class PerfTests : IClassFixture<SpikeFixture>
{
    private readonly SpikeFixture _spike;
    private readonly ITestOutputHelper _output;

    public PerfTests(SpikeFixture spike, ITestOutputHelper output)
    {
        _spike = spike;
        _output = output;
    }

    private static (double p50, double p99) Percentiles(List<double> samples)
    {
        var sorted = samples.OrderBy(v => v).ToList();
        double At(double q) => sorted[Math.Min(sorted.Count - 1, (int)Math.Ceiling(q * sorted.Count) - 1)];
        return (At(0.50), At(0.99));
    }

    [Fact]
    public void SOCacheSubscribed_Realistic_RoundTrip_Perf()
    {
        var socache = _spike.Fixture("fixtures/nethook/server_24_0001.bin");
        var b64 = Convert.ToBase64String(socache);

        // warmup (codegen'd encoder/decoder creation happens lazily)
        _spike.Engine.Invoke("__proto_roundtrip_bench", "CMsgSOCacheSubscribed", b64, 3);

        var samples = new List<double>();
        for (var i = 0; i < 40; i++)
        {
            var sw = Stopwatch.StartNew();
            _spike.Engine.Invoke("__proto_roundtrip_bench", "CMsgSOCacheSubscribed", b64, 1);
            sw.Stop();
            samples.Add(sw.Elapsed.TotalMilliseconds);
        }

        var (p50, p99) = Percentiles(samples);
        _output.WriteLine($"SOCacheSubscribed ({socache.Length} bytes) decode+encode under Jint: p50={p50:F2}ms p99={p99:F2}ms");

        // Generous ceiling: this runs once per login, not per frame. If this
        // trips, revisit before committing to the JS codec path (regla 7).
        Assert.True(p50 < 250, $"p50 {p50:F2}ms exceeds 250ms budget");
    }

    [Fact]
    public void EconItem_PerItem_Decode_Perf_Across_Full_Inventory()
    {
        var socache = _spike.Fixture("fixtures/nethook/server_24_0001.bin");
        var json = _spike.DecodeJson("CMsgSOCacheSubscribed", socache);
        var node = JsonNode.Parse(json)!;
        var blobs = node["objects"]!.AsArray().First(o => o!["typeId"]!.GetValue<int>() == 1)!["objectData"]!.AsArray();
        var itemB64 = blobs[0]!.GetValue<string>();
        var itemCount = blobs.Count;

        _spike.Engine.Invoke("__proto_roundtrip_bench", "CSOEconItem", itemB64, 5);

        var samples = new List<double>();
        for (var i = 0; i < 10; i++)
        {
            var sw = Stopwatch.StartNew();
            _spike.Engine.Invoke("__proto_roundtrip_bench", "CSOEconItem", itemB64, itemCount);
            sw.Stop();
            samples.Add(sw.Elapsed.TotalMilliseconds);
        }

        var (p50, p99) = Percentiles(samples);
        _output.WriteLine($"CSOEconItem x{itemCount} decode+encode under Jint: p50={p50:F2}ms p99={p99:F2}ms (~{p50 / itemCount:F3}ms/item)");
        Assert.True(p50 < 2000, $"p50 {p50:F2}ms exceeds 2000ms budget for {itemCount} items");
    }
}

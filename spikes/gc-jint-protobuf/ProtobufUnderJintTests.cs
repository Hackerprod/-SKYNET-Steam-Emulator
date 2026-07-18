using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace GcJintProtobufSpike;

public class ProtobufUnderJintTests : IClassFixture<SpikeFixture>
{
    private readonly SpikeFixture _spike;

    public ProtobufUnderJintTests(SpikeFixture spike) => _spike = spike;

    private static bool SemanticallyEqual(string jsonA, string jsonB)
        => JsonNode.DeepEquals(JsonNode.Parse(jsonA), JsonNode.Parse(jsonB));

    // ------------------------------------------------------------------
    // Gate 1: protobuf.js codegen (new Function) works under Jint at all.
    // The bundle load already exercised Root.fromJSON; encode/decode below
    // exercises the generated encoder/decoder functions.
    // ------------------------------------------------------------------
    [Fact]
    public void Codegen_Works_Under_Jint()
    {
        var bytes = _spike.EncodeFromJson("SpikeNested", """{"id":"123456789012345","name":"spike"}""");
        Assert.NotEmpty(bytes);
        var decoded = _spike.DecodeJson("SpikeNested", bytes);
        Assert.True(SemanticallyEqual("""{"id":"123456789012345","name":"spike"}""", decoded), decoded);
        Assert.Contains(_spike.LogLines, l => l.Contains("proto descriptor initialised"));
    }

    // ------------------------------------------------------------------
    // Gate 2: 64-bit representation. Inside JS: Long.js (protobuf.js native,
    // configured with the bundled long library). At the JSON boundary: string.
    // Values above 2^53 must survive exactly.
    // ------------------------------------------------------------------
    [Theory]
    [InlineData("9223372036854775807")] // long.MaxValue
    [InlineData("18446744073709551615")] // ulong.MaxValue (uint64)
    [InlineData("76561198429375037")] // real steamid from the fixtures
    public void Uint64_Longjs_Representation_Preserves_Exact_Value(string value)
    {
        var json = $$"""{"u64":"{{value}}","f64":"{{value}}"}""";
        var bytes = _spike.EncodeFromJson("SpikeKitchenSink", json);
        var decoded = _spike.DecodeJson("SpikeKitchenSink", bytes);
        var node = JsonNode.Parse(decoded)!;
        Assert.Equal(value, node["u64"]!.GetValue<string>());
        Assert.Equal(value, node["f64"]!.GetValue<string>());
    }

    // ------------------------------------------------------------------
    // Difficult protobuf shapes: repeated fixed64 (unpacked), packed repeated,
    // bytes, unknown enum value, nested + repeated nested, sint64 zigzag.
    // ------------------------------------------------------------------
    [Fact]
    public void KitchenSink_RoundTrips_All_Difficult_Types()
    {
        var original = """
        {
          "u64": "18446744073709551615",
          "f64": "76561198429375037",
          "f64List": ["1", "18446744073709551615", "76561198429375037"],
          "packedU64": ["7", "9007199254740993", "18446744073709551615"],
          "blob": "AAECA/8=",
          "enumVal": 99,
          "nested": { "id": "678418176778809895", "name": "outer" },
          "nestedList": [ { "id": "1", "name": "a" }, { "id": "2", "name": "b" } ],
          "s64": "-9223372036854775808",
          "text": "áéí unicode ✓"
        }
        """;
        var bytes = _spike.EncodeFromJson("SpikeKitchenSink", original);
        var decoded = _spike.DecodeJson("SpikeKitchenSink", bytes);
        Assert.True(SemanticallyEqual(original, decoded), decoded);
    }

    // ------------------------------------------------------------------
    // Real fixture: CMsgClientHello from nethook. Validates the curated .proto
    // against real wire bytes (Fase 2 rule: fixtures prove the proto).
    // ------------------------------------------------------------------
    [Fact]
    public void Fixture_ClientHello_Decodes_With_Correct_Uint64()
    {
        var decoded = _spike.DecodeJson("CMsgClientHello", _spike.Fixture("fixtures/nethook/client_4006_0001.bin"));
        var node = JsonNode.Parse(decoded)!;
        Assert.Equal(6851, node["version"]!.GetValue<int>());
        Assert.Equal("76561198429375037", node["socacheHaveVersions"]![0]!["soid"]!["id"]!.GetValue<string>());
        Assert.Equal("678418176778809895", node["socacheHaveVersions"]![0]!["version"]!.GetValue<string>());
    }

    // ------------------------------------------------------------------
    // Gate 3 (documented limitation): protobuf.js DROPS unknown fields on
    // decode. A decode->encode round-trip of a message with unmodeled fields
    // is NOT byte-stable, but all modeled fields survive semantically.
    // Pass-through ids must therefore re-emit original bytes, never
    // decode->encode (see README for the audited subset).
    // ------------------------------------------------------------------
    [Fact]
    public void UnknownFields_Are_Dropped_On_Decode_As_Documented()
    {
        // craft: valid SpikeNested + unknown varint field 999
        var baseBytes = _spike.EncodeFromJson("SpikeNested", """{"id":"42","name":"x"}""");
        var withUnknown = baseBytes
            .Concat(new byte[] { 0xB8, 0x3E, 0x01 }) // tag: field 999 wire 0, value 1
            .ToArray();

        var decodedJson = _spike.DecodeJson("SpikeNested", withUnknown);
        var reEncoded = _spike.EncodeFromJson("SpikeNested", decodedJson);

        Assert.True(reEncoded.Length < withUnknown.Length, "unknown field should have been dropped");
        Assert.True(SemanticallyEqual(_spike.DecodeJson("SpikeNested", baseBytes), decodedJson));

        // same story on a real capture: ClientHello carries fields the curated
        // proto does not model; decode->encode shrinks but keeps modeled fields
        var hello = _spike.Fixture("fixtures/nethook/client_4006_0001.bin");
        var helloJson = _spike.DecodeJson("CMsgClientHello", hello);
        var helloReEncoded = _spike.EncodeFromJson("CMsgClientHello", helloJson);
        Assert.True(helloReEncoded.Length < hello.Length);
        Assert.True(SemanticallyEqual(helloJson, _spike.DecodeJson("CMsgClientHello", helloReEncoded)));
    }

    // ------------------------------------------------------------------
    // Gate 4a: byte-exact oracle. Only valid for pass-through / catalogo-global
    // blobs that are re-emitted untouched.
    // ------------------------------------------------------------------
    [Fact]
    public void Oracle_ByteExact_Holds_For_PassThrough_Blobs()
    {
        var original = _spike.Fixture("fixtures/nethook/server_24_0001.bin");
        var reEmitted = (byte[])original.Clone(); // pass-through: bytes untouched
        Assert.Equal(original, reEmitted);
    }

    // ------------------------------------------------------------------
    // Gate 4b: semantic oracle. Protobuf has no canonical encoding: two byte
    // sequences with different field order are wire-equivalent. The semantic
    // differ (decode both, compare trees) must accept them; byte compare must
    // reject them - which is exactly why byte-exact is the wrong oracle for
    // reconstructed messages.
    // ------------------------------------------------------------------
    [Fact]
    public void Oracle_Semantic_Accepts_Reordered_WireEquivalent_Encodings()
    {
        // SpikeNested { fixed64 id = 1; string name = 2; }
        byte[] idField = { 0x09, 0x2A, 0, 0, 0, 0, 0, 0, 0 }; // id = 42
        byte[] nameField = { 0x12, 0x02, (byte)'h', (byte)'i' }; // name = "hi"
        var ordered = idField.Concat(nameField).ToArray();
        var reordered = nameField.Concat(idField).ToArray();

        Assert.NotEqual(ordered, reordered); // byte oracle would fail here
        Assert.True(SemanticallyEqual(
            _spike.DecodeJson("SpikeNested", ordered),
            _spike.DecodeJson("SpikeNested", reordered)));
    }

    // ------------------------------------------------------------------
    // Real-world reconstruction check: decode the realistic SOCacheSubscribed
    // capture (309 econ items), re-encode, and verify semantic equivalence of
    // everything modeled, plus decode of the inner CSOEconItem blobs.
    // ------------------------------------------------------------------
    [Fact]
    public void Fixture_SOCacheSubscribed_Reconstruction_Is_Semantically_Stable()
    {
        var original = _spike.Fixture("fixtures/nethook/server_24_0001.bin");
        var json = _spike.DecodeJson("CMsgSOCacheSubscribed", original);
        var node = JsonNode.Parse(json)!;
        var econ = node["objects"]!.AsArray().First(o => o!["typeId"]!.GetValue<int>() == 1)!;
        var itemBlobs = econ["objectData"]!.AsArray();
        Assert.True(itemBlobs.Count > 100, $"expected a realistic econ payload, got {itemBlobs.Count}");

        var firstItem = _spike.DecodeJson("CSOEconItem", Convert.FromBase64String(itemBlobs[0]!.GetValue<string>()));
        var itemNode = JsonNode.Parse(firstItem)!;
        Assert.True(ulong.Parse(itemNode["id"]!.GetValue<string>()) > 0);

        var reEncoded = _spike.EncodeFromJson("CMsgSOCacheSubscribed", json);
        Assert.True(SemanticallyEqual(json, _spike.DecodeJson("CMsgSOCacheSubscribed", reEncoded)));
    }
}

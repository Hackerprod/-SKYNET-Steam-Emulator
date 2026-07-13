using System.Security.Cryptography;
using System.Text.Json;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;

namespace SKYNET_server.Services;

/// <summary>
/// Owns the emulator's Steam Datagram (SDR) certificate authority. The CA key
/// pair is generated once and persisted, so every client on the LAN trusts the
/// same root: the DLL patches this CA public key into steamnetworkingsockets.dll,
/// and this service signs per-identity certs with the matching private key.
///
/// Ed25519 throughout, matching Steam's networking cert scheme. The protobuf field
/// numbers below follow the public GameNetworkingSockets definitions; if a target
/// Dota build rejects a cert, they are the first thing to re-verify.
/// </summary>
public sealed class SdrCertificateService
{
    private const int CertValidityDays = 365;

    // Fixed CA. Generated once and pinned in code so it never changes and always
    // matches the public key hard-coded in the DLL patcher. The private seed lives
    // only here (the server is trusted); the DLL only ever sees the public key.
    // Proven CA from the reference coordinator (NetworkingCertManager): its private
    // seed is literally the first 32 bytes of the old private.pem, giving CA public
    // key FEAA97C32C7E5BF684DF86F120F3C40C785DCECDEDCB91FC223E54E76AA30F59. Steam's
    // key-id for it is 13962645978238679445 (set via Sdr:CaKeyId). The Dota
    // steamnetworkingsockets.dll must be patched with this same public key.
    private const string DefaultSeedBase64 = "LS0tLS1CRUdJTiBPUEVOU1NIIFBSSVZBVEUgS0VZLS0=";

    private readonly object _sync = new();
    private readonly string _keyPath;
    private readonly ILogger<SdrCertificateService> _logger;
    private readonly ulong _caKeyIdOverride;

    private Ed25519PrivateKeyParameters _caPrivate = null!;
    private Ed25519PublicKeyParameters _caPublic = null!;
    private byte[] _caPublicBytes = Array.Empty<byte>();
    private ulong _caKeyId;

    public SdrCertificateService(IHostEnvironment hostEnvironment, IConfiguration configuration, ILogger<SdrCertificateService> logger)
    {
        _logger = logger;
        _keyPath = Path.Combine(hostEnvironment.ContentRootPath, "Data", "sdr-ca.json");

        // If the target DLL keeps its original CA key-id table entry and we only
        // swap the public key bytes in place, issued certs must carry that original
        // id. Set it here (decimal or 0x-hex). If left at 0 we derive the id from
        // our own public key, which is correct when the DLL derives ids at load.
        var configured = configuration["Sdr:CaKeyId"];
        _caKeyIdOverride = ParseKeyId(configured);

        LoadOrCreateCa();
    }

    public byte[] CaPublicKey
    {
        get { lock (_sync) { return (byte[])_caPublicBytes.Clone(); } }
    }

    public ulong CaKeyId
    {
        get { lock (_sync) { return _caKeyId; } }
    }

    public SdrCertResult IssueCertificate(ulong steamId, uint appId)
    {
        lock (_sync)
        {
            // Deterministic identity key pair per steamId. Steam's networking library
            // adopts the private key from the first cert and then requires every later
            // cert to carry the SAME key (otherwise "Private key mismatch" -> reject).
            // A random key per request breaks re-requests / cache misses, so we derive
            // the identity seed deterministically from the identity.
            var seed = System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes($"skynet-sdr-identity:{steamId}"));
            var identityPrivate = new Ed25519PrivateKeyParameters(seed, 0);
            var identityPublic = identityPrivate.GeneratePublicKey();

            // Build/sign exactly like the reference coordinator's NetworkingCertManager.
            var (cert, signature) = SKYNET_server.Certificate.NetworkingCertManager.BuildSignedCert(
                identityPublic.GetEncoded(), steamId, appId, _caPrivate);

            return new SdrCertResult
            {
                Certificate = cert,
                Signature = signature,
                CaKeyId = _caKeyId,
                PrivateKey = identityPrivate.GetEncoded(),
                PublicKey = identityPublic.GetEncoded()
            };
        }
    }

    private byte[] BuildCertificate(byte[] identityPublicKey, ulong steamId, uint appId)
    {
        var now = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var expiry = (uint)DateTimeOffset.UtcNow.AddDays(CertValidityDays).ToUnixTimeSeconds();

        var writer = new ProtoWriter();
        writer.WriteVarint(1, 1);                       // key_type = ED25519
        writer.WriteBytes(2, identityPublicKey);        // key_data
        writer.WriteFixed64(4, steamId);                // legacy_steam_id
        // The reference coordinator (proven working) sets BOTH identity forms, and
        // current Dota validates the modern identity rather than legacy_steam_id.
        if (steamId != 0)
        {
            // legacy_identity_binary (field 11) = { steam_id (field 16, fixed64) }
            var legacyIdentity = new ProtoWriter();
            legacyIdentity.WriteFixed64(16, steamId);
            writer.WriteBytes(11, legacyIdentity.ToArray());
            // identity_string (field 12) = "steamid:<steamid64>"
            writer.WriteBytes(12, System.Text.Encoding.UTF8.GetBytes($"steamid:{steamId}"));
        }
        // These are fixed32 in CMsgSteamDatagramCertificate.  Encoding them
        // as varints makes the native protobuf reader ignore both fields,
        // leaving the certificate without a valid time window.
        writer.WriteFixed32(8, now);                     // time_created
        writer.WriteFixed32(9, expiry);                  // time_expiry
        if (appId != 0)
        {
            writer.WriteVarint(10, appId);              // app_ids (packed-compatible repeated)
        }

        return writer.ToArray();
    }

    private static byte[] Sign(Ed25519PrivateKeyParameters key, byte[] message)
    {
        var signer = new Ed25519Signer();
        signer.Init(true, key);
        signer.BlockUpdate(message, 0, message.Length);
        return signer.GenerateSignature();
    }

    private void LoadOrCreateCa()
    {
        lock (_sync)
        {
            // A file under Data/sdr-ca.json overrides the pinned key only if present;
            // otherwise the fixed in-code seed is used, so the CA is deterministic.
            var seed = LoadSeedOverride() ?? Convert.FromBase64String(DefaultSeedBase64);
            _caPrivate = new Ed25519PrivateKeyParameters(seed, 0);
            _caPublic = _caPrivate.GeneratePublicKey();
            _caPublicBytes = _caPublic.GetEncoded();
            _caKeyId = _caKeyIdOverride != 0 ? _caKeyIdOverride : DeriveKeyId(_caPublicBytes);

            _logger.LogInformation(
                "SDR CA ready. Public key {Pub}, key id {KeyId:X16}",
                Convert.ToHexString(_caPublicBytes), _caKeyId);
        }
    }

    private byte[]? LoadSeedOverride()
    {
        if (!File.Exists(_keyPath))
        {
            return null;
        }

        try
        {
            var stored = JsonSerializer.Deserialize<StoredCa>(File.ReadAllText(_keyPath));
            return string.IsNullOrWhiteSpace(stored?.PrivateSeedBase64)
                ? null
                : Convert.FromBase64String(stored!.PrivateSeedBase64);
        }
        catch (Exception ex) when (ex is IOException or JsonException or FormatException)
        {
            _logger.LogError(ex, "Failed to load SDR CA override, using pinned key");
            return null;
        }
    }

    // Steam derives a public-key id as the leading 8 bytes of the SHA-256 of the
    // raw key, read little-endian. Used only when no explicit id is configured.
    private static ulong DeriveKeyId(byte[] publicKey)
    {
        var digest = SHA256.HashData(publicKey);
        return BitConverter.ToUInt64(digest, 0);
    }

    private static ulong ParseKeyId(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return 0;
        }

        value = value.Trim();
        if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            return ulong.TryParse(value[2..], System.Globalization.NumberStyles.HexNumber, null, out var hex) ? hex : 0;
        }

        return ulong.TryParse(value, out var dec) ? dec : 0;
    }

    private sealed class StoredCa
    {
        public string PrivateSeedBase64 { get; set; } = string.Empty;
        public string PublicKeyBase64 { get; set; } = string.Empty;
    }

    /// <summary>Minimal protobuf writer for the fixed cert layout.</summary>
    private sealed class ProtoWriter
    {
        private readonly List<byte> _buffer = new();

        public void WriteVarint(int field, ulong value)
        {
            WriteTag(field, 0);
            WriteRawVarint(value);
        }

        public void WriteFixed64(int field, ulong value)
        {
            WriteTag(field, 1);
            _buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void WriteFixed32(int field, uint value)
        {
            WriteTag(field, 5);
            _buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void WriteBytes(int field, byte[] value)
        {
            WriteTag(field, 2);
            WriteRawVarint((ulong)value.Length);
            _buffer.AddRange(value);
        }

        public byte[] ToArray() => _buffer.ToArray();

        private void WriteTag(int field, int wireType) => WriteRawVarint(((ulong)field << 3) | (ulong)wireType);

        private void WriteRawVarint(ulong value)
        {
            while (value >= 0x80)
            {
                _buffer.Add((byte)(value | 0x80));
                value >>= 7;
            }

            _buffer.Add((byte)value);
        }
    }
}

public sealed class SdrCertResult
{
    public byte[] Certificate { get; init; } = Array.Empty<byte>();
    public byte[] Signature { get; init; } = Array.Empty<byte>();
    public ulong CaKeyId { get; init; }
    public byte[] PrivateKey { get; init; } = Array.Empty<byte>();
    public byte[] PublicKey { get; init; } = Array.Empty<byte>();
}

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
    private const string DefaultSeedBase64 = "WfXuH+6DV2sY/9Kda3THnqLAUYKTT8FApSi9nbH5CcY=";

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
            // Per-session identity key pair. The private key travels to the DLL so
            // the networking library can perform the connection handshake; it never
            // needs the CA private key.
            var identityPrivate = new Ed25519PrivateKeyParameters(RandomNumberGenerator.GetBytes(Ed25519PrivateKeyParameters.KeySize), 0);
            var identityPublic = identityPrivate.GeneratePublicKey();

            var cert = BuildCertificate(identityPublic.GetEncoded(), steamId, appId);
            var signature = Sign(_caPrivate, cert);

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

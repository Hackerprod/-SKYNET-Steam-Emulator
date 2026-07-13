using System;
using System.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;

namespace SKYNET_server.Certificate;

// Port of the reference coordinator's NetworkingCertManager. Builds and signs a
// CMsgSteamDatagramCertificate with the exact same fields, identity and CA key-id
// that the old (working) coordinator used. The Dota steamnetworkingsockets.dll must
// be patched with the matching CA public key (FEAA97C3...), whose Steam key-id is
// the constant below.
public static class NetworkingCertManager
{
    public const ulong CaKeyId = 13962645978238679445;

    public static (byte[] cert, byte[] signature) BuildSignedCert(
        byte[] keyData, ulong steamId, uint appId, Ed25519PrivateKeyParameters caPrivate)
    {
        uint now = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        uint expiry = (uint)DateTimeOffset.UtcNow.AddDays(30).ToUnixTimeSeconds();

        // CMsgSteamDatagramCertificate, fields in tag order (matches the reference
        // coordinator's protobuf serialization).
        var w = new CertProto();
        w.Varint(1, 1);                          // key_type = ED25519
        w.Bytes(2, keyData);                     // key_data
        w.Fixed64(4, steamId);                   // legacy_steam_id
        w.Fixed32(8, now);                       // time_created
        w.Fixed32(9, expiry);                    // time_expiry
        if (appId != 0)
        {
            w.Varint(10, appId);                 // app_ids (repeated)
        }
        var legacy = new CertProto();
        legacy.Fixed64(16, steamId);             // CMsgSteamNetworkingIdentityLegacyBinary.steam_id
        w.Bytes(11, legacy.ToArray());           // legacy_identity_binary
        w.Bytes(12, System.Text.Encoding.UTF8.GetBytes($"steamid:{steamId}")); // identity_string

        byte[] cert = w.ToArray();

        var signer = new Ed25519Signer();
        signer.Init(true, caPrivate);
        signer.BlockUpdate(cert, 0, cert.Length);
        byte[] signature = signer.GenerateSignature();

        return (cert, signature);
    }

    private sealed class CertProto
    {
        private readonly MemoryStream _ms = new();

        public void Varint(int field, ulong value)
        {
            WriteVarint((ulong)((field << 3) | 0));
            WriteVarint(value);
        }

        public void Fixed64(int field, ulong value)
        {
            WriteVarint((ulong)((field << 3) | 1));
            _ms.Write(BitConverter.GetBytes(value), 0, 8);
        }

        public void Fixed32(int field, uint value)
        {
            WriteVarint((ulong)((field << 3) | 5));
            _ms.Write(BitConverter.GetBytes(value), 0, 4);
        }

        public void Bytes(int field, byte[] value)
        {
            WriteVarint((ulong)((field << 3) | 2));
            WriteVarint((ulong)value.Length);
            _ms.Write(value, 0, value.Length);
        }

        private void WriteVarint(ulong value)
        {
            while (value >= 0x80)
            {
                _ms.WriteByte((byte)(value | 0x80));
                value >>= 7;
            }
            _ms.WriteByte((byte)value);
        }

        public byte[] ToArray() => _ms.ToArray();
    }
}

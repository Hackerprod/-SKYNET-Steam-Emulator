using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using ProtoBuf;

namespace SKYNET_server.Certificate;

// Port of the reference coordinator's NetworkingCertManager. Builds and signs a
// CMsgSteamDatagramCertificate with typed protobuf classes instead of hand-written
// wire tags. The Dota steamnetworkingsockets.dll must be patched with the matching
// CA public key (FEAA97C3...), whose Steam key-id is the constant below.
public static class NetworkingCertManager
{
    public const ulong CaKeyId = 14779564839147732469;

    public static (byte[] cert, byte[] signature) BuildSignedCert(
        byte[] keyData, ulong steamId, uint appId, Ed25519PrivateKeyParameters caPrivate)
    {
        uint now = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        uint expiry = (uint)DateTimeOffset.UtcNow.AddDays(30).ToUnixTimeSeconds();

        var certificate = new SteamDatagramCertificateMessage
        {
            KeyType = SteamDatagramCertificateMessage.EKeyType.ED25519,
            KeyData = keyData,
            LegacySteamId = steamId,
            LegacyIdentityBinary = new SteamNetworkingIdentityLegacyBinaryMessage
            {
                SteamId = steamId
            },
            IdentityString = $"steamid:{steamId}",
            TimeCreated = now,
            TimeExpiry = expiry
        };
        if (appId != 0)
        {
            certificate.AppIds.Add(appId);
        }

        byte[] cert = certificate.Serialize();

        var signer = new Ed25519Signer();
        signer.Init(true, caPrivate);
        signer.BlockUpdate(cert, 0, cert.Length);
        byte[] signature = signer.GenerateSignature();

        return (cert, signature);
    }
}

[ProtoContract(Name = @"CMsgSteamNetworkingIdentityLegacyBinary")]
internal sealed class SteamNetworkingIdentityLegacyBinaryMessage
{
    [ProtoMember(16, IsRequired = false, Name = @"steam_id", DataFormat = DataFormat.FixedSize)]
    [DefaultValue(default(ulong))]
    public ulong SteamId { get; set; }
}

[ProtoContract(Name = @"CMsgSteamDatagramCertificate")]
internal sealed class SteamDatagramCertificateMessage
{
    [ProtoMember(1, IsRequired = false, Name = @"key_type", DataFormat = DataFormat.TwosComplement)]
    [DefaultValue(EKeyType.INVALID)]
    public EKeyType KeyType { get; set; } = EKeyType.INVALID;

    [ProtoMember(2, IsRequired = false, Name = @"key_data", DataFormat = DataFormat.Default)]
    [DefaultValue(null)]
    public byte[]? KeyData { get; set; }

    [ProtoMember(4, IsRequired = false, Name = @"legacy_steam_id", DataFormat = DataFormat.FixedSize)]
    [DefaultValue(default(ulong))]
    public ulong LegacySteamId { get; set; }

    [ProtoMember(5, Name = @"gameserver_datacenter_ids", DataFormat = DataFormat.FixedSize)]
    public List<uint> GameServerDatacenterIds { get; set; } = new();

    [ProtoMember(8, IsRequired = false, Name = @"time_created", DataFormat = DataFormat.FixedSize)]
    [DefaultValue(default(uint))]
    public uint TimeCreated { get; set; }

    [ProtoMember(9, IsRequired = false, Name = @"time_expiry", DataFormat = DataFormat.FixedSize)]
    [DefaultValue(default(uint))]
    public uint TimeExpiry { get; set; }

    [ProtoMember(10, Name = @"app_ids", DataFormat = DataFormat.TwosComplement)]
    public List<uint> AppIds { get; set; } = new();

    [ProtoMember(11, IsRequired = false, Name = @"legacy_identity_binary", DataFormat = DataFormat.Default)]
    [DefaultValue(null)]
    public SteamNetworkingIdentityLegacyBinaryMessage? LegacyIdentityBinary { get; set; }

    [ProtoMember(12, IsRequired = false, Name = @"identity_string", DataFormat = DataFormat.Default)]
    [DefaultValue("")]
    public string IdentityString { get; set; } = string.Empty;

    public byte[] Serialize()
    {
        using var stream = new MemoryStream();
        Serializer.Serialize(stream, this);
        return stream.ToArray();
    }

    public enum EKeyType
    {
        INVALID = 0,

        ED25519 = 1
    }
}

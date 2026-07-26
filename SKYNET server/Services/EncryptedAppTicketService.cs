using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using SKYNET_server.Models;

namespace SKYNET_server.Services;

/// <summary>
/// Issues Valve-compatible encrypted application tickets on the trusted server.
/// The wire format is consumed by the official sdkencryptedappticket library.
/// </summary>
public sealed class EncryptedAppTicketService
{
    private const int MaximumUserDataLength = 64 * 1024;
    private const int TicketLifetimeSeconds = 21 * 24 * 60 * 60;
    private static readonly TimeSpan RequestLimit = TimeSpan.FromSeconds(60);

    private readonly EncryptedAppTicketKeyStore _keys;
    private readonly ConcurrentDictionary<(ulong SteamId, uint AppId), long> _lastIssued =
        new();

    public EncryptedAppTicketService(EncryptedAppTicketKeyStore keys)
    {
        _keys = keys;
    }

    public ApiEncryptedAppTicketResponse Issue(ulong steamId, uint appId, ReadOnlySpan<byte> userData)
    {
        if (steamId == 0 || appId == 0 || userData.Length > MaximumUserDataLength)
        {
            return Failure(EResult.InvalidParam);
        }

        if (!_keys.TryGetKey(appId, out var key))
        {
            return Failure(EResult.AccessDenied);
        }

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var requestKey = (steamId, appId);
        if (_lastIssued.TryGetValue(requestKey, out var lastIssued) &&
            now - lastIssued < RequestLimit.TotalSeconds)
        {
            return Failure(EResult.LimitExceeded);
        }

        var ticket = EncryptedAppTicketCodec.Create(
            steamId,
            appId,
            userData,
            key,
            checked((uint)now),
            checked((uint)(now + TicketLifetimeSeconds)));
        _lastIssued[requestKey] = now;

        return new ApiEncryptedAppTicketResponse
        {
            Result = (int)EResult.OK,
            TicketBase64 = Convert.ToBase64String(ticket)
        };
    }

    private static ApiEncryptedAppTicketResponse Failure(EResult result) => new()
    {
        Result = (int)result
    };

    private enum EResult
    {
        OK = 1,
        InvalidParam = 8,
        AccessDenied = 15,
        LimitExceeded = 25
    }
}

internal static class EncryptedAppTicketCodec
{
    private const uint TicketVersion = 1;
    private const uint OwnershipTicketVersion = 4;
    private const int EncryptedIvLength = 16;
    private const int DigestLength = 20;

    public static byte[] Create(
        ulong steamId,
        uint appId,
        ReadOnlySpan<byte> userData,
        ReadOnlySpan<byte> key,
        uint issueTime,
        uint validUntil)
    {
        if (key.Length != 32)
        {
            throw new ArgumentException("Encrypted application ticket keys must contain 32 bytes.", nameof(key));
        }

        var ownership = CreateOwnershipTicket(steamId, appId, issueTime, validUntil);
        var plainTicket = CreatePlainTicket(userData, ownership);
        var encryptedTicket = Encrypt(plainTicket, key);

        using var output = new MemoryStream();
        WriteVarintField(output, 1, TicketVersion);
        WriteVarintField(output, 2, Crc32.Compute(plainTicket));
        WriteVarintField(output, 3, checked((uint)userData.Length));
        WriteVarintField(output, 4, checked((uint)ownership.Length));
        WriteBytesField(output, 5, encryptedTicket);
        return output.ToArray();
    }

    private static byte[] CreatePlainTicket(ReadOnlySpan<byte> userData, byte[] ownership)
    {
        using var output = new MemoryStream();
        output.Write(userData);
        output.Write(ownership);

        // The legacy ticket authenticator is part of the decrypted payload. Its
        // SHA-1 covers the whole ticket through the first eight authenticator bytes.
        WriteUInt32(output, 1);
        WriteUInt32(output, DigestLength);
        var authenticatedLength = checked((int)output.Length);
        var authenticated = output.GetBuffer().AsSpan(0, authenticatedLength);
        output.Write(SHA1.HashData(authenticated));
        WriteUInt32(output, checked((uint)ownership.Length));
        return output.ToArray();
    }

    private static byte[] CreateOwnershipTicket(
        ulong steamId,
        uint appId,
        uint issueTime,
        uint validUntil)
    {
        // Version 4 consists of the fixed 40-byte ownership header followed by
        // a bounded AppID list and VAC status. The base AppID is included so
        // BUserOwnsAppInTicket and GetTicketAppID agree for every title.
        const ushort appCount = 1;
        const int versionFourLength = 2 + appCount * sizeof(uint) + sizeof(uint);
        const int ownershipLength = 40 + versionFourLength;

        using var output = new MemoryStream(ownershipLength);
        WriteUInt32(output, ownershipLength);
        WriteUInt32(output, OwnershipTicketVersion);
        WriteUInt64(output, steamId);
        WriteUInt32(output, appId);
        WriteUInt32(output, 0);
        WriteUInt32(output, 0);
        WriteUInt32(output, 0);
        WriteUInt32(output, issueTime);
        WriteUInt32(output, validUntil);
        WriteUInt16(output, appCount);
        WriteUInt32(output, appId);
        WriteUInt32(output, 0);
        return output.ToArray();
    }

    private static byte[] Encrypt(byte[] plainTicket, ReadOnlySpan<byte> key)
    {
        var iv = RandomNumberGenerator.GetBytes(EncryptedIvLength);
        using var aes = Aes.Create();
        aes.Key = key.ToArray();
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.None;

        byte[] encryptedIv;
        using (var encryptor = aes.CreateEncryptor())
        {
            encryptedIv = encryptor.TransformFinalBlock(iv, 0, iv.Length);
        }

        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.IV = iv;
        byte[] encryptedPayload;
        using (var encryptor = aes.CreateEncryptor())
        {
            encryptedPayload = encryptor.TransformFinalBlock(plainTicket, 0, plainTicket.Length);
        }

        var result = new byte[encryptedIv.Length + encryptedPayload.Length];
        Buffer.BlockCopy(encryptedIv, 0, result, 0, encryptedIv.Length);
        Buffer.BlockCopy(encryptedPayload, 0, result, encryptedIv.Length, encryptedPayload.Length);
        return result;
    }

    private static void WriteBytesField(Stream output, int fieldNumber, ReadOnlySpan<byte> value)
    {
        WriteVarint(output, checked((uint)((fieldNumber << 3) | 2)));
        WriteVarint(output, checked((uint)value.Length));
        output.Write(value);
    }

    private static void WriteVarintField(Stream output, int fieldNumber, uint value)
    {
        WriteVarint(output, checked((uint)(fieldNumber << 3)));
        WriteVarint(output, value);
    }

    private static void WriteVarint(Stream output, uint value)
    {
        while (value >= 0x80)
        {
            output.WriteByte((byte)(value | 0x80));
            value >>= 7;
        }
        output.WriteByte((byte)value);
    }

    private static void WriteUInt16(Stream output, ushort value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(ushort)];
        BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
        output.Write(buffer);
    }

    private static void WriteUInt32(Stream output, uint value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(uint)];
        BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
        output.Write(buffer);
    }

    private static void WriteUInt64(Stream output, ulong value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(ulong)];
        BinaryPrimitives.WriteUInt64LittleEndian(buffer, value);
        output.Write(buffer);
    }

    private static class Crc32
    {
        private static readonly uint[] Table = CreateTable();

        public static uint Compute(ReadOnlySpan<byte> data)
        {
            var crc = uint.MaxValue;
            foreach (var value in data)
            {
                crc = (crc >> 8) ^ Table[(crc ^ value) & 0xFF];
            }
            return ~crc;
        }

        private static uint[] CreateTable()
        {
            const uint polynomial = 0xEDB88320;
            var table = new uint[256];
            for (uint i = 0; i < table.Length; i++)
            {
                var value = i;
                for (var bit = 0; bit < 8; bit++)
                {
                    value = (value >> 1) ^ ((value & 1) != 0 ? polynomial : 0);
                }
                table[i] = value;
            }
            return table;
        }
    }
}

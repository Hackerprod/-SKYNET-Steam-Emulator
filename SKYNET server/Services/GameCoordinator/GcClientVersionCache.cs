using System.Collections.Concurrent;

namespace SKYNET_server.Services;

/// <summary>
/// Fase B decision for GcEnvelope.clientVersion: the client build is NOT part
/// of the session handshake and steam_api (the client DLL) does not change, so
/// the version is derived server-side from the last CMsgClientHello (field 1,
/// varint) seen for the (appId, steamId) pair. Null until the first hello.
/// </summary>
public sealed class GcClientVersionCache
{
    private readonly ConcurrentDictionary<(uint AppId, ulong SteamId), uint> _versions = new();

    public void Set(uint appId, ulong steamId, uint version)
        => _versions[(appId, steamId)] = version;

    public uint? Get(uint appId, ulong steamId)
        => _versions.TryGetValue((appId, steamId), out var version) ? version : null;

    /// <summary>
    /// Reads varint field 1 (version) from a CMsgClientHello body. Host-side
    /// envelope plumbing only - GC logic never hand-parses wire bytes.
    /// </summary>
    public static uint? TryParseHelloVersion(byte[] payload)
    {
        var pos = 0;
        while (pos < payload.Length)
        {
            if (!TryReadVarint(payload, ref pos, out var tag))
            {
                return null;
            }

            var fieldNumber = (int)(tag >> 3);
            var wireType = (int)(tag & 7);
            if (fieldNumber == 1 && wireType == 0)
            {
                return TryReadVarint(payload, ref pos, out var version) ? (uint)version : null;
            }

            // skip other fields
            switch (wireType)
            {
                case 0:
                    if (!TryReadVarint(payload, ref pos, out _)) return null;
                    break;
                case 1:
                    pos += 8;
                    break;
                case 2:
                    if (!TryReadVarint(payload, ref pos, out var length) || pos + (int)length > payload.Length) return null;
                    pos += (int)length;
                    break;
                case 5:
                    pos += 4;
                    break;
                default:
                    return null;
            }
        }

        return null;
    }

    private static bool TryReadVarint(byte[] data, ref int pos, out ulong value)
    {
        value = 0;
        var shift = 0;
        while (pos < data.Length && shift < 64)
        {
            var b = data[pos++];
            value |= (ulong)(b & 0x7F) << shift;
            if ((b & 0x80) == 0)
            {
                return true;
            }

            shift += 7;
        }

        return false;
    }
}

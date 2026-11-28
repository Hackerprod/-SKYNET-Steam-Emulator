using System.Text;

namespace SKYNET.Iface;

/// <summary>How a C++ type travels through an x64 integer register.</summary>
internal enum K { Void, I8, U8, I16, U16, I32, U32, I64, U64, Bool, SteamId, CStr, Ptr, Enum }

/// <summary>
/// The decoder tables, keyed by C++ type text from the header-spec. Supporting a new interface = adding rows here
/// (aliases, enum names, flag parameters, out structs). This is the one place that needs real work for types that do
/// not fit an integer register: float/double (typed delegates), by-value structs, other hidden-return structs.
/// </summary>
internal static class Types
{
    private static readonly Dictionary<string, K> Alias = new Dictionary<string, K>
    {
        ["bool"] = K.Bool, ["CSteamID"] = K.SteamId,
        ["int"] = K.I32, ["int32"] = K.I32, ["uint32"] = K.U32, ["unsignedint"] = K.U32, ["int64"] = K.I64, ["uint64"] = K.U64,
        ["int16"] = K.I16, ["short"] = K.I16, ["uint16"] = K.U16, ["unsignedshort"] = K.U16, ["int8"] = K.I8, ["uint8"] = K.U8, ["unsignedchar"] = K.U8,
        // Steamworks handle typedefs (width from the SDK headers).
        ["HSteamUser"] = K.I32, ["HSteamPipe"] = K.I32, ["AppId_t"] = K.U32, ["DepotId_t"] = K.U32, ["HAuthTicket"] = K.U32,
        ["SteamAPICall_t"] = K.U64, ["UGCQueryHandle_t"] = K.U64, ["UGCUpdateHandle_t"] = K.U64, ["UGCHandle_t"] = K.U64,
        ["PublishedFileId_t"] = K.U64, ["PublishedFileUpdateHandle_t"] = K.U64, ["UGCFileWriteStreamHandle_t"] = K.U64,
        ["HSteamNetConnection"] = K.U32, ["HSteamListenSocket"] = K.U32, ["HHTMLBrowser"] = K.U32, ["HTTPRequestHandle"] = K.U32,
        ["InputHandle_t"] = K.U64, ["ControllerHandle_t"] = K.U64, ["SteamLeaderboard_t"] = K.U64, ["RTime32"] = K.U32,
        ["SNetSocket_t"] = K.U32, ["RemotePlaySessionID_t"] = K.U32, ["FriendsGroupID_t"] = K.I16, ["SteamInventoryResult_t"] = K.I32,
    };

    public static readonly Dictionary<string, string[]> Enums = new Dictionary<string, string[]>
    {
        ["EFriendRelationship"] = new[]
        {
            "k_EFriendRelationshipNone", "k_EFriendRelationshipBlocked", "k_EFriendRelationshipRequestRecipient",
            "k_EFriendRelationshipFriend", "k_EFriendRelationshipRequestInitiator", "k_EFriendRelationshipIgnored",
            "k_EFriendRelationshipIgnoredFriend", "k_EFriendRelationshipSuggested_DEPRECATED", "k_EFriendRelationshipMax",
        },
        ["EPersonaState"] = new[]
        {
            "k_EPersonaStateOffline", "k_EPersonaStateOnline", "k_EPersonaStateBusy", "k_EPersonaStateAway",
            "k_EPersonaStateSnooze", "k_EPersonaStateLookingToTrade", "k_EPersonaStateLookingToPlay", "k_EPersonaStateInvisible",
        },
    };

    // Bitmask parameters declared as plain int, keyed by parameter name.
    public static readonly Dictionary<string, KeyValuePair<int, string>[]> FlagParams = new Dictionary<string, KeyValuePair<int, string>[]>
    {
        ["iFriendFlags"] = new[]
        {
            new KeyValuePair<int, string>(0x01, "k_EFriendFlagBlocked"), new KeyValuePair<int, string>(0x02, "k_EFriendFlagFriendshipRequested"),
            new KeyValuePair<int, string>(0x04, "k_EFriendFlagImmediate"), new KeyValuePair<int, string>(0x08, "k_EFriendFlagClanMember"),
            new KeyValuePair<int, string>(0x10, "k_EFriendFlagOnGameServer"), new KeyValuePair<int, string>(0x80, "k_EFriendFlagRequestingFriendship"),
            new KeyValuePair<int, string>(0x100, "k_EFriendFlagRequestingInfo"), new KeyValuePair<int, string>(0x200, "k_EFriendFlagIgnored"),
            new KeyValuePair<int, string>(0x400, "k_EFriendFlagIgnoredFriend"), new KeyValuePair<int, string>(0x1000, "k_EFriendFlagChatMember"),
        },
    };

    // Out structs read after the call (when the call returned true / is not bool): size and a JSON renderer.
    public static readonly Dictionary<string, KeyValuePair<int, Func<byte[], string>>> OutStructs = new Dictionary<string, KeyValuePair<int, Func<byte[], string>>>
    {
        // FriendGameInfo_t (callbacks_x64.txt): m_gameID@0 m_unGameIP@8 m_usGamePort@12 m_usQueryPort@14 m_steamIDLobby@16.
        ["FriendGameInfo_t*"] = new KeyValuePair<int, Func<byte[], string>>(24, b =>
            "{\"gameID\":\"" + BitConverter.ToUInt64(b, 0) + "\",\"ip\":" + BitConverter.ToUInt32(b, 8) + ",\"port\":" + BitConverter.ToUInt16(b, 12) +
            ",\"queryPort\":" + BitConverter.ToUInt16(b, 14) + ",\"lobby\":\"" + BitConverter.ToUInt64(b, 16) + "\"}"),
    };

    public static string Norm(string t)
    {
        var n = t.Replace(" ", "");
        return n.StartsWith("const", StringComparison.Ordinal) ? n.Substring(5) : n;
    }

    /// <summary>Kind of a type, or null with the reason it is not supported yet.</summary>
    public static K? Resolve(string type, bool isReturn, out string? why)
    {
        why = null;
        var raw = type.Replace(" ", "");
        if (raw == "constchar*") return K.CStr;
        var n = Norm(type);
        if (n == "void") { if (isReturn) return K.Void; why = "void parameter"; return null; }
        if (n.EndsWith("*", StringComparison.Ordinal) || n.EndsWith("&", StringComparison.Ordinal)) return K.Ptr;
        if (Alias.TryGetValue(n, out var k)) return k;
        if (n.Length > 1 && n[0] == 'E' && char.IsUpper(n[1])) return K.Enum;
        why = n == "float" || n == "double" ? n + " needs typed delegates (xmm registers)" : "unknown or by-value struct type " + n;
        return null;
    }

    public static string FlagNames(KeyValuePair<int, string>[] table, int v)
    {
        if (v == 0) return "none";
        var parts = new List<string>();
        foreach (var kv in table)
            if ((v & kv.Key) != 0) { parts.Add(kv.Value); v &= ~kv.Key; }
        if (v != 0) parts.Add("0x" + v.ToString("X"));
        return string.Join("|", parts);
    }

    public static string SteamIdName(ulong id) => id == 0 ? "invalid" : "[U:" + (id >> 56) + ":" + (uint)(id & 0xFFFFFFFF) + "]";

    public static string Value(K k, long raw)
    {
        switch (k)
        {
            case K.I8: return ((sbyte)raw).ToString();
            case K.U8: return ((byte)raw).ToString();
            case K.I16: return ((short)raw).ToString();
            case K.U16: return ((ushort)raw).ToString();
            case K.I32: case K.Enum: return ((int)raw).ToString();
            case K.U32: return ((uint)raw).ToString();
            case K.I64: return raw.ToString();
            case K.U64: case K.SteamId: return ((ulong)raw).ToString();
            case K.Bool: return (raw & 0xFF) != 0 ? "true" : "false";
            case K.CStr: return Mem.ReadString((IntPtr)raw);
            default: return "0x" + raw.ToString("X");
        }
    }

    public static string EnumName(string typeNorm, string value) =>
        Enums.TryGetValue(typeNorm, out var names) && int.TryParse(value, out var v) && v >= 0 && v < names.Length ? names[v] : value;
}

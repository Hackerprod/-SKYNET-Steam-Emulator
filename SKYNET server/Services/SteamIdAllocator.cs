namespace SKYNET_server.Services;

/// <summary>
/// The single rule for the IDs this server hands out. The server is the only
/// authority: an account is created here and gets "highest account ID + 1"
/// (1000 when there is none), the same rule the original SKYNET GC server used
/// for its users. A client never proposes the ID of something that gets created.
/// </summary>
/// <remarks>
/// Account IDs live in [<see cref="FirstAccountId"/>, <see cref="MaxAccountId"/>]. Anything outside that range
/// (data from before this rule existed) is ignored when computing the next ID, so a single stray value can
/// never drag every new account up with it, and is reported at startup instead.
/// Each account type has its own counter, derived from its own collection.
/// </remarks>
internal static class SteamIdAllocator
{
    public const uint FirstAccountId = 1000;

    // Real Steam account IDs fit in 31 bits, and the game's native code is not built for anything else.
    public const uint MaxAccountId = int.MaxValue;

    private const ulong PublicUniverse = 1UL;
    private const ulong IndividualBase = 76561197960265728UL;
    private const ulong TypeGameServer = 3UL;
    private const ulong TypeAnonGameServer = 4UL;

    public static bool IsValidAccountId(uint accountId) => accountId >= FirstAccountId && accountId <= MaxAccountId;

    /// <summary>Highest valid existing ID + 1, or <see cref="FirstAccountId"/> when there is none.</summary>
    public static uint Next(IEnumerable<uint> existing)
    {
        uint highest = 0;
        foreach (var accountId in existing)
        {
            if (IsValidAccountId(accountId) && accountId > highest)
            {
                highest = accountId;
            }
        }

        if (highest == 0)
        {
            return FirstAccountId;
        }

        if (highest >= MaxAccountId)
        {
            throw new InvalidOperationException("The account ID range is exhausted.");
        }

        return highest + 1;
    }

    public static ulong IndividualSteamId(uint accountId) => IndividualBase + accountId;

    // The game rebuilds the server it connects to from the "[G:1:account]" text of the lobby, and that text has no
    // instance, so the rebuilt SteamID gets the default instance of a game server: 1. The identity the server
    // listens with has to be the same one or the P2P connection never matches ("Timed out attempting to connect").
    private const ulong GameServerInstance = 1UL;
    private const ulong InstanceMask = 0xFFFFFUL << 32;

    public static ulong GameServerSteamId(uint accountId, bool anonymous) =>
        (PublicUniverse << 56) | ((anonymous ? TypeAnonGameServer : TypeGameServer) << 52) | (GameServerInstance << 32) | accountId;

    /// <summary>The same game server SteamID with the instance the game expects (see <see cref="GameServerSteamId"/>).</summary>
    public static ulong CanonicalGameServerSteamId(ulong steamId) =>
        IsGameServerSteamId(steamId) ? (steamId & ~InstanceMask) | (GameServerInstance << 32) : steamId;

    /// <summary>A public-universe GameServer (3) or AnonGameServer (4) SteamID with a non-zero account.</summary>
    public static bool IsGameServerSteamId(ulong steamId)
    {
        var universe = steamId >> 56;
        var type = (steamId >> 52) & 0xF;
        return universe == PublicUniverse
            && (type == TypeGameServer || type == TypeAnonGameServer)
            && (uint)steamId != 0;
    }

    public static bool IsAnonymousGameServer(ulong steamId) => ((steamId >> 52) & 0xF) == TypeAnonGameServer;
}

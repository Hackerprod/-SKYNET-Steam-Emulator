using SKYNET.Types;
using System.Collections.Generic;

namespace SKYNET.IPC.Types
{
    public class IPCMessage
    {
        public ulong JobID { get; set; }
        public ulong To { get; set; }
        public int MessageType { get; set; }
        public string ParsedBody { get; set; }
        public bool WaitResult { get; set; }
        public object Result { get; set; }
    }

    public class IPC_MessageBase
    {

    }

    public enum IPCMessageType : int
    {
        // Users
        IPC_ClientHello,
        IPC_ClientWelcome,
        IPC_Settings,
        IPC_AvatarRequest,
        IPC_AvatarResponse,
        IPC_UserDataUpdated,
        IPC_P2PPacket,
        IPC_GetUserRequest,
        IPC_GetUserResponse,
        IPC_GetFriendsRequest,
        IPC_GetFriendsResponse,

        // Lobbies
        IPC_LobbyCreate,
        IPC_LobbyRemove,
        IPC_LobbyLeave,
        IPC_LobbyListRequest,
        IPC_LobbyListResponse,
        IPC_LobbyJoinRequest,
        IPC_LobbyJoinResponse,
        IPC_LobbyDataUpdate,
        IPC_LobbyChatUpdate,
        IPC_LobbyGameserver,
        IPC_LobbyRequest,
        IPC_LobbyResponse,
        IPC_LobbyByIndexRequest,
        IPC_LobbyByIndexResponse,
        IPC_LobbyCountRequest,
        IPC_LobbyCountResponse,
        IPC_LobbySetData,

        // User stats
        IPC_Leaderboards,
        IPC_Achievements,
        IPC_PlayerStats,
        IPC_SetAchievement,
        IPC_SetPlayerStat,
        IPC_SetLeaderboard,
        IPC_UpdateAchievement,
        IPC_ResetAllStats,

        // Gamecoordinator
        IPC_GCMessageRequest,
        IPC_GCMessageResponse,

        // File Log
        IPC_ModifyFileLog,

        // RichPresence
        IPC_ClearRichPresence,
        IPC_SetRichPresence,

        IPC_UsersRequest,
        IPC_UsersResponse,
        IPC_LobbiesRequest,
        IPC_LobbiesResponse,
    }

    public class IPC_ModifyFileLog : IPC_MessageBase
    {
        public bool Enabled { get; set; }
    }

    public class IPC_ClientHello : IPC_MessageBase
    {
        public string ExecutablePath { get; set; }
        public int ProcessID { get; set; }
    }

    public class IPC_ClientWelcome : IPC_MessageBase
    {
        public string PersonaName { get; set; }
        public string Language { get; set; }
        public uint AccountID { get; set; }
        public uint GameServerID { get; set; }
        public bool GameOverlay { get; set; }
        public bool LogToFile { get; set; }
        public bool LogToConsole { get; set; }
        public bool RunCallbacks { get; set; }
        public bool ISteamHTTP { get; set; }
        public string RemoteStoragePath { get; set; }
        public uint AppID { get; set; }
    }

    public class IPC_LobbyCreate : IPC_MessageBase
    {
        public string SerializedLobby { get; set; }
    }

    public class IPC_GCMessageRequest : IPC_MessageBase
    {
        public uint MsgType { get; set; }
        public byte[] Buffer { get; set; }
    }

    public class IPC_GCMessageResponse : IPC_MessageBase
    {
        public uint MsgType { get; set; }
        public byte[] Buffer { get; set; }
    }

    public class IPC_AvatarRequest : IPC_MessageBase
    {
        public uint AccountID { get; set; }
    }

    public class IPC_AvatarResponse : IPC_MessageBase
    {
        // TODO: Send AccountID 0 to fill default avatar 
        public uint AccountID { get; set; }
        public string HexAvatar { get; set; }
    }

    public class IPC_P2PPacket : IPC_MessageBase
    {
        public uint AccountID { get; set; }
        public byte[] Buffer { get; set; }
        public uint Sender { get; set; }
        public int P2PSendType { get; set; }
        public int Channel { get; set; }
    }

    public class IPC_UserDataUpdated : IPC_MessageBase
    {
        public uint AccountID { get; set; }
        public string PersonaName { get; set; }
        public uint LobbyID { get; set; }
        public UpdateType Type { get; set; }

        public enum UpdateType
        {
            PersonaName,
            LobbyID
        }
    }

    public class IPC_LobbyListRequest : IPC_MessageBase
    {
        public uint AppID { get; set; }
        public uint RequestID { get; set; }
    }

    public class IPC_LobbyListResponse : IPC_MessageBase
    {
        public string SerializedLobby { get; set; }
    }

    public class IPC_LobbyJoinRequest : IPC_MessageBase
    {
        public ulong SteamID { get; set; }
        public ulong LobbyID { get; set; }
        public ulong CallbackHandle { get; set; }
    }

    public class IPC_LobbyJoinResponse : IPC_MessageBase
    {
        public uint ChatRoomEnterResponse { get; set; }
        public ulong CallbackHandle { get; set; }
        public string SerializedLobby { get; set; }
    }

    public class IPC_LobbyDataUpdate : IPC_MessageBase
    {
        public ulong TargetSteamID { get; set; }
        public ulong SteamIDLobby { get; set; }
        public ulong SteamIDMember { get; set; }
        public string SerializedLobby { get; set; }
    }

    public class IPC_LobbyChatUpdate : IPC_MessageBase
    {
        public ulong SteamIDLobby { get; set; }
        public ulong SteamIDUserChanged { get; set; }
        public ulong SteamIDMakingChange { get; set; }
        public uint ChatMemberStateChange { get; set; }
    }

    public class IPC_LobbyLeave : IPC_MessageBase
    {
        public ulong LobbyID { get; set; }
        public ulong SteamID { get; set; }
    }

    public class IPC_LobbyRemove : IPC_MessageBase
    {
        public ulong LobbyID { get; set; }
    }

    public class IPC_LobbyGameserver : IPC_MessageBase
    {
        public ulong LobbyID { get; set; }
        public ulong SteamID { get; set; }
        public uint IP { get; set; }
        public uint Port { get; set; }
    }

    public class IPC_Leaderboards : IPC_MessageBase
    {
        public List<Leaderboard> Leaderboards { get; set; }
    }

    public class IPC_Achievements : IPC_MessageBase
    {
        public List<Achievement> Achievements { get; set; }
    }

    public class IPC_PlayerStats : IPC_MessageBase
    {
        public ulong SteamID { get; set; }
        public List<PlayerStat> PlayerStats { get; set; }
    }

    public class IPC_SetAchievement : IPC_MessageBase
    {
        public uint AppID { get; set; }
        public Achievement Achievement { get; set; }
    }
    public class IPC_UpdateAchievement : IPC_MessageBase
    {
        public uint AppID { get; set; }
        public Achievement Achievement { get; set; }
    }

    public class IPC_SetPlayerStat : IPC_MessageBase
    {
        public uint AppID { get; set; }
        public PlayerStat PlayerStat { get; set; }
    }

    public class IPC_SetLeaderboard : IPC_MessageBase
    {
        public uint AppID { get; set; }
        public Leaderboard Leaderboard { get; set; }
    }

    public class IPC_ResetAllStats : IPC_MessageBase
    {
        public uint AppID { get; set; }
        public bool AchievementsToo { get; set; }
    }

    public class IPC_GetUserRequest : IPC_MessageBase
    {
        public RequestType Type { get; set; }
        public ulong SteamID { get; set; }

        public enum RequestType
        {
            STEAMID,
            ACCOUNTID,
            IPADDRESS
        }
    }

    public class IPC_GetUserResponse : IPC_MessageBase
    {
        public SteamPlayer User { get; set; }
    }

    public class IPC_GetFriendsRequest : IPC_MessageBase
    {
    }

    public class IPC_GetFriendsResponse : IPC_MessageBase
    {
        public List<SteamPlayer> Friends { get; set; }
    }

    public class IPC_ClearRichPresence : IPC_MessageBase
    {
    }

    public class IPC_SetRichPresence : IPC_MessageBase
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class IPC_LobbyRequest : IPC_MessageBase
    {
        public ulong SteamID { get; internal set; }
        public bool ByOwner { get; internal set; }
    }

    public class IPC_LobbyResponse : IPC_MessageBase
    {
        public SteamLobby Lobby { get; set; }
    }

    public class IPC_LobbyByIndexRequest : IPC_MessageBase
    {
        public uint AppID { get; set; }
        public int Index { get; set; }
    }

    public class IPC_LobbyByIndexResponse : IPC_MessageBase
    {
        public SteamLobby Lobby { get; set; }
    }

    public class IPC_LobbyCountRequest : IPC_MessageBase
    {
        public uint AppID { get; set; }
    }

    public class IPC_LobbyCountResponse : IPC_MessageBase
    {
        public uint Count { get; set; }
    }

    public class IPC_LobbySetData : IPC_MessageBase
    {
        public ulong SteamID { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class IPC_UsersRequest : IPC_MessageBase
    {
    }

    public class IPC_UsersResponse : IPC_MessageBase
    {
        public List<SteamPlayer> Users { get; set; }
    }

    public class IPC_LobbiesRequest : IPC_MessageBase
    {
        public uint AppID { get; set; }
    }

    public class IPC_LobbiesResponse : IPC_MessageBase
    {
        public List<SteamLobby> Lobbies { get; set; }
    }
}

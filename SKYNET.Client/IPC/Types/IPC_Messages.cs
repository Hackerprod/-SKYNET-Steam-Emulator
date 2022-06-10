using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.IPC.Types
{
    public class IPCMessage
    {
        public ulong To { get; set; }
        public int MessageType { get; set; }
        public string ParsedBody { get; set; }
    }

    public class IPC_MessageBase
    {

    }

    public enum IPCMessageType : int
    {
        // Users
        IPC_User,
        IPC_AddOrUpdateUser,
        IPC_AvatarRequest,
        IPC_AvatarResponse,
        IPC_UserDataUpdated,
        IPC_P2PPacket,

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

        // Gamecoordinator
        IPC_GCMessageRequest,
        IPC_GCMessageResponse,
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

    public class IPC_User : IPC_MessageBase
    {
        public string PersonaName { get; set; }
        public string Language { get; set; }
        public uint AccountID { get; set; }
        public uint AppID { get; set; }
        public uint GameServerID { get; set; }
    }

    public class IPC_AddOrUpdateUser : IPC_MessageBase
    {
        public string PersonaName { get; set; }
        public uint AccountID { get; set; }
        public uint AppID { get; set; }
        public string IPAddress { get; set; }
    }

    public class IPC_AvatarRequest : IPC_MessageBase
    {
        public uint AccountID { get; set; }
    }

    public class IPC_AvatarResponse : IPC_MessageBase
    {
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
        public string IPAddress { get; set; }
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
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Network.Packets
{
    public class NetworkMessage
    {
        public int MessageType { get; set; }
        public string ParsedBody { get; set; }
    }

    public class NET_Base
    {

    }

    public class NET_Announce : NET_Base
    {
        public string PersonaName { get; set; }
        public uint AccountID { get; set; }
        public uint AppID { get; set; }
    }

    public class NET_AvatarRequest : NET_Base
    {

    }

    public class NET_AvatarResponse : NET_Base
    {
        public uint AccountID { get; set; }
        public string HexAvatar { get; set; }
    }

    public class NET_P2PPacket : NET_Base
    {
        public uint AccountID { get; set; }
        public byte[] Buffer { get; set; }
        public uint Sender { get; set; }
        public int P2PSendType { get; set; }
        public int Channel { get; set; }
    }

    public class NET_UserDataUpdated : NET_Base
    {
        public uint AccountID { get; set; }
        public string PersonaName { get; set; }
        public uint LobbyID { get; set; }
    }

    public class NET_LobbyListRequest : NET_Base
    {
        public uint AppID { get; set; }
        public uint RequestID { get; set; }
    }

    public class NET_LobbyListResponse : NET_Base
    {
        public string SerializedLobby { get; set; }
    }

    public class NET_LobbyJoinRequest : NET_Base
    {
        public ulong SteamID { get; set; }
        public ulong LobbyID { get; set; }
        public ulong CallbackHandle { get; set; }
    }

    public class NET_LobbyJoinResponse : NET_Base
    {
        public uint ChatRoomEnterResponse { get; set; }
        public ulong CallbackHandle { get; set; }
        public string SerializedLobby { get; set; }
    }

    public class NET_LobbyDataUpdate : NET_Base
    {
        public ulong SteamIDLobby { get; set; }
        public ulong SteamIDMember { get; set; }
        public string ParsedLobby { get; set; }
    }

    public class NET_LobbyChatUpdate : NET_Base
    {
        public ulong SteamIDLobby { get; set; }
        public ulong SteamIDUserChanged { get; set; }
        public ulong SteamIDMakingChange { get; set; }
        public uint  ChatMemberStateChange { get; set; }
    }

    public class NET_LobbyLeave : NET_Base
    {
        public ulong LobbyID { get; set; }
        public ulong SteamID { get; set; }
    }

    public class NET_LobbyGameserver : NET_Base
    {
        public ulong LobbyID { get; set; }
        public ulong SteamID { get; set; }
        public uint IP { get; set; }
        public uint Port { get; set; }
    }

    public enum MessageType : int
    {
        NET_Announce,
        NET_AnnounceResponse,
        NET_AvatarRequest,
        NET_AvatarResponse,
        NET_UserDataUpdated,
        NET_P2PPacket,

        // Lobbies
        NET_LobbyListRequest,
        NET_LobbyListResponse,
        NET_LobbyJoinRequest,
        NET_LobbyJoinResponse,
        NET_LobbyDataUpdate,
        NET_LobbyChatUpdate,
        NET_LobbyLeave,
        NET_LobbyGameserver,

    }
}

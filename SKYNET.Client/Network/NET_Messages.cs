//namespace SKYNET.Network.Types
//{
//    public class NETMessage
//    {
//        public int MessageType { get; set; }
//        public string ParsedBody { get; set; }
//    }

//    public enum NETMessageType : int
//    {
//        NET_Announce,
//        NET_AnnounceResponse,
//        NET_UserDataUpdated,
//        NET_P2PPacket,
//        NET_SetRichPresence,
//        NET_GameOpened,
//        NET_ChatMessage,

//        // Lobbies
//        NET_LobbyCreated,
//        NET_LobbyLeave,
//        NET_LobbyRemove,
//        NET_LobbyUpdate,
//        NET_LobbyListRequest,
//        NET_LobbyListResponse,
//        NET_LobbyJoinRequest,
//        NET_LobbyJoinResponse,
//        NET_LobbyDataUpdate,
//        NET_LobbyChatUpdate,
//        NET_LobbyGameserver,
//    }

//    public class NET_Base
//    {

//    }

//    public class NET_Announce : NET_Base
//    {
//        public string PersonaName { get; set; }
//        public uint AccountID { get; set; }
//        public uint AppID { get; set; }
//    }

//    public class NET_AvatarRequest : NET_Base
//    {

//    }

//    public class NET_AvatarResponse : NET_Base
//    {
//        public uint AccountID { get; set; }
//        public string HexAvatar { get; set; }
//    }

//    public class NET_P2PPacket : NET_Base
//    {
//        public uint AccountID { get; set; }
//        public byte[] Buffer { get; set; }
//        public uint Sender { get; set; }
//        public int P2PSendType { get; set; }
//        public int Channel { get; set; }
//    }

//    public class NET_UserDataUpdated : NET_Base
//    {
//        public uint AccountID { get; set; }
//        public string PersonaName { get; set; }
//        public uint LobbyID { get; set; }
//    }

//    public class NET_LobbyListRequest : NET_Base
//    {
//        public uint AppID { get; set; }
//        public uint RequestID { get; set; }
//    }

//    public class NET_LobbyListResponse : NET_Base
//    {
//        public string SerializedLobby { get; set; }
//    }

//    public class NET_LobbyJoinRequest : NET_Base
//    {
//        public ulong SteamID { get; set; }
//        public ulong LobbyID { get; set; }
//        public ulong CallbackHandle { get; set; }
//    }

//    public class NET_LobbyJoinResponse : NET_Base
//    {
//        public uint ChatRoomEnterResponse { get; set; }
//        public ulong CallbackHandle { get; set; }
//        public string SerializedLobby { get; set; }
//    }

//    public class NET_LobbyDataUpdate : NET_Base
//    {
//        public ulong SteamIDLobby { get; set; }
//        public ulong SteamIDMember { get; set; }
//        public string SerializedLobby { get; set; }
//    }

//    public class NET_LobbyCreated : NET_Base
//    {
//        public ulong SteamID { get; set; }
//        public string SerializedLobby { get; set; }
//    }

//    public class NET_LobbyUpdate : NET_Base
//    {
//        public ulong SteamID { get; set; }
//        public string SerializedLobby { get; set; }
//    }

//    public class NET_LobbyChatUpdate : NET_Base
//    {
//        public ulong SteamIDLobby { get; set; }
//        public ulong SteamIDUserChanged { get; set; }
//        public ulong SteamIDMakingChange { get; set; }
//        public uint  ChatMemberStateChange { get; set; }
//    }

//    public class NET_LobbyLeave : NET_Base
//    {
//        public ulong LobbyID { get; set; }
//        public ulong SteamID { get; set; }
//    }

//    public class NET_LobbyRemove : NET_Base
//    {
//        public ulong LobbyID { get; set; }
//    }

//    public class NET_LobbyGameserver : NET_Base
//    {
//        public ulong LobbyID { get; set; }
//        public ulong SteamID { get; set; }
//        public uint IP { get; set; }
//        public uint Port { get; set; }
//    }

//    public class NET_GameOpened : NET_Base
//    {
//        public uint AppID { get; set; }
//        public uint AccountID { get; set; }
//        public string Name { get; set; }
//    }

//    public class NET_ChatMessage : NET_Base
//    {
//        public int ID { get; set; }
//        public uint AccountID { get; set; }
//        public string PersonaName { get; set; }
//        public string Message { get; set; }
//    }
//}

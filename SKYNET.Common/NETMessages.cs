using SKYNET.Types;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace SKYNET.Network.Types
{
    public class NETMessage
    {
        public ulong SteamID { get; set; }
        public NETMessageType MessageType { get; set; }
        public string Body { get; set; }

        public NETMessage()
        {
        }

        public NETMessage(NETMessageType msgType, NET_Base message)
        {
            MessageType = msgType;
            Body = message.Serialize();
        }

        public NETMessage(ulong steamID, NETMessageType msgType, NET_Base message)
        {
            MessageType = msgType;
            Body = message.Serialize();
            SteamID = steamID;
        }

        public T Deserialize<T>()
        {
            try
            {
                var js = new JavaScriptSerializer();
                js.MaxJsonLength = 500000000;
                return js.Deserialize<T>(Body);
            }
            catch
            {
                return default;
            }
        }
    }
    public interface INETMessages { }

    public class NET_Base { }

    public enum NETMessageType
    {
        NET_CreateAccountRequest,
        NET_CreateAccountResponse,
        NET_AuthRequest,
        NET_AuthResponse,
        NET_GameInfoRequest,
        NET_GameInfoResponse,
        NET_UserOnline,
        NET_UserOffline,
        NET_UserInfoRequest,
        NET_UserInfoResponse,
        NET_ChatMessage,
        NET_PrivateChatMessage,
        NET_UserUpdated,
        NET_UpdateAvatarRequest,
        NET_UpdateAvatarResponse,
        NET_GameOpened,
        NET_GameClosed,
        NET_P2PPacket,
        NET_UpdateUserRequest,
        NET_UpdateUserResponse,

        NET_GetRichPresence,
        NET_SetRichPresence,
        NET_RichPresenceUpdated,
        NET_UserDataUpdated,

        NET_LobbyCreated,
        NET_LobbyChatUpdate,
        NET_LobbyGameserver,
        NET_LobbyUpdate,
        NET_LobbyJoinRequest,
        NET_LobbyDataUpdate,
        NET_LobbyListRequest,
        NET_LobbyLeave,
        NET_LobbyRemove,

        // STATS
        NET_SetAchievement,
        NET_UpdateAchievement,
        NET_SetLeaderboard,
        NET_SetPlayerStat,
    }

    public class NET_LobbyLeave : NET_Base
    {
        public ulong LobbyID { get; set; }
        public ulong SteamID { get; set; }
    }

    public class NET_LobbyRemove : NET_Base
    {
        public ulong LobbyID { get; set; }
    }

    public class NET_LobbyListRequest : NET_Base
    {
        public uint AppID { get; set; }
        public uint RequestID { get; set; }
    }

    public class NET_LobbyDataUpdate : NET_Base
    {
        public ulong SteamIDLobby { get; set; }
        public ulong SteamIDMember { get; set; }
        public string SerializedLobby { get; set; }
    }

    public class NET_LobbyJoinRequest : NET_Base
    {
        public ulong SteamID { get; set; }
        public ulong LobbyID { get; set; }
        public ulong CallbackHandle { get; set; }
    }

    public class NET_LobbyUpdate : NET_Base
    {
        public ulong SteamID { get; set; }
        public string SerializedLobby { get; set; }
    }

    public class NET_LobbyCreated : NET_Base
    {
        public ulong SteamID { get; set; }
        public string SerializedLobby { get; set; }
    }

    public class NET_UserDataUpdated : NET_Base
    {
        public uint AccountID { get; set; }
        public string PersonaName { get; set; }
        public uint LobbyID { get; set; }
    }

    public class NET_LobbyGameserver : NET_Base
    {
        public ulong LobbyID { get; set; }
        public ulong SteamID { get; set; }
        public uint IP { get; set; }
        public uint Port { get; set; }
    }

    public class NET_LobbyChatUpdate : NET_Base
    {
        public ulong SteamIDLobby { get; set; }
        public ulong SteamIDUserChanged { get; set; }
        public ulong SteamIDMakingChange { get; set; }
        public uint ChatMemberStateChange { get; set; }
    }

    public class NET_GetRichPresence : NET_Base
    {
        public uint AccountID { get; set; }
    }

    public class NET_SetRichPresence : NET_Base
    {
        public uint AccountID { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class NET_RichPresenceUpdated : NET_Base
    {
        public uint AccountID { get; set; }
        public List<RichPresence> RichPresence { get; set; }
    }

    public class NET_AuthRequest : NET_Base
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class NET_AuthResponse : NET_Base
    {
        public AuthResponseType Response { get; set; }
        public uint AccountID { get; set; }
        public string AccountName { get; set; }
        public string PersonaName { get; set; }
        public double Wallet { get; set; }
        public enum AuthResponseType : int
        {
            UnknownError = 0,
            Success = 1,
            AccountNotFound = 2,
            PasswordWrong = 3,
            AlreadyConnected = 4,
        }
    }

    public class NET_ChatMessage : NET_Base
    {
        public int ID { get; set; }
        public uint SenderAccountID { get; set; }
        public string PersonaName { get; set; }
        public string Message { get; set; }
    }

    public class NET_PrivateChatMessage : NET_Base
    {
        public uint SenderAccountID { get; set; }
        public uint TargetAccountID { get; set; }
        public uint Message { get; set; }
    }

    public class NET_UserOnline : NET_Base
    {
        public uint AccountID { get; set; }
        public string PersonaName { get; set; }
        public string AvatarURL { get; set; }
    }

    public class NET_UserOffline : NET_Base
    {
        public uint AccountID { get; set; }
    }

    public class NET_GameInfoRequest : NET_Base
    {
        public string Guid { get; set; }
    }

    public class NET_GameInfoResponse : NET_Base
    {
        public bool Playing { get; set; }
        public string LastPlayed { get; set; }
        public long TimePlayed { get; set; }
        public uint UsersPlaying { get; set; }
        public List<FriendPlaying> FriendsPlaying { get; set; }
        public string Header_Image { get; set; }
        public string LibraryHero_Image { get; set; }

        public class FriendPlaying
        {
            public uint AccountID { get; set; }
            public string PersonaName { get; set; }
            public string AvatarURL { get; set; }
        }
    }

    public class NET_CreateAccountRequest : NET_Base
    {
        public string AccountName { get; set; }
        public string Password { get; set; }
    }

    public class NET_CreateAccountResponse : NET_Base
    {
        public Result CreateAccountResult { get; set; }
        public string AccountName { get; set; }
        public uint AccountID { get; set; }
        public ulong SteamID { get; set; }
        public enum Result
        {
            ERROR,
            SUCCESS,
            ACCOUNTEXISTS
        }
    }

    public class NET_UserInfoRequest : NET_Base
    {
        public uint AccountID { get; set; }
    }

    public class NET_UserInfoResponse : NET_Base
    {
        public UserInfoResponseType Response { get; set; }
        public uint AccountID { get; set; }
        public string PersonaName { get; set; }
        public uint Playing { get; set; }
        public uint LastLogon { get; set; }

        public enum UserInfoResponseType
        {
            UnknownError = 0,
            Success = 1,
            AccountNotFound = 2,
        }
    }

    public class NET_UpdateUserRequest : NET_Base
    {
        public string PersonaName { get; set; }
        public string AvatarBase64 { get; set; }
    }

    public class NET_UpdateUserResponse : NET_Base
    {
        public string PersonaName { get; set; }
        public string AvatarName { get; set; }
    }

    public class NET_UpdateAvatarRequest : NET_Base
    {
        public string AvatarBase64 { get; set; }
    }

    public class NET_UpdateAvatarResponse : NET_Base
    {
        public string AvatarBase64 { get; set; }
    }

    public class NET_GameOpened : NET_Base
    {
        public uint AccountID { get; set; }
        public string Name { get; set; }
        public uint AppID { get; set; }
    }

    public class NET_GameClosed : NET_Base
    {
        public uint AccountID { get; set; }
        public string Name { get; set; }
        public uint AppID { get; set; }
    }

    public class NET_P2PPacket : NET_Base
    {
        public uint Sender { get; set; }
        public uint AccountID { get; set; }
        public byte[] Buffer { get; set; }
        public int Channel { get; set; }
        public int P2PSendType { get; set; }
    }

    public class NET_SetAchievement : NET_Base
    {
        public Achievement Achievement { get; set; }
    }

    public class NET_SetLeaderboard : NET_Base
    {
        public Leaderboard Leaderboard { get; set; }
    }

    public class NET_SetPlayerStat : NET_Base
    {
        public PlayerStat PlayerStat { get; set; }
    }

    public class NET_UpdateAchievement : NET_Base
    {
        public Achievement Achievement { get; set; }
    }

    
}

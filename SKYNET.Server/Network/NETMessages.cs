using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace SKYNET.Network
{
    public class NETMessage
    {
        public ulong SteamId { get; set; }
        public NETMessageType MessageType { get; set; }
        public string Body { get; set; }
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
        public string PersonaName { get; set; }
        public string Language { get; set; }
        public double Wallet { get; set; }
        public int DeviceInSelected { get; set; }
        public bool AllowRemoteAccess { get; set; }
        public bool ShowDebugConsole { get; set; }
        public enum AuthResponseType : int
        {
            UnknownError = 0,
            Success = 1,
            AccountNotFound = 2,
            PasswordWrong = 3,
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
        public string AvatarURL { get; set; }

        public enum UserInfoResponseType
        {
            UnknownError = 0,
            Success = 1,
            AccountNotFound = 2,
        }
    }

    public class NET_UpdateUser : NET_Base
    {
        public uint AccountID { get; set; }
        public string PersonaName { get; set; }
        public string Language { get; set; }
        public string AvatarBase64 { get; set; }
        public int DeviceInSelected { get; set; }
        public bool AllowRemoteAccess { get; set; }
        public bool ShowDebugConsole { get; set; }
    }

    public class NET_UserUpdated : NET_Base
    {
        public uint AccountID { get; set; }
        public string PersonaName { get; set; }
        public string Language { get; set; }
        public string AvatarURL { get; set; }
        public int DeviceInSelected { get; set; }
        public bool AllowRemoteAccess { get; set; }
        public bool ShowDebugConsole { get; set; }
    }

    public class NET_UpdateAvatar : NET_Base
    {
        public string AvatarBase64 { get; set; }
    }

    public class NET_AvatarUpdated : NET_Base
    {
        public string AvatarURL { get; set; }
    }
}

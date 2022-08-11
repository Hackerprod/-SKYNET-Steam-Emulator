using SKYNET.Types;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace SKYNET.WEB.Types
{
    public class WEBMessage
    {
        public WEBMessageType MessageType { get; set; }
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

    public class WEB_Base
    {
    }

    public enum WEBMessageType : int
    {
        WEB_CreateAccountRequest,
        WEB_CreateAccountResponse,
        WEB_AuthRequest,
        WEB_AuthResponse,
        WEB_GameListRequest,
        WEB_GameListResponse,
        WEB_GameAdded,
        WEB_GameUpdated,
        WEB_GameRemoved,
        WEB_GameOrderUpdated,
        WEB_GameLaunch,
        WEB_GameLaunched,
        WEB_GameStop,
        WEB_GameStopped,
        WEB_GameInfoRequest,
        WEB_GameInfoResponse,
        WEB_GameOpenContainerFolder,
        WEB_GameOpenWithoutEmulation,
        WEB_GameDownloadCache,
        WEB_GameCacheDownloadProgress,
        WEB_GameCacheDownloadProgressCompleted,
        WEB_UserOnline,
        WEB_UserOffline,
        WEB_UserInfoRequest,
        WEB_UserInfoResponse,
        WEB_ChatMessage,
        WEB_PrivateChatMessage,
        WEB_FileInfoRequest,
        WEB_FileInfoResponse,
        WEB_OpenFileDialogRequest,
        WEB_OpenFileDialogResponse,
        WEB_UpdateUser,
        WEB_UserUpdated,
        WEB_UpdateAvatar,
        WEB_AvatarUpdated,
        WEB_ConsoleMessage,
        WEB_UserLogOff,
        WEB_DeviceInRequest,
        WEB_DeviceInResponse,
        WEB_DeviceInSelected,
        WEB_LoadCompleted
    }

    public class WEB_LoadCompleted : WEB_Base
    {
    }

    public class WEB_DeviceInRequest : WEB_Base
    {
    }

    public class WEB_DeviceInResponse : WEB_Base
    {
        public List<Device> Devices { get; set; }

        public class Device
        {
            public int Index { get; set; }
            public string Name { get; set; }
        }
    }

    public class WEB_DeviceInSelected : WEB_Base
    {
        public int Index { get; set; }
        public string Name { get; set; }
    }

    public class WEB_ChatMessage : WEB_Base
    {
        public int ID { get; set; }
        public uint SenderAccountID { get; set; }
        public string PersonaName { get; set; }
        public string Message { get; set; }
    }

    public class WEB_PrivateChatMessage : WEB_Base
    {
        public uint SenderAccountID { get; set; }
        public uint TargetAccountID { get; set; }
        public uint Message { get; set; }
    }

    public class WEB_UserOnline : WEB_Base
    {
        public uint AccountID { get; set; }
        public string PersonaName { get; set; }
        public string AvatarURL { get; set; }
    }

    public class WEB_UserOffline : WEB_Base
    {
        public uint AccountID { get; set; }
    }

    public class WEB_GameInfoRequest : WEB_Base
    {
        public string Guid { get; set; }
    }

    public class WEB_GameInfoResponse : WEB_Base
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

    public class WEB_GameInfoMinimalResponse : WEB_Base
    {
        public bool Playing { get; set; }
        public uint LastPlayed { get; set; }
        public uint TimePlayed { get; set; }
        public uint UsersPlaying { get; set; }
    }

    public class WEB_CreateAccountRequest : WEB_Base
    {
        public string AccountName { get; set; }
        public string Password { get; set; }
    }

    public class WEB_CreateAccountResponse : WEB_Base
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

    public class WEB_GameAdded : WEB_Base
    {
        public Game Game { get; set; }
    }

    public class WEB_GameUpdated : WEB_Base
    {
        public Game Game { get; set; }
    }

    public class WEB_GameRemoved : WEB_Base
    {
        public string Guid { get; set; }
    }

    public class WEB_GameLaunch : WEB_Base
    {
        public string Guid { get; set; }
    }

    public class WEB_GameLaunched : WEB_Base
    {
        public string Guid { get; set; }
    }

    public class WEB_GameStop : WEB_Base
    {
        public string Guid { get; set; }
    }

    public class WEB_GameStopped : WEB_Base
    {
        public string Guid { get; set; }
    }

    public class WEB_AuthRequest : WEB_Base
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class WEB_AuthResponse : WEB_Base
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

    public class WEB_UserInfoRequest : WEB_Base
    {
        public uint AccountID { get; set; }
    }

    public class WEB_UserInfoResponse : WEB_Base
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

    public class WEB_GameListRequest : WEB_Base
    {
    }

    public class WEB_GameListResponse : WEB_Base
    {
        public List<Game> GameList { get; set; }
    }

    public class WEB_GameOpenContainerFolder : WEB_Base
    {
        public string Guid { get; set; }
    }

    public class WEB_GameOpenWithoutEmulation : WEB_Base
    {
        public string Guid { get; set; }
    }

    public class WEB_GameDownloadCache : WEB_Base
    {
        public string Guid { get; set; }
    }

    public class WEB_FileInfoRequest : WEB_Base
    {
        public string FilePath { get; set; }
    }

    public class WEB_FileInfoResponse : WEB_Base
    {
        public string FilePath { get; set; }
        public long Size { get; set; }
        public string ImageHex { get; set; }
        public int AppID { get; set; }
    }

    public class WEB_OpenFileDialogResponse : WEB_Base
    {
        public string FilePath { get; set; }
        public long Size { get; set; }
        public string ImageHex { get; set; }
        public int AppID { get; set; }
    }

    public class WEB_GameOrderUpdated : WEB_Base
    {
        public List<string> GameOrder { get; set; }
    }

    public class WEB_GameCacheDownloadProgress : WEB_Base
    {
        public int DownloadID { get; set; }
        public int Value { get; set; }
        public string Info { get; set; }
    }

    public class WEB_GameCacheDownloadProgressCompleted : WEB_Base
    {
        public int DownloadID { get; set; }
    }

    public class WEB_UpdateUser : WEB_Base
    {
        public uint AccountID { get; set; }
        public string PersonaName { get; set; }
        public string Language { get; set; }
        public string AvatarBase64 { get; set; }
        public int DeviceInSelected { get; set; }
        public bool AllowRemoteAccess { get; set; }
        public bool ShowDebugConsole { get; set; }
    }

    public class WEB_UserUpdated : WEB_Base
    {
        public uint AccountID { get; set; }
        public string PersonaName { get; set; }
        public string Language { get; set; }
        public string AvatarURL { get; set; }
        public int DeviceInSelected { get; set; }
        public bool AllowRemoteAccess { get; set; }
        public bool ShowDebugConsole { get; set; }
    }

    public class WEB_UpdateAvatar : WEB_Base
    {
        public string AvatarBase64 { get; set; }
    }

    public class WEB_AvatarUpdated : WEB_Base
    {
        public string AvatarURL { get; set; }
    }

    public class WEB_ConsoleMessage : WEB_Base
    {
        public string Sender { get; set; }
        public string Message { get; set; }
    }
}

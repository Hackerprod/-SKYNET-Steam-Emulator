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
        WEB_GameStoped,
        WEB_GameInfoRequest,
        WEB_GameInfoResponse,
        WEB_GameInfoMinimalResponse,
        WEB_GameOpenContainerFolder,
        WEB_GameOpenWithoutEmulation,
        WEB_GameDownloadCache,
        WEB_GameCacheDownloadProgress,
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
    }

    public class WEB_ChatMessage : WEB_Base
    {
        public uint SenderAccountID { get; set; }
        public uint Message { get; set; }
    }

    public class WEB_PrivateChatMessage : WEB_Base
    {
        public uint SenderAccountID { get; set; }
        public uint TargetAccountID { get; set; }
        public uint Message { get; set; }
    }

    public class WEB_UserOnline : WEB_Base
    {
        public UserInfo User { get; set; }
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
        public uint LastPlayed { get; set; }
        public uint TimePlayed { get; set; }
        public uint UsersPlaying { get; set; }
        public List<FriendPlaying> FriendsPlaying { get; set; }

        public class FriendPlaying
        {
            public uint AccountID { get; set; }
            public string PersonaName { get; set; }
            public string AvatarHex { get; set; }
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

    public class WEB_GameStoped : WEB_Base
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
        public WEB_AuthResponseType Response { get; set; }
        public UserInfo UserInfo { get; set; }
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
        public Dictionary<int, string> GameOrder { get; set; }
    }

    public class WEB_GameCacheDownloadProgress : WEB_Base
    {
        public int DownloadID { get; set; }
        public int Value { get; set; }
        public string Info { get; set; }
    }

    public enum WEB_AuthResponseType : int
    {
        UnknownError    = 0,
        Success         = 1,
        AccountNotFound = 2,
        PasswordWrong   = 3,
    }

    public class UserInfo
    {
        public uint AccountID { get; set; }
        public ulong SteamID { get; set; }
        public string PersonaName { get; set; }
        public string Language { get; set; }
        public string AvatarHex { get; set; }
    }
}

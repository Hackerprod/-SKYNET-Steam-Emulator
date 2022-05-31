using SKYNET.Helper;
using SKYNET.Steamworks;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using PublishedFileId_t = System.UInt64;
using SteamAPICall_t = System.UInt64;

namespace SKYNET.Callback
{
    /// <summary>
    /// Gives us a generic way to get the CallbackId of structs
    /// </summary>
    public interface ICallbackData
    {
        CallbackType CallbackType { get; }
        int DataSize { get; }
    }

    public class CallbackMessage
    {
        public ICallbackData Data { get; set; }
        public bool Called { get; set; }
        public bool ReadyToCall { get; set; }
        public DateTime Time { get; set; }
        public int TimeSeconds => (DateTime.Now - Time).Seconds;
        public bool CallComplete { get; set; }

        public CallbackMessage(ICallbackData data, bool readyToCall = true, bool callComplete = false)
        {
            Data = data;
            ReadyToCall = readyToCall;
            Time = DateTime.Now;
            CallComplete = callComplete;
        }

        public bool TimedOut()
        {
            return (DateTime.Now - Time).Seconds > 20;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SteamServersConnected_t : ICallbackData
    {
        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamServersConnected_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamServersConnected;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SteamServerConnectFailure_t : ICallbackData
    {
        public EResult Result; // m_eResult EResult
        [MarshalAs(UnmanagedType.I1)]
        public bool StillRetrying; // m_bStillRetrying bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamServerConnectFailure_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamServerConnectFailure;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SteamServersDisconnected_t : ICallbackData
    {
        public EResult m_eResult; // m_eResult EResult

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamServersDisconnected_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamServersDisconnected;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct ClientGameServerDeny_t : ICallbackData
    {
        public uint m_uAppID; // m_uAppID uint32
        public uint m_unGameServerIP; // m_unGameServerIP uint32
        public ushort m_usGameServerPort; // m_usGameServerPort uint16
        public ushort m_bSecure; // m_bSecure uint16
        public uint m_uReason; // m_uReason uint32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(ClientGameServerDeny_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.ClientGameServerDeny;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct IPCFailure_t : ICallbackData
    {
        public byte m_eFailureType; // m_eFailureType uint8

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(IPCFailure_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.IPCFailure;
        #endregion
        public enum EFailureType : int
        {
            FlushedCallbackQueue = 0,
            PipeFail = 1,
        }

    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct LicensesUpdated_t : ICallbackData
    {

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(LicensesUpdated_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.LicensesUpdated;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct ValidateAuthTicketResponse_t : ICallbackData
    {
        public ulong m_SteamID; // m_SteamID CSteamID
        public EAuthSessionResponse m_eAuthSessionResponse; // m_eAuthSessionResponse EAuthSessionResponse
        public ulong m_OwnerSteamID; // m_OwnerSteamID CSteamID

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(ValidateAuthTicketResponse_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.ValidateAuthTicketResponse;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct MicroTxnAuthorizationResponse_t : ICallbackData
    {
        public uint m_unAppID; // m_unAppID uint32
        public ulong m_ulOrderID; // m_ulOrderID uint64
        public byte m_bAuthorized; // m_bAuthorized uint8

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(MicroTxnAuthorizationResponse_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.MicroTxnAuthorizationResponse;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct EncryptedAppTicketResponse_t : ICallbackData
    {
        public EResult m_eResult; // m_eResult EResult

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(EncryptedAppTicketResponse_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.EncryptedAppTicketResponse;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct GetAuthSessionTicketResponse_t : ICallbackData
    {
        public uint AuthTicket; // m_hAuthTicket HAuthTicket
        public EResult Result; // m_eResult EResult

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GetAuthSessionTicketResponse_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GetAuthSessionTicketResponse;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct GameWebCallback_t : ICallbackData
    {
        public string URLUTF8() => System.Text.Encoding.UTF8.GetString(URL, 0, System.Array.IndexOf<byte>(URL, 0));
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] // byte[] m_szURL
        public byte[] URL; // m_szURL char [256]

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GameWebCallback_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GameWebCallback;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct StoreAuthURLResponse_t : ICallbackData
    {
        public string URLUTF8() => System.Text.Encoding.UTF8.GetString(URL, 0, System.Array.IndexOf<byte>(URL, 0));
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)] // byte[] m_szURL
        public byte[] URL; // m_szURL char [512]

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(StoreAuthURLResponse_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.StoreAuthURLResponse;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct MarketEligibilityResponse_t : ICallbackData
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool Allowed; // m_bAllowed bool
        public MarketNotAllowedReasonFlags NotAllowedReason; // m_eNotAllowedReason EMarketNotAllowedReasonFlags
        public uint TAllowedAtTime; // m_rtAllowedAtTime RTime32
        public int CdaySteamGuardRequiredDays; // m_cdaySteamGuardRequiredDays int
        public int CdayNewDeviceCooldown; // m_cdayNewDeviceCooldown int

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(MarketEligibilityResponse_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.MarketEligibilityResponse;
        #endregion
    }

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct DurationControl_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public AppId Appid; // m_appid AppId_t
    //    [MarshalAs(UnmanagedType.I1)]
    //    public bool Applicable; // m_bApplicable bool
    //    public int CsecsLast5h; // m_csecsLast5h int32
    //    public DurationControlProgress Progress; // m_progress EDurationControlProgress
    //    public DurationControlNotification Otification; // m_notification EDurationControlNotification
    //    public int CsecsToday; // m_csecsToday int32
    //    public int CsecsRemaining; // m_csecsRemaining int32

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(DurationControl_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.DurationControl;
    //    #endregion
    //}

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct PersonaStateChange_t : ICallbackData
    {
        public ulong m_ulSteamID; // m_ulSteamID uint64
        public int m_nChangeFlags; // m_nChangeFlags int

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(PersonaStateChange_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.PersonaStateChange;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct GameOverlayActivated_t : ICallbackData
    {
        public byte Active; // m_bActive uint8

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GameOverlayActivated_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GameOverlayActivated;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct GameServerChangeRequested_t : ICallbackData
    {
        //public string ServerUTF8() => System.Text.Encoding.UTF8.GetString(m_rgchServer, 0, System.Array.IndexOf<byte>(m_rgchServer, 0));
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] // byte[] m_rgchServer
        public string m_rgchServer; // m_rgchServer char [64]
        //public string PasswordUTF8() => System.Text.Encoding.UTF8.GetString(m_rgchPassword, 0, System.Array.IndexOf<byte>(m_rgchPassword, 0));
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)] // byte[] m_rgchPassword
        public string m_rgchPassword; // m_rgchPassword char [64]

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GameServerChangeRequested_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GameServerChangeRequested;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct GameLobbyJoinRequested_t : ICallbackData
    {
        public ulong m_steamIDLobby; // m_steamIDLobby CSteamID
        public ulong m_steamIDFriend; // m_steamIDFriend CSteamID

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GameLobbyJoinRequested_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GameLobbyJoinRequested;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct AvatarImageLoaded_t : ICallbackData
    {
        public ulong SteamID; // m_steamID CSteamID
        public int Image; // m_iImage int
        public int Wide; // m_iWide int
        public int Tall; // m_iTall int

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(AvatarImageLoaded_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.AvatarImageLoaded;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct ClanOfficerListResponse_t : ICallbackData
    {
        public ulong SteamIDClan; // m_steamIDClan CSteamID
        public int COfficers; // m_cOfficers int
        public byte Success; // m_bSuccess uint8

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(ClanOfficerListResponse_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.ClanOfficerListResponse;
        #endregion
    }

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct FriendRichPresenceUpdate_t : ICallbackData
    //{
    //    public ulong SteamIDFriend; // m_steamIDFriend CSteamID
    //    public AppId AppID; // m_nAppID AppId_t

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(FriendRichPresenceUpdate_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.FriendRichPresenceUpdate;
    //    #endregion
    //}

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct GameRichPresenceJoinRequested_t : ICallbackData
    {
        public ulong SteamIDFriend; // m_steamIDFriend CSteamID
        public string ConnectUTF8() => System.Text.Encoding.UTF8.GetString(Connect, 0, System.Array.IndexOf<byte>(Connect, 0));
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] // byte[] m_rgchConnect
        public byte[] Connect; // m_rgchConnect char [256]

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GameRichPresenceJoinRequested_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GameRichPresenceJoinRequested;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct GameConnectedClanChatMsg_t : ICallbackData
    {
        public ulong SteamIDClanChat; // m_steamIDClanChat CSteamID
        public ulong SteamIDUser; // m_steamIDUser CSteamID
        public int MessageID; // m_iMessageID int

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GameConnectedClanChatMsg_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GameConnectedClanChatMsg;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct GameConnectedChatJoin_t : ICallbackData
    {
        public ulong SteamIDClanChat; // m_steamIDClanChat CSteamID
        public ulong SteamIDUser; // m_steamIDUser CSteamID

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GameConnectedChatJoin_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GameConnectedChatJoin;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct GameConnectedChatLeave_t : ICallbackData
    {
        public ulong SteamIDClanChat; // m_steamIDClanChat CSteamID
        public ulong SteamIDUser; // m_steamIDUser CSteamID
        [MarshalAs(UnmanagedType.I1)]
        public bool Kicked; // m_bKicked bool
        [MarshalAs(UnmanagedType.I1)]
        public bool Dropped; // m_bDropped bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GameConnectedChatLeave_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GameConnectedChatLeave;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct DownloadClanActivityCountsResult_t : ICallbackData
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool Success; // m_bSuccess bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(DownloadClanActivityCountsResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.DownloadClanActivityCountsResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct JoinClanChatRoomCompletionResult_t : ICallbackData
    {
        public ulong SteamIDClanChat; // m_steamIDClanChat CSteamID
        public EChatRoomEnterResponse ChatRoomEnterResponse; // m_eChatRoomEnterResponse EChatRoomEnterResponse

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(JoinClanChatRoomCompletionResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.JoinClanChatRoomCompletionResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct GameConnectedFriendChatMsg_t : ICallbackData
    {
        public ulong SteamIDUser; // m_steamIDUser CSteamID
        public int MessageID; // m_iMessageID int

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GameConnectedFriendChatMsg_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GameConnectedFriendChatMsg;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct FriendsGetFollowerCount_t : ICallbackData
    {
        public EResult Result; // m_eResult EResult
        public ulong SteamID; // m_steamID CSteamID
        public int Count; // m_nCount int

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(FriendsGetFollowerCount_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.FriendsGetFollowerCount;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct FriendsIsFollowing_t : ICallbackData
    {
        public EResult Result; // m_eResult EResult
        public ulong SteamID; // m_steamID CSteamID
        [MarshalAs(UnmanagedType.I1)]
        public bool IsFollowing; // m_bIsFollowing bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(FriendsIsFollowing_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.FriendsIsFollowing;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct FriendsEnumerateFollowingList_t : ICallbackData
    {
        public EResult Result; // m_eResult EResult
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.U8)]
        public ulong[] GSteamID; // m_rgSteamID CSteamID [50]
        public int ResultsReturned; // m_nResultsReturned int32
        public int TotalResultCount; // m_nTotalResultCount int32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(FriendsEnumerateFollowingList_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.FriendsEnumerateFollowingList;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SetPersonaNameResponse_t : ICallbackData
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bSuccess; // m_bSuccess bool
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bLocalSuccess; // m_bLocalSuccess bool
        public EResult m_result; // m_result EResult

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SetPersonaNameResponse_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SetPersonaNameResponse;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct UnreadChatMessagesChanged_t : ICallbackData
    {

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(UnreadChatMessagesChanged_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.UnreadChatMessagesChanged;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct OverlayBrowserProtocolNavigation_t : ICallbackData
    {
        public string RgchURIUTF8() => System.Text.Encoding.UTF8.GetString(RgchURI, 0, System.Array.IndexOf<byte>(RgchURI, 0));
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)] // byte[] rgchURI
        public byte[] RgchURI; // rgchURI char [1024]

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(OverlayBrowserProtocolNavigation_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.OverlayBrowserProtocolNavigation;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct IPCountry_t : ICallbackData
    {

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(IPCountry_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.IPCountry;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct LowBatteryPower_t : ICallbackData
    {
        public byte MinutesBatteryLeft; // m_nMinutesBatteryLeft uint8

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(LowBatteryPower_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.LowBatteryPower;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SteamAPICallCompleted_t : ICallbackData
    {
        public SteamAPICall_t m_hAsyncCall; // m_hAsyncCall SteamAPICall_t
        public int m_iCallback; // m_iCallback int
        public UInt32 m_cubParam; // m_cubParam uint32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamAPICallCompleted_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamAPICallCompleted;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SteamShutdown_t : ICallbackData
    {

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamShutdown_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamShutdown;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct CheckFileSignature_t : ICallbackData
    {
        public CheckFileSignature CheckFileSignature; // m_eCheckFileSignature ECheckFileSignature

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(CheckFileSignature_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.CheckFileSignature;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct GamepadTextInputDismissed_t : ICallbackData
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool Submitted; // m_bSubmitted bool
        public uint SubmittedText; // m_unSubmittedText uint32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GamepadTextInputDismissed_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GamepadTextInputDismissed;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct AppResumingFromSuspend_t : ICallbackData
    {

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(AppResumingFromSuspend_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.AppResumingFromSuspend;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct FloatingGamepadTextInputDismissed_t : ICallbackData
    {

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(FloatingGamepadTextInputDismissed_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.FloatingGamepadTextInputDismissed;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct FavoritesListChanged_t : ICallbackData
    {
        public uint IP; // m_nIP uint32
        public uint QueryPort; // m_nQueryPort uint32
        public uint ConnPort; // m_nConnPort uint32
        public uint AppID; // m_nAppID uint32
        public uint Flags; // m_nFlags uint32
        [MarshalAs(UnmanagedType.I1)]
        public bool Add; // m_bAdd bool
        public uint AccountId; // m_unAccountId AccountID_t

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(FavoritesListChanged_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.FavoritesListChanged;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct LobbyInvite_t : ICallbackData
    {
        public ulong SteamIDUser; // m_ulSteamIDUser uint64
        public ulong SteamIDLobby; // m_ulSteamIDLobby uint64
        public ulong GameID; // m_ulGameID uint64

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(LobbyInvite_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.LobbyInvite;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct LobbyEnter_t : ICallbackData
    {
        public ulong m_ulSteamIDLobby; // m_ulSteamIDLobby uint64
        public uint m_rgfChatPermissions; // m_rgfChatPermissions uint32
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bLocked; // m_bLocked bool
        public uint m_EChatRoomEnterResponse; // m_EChatRoomEnterResponse uint32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(LobbyEnter_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.LobbyEnter;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct LobbyDataUpdate_t : ICallbackData
    {
        public ulong m_ulSteamIDLobby; // m_ulSteamIDLobby uint64
        public ulong m_ulSteamIDMember; // m_ulSteamIDMember uint64
        public bool m_bSuccess; // m_bSuccess uint8

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(LobbyDataUpdate_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.LobbyDataUpdate;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct LobbyChatUpdate_t : ICallbackData
    {
        public ulong m_ulSteamIDLobby; // m_ulSteamIDLobby uint64
        public ulong m_ulSteamIDUserChanged; // m_ulSteamIDUserChanged uint64
        public ulong m_ulSteamIDMakingChange; // m_ulSteamIDMakingChange uint64
        public uint m_rgfChatMemberStateChange; // m_rgfChatMemberStateChange uint32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(LobbyChatUpdate_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.LobbyChatUpdate;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct LobbyChatMsg_t : ICallbackData
    {
        public ulong SteamIDLobby; // m_ulSteamIDLobby uint64
        public ulong SteamIDUser; // m_ulSteamIDUser uint64
        public byte ChatEntryType; // m_eChatEntryType uint8
        public uint ChatID; // m_iChatID uint32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(LobbyChatMsg_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.LobbyChatMsg;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct LobbyGameCreated_t : ICallbackData
    {
        public ulong m_ulSteamIDLobby; // m_ulSteamIDLobby uint64
        public ulong m_ulSteamIDGameServer; // m_ulSteamIDGameServer uint64
        public uint m_unIP; // m_unIP uint32
        public ushort m_usPort; // m_usPort uint16

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(LobbyGameCreated_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.LobbyGameCreated;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct LobbyMatchList_t : ICallbackData
    {
        public uint m_nLobbiesMatching; // m_nLobbiesMatching uint32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(LobbyMatchList_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.LobbyMatchList;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct LobbyKicked_t : ICallbackData
    {
        public ulong SteamIDLobby; // m_ulSteamIDLobby uint64
        public ulong SteamIDAdmin; // m_ulSteamIDAdmin uint64
        public byte KickedDueToDisconnect; // m_bKickedDueToDisconnect uint8

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(LobbyKicked_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.LobbyKicked;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct LobbyCreated_t : ICallbackData
    {
        public EResult m_eResult; // m_eResult EResult
        public ulong m_ulSteamIDLobby; // m_ulSteamIDLobby uint64

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(LobbyCreated_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.LobbyCreated;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct PSNGameBootInviteResult_t : ICallbackData
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool GameBootInviteExists; // m_bGameBootInviteExists bool
        public ulong SteamIDLobby; // m_steamIDLobby CSteamID

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(PSNGameBootInviteResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.PSNGameBootInviteResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct FavoritesListAccountsUpdated_t : ICallbackData
    {
        public EResult Result; // m_eResult EResult

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(FavoritesListAccountsUpdated_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.FavoritesListAccountsUpdated;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct SearchForGameProgressCallback_t : ICallbackData
    {
        public ulong LSearchID; // m_ullSearchID uint64
        public EResult Result; // m_eResult EResult
        public ulong LobbyID; // m_lobbyID CSteamID
        public ulong SteamIDEndedSearch; // m_steamIDEndedSearch CSteamID
        public int SecondsRemainingEstimate; // m_nSecondsRemainingEstimate int32
        public int CPlayersSearching; // m_cPlayersSearching int32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SearchForGameProgressCallback_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SearchForGameProgressCallback;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct SearchForGameResultCallback_t : ICallbackData
    {
        public ulong LSearchID; // m_ullSearchID uint64
        public EResult Result; // m_eResult EResult
        public int CountPlayersInGame; // m_nCountPlayersInGame int32
        public int CountAcceptedGame; // m_nCountAcceptedGame int32
        public ulong SteamIDHost; // m_steamIDHost CSteamID
        [MarshalAs(UnmanagedType.I1)]
        public bool FinalCallback; // m_bFinalCallback bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SearchForGameResultCallback_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SearchForGameResultCallback;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RequestPlayersForGameProgressCallback_t : ICallbackData
    {
        public EResult Result; // m_eResult EResult
        public ulong LSearchID; // m_ullSearchID uint64

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RequestPlayersForGameProgressCallback_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RequestPlayersForGameProgressCallback;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct RequestPlayersForGameResultCallback_t : ICallbackData
    {
        public EResult Result; // m_eResult EResult
        public ulong LSearchID; // m_ullSearchID uint64
        public ulong SteamIDPlayerFound; // m_SteamIDPlayerFound CSteamID
        public ulong SteamIDLobby; // m_SteamIDLobby CSteamID
        public RequestPlayersForGameResultCallback_t.PlayerAcceptState_t PlayerAcceptState; // m_ePlayerAcceptState RequestPlayersForGameResultCallback_t::PlayerAcceptState_t
        public int PlayerIndex; // m_nPlayerIndex int32
        public int TotalPlayersFound; // m_nTotalPlayersFound int32
        public int TotalPlayersAcceptedGame; // m_nTotalPlayersAcceptedGame int32
        public int SuggestedTeamIndex; // m_nSuggestedTeamIndex int32
        public ulong LUniqueGameID; // m_ullUniqueGameID uint64

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RequestPlayersForGameResultCallback_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RequestPlayersForGameResultCallback;
        #endregion
        public enum PlayerAcceptState_t : int
        {
            Unknown = 0,
            PlayerAccepted = 1,
            PlayerDeclined = 2,
        }

    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RequestPlayersForGameFinalResultCallback_t : ICallbackData
    {
        public EResult Result; // m_eResult EResult
        public ulong LSearchID; // m_ullSearchID uint64
        public ulong LUniqueGameID; // m_ullUniqueGameID uint64

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RequestPlayersForGameFinalResultCallback_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RequestPlayersForGameFinalResultCallback;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct SubmitPlayerResultResultCallback_t : ICallbackData
    {
        public EResult Result; // m_eResult EResult
        public ulong UllUniqueGameID; // ullUniqueGameID uint64
        public ulong SteamIDPlayer; // steamIDPlayer CSteamID

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SubmitPlayerResultResultCallback_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SubmitPlayerResultResultCallback;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct EndGameResultCallback_t : ICallbackData
    {
        public EResult Result; // m_eResult EResult
        public ulong UllUniqueGameID; // ullUniqueGameID uint64

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(EndGameResultCallback_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.EndGameResultCallback;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct JoinPartyCallback_t : ICallbackData
    {
        public EResult Result; // m_eResult EResult
        public ulong BeaconID; // m_ulBeaconID PartyBeaconID_t
        public ulong SteamIDBeaconOwner; // m_SteamIDBeaconOwner CSteamID
        public string ConnectStringUTF8() => System.Text.Encoding.UTF8.GetString(ConnectString, 0, System.Array.IndexOf<byte>(ConnectString, 0));
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] // byte[] m_rgchConnectString
        public byte[] ConnectString; // m_rgchConnectString char [256]

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(JoinPartyCallback_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.JoinPartyCallback;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct CreateBeaconCallback_t : ICallbackData
    {
        public EResult Result; // m_eResult EResult
        public ulong BeaconID; // m_ulBeaconID PartyBeaconID_t

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(CreateBeaconCallback_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.CreateBeaconCallback;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct ReservationNotificationCallback_t : ICallbackData
    {
        public ulong BeaconID; // m_ulBeaconID PartyBeaconID_t
        public ulong SteamIDJoiner; // m_steamIDJoiner CSteamID

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(ReservationNotificationCallback_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.ReservationNotificationCallback;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct ChangeNumOpenSlotsCallback_t : ICallbackData
    {
        public EResult Result; // m_eResult EResult

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(ChangeNumOpenSlotsCallback_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.ChangeNumOpenSlotsCallback;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct AvailableBeaconLocationsUpdated_t : ICallbackData
    {

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(AvailableBeaconLocationsUpdated_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.AvailableBeaconLocationsUpdated;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct ActiveBeaconsUpdated_t : ICallbackData
    {

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(ActiveBeaconsUpdated_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.ActiveBeaconsUpdated;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStorageFileShareResult_t : ICallbackData
    {
        public EResult m_eResult; // m_eResult EResult
        public ulong m_hFile; // m_hFile UGCHandle_t
        public string FilenameUTF8() => System.Text.Encoding.UTF8.GetString(m_rgchFilename, 0, System.Array.IndexOf<byte>(m_rgchFilename, 0));
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)] // byte[] m_rgchFilename
        public byte[] m_rgchFilename; // m_rgchFilename char [260]

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageFileShareResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStorageFileShareResult;
        #endregion
    }

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct RemoteStoragePublishFileResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t
    //    [MarshalAs(UnmanagedType.I1)]
    //    public bool UserNeedsToAcceptWorkshopLegalAgreement; // m_bUserNeedsToAcceptWorkshopLegalAgreement bool

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(RemoteStoragePublishFileResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.RemoteStoragePublishFileResult;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct RemoteStorageDeletePublishedFileResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageDeletePublishedFileResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.RemoteStorageDeletePublishedFileResult;
    //    #endregion
    //}

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStorageEnumerateUserPublishedFilesResult_t : ICallbackData
    {
        public EResult m_eResult; // m_eResult EResult
        public int m_nResultsReturned; // m_nResultsReturned int32
        public int m_nTotalResultCount; // m_nTotalResultCount int32
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.U8)]
        public PublishedFileId_t[] m_rgPublishedFileId; // m_rgPublishedFileId PublishedFileId_t [50]

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageEnumerateUserPublishedFilesResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStorageEnumerateUserPublishedFilesResult;
        #endregion
    }

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct RemoteStorageSubscribePublishedFileResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageSubscribePublishedFileResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.RemoteStorageSubscribePublishedFileResult;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct RemoteStorageEnumerateUserSubscribedFilesResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public int ResultsReturned; // m_nResultsReturned int32
    //    public int TotalResultCount; // m_nTotalResultCount int32
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.U8)]
    //    public PublishedFileId[] GPublishedFileId; // m_rgPublishedFileId PublishedFileId_t [50]
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.U4)]
    //    public uint[] GRTimeSubscribed; // m_rgRTimeSubscribed uint32 [50]

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageEnumerateUserSubscribedFilesResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.RemoteStorageEnumerateUserSubscribedFilesResult;
    //    #endregion
    //}

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStorageUnsubscribePublishedFileResult_t : ICallbackData
    {
        public EResult m_eResult; // m_eResult EResult
        public PublishedFileId_t m_nPublishedFileId; // m_nPublishedFileId PublishedFileId_t

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageUnsubscribePublishedFileResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStorageUnsubscribePublishedFileResult;
        #endregion
    }

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct RemoteStorageUpdatePublishedFileResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t
    //    [MarshalAs(UnmanagedType.I1)]
    //    public bool UserNeedsToAcceptWorkshopLegalAgreement; // m_bUserNeedsToAcceptWorkshopLegalAgreement bool

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageUpdatePublishedFileResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.RemoteStorageUpdatePublishedFileResult;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct RemoteStorageDownloadUGCResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public ulong File; // m_hFile UGCHandle_t
    //    public AppId AppID; // m_nAppID AppId_t
    //    public int SizeInBytes; // m_nSizeInBytes int32
    //    public string PchFileNameUTF8() => System.Text.Encoding.UTF8.GetString(PchFileName, 0, System.Array.IndexOf<byte>(PchFileName, 0));
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)] // byte[] m_pchFileName
    //    public byte[] PchFileName; // m_pchFileName char [260]
    //    public ulong SteamIDOwner; // m_ulSteamIDOwner uint64

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageDownloadUGCResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.RemoteStorageDownloadUGCResult;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct RemoteStorageGetPublishedFileDetailsResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t
    //    public AppId CreatorAppID; // m_nCreatorAppID AppId_t
    //    public AppId ConsumerAppID; // m_nConsumerAppID AppId_t
    //    public string TitleUTF8() => System.Text.Encoding.UTF8.GetString(Title, 0, System.Array.IndexOf<byte>(Title, 0));
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 129)] // byte[] m_rgchTitle
    //    public byte[] Title; // m_rgchTitle char [129]
    //    public string DescriptionUTF8() => System.Text.Encoding.UTF8.GetString(Description, 0, System.Array.IndexOf<byte>(Description, 0));
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8000)] // byte[] m_rgchDescription
    //    public byte[] Description; // m_rgchDescription char [8000]
    //    public ulong File; // m_hFile UGCHandle_t
    //    public ulong PreviewFile; // m_hPreviewFile UGCHandle_t
    //    public ulong SteamIDOwner; // m_ulSteamIDOwner uint64
    //    public uint TimeCreated; // m_rtimeCreated uint32
    //    public uint TimeUpdated; // m_rtimeUpdated uint32
    //    public RemoteStoragePublishedFileVisibility Visibility; // m_eVisibility ERemoteStoragePublishedFileVisibility
    //    [MarshalAs(UnmanagedType.I1)]
    //    public bool Banned; // m_bBanned bool
    //    public string TagsUTF8() => System.Text.Encoding.UTF8.GetString(Tags, 0, System.Array.IndexOf<byte>(Tags, 0));
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1025)] // byte[] m_rgchTags
    //    public byte[] Tags; // m_rgchTags char [1025]
    //    [MarshalAs(UnmanagedType.I1)]
    //    public bool TagsTruncated; // m_bTagsTruncated bool
    //    public string PchFileNameUTF8() => System.Text.Encoding.UTF8.GetString(PchFileName, 0, System.Array.IndexOf<byte>(PchFileName, 0));
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)] // byte[] m_pchFileName
    //    public byte[] PchFileName; // m_pchFileName char [260]
    //    public int FileSize; // m_nFileSize int32
    //    public int PreviewFileSize; // m_nPreviewFileSize int32
    //    public string URLUTF8() => System.Text.Encoding.UTF8.GetString(URL, 0, System.Array.IndexOf<byte>(URL, 0));
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] // byte[] m_rgchURL
    //    public byte[] URL; // m_rgchURL char [256]
    //    public WorkshopFileType FileType; // m_eFileType EWorkshopFileType
    //    [MarshalAs(UnmanagedType.I1)]
    //    public bool AcceptedForUse; // m_bAcceptedForUse bool

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageGetPublishedFileDetailsResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.RemoteStorageGetPublishedFileDetailsResult;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct RemoteStorageEnumerateWorkshopFilesResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public int ResultsReturned; // m_nResultsReturned int32
    //    public int TotalResultCount; // m_nTotalResultCount int32
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.U8)]
    //    public PublishedFileId[] GPublishedFileId; // m_rgPublishedFileId PublishedFileId_t [50]
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.R4)]
    //    public float[] GScore; // m_rgScore float [50]
    //    public AppId AppId; // m_nAppId AppId_t
    //    public uint StartIndex; // m_unStartIndex uint32

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageEnumerateWorkshopFilesResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.RemoteStorageEnumerateWorkshopFilesResult;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct RemoteStorageGetPublishedItemVoteDetailsResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public PublishedFileId PublishedFileId; // m_unPublishedFileId PublishedFileId_t
    //    public int VotesFor; // m_nVotesFor int32
    //    public int VotesAgainst; // m_nVotesAgainst int32
    //    public int Reports; // m_nReports int32
    //    public float FScore; // m_fScore float

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageGetPublishedItemVoteDetailsResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.RemoteStorageGetPublishedItemVoteDetailsResult;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct RemoteStoragePublishedFileSubscribed_t : ICallbackData
    //{
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t
    //    public AppId AppID; // m_nAppID AppId_t

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(RemoteStoragePublishedFileSubscribed_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.RemoteStoragePublishedFileSubscribed;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct RemoteStoragePublishedFileUnsubscribed_t : ICallbackData
    //{
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t
    //    public AppId AppID; // m_nAppID AppId_t

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(RemoteStoragePublishedFileUnsubscribed_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.RemoteStoragePublishedFileUnsubscribed;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct RemoteStoragePublishedFileDeleted_t : ICallbackData
    //{
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t
    //    public AppId AppID; // m_nAppID AppId_t

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(RemoteStoragePublishedFileDeleted_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.RemoteStoragePublishedFileDeleted;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct RemoteStorageUpdateUserPublishedItemVoteResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageUpdateUserPublishedItemVoteResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.RemoteStorageUpdateUserPublishedItemVoteResult;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct RemoteStorageUserVoteDetails_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t
    //    public WorkshopVote Vote; // m_eVote EWorkshopVote

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageUserVoteDetails_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.RemoteStorageUserVoteDetails;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct RemoteStorageEnumerateUserSharedWorkshopFilesResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public int ResultsReturned; // m_nResultsReturned int32
    //    public int TotalResultCount; // m_nTotalResultCount int32
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.U8)]
    //    public PublishedFileId[] GPublishedFileId; // m_rgPublishedFileId PublishedFileId_t [50]

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageEnumerateUserSharedWorkshopFilesResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.RemoteStorageEnumerateUserSharedWorkshopFilesResult;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct RemoteStorageSetUserPublishedFileActionResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t
    //    public WorkshopFileAction Action; // m_eAction EWorkshopFileAction

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageSetUserPublishedFileActionResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.RemoteStorageSetUserPublishedFileActionResult;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct RemoteStorageEnumeratePublishedFilesByUserActionResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public WorkshopFileAction Action; // m_eAction EWorkshopFileAction
    //    public int ResultsReturned; // m_nResultsReturned int32
    //    public int TotalResultCount; // m_nTotalResultCount int32
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.U8)]
    //    public PublishedFileId[] GPublishedFileId; // m_rgPublishedFileId PublishedFileId_t [50]
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.U4)]
    //    public uint[] GRTimeUpdated; // m_rgRTimeUpdated uint32 [50]

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageEnumeratePublishedFilesByUserActionResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.RemoteStorageEnumeratePublishedFilesByUserActionResult;
    //    #endregion
    //}

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStoragePublishFileProgress_t : ICallbackData
    {
        public double DPercentFile; // m_dPercentFile double
        [MarshalAs(UnmanagedType.I1)]
        public bool Preview; // m_bPreview bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStoragePublishFileProgress_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStoragePublishFileProgress;
        #endregion
    }

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct RemoteStoragePublishedFileUpdated_t : ICallbackData
    //{
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t
    //    public AppId AppID; // m_nAppID AppId_t
    //    public ulong Unused; // m_ulUnused uint64

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(RemoteStoragePublishedFileUpdated_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.RemoteStoragePublishedFileUpdated;
    //    #endregion
    //}

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStorageFileWriteAsyncComplete_t : ICallbackData
    {
        public EResult m_eResult; // m_eResult EResult

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageFileWriteAsyncComplete_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStorageFileWriteAsyncComplete;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStorageFileReadAsyncComplete_t : ICallbackData
    {
        public SteamAPICall_t m_hFileReadAsync; // m_hFileReadAsync SteamAPICall_t
        public EResult m_eResult; // m_eResult EResult
        public uint m_nOffset; // m_nOffset uint32
        public uint m_cubRead; // m_cubRead uint32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageFileReadAsyncComplete_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStorageFileReadAsyncComplete;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStorageLocalFileChange_t : ICallbackData
    {

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageLocalFileChange_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStorageLocalFileChange;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct UserStatsReceived_t : ICallbackData
    {
        public ulong m_nGameID; // m_nGameID uint64
        public EResult m_eResult; // m_eResult EResult
        public CSteamID m_steamIDUser; // m_steamIDUser CSteamID

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(UserStatsReceived_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.UserStatsReceived;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct UserStatsStored_t : ICallbackData
    {
        public ulong m_nGameID; // m_nGameID uint64
        public EResult m_eResult; // m_eResult EResult

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(UserStatsStored_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.UserStatsStored;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct UserAchievementStored_t : ICallbackData
    {
        public ulong m_nGameID; // m_nGameID uint64
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bGroupAchievement; // m_bGroupAchievement bool
        public string AchievementNameUTF8() => System.Text.Encoding.UTF8.GetString(m_rgchAchievementName, 0, System.Array.IndexOf<byte>(m_rgchAchievementName, 0));
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)] // byte[] m_rgchAchievementName
        public byte[] m_rgchAchievementName; // m_rgchAchievementName char [128]
        public uint m_nCurProgress; // m_nCurProgress uint32
        public uint m_nMaxProgress; // m_nMaxProgress uint32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(UserAchievementStored_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.UserAchievementStored;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct LeaderboardFindResult_t : ICallbackData
    {
        public ulong SteamLeaderboard; // m_hSteamLeaderboard SteamLeaderboard_t
        public byte LeaderboardFound; // m_bLeaderboardFound uint8

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(LeaderboardFindResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.LeaderboardFindResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct LeaderboardScoresDownloaded_t : ICallbackData
    {
        public ulong SteamLeaderboard; // m_hSteamLeaderboard SteamLeaderboard_t
        public ulong SteamLeaderboardEntries; // m_hSteamLeaderboardEntries SteamLeaderboardEntries_t
        public int CEntryCount; // m_cEntryCount int

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(LeaderboardScoresDownloaded_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.LeaderboardScoresDownloaded;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct LeaderboardScoreUploaded_t : ICallbackData
    {
        public byte Success; // m_bSuccess uint8
        public ulong SteamLeaderboard; // m_hSteamLeaderboard SteamLeaderboard_t
        public int Score; // m_nScore int32
        public byte ScoreChanged; // m_bScoreChanged uint8
        public int GlobalRankNew; // m_nGlobalRankNew int
        public int GlobalRankPrevious; // m_nGlobalRankPrevious int

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(LeaderboardScoreUploaded_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.LeaderboardScoreUploaded;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct NumberOfCurrentPlayers_t : ICallbackData
    {
        public byte m_bSuccess; // m_bSuccess uint8
        public int m_cPlayers; // m_cPlayers int32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(NumberOfCurrentPlayers_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.NumberOfCurrentPlayers;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct UserStatsUnloaded_t : ICallbackData
    {
        public ulong SteamIDUser; // m_steamIDUser CSteamID

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(UserStatsUnloaded_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.UserStatsUnloaded;
        #endregion
    }

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct UserAchievementIconFetched_t : ICallbackData
    //{
    //    public GameId GameID; // m_nGameID CGameID
    //    public string AchievementNameUTF8() => System.Text.Encoding.UTF8.GetString(AchievementName, 0, System.Array.IndexOf<byte>(AchievementName, 0));
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)] // byte[] m_rgchAchievementName
    //    public byte[] AchievementName; // m_rgchAchievementName char [128]
    //    [MarshalAs(UnmanagedType.I1)]
    //    public bool Achieved; // m_bAchieved bool
    //    public int IconHandle; // m_nIconHandle int

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(UserAchievementIconFetched_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.UserAchievementIconFetched;
    //    #endregion
    //}

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct GlobalAchievementPercentagesReady_t : ICallbackData
    {
        public ulong GameID; // m_nGameID uint64
        public EResult Result; // m_eResult EResult

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GlobalAchievementPercentagesReady_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GlobalAchievementPercentagesReady;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct LeaderboardUGCSet_t : ICallbackData
    {
        public EResult Result; // m_eResult EResult
        public ulong SteamLeaderboard; // m_hSteamLeaderboard SteamLeaderboard_t

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(LeaderboardUGCSet_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.LeaderboardUGCSet;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct GlobalStatsReceived_t : ICallbackData
    {
        public ulong m_nGameID; // m_nGameID uint64
        public EResult m_eResult; // m_eResult EResult

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GlobalStatsReceived_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GlobalStatsReceived;
        #endregion
    }

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct DlcInstalled_t : ICallbackData
    //{
    //    public AppId AppID; // m_nAppID AppId_t

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(DlcInstalled_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.DlcInstalled;
    //    #endregion
    //}

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RegisterActivationCodeResponse_t : ICallbackData
    {
        public RegisterActivationCodeResult Result; // m_eResult ERegisterActivationCodeResult
        public uint PackageRegistered; // m_unPackageRegistered uint32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RegisterActivationCodeResponse_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RegisterActivationCodeResponse;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct NewUrlLaunchParameters_t : ICallbackData
    {

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(NewUrlLaunchParameters_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.NewUrlLaunchParameters;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct AppProofOfPurchaseKeyResponse_t : ICallbackData
    {
        public EResult Result; // m_eResult EResult
        public uint AppID; // m_nAppID uint32
        public uint CchKeyLength; // m_cchKeyLength uint32
        public string KeyUTF8() => System.Text.Encoding.UTF8.GetString(Key, 0, System.Array.IndexOf<byte>(Key, 0));
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 240)] // byte[] m_rgchKey
        public byte[] Key; // m_rgchKey char [240]

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(AppProofOfPurchaseKeyResponse_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.AppProofOfPurchaseKeyResponse;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct FileDetailsResult_t : ICallbackData
    {
        public EResult Result; // m_eResult EResult
        public ulong FileSize; // m_ulFileSize uint64
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)] //  m_FileSHA
        public byte[] FileSHA; // m_FileSHA uint8 [20]
        public uint Flags; // m_unFlags uint32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(FileDetailsResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.FileDetailsResult;
        #endregion
    }

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct TimedTrialStatus_t : ICallbackData
    //{
    //    public AppId AppID; // m_unAppID AppId_t
    //    [MarshalAs(UnmanagedType.I1)]
    //    public bool IsOffline; // m_bIsOffline bool
    //    public uint SecondsAllowed; // m_unSecondsAllowed uint32
    //    public uint SecondsPlayed; // m_unSecondsPlayed uint32

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(TimedTrialStatus_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.TimedTrialStatus;
    //    #endregion
    //}

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct P2PSessionRequest_t : ICallbackData
    {
        public ulong m_steamIDRemote; // m_steamIDRemote CSteamID

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(P2PSessionRequest_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.P2PSessionRequest;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct P2PSessionConnectFail_t : ICallbackData
    {
        public ulong SteamIDRemote; // m_steamIDRemote CSteamID
        public byte P2PSessionError; // m_eP2PSessionError uint8

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(P2PSessionConnectFail_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.P2PSessionConnectFail;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct ScreenshotReady_t : ICallbackData
    {
        public uint Local; // m_hLocal ScreenshotHandle
        public EResult Result; // m_eResult EResult

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(ScreenshotReady_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.ScreenshotReady;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct ScreenshotRequested_t : ICallbackData
    {

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(ScreenshotRequested_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.ScreenshotRequested;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct PlaybackStatusHasChanged_t : ICallbackData
    {

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(PlaybackStatusHasChanged_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.PlaybackStatusHasChanged;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct VolumeHasChanged_t : ICallbackData
    {
        public float NewVolume; // m_flNewVolume float

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(VolumeHasChanged_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.VolumeHasChanged;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct MusicPlayerRemoteWillActivate_t : ICallbackData
    {

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(MusicPlayerRemoteWillActivate_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.MusicPlayerRemoteWillActivate;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct MusicPlayerRemoteWillDeactivate_t : ICallbackData
    {

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(MusicPlayerRemoteWillDeactivate_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.MusicPlayerRemoteWillDeactivate;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct MusicPlayerRemoteToFront_t : ICallbackData
    {

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(MusicPlayerRemoteToFront_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.MusicPlayerRemoteToFront;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct MusicPlayerWillQuit_t : ICallbackData
    {

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(MusicPlayerWillQuit_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.MusicPlayerWillQuit;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct MusicPlayerWantsPlay_t : ICallbackData
    {

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(MusicPlayerWantsPlay_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.MusicPlayerWantsPlay;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct MusicPlayerWantsPause_t : ICallbackData
    {

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(MusicPlayerWantsPause_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.MusicPlayerWantsPause;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct MusicPlayerWantsPlayPrevious_t : ICallbackData
    {

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(MusicPlayerWantsPlayPrevious_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.MusicPlayerWantsPlayPrevious;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct MusicPlayerWantsPlayNext_t : ICallbackData
    {

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(MusicPlayerWantsPlayNext_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.MusicPlayerWantsPlayNext;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct MusicPlayerWantsShuffled_t : ICallbackData
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool Shuffled; // m_bShuffled bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(MusicPlayerWantsShuffled_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.MusicPlayerWantsShuffled;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct MusicPlayerWantsLooped_t : ICallbackData
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool Looped; // m_bLooped bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(MusicPlayerWantsLooped_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.MusicPlayerWantsLooped;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct MusicPlayerWantsVolume_t : ICallbackData
    {
        public float NewVolume; // m_flNewVolume float

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(MusicPlayerWantsVolume_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.MusicPlayerWantsVolume;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct MusicPlayerSelectsQueueEntry_t : ICallbackData
    {
        public int NID; // nID int

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(MusicPlayerSelectsQueueEntry_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.MusicPlayerSelectsQueueEntry;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct MusicPlayerSelectsPlaylistEntry_t : ICallbackData
    {
        public int NID; // nID int

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(MusicPlayerSelectsPlaylistEntry_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.MusicPlayerSelectsPlaylistEntry;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct MusicPlayerWantsPlayingRepeatStatus_t : ICallbackData
    {
        public int PlayingRepeatStatus; // m_nPlayingRepeatStatus int

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(MusicPlayerWantsPlayingRepeatStatus_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.MusicPlayerWantsPlayingRepeatStatus;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTTPRequestCompleted_t : ICallbackData
    {
        public uint Request; // m_hRequest HTTPRequestHandle
        public ulong ContextValue; // m_ulContextValue uint64
        [MarshalAs(UnmanagedType.I1)]
        public bool RequestSuccessful; // m_bRequestSuccessful bool
        public HTTPStatusCode StatusCode; // m_eStatusCode EHTTPStatusCode
        public uint BodySize; // m_unBodySize uint32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTTPRequestCompleted_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTTPRequestCompleted;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTTPRequestHeadersReceived_t : ICallbackData
    {
        public uint Request; // m_hRequest HTTPRequestHandle
        public ulong ContextValue; // m_ulContextValue uint64

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTTPRequestHeadersReceived_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTTPRequestHeadersReceived;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTTPRequestDataReceived_t : ICallbackData
    {
        public uint Request; // m_hRequest HTTPRequestHandle
        public ulong ContextValue; // m_ulContextValue uint64
        public uint COffset; // m_cOffset uint32
        public uint CBytesReceived; // m_cBytesReceived uint32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTTPRequestDataReceived_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTTPRequestDataReceived;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SteamInputDeviceConnected_t : ICallbackData
    {
        public ulong ConnectedDeviceHandle; // m_ulConnectedDeviceHandle InputHandle_t

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamInputDeviceConnected_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamInputDeviceConnected;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SteamInputDeviceDisconnected_t : ICallbackData
    {
        public ulong DisconnectedDeviceHandle; // m_ulDisconnectedDeviceHandle InputHandle_t

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamInputDeviceDisconnected_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamInputDeviceDisconnected;
        #endregion
    }

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    //public struct SteamInputConfigurationLoaded_t : ICallbackData
    //{
    //    public AppId AppID; // m_unAppID AppId_t
    //    public ulong DeviceHandle; // m_ulDeviceHandle InputHandle_t
    //    public ulong MappingCreator; // m_ulMappingCreator CSteamID
    //    public uint MajorRevision; // m_unMajorRevision uint32
    //    public uint MinorRevision; // m_unMinorRevision uint32
    //    [MarshalAs(UnmanagedType.I1)]
    //    public bool UsesSteamInputAPI; // m_bUsesSteamInputAPI bool
    //    [MarshalAs(UnmanagedType.I1)]
    //    public bool UsesGamepadAPI; // m_bUsesGamepadAPI bool

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(SteamInputConfigurationLoaded_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.SteamInputConfigurationLoaded;
    //    #endregion
    //}

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SteamUGCQueryCompleted_t : ICallbackData
    {
        public ulong Handle; // m_handle UGCQueryHandle_t
        public EResult Result; // m_eResult EResult
        public uint NumResultsReturned; // m_unNumResultsReturned uint32
        public uint TotalMatchingResults; // m_unTotalMatchingResults uint32
        [MarshalAs(UnmanagedType.I1)]
        public bool CachedData; // m_bCachedData bool
        public string NextCursorUTF8() => System.Text.Encoding.UTF8.GetString(NextCursor, 0, System.Array.IndexOf<byte>(NextCursor, 0));
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] // byte[] m_rgchNextCursor
        public byte[] NextCursor; // m_rgchNextCursor char [256]

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamUGCQueryCompleted_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamUGCQueryCompleted;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SteamUGCRequestUGCDetailsResult_t : ICallbackData
    {
        public SteamUGCDetails_t Details; // m_details SteamUGCDetails_t
        [MarshalAs(UnmanagedType.I1)]
        public bool CachedData; // m_bCachedData bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamUGCRequestUGCDetailsResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamUGCRequestUGCDetailsResult;
        #endregion
    }

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct CreateItemResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t
    //    [MarshalAs(UnmanagedType.I1)]
    //    public bool UserNeedsToAcceptWorkshopLegalAgreement; // m_bUserNeedsToAcceptWorkshopLegalAgreement bool

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(CreateItemResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.CreateItemResult;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct SubmitItemUpdateResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    [MarshalAs(UnmanagedType.I1)]
    //    public bool UserNeedsToAcceptWorkshopLegalAgreement; // m_bUserNeedsToAcceptWorkshopLegalAgreement bool
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(SubmitItemUpdateResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.SubmitItemUpdateResult;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct ItemInstalled_t : ICallbackData
    //{
    //    public AppId AppID; // m_unAppID AppId_t
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(ItemInstalled_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.ItemInstalled;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct DownloadItemResult_t : ICallbackData
    //{
    //    public AppId AppID; // m_unAppID AppId_t
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t
    //    public EResult Result; // m_eResult EResult

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(DownloadItemResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.DownloadItemResult;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct UserFavoriteItemsListChanged_t : ICallbackData
    //{
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t
    //    public EResult Result; // m_eResult EResult
    //    [MarshalAs(UnmanagedType.I1)]
    //    public bool WasAddRequest; // m_bWasAddRequest bool

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(UserFavoriteItemsListChanged_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.UserFavoriteItemsListChanged;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct SetUserItemVoteResult_t : ICallbackData
    //{
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t
    //    public EResult Result; // m_eResult EResult
    //    [MarshalAs(UnmanagedType.I1)]
    //    public bool VoteUp; // m_bVoteUp bool

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(SetUserItemVoteResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.SetUserItemVoteResult;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct GetUserItemVoteResult_t : ICallbackData
    //{
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t
    //    public EResult Result; // m_eResult EResult
    //    [MarshalAs(UnmanagedType.I1)]
    //    public bool VotedUp; // m_bVotedUp bool
    //    [MarshalAs(UnmanagedType.I1)]
    //    public bool VotedDown; // m_bVotedDown bool
    //    [MarshalAs(UnmanagedType.I1)]
    //    public bool VoteSkipped; // m_bVoteSkipped bool

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(GetUserItemVoteResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.GetUserItemVoteResult;
    //    #endregion
    //}

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct StartPlaytimeTrackingResult_t : ICallbackData
    {
        public EResult Result; // m_eResult EResult

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(StartPlaytimeTrackingResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.StartPlaytimeTrackingResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct StopPlaytimeTrackingResult_t : ICallbackData
    {
        public EResult m_eResult; // m_eResult EResult

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(StopPlaytimeTrackingResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.StopPlaytimeTrackingResult;
        #endregion
    }

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct AddUGCDependencyResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t
    //    public PublishedFileId ChildPublishedFileId; // m_nChildPublishedFileId PublishedFileId_t

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(AddUGCDependencyResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.AddUGCDependencyResult;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct RemoveUGCDependencyResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t
    //    public PublishedFileId ChildPublishedFileId; // m_nChildPublishedFileId PublishedFileId_t

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(RemoveUGCDependencyResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.RemoveUGCDependencyResult;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct AddAppDependencyResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t
    //    public AppId AppID; // m_nAppID AppId_t

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(AddAppDependencyResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.AddAppDependencyResult;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct RemoveAppDependencyResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t
    //    public AppId AppID; // m_nAppID AppId_t

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(RemoveAppDependencyResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.RemoveAppDependencyResult;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct GetAppDependenciesResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.U4)]
    //    public AppId[] GAppIDs; // m_rgAppIDs AppId_t [32]
    //    public uint NumAppDependencies; // m_nNumAppDependencies uint32
    //    public uint TotalNumAppDependencies; // m_nTotalNumAppDependencies uint32

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(GetAppDependenciesResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.GetAppDependenciesResult;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct DeleteItemResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public PublishedFileId PublishedFileId; // m_nPublishedFileId PublishedFileId_t

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(DeleteItemResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.DeleteItemResult;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct UserSubscribedItemsListChanged_t : ICallbackData
    //{
    //    public AppId AppID; // m_nAppID AppId_t

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(UserSubscribedItemsListChanged_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.UserSubscribedItemsListChanged;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct WorkshopEULAStatus_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public AppId AppID; // m_nAppID AppId_t
    //    public uint Version; // m_unVersion uint32
    //    public uint TAction; // m_rtAction RTime32
    //    [MarshalAs(UnmanagedType.I1)]
    //    public bool Accepted; // m_bAccepted bool
    //    [MarshalAs(UnmanagedType.I1)]
    //    public bool NeedsAction; // m_bNeedsAction bool

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(WorkshopEULAStatus_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.WorkshopEULAStatus;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct SteamAppInstalled_t : ICallbackData
    //{
    //    public AppId AppID; // m_nAppID AppId_t
    //    public int InstallFolderIndex; // m_iInstallFolderIndex int

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(SteamAppInstalled_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.SteamAppInstalled;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct SteamAppUninstalled_t : ICallbackData
    //{
    //    public AppId AppID; // m_nAppID AppId_t
    //    public int InstallFolderIndex; // m_iInstallFolderIndex int

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(SteamAppUninstalled_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.SteamAppUninstalled;
    //    #endregion
    //}

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTML_BrowserReady_t : ICallbackData
    {
        public uint UnBrowserHandle; // unBrowserHandle HHTMLBrowser

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTML_BrowserReady_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTML_BrowserReady;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTML_NeedsPaint_t : ICallbackData
    {
        public uint UnBrowserHandle; // unBrowserHandle HHTMLBrowser
        public string PBGRA; // pBGRA const char *
        public uint UnWide; // unWide uint32
        public uint UnTall; // unTall uint32
        public uint UnUpdateX; // unUpdateX uint32
        public uint UnUpdateY; // unUpdateY uint32
        public uint UnUpdateWide; // unUpdateWide uint32
        public uint UnUpdateTall; // unUpdateTall uint32
        public uint UnScrollX; // unScrollX uint32
        public uint UnScrollY; // unScrollY uint32
        public float FlPageScale; // flPageScale float
        public uint UnPageSerial; // unPageSerial uint32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTML_NeedsPaint_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTML_NeedsPaint;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTML_StartRequest_t : ICallbackData
    {
        public uint UnBrowserHandle; // unBrowserHandle HHTMLBrowser
        public string PchURL; // pchURL const char *
        public string PchTarget; // pchTarget const char *
        public string PchPostData; // pchPostData const char *
        [MarshalAs(UnmanagedType.I1)]
        public bool BIsRedirect; // bIsRedirect bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTML_StartRequest_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTML_StartRequest;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTML_CloseBrowser_t : ICallbackData
    {
        public uint UnBrowserHandle; // unBrowserHandle HHTMLBrowser

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTML_CloseBrowser_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTML_CloseBrowser;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTML_URLChanged_t : ICallbackData
    {
        public uint UnBrowserHandle; // unBrowserHandle HHTMLBrowser
        public string PchURL; // pchURL const char *
        public string PchPostData; // pchPostData const char *
        [MarshalAs(UnmanagedType.I1)]
        public bool BIsRedirect; // bIsRedirect bool
        public string PchPageTitle; // pchPageTitle const char *
        [MarshalAs(UnmanagedType.I1)]
        public bool BNewNavigation; // bNewNavigation bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTML_URLChanged_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTML_URLChanged;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTML_FinishedRequest_t : ICallbackData
    {
        public uint UnBrowserHandle; // unBrowserHandle HHTMLBrowser
        public string PchURL; // pchURL const char *
        public string PchPageTitle; // pchPageTitle const char *

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTML_FinishedRequest_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTML_FinishedRequest;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTML_OpenLinkInNewTab_t : ICallbackData
    {
        public uint UnBrowserHandle; // unBrowserHandle HHTMLBrowser
        public string PchURL; // pchURL const char *

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTML_OpenLinkInNewTab_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTML_OpenLinkInNewTab;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTML_ChangedTitle_t : ICallbackData
    {
        public uint UnBrowserHandle; // unBrowserHandle HHTMLBrowser
        public string PchTitle; // pchTitle const char *

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTML_ChangedTitle_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTML_ChangedTitle;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTML_SearchResults_t : ICallbackData
    {
        public uint UnBrowserHandle; // unBrowserHandle HHTMLBrowser
        public uint UnResults; // unResults uint32
        public uint UnCurrentMatch; // unCurrentMatch uint32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTML_SearchResults_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTML_SearchResults;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTML_CanGoBackAndForward_t : ICallbackData
    {
        public uint UnBrowserHandle; // unBrowserHandle HHTMLBrowser
        [MarshalAs(UnmanagedType.I1)]
        public bool BCanGoBack; // bCanGoBack bool
        [MarshalAs(UnmanagedType.I1)]
        public bool BCanGoForward; // bCanGoForward bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTML_CanGoBackAndForward_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTML_CanGoBackAndForward;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTML_HorizontalScroll_t : ICallbackData
    {
        public uint UnBrowserHandle; // unBrowserHandle HHTMLBrowser
        public uint UnScrollMax; // unScrollMax uint32
        public uint UnScrollCurrent; // unScrollCurrent uint32
        public float FlPageScale; // flPageScale float
        [MarshalAs(UnmanagedType.I1)]
        public bool BVisible; // bVisible bool
        public uint UnPageSize; // unPageSize uint32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTML_HorizontalScroll_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTML_HorizontalScroll;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTML_VerticalScroll_t : ICallbackData
    {
        public uint UnBrowserHandle; // unBrowserHandle HHTMLBrowser
        public uint UnScrollMax; // unScrollMax uint32
        public uint UnScrollCurrent; // unScrollCurrent uint32
        public float FlPageScale; // flPageScale float
        [MarshalAs(UnmanagedType.I1)]
        public bool BVisible; // bVisible bool
        public uint UnPageSize; // unPageSize uint32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTML_VerticalScroll_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTML_VerticalScroll;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTML_LinkAtPosition_t : ICallbackData
    {
        public uint UnBrowserHandle; // unBrowserHandle HHTMLBrowser
        public uint X; // x uint32
        public uint Y; // y uint32
        public string PchURL; // pchURL const char *
        [MarshalAs(UnmanagedType.I1)]
        public bool BInput; // bInput bool
        [MarshalAs(UnmanagedType.I1)]
        public bool BLiveLink; // bLiveLink bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTML_LinkAtPosition_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTML_LinkAtPosition;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTML_JSAlert_t : ICallbackData
    {
        public uint UnBrowserHandle; // unBrowserHandle HHTMLBrowser
        public string PchMessage; // pchMessage const char *

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTML_JSAlert_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTML_JSAlert;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTML_JSConfirm_t : ICallbackData
    {
        public uint UnBrowserHandle; // unBrowserHandle HHTMLBrowser
        public string PchMessage; // pchMessage const char *

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTML_JSConfirm_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTML_JSConfirm;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTML_FileOpenDialog_t : ICallbackData
    {
        public uint UnBrowserHandle; // unBrowserHandle HHTMLBrowser
        public string PchTitle; // pchTitle const char *
        public string PchInitialFile; // pchInitialFile const char *

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTML_FileOpenDialog_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTML_FileOpenDialog;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTML_NewWindow_t : ICallbackData
    {
        public uint UnBrowserHandle; // unBrowserHandle HHTMLBrowser
        public string PchURL; // pchURL const char *
        public uint UnX; // unX uint32
        public uint UnY; // unY uint32
        public uint UnWide; // unWide uint32
        public uint UnTall; // unTall uint32
        public uint UnNewWindow_BrowserHandle_IGNORE; // unNewWindow_BrowserHandle_IGNORE HHTMLBrowser

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTML_NewWindow_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTML_NewWindow;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTML_SetCursor_t : ICallbackData
    {
        public uint UnBrowserHandle; // unBrowserHandle HHTMLBrowser
        public uint EMouseCursor; // eMouseCursor uint32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTML_SetCursor_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTML_SetCursor;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTML_StatusText_t : ICallbackData
    {
        public uint UnBrowserHandle; // unBrowserHandle HHTMLBrowser
        public string PchMsg; // pchMsg const char *

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTML_StatusText_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTML_StatusText;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTML_ShowToolTip_t : ICallbackData
    {
        public uint UnBrowserHandle; // unBrowserHandle HHTMLBrowser
        public string PchMsg; // pchMsg const char *

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTML_ShowToolTip_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTML_ShowToolTip;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTML_UpdateToolTip_t : ICallbackData
    {
        public uint UnBrowserHandle; // unBrowserHandle HHTMLBrowser
        public string PchMsg; // pchMsg const char *

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTML_UpdateToolTip_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTML_UpdateToolTip;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTML_HideToolTip_t : ICallbackData
    {
        public uint UnBrowserHandle; // unBrowserHandle HHTMLBrowser

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTML_HideToolTip_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTML_HideToolTip;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct HTML_BrowserRestarted_t : ICallbackData
    {
        public uint UnBrowserHandle; // unBrowserHandle HHTMLBrowser
        public uint UnOldBrowserHandle; // unOldBrowserHandle HHTMLBrowser

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(HTML_BrowserRestarted_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.HTML_BrowserRestarted;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SteamInventoryResultReady_t : ICallbackData
    {
        public int Handle; // m_handle SteamInventoryResult_t
        public EResult Result; // m_result EResult

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamInventoryResultReady_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamInventoryResultReady;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SteamInventoryFullUpdate_t : ICallbackData
    {
        public int Handle; // m_handle SteamInventoryResult_t

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamInventoryFullUpdate_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamInventoryFullUpdate;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SteamInventoryDefinitionUpdate_t : ICallbackData
    {

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamInventoryDefinitionUpdate_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamInventoryDefinitionUpdate;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct SteamInventoryEligiblePromoItemDefIDs_t : ICallbackData
    {
        public EResult Result; // m_result EResult
        public ulong SteamID; // m_steamID CSteamID
        public int UmEligiblePromoItemDefs; // m_numEligiblePromoItemDefs int
        [MarshalAs(UnmanagedType.I1)]
        public bool CachedData; // m_bCachedData bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamInventoryEligiblePromoItemDefIDs_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamInventoryEligiblePromoItemDefIDs;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SteamInventoryStartPurchaseResult_t : ICallbackData
    {
        public EResult Result; // m_result EResult
        public ulong OrderID; // m_ulOrderID uint64
        public ulong TransID; // m_ulTransID uint64

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamInventoryStartPurchaseResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamInventoryStartPurchaseResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SteamInventoryRequestPricesResult_t : ICallbackData
    {
        public EResult Result; // m_result EResult
        public string CurrencyUTF8() => System.Text.Encoding.UTF8.GetString(Currency, 0, System.Array.IndexOf<byte>(Currency, 0));
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] // byte[] m_rgchCurrency
        public byte[] Currency; // m_rgchCurrency char [4]

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamInventoryRequestPricesResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamInventoryRequestPricesResult;
        #endregion
    }

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct GetVideoURLResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public AppId VideoAppID; // m_unVideoAppID AppId_t
    //    public string URLUTF8() => System.Text.Encoding.UTF8.GetString(URL, 0, System.Array.IndexOf<byte>(URL, 0));
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] // byte[] m_rgchURL
    //    public byte[] URL; // m_rgchURL char [256]

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(GetVideoURLResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.GetVideoURLResult;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct GetOPFSettingsResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public AppId VideoAppID; // m_unVideoAppID AppId_t

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(GetOPFSettingsResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.GetOPFSettingsResult;
    //    #endregion
    //}

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SteamParentalSettingsChanged_t : ICallbackData
    {

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamParentalSettingsChanged_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamParentalSettingsChanged;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SteamRemotePlaySessionConnected_t : ICallbackData
    {
        public uint SessionID; // m_unSessionID RemotePlaySessionID_t

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamRemotePlaySessionConnected_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamRemotePlaySessionConnected;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SteamRemotePlaySessionDisconnected_t : ICallbackData
    {
        public uint SessionID; // m_unSessionID RemotePlaySessionID_t

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamRemotePlaySessionDisconnected_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamRemotePlaySessionDisconnected;
        #endregion
    }

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct SteamNetworkingMessagesSessionRequest_t : ICallbackData
    //{
    //    public NetIdentity DentityRemote; // m_identityRemote SteamNetworkingIdentity

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(SteamNetworkingMessagesSessionRequest_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.SteamNetworkingMessagesSessionRequest;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct SteamNetworkingMessagesSessionFailed_t : ICallbackData
    //{
    //    public ConnectionInfo Nfo; // m_info SteamNetConnectionInfo_t

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(SteamNetworkingMessagesSessionFailed_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.SteamNetworkingMessagesSessionFailed;
    //    #endregion
    //}

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct SteamNetConnectionStatusChangedCallback_t : ICallbackData
    //{
    //    public Connection Conn; // m_hConn HSteamNetConnection
    //    public ConnectionInfo Nfo; // m_info SteamNetConnectionInfo_t
    //    public ConnectionState OldState; // m_eOldState ESteamNetworkingConnectionState

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(SteamNetConnectionStatusChangedCallback_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.SteamNetConnectionStatusChangedCallback;
    //    #endregion
    //}

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SteamNetAuthenticationStatus_t : ICallbackData
    {
        public ESteamNetworkingAvailability Avail; // m_eAvail ESteamNetworkingAvailability
        public string DebugMsgUTF8() => System.Text.Encoding.UTF8.GetString(DebugMsg, 0, System.Array.IndexOf<byte>(DebugMsg, 0));
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] // byte[] m_debugMsg
        public byte[] DebugMsg; // m_debugMsg char [256]

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamNetAuthenticationStatus_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamNetAuthenticationStatus;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SteamRelayNetworkStatus_t : ICallbackData
    {
        public ESteamNetworkingAvailability m_eAvail; // m_eAvail ESteamNetworkingAvailability
        public int m_bPingMeasurementInProgress; // m_bPingMeasurementInProgress int
        public ESteamNetworkingAvailability m_eAvailNetworkConfig; // m_eAvailNetworkConfig ESteamNetworkingAvailability
        public ESteamNetworkingAvailability m_eAvailAnyRelay; // m_eAvailAnyRelay ESteamNetworkingAvailability
        public string DebugMsgUTF8() => System.Text.Encoding.UTF8.GetString(DebugMsg, 0, System.Array.IndexOf<byte>(DebugMsg, 0));
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)] // byte[] m_debugMsg
        public byte[] DebugMsg; // m_debugMsg char [256]

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamRelayNetworkStatus_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamRelayNetworkStatus;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct GSClientApprove_t : ICallbackData
    {
        public ulong SteamID; // m_SteamID CSteamID
        public ulong OwnerSteamID; // m_OwnerSteamID CSteamID

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GSClientApprove_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GSClientApprove;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct GSClientDeny_t : ICallbackData
    {
        public ulong SteamID; // m_SteamID CSteamID
        public DenyReason DenyReason; // m_eDenyReason EDenyReason
        public string OptionalTextUTF8() => System.Text.Encoding.UTF8.GetString(OptionalText, 0, System.Array.IndexOf<byte>(OptionalText, 0));
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)] // byte[] m_rgchOptionalText
        public byte[] OptionalText; // m_rgchOptionalText char [128]

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GSClientDeny_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GSClientDeny;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct GSClientKick_t : ICallbackData
    {
        public ulong SteamID; // m_SteamID CSteamID
        public DenyReason DenyReason; // m_eDenyReason EDenyReason

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GSClientKick_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GSClientKick;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct GSClientAchievementStatus_t : ICallbackData
    {
        public ulong SteamID; // m_SteamID uint64
        public string PchAchievementUTF8() => System.Text.Encoding.UTF8.GetString(PchAchievement, 0, System.Array.IndexOf<byte>(PchAchievement, 0));
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)] // byte[] m_pchAchievement
        public byte[] PchAchievement; // m_pchAchievement char [128]
        [MarshalAs(UnmanagedType.I1)]
        public bool Unlocked; // m_bUnlocked bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GSClientAchievementStatus_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GSClientAchievementStatus;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct GSPolicyResponse_t : ICallbackData
    {
        public byte Secure; // m_bSecure uint8

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GSPolicyResponse_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GSPolicyResponse;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct GSGameplayStats_t : ICallbackData
    {
        public EResult Result; // m_eResult EResult
        public int Rank; // m_nRank int32
        public uint TotalConnects; // m_unTotalConnects uint32
        public uint TotalMinutesPlayed; // m_unTotalMinutesPlayed uint32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GSGameplayStats_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GSGameplayStats;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct GSClientGroupStatus_t : ICallbackData
    {
        public ulong SteamIDUser; // m_SteamIDUser CSteamID
        public ulong SteamIDGroup; // m_SteamIDGroup CSteamID
        [MarshalAs(UnmanagedType.I1)]
        public bool Member; // m_bMember bool
        [MarshalAs(UnmanagedType.I1)]
        public bool Officer; // m_bOfficer bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GSClientGroupStatus_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GSClientGroupStatus;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct GSReputation_t : ICallbackData
    {
        public EResult Result; // m_eResult EResult
        public uint ReputationScore; // m_unReputationScore uint32
        [MarshalAs(UnmanagedType.I1)]
        public bool Banned; // m_bBanned bool
        public uint BannedIP; // m_unBannedIP uint32
        public ushort BannedPort; // m_usBannedPort uint16
        public ulong BannedGameID; // m_ulBannedGameID uint64
        public uint BanExpires; // m_unBanExpires uint32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GSReputation_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GSReputation;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct AssociateWithClanResult_t : ICallbackData
    {
        public EResult Result; // m_eResult EResult

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(AssociateWithClanResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.AssociateWithClanResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct ComputeNewPlayerCompatibilityResult_t : ICallbackData
    {
        public EResult Result; // m_eResult EResult
        public int CPlayersThatDontLikeCandidate; // m_cPlayersThatDontLikeCandidate int
        public int CPlayersThatCandidateDoesntLike; // m_cPlayersThatCandidateDoesntLike int
        public int CClanPlayersThatDontLikeCandidate; // m_cClanPlayersThatDontLikeCandidate int
        public ulong SteamIDCandidate; // m_SteamIDCandidate CSteamID

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(ComputeNewPlayerCompatibilityResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.ComputeNewPlayerCompatibilityResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct GSStatsReceived_t : ICallbackData
    {
        public EResult Result; // m_eResult EResult
        public ulong SteamIDUser; // m_steamIDUser CSteamID

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GSStatsReceived_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GSStatsReceived;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct GSStatsStored_t : ICallbackData
    {
        public EResult Result; // m_eResult EResult
        public ulong SteamIDUser; // m_steamIDUser CSteamID

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GSStatsStored_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GSStatsStored;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct GSStatsUnloaded_t : ICallbackData
    {
        public ulong SteamIDUser; // m_steamIDUser CSteamID

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GSStatsUnloaded_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GSStatsUnloaded;
        #endregion
    }

    //[StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    //public struct SteamNetworkingFakeIPResult_t : ICallbackData
    //{
    //    public EResult Result; // m_eResult EResult
    //    public NetIdentity Dentity; // m_identity SteamNetworkingIdentity
    //    public uint IP; // m_unIP uint32
    //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.U2)]
    //    public ushort[] Ports; // m_unPorts uint16 [8]

    //    #region SteamCallback
    //    public static int _datasize = Marshal.SizeOf(typeof(SteamNetworkingFakeIPResult_t));
    //    public int DataSize => _datasize;
    //    public CallbackType CallbackType => CallbackType.SteamNetworkingFakeIPResult;
    //    #endregion
    //}

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct GCMessageAvailable_t : ICallbackData
    {
        public uint m_nMessageSize;

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GCMessageAvailable_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamGameCoordinator;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct GCMessageFailed_t : ICallbackData
    {
        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GCMessageAvailable_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamGameCoordinatorFailed;
        #endregion
    };

    public struct SteamNetworkingSocketsCert_t : ICallbackData
    {
        public EResult m_eResult;
        public UInt32 m_cbCert;
        public byte[] m_certOrMsg; //CMsgSteamDatagramCertificate protobuf
        public UInt64 m_caKeyID;
        public UInt32 m_cbSignature;
        public byte[] m_signature;
        public UInt32 m_cbPrivKey;
        public byte[] m_privKey;
        //total size: 792
        //0
        //4
        //8 []
        //0x208
        //0x20C
        //0x210
        //0x214 []
        //0x294
        //0x298 []

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamNetworkingSocketsCert_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => (CallbackType)Constants.k_iSteamNetworkingCallbacks + 96;
        #endregion
    };
}
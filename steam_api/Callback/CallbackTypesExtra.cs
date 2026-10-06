using System;
using System.Runtime.InteropServices;
using SKYNET.Helpers;
using SKYNET.Steamworks;

using PublishedFileId_t = System.UInt64;
using UGCHandle_t = System.UInt64;

namespace SKYNET.Callback
{
    // Callback structs declared in the SDK 1.65 headers that the emulator does not deliver itself yet.
    // Layout is verified by DeveloperTools/CallbackLayoutCheck against the real headers (x64 + x86).
    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct AddAppDependencyResult_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public ulong m_nPublishedFileId; // PublishedFileId_t
        public uint m_nAppID; // AppId_t

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(AddAppDependencyResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.AddAppDependencyResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct AddUGCDependencyResult_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public ulong m_nPublishedFileId; // PublishedFileId_t
        public ulong m_nChildPublishedFileId; // PublishedFileId_t

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(AddUGCDependencyResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.AddUGCDependencyResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct BroadcastUploadStart_t : ICallbackData
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bIsRTMP; // bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(BroadcastUploadStart_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.BroadcastUploadStart;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct BroadcastUploadStop_t : ICallbackData
    {
        public int m_eResult; // EBroadcastUploadResult

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(BroadcastUploadStop_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.BroadcastUploadStop;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct CreateItemResult_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public ulong m_nPublishedFileId; // PublishedFileId_t
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bUserNeedsToAcceptWorkshopLegalAgreement; // bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(CreateItemResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.CreateItemResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct DeleteItemResult_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public ulong m_nPublishedFileId; // PublishedFileId_t

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(DeleteItemResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.DeleteItemResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct DownloadItemResult_t : ICallbackData
    {
        public uint m_unAppID; // AppId_t
        public ulong m_nPublishedFileId; // PublishedFileId_t
        public EResult m_eResult; // EResult

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(DownloadItemResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.DownloadItemResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct EquippedProfileItemsChanged_t : ICallbackData
    {
        public ulong m_steamID; // CSteamID

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(EquippedProfileItemsChanged_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.EquippedProfileItemsChanged;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct EquippedProfileItems_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public ulong m_steamID; // CSteamID
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bHasAnimatedAvatar; // bool
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bHasAvatarFrame; // bool
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bHasProfileModifier; // bool
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bHasProfileBackground; // bool
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bHasMiniProfileBackground; // bool
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bFromCache; // bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(EquippedProfileItems_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.EquippedProfileItems;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct FilterTextDictionaryChanged_t : ICallbackData
    {
        public int m_eLanguage; // int

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(FilterTextDictionaryChanged_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.FilterTextDictionaryChanged;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPackSize)]
    public struct FriendRichPresenceUpdate_t : ICallbackData
    {
        public ulong m_steamIDFriend; // CSteamID
        public uint m_nAppID; // AppId_t

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(FriendRichPresenceUpdate_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.FriendRichPresenceUpdate;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct GetAppDependenciesResult_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public ulong m_nPublishedFileId; // PublishedFileId_t
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.U4)]
        public uint[] m_rgAppIDs; // AppId_t [32]
        public uint m_nNumAppDependencies; // uint32
        public uint m_nTotalNumAppDependencies; // uint32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GetAppDependenciesResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GetAppDependenciesResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct GetOPFSettingsResult_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public uint m_unVideoAppID; // AppId_t

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GetOPFSettingsResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GetOPFSettingsResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct GetTicketForWebApiResponse_t : ICallbackData
    {
        public uint m_hAuthTicket; // HAuthTicket
        public EResult m_eResult; // EResult
        public int m_cubTicket; // int
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2560)]
        public byte[] m_rgubTicket; // uint8 [2560]

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GetTicketForWebApiResponse_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GetTicketForWebApiResponse;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct GetUserItemVoteResult_t : ICallbackData
    {
        public ulong m_nPublishedFileId; // PublishedFileId_t
        public EResult m_eResult; // EResult
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bVotedUp; // bool
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bVotedDown; // bool
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bVoteSkipped; // bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GetUserItemVoteResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GetUserItemVoteResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct GetVideoURLResult_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public uint m_unVideoAppID; // AppId_t
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] m_rgchURL; // char [256]

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(GetVideoURLResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.GetVideoURLResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStorageDeletePublishedFileResult_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public ulong m_nPublishedFileId; // PublishedFileId_t

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageDeletePublishedFileResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStorageDeletePublishedFileResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStorageDownloadUGCResult_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public ulong m_hFile; // UGCHandle_t
        public uint m_nAppID; // AppId_t
        public int m_nSizeInBytes; // int32
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
        public byte[] m_pchFileName; // char [260]
        public ulong m_ulSteamIDOwner; // uint64

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageDownloadUGCResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStorageDownloadUGCResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStorageEnumeratePublishedFilesByUserActionResult_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public EWorkshopFileAction m_eAction; // EWorkshopFileAction
        public int m_nResultsReturned; // int32
        public int m_nTotalResultCount; // int32
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.U8)]
        public ulong[] m_rgPublishedFileId; // PublishedFileId_t [50]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.U4)]
        public uint[] m_rgRTimeUpdated; // uint32 [50]

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageEnumeratePublishedFilesByUserActionResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStorageEnumeratePublishedFilesByUserActionResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStorageEnumerateUserSharedWorkshopFilesResult_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public int m_nResultsReturned; // int32
        public int m_nTotalResultCount; // int32
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.U8)]
        public ulong[] m_rgPublishedFileId; // PublishedFileId_t [50]

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageEnumerateUserSharedWorkshopFilesResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStorageEnumerateUserSharedWorkshopFilesResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStorageEnumerateUserSubscribedFilesResult_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public int m_nResultsReturned; // int32
        public int m_nTotalResultCount; // int32
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.U8)]
        public ulong[] m_rgPublishedFileId; // PublishedFileId_t [50]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.U4)]
        public uint[] m_rgRTimeSubscribed; // uint32 [50]

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageEnumerateUserSubscribedFilesResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStorageEnumerateUserSubscribedFilesResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStorageEnumerateWorkshopFilesResult_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public int m_nResultsReturned; // int32
        public int m_nTotalResultCount; // int32
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.U8)]
        public ulong[] m_rgPublishedFileId; // PublishedFileId_t [50]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.R4)]
        public float[] m_rgScore; // float [50]
        public uint m_nAppId; // AppId_t
        public uint m_unStartIndex; // uint32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageEnumerateWorkshopFilesResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStorageEnumerateWorkshopFilesResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStorageGetPublishedFileDetailsResult_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public ulong m_nPublishedFileId; // PublishedFileId_t
        public uint m_nCreatorAppID; // AppId_t
        public uint m_nConsumerAppID; // AppId_t
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 129)]
        public byte[] m_rgchTitle; // char [129]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8000)]
        public byte[] m_rgchDescription; // char [8000]
        public ulong m_hFile; // UGCHandle_t
        public ulong m_hPreviewFile; // UGCHandle_t
        public ulong m_ulSteamIDOwner; // uint64
        public uint m_rtimeCreated; // uint32
        public uint m_rtimeUpdated; // uint32
        public ERemoteStoragePublishedFileVisibility m_eVisibility; // ERemoteStoragePublishedFileVisibility
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bBanned; // bool
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1025)]
        public byte[] m_rgchTags; // char [1025]
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bTagsTruncated; // bool
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
        public byte[] m_pchFileName; // char [260]
        public int m_nFileSize; // int32
        public int m_nPreviewFileSize; // int32
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] m_rgchURL; // char [256]
        public EWorkshopFileType m_eFileType; // EWorkshopFileType
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bAcceptedForUse; // bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageGetPublishedFileDetailsResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStorageGetPublishedFileDetailsResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStorageGetPublishedItemVoteDetailsResult_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public ulong m_unPublishedFileId; // PublishedFileId_t
        public int m_nVotesFor; // int32
        public int m_nVotesAgainst; // int32
        public int m_nReports; // int32
        public float m_fScore; // float

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageGetPublishedItemVoteDetailsResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStorageGetPublishedItemVoteDetailsResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStoragePublishFileResult_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public ulong m_nPublishedFileId; // PublishedFileId_t
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bUserNeedsToAcceptWorkshopLegalAgreement; // bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStoragePublishFileResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStoragePublishFileResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStoragePublishedFileDeleted_t : ICallbackData
    {
        public ulong m_nPublishedFileId; // PublishedFileId_t
        public uint m_nAppID; // AppId_t

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStoragePublishedFileDeleted_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStoragePublishedFileDeleted;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStoragePublishedFileSubscribed_t : ICallbackData
    {
        public ulong m_nPublishedFileId; // PublishedFileId_t
        public uint m_nAppID; // AppId_t

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStoragePublishedFileSubscribed_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStoragePublishedFileSubscribed;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStoragePublishedFileUnsubscribed_t : ICallbackData
    {
        public ulong m_nPublishedFileId; // PublishedFileId_t
        public uint m_nAppID; // AppId_t

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStoragePublishedFileUnsubscribed_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStoragePublishedFileUnsubscribed;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStoragePublishedFileUpdated_t : ICallbackData
    {
        public ulong m_nPublishedFileId; // PublishedFileId_t
        public uint m_nAppID; // AppId_t
        public ulong m_ulUnused; // uint64

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStoragePublishedFileUpdated_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStoragePublishedFileUpdated;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStorageSetUserPublishedFileActionResult_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public ulong m_nPublishedFileId; // PublishedFileId_t
        public EWorkshopFileAction m_eAction; // EWorkshopFileAction

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageSetUserPublishedFileActionResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStorageSetUserPublishedFileActionResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStorageUpdatePublishedFileResult_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public ulong m_nPublishedFileId; // PublishedFileId_t
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bUserNeedsToAcceptWorkshopLegalAgreement; // bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageUpdatePublishedFileResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStorageUpdatePublishedFileResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStorageUpdateUserPublishedItemVoteResult_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public ulong m_nPublishedFileId; // PublishedFileId_t

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageUpdateUserPublishedItemVoteResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStorageUpdateUserPublishedItemVoteResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoteStorageUserVoteDetails_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public ulong m_nPublishedFileId; // PublishedFileId_t
        public int m_eVote; // EWorkshopVote

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageUserVoteDetails_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoteStorageUserVoteDetails;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoveAppDependencyResult_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public ulong m_nPublishedFileId; // PublishedFileId_t
        public uint m_nAppID; // AppId_t

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoveAppDependencyResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoveAppDependencyResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct RemoveUGCDependencyResult_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public ulong m_nPublishedFileId; // PublishedFileId_t
        public ulong m_nChildPublishedFileId; // PublishedFileId_t

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(RemoveUGCDependencyResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.RemoveUGCDependencyResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SetUserItemVoteResult_t : ICallbackData
    {
        public ulong m_nPublishedFileId; // PublishedFileId_t
        public EResult m_eResult; // EResult
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bVoteUp; // bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SetUserItemVoteResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SetUserItemVoteResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SteamInputConfigurationLoaded_t : ICallbackData
    {
        public uint m_unAppID; // AppId_t
        public ulong m_ulDeviceHandle; // InputHandle_t
        public ulong m_ulMappingCreator; // CSteamID
        public uint m_unMajorRevision; // uint32
        public uint m_unMinorRevision; // uint32
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bUsesSteamInputAPI; // bool
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bUsesGamepadAPI; // bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamInputConfigurationLoaded_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamInputConfigurationLoaded;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SteamInputGamepadSlotChange_t : ICallbackData
    {
        public uint m_unAppID; // AppId_t
        public ulong m_ulDeviceHandle; // InputHandle_t
        public int m_eDeviceType; // ESteamInputType
        public int m_nOldGamepadSlot; // int
        public int m_nNewGamepadSlot; // int

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamInputGamepadSlotChange_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamInputGamepadSlotChange;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SteamNetworkingFakeIPResult_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public SteamNetworkingIdentity_t m_identity; // SteamNetworkingIdentity
        public uint m_unIP; // uint32
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.U2)]
        public ushort[] m_unPorts; // uint16 [8]

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SteamNetworkingFakeIPResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SteamNetworkingFakeIPResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct SubmitItemUpdateResult_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bUserNeedsToAcceptWorkshopLegalAgreement; // bool
        public ulong m_nPublishedFileId; // PublishedFileId_t

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(SubmitItemUpdateResult_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.SubmitItemUpdateResult;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct TimedTrialStatus_t : ICallbackData
    {
        public uint m_unAppID; // AppId_t
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bIsOffline; // bool
        public uint m_unSecondsAllowed; // uint32
        public uint m_unSecondsPlayed; // uint32

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(TimedTrialStatus_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.TimedTrialStatus;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct UserAchievementIconFetched_t : ICallbackData
    {
        public ulong m_nGameID; // CGameID
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] m_rgchAchievementName; // char [128]
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bAchieved; // bool
        public int m_nIconHandle; // int

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(UserAchievementIconFetched_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.UserAchievementIconFetched;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct UserFavoriteItemsListChanged_t : ICallbackData
    {
        public ulong m_nPublishedFileId; // PublishedFileId_t
        public EResult m_eResult; // EResult
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bWasAddRequest; // bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(UserFavoriteItemsListChanged_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.UserFavoriteItemsListChanged;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct UserSubscribedItemsListChanged_t : ICallbackData
    {
        public uint m_nAppID; // AppId_t

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(UserSubscribedItemsListChanged_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.UserSubscribedItemsListChanged;
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = Platform.StructPlatformPackSize)]
    public struct WorkshopEULAStatus_t : ICallbackData
    {
        public EResult m_eResult; // EResult
        public uint m_nAppID; // AppId_t
        public uint m_unVersion; // uint32
        public uint m_rtAction; // RTime32
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bAccepted; // bool
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bNeedsAction; // bool

        #region SteamCallback
        public static int _datasize = Marshal.SizeOf(typeof(WorkshopEULAStatus_t));
        public int DataSize => _datasize;
        public CallbackType CallbackType => CallbackType.WorkshopEULAStatus;
        #endregion
    }
}

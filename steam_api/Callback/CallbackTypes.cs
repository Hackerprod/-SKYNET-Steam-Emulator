using SKYNET.Steamworks;
using SKYNET.Types;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Callback
{
    public interface IBaseCallback
    {
        int k_iCallback { get;}
    }
    public enum UserStatsCallbacks : int
    {
        k_iSteamUserCallbacks = 100,
        k_iSteamGameServerCallbacks = 200,
        k_iSteamFriendsCallbacks = 300,
        k_iSteamBillingCallbacks = 400,
        k_iSteamMatchmakingCallbacks = 500,
        k_iSteamContentServerCallbacks = 600,
        k_iSteamUtilsCallbacks = 700,
        k_iSteamAppsCallbacks = 1000,
        k_iSteamUserStatsCallbacks = 1100,
        k_iSteamNetworkingCallbacks = 1200,
        k_iSteamNetworkingSocketsCallbacks = 1220,
        k_iSteamNetworkingMessagesCallbacks = 1250,
        k_iSteamNetworkingUtilsCallbacks = 1280,
        k_iSteamRemoteStorageCallbacks = 1300,
        k_iSteamGameServerItemsCallbacks = 1500,
        k_iSteamGameCoordinatorCallbacks = 1700,
        k_iSteamGameServerStatsCallbacks = 1800,
        k_iSteam2AsyncCallbacks = 1900,
        k_iSteamGameStatsCallbacks = 2000,
        k_iSteamHTTPCallbacks = 2100,
        k_iSteamScreenshotsCallbacks = 2300,
        // NOTE: 2500-2599 are reserved
        k_iSteamStreamLauncherCallbacks = 2600,
        k_iSteamControllerCallbacks = 2800,
        k_iSteamUGCCallbacks = 3400,
        k_iSteamStreamClientCallbacks = 3500,
        k_iSteamAppListCallbacks = 3900,
        k_iSteamMusicCallbacks = 4000,
        k_iSteamMusicRemoteCallbacks = 4100,
        k_iSteamGameNotificationCallbacks = 4400,
        k_iSteamHTMLSurfaceCallbacks = 4500,
        k_iSteamVideoCallbacks = 4600,
        k_iSteamInventoryCallbacks = 4700,
        k_ISteamParentalSettingsCallbacks = 5000,
        k_iSteamGameSearchCallbacks = 5200,
        k_iSteamPartiesCallbacks = 5300,
        k_iSteamSTARCallbacks = 5500,
        k_iSteamRemotePlayCallbacks = 5700,
        k_iSteamChatCallbacks = 5900,
    }

    public interface ICallbackData : IBaseCallback
    {
        CallbackType CallbackType { get; }

        int DataSize { get; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct SetPersonaNameResponse_t : ICallbackData
    {
        [MarshalAs(UnmanagedType.I1)]
        internal bool Success;

        [MarshalAs(UnmanagedType.I1)]
        internal bool LocalSuccess;

        internal Result Result;

        public static int _datasize = Marshal.SizeOf(typeof(SetPersonaNameResponse_t));

        public int DataSize => _datasize;

        public CallbackType CallbackType => CallbackType.k_iSetPersonaNameResponse;
        public int k_iCallback { get { return (int)0; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UserStatsReceived_t : IBaseCallback
    {
        public int k_iCallback { get { return (int)UserStatsCallbacks.k_iSteamUserStatsCallbacks + 1; } }
        public ulong m_nGameID;         // Game these stats are for
        public EResult m_eResult;       // Success / error fetching the stats
        public SteamID m_steamIDUser;	// The user for whom the stats are retrieved for
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UserStatsStored_t : IBaseCallback
    {
        public int k_iCallback { get { return (int)UserStatsCallbacks.k_iSteamUserStatsCallbacks + 2; } }
        public ulong m_nGameID;       // Game these stats are for
        public EResult m_eResult;      // success / error
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct LeaderboardFindResult_t : IBaseCallback
    {
        public int k_iCallback { get { return (int)UserStatsCallbacks.k_iSteamUserStatsCallbacks + 4; } }
        public ulong m_hSteamLeaderboard; // handle to the leaderboard serarched for, 0 if no leaderboard found
        public int m_bLeaderboardFound;              // 0 if no leaderboard found
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct NumberOfCurrentPlayers_t : IBaseCallback
    {
        public int k_iCallback { get { return (int)UserStatsCallbacks.k_iSteamUserStatsCallbacks + 7; } }
        public int m_bSuccess;           // 1 if the call was successful
        public uint m_cPlayers;          // Number of players currently playing    };
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GlobalStatsReceived_t : IBaseCallback
    {
        public int k_iCallback { get { return (int)UserStatsCallbacks.k_iSteamUserStatsCallbacks + 12; } }
        public ulong m_nGameID;               // Game global stats were requested for
        public EResult m_eResult;             // The result of the request    
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SteamUGCQueryCompleted_t : IBaseCallback
    {
        public int k_iCallback { get { return (int)UserStatsCallbacks.k_iSteamUGCCallbacks + 1; } }
        public UGCQueryHandle_t m_handle;
        public EResult m_eResult;
        public uint m_unNumResultsReturned;
        public uint m_unTotalMatchingResults;
        public bool m_bCachedData; // indicates whether this data was retrieved from the local on-disk cache
        public string m_rgchNextCursor; // If a paging cursor was used, then this will be the next cursor to get the next result set.    }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct SteamUGCDetails_t
    {
        internal PublishedFileId_t m_nPublishedFileId;

        internal Result m_eResult;

        internal WorkshopFileType m_eFileType;

        internal uint m_nCreatorAppID;

        internal uint m_nConsumerAppID;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 129)]
        internal byte[] Title;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8000)]
        internal byte[] Description;

        internal ulong SteamIDOwner;

        internal uint TimeCreated;

        internal uint TimeUpdated;

        internal uint TimeAddedToUserList;

        internal RemoteStoragePublishedFileVisibility Visibility;

        [MarshalAs(UnmanagedType.I1)]
        internal bool Banned;

        [MarshalAs(UnmanagedType.I1)]
        internal bool AcceptedForUse;

        [MarshalAs(UnmanagedType.I1)]
        internal bool TagsTruncated;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1025)]
        internal byte[] Tags;

        internal ulong File;

        internal ulong PreviewFile;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
        internal byte[] PchFileName;

        internal int FileSize;

        internal int PreviewFileSize;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        internal byte[] URL;

        internal uint VotesUp;

        internal uint VotesDown;

        internal float Score;

        internal uint NumChildren;

        internal string TitleUTF8()
        {
            return Encoding.UTF8.GetString(Title, 0, Array.IndexOf(Title, (byte)0));
        }

        internal string DescriptionUTF8()
        {
            return Encoding.UTF8.GetString(Description, 0, Array.IndexOf(Description, (byte)0));
        }

        internal string TagsUTF8()
        {
            return Encoding.UTF8.GetString(Tags, 0, Array.IndexOf(Tags, (byte)0));
        }

        internal string PchFileNameUTF8()
        {
            return Encoding.UTF8.GetString(PchFileName, 0, Array.IndexOf(PchFileName, (byte)0));
        }

        internal string URLUTF8()
        {
            return Encoding.UTF8.GetString(URL, 0, Array.IndexOf(URL, (byte)0));
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct RemoteStorageSubscribePublishedFileResult_t : IBaseCallback
    {
        public int k_iCallback { get { return (int)UserStatsCallbacks.k_iSteamRemoteStorageCallbacks + 13; } }
        public EResult m_eResult;              // The result of the operation.
        public PublishedFileId_t m_nPublishedFileId;
    };

    [StructLayout(LayoutKind.Sequential)]
    struct RemoteStorageUnsubscribePublishedFileResult_t : IBaseCallback
    {
        public int k_iCallback { get { return (int)UserStatsCallbacks.k_iSteamRemoteStorageCallbacks + 15; } }
        public EResult m_eResult;              // The result of the operation.
        public PublishedFileId_t m_nPublishedFileId;
    };

    [StructLayout(LayoutKind.Sequential)]
    struct StopPlaytimeTrackingResult_t : IBaseCallback
    {
        public int k_iCallback { get { return (int)UserStatsCallbacks.k_iSteamUGCCallbacks + 11; } }
        public EResult m_eResult;
    };

    [StructLayout(LayoutKind.Sequential)]
    struct RemoteStorageFileWriteAsyncComplete_t : IBaseCallback
    {
        public int k_iCallback { get { return (int)UserStatsCallbacks.k_iSteamRemoteStorageCallbacks + 31; } }
        public EResult m_eResult;                      // result
    };

}
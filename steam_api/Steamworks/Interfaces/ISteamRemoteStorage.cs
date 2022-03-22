using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    public interface ISteamRemoteStorage
    {
        // NOTE
        //
        // Filenames are case-insensitive, and will be converted to lowercase automatically.
        // So "foo.bar" and "Foo.bar" are the same file, and if you write "Foo.bar" then
        // iterate the files, the filename returned will be "foo.bar".
        //

        // file operations
        bool FileWrite(string pchFile, IntPtr pvData, int cubData);
        int FileRead(string pchFile, IntPtr pvData, int cubDataToRead);

        SteamAPICall_t FileWriteAsync(string pchFile, IntPtr pvData, uint cubData);
        SteamAPICall_t FileReadAsync(string pchFile, uint nOffset, uint cubToRead);
        bool FileReadAsyncComplete(SteamAPICall_t hReadCall, IntPtr pvBuffer, uint cubToRead);
        bool FileForget(string pchFile);
        bool FileDelete(string pchFile);
        SteamAPICall_t FileShare(string pchFile);
        bool SetSyncPlatforms(string pchFile, ERemoteStoragePlatform eRemoteStoragePlatform);

        // file operations that cause network IO
        UGCFileWriteStreamHandle_t FileWriteStreamOpen(string pchFile);
        bool FileWriteStreamWriteChunk(UGCFileWriteStreamHandle_t writeHandle, IntPtr pvData, int cubData);
        bool FileWriteStreamClose(UGCFileWriteStreamHandle_t writeHandle);
        bool FileWriteStreamCancel(UGCFileWriteStreamHandle_t writeHandle);

        // file information
        bool FileExists(string pchFile);
        bool FilePersisted(string pchFile);
        int GetFileSize(string pchFile);
        uint GetFileTimestamp(string pchFile);
        ERemoteStoragePlatform GetSyncPlatforms(string pchFile);

        // iteration
        int GetFileCount(IntPtr _);
        string GetFileNameAndSize(int iFile, int pnFileSizeInBytes);

        // configuration management
        bool GetQuota(uint pnTotalBytes, uint puAvailableBytes);
        bool IsCloudEnabledForAccount(IntPtr _);
        bool IsCloudEnabledForApp(IntPtr _);
        void SetCloudEnabledForApp(bool bEnabled);

        // user generated content

        // Downloads a UGC file.  A priority value of 0 will download the file immediately,
        // otherwise it will wait to download the file until all downloads with a lower priority
        // value are completed.  Downloads with equal priority will occur simultaneously.

        SteamAPICall_t UGCDownload(UGCHandle_t hContent, uint unPriority);

        // Gets the amount of data downloaded so far for a piece of content. pnBytesExpected can be 0 if function returns false
        // or if the transfer hasn't started yet, so be careful to check for that before dividing to get a percentage
        bool GetUGCDownloadProgress(UGCHandle_t hContent, int pnBytesDownloaded, int pnBytesExpected);

        // Gets metadata for a file after it has been downloaded. This is the same metadata given in the RemoteStorageDownloadUGCResult_t call result
        bool GetUGCDetails(UGCHandle_t hContent, AppId_t pnAppID, string ppchName, int pnFileSizeInBytes, IntPtr pSteamIDOwner);

        // After download, gets the content of the file.  
        // Small files can be read all at once by calling this function with an offset of 0 and cubDataToRead equal to the size of the file.
        // Larger files can be read in chunks to reduce memory usage (since both sides of the IPC client and the game itself must allocate
        // enough memory for each chunk).  Once the last byte is read, the file is implicitly closed and further calls to UGCRead will fail
        // unless UGCDownload is called again.
        // For especially large files (anything over 100MB) it is a requirement that the file is read in chunks.
        int UGCRead(UGCHandle_t hContent, IntPtr pvData, int cubDataToRead, uint cOffset, IntPtr eAction);

        // Functions to iterate through UGC that has finished downloading but has not yet been read via UGCRead()
        int GetCachedUGCCount(IntPtr _);
        UGCHandle_t GetCachedUGCHandle(int iCachedContent);

        // The following functions are only necessary on the Playstation 3. On PC & Mac, the Steam client will handle these operations for you
        // On Playstation 3, the game controls which files are stored in the cloud, via FilePersist, FileFetch, and FileForget.

        // publishing UGC

        SteamAPICall_t PublishWorkshopFile(string pchFile, string pchPreviewFile, AppId_t nConsumerAppId, string pchTitle, string pchDescription, ERemoteStoragePublishedFileVisibility eVisibility, IntPtr pTags, int int2);
        PublishedFileUpdateHandle_t CreatePublishedFileUpdateRequest(PublishedFileId_t unPublishedFileId);
        bool UpdatePublishedFileFile(PublishedFileUpdateHandle_t updateHandle, string pchFile);
        bool UpdatePublishedFilePreviewFile(PublishedFileUpdateHandle_t updateHandle, string pchPreviewFile);
        bool UpdatePublishedFileTitle(PublishedFileUpdateHandle_t updateHandle, string pchTitle);
        bool UpdatePublishedFileDescription(PublishedFileUpdateHandle_t updateHandle, string pchDescription);
        bool UpdatePublishedFileVisibility(PublishedFileUpdateHandle_t updateHandle, ERemoteStoragePublishedFileVisibility eVisibility);
        bool UpdatePublishedFileTags(PublishedFileUpdateHandle_t updateHandle, IntPtr pTags);

        SteamAPICall_t CommitPublishedFileUpdate(PublishedFileUpdateHandle_t updateHandle);
        // Gets published file details for the given publishedfileid.  If unMaxSecondsOld is greater than 0,
        // cached data may be returned, depending on how long ago it was cached.  A value of 0 will force a refresh.
        // A value of k_WorkshopForceLoadPublishedFileDetailsFromCache will use cached data if it exists, no matter how old it is.

        SteamAPICall_t GetPublishedFileDetails(PublishedFileId_t unPublishedFileId, uint unMaxSecondsOld);

        SteamAPICall_t DeletePublishedFile(PublishedFileId_t unPublishedFileId);
        // enumerate the files that the current user published with this app

        SteamAPICall_t EnumerateUserPublishedFiles(uint unStartIndex);

        SteamAPICall_t SubscribePublishedFile(PublishedFileId_t unPublishedFileId);

        SteamAPICall_t EnumerateUserSubscribedFiles(uint unStartIndex);

        SteamAPICall_t UnsubscribePublishedFile(PublishedFileId_t unPublishedFileId);
        bool UpdatePublishedFileSetChangeDescription(PublishedFileUpdateHandle_t updateHandle, string pchChangeDescription);


        SteamAPICall_t GetPublishedItemVoteDetails(PublishedFileId_t unPublishedFileId);

        SteamAPICall_t UpdateUserPublishedItemVote(PublishedFileId_t unPublishedFileId, bool bVoteUp);


        SteamAPICall_t GetUserPublishedItemVoteDetails(PublishedFileId_t unPublishedFileId);

        SteamAPICall_t EnumerateUserSharedWorkshopFiles(IntPtr steamId, uint unStartIndex, IntPtr pRequiredTags, IntPtr pExcludedTags);

        SteamAPICall_t PublishVideo(EWorkshopVideoProvider eVideoProvider, string pchVideoAccount, string pchVideoIdentifier, string pchPreviewFile, AppId_t nConsumerAppId, string pchTitle, string pchDescription, ERemoteStoragePublishedFileVisibility eVisibility, IntPtr pTags);

        SteamAPICall_t SetUserPublishedFileAction(PublishedFileId_t unPublishedFileId, EWorkshopFileAction eAction);

        SteamAPICall_t EnumeratePublishedFilesByUserAction(EWorkshopFileAction eAction, uint unStartIndex);
        // this method enumerates the public view of workshop files

        SteamAPICall_t EnumeratePublishedWorkshopFiles(EWorkshopEnumerationType eEnumerationType, uint unStartIndex, uint unCount, uint unDays, IntPtr pTags, IntPtr pUserTags);


        SteamAPICall_t UGCDownloadToLocation(UGCHandle_t hContent, string pchLocation, uint unPriority);

    }
    public enum EWorkshopVideoProvider : int
    {
        k_EWorkshopVideoProviderNone = 0,
        k_EWorkshopVideoProviderYoutube = 1
    };
    public enum EWorkshopFileAction : int
    {
        k_EWorkshopFileActionPlayed = 0,
        k_EWorkshopFileActionCompleted = 1,
    };
    public enum EWorkshopEnumerationType : int
    {
        k_EWorkshopEnumerationTypeRankedByVote = 0,
        k_EWorkshopEnumerationTypeRecent = 1,
        k_EWorkshopEnumerationTypeTrending = 2,
        k_EWorkshopEnumerationTypeFavoritesOfFriends = 3,
        k_EWorkshopEnumerationTypeVotedByFriends = 4,
        k_EWorkshopEnumerationTypeContentByFriends = 5,
        k_EWorkshopEnumerationTypeRecentFromFollowedUsers = 6,
    };
    public enum ERemoteStoragePublishedFileVisibility : int
    {
        k_ERemoteStoragePublishedFileVisibilityPublic = 0,
        k_ERemoteStoragePublishedFileVisibilityFriendsOnly = 1,
        k_ERemoteStoragePublishedFileVisibilityPrivate = 2,
        k_ERemoteStoragePublishedFileVisibilityUnlisted = 3,
    };
    public enum ERemoteStoragePlatform : uint
    {
        k_ERemoteStoragePlatformNone = 0,
        k_ERemoteStoragePlatformWindows = (1 << 0),
        k_ERemoteStoragePlatformOSX = (1 << 1),
        k_ERemoteStoragePlatformPS3 = (1 << 2),
        k_ERemoteStoragePlatformLinux = (1 << 3),
        k_ERemoteStoragePlatformSwitch = (1 << 4),
        k_ERemoteStoragePlatformAndroid = (1 << 5),
        k_ERemoteStoragePlatformIOS = (1 << 6),
        // NB we get one more before we need to widen some things

        k_ERemoteStoragePlatformAll = 0xffffffff
    };
}

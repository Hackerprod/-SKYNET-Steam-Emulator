using System;

using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;
using uint16 = System.UInt16;
using uint32 = System.UInt32;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamClient017")]
    public class SteamClient017 : ISteamInterface
    {
        public HSteamPipe CreateSteamPipe(IntPtr _)
        {
            return SteamEmulator.SteamClient.CreateSteamPipe();
        }

        // Releases a previously created communications pipe
        // NOT THREADSAFE - ensure that no other threads are accessing Steamworks API when calling
        public bool BReleaseSteamPipe(IntPtr _, HSteamPipe hSteamPipe)
        {
            return SteamEmulator.SteamClient.BReleaseSteamPipe(hSteamPipe);
        }

        // connects to an existing global user, failing if none exists
        // used by the game to coordinate with the steamUI
        // NOT THREADSAFE - ensure that no other threads are accessing Steamworks API when calling
        public HSteamUser ConnectToGlobalUser(IntPtr _, HSteamPipe hSteamPipe)
        {
            return SteamEmulator.SteamClient.ConnectToGlobalUser(hSteamPipe);
        }

        // used by game servers, create a steam user that won't be shared with anyone else
        // NOT THREADSAFE - ensure that no other threads are accessing Steamworks API when calling
        public HSteamUser CreateLocalUser(IntPtr _, HSteamPipe phSteamPipe, EAccountType eAccountType)
        {
            return SteamEmulator.SteamClient.CreateLocalUser(phSteamPipe, (int)eAccountType);
        }

        // removes an allocated user
        // NOT THREADSAFE - ensure that no other threads are accessing Steamworks API when calling
        public void ReleaseUser(IntPtr _, HSteamPipe hSteamPipe, HSteamUser hUser)
        {
            SteamEmulator.SteamClient.ReleaseUser(hSteamPipe, hUser);
        }

        // retrieves the ISteamUser interface associated with the handle
        public IntPtr GetISteamUser(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamUser(hSteamUser, hSteamPipe, pchVersion);
        }

        // retrieves the ISteamGameServer interface associated with the handle
        public IntPtr GetISteamGameServer(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamGameServer(hSteamUser, hSteamPipe, pchVersion);
        }

        // set the local IP and Port to bind to
        // this must be set before CreateLocalUser(IntPtr _)
        public void SetLocalIPBinding(IntPtr _, uint32 unIP, uint16 usPort)
        {
            SteamEmulator.SteamClient.SetLocalIPBinding(unIP, usPort);
        }

        // returns the ISteamFriends interface
        public IntPtr GetISteamFriends(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamFriends(hSteamUser, hSteamPipe, pchVersion);
        }

        // returns the ISteamUtils interface
        public IntPtr GetISteamUtils(IntPtr _, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamUtils(hSteamPipe, pchVersion);
        }

        // returns the ISteamMatchmaking interface
        public IntPtr GetISteamMatchmaking(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamMatchmaking(hSteamUser, hSteamPipe, pchVersion);
        }

        // returns the ISteamMatchmakingServers interface
        public IntPtr GetISteamMatchmakingServers(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamMatchmakingServers(hSteamUser, hSteamPipe, pchVersion);
        }

        // returns the a generic interface
        public void GetISteamGenericInterface(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            SteamEmulator.SteamClient.GetISteamGenericInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        // returns the ISteamUserStats interface
        public IntPtr GetISteamUserStats(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamUserStats(hSteamUser, hSteamPipe, pchVersion);
        }

        // returns the ISteamGameServerStats interface
        public IntPtr GetISteamGameServerStats(IntPtr _, HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamGameServerStats(hSteamuser, hSteamPipe, pchVersion);
        }

        // returns apps interface
        public IntPtr GetISteamApps(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamApps(hSteamUser, hSteamPipe, pchVersion);
        }

        // networking
        public IntPtr GetISteamNetworking(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamNetworking(hSteamUser, hSteamPipe, pchVersion);
        }

        // remote storage
        public IntPtr GetISteamRemoteStorage(IntPtr _, HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamRemoteStorage(hSteamuser, hSteamPipe, pchVersion);
        }

        // user screenshots
        public IntPtr GetISteamScreenshots(IntPtr _, HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamScreenshots(hSteamuser, hSteamPipe, pchVersion);
        }

        // Deprecated. Applications should use SteamAPI_RunCallbacks(IntPtr _) or SteamGameServer_RunCallbacks(IntPtr _) instead.
        public void RunFrame(IntPtr _)
        {
            SteamEmulator.SteamClient.RunFrame();
        }

        // returns the number of IPC calls made since the last time this function was called
        // Used for perf debugging so you can understand how many IPC calls your game makes per frame
        // Every IPC call is at minimum a thread context switch if not a process one so you want to rate
        // control how often you do them.
        public uint32 GetIPCCallCount(IntPtr _)
        {
            return SteamEmulator.SteamClient.GetIPCCallCount();
        }

        // API warning handling
        // 'int' is the severity; 0 for msg, 1 for warning
        // ' char ' is the text of the message
        // callbacks will occur directly after the API function is called that generated the warning or message.
        public void SetWarningMessageHook(IntPtr _, IntPtr pFunction)
        {
            SteamEmulator.SteamClient.SetWarningMessageHook(pFunction);
        }

        // Trigger global shutdown for the DLL
        public bool BShutdownIfAllPipesClosed(IntPtr _)
        {
            return SteamEmulator.SteamClient.BShutdownIfAllPipesClosed();
        }

        // Expose HTTP interface
        public IntPtr GetISteamHTTP(IntPtr _, HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamHTTP(hSteamuser, hSteamPipe, pchVersion);
        }

        // Deprecated - the ISteamUnifiedMessages interface is no longer intended for public consumption.
        public void DEPRECATED_GetISteamUnifiedMessages(IntPtr _, HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            SteamEmulator.SteamClient.DEPRECATED_GetISteamUnifiedMessages(hSteamuser, hSteamPipe, pchVersion);
        }

        // Exposes the ISteamController interface
        public IntPtr GetISteamController(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamController(hSteamUser, hSteamPipe, pchVersion);
        }

        // Exposes the ISteamUGC interface
        public IntPtr GetISteamUGC(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamUGC(hSteamUser, hSteamPipe, pchVersion);
        }

        // returns app list interface, only available on specially registered apps
        public IntPtr GetISteamAppList(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamAppList(hSteamUser, hSteamPipe, pchVersion);
        }

        // Music Player
        public IntPtr GetISteamMusic(IntPtr _, HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamMusic(hSteamuser, hSteamPipe, pchVersion);
        }

        // Music Player Remote
        public IntPtr GetISteamMusicRemote(IntPtr _, HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamMusicRemote(hSteamuser, hSteamPipe, pchVersion);
        }

        // html page display
        public IntPtr GetISteamHTMLSurface(IntPtr _, HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamHTMLSurface(hSteamuser, hSteamPipe, pchVersion);
        }

        // Helper functions for internal Steam usage
        public void DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess(IntPtr _, IntPtr msg )
        {
            SteamEmulator.SteamClient.DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess(msg);
        }

        public void DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess(IntPtr _, IntPtr msg)
        {
            SteamEmulator.SteamClient.DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess(msg);
        }

        public void Set_SteamAPI_CCheckCallbackRegisteredInProcess(IntPtr _, IntPtr func)
        {
            SteamEmulator.SteamClient.Set_SteamAPI_CCheckCallbackRegisteredInProcess(func);
        }

        // inventory
        public IntPtr GetISteamInventory(IntPtr _, HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamInventory(hSteamuser, hSteamPipe, pchVersion);
        }

        // Video
        public IntPtr GetISteamVideo(IntPtr _, HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamVideo(hSteamuser, hSteamPipe, pchVersion);
        }

        // Parental controls
        public IntPtr GetISteamParentalSettings(IntPtr _, HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamParentalSettings(hSteamuser, hSteamPipe, pchVersion);
        }
    }
}

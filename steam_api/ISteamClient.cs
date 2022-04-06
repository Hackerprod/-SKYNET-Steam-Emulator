using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks;
using Steamworks;

namespace SKYNET.Steamworks.Implementation
{
    public interface ISteamClient
    {
        void SetAppId(uint appId);

        int CreateSteamPipe(IntPtr _);

        bool BReleaseSteamPipe(int hSteamPipe);

        int ConnectToGlobalUser(int hSteamPipe);

        int CreateLocalUser(out int phSteamPipe, EAccountType eAccountType);

        void ReleaseUser(int hSteamPipe, int hUser);

        void SetLocalIPBinding(uint unIP, ushort usPort);

        uint GetIPCCallCount(IntPtr _);

        bool BShutdownIfAllPipesClosed(IntPtr _);

        #region Interfaces

        IntPtr GetISteamUser(int hSteamUser, int hSteamPipe, string pchVersion);

        IntPtr GetISteamGameServer(int hSteamUser, int hSteamPipe, string pchVersion);

        IntPtr GetISteamFriends(int user, int pipe, string pchVersion);

        IntPtr GetISteamGameSearch(int hSteamUser, int hSteamPipe, string pchVersion);

        IntPtr GetISteamUtils(int hSteamPipe, string pchVersion);

        IntPtr GetISteamMatchmaking(int hSteamUser, int hSteamPipe, string pchVersion);

        IntPtr GetISteamMatchmakingServers(int hSteamUser, int hSteamPipe, string pchVersion);

        IntPtr GetISteamGenericInterface(int hSteamUser, int hSteamPipe, string pchVersion);

        IntPtr GetISteamUserStats(int hSteamUser, int hSteamPipe, string pchVersion);

        IntPtr GetISteamGameServerStats(int hSteamUser, int hSteamPipe, string pchVersion);

        IntPtr GetISteamApps(int hSteamUser, int hSteamPipe, string pchVersion);

        IntPtr GetISteamNetworking(int hSteamUser, int hSteamPipe, string pchVersion);

        IntPtr GetISteamRemoteStorage(int hSteamUser, int hSteamPipe, string pchVersion);

        IntPtr GetISteamScreenshots(int hSteamUser, int hSteamPipe, [MarshalAs(UnmanagedType.LPStr)] string sdsd);

        IntPtr GetISteamHTTP(int hSteamUser, int hSteamPipe, IntPtr pchVersion);


        IntPtr GetISteamController(int hSteamUser, int hSteamPipe, string pchVersion);

        IntPtr GetISteamUGC(int hSteamUser, int hSteamPipe, string pchVersion);

        IntPtr GetISteamAppList(int hSteamUser, int hSteamPipe, string pchVersion);

        IntPtr GetISteamMusic(int hSteamUser, int hSteamPipe, string pchVersion);

        IntPtr GetISteamMusicRemote(int hSteamUser, int hSteamPipe, string pchVersion);

        IntPtr GetISteamInput(int hSteamUser, int hSteamPipe, string pchVersion);

        IntPtr GetISteamHTMLSurface(int hSteamUser, int hSteamPipe, string pchVersion);

        IntPtr GetISteamParties(int hSteamUser, int hSteamPipe, string pchVersion);

        IntPtr GetISteamInventory(int hSteamUser, int hSteamPipe, string pchVersion);

        IntPtr GetISteamRemotePlay(int hSteamUser, int hSteamPipe, string pchVersion);

        IntPtr GetISteamVideo(int hSteamUser, int hSteamPipe, string pchVersion);

        IntPtr GetISteamParentalSettings(int hSteamUser, int hSteamPipe, string pchVersion);

        #endregion

        void SetPersonaName(string pchPersonaName);

        void SetWarningMessageHook(SteamAPIWarningMessageHook_t pFunction);

        }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steamworks;

namespace SKYNET.Interface
{
    public interface ISteamClient
    {
        int CreateSteamPipe(IntPtr _);
        bool BReleaseSteamPipe(int hSteamPipe);
        int ConnectToGlobalUser(int hSteamPipe);
        int CreateLocalUser(out int phSteamPipe, EAccountType eAccountType);
        void ReleaseUser(int hSteamPipe, int hUser);
        IntPtr GetISteamUser(int hSteamuser, int hSteamPipe, string pchVersion);
        IntPtr GetISteamGameServer(int hSteamuser, int hSteamPipe, string pchVersion);
        void SetLocalIPBinding(uint unIP, ushort usPort);
        IntPtr GetISteamFriends(int hSteamuser, int hSteamPipe, string pchVersion);
        IntPtr GetISteamUtils(int hSteamPipe, string pchVersion);
        IntPtr GetISteamMatchmaking(int hSteamUser, int hSteamPipe, string pchVersion);
        IntPtr GetISteamMatchmakingServers(int hSteamUser, int hSteamPipe, string pchVersion);
        IntPtr GetISteamGenericInterface(int hSteamUser, int hSteamPipe, string pchVersion);
        IntPtr GetISteamUserStats(int hSteamUser, int hSteamPipe, string pchVersion);
        IntPtr GetISteamGameServerStats(int hSteamuser, int hSteamPipe, string pchVersion);
        IntPtr GetISteamApps(int hSteamUser, int hSteamPipe, string pchVersion);
        IntPtr GetISteamNetworking(int hSteamUser, int hSteamPipe, string pchVersion);
        IntPtr GetISteamRemoteStorage(int hSteamuser, int hSteamPipe, string pchVersion);
        IntPtr GetISteamScreenshots(int hSteamuser, int hSteamPipe, string pchVersion);
        uint GetIPCCallCount(IntPtr _);
        bool BShutdownIfAllPipesClosed(IntPtr _);
        IntPtr GetISteamHTTP(int hSteamuser, int hSteamPipe, string pchVersion);
        IntPtr GetISteamController(int hSteamuser, int hSteamPipe, string pchVersion);
        IntPtr GetISteamUGC(int hSteamuser, int hSteamPipe, string pchVersion);
        IntPtr GetISteamAppList(int hSteamuser, int hSteamPipe, string pchVersion);
        IntPtr GetISteamMusic(int hSteamuser, int hSteamPipe, string pchVersion);
        IntPtr GetISteamMusicRemote(int hSteamuser, int hSteamPipe, string pchVersion);
        IntPtr GetISteamHTMLSurface(int hSteamuser, int hSteamPipe, string pchVersion);
        IntPtr GetISteamInventory(int hSteamuser, int hSteamPipe, string pchVersion);
        IntPtr GetISteamVideo(int hSteamuser, int hSteamPipe, string pchVersion);
        IntPtr GetISteamParentalSettings(int hSteamuser, int hSteamPipe, string pchVersion);
    }
}
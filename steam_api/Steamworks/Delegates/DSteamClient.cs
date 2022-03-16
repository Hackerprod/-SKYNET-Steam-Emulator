using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SKYNET.Interface;
using Steamworks;

namespace SKYNET.Delegate
{
    [Delegate("SteamClient")]
    public class DSteamClient : SteamDelegate
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate HSteamPipe CreateSteamPipe();
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BReleaseSteamPipe(HSteamPipe hSteamPipe);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate HSteamUser ConnectToGlobalUser(HSteamPipe hSteamPipe);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate HSteamUser CreateLocalUser(out HSteamPipe phSteamPipe, EAccountType eAccountType);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ReleaseUser(HSteamPipe hSteamPipe, HSteamUser hUser);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ISteamUser GetISteamUser(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ISteamGameServer GetISteamGameServer(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetLocalIPBinding(uint unIP, ushort usPort);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ISteamFriends GetISteamFriends(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ISteamUtils GetISteamUtils(HSteamPipe hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ISteamMatchmaking GetISteamMatchmaking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ISteamMatchmakingServers GetISteamMatchmakingServers(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamGenericInterface(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ISteamUserStats GetISteamUserStats(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ISteamGameServerStats GetISteamGameServerStats(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ISteamApps GetISteamApps(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ISteamNetworking GetISteamNetworking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ISteamRemoteStorage GetISteamRemoteStorage(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ISteamScreenshots GetISteamScreenshots(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetIPCCallCount();
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BShutdownIfAllPipesClosed();
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ISteamHTTP GetISteamHTTP(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ISteamController GetISteamController(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ISteamUGC GetISteamUGC(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ISteamAppList GetISteamAppList(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ISteamMusic GetISteamMusic(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ISteamMusicRemote GetISteamMusicRemote(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ISteamHTMLSurface GetISteamHTMLSurface(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ISteamInventory GetISteamInventory(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ISteamVideo GetISteamVideo(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ISteamParentalSettings GetISteamParentalSettings(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion);
    }
}
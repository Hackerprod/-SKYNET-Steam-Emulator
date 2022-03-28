using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SKYNET.Steamworks;
using Steamworks;

namespace SKYNET.Delegate
{
    [Delegate(Name = "SteamClient")]
    public class DSteamClient
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetAppId(uint appId);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int CreateSteamPipe(IntPtr _);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BReleaseSteamPipe(int hSteamPipe);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int ConnectToGlobalUser(int hSteamPipe);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int CreateLocalUser(out int phSteamPipe, EAccountType eAccountType);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ReleaseUser(int hSteamPipe, int hUser);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamUser(int hSteamUser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamGameServer(int hSteamUser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetLocalIPBinding(uint unIP, ushort usPort);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamFriends(int hSteamUser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamGameSearch(int hSteamUser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamUtils(int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamMatchmaking(int hSteamUser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamMatchmakingServers(int hSteamUser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamGenericInterface(int hSteamUser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamUserStats(int hSteamUser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamGameServerStats(int hSteamuser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamApps(int hSteamUser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamNetworking(int hSteamUser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamRemoteStorage(int hSteamuser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamScreenshots(int hSteamuser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetIPCCallCount(IntPtr _);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BShutdownIfAllPipesClosed(IntPtr _);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamHTTP(int hSteamuser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamController(int hSteamUser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamUGC(int hSteamUser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamAppList(int hSteamUser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamMusic(int hSteamuser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamMusicRemote(int hSteamuser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamInput(int hSteamUser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamHTMLSurface(int hSteamuser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamParties(int hSteamUser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamInventory(int hSteamuser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamRemotePlay(int hSteamUser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamVideo(int hSteamuser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetISteamParentalSettings(int hSteamuser, int hSteamPipe, string pchVersion);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetPersonaName(string pchPersonaName);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetWarningMessageHook(SteamAPIWarningMessageHook_t pFunction);
    }
}
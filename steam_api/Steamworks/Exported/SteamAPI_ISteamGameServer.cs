using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SKYNET.Helpers;
using Steamworks;

namespace SKYNET.Steamworks.Exported
{
    public partial class SteamAPI_SteamGameServer 
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamGameServer_GetHSteamUser()
        {
            Write("SteamGameServer_GetHSteamUser");
            return SteamEmulator.HSteamUser_GS;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamGameServer_GetHSteamPipe()
        {
            Write("SteamGameServer_GetHSteamPipe");
            return SteamEmulator.HSteamPipe_GS;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamGameServer_Init(uint unIP, int usGamePort, int usQueryPort, uint unFlags, uint nGameAppId, string pchVersionString)
        {
            Write("SteamGameServer_Init");
            return SteamEmulator.SteamGameServer.InitGameServer(unIP, usGamePort, usQueryPort, unFlags, nGameAppId, pchVersionString);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamGameServer_InitSafe(IntPtr unIP, IntPtr usSteamPort, IntPtr usGamePort, IntPtr unknown, IntPtr eServerMode, IntPtr unknown1, IntPtr unknown2, IntPtr unknown3)
        {
            Write("SteamGameServer_InitSafe");
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamGameServer_Shutdown()
        {
            Write("SteamGameServer_Shutdown");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamGameServer_RunCallbacks()
        {
            Write("SteamGameServer_RunCallbacks");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamGameServer_BSecure()
        {
            Write("SteamGameServer_BSecure");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamGameServer_GetSteamID()
        {
            Write("SteamGameServer_GetSteamID");
            return SteamEmulator.SteamId_GS;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamGameServer_GetIPCCallCount()
        {
            Write("SteamGameServer_GetIPCCallCount");
            return 0;
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("SteamAPI_SteamGameServer", msg);
        }
    }
}
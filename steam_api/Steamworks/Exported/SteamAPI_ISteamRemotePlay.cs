using SKYNET;
using SKYNET.Steamworks;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamRemotePlay 
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamRemotePlay_GetSessionCount(IntPtr _)
        {
            Write("SteamAPI_ISteamRemotePlay_GetSessionCount");
            return SteamEmulator.SteamRemotePlay.GetSessionCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamRemotePlay_GetSessionID(IntPtr _, int iSessionIndex)
        {
            Write("SteamAPI_ISteamRemotePlay_GetSessionID");
            return SteamEmulator.SteamRemotePlay.GetSessionID(iSessionIndex);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static CSteamID SteamAPI_ISteamRemotePlay_GetSessionSteamID(IntPtr _, uint unSessionID)
        {
            Write("SteamAPI_ISteamRemotePlay_IntPtrGetSessionSteamID");
            return SteamEmulator.SteamRemotePlay.GetSessionSteamID(unSessionID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamRemotePlay_GetSessionClientName(IntPtr _, uint unSessionID)
        {
            Write("SteamAPI_ISteamRemotePlay_stringGetSessionClientName");
            return SteamEmulator.SteamRemotePlay.GetSessionClientName(unSessionID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamRemotePlay_GetSessionClientFormFactor(IntPtr _, uint unSessionID)
        {
            Write("SteamAPI_ISteamRemotePlay_GetSessionClientFormFactor");
            return SteamEmulator.SteamRemotePlay.GetSessionClientFormFactor(unSessionID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemotePlay_BGetSessionClientResolution(IntPtr _, uint unSessionID, int pnResolutionX, int pnResolutionY)
        {
            Write("SteamAPI_ISteamRemotePlay_BGetSessionClientResolution");
            return SteamEmulator.SteamRemotePlay.BGetSessionClientResolution(unSessionID, pnResolutionX, pnResolutionY);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemotePlay_BSendRemotePlayTogetherInvite(IntPtr _, ulong steamIDFriend)
        {
            Write("SteamAPI_ISteamRemotePlay_BSendRemotePlayTogetherInvite");
            return SteamEmulator.SteamRemotePlay.BSendRemotePlayTogetherInvite(steamIDFriend);
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}


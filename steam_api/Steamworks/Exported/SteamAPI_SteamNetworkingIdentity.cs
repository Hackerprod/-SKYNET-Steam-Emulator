using System;
using System.Runtime.InteropServices;
using SKYNET.Helpers;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_SteamNetworkingIdentity
    {
        static SteamAPI_SteamNetworkingIdentity()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIdentity_Clear(IntPtr _)
        {
            SteamNetworkingIdentityInterop.Clear(_);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_SteamNetworkingIdentity_GetGenericBytes(IntPtr _, int cbLen)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_GetGenericBytes");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_SteamNetworkingIdentity_GetGenericString(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_GetGenericString");
            return "";
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamNetworkingIdentity_GetIPAddr(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_GetIPAddr");
            return default;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamNetworkingIdentity_GetSteamID(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_GetSteamID");
            return default;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_SteamNetworkingIdentity_GetSteamID64(IntPtr _)
        {
            return SteamNetworkingIdentityInterop.TryReadSteamId(_, out var steamId) ? steamId : 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_SteamNetworkingIdentity_GetXboxPairwiseID(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_GetXboxPairwiseID");
            return "";
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIdentity_IsEqualTo(IntPtr _, IntPtr x)
        {
            return SteamNetworkingIdentityInterop.TryReadSteamId(_, out var first) &&
                   SteamNetworkingIdentityInterop.TryReadSteamId(x, out var second) &&
                   first == second;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIdentity_IsInvalid(IntPtr _)
        {
            ulong ignoredSteamId;
            return !SteamNetworkingIdentityInterop.TryReadSteamId(_, out ignoredSteamId);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIdentity_IsLocalHost(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_IsLocalHost");
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIdentity_ParseString(IntPtr _, string pszStr)
        {
            if (!SteamNetworkingIdentityInterop.TryParseSteamId(pszStr, out var steamId))
            {
                SteamNetworkingIdentityInterop.Clear(_);
                return false;
            }
            SteamNetworkingIdentityInterop.WriteSteamId(_, steamId);
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIdentity_SetGenericBytes(IntPtr _, IntPtr data, uint cbLen)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_SetGenericBytes");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIdentity_SetIPAddr(IntPtr _, IntPtr addr)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_SetIPAddr");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIdentity_SetLocalHost(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_SetLocalHost");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIdentity_SetSteamID(IntPtr _, IntPtr steamID)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_SetSteamID");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIdentity_SetSteamID64(IntPtr _, ulong steamID)
        {
            SteamNetworkingIdentityInterop.WriteSteamId(_, steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIdentity_SetXboxPairwiseID(IntPtr _, string pszString)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_SetXboxPairwiseID");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIdentity_ToString(IntPtr _, IntPtr buf, UIntPtr cbBuf)
        {
            NativeStringCache.WriteUtf8Buffer(buf, checked((int)cbBuf.ToUInt64()), SteamNetworkingIdentityInterop.Format(_));
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_SteamNetworkingIdentityRender_c_str(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_GetSteamID64");
            return "";
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}

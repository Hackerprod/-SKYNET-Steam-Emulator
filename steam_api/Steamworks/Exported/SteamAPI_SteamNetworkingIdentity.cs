using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_SteamNetworkingIdentity 
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIdentity_Clear(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_Clear");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_SteamNetworkingIdentity_GetGenericBytes(IntPtr _, int cbLen)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_GetGenericBytes");
            return 0;
            //return self.GetGenericBytes(cbLen);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_SteamNetworkingIdentity_GetGenericString(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_GetGenericString");
            return "";
            //return self.GetGenericString();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamNetworkingIdentity_GetIPAddr(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_GetIPAddr");
            return default;
            //return self.GetIPAddr();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamNetworkingIdentity_GetSteamID(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_GetSteamID");
            return default;
            //return self.GetSteamID().ConvertToUint64();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_SteamNetworkingIdentity_GetSteamID64(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_GetSteamID64");
            return 0;
            //return self.GetSteamID64();
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
            Write($"SteamAPI_SteamNetworkingIdentity_IsEqualTo");
            return false;
            //return self.operator==(x);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIdentity_IsInvalid(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_IsInvalid");
            return false;
            //return self.IsInvalid();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIdentity_IsLocalHost(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_IsLocalHost");
            return true;
            //return self.IsLocalHost();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIdentity_ParseString(IntPtr _, string pszStr)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_GetSteamID64");
            return true;
            //return self.ParseString(pszStr);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIdentity_SetGenericBytes(IntPtr _, IntPtr data, uint cbLen)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_SetGenericBytes");
            return false;
            //return self.SetGenericBytes(data, cbLen);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIdentity_SetIPAddr(IntPtr _, IntPtr addr)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_SetIPAddr");
            //self.SetIPAddr(addr);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIdentity_SetLocalHost(IntPtr _)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_SetLocalHost");
            //self.SetLocalHost();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIdentity_SetSteamID(IntPtr _, IntPtr steamID)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_SetSteamID");
            //self.SetSteamID(steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIdentity_SetSteamID64(IntPtr _, uint steamID)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_SetSteamID64");
            //self.SetSteamID64(steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIdentity_SetXboxPairwiseID(IntPtr _, string pszString)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_SetXboxPairwiseID");
            return false;
            //return false;//self.SetXboxPairwiseID(pszString);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIdentity_ToString(IntPtr _, string buf, uint cbBuf)
        {
            Write($"SteamAPI_SteamNetworkingIdentity_ToString");
            //self.ToString(buf, cbBuf);
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

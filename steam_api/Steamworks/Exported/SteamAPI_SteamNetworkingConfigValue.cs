using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_SteamNetworkingConfigValue
    {
        static SteamAPI_SteamNetworkingConfigValue()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingConfigValue_t_SetInt32(IntPtr _, IntPtr eVal, IntPtr data)
        {
            Write($"SteamAPI_SteamNetworkingConfigValue_t_SetInt32");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingConfigValue_t_SetInt64(IntPtr _, IntPtr eVal, IntPtr data)
        {
            Write($"SteamAPI_SteamNetworkingConfigValue_t_SetInt64");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingConfigValue_t_SetFloat(IntPtr _, IntPtr eVal, float data)
        {
            Write($"SteamAPI_SteamNetworkingConfigValue_t_SetFloat");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingConfigValue_t_SetPtr(IntPtr _, IntPtr eVal, IntPtr data)
        {
            Write($"SteamAPI_SteamNetworkingConfigValue_t_SetPtr");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingConfigValue_t_SetString(IntPtr _, IntPtr eVal, IntPtr data)
        {
            Write($"SteamAPI_SteamNetworkingConfigValue_t_SetString");
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}

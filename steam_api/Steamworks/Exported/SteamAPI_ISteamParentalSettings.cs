using SKYNET;
using SKYNET.Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamParentalSettings 
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamParentalSettings_BIsParentalLockEnabled(IntPtr _)
        {
            Write("SteamAPI_ISteamParentalSettings_BIsParentalLockEnabled");
            return SteamEmulator.SteamParentalSettings.BIsParentalLockEnabled();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamParentalSettings_BIsParentalLockLocked(IntPtr _)
        {
            Write("SteamAPI_ISteamParentalSettings_BIsParentalLockLocked");
            return SteamEmulator.SteamParentalSettings.BIsParentalLockLocked();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamParentalSettings_BIsAppBlocked(IntPtr _, uint nAppID)
        {
            Write("SteamAPI_ISteamParentalSettings_BIsAppBlocked");
            return SteamEmulator.SteamParentalSettings.BIsAppBlocked(nAppID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamParentalSettings_BIsAppInBlockList(IntPtr _, uint nAppID)
        {
            Write("SteamAPI_ISteamParentalSettings_BIsAppInBlockList");
            return SteamEmulator.SteamParentalSettings.BIsAppInBlockList(nAppID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamParentalSettings_BIsFeatureBlocked(IntPtr _, int eFeature)
        {
            Write("SteamAPI_ISteamParentalSettings_BIsFeatureBlocked");
            return SteamEmulator.SteamParentalSettings.BIsFeatureBlocked(eFeature);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamParentalSettings_BIsFeatureInBlockList(IntPtr _, int eFeature)
        {
            Write("SteamAPI_ISteamParentalSettings_BIsFeatureInBlockList");
            return SteamEmulator.SteamParentalSettings.BIsFeatureInBlockList(eFeature);
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}


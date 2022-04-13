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
    public class SteamAPI_ISteamParentalSettings : BaseCalls
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamParentalSettings_BIsParentalLockEnabled(IntPtr _)
        {
            Write("SteamAPI_ISteamParentalSettings_BIsParentalLockEnabled");
            return SteamEmulator.SteamParentalSettings.BIsParentalLockEnabled(_);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamParentalSettings_BIsParentalLockLocked(IntPtr _)
        {
            Write("SteamAPI_ISteamParentalSettings_BIsParentalLockLocked");
            return SteamEmulator.SteamParentalSettings.BIsParentalLockLocked(_);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamParentalSettings_BIsAppBlocked(uint nAppID)
        {
            Write("SteamAPI_ISteamParentalSettings_BIsAppBlocked");
            return SteamEmulator.SteamParentalSettings.BIsAppBlocked(nAppID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamParentalSettings_BIsAppInBlockList(uint nAppID)
        {
            Write("SteamAPI_ISteamParentalSettings_BIsAppInBlockList");
            return SteamEmulator.SteamParentalSettings.BIsAppInBlockList(nAppID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamParentalSettings_BIsFeatureBlocked(int eFeature)
        {
            Write("SteamAPI_ISteamParentalSettings_BIsFeatureBlocked");
            return SteamEmulator.SteamParentalSettings.BIsFeatureBlocked(eFeature);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamParentalSettings_BIsFeatureInBlockList(int eFeature)
        {
            Write("SteamAPI_ISteamParentalSettings_BIsFeatureInBlockList");
            return SteamEmulator.SteamParentalSettings.BIsFeatureInBlockList(eFeature);
        }
    }
}


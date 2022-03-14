using SKYNET.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class SteamAPI_ISteamParentalSettings : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamParentalSettings_BIsParentalLockEnabled()
    {
        Write("SteamAPI_ISteamParentalSettings_BIsParentalLockEnabled");
        return SteamClient.SteamParentalSettings.BIsParentalLockEnabled();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamParentalSettings_BIsParentalLockLocked()
    {
        Write("SteamAPI_ISteamParentalSettings_BIsParentalLockLocked");
        return SteamClient.SteamParentalSettings.BIsParentalLockLocked();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamParentalSettings_BIsAppBlocked(IntPtr nAppID)
    {
        Write("SteamAPI_ISteamParentalSettings_BIsAppBlocked");
        return SteamClient.SteamParentalSettings.BIsAppBlocked(nAppID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamParentalSettings_BIsAppInBlockList(IntPtr nAppID)
    {
        Write("SteamAPI_ISteamParentalSettings_BIsAppInBlockList");
        return SteamClient.SteamParentalSettings.BIsAppInBlockList(nAppID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamParentalSettings_BIsFeatureBlocked(EParentalFeature eFeature)
    {
        Write("SteamAPI_ISteamParentalSettings_BIsFeatureBlocked");
        return SteamClient.SteamParentalSettings.BIsFeatureBlocked(eFeature);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamParentalSettings_BIsFeatureInBlockList(EParentalFeature eFeature)
    {
        Write("SteamAPI_ISteamParentalSettings_BIsFeatureInBlockList");
        return SteamClient.SteamParentalSettings.BIsFeatureInBlockList(eFeature);
    }

}


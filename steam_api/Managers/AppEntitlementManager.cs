using SKYNET.Steamworks;

namespace SKYNET.Managers
{
    /// <summary>
    /// Central license decision used by SteamApps, SteamUser and SteamGameServer.
    /// Games are not consistent about which Steamworks surface they use for DLC
    /// checks, so all entitlement-facing APIs must answer from the same state.
    /// </summary>
    public static class AppEntitlementManager
    {
        public static bool HasLicense(uint appId)
        {
            if (appId == 0 || appId == uint.MaxValue)
            {
                return false;
            }

            return appId == SteamEmulator.AppID
                || DLCManager.IsOwned(appId)
                || AppContentManager.IsAppInstalled(appId);
        }

        public static EUserHasLicenseForAppResult GetLicenseResult(uint appId)
        {
            return HasLicense(appId)
                ? EUserHasLicenseForAppResult.k_EUserHasLicenseResultHasLicense
                : EUserHasLicenseForAppResult.k_EUserHasLicenseResultDoesNotHaveLicense;
        }
    }
}

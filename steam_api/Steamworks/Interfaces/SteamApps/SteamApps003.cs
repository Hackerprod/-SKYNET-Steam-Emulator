using System;

namespace SKYNET.Steamworks.Interfaces
{
    /// <summary>Legacy ISteamApps ABI used by Left 4 Dead.</summary>
    [Interface("STEAMAPPS_INTERFACE_VERSION003")]
    public class SteamApps003 : ISteamInterface
    {
        public bool BIsSubscribed(IntPtr _) => SteamEmulator.SteamApps.BIsSubscribed();
        public bool BIsLowViolence(IntPtr _) => SteamEmulator.SteamApps.BIsLowViolence();
        public bool BIsCybercafe(IntPtr _) => SteamEmulator.SteamApps.BIsCybercafe();
        public bool BIsVACBanned(IntPtr _) => SteamEmulator.SteamApps.BIsVACBanned();
        public IntPtr GetCurrentGameLanguage(IntPtr _) => SteamEmulator.SteamApps.GetCurrentGameLanguage();
        public IntPtr GetAvailableGameLanguages(IntPtr _) => SteamEmulator.SteamApps.GetAvailableGameLanguages();
        public bool BIsSubscribedApp(IntPtr _, uint appID) => SteamEmulator.SteamApps.BIsSubscribedApp(appID);
        public bool BIsDlcInstalled(IntPtr _, uint appID) => SteamEmulator.SteamApps.BIsDlcInstalled(appID);
    }
}

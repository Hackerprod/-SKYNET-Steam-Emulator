using System;

namespace SKYNET.Steamworks.Interfaces
{
    [InterfaceLayout("STEAMPARENTALSETTINGS_INTERFACE_VERSION001",
        "BIsParentalLockEnabled", "BIsParentalLockLocked", "BIsAppBlocked", "BIsAppInBlockList",
        "BIsFeatureBlocked", "BIsFeatureInBlockList")]
    public class SteamParentalSettings001 : ISteamInterface
    {
        public bool BIsParentalLockEnabled(IntPtr _)
        {
            return SteamEmulator.SteamParentalSettings.BIsParentalLockEnabled();
        }

        public bool BIsParentalLockLocked(IntPtr _)
        {
            return SteamEmulator.SteamParentalSettings.BIsParentalLockLocked();
        }

        public bool BIsAppBlocked(IntPtr _, uint nAppID)
        {
            return SteamEmulator.SteamParentalSettings.BIsAppBlocked(nAppID);
        }

        public bool BIsAppInBlockList(IntPtr _, uint nAppID)
        {
            return SteamEmulator.SteamParentalSettings.BIsAppInBlockList(nAppID);
        }

        public bool BIsFeatureBlocked(IntPtr _, int eFeature)
        {
            return SteamEmulator.SteamParentalSettings.BIsFeatureBlocked(eFeature);
        }

        public bool BIsFeatureInBlockList(IntPtr _, int eFeature)
        {
            return SteamEmulator.SteamParentalSettings.BIsFeatureInBlockList(eFeature);
        }


    }
}

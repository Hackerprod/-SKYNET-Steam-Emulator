using SKYNET.Interface;
using System;

namespace SKYNET.Managers
{
    [Interface("STEAMPARENTALSETTINGS_INTERFACE_VERSION")]
    [Interface("SteamParentalSettings")]
    public class SteamParentalSettings : SteamInterface, ISteamParentalSettings
    {
        public bool BIsParentalLockEnabled()
        {
            return false;
        }

        public bool BIsParentalLockLocked()
        {
            return false;
        }

        public bool BIsAppBlocked(IntPtr nAppID)
        {
            return false;
        }

        public bool BIsAppInBlockList(IntPtr nAppID)
        {
            return false;
        }

        public bool BIsFeatureBlocked(EParentalFeature eFeature)
        {
            return false;
        }

        public bool BIsFeatureInBlockList(EParentalFeature eFeature)
        {
            return false;
        }

    }

}
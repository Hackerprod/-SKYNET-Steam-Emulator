using Core.Interface;
using SKYNET.Interface;
using System;

namespace SKYNET.Managers
{
    //[Map("STEAMPARENTALSETTINGS_INTERFACE_VERSION")]
    //[Map("SteamParentalSettings")]
    public class SteamParentalSettings : IBaseInterface, ISteamParentalSettings
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
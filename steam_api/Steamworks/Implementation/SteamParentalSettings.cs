
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamParentalSettings : ISteamInterface
    {
        public SteamParentalSettings()
        {
            InterfaceVersion = "SteamParentalSettings";
        }

        public bool BIsParentalLockEnabled(IntPtr _)
        {
            Write("boolBIsParentalLockEnabled");
            return false;
        }

        public bool BIsParentalLockLocked(IntPtr _)
        {
            Write("boolBIsParentalLockLocked");
            return false;
        }

        public bool BIsAppBlocked(uint nAppID)
        {
            Write("boolBIsAppBlocked");
            return false;
        }

        public bool BIsAppInBlockList(uint nAppID)
        {
            Write("boolBIsAppInBlockList");
            return false;
        }

        public bool BIsFeatureBlocked(int eFeature)
        {
            Write("boolBIsFeatureBlocked");
            return false;
        }

        public bool BIsFeatureInBlockList(int eFeature)
        {
            Write("boolBIsFeatureInBlockList");
            return false;
        }
    }
}
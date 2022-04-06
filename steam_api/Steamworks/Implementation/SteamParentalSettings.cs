
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using System;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamParentalSettings : ISteamInterface
    {
        public IntPtr MemoryAddress { get; set; }
        public string InterfaceVersion { get; set; }

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

        public bool BIsAppBlocked(IntPtr nAppID)
        {
            Write("boolBIsAppBlocked");
            return false;
        }

        public bool BIsAppInBlockList(IntPtr nAppID)
        {
            Write("boolBIsAppInBlockList");
            return false;
        }

        public bool BIsFeatureBlocked(EParentalFeature eFeature)
        {
            Write("boolBIsFeatureBlocked");
            return false;
        }

        public bool BIsFeatureInBlockList(EParentalFeature eFeature)
        {
            Write("boolBIsFeatureInBlockList");
            return false;
        }

        private void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }
    }
}
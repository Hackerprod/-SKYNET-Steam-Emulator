
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Implementation
{
    [StructLayout(LayoutKind.Sequential)]
    public class SteamParentalSettings : ISteamInterface
    {
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

        public IntPtr MemoryAddress { get; set; }
        public string InterfaceVersion { get; set; }

        public SteamParentalSettings()
        {
            InterfaceVersion = "SteamParentalSettings";
        }

        private void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }
    }
}
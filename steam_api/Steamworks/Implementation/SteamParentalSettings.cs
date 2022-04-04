
using SKYNET;
using SKYNET.Helper;
using SKYNET.Steamworks;
using System;

public class SteamParentalSettings : ISteamInterface
{
    public IntPtr MemoryAddress { get; set; }
    public string InterfaceVersion { get; set; }


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
        Log.Write(InterfaceVersion, v);
    }
}
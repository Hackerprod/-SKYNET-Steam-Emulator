using Core.Interface;
using SKYNET;
using SKYNET.Steamworks;
using System;

//[Map("STEAMPARENTALSETTINGS_INTERFACE_VERSION")]
//[Map("SteamParentalSettings")]
public class SteamParentalSettings : IBaseInterface
{
    public bool BIsParentalLockEnabled(IntPtr _)
    {
        return false;
    }

    public bool BIsParentalLockLocked(IntPtr _)
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

    private void Write(string v)
    {
        Main.Write(InterfaceVersion, v);
    }
}
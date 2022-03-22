using Core.Interface;
using SKYNET.Interface;
using System;

//[Map("STEAMTV_INTERFACE_V001")]
//[Map("SteamScreenshots")]
public class SteamTV : IBaseInterface, ISteamTV
{
    public bool IsBroadcasting(int pnNumViewers)
    {
        return false;
    }

    public void AddBroadcastGameData(string pchKey, string pchValue)
    {
        //
    }

    public void RemoveBroadcastGameData(string pchKey)
    {
        //
    }

    public void AddTimelineMarker(string pchTemplateName, bool bPersistent, uint nColorR, uint nColorG, uint nColorB)
    {
        //
    }

    public void RemoveTimelineMarker()
    {
        //
    }

    public uint AddRegion(string pchElementName, string pchTimelineDataSection, IntPtr pSteamTVRegion, int eSteamTVRegionBehavior)
    {
        return 0;
    }

    public void RemoveRegion(uint unRegionHandle)
    {
        //
    }

}
using SKYNET;
using SKYNET.Steamworks;
using Steamworks;
using System;
using System.Runtime.InteropServices;

public class SteamParties : SteamInterface
{
    public void CancelReservation(uint ulBeacon, IntPtr steamIDUser)
    {
        Write("CancelReservation");
    }

    public SteamAPICall_t ChangeNumOpenSlots(uint ulBeacon, uint unOpenSlots)
    {
        Write("ChangeNumOpenSlots");
        return default;
    }

    public SteamAPICall_t CreateBeacon(uint unOpenSlots, SteamPartyBeaconLocation_t pBeaconLocation, string pchConnectString, string pchMetadata)
    {
        Write("CreateBeacon");
        return default;
    }

    public bool DestroyBeacon(uint ulBeacon)
    {
        Write("DestroyBeacon");
        return false;
    }

    public bool GetAvailableBeaconLocations(IntPtr _, SteamPartyBeaconLocation_t pLocationList, uint uMaxNumLocations)
    {
        Write("GetAvailableBeaconLocations");
        return false;
    }

    public uint GetBeaconByIndex(uint unIndex)
    {
        Write("GetBeaconByIndex");
        return 0;
    }

    public bool GetBeaconDetails(uint ulBeaconID, IntPtr pSteamIDBeaconOwner, SteamPartyBeaconLocation_t pLocation, string pchMetadata, int cchMetadata)
    {
        Write("GetBeaconDetails");
        return false;
    }

    public bool GetBeaconLocationData(IntPtr _, SteamPartyBeaconLocation_t BeaconLocation, ESteamPartyBeaconLocationData eData, string pchDataStringOut, int cchDataStringOut)
    {
        Write("GetBeaconLocationData");
        return false;
    }

    public uint GetNumActiveBeacons(IntPtr _)
    {
        Write("GetNumActiveBeacons");
        return 0;
    }

    public bool GetNumAvailableBeaconLocations(uint puNumLocations)
    {
        Write("GetNumAvailableBeaconLocations");
        return false;
    }

    public SteamAPICall_t JoinParty(uint ulBeaconID)
    {
        Write("JoinParty");
        return default;
    }

    public void OnReservationCompleted(uint ulBeacon, IntPtr steamIDUser)
    {
        Write("OnReservationCompleted");
    }

    private void Write(string v)
    {
        Main.Write(InterfaceVersion, v);
    }
}
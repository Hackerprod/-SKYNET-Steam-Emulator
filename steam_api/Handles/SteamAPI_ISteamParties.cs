using SKYNET;
using SKYNET.Interface;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class SteamAPI_ISteamParties : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamParties_CancelReservation(uint ulBeacon, IntPtr steamIDUser)
    {
        Write("SteamAPI_ISteamParties_CancelReservation");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamParties_ChangeNumOpenSlots(uint ulBeacon, uint unOpenSlots)
    {
        Write("SteamAPI_ISteamParties_ChangeNumOpenSlots");
        return SteamEmulator.SteamParties.ChangeNumOpenSlots(ulBeacon, unOpenSlots);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamParties_CreateBeacon(uint unOpenSlots, SteamPartyBeaconLocation_t pBeaconLocation, string pchConnectString, string pchMetadata)
    {
        Write("SteamAPI_ISteamParties_CreateBeacon");
        return SteamEmulator.SteamParties.CreateBeacon(unOpenSlots, pBeaconLocation, pchConnectString, pchMetadata);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamParties_DestroyBeacon(uint ulBeacon)
    {
        Write("SteamAPI_ISteamParties_DestroyBeacon");
        return SteamEmulator.SteamParties.DestroyBeacon(ulBeacon);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamParties_GetAvailableBeaconLocations(SteamPartyBeaconLocation_t pLocationList, uint uMaxNumLocations)
    {
        Write("SteamAPI_ISteamParties_GetAvailableBeaconLocations");
        return SteamEmulator.SteamParties.GetAvailableBeaconLocations(pLocationList, uMaxNumLocations);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamParties_GetBeaconByIndex(uint unIndex)
    {
        Write("SteamAPI_ISteamParties_GetBeaconByIndex");
        return SteamEmulator.SteamParties.GetBeaconByIndex(unIndex);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamParties_GetBeaconDetails(uint ulBeaconID, IntPtr pSteamIDBeaconOwner, SteamPartyBeaconLocation_t pLocation, string pchMetadata, int cchMetadata)
    {
        Write("SteamAPI_ISteamParties_GetBeaconDetails");
        return SteamEmulator.SteamParties.GetBeaconDetails(ulBeaconID, pSteamIDBeaconOwner, pLocation, pchMetadata, cchMetadata);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamParties_GetBeaconLocationData(SteamPartyBeaconLocation_t BeaconLocation, ESteamPartyBeaconLocationData eData, string pchDataStringOut, int cchDataStringOut)
    {
        Write("SteamAPI_ISteamParties_GetBeaconLocationData");
        return SteamEmulator.SteamParties.GetBeaconLocationData(BeaconLocation, eData, pchDataStringOut, cchDataStringOut);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamParties_GetNumActiveBeacons(IntPtr _)
    {
        Write("SteamAPI_ISteamParties_GetNumActiveBeacons");
        return SteamEmulator.SteamParties.GetNumActiveBeacons(_);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamParties_GetNumAvailableBeaconLocations(uint puNumLocations)
    {
        Write("SteamAPI_ISteamParties_GetNumAvailableBeaconLocations");
        return SteamEmulator.SteamParties.GetNumAvailableBeaconLocations(puNumLocations);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamParties_JoinParty(uint ulBeaconID)
    {
        Write("SteamAPI_ISteamParties_JoinParty");
        return SteamEmulator.SteamParties.JoinParty(ulBeaconID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamParties_OnReservationCompleted(uint ulBeacon, IntPtr steamIDUser)
    {
        Write("SteamAPI_ISteamParties_OnReservationCompleted");
        //
    }

}


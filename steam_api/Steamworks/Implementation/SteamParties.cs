using System;
using System.Runtime.InteropServices;
using SKYNET.Steamworks.Interfaces;

using SteamAPICall_t = System.UInt64;
using PartyBeaconID_t = System.UInt64;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamParties : ISteamInterface
    {
        public static SteamParties Instance;

        public SteamParties()
        {
            Instance = this;
            InterfaceName = "SteamParties";
            InterfaceVersion = "SteamParties002";
        }

        public void CancelReservation(PartyBeaconID_t ulBeacon, ulong steamIDUser)
        {
            Write("CancelReservation");
        }

        public SteamAPICall_t ChangeNumOpenSlots(PartyBeaconID_t ulBeacon, uint unOpenSlots)
        {
            Write("ChangeNumOpenSlots");
            return default;
        }

        public SteamAPICall_t CreateBeacon(uint unOpenSlots, IntPtr pBeaconLocation, string pchConnectString, string pchMetadata)
        {
            Write("CreateBeacon");
            return default;
        }

        public bool DestroyBeacon(PartyBeaconID_t ulBeacon)
        {
            Write("DestroyBeacon");
            return false;
        }

        public bool GetAvailableBeaconLocations(IntPtr pLocationList, uint uMaxNumLocations)
        {
            Write("GetAvailableBeaconLocations");
            return false;
        }

        public PartyBeaconID_t GetBeaconByIndex(uint unIndex)
        {
            Write("GetBeaconByIndex");
            return 0;
        }

        public bool GetBeaconDetails(PartyBeaconID_t ulBeaconID, IntPtr pSteamIDBeaconOwner, IntPtr pLocation, IntPtr pchMetadata, int cchMetadata)
        {
            if (pSteamIDBeaconOwner != IntPtr.Zero)
            {
                Marshal.WriteInt64(pSteamIDBeaconOwner, 0);
            }
            Write("GetBeaconDetails");
            return false;
        }

        public bool GetBeaconDetails(PartyBeaconID_t ulBeaconID, ulong pSteamIDBeaconOwner, IntPtr pLocation, string pchMetadata, int cchMetadata)
        {
            return GetBeaconDetails(ulBeaconID, new IntPtr(unchecked((long)pSteamIDBeaconOwner)), pLocation, IntPtr.Zero, cchMetadata);
        }

        public bool GetBeaconLocationData(SteamPartyBeaconLocation_t BeaconLocation, int eData, IntPtr pchDataStringOut, int cchDataStringOut)
        {
            Write("GetBeaconLocationData");
            return false;
        }

        public bool GetBeaconLocationData(IntPtr BeaconLocation, int eData, ref string pchDataStringOut, ref int cchDataStringOut)
        {
            return GetBeaconLocationData(default(SteamPartyBeaconLocation_t), eData, IntPtr.Zero, cchDataStringOut);
        }

        public uint GetNumActiveBeacons()
        {
            Write("GetNumActiveBeacons");
            return 0;
        }

        public bool GetNumAvailableBeaconLocations(IntPtr puNumLocations)
        {
            if (puNumLocations != IntPtr.Zero)
            {
                Marshal.WriteInt32(puNumLocations, 0);
            }
            Write("GetNumAvailableBeaconLocations");
            return false;
        }

        public bool GetNumAvailableBeaconLocations(uint puNumLocations)
        {
            return GetNumAvailableBeaconLocations(IntPtr.Zero);
        }

        public SteamAPICall_t JoinParty(PartyBeaconID_t ulBeaconID)
        {
            Write("JoinParty");
            return default;
        }

        public void OnReservationCompleted(PartyBeaconID_t ulBeacon, ulong steamIDUser)
        {
            Write("OnReservationCompleted");
        }
    }
}

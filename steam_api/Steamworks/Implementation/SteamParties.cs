using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using Steamworks;
using System;
using System.Runtime.InteropServices;
using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamParties : ISteamInterface
    {
        public SteamParties()
        {
            InterfaceVersion = "SteamParties";
        }

        public void CancelReservation(uint ulBeacon, ulong steamIDUser)
        {
            Write("CancelReservation");
        }

        public SteamAPICall_t ChangeNumOpenSlots(uint ulBeacon, uint unOpenSlots)
        {
            Write("ChangeNumOpenSlots");
            return default;
        }

        public SteamAPICall_t CreateBeacon(uint unOpenSlots, IntPtr pBeaconLocation, string pchConnectString, string pchMetadata)
        {
            Write("CreateBeacon");
            return default;
        }

        public bool DestroyBeacon(uint ulBeacon)
        {
            Write("DestroyBeacon");
            return false;
        }

        public bool GetAvailableBeaconLocations(IntPtr pLocationList, uint uMaxNumLocations)
        {
            Write("GetAvailableBeaconLocations");
            return false;
        }

        public uint GetBeaconByIndex(uint unIndex)
        {
            Write("GetBeaconByIndex");
            return 0;
        }

        public bool GetBeaconDetails(uint ulBeaconID, IntPtr pSteamIDBeaconOwner, IntPtr pLocation, string pchMetadata, int cchMetadata)
        {
            Write("GetBeaconDetails");
            return false;
        }

        public bool GetBeaconLocationData(IntPtr BeaconLocation, int eData, string pchDataStringOut, int cchDataStringOut)
        {
            Write("GetBeaconLocationData");
            return false;
        }

        public uint GetNumActiveBeacons()
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

        public void OnReservationCompleted(uint ulBeacon, ulong steamIDUser)
        {
            Write("OnReservationCompleted");
        }
    }
}
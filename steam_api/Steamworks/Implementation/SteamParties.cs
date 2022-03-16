using SKYNET.Interface;
using Steamworks;
using System;

namespace SKYNET.Managers
{
    [Interface("SteamParties")]
    public class SteamParties : SteamInterface, ISteamParties
    {
        public void CancelReservation(uint ulBeacon, IntPtr steamIDUser)
        {
            //
        }

        public SteamAPICall_t ChangeNumOpenSlots(uint ulBeacon, uint unOpenSlots)
        {
            return default;
        }

        public SteamAPICall_t CreateBeacon(uint unOpenSlots, SteamPartyBeaconLocation_t pBeaconLocation, string pchConnectString, string pchMetadata)
        {
            return default;
        }

        public bool DestroyBeacon(uint ulBeacon)
        {
            return false;
        }

        public bool GetAvailableBeaconLocations(SteamPartyBeaconLocation_t pLocationList, uint uMaxNumLocations)
        {
            return false;
        }

        public uint GetBeaconByIndex(uint unIndex)
        {
            return 0;
        }

        public bool GetBeaconDetails(uint ulBeaconID, IntPtr pSteamIDBeaconOwner, SteamPartyBeaconLocation_t pLocation, string pchMetadata, int cchMetadata)
        {
            return false;
        }

        public bool GetBeaconLocationData(SteamPartyBeaconLocation_t BeaconLocation, ESteamPartyBeaconLocationData eData, string pchDataStringOut, int cchDataStringOut)
        {
            return false;
        }

        public uint GetNumActiveBeacons()
        {
            return 0;
        }

        public bool GetNumAvailableBeaconLocations(uint puNumLocations)
        {
            return false;
        }

        public SteamAPICall_t JoinParty(uint ulBeaconID)
        {
            return default;
        }

        public void OnReservationCompleted(uint ulBeacon, IntPtr steamIDUser)
        {
            //
        }

    }

}
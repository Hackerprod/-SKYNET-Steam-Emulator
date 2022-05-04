using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SteamAPICall_t = System.UInt64;
using PartyBeaconID_t = System.UInt64;

namespace SKYNET.Interface
{
    [Interface("SteamParties002")]
    public class SteamParties002 : ISteamInterface
    {
        public UInt32 GetNumActiveBeacons(IntPtr _)
        {
            return SteamEmulator.SteamParties.GetNumActiveBeacons();
        }

        public PartyBeaconID_t GetBeaconByIndex(IntPtr _, UInt32 unIndex)
        {
            return SteamEmulator.SteamParties.GetBeaconByIndex(unIndex);
        }

        public bool GetBeaconDetails(IntPtr _, PartyBeaconID_t ulBeaconID, ulong pSteamIDBeaconOwner, IntPtr pLocation, string pchMetadata, int cchMetadata)
        {
            return SteamEmulator.SteamParties.GetBeaconDetails(ulBeaconID, pSteamIDBeaconOwner, pLocation, pchMetadata, cchMetadata);
        }

        public SteamAPICall_t JoinParty(IntPtr _, PartyBeaconID_t ulBeaconID)
        {
            return SteamEmulator.SteamParties.JoinParty(ulBeaconID);
        }


        public bool GetNumAvailableBeaconLocations(IntPtr _, UInt32 puNumLocations)
        {
            return SteamEmulator.SteamParties.GetNumAvailableBeaconLocations(puNumLocations);
        }

        public bool GetAvailableBeaconLocations(IntPtr _, IntPtr pLocationList, UInt32 uMaxNumLocations)
        {
            return SteamEmulator.SteamParties.GetAvailableBeaconLocations(pLocationList, uMaxNumLocations);
        }

        public SteamAPICall_t CreateBeacon(IntPtr _, UInt32 unOpenSlots, IntPtr pBeaconLocation, string pchConnectString, string pchMetadata)
        {
            return SteamEmulator.SteamParties.CreateBeacon(unOpenSlots, pBeaconLocation, pchConnectString, pchMetadata);
        }

        public void OnReservationCompleted(IntPtr _, PartyBeaconID_t ulBeacon, ulong steamIDUser)
        {
            SteamEmulator.SteamParties.OnReservationCompleted(ulBeacon, steamIDUser);
        }

        public void CancelReservation(IntPtr _, PartyBeaconID_t ulBeacon, ulong steamIDUser)
        {
            SteamEmulator.SteamParties.CancelReservation(ulBeacon, steamIDUser);
        }

        public SteamAPICall_t ChangeNumOpenSlots(IntPtr _, PartyBeaconID_t ulBeacon, UInt32 unOpenSlots)
        {
            return SteamEmulator.SteamParties.ChangeNumOpenSlots(ulBeacon, unOpenSlots);
        }

        public bool DestroyBeacon(IntPtr _, PartyBeaconID_t ulBeacon)
        {
            return SteamEmulator.SteamParties.DestroyBeacon(ulBeacon);
        }

        public bool GetBeaconLocationData(IntPtr _, IntPtr BeaconLocation, int eData, ref string pchDataStringOut, ref int cchDataStringOut)
        {
            return SteamEmulator.SteamParties.GetBeaconLocationData(BeaconLocation, eData, ref pchDataStringOut, ref cchDataStringOut);
        }
    }
}

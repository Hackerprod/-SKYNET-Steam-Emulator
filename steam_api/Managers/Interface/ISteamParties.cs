using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    public interface ISteamParties
    {
        void CancelReservation(uint ulBeacon, IntPtr steamIDUser);
        SteamAPICall_t ChangeNumOpenSlots(uint ulBeacon, uint unOpenSlots);
        SteamAPICall_t CreateBeacon(uint unOpenSlots, SteamPartyBeaconLocation_t pBeaconLocation, string pchConnectString, string pchMetadata);
        bool DestroyBeacon(uint ulBeacon);
        bool GetAvailableBeaconLocations(SteamPartyBeaconLocation_t pLocationList, uint uMaxNumLocations);
        uint GetBeaconByIndex(uint unIndex);
        bool GetBeaconDetails(uint ulBeaconID, IntPtr pSteamIDBeaconOwner, SteamPartyBeaconLocation_t pLocation, string pchMetadata, int cchMetadata);
        bool GetBeaconLocationData(SteamPartyBeaconLocation_t BeaconLocation, ESteamPartyBeaconLocationData eData, string pchDataStringOut, int cchDataStringOut);
        uint GetNumActiveBeacons();
        bool GetNumAvailableBeaconLocations(uint puNumLocations);
        SteamAPICall_t JoinParty(uint ulBeaconID);
        void OnReservationCompleted(uint ulBeacon, IntPtr steamIDUser);

    }
    public struct SteamPartyBeaconLocation_t
    {
        uint m_eType;
        uint m_ulLocationID;
    };
    public enum ESteamPartyBeaconLocationData : int
    {
        k_ESteamPartyBeaconLocationDataInvalid = 0,
        k_ESteamPartyBeaconLocationDataName = 1,
        k_ESteamPartyBeaconLocationDataIconURLSmall = 2,
        k_ESteamPartyBeaconLocationDataIconURLMedium = 3,
        k_ESteamPartyBeaconLocationDataIconURLLarge = 4,
    };
}

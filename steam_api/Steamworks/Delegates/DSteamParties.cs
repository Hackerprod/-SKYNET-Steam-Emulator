using Core.Interface;
using SKYNET.Steamworks;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate(Name = "SteamParties")]
    public class DSteamParties 
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void CancelReservation(uint ulBeacon, IntPtr steamIDUser);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t ChangeNumOpenSlots(uint ulBeacon, uint unOpenSlots);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t CreateBeacon(uint unOpenSlots, SteamPartyBeaconLocation_t pBeaconLocation, string pchConnectString, string pchMetadata);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool DestroyBeacon(uint ulBeacon);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetAvailableBeaconLocations(SteamPartyBeaconLocation_t pLocationList, uint uMaxNumLocations);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetBeaconByIndex(uint unIndex);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetBeaconDetails(uint ulBeaconID, IntPtr pSteamIDBeaconOwner, SteamPartyBeaconLocation_t pLocation, string pchMetadata, int cchMetadata);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetBeaconLocationData(SteamPartyBeaconLocation_t BeaconLocation, ESteamPartyBeaconLocationData eData, string pchDataStringOut, int cchDataStringOut);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetNumActiveBeacons(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetNumAvailableBeaconLocations(uint puNumLocations);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t JoinParty(uint ulBeaconID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void OnReservationCompleted(uint ulBeacon, IntPtr steamIDUser);
    }
}

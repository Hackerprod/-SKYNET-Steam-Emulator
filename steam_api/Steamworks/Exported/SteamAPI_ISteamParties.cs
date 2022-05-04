using SKYNET;
using SKYNET.Steamworks;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using SteamAPICall_t = System.UInt64;
using PartyBeaconID_t = System.UInt64;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamParties 
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamParties_CancelReservation(IntPtr _, uint ulBeacon, IntPtr steamIDUser)
        {
            Write("SteamAPI_ISteamParties_CancelReservation");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamParties_ChangeNumOpenSlots(IntPtr _, uint ulBeacon, uint unOpenSlots)
        {
            Write("SteamAPI_ISteamParties_ChangeNumOpenSlots");
            return SteamEmulator.SteamParties.ChangeNumOpenSlots(ulBeacon, unOpenSlots);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamParties_CreateBeacon(IntPtr _, uint unOpenSlots, IntPtr pBeaconLocation, string pchConnectString, string pchMetadata)
        {
            Write("SteamAPI_ISteamParties_CreateBeacon");
            return SteamEmulator.SteamParties.CreateBeacon(unOpenSlots, pBeaconLocation, pchConnectString, pchMetadata);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamParties_DestroyBeacon(IntPtr _, uint ulBeacon)
        {
            Write("SteamAPI_ISteamParties_DestroyBeacon");
            return SteamEmulator.SteamParties.DestroyBeacon(ulBeacon);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamParties_GetAvailableBeaconLocations(IntPtr _, IntPtr pLocationList, uint uMaxNumLocations)
        {
            Write("SteamAPI_ISteamParties_GetAvailableBeaconLocations");
            return SteamEmulator.SteamParties.GetAvailableBeaconLocations(pLocationList, uMaxNumLocations);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static PartyBeaconID_t SteamAPI_ISteamParties_GetBeaconByIndex(IntPtr _, uint unIndex)
        {
            Write("SteamAPI_ISteamParties_GetBeaconByIndex");
            return SteamEmulator.SteamParties.GetBeaconByIndex(unIndex);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamParties_GetBeaconDetails(IntPtr _, uint ulBeaconID, ulong pSteamIDBeaconOwner, IntPtr pLocation, string pchMetadata, int cchMetadata)
        {
            Write("SteamAPI_ISteamParties_GetBeaconDetails");
            return SteamEmulator.SteamParties.GetBeaconDetails(ulBeaconID, pSteamIDBeaconOwner, pLocation, pchMetadata, cchMetadata);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamParties_GetBeaconLocationData(IntPtr _, IntPtr BeaconLocation, int eData, ref string pchDataStringOut, ref int cchDataStringOut)
        {
            Write("SteamAPI_ISteamParties_GetBeaconLocationData");
            return SteamEmulator.SteamParties.GetBeaconLocationData(BeaconLocation, eData, ref pchDataStringOut, ref cchDataStringOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamParties_GetNumActiveBeacons(IntPtr _)
        {
            Write("SteamAPI_ISteamParties_GetNumActiveBeacons");
            return SteamEmulator.SteamParties.GetNumActiveBeacons();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamParties_GetNumAvailableBeaconLocations(IntPtr _, uint puNumLocations)
        {
            Write("SteamAPI_ISteamParties_GetNumAvailableBeaconLocations");
            return SteamEmulator.SteamParties.GetNumAvailableBeaconLocations(puNumLocations);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamParties_JoinParty(IntPtr _, uint ulBeaconID)
        {
            Write("SteamAPI_ISteamParties_JoinParty");
            return SteamEmulator.SteamParties.JoinParty(ulBeaconID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamParties_OnReservationCompleted(IntPtr _, uint ulBeacon, IntPtr steamIDUser)
        {
            Write("SteamAPI_ISteamParties_OnReservationCompleted");
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}


using SKYNET;
using SKYNET.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace SKYNET.Managers
{
    public class InterfaceManager
    {
        static InterfaceManager()
        {

        }

        public static void Initialize()
        {

        }

        public static T CreateInterface<T>(out IntPtr BaseAddress) where T : ISteamInterface
        {
            var (iface, context) = MemoryManager.CreateInterface<T>();
            BaseAddress = context;
            T baseClass = (T)iface;
            baseClass.MemoryAddress = context;
            return (T)baseClass;
        }

        public static IntPtr FindOrCreateInterface(string pchVersion)
        {
            return FindOrCreateInterface(1, 1, pchVersion);
        }
        public static IntPtr FindOrCreateInterface(int hSteamUser, int hSteamPipe, string pszVersion)
        {
            if (pszVersion.StartsWith("SteamUtils"))
            {
                SteamEmulator.SteamUtils.InterfaceVersion = pszVersion;
                return SteamEmulator.SteamUtils.MemoryAddress;
            }
            if (pszVersion.StartsWith("SteamUser"))
            {
                SteamEmulator.SteamUser.InterfaceVersion = pszVersion;
                return SteamEmulator.SteamUser.MemoryAddress;
            }
            if (pszVersion.StartsWith("SteamClient"))
            {
                SteamEmulator.SteamClient.InterfaceVersion = pszVersion;
                return SteamEmulator.SteamClient.MemoryAddress;
            }
            if (pszVersion.StartsWith("SteamFriends"))
            {
                SteamEmulator.SteamFriends.InterfaceVersion = pszVersion;
                return SteamEmulator.SteamFriends.MemoryAddress;
            }
            if (pszVersion.StartsWith("SteamMatchMaking"))
            {
                SteamEmulator.SteamMatchmaking.InterfaceVersion = pszVersion;
                return SteamEmulator.SteamMatchmaking.MemoryAddress;
            }
            if (pszVersion.StartsWith("SteamMatchGameSearch"))
            {
                SteamEmulator.SteamGameSearch.InterfaceVersion = pszVersion;
                return SteamEmulator.SteamGameSearch.MemoryAddress;
            }
            if (pszVersion.StartsWith("SteamMatchMakingServers"))
            {
                SteamEmulator.SteamMatchMakingServers.InterfaceVersion = pszVersion;
                return SteamEmulator.SteamMatchMakingServers.MemoryAddress;
            }
            if (pszVersion.StartsWith("STEAMUSERSTATS_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamUserStats.InterfaceVersion = pszVersion;
                return SteamEmulator.SteamUserStats.MemoryAddress;
            }
            if (pszVersion.StartsWith("STEAMAPPS_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamApps.InterfaceVersion = pszVersion;
                return SteamEmulator.SteamApps.MemoryAddress;
            }
            if (pszVersion.StartsWith("SteamNetworking"))
            {
                SteamEmulator.SteamNetworking.InterfaceVersion = pszVersion;
                return SteamEmulator.SteamNetworking.MemoryAddress;
            }
            if (pszVersion.StartsWith("STEAMREMOTESTORAGE_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamRemoteStorage.InterfaceVersion = pszVersion;
                return SteamEmulator.SteamRemoteStorage.MemoryAddress;
            }
            if (pszVersion.StartsWith("STEAMSCREENSHOTS_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamScreenshots.InterfaceVersion = pszVersion;
                return SteamEmulator.SteamScreenshots.MemoryAddress;
            }
            if (pszVersion.StartsWith("STEAMHTTP_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamHTTP.InterfaceVersion = pszVersion;
                return SteamEmulator.SteamHTTP.MemoryAddress;
            }
            if (pszVersion.StartsWith("SteamController"))
            {
                SteamEmulator.SteamController.InterfaceVersion = pszVersion;
                return SteamEmulator.SteamController.MemoryAddress;
            }
            if (pszVersion.StartsWith("STEAMUGC_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamUGC.InterfaceVersion = pszVersion;
                return SteamEmulator.SteamUGC.MemoryAddress;
            }
            if (pszVersion.StartsWith("STEAMAPPLIST_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamAppList.InterfaceVersion = pszVersion;
                return SteamEmulator.SteamAppList.MemoryAddress;
            }
            if (pszVersion.StartsWith("STEAMMUSIC_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamMusic.InterfaceVersion = pszVersion;
                return SteamEmulator.SteamMusic.MemoryAddress;
            }
            if (pszVersion.StartsWith("STEAMMUSICREMOTE_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamMusicRemote.InterfaceVersion = pszVersion;
                return SteamEmulator.SteamMusicRemote.MemoryAddress;
            }
            if (pszVersion.StartsWith("STEAMHTMLSURFACE_INTERFACE_VERSION_"))
            {
                SteamEmulator.SteamHTMLSurface.InterfaceVersion = pszVersion;
                return SteamEmulator.SteamHTMLSurface.MemoryAddress;
            }
            if (pszVersion.StartsWith("STEAMINVENTORY_INTERFACE_V"))
            {
                SteamEmulator.SteamInventory.InterfaceVersion = pszVersion;
                return SteamEmulator.SteamInventory.MemoryAddress;
            }
            if (pszVersion.StartsWith("STEAMVIDEO_INTERFACE_V"))
            {
                SteamEmulator.SteamVideo.InterfaceVersion = pszVersion;
                return SteamEmulator.SteamVideo.MemoryAddress;
            }
            if (pszVersion.StartsWith("STEAMPARENTALSETTINGS_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamParentalSettings.InterfaceVersion = pszVersion;
                return SteamEmulator.SteamParentalSettings.MemoryAddress;
            }

            Write($"Not found Interface for {pszVersion}");
            return default;
        }

        private static void Write(string v)
        {
            SteamEmulator.Write(v);
        }
    }
}
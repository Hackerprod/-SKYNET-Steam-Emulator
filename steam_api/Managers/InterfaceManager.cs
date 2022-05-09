using SKYNET.Steamworks.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Reflection;

using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;

namespace SKYNET.Managers
{
    public class InterfaceManager
    {
        private static ConcurrentDictionary<string, Type> interfaceTypes;
        private static ConcurrentDictionary<string, IntPtr> StoredInterfaces;
        private static ConcurrentDictionary<string, IntPtr> StoredInterfaces_Gameserver;

        static InterfaceManager()
        {
            interfaceTypes = new ConcurrentDictionary<string, Type>();
            StoredInterfaces = new ConcurrentDictionary<string, IntPtr>();
            StoredInterfaces_Gameserver = new ConcurrentDictionary<string, IntPtr>();
        }

        public static void Initialize()
        {
            Assembly currentAssembly = Assembly.GetAssembly(typeof(InterfaceManager));
            foreach (var type in currentAssembly.GetTypes())
            {
                if (type.IsDefined(typeof(InterfaceAttribute)))
                {
                    foreach (var attribute in type.GetCustomAttributes<InterfaceAttribute>())
                    {
                        interfaceTypes.TryAdd(attribute.Name, type);
                    }
                }
            }
        }

        public static IntPtr FindOrCreateInterface(string pchVersion)
        {
            return FindOrCreateInterface(1, 1, pchVersion);
        }

        public static IntPtr FindOrCreateInterface(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pszVersion, bool GameServer = false)
        {
            if (InvalidateInterface(pszVersion))
            {
                //Write($"Skipping {pszVersion}");
                //return default;
            }

            if (GameServer)
            {
                if (StoredInterfaces_Gameserver.TryGetValue(pszVersion, out IntPtr BaseAddress))
                {
                    return BaseAddress;
                }
            }
            else if (StoredInterfaces.TryGetValue(pszVersion, out IntPtr BaseAddress))
            {
                return BaseAddress;
            }

            if (!interfaceTypes.ContainsKey(pszVersion))
            {
                Write($"Not found Interface for {pszVersion}");
                return default;
            }

            Type interfaceType = interfaceTypes[pszVersion];

            IntPtr address = MemoryManager.CreateInterface(interfaceType);

            if (address == IntPtr.Zero)
            {
                Write($"Error creating Interface for {pszVersion}");
                return address;
            }

            if (GameServer)
            {
                StoredInterfaces_Gameserver.TryAdd(pszVersion, address);
            }
            else
            {
                StoredInterfaces.TryAdd(pszVersion, address);
            }
            
            SetInterfaceName(pszVersion, interfaceType);

            return address;
        }

        private static bool InvalidateInterface(string pszVersion)
        {
            #region For test purposes

            //if (pszVersion.StartsWith("SteamUtils"))
            //{
            //    return true;
            //}
            //if (pszVersion.StartsWith("SteamUser"))
            //{
            //    return true;
            //}
            //if (pszVersion.StartsWith("SteamClient"))
            //{
            //    return true;
            //}
            //if (pszVersion.StartsWith("SteamFriends"))
            //{
            //    return true;
            //}
            //if (pszVersion.StartsWith("SteamMatchMaking"))
            //{
            //    return true;
            //}
            //if (pszVersion.StartsWith("SteamMatchGameSearch"))
            //{
            //    return true;
            //}
            //if (pszVersion.StartsWith("SteamMatchMakingServers"))
            //{
            //    return true;
            //}
            //if (pszVersion.StartsWith("STEAMUSERSTATS_INTERFACE_VERSION"))
            //{
            //    return true;
            //}
            //if (pszVersion.StartsWith("STEAMAPPS_INTERFACE_VERSION"))
            //{
            //    return true;
            //}
            if (pszVersion.StartsWith("SteamNetworkingUtils"))
            {
                return true;
            }
            //if (pszVersion.StartsWith("SteamNetworking"))
            //{
            //    return true;
            //}
            //if (pszVersion.StartsWith("STEAMREMOTESTORAGE_INTERFACE_VERSION"))
            //{
            //    return true;
            //}
            //if (pszVersion.StartsWith("STEAMSCREENSHOTS_INTERFACE_VERSION"))
            //{
            //    return true;
            //}
            //if (pszVersion.StartsWith("STEAMHTTP_INTERFACE_VERSION"))
            //{
            //    return true;
            //}
            //if (pszVersion.StartsWith("SteamController"))
            //{
            //    return true;
            //}
            //if (pszVersion.StartsWith("STEAMUGC_INTERFACE_VERSION"))
            //{
            //    return true;
            //}
            //if (pszVersion.StartsWith("STEAMAPPLIST_INTERFACE_VERSION"))
            //{
            //    return true;
            //}
            //if (pszVersion.StartsWith("STEAMMUSIC_INTERFACE_VERSION"))
            //{
            //    return true;
            //}
            //if (pszVersion.StartsWith("STEAMMUSICREMOTE_INTERFACE_VERSION"))
            //{
            //    return true;
            //}
            //if (pszVersion.StartsWith("STEAMHTMLSURFACE_INTERFACE_VERSION_"))
            //{
            //    return true;
            //}
            //if (pszVersion.StartsWith("STEAMINVENTORY_INTERFACE_V"))
            //{
            //    return true;
            //}
            //if (pszVersion.StartsWith("STEAMVIDEO_INTERFACE_V"))
            //{
            //    return true;
            //}
            //if (pszVersion.StartsWith("STEAMPARENTALSETTINGS_INTERFACE_VERSION"))
            //{
            //    return true;
            //}
            //if (pszVersion.StartsWith("SteamGameServer0"))
            //{
            //    return true;
            //}
            //if (pszVersion.StartsWith("SteamGameCoordinator"))
            //{
            //    return true;
            //}
            //if (pszVersion.StartsWith("SteamNetworkingSocketsSerialized"))
            //{
            //    return true;
            //}

            return false;

            #endregion
        }

        private static void SetInterfaceName(string pszVersion, Type type)
        {
            if (pszVersion.StartsWith("SteamUtils"))
            {
                SteamEmulator.SteamUtils.InterfaceName = type.Name;
                SteamEmulator.SteamUtils.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamUser"))
            {
                SteamEmulator.SteamUser.InterfaceName = type.Name;
                SteamEmulator.SteamUser.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamClient"))
            {
                SteamEmulator.SteamClient.InterfaceName = type.Name;
                SteamEmulator.SteamClient.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamFriends"))
            {
                SteamEmulator.SteamFriends.InterfaceName = type.Name;
                SteamEmulator.SteamFriends.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamMatchMaking"))
            {
                SteamEmulator.SteamMatchmaking.InterfaceName = type.Name;
                SteamEmulator.SteamMatchmaking.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamMatchGameSearch"))
            {
                SteamEmulator.SteamGameSearch.InterfaceName = type.Name;
                SteamEmulator.SteamGameSearch.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamMatchMakingServers"))
            {
                SteamEmulator.SteamMatchMakingServers.InterfaceName = type.Name;
                SteamEmulator.SteamMatchMakingServers.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMUSERSTATS_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamUserStats.InterfaceName = type.Name;
                SteamEmulator.SteamUserStats.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMAPPS_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamApps.InterfaceName = type.Name;
                SteamEmulator.SteamApps.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamNetworking"))
            {
                SteamEmulator.SteamNetworking.InterfaceName = type.Name;
                SteamEmulator.SteamNetworking.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMREMOTESTORAGE_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamRemoteStorage.InterfaceName = type.Name;
                SteamEmulator.SteamRemoteStorage.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMSCREENSHOTS_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamScreenshots.InterfaceName = type.Name;
                SteamEmulator.SteamScreenshots.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMHTTP_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamHTTP.InterfaceName = type.Name;
                SteamEmulator.SteamHTTP.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamController"))
            {
                SteamEmulator.SteamController.InterfaceName = type.Name;
                SteamEmulator.SteamController.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMUGC_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamUGC.InterfaceName = type.Name;
                SteamEmulator.SteamUGC.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMAPPLIST_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamAppList.InterfaceName = type.Name;
                SteamEmulator.SteamAppList.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMMUSIC_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamMusic.InterfaceName = type.Name;
                SteamEmulator.SteamMusic.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMMUSICREMOTE_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamMusicRemote.InterfaceName = type.Name;
                SteamEmulator.SteamMusicRemote.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMHTMLSURFACE_INTERFACE_VERSION_"))
            {
                SteamEmulator.SteamHTMLSurface.InterfaceName = type.Name;
                SteamEmulator.SteamHTMLSurface.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMINVENTORY_INTERFACE_V"))
            {
                SteamEmulator.SteamInventory.InterfaceName = type.Name;
                SteamEmulator.SteamInventory.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMVIDEO_INTERFACE_V"))
            {
                SteamEmulator.SteamVideo.InterfaceName = type.Name;
                SteamEmulator.SteamVideo.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMPARENTALSETTINGS_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamParentalSettings.InterfaceName = type.Name;
                SteamEmulator.SteamParentalSettings.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamGameServer0"))
            {
                SteamEmulator.SteamGameServer.InterfaceName = type.Name;
                SteamEmulator.SteamGameServer.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamGameCoordinator"))
            {
                SteamEmulator.SteamGameCoordinator.InterfaceName = type.Name;
                SteamEmulator.SteamGameCoordinator.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamNetworkingSocketsSerialized"))
            {
                SteamEmulator.SteamNetworkingSocketsSerialized.InterfaceName = type.Name;
                SteamEmulator.SteamNetworkingSocketsSerialized.InterfaceVersion = pszVersion;
            }
        }

        private static void Write(string v)
        {
            SteamEmulator.Write("InterfaceManager", v);
        }
    }
}
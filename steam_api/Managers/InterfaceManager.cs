using SKYNET.Steamworks.Implementation;
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
        private static ConcurrentDictionary<IntPtr, bool> GameServerInterfacePointers;

        static InterfaceManager()
        {
            interfaceTypes = new ConcurrentDictionary<string, Type>();
            StoredInterfaces = new ConcurrentDictionary<string, IntPtr>();
            StoredInterfaces_Gameserver = new ConcurrentDictionary<string, IntPtr>();
            GameServerInterfacePointers = new ConcurrentDictionary<IntPtr, bool>();
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
                Write($"Skipping {pszVersion}");
                return default;
            }

            bool gameServerContext = GameServer
                || hSteamUser == SteamEmulator.HSteamUser_GS
                || hSteamPipe == SteamEmulator.HSteamPipe_GS;

            if (gameServerContext)
            {
                if (StoredInterfaces_Gameserver.TryGetValue(pszVersion, out IntPtr BaseAddress))
                {
                    GameServerInterfacePointers[BaseAddress] = true;
                    return BaseAddress;
                }
            }
            else if (StoredInterfaces.TryGetValue(pszVersion, out IntPtr BaseAddress))
            {
                GameServerInterfacePointers.TryAdd(BaseAddress, false);
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

            if (gameServerContext)
            {
                StoredInterfaces_Gameserver.TryAdd(pszVersion, address);
                GameServerInterfacePointers[address] = true;
            }
            else
            {
                StoredInterfaces.TryAdd(pszVersion, address);
                GameServerInterfacePointers.TryAdd(address, false);
            }
            
            SetInterfaceName(pszVersion, interfaceType);

            return address;
        }

        public static bool IsGameServerInterfacePointer(IntPtr address)
        {
            return address != IntPtr.Zero
                && GameServerInterfacePointers.TryGetValue(address, out bool gameServer)
                && gameServer;
        }

        public static bool IsKnownInterfacePointer(IntPtr address)
        {
            if (address == IntPtr.Zero)
            {
                return false;
            }

            foreach (var storedAddress in StoredInterfaces.Values)
            {
                if (storedAddress == address)
                {
                    return true;
                }
            }

            foreach (var storedAddress in StoredInterfaces_Gameserver.Values)
            {
                if (storedAddress == address)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool InvalidateInterface(string pszVersion)
        {
            #region For test purposes

            if (pszVersion.StartsWith("STEAMHTTP_INTERFACE_VERSION"))
            {
                return !SteamEmulator.ISteamHTTP;
            }
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
            //if (pszVersion.StartsWith("SteamNetworkingUtils"))
            //{
            //    return true;
            //}
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
                SteamFriends.Instance.InterfaceName = type.Name;
                SteamFriends.Instance.InterfaceVersion = pszVersion;
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
            if (pszVersion.StartsWith("SteamNetworkingMessages"))
            {
                SteamEmulator.SteamNetworkingMessages.InterfaceName = type.Name;
                SteamEmulator.SteamNetworkingMessages.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamNetworkingSocketsSerialized"))
            {
                SteamEmulator.SteamNetworkingSocketsSerialized.InterfaceName = type.Name;
                SteamEmulator.SteamNetworkingSocketsSerialized.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamNetworkingSockets"))
            {
                SteamEmulator.SteamNetworkingSockets.InterfaceName = type.Name;
                SteamEmulator.SteamNetworkingSockets.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamNetworkingUtils"))
            {
                SteamEmulator.SteamNetworkingUtils.InterfaceName = type.Name;
                SteamEmulator.SteamNetworkingUtils.InterfaceVersion = pszVersion;
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
            if (pszVersion.StartsWith("SteamInput"))
            {
                SteamEmulator.SteamInput.InterfaceName = type.Name;
                SteamEmulator.SteamInput.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamParties"))
            {
                SteamEmulator.SteamParties.InterfaceName = type.Name;
                SteamEmulator.SteamParties.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMREMOTEPLAY_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamRemotePlay.InterfaceName = type.Name;
                SteamEmulator.SteamRemotePlay.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMTIMELINE_INTERFACE_V"))
            {
                SteamEmulator.SteamTimeline.InterfaceName = type.Name;
                SteamEmulator.SteamTimeline.InterfaceVersion = pszVersion;
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
        }

        private static void Write(string v)
        {
            SteamEmulator.Write("InterfaceManager", v);
        }
    }
}

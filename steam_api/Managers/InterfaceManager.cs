using SKYNET.Steamworks.Implementation;
using SKYNET.Steamworks.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;

namespace SKYNET.Managers
{
    public class InterfaceManager
    {
        private static ConcurrentDictionary<string, Type> interfaceTypes;
        private static ConcurrentDictionary<string, List<MethodInfo>> interfaceLayouts;
        private static ConcurrentDictionary<string, IntPtr> StoredInterfaces;
        private static ConcurrentDictionary<string, IntPtr> StoredInterfaces_Gameserver;
        private static ConcurrentDictionary<IntPtr, bool> GameServerInterfacePointers;

        static InterfaceManager()
        {
            interfaceTypes = new ConcurrentDictionary<string, Type>();
            interfaceLayouts = new ConcurrentDictionary<string, List<MethodInfo>>();
            StoredInterfaces = new ConcurrentDictionary<string, IntPtr>();
            StoredInterfaces_Gameserver = new ConcurrentDictionary<string, IntPtr>();
            GameServerInterfacePointers = new ConcurrentDictionary<IntPtr, bool>();
        }

        public static void Initialize()
        {
            Assembly currentAssembly = Assembly.GetAssembly(typeof(InterfaceManager));
            foreach (var type in currentAssembly.GetTypes())
            {
                foreach (var layout in type.GetCustomAttributes<InterfaceLayoutAttribute>())
                {
                    var methods = MemoryManager.ResolveLayoutMethods(type, layout.Name, layout.MethodNames);
                    if (!interfaceLayouts.TryAdd(layout.Name, methods))
                    {
                        throw new InvalidOperationException($"Interface layout '{layout.Name}' is declared more than once ({type.FullName}).");
                    }

                    interfaceTypes[layout.Name] = type;
                }
            }
        }

        public static List<MethodInfo> GetInterfaceMethods(string version)
        {
            return interfaceLayouts[version];
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

            if (!interfaceTypes.TryGetValue(pszVersion, out Type interfaceType))
            {
                Write($"Not found Interface for {pszVersion}");
                return default;
            }

            IntPtr address = MemoryManager.CreateInterface(interfaceType, interfaceLayouts[pszVersion]);

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
            
            SetInterfaceName(pszVersion);

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
            if (pszVersion.StartsWith("STEAMHTTP_INTERFACE_VERSION"))
            {
                return !SteamEmulator.ISteamHTTP;
            }

            return false;
        }

        private static bool IsVersionOf(string pszVersion, string prefix)
        {
            return pszVersion.Length > prefix.Length
                && pszVersion.StartsWith(prefix, StringComparison.Ordinal)
                && char.IsDigit(pszVersion[prefix.Length]);
        }

        private static void SetInterfaceName(string pszVersion)
        {
            if (pszVersion.StartsWith("SteamUtils"))
            {
                SteamEmulator.SteamUtils.InterfaceName = pszVersion;
                SteamEmulator.SteamUtils.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamUser"))
            {
                SteamEmulator.SteamUser.InterfaceName = pszVersion;
                SteamEmulator.SteamUser.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamClient"))
            {
                SteamEmulator.SteamClient.InterfaceName = pszVersion;
                SteamEmulator.SteamClient.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamFriends"))
            {
                SteamFriends.Instance.InterfaceName = pszVersion;
                SteamFriends.Instance.InterfaceVersion = pszVersion;
            }
            if (IsVersionOf(pszVersion, "SteamMatchMaking"))
            {
                SteamEmulator.SteamMatchmaking.InterfaceName = pszVersion;
                SteamEmulator.SteamMatchmaking.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamMatchGameSearch"))
            {
                SteamEmulator.SteamGameSearch.InterfaceName = pszVersion;
                SteamEmulator.SteamGameSearch.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamMatchMakingServers"))
            {
                SteamEmulator.SteamMatchMakingServers.InterfaceName = pszVersion;
                SteamEmulator.SteamMatchMakingServers.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMUSERSTATS_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamUserStats.InterfaceName = pszVersion;
                SteamEmulator.SteamUserStats.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMAPPS_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamApps.InterfaceName = pszVersion;
                SteamEmulator.SteamApps.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamNetworkingMessages"))
            {
                SteamEmulator.SteamNetworkingMessages.InterfaceName = pszVersion;
                SteamEmulator.SteamNetworkingMessages.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamNetworkingSocketsSerialized"))
            {
                SteamEmulator.SteamNetworkingSocketsSerialized.InterfaceName = pszVersion;
                SteamEmulator.SteamNetworkingSocketsSerialized.InterfaceVersion = pszVersion;
            }
            if (IsVersionOf(pszVersion, "SteamNetworkingSockets"))
            {
                SteamEmulator.SteamNetworkingSockets.InterfaceName = pszVersion;
                SteamEmulator.SteamNetworkingSockets.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamNetworkingUtils"))
            {
                SteamEmulator.SteamNetworkingUtils.InterfaceName = pszVersion;
                SteamEmulator.SteamNetworkingUtils.InterfaceVersion = pszVersion;
            }
            if (IsVersionOf(pszVersion, "SteamNetworking"))
            {
                SteamEmulator.SteamNetworking.InterfaceName = pszVersion;
                SteamEmulator.SteamNetworking.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMREMOTESTORAGE_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamRemoteStorage.InterfaceName = pszVersion;
                SteamEmulator.SteamRemoteStorage.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMSCREENSHOTS_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamScreenshots.InterfaceName = pszVersion;
                SteamEmulator.SteamScreenshots.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMHTTP_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamHTTP.InterfaceName = pszVersion;
                SteamEmulator.SteamHTTP.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamController"))
            {
                SteamEmulator.SteamController.InterfaceName = pszVersion;
                SteamEmulator.SteamController.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMUGC_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamUGC.InterfaceName = pszVersion;
                SteamEmulator.SteamUGC.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMAPPLIST_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamAppList.InterfaceName = pszVersion;
                SteamEmulator.SteamAppList.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMMUSIC_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamMusic.InterfaceName = pszVersion;
                SteamEmulator.SteamMusic.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMMUSICREMOTE_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamMusicRemote.InterfaceName = pszVersion;
                SteamEmulator.SteamMusicRemote.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMHTMLSURFACE_INTERFACE_VERSION_"))
            {
                SteamEmulator.SteamHTMLSurface.InterfaceName = pszVersion;
                SteamEmulator.SteamHTMLSurface.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMINVENTORY_INTERFACE_V"))
            {
                SteamEmulator.SteamInventory.InterfaceName = pszVersion;
                SteamEmulator.SteamInventory.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMVIDEO_INTERFACE_V"))
            {
                SteamEmulator.SteamVideo.InterfaceName = pszVersion;
                SteamEmulator.SteamVideo.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMPARENTALSETTINGS_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamParentalSettings.InterfaceName = pszVersion;
                SteamEmulator.SteamParentalSettings.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamInput"))
            {
                SteamEmulator.SteamInput.InterfaceName = pszVersion;
                SteamEmulator.SteamInput.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamParties"))
            {
                SteamEmulator.SteamParties.InterfaceName = pszVersion;
                SteamEmulator.SteamParties.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMREMOTEPLAY_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamRemotePlay.InterfaceName = pszVersion;
                SteamEmulator.SteamRemotePlay.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("STEAMTIMELINE_INTERFACE_V"))
            {
                SteamEmulator.SteamTimeline.InterfaceName = pszVersion;
                SteamEmulator.SteamTimeline.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamGameServer0"))
            {
                SteamEmulator.SteamGameServer.InterfaceName = pszVersion;
                SteamEmulator.SteamGameServer.InterfaceVersion = pszVersion;
            }
            if (pszVersion.StartsWith("SteamGameCoordinator"))
            {
                SteamEmulator.SteamGameCoordinator.InterfaceName = pszVersion;
                SteamEmulator.SteamGameCoordinator.InterfaceVersion = pszVersion;
            }
        }

        private static void Write(string v)
        {
            SteamEmulator.Write("InterfaceManager", v);
        }
    }
}

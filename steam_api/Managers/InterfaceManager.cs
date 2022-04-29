using SKYNET.Interface;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
                Write($"Skipping {pszVersion}");
                return default;
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
            if (pszVersion.StartsWith("STEAMHTTP_INTERFACE_VERSION"))
            {
                return true;
            }
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
                SteamEmulator.SteamUtils.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("SteamUser"))
            {
                SteamEmulator.SteamUser.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("SteamClient"))
            {
                SteamEmulator.SteamClient.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("SteamFriends"))
            {
                SteamEmulator.SteamFriends.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("SteamMatchMaking"))
            {
                SteamEmulator.SteamMatchmaking.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("SteamMatchGameSearch"))
            {
                SteamEmulator.SteamGameSearch.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("SteamMatchMakingServers"))
            {
                SteamEmulator.SteamMatchMakingServers.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("STEAMUSERSTATS_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamUserStats.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("STEAMAPPS_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamApps.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("SteamNetworking"))
            {
                SteamEmulator.SteamNetworking.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("STEAMREMOTESTORAGE_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamRemoteStorage.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("STEAMSCREENSHOTS_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamScreenshots.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("STEAMHTTP_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamHTTP.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("SteamController"))
            {
                SteamEmulator.SteamController.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("STEAMUGC_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamUGC.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("STEAMAPPLIST_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamAppList.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("STEAMMUSIC_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamMusic.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("STEAMMUSICREMOTE_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamMusicRemote.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("STEAMHTMLSURFACE_INTERFACE_VERSION_"))
            {
                SteamEmulator.SteamHTMLSurface.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("STEAMINVENTORY_INTERFACE_V"))
            {
                SteamEmulator.SteamInventory.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("STEAMVIDEO_INTERFACE_V"))
            {
                SteamEmulator.SteamVideo.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("STEAMPARENTALSETTINGS_INTERFACE_VERSION"))
            {
                SteamEmulator.SteamParentalSettings.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("SteamGameServer0"))
            {
                SteamEmulator.SteamGameServer.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("SteamGameCoordinator"))
            {
                SteamEmulator.SteamGameCoordinator.InterfaceVersion = type.Name;
            }
            if (pszVersion.StartsWith("SteamNetworkingSocketsSerialized"))
            {
                SteamEmulator.SteamNetworkingSocketsSerialized.InterfaceVersion = type.Name;
            }
        }

        private static void Write(string v)
        {
            SteamEmulator.Write("Interface Manager", v);
        }
    }
}
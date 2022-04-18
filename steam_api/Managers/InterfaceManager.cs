using SKYNET;
using SKYNET.Helpers;
using SKYNET.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SKYNET.Managers
{
    public class InterfaceManager
    {
        private static ConcurrentDictionary<string, Type> interfaceTypes;
        private static Dictionary<string, IntPtr> StoredInterfaces;
        private static Dictionary<string, IntPtr> StoredInterfaces_Gameserver;

        static InterfaceManager()
        {
            interfaceTypes = new ConcurrentDictionary<string, Type>();
            StoredInterfaces = new Dictionary<string, IntPtr>();
            StoredInterfaces_Gameserver = new Dictionary<string, IntPtr>();
        }

        public static void Initialize()
        {
            //Assembly currentAssembly = Assembly.GetAssembly(typeof(InterfaceManager));
            //    foreach (var type in currentAssembly.GetTypes())
            //    {
            //        if (type.IsDefined(typeof(InterfaceAttribute)))
            //        {
            //            var interfaceAttribute = type.GetCustomAttributes<InterfaceAttribute>().ToList()[0];
            //            interfaceTypes.TryAdd(interfaceAttribute.Name, type);
            //        }
            //    }

            interfaceTypes.TryAdd("STEAMAPPLIST_INTERFACE_VERSION001", typeof(SteamAppList001));
            interfaceTypes.TryAdd("STEAMAPPS_INTERFACE_VERSION008", typeof(SteamApps008));
            interfaceTypes.TryAdd("SteamClient020", typeof(SteamClient020));
            interfaceTypes.TryAdd("SteamController008", typeof(SteamController008));
            interfaceTypes.TryAdd("SteamFriends015", typeof(SteamFriends015));
            interfaceTypes.TryAdd("SteamFriends017", typeof(SteamFriends017));
            interfaceTypes.TryAdd("SteamGameCoordinator001", typeof(SteamGameCoordinator001));
            interfaceTypes.TryAdd("SteamGameServer012", typeof(SteamGameServer012));
            interfaceTypes.TryAdd("SteamGameServer014", typeof(SteamGameServer014));
            interfaceTypes.TryAdd("SteamGameServerStats001", typeof(SteamGameServerStats001));
            interfaceTypes.TryAdd("SteamGameStats001", typeof(SteamGameStats001));
            interfaceTypes.TryAdd("STEAMHTMLSURFACE_INTERFACE_VERSION_004", typeof(SteamHTMLSurface004));
            interfaceTypes.TryAdd("STEAMHTMLSURFACE_INTERFACE_VERSION_005", typeof(SteamHTMLSurface005));
            interfaceTypes.TryAdd("STEAMHTTP_INTERFACE_VERSION003", typeof(SteamHTTP003));
            interfaceTypes.TryAdd("STEAMINVENTORY_INTERFACE_V002", typeof(SteamInventory002));
            interfaceTypes.TryAdd("STEAMINVENTORY_INTERFACE_V003", typeof(SteamInventory003));
            interfaceTypes.TryAdd("SteamMatchGameSearch001", typeof(SteamMatchGameSearch001));
            interfaceTypes.TryAdd("SteamMatchMaking008", typeof(SteamMatchMaking008));
            interfaceTypes.TryAdd("SteamMatchMaking009", typeof(SteamMatchMaking009));
            interfaceTypes.TryAdd("SteamMatchMakingServers002", typeof(SteamMatchMakingServers002));
            interfaceTypes.TryAdd("STEAMMUSIC_INTERFACE_VERSION001", typeof(SteamMusic001));
            interfaceTypes.TryAdd("STEAMMUSICREMOTE_INTERFACE_VERSION001", typeof(SteamMusicRemote001));
            interfaceTypes.TryAdd("SteamNetworking005", typeof(SteamNetworking005));
            interfaceTypes.TryAdd("SteamNetworking006", typeof(SteamNetworking006));
            interfaceTypes.TryAdd("SteamNetworkingSocketsSerialized002", typeof(SteamNetworkingSocketsSerialized002));
            interfaceTypes.TryAdd("SteamNetworkingSocketsSerialized003", typeof(SteamNetworkingSocketsSerialized003));
            interfaceTypes.TryAdd("SteamNetworkingSocketsSerialized004", typeof(SteamNetworkingSocketsSerialized004));
            interfaceTypes.TryAdd("SteamNetworkingSocketsSerialized005", typeof(SteamNetworkingSocketsSerialized005));
            interfaceTypes.TryAdd("STEAMPARENTALSETTINGS_INTERFACE_VERSION001", typeof(SteamParentalSettings001));
            interfaceTypes.TryAdd("STEAMREMOTESTORAGE_INTERFACE_VERSION013", typeof(SteamRemoteStorage013));
            interfaceTypes.TryAdd("STEAMREMOTESTORAGE_INTERFACE_VERSION014", typeof(SteamRemoteStorage014));
            interfaceTypes.TryAdd("STEAMREMOTESTORAGE_INTERFACE_VERSION016", typeof(SteamRemoteStorage016));
            interfaceTypes.TryAdd("STEAMSCREENSHOTS_INTERFACE_VERSION003", typeof(SteamScreenshots003));
            interfaceTypes.TryAdd("STEAMUGC_INTERFACE_VERSION014", typeof(SteamUGC014));
            interfaceTypes.TryAdd("STEAMUGC_INTERFACE_VERSION015", typeof(SteamUGC015));
            interfaceTypes.TryAdd("STEAMUGC_INTERFACE_VERSION016", typeof(SteamUGC016));
            interfaceTypes.TryAdd("SteamUser019", typeof(SteamUser019));
            interfaceTypes.TryAdd("SteamUser021", typeof(SteamUser021));
            interfaceTypes.TryAdd("STEAMUSERSTATS_INTERFACE_VERSION012", typeof(SteamUserStats012));
            interfaceTypes.TryAdd("SteamUtils009", typeof(SteamUtils009));
            interfaceTypes.TryAdd("SteamUtils010", typeof(SteamUtils010));
            interfaceTypes.TryAdd("STEAMVIDEO_INTERFACE_V002", typeof(SteamVideo002));
            interfaceTypes.TryAdd("SteamAppDisableUpdate001", typeof(SteamAppDisableUpdate001));
        }

        public static T CreateInterface<T>(out IntPtr BaseAddress) where T : ISteamInterface
        {
            var (iface, context) = MemoryManager.CreateInterface<T>();
            BaseAddress = context;
            T baseClass = (T)iface;
            //baseClass.MemoryAddress = context;
            return (T)baseClass;
        }

        public static IntPtr FindOrCreateInterface(string pchVersion)
        {
            return FindOrCreateInterface(1, 1, pchVersion);
        }

        public static IntPtr FindOrCreateInterface(int hSteamUser, int hSteamPipe, string pszVersion)
        {
            bool GameServer = hSteamUser == SteamEmulator.HSteamUser_GS;

            //if (pszVersion.StartsWith("SteamUtils"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}
            //if (pszVersion.StartsWith("SteamUser"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}
            //if (pszVersion.StartsWith("SteamClient"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}
            //if (pszVersion.StartsWith("SteamFriends"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}
            //if (pszVersion.StartsWith("SteamMatchMaking"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}
            //if (pszVersion.StartsWith("SteamMatchGameSearch"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}
            //if (pszVersion.StartsWith("SteamMatchMakingServers"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}
            //if (pszVersion.StartsWith("STEAMUSERSTATS_INTERFACE_VERSION"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}
            //if (pszVersion.StartsWith("STEAMAPPS_INTERFACE_VERSION"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}
            //if (pszVersion.StartsWith("SteamNetworking"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}
            //if (pszVersion.StartsWith("STEAMREMOTESTORAGE_INTERFACE_VERSION"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}
            //if (pszVersion.StartsWith("STEAMSCREENSHOTS_INTERFACE_VERSION"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}
            //if (pszVersion.StartsWith("STEAMHTTP_INTERFACE_VERSION"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}
            //if (pszVersion.StartsWith("SteamController"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}
            //if (pszVersion.StartsWith("STEAMUGC_INTERFACE_VERSION"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}
            //if (pszVersion.StartsWith("STEAMAPPLIST_INTERFACE_VERSION"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}
            //if (pszVersion.StartsWith("STEAMMUSIC_INTERFACE_VERSION"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}
            //if (pszVersion.StartsWith("STEAMMUSICREMOTE_INTERFACE_VERSION"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}
            //if (pszVersion.StartsWith("STEAMHTMLSURFACE_INTERFACE_VERSION_"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}
            //if (pszVersion.StartsWith("STEAMINVENTORY_INTERFACE_V"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}
            //if (pszVersion.StartsWith("STEAMVIDEO_INTERFACE_V"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}
            //if (pszVersion.StartsWith("STEAMPARENTALSETTINGS_INTERFACE_VERSION"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}
            if (pszVersion.StartsWith("SteamGameServer0"))
            {
                Write($"Skipping {pszVersion}"); return default;
            }
            //if (pszVersion.StartsWith("SteamGameCoordinator"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}
            //if (pszVersion.StartsWith("SteamNetworkingSocketsSerialized"))
            //{
            //    Write($"Skipping {pszVersion}"); return default;
            //}

            ///////////////////////////////////////////////////////////////////////

            if (GameServer && StoredInterfaces_Gameserver.ContainsKey(pszVersion))
            {
                return StoredInterfaces_Gameserver[pszVersion];
            }
            else if (StoredInterfaces.ContainsKey(pszVersion))
            {
                return StoredInterfaces[pszVersion];
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
                StoredInterfaces_Gameserver.Add(pszVersion, address);
            }
            else
            {
                StoredInterfaces.Add(pszVersion, address);
            }
            
            SetInterfaceName(pszVersion, interfaceType);

            return address;
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
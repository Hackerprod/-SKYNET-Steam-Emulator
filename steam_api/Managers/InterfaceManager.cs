using Core.Interface;
using SKYNET;
using SKYNET.Delegate;
using SKYNET.Helper;
using SKYNET.Interface;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

public class InterfaceManager
{
    public static List<Plugin> LoadedPlugins { get; set; }

    public static List<IBaseInterfaceMap> Delegates;

    private static Dictionary<string, IntPtr> Interfaces;

    private static string filePath;

    private const string x86 = "steam_api.dll";
    private const string x64 = "steam_api64.dll";

    static InterfaceManager()
    {
        Interfaces = new Dictionary<string, IntPtr>();
        Delegates = new List<IBaseInterfaceMap>();
        LoadedPlugins = new List<Plugin>();
    }

    public static void Initialize()
    {
        return;
        filePath = modCommon.GetPath();
        if (File.Exists(Path.Combine(filePath, x86)))
        {
            filePath = Path.Combine(filePath, x86);
        }
        else
        {
            filePath = Path.Combine(filePath, x64);
        }

        Assembly a = Assembly.LoadFile(filePath);

        var p = new Plugin { name = a.GetName().Name };
        foreach (var t in a.GetTypes())
        {
            var attributes = t.GetCustomAttributes(true);
            foreach (Attribute item in attributes)
            {
                if (item.ToString() == "SKYNET.Delegate.DelegateAttribute")
                {
                    modCommon.Show(t + " " + item);
                }
            }



            if (IsInterfaceDelegate(t))
            {
                
                var attribute = t.GetCustomAttribute<DelegateAttribute>();
                var name = attribute.Name;

                var new_interface = new Plugin.InterfaceDelegates { name = name };

                Log.Write($"Found interface delegates \"{name}\"");

                var types = t.GetNestedTypes(BindingFlags.Public);

                foreach (var type in types)
                {
                    // Filter out types that are not delegates
                    if (type.IsSubclassOf(typeof(System.Delegate))) new_interface.delegate_types.Add(type);
                }

                // Just assume all members are delegate types
                p.interface_delegates.Add(new_interface);
            }
            else if (IsInterfaceMap(t))
            {
                var attribute = t.GetCustomAttribute<MapAttribute>();
                var name = attribute.Name;

                var new_interface_map = new Plugin.InterfaceMap
                {
                    name = name,
                    this_type = t,
                    methods = InterfaceMethodsForType(t),
                };

                Log.Write($"Found interface map \"{name}\"");

                p.interface_maps.Add(new_interface_map);
            }
            else if (IsInterfaceImpl(t))
            {
                var attribute = t.GetCustomAttribute<MapAttribute>();
                var name = attribute.Name;

                var new_interface_impl = new Plugin.InterfaceImpl
                {
                    name = name,
                    this_type = t,
                    methods = InterfaceMethodsForType(t)
                };

                Log.Write($"Found interface impl \"{name}\"");

                p.interface_impls.Add(new_interface_impl);
            }
            LoadedPlugins.Add(p);
        }

        
    }


    internal static IntPtr CreateInterfaceNoUser(int pipe, string version)
    {
        return FindOrCreateInterface(1, pipe, version);
    }

    public static bool IsInterfaceImpl(Type t)
    {
        var has_attribute = t.IsDefined(typeof(ImplAttribute));
        return has_attribute;
    }

    public static bool IsInterfaceDelegate(Type t) => t.IsDefined(typeof(DelegateAttribute));

    public static bool IsInterfaceMap(Type t)
    {
        var has_attribute = t.IsDefined(typeof(MapAttribute));
        return has_attribute;
    }

    public static List<MethodInfo> InterfaceMethodsForType(Type t)
    {
        var all_methods = new List<MethodInfo>(t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));

        // Remove methods that we dont want to deal with
        // This includes setters and getters for properties
        all_methods.RemoveAll(x => x.Name.StartsWith("get_") || x.Name.StartsWith("set_"));

        return all_methods;
    }

    public static IntPtr FindOrCreateGameServerInterface(int hSteamUser, string pszVersion)
    {
        return FindOrCreateInterface(hSteamUser, 1, pszVersion);
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
            return SteamEmulator.SteamUtils.BaseAddress;
        }
        if (pszVersion.StartsWith("SteamUser"))
        {
            SteamEmulator.SteamUser.InterfaceVersion = pszVersion;
            return SteamEmulator.SteamUser.BaseAddress;
        }
        if (pszVersion.StartsWith("SteamClient"))
        {
            SteamEmulator.SteamClient.InterfaceVersion = pszVersion;
            return SteamEmulator.SteamClient.BaseAddress;
        }
        if (pszVersion.StartsWith("SteamFriends"))
        {
            SteamEmulator.SteamFriends.InterfaceVersion = pszVersion;
            return SteamEmulator.SteamFriends.BaseAddress;
        }
        if (pszVersion.StartsWith("SteamMatchMaking"))
        {
            SteamEmulator.SteamMatchmaking.InterfaceVersion = pszVersion;
            return SteamEmulator.SteamMatchmaking.BaseAddress;
        }
        if (pszVersion.StartsWith("SteamMatchGameSearch"))
        {
            SteamEmulator.SteamGameSearch.InterfaceVersion = pszVersion;
            return SteamEmulator.SteamGameSearch.BaseAddress;
        }
        if (pszVersion.StartsWith("SteamMatchMakingServers"))
        {
            SteamEmulator.SteamMatchMakingServers.InterfaceVersion = pszVersion;
            return SteamEmulator.SteamMatchMakingServers.BaseAddress;
        }
        if (pszVersion.StartsWith("STEAMUSERSTATS_INTERFACE_VERSION"))
        {
            SteamEmulator.SteamUserStats.InterfaceVersion = pszVersion;
            return SteamEmulator.SteamUserStats.BaseAddress;
        }
        if (pszVersion.StartsWith("STEAMAPPS_INTERFACE_VERSION"))
        {
            SteamEmulator.SteamApps.InterfaceVersion = pszVersion;
            return SteamEmulator.SteamApps.BaseAddress;
        }
        if (pszVersion.StartsWith("SteamNetworking"))
        {
            SteamEmulator.SteamNetworking.InterfaceVersion = pszVersion;
            return SteamEmulator.SteamNetworking.BaseAddress;
        }
        if (pszVersion.StartsWith("STEAMREMOTESTORAGE_INTERFACE_VERSION"))
        {
            SteamEmulator.SteamRemoteStorage.InterfaceVersion = pszVersion;
            return SteamEmulator.SteamRemoteStorage.BaseAddress;
        }
        if (pszVersion.StartsWith("STEAMSCREENSHOTS_INTERFACE_VERSION"))
        {
            SteamEmulator.SteamScreenshots.InterfaceVersion = pszVersion;
            return SteamEmulator.SteamScreenshots.BaseAddress;
        }
        if (pszVersion.StartsWith("STEAMHTTP_INTERFACE_VERSION"))
        {
            SteamEmulator.SteamFriends.InterfaceVersion = pszVersion;
            return SteamEmulator.SteamFriends.BaseAddress;
        }
        if (pszVersion.StartsWith("SteamController"))
        {
            SteamEmulator.SteamController.InterfaceVersion = pszVersion;
            return SteamEmulator.SteamController.BaseAddress;
        }
        if (pszVersion.StartsWith("STEAMUGC_INTERFACE_VERSION"))
        {
            SteamEmulator.SteamUGC.InterfaceVersion = pszVersion;
            return SteamEmulator.SteamUGC.BaseAddress;
        }
        if (pszVersion.StartsWith("STEAMAPPLIST_INTERFACE_VERSION"))
        {
            SteamEmulator.SteamAppList.InterfaceVersion = pszVersion;
            return SteamEmulator.SteamAppList.BaseAddress;
        }
        if (pszVersion.StartsWith("STEAMMUSIC_INTERFACE_VERSION"))
        {
            SteamEmulator.SteamMusic.InterfaceVersion = pszVersion;
            return SteamEmulator.SteamMusic.BaseAddress;
        }
        if (pszVersion.StartsWith("STEAMMUSICREMOTE_INTERFACE_VERSION"))
        {
            SteamEmulator.SteamMusicRemote.InterfaceVersion = pszVersion;
            return SteamEmulator.SteamMusicRemote.BaseAddress;
        }
        if (pszVersion.StartsWith("STEAMHTMLSURFACE_INTERFACE_VERSION_"))
        {
            SteamEmulator.SteamHTMLSurface.InterfaceVersion = pszVersion;
            return SteamEmulator.SteamHTMLSurface.BaseAddress;
        }
        if (pszVersion.StartsWith("STEAMINVENTORY_INTERFACE_V"))
        {
            SteamEmulator.SteamInventory.InterfaceVersion = pszVersion;
            return SteamEmulator.SteamInventory.BaseAddress;
        }
        if (pszVersion.StartsWith("STEAMVIDEO_INTERFACE_V"))
        {
            SteamEmulator.SteamVideo.InterfaceVersion = pszVersion;
            return SteamEmulator.SteamVideo.BaseAddress;
        }
        if (pszVersion.StartsWith("STEAMPARENTALSETTINGS_INTERFACE_VERSION"))
        {
            SteamEmulator.SteamParentalSettings.InterfaceVersion = pszVersion;
            return SteamEmulator.SteamParentalSettings.BaseAddress;
        }

        var (context, iface) = Context.CreateInterface(pszVersion);
        return context;


        //if (Interfaces.ContainsKey(pchVersion))
        //{
        //    return Interfaces[pchVersion];
        //}
        //var (Address, IBaseInterface) = Context.CreateInterface(pchVersion);
        //Interfaces.Add(pchVersion, Address);
        //return Address;
    }


}
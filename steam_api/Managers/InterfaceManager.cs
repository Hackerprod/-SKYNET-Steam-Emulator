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

    internal static IntPtr FindOrCreateInterface(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        if (Interfaces.ContainsKey(pchVersion))
        {
            return Interfaces[pchVersion];
        }
        var(Address, IBaseInterface) = Context.CreateInterface(pchVersion);
        Interfaces.Add(pchVersion, Address);
        return Address;
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

    public static IntPtr CreateInterface(string version)
    {
        return FindOrCreateInterface(1, 1, version);
    }

    public static IntPtr CreateInterface(int hSteamUser, int hSteamPipe, string version)
    {
        return FindOrCreateInterface(hSteamUser, hSteamPipe, version);
    }

    public static IntPtr CreateInterface(int hSteamUser, string pszVersion)
    {
        return FindOrCreateInterface(hSteamUser, 1, pszVersion);
    }

    public static IntPtr FindOrCreateGameServerInterface(int hSteamUser, string pszVersion)
    {
        return FindOrCreateInterface(hSteamUser, 1, pszVersion);
    }

    public static IntPtr GetGenericInterface(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        return FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
    }

}
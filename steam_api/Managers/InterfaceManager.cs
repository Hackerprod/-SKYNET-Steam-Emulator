using SKYNET;
using SKYNET.Delegate;
using SKYNET.Helper;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

public class InterfaceManager
{
    public static List<InterfaceDelegates> interface_delegates;

    static InterfaceManager()
    {
        interface_delegates = new List<InterfaceDelegates>();
    }

    public static void Initialize()
    {
        Assembly a = Assembly.LoadFile(Main.HookInterface.DllPath);

        foreach (var t in a.GetTypes())
        {
            if (t.IsDefined(typeof(DelegateAttribute)))
            {
                var attribute = t.GetCustomAttribute<DelegateAttribute>();
                var name = attribute.Name;

                var new_interface = new InterfaceDelegates { Name = name };

                //Main.Write(string.Format("Found interface delegates \"{0}\"", name));

                var types = t.GetNestedTypes(BindingFlags.Public);

                foreach (var type in types)
                {
                    // Filter out types that are not delegates
                    if (type.IsSubclassOf(typeof(System.Delegate))) new_interface.DelegateTypes.Add(type);
                }

                // Just assume all members are delegate types
                interface_delegates.Add(new_interface);
            }
        }
    }

    public static T CreateInterface<T>(out IntPtr BaseAddress) where T : SteamInterface
    {
        var (context, iface) = CreateInterface(typeof(T));
        BaseAddress = context;
        T baseClass = (T)iface;
        baseClass.BaseAddress = context;
        return (T)baseClass;
    }

    public static (IntPtr, SteamInterface) CreateInterface(Type type)
    {
        string Name = type.ToString();

        var iface = interface_delegates.Find(d => d.Name == Name);

        if (iface == null)
        {
            Main.Write(string.Format("Unable to find delegates for interface that implements {0}", Name));
            return (IntPtr.Zero, null);
        }

        var impl = new InterfaceImplementation
        {
            Name = Name,
            Type = type,
            Methods = InterfaceMethodsForType(type)
        };

        // Try to create a new context based on this interface + impl pair
        var (context, instance) = Create(iface, impl);

        return (context, instance);
    }

    private static (IntPtr, SteamInterface) Create(InterfaceDelegates iface, InterfaceImplementation impl)
    {
        var instance = Activator.CreateInstance(impl.Type);

        var new_delegates = new List<Delegate>();

        for (var i = 0; i < impl.Methods.Count; i++)
        {
            // Find the delegate type that matches the method
            var mi = impl.Methods[i];

            var type = iface.DelegateTypes.Find(x => x.Name.Equals(mi.Name));
            //Write($"Finding delegate for type {mi.Name}");

            if (type == null)
            {
                Main.Write(string.Format("Unable to find delegate for {0} in {1}!", mi.Name, iface.Name));
                return (IntPtr.Zero, null);
            }

            // Create new delegates that are bounded to this instance
            Delegate new_delegate;
            try
            {
                new_delegate = Delegate.CreateDelegate(type, instance, mi, true);
                new_delegates.Add(new_delegate);
            }
            catch (Exception e)
            {
                Main.Write(string.Format("EXCEPTION whilst binding function {0}, class {1}", mi.Name, impl.Name));
            }
        }

        impl.Delegates.Add(new_delegates);

        var ptr_size = Marshal.SizeOf(typeof(IntPtr));

        // Allocate enough space for the new pointers in local memory
        var vtable = Marshal.AllocHGlobal(impl.Methods.Count * ptr_size);

        for (var i = 0; i < new_delegates.Count; i++)
        {
            try
            {
                Marshal.WriteIntPtr(vtable, i * ptr_size, Marshal.GetFunctionPointerForDelegate(new_delegates[i]));
            }
            catch (Exception)
            {
                Main.Write($"Error Injecting Delegate {new_delegates[i]}");
            }
            // Create all function pointers as neccessary
        }

        // create the context
        var new_context = Marshal.AllocHGlobal(ptr_size);

        // Write the pointer to the vtable at the address pointed to by new_context;
        Marshal.WriteIntPtr(new_context, vtable);

        return (new_context, (SteamInterface)instance);
    }

    public static List<MethodInfo> InterfaceMethodsForType(Type t)
    {
        var all_methods = new List<MethodInfo>(t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
        all_methods.RemoveAll(x => x.Name.StartsWith("get_") || x.Name.StartsWith("set_"));
        return all_methods;
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

        Main.Write($"Not found Interface for {pszVersion}");
        return default;
    }
}
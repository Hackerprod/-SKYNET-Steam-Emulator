using SKYNET.Delegate.Helper;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SKYNET.Manager
{
    public class InterfaceManager
    {
        public static List<InterfaceDelegates> LoadedDelegates;
        public static List<System.Delegate> StoredDelegates;

        //https://github.com/Reloaded-Project/Reloaded.Memory/blob/master/Docs/Examples.md#sample-readingwriting-structs

        static InterfaceManager()
        {
            LoadedDelegates = new List<InterfaceDelegates>();
            StoredDelegates = new List<System.Delegate>();
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

                    var types = t.GetNestedTypes(BindingFlags.Public);

                    foreach (var type in types)
                    {
                        if (type.IsSubclassOf(typeof(System.Delegate))) new_interface.DelegateTypes.Add(type);
                    }

                    LoadedDelegates.Add(new_interface);
                }
            }
        }

        public static T CreateSteamInterface<T>() where T : SteamInterface
        {
            T baseClass = CreateInterface<T>(out IntPtr MemoryAddress);
            baseClass.MemoryAddress = MemoryAddress;
            return (T)baseClass;
        }

        public static T CreateInterface<T>(out IntPtr MemoryAddress)
        {
            MemoryAddress = IntPtr.Zero;

            var type = typeof(T);
            var Name = typeof(T).ToString();

            var delegatesForType = LoadedDelegates.Find(d => d.Name == Name);

            if (delegatesForType == null)
            {
                Main.Write($"Unable to find delegates for interface that implements {Name}");
                return default;
            }

            var instance = Activator.CreateInstance(type);

            var Methods = MethodsInType(type);

            var new_delegates = new List<System.Delegate>();

            foreach (var MethodInfo in Methods)
            {
                var delegateType = delegatesForType.DelegateTypes.Find(x => x.Name.Equals(MethodInfo.Name));

                if (delegateType == null)
                {
                    Main.Write($"Unable to find delegate for {MethodInfo.Name} in {delegatesForType.Name}!");
                    return default;
                }

                try
                {
                    System.Delegate new_delegate = System.Delegate.CreateDelegate(delegateType, instance, MethodInfo, true);
                    new_delegates.Add(new_delegate);
                }
                catch (Exception e)
                {
                    Main.Write($"Exception binding function {MethodInfo.Name} in {Name}");
                }
            }

            StoredDelegates.AddRange(new_delegates);

            var ptr_size = Marshal.SizeOf(typeof(IntPtr));

            var vtable = Marshal.AllocHGlobal(Methods.Count * ptr_size);

            for (var i = 0; i < new_delegates.Count; i++)
            {
                try
                {
                    Marshal.WriteIntPtr(vtable, i * ptr_size, Marshal.GetFunctionPointerForDelegate(new_delegates[i]));
                }
                catch (Exception ex)
                {
                    Main.Write($"Error Injecting Delegate {new_delegates[i]}, {ex.Message}");
                }
            }

            var new_context = Marshal.AllocHGlobal(ptr_size);

            Marshal.WriteIntPtr(new_context, vtable);

            MemoryAddress = new_context;
            return (T)instance;
        }

        public static IntPtr WriteInMemory(object obj)
        {
            Type type = obj.GetType();

            var ptr_size = Marshal.SizeOf(typeof(IntPtr));

            var obj_size = Marshal.SizeOf(type);
            var ptr = Marshal.AllocHGlobal(obj_size);
            Marshal.StructureToPtr(obj, ptr, true);

            var address = Marshal.AllocHGlobal(ptr_size);

            Marshal.WriteIntPtr(address, ptr);

            return address;
        }


        public static List<MethodInfo> MethodsInType(Type t)
        {
            var methods = new List<MethodInfo>(t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            methods.RemoveAll(x => x.Name.StartsWith("get_") || x.Name.StartsWith("set_"));
            return methods;
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
                SteamEmulator.SteamFriends.InterfaceVersion = pszVersion;
                return SteamEmulator.SteamFriends.MemoryAddress;
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

            Main.Write($"Not found Interface for {pszVersion}");
            return default;
        }
    }
}
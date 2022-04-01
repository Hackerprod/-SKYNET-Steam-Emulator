using SKYNET;
using SKYNET.Delegate;
using SKYNET.Delegate.Helper;
using SKYNET.Helper;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

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

                    //Main.Write(string.Format("Found interface delegates \"{0}\"", name));

                    var types = t.GetNestedTypes(BindingFlags.Public);

                    foreach (var type in types)
                    {
                        // Filter out types that are not delegates
                        if (type.IsSubclassOf(typeof(System.Delegate))) new_interface.DelegateTypes.Add(type);
                    }

                    // Just assume all members are delegate types
                    LoadedDelegates.Add(new_interface);
                }
            }
        }

        public static T CreateInstance<T>(out IntPtr MemoryAddress)
        {
            var instance = Activator.CreateInstance(typeof(T));
            IntPtr pointer = IntPtr.Zero;
            ReferenceHelpers.GetPinnedPtr(instance, ptr => pointer = ptr);
            MemoryAddress = pointer;
            return (T)instance;
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
            Type type = typeof(T);
            string Name = type.ToString();

            var iface = LoadedDelegates.Find(d => d.Name == Name);

            if (iface == null)
            {
                Main.Write(string.Format("Unable to find delegates for interface that implements {0}", Name));
                return default;
            }

            var instance = Activator.CreateInstance(type);

            var Methods = InterfaceMethodsForType(type);

            var new_delegates = new List<System.Delegate>();

            foreach (var MethodInfo in Methods)
            {
                // Find the delegate type that matches the method
                var delegateType = iface.DelegateTypes.Find(x => x.Name.Equals(MethodInfo.Name));

                //Write($"Finding delegate for type {mi.Name}");

                if (delegateType == null)
                {
                    Main.Write(string.Format("Unable to find delegate for {0} in {1}!", MethodInfo.Name, iface.Name));
                    return default;
                }

                // Create new delegates that are bounded to this instance
                System.Delegate new_delegate;
                try
                {
                    new_delegate = System.Delegate.CreateDelegate(delegateType, instance, MethodInfo, true);
                    new_delegates.Add(new_delegate);
                }
                catch (Exception e)
                {
                    Main.Write(string.Format("EXCEPTION whilst binding function {0}, class {1}", MethodInfo.Name, Name));
                }
            }

            StoredDelegates.AddRange(new_delegates);

            var ptr_size = Marshal.SizeOf(typeof(IntPtr));

            // Allocate enough space for the new pointers in local memory
            var vtable = Marshal.AllocHGlobal(Methods.Count * ptr_size);

            for (var i = 0; i < new_delegates.Count; i++)
            {
                try
                {
                    Marshal.WriteIntPtr(vtable, i * ptr_size, Marshal.GetFunctionPointerForDelegate(new_delegates[i]));
                }
                catch (Exception ex)
                {
                    Main.Write($"Error Injecting Delegate {new_delegates[i]} - {ex.Message}");
                }
                // Create all function pointers as neccessary
            }

            // create the context
            var new_context = Marshal.AllocHGlobal(ptr_size);

            // Write the pointer to the vtable at the address pointed to by new_context;
            Marshal.WriteIntPtr(new_context, vtable);

            MemoryAddress = new_context;
            return (T)instance;
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
    public static class ReferenceHelpers
    {
        public static readonly Action<object, Action<IntPtr>> GetPinnedPtr;

        static ReferenceHelpers()
        {
            var dyn = new DynamicMethod("GetPinnedPtr", typeof(void), new[] { typeof(object), typeof(Action<IntPtr>) }, typeof(ReferenceHelpers).Module);
            var il = dyn.GetILGenerator();
            il.DeclareLocal(typeof(object), true);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Conv_I);
            il.Emit(OpCodes.Call, typeof(Action<IntPtr>).GetMethod("Invoke"));
            il.Emit(OpCodes.Ret);
            GetPinnedPtr = (Action<object, Action<IntPtr>>)dyn.CreateDelegate(typeof(Action<object, Action<IntPtr>>));
        }
    }
}
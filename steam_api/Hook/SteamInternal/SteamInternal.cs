using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SKYNET;
using SKYNET.Helper;
using SKYNET.Types;

namespace SKYNET.Hook.Handles
{
    public partial class SteamInternal : BaseHook
    {
        public override bool Installed { get; set; }
        public override void Install()
        {
            // SteamInternal Handles
            base.Install<SteamInternal_FindOrCreateUserInterfaceDelegate>("SteamInternal_FindOrCreateUserInterface", _SteamInternal_FindOrCreateUserInterface, new SteamInternal_FindOrCreateUserInterfaceDelegate(SteamInternal_FindOrCreateUserInterface));
            base.Install<SteamInternal_FindOrCreateGameServerInterfaceDelegate>("SteamInternal_FindOrCreateGameServerInterface", _SteamInternal_FindOrCreateGameServerInterfaceDelegate, new SteamInternal_FindOrCreateGameServerInterfaceDelegate(SteamInternal_FindOrCreateGameServerInterface));
            base.Install<SteamInternal_CreateInterfaceDelegate>("SteamInternal_CreateInterface", _SteamInternal_CreateInterfaceDelegate, new SteamInternal_CreateInterfaceDelegate(SteamInternal_CreateInterface));
            base.Install<SteamInternal_GameServer_InitDelegate>("SteamInternal_GameServer_Init", _SteamInternal_GameServer_InitDelegate, new SteamInternal_GameServer_InitDelegate(SteamInternal_GameServer_Init));
            base.Install<SteamInternal_ContextInitDelegate>("SteamInternal_ContextInit", _SteamInternal_ContextInitDelegate, new SteamInternal_ContextInitDelegate(SteamInternal_ContextInit));
        }
        public IntPtr SteamInternal_FindOrCreateUserInterface(int hSteamUser, [MarshalAs(UnmanagedType.LPStr)] string pszVersion)
        {
            Write($"SteamInternal_FindOrCreateUserInterface {pszVersion}");
            return InterfaceManager.FindOrCreateInterface(pszVersion); 
        }

        public IntPtr SteamInternal_FindOrCreateGameServerInterface(int hSteamUser, [MarshalAs(UnmanagedType.LPStr)] string pszVersion)
        {
            Write($"SteamInternal_FindOrCreateGameServerInterface {pszVersion}");
            return InterfaceManager.FindOrCreateInterface(pszVersion);
        }

        public IntPtr SteamInternal_CreateInterface([MarshalAs(UnmanagedType.LPStr)] string pszVersion)
        {
            Write($"SteamInternal_CreateInterface {pszVersion}");
            return InterfaceManager.FindOrCreateInterface(pszVersion);
        }

        public bool SteamInternal_GameServer_Init(IntPtr unIP, IntPtr usPort, IntPtr usGamePort, IntPtr usQueryPort, IntPtr eServerMode, IntPtr pchVersionString)
        {
            Write($"SteamInternal_GameServer_Init");
            return true;
        }
        
        public IntPtr SteamInternal_ContextInit(IntPtr pContextInitData)
        {
            Write($"SteamInternal_ContextInit");

            var context = AddressHelper.GetAddress(SteamEmulator.Context);
            Write(context);

            return SteamEmulator.Context.BaseAddress;
        }



        public override void Write(object v)
        {
            Main.Write("SteamInternal", v);
        }
    }
}

public static class AddressHelper
{
    private static object mutualObject;
    private static ObjectReinterpreter reinterpreter;

    static AddressHelper()
    {
        AddressHelper.mutualObject = new object();
        AddressHelper.reinterpreter = new ObjectReinterpreter();
        AddressHelper.reinterpreter.AsObject = new ObjectWrapper();
    }

    public static IntPtr GetAddress(object obj)
    {
        lock (AddressHelper.mutualObject)
        {
            AddressHelper.reinterpreter.AsObject.Object = obj;
            IntPtr address = AddressHelper.reinterpreter.AsIntPtr.Value;
            AddressHelper.reinterpreter.AsObject.Object = null;
            return address;
        }
    }

    public static T GetInstance<T>(IntPtr address)
    {
        lock (AddressHelper.mutualObject)
        {
            AddressHelper.reinterpreter.AsIntPtr.Value = address;
            return (T)AddressHelper.reinterpreter.AsObject.Object;
        }
    }

    // I bet you thought C# was type-safe.
    [StructLayout(LayoutKind.Explicit)]
    private struct ObjectReinterpreter
    {
        [FieldOffset(0)] public ObjectWrapper AsObject;
        [FieldOffset(0)] public IntPtrWrapper AsIntPtr;
    }

    private class ObjectWrapper
    {
        public object Object;
    }

    private class IntPtrWrapper
    {
        public IntPtr Value;
    }
}

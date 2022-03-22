using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Interface;
using SKYNET;
using SKYNET.Helper;
using SKYNET.Interface;
using SKYNET.Types;
using Steamworks.Core;
using static SteamInternal;

namespace SKYNET.Hook
{
    public partial class SteamInternal : BaseHook
    {
        public override bool Installed { get; set; }

        public SteamInternal()
        {
            Installed = false;
        }

        public override void Install()
        {
            Write("Injecting SteamInternal functions");
            base.Install<SteamInternal_FindOrCreateUserInterfaceDelegate>("SteamInternal_FindOrCreateUserInterface", _SteamInternal_FindOrCreateUserInterface, new SteamInternal_FindOrCreateUserInterfaceDelegate(SteamInternal_FindOrCreateUserInterface));
            base.Install<SteamInternal_FindOrCreateGameServerInterfaceDelegate>("SteamInternal_FindOrCreateGameServerInterface", _SteamInternal_FindOrCreateGameServerInterfaceDelegate, new SteamInternal_FindOrCreateGameServerInterfaceDelegate(SteamInternal_FindOrCreateGameServerInterface));
            base.Install<SteamInternal_CreateInterfaceDelegate>("SteamInternal_CreateInterface", _SteamInternal_CreateInterfaceDelegate, new SteamInternal_CreateInterfaceDelegate(SteamInternal_CreateInterface));
            base.Install<SteamInternal_GameServer_InitDelegate>("SteamInternal_GameServer_Init", _SteamInternal_GameServer_InitDelegate, new SteamInternal_GameServer_InitDelegate(SteamInternal_GameServer_Init));
            //base.Install<SteamInternal_ContextInitDelegate>("SteamInternal_ContextInit", _SteamInternal_ContextInitDelegate, new SteamInternal_ContextInitDelegate(SteamInternal_ContextInit));
            Installed = true;
        }

        public IntPtr SteamInternal_FindOrCreateUserInterface(IntPtr hSteamUser, [MarshalAs(UnmanagedType.LPStr)] string pszVersion)
        {
            Write($"SteamInternal_FindOrCreateUserInterface {pszVersion}");

            return GetInterface(pszVersion);
        }

        public IntPtr SteamInternal_FindOrCreateGameServerInterface(IntPtr hSteamUser, IntPtr pszVersion)
        {
            Write($"SteamInternal_FindOrCreateGameServerInterface {pszVersion}");
            return InterfaceManager.FindOrCreateGameServerInterface(hSteamUser, pszVersion);
        }

        public IntPtr SteamInternal_CreateInterface([MarshalAs(UnmanagedType.LPStr)] string pszVersion)
        {
            Write($"SteamInternal_CreateInterface {pszVersion}");
            return GetInterface(pszVersion);
        }

        public bool SteamInternal_GameServer_Init(IntPtr unIP, IntPtr usPort, IntPtr usGamePort, IntPtr usQueryPort, IntPtr eServerMode, IntPtr pchVersionString)
        {
            Write($"SteamInternal_GameServer_Init");
            return true;
        }
        
        public IntPtr SteamInternal_ContextInit(IntPtr pContextInitData)
        {
            ContextInitData contextInitData = Marshal.PtrToStructure<ContextInitData>(pContextInitData);

            if (contextInitData.counter != 1)
            {
                Write($"SteamInternal_ContextInit");

                IntPtr MemoryAddress = MemoryHelper.MemoryAddress(SteamEmulator.Context);

                return MemoryAddress == IntPtr.Zero ? contextInitData.Context : MemoryAddress;
            }
            return contextInitData.Context;
        }

        private IntPtr GetInterface(string pszVersion)
        {
            if (pszVersion.StartsWith("SteamUtils"))
            {
                pszVersion = "";
            }
            if (pszVersion.StartsWith("SteamUser"))
            { pszVersion = "SteamUser019"; }
            if (pszVersion.StartsWith("SteamClient"))
            { pszVersion = "SteamClient017"; }
            if (pszVersion.StartsWith("SteamFriends"))
            { pszVersion = "SteamFriends015"; }
            if (pszVersion.StartsWith("SteamMatchMaking"))
            { pszVersion = "SteamMatchmaking009"; }
            if (pszVersion.StartsWith("SteamMatchGameSearch"))
            { pszVersion = "SteamMatchmakingServers002"; }
            if (pszVersion.StartsWith("SteamMatchMakingServers"))
            { pszVersion = "SteamMatchmakingServers002"; }
            if (pszVersion.StartsWith("STEAMUSERSTATS_INTERFACE_VERSION"))
            { pszVersion = "SteamUserStats011"; }
            if (pszVersion.StartsWith("STEAMAPPS_INTERFACE_VERSION"))
            { pszVersion = "SteamApps008"; }
            if (pszVersion.StartsWith("SteamNetworking"))
            { pszVersion = "SteamNetworking005"; }
            if (pszVersion.StartsWith("STEAMREMOTESTORAGE_INTERFACE_VERSION"))
            { pszVersion = "SteamRemoteStorage014"; }
            if (pszVersion.StartsWith("STEAMSCREENSHOTS_INTERFACE_VERSION"))
            { pszVersion = "SteamScreenshots003"; }
            if (pszVersion.StartsWith("STEAMHTTP_INTERFACE_VERSION"))
            { pszVersion = "SteamHTTP002"; }
            if (pszVersion.StartsWith("SteamController"))
            { pszVersion = "SteamController006"; }
            if (pszVersion.StartsWith("STEAMUGC_INTERFACE_VERSION"))
            { pszVersion = "SteamUGC010"; }
            if (pszVersion.StartsWith("STEAMAPPLIST_INTERFACE_VERSION"))
            { pszVersion = "SteamAppList001"; }
            if (pszVersion.StartsWith("STEAMMUSIC_INTERFACE_VERSION"))
            { pszVersion = "SteamMusic001"; }
            if (pszVersion.StartsWith("STEAMMUSICREMOTE_INTERFACE_VERSION"))
            { pszVersion = "SteamMusicRemote001"; }
            if (pszVersion.StartsWith("STEAMHTMLSURFACE_INTERFACE_VERSION_"))
            { pszVersion = "SteamHTMLSurface004"; }
            if (pszVersion.StartsWith("STEAMINVENTORY_INTERFACE_V"))
            { pszVersion = "SteamInventory003"; }
            if (pszVersion.StartsWith("STEAMVIDEO_INTERFACE_V"))
            { pszVersion = "SteamVideo002"; }
            //if (pszVersion.StartsWith("STEAMPARENTALSETTINGS_INTERFACE_VERSION"))
            //{ pszVersion = ""; }

            var (context, iface) = Context.CreateInterface(pszVersion);
            return context;
        }

    }
}

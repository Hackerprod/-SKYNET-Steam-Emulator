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
        public unsafe override void Install()
        {
            // SteamInternal Handles
            base.Install<SteamInternal_FindOrCreateUserInterfaceDelegate>("SteamInternal_FindOrCreateUserInterface", _SteamInternal_FindOrCreateUserInterface, new SteamInternal_FindOrCreateUserInterfaceDelegate(SteamInternal_FindOrCreateUserInterface));
            base.Install<SteamInternal_FindOrCreateGameServerInterfaceDelegate>("SteamInternal_FindOrCreateGameServerInterface", _SteamInternal_FindOrCreateGameServerInterfaceDelegate, new SteamInternal_FindOrCreateGameServerInterfaceDelegate(SteamInternal_FindOrCreateGameServerInterface));
            base.Install<SteamInternal_CreateInterfaceDelegate>("SteamInternal_CreateInterface", _SteamInternal_CreateInterfaceDelegate, new SteamInternal_CreateInterfaceDelegate(SteamInternal_CreateInterface));
            base.Install<SteamInternal_GameServer_InitDelegate>("SteamInternal_GameServer_Init", _SteamInternal_GameServer_InitDelegate, new SteamInternal_GameServer_InitDelegate(SteamInternal_GameServer_Init));
            //base.Install<SteamInternal_ContextInitDelegate>("SteamInternal_ContextInit", _SteamInternal_ContextInitDelegate, new SteamInternal_ContextInitDelegate(SteamInternal_ContextInit));
            ContextInitTest.Init();
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

        public bool SteamInternal_GameServer_Init(IntPtr unIP, IntPtr usPort, IntPtr usGamePort, IntPtr usQueryPort, IntPtr eServerMode, [MarshalAs(UnmanagedType.LPStr)] string pchVersionString)
        {
            Write($"SteamInternal_GameServer_Init {pchVersionString}");
            return true;
        }

        //public IntPtr SteamInternal_ContextInit(IntPtr pContextInitData)
        //{
        //    Write($"SteamInternal_ContextInit");
        //    return SteamEmulator.Context.BaseAddress;
        //}

        public unsafe CSteamApiContext* SteamInternal_ContextInit(ContextInitData* pContextInitData)
        {
            Write($"SteamInternal_ContextInit Counter: {pContextInitData->counter}, SteamUGC {pContextInitData->Context->SteamClient()}");

            ////var sa = pContextInitData->Context->m_pSteamClient.ToInt32();
            ////Write($"wuassa");

            //pContextInitData->Context->Clear();

            //if (pContextInitData->counter == 0)
            //{
            //    pContextInitData->counter = 1;
            //    pContextInitData->Context->Init();
            //}

            return pContextInitData->Context;
        }

        public unsafe struct ContextInitData
        {
            public CSteamApiContext* Context;
            public uint counter;
        }



        public override void Write(object v)
        {
            Main.Write("SteamInternal", v);
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct CSteamApiContext 
    {
        public IntPtr m_pSteamClient;              //ISteamClient* 
        public IntPtr m_pSteamUser;                //ISteamUser* 
        public IntPtr m_pSteamFriends;             //ISteamFriends* 
        public IntPtr m_pSteamUtils;               //ISteamUtils* 
        public IntPtr m_pSteamMatchmaking;         //ISteamMatchmaking* 
        public IntPtr m_pSteamGameSearch;          //ISteamGameSearch* 
        public IntPtr m_pSteamUserStats;           //ISteamUserStats* 
        public IntPtr m_pSteamApps;                //ISteamApps* 
        public IntPtr m_pSteamMatchmakingServers;  //ISteamMatchmakingServers* 
        public IntPtr m_pSteamNetworking;          //ISteamNetworking* 
        public IntPtr m_pSteamRemoteStorage;       //ISteamRemoteStorage* 
        public IntPtr m_pSteamScreenshots;         //ISteamScreenshots* 
        public IntPtr m_pSteamHTTP;                //ISteamHTTP* 
        public IntPtr m_pSteamController;          //ISteamController* 
        public IntPtr m_pSteamUGC;                 //ISteamUGC* 
        public IntPtr m_pSteamAppList;             //ISteamAppList* 
        public IntPtr m_pSteamMusic;               //ISteamMusic* 
        public IntPtr m_pSteamMusicRemote;         //ISteamMusicRemote* 
        public IntPtr m_pSteamHTMLSurface;         //ISteamHTMLSurface* 
        public IntPtr m_pSteamInventory;           //ISteamInventory* 
        public IntPtr m_pSteamVideo;               //ISteamVideo* 
        public IntPtr m_pSteamTV;                  //ISteamTV* 
        public IntPtr m_pSteamParentalSettings;    //ISteamParentalSettings* 
        public IntPtr m_pSteamInput;               //ISteamInput* 

        public IntPtr SteamClient() => m_pSteamClient;
        public IntPtr SteamUser() => m_pSteamUser;
        public IntPtr SteamFriends() => m_pSteamFriends;
        public IntPtr SteamUtils() => m_pSteamUtils;
        public IntPtr SteamMatchmaking() => m_pSteamMatchmaking;
        public IntPtr SteamGameSearch() => m_pSteamGameSearch;
        public IntPtr SteamUserStats() => m_pSteamUserStats;
        public IntPtr SteamApps() => m_pSteamApps;
        public IntPtr SteamMatchmakingServers() => m_pSteamMatchmakingServers;
        public IntPtr SteamNetworking() => m_pSteamNetworking;
        public IntPtr SteamRemoteStorage() => m_pSteamRemoteStorage;
        public IntPtr SteamScreenshots() => m_pSteamScreenshots;
        public IntPtr SteamHTTP() => m_pSteamHTTP;
        public IntPtr SteamController() => m_pSteamController;
        public IntPtr SteamUGC() => m_pSteamUGC;
        public IntPtr SteamAppList() => m_pSteamAppList;
        public IntPtr SteamMusic() => m_pSteamMusic;
        public IntPtr SteamMusicRemote() => m_pSteamMusicRemote;
        public IntPtr SteamHTMLSurface() => m_pSteamHTMLSurface;
        public IntPtr SteamInventory() => m_pSteamInventory;
        public IntPtr SteamVideo() => m_pSteamVideo;
        public IntPtr SteamTV() => m_pSteamTV;
        public IntPtr SteamParentalSettings() => m_pSteamParentalSettings;
        public IntPtr SteamInput() => m_pSteamInput;

        public void Clear()
        {
            SteamEmulator.Write($"Cleaning CSteamApiContext");

            m_pSteamClient = IntPtr.Zero;
            m_pSteamUser = IntPtr.Zero;
            m_pSteamFriends = IntPtr.Zero;
            m_pSteamUtils = IntPtr.Zero;
            m_pSteamMatchmaking = IntPtr.Zero;
            m_pSteamUserStats = IntPtr.Zero;
            m_pSteamApps = IntPtr.Zero;
            m_pSteamMatchmakingServers = IntPtr.Zero;
            m_pSteamNetworking = IntPtr.Zero;
            m_pSteamRemoteStorage = IntPtr.Zero;
            m_pSteamScreenshots = IntPtr.Zero;
            m_pSteamHTTP = IntPtr.Zero;
            m_pSteamController = IntPtr.Zero;
            m_pSteamUGC = IntPtr.Zero;
            m_pSteamAppList = IntPtr.Zero;
            m_pSteamMusic = IntPtr.Zero;
            m_pSteamMusicRemote = IntPtr.Zero;
            m_pSteamHTMLSurface = IntPtr.Zero;
            m_pSteamInventory = IntPtr.Zero;
            m_pSteamVideo = IntPtr.Zero;

            SteamEmulator.Write($"CSteamApiContext cleaned");
        }

        public bool Init()
        {
            SteamEmulator.Write($"Initializing CSteamApiContext");

            var a_steamUser = SteamEmulator.HSteamUser;
            var a_steamPipe = SteamEmulator.HSteamPipe;

            if ((int)a_steamPipe == 0)
            {
                return false;
            }

            m_pSteamClient = SteamEmulator.SteamClient.BaseAddress;
            if (m_pSteamClient == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamUser = SteamEmulator.SteamUser.BaseAddress;
            if (m_pSteamUser == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamFriends = SteamEmulator.SteamFriends.BaseAddress;
            if (m_pSteamFriends == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamUtils = SteamEmulator.SteamUtils.BaseAddress;
            if (m_pSteamUtils == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamMatchmaking = SteamEmulator.SteamMatchmaking.BaseAddress;
            if (m_pSteamMatchmaking == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamMatchmakingServers = SteamEmulator.SteamMatchMakingServers.BaseAddress;
            if (m_pSteamMatchmakingServers == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamUserStats = SteamEmulator.SteamUserStats.BaseAddress;
            if (m_pSteamUserStats == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamApps = SteamEmulator.SteamApps.BaseAddress;
            if (m_pSteamApps == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamNetworking = SteamEmulator.SteamNetworking.BaseAddress;
            if (m_pSteamNetworking == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamRemoteStorage = SteamEmulator.SteamMusicRemote.BaseAddress;
            if (m_pSteamRemoteStorage == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamScreenshots = SteamEmulator.SteamScreenshots.BaseAddress;
            if (m_pSteamScreenshots == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamHTTP = SteamEmulator.SteamHTTP.BaseAddress;
            if (m_pSteamHTTP == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamController = SteamEmulator.SteamController.BaseAddress;
            if (m_pSteamController == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamUGC = SteamEmulator.SteamUGC.BaseAddress;
            if (m_pSteamUGC == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamAppList = SteamEmulator.SteamAppList.BaseAddress;
            if (m_pSteamAppList == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamMusic = SteamEmulator.SteamMusic.BaseAddress;
            if (m_pSteamMusic == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamMusicRemote = SteamEmulator.SteamMusicRemote.BaseAddress;
            if (m_pSteamMusicRemote == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamHTMLSurface = SteamEmulator.SteamHTMLSurface.BaseAddress;
            if (m_pSteamHTMLSurface == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamInventory = SteamEmulator.SteamInventory.BaseAddress;
            if (m_pSteamInventory == IntPtr.Zero)
            {
                return false;
            }

            m_pSteamVideo = SteamEmulator.SteamVideo.BaseAddress;
            if (m_pSteamVideo == IntPtr.Zero)
            {
                return false;
            }

            return true;
        }
    }

}


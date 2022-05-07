using SKYNET.Managers;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SKYNET.Helper;
using SKYNET.Callback;

using SteamAPICall_t = System.UInt64;
using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;
using Steamworks;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_Init()
        {
            if (!SteamEmulator.Initialized)
            {
                Write($"SteamAPI_Init : Initializing");
                SteamEmulator.Initialize();
            }
            else
            {
                Write($"SteamAPI_Init : Initialized");
            }

            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_Shutdown(IntPtr pContextInitData)
        {
            Write("SteamAPI_Shutdown");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public unsafe static void SteamAPI_RegisterCallback(IntPtr pCallback, int iCallback)
        {
            //CallbackWrapper.RegisterCallback(pCallback, iCallback);
            //return;

            // Check if Steam emulator is not initialized 
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }

            if (!SteamEmulator.Initialized) return;

            try
            {
                var callMessage = $"SteamAPI_RegisterCallback: ";

                SteamCallback sCallback = new SteamCallback(pCallback, iCallback);

                var isGameServer = sCallback.HasGameserver ? "[ GAMESERVER ]" : "[   CLIENT   ]";
                var space = (int)sCallback.CallbackType < 1000 ? " " : "";

                callMessage += $"{isGameServer}  {(int)sCallback.CallbackType} {space} {sCallback.BaseType} {sCallback.CallbackType}";

                Write(callMessage);

                CallbackManager.RegisterCallback(sCallback);
            }
            catch (Exception ex)
            {
                Write("SteamAPI_RegisterCallback " + ex.ToString());
            }
        }


        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_RunCallbacks()
        {
            Write("SteamAPI_RunCallbacks");
            CallbackManager.RunCallbacks();
            //CallbackWrapper.RunCallbacks();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_RegisterCallResult(IntPtr pCallback, SteamAPICall_t hAPICall)
        {
            //CallbackWrapper.RegisterCallResult(pCallback, hAPICall);
            //return;

            try
            {
                var callMessage = $"SteamAPI_RegisterCallResult: ";

                SteamCallback sCallback = new SteamCallback(pCallback, true);
                sCallback.SteamAPICall = hAPICall;

                var isGameServer = sCallback.HasGameserver ? "[ GAMESERVER ]" : "[   CLIENT   ]";
                var space = (int)sCallback.CallbackType < 1000 ? " " : "";

                callMessage += $"{isGameServer}  {hAPICall}  {(int)sCallback.CallbackType} {space} {sCallback.BaseType} {sCallback.CallbackType}";

                Write(callMessage);

                CallbackManager.RegisterCallResult(sCallback);
            }
            catch (Exception ex)
            {
                Write($"SteamAPI_RegisterCallResult: {ex}");
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_UnregisterCallback(IntPtr pCallback)
        {
            //CallbackWrapper.UnregisterCallback(pCallback);
            //return;
            SteamCallback sCallback = new SteamCallback(pCallback);
            Write($"SteamAPI_UnregisterCallback {sCallback.CallbackType}");
            CallbackManager.UnregisterCallback(pCallback);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_UnregisterCallResult(IntPtr pCallback, SteamAPICall_t hAPICall)
        {
            Write("SteamAPI_UnregisterCallResult\n");
            //CallbackManager.UnregisterCallResult(pCallback, hAPICall);
            //CallbackWrapper.UnregisterCallResult(pCallback, hAPICall);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_InitSafe()
        {
            Write("SteamAPI_InitSafe");
            return SteamAPI_Init();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_InitAnonymousUser()
        {
            Write("SteamAPI_InitAnonymousUser");
            SteamAPI_Init();
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_IsSteamRunning()
        {
            Write($"{"SteamAPI_IsSteamRunning"}");
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_RestartAppIfNecessary(uint appId)
        {
            Write($"SteamAPI_RestartAppIfNecessary called {appId}");
            SteamEmulator.AppId = appId;
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_GetSteamInstallPath()
        {
            Write($"{"SteamAPI_GetSteamInstallPath"}");

            string path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            return Marshal.StringToBSTR(path);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HSteamUser SteamAPI_GetHSteamUser()
        {
            Write("SteamAPI_GetHSteamUser");
            if (SteamEmulator.HSteamUser == 0)
            {
                SteamEmulator.CreateSteamUser();
            }
            return SteamEmulator.HSteamUser;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HSteamPipe SteamAPI_GetHSteamPipe()
        {
            Write("SteamAPI_GetHSteamPipe");
            if (SteamEmulator.HSteamPipe == 0)
            {
                SteamEmulator.CreateSteamPipe();
            }
            return SteamEmulator.HSteamPipe;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HSteamPipe GetHSteamPipe()
        {
            Write("GetHSteamPipe");
            return SteamAPI_GetHSteamPipe();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HSteamUser GetHSteamUser()
        {
            Write("GetHSteamUser");
            return SteamAPI_GetHSteamUser();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SetTryCatchCallbacks(bool bTryCatchCallbacks)
        {
            Write($"SteamAPI_SetTryCatchCallbacks");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SetBreakpadAppID(UInt32 unAppID)
        {
            Write($"SteamAPI_SetBreakpadAppID {unAppID}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_UseBreakpadCrashHandler([MarshalAs(UnmanagedType.LPStr)] string pchVersion, [MarshalAs(UnmanagedType.LPStr)] string pchDate, [MarshalAs(UnmanagedType.LPStr)] string pchTime, bool bFullMemoryDumps, IntPtr pvContext, IntPtr m_pfnPreMinidumpCallback)
        {
            Write($"SteamAPI_UseBreakpadCrashHandler | Date: {pchDate} | Time: {pchTime} | Version : {pchVersion}");
        }

        
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ManualDispatch_Init()
        {
            Write($"SteamAPI_ManualDispatch_Init");
        }


        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ManualDispatch_RunFrame(HSteamPipe hSteamPipe)
        {
            Write($"SteamAPI_ManualDispatch_RunFrame");
            CallbackManager.RunCallbacks();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ManualDispatch_GetNextCallback(HSteamPipe hSteamPipe, IntPtr pCallbackMsg)
        {
            Write($"SteamAPI_ManualDispatch_GetNextCallback");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ManualDispatch_FreeLastCallback(HSteamPipe hSteamPipe)
        {
            Write($"SteamAPI_ManualDispatch_FreeLastCallback");
            //Marshal.FreeHGlobal();
        }


        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ManualDispatch_GetAPICallResult(HSteamPipe hSteamPipe, SteamAPICall_t hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, ref bool pbFailed)
        {
            Write($"SteamAPI_ManualDispatch_GetAPICallResult");
            return SteamEmulator.SteamUtils.GetAPICallResult(hSteamAPICall, pCallback, cubCallback, iCallbackExpected, ref pbFailed);
        }


        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_RestartApp(UInt32 appid)
        {
            Write($"SteamAPI_RestartApp");
            return SteamAPI_RestartAppIfNecessary(appid);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SetMiniDumpComment([MarshalAs(UnmanagedType.LPStr)] string pchMsg)
        {
            string Msg = "SteamAPI_SetMiniDumpComment" + Environment.NewLine;
            Msg += "////////////////////////////// Mini Dump Content \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\" + Environment.NewLine;
            Msg += $"{pchMsg}" + Environment.NewLine;
            Msg += "//////////////////////////////   End Mini Dump   \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\" + Environment.NewLine;
            //Write(Msg);
            Write("SteamAPI_SetMiniDumpComment");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_WriteMiniDump(UInt32 uStructuredExceptionCode, IntPtr pvExceptionInfo, UInt32 uBuildID)
        {
            Write($"SteamAPI_WriteMiniDump");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ReleaseCurrentThreadMemory()
        {
            Write($"SteamAPI_ReleaseCurrentThreadMemory");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_gameserveritem_t_Construct(IntPtr self)
        {
            Write($"SteamAPI_gameserveritem_t_Construct");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_gameserveritem_t_GetName(IntPtr self)
        {
            Write($"SteamAPI_gameserveritem_t_GetName");
            return "";
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_gameserveritem_t_SetName(IntPtr self, IntPtr pName)
        {
            Write($"SteamAPI_gameserveritem_t_SetName");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr g_pSteamClientGameServer()
        {
            Write($"g_pSteamClientGameServer");
            return InterfaceManager.FindOrCreateInterface("g_pSteamClientGameServer");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void Steam_RegisterInterfaceFuncs(IntPtr hModule)
        {
            Write($"Steam_RegisterInterfaceFuncs");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void Steam_RunCallbacks(IntPtr hSteamPipe, bool bGameServerCallbacks)
        {
            Write("Steam_RunCallbacks\n");

            SteamAPI_RunCallbacks();

            //if (bGameServerCallbacks)
            //    SteamEmulator.SteamGameServer.RunCallbacks(IntPtr.Zero);
        }

        #region Interfaces

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamAppList_v001()
        {
            Write($"SteamAPI_SteamAppList_v001");
            return InterfaceManager.FindOrCreateInterface("STEAMAPPLIST_INTERFACE_VERSION001");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamApps_v008()
        {
            Write($"SteamAPI_SteamApps_v008");
            return InterfaceManager.FindOrCreateInterface("STEAMAPPS_INTERFACE_VERSION008");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamGameServerApps_v008()
        {
            Write($"SteamAPI_SteamGameServerApps_v008");
            return InterfaceManager.FindOrCreateInterface("STEAMAPPS_INTERFACE_VERSION008");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamController_v007()
        {
            Write($"SteamAPI_SteamController_v007");
            return InterfaceManager.FindOrCreateInterface("SteamController007");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamController_v008()
        {
            Write($"SteamAPI_SteamController_v008");
            return InterfaceManager.FindOrCreateInterface("SteamController008");
        }


        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamFriends_v017()
        {
            Write($"SteamAPI_SteamFriends_v017");
            return InterfaceManager.FindOrCreateInterface("SteamFriends017");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamUtils_v010()
        {
            Write($"SteamAPI_SteamUtils_v010");
            return InterfaceManager.FindOrCreateInterface("SteamUtils010");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamGameServerUtils_v010()
        {
            Write($"SteamAPI_SteamGameServerUtils_v010");
            return InterfaceManager.FindOrCreateInterface("SteamUtils010");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamUtils_v009()
        {
            Write($"SteamAPI_SteamUtils_v009");
            return InterfaceManager.FindOrCreateInterface("SteamUtils009");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamGameServerUtils_v009()
        {
            Write($"SteamAPI_SteamGameServerUtils_v009");
            return InterfaceManager.FindOrCreateInterface("SteamUtils009");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamMatchmaking_v009()
        {
            Write($"SteamAPI_SteamMatchmaking_v009");
            return InterfaceManager.FindOrCreateInterface("SteamMatchMaking009");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamMatchmakingServers_v002()
        {
            Write($"SteamAPI_SteamMatchmakingServers_v002");
            return InterfaceManager.FindOrCreateInterface("SteamMatchMakingServers002");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamGameSearch_v001()
        {
            Write($"SteamAPI_SteamGameSearch_v001");
            return InterfaceManager.FindOrCreateInterface("SteamAPI_SteamGameSearch_v001");
        }


        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamParties_v002()
        {
            Write($"SteamAPI_SteamParties_v002");
            return InterfaceManager.FindOrCreateInterface("SteamParties002");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamNetworking_v006()
        {
            Write($"SteamAPI_SteamNetworking_v006");
            return InterfaceManager.FindOrCreateInterface("SteamNetworking006");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamGameServerNetworking_v006()
        {
            Write($"SteamAPI_SteamGameServerNetworking_v006");
            return InterfaceManager.FindOrCreateInterface("SteamNetworking006");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamMusicRemote_v001()
        {
            Write($"SteamAPI_SteamMusicRemote_v001");
            return InterfaceManager.FindOrCreateInterface("STEAMMUSICREMOTE_INTERFACE_VERSION001");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamInput_v001()
        {
            Write($"SteamAPI_SteamInput_v001");
            return InterfaceManager.FindOrCreateInterface("SteamInput001");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamInput_v002()
        {
            Write($"SteamAPI_SteamInput_v002");
            return InterfaceManager.FindOrCreateInterface("SteamInput002");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamHTMLSurface_v005()
        {
            Write($"SteamAPI_SteamHTMLSurface_v005");
            return InterfaceManager.FindOrCreateInterface("STEAMHTMLSURFACE_INTERFACE_VERSION_005");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamInventory_v003()
        {
            Write($"SteamAPI_SteamInventory_v003");
            return InterfaceManager.FindOrCreateInterface("STEAMINVENTORY_INTERFACE_V003");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamGameServerInventory_v003()
        {
            Write($"SteamAPI_SteamGameServerInventory_v003");
            return InterfaceManager.FindOrCreateInterface("STEAMINVENTORY_INTERFACE_V003");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamVideo_v002()
        {
            Write($"SteamAPI_SteamVideo_v002");
            return InterfaceManager.FindOrCreateInterface("STEAMVIDEO_INTERFACE_V002");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamParentalSettings_v001()
        {
            Write($"SteamAPI_SteamParentalSettings_v001");
            return InterfaceManager.FindOrCreateInterface("STEAMPARENTALSETTINGS_INTERFACE_VERSION001");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamRemotePlay_v001()
        {
            Write($"SteamAPI_SteamRemotePlay_v001");
            return InterfaceManager.FindOrCreateInterface("STEAMREMOTEPLAY_INTERFACE_VERSION001");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamNetworkingMessages_v002()
        {
            Write($"SteamAPI_SteamNetworkingMessages_v002");
            return InterfaceManager.FindOrCreateInterface("SteamNetworkingMessages002");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamNetworkingMessages_SteamAPI_v002()
        {
            Write($"SteamAPI_SteamNetworkingMessages_SteamAPI_v002");
            return InterfaceManager.FindOrCreateInterface("SteamNetworkingMessages002");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamGameServerNetworkingMessages_v002()
        {
            Write($"SteamAPI_SteamGameServerNetworkingMessages_v002");
            return InterfaceManager.FindOrCreateInterface("SteamNetworkingMessages002");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002()
        {
            Write($"SteamAPI_SteamGameServerNetworkingMessages_SteamAPI_v002");
            return InterfaceManager.FindOrCreateInterface("SteamNetworkingMessages002");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamNetworkingSockets_SteamAPI_v009()
        {
            Write($"SteamAPI_SteamNetworkingSockets_SteamAPI_v009");
            return InterfaceManager.FindOrCreateInterface("SteamNetworkingSockets009");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamGameServerNetworkingSockets_SteamAPI_v009()
        {
            Write($"SteamAPI_SteamGameServerNetworkingSockets_SteamAPI_v009");
            return InterfaceManager.FindOrCreateInterface("SteamNetworkingSockets009");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamNetworkingSockets_v009()
        {
            Write($"SteamAPI_SteamNetworkingSockets_v009");
            return InterfaceManager.FindOrCreateInterface("SteamNetworkingSockets009");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamGameServerNetworkingSockets_v009()
        {
            Write($"SteamAPI_SteamGameServerNetworkingSockets_v009");
            return InterfaceManager.FindOrCreateInterface("SteamNetworkingSockets009");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamNetworkingSockets_v008()
        {
            Write($"SteamAPI_SteamNetworkingSockets_v008");
            return InterfaceManager.FindOrCreateInterface("SteamNetworkingSockets008");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamGameServerNetworkingSockets_v008()
        {
            Write($"SteamAPI_SteamGameServerNetworkingSockets_v008");
            return InterfaceManager.FindOrCreateInterface("SteamNetworkingSockets008");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamNetworkingUtils_SteamAPI_v003()
        {
            Write($"SteamAPI_SteamNetworkingUtils_SteamAPI_v003");
                        return InterfaceManager.FindOrCreateInterface("SteamNetworkingUtils003");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamNetworkingUtils_v003()
        {
            Write($"SteamAPI_SteamNetworkingUtils_v003");
            return InterfaceManager.FindOrCreateInterface("SteamNetworkingUtils003");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamGameServerStats_v001()
        {
            Write($"SteamAPI_SteamGameServerStats_v001");
            return InterfaceManager.FindOrCreateInterface("STEAMUSERSTATS_INTERFACE_VERSION001");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAppList()
        {
            Write($"SteamAppList");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamAppList.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamApps()
        {
            Write($"ISteamApps");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamApps.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamClient()
        {
            Write($"SteamClient");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamClient.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamController()
        {
            Write($"SteamController");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamController.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamFriends()
        {
            Write($"SteamFriends");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamFriends.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamGameServer()
        {
            Write($"SteamGameServer");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamGameServer.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamGameServerApps()
        {
            Write($"SteamGameServerApps");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamApps.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamGameServerHTTP()
        {
            Write($"SteamGameServerHTTP");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamHTTP.InterfaceVersion);
        }


        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamGameServerInventory()
        {
            Write($"SteamGameServerInventory");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamInventory.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamGameServerNetworking()
        {
            Write($"SteamGameServerNetworking");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamNetworking.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamGameServerStats()
        {
            Write($"SteamGameServerStats");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamGameServerStats.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamGameServerUGC()
        {
            Write($"SteamGameServerUGC");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamUGC.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamGameServerUtils()
        {
            Write($"SteamGameServerUtils");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamUtils.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamHTTP()
        {
            Write($"SteamHTTP");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamHTTP.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamHTMLSurface()
        {
            Write($"SteamHTMLSurface");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamHTMLSurface.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamInventory()
        {
            Write($"SteamInventory");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamInventory.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamMasterServerUpdater()
        {
            Write($"SteamMasterServerUpdater");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamMasterServerUpdater.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamMatchmaking()
        {
            Write($"SteamMatchmaking");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamMatchmaking.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamMatchmakingServers()
        {
            Write($"SteamMatchmakingServers");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamMatchMakingServers.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamMusic()
        {
            Write($"SteamMusic");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamMusic.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamMusicRemote()
        {
            Write($"SteamMusicRemote");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamMusicRemote.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamNetworking()
        {
            Write($"SteamNetworking");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamNetworking.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamParentalSettings()
        {
            Write($"SteamParentalSettings");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamParentalSettings.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamRemoteStorage()
        {
            Write($"SteamRemoteStorage");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamRemoteStorage.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamScreenshots()
        {
            Write($"SteamScreenshots");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamScreenshots.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamUGC()
        {
            Write($"SteamUGC");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamUGC.InterfaceVersion);
        }

        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        //public static ISteamUnifiedMessages SteamUnifiedMessages()
        //{
        //    Write($"SteamUnifiedMessages");
        //    return SteamEmulator.SteamUnifiedMessages;
        //}

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamUser()
        {
            Write($"SteamUser");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamUser.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamUserStats()
        {
            Write($"SteamUserStats");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamUserStats.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamUtils()
        {
            Write($"SteamUtils");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamUtils.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamVideo()
        {
            Write($"SteamVideo");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamVideo.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void VR_GetGenericInterface(string pchInterfaceVersion, int peError)
        {
            Write($"VR_GetGenericInterface version {pchInterfaceVersion}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string VR_GetStringForHmdError(int error)
        {
            Write($"VR_GetStringForHmdError");
            return "";
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void VR_Init(int error, int type)
        {
            Write($"VR_Init");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool VR_IsHmdPresent()
        {
            Write($"VR_IsHmdPresent");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void VR_Shutdown()
        {
            Write($"VR_Shutdown");
        }


        #endregion

        private static void Write(string v)
        {
            SteamEmulator.Write("SteamAPI", v);
        }
    }
}

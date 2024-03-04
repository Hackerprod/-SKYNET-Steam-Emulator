using System.Runtime.InteropServices;
using System;
using SKYNET.Managers;
using SKYNET.Callback;
using SKYNET.Steamworks.Implementation;

#region using types

using AppId_t = System.UInt32;
using SteamAPICall_t = System.UInt64;
using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;
using FriendsGroupID_t = System.UInt16;
using SteamLeaderboard_t = System.UInt64;
using DepotId_t = System.UInt32;
using PublishedFileId_t = System.UInt64;
using UGCQueryHandle_t = System.UInt64;
using UGCUpdateHandle_t = System.UInt64;
using HAuthTicket = System.UInt32;
using HHTMLBrowser = System.UInt32;
using HTTPRequestHandle = System.UInt32;
using HTTPCookieContainerHandle = System.UInt32;
using SteamInventoryResult_t = System.UInt32;
using SteamItemInstanceID_t = System.UInt64;
using UGCHandle_t = System.UInt64;
using SteamItemDef_t = System.UInt32;
using HServerQuery = System.UInt16;
using SNetSocket_t = System.UInt32;
using SNetListenSocket_t = System.UInt32;
using UGCFileWriteStreamHandle_t = System.UInt64;
using PublishedFileUpdateHandle_t = System.UInt64;
using ScreenshotHandle = System.UInt32;
using AccountID_t = System.UInt32;
using SteamLeaderboardEntries_t = System.UInt64;
using ClientUnifiedMessageHandle = System.UInt64;
using HServerListRequest = System.IntPtr;
using CGameID = System.UInt32;

#endregion

namespace SKYNET.Steamworks.Exported
{
    public static class CSteamworks
    {
        static CSteamworks()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool Init()
        {
            Write("Init");
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void Shutdown()
        {
            Write("Shutdown");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool IsSteamRunning()
        {
            Write("IsSteamRunning");
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool RestartAppIfNecessary(AppId_t unOwnAppID)
        {
            Write("RestartAppIfNecessary");
            SteamEmulator.AppID = unOwnAppID;
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void WriteMiniDump(uint uStructuredExceptionCode, IntPtr pvExceptionInfo, uint uBuildID)
        {
            Write($"WriteMiniDump");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SetMiniDumpComment(string pchMsg)
        {
            Write($"SetMiniDumpComment");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamClient_()
        {
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamClient.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool InitSafe()
        {
            Write($"InitSafe");
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void RunCallbacks()
        {
            CallbackManager.RunCallbacks();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void RegisterCallback(IntPtr pCallback, int iCallback)
        {
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
        public static void UnregisterCallback(IntPtr pCallback)
        {
            SteamCallback sCallback = new SteamCallback(pCallback);
            string success = CallbackManager.UnregisterCallback(sCallback) ? "OK" : "Error";
            Write($"UnregisterCallback {sCallback.CallbackType} {success}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void RegisterCallResult(IntPtr pCallback, ulong hAPICall)
        {
            try
            {
                SteamCallback sCallback = new SteamCallback(pCallback, true);
                sCallback.SteamAPICall = hAPICall;

                var isGameServer = sCallback.HasGameserver ? "[ GAMESERVER ]" : "[   CLIENT   ]";
                var space = (int)sCallback.CallbackType < 1000 ? " " : "";

                Write($"RegisterCallResult: {isGameServer}  {hAPICall}  {(int)sCallback.CallbackType} {space} {sCallback.BaseType} {sCallback.CallbackType}");

                CallbackManager.RegisterCallResult(sCallback);
            }
            catch (Exception ex)
            {
                Write($"RegisterCallResult: {ex}");
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void UnregisterCallResult(IntPtr pCallback, ulong hAPICall)
        {
            SteamCallback sCallback = new SteamCallback(pCallback);
            string success = CallbackManager.UnregisterCallResult(sCallback, hAPICall) ? "OK" : "Error";
            Write($"UnregisterCallResult {sCallback.CallbackType} {success}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void Steam_RunCallbacks_(HSteamPipe hSteamPipe, bool bGameServerCallbacks)
        {
            Write($"Steam_RunCallbacks");
            CallbackManager.RunCallbacks();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void Steam_RegisterInterfaceFuncs_(IntPtr hModule)
        {
            Write($"Steam_RegisterInterfaceFuncs");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HSteamUser Steam_GetHSteamUserCurrent_()
        {
            Write($"Steam_GetHSteamUserCurrent");
            return SteamEmulator.HSteamUser;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetSteamInstallPath()
        {
            Write($"GetSteamInstallPath");
            return 0;       // OJO
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HSteamPipe GetHSteamPipe_()
        {
            Write($"GetHSteamPipe");
            return SteamEmulator.HSteamPipe;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SetTryCatchCallbacks(bool bTryCatchCallbacks)
        {
            Write($"SetTryCatchCallbacks");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HSteamUser GetHSteamUser_()
        {
            Write($"GetHSteamUser");
            return SteamEmulator.HSteamUser;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void UseBreakpadCrashHandler(string pchVersion, string pchDate, string pchTime, bool bFullMemoryDumps, IntPtr pvContext, IntPtr m_pfnPreMinidumpCallback)
        {
            Write($"UseBreakpadCrashHandler");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamGameServerClient_()
        {
            Write("SteamGameServerClient_");
            return InterfaceManager.FindOrCreateInterface(SteamEmulator.SteamGameServer.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool GameServer_Init(uint unIP, ushort usSteamPort, ushort usGamePort, ushort usQueryPort, int eServerMode, string pchVersionString)
        {
            Write("GameServer_Init");
            uint nGameAppId = SteamEmulator.AppID;
            return SteamEmulator.SteamGameServer.InitGameServer(unIP, usGamePort, usQueryPort, (uint)eServerMode, nGameAppId, pchVersionString);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool GameServer_InitSafe(uint unIP, ushort usSteamPort, ushort usGamePort, ushort usQueryPort, EServerMode eServerMode, string pchVersionString)
        {
            return SteamEmulator.SteamGameServer.InitGameServer(unIP, usSteamPort, usGamePort, usQueryPort, (uint)eServerMode, pchVersionString);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void GameServer_Shutdown()
        {
            //SteamEmulator.SteamGameServer.Shutdown();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void GameServer_RunCallbacks()
        {
            //SteamEmulator.SteamGameServer.call();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool GameServer_BSecure()
        {
            return SteamEmulator.SteamGameServer.BSecure();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong GameServer_GetSteamID()
        {
            return (ulong)SteamEmulator.SteamGameServer.GetSteamID();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HSteamPipe GameServer_GetHSteamPipe()
        {
            return SteamEmulator.HSteamPipe_GS;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HSteamUser GameServer_GetHSteamUser()
        {
            return SteamEmulator.HSteamUser_GS;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamClientGameServer()
        {
            Write($"SteamClientGameServer");
            return SteamEmulator.SteamClient.GetISteamGameSearch(1, 1, SteamEmulator.SteamGameServer.InterfaceVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamAppList_GetNumInstalledApps()
        {
            return SteamEmulator.SteamAppList.GetNumInstalledApps();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamAppList_GetInstalledApps(ref AppId_t[] pvecAppID, uint unMaxAppIDs)
        {
            return SteamEmulator.SteamAppList.GetInstalledApps(pvecAppID[0], unMaxAppIDs);                                                  // OJO
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamAppList_GetAppName(AppId_t nAppID, string pchName, int cchNameMax)
        {
            return SteamEmulator.SteamAppList.GetAppName(nAppID, pchName, cchNameMax);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamAppList_GetAppInstallDir(AppId_t nAppID, string pchDirectory, int cchNameMax)
        {
            return SteamEmulator.SteamAppList.GetAppInstallDir(nAppID, pchDirectory, cchNameMax);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamAppList_GetAppBuildId(AppId_t nAppID)
        {
            return SteamEmulator.SteamAppList.GetAppBuildId(nAppID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamApps_BIsSubscribed()
        {
            return SteamEmulator.SteamApps.BIsSubscribed();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamApps_BIsLowViolence()
        {
            return SteamEmulator.SteamApps.BIsLowViolence();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamApps_BIsCybercafe()
        {
            return SteamEmulator.SteamApps.BIsCybercafe();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamApps_BIsVACBanned()
        {
            return SteamEmulator.SteamApps.BIsVACBanned();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string ISteamApps_GetCurrentGameLanguage()
        {
            return SteamEmulator.SteamApps.GetCurrentGameLanguage();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string ISteamApps_GetAvailableGameLanguages()
        {
            return SteamEmulator.SteamApps.GetAvailableGameLanguages();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamApps_BIsSubscribedApp(AppId_t appID)
        {
            return SteamEmulator.SteamApps.BIsSubscribedApp(appID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamApps_BIsDlcInstalled(AppId_t appID)
        {
            return SteamEmulator.SteamApps.BIsDlcInstalled(appID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamApps_GetEarliestPurchaseUnixTime(AppId_t nAppID)
        {
            return SteamEmulator.SteamApps.GetEarliestPurchaseUnixTime(nAppID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamApps_BIsSubscribedFromFreeWeekend()
        {
            return SteamEmulator.SteamApps.BIsSubscribedFromFreeWeekend();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamApps_GetDLCCount()
        {
            return SteamEmulator.SteamApps.GetDLCCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamApps_BGetDLCDataByIndex(int iDLC, AppId_t pAppID, bool pbAvailable, string pchName, int cchNameBufferSize)
        {
            return SteamEmulator.SteamApps.BGetDLCDataByIndex(iDLC, pAppID, pbAvailable, pchName, cchNameBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamApps_InstallDLC(AppId_t nAppID)
        {
            SteamEmulator.SteamApps.InstallDLC(nAppID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamApps_UninstallDLC(AppId_t nAppID)
        {
            SteamEmulator.SteamApps.UninstallDLC(nAppID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamApps_RequestAppProofOfPurchaseKey(AppId_t nAppID)
        {
            SteamEmulator.SteamApps.RequestAppProofOfPurchaseKey(nAppID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamApps_GetCurrentBetaName(IntPtr pchName, int cchNameBufferSize)
        {
            return SteamEmulator.SteamApps.GetCurrentBetaName(pchName, cchNameBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamApps_MarkContentCorrupt(bool bMissingFilesOnly)
        {
            return SteamEmulator.SteamApps.MarkContentCorrupt(bMissingFilesOnly);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamApps_GetInstalledDepots(AppId_t appID, ref DepotId_t[] pvecDepots, uint cMaxDepots)
        {
            return SteamEmulator.SteamApps.GetInstalledDepots(appID, ref pvecDepots, cMaxDepots);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamApps_GetAppInstallDir(AppId_t appID, string pchFolder, uint cchFolderBufferSize)
        {
            return SteamEmulator.SteamApps.GetAppInstallDir(appID, pchFolder, cchFolderBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamApps_BIsAppInstalled(AppId_t appID)
        {
            return SteamEmulator.SteamApps.BIsAppInstalled(appID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamApps_GetAppOwner()
        {
            return (ulong)SteamEmulator.SteamApps.GetAppOwner();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamApps_GetLaunchQueryParam(string pchKey)
        {
            return SteamEmulator.SteamApps.GetLaunchQueryParam(pchKey);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamApps_GetDlcDownloadProgress(AppId_t nAppID, ulong punBytesDownloaded, ulong punBytesTotal)
        {
            return SteamEmulator.SteamApps.GetDlcDownloadProgress(nAppID, punBytesDownloaded, punBytesTotal);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamApps_GetAppBuildId()
        {
            return SteamEmulator.SteamApps.GetAppBuildId();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HSteamPipe ISteamClient_CreateSteamPipe()
        {
            return SteamEmulator.SteamClient.CreateSteamPipe();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamClient_BReleaseSteamPipe(HSteamPipe hSteamPipe)
        {
            return SteamEmulator.SteamClient.BReleaseSteamPipe(hSteamPipe);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HSteamUser ISteamClient_ConnectToGlobalUser(HSteamPipe hSteamPipe)
        {
            return SteamEmulator.SteamClient.ConnectToGlobalUser(hSteamPipe);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HSteamUser ISteamClient_CreateLocalUser(ref HSteamPipe phSteamPipe, EAccountType eAccountType)
        {
            return SteamEmulator.SteamClient.CreateLocalUser(phSteamPipe, (int)eAccountType);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamClient_ReleaseUser(HSteamPipe hSteamPipe, HSteamUser hUser)
        {
            SteamEmulator.SteamClient.ReleaseUser(hSteamPipe, hUser);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamClient_GetISteamUser(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamUser(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamClient_GetISteamGameServer(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamGameServer(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamClient_SetLocalIPBinding(uint unIP, ushort usPort)
        {
            SteamEmulator.SteamClient.SetLocalIPBinding(unIP, usPort);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamClient_GetISteamFriends(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamFriends(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamClient_GetISteamUtils(HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamUtils(hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamClient_GetISteamMatchmaking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamMatchmaking(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamClient_GetISteamMatchmakingServers(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamMatchmakingServers(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamClient_GetISteamGenericInterface(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamGenericInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamClient_GetISteamUserStats(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamUserStats(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamClient_GetISteamGameServerStats(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamGameServerStats(hSteamuser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamClient_GetISteamApps(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamApps(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamClient_GetISteamNetworking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamNetworking(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamClient_GetISteamRemoteStorage(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamRemoteStorage(hSteamuser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamClient_GetISteamScreenshots(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamScreenshots(hSteamuser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamClient_RunFrame()
        {
            //SteamEmulator.SteamClient.RunFrame();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamClient_GetIPCCallCount()
        {
            return SteamEmulator.SteamClient.GetIPCCallCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamClient_SetWarningMessageHook(IntPtr pFunction)
        {
            SteamEmulator.SteamClient.SetWarningMessageHook(pFunction);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamClient_BShutdownIfAllPipesClosed()
        {
            return SteamEmulator.SteamClient.BShutdownIfAllPipesClosed();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamClient_GetISteamHTTP(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamHTTP(hSteamuser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamClient_GetISteamUnifiedMessages(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamUnifiedMessages(hSteamuser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamClient_GetISteamController(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamController(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamClient_GetISteamUGC(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamUGC(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamClient_GetISteamAppList(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamAppList(hSteamUser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamClient_GetISteamMusic(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamMusic(hSteamuser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamClient_GetISteamMusicRemote(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamMusicRemote(hSteamuser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamClient_GetISteamHTMLSurface(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamHTMLSurface(hSteamuser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamClient_Set_SteamAPI_CPostAPIResultInProcess(IntPtr func)
        {
            SteamEmulator.SteamClient.DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess(func);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamClient_Remove_SteamAPI_CPostAPIResultInProcess(IntPtr func)
        {
            SteamEmulator.SteamClient.DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess(func);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamClient_Set_SteamAPI_CCheckCallbackRegisteredInProcess(IntPtr func)
        {
            SteamEmulator.SteamClient.Set_SteamAPI_CCheckCallbackRegisteredInProcess(func);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamClient_GetISteamInventory(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamInventory(hSteamuser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamClient_GetISteamVideo(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            return SteamEmulator.SteamClient.GetISteamVideo(hSteamuser, hSteamPipe, pchVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamController_Init(string pchAbsolutePathToControllerConfigVDF)
        {
            return SteamEmulator.SteamController.Init();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamController_Shutdown()
        {
            return SteamEmulator.SteamController.Shutdown();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamController_RunFrame()
        {
            SteamEmulator.SteamController.RunFrame();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamController_GetControllerState(uint unControllerIndex, ref IntPtr pState)
        {
            //return SteamEmulator.SteamController.GetControllerState(unControllerIndex, pState);
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamController_TriggerHapticPulse(uint unControllerIndex, ESteamControllerPad eTargetPad, ushort usDurationMicroSec)
        {
            SteamEmulator.SteamController.TriggerHapticPulse(unControllerIndex, (int)eTargetPad, (short)usDurationMicroSec);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamController_SetOverrideMode(string pchMode)
        {
            //SteamEmulator.SteamController.SetOverrideMode(pchMode);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string ISteamFriends_GetPersonaName()
        {
            return SteamFriends.Instance.GetPersonaName();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamFriends_SetPersonaName(string pchPersonaName)
        {
            return SteamFriends.Instance.SetPersonaName(pchPersonaName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EPersonaState ISteamFriends_GetPersonaState()
        {
            return (EPersonaState)SteamFriends.Instance.GetPersonaState();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamFriends_GetFriendCount(EFriendFlags iFriendFlags)
        {
            return SteamFriends.Instance.GetFriendCount((int)iFriendFlags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamFriends_GetFriendByIndex(int iFriend, EFriendFlags iFriendFlags)
        {
            return (ulong)SteamFriends.Instance.GetFriendByIndex(iFriend, (int)iFriendFlags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EFriendRelationship ISteamFriends_GetFriendRelationship(ulong steamIDFriend)
        {
            return (EFriendRelationship)SteamFriends.Instance.GetFriendRelationship((ulong)steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EPersonaState ISteamFriends_GetFriendPersonaState(ulong steamIDFriend)
        {
            return (EPersonaState)SteamFriends.Instance.GetFriendPersonaState((ulong)steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string ISteamFriends_GetFriendPersonaName(ulong steamIDFriend)
        {
            return SteamFriends.Instance.GetFriendPersonaName(steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamFriends_GetFriendGamePlayed(ulong steamIDFriend, ref FriendGameInfo_t pFriendGameInfo)
        {
            return SteamFriends.Instance.GetFriendGamePlayed(steamIDFriend, ref pFriendGameInfo);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string ISteamFriends_GetFriendPersonaNameHistory(ulong steamIDFriend, int iPersonaName)
        {
            return SteamFriends.Instance.GetFriendPersonaNameHistory((ulong)steamIDFriend, iPersonaName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamFriends_GetFriendSteamLevel(ulong steamIDFriend)
        {
            return SteamFriends.Instance.GetFriendSteamLevel((ulong)steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string ISteamFriends_GetPlayerNickname(ulong steamIDPlayer)
        {
            return SteamFriends.Instance.GetPlayerNickname((ulong)steamIDPlayer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamFriends_GetFriendsGroupCount()
        {
            return SteamFriends.Instance.GetFriendsGroupCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamFriends_GetFriendsGroupIDByIndex(int iFG)
        {
            return SteamFriends.Instance.GetFriendsGroupIDByIndex(iFG);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string ISteamFriends_GetFriendsGroupName(FriendsGroupID_t friendsGroupID)
        {
            return SteamFriends.Instance.GetFriendsGroupName(friendsGroupID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamFriends_GetFriendsGroupMembersCount(FriendsGroupID_t friendsGroupID)
        {
            return SteamFriends.Instance.GetFriendsGroupMembersCount(friendsGroupID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamFriends_GetFriendsGroupMembersList(FriendsGroupID_t friendsGroupID, ref ulong[] pOutSteamIDMembers, int nMembersCount)
        {
            SteamFriends.Instance.GetFriendsGroupMembersList(friendsGroupID, ref pOutSteamIDMembers, nMembersCount);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamFriends_HasFriend(ulong steamIDFriend, EFriendFlags iFriendFlags)
        {
            return SteamFriends.Instance.HasFriend((ulong)steamIDFriend, (int)iFriendFlags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamFriends_GetClanCount()
        {
            return SteamFriends.Instance.GetClanCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamFriends_GetClanByIndex(int iClan)
        {
            return (ulong)SteamFriends.Instance.GetClanByIndex(iClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string ISteamFriends_GetClanName(ulong steamIDClan)
        {
            return SteamFriends.Instance.GetClanName((ulong)steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string ISteamFriends_GetClanTag(ulong steamIDClan)
        {
            return SteamFriends.Instance.GetClanTag((ulong)steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamFriends_GetClanActivityCounts(ulong steamIDClan, ref int pnOnline, ref int pnInGame, ref int pnChatting)
        {
            return SteamFriends.Instance.GetClanActivityCounts((ulong)steamIDClan, ref pnOnline, ref pnInGame, ref pnChatting);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamFriends_DownloadClanActivityCounts(IntPtr psteamIDClans, int cClansToRequest)
        //public static ulong ISteamFriends_DownloadClanActivityCounts(ref ulong[] psteamIDClans, int cClansToRequest)
        {
            return SteamFriends.Instance.DownloadClanActivityCounts(psteamIDClans, cClansToRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamFriends_GetFriendCountFromSource(ulong steamIDSource)
        {
            return SteamFriends.Instance.GetFriendCountFromSource((ulong)steamIDSource);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamFriends_GetFriendFromSourceByIndex(ulong steamIDSource, int iFriend)
        {
            return (ulong)SteamFriends.Instance.GetFriendFromSourceByIndex((ulong)steamIDSource, iFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamFriends_IsUserInSource(ulong steamIDUser, ulong steamIDSource)
        {
            return SteamFriends.Instance.IsUserInSource((ulong)steamIDUser, (ulong)steamIDSource);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamFriends_SetInGameVoiceSpeaking(ulong steamIDUser, bool bSpeaking)
        {
            SteamFriends.Instance.SetInGameVoiceSpeaking((ulong)steamIDUser, bSpeaking);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamFriends_ActivateGameOverlay(string pchDialog)
        {
            SteamFriends.Instance.ActivateGameOverlay(pchDialog);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamFriends_ActivateGameOverlayToUser(string pchDialog, ulong steamID)
        {
            SteamFriends.Instance.ActivateGameOverlayToUser(pchDialog, (ulong)steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamFriends_ActivateGameOverlayToWebPage(string pchURL)
        {
            SteamFriends.Instance.ActivateGameOverlayToWebPage(pchURL, 0);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamFriends_ActivateGameOverlayToStore(AppId_t nAppID, EOverlayToStoreFlag eFlag)
        {
            SteamFriends.Instance.ActivateGameOverlayToStore(nAppID, (int)eFlag);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamFriends_SetPlayedWith(ulong steamIDUserPlayedWith)
        {
            SteamFriends.Instance.SetPlayedWith((ulong)steamIDUserPlayedWith);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamFriends_ActivateGameOverlayInviteDialog(ulong steamIDLobby)
        {
            SteamFriends.Instance.ActivateGameOverlayInviteDialog((ulong)steamIDLobby);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamFriends_GetSmallFriendAvatar(ulong steamIDFriend)
        {
            return SteamFriends.Instance.GetSmallFriendAvatar(steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamFriends_GetMediumFriendAvatar(ulong steamIDFriend)
        {
            return SteamFriends.Instance.GetMediumFriendAvatar((ulong)steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamFriends_GetLargeFriendAvatar(ulong steamIDFriend)
        {
            return SteamFriends.Instance.GetLargeFriendAvatar((ulong)steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamFriends_RequestUserInformation(ulong steamIDUser, bool bRequireNameOnly)
        {
            return SteamFriends.Instance.RequestUserInformation((ulong)steamIDUser, bRequireNameOnly);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamFriends_RequestClanOfficerList(ulong steamIDClan)
        {
            return SteamFriends.Instance.RequestClanOfficerList((ulong)steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamFriends_GetClanOwner(ulong steamIDClan)
        {
            return (ulong)SteamFriends.Instance.GetClanOwner((ulong)steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamFriends_GetClanOfficerCount(ulong steamIDClan)
        {
            return SteamFriends.Instance.GetClanOfficerCount((ulong)steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamFriends_GetClanOfficerByIndex(ulong steamIDClan, int iOfficer)
        {
            return (ulong)SteamFriends.Instance.GetClanOfficerByIndex((ulong)steamIDClan, iOfficer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamFriends_GetUserRestrictions()
        {
            return SteamFriends.Instance.GetUserRestrictions();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamFriends_SetRichPresence(string pchKey, string pchValue)
        {
            return SteamFriends.Instance.SetRichPresence(pchKey, pchValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamFriends_ClearRichPresence()
        {
            SteamFriends.Instance.ClearRichPresence();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string ISteamFriends_GetFriendRichPresence(ulong steamIDFriend, string pchKey)
        {
            return SteamFriends.Instance.GetFriendRichPresence((ulong)steamIDFriend, pchKey);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamFriends_GetFriendRichPresenceKeyCount(ulong steamIDFriend)
        {
            return SteamFriends.Instance.GetFriendRichPresenceKeyCount((ulong)steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string ISteamFriends_GetFriendRichPresenceKeyByIndex(ulong steamIDFriend, int iKey)
        {
            return SteamFriends.Instance.GetFriendRichPresenceKeyByIndex((ulong)steamIDFriend, iKey);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamFriends_RequestFriendRichPresence(ulong steamIDFriend)
        {
            SteamFriends.Instance.RequestFriendRichPresence((ulong)steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamFriends_InviteUserToGame(ulong steamIDFriend, string pchConnectString)
        {
            return SteamFriends.Instance.InviteUserToGame((ulong)steamIDFriend, pchConnectString);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamFriends_GetCoplayFriendCount()
        {
            return SteamFriends.Instance.GetCoplayFriendCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamFriends_GetCoplayFriend(int iCoplayFriend)
        {
            return (ulong)SteamFriends.Instance.GetCoplayFriend(iCoplayFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamFriends_GetFriendCoplayTime(ulong steamIDFriend)
        {
            return SteamFriends.Instance.GetFriendCoplayTime((ulong)steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamFriends_GetFriendCoplayGame(ulong steamIDFriend)
        {
            return SteamFriends.Instance.GetFriendCoplayGame((ulong)steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamFriends_JoinClanChatRoom(ulong steamIDClan)
        {
            return SteamFriends.Instance.JoinClanChatRoom((ulong)steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamFriends_LeaveClanChatRoom(ulong steamIDClan)
        {
            return SteamFriends.Instance.LeaveClanChatRoom((ulong)steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamFriends_GetClanChatMemberCount(ulong steamIDClan)
        {
            return SteamFriends.Instance.GetClanChatMemberCount((ulong)steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamFriends_GetChatMemberByIndex(ulong steamIDClan, int iUser)
        {
            return (ulong)SteamFriends.Instance.GetChatMemberByIndex((ulong)steamIDClan, iUser);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamFriends_SendClanChatMessage(ulong steamIDClanChat, string pchText)
        {
            return SteamFriends.Instance.SendClanChatMessage((ulong)steamIDClanChat, pchText);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamFriends_GetClanChatMessage(ulong steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, int peChatEntryType, ref ulong[] psteamidChatter)
        {
            return SteamFriends.Instance.GetClanChatMessage((ulong)steamIDClanChat, iMessage, prgchText, cchTextMax, peChatEntryType, ref psteamidChatter);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamFriends_IsClanChatAdmin(ulong steamIDClanChat, ulong steamIDUser)
        {
            return SteamFriends.Instance.IsClanChatAdmin((ulong)steamIDClanChat, (ulong)steamIDUser);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamFriends_IsClanChatWindowOpenInSteam(ulong steamIDClanChat)
        {
            return SteamFriends.Instance.IsClanChatWindowOpenInSteam(steamIDClanChat);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamFriends_OpenClanChatWindowInSteam(ulong steamIDClanChat)
        {
            return SteamFriends.Instance.OpenClanChatWindowInSteam((ulong)steamIDClanChat);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamFriends_CloseClanChatWindowInSteam(ulong steamIDClanChat)
        {
            return SteamFriends.Instance.CloseClanChatWindowInSteam((ulong)steamIDClanChat);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamFriends_SetListenForFriendsMessages(bool bInterceptEnabled)
        {
            return SteamFriends.Instance.SetListenForFriendsMessages(bInterceptEnabled);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamFriends_ReplyToFriendMessage(ulong steamIDFriend, string pchMsgToSend)
        {
            return SteamFriends.Instance.ReplyToFriendMessage((ulong)steamIDFriend, pchMsgToSend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamFriends_GetFriendMessage(ulong steamIDFriend, int iMessageID, IntPtr pvData, int cubData, ref int peChatEntryType)
        {
            return SteamFriends.Instance.GetFriendMessage((ulong)steamIDFriend, iMessageID, pvData, cubData, peChatEntryType);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamFriends_GetFollowerCount(ulong steamID)
        {
            return SteamFriends.Instance.GetFollowerCount((ulong)steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamFriends_IsFollowing(ulong steamID)
        {
            return SteamFriends.Instance.IsFollowing((ulong)steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamFriends_EnumerateFollowingList(uint unStartIndex)
        {
            return SteamFriends.Instance.EnumerateFollowingList(unStartIndex);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServer_InitGameServer(uint unIP, ushort usGamePort, ushort usQueryPort, uint unFlags, AppId_t nGameAppId, string pchVersionString)
        {
            return SteamEmulator.SteamGameServer.InitGameServer(unIP, usGamePort, usQueryPort, unFlags, nGameAppId, pchVersionString);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_SetProduct(string pszProduct)
        {
            SteamEmulator.SteamGameServer.SetProduct(pszProduct);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_SetGameDescription(string pszGameDescription)
        {
            SteamEmulator.SteamGameServer.SetGameDescription(pszGameDescription);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_SetModDir(string pszModDir)
        {
            SteamEmulator.SteamGameServer.SetModDir(pszModDir);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_SetDedicatedServer(bool bDedicated)
        {
            SteamEmulator.SteamGameServer.SetDedicatedServer(bDedicated);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_LogOn(string pszToken)
        {
            SteamEmulator.SteamGameServer.LogOn(pszToken);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_LogOnAnonymous()
        {
            SteamEmulator.SteamGameServer.LogOnAnonymous();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_LogOff()
        {
            SteamEmulator.SteamGameServer.LogOff();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServer_BLoggedOn()
        {
            return SteamEmulator.SteamGameServer.BLoggedOn();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServer_BSecure()
        {
            return SteamEmulator.SteamGameServer.BSecure();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamGameServer_GetSteamID()
        {
            return (ulong)SteamEmulator.SteamGameServer.GetSteamID();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServer_WasRestartRequested()
        {
            return SteamEmulator.SteamGameServer.WasRestartRequested();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_SetMaxPlayerCount(int cPlayersMax)
        {
            SteamEmulator.SteamGameServer.SetMaxPlayerCount(cPlayersMax);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_SetBotPlayerCount(int cBotplayers)
        {
            SteamEmulator.SteamGameServer.SetBotPlayerCount(cBotplayers);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_SetServerName(string pszServerName)
        {
            SteamEmulator.SteamGameServer.SetServerName(pszServerName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_SetMapName(string pszMapName)
        {
            SteamEmulator.SteamGameServer.SetMapName(pszMapName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_SetPasswordProtected(bool bPasswordProtected)
        {
            SteamEmulator.SteamGameServer.SetPasswordProtected(bPasswordProtected);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_SetSpectatorPort(ushort unSpectatorPort)
        {
            SteamEmulator.SteamGameServer.SetSpectatorPort(unSpectatorPort);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_SetSpectatorServerName(string pszSpectatorServerName)
        {
            SteamEmulator.SteamGameServer.SetSpectatorServerName(pszSpectatorServerName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_ClearAllKeyValues()
        {
            SteamEmulator.SteamGameServer.ClearAllKeyValues();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_SetKeyValue(string pKey, string pValue)
        {
            SteamEmulator.SteamGameServer.SetKeyValue(pKey, pValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_SetGameTags(string pchGameTags)
        {
            SteamEmulator.SteamGameServer.SetGameTags(pchGameTags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_SetGameData(string pchGameData)
        {
            SteamEmulator.SteamGameServer.SetGameData(pchGameData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_SetRegion(string pszRegion)
        {
            SteamEmulator.SteamGameServer.SetRegion(pszRegion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServer_SendUserConnectAndAuthenticate(uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, ref ulong pSteamIDUser)
        //public static bool ISteamGameServer_SendUserConnectAndAuthenticate(uint unIPClient, ref byte[] pvAuthBlob, uint cubAuthBlobSize, ref ulong pSteamIDUser)
        {
            return SteamEmulator.SteamGameServer.SendUserConnectAndAuthenticate(unIPClient, pvAuthBlob, cubAuthBlobSize, (ulong)pSteamIDUser);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamGameServer_CreateUnauthenticatedUserConnection()
        {
            return (ulong)SteamEmulator.SteamGameServer.CreateUnauthenticatedUserConnection();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_SendUserDisconnect(ulong steamIDUser)
        {
            SteamEmulator.SteamGameServer.SendUserDisconnect((ulong)steamIDUser);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServer_BUpdateUserData(ulong steamIDUser, string pchPlayerName, uint uScore)
        {
            return SteamEmulator.SteamGameServer.BUpdateUserData((ulong)steamIDUser, pchPlayerName, uScore);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamGameServer_GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
        //public static uint ISteamGameServer_GetAuthSessionTicket(ref byte[] pTicket, int cbMaxTicket, ref uint pcbTicket)
        {
            return SteamEmulator.SteamGameServer.GetAuthSessionTicket(pTicket, cbMaxTicket, ref pcbTicket);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EBeginAuthSessionResult ISteamGameServer_BeginAuthSession(IntPtr pAuthTicket, int cbAuthTicket, ulong steamID)
        //public static EBeginAuthSessionResult ISteamGameServer_BeginAuthSession(ref byte[] pAuthTicket, int cbAuthTicket, ulong steamID)
        {
            return (EBeginAuthSessionResult)SteamEmulator.SteamGameServer.BeginAuthSession(pAuthTicket, cbAuthTicket, (ulong)steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_EndAuthSession(ulong steamID)
        {
            SteamEmulator.SteamGameServer.EndAuthSession((ulong)steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_CancelAuthTicket(HAuthTicket hAuthTicket)
        {
            SteamEmulator.SteamGameServer.CancelAuthTicket(hAuthTicket);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamGameServer_UserHasLicenseForApp(ulong steamID, AppId_t appID)
        {
            return SteamEmulator.SteamGameServer.UserHasLicenseForApp((ulong)steamID, appID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServer_RequestUserGroupStatus(ulong steamIDUser, ulong steamIDGroup)
        {
            return SteamEmulator.SteamGameServer.RequestUserGroupStatus((ulong)steamIDUser, (ulong)steamIDGroup);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_GetGameplayStats()
        {
            SteamEmulator.SteamGameServer.GetGameplayStats();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamGameServer_GetServerReputation()
        {
            return SteamEmulator.SteamGameServer.GetServerReputation();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamGameServer_GetPublicIP()
        {
            return SteamEmulator.SteamGameServer.GetPublicIP();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServer_HandleIncomingPacket(IntPtr pData, int cbData, uint srcIP, ushort srcPort)
        //public static bool ISteamGameServer_HandleIncomingPacket(ref byte[] pData, int cbData, uint srcIP, ushort srcPort)
        {
            return SteamEmulator.SteamGameServer.HandleIncomingPacket(pData, cbData, srcIP, srcPort);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamGameServer_GetNextOutgoingPacket(IntPtr pOut, int cbMaxOut, ref uint pNetAdr, ref ushort pPort)
        //public static int ISteamGameServer_GetNextOutgoingPacket(ref byte[] pOut, int cbMaxOut, ref uint pNetAdr, ref ushort pPort)
        {
            return SteamEmulator.SteamGameServer.GetNextOutgoingPacket(pOut, cbMaxOut, pNetAdr, pPort);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_EnableHeartbeats(bool bActive)
        {
            SteamEmulator.SteamGameServer.EnableHeartbeats(bActive);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_SetHeartbeatInterval(int iHeartbeatInterval)
        {
            SteamEmulator.SteamGameServer.SetHeartbeatInterval(iHeartbeatInterval);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServer_ForceHeartbeat()
        {
            SteamEmulator.SteamGameServer.ForceHeartbeat();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamGameServer_AssociateWithClan(ulong steamIDClan)
        {
            return SteamEmulator.SteamGameServer.AssociateWithClan((ulong)steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamGameServer_ComputeNewPlayerCompatibility(ulong steamIDNewPlayer)
        {
            return SteamEmulator.SteamGameServer.ComputeNewPlayerCompatibility((ulong)steamIDNewPlayer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamGameServerStats_RequestUserStats(ulong steamIDUser)
        {
            return SteamEmulator.SteamGameServerStats.RequestUserStats((ulong)steamIDUser);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerStats_GetUserStat(ulong steamIDUser, string pchName, ref int pData)
        {
            return SteamEmulator.SteamGameServerStats.GetUserStat((ulong)steamIDUser, pchName, pData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerStats_GetUserStat_(ulong steamIDUser, string pchName, ref float pData)
        {
            return SteamEmulator.SteamGameServerStats.GetUserStat((ulong)steamIDUser, pchName, (int)pData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerStats_GetUserAchievement(ulong steamIDUser, string pchName, ref bool pbAchieved)
        {
            return SteamEmulator.SteamGameServerStats.GetUserAchievement((ulong)steamIDUser, pchName, pbAchieved);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerStats_SetUserStat(ulong steamIDUser, string pchName, int nData)
        {
            return SteamEmulator.SteamGameServerStats.SetUserStat((ulong)steamIDUser, pchName, nData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerStats_SetUserStat_(ulong steamIDUser, string pchName, float fData)
        {
            return SteamEmulator.SteamGameServerStats.SetUserStat((ulong)steamIDUser, pchName, (int)fData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerStats_UpdateUserAvgRateStat(ulong steamIDUser, string pchName, float flCountThisSession, double dSessionLength)
        {
            return SteamEmulator.SteamGameServerStats.UpdateUserAvgRateStat((ulong)steamIDUser, pchName, flCountThisSession, dSessionLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerStats_SetUserAchievement(ulong steamIDUser, string pchName)
        {
            return SteamEmulator.SteamGameServerStats.SetUserAchievement((ulong)steamIDUser, pchName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerStats_ClearUserAchievement(ulong steamIDUser, string pchName)
        {
            return SteamEmulator.SteamGameServerStats.ClearUserAchievement((ulong)steamIDUser, pchName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamGameServerStats_StoreUserStats(ulong steamIDUser)
        {
            return SteamEmulator.SteamGameServerStats.StoreUserStats((ulong)steamIDUser);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTMLSurface_Init()
        {
            return SteamEmulator.SteamHTMLSurface.Init();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTMLSurface_Shutdown()
        {
            return SteamEmulator.SteamHTMLSurface.Shutdown();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamHTMLSurface_CreateBrowser(string pchUserAgent, string pchUserCSS)
        {
            return SteamEmulator.SteamHTMLSurface.CreateBrowser(pchUserAgent, pchUserCSS);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_RemoveBrowser(HHTMLBrowser unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.RemoveBrowser(unBrowserHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_LoadURL(HHTMLBrowser unBrowserHandle, string pchURL, string pchPostData)
        {
            SteamEmulator.SteamHTMLSurface.LoadURL(unBrowserHandle, pchURL, pchPostData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_SetSize(HHTMLBrowser unBrowserHandle, uint unWidth, uint unHeight)
        {
            SteamEmulator.SteamHTMLSurface.SetSize(unBrowserHandle, unWidth, unHeight);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_StopLoad(HHTMLBrowser unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.StopLoad(unBrowserHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_Reload(HHTMLBrowser unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.Reload(unBrowserHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_GoBack(HHTMLBrowser unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.GoBack(unBrowserHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_GoForward(HHTMLBrowser unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.GoForward(unBrowserHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_AddHeader(HHTMLBrowser unBrowserHandle, string pchKey, string pchValue)
        {
            //SteamEmulator.SteamHTMLSurface.AddHeader(unBrowserHandle, pchKey, pchValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_ExecuteJavascript(HHTMLBrowser unBrowserHandle, string pchScript)
        {
            SteamEmulator.SteamHTMLSurface.ExecuteJavascript(unBrowserHandle, pchScript);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_MouseUp(HHTMLBrowser unBrowserHandle, int eMouseButton)
        {
            SteamEmulator.SteamHTMLSurface.MouseUp(unBrowserHandle, eMouseButton);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_MouseDown(HHTMLBrowser unBrowserHandle, int eMouseButton)
        {
            SteamEmulator.SteamHTMLSurface.MouseDown(unBrowserHandle, eMouseButton);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_MouseDoubleClick(HHTMLBrowser unBrowserHandle, EHTMLMouseButton eMouseButton)
        {
            SteamEmulator.SteamHTMLSurface.MouseDoubleClick(unBrowserHandle, (int)eMouseButton);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_MouseMove(HHTMLBrowser unBrowserHandle, int x, int y)
        {
            SteamEmulator.SteamHTMLSurface.MouseMove(unBrowserHandle, x, y);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_MouseWheel(HHTMLBrowser unBrowserHandle, int nDelta)
        {
            SteamEmulator.SteamHTMLSurface.MouseWheel(unBrowserHandle, nDelta);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_KeyDown(HHTMLBrowser unBrowserHandle, uint nNativeKeyCode, int eHTMLKeyModifiers)
        {
            SteamEmulator.SteamHTMLSurface.KeyDown(unBrowserHandle, nNativeKeyCode, eHTMLKeyModifiers);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_KeyUp(HHTMLBrowser unBrowserHandle, uint nNativeKeyCode, int eHTMLKeyModifiers)
        {
            SteamEmulator.SteamHTMLSurface.KeyUp(unBrowserHandle, nNativeKeyCode, eHTMLKeyModifiers);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_KeyChar(HHTMLBrowser unBrowserHandle, uint cUnicodeChar, int eHTMLKeyModifiers)
        {
            //SteamEmulator.SteamHTMLSurface.KeyChar(unBrowserHandle, cUnicodeChar, eHTMLKeyModifiers);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_SetHorizontalScroll(HHTMLBrowser unBrowserHandle, uint nAbsolutePixelScroll)
        {
            SteamEmulator.SteamHTMLSurface.SetHorizontalScroll(unBrowserHandle, nAbsolutePixelScroll);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_SetVerticalScroll(HHTMLBrowser unBrowserHandle, uint nAbsolutePixelScroll)
        {
            SteamEmulator.SteamHTMLSurface.SetVerticalScroll(unBrowserHandle, nAbsolutePixelScroll);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_SetKeyFocus(HHTMLBrowser unBrowserHandle, bool bHasKeyFocus)
        {
            SteamEmulator.SteamHTMLSurface.SetKeyFocus(unBrowserHandle, bHasKeyFocus);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_ViewSource(HHTMLBrowser unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.ViewSource(unBrowserHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_CopyToClipboard(HHTMLBrowser unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.CopyToClipboard(unBrowserHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_PasteFromClipboard(HHTMLBrowser unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.PasteFromClipboard(unBrowserHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_Find(HHTMLBrowser unBrowserHandle, string pchSearchStr, bool bCurrentlyInFind, bool bReverse)
        {
            SteamEmulator.SteamHTMLSurface.Find(unBrowserHandle, pchSearchStr, bCurrentlyInFind, bReverse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_StopFind(HHTMLBrowser unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.StopFind(unBrowserHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_GetLinkAtPosition(HHTMLBrowser unBrowserHandle, int x, int y)
        {
            SteamEmulator.SteamHTMLSurface.GetLinkAtPosition(unBrowserHandle, x, y);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_SetCookie(string pchHostname, string pchKey, string pchValue, string pchPath, uint nExpires, bool bSecure, bool bHTTPOnly)
        {
            SteamEmulator.SteamHTMLSurface.SetCookie(pchHostname, pchKey, pchValue, pchPath, nExpires, bSecure);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_SetPageScaleFactor(HHTMLBrowser unBrowserHandle, float flZoom, int nPointX, int nPointY)
        {
            SteamEmulator.SteamHTMLSurface.SetPageScaleFactor(unBrowserHandle, flZoom, nPointX, nPointY);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_AllowStartRequest(HHTMLBrowser unBrowserHandle, bool bAllowed)
        {
            SteamEmulator.SteamHTMLSurface.AllowStartRequest(unBrowserHandle, bAllowed);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_JSDialogResponse(HHTMLBrowser unBrowserHandle, bool bResult)
        {
            SteamEmulator.SteamHTMLSurface.JSDialogResponse(unBrowserHandle, bResult);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamHTMLSurface_FileLoadDialogResponse(HHTMLBrowser unBrowserHandle, string pchSelectedFiles)
        {
            SteamEmulator.SteamHTMLSurface.FileLoadDialogResponse(unBrowserHandle, pchSelectedFiles);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamHTTP_CreateHTTPRequest(uint eHTTPRequestMethod, string pchAbsoluteURL)
        {
            return SteamEmulator.SteamHTTP.CreateHTTPRequest(eHTTPRequestMethod, pchAbsoluteURL);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTTP_SetHTTPRequestContextValue(HTTPRequestHandle hRequest, ulong ulContextValue)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestContextValue(hRequest, ulContextValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTTP_SetHTTPRequestNetworkActivityTimeout(HTTPRequestHandle hRequest, uint unTimeoutSeconds)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestNetworkActivityTimeout(hRequest, unTimeoutSeconds);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTTP_SetHTTPRequestHeaderValue(HTTPRequestHandle hRequest, string pchHeaderName, string pchHeaderValue)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestHeaderValue(hRequest, pchHeaderName, pchHeaderValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTTP_SetHTTPRequestGetOrPostParameter(HTTPRequestHandle hRequest, string pchParamName, string pchParamValue)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestGetOrPostParameter(hRequest, pchParamName, pchParamValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTTP_SendHTTPRequest(HTTPRequestHandle hRequest, ref SteamAPICall_t pCallHandle)
        {
            return SteamEmulator.SteamHTTP.SendHTTPRequest(hRequest, ref pCallHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTTP_SendHTTPRequestAndStreamResponse(HTTPRequestHandle hRequest, ref SteamAPICall_t pCallHandle)
        {
            return SteamEmulator.SteamHTTP.SendHTTPRequestAndStreamResponse(hRequest, pCallHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTTP_DeferHTTPRequest(HTTPRequestHandle hRequest)
        {
            return SteamEmulator.SteamHTTP.DeferHTTPRequest(hRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTTP_PrioritizeHTTPRequest(HTTPRequestHandle hRequest)
        {
            return SteamEmulator.SteamHTTP.PrioritizeHTTPRequest(hRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTTP_GetHTTPResponseHeaderSize(HTTPRequestHandle hRequest, string pchHeaderName, ref uint unResponseHeaderSize)
        {
            return SteamEmulator.SteamHTTP.GetHTTPResponseHeaderSize(hRequest, pchHeaderName, unResponseHeaderSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTTP_GetHTTPResponseHeaderValue(HTTPRequestHandle hRequest, string pchHeaderName, int pHeaderValueBuffer, uint unBufferSize)
        //public static bool ISteamHTTP_GetHTTPResponseHeaderValue(HTTPRequestHandle hRequest, string pchHeaderName, ref byte[] pHeaderValueBuffer, uint unBufferSize)
        {
            return SteamEmulator.SteamHTTP.GetHTTPResponseHeaderValue(hRequest, pchHeaderName, pHeaderValueBuffer, unBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTTP_GetHTTPResponseBodySize(HTTPRequestHandle hRequest, ref uint unBodySize)
        {
            return SteamEmulator.SteamHTTP.GetHTTPResponseBodySize(hRequest, unBodySize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTTP_GetHTTPResponseBodyData(HTTPRequestHandle hRequest, IntPtr pBodyDataBuffer, uint unBufferSize)
        //public static bool ISteamHTTP_GetHTTPResponseBodyData(HTTPRequestHandle hRequest, ref byte[] pBodyDataBuffer, uint unBufferSize)
        {
            return SteamEmulator.SteamHTTP.GetHTTPResponseBodyData(hRequest, pBodyDataBuffer, unBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTTP_GetHTTPStreamingResponseBodyData(HTTPRequestHandle hRequest, uint cOffset, IntPtr pBodyDataBuffer, uint unBufferSize)
        //public static bool ISteamHTTP_GetHTTPStreamingResponseBodyData(HTTPRequestHandle hRequest, uint cOffset, ref byte[] pBodyDataBuffer, uint unBufferSize)
        {
            return SteamEmulator.SteamHTTP.GetHTTPStreamingResponseBodyData(hRequest, cOffset, pBodyDataBuffer, unBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTTP_ReleaseHTTPRequest(HTTPRequestHandle hRequest)
        {
            return SteamEmulator.SteamHTTP.ReleaseHTTPRequest(hRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTTP_GetHTTPDownloadProgressPct(HTTPRequestHandle hRequest, ref float pflPercentOut)
        {
            return SteamEmulator.SteamHTTP.GetHTTPDownloadProgressPct(hRequest, pflPercentOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTTP_SetHTTPRequestRawPostBody(HTTPRequestHandle hRequest, string pchContentType, IntPtr pubBody, uint unBodyLen)
        //public static bool ISteamHTTP_SetHTTPRequestRawPostBody(HTTPRequestHandle hRequest, string pchContentType, ref byte[] pubBody, uint unBodyLen)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestRawPostBody(hRequest, pchContentType, pubBody, unBodyLen);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamHTTP_CreateCookieContainer(bool bAllowResponsesToModify)
        {
            return SteamEmulator.SteamHTTP.CreateCookieContainer(bAllowResponsesToModify);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTTP_ReleaseCookieContainer(HTTPCookieContainerHandle hCookieContainer)
        {
            return SteamEmulator.SteamHTTP.ReleaseCookieContainer(hCookieContainer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTTP_SetCookie(HTTPCookieContainerHandle hCookieContainer, string pchHost, string pchUrl, string pchCookie)
        {
            return SteamEmulator.SteamHTTP.SetCookie(hCookieContainer, pchHost, pchUrl, pchCookie);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTTP_SetHTTPRequestCookieContainer(HTTPRequestHandle hRequest, HTTPCookieContainerHandle hCookieContainer)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestCookieContainer(hRequest, hCookieContainer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTTP_SetHTTPRequestUserAgentInfo(HTTPRequestHandle hRequest, string pchUserAgentInfo)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestUserAgentInfo(hRequest, pchUserAgentInfo);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTTP_SetHTTPRequestRequiresVerifiedCertificate(HTTPRequestHandle hRequest, bool bRequireVerifiedCertificate)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestRequiresVerifiedCertificate(hRequest, bRequireVerifiedCertificate);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTTP_SetHTTPRequestAbsoluteTimeoutMS(HTTPRequestHandle hRequest, uint unMilliseconds)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestAbsoluteTimeoutMS(hRequest, unMilliseconds);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamHTTP_GetHTTPRequestWasTimedOut(HTTPRequestHandle hRequest, ref bool pbWasTimedOut)
        {
            return SteamEmulator.SteamHTTP.GetHTTPRequestWasTimedOut(hRequest, pbWasTimedOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EResult ISteamInventory_GetResultStatus(SteamInventoryResult_t resultHandle)
        {
            return (EResult)SteamEmulator.SteamInventory.GetResultStatus(resultHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamInventory_GetResultItems(SteamInventoryResult_t resultHandle, IntPtr pOutItemsArray, ref uint punOutItemsArraySize)
        //public static bool ISteamInventory_GetResultItems(SteamInventoryResult_t resultHandle, ref SteamItemDetails_t[] pOutItemsArray, ref uint punOutItemsArraySize)
        {
            return SteamEmulator.SteamInventory.GetResultItems(resultHandle, pOutItemsArray, punOutItemsArraySize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamInventory_GetResultTimestamp(SteamInventoryResult_t resultHandle)
        {
            return SteamEmulator.SteamInventory.GetResultTimestamp(resultHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamInventory_CheckResultSteamID(SteamInventoryResult_t resultHandle, ulong steamIDExpected)
        {
            return SteamEmulator.SteamInventory.CheckResultSteamID(resultHandle, (ulong)steamIDExpected);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamInventory_DestroyResult(SteamInventoryResult_t resultHandle)
        {
            SteamEmulator.SteamInventory.DestroyResult(resultHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamInventory_GetAllItems(ref SteamInventoryResult_t pResultHandle)
        {
            return SteamEmulator.SteamInventory.GetAllItems(pResultHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamInventory_GetItemsByID(ref SteamInventoryResult_t pResultHandle, ref SteamItemInstanceID_t pInstanceIDs, uint unCountInstanceIDs)
        //public static bool ISteamInventory_GetItemsByID(ref SteamInventoryResult_t pResultHandle, ref SteamItemInstanceID_t[] pInstanceIDs, uint unCountInstanceIDs)
        {
            return SteamEmulator.SteamInventory.GetItemsByID(pResultHandle, ref pInstanceIDs, unCountInstanceIDs);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamInventory_SerializeResult(SteamInventoryResult_t resultHandle, IntPtr pOutBuffer, ref uint punOutBufferSize)
        //public static bool ISteamInventory_SerializeResult(SteamInventoryResult_t resultHandle, ref byte[] pOutBuffer, ref uint punOutBufferSize)
        {
            return SteamEmulator.SteamInventory.SerializeResult(resultHandle, pOutBuffer, punOutBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamInventory_DeserializeResult(ref SteamInventoryResult_t pOutResultHandle, IntPtr pBuffer, uint unBufferSize, bool bRESERVED_MUST_BE_FALSE)
        //public static bool ISteamInventory_DeserializeResult(ref SteamInventoryResult_t pOutResultHandle, ref byte[] pBuffer, uint unBufferSize, bool bRESERVED_MUST_BE_FALSE)
        {
            return SteamEmulator.SteamInventory.DeserializeResult(pOutResultHandle, pBuffer, unBufferSize, bRESERVED_MUST_BE_FALSE);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamInventory_GenerateItems(ref SteamInventoryResult_t pResultHandle, IntPtr pArrayItemDefs, IntPtr punArrayQuantity, uint unArrayLength)
        //public static bool ISteamInventory_GenerateItems(ref SteamInventoryResult_t pResultHandle, ref SteamItemDef_t[] pArrayItemDefs, ref uint[] punArrayQuantity, uint unArrayLength)
        {
            return SteamEmulator.SteamInventory.GenerateItems(pResultHandle, pArrayItemDefs, punArrayQuantity, unArrayLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamInventory_GrantPromoItems(ref SteamInventoryResult_t pResultHandle)
        {
            return SteamEmulator.SteamInventory.GrantPromoItems(pResultHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamInventory_AddPromoItem(ref SteamInventoryResult_t pResultHandle, SteamItemDef_t itemDef)
        {
            return SteamEmulator.SteamInventory.AddPromoItem(pResultHandle, itemDef);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamInventory_AddPromoItems(ref SteamInventoryResult_t pResultHandle, IntPtr pArrayItemDefs, uint unArrayLength)
        //public static bool ISteamInventory_AddPromoItems(ref SteamInventoryResult_t pResultHandle, ref SteamItemDef_t[] pArrayItemDefs, uint unArrayLength)
        {
            return SteamEmulator.SteamInventory.AddPromoItems(pResultHandle, pArrayItemDefs, unArrayLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamInventory_ConsumeItem(ref SteamInventoryResult_t pResultHandle, SteamItemInstanceID_t itemConsume, uint unQuantity)
        {
            return SteamEmulator.SteamInventory.ConsumeItem(pResultHandle, itemConsume, unQuantity);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamInventory_ExchangeItems(ref SteamInventoryResult_t pResultHandle, ref SteamItemDef_t[] pArrayGenerate, ref uint[] punArrayGenerateQuantity, uint unArrayGenerateLength, ref SteamItemInstanceID_t[] pArrayDestroy, ref uint[] punArrayDestroyQuantity, uint unArrayDestroyLength)
        {
            return SteamEmulator.SteamInventory.ExchangeItems(ref pResultHandle, ref pArrayGenerate, ref punArrayGenerateQuantity, unArrayGenerateLength, ref pArrayDestroy, ref punArrayDestroyQuantity, unArrayDestroyLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamInventory_TransferItemQuantity(ref SteamInventoryResult_t pResultHandle, SteamItemInstanceID_t itemIdSource, uint unQuantity, SteamItemInstanceID_t itemIdDest)
        {
            return SteamEmulator.SteamInventory.TransferItemQuantity(pResultHandle, (uint)itemIdSource, unQuantity, (uint)itemIdDest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamInventory_SendItemDropHeartbeat()
        {
            SteamEmulator.SteamInventory.SendItemDropHeartbeat();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamInventory_TriggerItemDrop(ref SteamInventoryResult_t pResultHandle, SteamItemDef_t dropListDefinition)
        {
            return SteamEmulator.SteamInventory.TriggerItemDrop(pResultHandle, dropListDefinition);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamInventory_TradeItems(ref SteamInventoryResult_t pResultHandle, ulong steamIDTradePartner, IntPtr pArrayGive, IntPtr pArrayGiveQuantity, uint nArrayGiveLength, IntPtr pArrayGet, IntPtr pArrayGetQuantity, uint nArrayGetLength)
        //public static bool ISteamInventory_TradeItems(ref SteamInventoryResult_t pResultHandle, ulong steamIDTradePartner, ref SteamItemInstanceID_t[] pArrayGive, ref uint[] pArrayGiveQuantity, uint nArrayGiveLength, ref SteamItemInstanceID_t[] pArrayGet, ref uint[] pArrayGetQuantity, uint nArrayGetLength)
        {
            return SteamEmulator.SteamInventory.TradeItems(pResultHandle, (ulong)steamIDTradePartner, pArrayGive, pArrayGiveQuantity, nArrayGiveLength, pArrayGet, pArrayGetQuantity, nArrayGetLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamInventory_LoadItemDefinitions()
        {
            return SteamEmulator.SteamInventory.LoadItemDefinitions();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamInventory_GetItemDefinitionIDs(IntPtr pItemDefIDs, ref uint punItemDefIDsArraySize)
        //public static bool ISteamInventory_GetItemDefinitionIDs(ref SteamItemDef_t[] pItemDefIDs, ref uint punItemDefIDsArraySize)
        {
            return SteamEmulator.SteamInventory.GetItemDefinitionIDs(pItemDefIDs, punItemDefIDsArraySize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamInventory_GetItemDefinitionProperty(SteamItemDef_t iDefinition, string pchPropertyName, IntPtr pchValueBuffer, ref uint punValueBufferSize)
        {
            return SteamEmulator.SteamInventory.GetItemDefinitionProperty(iDefinition, pchPropertyName, pchValueBuffer, punValueBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamMatchmaking_GetFavoriteGameCount()
        {
            return SteamEmulator.SteamMatchmaking.GetFavoriteGameCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMatchmaking_GetFavoriteGame(int iGame, ref AppId_t pnAppID, ref uint pnIP, ref uint pnConnPort, ref uint pnQueryPort, ref uint punFlags, ref uint pRTime32LastPlayedOnServer)
        {
            return SteamEmulator.SteamMatchmaking.GetFavoriteGame(iGame, ref pnAppID, ref pnIP, ref pnConnPort, ref pnQueryPort, ref punFlags, pRTime32LastPlayedOnServer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamMatchmaking_AddFavoriteGame(AppId_t nAppID, uint nIP, ushort nConnPort, ushort nQueryPort, uint unFlags, uint rTime32LastPlayedOnServer)
        {
            return SteamEmulator.SteamMatchmaking.AddFavoriteGame(nAppID, nIP, nConnPort, nQueryPort, unFlags, rTime32LastPlayedOnServer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMatchmaking_RemoveFavoriteGame(AppId_t nAppID, uint nIP, ushort nConnPort, ushort nQueryPort, uint unFlags)
        {
            return SteamEmulator.SteamMatchmaking.RemoveFavoriteGame(nAppID, nIP, nConnPort, nQueryPort, unFlags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamMatchmaking_RequestLobbyList()
        {
            return SteamEmulator.SteamMatchmaking.RequestLobbyList();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamMatchmaking_AddRequestLobbyListStringFilter(string pchKeyToMatch, string pchValueToMatch, ELobbyComparison eComparisonType)
        {
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListStringFilter(pchKeyToMatch, pchValueToMatch, (int)eComparisonType);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamMatchmaking_AddRequestLobbyListNumericalFilter(string pchKeyToMatch, int nValueToMatch, ELobbyComparison eComparisonType)
        {
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListNumericalFilter(pchKeyToMatch, nValueToMatch, (int)eComparisonType);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamMatchmaking_AddRequestLobbyListNearValueFilter(string pchKeyToMatch, int nValueToBeCloseTo)
        {
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListNearValueFilter(pchKeyToMatch, nValueToBeCloseTo);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamMatchmaking_AddRequestLobbyListFilterSlotsAvailable(int nSlotsAvailable)
        {
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(nSlotsAvailable);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamMatchmaking_AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter eLobbyDistanceFilter)
        {
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListDistanceFilter((int)eLobbyDistanceFilter);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamMatchmaking_AddRequestLobbyListResultCountFilter(int cMaxResults)
        {
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListResultCountFilter(cMaxResults);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamMatchmaking_AddRequestLobbyListCompatibleMembersFilter(ulong steamIDLobby)
        {
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListCompatibleMembersFilter((ulong)steamIDLobby);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamMatchmaking_GetLobbyByIndex(int iLobby)
        {
            return (ulong)SteamEmulator.SteamMatchmaking.GetLobbyByIndex(iLobby);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamMatchmaking_CreateLobby(ELobbyType eLobbyType, int cMaxMembers)
        {
            return SteamEmulator.SteamMatchmaking.CreateLobby((int)eLobbyType, cMaxMembers);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamMatchmaking_JoinLobby(ulong steamIDLobby)
        {
            return SteamEmulator.SteamMatchmaking.JoinLobby((ulong)steamIDLobby);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamMatchmaking_LeaveLobby(ulong steamIDLobby)
        {
            SteamEmulator.SteamMatchmaking.LeaveLobby((ulong)steamIDLobby);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMatchmaking_InviteUserToLobby(ulong steamIDLobby, ulong steamIDInvitee)
        {
            return SteamEmulator.SteamMatchmaking.InviteUserToLobby((ulong)steamIDLobby, (ulong)steamIDInvitee);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamMatchmaking_GetNumLobbyMembers(ulong steamIDLobby)
        {
            return SteamEmulator.SteamMatchmaking.GetNumLobbyMembers((ulong)steamIDLobby);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamMatchmaking_GetLobbyMemberByIndex(ulong steamIDLobby, int iMember)
        {
            return (ulong)SteamEmulator.SteamMatchmaking.GetLobbyMemberByIndex((ulong)steamIDLobby, iMember);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string ISteamMatchmaking_GetLobbyData(ulong steamIDLobby, string pchKey)
        {
            return SteamEmulator.SteamMatchmaking.GetLobbyData((ulong)steamIDLobby, pchKey);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMatchmaking_SetLobbyData(ulong steamIDLobby, string pchKey, string pchValue)
        {
            return SteamEmulator.SteamMatchmaking.SetLobbyData((ulong)steamIDLobby, pchKey, pchValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamMatchmaking_GetLobbyDataCount(ulong steamIDLobby)
        {
            return SteamEmulator.SteamMatchmaking.GetLobbyDataCount((ulong)steamIDLobby);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMatchmaking_GetLobbyDataByIndex(ulong steamIDLobby, int iLobbyData, IntPtr pchKey, int cchKeyBufferSize, IntPtr pchValue, int cchValueBufferSize)
        {
            return SteamEmulator.SteamMatchmaking.GetLobbyDataByIndex((ulong)steamIDLobby, iLobbyData, pchKey, cchKeyBufferSize, pchValue, cchValueBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMatchmaking_DeleteLobbyData(ulong steamIDLobby, string pchKey)
        {
            return SteamEmulator.SteamMatchmaking.DeleteLobbyData((ulong)steamIDLobby, pchKey);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string ISteamMatchmaking_GetLobbyMemberData(ulong steamIDLobby, ulong steamIDUser, string pchKey)
        {
            return SteamEmulator.SteamMatchmaking.GetLobbyMemberData((ulong)steamIDLobby, (ulong)steamIDUser, pchKey);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamMatchmaking_SetLobbyMemberData(ulong steamIDLobby, string pchKey, string pchValue)
        {
            SteamEmulator.SteamMatchmaking.SetLobbyMemberData((ulong)steamIDLobby, pchKey, pchValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMatchmaking_SendLobbyChatMsg(ulong steamIDLobby, IntPtr pvMsgBody, int cubMsgBody)
        //public static bool ISteamMatchmaking_SendLobbyChatMsg(ulong steamIDLobby, ref byte[] pvMsgBody, int cubMsgBody)
        {
            return SteamEmulator.SteamMatchmaking.SendLobbyChatMsg((ulong)steamIDLobby, pvMsgBody, cubMsgBody);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamMatchmaking_GetLobbyChatEntry(ulong steamIDLobby, int iChatID, ref ulong pSteamIDUser, IntPtr pvData, int cubData, ref EChatEntryType peChatEntryType)
        //public static int ISteamMatchmaking_GetLobbyChatEntry(ulong steamIDLobby, int iChatID, ref ulong pSteamIDUser, ref byte[] pvData, int cubData, ref EChatEntryType peChatEntryType)
        {
            return SteamEmulator.SteamMatchmaking.GetLobbyChatEntry((ulong)steamIDLobby, iChatID, (ulong)pSteamIDUser, pvData, cubData, (int)peChatEntryType);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMatchmaking_RequestLobbyData(ulong steamIDLobby)
        {
            return SteamEmulator.SteamMatchmaking.RequestLobbyData((ulong)steamIDLobby);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamMatchmaking_SetLobbyGameServer(ulong steamIDLobby, uint unGameServerIP, ushort unGameServerPort, ulong steamIDGameServer)
        {
            SteamEmulator.SteamMatchmaking.SetLobbyGameServer((ulong)steamIDLobby, unGameServerIP, unGameServerPort, (ulong)steamIDGameServer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMatchmaking_GetLobbyGameServer(ulong steamIDLobby, ref uint punGameServerIP, ref uint punGameServerPort, ref ulong psteamIDGameServer)
        {
            return SteamEmulator.SteamMatchmaking.GetLobbyGameServer((ulong)steamIDLobby, ref punGameServerIP, ref punGameServerPort, ref psteamIDGameServer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMatchmaking_SetLobbyMemberLimit(ulong steamIDLobby, int cMaxMembers)
        {
            return SteamEmulator.SteamMatchmaking.SetLobbyMemberLimit((ulong)steamIDLobby, cMaxMembers);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamMatchmaking_GetLobbyMemberLimit(ulong steamIDLobby)
        {
            return SteamEmulator.SteamMatchmaking.GetLobbyMemberLimit((ulong)steamIDLobby);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMatchmaking_SetLobbyType(ulong steamIDLobby, ELobbyType eLobbyType)
        {
            return SteamEmulator.SteamMatchmaking.SetLobbyType((ulong)steamIDLobby, (int)eLobbyType);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMatchmaking_SetLobbyJoinable(ulong steamIDLobby, bool bLobbyJoinable)
        {
            return SteamEmulator.SteamMatchmaking.SetLobbyJoinable((ulong)steamIDLobby, bLobbyJoinable);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamMatchmaking_GetLobbyOwner(ulong steamIDLobby)
        {
            return (ulong)SteamEmulator.SteamMatchmaking.GetLobbyOwner((ulong)steamIDLobby);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMatchmaking_SetLobbyOwner(ulong steamIDLobby, ulong steamIDNewOwner)
        {
            return SteamEmulator.SteamMatchmaking.SetLobbyOwner((ulong)steamIDLobby, (ulong)steamIDNewOwner);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMatchmaking_SetLinkedLobby(ulong steamIDLobby, ulong steamIDLobbyDependent)
        {
            return SteamEmulator.SteamMatchmaking.SetLinkedLobby((ulong)steamIDLobby, (ulong)steamIDLobbyDependent);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HServerListRequest ISteamMatchmakingServers_RequestInternetServerList(AppId_t iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            return SteamEmulator.SteamMatchMakingServers.RequestInternetServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HServerListRequest ISteamMatchmakingServers_RequestLANServerList(AppId_t iApp, IntPtr pRequestServersResponse)
        {
            return SteamEmulator.SteamMatchMakingServers.RequestLANServerList(iApp, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HServerListRequest ISteamMatchmakingServers_RequestFriendsServerList(AppId_t iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            return SteamEmulator.SteamMatchMakingServers.RequestFriendsServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HServerListRequest ISteamMatchmakingServers_RequestFavoritesServerList(AppId_t iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            return SteamEmulator.SteamMatchMakingServers.RequestFavoritesServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamMatchmakingServers_RequestHistoryServerList(AppId_t iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            return SteamEmulator.SteamMatchMakingServers.RequestHistoryServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HServerListRequest ISteamMatchmakingServers_RequestSpectatorServerList(AppId_t iApp, IntPtr ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
        {
            return SteamEmulator.SteamMatchMakingServers.RequestSpectatorServerList(iApp, ppchFilters, nFilters, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamMatchmakingServers_ReleaseRequest(IntPtr hServerListRequest)
        {
            SteamEmulator.SteamMatchMakingServers.ReleaseRequest(hServerListRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr ISteamMatchmakingServers_GetServerDetails(HServerListRequest hRequest, int iServer)
        {
            return SteamEmulator.SteamMatchMakingServers.GetServerDetails(hRequest, iServer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamMatchmakingServers_CancelQuery(HServerListRequest hRequest)
        {
            SteamEmulator.SteamMatchMakingServers.CancelQuery(hRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamMatchmakingServers_RefreshQuery(HServerListRequest hRequest)
        {
            SteamEmulator.SteamMatchMakingServers.RefreshQuery(hRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMatchmakingServers_IsRefreshing(HServerListRequest hRequest)
        {
            return SteamEmulator.SteamMatchMakingServers.IsRefreshing(hRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamMatchmakingServers_GetServerCount(HServerListRequest hRequest)
        {
            return SteamEmulator.SteamMatchMakingServers.GetServerCount(hRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamMatchmakingServers_RefreshServer(HServerListRequest hRequest, int iServer)
        {
            SteamEmulator.SteamMatchMakingServers.RefreshServer(hRequest, iServer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamMatchmakingServers_PingServer(uint unIP, ushort usPort, IntPtr pRequestServersResponse)
        {
            return (int)SteamEmulator.SteamMatchMakingServers.PingServer(unIP, usPort, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamMatchmakingServers_PlayerDetails(uint unIP, ushort usPort, IntPtr pRequestServersResponse)
        {
            return (int)SteamEmulator.SteamMatchMakingServers.PlayerDetails(unIP, usPort, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamMatchmakingServers_ServerRules(uint unIP, ushort usPort, IntPtr pRequestServersResponse)
        {
            return (int)SteamEmulator.SteamMatchMakingServers.ServerRules(unIP, usPort, pRequestServersResponse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamMatchmakingServers_CancelServerQuery(HServerQuery hServerQuery)
        {
            SteamEmulator.SteamMatchMakingServers.CancelServerQuery(hServerQuery);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusic_BIsEnabled()
        {
            return SteamEmulator.SteamMusic.BIsEnabled();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusic_BIsPlaying()
        {
            return SteamEmulator.SteamMusic.BIsPlaying();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static AudioPlayback_Status ISteamMusic_GetPlaybackStatus()
        {
            return (AudioPlayback_Status)SteamEmulator.SteamMusic.GetPlaybackStatus();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamMusic_Play()
        {
            SteamEmulator.SteamMusic.Play();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamMusic_Pause()
        {
            SteamEmulator.SteamMusic.Pause();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamMusic_PlayPrevious()
        {
            SteamEmulator.SteamMusic.PlayPrevious();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamMusic_PlayNext()
        {
            SteamEmulator.SteamMusic.PlayNext();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamMusic_SetVolume(float flVolume)
        {
            SteamEmulator.SteamMusic.SetVolume(flVolume);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static float ISteamMusic_GetVolume()
        {
            return SteamEmulator.SteamMusic.GetVolume();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_RegisterSteamMusicRemote(string pchName)
        {
            return SteamEmulator.SteamMusicRemote.RegisterSteamMusicRemote(pchName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_DeregisterSteamMusicRemote()
        {
            return SteamEmulator.SteamMusicRemote.DeregisterSteamMusicRemote();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_BIsCurrentMusicRemote()
        {
            return SteamEmulator.SteamMusicRemote.BIsCurrentMusicRemote();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_BActivationSuccess(bool bValue)
        {
            return SteamEmulator.SteamMusicRemote.BActivationSuccess(bValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_SetDisplayName(string pchDisplayName)
        {
            return SteamEmulator.SteamMusicRemote.SetDisplayName(pchDisplayName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_SetPNGIcon_64x64(IntPtr pvBuffer, uint cbBufferLength)
        //public static bool ISteamMusicRemote_SetPNGIcon_64x64(ref byte[] pvBuffer, uint cbBufferLength)
        {
            return SteamEmulator.SteamMusicRemote.SetPNGIcon_64x64(pvBuffer, cbBufferLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_EnablePlayPrevious(bool bValue)
        {
            return SteamEmulator.SteamMusicRemote.EnablePlayPrevious(bValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_EnablePlayNext(bool bValue)
        {
            return SteamEmulator.SteamMusicRemote.EnablePlayNext(bValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_EnableShuffled(bool bValue)
        {
            return SteamEmulator.SteamMusicRemote.EnableShuffled(bValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_EnableLooped(bool bValue)
        {
            return SteamEmulator.SteamMusicRemote.EnableLooped(bValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_EnableQueue(bool bValue)
        {
            return SteamEmulator.SteamMusicRemote.EnableQueue(bValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_EnablePlaylists(bool bValue)
        {
            return SteamEmulator.SteamMusicRemote.EnablePlaylists(bValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_UpdatePlaybackStatus(AudioPlayback_Status nStatus)
        {
            return SteamEmulator.SteamMusicRemote.UpdatePlaybackStatus((int)nStatus);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_UpdateShuffled(bool bValue)
        {
            return SteamEmulator.SteamMusicRemote.UpdateShuffled(bValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_UpdateLooped(bool bValue)
        {
            return SteamEmulator.SteamMusicRemote.UpdateLooped(bValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_UpdateVolume(float flValue)
        {
            return SteamEmulator.SteamMusicRemote.UpdateVolume(flValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_CurrentEntryWillChange()
        {
            return SteamEmulator.SteamMusicRemote.CurrentEntryWillChange();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_CurrentEntryIsAvailable(bool bAvailable)
        {
            return SteamEmulator.SteamMusicRemote.CurrentEntryIsAvailable(bAvailable);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_UpdateCurrentEntryText(string pchText)
        {
            return SteamEmulator.SteamMusicRemote.UpdateCurrentEntryText(pchText);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_UpdateCurrentEntryElapsedSeconds(int nValue)
        {
            return SteamEmulator.SteamMusicRemote.UpdateCurrentEntryElapsedSeconds(nValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_UpdateCurrentEntryCoverArt(IntPtr pvBuffer, uint cbBufferLength)
        //public static bool ISteamMusicRemote_UpdateCurrentEntryCoverArt(ref byte[] pvBuffer, uint cbBufferLength)
        {
            return SteamEmulator.SteamMusicRemote.UpdateCurrentEntryCoverArt(pvBuffer, cbBufferLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_CurrentEntryDidChange()
        {
            return SteamEmulator.SteamMusicRemote.CurrentEntryDidChange();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_QueueWillChange()
        {
            return SteamEmulator.SteamMusicRemote.QueueWillChange();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_ResetQueueEntries()
        {
            return SteamEmulator.SteamMusicRemote.ResetQueueEntries();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_SetQueueEntry(int nID, int nPosition, string pchEntryText)
        {
            return SteamEmulator.SteamMusicRemote.SetQueueEntry(nID, nPosition, pchEntryText);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_SetCurrentQueueEntry(int nID)
        {
            return SteamEmulator.SteamMusicRemote.SetCurrentQueueEntry(nID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_QueueDidChange()
        {
            return SteamEmulator.SteamMusicRemote.QueueDidChange();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_PlaylistWillChange()
        {
            return SteamEmulator.SteamMusicRemote.PlaylistWillChange();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_ResetPlaylistEntries()
        {
            return SteamEmulator.SteamMusicRemote.ResetPlaylistEntries();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_SetPlaylistEntry(int nID, int nPosition, string pchEntryText)
        {
            return SteamEmulator.SteamMusicRemote.SetPlaylistEntry(nID, nPosition, pchEntryText);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_SetCurrentPlaylistEntry(int nID)
        {
            return SteamEmulator.SteamMusicRemote.SetCurrentPlaylistEntry(nID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamMusicRemote_PlaylistDidChange()
        {
            return SteamEmulator.SteamMusicRemote.PlaylistDidChange();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamNetworking_SendP2PPacket(ulong steamIDRemote, IntPtr pubData, uint cubData, EP2PSend eP2PSendType, int nChannel)
        {
            return SteamEmulator.SteamNetworking.SendP2PPacket((ulong)steamIDRemote, pubData, cubData, (int)eP2PSendType, nChannel);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamNetworking_IsP2PPacketAvailable(ref uint pcubMsgSize, int nChannel)
        {
            return SteamEmulator.SteamNetworking.IsP2PPacketAvailable(ref pcubMsgSize, nChannel);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamNetworking_ReadP2PPacket(IntPtr pubDest, uint cubDest, ref uint pcubMsgSize, ref ulong psteamIDRemote, int nChannel)
        {
            return SteamEmulator.SteamNetworking.ReadP2PPacket(pubDest, cubDest, ref pcubMsgSize, ref psteamIDRemote, nChannel);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamNetworking_AcceptP2PSessionWithUser(ulong steamIDRemote)
        {
            return SteamEmulator.SteamNetworking.AcceptP2PSessionWithUser((ulong)steamIDRemote);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamNetworking_CloseP2PSessionWithUser(ulong steamIDRemote)
        {
            return SteamEmulator.SteamNetworking.CloseP2PSessionWithUser((ulong)steamIDRemote);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamNetworking_CloseP2PChannelWithUser(ulong steamIDRemote, int nChannel)
        {
            return SteamEmulator.SteamNetworking.CloseP2PChannelWithUser((ulong)steamIDRemote, nChannel);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamNetworking_GetP2PSessionState(ulong steamIDRemote, IntPtr pConnectionState)
        //public static bool ISteamNetworking_GetP2PSessionState(ulong steamIDRemote, ref P2PSessionState_t pConnectionState)
        {
            return SteamEmulator.SteamNetworking.GetP2PSessionState((ulong)steamIDRemote, pConnectionState);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamNetworking_AllowP2PPacketRelay(bool bAllow)
        {
            return SteamEmulator.SteamNetworking.AllowP2PPacketRelay(bAllow);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamNetworking_CreateListenSocket(int nVirtualP2PPort, uint nIP, ushort nPort, bool bAllowUseOfPacketRelay)
        {
            return SteamEmulator.SteamNetworking.CreateListenSocket(nVirtualP2PPort, nIP, nPort, bAllowUseOfPacketRelay);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamNetworking_CreateP2PConnectionSocket(ulong steamIDTarget, int nVirtualPort, int nTimeoutSec, bool bAllowUseOfPacketRelay)
        {
            return SteamEmulator.SteamNetworking.CreateP2PConnectionSocket((ulong)steamIDTarget, nVirtualPort, nTimeoutSec, bAllowUseOfPacketRelay);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamNetworking_CreateConnectionSocket(uint nIP, ushort nPort, int nTimeoutSec)
        {
            return SteamEmulator.SteamNetworking.CreateConnectionSocket(nIP, nPort, nTimeoutSec);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamNetworking_DestroySocket(SNetSocket_t hSocket, bool bNotifyRemoteEnd)
        {
            return SteamEmulator.SteamNetworking.DestroySocket(hSocket, bNotifyRemoteEnd);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamNetworking_DestroyListenSocket(SNetListenSocket_t hSocket, bool bNotifyRemoteEnd)
        {
            return SteamEmulator.SteamNetworking.DestroyListenSocket(hSocket, bNotifyRemoteEnd);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamNetworking_SendDataOnSocket(SNetSocket_t hSocket, IntPtr pubData, uint cubData, bool bReliable)
        {
            return SteamEmulator.SteamNetworking.SendDataOnSocket(hSocket, pubData, cubData, bReliable);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamNetworking_IsDataAvailableOnSocket(SNetSocket_t hSocket, ref uint pcubMsgSize)
        {
            return SteamEmulator.SteamNetworking.IsDataAvailableOnSocket(hSocket, pcubMsgSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamNetworking_RetrieveDataFromSocket(SNetSocket_t hSocket, IntPtr pubDest, uint cubDest, ref uint pcubMsgSize)
        {
            return SteamEmulator.SteamNetworking.RetrieveDataFromSocket(hSocket, pubDest, cubDest, pcubMsgSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamNetworking_IsDataAvailable(SNetListenSocket_t hListenSocket, ref uint pcubMsgSize, ref SNetSocket_t phSocket)
        {
            return SteamEmulator.SteamNetworking.IsDataAvailable(hListenSocket, pcubMsgSize, phSocket);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamNetworking_RetrieveData(SNetListenSocket_t hListenSocket, IntPtr pubDest, uint cubDest, ref uint pcubMsgSize, ref SNetSocket_t phSocket)
        {
            return SteamEmulator.SteamNetworking.RetrieveData(hListenSocket, pubDest, cubDest, pcubMsgSize, phSocket);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamNetworking_GetSocketInfo(SNetSocket_t hSocket, ref ulong pSteamIDRemote, ref int peSocketStatus, ref uint punIPRemote, ref ushort punPortRemote)
        {
            return SteamEmulator.SteamNetworking.GetSocketInfo(hSocket, (ulong)pSteamIDRemote, peSocketStatus, punIPRemote, punPortRemote);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamNetworking_GetListenSocketInfo(SNetListenSocket_t hListenSocket, ref uint pnIP, ref ushort pnPort)
        {
            return SteamEmulator.SteamNetworking.GetListenSocketInfo(hListenSocket, pnIP, pnPort);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ESNetSocketConnectionType ISteamNetworking_GetSocketConnectionType(SNetSocket_t hSocket)
        {
            return (ESNetSocketConnectionType)SteamEmulator.SteamNetworking.GetSocketConnectionType(hSocket);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamNetworking_GetMaxPacketSize(SNetSocket_t hSocket)
        {
            return SteamEmulator.SteamNetworking.GetMaxPacketSize(hSocket);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamRemoteStorage_FileWrite(string pchFile, IntPtr pvData, int cubData)
        {
            return SteamEmulator.SteamRemoteStorage.FileWrite(pchFile, pvData, cubData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamRemoteStorage_FileRead(string pchFile, IntPtr pvData, int cubDataToRead)
        {
            return SteamEmulator.SteamRemoteStorage.FileRead(pchFile, pvData, cubDataToRead);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamRemoteStorage_FileForget(string pchFile)
        {
            return SteamEmulator.SteamRemoteStorage.FileForget(pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamRemoteStorage_FileDelete(string pchFile)
        {
            return SteamEmulator.SteamRemoteStorage.FileDelete(pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamRemoteStorage_FileShare(string pchFile)
        {
            return SteamEmulator.SteamRemoteStorage.FileShare(pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamRemoteStorage_SetSyncPlatforms(string pchFile, ERemoteStoragePlatform eRemoteStoragePlatform)
        {
            return SteamEmulator.SteamRemoteStorage.SetSyncPlatforms(pchFile, (int)eRemoteStoragePlatform);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamRemoteStorage_FileWriteStreamOpen(string pchFile)
        {
            return SteamEmulator.SteamRemoteStorage.FileWriteStreamOpen(pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamRemoteStorage_FileWriteStreamWriteChunk(UGCFileWriteStreamHandle_t writeHandle, IntPtr pvData, int cubData)
        //public static bool ISteamRemoteStorage_FileWriteStreamWriteChunk(UGCFileWriteStreamHandle_t writeHandle, ref byte[] pvData, int cubData)
        {
            return SteamEmulator.SteamRemoteStorage.FileWriteStreamWriteChunk(writeHandle, pvData, cubData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamRemoteStorage_FileWriteStreamClose(UGCFileWriteStreamHandle_t writeHandle)
        {
            return SteamEmulator.SteamRemoteStorage.FileWriteStreamClose(writeHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamRemoteStorage_FileWriteStreamCancel(UGCFileWriteStreamHandle_t writeHandle)
        {
            return SteamEmulator.SteamRemoteStorage.FileWriteStreamCancel(writeHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamRemoteStorage_FileExists(string pchFile)
        {
            return SteamEmulator.SteamRemoteStorage.FileExists(pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamRemoteStorage_FilePersisted(string pchFile)
        {
            return SteamEmulator.SteamRemoteStorage.FilePersisted(pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamRemoteStorage_GetFileSize(string pchFile)
        {
            return SteamEmulator.SteamRemoteStorage.GetFileSize(pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static long ISteamRemoteStorage_GetFileTimestamp(string pchFile)
        {
            return SteamEmulator.SteamRemoteStorage.GetFileTimestamp(pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ERemoteStoragePlatform ISteamRemoteStorage_GetSyncPlatforms(string pchFile)
        {
            return (ERemoteStoragePlatform)SteamEmulator.SteamRemoteStorage.GetSyncPlatforms(pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamRemoteStorage_GetFileCount()
        {
            return SteamEmulator.SteamRemoteStorage.GetFileCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string ISteamRemoteStorage_GetFileNameAndSize(int iFile, ref int pnFileSizeInBytes)
        {
            return SteamEmulator.SteamRemoteStorage.GetFileNameAndSize(iFile, ref pnFileSizeInBytes);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamRemoteStorage_GetQuota(ref ulong pnTotalBytes, ref ulong puAvailableBytes)
        {
            return SteamEmulator.SteamRemoteStorage.GetQuota(ref pnTotalBytes, ref puAvailableBytes);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamRemoteStorage_IsCloudEnabledForAccount()
        {
            return SteamEmulator.SteamRemoteStorage.IsCloudEnabledForAccount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamRemoteStorage_IsCloudEnabledForApp()
        {
            return SteamEmulator.SteamRemoteStorage.IsCloudEnabledForApp();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamRemoteStorage_SetCloudEnabledForApp(bool bEnabled)
        {
            SteamEmulator.SteamRemoteStorage.SetCloudEnabledForApp(bEnabled);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamRemoteStorage_UGCDownload(UGCHandle_t hContent, uint unPriority)
        {
            return SteamEmulator.SteamRemoteStorage.UGCDownload(hContent, unPriority);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamRemoteStorage_GetUGCDownloadProgress(UGCHandle_t hContent, ref int pnBytesDownloaded, ref int pnBytesExpected)
        {
            return SteamEmulator.SteamRemoteStorage.GetUGCDownloadProgress(hContent, pnBytesDownloaded, pnBytesExpected);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamRemoteStorage_GetUGCDetails(UGCHandle_t hContent, ref AppId_t pnAppID, ref string ppchName, ref int pnFileSizeInBytes, ref ulong pSteamIDOwner)
        {
            return SteamEmulator.SteamRemoteStorage.GetUGCDetails(hContent, pnAppID, ppchName, pnFileSizeInBytes, (ulong)pSteamIDOwner);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamRemoteStorage_UGCRead(UGCHandle_t hContent, IntPtr pvData, int cubDataToRead, uint cOffset, EUGCReadAction eAction)
        //public static int ISteamRemoteStorage_UGCRead(UGCHandle_t hContent, ref byte[] pvData, int cubDataToRead, uint cOffset, EUGCReadAction eAction)
        {
            return SteamEmulator.SteamRemoteStorage.UGCRead(hContent, pvData, cubDataToRead, cOffset, (int)eAction);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamRemoteStorage_GetCachedUGCCount()
        {
            return SteamEmulator.SteamRemoteStorage.GetCachedUGCCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamRemoteStorage_GetCachedUGCHandle(int iCachedContent)
        {
            return SteamEmulator.SteamRemoteStorage.GetCachedUGCHandle(iCachedContent);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamRemoteStorage_PublishWorkshopFile(string pchFile, string pchPreviewFile, AppId_t nConsumerAppId, string pchTitle, string pchDescription, ERemoteStoragePublishedFileVisibility eVisibility, IntPtr pTags, EWorkshopFileType eWorkshopFileType)
        {
            return SteamEmulator.SteamRemoteStorage.PublishWorkshopFile(pchFile, pchPreviewFile, nConsumerAppId, pchTitle, pchDescription, (int)eVisibility, pTags, (int)eWorkshopFileType);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamRemoteStorage_CreatePublishedFileUpdateRequest(PublishedFileId_t unPublishedFileId)
        {
            return SteamEmulator.SteamRemoteStorage.CreatePublishedFileUpdateRequest(unPublishedFileId);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamRemoteStorage_UpdatePublishedFileFile(PublishedFileUpdateHandle_t updateHandle, string pchFile)
        {
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileFile(updateHandle, pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamRemoteStorage_UpdatePublishedFilePreviewFile(PublishedFileUpdateHandle_t updateHandle, string pchPreviewFile)
        {
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFilePreviewFile(updateHandle, pchPreviewFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamRemoteStorage_UpdatePublishedFileTitle(PublishedFileUpdateHandle_t updateHandle, string pchTitle)
        {
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileTitle(updateHandle, pchTitle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamRemoteStorage_UpdatePublishedFileDescription(PublishedFileUpdateHandle_t updateHandle, string pchDescription)
        {
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileDescription(updateHandle, pchDescription);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamRemoteStorage_UpdatePublishedFileVisibility(PublishedFileUpdateHandle_t updateHandle, ERemoteStoragePublishedFileVisibility eVisibility)
        {
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileVisibility(updateHandle, (int)eVisibility);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamRemoteStorage_UpdatePublishedFileTags(PublishedFileUpdateHandle_t updateHandle, IntPtr pTags)
        {
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileTags(updateHandle, pTags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamRemoteStorage_CommitPublishedFileUpdate(PublishedFileUpdateHandle_t updateHandle)
        {
            return SteamEmulator.SteamRemoteStorage.CommitPublishedFileUpdate(updateHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamRemoteStorage_GetPublishedFileDetails(PublishedFileId_t unPublishedFileId, uint unMaxSecondsOld)
        {
            return SteamEmulator.SteamRemoteStorage.GetPublishedFileDetails(unPublishedFileId, unMaxSecondsOld);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamRemoteStorage_DeletePublishedFile(PublishedFileId_t unPublishedFileId)
        {
            return SteamEmulator.SteamRemoteStorage.DeletePublishedFile(unPublishedFileId);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamRemoteStorage_EnumerateUserPublishedFiles(uint unStartIndex)
        {
            return SteamEmulator.SteamRemoteStorage.EnumerateUserPublishedFiles(unStartIndex);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamRemoteStorage_SubscribePublishedFile(PublishedFileId_t unPublishedFileId)
        {
            return SteamEmulator.SteamRemoteStorage.SubscribePublishedFile(unPublishedFileId);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamRemoteStorage_EnumerateUserSubscribedFiles(uint unStartIndex)
        {
            return SteamEmulator.SteamRemoteStorage.EnumerateUserSubscribedFiles(unStartIndex);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamRemoteStorage_UnsubscribePublishedFile(PublishedFileId_t unPublishedFileId)
        {
            return SteamEmulator.SteamRemoteStorage.UnsubscribePublishedFile(unPublishedFileId);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamRemoteStorage_UpdatePublishedFileSetChangeDescription(PublishedFileUpdateHandle_t updateHandle, string pchChangeDescription)
        {
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileSetChangeDescription(updateHandle, pchChangeDescription);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamRemoteStorage_GetPublishedItemVoteDetails(PublishedFileId_t unPublishedFileId)
        {
            return SteamEmulator.SteamRemoteStorage.GetPublishedItemVoteDetails(unPublishedFileId);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamRemoteStorage_UpdateUserPublishedItemVote(PublishedFileId_t unPublishedFileId, bool bVoteUp)
        {
            return SteamEmulator.SteamRemoteStorage.UpdateUserPublishedItemVote(unPublishedFileId, bVoteUp);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamRemoteStorage_GetUserPublishedItemVoteDetails(PublishedFileId_t unPublishedFileId)
        {
            return SteamEmulator.SteamRemoteStorage.GetUserPublishedItemVoteDetails(unPublishedFileId);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamRemoteStorage_EnumerateUserSharedWorkshopFiles(ulong steamId, uint unStartIndex, IntPtr pRequiredTags, IntPtr pExcludedTags)
        {
            return SteamEmulator.SteamRemoteStorage.EnumerateUserSharedWorkshopFiles((ulong)steamId, unStartIndex, pRequiredTags, pExcludedTags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamRemoteStorage_PublishVideo(EWorkshopVideoProvider eVideoProvider, string pchVideoAccount, string pchVideoIdentifier, string pchPreviewFile, AppId_t nConsumerAppId, string pchTitle, string pchDescription, ERemoteStoragePublishedFileVisibility eVisibility, IntPtr pTags)
        {
            return SteamEmulator.SteamRemoteStorage.PublishVideo((int)eVideoProvider, pchVideoAccount, pchVideoIdentifier, pchPreviewFile, nConsumerAppId, pchTitle, pchDescription, (int)eVisibility, pTags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamRemoteStorage_SetUserPublishedFileAction(PublishedFileId_t unPublishedFileId, EWorkshopFileAction eAction)
        {
            return SteamEmulator.SteamRemoteStorage.SetUserPublishedFileAction(unPublishedFileId, (int)eAction);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamRemoteStorage_EnumeratePublishedFilesByUserAction(EWorkshopFileAction eAction, uint unStartIndex)
        {
            return SteamEmulator.SteamRemoteStorage.EnumeratePublishedFilesByUserAction((int)eAction, unStartIndex);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamRemoteStorage_EnumeratePublishedWorkshopFiles(EWorkshopEnumerationType eEnumerationType, uint unStartIndex, uint unCount, uint unDays, IntPtr pTags, IntPtr pUserTags)
        {
            return SteamEmulator.SteamRemoteStorage.EnumeratePublishedWorkshopFiles((int)eEnumerationType, unStartIndex, unCount, unDays, pTags, pUserTags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamRemoteStorage_UGCDownloadToLocation(UGCHandle_t hContent, string pchLocation, uint unPriority)
        {
            return SteamEmulator.SteamRemoteStorage.UGCDownloadToLocation(hContent, pchLocation, unPriority);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamScreenshots_WriteScreenshot(IntPtr pubRGB, uint cubRGB, int nWidth, int nHeight)
        //public static uint ISteamScreenshots_WriteScreenshot(ref byte[] pubRGB, uint cubRGB, int nWidth, int nHeight)
        {
            return SteamEmulator.SteamScreenshots.WriteScreenshot(pubRGB, cubRGB, nWidth, nHeight);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamScreenshots_AddScreenshotToLibrary(string pchFilename, string pchThumbnailFilename, int nWidth, int nHeight)
        {
            return SteamEmulator.SteamScreenshots.AddScreenshotToLibrary(pchFilename, pchThumbnailFilename, nWidth, nHeight);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamScreenshots_TriggerScreenshot()
        {
            SteamEmulator.SteamScreenshots.TriggerScreenshot();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamScreenshots_HookScreenshots(bool bHook)
        {
            SteamEmulator.SteamScreenshots.HookScreenshots(bHook);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamScreenshots_SetLocation(ScreenshotHandle hScreenshot, string pchLocation)
        {
            return SteamEmulator.SteamScreenshots.SetLocation(hScreenshot, pchLocation);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamScreenshots_TagUser(ScreenshotHandle hScreenshot, ulong steamID)
        {
            return SteamEmulator.SteamScreenshots.TagUser(hScreenshot, (ulong)steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamScreenshots_TagPublishedFile(ScreenshotHandle hScreenshot, uint unPublishedFileID)
        {
            return SteamEmulator.SteamScreenshots.TagPublishedFile(hScreenshot, unPublishedFileID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUGC_CreateQueryUserUGCRequest(AccountID_t unAccountID, EUserUGCList eListType, EUGCMatchingUGCType eMatchingUGCType, EUserUGCListSortOrder eSortOrder, AppId_t nCreatorAppID, AppId_t nConsumerAppID, uint unPage)
        {
            return SteamEmulator.SteamUGC.CreateQueryUserUGCRequest(unAccountID, (int)eListType, (int)eMatchingUGCType, (int)eSortOrder, nCreatorAppID, nConsumerAppID, unPage);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUGC_CreateQueryAllUGCRequest(EUGCQuery eQueryType, EUGCMatchingUGCType eMatchingeMatchingUGCTypeFileType, AppId_t nCreatorAppID, AppId_t nConsumerAppID, uint unPage)
        {
            return SteamEmulator.SteamUGC.CreateQueryAllUGCRequest((int)eQueryType, (int)eMatchingeMatchingUGCTypeFileType, nCreatorAppID, nConsumerAppID, unPage);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUGC_CreateQueryUGCDetailsRequest(PublishedFileId_t pvecPublishedFileID, uint unNumPublishedFileIDs)
        //public static ulong ISteamUGC_CreateQueryUGCDetailsRequest(ref PublishedFileId_t[] pvecPublishedFileID, uint unNumPublishedFileIDs)
        {
            return SteamEmulator.SteamUGC.CreateQueryUGCDetailsRequest(pvecPublishedFileID, unNumPublishedFileIDs);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUGC_SendQueryUGCRequest(UGCQueryHandle_t handle)
        {
            return SteamEmulator.SteamUGC.SendQueryUGCRequest(handle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_GetQueryUGCResult(UGCQueryHandle_t handle, uint index, ref SteamUGCDetails_t pDetails)
        {
            return SteamEmulator.SteamUGC.GetQueryUGCResult(handle, index, ref pDetails);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_GetQueryUGCPreviewURL(UGCQueryHandle_t handle, uint index, string pchURL, uint cchURLSize)
        {
            return SteamEmulator.SteamUGC.GetQueryUGCPreviewURL(handle, index, pchURL, cchURLSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_GetQueryUGCMetadata(UGCQueryHandle_t handle, uint index, string pchMetadata, uint cchMetadatasize)
        {
            return SteamEmulator.SteamUGC.GetQueryUGCMetadata(handle, index, pchMetadata, cchMetadatasize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_GetQueryUGCChildren(UGCQueryHandle_t handle, uint index, ref PublishedFileId_t[] pvecPublishedFileID, uint cMaxEntries)
        {
            return SteamEmulator.SteamUGC.GetQueryUGCChildren(handle, index, ref pvecPublishedFileID, cMaxEntries);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_GetQueryUGCStatistic(UGCQueryHandle_t handle, uint index, EItemStatistic eStatType, ref uint pStatValue)
        {
            return SteamEmulator.SteamUGC.GetQueryUGCStatistic(handle, index, (int)eStatType, pStatValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamUGC_GetQueryUGCNumAdditionalPreviews(UGCQueryHandle_t handle, uint index)
        {
            return SteamEmulator.SteamUGC.GetQueryUGCNumAdditionalPreviews(handle, index);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_GetQueryUGCAdditionalPreview(UGCQueryHandle_t handle, uint index, uint previewIndex, string pchURLOrVideoID, uint cchURLSize, ref bool pbIsImage)
        {
            return SteamEmulator.SteamUGC.GetQueryUGCAdditionalPreview(handle, index, previewIndex, pchURLOrVideoID, cchURLSize, "", 0, 0);                                            // OJO
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_ReleaseQueryUGCRequest(UGCQueryHandle_t handle)
        {
            return SteamEmulator.SteamUGC.ReleaseQueryUGCRequest(handle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_AddRequiredTag(UGCQueryHandle_t handle, string pTagName)
        {
            return SteamEmulator.SteamUGC.AddRequiredTag(handle, pTagName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_AddExcludedTag(UGCQueryHandle_t handle, string pTagName)
        {
            return SteamEmulator.SteamUGC.AddExcludedTag(handle, pTagName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_SetReturnLongDescription(UGCQueryHandle_t handle, bool bReturnLongDescription)
        {
            return SteamEmulator.SteamUGC.SetReturnLongDescription(handle, bReturnLongDescription);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_SetReturnMetadata(UGCQueryHandle_t handle, bool bReturnMetadata)
        {
            return SteamEmulator.SteamUGC.SetReturnMetadata(handle, bReturnMetadata);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_SetReturnChildren(UGCQueryHandle_t handle, bool bReturnChildren)
        {
            return SteamEmulator.SteamUGC.SetReturnChildren(handle, bReturnChildren);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_SetReturnAdditionalPreviews(UGCQueryHandle_t handle, bool bReturnAdditionalPreviews)
        {
            return SteamEmulator.SteamUGC.SetReturnAdditionalPreviews(handle, bReturnAdditionalPreviews);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_SetReturnTotalOnly(UGCQueryHandle_t handle, bool bReturnTotalOnly)
        {
            return SteamEmulator.SteamUGC.SetReturnTotalOnly(handle, bReturnTotalOnly);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_SetAllowCachedResponse(UGCQueryHandle_t handle, uint unMaxAgeSeconds)
        {
            return SteamEmulator.SteamUGC.SetAllowCachedResponse(handle, unMaxAgeSeconds);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_SetCloudFileNameFilter(UGCQueryHandle_t handle, string pMatchCloudFileName)
        {
            return SteamEmulator.SteamUGC.SetCloudFileNameFilter(handle, pMatchCloudFileName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_SetMatchAnyTag(UGCQueryHandle_t handle, bool bMatchAnyTag)
        {
            return SteamEmulator.SteamUGC.SetMatchAnyTag(handle, bMatchAnyTag);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_SetSearchText(UGCQueryHandle_t handle, string pSearchText)
        {
            return SteamEmulator.SteamUGC.SetSearchText(handle, pSearchText);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_SetRankedByTrendDays(UGCQueryHandle_t handle, uint unDays)
        {
            return SteamEmulator.SteamUGC.SetRankedByTrendDays(handle, unDays);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUGC_RequestUGCDetails(PublishedFileId_t nPublishedFileID, uint unMaxAgeSeconds)
        {
            return SteamEmulator.SteamUGC.RequestUGCDetails(nPublishedFileID, unMaxAgeSeconds);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUGC_CreateItem(AppId_t nConsumerAppId, EWorkshopFileType eFileType)
        {
            return SteamEmulator.SteamUGC.CreateItem(nConsumerAppId, (int)eFileType);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUGC_StartItemUpdate(AppId_t nConsumerAppId, PublishedFileId_t nPublishedFileID)
        {
            return SteamEmulator.SteamUGC.StartItemUpdate(nConsumerAppId, nPublishedFileID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_SetItemTitle(UGCUpdateHandle_t handle, string pchTitle)
        {
            return SteamEmulator.SteamUGC.SetItemTitle(handle, pchTitle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_SetItemDescription(UGCUpdateHandle_t handle, string pchDescription)
        {
            return SteamEmulator.SteamUGC.SetItemDescription(handle, pchDescription);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_SetItemMetadata(UGCUpdateHandle_t handle, string pchMetaData)
        {
            return SteamEmulator.SteamUGC.SetItemMetadata(handle, pchMetaData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_SetItemVisibility(UGCUpdateHandle_t handle, ERemoteStoragePublishedFileVisibility eVisibility)
        {
            return SteamEmulator.SteamUGC.SetItemVisibility(handle, (int)eVisibility);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_SetItemTags(UGCUpdateHandle_t updateHandle, IntPtr pTags)
        {
            return SteamEmulator.SteamUGC.SetItemTags(updateHandle, pTags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_SetItemContent(UGCUpdateHandle_t handle, string pszContentFolder)
        {
            return SteamEmulator.SteamUGC.SetItemContent(handle, pszContentFolder);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_SetItemPreview(UGCUpdateHandle_t handle, string pszPreviewFile)
        {
            return SteamEmulator.SteamUGC.SetItemPreview(handle, pszPreviewFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUGC_SubmitItemUpdate(UGCUpdateHandle_t handle, string pchChangeNote)
        {
            return SteamEmulator.SteamUGC.SubmitItemUpdate(handle, pchChangeNote);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EItemUpdateStatus ISteamUGC_GetItemUpdateProgress(UGCUpdateHandle_t handle, ref ulong punBytesProcessed, ref ulong punBytesTotal)
        {
            return (EItemUpdateStatus)SteamEmulator.SteamUGC.GetItemUpdateProgress(handle, (uint)punBytesProcessed, (uint)punBytesTotal);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUGC_AddItemToFavorites(AppId_t nAppId, PublishedFileId_t nPublishedFileID)
        {
            return SteamEmulator.SteamUGC.AddItemToFavorites(nAppId, nPublishedFileID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUGC_RemoveItemFromFavorites(AppId_t nAppId, PublishedFileId_t nPublishedFileID)
        {
            return SteamEmulator.SteamUGC.RemoveItemFromFavorites(nAppId, nPublishedFileID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUGC_SubscribeItem(PublishedFileId_t nPublishedFileID)
        {
            return SteamEmulator.SteamUGC.SubscribeItem(nPublishedFileID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUGC_UnsubscribeItem(PublishedFileId_t nPublishedFileID)
        {
            return SteamEmulator.SteamUGC.UnsubscribeItem(nPublishedFileID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamUGC_GetNumSubscribedItems()
        {
            return SteamEmulator.SteamUGC.GetNumSubscribedItems();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamUGC_GetSubscribedItems(PublishedFileId_t pvecPublishedFileID, uint cMaxEntries)
        //public static uint ISteamUGC_GetSubscribedItems(ref PublishedFileId_t[] pvecPublishedFileID, uint cMaxEntries)
        {
            return SteamEmulator.SteamUGC.GetSubscribedItems(pvecPublishedFileID, cMaxEntries);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamUGC_GetItemState(PublishedFileId_t nPublishedFileID)
        {
            return SteamEmulator.SteamUGC.GetItemState(nPublishedFileID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_GetItemInstallInfo(PublishedFileId_t nPublishedFileID, ref ulong punSizeOnDisk, string pchFolder, uint cchFolderSize, ref uint punTimeStamp)
        {
            return SteamEmulator.SteamUGC.GetItemInstallInfo(nPublishedFileID, (uint)punSizeOnDisk, pchFolder, cchFolderSize, punTimeStamp);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_GetItemDownloadInfo(PublishedFileId_t nPublishedFileID, ref ulong punBytesDownloaded, ref ulong punBytesTotal)
        {
            return SteamEmulator.SteamUGC.GetItemDownloadInfo(nPublishedFileID, (uint)punBytesDownloaded, (uint)punBytesTotal);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUGC_DownloadItem(PublishedFileId_t nPublishedFileID, bool bHighPriority)
        {
            return SteamEmulator.SteamUGC.DownloadItem(nPublishedFileID, bHighPriority);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        //public static ulong ISteamUnifiedMessages_SendMethod(string pchServiceMethod, ref byte[] pRequestBuffer, uint unRequestBufferSize, ulong unContext)
        public static ulong ISteamUnifiedMessages_SendMethod(string pchServiceMethod, ref byte[] pRequestBuffer, uint unRequestBufferSize, ulong unContext)
        {
            //return SteamAPI_ISteamUnifiedMessages_SendMethod(pchServiceMethod, xxx, xxx);
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUnifiedMessages_GetMethodResponseInfo(ClientUnifiedMessageHandle hHandle, ref uint punResponseSize, ref EResult peResult)
        {
            //return SteamAPI_ISteamUnifiedMessages_GetMethodResponseInfo(xxx, xxx, xxx);
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUnifiedMessages_GetMethodResponseData(ClientUnifiedMessageHandle hHandle, ref byte[] pResponseBuffer, uint unResponseBufferSize, bool bAutoRelease)
        {
            //return SteamAPI_ISteamUnifiedMessages_GetMethodResponseData(xxx, xxx, xxx);
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUnifiedMessages_ReleaseMethod(ClientUnifiedMessageHandle hHandle)
        {
            //return SteamAPI_ISteamUnifiedMessages_ReleaseMethod(xxx, xxx, xxx);
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUnifiedMessages_SendNotification(string pchServiceNotification, ref byte[] pNotificationBuffer, uint unNotificationBufferSize)
        {
            //return SteamAPI_ISteamUnifiedMessages_SendNotification(xxx, xxx, xxx);
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HSteamUser ISteamUser_GetHSteamUser()
        {
            return SteamEmulator.SteamUser.GetHSteamUser();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUser_BLoggedOn()
        {
            return SteamEmulator.SteamUser.BLoggedOn();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUser_GetSteamID()
        {
            return (ulong)SteamEmulator.SteamUser.GetSteamID();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamUser_InitiateGameConnection(IntPtr pAuthBlob, int cbMaxAuthBlob, ulong steamIDGameServer, CGameID gameID, uint unIPServer, ushort usPortServer, bool bSecure)
        //public static int ISteamUser_InitiateGameConnection(ref byte[] pAuthBlob, int cbMaxAuthBlob, ulong steamIDGameServer, uint unIPServer, ushort usPortServer, bool bSecure)
        {
            return SteamEmulator.SteamUser.InitiateGameConnection(pAuthBlob, cbMaxAuthBlob, (ulong)steamIDGameServer, gameID, unIPServer, usPortServer, bSecure);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamUser_TerminateGameConnection(uint unIPServer, ushort usPortServer)
        {
            SteamEmulator.SteamUser.TerminateGameConnection(unIPServer, usPortServer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamUser_TrackAppUsageEvent(CGameID gameID, int eAppUsageEvent, string pchExtraInfo)
        {
            SteamEmulator.SteamUser.TrackAppUsageEvent(gameID, eAppUsageEvent, pchExtraInfo);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUser_GetUserDataFolder(ref string pchBuffer, int cubBuffer)
        {
            return SteamEmulator.SteamUser.GetUserDataFolder(ref pchBuffer, cubBuffer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamUser_StartVoiceRecording()
        {
            SteamEmulator.SteamUser.StartVoiceRecording();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamUser_StopVoiceRecording()
        {
            SteamEmulator.SteamUser.StopVoiceRecording();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EVoiceResult ISteamUser_GetAvailableVoice(ref uint pcbCompressed, ref uint pcbUncompressed, uint nUncompressedVoiceDesiredSampleRate)
        {
            return (EVoiceResult)SteamEmulator.SteamUser.GetAvailableVoice(ref pcbCompressed, ref pcbUncompressed, nUncompressedVoiceDesiredSampleRate);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EVoiceResult ISteamUser_GetVoice(bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, ref uint nBytesWritten, bool bWantUncompressed, IntPtr pUncompressedDestBuffer, uint cbUncompressedDestBufferSize, ref uint nUncompressBytesWritten, uint nUncompressedVoiceDesiredSampleRate)
        //public static EVoiceResult ISteamUser_GetVoice(bool bWantCompressed, ref byte[] pDestBuffer, uint cbDestBufferSize, ref uint nBytesWritten, bool bWantUncompressed, ref byte[] pUncompressedDestBuffer, uint cbUncompressedDestBufferSize, ref uint nUncompressBytesWritten, uint nUncompressedVoiceDesiredSampleRate)
        {
            return (EVoiceResult)SteamEmulator.SteamUser.GetVoice(bWantCompressed, pDestBuffer, cbDestBufferSize, ref nBytesWritten, bWantUncompressed, pUncompressedDestBuffer, cbUncompressedDestBufferSize, ref nUncompressBytesWritten, nUncompressedVoiceDesiredSampleRate);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EVoiceResult ISteamUser_DecompressVoice(IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, ref uint nBytesWritten, uint nDesiredSampleRate)
        //public static EVoiceResult ISteamUser_DecompressVoice(ref byte[] pCompressed, uint cbCompressed, ref byte[] pDestBuffer, uint cbDestBufferSize, ref uint nBytesWritten, uint nDesiredSampleRate)
        {
            return (EVoiceResult)SteamEmulator.SteamUser.DecompressVoice(pCompressed, cbCompressed, pDestBuffer, cbDestBufferSize, ref nBytesWritten, nDesiredSampleRate);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamUser_GetVoiceOptimalSampleRate()
        {
            return SteamEmulator.SteamUser.GetVoiceOptimalSampleRate();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamUser_GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
        {
            return SteamEmulator.SteamUser.GetAuthSessionTicket(pTicket, cbMaxTicket, ref pcbTicket);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EBeginAuthSessionResult ISteamUser_BeginAuthSession(IntPtr pAuthTicket, int cbAuthTicket, ulong steamID)
        {
            return (EBeginAuthSessionResult)SteamEmulator.SteamUser.BeginAuthSession(pAuthTicket, cbAuthTicket, (ulong)steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamUser_EndAuthSession(ulong steamID)
        {
            SteamEmulator.SteamUser.EndAuthSession((ulong)steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamUser_CancelAuthTicket(HAuthTicket hAuthTicket)
        {
            SteamEmulator.SteamUser.CancelAuthTicket(hAuthTicket);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EUserHasLicenseForAppResult ISteamUser_UserHasLicenseForApp(ulong steamID, AppId_t appID)
        {
            return (EUserHasLicenseForAppResult)SteamEmulator.SteamUser.UserHasLicenseForApp((ulong)steamID, appID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUser_BIsBehindNAT()
        {
            return SteamEmulator.SteamUser.BIsBehindNAT();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamUser_AdvertiseGame(ulong steamIDGameServer, uint unIPServer, ushort usPortServer)
        {
            SteamEmulator.SteamUser.AdvertiseGame((ulong)steamIDGameServer, unIPServer, usPortServer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUser_RequestEncryptedAppTicket(IntPtr pDataToInclude, int cbDataToInclude)
        //public static ulong ISteamUser_RequestEncryptedAppTicket(ref byte[] pDataToInclude, int cbDataToInclude)
        {
            return SteamEmulator.SteamUser.RequestEncryptedAppTicket(pDataToInclude, cbDataToInclude);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUser_GetEncryptedAppTicket(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
        //public static bool ISteamUser_GetEncryptedAppTicket(ref byte[] pTicket, int cbMaxTicket, ref uint pcbTicket)
        {
            return SteamEmulator.SteamUser.GetEncryptedAppTicket(pTicket, cbMaxTicket, pcbTicket);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamUser_GetGameBadgeLevel(int nSeries, bool bFoil)
        {
            return SteamEmulator.SteamUser.GetGameBadgeLevel(nSeries, bFoil);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamUser_GetPlayerSteamLevel()
        {
            return SteamEmulator.SteamUser.GetPlayerSteamLevel();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUser_RequestStoreAuthURL(string pchRedirectURL)
        {
            return SteamEmulator.SteamUser.RequestStoreAuthURL(pchRedirectURL);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUserStats_RequestCurrentStats()
        {
            return SteamEmulator.SteamUserStats.RequestCurrentStats();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUserStats_GetStat(string pchName, ref uint pData)
        {
            return SteamEmulator.SteamUserStats.GetStat(pchName, ref pData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUserStats_GetStat_(string pchName, ref float pData)
        {
            uint fResult = (uint)pData;
            bool result = SteamEmulator.SteamUserStats.GetStat(pchName, ref fResult);
            pData = (float)fResult;
            return result;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUserStats_SetStat(string pchName, uint nData)
        {
            return SteamEmulator.SteamUserStats.SetStat(pchName, nData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUserStats_SetStat_(string pchName, float fData)
        {
            return SteamEmulator.SteamUserStats.SetStat(pchName, (uint)fData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUserStats_UpdateAvgRateStat(string pchName, float flCountThisSession, double dSessionLength)
        {
            return SteamEmulator.SteamUserStats.UpdateAvgRateStat(pchName, (uint)flCountThisSession, dSessionLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUserStats_GetAchievement(string pchName, ref bool pbAchieved)
        {
            return SteamEmulator.SteamUserStats.GetAchievement(pchName, ref pbAchieved);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUserStats_SetAchievement(string pchName)
        {
            return SteamEmulator.SteamUserStats.SetAchievement(pchName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUserStats_ClearAchievement(string pchName)
        {
            return SteamEmulator.SteamUserStats.ClearAchievement(pchName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUserStats_GetAchievementAndUnlockTime(string pchName, ref bool pbAchieved, ref uint punUnlockTime)
        {
            return SteamEmulator.SteamUserStats.GetAchievementAndUnlockTime(pchName, ref pbAchieved, ref punUnlockTime);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUserStats_StoreStats()
        {
            return SteamEmulator.SteamUserStats.StoreStats();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamUserStats_GetAchievementIcon(string pchName)
        {
            return SteamEmulator.SteamUserStats.GetAchievementIcon(pchName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string ISteamUserStats_GetAchievementDisplayAttribute(string pchName, string pchKey)
        {
            return SteamEmulator.SteamUserStats.GetAchievementDisplayAttribute(pchName, pchKey);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUserStats_IndicateAchievementProgress(string pchName, uint nCurProgress, uint nMaxProgress)
        {
            return SteamEmulator.SteamUserStats.IndicateAchievementProgress(pchName, nCurProgress, nMaxProgress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamUserStats_GetNumAchievements()
        {
            return SteamEmulator.SteamUserStats.GetNumAchievements();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string ISteamUserStats_GetAchievementName(uint iAchievement)
        {
            return SteamEmulator.SteamUserStats.GetAchievementName(iAchievement);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUserStats_RequestUserStats(ulong steamIDUser)
        {
            return SteamEmulator.SteamUserStats.RequestUserStats((ulong)steamIDUser);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUserStats_GetUserStat(ulong steamIDUser, string pchName, uint pData)
        {
            return SteamEmulator.SteamUserStats.GetUserStat((ulong)steamIDUser, pchName, pData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUserStats_GetUserStat_(ulong steamIDUser, string pchName, ref float pData)
        {
            return SteamEmulator.SteamUserStats.GetUserStat((ulong)steamIDUser, pchName, (uint)pData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUserStats_GetUserAchievement(ulong steamIDUser, string pchName, ref bool pbAchieved)
        {
            return SteamEmulator.SteamUserStats.GetUserAchievement((ulong)steamIDUser, pchName, pbAchieved);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUserStats_GetUserAchievementAndUnlockTime(ulong steamIDUser, string pchName, ref bool pbAchieved, ref uint punUnlockTime)
        {
            return SteamEmulator.SteamUserStats.GetUserAchievementAndUnlockTime((ulong)steamIDUser, pchName, pbAchieved, punUnlockTime);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUserStats_ResetAllStats(bool bAchievementsToo)
        {
            return SteamEmulator.SteamUserStats.ResetAllStats(bAchievementsToo);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUserStats_FindOrCreateLeaderboard(string pchLeaderboardName, ELeaderboardSortMethod eLeaderboardSortMethod, ELeaderboardDisplayType eLeaderboardDisplayType)
        {
            return SteamEmulator.SteamUserStats.FindOrCreateLeaderboard(pchLeaderboardName, eLeaderboardSortMethod, eLeaderboardDisplayType);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUserStats_FindLeaderboard(string pchLeaderboardName)
        {
            return SteamEmulator.SteamUserStats.FindLeaderboard(pchLeaderboardName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string ISteamUserStats_GetLeaderboardName(SteamLeaderboard_t hSteamLeaderboard)
        {
            return SteamEmulator.SteamUserStats.GetLeaderboardName(hSteamLeaderboard);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamUserStats_GetLeaderboardEntryCount(SteamLeaderboard_t hSteamLeaderboard)
        {
            return SteamEmulator.SteamUserStats.GetLeaderboardEntryCount(hSteamLeaderboard);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ELeaderboardSortMethod ISteamUserStats_GetLeaderboardSortMethod(SteamLeaderboard_t hSteamLeaderboard)
        {
            return (ELeaderboardSortMethod)SteamEmulator.SteamUserStats.GetLeaderboardSortMethod(hSteamLeaderboard);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ELeaderboardDisplayType ISteamUserStats_GetLeaderboardDisplayType(SteamLeaderboard_t hSteamLeaderboard)
        {
            return (ELeaderboardDisplayType)SteamEmulator.SteamUserStats.GetLeaderboardDisplayType(hSteamLeaderboard);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUserStats_DownloadLeaderboardEntries(SteamLeaderboard_t hSteamLeaderboard, ELeaderboardDataRequest eLeaderboardDataRequest, int nRangeStart, int nRangeEnd)
        {
            return SteamEmulator.SteamUserStats.DownloadLeaderboardEntries(hSteamLeaderboard, (int)eLeaderboardDataRequest, nRangeStart, nRangeEnd);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUserStats_DownloadLeaderboardEntriesForUsers(SteamLeaderboard_t hSteamLeaderboard, IntPtr prgUsers, int cUsers)
        //public static ulong ISteamUserStats_DownloadLeaderboardEntriesForUsers(SteamLeaderboard_t hSteamLeaderboard, ref ulong[] prgUsers, int cUsers)
        {
            return SteamEmulator.SteamUserStats.DownloadLeaderboardEntriesForUsers(hSteamLeaderboard, prgUsers, cUsers);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUserStats_GetDownloadedLeaderboardEntry(SteamLeaderboardEntries_t hSteamLeaderboardEntries, int index, ref IntPtr pLeaderboardEntry, IntPtr pDetails, int cDetailsMax)
        //public static bool ISteamUserStats_GetDownloadedLeaderboardEntry(SteamLeaderboardEntries_t hSteamLeaderboardEntries, int index, ref IntPtr pLeaderboardEntry, ref int[] pDetails, int cDetailsMax)
        {
            return SteamEmulator.SteamUserStats.GetDownloadedLeaderboardEntry(hSteamLeaderboardEntries, index, pLeaderboardEntry, pDetails, cDetailsMax);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUserStats_UploadLeaderboardScore(SteamLeaderboard_t hSteamLeaderboard, ELeaderboardUploadScoreMethod eLeaderboardUploadScoreMethod, uint nScore, IntPtr pScoreDetails, int cScoreDetailsCount)
        //public static ulong ISteamUserStats_UploadLeaderboardScore(SteamLeaderboard_t hSteamLeaderboard, ELeaderboardUploadScoreMethod eLeaderboardUploadScoreMethod, int nScore, ref int[] pScoreDetails, int cScoreDetailsCount)
        {
            return SteamEmulator.SteamUserStats.UploadLeaderboardScore(hSteamLeaderboard, (int)eLeaderboardUploadScoreMethod, nScore, pScoreDetails, cScoreDetailsCount);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUserStats_AttachLeaderboardUGC(SteamLeaderboard_t hSteamLeaderboard, UGCHandle_t hUGC)
        {
            return SteamEmulator.SteamUserStats.AttachLeaderboardUGC(hSteamLeaderboard, hUGC);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUserStats_GetNumberOfCurrentPlayers()
        {
            return SteamEmulator.SteamUserStats.GetNumberOfCurrentPlayers();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUserStats_RequestGlobalAchievementPercentages()
        {
            return SteamEmulator.SteamUserStats.RequestGlobalAchievementPercentages();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamUserStats_GetMostAchievedAchievementInfo(string pchName, uint unNameBufLen, ref float pflPercent, ref bool pbAchieved)
        {
            return SteamEmulator.SteamUserStats.GetMostAchievedAchievementInfo(pchName, unNameBufLen, (uint)pflPercent, pbAchieved);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamUserStats_GetNextMostAchievedAchievementInfo(int iIteratorPrevious, string pchName, uint unNameBufLen, ref float pflPercent, ref bool pbAchieved)
        {
            return SteamEmulator.SteamUserStats.GetNextMostAchievedAchievementInfo(iIteratorPrevious, pchName, unNameBufLen, (uint)pflPercent, pbAchieved);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUserStats_GetAchievementAchievedPercent(string pchName, ref float pflPercent)
        {
            return SteamEmulator.SteamUserStats.GetAchievementAchievedPercent(pchName, (uint)pflPercent);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUserStats_RequestGlobalStats(int nHistoryDays)
        {
            return SteamEmulator.SteamUserStats.RequestGlobalStats(nHistoryDays);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUserStats_GetGlobalStat(string pchStatName, ref long pData)
        {
            return SteamEmulator.SteamUserStats.GetGlobalStat(pchStatName, (uint)pData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUserStats_GetGlobalStat_(string pchStatName, ref double pData)
        {
            return SteamEmulator.SteamUserStats.GetGlobalStat(pchStatName, (uint)pData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamUserStats_GetGlobalStatHistory(string pchStatName, uint pData, uint cubData)
        //public static int ISteamUserStats_GetGlobalStatHistory(string pchStatName, ref long[] pData, uint cubData)
        {
            return SteamEmulator.SteamUserStats.GetGlobalStatHistory(pchStatName, pData, cubData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamUserStats_GetGlobalStatHistory_(string pchStatName, uint pData, uint cubData)
        //public static int ISteamUserStats_GetGlobalStatHistory_(string pchStatName, ref double[] pData, uint cubData)
        {
            return SteamEmulator.SteamUserStats.GetGlobalStatHistory(pchStatName, pData, cubData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamUtils_GetSecondsSinceAppActive()
        {
            return SteamEmulator.SteamUtils.GetSecondsSinceAppActive();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamUtils_GetSecondsSinceComputerActive()
        {
            return SteamEmulator.SteamUtils.GetSecondsSinceComputerActive();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EUniverse ISteamUtils_GetConnectedUniverse()
        {
            return (EUniverse)SteamEmulator.SteamUtils.GetConnectedUniverse();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamUtils_GetServerRealTime()
        {
            return SteamEmulator.SteamUtils.GetServerRealTime();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string ISteamUtils_GetIPCountry()
        {
            return SteamEmulator.SteamUtils.GetIPCountry();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUtils_GetImageSize(int iImage, ref uint pnWidth, ref uint pnHeight)
        {
            return SteamEmulator.SteamUtils.GetImageSize(iImage, ref pnWidth, ref pnHeight);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUtils_GetImageRGBA(int iImage, IntPtr pubDest, int nDestBufferSize)
        //public static bool ISteamUtils_GetImageRGBA(int iImage, ref byte[] pubDest, int nDestBufferSize)
        {
            return SteamEmulator.SteamUtils.GetImageRGBA(iImage, pubDest, nDestBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUtils_GetCSERIPPort(ref uint unIP, ref ushort usPort)
        {
            return SteamEmulator.SteamUtils.GetCSERIPPort(unIP, usPort);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static byte ISteamUtils_GetCurrentBatteryPower()
        {
            return (byte)SteamEmulator.SteamUtils.GetCurrentBatteryPower();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamUtils_GetAppID()
        {
            return SteamEmulator.SteamUtils.GetAppID();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamUtils_SetOverlayNotificationPosition(ENotificationPosition eNotificationPosition)
        {
            SteamEmulator.SteamUtils.SetOverlayNotificationPosition((int)eNotificationPosition);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUtils_IsAPICallCompleted(SteamAPICall_t hSteamAPICall, ref bool pbFailed)
        {
            return SteamEmulator.SteamUtils.IsAPICallCompleted(hSteamAPICall, ref pbFailed);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ESteamAPICallFailure ISteamUtils_GetAPICallFailureReason(SteamAPICall_t hSteamAPICall)
        {
            return (ESteamAPICallFailure)SteamEmulator.SteamUtils.GetAPICallFailureReason(hSteamAPICall);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUtils_GetAPICallResult(SteamAPICall_t hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, ref bool pbFailed)
        {
            return SteamEmulator.SteamUtils.GetAPICallResult(hSteamAPICall, pCallback, cubCallback, iCallbackExpected, ref pbFailed);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamUtils_RunFrame()
        {
            //SteamEmulator.SteamUtils.RunFrame();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamUtils_GetIPCCallCount()
        {
            return SteamEmulator.SteamUtils.GetIPCCallCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamUtils_SetWarningMessageHook(IntPtr pFunction)
        {
            SteamEmulator.SteamUtils.SetWarningMessageHook(pFunction);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUtils_IsOverlayEnabled()
        {
            return SteamEmulator.SteamUtils.IsOverlayEnabled();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUtils_BOverlayNeedsPresent()
        {
            return SteamEmulator.SteamUtils.BOverlayNeedsPresent();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamUtils_CheckFileSignature(string szFileName)
        {
            return SteamEmulator.SteamUtils.CheckFileSignature(szFileName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUtils_ShowGamepadTextInput(EGamepadTextInputMode eInputMode, EGamepadTextInputLineMode eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText)
        {
            return SteamEmulator.SteamUtils.ShowGamepadTextInput((int)eInputMode, (int)eLineInputMode, pchDescription, unCharMax, pchExistingText);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamUtils_GetEnteredGamepadTextLength()
        {
            return SteamEmulator.SteamUtils.GetEnteredGamepadTextLength();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUtils_GetEnteredGamepadTextInput(string pchText, uint cchText)
        {
            return SteamEmulator.SteamUtils.GetEnteredGamepadTextInput(pchText, cchText);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string ISteamUtils_GetSteamUILanguage()
        {
            return SteamEmulator.SteamUtils.GetSteamUILanguage();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamUtils_IsSteamRunningInVR()
        {
            return SteamEmulator.SteamUtils.IsSteamRunningInVR();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamVideo_GetVideoURL(AppId_t unVideoAppID)
        {
            //SteamAPI_ISteamVideo_GetVideoURL(unVideoAppID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamVideo_IsBroadcasting(ref int pnNumViewers)
        {
            //return SteamAPI_ISteamVideo_IsBroadcasting(pnNumViewers);
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamGameServerHTTP_CreateHTTPRequest(uint eHTTPRequestMethod, string pchAbsoluteURL)
        {
            return SteamEmulator.SteamHTTP.CreateHTTPRequest(eHTTPRequestMethod, pchAbsoluteURL);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerHTTP_SetHTTPRequestContextValue(HTTPRequestHandle hRequest, ulong ulContextValue)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestContextValue(hRequest, ulContextValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerHTTP_SetHTTPRequestNetworkActivityTimeout(HTTPRequestHandle hRequest, uint unTimeoutSeconds)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestNetworkActivityTimeout(hRequest, unTimeoutSeconds);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerHTTP_SetHTTPRequestHeaderValue(HTTPRequestHandle hRequest, string pchHeaderName, string pchHeaderValue)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestHeaderValue(hRequest, pchHeaderName, pchHeaderValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerHTTP_SetHTTPRequestGetOrPostParameter(HTTPRequestHandle hRequest, string pchParamName, string pchParamValue)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestGetOrPostParameter(hRequest, pchParamName, pchParamValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerHTTP_SendHTTPRequest(HTTPRequestHandle hRequest, ref SteamAPICall_t pCallHandle)
        {
            return SteamEmulator.SteamHTTP.SendHTTPRequest(hRequest, ref pCallHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerHTTP_SendHTTPRequestAndStreamResponse(HTTPRequestHandle hRequest, ref SteamAPICall_t pCallHandle)
        {
            return SteamEmulator.SteamHTTP.SendHTTPRequestAndStreamResponse(hRequest, pCallHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerHTTP_DeferHTTPRequest(HTTPRequestHandle hRequest)
        {
            return SteamEmulator.SteamHTTP.DeferHTTPRequest(hRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerHTTP_PrioritizeHTTPRequest(HTTPRequestHandle hRequest)
        {
            return SteamEmulator.SteamHTTP.PrioritizeHTTPRequest(hRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerHTTP_GetHTTPResponseHeaderSize(HTTPRequestHandle hRequest, string pchHeaderName, ref uint unResponseHeaderSize)
        {
            return SteamEmulator.SteamHTTP.GetHTTPResponseHeaderSize(hRequest, pchHeaderName, unResponseHeaderSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerHTTP_GetHTTPResponseHeaderValue(HTTPRequestHandle hRequest, string pchHeaderName, int pHeaderValueBuffer, uint unBufferSize)
        //public static bool ISteamGameServerHTTP_GetHTTPResponseHeaderValue(HTTPRequestHandle hRequest, string pchHeaderName, ref byte[] pHeaderValueBuffer, uint unBufferSize)
        {
            return SteamEmulator.SteamHTTP.GetHTTPResponseHeaderValue(hRequest, pchHeaderName, pHeaderValueBuffer, unBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerHTTP_GetHTTPResponseBodySize(HTTPRequestHandle hRequest, ref uint unBodySize)
        {
            return SteamEmulator.SteamHTTP.GetHTTPResponseBodySize(hRequest, unBodySize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerHTTP_GetHTTPResponseBodyData(HTTPRequestHandle hRequest, IntPtr pBodyDataBuffer, uint unBufferSize)
        //public static bool ISteamGameServerHTTP_GetHTTPResponseBodyData(HTTPRequestHandle hRequest, ref byte[] pBodyDataBuffer, uint unBufferSize)
        {
            return SteamEmulator.SteamHTTP.GetHTTPResponseBodyData(hRequest, pBodyDataBuffer, unBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerHTTP_GetHTTPStreamingResponseBodyData(HTTPRequestHandle hRequest, uint cOffset, IntPtr pBodyDataBuffer, uint unBufferSize)
        //public static bool ISteamGameServerHTTP_GetHTTPStreamingResponseBodyData(HTTPRequestHandle hRequest, uint cOffset, ref byte[] pBodyDataBuffer, uint unBufferSize)
        {
            return SteamEmulator.SteamHTTP.GetHTTPStreamingResponseBodyData(hRequest, cOffset, pBodyDataBuffer, unBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerHTTP_ReleaseHTTPRequest(HTTPRequestHandle hRequest)
        {
            return SteamEmulator.SteamHTTP.ReleaseHTTPRequest(hRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerHTTP_GetHTTPDownloadProgressPct(HTTPRequestHandle hRequest, ref float pflPercentOut)
        {
            return SteamEmulator.SteamHTTP.GetHTTPDownloadProgressPct(hRequest, pflPercentOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerHTTP_SetHTTPRequestRawPostBody(HTTPRequestHandle hRequest, string pchContentType, IntPtr pubBody, uint unBodyLen)
        //public static bool ISteamGameServerHTTP_SetHTTPRequestRawPostBody(HTTPRequestHandle hRequest, string pchContentType, ref byte[] pubBody, uint unBodyLen)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestRawPostBody(hRequest, pchContentType, pubBody, unBodyLen);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamGameServerHTTP_CreateCookieContainer(bool bAllowResponsesToModify)
        {
            return SteamEmulator.SteamHTTP.CreateCookieContainer(bAllowResponsesToModify);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerHTTP_ReleaseCookieContainer(HTTPCookieContainerHandle hCookieContainer)
        {
            return SteamEmulator.SteamHTTP.ReleaseCookieContainer(hCookieContainer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerHTTP_SetCookie(HTTPCookieContainerHandle hCookieContainer, string pchHost, string pchUrl, string pchCookie)
        {
            return SteamEmulator.SteamHTTP.SetCookie(hCookieContainer, pchHost, pchUrl, pchCookie);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerHTTP_SetHTTPRequestCookieContainer(HTTPRequestHandle hRequest, HTTPCookieContainerHandle hCookieContainer)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestCookieContainer(hRequest, (uint)hCookieContainer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerHTTP_SetHTTPRequestUserAgentInfo(HTTPRequestHandle hRequest, string pchUserAgentInfo)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestUserAgentInfo(hRequest, pchUserAgentInfo);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerHTTP_SetHTTPRequestRequiresVerifiedCertificate(HTTPRequestHandle hRequest, bool bRequireVerifiedCertificate)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestRequiresVerifiedCertificate(hRequest, bRequireVerifiedCertificate);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerHTTP_SetHTTPRequestAbsoluteTimeoutMS(HTTPRequestHandle hRequest, uint unMilliseconds)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestAbsoluteTimeoutMS(hRequest, unMilliseconds);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerHTTP_GetHTTPRequestWasTimedOut(HTTPRequestHandle hRequest, ref bool pbWasTimedOut)
        {
            return SteamEmulator.SteamHTTP.GetHTTPRequestWasTimedOut(hRequest, pbWasTimedOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EResult ISteamGameServerInventory_GetResultStatus(SteamInventoryResult_t resultHandle)
        {
            return (EResult)SteamEmulator.SteamInventory.GetResultStatus(resultHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerInventory_GetResultItems(SteamInventoryResult_t resultHandle, IntPtr pOutItemsArray, ref uint punOutItemsArraySize)
        //public static bool ISteamGameServerInventory_GetResultItems(SteamInventoryResult_t resultHandle, ref SteamItemDetails_t[] pOutItemsArray, ref uint punOutItemsArraySize)
        {
            return SteamEmulator.SteamInventory.GetResultItems(resultHandle, pOutItemsArray, punOutItemsArraySize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamGameServerInventory_GetResultTimestamp(SteamInventoryResult_t resultHandle)
        {
            return SteamEmulator.SteamInventory.GetResultTimestamp(resultHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerInventory_CheckResultSteamID(SteamInventoryResult_t resultHandle, ulong steamIDExpected)
        {
            return SteamEmulator.SteamInventory.CheckResultSteamID(resultHandle, (ulong)steamIDExpected);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServerInventory_DestroyResult(SteamInventoryResult_t resultHandle)
        {
            SteamEmulator.SteamInventory.DestroyResult(resultHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerInventory_GetAllItems(ref SteamInventoryResult_t pResultHandle)
        {
            return SteamEmulator.SteamInventory.GetAllItems(pResultHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerInventory_GetItemsByID(ref SteamInventoryResult_t pResultHandle, ref SteamItemInstanceID_t pInstanceIDs, uint unCountInstanceIDs)
        //public static bool ISteamGameServerInventory_GetItemsByID(ref SteamInventoryResult_t pResultHandle, ref SteamItemInstanceID_t[] pInstanceIDs, uint unCountInstanceIDs)
        {
            return SteamEmulator.SteamInventory.GetItemsByID(pResultHandle, ref pInstanceIDs, unCountInstanceIDs);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerInventory_SerializeResult(SteamInventoryResult_t resultHandle, IntPtr pOutBuffer, ref uint punOutBufferSize)
        //public static bool ISteamGameServerInventory_SerializeResult(SteamInventoryResult_t resultHandle, ref byte[] pOutBuffer, ref uint punOutBufferSize)
        {
            return SteamEmulator.SteamInventory.SerializeResult(resultHandle, pOutBuffer, punOutBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerInventory_DeserializeResult(ref SteamInventoryResult_t pOutResultHandle, IntPtr pBuffer, uint unBufferSize, bool bRESERVED_MUST_BE_FALSE)
        //public static bool ISteamGameServerInventory_DeserializeResult(ref SteamInventoryResult_t pOutResultHandle, ref byte[] pBuffer, uint unBufferSize, bool bRESERVED_MUST_BE_FALSE)
        {
            return SteamEmulator.SteamInventory.DeserializeResult(pOutResultHandle, pBuffer, unBufferSize, bRESERVED_MUST_BE_FALSE);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerInventory_GenerateItems(ref SteamInventoryResult_t pResultHandle, IntPtr pArrayItemDefs, IntPtr punArrayQuantity, uint unArrayLength)
        //public static bool ISteamGameServerInventory_GenerateItems(ref SteamInventoryResult_t pResultHandle, ref SteamItemDef_t[] pArrayItemDefs, ref uint[] punArrayQuantity, uint unArrayLength)
        {
            return SteamEmulator.SteamInventory.GenerateItems(pResultHandle, pArrayItemDefs, punArrayQuantity, unArrayLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerInventory_GrantPromoItems(ref SteamInventoryResult_t pResultHandle)
        {
            return SteamEmulator.SteamInventory.GrantPromoItems(pResultHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerInventory_AddPromoItem(ref SteamInventoryResult_t pResultHandle, SteamItemDef_t itemDef)
        {
            return SteamEmulator.SteamInventory.AddPromoItem(pResultHandle, itemDef);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerInventory_AddPromoItems(ref SteamInventoryResult_t pResultHandle, IntPtr pArrayItemDefs, uint unArrayLength)
        //public static bool ISteamGameServerInventory_AddPromoItems(ref SteamInventoryResult_t pResultHandle, ref SteamItemDef_t[] pArrayItemDefs, uint unArrayLength)
        {
            return SteamEmulator.SteamInventory.AddPromoItems(pResultHandle, pArrayItemDefs, unArrayLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerInventory_ConsumeItem(ref SteamInventoryResult_t pResultHandle, SteamItemInstanceID_t itemConsume, uint unQuantity)
        {
            return SteamEmulator.SteamInventory.ConsumeItem(pResultHandle, itemConsume, unQuantity);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerInventory_ExchangeItems(ref SteamInventoryResult_t pResultHandle, ref SteamItemDef_t[] pArrayGenerate, ref uint[] punArrayGenerateQuantity, uint unArrayGenerateLength, ref SteamItemInstanceID_t[] pArrayDestroy, ref uint[] punArrayDestroyQuantity, uint unArrayDestroyLength)
        //public static bool ISteamGameServerInventory_ExchangeItems(ref SteamInventoryResult_t pResultHandle, ref SteamItemDef_t[] pArrayGenerate, ref uint[] punArrayGenerateQuantity, uint unArrayGenerateLength, ref SteamItemInstanceID_t[] pArrayDestroy, ref uint[] punArrayDestroyQuantity, uint unArrayDestroyLength)
        {
            return SteamEmulator.SteamInventory.ExchangeItems(ref pResultHandle, ref pArrayGenerate, ref punArrayGenerateQuantity, unArrayGenerateLength, ref pArrayDestroy, ref punArrayDestroyQuantity, unArrayDestroyLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerInventory_TransferItemQuantity(ref SteamInventoryResult_t pResultHandle, SteamItemInstanceID_t itemIdSource, uint unQuantity, SteamItemInstanceID_t itemIdDest)
        {
            return SteamEmulator.SteamInventory.TransferItemQuantity(pResultHandle, (uint)itemIdSource, unQuantity, (uint)itemIdDest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServerInventory_SendItemDropHeartbeat()
        {
            SteamEmulator.SteamInventory.SendItemDropHeartbeat();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerInventory_TriggerItemDrop(ref SteamInventoryResult_t pResultHandle, SteamItemDef_t dropListDefinition)
        {
            return SteamEmulator.SteamInventory.TriggerItemDrop(pResultHandle, dropListDefinition);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerInventory_TradeItems(uint pResultHandle, ulong steamIDTradePartner, IntPtr pArrayGive, IntPtr pArrayGiveQuantity, uint nArrayGiveLength, IntPtr pArrayGet, IntPtr pArrayGetQuantity, uint nArrayGetLength)
        //public static bool ISteamGameServerInventory_TradeItems(ref SteamInventoryResult_t pResultHandle, ulong steamIDTradePartner, ref SteamItemInstanceID_t[] pArrayGive, ref uint[] pArrayGiveQuantity, uint nArrayGiveLength, ref SteamItemInstanceID_t[] pArrayGet, ref uint[] pArrayGetQuantity, uint nArrayGetLength)
        {
            return SteamEmulator.SteamInventory.TradeItems(pResultHandle, steamIDTradePartner, pArrayGive, pArrayGiveQuantity, nArrayGiveLength, pArrayGet, pArrayGetQuantity, nArrayGetLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerInventory_LoadItemDefinitions()
        {
            return SteamEmulator.SteamInventory.LoadItemDefinitions();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerInventory_GetItemDefinitionIDs(IntPtr pItemDefIDs, ref uint punItemDefIDsArraySize)
        //public static bool ISteamGameServerInventory_GetItemDefinitionIDs(ref SteamItemDef_t[] pItemDefIDs, ref uint punItemDefIDsArraySize)
        {
            return SteamEmulator.SteamInventory.GetItemDefinitionIDs(pItemDefIDs, punItemDefIDsArraySize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerInventory_GetItemDefinitionProperty(SteamItemDef_t iDefinition, string pchPropertyName, IntPtr pchValueBuffer, ref uint punValueBufferSize)
        {
            return SteamEmulator.SteamInventory.GetItemDefinitionProperty(iDefinition, pchPropertyName, pchValueBuffer, punValueBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerNetworking_SendP2PPacket(ulong steamIDRemote, IntPtr pubData, uint cubData, EP2PSend eP2PSendType, int nChannel)
        {
            return SteamEmulator.SteamNetworking.SendP2PPacket((ulong)steamIDRemote, pubData, cubData, (int)eP2PSendType, nChannel);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerNetworking_IsP2PPacketAvailable(ref uint pcubMsgSize, int nChannel)
        {
            return SteamEmulator.SteamNetworking.IsP2PPacketAvailable(ref pcubMsgSize, nChannel);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerNetworking_ReadP2PPacket(IntPtr pubDest, uint cubDest, ref uint pcubMsgSize, ref ulong psteamIDRemote, int nChannel)
        {
            return SteamEmulator.SteamNetworking.ReadP2PPacket(pubDest, cubDest, ref pcubMsgSize, ref psteamIDRemote, nChannel);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerNetworking_AcceptP2PSessionWithUser(ulong steamIDRemote)
        {
            return SteamEmulator.SteamNetworking.AcceptP2PSessionWithUser((ulong)steamIDRemote);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerNetworking_CloseP2PSessionWithUser(ulong steamIDRemote)
        {
            return SteamEmulator.SteamNetworking.CloseP2PSessionWithUser((ulong)steamIDRemote);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerNetworking_CloseP2PChannelWithUser(ulong steamIDRemote, int nChannel)
        {
            return SteamEmulator.SteamNetworking.CloseP2PChannelWithUser((ulong)steamIDRemote, nChannel);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerNetworking_GetP2PSessionState(ulong steamIDRemote, IntPtr pConnectionState)
        {
            return SteamEmulator.SteamNetworking.GetP2PSessionState((ulong)steamIDRemote, pConnectionState);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerNetworking_AllowP2PPacketRelay(bool bAllow)
        {
            return SteamEmulator.SteamNetworking.AllowP2PPacketRelay(bAllow);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamGameServerNetworking_CreateListenSocket(int nVirtualP2PPort, uint nIP, ushort nPort, bool bAllowUseOfPacketRelay)
        {
            return SteamEmulator.SteamNetworking.CreateListenSocket(nVirtualP2PPort, nIP, nPort, bAllowUseOfPacketRelay);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamGameServerNetworking_CreateP2PConnectionSocket(ulong steamIDTarget, int nVirtualPort, int nTimeoutSec, bool bAllowUseOfPacketRelay)
        {
            return SteamEmulator.SteamNetworking.CreateP2PConnectionSocket((ulong)steamIDTarget, nVirtualPort, nTimeoutSec, bAllowUseOfPacketRelay);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamGameServerNetworking_CreateConnectionSocket(uint nIP, ushort nPort, int nTimeoutSec)
        {
            return SteamEmulator.SteamNetworking.CreateConnectionSocket(nIP, nPort, nTimeoutSec);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerNetworking_DestroySocket(SNetSocket_t hSocket, bool bNotifyRemoteEnd)
        {
            return SteamEmulator.SteamNetworking.DestroySocket(hSocket, bNotifyRemoteEnd);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerNetworking_DestroyListenSocket(SNetListenSocket_t hSocket, bool bNotifyRemoteEnd)
        {
            return SteamEmulator.SteamNetworking.DestroyListenSocket(hSocket, bNotifyRemoteEnd);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerNetworking_SendDataOnSocket(SNetSocket_t hSocket, IntPtr pubData, uint cubData, bool bReliable)
        {
            return SteamEmulator.SteamNetworking.SendDataOnSocket(hSocket, pubData, cubData, bReliable);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerNetworking_IsDataAvailableOnSocket(SNetSocket_t hSocket, ref uint pcubMsgSize)
        {
            return SteamEmulator.SteamNetworking.IsDataAvailableOnSocket(hSocket, pcubMsgSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerNetworking_RetrieveDataFromSocket(SNetSocket_t hSocket, IntPtr pubDest, uint cubDest, ref uint pcubMsgSize)
        {
            return SteamEmulator.SteamNetworking.RetrieveDataFromSocket(hSocket, pubDest, cubDest, pcubMsgSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerNetworking_IsDataAvailable(SNetListenSocket_t hListenSocket, ref uint pcubMsgSize, ref SNetSocket_t phSocket)
        {
            return SteamEmulator.SteamNetworking.IsDataAvailable(hListenSocket, pcubMsgSize, phSocket);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerNetworking_RetrieveData(SNetListenSocket_t hListenSocket, IntPtr pubDest, uint cubDest, ref uint pcubMsgSize, ref SNetSocket_t phSocket)
        {
            return SteamEmulator.SteamNetworking.RetrieveData(hListenSocket, pubDest, cubDest, pcubMsgSize, phSocket);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerNetworking_GetSocketInfo(SNetSocket_t hSocket, ref ulong pSteamIDRemote, ref int peSocketStatus, ref uint punIPRemote, ref ushort punPortRemote)
        {
            return SteamEmulator.SteamNetworking.GetSocketInfo(hSocket, (ulong)pSteamIDRemote, peSocketStatus, punIPRemote, punPortRemote);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerNetworking_GetListenSocketInfo(SNetListenSocket_t hListenSocket, ref uint pnIP, ref ushort pnPort)
        {
            return SteamEmulator.SteamNetworking.GetListenSocketInfo(hListenSocket, pnIP, pnPort);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ESNetSocketConnectionType ISteamGameServerNetworking_GetSocketConnectionType(SNetSocket_t hSocket)
        {
            return (ESNetSocketConnectionType)SteamEmulator.SteamNetworking.GetSocketConnectionType(hSocket);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int ISteamGameServerNetworking_GetMaxPacketSize(SNetSocket_t hSocket)
        {
            return SteamEmulator.SteamNetworking.GetMaxPacketSize(hSocket);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamGameServerUtils_GetSecondsSinceAppActive()
        {
            return SteamEmulator.SteamUtils.GetSecondsSinceAppActive();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamGameServerUtils_GetSecondsSinceComputerActive()
        {
            return SteamEmulator.SteamUtils.GetSecondsSinceComputerActive();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EUniverse ISteamGameServerUtils_GetConnectedUniverse()
        {
            return (EUniverse)SteamEmulator.SteamUtils.GetConnectedUniverse();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamGameServerUtils_GetServerRealTime()
        {
            return SteamEmulator.SteamUtils.GetServerRealTime();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string ISteamGameServerUtils_GetIPCountry()
        {
            return SteamEmulator.SteamUtils.GetIPCountry();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerUtils_GetImageSize(int iImage, ref uint pnWidth, ref uint pnHeight)
        {
            return SteamEmulator.SteamUtils.GetImageSize(iImage, ref pnWidth, ref pnHeight);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerUtils_GetImageRGBA(int iImage, IntPtr pubDest, int nDestBufferSize)
        //public static bool ISteamGameServerUtils_GetImageRGBA(int iImage, ref byte[] pubDest, int nDestBufferSize)
        {
            return SteamEmulator.SteamUtils.GetImageRGBA(iImage, pubDest, nDestBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerUtils_GetCSERIPPort(ref uint unIP, ref ushort usPort)
        {
            return SteamEmulator.SteamUtils.GetCSERIPPort(unIP, usPort);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static byte ISteamGameServerUtils_GetCurrentBatteryPower()
        {
            return (byte)SteamEmulator.SteamUtils.GetCurrentBatteryPower();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamGameServerUtils_GetAppID()
        {
            return SteamEmulator.SteamUtils.GetAppID();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServerUtils_SetOverlayNotificationPosition(ENotificationPosition eNotificationPosition)
        {
            SteamEmulator.SteamUtils.SetOverlayNotificationPosition((int)eNotificationPosition);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerUtils_IsAPICallCompleted(SteamAPICall_t hSteamAPICall, ref bool pbFailed)
        {
            return SteamEmulator.SteamUtils.IsAPICallCompleted(hSteamAPICall, ref pbFailed);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ESteamAPICallFailure ISteamGameServerUtils_GetAPICallFailureReason(SteamAPICall_t hSteamAPICall)
        {
            return (ESteamAPICallFailure)SteamEmulator.SteamUtils.GetAPICallFailureReason(hSteamAPICall);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerUtils_GetAPICallResult(SteamAPICall_t hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, ref bool pbFailed)
        {
            return SteamEmulator.SteamUtils.GetAPICallResult(hSteamAPICall, pCallback, cubCallback, iCallbackExpected, ref pbFailed);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServerUtils_RunFrame()
        {
            //SteamEmulator.SteamUtils.RunFrame();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamGameServerUtils_GetIPCCallCount()
        {
            return SteamEmulator.SteamUtils.GetIPCCallCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ISteamGameServerUtils_SetWarningMessageHook(IntPtr pFunction)
        {
            SteamEmulator.SteamUtils.SetWarningMessageHook(pFunction);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerUtils_IsOverlayEnabled()
        {
            return SteamEmulator.SteamUtils.IsOverlayEnabled();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerUtils_BOverlayNeedsPresent()
        {
            return SteamEmulator.SteamUtils.BOverlayNeedsPresent();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong ISteamGameServerUtils_CheckFileSignature(string szFileName)
        {
            return SteamEmulator.SteamUtils.CheckFileSignature(szFileName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerUtils_ShowGamepadTextInput(EGamepadTextInputMode eInputMode, EGamepadTextInputLineMode eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText)
        {
            return SteamEmulator.SteamUtils.ShowGamepadTextInput((int)eInputMode, (int)eLineInputMode, pchDescription, unCharMax, pchExistingText);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint ISteamGameServerUtils_GetEnteredGamepadTextLength()
        {
            return SteamEmulator.SteamUtils.GetEnteredGamepadTextLength();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerUtils_GetEnteredGamepadTextInput(string pchText, uint cchText)
        {
            return SteamEmulator.SteamUtils.GetEnteredGamepadTextInput(pchText, cchText);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string ISteamGameServerUtils_GetSteamUILanguage()
        {
            return SteamEmulator.SteamUtils.GetSteamUILanguage();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ISteamGameServerUtils_IsSteamRunningInVR()
        {
            return SteamEmulator.SteamUtils.IsSteamRunningInVR();
        }

        private static void Write(object msg)
        {
            SteamEmulator.Write("CSteamworks", msg);
        }
    }
}

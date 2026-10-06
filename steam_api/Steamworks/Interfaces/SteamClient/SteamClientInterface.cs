using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Managers;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;

namespace SKYNET.Steamworks.Interfaces
{
    [InterfaceLayout("SteamClient001",
        "CreateGlobalInstance", "ConnectToGlobalInstance", "CreateLocalInstance", "ReleaseInstance",
        "GetISteamUser__V001", "GetIVAC", "BMainLoop", "Test_SetSpew",
        "Test_SetSpewFunc", "Test_OverrideIPs", "Test_SetServerLoadState", "Test_SetStressMode",
        "Test_GetStatsVConn")]
    [InterfaceLayout("SteamClient002",
        "CreateGlobalInstance", "ConnectToGlobalInstance", "CreateLocalInstance", "ReleaseInstance",
        "GetISteamUser__V001", "GetIVAC", "BMainLoop", "Test_SetSpew",
        "Test_SetSpewFunc", "Test_OverrideIPs", "Test_SetServerLoadState", "Test_SetStressMode",
        "Test_GetStatsVConn", "Test_RemoveAllClients")]
    [InterfaceLayout("SteamClient003",
        "CreateGlobalInstance", "ConnectToGlobalInstance", "CreateLocalInstance", "ReleaseInstance",
        "GetISteamUser__V001", "GetIVAC", "BMainLoop__V003", "Test_SetSpew",
        "Test_SetSpewFunc", "Test_OverrideIPs", "Test_SetServerLoadState", "Test_SetStressMode",
        "Test_GetStatsVConn", "Test_RemoveAllClients")]
    [InterfaceLayout("SteamClient004",
        "CreateGlobalInstance", "ConnectToGlobalInstance", "CreateLocalInstance", "ReleaseInstance",
        "GetISteamUser__V001", "GetIVAC", "GetISteamGameServer__V004", "SetLocalIPBinding")]
    [InterfaceLayout("SteamClient005",
        "CreateSteamPipe", "BReleaseSteamPipe", "CreateGlobalUser", "ConnectToGlobalUser",
        "CreateLocalUser__V007", "ReleaseUser", "GetISteamUser", "GetIVAC",
        "GetISteamGameServer", "SetLocalIPBinding", "GetConnectedUniverse", "GetUniverseName",
        "GetISteamFriends", "BGetCallback", "FreeLastCallback", "SetEUniverse")]
    [InterfaceLayout("SteamClient006",
        "CreateSteamPipe", "BReleaseSteamPipe", "CreateGlobalUser", "ConnectToGlobalUser",
        "CreateLocalUser__V007", "ReleaseUser", "GetISteamUser", "GetIVAC",
        "GetISteamGameServer", "SetLocalIPBinding", "GetUniverseName", "GetISteamFriends",
        "GetISteamUtils", "GetISteamBilling_old", "GetISteamMatchmaking", "GetISteamApps",
        "GetISteamContentServer", "GetISteamMasterServerUpdater", "GetISteamMatchmakingServers", "RunFrame",
        "GetIPCCallCount")]
    [InterfaceLayout("SteamClient007",
        "CreateSteamPipe", "BReleaseSteamPipe", "ConnectToGlobalUser", "CreateLocalUser__V007",
        "ReleaseUser", "GetISteamUser", "GetISteamGameServer", "SetLocalIPBinding",
        "GetISteamFriends", "GetISteamUtils", "GetISteamMatchmaking", "GetISteamContentServer",
        "GetISteamMasterServerUpdater", "GetISteamMatchmakingServers", "GetISteamGenericInterface", "RunFrame",
        "GetIPCCallCount", "GetISteamUserStats", "GetISteamApps", "GetISteamNetworking",
        "SetWarningMessageHook")]
    [InterfaceLayout("SteamClient008",
        "CreateSteamPipe", "BReleaseSteamPipe", "ConnectToGlobalUser", "CreateLocalUser",
        "ReleaseUser", "GetISteamUser", "GetISteamGameServer", "SetLocalIPBinding",
        "GetISteamFriends", "GetISteamUtils", "GetISteamMatchmaking", "GetISteamMasterServerUpdater",
        "GetISteamMatchmakingServers", "GetISteamGenericInterface", "GetISteamUserStats", "GetISteamApps",
        "GetISteamNetworking", "GetISteamRemoteStorage", "RunFrame", "GetIPCCallCount",
        "SetWarningMessageHook")]
    [InterfaceLayout("SteamClient009",
        "CreateSteamPipe", "BReleaseSteamPipe", "ConnectToGlobalUser", "CreateLocalUser",
        "ReleaseUser", "GetISteamUser", "GetISteamGameServer", "SetLocalIPBinding",
        "GetISteamFriends", "GetISteamUtils", "GetISteamMatchmaking", "GetISteamMasterServerUpdater",
        "GetISteamMatchmakingServers", "GetISteamGenericInterface", "GetISteamUserStats", "GetISteamGameServerStats",
        "GetISteamApps", "GetISteamNetworking", "GetISteamRemoteStorage", "RunFrame",
        "GetIPCCallCount", "SetWarningMessageHook")]
    [InterfaceLayout("SteamClient010",
        "CreateSteamPipe", "BReleaseSteamPipe", "ConnectToGlobalUser", "CreateLocalUser",
        "ReleaseUser", "GetISteamUser", "GetISteamGameServer", "SetLocalIPBinding",
        "GetISteamFriends", "GetISteamUtils", "GetISteamMatchmaking", "GetISteamMasterServerUpdater",
        "GetISteamMatchmakingServers", "GetISteamGenericInterface", "GetISteamUserStats", "GetISteamGameServerStats",
        "GetISteamApps", "GetISteamNetworking", "GetISteamRemoteStorage", "RunFrame",
        "GetIPCCallCount", "SetWarningMessageHook", "BShutdownIfAllPipesClosed", "GetISteamHTTP")]
    [InterfaceLayout("SteamClient011",
        "CreateSteamPipe", "BReleaseSteamPipe", "ConnectToGlobalUser", "CreateLocalUser",
        "ReleaseUser", "GetISteamUser", "GetISteamGameServer", "SetLocalIPBinding",
        "GetISteamFriends", "GetISteamUtils", "GetISteamMatchmaking", "GetISteamMasterServerUpdater",
        "GetISteamMatchmakingServers", "GetISteamGenericInterface", "GetISteamUserStats", "GetISteamGameServerStats",
        "GetISteamApps", "GetISteamNetworking", "GetISteamRemoteStorage", "GetISteamScreenshots",
        "RunFrame", "GetIPCCallCount", "SetWarningMessageHook", "BShutdownIfAllPipesClosed",
        "GetISteamHTTP")]
    [InterfaceLayout("SteamClient012",
        "CreateSteamPipe", "BReleaseSteamPipe", "ConnectToGlobalUser", "CreateLocalUser",
        "ReleaseUser", "GetISteamUser", "GetISteamGameServer", "SetLocalIPBinding",
        "GetISteamFriends", "GetISteamUtils", "GetISteamMatchmaking", "GetISteamMatchmakingServers",
        "GetISteamGenericInterface", "GetISteamUserStats", "GetISteamGameServerStats", "GetISteamApps",
        "GetISteamNetworking", "GetISteamRemoteStorage", "GetISteamScreenshots", "RunFrame",
        "GetIPCCallCount", "SetWarningMessageHook", "BShutdownIfAllPipesClosed", "GetISteamHTTP",
        "GetISteamUnifiedMessages", "GetISteamController", "GetISteamUGC")]
    [InterfaceLayout("SteamClient013",
        "CreateSteamPipe", "BReleaseSteamPipe", "ConnectToGlobalUser", "CreateLocalUser",
        "ReleaseUser", "GetISteamUser", "GetISteamGameServer", "SetLocalIPBinding",
        "GetISteamFriends", "GetISteamUtils", "GetISteamMatchmaking", "GetISteamMatchmakingServers",
        "GetISteamGenericInterface", "GetISteamUserStats", "GetISteamGameServerStats", "GetISteamApps",
        "GetISteamNetworking", "GetISteamRemoteStorage", "GetISteamScreenshots", "RunFrame",
        "GetIPCCallCount", "SetWarningMessageHook", "BShutdownIfAllPipesClosed", "GetISteamHTTP",
        "GetISteamUnifiedMessages", "GetISteamController", "GetISteamUGC", "GetISteamInventory",
        "GetISteamVideo", "GetISteamAppList")]
    [InterfaceLayout("SteamClient014",
        "CreateSteamPipe", "BReleaseSteamPipe", "ConnectToGlobalUser", "CreateLocalUser",
        "ReleaseUser", "GetISteamUser", "GetISteamGameServer", "SetLocalIPBinding",
        "GetISteamFriends", "GetISteamUtils", "GetISteamMatchmaking", "GetISteamMatchmakingServers",
        "GetISteamGenericInterface", "GetISteamUserStats", "GetISteamGameServerStats", "GetISteamApps",
        "GetISteamNetworking", "GetISteamRemoteStorage", "GetISteamScreenshots", "RunFrame",
        "GetIPCCallCount", "SetWarningMessageHook", "BShutdownIfAllPipesClosed", "GetISteamHTTP",
        "GetISteamUnifiedMessages", "GetISteamController", "GetISteamUGC", "GetISteamAppList",
        "GetISteamMusic")]
    [InterfaceLayout("SteamClient015",
        "CreateSteamPipe", "BReleaseSteamPipe", "ConnectToGlobalUser", "CreateLocalUser",
        "ReleaseUser", "GetISteamUser", "GetISteamGameServer", "SetLocalIPBinding",
        "GetISteamFriends", "GetISteamUtils", "GetISteamMatchmaking", "GetISteamMatchmakingServers",
        "GetISteamGenericInterface", "GetISteamUserStats", "GetISteamGameServerStats", "GetISteamApps",
        "GetISteamNetworking", "GetISteamRemoteStorage", "GetISteamScreenshots", "RunFrame",
        "GetIPCCallCount", "SetWarningMessageHook", "BShutdownIfAllPipesClosed", "GetISteamHTTP",
        "GetISteamUnifiedMessages", "GetISteamController", "GetISteamUGC", "GetISteamAppList",
        "GetISteamMusic", "GetISteamMusicRemote")]
    [InterfaceLayout("SteamClient016",
        "CreateSteamPipe", "BReleaseSteamPipe", "ConnectToGlobalUser", "CreateLocalUser",
        "ReleaseUser", "GetISteamUser", "GetISteamGameServer", "SetLocalIPBinding",
        "GetISteamFriends", "GetISteamUtils", "GetISteamMatchmaking", "GetISteamMatchmakingServers",
        "GetISteamGenericInterface", "GetISteamUserStats", "GetISteamGameServerStats", "GetISteamApps",
        "GetISteamNetworking", "GetISteamRemoteStorage", "GetISteamScreenshots", "RunFrame",
        "GetIPCCallCount", "SetWarningMessageHook", "BShutdownIfAllPipesClosed", "GetISteamHTTP",
        "GetISteamUnifiedMessages", "GetISteamController", "GetISteamUGC", "GetISteamAppList",
        "GetISteamMusic", "GetISteamMusicRemote", "GetISteamHTMLSurface", "Set_SteamAPI_CPostAPIResultInProcess",
        "Remove_SteamAPI_CPostAPIResultInProcess", "Set_SteamAPI_CCheckCallbackRegisteredInProcess")]
    [InterfaceLayout("SteamClient017",
        "CreateSteamPipe", "BReleaseSteamPipe", "ConnectToGlobalUser", "CreateLocalUser__NoPipe",
        "ReleaseUser", "GetISteamUser", "GetISteamGameServer", "SetLocalIPBinding",
        "GetISteamFriends", "GetISteamUtils", "GetISteamMatchmaking", "GetISteamMatchmakingServers",
        "GetISteamGenericInterface", "GetISteamUserStats", "GetISteamGameServerStats", "GetISteamApps",
        "GetISteamNetworking", "GetISteamRemoteStorage", "GetISteamScreenshots", "RunFrame",
        "GetIPCCallCount", "SetWarningMessageHook", "BShutdownIfAllPipesClosed", "GetISteamHTTP",
        "DEPRECATED_GetISteamUnifiedMessages", "GetISteamController", "GetISteamUGC", "GetISteamAppList",
        "GetISteamMusic", "GetISteamMusicRemote", "GetISteamHTMLSurface", "DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess",
        "DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess", "Set_SteamAPI_CCheckCallbackRegisteredInProcess", "GetISteamInventory", "GetISteamVideo",
        "GetISteamParentalSettings")]
    [InterfaceLayout("SteamClient018",
        "CreateSteamPipe", "BReleaseSteamPipe", "ConnectToGlobalUser", "CreateLocalUser",
        "ReleaseUser", "GetISteamUser", "GetISteamGameServer", "SetLocalIPBinding",
        "GetISteamFriends", "GetISteamUtils", "GetISteamMatchmaking", "GetISteamMatchmakingServers",
        "GetISteamGenericInterface", "GetISteamUserStats", "GetISteamGameServerStats", "GetISteamApps",
        "GetISteamNetworking", "GetISteamRemoteStorage", "GetISteamScreenshots", "GetISteamGameSearch",
        "RunFrame", "GetIPCCallCount", "SetWarningMessageHook", "BShutdownIfAllPipesClosed",
        "GetISteamHTTP", "DEPRECATED_GetISteamUnifiedMessages", "GetISteamController", "GetISteamUGC",
        "GetISteamAppList", "GetISteamMusic", "GetISteamMusicRemote", "GetISteamHTMLSurface",
        "DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess", "DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess", "Set_SteamAPI_CCheckCallbackRegisteredInProcess", "GetISteamInventory",
        "GetISteamVideo", "GetISteamParentalSettings", "GetISteamInput", "GetISteamParties")]
    [InterfaceLayout("SteamClient019",
        "CreateSteamPipe", "BReleaseSteamPipe", "ConnectToGlobalUser", "CreateLocalUser",
        "ReleaseUser", "GetISteamUser", "GetISteamGameServer", "SetLocalIPBinding",
        "GetISteamFriends", "GetISteamUtils", "GetISteamMatchmaking", "GetISteamMatchmakingServers",
        "GetISteamGenericInterface", "GetISteamUserStats", "GetISteamGameServerStats", "GetISteamApps",
        "GetISteamNetworking", "GetISteamRemoteStorage", "GetISteamScreenshots", "GetISteamGameSearch",
        "RunFrame", "GetIPCCallCount", "SetWarningMessageHook", "BShutdownIfAllPipesClosed",
        "GetISteamHTTP", "DEPRECATED_GetISteamUnifiedMessages", "GetISteamController", "GetISteamUGC",
        "GetISteamAppList", "GetISteamMusic", "GetISteamMusicRemote", "GetISteamHTMLSurface",
        "DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess", "DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess", "Set_SteamAPI_CCheckCallbackRegisteredInProcess", "GetISteamInventory",
        "GetISteamVideo", "GetISteamParentalSettings", "GetISteamInput", "GetISteamParties",
        "GetISteamRemotePlay")]
    [InterfaceLayout("SteamClient020",
        "CreateSteamPipe", "BReleaseSteamPipe", "ConnectToGlobalUser__Int32", "CreateLocalUser",
        "ReleaseUser", "GetISteamUser", "GetISteamGameServer", "SetLocalIPBinding__IntPtr",
        "GetISteamFriends", "GetISteamUtils", "GetISteamMatchmaking", "GetISteamMatchmakingServers",
        "GetISteamGenericInterface", "GetISteamUserStats", "GetISteamGameServerStats", "GetISteamApps",
        "GetISteamNetworking", "GetISteamRemoteStorage", "GetISteamScreenshots", "GetISteamGameSearch",
        "RunFrame", "GetIPCCallCount", "SetWarningMessageHook", "BShutdownIfAllPipesClosed",
        "GetISteamHTTP", "DEPRECATED_GetISteamUnifiedMessages", "GetISteamController", "GetISteamUGC",
        "GetISteamAppList", "GetISteamMusic", "GetISteamMusicRemote", "GetISteamHTMLSurface",
        "DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess", "DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess", "Set_SteamAPI_CCheckCallbackRegisteredInProcess", "GetISteamInventory",
        "GetISteamVideo", "GetISteamParentalSettings", "GetISteamInput", "GetISteamParties",
        "GetISteamRemotePlay", "DestroyAllInterfaces")]
    [InterfaceLayout("SteamClient021",
        "CreateSteamPipe", "BReleaseSteamPipe", "ConnectToGlobalUser", "CreateLocalUser",
        "ReleaseUser", "GetISteamUser", "GetISteamGameServer", "SetLocalIPBinding__IntPtr",
        "GetISteamFriends", "GetISteamUtils", "GetISteamMatchmaking", "GetISteamMatchmakingServers",
        "GetISteamGenericInterface", "GetISteamUserStats", "GetISteamGameServerStats", "GetISteamApps",
        "GetISteamNetworking", "GetISteamRemoteStorage", "GetISteamScreenshots", "GetISteamGameSearch",
        "RunFrame", "GetIPCCallCount", "SetWarningMessageHook", "BShutdownIfAllPipesClosed",
        "GetISteamHTTP", "GetISteamController", "GetISteamUGC", "GetISteamMusic",
        "GetISteamMusicRemote", "GetISteamHTMLSurface", "DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess", "DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess",
        "Set_SteamAPI_CCheckCallbackRegisteredInProcess", "GetISteamInventory", "GetISteamVideo", "GetISteamParentalSettings",
        "GetISteamInput", "GetISteamParties", "GetISteamRemotePlay", "DestroyAllInterfaces")]
    [InterfaceLayout("SteamClient022",
        "CreateSteamPipe", "BReleaseSteamPipe", "ConnectToGlobalUser", "CreateLocalUser",
        "ReleaseUser", "GetISteamUser", "GetISteamGameServer", "SetLocalIPBinding__IntPtr",
        "GetISteamFriends", "GetISteamUtils", "GetISteamMatchmaking", "GetISteamMatchmakingServers",
        "GetISteamGenericInterface", "GetISteamUserStats", "GetISteamGameServerStats", "GetISteamApps",
        "GetISteamNetworking", "GetISteamRemoteStorage", "GetISteamScreenshots", "RunFrame",
        "GetIPCCallCount", "SetWarningMessageHook", "BShutdownIfAllPipesClosed", "GetISteamHTTP",
        "GetISteamController", "GetISteamUGC", "GetISteamMusic", "GetISteamMusicRemote",
        "GetISteamHTMLSurface", "DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess", "DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess", "Set_SteamAPI_CCheckCallbackRegisteredInProcess",
        "GetISteamInventory", "GetISteamVideo", "GetISteamParentalSettings", "GetISteamInput",
        "GetISteamParties", "GetISteamRemotePlay", "DestroyAllInterfaces")]
    [InterfaceLayout("SteamClient023",
        "CreateSteamPipe", "BReleaseSteamPipe", "ConnectToGlobalUser", "CreateLocalUser__RefPipe",
        "ReleaseUser", "GetISteamUser", "GetISteamGameServer", "SetLocalIPBinding__IntPtr",
        "GetISteamFriends", "GetISteamUtils", "GetISteamMatchmaking", "GetISteamMatchmakingServers",
        "GetISteamGenericInterface", "GetISteamUserStats", "GetISteamGameServerStats", "GetISteamApps",
        "GetISteamNetworking", "GetISteamRemoteStorage", "GetISteamScreenshots", "RunFrame",
        "GetIPCCallCount", "SetWarningMessageHook", "BShutdownIfAllPipesClosed", "GetISteamHTTP",
        "GetISteamController", "GetISteamUGC", "GetISteamMusic", "GetISteamHTMLSurface",
        "DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess", "DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess", "Set_SteamAPI_CCheckCallbackRegisteredInProcess", "GetISteamInventory",
        "GetISteamVideo", "GetISteamParentalSettings", "GetISteamInput", "GetISteamParties",
        "GetISteamRemotePlay", "DestroyAllInterfaces")]
    public class SteamClientInterface : ISteamInterface
    {
        public HSteamPipe CreateSteamPipe(IntPtr _) => SteamEmulator.SteamClient.CreateSteamPipe();
        public bool BReleaseSteamPipe(IntPtr _, HSteamPipe hSteamPipe) => SteamEmulator.SteamClient.BReleaseSteamPipe(hSteamPipe);
        public HSteamUser ConnectToGlobalUser(IntPtr _, HSteamPipe hSteamPipe) => SteamEmulator.SteamClient.ConnectToGlobalUser(hSteamPipe);
        public int ConnectToGlobalUser__Int32(IntPtr _, HSteamPipe hSteamPipe) => (int)SteamEmulator.SteamClient.ConnectToGlobalUser(hSteamPipe);

        public HSteamUser CreateLocalUser(IntPtr _, IntPtr phSteamPipe, int eAccountType)
        {
            HSteamPipe pipe = SteamEmulator.CreateSteamPipe();
            if (phSteamPipe != IntPtr.Zero)
            {
                Marshal.WriteInt32(phSteamPipe, unchecked((int)pipe));
            }
            return SteamEmulator.SteamClient.CreateLocalUser(pipe, eAccountType);
        }

        public HSteamUser CreateLocalUser__NoPipe(IntPtr _, IntPtr phSteamPipe, int eAccountType)
        {
            HSteamPipe pipe = 0;
            HSteamUser user = SteamEmulator.SteamClient.CreateLocalUser(pipe, eAccountType);
            if (phSteamPipe != IntPtr.Zero)
            {
                Marshal.WriteInt32(phSteamPipe, unchecked((int)pipe));
            }

            return user;
        }

        public HSteamUser CreateLocalUser__RefPipe(IntPtr _, ref HSteamPipe phSteamPipe, int eAccountType)
        {
            phSteamPipe = SteamEmulator.CreateSteamPipe();
            return SteamEmulator.SteamClient.CreateLocalUser(phSteamPipe, eAccountType);
        }

        public HSteamUser CreateLocalUser__V007(IntPtr _, IntPtr phSteamPipe) => CreateLocalUser(_, phSteamPipe, 1);

        public void ReleaseUser(IntPtr _, HSteamPipe hSteamPipe, HSteamUser hUser) => SteamEmulator.SteamClient.ReleaseUser(hSteamPipe, hUser);
        public IntPtr GetISteamUser(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamUser(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamGameServer(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamGameServer(hSteamUser, hSteamPipe, pchVersion);
        public void SetLocalIPBinding(IntPtr _, uint unIP, ushort usPort) => SteamEmulator.SteamClient.SetLocalIPBinding(unIP, usPort);
        public void SetLocalIPBinding__IntPtr(IntPtr _, IntPtr unIP, ushort usPort) => SteamEmulator.SteamClient.SetLocalIPBinding(unIP, usPort);
        public IntPtr GetISteamFriends(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamFriends(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamUtils(IntPtr _, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamUtils(hSteamPipe, pchVersion);
        public IntPtr GetISteamMatchmaking(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamMatchmaking(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamMasterServerUpdater(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamMasterServerUpdater(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamContentServer(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamContentServer(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamMatchmakingServers(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamMatchmakingServers(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamGenericInterface(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamGenericInterface(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamUserStats(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamUserStats(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamGameServerStats(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamGameServerStats(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamApps(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamApps(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamNetworking(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamNetworking(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamRemoteStorage(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamRemoteStorage(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamScreenshots(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamScreenshots(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamGameSearch(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamGameSearch(hSteamUser, hSteamPipe, pchVersion);
        public void RunFrame(IntPtr _) => SteamEmulator.SteamClient.RunFrame();
        public uint GetIPCCallCount(IntPtr _) => SteamEmulator.SteamClient.GetIPCCallCount();
        public void SetWarningMessageHook(IntPtr _, IntPtr pFunction) => SteamEmulator.SteamClient.SetWarningMessageHook(pFunction);
        public bool BShutdownIfAllPipesClosed(IntPtr _) => SteamEmulator.SteamClient.BShutdownIfAllPipesClosed();
        public IntPtr GetISteamHTTP(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamHTTP(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr DEPRECATED_GetISteamUnifiedMessages(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.DEPRECATED_GetISteamUnifiedMessages(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamUnifiedMessages(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamUnifiedMessages(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamController(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamController(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamUGC(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamUGC(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamAppList(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamAppList(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamMusic(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamMusic(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamMusicRemote(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamMusicRemote(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamHTMLSurface(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamHTMLSurface(hSteamUser, hSteamPipe, pchVersion);
        public void Set_SteamAPI_CPostAPIResultInProcess(IntPtr _, IntPtr arg0) => SteamEmulator.SteamClient.Set_SteamAPI_CPostAPIResultInProcess(arg0);
        public void Remove_SteamAPI_CPostAPIResultInProcess(IntPtr _, IntPtr arg0) => SteamEmulator.SteamClient.Remove_SteamAPI_CPostAPIResultInProcess(arg0);
        public void DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess(IntPtr _, IntPtr arg0) => SteamEmulator.SteamClient.DEPRECATED_Set_SteamAPI_CPostAPIResultInProcess(arg0);
        public void DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess(IntPtr _, IntPtr arg0) => SteamEmulator.SteamClient.DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess(arg0);
        public void Set_SteamAPI_CCheckCallbackRegisteredInProcess(IntPtr _, IntPtr arg0) => SteamEmulator.SteamClient.Set_SteamAPI_CCheckCallbackRegisteredInProcess(arg0);
        public IntPtr GetISteamInventory(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamInventory(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamVideo(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamVideo(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamParentalSettings(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamParentalSettings(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamInput(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamInput(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamParties(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamParties(hSteamUser, hSteamPipe, pchVersion);
        public IntPtr GetISteamRemotePlay(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) => SteamEmulator.SteamClient.GetISteamRemotePlay(hSteamUser, hSteamPipe, pchVersion);
        public void DestroyAllInterfaces(IntPtr _) => SteamEmulator.SteamClient.DestroyAllInterfaces();

        private static readonly HashSet<string> _stubsLogged = new HashSet<string>();

        private static void LogStub(string method)
        {
            lock (_stubsLogged)
            {
                if (!_stubsLogged.Add(method))
                {
                    return;
                }
            }

            SteamEmulator.Write("SteamClient", method + " not implemented");
        }

        public HSteamUser CreateGlobalInstance(IntPtr _) => SteamEmulator.SteamClient.ConnectToGlobalUser(0);
        public HSteamUser ConnectToGlobalInstance(IntPtr _) => SteamEmulator.SteamClient.ConnectToGlobalUser(0);
        public HSteamUser CreateLocalInstance(IntPtr _) => SteamEmulator.SteamClient.CreateLocalUser(0, 1);
        public void ReleaseInstance(IntPtr _, HSteamUser hSteamUser) => SteamEmulator.SteamClient.ReleaseUser(0, hSteamUser);
        public IntPtr GetISteamUser__V001(IntPtr _, HSteamUser hSteamUser, string pchVersion) => SteamEmulator.SteamClient.GetISteamUser(hSteamUser, SteamEmulator.HSteamPipe, pchVersion);
        public IntPtr GetISteamGameServer__V004(IntPtr _, HSteamUser hSteamUser, string pchVersion) => SteamEmulator.SteamClient.GetISteamGameServer(hSteamUser, SteamEmulator.HSteamPipe, pchVersion);

        public IntPtr GetIVAC(IntPtr _, HSteamUser hSteamUser)
        {
            LogStub("GetIVAC");
            return IntPtr.Zero;
        }

        public bool BMainLoop(IntPtr _, ulong time, bool unk)
        {
            LogStub("BMainLoop");
            return false;
        }

        public bool BMainLoop__V003(IntPtr _, ulong time)
        {
            LogStub("BMainLoop");
            return false;
        }

        public void Test_SetSpew(IntPtr _, string unk1, int unk2) => LogStub("Test_SetSpew");
        public void Test_SetSpewFunc(IntPtr _, IntPtr unk) => LogStub("Test_SetSpewFunc");
        public void Test_OverrideIPs(IntPtr _, uint unIPPublic, uint unIPPrivate) => LogStub("Test_OverrideIPs");
        public void Test_SetServerLoadState(IntPtr _, bool unk1, bool unk2) => LogStub("Test_SetServerLoadState");
        public void Test_SetStressMode(IntPtr _, bool unk) => LogStub("Test_SetStressMode");

        public int Test_GetStatsVConn(IntPtr _)
        {
            LogStub("Test_GetStatsVConn");
            return 0;
        }

        public void Test_RemoveAllClients(IntPtr _) => LogStub("Test_RemoveAllClients");

        public HSteamUser CreateGlobalUser(IntPtr _, IntPtr phSteamPipe)
        {
            HSteamPipe pipe = SteamEmulator.CreateSteamPipe();
            if (phSteamPipe != IntPtr.Zero)
            {
                Marshal.WriteInt32(phSteamPipe, unchecked((int)pipe));
            }
            return SteamEmulator.SteamClient.ConnectToGlobalUser(pipe);
        }

        public int GetConnectedUniverse(IntPtr _) => SteamEmulator.SteamUtils.GetConnectedUniverse();

        public IntPtr GetUniverseName(IntPtr _, int eUniverse)
        {
            switch (eUniverse)
            {
                case 1:
                    return NativeStringCache.ToUtf8Ptr("Public");
                case 2:
                    return NativeStringCache.ToUtf8Ptr("Beta");
                case 3:
                    return NativeStringCache.ToUtf8Ptr("Internal");
                case 4:
                    return NativeStringCache.ToUtf8Ptr("Dev");
                default:
                    return IntPtr.Zero;
            }
        }

        public bool BGetCallback(IntPtr _, HSteamPipe hSteamPipe, IntPtr pCallbackMsg, IntPtr unk)
        {
            if (pCallbackMsg == IntPtr.Zero)
            {
                return false;
            }

            CallbackMsg_t message = Marshal.PtrToStructure<CallbackMsg_t>(pCallbackMsg);
            bool result = CallbackManager.ManualDispatchGetNextCallback(hSteamPipe, ref message);
            if (result)
            {
                Marshal.StructureToPtr(message, pCallbackMsg, false);
            }
            return result;
        }

        public void FreeLastCallback(IntPtr _, HSteamPipe hSteamPipe) => CallbackManager.ManualDispatchFreeLastCallback(hSteamPipe);

        public void SetEUniverse(IntPtr _, int eUniverse) => LogStub("SetEUniverse");

        public IntPtr GetISteamBilling_old(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            LogStub("GetISteamBilling_old");
            return IntPtr.Zero;
        }
    }
}

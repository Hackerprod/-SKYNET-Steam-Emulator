using SKYNET.Helpers;
using SKYNET.Steamworks.Types;
using System;
using System.Collections.Generic;

using AppId_t = System.UInt32;
using HAuthTicket = System.UInt32;
using HSteamUser = System.UInt32;
using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Interfaces
{
    [InterfaceLayout("SteamUser001",
        "Init", "ProcessCall", "LogOn_old", "LogOff",
        "BLoggedOn", "BConnected", "CreateAccount", "IsVACBanned",
        "RequireShowVACBannedMessage", "AcknowledgeVACBanning", "GSSendLogonRequest", "GSSendDisconnect",
        "GSSendStatusResponse", "GSSetStatus", "NClientGameIDAdd", "RemoveClientGame",
        "SetClientGameServer", "Test_SuspendActivity", "Test_ResumeActivity", "Test_SendVACResponse",
        "Test_SetFakePrivateIP", "Test_SendBigMessage", "Test_BBigMessageResponseReceived", "Test_SetPktLossPct",
        "Test_SetForceTCP", "Test_Heartbeat", "Test_FakeDisconnect", "Test_GetEUniverse")]
    [InterfaceLayout("SteamUser002",
        "Init", "ProcessCall", "LogOn_old", "LogOff",
        "BLoggedOn", "GetLogonState", "BConnected", "CreateAccount",
        "IsVACBanned", "RequireShowVACBannedMessage", "AcknowledgeVACBanning", "GSSendLogonRequest",
        "GSSendDisconnect", "GSSendStatusResponse", "GSSetStatus__V002", "NClientGameIDAdd",
        "RemoveClientGame", "SetClientGameServer", "Test_SuspendActivity", "Test_ResumeActivity",
        "Test_SendVACResponse", "Test_SetFakePrivateIP", "Test_SendBigMessage", "Test_BBigMessageResponseReceived",
        "Test_SetPktLossPct", "Test_SetForceTCP", "Test_SetMaxUDPConnectionAttempts", "Test_Heartbeat",
        "Test_FakeDisconnect", "Test_GetEUniverse")]
    [InterfaceLayout("SteamUser004",
        "GetHSteamUser", "LogOn", "LogOff", "BLoggedOn",
        "GetLogonState", "BConnected", "GetSteamID", "IsVACBanned",
        "RequireShowVACBannedMessage", "AcknowledgeVACBanning", "NClientGameIDAdd", "RemoveClientGame",
        "SetClientGameServer", "SetSteam2Ticket", "AddServerNetAddress", "SetEmail",
        "GetSteamGameConnectToken", "SetRegistryString", "GetRegistryString", "SetRegistryInt",
        "GetRegistryInt", "InitiateGameConnection__V004", "TerminateGameConnection", "SetSelfAsPrimaryChatDestination",
        "IsPrimaryChatDestination", "RequestLegacyCDKey")]
    [InterfaceLayout("SteamUser005",
        "GetHSteamUser", "LogOn", "LogOff", "BLoggedOn",
        "GetLogonState", "BConnected", "GetSteamID", "IsVACBanned",
        "RequireShowVACBannedMessage", "AcknowledgeVACBanning", "SetSteam2Ticket", "AddServerNetAddress",
        "SetEmail", "SetRegistryString", "GetRegistryString", "SetRegistryInt",
        "GetRegistryInt", "InitiateGameConnection__V009", "TerminateGameConnection", "SetSelfAsPrimaryChatDestination",
        "IsPrimaryChatDestination", "RequestLegacyCDKey", "SendGuestPassByEmail", "SendGuestPassByAccountID",
        "AckGuestPass", "RedeemGuestPass", "GetGuestPassToGiveCount", "GetGuestPassToRedeemCount",
        "GetGuestPassLastUpdateTime", "GetGuestPassToGiveInfo", "GetGuestPassToRedeemInfo", "GetGuestPassToRedeemSenderAddress",
        "GetGuestPassToRedeemSenderName", "AcknowledgeMessageByGID", "SetLanguage", "TrackAppUsageEvent",
        "SetAccountName", "SetPassword", "SetAccountCreationTime")]
    [InterfaceLayout("SteamUser006",
        "GetHSteamUser", "LogOn", "LogOff", "BLoggedOn",
        "GetSteamID", "SetRegistryString", "GetRegistryString", "SetRegistryInt",
        "GetRegistryInt", "InitiateGameConnection__V009", "TerminateGameConnection", "TrackAppUsageEvent")]
    [InterfaceLayout("SteamUser007",
        "GetHSteamUser", "LogOn", "LogOff", "BLoggedOn",
        "GetSteamID", "SetRegistryString", "GetRegistryString", "SetRegistryInt",
        "GetRegistryInt", "InitiateGameConnection__V007", "TerminateGameConnection", "TrackAppUsageEvent",
        "RefreshSteam2Login")]
    [InterfaceLayout("SteamUser008",
        "GetHSteamUser", "BLoggedOn", "GetSteamID", "InitiateGameConnection__V007",
        "TerminateGameConnection", "TrackAppUsageEvent", "RefreshSteam2Login")]
    [InterfaceLayout("SteamUser009",
        "GetHSteamUser", "BLoggedOn", "GetSteamID", "InitiateGameConnection__V009",
        "TerminateGameConnection", "TrackAppUsageEvent", "RefreshSteam2Login")]
    [InterfaceLayout("SteamUser010",
        "GetHSteamUser", "BLoggedOn", "GetSteamID", "InitiateGameConnection",
        "TerminateGameConnection", "TrackAppUsageEvent")]
    [InterfaceLayout("SteamUser011",
        "GetHSteamUser", "BLoggedOn", "GetSteamID", "InitiateGameConnection",
        "TerminateGameConnection", "TrackAppUsageEvent", "GetUserDataFolder", "StartVoiceRecording",
        "StopVoiceRecording", "GetCompressedVoice", "DecompressVoice__V013")]
    [InterfaceLayout("SteamUser012",
        "GetHSteamUser", "BLoggedOn", "GetSteamID", "InitiateGameConnection",
        "TerminateGameConnection", "TrackAppUsageEvent", "GetUserDataFolder", "StartVoiceRecording",
        "StopVoiceRecording", "GetCompressedVoice", "DecompressVoice__V013", "GetAuthSessionTicket",
        "BeginAuthSession", "EndAuthSession", "CancelAuthTicket", "UserHasLicenseForApp")]
    [InterfaceLayout("SteamUser013",
        "GetHSteamUser", "BLoggedOn", "GetSteamID", "InitiateGameConnection",
        "TerminateGameConnection", "TrackAppUsageEvent", "GetUserDataFolder", "StartVoiceRecording",
        "StopVoiceRecording", "GetAvailableVoice__V013", "GetVoice__V013", "DecompressVoice__V013",
        "GetAuthSessionTicket", "BeginAuthSession", "EndAuthSession", "CancelAuthTicket",
        "UserHasLicenseForApp")]
    [InterfaceLayout("SteamUser014",
        "GetHSteamUser", "BLoggedOn", "GetSteamID", "InitiateGameConnection",
        "TerminateGameConnection", "TrackAppUsageEvent", "GetUserDataFolder", "StartVoiceRecording",
        "StopVoiceRecording", "GetAvailableVoice__V013", "GetVoice__V013", "DecompressVoice__V013",
        "GetAuthSessionTicket", "BeginAuthSession", "EndAuthSession", "CancelAuthTicket",
        "UserHasLicenseForApp", "BIsBehindNAT", "AdvertiseGame", "RequestEncryptedAppTicket",
        "GetEncryptedAppTicket")]
    [InterfaceLayout("SteamUser015",
        "GetHSteamUser", "BLoggedOn", "GetSteamID", "InitiateGameConnection",
        "TerminateGameConnection", "TrackAppUsageEvent", "GetUserDataFolder", "StartVoiceRecording",
        "StopVoiceRecording", "GetAvailableVoice__V013", "GetVoice__V013", "DecompressVoice",
        "GetVoiceOptimalSampleRate", "GetAuthSessionTicket", "BeginAuthSession", "EndAuthSession",
        "CancelAuthTicket", "UserHasLicenseForApp", "BIsBehindNAT", "AdvertiseGame",
        "RequestEncryptedAppTicket", "GetEncryptedAppTicket")]
    [InterfaceLayout("SteamUser016",
        "GetHSteamUser", "BLoggedOn", "GetSteamID", "InitiateGameConnection",
        "TerminateGameConnection", "TrackAppUsageEvent", "GetUserDataFolder", "StartVoiceRecording",
        "StopVoiceRecording", "GetAvailableVoice", "GetVoice", "DecompressVoice",
        "GetVoiceOptimalSampleRate", "GetAuthSessionTicket", "BeginAuthSession", "EndAuthSession",
        "CancelAuthTicket", "UserHasLicenseForApp", "BIsBehindNAT", "AdvertiseGame",
        "RequestEncryptedAppTicket", "GetEncryptedAppTicket")]
    [InterfaceLayout("SteamUser017",
        "GetHSteamUser", "BLoggedOn", "GetSteamID", "InitiateGameConnection",
        "TerminateGameConnection", "TrackAppUsageEvent", "GetUserDataFolder", "StartVoiceRecording",
        "StopVoiceRecording", "GetAvailableVoice", "GetVoice", "DecompressVoice",
        "GetVoiceOptimalSampleRate", "GetAuthSessionTicket", "BeginAuthSession", "EndAuthSession",
        "CancelAuthTicket", "UserHasLicenseForApp", "BIsBehindNAT", "AdvertiseGame",
        "RequestEncryptedAppTicket", "GetEncryptedAppTicket", "GetGameBadgeLevel", "GetPlayerSteamLevel")]
    [InterfaceLayout("SteamUser018",
        "GetHSteamUser", "BLoggedOn", "GetSteamID", "InitiateGameConnection",
        "TerminateGameConnection", "TrackAppUsageEvent", "GetUserDataFolder", "StartVoiceRecording",
        "StopVoiceRecording", "GetAvailableVoice", "GetVoice", "DecompressVoice",
        "GetVoiceOptimalSampleRate", "GetAuthSessionTicket", "BeginAuthSession", "EndAuthSession",
        "CancelAuthTicket", "UserHasLicenseForApp", "BIsBehindNAT", "AdvertiseGame",
        "RequestEncryptedAppTicket", "GetEncryptedAppTicket", "GetGameBadgeLevel", "GetPlayerSteamLevel",
        "RequestStoreAuthURL")]
    [InterfaceLayout("SteamUser019",
        "GetHSteamUser", "BLoggedOn", "GetSteamID", "InitiateGameConnection",
        "TerminateGameConnection", "TrackAppUsageEvent", "GetUserDataFolder", "StartVoiceRecording",
        "StopVoiceRecording", "GetAvailableVoice", "GetVoice", "DecompressVoice",
        "GetVoiceOptimalSampleRate", "GetAuthSessionTicket", "BeginAuthSession__V019", "EndAuthSession",
        "CancelAuthTicket", "UserHasLicenseForApp__V019", "BIsBehindNAT", "AdvertiseGame",
        "RequestEncryptedAppTicket", "GetEncryptedAppTicket", "GetGameBadgeLevel", "GetPlayerSteamLevel",
        "RequestStoreAuthURL", "BIsPhoneVerified", "BIsTwoFactorEnabled", "BIsPhoneIdentifying",
        "BIsPhoneRequiringVerification")]
    [InterfaceLayout("SteamUser020",
        "GetHSteamUser", "BLoggedOn", "GetSteamID", "InitiateGameConnection",
        "TerminateGameConnection", "TrackAppUsageEvent", "GetUserDataFolder", "StartVoiceRecording",
        "StopVoiceRecording", "GetAvailableVoice", "GetVoice", "DecompressVoice",
        "GetVoiceOptimalSampleRate", "GetAuthSessionTicket", "BeginAuthSession", "EndAuthSession",
        "CancelAuthTicket", "UserHasLicenseForApp", "BIsBehindNAT", "AdvertiseGame",
        "RequestEncryptedAppTicket", "GetEncryptedAppTicket", "GetGameBadgeLevel", "GetPlayerSteamLevel",
        "RequestStoreAuthURL", "BIsPhoneVerified", "BIsTwoFactorEnabled", "BIsPhoneIdentifying",
        "BIsPhoneRequiringVerification", "GetMarketEligibility", "GetDurationControl")]
    [InterfaceLayout("SteamUser021",
        "GetHSteamUser", "BLoggedOn", "GetSteamID", "InitiateGameConnection",
        "TerminateGameConnection", "TrackAppUsageEvent", "GetUserDataFolder", "StartVoiceRecording",
        "StopVoiceRecording", "GetAvailableVoice", "GetVoice", "DecompressVoice",
        "GetVoiceOptimalSampleRate", "GetAuthSessionTicket", "BeginAuthSession", "EndAuthSession",
        "CancelAuthTicket", "UserHasLicenseForApp", "BIsBehindNAT", "AdvertiseGame",
        "RequestEncryptedAppTicket", "GetEncryptedAppTicket", "GetGameBadgeLevel", "GetPlayerSteamLevel",
        "RequestStoreAuthURL", "BIsPhoneVerified", "BIsTwoFactorEnabled", "BIsPhoneIdentifying",
        "BIsPhoneRequiringVerification", "GetMarketEligibility", "GetDurationControl", "BSetDurationControlOnlineState")]
    [InterfaceLayout("SteamUser022",
        "GetHSteamUser", "BLoggedOn", "GetSteamID", "InitiateGameConnection",
        "TerminateGameConnection", "TrackAppUsageEvent", "GetUserDataFolder", "StartVoiceRecording",
        "StopVoiceRecording", "GetAvailableVoice", "GetVoice", "DecompressVoice",
        "GetVoiceOptimalSampleRate", "GetAuthSessionTicket__V023", "BeginAuthSession", "EndAuthSession",
        "CancelAuthTicket", "UserHasLicenseForApp", "BIsBehindNAT", "AdvertiseGame",
        "RequestEncryptedAppTicket", "GetEncryptedAppTicket", "GetGameBadgeLevel", "GetPlayerSteamLevel",
        "RequestStoreAuthURL", "BIsPhoneVerified", "BIsTwoFactorEnabled", "BIsPhoneIdentifying",
        "BIsPhoneRequiringVerification", "GetMarketEligibility", "GetDurationControl", "BSetDurationControlOnlineState")]
    [InterfaceLayout("SteamUser023",
        "GetHSteamUser", "BLoggedOn", "GetSteamID", "InitiateGameConnection_DEPRECATED",
        "TerminateGameConnection_DEPRECATED", "TrackAppUsageEvent", "GetUserDataFolder", "StartVoiceRecording",
        "StopVoiceRecording", "GetAvailableVoice", "GetVoice", "DecompressVoice",
        "GetVoiceOptimalSampleRate", "GetAuthSessionTicket__V023", "GetAuthTicketForWebApi", "BeginAuthSession",
        "EndAuthSession", "CancelAuthTicket", "UserHasLicenseForApp", "BIsBehindNAT",
        "AdvertiseGame", "RequestEncryptedAppTicket", "GetEncryptedAppTicket", "GetGameBadgeLevel",
        "GetPlayerSteamLevel", "RequestStoreAuthURL", "BIsPhoneVerified", "BIsTwoFactorEnabled",
        "BIsPhoneIdentifying", "BIsPhoneRequiringVerification", "GetMarketEligibility", "GetDurationControl",
        "BSetDurationControlOnlineState")]
    public class SteamUserInterface : ISteamInterface
    {
        private const uint LegacyVoiceSampleRate = 11025;
        private static bool _refreshSteam2LoginLogged;

        public HSteamUser GetHSteamUser(IntPtr _)
        {
            return SteamEmulator.SteamUser.GetHSteamUser();
        }

        public bool BLoggedOn(IntPtr _)
        {
            return SteamEmulator.SteamUser.BLoggedOn();
        }

        public IntPtr GetSteamID(IntPtr _, IntPtr pSteamID)
        {
            return NativeSteamId.Write(pSteamID, SteamEmulator.SteamUser.GetSteamID());
        }

        public int InitiateGameConnection(IntPtr _, IntPtr pAuthBlob, int cbMaxAuthBlob, ulong steamIDGameServer, uint unIPServer, ushort usPortServer, bool bSecure)
        {
            return SteamEmulator.SteamUser.InitiateGameConnection(pAuthBlob, cbMaxAuthBlob, steamIDGameServer, unIPServer, usPortServer, bSecure);
        }

        public void TerminateGameConnection(IntPtr _, uint unIPServer, ushort usPortServer)
        {
            SteamEmulator.SteamUser.TerminateGameConnection(unIPServer, usPortServer);
        }

        public void TrackAppUsageEvent(IntPtr _, ulong gameID, int eAppUsageEvent, string pchExtraInfo)
        {
            SteamEmulator.SteamUser.TrackAppUsageEvent(gameID, eAppUsageEvent, pchExtraInfo);
        }

        public bool GetUserDataFolder(IntPtr _, IntPtr pchBuffer, int cubBuffer)
        {
            return SteamEmulator.SteamUser.GetUserDataFolder(pchBuffer, cubBuffer);
        }

        public void StartVoiceRecording(IntPtr _)
        {
            SteamEmulator.SteamUser.StartVoiceRecording();
        }

        public void StopVoiceRecording(IntPtr _)
        {
            SteamEmulator.SteamUser.StopVoiceRecording();
        }

        public EVoiceResult GetAvailableVoice__V013(IntPtr _, IntPtr pcbCompressed, IntPtr pcbUncompressed) =>
            SteamEmulator.SteamUser.GetAvailableVoice(
                pcbCompressed,
                pcbUncompressed,
                LegacyVoiceSampleRate);

        public EVoiceResult GetAvailableVoice(IntPtr _, IntPtr pcbCompressed, IntPtr pcbUncompressed_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
        {
            return SteamEmulator.SteamUser.GetAvailableVoice(pcbCompressed, pcbUncompressed_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
        }

        public EVoiceResult GetVoice__V013(
            IntPtr _,
            bool bWantCompressed,
            IntPtr pDestBuffer,
            uint cbDestBufferSize,
            IntPtr nBytesWritten,
            bool bWantUncompressed,
            IntPtr pUncompressedDestBuffer,
            uint cbUncompressedDestBufferSize,
            IntPtr nUncompressBytesWritten) =>
            SteamEmulator.SteamUser.GetVoice(
                bWantCompressed,
                pDestBuffer,
                cbDestBufferSize,
                nBytesWritten,
                bWantUncompressed,
                pUncompressedDestBuffer,
                cbUncompressedDestBufferSize,
                nUncompressBytesWritten,
                LegacyVoiceSampleRate);

        public EVoiceResult GetVoice(IntPtr _, bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, IntPtr nBytesWritten, bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint cbUncompressedDestBufferSize_Deprecated, IntPtr nUncompressBytesWritten_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
        {
            return SteamEmulator.SteamUser.GetVoice(bWantCompressed, pDestBuffer, cbDestBufferSize, nBytesWritten, bWantUncompressed_Deprecated, pUncompressedDestBuffer_Deprecated, cbUncompressedDestBufferSize_Deprecated, nUncompressBytesWritten_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
        }

        public EVoiceResult DecompressVoice__V013(
            IntPtr _,
            IntPtr pCompressed,
            uint cbCompressed,
            IntPtr pDestBuffer,
            uint cbDestBufferSize,
            IntPtr nBytesWritten) =>
            SteamEmulator.SteamUser.DecompressVoice(
                pCompressed,
                cbCompressed,
                pDestBuffer,
                cbDestBufferSize,
                nBytesWritten,
                LegacyVoiceSampleRate);

        public EVoiceResult DecompressVoice(IntPtr _, IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, IntPtr nBytesWritten, uint nDesiredSampleRate)
        {
            return SteamEmulator.SteamUser.DecompressVoice(pCompressed, cbCompressed, pDestBuffer, cbDestBufferSize, nBytesWritten, nDesiredSampleRate);
        }

        public uint GetAuthSessionTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
        {
            return SteamEmulator.SteamUser.GetAuthSessionTicket(pTicket, cbMaxTicket, out pcbTicket);
        }

        public HAuthTicket GetAuthSessionTicket__V023(IntPtr _, IntPtr pTicket, int cbMaxTicket, out uint pcbTicket, IntPtr pSteamNetworkingIdentity)
        {
            return SteamEmulator.SteamUser.GetAuthSessionTicket(pTicket, cbMaxTicket, out pcbTicket);
        }

        public int BeginAuthSession(IntPtr _, IntPtr pAuthTicket, int cbAuthTicket, ulong steamID)
        {
            return SteamEmulator.SteamUser.BeginAuthSession(pAuthTicket, cbAuthTicket, steamID);
        }

        public EBeginAuthSessionResult BeginAuthSession__V019(IntPtr _, IntPtr pAuthTicket, int cbAuthTicket, ulong steamID)
        {
            return (EBeginAuthSessionResult)SteamEmulator.SteamUser.BeginAuthSession(pAuthTicket, cbAuthTicket, steamID);
        }

        public void EndAuthSession(IntPtr _, ulong steamID)
        {
            SteamEmulator.SteamUser.EndAuthSession(steamID);
        }

        public void CancelAuthTicket(IntPtr _, HAuthTicket hAuthTicket)
        {
            SteamEmulator.SteamUser.CancelAuthTicket(hAuthTicket);
        }

        public int UserHasLicenseForApp(IntPtr _, ulong steamID, AppId_t appID)
        {
            return SteamEmulator.SteamUser.UserHasLicenseForApp(steamID, appID);
        }

        public EUserHasLicenseForAppResult UserHasLicenseForApp__V019(IntPtr _, ulong steamID, AppId_t appID)
        {
            return (EUserHasLicenseForAppResult)SteamEmulator.SteamUser.UserHasLicenseForApp(steamID, appID);
        }

        public int InitiateGameConnection__V009(IntPtr _, IntPtr pAuthBlob, int cbMaxAuthBlob, ulong steamIDGameServer, ulong gameID, uint unIPServer, ushort usPortServer, bool bSecure)
        {
            return SteamEmulator.SteamUser.InitiateGameConnection(pAuthBlob, cbMaxAuthBlob, steamIDGameServer, gameID, unIPServer, usPortServer, bSecure);
        }

        public void RefreshSteam2Login(IntPtr _)
        {
            if (!_refreshSteam2LoginLogged)
            {
                _refreshSteam2LoginLogged = true;
                SteamEmulator.Write("SteamUser", "RefreshSteam2Login not implemented");
            }
        }

        public EVoiceResult GetCompressedVoice(IntPtr _, IntPtr pDestBuffer, uint cbDestBufferSize, IntPtr nBytesWritten) =>
            SteamEmulator.SteamUser.GetVoice(
                true,
                pDestBuffer,
                cbDestBufferSize,
                nBytesWritten,
                false,
                IntPtr.Zero,
                0,
                IntPtr.Zero,
                LegacyVoiceSampleRate);

        public uint GetVoiceOptimalSampleRate(IntPtr _)
        {
            return SteamEmulator.SteamUser.GetVoiceOptimalSampleRate();
        }

        public bool BIsBehindNAT(IntPtr _)
        {
            return SteamEmulator.SteamUser.BIsBehindNAT();
        }

        public void AdvertiseGame(IntPtr _, ulong steamIDGameServer, uint unIPServer, ushort usPortServer)
        {
            SteamEmulator.SteamUser.AdvertiseGame(steamIDGameServer, unIPServer, usPortServer);
        }

        public SteamAPICall_t RequestEncryptedAppTicket(IntPtr _, IntPtr pDataToInclude, int cbDataToInclude)
        {
            return SteamEmulator.SteamUser.RequestEncryptedAppTicket(pDataToInclude, cbDataToInclude);
        }

        public bool GetEncryptedAppTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, IntPtr pcbTicket)
        {
            return SteamEmulator.SteamUser.GetEncryptedAppTicket(pTicket, cbMaxTicket, pcbTicket);
        }

        public int GetGameBadgeLevel(IntPtr _, int nSeries, bool bFoil)
        {
            return SteamEmulator.SteamUser.GetGameBadgeLevel(nSeries, bFoil);
        }

        public int GetPlayerSteamLevel(IntPtr _)
        {
            return SteamEmulator.SteamUser.GetPlayerSteamLevel();
        }

        public SteamAPICall_t RequestStoreAuthURL(IntPtr _, string pchRedirectURL)
        {
            return SteamEmulator.SteamUser.RequestStoreAuthURL(pchRedirectURL);
        }

        public bool BIsPhoneVerified(IntPtr _)
        {
            return SteamEmulator.SteamUser.BIsPhoneVerified();
        }

        public bool BIsTwoFactorEnabled(IntPtr _)
        {
            return SteamEmulator.SteamUser.BIsTwoFactorEnabled();
        }

        public bool BIsPhoneIdentifying(IntPtr _)
        {
            return SteamEmulator.SteamUser.BIsPhoneIdentifying();
        }

        public bool BIsPhoneRequiringVerification(IntPtr _)
        {
            return SteamEmulator.SteamUser.BIsPhoneRequiringVerification();
        }

        public SteamAPICall_t GetMarketEligibility(IntPtr _)
        {
            return SteamEmulator.SteamUser.GetMarketEligibility();
        }

        public SteamAPICall_t GetDurationControl(IntPtr _)
        {
            return SteamEmulator.SteamUser.GetDurationControl();
        }

        public bool BSetDurationControlOnlineState(IntPtr _, int eNewState)
        {
            return SteamEmulator.SteamUser.BSetDurationControlOnlineState(eNewState);
        }

        public int InitiateGameConnection_DEPRECATED(IntPtr _, IntPtr pAuthBlob, int cbMaxAuthBlob, ulong steamIDGameServer, uint unIPServer, ushort usPortServer, bool bSecure)
        {
            return SteamEmulator.SteamUser.InitiateGameConnection_DEPRECATED(pAuthBlob, cbMaxAuthBlob, steamIDGameServer, unIPServer, usPortServer, bSecure);
        }

        public void TerminateGameConnection_DEPRECATED(IntPtr _, uint unIPServer, ushort usPortServer)
        {
            SteamEmulator.SteamUser.TerminateGameConnection_DEPRECATED(unIPServer, usPortServer);
        }

        public HAuthTicket GetAuthTicketForWebApi(IntPtr _, string pchIdentity)
        {
            return SteamEmulator.SteamUser.GetAuthTicketForWebApi(pchIdentity);
        }

        private const int LogonStateNotLoggedOn = 0;
        private const int LogonStateLoggedOn = 2;
        private const int UniversePublic = 1;

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

            SteamEmulator.Write("SteamUser", method + " not implemented");
        }

        public void Init(IntPtr _, IntPtr cmcallback, IntPtr steam2auth) => LogStub("Init");

        public int ProcessCall(IntPtr _, int unk)
        {
            LogStub("ProcessCall");
            return 0;
        }

        public void LogOn_old(IntPtr _, IntPtr steamID)
        {
            SteamEmulator.SteamUser.BLoggedOn();
        }

        public void LogOn(IntPtr _, ulong steamID)
        {
            SteamEmulator.SteamUser.BLoggedOn();
        }

        public void LogOff(IntPtr _) => LogStub("LogOff");

        public bool BConnected(IntPtr _) => SteamEmulator.SteamUser.BLoggedOn();

        public int GetLogonState(IntPtr _) => SteamEmulator.SteamUser.BLoggedOn() ? LogonStateLoggedOn : LogonStateNotLoggedOn;

        public int CreateAccount(IntPtr _, string unk1, IntPtr unk2, IntPtr unk3, string unk4, int unk5, IntPtr unk6)
        {
            LogStub("CreateAccount");
            return 0;
        }

        public bool IsVACBanned(IntPtr _, int nGameID)
        {
            LogStub("IsVACBanned");
            return false;
        }

        public bool RequireShowVACBannedMessage(IntPtr _, int nGameID)
        {
            LogStub("RequireShowVACBannedMessage");
            return false;
        }

        public void AcknowledgeVACBanning(IntPtr _, int nGameID) => LogStub("AcknowledgeVACBanning");

        public bool GSSendLogonRequest(IntPtr _, IntPtr steamID)
        {
            LogStub("GSSendLogonRequest");
            return false;
        }

        public bool GSSendDisconnect(IntPtr _, IntPtr steamID)
        {
            LogStub("GSSendDisconnect");
            return false;
        }

        public bool GSSendStatusResponse(IntPtr _, IntPtr steamID, int nSecondsConnected, int nSecondsSinceLast)
        {
            LogStub("GSSendStatusResponse");
            return false;
        }

        public bool GSSetStatus(IntPtr _, int nAppIdServed, uint unServerFlags, int cPlayers, int cPlayersMax)
        {
            LogStub("GSSetStatus");
            return false;
        }

        public bool GSSetStatus__V002(IntPtr _, int nAppIdServed, uint unServerFlags, int cPlayers, int cPlayersMax, int cBotPlayers, int unGamePort, string pchServerName, string pchGameDir, string pchMapName, string pchVersion)
        {
            LogStub("GSSetStatus");
            return false;
        }

        public int NClientGameIDAdd(IntPtr _, int nGameID)
        {
            LogStub("NClientGameIDAdd");
            return 0;
        }

        public void RemoveClientGame(IntPtr _, int nClientGameID) => LogStub("RemoveClientGame");

        public void SetClientGameServer(IntPtr _, int nClientGameID, uint unIPServer, ushort usPortServer) => LogStub("SetClientGameServer");

        public void Test_SuspendActivity(IntPtr _) => LogStub("Test_SuspendActivity");

        public void Test_ResumeActivity(IntPtr _) => LogStub("Test_ResumeActivity");

        public void Test_SendVACResponse(IntPtr _, int nClientGameID, IntPtr pubResponse, int cubResponse) => LogStub("Test_SendVACResponse");

        public void Test_SetFakePrivateIP(IntPtr _, uint unIPPrivate) => LogStub("Test_SetFakePrivateIP");

        public void Test_SendBigMessage(IntPtr _) => LogStub("Test_SendBigMessage");

        public bool Test_BBigMessageResponseReceived(IntPtr _)
        {
            LogStub("Test_BBigMessageResponseReceived");
            return false;
        }

        public void Test_SetPktLossPct(IntPtr _, int nPct) => LogStub("Test_SetPktLossPct");

        public void Test_SetForceTCP(IntPtr _, bool bForceTCP) => LogStub("Test_SetForceTCP");

        public void Test_SetMaxUDPConnectionAttempts(IntPtr _, int unk1) => LogStub("Test_SetMaxUDPConnectionAttempts");

        public void Test_Heartbeat(IntPtr _) => LogStub("Test_Heartbeat");

        public void Test_FakeDisconnect(IntPtr _) => LogStub("Test_FakeDisconnect");

        public int Test_GetEUniverse(IntPtr _) => UniversePublic;

        public void SetSteam2Ticket(IntPtr _, IntPtr pubTicket, int cubTicket) => LogStub("SetSteam2Ticket");

        public void AddServerNetAddress(IntPtr _, uint unIP, ushort unPort) => LogStub("AddServerNetAddress");

        public bool SetEmail(IntPtr _, string pchEmail)
        {
            LogStub("SetEmail");
            return false;
        }

        public int GetSteamGameConnectToken(IntPtr _, IntPtr pBlob, int cbMaxBlob)
        {
            LogStub("GetSteamGameConnectToken");
            return 0;
        }

        public bool SetRegistryString(IntPtr _, int eRegistrySubTree, string pchKey, string pchValue)
        {
            LogStub("SetRegistryString");
            return false;
        }

        public bool GetRegistryString(IntPtr _, int eRegistrySubTree, string pchKey, IntPtr pchValue, int cbValue)
        {
            LogStub("GetRegistryString");
            return false;
        }

        public bool SetRegistryInt(IntPtr _, int eRegistrySubTree, string pchKey, int iValue)
        {
            LogStub("SetRegistryInt");
            return false;
        }

        public bool GetRegistryInt(IntPtr _, int eRegistrySubTree, string pchKey, IntPtr piValue)
        {
            LogStub("GetRegistryInt");
            return false;
        }

        public int InitiateGameConnection__V004(IntPtr _, IntPtr pAuthBlob, int cbMaxAuthBlob, ulong steamIDGameServer, int nGameAppID, uint unIPServer, ushort usPortServer, bool bSecure)
        {
            return SteamEmulator.SteamUser.InitiateGameConnection(pAuthBlob, cbMaxAuthBlob, steamIDGameServer, unchecked((uint)nGameAppID), unIPServer, usPortServer, bSecure);
        }

        public int InitiateGameConnection__V007(IntPtr _, IntPtr pAuthBlob, int cbMaxAuthBlob, ulong steamIDGameServer, ulong gameID, uint unIPServer, ushort usPortServer, bool bSecure, IntPtr pvSteam2GetEncryptionKey, int cbSteam2GetEncryptionKey)
        {
            return SteamEmulator.SteamUser.InitiateGameConnection(pAuthBlob, cbMaxAuthBlob, steamIDGameServer, gameID, unIPServer, usPortServer, bSecure);
        }

        public void SetSelfAsPrimaryChatDestination(IntPtr _) => LogStub("SetSelfAsPrimaryChatDestination");

        public bool IsPrimaryChatDestination(IntPtr _)
        {
            LogStub("IsPrimaryChatDestination");
            return false;
        }

        public void RequestLegacyCDKey(IntPtr _, uint nAppID) => LogStub("RequestLegacyCDKey");

        public bool SendGuestPassByEmail(IntPtr _, string pchEmailAccount, ulong gidGuestPassID, bool bResending)
        {
            LogStub("SendGuestPassByEmail");
            return false;
        }

        public bool SendGuestPassByAccountID(IntPtr _, uint uAccountID, ulong gidGuestPassID, bool bResending)
        {
            LogStub("SendGuestPassByAccountID");
            return false;
        }

        public bool AckGuestPass(IntPtr _, string pchGuestPassCode)
        {
            LogStub("AckGuestPass");
            return false;
        }

        public bool RedeemGuestPass(IntPtr _, string pchGuestPassCode)
        {
            LogStub("RedeemGuestPass");
            return false;
        }

        public uint GetGuestPassToGiveCount(IntPtr _)
        {
            LogStub("GetGuestPassToGiveCount");
            return 0;
        }

        public uint GetGuestPassToRedeemCount(IntPtr _)
        {
            LogStub("GetGuestPassToRedeemCount");
            return 0;
        }

        public uint GetGuestPassLastUpdateTime(IntPtr _)
        {
            LogStub("GetGuestPassLastUpdateTime");
            return 0;
        }

        public bool GetGuestPassToGiveInfo(IntPtr _, uint nPassIndex, IntPtr pgidGuestPassID, IntPtr pnPackageID, IntPtr pRTime32Created, IntPtr pRTime32Expiration, IntPtr pRTime32Sent, IntPtr pRTime32Redeemed, IntPtr pchRecipientAddress, int cRecipientAddressSize)
        {
            LogStub("GetGuestPassToGiveInfo");
            return false;
        }

        public bool GetGuestPassToRedeemInfo(IntPtr _, uint nPassIndex, IntPtr pgidGuestPassID, IntPtr pnPackageID, IntPtr pRTime32Created, IntPtr pRTime32Expiration, IntPtr pRTime32Sent, IntPtr pRTime32Redeemed)
        {
            LogStub("GetGuestPassToRedeemInfo");
            return false;
        }

        public bool GetGuestPassToRedeemSenderAddress(IntPtr _, uint nPassIndex, IntPtr pchSenderAddress, int cSenderAddressSize)
        {
            LogStub("GetGuestPassToRedeemSenderAddress");
            return false;
        }

        public bool GetGuestPassToRedeemSenderName(IntPtr _, uint nPassIndex, IntPtr pchSenderName, int cSenderNameSize)
        {
            LogStub("GetGuestPassToRedeemSenderName");
            return false;
        }

        public void AcknowledgeMessageByGID(IntPtr _, string pchMessageGID) => LogStub("AcknowledgeMessageByGID");

        public bool SetLanguage(IntPtr _, string pchLanguage)
        {
            LogStub("SetLanguage");
            return false;
        }

        public void SetAccountName(IntPtr _, string pchAccountName) => LogStub("SetAccountName");

        public void SetPassword(IntPtr _, string pchPassword) => LogStub("SetPassword");

        public void SetAccountCreationTime(IntPtr _, uint rt) => LogStub("SetAccountCreationTime");
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SKYNET.Helpers;
using SKYNET.Managers;

using SteamAPICall_t = System.UInt64;
using HAuthTicket = System.UInt32;

namespace SKYNET.Steamworks.Interfaces
{
    [InterfaceLayout("SteamGameServer001",
        "GSSendUserConnect__V001", "GSSendUserDisconnect__V001", "GSSendUserStatusResponse", "Obsolete_GSSetStatus")]
    [InterfaceLayout("SteamGameServer002",
        "LogOn__V009", "LogOff", "BLoggedOn", "GSSetSpawnCount",
        "GSGetSteam2GetEncryptionKeyToSendToNewClient", "GSSendSteam2UserConnect", "GSSendSteam3UserConnect", "GSRemoveUserConnect",
        "GSSendUserDisconnect", "GSSendUserStatusResponse", "Obsolete_GSSetStatus", "GSUpdateStatus__V002",
        "BSecure", "GetSteamID", "GSSetServerType__V002", "GSSetServerType2",
        "GSUpdateStatus2", "GSCreateUnauthenticatedUser", "GSSetUserData", "GSUpdateSpectatorPort",
        "GSSetGameType")]
    [InterfaceLayout("SteamGameServer003",
        "LogOn__V009", "LogOff", "BLoggedOn", "BSecure",
        "GetSteamID", "GSGetSteam2GetEncryptionKeyToSendToNewClient", "GSSendUserConnect", "GSRemoveUserConnect",
        "GSSendUserDisconnect", "GSSetSpawnCount", "GSSetServerType", "GSUpdateStatus",
        "GSCreateUnauthenticatedUser", "GSSetUserData", "GSUpdateSpectatorPort", "GSSetGameType",
        "GSGetUserAchievementStatus")]
    [InterfaceLayout("SteamGameServer004",
        "LogOn__V009", "LogOff", "BLoggedOn", "BSecure",
        "GetSteamID", "SendUserConnectAndAuthenticate__V004", "CreateUnauthenticatedUserConnection", "SendUserDisconnect",
        "BUpdateUserData__V009", "BSetServerType__V004", "UpdateServerStatus", "UpdateSpectatorPort",
        "SetGameType", "BGetUserAchievementStatus")]
    [InterfaceLayout("SteamGameServer005",
        "LogOn__V009", "LogOff", "BLoggedOn", "BSecure",
        "GetSteamID", "SendUserConnectAndAuthenticate", "CreateUnauthenticatedUserConnection", "SendUserDisconnect",
        "BUpdateUserData__V009", "BSetServerType", "UpdateServerStatus", "UpdateSpectatorPort",
        "SetGameType", "BGetUserAchievementStatus")]
    [InterfaceLayout("SteamGameServer008",
        "LogOn__V009", "LogOff", "BLoggedOn", "BSecure",
        "GetSteamID", "SendUserConnectAndAuthenticate", "CreateUnauthenticatedUserConnection", "SendUserDisconnect",
        "BUpdateUserData__V009", "BSetServerType", "UpdateServerStatus", "UpdateSpectatorPort",
        "SetGameType", "BGetUserAchievementStatus", "GetGameplayStats", "RequestUserGroupStatus",
        "GetPublicIP_old")]
    [InterfaceLayout("SteamGameServer009",
        "LogOn__V009", "LogOff", "BLoggedOn", "BSecure",
        "GetSteamID", "SendUserConnectAndAuthenticate", "CreateUnauthenticatedUserConnection", "SendUserDisconnect",
        "BUpdateUserData__V009", "BSetServerType", "UpdateServerStatus", "UpdateSpectatorPort",
        "SetGameType", "BGetUserAchievementStatus", "GetGameplayStats", "RequestUserGroupStatus",
        "GetPublicIP_old", "SetGameData__V009", "UserHasLicenseForApp")]
    [InterfaceLayout("SteamGameServer010",
        "LogOn__V009", "LogOff", "BLoggedOn", "BSecure",
        "GetSteamID", "SendUserConnectAndAuthenticate", "CreateUnauthenticatedUserConnection", "SendUserDisconnect",
        "BUpdateUserData__V009", "BSetServerType", "UpdateServerStatus", "UpdateSpectatorPort",
        "SetGameTags", "GetGameplayStats", "GetServerReputation", "RequestUserGroupStatus",
        "GetPublicIP_old", "SetGameData__V009", "UserHasLicenseForApp", "GetAuthSessionTicket",
        "BeginAuthSession", "EndAuthSession", "CancelAuthTicket")]
    [InterfaceLayout("SteamGameServer011",
        "InitGameServer", "SetProduct", "SetGameDescription", "SetModDir",
        "SetDedicatedServer", "LogOn__V011", "LogOnAnonymous", "LogOff",
        "BLoggedOn", "BSecure", "GetSteamID", "WasRestartRequested",
        "SetMaxPlayerCount", "SetBotPlayerCount", "SetServerName", "SetMapName",
        "SetPasswordProtected", "SetSpectatorPort", "SetSpectatorServerName", "ClearAllKeyValues",
        "SetKeyValue", "SetGameTags", "SetGameData", "SetRegion",
        "SendUserConnectAndAuthenticate", "CreateUnauthenticatedUserConnection", "SendUserDisconnect", "BUpdateUserData",
        "GetAuthSessionTicket", "BeginAuthSession", "EndAuthSession", "CancelAuthTicket",
        "UserHasLicenseForApp", "RequestUserGroupStatus", "GetGameplayStats", "GetServerReputation",
        "GetPublicIP_old", "HandleIncomingPacket", "GetNextOutgoingPacket", "EnableHeartbeats",
        "SetHeartbeatInterval", "ForceHeartbeat", "AssociateWithClan", "ComputeNewPlayerCompatibility")]
    [InterfaceLayout("SteamGameServer012",
        "InitGameServer", "SetProduct", "SetGameDescription", "SetModDir",
        "SetDedicatedServer", "LogOn", "LogOnAnonymous", "LogOff",
        "BLoggedOn", "BSecure", "GetSteamID", "WasRestartRequested",
        "SetMaxPlayerCount", "SetBotPlayerCount", "SetServerName", "SetMapName",
        "SetPasswordProtected", "SetSpectatorPort__V012", "SetSpectatorServerName", "ClearAllKeyValues",
        "SetKeyValue", "SetGameTags", "SetGameData", "SetRegion",
        "SendUserConnectAndAuthenticate", "CreateUnauthenticatedUserConnection", "SendUserDisconnect", "BUpdateUserData",
        "GetAuthSessionTicket", "BeginAuthSession", "EndAuthSession", "CancelAuthTicket",
        "UserHasLicenseForApp", "RequestUserGroupStatus", "GetGameplayStats", "GetServerReputation",
        "GetPublicIP_old", "HandleIncomingPacket", "GetNextOutgoingPacket", "EnableHeartbeats",
        "SetHeartbeatInterval", "ForceHeartbeat", "AssociateWithClan", "ComputeNewPlayerCompatibility")]
    [InterfaceLayout("SteamGameServer013",
        "InitGameServer", "SetProduct", "SetGameDescription", "SetModDir",
        "SetDedicatedServer", "LogOn", "LogOnAnonymous", "LogOff",
        "BLoggedOn", "BSecure", "GetSteamID", "WasRestartRequested",
        "SetMaxPlayerCount", "SetBotPlayerCount", "SetServerName", "SetMapName",
        "SetPasswordProtected", "SetSpectatorPort", "SetSpectatorServerName", "ClearAllKeyValues",
        "SetKeyValue", "SetGameTags", "SetGameData", "SetRegion",
        "SendUserConnectAndAuthenticate__V013", "CreateUnauthenticatedUserConnection", "SendUserDisconnect__V013", "BUpdateUserData",
        "GetAuthSessionTicket", "BeginAuthSession", "EndAuthSession", "CancelAuthTicket",
        "UserHasLicenseForApp", "RequestUserGroupStatus", "GetGameplayStats", "GetServerReputation",
        "GetPublicIP", "HandleIncomingPacket", "GetNextOutgoingPacket", "EnableHeartbeats",
        "SetHeartbeatInterval", "ForceHeartbeat", "AssociateWithClan", "ComputeNewPlayerCompatibility")]
    [InterfaceLayout("SteamGameServer014",
        "InitGameServer", "SetProduct", "SetGameDescription", "SetModDir",
        "SetDedicatedServer", "LogOn", "LogOnAnonymous", "LogOff",
        "BLoggedOn", "BSecure", "GetSteamID", "WasRestartRequested",
        "SetMaxPlayerCount", "SetBotPlayerCount", "SetServerName", "SetMapName",
        "SetPasswordProtected", "SetSpectatorPort", "SetSpectatorServerName", "ClearAllKeyValues",
        "SetKeyValue", "SetGameTags", "SetGameData", "SetRegion",
        "SetAdvertiseServerActive", "GetAuthSessionTicket", "BeginAuthSession", "EndAuthSession",
        "CancelAuthTicket", "UserHasLicenseForApp", "RequestUserGroupStatus", "GetGameplayStats",
        "GetServerReputation", "GetPublicIP", "HandleIncomingPacket", "GetNextOutgoingPacket",
        "AssociateWithClan", "ComputeNewPlayerCompatibility", "SendUserConnectAndAuthenticate__V013", "CreateUnauthenticatedUserConnection",
        "SendUserDisconnect__V013", "BUpdateUserData", "SetMasterServerHeartbeatInterval_DEPRECATED", "ForceMasterServerHeartbeat_DEPRECATED")]
    [InterfaceLayout("SteamGameServer015",
        "InitGameServer", "SetProduct", "SetGameDescription", "SetModDir",
        "SetDedicatedServer", "LogOn", "LogOnAnonymous", "LogOff",
        "BLoggedOn", "BSecure", "GetSteamID", "WasRestartRequested",
        "SetMaxPlayerCount", "SetBotPlayerCount", "SetServerName", "SetMapName",
        "SetPasswordProtected", "SetSpectatorPort", "SetSpectatorServerName", "ClearAllKeyValues",
        "SetKeyValue", "SetGameTags", "SetGameData", "SetRegion",
        "SetAdvertiseServerActive", "GetAuthSessionTicket__V015", "BeginAuthSession", "EndAuthSession",
        "CancelAuthTicket", "UserHasLicenseForApp", "RequestUserGroupStatus", "GetGameplayStats",
        "GetServerReputation", "GetPublicIP", "HandleIncomingPacket", "GetNextOutgoingPacket__V015",
        "AssociateWithClan", "ComputeNewPlayerCompatibility", "SendUserConnectAndAuthenticate_DEPRECATED", "CreateUnauthenticatedUserConnection",
        "SendUserDisconnect_DEPRECATED", "BUpdateUserData", "SetMasterServerHeartbeatInterval_DEPRECATED", "ForceMasterServerHeartbeat_DEPRECATED")]
    public class SteamGameServerInterface : ISteamInterface
    {
        public bool InitGameServer(IntPtr _, uint unIP, ushort usGamePort, ushort usQueryPort, uint unFlags, uint nGameAppId, string pchVersionString)
        {
            return SteamEmulator.SteamGameServer.InitGameServer(unIP, usGamePort, usQueryPort, unFlags, nGameAppId, pchVersionString);
        }

        public void SetProduct(IntPtr _, string pszProduct)
        {
            SteamEmulator.SteamGameServer.SetProduct(pszProduct);
        }

        public void SetGameDescription(IntPtr _, string pszGameDescription)
        {
            SteamEmulator.SteamGameServer.SetGameDescription(pszGameDescription);
        }

        public void SetModDir(IntPtr _, string pszModDir)
        {
            SteamEmulator.SteamGameServer.SetModDir(pszModDir);
        }

        public void SetDedicatedServer(IntPtr _, bool bDedicated)
        {
            SteamEmulator.SteamGameServer.SetDedicatedServer(bDedicated);
        }

        public void LogOn(IntPtr _, string pszToken)
        {
            SteamEmulator.SteamGameServer.LogOn(pszToken);
        }

        public void LogOnAnonymous(IntPtr _)
        {
            SteamEmulator.SteamGameServer.LogOnAnonymous();
        }

        public void LogOff(IntPtr _)
        {
            SteamEmulator.SteamGameServer.LogOff();
        }

        public bool BLoggedOn(IntPtr _)
        {
            return SteamEmulator.SteamGameServer.BLoggedOn();
        }

        public bool BSecure(IntPtr _)
        {
            return SteamEmulator.SteamGameServer.BSecure();
        }

        public IntPtr GetSteamID(IntPtr _, IntPtr pSteamID)
        {
            return NativeSteamId.Write(pSteamID, SteamEmulator.SteamGameServer.GetSteamID());
        }

        public bool WasRestartRequested(IntPtr _)
        {
            return SteamEmulator.SteamGameServer.WasRestartRequested();
        }

        public void SetMaxPlayerCount(IntPtr _, int cPlayersMax)
        {
            SteamEmulator.SteamGameServer.SetMaxPlayerCount(cPlayersMax);
        }

        public void SetBotPlayerCount(IntPtr _, int cBotplayers)
        {
            SteamEmulator.SteamGameServer.SetBotPlayerCount(cBotplayers);
        }

        public void SetServerName(IntPtr _, string pszServerName)
        {
            SteamEmulator.SteamGameServer.SetServerName(pszServerName);
        }

        public void SetMapName(IntPtr _, string pszMapName)
        {
            SteamEmulator.SteamGameServer.SetMapName(pszMapName);
        }

        public void SetPasswordProtected(IntPtr _, bool bPasswordProtected)
        {
            SteamEmulator.SteamGameServer.SetPasswordProtected(bPasswordProtected);
        }

        public void SetSpectatorPort(IntPtr _, ushort unSpectatorPort) => SteamEmulator.SteamGameServer.SetSpectatorPort(unSpectatorPort);

        public void SetSpectatorServerName(IntPtr _, string pszSpectatorServerName)
        {
            SteamEmulator.SteamGameServer.SetSpectatorServerName(pszSpectatorServerName);
        }

        public void ClearAllKeyValues(IntPtr _)
        {
            SteamEmulator.SteamGameServer.ClearAllKeyValues();
        }

        public void SetKeyValue(IntPtr _, string pKey, string pValue)
        {
            SteamEmulator.SteamGameServer.SetKeyValue(pKey, pValue);
        }

        public void SetGameTags(IntPtr _, string pchGameTags)
        {
            SteamEmulator.SteamGameServer.SetGameTags(pchGameTags);
        }

        public void SetGameData(IntPtr _, string pchGameData)
        {
            SteamEmulator.SteamGameServer.SetGameData(pchGameData);
        }

        public void SetRegion(IntPtr _, string pszRegion)
        {
            SteamEmulator.SteamGameServer.SetRegion(pszRegion);
        }

        public void SetAdvertiseServerActive(IntPtr _, bool bActive)
        {
            SteamEmulator.SteamGameServer.SetAdvertiseServerActive(bActive);
        }

        public uint GetAuthSessionTicket__V015(IntPtr _, IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket, IntPtr pSnid)
        {
            return SteamEmulator.SteamGameServer.GetAuthSessionTicket(pTicket, cbMaxTicket, ref pcbTicket);
        }

        public int BeginAuthSession(IntPtr _, IntPtr pAuthTicket, int cbAuthTicket, ulong steamID)
        {
            return SteamEmulator.SteamGameServer.BeginAuthSession(pAuthTicket, cbAuthTicket, steamID);
        }

        public void EndAuthSession(IntPtr _, ulong steamID)
        {
            SteamEmulator.SteamGameServer.EndAuthSession(steamID);
        }

        public void CancelAuthTicket(IntPtr _, HAuthTicket hAuthTicket)
        {
            SteamEmulator.SteamGameServer.CancelAuthTicket(hAuthTicket);
        }

        public int UserHasLicenseForApp(IntPtr _, ulong steamID, uint appID)
        {
            return SteamEmulator.SteamGameServer.UserHasLicenseForApp(steamID, appID);
        }

        public bool RequestUserGroupStatus(IntPtr _, ulong steamIDUser, ulong steamIDGroup)
        {
            return SteamEmulator.SteamGameServer.RequestUserGroupStatus(steamIDUser, steamIDGroup);
        }

        public void GetGameplayStats(IntPtr _)
        {
            SteamEmulator.SteamGameServer.GetGameplayStats();
        }

        public SteamAPICall_t GetServerReputation(IntPtr _)
        {
            return SteamEmulator.SteamGameServer.GetServerReputation();
        }

        public IntPtr GetPublicIP(IntPtr arg0, IntPtr arg1) => SteamEmulator.SteamGameServer.GetPublicIP(arg0, arg1);

        public bool HandleIncomingPacket(IntPtr _, IntPtr pData, int cbData, uint srcIP, ushort srcPort)
        {
            return SteamEmulator.SteamGameServer.HandleIncomingPacket(pData, cbData, srcIP, srcPort);
        }

        public int GetNextOutgoingPacket__V015(IntPtr _, IntPtr pOut, int cbMaxOut, IntPtr pNetAdr, IntPtr pPort) { return SteamEmulator.SteamGameServer.GetNextOutgoingPacket(pOut, cbMaxOut, pNetAdr, pPort); }

        public SteamAPICall_t AssociateWithClan(IntPtr _, ulong steamIDClan)
        {
            return SteamEmulator.SteamGameServer.AssociateWithClan(steamIDClan);
        }

        public SteamAPICall_t ComputeNewPlayerCompatibility(IntPtr _, ulong steamIDNewPlayer)
        {
            return SteamEmulator.SteamGameServer.ComputeNewPlayerCompatibility(steamIDNewPlayer);
        }

        public bool SendUserConnectAndAuthenticate_DEPRECATED(IntPtr _, uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, IntPtr pSteamIDUser) { return SteamEmulator.SteamGameServer.SendUserConnectAndAuthenticate_DEPRECATED(unIPClient, pvAuthBlob, cubAuthBlobSize, pSteamIDUser); }

        public IntPtr CreateUnauthenticatedUserConnection(IntPtr _, IntPtr pSteamID)
        {
            return NativeSteamId.Write(pSteamID, SteamEmulator.SteamGameServer.CreateUnauthenticatedUserConnection());
        }

        public void SendUserDisconnect_DEPRECATED(IntPtr _, ulong steamIDUser) { SteamEmulator.SteamGameServer.SendUserDisconnect_DEPRECATED(steamIDUser); }

        public bool BUpdateUserData(IntPtr _, ulong steamIDUser, string pchPlayerName, uint uScore)
        {
            return SteamEmulator.SteamGameServer.BUpdateUserData(steamIDUser, pchPlayerName, uScore);
        }

        public void SetMasterServerHeartbeatInterval_DEPRECATED(IntPtr _, int iHeartbeatInterval)
        {
            SteamEmulator.SteamGameServer.SetMasterServerHeartbeatInterval_DEPRECATED(iHeartbeatInterval);
        }

        public void ForceMasterServerHeartbeat_DEPRECATED(IntPtr _)
        {
            SteamEmulator.SteamGameServer.ForceMasterServerHeartbeat_DEPRECATED();
        }

        public uint GetAuthSessionTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
        {
            return SteamEmulator.SteamGameServer.GetAuthSessionTicket(pTicket, cbMaxTicket, ref pcbTicket);
        }

        public int GetNextOutgoingPacket(IntPtr _, IntPtr pOut, int cbMaxOut, IntPtr pNetAdr, IntPtr pPort)
        {
            if (pNetAdr != IntPtr.Zero)
            {
                System.Runtime.InteropServices.Marshal.WriteInt32(pNetAdr, 0);
            }

            if (pPort != IntPtr.Zero)
            {
                System.Runtime.InteropServices.Marshal.WriteInt16(pPort, 0);
            }

            return SteamEmulator.SteamGameServer.GetNextOutgoingPacket(pOut, cbMaxOut, 0, 0);
        }

        public bool SendUserConnectAndAuthenticate__V013(IntPtr _, uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, IntPtr pSteamIDUser) => SteamEmulator.SteamGameServer.SendUserConnectAndAuthenticate_DEPRECATED(unIPClient, pvAuthBlob, cubAuthBlobSize, pSteamIDUser);

        public void SendUserDisconnect__V013(IntPtr _, ulong steamIDUser) => SteamEmulator.SteamGameServer.SendUserDisconnect_DEPRECATED(steamIDUser);

        public void EnableHeartbeats(IntPtr _, bool bActive)
        {
            SteamEmulator.SteamGameServer.EnableHeartbeats(bActive);
        }

        public void SetHeartbeatInterval(IntPtr _, int iHeartbeatInterval)
        {
            SteamEmulator.SteamGameServer.SetHeartbeatInterval(iHeartbeatInterval);
        }

        public void ForceHeartbeat(IntPtr _)
        {
            SteamEmulator.SteamGameServer.ForceHeartbeat();
        }

        public void SetSpectatorPort__V012(IntPtr _, ushort unSpectatorPort)
        {
            SteamEmulator.SteamGameServer.SetSpectatorPort((int)unSpectatorPort);
        }

        public bool SendUserConnectAndAuthenticate(IntPtr _, uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, IntPtr pSteamIDUser)
        {
            return SteamEmulator.SteamGameServer.SendUserConnectAndAuthenticate(unIPClient, pvAuthBlob, cubAuthBlobSize, pSteamIDUser);
        }

        public void SendUserDisconnect(IntPtr _, ulong steamIDUser)
        {
            SteamEmulator.SteamGameServer.SendUserDisconnect(steamIDUser);
        }

        public uint GetPublicIP_old(IntPtr _)
        {
            return SteamEmulator.SteamGameServer.GetPublicIP_old();
        }

        public void LogOn__V009(IntPtr _)
        {
            SteamEmulator.SteamGameServer.LogOnAnonymous();
        }

        public bool BUpdateUserData__V009(IntPtr _, ulong steamIDUser, IntPtr pchPlayerName, uint uScore)
        {
            SteamEmulator.Write("SteamGameServer009", $"BUpdateUserData name=0x{pchPlayerName.ToInt64():X}");
            return SteamEmulator.SteamGameServer.BUpdateUserData(steamIDUser, ReadAnsi(pchPlayerName), uScore);
        }

        public bool BSetServerType(
            IntPtr _,
            uint unServerFlags,
            uint unGameIP,
            ushort unGamePort,
            ushort unSpectatorPort,
            ushort usQueryPort,
            IntPtr pchGameDir,
            IntPtr pchVersion,
            bool bLANMode)
        {
            SteamEmulator.Write(
                "SteamGameServer009",
                $"BSetServerType flags={unServerFlags} ip={unGameIP} gamePort={unGamePort} spectatorPort={unSpectatorPort} queryPort={usQueryPort} gameDir=0x{pchGameDir.ToInt64():X} version=0x{pchVersion.ToInt64():X} lan={bLANMode}");
            bool result = SteamEmulator.SteamGameServer.InitGameServer(
                unGameIP,
                unGamePort,
                usQueryPort,
                unServerFlags,
                SteamEmulator.InternalAppId,
                ReadAnsi(pchVersion));
            SteamEmulator.SteamGameServer.SetModDir(ReadAnsi(pchGameDir));
            SteamEmulator.SteamGameServer.SetSpectatorPort(unSpectatorPort);
            return result;
        }

        public bool BSetServerType__V004(
            IntPtr _,
            int nGameAppId,
            uint unServerFlags,
            uint unGameIP,
            ushort unGamePort,
            ushort unSpectatorPort,
            ushort usQueryPort,
            IntPtr pchGameDir,
            IntPtr pchVersion,
            bool bLANMode)
        {
            SteamEmulator.Write(
                "SteamGameServer004",
                $"BSetServerType appId={nGameAppId} flags={unServerFlags} ip={unGameIP} gamePort={unGamePort} spectatorPort={unSpectatorPort} queryPort={usQueryPort} gameDir=0x{pchGameDir.ToInt64():X} version=0x{pchVersion.ToInt64():X} lan={bLANMode}");
            bool result = SteamEmulator.SteamGameServer.InitGameServer(
                unGameIP,
                unGamePort,
                usQueryPort,
                unServerFlags,
                unchecked((uint)nGameAppId),
                ReadAnsi(pchVersion));
            SteamEmulator.SteamGameServer.SetModDir(ReadAnsi(pchGameDir));
            SteamEmulator.SteamGameServer.SetSpectatorPort(unSpectatorPort);
            return result;
        }

        public void SendUserConnectAndAuthenticate__V004(IntPtr _, ulong steamIDUser, uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize)
        {
            SteamEmulator.SteamGameServer.SendUserConnectAndAuthenticate(unIPClient, pvAuthBlob, cubAuthBlobSize, out ulong _authenticatedUser);
        }

        public void LogOn__V011(IntPtr _, string pszAccountName, string pszPassword)
        {
            // Account/password logon cannot be validated here, so it is treated as an anonymous logon.
            SteamEmulator.SteamGameServer.LogOnAnonymous();
        }

        public void UpdateServerStatus(
            IntPtr _,
            int cPlayers,
            int cPlayersMax,
            int cBotPlayers,
            IntPtr pchServerName,
            IntPtr pSpectatorServerName,
            IntPtr pchMapName)
        {
            SteamEmulator.Write(
                "SteamGameServer009",
                $"UpdateServerStatus serverName=0x{pchServerName.ToInt64():X} spectatorName=0x{pSpectatorServerName.ToInt64():X} map=0x{pchMapName.ToInt64():X}");
            SteamEmulator.SteamGameServer.SetMaxPlayerCount(cPlayersMax);
            SteamEmulator.SteamGameServer.SetBotPlayerCount(cBotPlayers);
            SteamEmulator.SteamGameServer.SetServerName(ReadAnsi(pchServerName));
            SteamEmulator.SteamGameServer.SetSpectatorServerName(ReadAnsi(pSpectatorServerName));
            SteamEmulator.SteamGameServer.SetMapName(ReadAnsi(pchMapName));
        }

        public void UpdateSpectatorPort(IntPtr _, ushort unSpectatorPort)
        {
            SteamEmulator.SteamGameServer.SetSpectatorPort(unSpectatorPort);
        }

        public void SetGameType(IntPtr _, IntPtr pchGameType)
        {
            SteamEmulator.Write("SteamGameServer009", $"SetGameType value=0x{pchGameType.ToInt64():X}");
            SteamEmulator.SteamGameServer.SetGameTags(ReadAnsi(pchGameType));
        }

        public bool BGetUserAchievementStatus(IntPtr _, ulong steamID, IntPtr pchAchievementName)
        {
            SteamEmulator.Write("SteamGameServer009", $"BGetUserAchievementStatus name=0x{pchAchievementName.ToInt64():X}");
            return false;
        }

        public void SetGameData__V009(IntPtr _, IntPtr pchGameData)
        {
            SteamEmulator.Write("SteamGameServer009", $"SetGameData value=0x{pchGameData.ToInt64():X}");
            SteamEmulator.SteamGameServer.SetGameData(ReadAnsi(pchGameData));
        }

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

            SteamEmulator.Write("SteamGameServer", method + " not implemented");
        }

        public bool GSSendUserConnect__V001(IntPtr _, ulong steamID, uint unIPPublic, uint unk)
        {
            LogStub("GSSendUserConnect");
            return false;
        }

        public bool GSSendUserConnect(IntPtr _, uint unUserID, uint unIPPublic, ushort usPort, IntPtr pvCookie, uint cubCookie)
        {
            return SteamEmulator.SteamGameServer.SendUserConnectAndAuthenticate(unIPPublic, pvCookie, cubCookie, out ulong _authenticatedUser);
        }

        public bool GSSendSteam3UserConnect(IntPtr _, ulong steamID, uint unIPPublic, IntPtr pvCookie, uint cubCookie)
        {
            return SteamEmulator.SteamGameServer.SendUserConnectAndAuthenticate(unIPPublic, pvCookie, cubCookie, out ulong _authenticatedUser);
        }

        public bool GSSendSteam2UserConnect(IntPtr _, uint unUserID, IntPtr pvRawKey, uint unKeyLen, uint unIPPublic, ushort usPort, IntPtr pvCookie, uint cubCookie)
        {
            LogStub("GSSendSteam2UserConnect");
            return false;
        }

        public bool GSGetSteam2GetEncryptionKeyToSendToNewClient(IntPtr _, IntPtr pvEncryptionKey, IntPtr pcbEncryptionKey, uint cbMaxEncryptionKey)
        {
            LogStub("GSGetSteam2GetEncryptionKeyToSendToNewClient");
            return false;
        }

        public bool GSRemoveUserConnect(IntPtr _, uint unUserID)
        {
            LogStub("GSRemoveUserConnect");
            return false;
        }

        public void GSSetSpawnCount(IntPtr _, uint ucSpawn) => LogStub("GSSetSpawnCount");

        public bool GSSendUserDisconnect__V001(IntPtr _, ulong steamID)
        {
            SteamEmulator.SteamGameServer.SendUserDisconnect(steamID);
            return true;
        }

        public bool GSSendUserDisconnect(IntPtr _, ulong steamID, uint unUserID)
        {
            SteamEmulator.SteamGameServer.SendUserDisconnect(steamID);
            return true;
        }

        public bool GSSendUserStatusResponse(IntPtr _, ulong steamID, int nSecondsConnected, int nSecondsSinceLast)
        {
            LogStub("GSSendUserStatusResponse");
            return false;
        }

        public bool Obsolete_GSSetStatus(IntPtr _, int nAppIdServed, uint unServerFlags, int cPlayers, int cPlayersMax, int cBotPlayers, int unGamePort, IntPtr pchServerName, IntPtr pchGameDir, IntPtr pchMapName, IntPtr pchVersion)
        {
            if (!SteamEmulator.SteamGameServer.LoggedIn)
            {
                SteamEmulator.SteamGameServer.InitGameServer(0, unGamePort, unGamePort, unServerFlags, unchecked((uint)nAppIdServed), ReadAnsi(pchVersion));
                SteamEmulator.SteamGameServer.SetModDir(ReadAnsi(pchGameDir));
            }

            SteamEmulator.SteamGameServer.SetMaxPlayerCount(cPlayersMax);
            SteamEmulator.SteamGameServer.SetBotPlayerCount(cBotPlayers);
            SteamEmulator.SteamGameServer.SetServerName(ReadAnsi(pchServerName));
            SteamEmulator.SteamGameServer.SetMapName(ReadAnsi(pchMapName));
            return true;
        }

        public bool GSUpdateStatus__V002(IntPtr _, int cPlayers, int cPlayersMax, int cBotPlayers, IntPtr pchServerName, IntPtr pchMapName)
        {
            SteamEmulator.SteamGameServer.SetMaxPlayerCount(cPlayersMax);
            SteamEmulator.SteamGameServer.SetBotPlayerCount(cBotPlayers);
            SteamEmulator.SteamGameServer.SetServerName(ReadAnsi(pchServerName));
            SteamEmulator.SteamGameServer.SetMapName(ReadAnsi(pchMapName));
            return true;
        }

        public bool GSUpdateStatus(IntPtr _, int cPlayers, int cPlayersMax, int cBotPlayers, IntPtr pchServerName, IntPtr pSpectatorServerName, IntPtr pchMapName)
        {
            UpdateServerStatus(_, cPlayers, cPlayersMax, cBotPlayers, pchServerName, pSpectatorServerName, pchMapName);
            return true;
        }

        public bool GSUpdateStatus2(IntPtr _, int cPlayers, int cPlayersMax, int cBotPlayers, IntPtr pchServerName, IntPtr pSpectatorServerName, IntPtr pchMapName)
        {
            UpdateServerStatus(_, cPlayers, cPlayersMax, cBotPlayers, pchServerName, pSpectatorServerName, pchMapName);
            return true;
        }

        public bool GSSetServerType(IntPtr _, int nGameAppId, uint unServerFlags, uint unGameIP, ushort unGamePort, ushort unSpectatorPort, ushort usQueryPort, IntPtr pchGameDir, IntPtr pchVersion, bool bLANMode)
        {
            return BSetServerType__V004(_, nGameAppId, unServerFlags, unGameIP, unGamePort, unSpectatorPort, usQueryPort, pchGameDir, pchVersion, bLANMode);
        }

        public bool GSSetServerType2(IntPtr _, int nGameAppId, uint unServerFlags, uint unGameIP, ushort unGamePort, ushort unSpectatorPort, ushort usQueryPort, IntPtr pchGameDir, IntPtr pchVersion, bool bLANMode)
        {
            return BSetServerType__V004(_, nGameAppId, unServerFlags, unGameIP, unGamePort, unSpectatorPort, usQueryPort, pchGameDir, pchVersion, bLANMode);
        }

        public bool GSSetServerType__V002(IntPtr _, int nGameAppId, uint unServerFlags, uint unGameIP, uint unGamePort, IntPtr pchGameDir, IntPtr pchVersion)
        {
            bool result = SteamEmulator.SteamGameServer.InitGameServer(
                unGameIP,
                unchecked((int)unGamePort),
                unchecked((int)unGamePort),
                unServerFlags,
                unchecked((uint)nGameAppId),
                ReadAnsi(pchVersion));
            SteamEmulator.SteamGameServer.SetModDir(ReadAnsi(pchGameDir));
            return result;
        }

        public bool GSCreateUnauthenticatedUser(IntPtr _, IntPtr pSteamID)
        {
            NativeSteamId.Write(pSteamID, SteamEmulator.SteamGameServer.CreateUnauthenticatedUserConnection());
            return true;
        }

        public bool GSSetUserData(IntPtr _, ulong steamID, IntPtr pPlayerName, uint nFrags)
        {
            return SteamEmulator.SteamGameServer.BUpdateUserData(steamID, ReadAnsi(pPlayerName), nFrags);
        }

        public void GSUpdateSpectatorPort(IntPtr _, ushort unSpectatorPort)
        {
            SteamEmulator.SteamGameServer.SetSpectatorPort(unSpectatorPort);
        }

        public void GSSetGameType(IntPtr _, IntPtr pchType)
        {
            SetGameType(_, pchType);
        }

        public bool GSGetUserAchievementStatus(IntPtr _, ulong steamID, IntPtr pchAchievementName)
        {
            return BGetUserAchievementStatus(_, steamID, pchAchievementName);
        }

        private static string ReadAnsi(IntPtr value)
        {
            long address = value.ToInt64();
            if (address == 0)
            {
                return string.Empty;
            }
            if (address > 0 && address < 0x10000)
            {
                return $"<invalid:0x{address:X}>";
            }

            try
            {
                return Marshal.PtrToStringAnsi(value) ?? string.Empty;
            }
            catch
            {
                return $"<invalid:0x{address:X}>";
            }
        }
    }
}

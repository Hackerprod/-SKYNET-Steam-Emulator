using SKYNET.Managers;
using System;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamGameServer013")]
    [Interface("SteamGameServer014")]
    public class SteamGameServer014 : ISteamInterface
    {
        public bool InitGameServer(IntPtr _, uint unIP, int usGamePort, int usQueryPort, uint unFlags, uint nGameAppId, string pchVersionString)
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

        public CSteamID GetSteamID(IntPtr _)
        {
            return SteamEmulator.SteamGameServer.GetSteamID();
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

        public void SetSpectatorPort(IntPtr _, int unSpectatorPort)
        {
            SteamEmulator.SteamGameServer.SetSpectatorPort(unSpectatorPort);
        }

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

        public uint GetAuthSessionTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
        {
            return SteamEmulator.SteamGameServer.GetAuthSessionTicket(pTicket, cbMaxTicket, ref  pcbTicket);
        }

        public int BeginAuthSession(IntPtr _, IntPtr pAuthTicket, int cbAuthTicket, ulong steamID)
        {
            return SteamEmulator.SteamGameServer.BeginAuthSession(pAuthTicket, cbAuthTicket, steamID);
        }

        public void EndAuthSession(IntPtr _, ulong steamID)
        {
            SteamEmulator.SteamGameServer.EndAuthSession(steamID);
        }

        public void CancelAuthTicket(IntPtr _, uint hAuthTicket)
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

        public uint GetPublicIP(IntPtr _)
        {
            return SteamEmulator.SteamGameServer.GetPublicIP();
        }

        public bool HandleIncomingPacket(IntPtr _, IntPtr pData, int cbData, uint srcIP, uint srcPort)
        {
            return SteamEmulator.SteamGameServer.HandleIncomingPacket(pData, cbData, srcIP, srcPort);
        }

        public int GetNextOutgoingPacket(IntPtr _, IntPtr pOut, int cbMaxOut, uint pNetAdr, uint pPort)
        {
            return SteamEmulator.SteamGameServer.GetNextOutgoingPacket(pOut, cbMaxOut, pNetAdr, pPort);
        }

        public SteamAPICall_t AssociateWithClan(IntPtr _, ulong steamIDClan)
        {
            return SteamEmulator.SteamGameServer.AssociateWithClan(steamIDClan);
        }

        public SteamAPICall_t ComputeNewPlayerCompatibility(IntPtr _, ulong steamIDNewPlayer)
        {
            return SteamEmulator.SteamGameServer.ComputeNewPlayerCompatibility(steamIDNewPlayer);
        }

        public bool SendUserConnectAndAuthenticate_DEPRECATED(IntPtr _, uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, ulong pSteamIDUser)
        {
            return SteamEmulator.SteamGameServer.SendUserConnectAndAuthenticate_DEPRECATED(unIPClient, pvAuthBlob, cubAuthBlobSize, pSteamIDUser);
        }

        public CSteamID CreateUnauthenticatedUserConnection(IntPtr _)
        {
            return SteamEmulator.SteamGameServer.CreateUnauthenticatedUserConnection();
        }

        public void SendUserDisconnect_DEPRECATED(IntPtr _, ulong steamIDUser)
        {
            SteamEmulator.SteamGameServer.SendUserDisconnect_DEPRECATED(steamIDUser);
        }

        public bool BUpdateUserData(IntPtr _, ulong steamIDUser, string pchPlayerName, uint uScore)
        {
            return SteamEmulator.SteamGameServer.BUpdateUserData(steamIDUser, pchPlayerName, uScore);
        }

        // Deprecated functions.  These will be removed in a future version of the SDK.
        public void SetMasterServerHeartbeatInterval_DEPRECATED(IntPtr _, int iHeartbeatInterval)
        {
            SteamEmulator.SteamGameServer.SetMasterServerHeartbeatInterval_DEPRECATED(iHeartbeatInterval);
        }

        public void ForceMasterServerHeartbeat_DEPRECATED(IntPtr _)
        {
            SteamEmulator.SteamGameServer.ForceMasterServerHeartbeat_DEPRECATED();
        }

        // Inline functions 

        public IntPtr SteamGameServer(IntPtr _)
        {
            SteamEmulator.Write("DEBUG", "SteamClient in SteamClient XD");
            return InterfaceManager.FindOrCreateInterface("SteamGameServer014");
        }


    }
}

using System;


namespace SKYNET.Interface
{
    [Interface("SteamGameServer012")]
    public class SteamGameServer012 : ISteamInterface
    {
        public bool InitGameServer(IntPtr _, uint unIP, uint usGamePort, uint usQueryPort, uint unFlags, IntPtr nGameAppId, string pchVersionString)
        {
            return SteamEmulator.SteamGameServer.InitGameServer(_, unIP, usGamePort, usQueryPort, unFlags, nGameAppId, pchVersionString);
        }

        public void SetProduct(IntPtr _, string pszProduct)
        {
            SteamEmulator.SteamGameServer.SetProduct(_, pszProduct);
        }

        public void SetGameDescription(IntPtr _, string pszGameDescription)
        {
            SteamEmulator.SteamGameServer.SetGameDescription(_, pszGameDescription);
        }

        public void SetModDir(IntPtr _, string pszModDir)
        {
            SteamEmulator.SteamGameServer.SetModDir(_, pszModDir);
        }

        public void SetDedicatedServer(IntPtr _, bool bDedicated)
        {
            SteamEmulator.SteamGameServer.SetDedicatedServer(_, bDedicated);
        }

        public void LogOn(IntPtr _, string pszToken)
        {
            SteamEmulator.SteamGameServer.LogOn(_, pszToken);
        }

        public void LogOnAnonymous(IntPtr _)
        {
            SteamEmulator.SteamGameServer.LogOnAnonymous(_);
        }

        public void LogOff(IntPtr _)
        {
            SteamEmulator.SteamGameServer.LogOff(_);
        }

        public bool BLoggedOn(IntPtr _)
        {
            return SteamEmulator.SteamGameServer.BLoggedOn(_);
        }

        public bool BSecure(IntPtr _)
        {
            return SteamEmulator.SteamGameServer.BSecure(_);
        }

        public ulong GetSteamID(IntPtr _)
        {
            return SteamEmulator.SteamGameServer.GetSteamID(_);
        }

        public bool WasRestartRequested(IntPtr _)
        {
            return SteamEmulator.SteamGameServer.WasRestartRequested(_);
        }

        public void SetMaxPlayerCount(IntPtr _, int cPlayersMax)
        {
            SteamEmulator.SteamGameServer.SetMaxPlayerCount(_, cPlayersMax);
        }

        public void SetBotPlayerCount(IntPtr _, int cBotplayers)
        {
            SteamEmulator.SteamGameServer.SetBotPlayerCount(_, cBotplayers);
        }

        public void SetServerName(IntPtr _, string pszServerName)
        {
            SteamEmulator.SteamGameServer.SetServerName(_, pszServerName);
        }

        public void SetMapName(IntPtr _, string pszMapName)
        {
            SteamEmulator.SteamGameServer.SetMapName(_, pszMapName);
        }

        public void SetPasswordProtected(IntPtr _, bool bPasswordProtected)
        {
            SteamEmulator.SteamGameServer.SetPasswordProtected(_, bPasswordProtected);
        }

        public void SetSpectatorPort(IntPtr _, uint unSpectatorPort)
        {
            SteamEmulator.SteamGameServer.SetSpectatorPort(_, unSpectatorPort);
        }

        public void SetSpectatorServerName(IntPtr _, string pszSpectatorServerName)
        {
            SteamEmulator.SteamGameServer.SetSpectatorServerName(_, pszSpectatorServerName);
        }

        public void ClearAllKeyValues(IntPtr _)
        {
            SteamEmulator.SteamGameServer.ClearAllKeyValues(_);
        }

        public void SetKeyValue(IntPtr _, string pKey, string pValue)
        {
            SteamEmulator.SteamGameServer.SetKeyValue(_, pKey, pValue);
        }

        public void SetGameTags(IntPtr _, string pchGameTags)
        {
            SteamEmulator.SteamGameServer.SetGameTags(_, pchGameTags);
        }

        public void SetGameData(IntPtr _, string pchGameData)
        {
            SteamEmulator.SteamGameServer.SetGameData(_, pchGameData);
        }

        public void SetRegion(IntPtr _, string pszRegion)
        {
            SteamEmulator.SteamGameServer.SetRegion(_, pszRegion);
        }

        public bool SendUserConnectAndAuthenticate(IntPtr _, uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, ulong pSteamIDUser)
        {
            return SteamEmulator.SteamGameServer.SendUserConnectAndAuthenticate(_, unIPClient, pvAuthBlob, cubAuthBlobSize, pSteamIDUser);
        }

        public ulong CreateUnauthenticatedUserConnection(IntPtr _)
        {
            return SteamEmulator.SteamGameServer.CreateUnauthenticatedUserConnection(_);
        }

        public void SendUserDisconnect(IntPtr _, ulong steamIDUser)
        {
            SteamEmulator.SteamGameServer.SendUserDisconnect(_, steamIDUser);
        }

        public bool BUpdateUserData(IntPtr _, ulong steamIDUser, string pchPlayerName, uint uScore)
        {
            return SteamEmulator.SteamGameServer.BUpdateUserData(_, steamIDUser, pchPlayerName, uScore);
        }

        public IntPtr GetAuthSessionTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, uint pcbTicket)
        {
            return SteamEmulator.SteamGameServer.GetAuthSessionTicket(_, pTicket, cbMaxTicket, pcbTicket);
        }

        public int BeginAuthSession(IntPtr _, IntPtr pAuthTicket, int cbAuthTicket, ulong steamID)
        {
            return SteamEmulator.SteamGameServer.BeginAuthSession(_, pAuthTicket, cbAuthTicket, steamID);
        }

        public void EndAuthSession(IntPtr _, ulong steamID)
        {
            SteamEmulator.SteamGameServer.EndAuthSession(_, steamID);
        }

        public void CancelAuthTicket(IntPtr _, IntPtr hAuthTicket)
        {
            SteamEmulator.SteamGameServer.CancelAuthTicket(_, hAuthTicket);
        }

        public int UserHasLicenseForApp(IntPtr _, ulong steamID, IntPtr appID)
        {
            return SteamEmulator.SteamGameServer.UserHasLicenseForApp(_, steamID, appID);
        }

        public bool RequestUserGroupStatus(IntPtr _, ulong steamIDUser, ulong steamIDGroup)
        {
            return SteamEmulator.SteamGameServer.RequestUserGroupStatus(_, steamIDUser, steamIDGroup);
        }

        public void GetGameplayStats(IntPtr _)
        {
            SteamEmulator.SteamGameServer.GetGameplayStats(_);
        }

        public ulong GetServerReputation(IntPtr _)
        {
            return SteamEmulator.SteamGameServer.GetServerReputation(_);
        }

        public uint GetPublicIP_old(IntPtr _)
        {
            return SteamEmulator.SteamGameServer.GetPublicIP_old(_);
        }

        public bool HandleIncomingPacket(IntPtr _, IntPtr pData, int cbData, uint srcIP, uint srcPort)
        {
            return SteamEmulator.SteamGameServer.HandleIncomingPacket(_, pData, cbData, srcIP, srcPort);
        }

        public int GetNextOutgoingPacket(IntPtr _, IntPtr pOut, int cbMaxOut, uint pNetAdr, uint pPort)
        {
            return SteamEmulator.SteamGameServer.GetNextOutgoingPacket(_, pOut, cbMaxOut, pNetAdr, pPort);
        }

        public void EnableHeartbeats(IntPtr _, bool bActive)
        {
            SteamEmulator.SteamGameServer.EnableHeartbeats(_, bActive);
        }

        public void SetHeartbeatInterval(IntPtr _, int iHeartbeatInterval)
        {
            SteamEmulator.SteamGameServer.SetHeartbeatInterval(_, iHeartbeatInterval);
        }

        public void ForceHeartbeat(IntPtr _)
        {
            SteamEmulator.SteamGameServer.ForceHeartbeat(_);
        }

        public ulong AssociateWithClan(IntPtr _, ulong steamIDClan)
        {
            return SteamEmulator.SteamGameServer.AssociateWithClan(_, steamIDClan);
        }

        public ulong ComputeNewPlayerCompatibility(IntPtr _, ulong steamIDNewPlayer)
        {
            return SteamEmulator.SteamGameServer.ComputeNewPlayerCompatibility(_, steamIDNewPlayer);
        }


    }
}

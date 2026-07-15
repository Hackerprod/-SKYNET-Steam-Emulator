using System;
using System.Runtime.InteropServices;
using SKYNET.Helpers;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamGameServer013")]
    public class SteamGameServer013 : ISteamInterface
    {
        public bool InitGameServer(IntPtr _, uint unIP, ushort usGamePort, ushort usQueryPort, uint unFlags, uint nGameAppId, string pchVersionString) => SteamEmulator.SteamGameServer.InitGameServer(unIP, usGamePort, usQueryPort, unFlags, nGameAppId, pchVersionString);
        public void SetProduct(IntPtr _, string pszProduct) => SteamEmulator.SteamGameServer.SetProduct(pszProduct);
        public void SetGameDescription(IntPtr _, string pszGameDescription) => SteamEmulator.SteamGameServer.SetGameDescription(pszGameDescription);
        public void SetModDir(IntPtr _, string pszModDir) => SteamEmulator.SteamGameServer.SetModDir(pszModDir);
        public void SetDedicatedServer(IntPtr _, bool bDedicated) => SteamEmulator.SteamGameServer.SetDedicatedServer(bDedicated);
        public void LogOn(IntPtr _, string pszToken) => SteamEmulator.SteamGameServer.LogOn(pszToken);
        public void LogOnAnonymous(IntPtr _) => SteamEmulator.SteamGameServer.LogOnAnonymous();
        public void LogOff(IntPtr _) => SteamEmulator.SteamGameServer.LogOff();
        public bool BLoggedOn(IntPtr _) => SteamEmulator.SteamGameServer.BLoggedOn();
        public bool BSecure(IntPtr _) => SteamEmulator.SteamGameServer.BSecure();
        public IntPtr GetSteamID(IntPtr _, IntPtr pSteamID) => NativeSteamId.Write(pSteamID, SteamEmulator.SteamGameServer.GetSteamID());
        public bool WasRestartRequested(IntPtr _) => SteamEmulator.SteamGameServer.WasRestartRequested();
        public void SetMaxPlayerCount(IntPtr _, int cPlayersMax) => SteamEmulator.SteamGameServer.SetMaxPlayerCount(cPlayersMax);
        public void SetBotPlayerCount(IntPtr _, int cBotplayers) => SteamEmulator.SteamGameServer.SetBotPlayerCount(cBotplayers);
        public void SetServerName(IntPtr _, string pszServerName) => SteamEmulator.SteamGameServer.SetServerName(pszServerName);
        public void SetMapName(IntPtr _, string pszMapName) => SteamEmulator.SteamGameServer.SetMapName(pszMapName);
        public void SetPasswordProtected(IntPtr _, bool bPasswordProtected) => SteamEmulator.SteamGameServer.SetPasswordProtected(bPasswordProtected);
        public void SetSpectatorPort(IntPtr _, ushort unSpectatorPort) => SteamEmulator.SteamGameServer.SetSpectatorPort(unSpectatorPort);
        public void SetSpectatorServerName(IntPtr _, string pszSpectatorServerName) => SteamEmulator.SteamGameServer.SetSpectatorServerName(pszSpectatorServerName);
        public void ClearAllKeyValues(IntPtr _) => SteamEmulator.SteamGameServer.ClearAllKeyValues();
        public void SetKeyValue(IntPtr _, string pKey, string pValue) => SteamEmulator.SteamGameServer.SetKeyValue(pKey, pValue);
        public void SetGameTags(IntPtr _, string pchGameTags) => SteamEmulator.SteamGameServer.SetGameTags(pchGameTags);
        public void SetGameData(IntPtr _, string pchGameData) => SteamEmulator.SteamGameServer.SetGameData(pchGameData);
        public void SetRegion(IntPtr _, string pszRegion) => SteamEmulator.SteamGameServer.SetRegion(pszRegion);
        public bool SendUserConnectAndAuthenticate(IntPtr _, uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, IntPtr pSteamIDUser) => SteamEmulator.SteamGameServer.SendUserConnectAndAuthenticate_DEPRECATED(unIPClient, pvAuthBlob, cubAuthBlobSize, pSteamIDUser);
        public IntPtr CreateUnauthenticatedUserConnection(IntPtr _, IntPtr pSteamID) => NativeSteamId.Write(pSteamID, SteamEmulator.SteamGameServer.CreateUnauthenticatedUserConnection());
        public void SendUserDisconnect(IntPtr _, ulong steamIDUser) => SteamEmulator.SteamGameServer.SendUserDisconnect_DEPRECATED(steamIDUser);
        public bool BUpdateUserData(IntPtr _, ulong steamIDUser, string pchPlayerName, uint uScore) => SteamEmulator.SteamGameServer.BUpdateUserData(steamIDUser, pchPlayerName, uScore);
        public uint GetAuthSessionTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket) => SteamEmulator.SteamGameServer.GetAuthSessionTicket(pTicket, cbMaxTicket, ref pcbTicket);
        public int BeginAuthSession(IntPtr _, IntPtr pAuthTicket, int cbAuthTicket, ulong steamID) => SteamEmulator.SteamGameServer.BeginAuthSession(pAuthTicket, cbAuthTicket, steamID);
        public void EndAuthSession(IntPtr _, ulong steamID) => SteamEmulator.SteamGameServer.EndAuthSession(steamID);
        public void CancelAuthTicket(IntPtr _, uint hAuthTicket) => SteamEmulator.SteamGameServer.CancelAuthTicket(hAuthTicket);
        public int UserHasLicenseForApp(IntPtr _, ulong steamID, uint appID) => SteamEmulator.SteamGameServer.UserHasLicenseForApp(steamID, appID);
        public bool RequestUserGroupStatus(IntPtr _, ulong steamIDUser, ulong steamIDGroup) => SteamEmulator.SteamGameServer.RequestUserGroupStatus(steamIDUser, steamIDGroup);
        public void GetGameplayStats(IntPtr _) => SteamEmulator.SteamGameServer.GetGameplayStats();
        public SteamAPICall_t GetServerReputation(IntPtr _) => SteamEmulator.SteamGameServer.GetServerReputation();
        public IntPtr GetPublicIP(IntPtr arg0, IntPtr arg1) => SteamEmulator.SteamGameServer.GetPublicIP(arg0, arg1);
        public bool HandleIncomingPacket(IntPtr _, IntPtr pData, int cbData, uint srcIP, ushort srcPort) => SteamEmulator.SteamGameServer.HandleIncomingPacket(pData, cbData, srcIP, srcPort);
        public int GetNextOutgoingPacket(IntPtr _, IntPtr pOut, int cbMaxOut, IntPtr pNetAdr, IntPtr pPort)
        {
            if (pNetAdr != IntPtr.Zero) Marshal.WriteInt32(pNetAdr, 0);
            if (pPort != IntPtr.Zero) Marshal.WriteInt16(pPort, 0);
            return SteamEmulator.SteamGameServer.GetNextOutgoingPacket(pOut, cbMaxOut, 0, 0);
        }
        public void EnableHeartbeats(IntPtr _, bool bActive) => SteamEmulator.SteamGameServer.EnableHeartbeats(bActive);
        public void SetHeartbeatInterval(IntPtr _, int iHeartbeatInterval) => SteamEmulator.SteamGameServer.SetHeartbeatInterval(iHeartbeatInterval);
        public void ForceHeartbeat(IntPtr _) => SteamEmulator.SteamGameServer.ForceHeartbeat();
        public SteamAPICall_t AssociateWithClan(IntPtr _, ulong steamIDClan) => SteamEmulator.SteamGameServer.AssociateWithClan(steamIDClan);
        public SteamAPICall_t ComputeNewPlayerCompatibility(IntPtr _, ulong steamIDNewPlayer) => SteamEmulator.SteamGameServer.ComputeNewPlayerCompatibility(steamIDNewPlayer);
    }
}

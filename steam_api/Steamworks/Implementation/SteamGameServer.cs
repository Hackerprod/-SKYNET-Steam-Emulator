using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SKYNET;
using SKYNET.Helpers;
using Steamworks;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamGameServer : ISteamInterface
    {
        public SteamGameServer()
        {
            InterfaceVersion = "SteamGameServer";
        }


        internal bool InitGameServer(IntPtr _, uint unIP, uint usGamePort, uint usQueryPort, uint unFlags, IntPtr nGameAppId, string pchVersionString)
        {
            Write("InitGameServer");
            return true;
        }

        internal void SetProduct(IntPtr _, string pszProduct)
        {
            Write($"SetProduct {pszProduct}");
        }

        internal void SetGameDescription(IntPtr _, string pszGameDescription)
        {
            Write($"SetGameDescription {pszGameDescription}");
        }

        internal void SetModDir(IntPtr _, string pszModDir)
        {
            Write("SetModDir");
        }

        internal void SetDedicatedServer(IntPtr _, bool bDedicated)
        {
            Write("SetDedicatedServer");
        }

        internal void LogOn(IntPtr _, string pszToken)
        {
            Write("LogOn");
        }

        internal void LogOnAnonymous(IntPtr _)
        {
            Write("LogOnAnonymous");
            
        }

        internal void LogOff(IntPtr _)
        {
            Write("LogOff");
            
        }

        internal bool BLoggedOn(IntPtr _)
        {
            Write("BLoggedOn");
            return true;
        }

        internal bool BSecure(IntPtr _)
        {
            Write("BSecure");
            return true;
        }

        internal ulong GetSteamID(IntPtr _)
        {
            Write("GetSteamID");
            return SteamEmulator.SteamId_GS;
        }

        internal bool WasRestartRequested(IntPtr _)
        {
            Write("WasRestartRequested");
            return false;
        }

        internal void SetMaxPlayerCount(IntPtr _, int cPlayersMax)
        {
            Write("SetMaxPlayerCount");
        }

        internal void SetBotPlayerCount(IntPtr _, int cBotplayers)
        {
            Write("SetBotPlayerCount");
        }

        internal void SetServerName(IntPtr _, string pszServerName)
        {
            Write("SetServerName");
        }

        internal void SetMapName(IntPtr _, string pszMapName)
        {
            Write("SetMapName");
        }

        internal void SetPasswordProtected(IntPtr _, bool bPasswordProtected)
        {
            Write("SetPasswordProtected");
        }

        internal void SetSpectatorPort(IntPtr _, uint unSpectatorPort)
        {
            Write("SetSpectatorPort");
        }

        internal void SetSpectatorServerName(IntPtr _, string pszSpectatorServerName)
        {
            Write("SetSpectatorServerName");
        }

        internal void ClearAllKeyValues(IntPtr _)
        {
            Write("ClearAllKeyValues");
        }

        internal void SetKeyValue(IntPtr _, string pKey, string pValue)
        {
            Write("SetKeyValue");
        }

        internal void SetGameTags(IntPtr _, string pchGameTags)
        {
            Write("SetGameTags");
        }

        internal void SetGameData(IntPtr _, string pchGameData)
        {
            Write("SetGameData");
        }

        internal void SetRegion(IntPtr _, string pszRegion)
        {
            Write("SetRegion");
        }

        internal bool SendUserConnectAndAuthenticate(IntPtr _, uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, ulong pSteamIDUser)
        {
            Write("SendUserConnectAndAuthenticate");
            return true;
        }

        internal ulong CreateUnauthenticatedUserConnection(IntPtr _)
        {
            Write("CreateUnauthenticatedUserConnection");
            return 0;
        }

        internal void SendUserDisconnect(IntPtr _, ulong steamIDUser)
        {
            Write("SendUserDisconnect");
        }

        internal bool BUpdateUserData(IntPtr _, ulong steamIDUser, string pchPlayerName, uint uScore)
        {
            Write("BUpdateUserData");
            return true;
        }

        internal IntPtr GetAuthSessionTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, uint pcbTicket)
        {
            Write("GetAuthSessionTicket");
            return IntPtr.Zero;
        }

        internal int BeginAuthSession(IntPtr _, IntPtr pAuthTicket, int cbAuthTicket, ulong steamID)
        {
            Write("BeginAuthSession");
            return (int)EBeginAuthSessionResult.k_EBeginAuthSessionResultOK;
        }

        internal void EndAuthSession(IntPtr _, ulong steamID)
        {
            Write("EndAuthSession");
        }

        internal void CancelAuthTicket(IntPtr _, IntPtr hAuthTicket)
        {
            Write("CancelAuthTicket");
        }

        internal int UserHasLicenseForApp(IntPtr _, ulong steamID, IntPtr appID)
        {
            Write("UserHasLicenseForApp");
            return (int)EUserHasLicenseForAppResult.k_EUserHasLicenseResultHasLicense; 
        }

        internal bool RequestUserGroupStatus(IntPtr _, ulong steamIDUser, ulong steamIDGroup)
        {
            Write("RequestUserGroupStatus");
            return false;
        }

        internal void GetGameplayStats(IntPtr _)
        {
            Write("GetGameplayStats");
        }

        internal ulong GetServerReputation(IntPtr _)
        {
            Write("GetServerReputation");
            return 100;
        }

        internal uint GetPublicIP_old(IntPtr _)
        {
            Write("GetPublicIP_old");
            return 0;
        }

        internal bool HandleIncomingPacket(IntPtr _, IntPtr pData, int cbData, uint srcIP, uint srcPort)
        {
            Write("HandleIncomingPacket");
            return true;
        }

        internal int GetNextOutgoingPacket(IntPtr _, IntPtr pOut, int cbMaxOut, uint pNetAdr, uint pPort)
        {
            Write("GetNextOutgoingPacket");
            return 0;
        }

        internal void EnableHeartbeats(IntPtr _, bool bActive)
        {
            Write("EnableHeartbeats");
        }

        internal void SetHeartbeatInterval(IntPtr _, int iHeartbeatInterval)
        {
            Write("SetHeartbeatInterval");
        }

        internal void ForceHeartbeat(IntPtr _)
        {
            Write("ForceHeartbeat");
        }
        internal ulong AssociateWithClan(IntPtr _, ulong steamIDClan)
        {
            Write("AssociateWithClan");
            return 0;
        }

        internal ulong ComputeNewPlayerCompatibility(IntPtr _, ulong steamIDNewPlayer)
        {
            Write("ComputeNewPlayerCompatibility");
            return 0;
        }
    }
}

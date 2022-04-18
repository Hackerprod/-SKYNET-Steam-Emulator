using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Types;
using Steamworks;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamGameServer : ISteamInterface
    {
        public SteamGameServer()
        {
            InterfaceVersion = "SteamGameServer";
        }


        public bool InitGameServer(uint unIP, int usGamePort, int usQueryPort, uint unFlags, uint nGameAppId, string pchVersionString)
        {
            Write("InitGameServer");
            return true;
        }

        public void SetProduct(string pszProduct)
        {
            Write($"SetProduct {pszProduct}");
        }

        public void SetGameDescription(string pszGameDescription)
        {
            Write($"SetGameDescription {pszGameDescription}");
        }

        public void SetModDir(string pszModDir)
        {
            Write($"SetModDir {pszModDir}");
        }

        public void SetDedicatedServer(bool bDedicated)
        {
            Write("SetDedicatedServer");
        }

        public void LogOn(string pszToken)
        {
            Write("LogOn");
        }

        public void LogOnAnonymous()
        {
            Write("LogOnAnonymous");
            
        }

        public void LogOff()
        {
            Write("LogOff");
            
        }

        public bool BLoggedOn()
        {
            Write("BLoggedOn");
            return true;
        }

        public bool BSecure()
        {
            Write("BSecure");
            return true;
        }

        public ulong GetSteamID()
        {
            Write("GetSteamID");
            return SteamEmulator.SteamId_GS;
        }

        public bool WasRestartRequested()
        {
            Write("WasRestartRequested");
            return false;
        }

        public void SetMaxPlayerCount(int cPlayersMax)
        {
            Write("SetMaxPlayerCount");
        }

        public void SetBotPlayerCount(int cBotplayers)
        {
            Write("SetBotPlayerCount");
        }

        public void SetServerName(string pszServerName)
        {
            Write("SetServerName");
        }

        public void SetMapName(string pszMapName)
        {
            Write("SetMapName");
        }

        public void SetPasswordProtected(bool bPasswordProtected)
        {
            Write("SetPasswordProtected");
        }

        public void SetSpectatorPort(uint unSpectatorPort)
        {
            Write("SetSpectatorPort");
        }

        public void SetSpectatorServerName(string pszSpectatorServerName)
        {
            Write("SetSpectatorServerName");
        }

        public void ClearAllKeyValues()
        {
            Write("ClearAllKeyValues");
        }

        public void SetKeyValue(string pKey, string pValue)
        {
            Write("SetKeyValue");
        }

        public void SetGameTags(string pchGameTags)
        {
            Write("SetGameTags");
        }

        public void SetGameData(string pchGameData)
        {
            Write("SetGameData");
        }

        public void SetRegion(string pszRegion)
        {
            Write("SetRegion");
        }

        public bool SendUserConnectAndAuthenticate(uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, ulong pSteamIDUser)
        {
            Write("SendUserConnectAndAuthenticate");
            return true;
        }

        public ulong CreateUnauthenticatedUserConnection()
        {
            Write("CreateUnauthenticatedUserConnection");
            return 0;
        }

        public void SendUserDisconnect(SteamID steamIDUser)
        {
            Write("SendUserDisconnect");
        }

        public bool BUpdateUserData(SteamID steamIDUser, string pchPlayerName, uint uScore)
        {
            Write("BUpdateUserData");
            return true;
        }

        public IntPtr GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, uint pcbTicket)
        {
            Write("GetAuthSessionTicket");
            return IntPtr.Zero;
        }

        public void SetAdvertiseServerActive(bool bActive)
        {
            Write("SetAdvertiseServerActive");
        }

        public int BeginAuthSession(IntPtr pAuthTicket, int cbAuthTicket, SteamID steamID)
        {
            Write("BeginAuthSession");
            return (int)EBeginAuthSessionResult.k_EBeginAuthSessionResultOK;
        }

        public void EndAuthSession(SteamID steamID)
        {
            Write("EndAuthSession");
        }

        public void CancelAuthTicket(IntPtr hAuthTicket)
        {
            Write("CancelAuthTicket");
        }

        public int UserHasLicenseForApp(SteamID steamID, uint appID)
        {
            Write("UserHasLicenseForApp");
            return (int)EUserHasLicenseForAppResult.k_EUserHasLicenseResultHasLicense; 
        }

        public bool RequestUserGroupStatus(SteamID steamIDUser, SteamID steamIDGroup)
        {
            Write("RequestUserGroupStatus");
            return false;
        }

        public void GetGameplayStats()
        {
            Write("GetGameplayStats");
        }

        public SteamAPICall_t GetServerReputation()
        {
            Write("GetServerReputation");
            // GSReputation_t
            return 0;
        }

        public uint GetPublicIP_old()
        {
            Write("GetPublicIP_old");
            return 0;
        }

        public bool HandleIncomingPacket(IntPtr pData, int cbData, uint srcIP, uint srcPort)
        {
            Write("HandleIncomingPacket");
            return true;
        }

        public int GetNextOutgoingPacket(IntPtr pOut, int cbMaxOut, uint pNetAdr, uint pPort)
        {
            Write("GetNextOutgoingPacket");
            return 0;
        }

        internal IntPtr GetPublicIP()
        {
            Write("GetNextOutgoingPacket");
            return IntPtr.Zero;
        }

        public void EnableHeartbeats(bool bActive)
        {
            Write("EnableHeartbeats");
        }

        public void SetHeartbeatInterval(int iHeartbeatInterval)
        {
            Write("SetHeartbeatInterval");
        }

        public void ForceHeartbeat()
        {
            Write("ForceHeartbeat");
        }
        public SteamAPICall_t AssociateWithClan(SteamID steamIDClan)
        {
            Write("AssociateWithClan");
            return 0;
        }

        public SteamAPICall_t ComputeNewPlayerCompatibility(SteamID steamIDNewPlayer)
        {
            Write("ComputeNewPlayerCompatibility");
            return 0;
        }

        public bool SendUserConnectAndAuthenticate_DEPRECATED(uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, ulong pSteamIDUser)
        {
            Write("SendUserConnectAndAuthenticate_DEPRECATED");
            return true;
        }

        public void SendUserDisconnect_DEPRECATED(SteamID steamIDUser)
        {
            Write("SendUserDisconnect_DEPRECATED");
        }

        public void SetMasterServerHeartbeatInterval_DEPRECATED(int iHeartbeatInterval)
        {
            Write("SetMasterServerHeartbeatInterval_DEPRECATED");
        }

        public void ForceMasterServerHeartbeat_DEPRECATED()
        {
            Write("ForceMasterServerHeartbeat_DEPRECATED");
        }
    }
}

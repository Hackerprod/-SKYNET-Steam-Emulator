using System;
using SKYNET.Managers;
using SKYNET.Types;
using Steamworks;

using SteamAPICall_t = System.UInt64;
using HAuthTicket = System.UInt32;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamGameServer : ISteamInterface
    {
        private GameServerData ServerData;
        private bool LoggedIn;
        private SteamAPICall_t k_uAPICallInvalid = 0x0;

        public SteamGameServer()
        {
            InterfaceVersion = "SteamGameServer";
            ServerData = new GameServerData();
        }


        public bool InitGameServer(uint unIP, int usGamePort, int usQueryPort, uint unFlags, uint nGameAppId, string pchVersionString)
        {
            Write("InitGameServer");

            if (LoggedIn)
            {
                return false;
            }

            ServerData.IP = unIP;
            ServerData.Port = usGamePort;
            ServerData.QueryPort = usQueryPort;
            ServerData.Flags = unFlags;
            ServerData.AppId = nGameAppId;
            ServerData.VersionString = pchVersionString;

            if (SteamEmulator.AppId == 0) SteamEmulator.AppId = nGameAppId;

            return true;
        }

        public void SetProduct(string pszProduct)
        {
            Write($"SetProduct {pszProduct}");
            ServerData.Product = pszProduct;
        }

        public void SetGameDescription(string pszGameDescription)
        {
            Write($"SetGameDescription {pszGameDescription}");
            ServerData.Description = pszGameDescription;
        }

        public void SetModDir(string pszModDir)
        {
            Write($"SetModDir {pszModDir}");
            ServerData.ModDir = pszModDir;
        }

        public void SetDedicatedServer(bool bDedicated)
        {
            Write("SetDedicatedServer");
            ServerData.Dedicated = bDedicated;
        }

        public void LogOn(string pszToken)
        {
            Write("LogOn");
            LoggedIn = true;
        }

        public void LogOnAnonymous()
        {
            Write("LogOnAnonymous");
            LoggedIn = true;
        }

        public void LogOff()
        {
            Write("LogOff");
            LoggedIn = false;
        }

        public bool BLoggedOn()
        {
            Write("BLoggedOn");
            return LoggedIn;
        }

        public bool BSecure()
        {
            Write("BSecure");
            return true;
        }

        public CSteamID GetSteamID()
        {
            var SteamId = SteamEmulator.SteamId_GS;
            Write($"GetSteamID {(string)SteamId}");
            return SteamId; 
        }

        public bool WasRestartRequested()
        {
            Write("WasRestartRequested");
            return false;
        }

        public void SetMaxPlayerCount(int cPlayersMax)
        {
            Write("SetMaxPlayerCount");
            ServerData.MaxPlayers = cPlayersMax;
        }

        public void SetBotPlayerCount(int cBotplayers)
        {
            Write("SetBotPlayerCount");
            ServerData.BotPlayers = cBotplayers;
        }

        public void SetServerName(string pszServerName)
        {
            Write("SetServerName");
            ServerData.ServerName = pszServerName;
        }

        public void SetMapName(string pszMapName)
        {
            Write("SetMapName");
            ServerData.MapName = pszMapName;
        }

        public void SetPasswordProtected(bool bPasswordProtected)
        {
            Write("SetPasswordProtected");
            ServerData.PasswordProtected = bPasswordProtected;
        }

        public void SetSpectatorPort(uint unSpectatorPort)
        {
            Write("SetSpectatorPort");
            ServerData.SpectatorPort = unSpectatorPort;
        }

        public void SetSpectatorServerName(string pszSpectatorServerName)
        {
            Write("SetSpectatorServerName");
            ServerData.SpectatorServerName = pszSpectatorServerName;
        }

        public void ClearAllKeyValues()
        {
            Write("ClearAllKeyValues");
            ServerData.KeyValues.Clear();
        }

        public void SetKeyValue(string pKey, string pValue)
        {
            Write("SetKeyValue");
            ServerData.KeyValues.TryAdd(pKey, pValue);
        }

        public void SetGameTags(string pchGameTags)
        {
            Write("SetGameTags");
            ServerData.GameTags = pchGameTags;
        }

        public void SetGameData(string pchGameData)
        {
            Write("SetGameData");
            ServerData.GameData = pchGameData;
        }

        public void SetRegion(string pszRegion)
        {
            Write("SetRegion");
            ServerData.Region = pszRegion;
        }

        public bool SendUserConnectAndAuthenticate(uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, ulong pSteamIDUser)
        {
            var IPAddress = modCommon.GetIPAddress(unIPClient);
            Write($"SendUserConnectAndAuthenticate {new CSteamID(pSteamIDUser)} | {IPAddress}");
            return TicketManager.ConnectAndAuthenticate(unIPClient, pvAuthBlob, cubAuthBlobSize, pSteamIDUser);
        }

        public CSteamID CreateUnauthenticatedUserConnection()
        {
            Write("CreateUnauthenticatedUserConnection");
            return new CSteamID((uint)new Random().Next(1000, 9999), EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeAnonUser);
        }

        public void SendUserDisconnect(ulong steamIDUser)
        {
            Write("SendUserDisconnect");
            TicketManager.RemoveTicket(steamIDUser);
        }

        public bool BUpdateUserData(ulong steamIDUser, string pchPlayerName, uint uScore)
        {
            Write("BUpdateUserData");
            return true;
        }

        // Retrieve ticket to be sent to the entity who wishes to authenticate you ( using BeginAuthSession API ). 
        // uint32 *pcbTicket  retrieves the length of the actual ticket.
        public HAuthTicket GetAuthSessionTicket(IntPtr pTicket, ref int cbMaxTicket, ref uint pcbTicket)
        {
            SteamEmulator.Write("DEBUG", "GetAuthSessionTicket");
            NetworkManager.AnnounceClient();
            return TicketManager.GetAuthSessionTicket(pTicket, ref cbMaxTicket, ref pcbTicket, true);
        }

        public void SetAdvertiseServerActive(bool bActive)
        {
            Write("SetAdvertiseServerActive");
        }

        public int BeginAuthSession(IntPtr pAuthTicket, int cbAuthTicket, ulong steamID)
        {
            Write("BeginAuthSession");
            return (int)EBeginAuthSessionResult.k_EBeginAuthSessionResultOK;
        }

        public void EndAuthSession(ulong steamID)
        {
            Write("EndAuthSession");
        }

        public void CancelAuthTicket(IntPtr hAuthTicket)
        {
            Write("CancelAuthTicket");
        }

        public int UserHasLicenseForApp(ulong steamID, uint appID)
        {
            Write("UserHasLicenseForApp");
            return (int)EUserHasLicenseForAppResult.k_EUserHasLicenseResultHasLicense; 
        }

        public bool RequestUserGroupStatus(ulong steamIDUser, ulong steamIDGroup)
        {
            Write("RequestUserGroupStatus");
            return true;
        }

        public void GetGameplayStats()
        {
            Write("GetGameplayStats");
        }

        public SteamAPICall_t GetServerReputation()
        {
            Write("GetServerReputation");
            // GSReputation_t
            return k_uAPICallInvalid;
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
            SteamEmulator.Write("DEBUG", "GetNextOutgoingPacket");
            return 0;
        }

        internal SteamIPAddress_t GetPublicIP()
        {
            Write("GetPublicIP");
            SteamIPAddress_t iPAddress = new SteamIPAddress_t(NetworkManager.GetIPAddress());
            return iPAddress;
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
        public SteamAPICall_t AssociateWithClan(ulong steamIDClan)
        {
            Write("AssociateWithClan");
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t ComputeNewPlayerCompatibility(ulong steamIDNewPlayer)
        {
            Write("ComputeNewPlayerCompatibility");
            return k_uAPICallInvalid;
        }

        public bool SendUserConnectAndAuthenticate_DEPRECATED(uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, ulong pSteamIDUser)
        {
            Write("SendUserConnectAndAuthenticate_DEPRECATED");
            return true;
        }

        public void SendUserDisconnect_DEPRECATED(ulong steamIDUser)
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

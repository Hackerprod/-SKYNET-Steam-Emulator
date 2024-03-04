using System;
using SKYNET.Managers;
using SKYNET.Types;
using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Steamworks.Interfaces;

using SteamAPICall_t = System.UInt64;
using HAuthTicket = System.UInt32;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamGameServer : ISteamInterface
    {
        public static SteamGameServer Instance;

        public GameServerData ServerData;
        public bool LoggedIn;

        public SteamGameServer()
        {
            Instance = this;
            InterfaceName = "SteamGameServer";
            InterfaceVersion = "SteamGameServer014";
            ServerData = new GameServerData();
        }

        public bool InitGameServer(uint unIP, int usGamePort, int usQueryPort, uint unFlags, uint nGameAppId, string pchVersionString)
        {
            Write($"InitGameServer (IP = {unIP}, GamePort = {usGamePort}, QueryPort = {usQueryPort}, Flags = {unFlags}, GameAppId = {nGameAppId}, VersionString = {pchVersionString})");

            if (LoggedIn)
            {
                Write($"InitGameServer skiping, gameserver logged in");
                return false;
            }

            uint IP = 0;
            if (unIP != 0)
            {
                IP = unIP;
            }
            else
            {
                IP = NetworkHelper.ConvertFromIPAddress(NetworkHelper.GetIPAddress());
            }

            ServerData.IP = IP;
            ServerData.Port = usGamePort;
            ServerData.QueryPort = usQueryPort;
            ServerData.Flags = unFlags;
            ServerData.AppId = nGameAppId;
            ServerData.VersionString = pchVersionString;

            var lobby = LobbyManager.GetLobbyByOwner((ulong)SteamEmulator.SteamID);
            if (lobby != null)
            {
                Write($"Setting lobby gameserver IP = {IP}, Port = {(uint)usQueryPort}");
                //IPCManager.SendLobbyGameServerEndPoint(lobby.SteamID, IP, (uint)usQueryPort);
            }
            else
            {

            }

            // TODO: Necessary
            GSPolicyResponse_t Policy = new GSPolicyResponse_t()
            {
                Secure = (byte)(ServerData.Flags & Constants.k_unServerFlagSecure)
            };

            CallbackManager.AddCallbackResult(Policy);

            return true;
        }

        public void SetProduct(string pszProduct)
        {
            Write($"SetProduct (Product = {pszProduct})");
            ServerData.Product = pszProduct;
        }

        public void SetGameDescription(string pszGameDescription)
        {
            Write($"SetGameDescription (GameDescription = {pszGameDescription})");
            ServerData.Description = pszGameDescription;
        }

        public void SetModDir(string pszModDir)
        {
            Write($"SetModDir (ModDir = {pszModDir})");
            ServerData.ModDir = pszModDir;
        }

        public void SetDedicatedServer(bool bDedicated)
        {
            Write($"SetDedicatedServer (Dedicated = {bDedicated})");
            ServerData.Dedicated = bDedicated;
        }

        public void LogOn(string pszToken)
        {
            Write($"LogOn (Token = {pszToken})");

            SteamServersConnected_t data = new SteamServersConnected_t();
            CallbackManager.AddCallbackResult(data);

            LoggedIn = true;
        }

        public void LogOnAnonymous()
        {
            Write($"LogOnAnonymous");

            SteamServersConnected_t data = new SteamServersConnected_t();
            CallbackManager.AddCallbackResult(data);

            LoggedIn = true;
        }

        public void LogOff()
        {
            Write($"LogOff");
            LoggedIn = false;

            SteamServersDisconnected_t data = new SteamServersDisconnected_t()
            {
                m_eResult = EResult.k_EResultOK
            };
            CallbackManager.AddCallbackResult(data);
        }

        public bool BLoggedOn()
        {
            Write($"BLoggedOn");
            return LoggedIn;
        }

        public bool BSecure()
        {
            Write($"BSecure");
            return true;
        }

        public CSteamID GetSteamID()
        {
            var SteamId = SteamEmulator.SteamID_GS;
            Write($"GetSteamID {SteamId.ToString()}");
            return SteamId; 
        }

        public bool WasRestartRequested()
        {
            Write($"WasRestartRequested");
            return false;
        }

        public void SetMaxPlayerCount(int cPlayersMax)
        {
            Write($"SetMaxPlayerCount (PlayersMax = {cPlayersMax})");
            ServerData.MaxPlayers = cPlayersMax;
        }

        public void SetBotPlayerCount(int cBotplayers)
        {
            Write($"SetBotPlayerCount (Botplayers = {cBotplayers})");
            ServerData.BotPlayers = cBotplayers;
        }

        public void SetServerName(string pszServerName)
        {
            Write($"SetServerName (ServerName = {pszServerName})");
            ServerData.ServerName = pszServerName;
        }

        public void SetMapName(string pszMapName)
        {
            Write($"SetMapName (MapName = {pszMapName})");
            ServerData.MapName = pszMapName;
        }

        public void SetPasswordProtected(bool bPasswordProtected)
        {
            Write($"SetPasswordProtected (PasswordProtected = {bPasswordProtected})");
            ServerData.PasswordProtected = bPasswordProtected;
        }

        public void SetSpectatorPort(int unSpectatorPort)
        {
            Write($"SetSpectatorPort (SpectatorPort = {unSpectatorPort})");
            ServerData.SpectatorPort = (uint)unSpectatorPort;
        }

        public void SetSpectatorServerName(string pszSpectatorServerName)
        {
            Write($"SetSpectatorServerName (SpectatorServerName = {pszSpectatorServerName})");
            ServerData.SpectatorServerName = pszSpectatorServerName;
        }

        public void ClearAllKeyValues()
        {
            Write($"ClearAllKeyValues");
            ServerData.KeyValues.Clear();
        }

        public void SetKeyValue(string pKey, string pValue)
        {
            Write($"SetKeyValue (Key = {pKey}, Value = {pValue})");
            ServerData.KeyValues.TryAdd(pKey, pValue);
        }

        public void SetGameTags(string pchGameTags)
        {
            Write($"SetGameTags (GameTags = {pchGameTags})");
            ServerData.GameTags = pchGameTags;
        }

        public void SetGameData(string pchGameData)
        {
            Write($"SetGameData (GameData = {pchGameData})");
            ServerData.GameData = pchGameData;
        }

        public void SetRegion(string pszRegion)
        {
            Write($"SetRegion (Region = {pszRegion})");
            ServerData.Region = pszRegion;
        }

        public bool SendUserConnectAndAuthenticate(uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, ulong pSteamIDUser)
        {
            var IPAddress = Common.GetIPAddress(unIPClient);
            Write($"SendUserConnectAndAuthenticate (SteamID = {new CSteamID(pSteamIDUser)} | {IPAddress})");
            return TicketManager.ConnectAndAuthenticate(unIPClient, pvAuthBlob, cubAuthBlobSize, pSteamIDUser);
        }

        public CSteamID CreateUnauthenticatedUserConnection()
        {
            Write($"CreateUnauthenticatedUserConnection");
            return CSteamID.CreateUnauthenticatedUser();
        }

        public void SendUserDisconnect(ulong steamIDUser)
        {
            Write($"SendUserDisconnect (SteamID = {steamIDUser})");
            TicketManager.RemoveTicket(steamIDUser);
        }

        public bool BUpdateUserData(ulong steamIDUser, string pchPlayerName, uint uScore)
        {
            Write($"BUpdateUserData (SteamID = {steamIDUser}, PlayerName = {pchPlayerName})");
            return true;
        }

        public HAuthTicket GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
        {
            SteamEmulator.Write($"DEBUG", "GetAuthSessionTicket");
            return TicketManager.GetAuthSessionTicket(pTicket, cbMaxTicket, ref pcbTicket);
        }

        public void SetAdvertiseServerActive(bool bActive)
        {
            Write($"SetAdvertiseServerActive (Active = {bActive})");
        }

        public int BeginAuthSession(IntPtr pAuthTicket, int cbAuthTicket, ulong steamID)
        {
            Write($"BeginAuthSession (SteamID = {(CSteamID)steamID}) = {EBeginAuthSessionResult.k_EBeginAuthSessionResultOK}");

            ValidateAuthTicketResponse_t data = new ValidateAuthTicketResponse_t()
            {
                m_eAuthSessionResponse = EAuthSessionResponse.OK,
                m_OwnerSteamID = steamID,
                m_SteamID = steamID
            };
            CallbackManager.AddCallbackResult(data);

            return (int)EBeginAuthSessionResult.k_EBeginAuthSessionResultOK;
        }

        public void EndAuthSession(ulong steamID)
        {
            Write($"EndAuthSession (SteamID = {steamID})");
        }

        public void CancelAuthTicket(HAuthTicket hAuthTicket)
        {
            Write($"CancelAuthTicket");
        }

        public int UserHasLicenseForApp(ulong steamID, uint appID)
        {
            Write($"UserHasLicenseForApp (SteamID = {steamID}, AppID = {appID})");
            return (int)EUserHasLicenseForAppResult.k_EUserHasLicenseResultHasLicense; 
        }

        public bool RequestUserGroupStatus(ulong steamIDUser, ulong steamIDGroup)
        {
            Write($"RequestUserGroupStatus (SteamID = {steamIDUser}, IDGroup = {steamIDGroup})");
            return true;
        }

        public void GetGameplayStats()
        {
            Write($"GetGameplayStats");
        }

        public SteamAPICall_t GetServerReputation()
        {
            Write($"GetServerReputation");
            // GSReputation_t
            return k_uAPICallInvalid;
        }

        public uint GetPublicIP_old()
        {
            Write($"GetPublicIP_old");
            return 0;
        }

        public bool HandleIncomingPacket(IntPtr pData, int cbData, uint srcIP, uint srcPort)
        {
            Write($"HandleIncomingPacket");
            return true;
        }

        public int GetNextOutgoingPacket(IntPtr pOut, int cbMaxOut, uint pNetAdr, uint pPort)
        {
            SteamEmulator.Write($"DEBUG", "GetNextOutgoingPacket");
            return 0;
        }

        internal uint GetPublicIP()
        {
            Write($"GetPublicIP");
            //SteamIPAddress_t iPAddress = new SteamIPAddress_t(NetworkManager.GetIPAddress());
            return 0;
        }

        public void EnableHeartbeats(bool bActive)
        {
            Write($"EnableHeartbeats (Active = {bActive})");
        }

        public void SetHeartbeatInterval(int iHeartbeatInterval)
        {
            Write($"SetHeartbeatInterval (HeartbeatInterval = {iHeartbeatInterval})");
        }

        public void ForceHeartbeat()
        {
            Write($"ForceHeartbeat");
        }
        public SteamAPICall_t AssociateWithClan(ulong steamIDClan)
        {
            Write($"AssociateWithClan (SteamID = {steamIDClan})");
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t ComputeNewPlayerCompatibility(ulong steamIDNewPlayer)
        {
            Write($"ComputeNewPlayerCompatibility (SteamID = {steamIDNewPlayer})");
            return k_uAPICallInvalid;
        }

        public bool SendUserConnectAndAuthenticate_DEPRECATED(uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, ulong pSteamIDUser)
        {
            Write($"SendUserConnectAndAuthenticate_DEPRECATED");
            return true;
        }

        public void SendUserDisconnect_DEPRECATED(ulong steamIDUser)
        {
            Write($"SendUserDisconnect_DEPRECATED (SteamID = {steamIDUser})");
        }

        public void SetMasterServerHeartbeatInterval_DEPRECATED(int iHeartbeatInterval)
        {
            Write($"SetMasterServerHeartbeatInterval_DEPRECATED");
        }

        public void ForceMasterServerHeartbeat_DEPRECATED()
        {
            Write($"ForceMasterServerHeartbeat_DEPRECATED");
        }
    }
}

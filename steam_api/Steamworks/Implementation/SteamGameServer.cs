using System;
using System.Linq;
using System.Runtime.InteropServices;
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
            InterfaceVersion = "SteamGameServer015";
            ServerData = new GameServerData();
        }

        public bool InitGameServer(uint unIP, int usGamePort, int usQueryPort, uint unFlags, uint nGameAppId, string pchVersionString)
        {
            if (ShouldReportSecure())
            {
                unFlags |= Constants.k_unServerFlagSecure;
            }
            else
            {
                unFlags &= ~Constants.k_unServerFlagSecure;
            }

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
            ServerData.Secure = NormalizeSecure((byte)(IsSecureFlagSet(unFlags) ? 1 : 0));
            ServerData.AppId = nGameAppId;
            ServerData.VersionString = pchVersionString;

            var lobby = LobbyManager.GetLobbyByOwner((ulong)SteamEmulator.SteamID);
            if (lobby != null)
            {
                Write($"Setting lobby gameserver IP = {IP}, Port = {(uint)usQueryPort}");
            }
            else
            {

            }

            if (SkyNetApiClient.IsEnabled)
            {
                QueueRegisterGameServer();
            }

            return true;
        }

        public void SetProduct(string pszProduct)
        {
            Write($"SetProduct (Product = {pszProduct})");
            ServerData.Product = pszProduct;
            SyncServerState();
        }

        public void SetGameDescription(string pszGameDescription)
        {
            Write($"SetGameDescription (GameDescription = {pszGameDescription})");
            ServerData.Description = pszGameDescription;
            SyncServerState();
        }

        public void SetModDir(string pszModDir)
        {
            Write($"SetModDir (ModDir = {pszModDir})");
            ServerData.ModDir = pszModDir;
            SyncServerState();
        }

        public void SetDedicatedServer(bool bDedicated)
        {
            Write($"SetDedicatedServer (Dedicated = {bDedicated})");
            ServerData.Dedicated = bDedicated;
            SyncServerState();
        }

        public void LogOn(string pszToken)
        {
            Write($"LogOn (Token = {pszToken})");
            CallbackManager.ResetGameServerConnectionReplay();

            LoggedIn = true;
            if (SkyNetApiClient.IsEnabled)
            {
                QueueLogOnGameServer(pszToken, false);
            }

            EmitSecurePolicy();
            CallbackManager.AddCallbackGameServer(new SteamServersConnected_t());
        }

        public void LogOnAnonymous()
        {
            Write($"LogOnAnonymous");
            CallbackManager.ResetGameServerConnectionReplay();

            LoggedIn = true;
            if (SkyNetApiClient.IsEnabled)
            {
                QueueLogOnGameServer(string.Empty, true);
            }

            EmitSecurePolicy();
            CallbackManager.AddCallbackGameServer(new SteamServersConnected_t());
        }

        public void LogOff()
        {
            Write($"LogOff");
            if (SkyNetApiClient.IsEnabled)
            {
                WorkQueue.Enqueue("GameServer logoff", () => SkyNetApiClient.LogOffGameServer(), "gameserver:logoff");
            }

            LoggedIn = false;

            SteamServersDisconnected_t data = new SteamServersDisconnected_t()
            {
                m_eResult = EResult.k_EResultOK
            };
            CallbackManager.AddCallbackGameServer(data);
        }

        public bool BLoggedOn()
        {
            Write($"BLoggedOn");
            return LoggedIn;
        }

        public bool BSecure()
        {
            var secure = EnsureEffectiveSecure() != 0;
            Write($"BSecure = {secure}");
            return secure;
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
            SyncServerState();
        }

        public void SetBotPlayerCount(int cBotplayers)
        {
            Write($"SetBotPlayerCount (Botplayers = {cBotplayers})");
            ServerData.BotPlayers = cBotplayers;
            SyncServerState();
        }

        public void SetServerName(string pszServerName)
        {
            Write($"SetServerName (ServerName = {pszServerName})");
            ServerData.ServerName = pszServerName;
            SyncServerState();
        }

        public void SetMapName(string pszMapName)
        {
            Write($"SetMapName (MapName = {pszMapName})");
            ServerData.MapName = pszMapName;
            SyncServerState();
        }

        public void SetPasswordProtected(bool bPasswordProtected)
        {
            Write($"SetPasswordProtected (PasswordProtected = {bPasswordProtected})");
            ServerData.PasswordProtected = bPasswordProtected;
            SyncServerState();
        }

        public void SetSpectatorPort(int unSpectatorPort)
        {
            Write($"SetSpectatorPort (SpectatorPort = {unSpectatorPort})");
            ServerData.SpectatorPort = (uint)unSpectatorPort;
            SyncServerState();
        }

        public void SetSpectatorServerName(string pszSpectatorServerName)
        {
            Write($"SetSpectatorServerName (SpectatorServerName = {pszSpectatorServerName})");
            ServerData.SpectatorServerName = pszSpectatorServerName;
            SyncServerState();
        }

        public void ClearAllKeyValues()
        {
            Write($"ClearAllKeyValues");
            ServerData.KeyValues.Clear();
            SyncServerState();
        }

        public void SetKeyValue(string pKey, string pValue)
        {
            Write($"SetKeyValue (Key = {pKey}, Value = {pValue})");
            ServerData.KeyValues.TryAdd(pKey, pValue);
            SyncServerState();
        }

        public void SetGameTags(string pchGameTags)
        {
            Write($"SetGameTags (GameTags = {pchGameTags})");
            ServerData.GameTags = NormalizeGameTags(pchGameTags, EnsureEffectiveSecure() != 0);
            SyncServerState();
        }

        public void SetGameData(string pchGameData)
        {
            Write($"SetGameData (GameData = {pchGameData})");
            ServerData.GameData = pchGameData;
            SyncServerState();
        }

        public void SetRegion(string pszRegion)
        {
            Write($"SetRegion (Region = {pszRegion})");
            ServerData.Region = pszRegion;
            SyncServerState();
        }

        public bool SendUserConnectAndAuthenticate(uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, IntPtr pSteamIDUser)
        {
            var IPAddress = Common.GetIPAddress(unIPClient);
            Write($"SendUserConnectAndAuthenticate (Output = 0x{pSteamIDUser.ToInt64():X}, IP = {IPAddress}, Blob = {cubAuthBlobSize})");
            return TicketManager.ConnectAndAuthenticate(unIPClient, pvAuthBlob, cubAuthBlobSize, pSteamIDUser);
        }

        public bool SendUserConnectAndAuthenticate(uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, out ulong steamIDUser)
        {
            var IPAddress = Common.GetIPAddress(unIPClient);
            var approved = TicketManager.ConnectAndAuthenticate(unIPClient, pvAuthBlob, cubAuthBlobSize, out steamIDUser);
            Write($"SendUserConnectAndAuthenticate (SteamID = {(CSteamID)steamIDUser} | {IPAddress}, Blob = {cubAuthBlobSize}) = {approved}");
            return approved;
        }

        public CSteamID CreateUnauthenticatedUserConnection()
        {
            Write($"CreateUnauthenticatedUserConnection");
            return CSteamID.CreateUnauthenticatedUser();
        }

        public void SendUserDisconnect(ulong steamIDUser)
        {
            Write($"SendUserDisconnect (SteamID = {steamIDUser})");
            if (SkyNetApiClient.IsEnabled)
            {
                WorkQueue.Enqueue("GameServer user disconnect", () => SkyNetApiClient.DisconnectGameServerUser(steamIDUser),
                    "gameserver:user-disconnect:" + steamIDUser, true);
            }
            TicketManager.RemoveTicket(steamIDUser);
        }

        public bool BUpdateUserData(ulong steamIDUser, string pchPlayerName, uint uScore)
        {
            Write($"BUpdateUserData (SteamID = {steamIDUser}, PlayerName = {pchPlayerName})");
            if (SkyNetApiClient.IsEnabled)
            {
                WorkQueue.Enqueue("GameServer user data", () => SkyNetApiClient.UpdateGameServerUserData(steamIDUser, pchPlayerName, uScore),
                    "gameserver:user-data:" + steamIDUser);
            }

            return true;
        }

        public HAuthTicket GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
        {
            SteamEmulator.Write($"DEBUG", "GetAuthSessionTicket");
            return TicketManager.GetAuthSessionTicket(pTicket, cbMaxTicket, out pcbTicket, true);
        }

        public void SetAdvertiseServerActive(bool bActive)
        {
            Write($"SetAdvertiseServerActive (Active = {bActive})");
            SyncServerState();
        }

        public int BeginAuthSession(IntPtr pAuthTicket, int cbAuthTicket, ulong steamID)
        {
            var result = TicketManager.BeginAuthSession(pAuthTicket, cbAuthTicket, steamID, true);
            Write($"BeginAuthSession (SteamID = {(CSteamID)steamID}) = {(EBeginAuthSessionResult)result}");
            return result;
        }

        public void EndAuthSession(ulong steamID)
        {
            Write($"EndAuthSession (SteamID = {steamID})");
            TicketManager.EndAuthSession(steamID, true);
        }

        public void CancelAuthTicket(HAuthTicket hAuthTicket)
        {
            Write($"CancelAuthTicket");
            TicketManager.CancelAuthTicket(hAuthTicket, true);
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
            return GetPublicIPv4();
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

        public int GetNextOutgoingPacket(IntPtr pOut, int cbMaxOut, IntPtr pNetAdr, IntPtr pPort)
        {
            SteamEmulator.Write($"DEBUG", "GetNextOutgoingPacket");
            if (pNetAdr != IntPtr.Zero)
            {
                Marshal.WriteInt32(pNetAdr, 0);
            }
            if (pPort != IntPtr.Zero)
            {
                Marshal.WriteInt16(pPort, 0);
            }
            return 0;
        }

        internal SteamIPAddress_t GetPublicIP()
        {
            Write($"GetPublicIP");
            return new SteamIPAddress_t(GetPublicIPv4());
        }

        internal IntPtr GetPublicIP(IntPtr returnBuffer)
        {
            Write($"GetPublicIP");
            if (returnBuffer != IntPtr.Zero)
            {
                WriteSteamIPAddress(returnBuffer, GetPublicIPv4());
            }

            return returnBuffer;
        }

        internal IntPtr GetPublicIP(IntPtr arg0, IntPtr arg1)
        {
            var arg0IsInterface = InterfaceManager.IsKnownInterfacePointer(arg0);
            var arg1IsInterface = InterfaceManager.IsKnownInterfacePointer(arg1);
            var returnBuffer = arg1IsInterface || !arg0IsInterface ? arg0 : arg1;

            Write($"GetPublicIP ABI (Arg0=0x{arg0.ToInt64():X}, Arg1=0x{arg1.ToInt64():X}, Arg0Interface={arg0IsInterface}, Arg1Interface={arg1IsInterface}, ReturnBuffer=0x{returnBuffer.ToInt64():X})");

            if (returnBuffer == IntPtr.Zero || InterfaceManager.IsKnownInterfacePointer(returnBuffer))
            {
                return new IntPtr(unchecked((int)GetPublicIPv4()));
            }

            var ip = GetPublicIPv4();
            WriteSteamIPAddress(returnBuffer, ip);
            Write($"GetPublicIP ABI wrote IPv4=0x{ip:X8}");
            return returnBuffer;
        }

        private static void WriteSteamIPAddress(IntPtr destination, uint ipv4)
        {
            System.Runtime.InteropServices.Marshal.WriteInt32(destination, 0, unchecked((int)ipv4));
            System.Runtime.InteropServices.Marshal.WriteInt32(destination, 4, 0);
            System.Runtime.InteropServices.Marshal.WriteInt64(destination, 8, 0);
            System.Runtime.InteropServices.Marshal.WriteInt32(destination, 16, (int)SteamIPType.Type4);
        }

        internal uint GetPublicIPv4()
        {
            if (ServerData.IP != 0)
            {
                return ServerData.IP;
            }

            if (SkyNetApiClient.IsEnabled)
            {
                WorkQueue.Enqueue("GameServer public ip", () =>
                {
                    var ip = SkyNetApiClient.GetGameServerPublicIP();
                    if (ip != 0)
                    {
                        ServerData.IP = ip;
                    }
                }, "gameserver:public-ip");
            }

            return ServerData.IP;
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
            if (SkyNetApiClient.IsEnabled)
            {
                WorkQueue.Enqueue("GameServer heartbeat", () => SkyNetApiClient.HeartbeatGameServer(ServerData),
                    "gameserver:heartbeat");
            }
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

        private static bool IsSecureFlagSet(uint flags)
        {
            return (flags & Constants.k_unServerFlagSecure) != 0;
        }

        private static byte NormalizeSecure(byte secure)
        {
            return ShouldReportSecure() ? (byte)1 : (byte)0;
        }

        private static bool ShouldReportSecure()
        {
            return SteamEmulator.VacSecureGameServer && !IsDedicatedInsecureMode();
        }

        private static bool IsDedicatedInsecureMode()
        {
            var role = Environment.GetEnvironmentVariable("SKYNET_PROCESS_ROLE");
            if (!string.Equals(role, "dedicated", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var configured = Environment.GetEnvironmentVariable("SKYNET_DEDICATED_INSECURE");
            if (configured == "1" || configured?.Equals("true", StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            return Environment.CommandLine.IndexOf("-insecure", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private byte EnsureEffectiveSecure()
        {
            ServerData.Secure = NormalizeSecure(ServerData.Secure);
            if (ServerData.Secure != 0)
            {
                ServerData.Flags |= Constants.k_unServerFlagSecure;
            }
            else
            {
                ServerData.Flags &= ~Constants.k_unServerFlagSecure;
            }

            return ServerData.Secure;
        }

        private void EmitSecurePolicy()
        {
            EnsureEffectiveSecure();
            // Auth approval belongs to ticket validation paths.  Policy only reports
            // whether Steam considers this game server VAC-secure.
            CallbackManager.AddCallbackGameServer(new GSPolicyResponse_t
            {
                Secure = ServerData.Secure
            });
        }

        private static string NormalizeGameTags(string gameTags, bool secure)
        {
            if (string.IsNullOrWhiteSpace(gameTags))
            {
                return secure ? "secure" : string.Empty;
            }

            var tags = gameTags
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(tag => tag.Trim())
                .Where(tag => tag.Length != 0 &&
                    !tag.Equals("secure", StringComparison.OrdinalIgnoreCase) &&
                    !tag.Equals("insecure", StringComparison.OrdinalIgnoreCase))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (secure)
            {
                tags.Add("secure");
            }

            return string.Join(",", tags);
        }

        public bool SendUserConnectAndAuthenticate_DEPRECATED(uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, IntPtr pSteamIDUser)
        {
            Write($"SendUserConnectAndAuthenticate_DEPRECATED");
            return SendUserConnectAndAuthenticate(unIPClient, pvAuthBlob, cubAuthBlobSize, pSteamIDUser);
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

        private void SyncServerState()
        {
            if (LoggedIn && SkyNetApiClient.IsEnabled)
            {
                WorkQueue.Enqueue("GameServer state", () => SkyNetApiClient.UpdateGameServerState(ServerData),
                    "gameserver:state");
            }
        }

        private void QueueRegisterGameServer()
        {
            WorkQueue.Enqueue("GameServer register", () =>
            {
                var result = SkyNetApiClient.RegisterGameServer(ServerData);
                ApplyServerResult(result);
            }, "gameserver:register", true);
        }

        private void QueueLogOnGameServer(string token, bool anonymous)
        {
            WorkQueue.Enqueue("GameServer logon", () =>
            {
                var result = SkyNetApiClient.LogOnGameServer(ServerData, token, anonymous);
                ApplyServerResult(result);
                if (result != null && !result.Success)
                {
                    Write("GameServer backend logon failed; keeping local gameserver logged on");
                }
            }, "gameserver:logon", true);
        }

        private void ApplyServerResult(SkyNetApiClient.ApiGameServerResult result)
        {
            if (result == null)
            {
                return;
            }

            if (result.PublicIP != 0)
            {
                ServerData.IP = result.PublicIP;
            }

            ServerData.Secure = NormalizeSecure(result.Secure);
            if (ServerData.Secure != 0)
            {
                ServerData.Flags |= Constants.k_unServerFlagSecure;
            }
            else
            {
                ServerData.Flags &= ~Constants.k_unServerFlagSecure;
            }
            ServerData.GameTags = NormalizeGameTags(ServerData.GameTags, ServerData.Secure != 0);
        }
    }
}

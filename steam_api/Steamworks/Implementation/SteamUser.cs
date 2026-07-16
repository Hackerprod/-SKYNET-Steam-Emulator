using System;
using System.Runtime.InteropServices;
using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks.Interfaces;

using SteamAPICall_t = System.UInt64;
using HSteamUser = System.UInt32;
using HAuthTicket = System.UInt32;
using CGameID = System.UInt64;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamUser : ISteamInterface
    {
        public static SteamUser Instance;

        private bool Recording;
        private bool SteamServersConnectedQueued;

        public SteamUser()
        {
            Instance = this;
            InterfaceName = "SteamUser";
            InterfaceVersion = "SteamUser023";
        }

        public HSteamUser GetHSteamUser()
        {
            Write("GetHSteamUser");
            return SteamEmulator.HSteamUser;
        }

        public bool BLoggedOn()
        {
            if (APIClient.IsEnabled)
            {
                // Report the cached connection state without blocking. If we are
                // not connected yet, the handshake is kicked off in background and
                // the connected callback fires from OnServerSessionConnected().
                bool connected = APIClient.IsConnected;
                if (connected)
                {
                    APIClient.QueueSelfRefresh();
                    QueueSteamServersConnected();
                }
                else
                {
                    APIClient.EnsureSession();
                }

                Write($"BLoggedOn = {connected}");
                return connected;
            }

            Write("BLoggedOn");
            QueueSteamServersConnected();
            return true;
        }

        // Invoked from the background session handshake once a token is obtained,
        // so the game receives SteamServersConnected_t without polling.
        public void OnServerSessionConnected()
        {
            QueueSteamServersConnected();
        }

        private void QueueSteamServersConnected()
        {
            if (SteamServersConnectedQueued)
            {
                return;
            }

            SteamServersConnectedQueued = true;
            CallbackManager.AddCallback(new SteamServersConnected_t());
        }

        public CSteamID GetSteamID()
        {
            if (APIClient.IsEnabled)
            {
                if (!APIClient.IsConnected)
                {
                    APIClient.EnsureSession();
                    Write("GetSteamID unavailable: no active server session");
                    return CSteamID.Invalid;
                }

                // Connected: return the cached identity and refresh in background.
                APIClient.QueueSelfRefresh();
            }

            var SteamId = SteamEmulator.SteamID;
            Write($"GetSteamID {SteamId}");
            return SteamId;
        }

        public int InitiateGameConnection(IntPtr pAuthBlob, int cbMaxAuthBlob, ulong steamIDGameServer, CGameID gameID, uint unIPServer, uint usPortServer, bool bSecure)
        {
            return CreateGameConnectionAuthBlob("InitiateGameConnection", pAuthBlob, cbMaxAuthBlob, steamIDGameServer, unIPServer, usPortServer, bSecure);
        }

        internal int InitiateGameConnection(IntPtr pAuthBlob, int cbMaxAuthBlob, ulong steamIDGameServer, uint unIPServer, uint usPortServer, bool bSecure)
        {
            return CreateGameConnectionAuthBlob("InitiateGameConnection2", pAuthBlob, cbMaxAuthBlob, steamIDGameServer, unIPServer, usPortServer, bSecure);
        }

        public void TerminateGameConnection(uint unIPServer, uint usPortServer)
        {
            Write("TerminateGameConnection");
        }

        public void TrackAppUsageEvent(CGameID gameID, int eAppUsageEvent, string pchExtraInfo)
        {
            Write("TrackAppUsageEvent");
        }

        public bool GetUserDataFolder(out string pchBuffer, int cubBuffer)
        {
            Write("GetUserDataFolder");
            pchBuffer = SteamEmulator.SteamRemoteStorage.StoragePath;
            if (cubBuffer == 0) return false;
            return true;
        }

        public bool GetUserDataFolder(IntPtr pchBuffer, int cubBuffer)
        {
            Write("GetUserDataFolder");
            return NativeStringCache.WriteUtf8Buffer(pchBuffer, cubBuffer, SteamEmulator.SteamRemoteStorage.StoragePath);
        }

        public void StartVoiceRecording()
        {
            Write("StartVoiceRecording");
            Recording = AudioManager.StartVoiceRecording();
        }

        public void StopVoiceRecording()
        {
            Write("StopVoiceRecording");
            AudioManager.StopVoiceRecording();
            Recording = false;
        }

        public EVoiceResult GetAvailableVoice(out uint pcbCompressed, out uint pcbUncompressed_Deprecated, uint nUncompressedVoiceDesiredSampleRate)
        {
            var Result = EVoiceResult.k_EVoiceResultNotRecording;
            pcbCompressed = 0;
            pcbUncompressed_Deprecated = 0;
            try
            {
                if (Recording)
                {
                    if (AudioManager.GetAvailableVoice(out var Compressed, out var Uncompressed))
                    {
                        pcbCompressed = Compressed;
                        pcbUncompressed_Deprecated = Uncompressed;
                        Result = EVoiceResult.k_EVoiceResultOK;
                    }
                    else
                        Result = EVoiceResult.k_EVoiceResultNoData;
                }
            }
            catch (Exception ex)
            {
                Write($"GetAvailableVoice failed: {ex.GetType().Name}: {ex.Message}");
                Result = EVoiceResult.k_EVoiceResultNotRecording;
            }
            //Write($"GetAvailableVoice (Compressed = {pcbCompressed}, Uncompressed = {pcbUncompressed_Deprecated}) = {Result}");
            return Result;
        }

        public EVoiceResult GetAvailableVoice(IntPtr pcbCompressed, IntPtr pcbUncompressed_Deprecated, uint nUncompressedVoiceDesiredSampleRate)
        {
            // Native callers may pass NULL for deprecated output pointers; write only when present.
            uint compressed;
            uint uncompressed;
            var result = GetAvailableVoice(out compressed, out uncompressed, nUncompressedVoiceDesiredSampleRate);
            WriteUInt32(pcbCompressed, compressed);
            WriteUInt32(pcbUncompressed_Deprecated, uncompressed);
            return result;
        }

        public EVoiceResult GetVoice(bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, out uint nBytesWritten, bool bWantUncompressed, IntPtr pUncompressedDestBuffer, uint cbUncompressedDestBufferSize, out uint nUncompressBytesWritten, uint nUncompressedVoiceDesiredSampleRate)
        {
            var Result = EVoiceResult.k_EVoiceResultNotRecording;
            nUncompressBytesWritten = 0;
            nBytesWritten = 0;
            try
            {
                if (Recording)
                {
                    uint maxBytes = Math.Max(bWantCompressed ? cbDestBufferSize : 0, bWantUncompressed ? cbUncompressedDestBufferSize : 0);
                    if (AudioManager.GetVoice(maxBytes, out byte[] buffer))
                    {
                        // Compressed 
                        if (bWantCompressed)
                        {
                            var Size = cbDestBufferSize < buffer.Length ? cbDestBufferSize : (uint)buffer.Length;
                            if (Size > 0 && pDestBuffer != IntPtr.Zero)
                            {
                                Marshal.Copy(buffer, 0, pDestBuffer, (int)Size);
                                nBytesWritten = Size;
                            }
                        }

                        // Uncompressed
                        if (bWantUncompressed)
                        {
                            var Size = cbUncompressedDestBufferSize < buffer.Length ? cbUncompressedDestBufferSize : (uint)buffer.Length;
                            if (Size > 0 && pUncompressedDestBuffer != IntPtr.Zero)
                            {
                                Marshal.Copy(buffer, 0, pUncompressedDestBuffer, (int)Size);
                                nUncompressBytesWritten = Size;
                            }
                        }

                        Result = EVoiceResult.k_EVoiceResultOK;
                    }
                    else
                        Result = EVoiceResult.k_EVoiceResultNoData;
                }
            }
            catch (Exception ex)
            {
                Write($"GetVoice failed: {ex.GetType().Name}: {ex.Message}");
                Result = EVoiceResult.k_EVoiceResultNotRecording;
            }
            //Write($"GetVoice (BytesWritte = {nBytesWritten}) = {Result}");
            return Result;
        }

        public EVoiceResult GetVoice(bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, IntPtr nBytesWritten, bool bWantUncompressed, IntPtr pUncompressedDestBuffer, uint cbUncompressedDestBufferSize, IntPtr nUncompressBytesWritten, uint nUncompressedVoiceDesiredSampleRate)
        {
            uint compressedBytes;
            uint uncompressedBytes;
            var result = GetVoice(bWantCompressed, pDestBuffer, cbDestBufferSize, out compressedBytes, bWantUncompressed, pUncompressedDestBuffer, cbUncompressedDestBufferSize, out uncompressedBytes, nUncompressedVoiceDesiredSampleRate);
            WriteUInt32(nBytesWritten, compressedBytes);
            WriteUInt32(nUncompressBytesWritten, uncompressedBytes);
            return result;
        }

        public EVoiceResult DecompressVoice(IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, out uint nBytesWritten, uint nDesiredSampleRate)
        {
            nBytesWritten = 0;
            EVoiceResult Result = EVoiceResult.k_EVoiceResultNoData;
            if (cbCompressed != 0)
            {
                try
                {
                    var Size = cbCompressed > cbDestBufferSize ? cbDestBufferSize : cbCompressed;
                    if (Size > 0 && pCompressed != IntPtr.Zero && pDestBuffer != IntPtr.Zero)
                    {
                        byte[] buffer = new byte[Size];
                        Marshal.Copy(pCompressed, buffer, 0, (int)Size);
                        Marshal.Copy(buffer, 0, pDestBuffer, (int)Size);
                        nBytesWritten = Size;
                        Result = EVoiceResult.k_EVoiceResultOK;
                    }
                }
                catch 
                {
                }
            }
            Write($"DecompressVoice = {Result}");
            return Result;
        }

        public EVoiceResult DecompressVoice(IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, IntPtr nBytesWritten, uint nDesiredSampleRate)
        {
            uint written;
            var result = DecompressVoice(pCompressed, cbCompressed, pDestBuffer, cbDestBufferSize, out written, nDesiredSampleRate);
            WriteUInt32(nBytesWritten, written);
            return result;
        }

        private static void WriteUInt32(IntPtr target, uint value)
        {
            if (target != IntPtr.Zero)
            {
                Marshal.WriteInt32(target, unchecked((int)value));
            }
        }

        public uint GetVoiceOptimalSampleRate()
        {
            int SampleRate = AudioManager.SampleRate;
            Write($"GetVoiceOptimalSampleRate {SampleRate}");
            return (uint)SampleRate;
        }

        public HAuthTicket GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, out uint pcbTicket)
        {
            Write("GetAuthSessionTicket");
            HAuthTicket Ticket = TicketManager.GetAuthSessionTicket(pTicket, cbMaxTicket, out pcbTicket, false);
            return Ticket;
        }

        public int BeginAuthSession(IntPtr pAuthTicket, int cbAuthTicket, ulong steamID)
        {
            Write("BeginAuthSession");
            return TicketManager.BeginAuthSession(pAuthTicket, cbAuthTicket, steamID, false);
        }

        public void EndAuthSession(ulong steamID)
        {
            Write("EndAuthSession");
            TicketManager.EndAuthSession(steamID, false);
        }

        public void CancelAuthTicket(uint hAuthTicket)
        {
            Write("CancelAuthTicket");
            TicketManager.CancelAuthTicket(hAuthTicket, false);
        }

        public int UserHasLicenseForApp(ulong steamID, uint appID)
        {
            Write("EUserHasLicenseForAppResult");
            return (int)EUserHasLicenseForAppResult.k_EUserHasLicenseResultHasLicense;
        }

        public bool BIsBehindNAT()
        {
            Write("BIsBehindNAT");
            return false;
        }

        public void AdvertiseGame(ulong steamIDGameServer, uint unIPServer, uint usPortServer)
        {
            Write("AdvertiseGame");
        }

        public SteamAPICall_t RequestEncryptedAppTicket(IntPtr pDataToInclude, int cbDataToInclude)
        {
            Write("RequestEncryptedAppTicket");
            // EncryptedAppTicketResponse_t
            MutexHelper.Wait("RequestEncryptedAppTicket", delegate
            {

            });
            return k_uAPICallInvalid;
        }

        public bool GetEncryptedAppTicket(IntPtr pTicket, int cbMaxTicket, IntPtr pcbTicket)
        {
            Write("GetEncryptedAppTicket");
            if (pcbTicket != IntPtr.Zero)
            {
                Marshal.WriteInt32(pcbTicket, 0);
            }
            return false;
        }

        public bool GetEncryptedAppTicket(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
        {
            pcbTicket = 0;
            return GetEncryptedAppTicket(pTicket, cbMaxTicket, IntPtr.Zero);
        }

        public int GetGameBadgeLevel(int nSeries, bool bFoil)
        {
            Write("GetGameBadgeLevel");
            return 0;
        }

        public int GetPlayerSteamLevel()
        {
            APIClient.QueueSelfRefresh();
            int level = StateCache.GetSelfPlayerLevel();
            Write($"GetPlayerSteamLevel {level}");
            return level == 0 ? 100 : level;
        }

        public SteamAPICall_t RequestStoreAuthURL(string pchRedirectURL)
        {
            Write($"RequestStoreAuthURL {pchRedirectURL}");
            // StoreAuthURLResponse_t
            return k_uAPICallInvalid;
        }

        public bool BIsPhoneVerified()
        {
            //Write("BIsPhoneVerified");
            return true;
        }

        public bool BIsTwoFactorEnabled()
        {
            Write("BIsTwoFactorEnabled");
            return false;
        }

        public bool BIsPhoneIdentifying()
        {
            Write("BIsPhoneIdentifying");
            return false;
        }

        public bool BIsPhoneRequiringVerification()
        {
            Write("BIsPhoneRequiringVerification");
            return false;
        }

        public SteamAPICall_t GetMarketEligibility()
        {
            Write("GetMarketEligibility");
            // MarketEligibilityResponse_t
            return default;
        }

        public SteamAPICall_t GetDurationControl()
        {
            Write("GetDurationControl");
            // DurationControl_t
            return k_uAPICallInvalid;
        }

        public bool BSetDurationControlOnlineState(int eNewState)
        {
            Write("BSetDurationControlOnlineState");
            return false;
        }

        public int InitiateGameConnection_DEPRECATED(IntPtr pAuthBlob, int cbMaxAuthBlob, ulong steamIDGameServer, uint unIPServer, ushort usPortServer, bool bSecure)
        {
            return CreateGameConnectionAuthBlob("InitiateGameConnection_DEPRECATED", pAuthBlob, cbMaxAuthBlob, steamIDGameServer, unIPServer, usPortServer, bSecure);
        }

        public void TerminateGameConnection_DEPRECATED(uint unIPServer, ushort usPortServer)
        {
            Write("TerminateGameConnection_DEPRECATED");
        }



        internal uint GetAuthTicketForWebApi(string pchIdentity)
        {
            Write($"GetAuthTicketForWebApi {pchIdentity}");
            return 1;
        }

        private int CreateGameConnectionAuthBlob(string caller, IntPtr pAuthBlob, int cbMaxAuthBlob, ulong steamIDGameServer, uint unIPServer, uint usPortServer, bool bSecure)
        {
            uint ticketSize = 0;
            Write($"{caller} (GameServer = {(CSteamID)steamIDGameServer}, IP = {Common.GetIPAddress(unIPServer)}, Port = {usPortServer}, Secure = {bSecure}, MaxBlob = {cbMaxAuthBlob})");

            MutexHelper.Wait(caller, delegate
            {
                TicketManager.GetAuthSessionTicket(pAuthBlob, cbMaxAuthBlob, out ticketSize, false);
            });

            Write($"{caller} = {ticketSize}");
            return (int)ticketSize;
        }
    }
}

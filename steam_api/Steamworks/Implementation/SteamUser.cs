using System;
using System.Runtime.InteropServices;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks.Interfaces;

using SteamAPICall_t = System.UInt64;
using HSteamUser = System.UInt32;
using HAuthTicket = System.UInt32;
using CGameID = System.UInt32;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamUser : ISteamInterface
    {
        public static SteamUser Instance;

        private bool Recording;

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
            if (SkyNetApiClient.IsEnabled)
            {
                bool loggedOn = SkyNetApiClient.EnsureSession() || SkyNetApiClient.RefreshSelf();
                Write($"BLoggedOn = {loggedOn}");
                return loggedOn;
            }

            Write("BLoggedOn");
            return true;
        }

        public CSteamID GetSteamID()
        {
            SkyNetApiClient.RefreshSelf();
            var SteamId = SteamEmulator.SteamID;
            Write($"GetSteamID {SteamId}");
            return SteamId;
        }

        public int InitiateGameConnection(IntPtr pAuthBlob, int cbMaxAuthBlob, ulong steamIDGameServer, CGameID gameID, uint unIPServer, uint usPortServer, bool bSecure)
        {
            Write("InitiateGameConnection");
            MutexHelper.Wait("InitiateGameConnection", delegate
            {
                // TODO
            });
            return 0;
        }

        internal int InitiateGameConnection(IntPtr pAuthBlob, int cbMaxAuthBlob, ulong steamIDGameServer, uint unIPServer, uint usPortServer, bool bSecure)
        {
            Write("InitiateGameConnection");
            MutexHelper.Wait("InitiateGameConnection2", delegate
            {
                // TODO
            });
            return 0;
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

        public void StartVoiceRecording()
        {
            Write("StartVoiceRecording");
            AudioManager.StartVoiceRecording();
            Recording = true;
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
            //Write($"GetAvailableVoice (Compressed = {pcbCompressed}, Uncompressed = {pcbUncompressed_Deprecated}) = {Result}");
            return Result;
        }

        public EVoiceResult GetVoice(bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, out uint nBytesWritten, bool bWantUncompressed, IntPtr pUncompressedDestBuffer, uint cbUncompressedDestBufferSize, out uint nUncompressBytesWritten, uint nUncompressedVoiceDesiredSampleRate)
        {
            var Result = EVoiceResult.k_EVoiceResultNotRecording;
            nUncompressBytesWritten = 0;
            nBytesWritten = 0;
            if (Recording)
            {
                if (AudioManager.GetVoice(out byte[] buffer))
                {
                    // Compressed 
                    if (bWantCompressed)
                    {
                        var Size = cbDestBufferSize < buffer.Length ? cbDestBufferSize : (uint)buffer.Length;
                        Marshal.Copy(buffer, 0, pDestBuffer, (int)Size);
                        nBytesWritten = Size;
                    }

                    // Uncompressed
                    if (bWantUncompressed)
                    {
                        var Size = cbUncompressedDestBufferSize < buffer.Length ? cbUncompressedDestBufferSize : (uint)buffer.Length;
                        Marshal.Copy(buffer, 0, pUncompressedDestBuffer, (int)Size);
                        nUncompressBytesWritten = Size;
                    }

                    Result = EVoiceResult.k_EVoiceResultOK;
                }
                else
                    Result = EVoiceResult.k_EVoiceResultNoData;
            }
            //Write($"GetVoice (BytesWritte = {nBytesWritten}) = {Result}");
            return Result;
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
                    byte[] buffer = new byte[Size];
                    Marshal.Copy(pCompressed, buffer, 0, (int)Size);
                    Marshal.Copy(buffer, 0, pDestBuffer, (int)Size);
                    nBytesWritten = Size;
                    Result = EVoiceResult.k_EVoiceResultOK;
                }
                catch 
                {
                }
            }
            Write($"DecompressVoice = {Result}");
            return Result;
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

        public bool GetEncryptedAppTicket(IntPtr pTicket, int cbMaxTicket, uint pcbTicket)
        {
            Write("GetEncryptedAppTicket");
            return false;
        }

        public int GetGameBadgeLevel(int nSeries, bool bFoil)
        {
            Write("GetGameBadgeLevel");
            return 0;
        }

        public int GetPlayerSteamLevel()
        {
            SkyNetApiClient.RefreshSelf();
            int level = SkyNetStateCache.GetSelfPlayerLevel();
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
            Write("InitiateGameConnection_DEPRECATED");
            return 1;
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
    }
}

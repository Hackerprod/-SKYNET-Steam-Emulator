using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helper;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks;
using SKYNET.Types;
using Steamworks;
using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamUser : ISteamInterface
    {
        private bool Recording;
        private SteamAPICall_t k_uAPICallInvalid = 0x0;

        public SteamUser()
        {
            InterfaceVersion = "SteamUser";
        }

        public int GetHSteamUser()
        {
            Write("GetHSteamUser");
            return (int)SteamEmulator.HSteamUser;
        }

        public bool BLoggedOn()
        {
            Write("BLoggedOn");
            return true;
        }

        public CSteamID GetSteamID()
        {
            var SteamId = SteamEmulator.SteamId;
            Write($"GetSteamID {SteamId}");
            return SteamId;
        }

        public int InitiateGameConnection(IntPtr pAuthBlob, int cbMaxAuthBlob, ulong steamIDGameServer, uint unIPServer, uint usPortServer, bool bSecure)
        {
            Write("InitiateGameConnection");
            MutexHelper.Wait("InitiateGameConnection", delegate
            {
                // TODO
            });
            return 0;
        }

        public void TerminateGameConnection(uint unIPServer, uint usPortServer)
        {
            Write("TerminateGameConnection");
        }

        public void TrackAppUsageEvent(IntPtr gameID, int eAppUsageEvent, string pchExtraInfo = "")
        {
            Write("TrackAppUsageEvent");
        }

        public bool GetUserDataFolder(ref string pchBuffer, int cubBuffer)
        {
            Write("GetUserDataFolder");
            if (cubBuffer == 0) return false;
            pchBuffer = SteamEmulator.SteamRemoteStorage.StoragePath;
            return true;
        }

        public void StartVoiceRecording()
        {
            Write("StartVoiceRecording");
            Recording = true;
        }

        public void StopVoiceRecording()
        {
            Write("StopVoiceRecording");
            Recording = false;
        }

        public int GetAvailableVoice(uint pcbCompressed, uint pcbUncompressed_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
        {
            //Write("GetAvailableVoice");
            return (int)EVoiceResult.k_EVoiceResultOK;
        }

        public int GetVoice(bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, ref uint nBytesWritten, bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint cbUncompressedDestBufferSize_Deprecated, uint nUncompressBytesWritten_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
        {
            Write("GetVoice");
            if (Recording) return (int)EVoiceResult.k_EVoiceResultNotRecording;
            nBytesWritten = 0;
            return (int)EVoiceResult.k_EVoiceResultNoData;
        }

        public int DecompressVoice(IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, uint nBytesWritten, uint nDesiredSampleRate)
        {
            Write("DecompressVoice");
            return (int)EVoiceResult.k_EVoiceResultNoData;
        }

        public int GetVoiceOptimalSampleRate()
        {
            Write("GetVoiceOptimalSampleRate");
            return 4800;
        }

        public uint GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
        {
            Write("GetAuthSessionTicket");
            pcbTicket = 10;
            return 100;
        }

        public int BeginAuthSession(IntPtr pAuthTicket, int cbAuthTicket, ulong steamID)
        {
            Write("BeginAuthSession");
            MutexHelper.Wait("BeginAuthSession", delegate
            {
                // TODO
            });
            return (int)EBeginAuthSessionResult.k_EBeginAuthSessionResultOK;
        }

        public void EndAuthSession(ulong steamID)
        {
            Write("EndAuthSession");
            MutexHelper.Wait("EndAuthSession", delegate
            {
                // TODO
            });
        }

        public void CancelAuthTicket(uint hAuthTicket)
        {
            Write("CancelAuthTicket");
            MutexHelper.Wait("CancelAuthTicket", delegate
            {
                // TODO
            });
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
            Write("GetPlayerSteamLevel");
            return 100;
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
    }
}
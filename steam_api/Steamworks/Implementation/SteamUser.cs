using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using SKYNET.Types;
using Steamworks;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamUser : ISteamInterface
    {
        public IntPtr MemoryAddress { get; set; }
        public string InterfaceVersion { get; set; }

        public SteamUser()
        {
            InterfaceVersion = "SteamUser";
        }

        public HSteamUser GetHSteamUser(IntPtr _)
        {
            Write("GetHSteamUser");
            return SteamEmulator.HSteamUser;
        }

        public bool BLoggedOn(IntPtr _)
        {
            Write("BLoggedOn");
            return true;
        }

        public SteamId GetSteamID(IntPtr _)
        {
            var SId = new SteamId();
            SId.Value = SteamEmulator.SteamId;
            Write($"GetSteamID {SId.Value}");
            return SId;
        }

        public int InitiateGameConnection(IntPtr _, IntPtr pAuthBlob, int cbMaxAuthBlob, IntPtr steamIDGameServer, uint unIPServer, uint usPortServer, bool bSecure)
        {
            Write("InitiateGameConnection");
            return 0;
        }

        public void TerminateGameConnection(IntPtr _, uint unIPServer, uint usPortServer)
        {
            Write("TerminateGameConnection");
        }

        public void TrackAppUsageEvent(IntPtr _, IntPtr gameID, int eAppUsageEvent, string pchExtraInfo = "")
        {
            Write("TrackAppUsageEvent");
        }

        public bool GetUserDataFolder(IntPtr _, string pchBuffer, int cubBuffer)
        {
            Write("GetUserDataFolder");
            return false;
        }

        public void StartVoiceRecording(IntPtr _)
        {
            Write("StartVoiceRecording");
        }

        public void StopVoiceRecording(IntPtr _)
        {
            Write("StopVoiceRecording");
        }

        public EVoiceResult GetAvailableVoice(IntPtr _, uint pcbCompressed, uint pcbUncompressed_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
        {
            Write("GetAvailableVoice");
            return EVoiceResult.k_EVoiceResultNoData;
        }

        public EVoiceResult GetVoice(IntPtr _, bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, uint nBytesWritten, bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint cbUncompressedDestBufferSize_Deprecated, uint nUncompressBytesWritten_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
        {
            Write("GetVoice");
            return EVoiceResult.k_EVoiceResultNoData;
        }

        public EVoiceResult DecompressVoice(IntPtr _, IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, uint nBytesWritten, uint nDesiredSampleRate)
        {
            Write("DecompressVoice");
            return EVoiceResult.k_EVoiceResultNoData;
        }

        public uint GetVoiceOptimalSampleRate(IntPtr _)
        {
            Write("GetVoiceOptimalSampleRate");
            return 0;
        }

        public HAuthTicket GetAuthSessionTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, uint pcbTicket)
        {
            Write("GetAuthSessionTicket");
            return HAuthTicket.Invalid;
        }

        public EBeginAuthSessionResult BeginAuthSession(IntPtr _, IntPtr pAuthTicket, int cbAuthTicket, IntPtr steamID)
        {
            Write("BeginAuthSession");
            return EBeginAuthSessionResult.k_EBeginAuthSessionResultOK;
        }

        public void EndAuthSession(IntPtr _, IntPtr steamID)
        {
            Write("EndAuthSession");
        }

        public void CancelAuthTicket(IntPtr _, HAuthTicket hAuthTicket)
        {
            Write("CancelAuthTicket");
        }

        public EUserHasLicenseForAppResult UserHasLicenseForApp(IntPtr _, IntPtr steamID, AppId_t appID)
        {
            Write("EUserHasLicenseForAppResult");
            return EUserHasLicenseForAppResult.k_EUserHasLicenseResultHasLicense;
        }

        public bool BIsBehindNAT(IntPtr _)
        {
            Write("BIsBehindNAT");
            return false;
        }

        public void AdvertiseGame(IntPtr _, IntPtr steamIDGameServer, uint unIPServer, uint usPortServer)
        {
            Write("AdvertiseGame");
        }

        public SteamAPICall_t RequestEncryptedAppTicket(IntPtr _, IntPtr pDataToInclude, int cbDataToInclude)
        {
            Write("RequestEncryptedAppTicket");
            return SteamAPICall_t.Invalid;
        }

        public bool GetEncryptedAppTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, uint pcbTicket)
        {
            Write("GetEncryptedAppTicket");
            return false;
        }

        public int GetGameBadgeLevel(IntPtr _, int nSeries, bool bFoil)
        {
            Write("GetGameBadgeLevel");
            return 0;
        }

        public int GetPlayerSteamLevel(IntPtr _)
        {
            Write("GetPlayerSteamLevel");
            return 0;
        }

        public SteamAPICall_t RequestStoreAuthURL(IntPtr _, string pchRedirectURL)
        {
            Write("RequestStoreAuthURL");
            return SteamAPICall_t.Invalid;
        }

        public bool BIsPhoneVerified(IntPtr _)
        {
            Write("BIsPhoneVerified");
            return true;
        }

        public bool BIsTwoFactorEnabled(IntPtr _)
        {
            Write("BIsTwoFactorEnabled");
            return false;
        }

        public bool BIsPhoneIdentifying(IntPtr _)
        {
            Write("BIsPhoneIdentifying");
            return false;
        }

        public bool BIsPhoneRequiringVerification(IntPtr _)
        {
            Write("BIsPhoneRequiringVerification");
            return false;
        }

        public SteamAPICall_t GetMarketEligibility(IntPtr _)
        {
            Write("GetMarketEligibility");
            return default;
        }

        public SteamAPICall_t GetDurationControl(IntPtr _)
        {
            Write("GetDurationControl");
            return default;
        }

        public bool BSetDurationControlOnlineState(IntPtr _, EDurationControlOnlineState eNewState)
        {
            Write("BSetDurationControlOnlineState");
            return false;
        }

        private void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }
    }
}
using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using SKYNET.Steamworks.Types;
using SKYNET.Types;
using Steamworks;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamUser : ISteamInterface
    {
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

        public SteamID GetSteamID()
        {
            var SId = SteamEmulator.SteamId;
            Write($"GetSteamID {(ulong)SId}");
            return SId;
        }

        public int InitiateGameConnection(IntPtr pAuthBlob, int cbMaxAuthBlob, ulong steamIDGameServer, uint unIPServer, uint usPortServer, bool bSecure)
        {
            Write("InitiateGameConnection");
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

        public bool GetUserDataFolder(string pchBuffer, int cubBuffer)
        {
            Write("GetUserDataFolder");
            return false;
        }

        public void StartVoiceRecording()
        {
            Write("StartVoiceRecording");
        }

        public void StopVoiceRecording()
        {
            Write("StopVoiceRecording");
        }

        public int GetAvailableVoice(uint pcbCompressed, uint pcbUncompressed_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
        {
            Write("GetAvailableVoice");
            return (int)EVoiceResult.k_EVoiceResultNoData;
        }

        public int GetVoice(bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, ref uint nBytesWritten, bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint cbUncompressedDestBufferSize_Deprecated, uint nUncompressBytesWritten_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
        {
            Write("GetVoice");
            nBytesWritten = 0;
            return (int)EVoiceResult.k_EVoiceResultNoData;
        }

        public int DecompressVoice(IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, uint nBytesWritten, uint nDesiredSampleRate)
        {
            Write("DecompressVoice");
            return (int)EVoiceResult.k_EVoiceResultNoData;
        }

        public uint GetVoiceOptimalSampleRate()
        {
            Write("GetVoiceOptimalSampleRate");
            return 2400;
        }

        public uint GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
        {
            Write("GetAuthSessionTicket");
            pcbTicket = 0;
            return 0;
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

        public void CancelAuthTicket(uint hAuthTicket)
        {
            Write("CancelAuthTicket");
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

        public ulong RequestEncryptedAppTicket(IntPtr pDataToInclude, int cbDataToInclude)
        {
            Write("RequestEncryptedAppTicket");
            return 0;
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
            return 0;
        }

        public ulong RequestStoreAuthURL(string pchRedirectURL)
        {
            Write("RequestStoreAuthURL");
            return 0;
        }

        public bool BIsPhoneVerified()
        {
            Write("BIsPhoneVerified");
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

        public ulong GetMarketEligibility()
        {
            Write("GetMarketEligibility");
            return default;
        }

        public ulong GetDurationControl()
        {
            Write("GetDurationControl");
            return default;
        }

        public bool BSetDurationControlOnlineState(int eNewState)
        {
            Write("BSetDurationControlOnlineState");
            return false;
        }
    }
}
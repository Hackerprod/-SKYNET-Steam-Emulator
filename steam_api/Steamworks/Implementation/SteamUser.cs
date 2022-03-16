using System;
using System.Runtime.InteropServices;
using SKYNET.Interface;
using Steamworks;

namespace SKYNET.Managers
{
    [Map("SteamUser")]
    public class SteamUser : IBaseInterface, ISteamUser
    {
        public HSteamUser GetHSteamUser()
        {
            return default;
        }

        public bool BLoggedOn()
        {
            return false;
        }

        public IntPtr GetSteamID()
        {
            return default;
        }

        public int InitiateGameConnection(IntPtr pAuthBlob, int cbMaxAuthBlob, IntPtr steamIDGameServer, uint unIPServer, uint usPortServer, bool bSecure)
        {
            return 0;
        }

        public void TerminateGameConnection(uint unIPServer, uint usPortServer)
        {
            //
        }

        public void TrackAppUsageEvent(IntPtr gameID, int eAppUsageEvent, string pchExtraInfo = "")
        {
            //
        }

        public bool GetUserDataFolder(string pchBuffer, int cubBuffer)
        {
            return false;
        }

        public void StartVoiceRecording()
        {
            //
        }

        public void StopVoiceRecording()
        {
            //
        }

        public EVoiceResult GetAvailableVoice(uint pcbCompressed, uint pcbUncompressed_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
        {
            return default;
        }

        public EVoiceResult GetVoice(bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, uint nBytesWritten, bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint cbUncompressedDestBufferSize_Deprecated, uint nUncompressBytesWritten_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
        {
            return default;
        }

        public EVoiceResult DecompressVoice(IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, uint nBytesWritten, uint nDesiredSampleRate)
        {
            return default;
        }

        public uint GetVoiceOptimalSampleRate()
        {
            return 0;
        }

        public HAuthTicket GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, uint pcbTicket)
        {
            return default;
        }

        public EBeginAuthSessionResult BeginAuthSession(IntPtr pAuthTicket, int cbAuthTicket, IntPtr steamID)
        {
            return default;
        }

        public void EndAuthSession(IntPtr steamID)
        {
            //
        }

        public void CancelAuthTicket(HAuthTicket hAuthTicket)
        {
            //
        }

        public EUserHasLicenseForAppResult UserHasLicenseForApp(IntPtr steamID, AppId_t appID)
        {
            return default;
        }

        public bool BIsBehindNAT()
        {
            return false;
        }

        public void AdvertiseGame(IntPtr steamIDGameServer, uint unIPServer, uint usPortServer)
        {
            //
        }

        public SteamAPICall_t RequestEncryptedAppTicket(IntPtr pDataToInclude, int cbDataToInclude)
        {
            return default;
        }

        public bool GetEncryptedAppTicket(IntPtr pTicket, int cbMaxTicket, uint pcbTicket)
        {
            return false;
        }

        public int GetGameBadgeLevel(int nSeries, bool bFoil)
        {
            return 0;
        }

        public int GetPlayerSteamLevel()
        {
            return 0;
        }

        public SteamAPICall_t RequestStoreAuthURL(string pchRedirectURL)
        {
            return default;
        }

        public bool BIsPhoneVerified()
        {
            return false;
        }

        public bool BIsTwoFactorEnabled()
        {
            return false;
        }

        public bool BIsPhoneIdentifying()
        {
            return false;
        }

        public bool BIsPhoneRequiringVerification()
        {
            return false;
        }

        public SteamAPICall_t GetMarketEligibility()
        {
            return default;
        }

        public SteamAPICall_t GetDurationControl()
        {
            return default;
        }

        public bool BSetDurationControlOnlineState(EDurationControlOnlineState eNewState)
        {
            return false;
        }

    }

}
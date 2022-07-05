using System;
using SKYNET.Helper;
using SKYNET.Managers;

using SteamAPICall_t = System.UInt64;
using HSteamUser = System.UInt32;
using HAuthTicket = System.UInt32;
using System.Runtime.InteropServices;

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
            InterfaceVersion = "SteamUser020";
        }

        public HSteamUser GetHSteamUser()
        {
            Write("GetHSteamUser");
            return SteamEmulator.HSteamUser;
        }

        public bool BLoggedOn()
        {
            Write("BLoggedOn");
            return true;
        }

        public CSteamID GetSteamID()
        {
            var SteamId = SteamEmulator.SteamID;
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

        public void TrackAppUsageEvent(IntPtr gameID, int eAppUsageEvent, string pchExtraInfo)
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
            //IPCManager.SendStartVoiceRecording();
            Recording = true;
        }

        public void StopVoiceRecording()
        {
            Write("StopVoiceRecording");
            //IPCManager.SendStopVoiceRecording();
            Recording = false;
        }

        public int GetAvailableVoice(ref uint pcbCompressed, ref uint pcbUncompressed_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
        {
            Write("GetAvailableVoice");
            //if (IPCManager.SendGetAvailableVoice(out pcbCompressed, out pcbUncompressed_Deprecated))
            //{
            //    return (int)EVoiceResult.k_EVoiceResultOK;
            //}
            return (int)EVoiceResult.k_EVoiceResultNoData;
        }

        public int GetVoice(bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, ref uint nBytesWritten, bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint cbUncompressedDestBufferSize_Deprecated, uint nUncompressBytesWritten_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
        {
            Write("GetVoice");
            if (Recording) return (int)EVoiceResult.k_EVoiceResultNotRecording;
            nBytesWritten = 0;
            //if (IPCManager.SendGetVoice(out byte[] buffer))
            //{
            //    Marshal.Copy(buffer, 0, pDestBuffer, buffer.Length);
            //    nBytesWritten = (uint)buffer.Length;
            //    return (int)EVoiceResult.k_EVoiceResultOK;
            //}
            return (int)EVoiceResult.k_EVoiceResultNoData;
        }

        public int DecompressVoice(IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, uint nBytesWritten, uint nDesiredSampleRate)
        {
            Write("DecompressVoice");
            return (int)EVoiceResult.k_EVoiceResultNoData;
        }

        public uint GetVoiceOptimalSampleRate()
        {
            //Write("GetVoiceOptimalSampleRate");
            return 4800;
        }

        public HAuthTicket GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
        {
            Write("GetAuthSessionTicket");
            HAuthTicket Ticket = TicketManager.GetAuthSessionTicket(pTicket, cbMaxTicket, ref pcbTicket);
            return Ticket;
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
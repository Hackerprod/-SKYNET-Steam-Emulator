using System;
using SKYNET.Helpers;

using SteamAPICall_t = System.UInt64;
using HSteamUser = System.UInt32;
using uint16 = System.UInt16;
using uint32 = System.UInt32;
using AppId_t = System.UInt32;
using HAuthTicket = System.UInt32;
using CGameID = System.UInt64;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamUser019")]
    public class SteamUser019 : ISteamInterface
    {
        public HSteamUser GetHSteamUser(IntPtr _)
        {
            return SteamEmulator.SteamUser.GetHSteamUser();
        }

        public bool BLoggedOn(IntPtr _)
        {
            return SteamEmulator.SteamUser.BLoggedOn();
        }

        public IntPtr GetSteamID(IntPtr _, IntPtr pSteamID)
        {
            return NativeSteamId.Write(pSteamID, SteamEmulator.SteamUser.GetSteamID());
        }

        public int InitiateGameConnection(IntPtr _, IntPtr pAuthBlob, int cbMaxAuthBlob, ulong steamIDGameServer, uint32 unIPServer, uint16 usPortServer, bool bSecure)
        {
            return SteamEmulator.SteamUser.InitiateGameConnection(pAuthBlob, cbMaxAuthBlob, steamIDGameServer, unIPServer, usPortServer, bSecure);
        }

        public void TerminateGameConnection(IntPtr _, uint32 unIPServer, uint16 usPortServer)
        {
            SteamEmulator.SteamUser.TerminateGameConnection(unIPServer, usPortServer);
        }

        public void TrackAppUsageEvent(IntPtr _, CGameID gameID, int eAppUsageEvent, string pchExtraInfo = "")
        {
            SteamEmulator.SteamUser.TrackAppUsageEvent(gameID, eAppUsageEvent, pchExtraInfo);
        }

        public bool GetUserDataFolder(IntPtr _, IntPtr pchBuffer, int cubBuffer)
        {
            return SteamEmulator.SteamUser.GetUserDataFolder(pchBuffer, cubBuffer);
        }

        public void StartVoiceRecording(IntPtr _)
        {
            SteamEmulator.SteamUser.StartVoiceRecording();
        }

        public void StopVoiceRecording(IntPtr _)
        {
            SteamEmulator.SteamUser.StopVoiceRecording();
        }

        public EVoiceResult GetAvailableVoice(IntPtr _, IntPtr pcbCompressed, IntPtr pcbUncompressed_Deprecated, uint32 nUncompressedVoiceDesiredSampleRate_Deprecated = 0)
        {
            // Steamworks declares these as uint32*. The deprecated uncompressed pointer is optional.
            return SteamEmulator.SteamUser.GetAvailableVoice(pcbCompressed, pcbUncompressed_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
        }

        public EVoiceResult GetVoice(IntPtr _, bool bWantCompressed, IntPtr pDestBuffer, uint32 cbDestBufferSize, IntPtr nBytesWritten, bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint32 cbUncompressedDestBufferSize_Deprecated, IntPtr nUncompressBytesWritten_Deprecated, uint32 nUncompressedVoiceDesiredSampleRate_Deprecated = 0)
        {
            return SteamEmulator.SteamUser.GetVoice(bWantCompressed, pDestBuffer, cbDestBufferSize, nBytesWritten, bWantUncompressed_Deprecated, pUncompressedDestBuffer_Deprecated, cbUncompressedDestBufferSize_Deprecated, nUncompressBytesWritten_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
        }

        public EVoiceResult DecompressVoice(IntPtr _, IntPtr pCompressed, uint32 cbCompressed, IntPtr pDestBuffer, uint32 cbDestBufferSize, IntPtr nBytesWritten, uint32 nDesiredSampleRate)
        {
            return SteamEmulator.SteamUser.DecompressVoice(pCompressed, cbCompressed, pDestBuffer, cbDestBufferSize, nBytesWritten, nDesiredSampleRate);
        }

        public uint32 GetVoiceOptimalSampleRate(IntPtr _)
        {
            return SteamEmulator.SteamUser.GetVoiceOptimalSampleRate();
        }

        public HAuthTicket GetAuthSessionTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
        {
            return SteamEmulator.SteamUser.GetAuthSessionTicket(pTicket, cbMaxTicket, out pcbTicket);
        }

        public EBeginAuthSessionResult BeginAuthSession(IntPtr _, IntPtr pAuthTicket, int cbAuthTicket, ulong steamID)
        {
            return (EBeginAuthSessionResult)SteamEmulator.SteamUser.BeginAuthSession(pAuthTicket, cbAuthTicket, steamID);
        }

        public void EndAuthSession(IntPtr _, ulong steamID)
        {
            SteamEmulator.SteamUser.EndAuthSession(steamID);
        }

        public void CancelAuthTicket(IntPtr _, HAuthTicket hAuthTicket)
        {
            SteamEmulator.SteamUser.CancelAuthTicket(hAuthTicket);
        }

        public EUserHasLicenseForAppResult UserHasLicenseForApp(IntPtr _, ulong steamID, AppId_t appID)
        {
            return (EUserHasLicenseForAppResult)SteamEmulator.SteamUser.UserHasLicenseForApp(steamID, appID);
        }

        public bool BIsBehindNAT(IntPtr _)
        {
            return SteamEmulator.SteamUser.BIsBehindNAT();
        }

        public void AdvertiseGame(IntPtr _, ulong steamIDGameServer, uint32 unIPServer, uint16 usPortServer)
        {
            SteamEmulator.SteamUser.AdvertiseGame(steamIDGameServer, unIPServer, usPortServer);
        }

        public SteamAPICall_t RequestEncryptedAppTicket(IntPtr _, IntPtr pDataToInclude, int cbDataToInclude)
        {
            return SteamEmulator.SteamUser.RequestEncryptedAppTicket(pDataToInclude, cbDataToInclude);
        }

        public bool GetEncryptedAppTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, IntPtr pcbTicket)
        {
            return SteamEmulator.SteamUser.GetEncryptedAppTicket(pTicket, cbMaxTicket, pcbTicket);
        }

        public int GetGameBadgeLevel(IntPtr _, int nSeries, bool bFoil)
        {
            return SteamEmulator.SteamUser.GetGameBadgeLevel(nSeries, bFoil);
        }

        public int GetPlayerSteamLevel(IntPtr _)
        {
            return SteamEmulator.SteamUser.GetPlayerSteamLevel();
        }

        public SteamAPICall_t RequestStoreAuthURL(IntPtr _, string pchRedirectURL)
        {
            return SteamEmulator.SteamUser.RequestStoreAuthURL(pchRedirectURL);
        }

        public bool BIsPhoneVerified(IntPtr _)
        {
            return SteamEmulator.SteamUser.BIsPhoneVerified();
        }

        public bool BIsTwoFactorEnabled(IntPtr _)
        {
            return SteamEmulator.SteamUser.BIsTwoFactorEnabled();
        }

        public bool BIsPhoneIdentifying(IntPtr _)
        {
            return SteamEmulator.SteamUser.BIsPhoneIdentifying();
        }

        public bool BIsPhoneRequiringVerification(IntPtr _)
        {
            return SteamEmulator.SteamUser.BIsPhoneRequiringVerification();
        }
    }
}

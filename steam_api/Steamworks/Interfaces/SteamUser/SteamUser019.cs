using SKYNET.Types;
using Steamworks;
using System;

using SteamAPICall_t = System.UInt64;
using HSteamUser = System.UInt32;

namespace SKYNET.Interface
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

        public ulong GetSteamID(IntPtr _)
        {
            return (ulong)SteamEmulator.SteamUser.GetSteamID();
        }

        public int InitiateGameConnection(IntPtr _, IntPtr pAuthBlob, int cbMaxAuthBlob, ulong steamIDGameServer, uint unIPServer, uint usPortServer, bool bSecure)
        {
            return SteamEmulator.SteamUser.InitiateGameConnection(pAuthBlob, cbMaxAuthBlob, steamIDGameServer, unIPServer, usPortServer, bSecure);
        }

        public void TerminateGameConnection(IntPtr _, uint unIPServer, uint usPortServer)
        {
            SteamEmulator.SteamUser.TerminateGameConnection(unIPServer, usPortServer);
        }

        public void TrackAppUsageEvent(IntPtr _, IntPtr gameID, int eAppUsageEvent, string pchExtraInfo)
        {
            SteamEmulator.SteamUser.TrackAppUsageEvent(gameID, eAppUsageEvent, pchExtraInfo);
        }

        public bool GetUserDataFolder(IntPtr _, ref string pchBuffer, int cubBuffer)
        {
            return SteamEmulator.SteamUser.GetUserDataFolder(ref pchBuffer, cubBuffer);
        }

        public void StartVoiceRecording(IntPtr _)
        {
            SteamEmulator.SteamUser.StartVoiceRecording();
        }

        public void StopVoiceRecording(IntPtr _)
        {
            SteamEmulator.SteamUser.StopVoiceRecording();
        }

        public int GetAvailableVoice(IntPtr _, uint pcbCompressed, uint pcbUncompressed_Deprecated = 0, uint nUncompressedVoiceDesiredSampleRate_Deprecated = 0)
        {
            return SteamEmulator.SteamUser.GetAvailableVoice(pcbCompressed, pcbUncompressed_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
        }

        public int GetVoice(IntPtr _, bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, ref uint nBytesWritten, bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint cbUncompressedDestBufferSize_Deprecated, uint nUncompressBytesWritten_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
        {
            return SteamEmulator.SteamUser.GetVoice(bWantCompressed, pDestBuffer, cbDestBufferSize, ref nBytesWritten, bWantUncompressed_Deprecated, pUncompressedDestBuffer_Deprecated, cbUncompressedDestBufferSize_Deprecated, nUncompressBytesWritten_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
        }

        public int DecompressVoice(IntPtr _, IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, uint nBytesWritten, uint nDesiredSampleRate)
        {
            return SteamEmulator.SteamUser.DecompressVoice(pCompressed, cbCompressed, pDestBuffer, cbDestBufferSize, nBytesWritten, nDesiredSampleRate);
        }

        public int GetVoiceOptimalSampleRate(IntPtr _)
        {
            return SteamEmulator.SteamUser.GetVoiceOptimalSampleRate();
        }

        public uint GetAuthSessionTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
        {
            return SteamEmulator.SteamUser.GetAuthSessionTicket(pTicket, cbMaxTicket, ref pcbTicket);
        }

        public int BeginAuthSession(IntPtr _, IntPtr pAuthTicket, int cbAuthTicket, ulong steamID)
        {
            return SteamEmulator.SteamUser.BeginAuthSession(pAuthTicket, cbAuthTicket, steamID);
        }

        public void EndAuthSession(IntPtr _, ulong steamID)
        {
            SteamEmulator.SteamUser.EndAuthSession(steamID);
        }

        public void CancelAuthTicket(IntPtr _, uint hAuthTicket)
        {
            SteamEmulator.SteamUser.CancelAuthTicket(hAuthTicket);
        }

        public int UserHasLicenseForApp(IntPtr _, ulong steamID, uint appID)
        {
            return SteamEmulator.SteamUser.UserHasLicenseForApp(steamID, appID);
        }

        public bool BIsBehindNAT(IntPtr _)
        {
            return SteamEmulator.SteamUser.BIsBehindNAT();
        }

        public void AdvertiseGame(IntPtr _, ulong steamIDGameServer, uint unIPServer, uint usPortServer)
        {
            SteamEmulator.SteamUser.AdvertiseGame(steamIDGameServer, unIPServer, usPortServer);
        }

        public SteamAPICall_t RequestEncryptedAppTicket(IntPtr _, IntPtr pDataToInclude, int cbDataToInclude)
        {
            return SteamEmulator.SteamUser.RequestEncryptedAppTicket(pDataToInclude, cbDataToInclude);
        }

        public bool GetEncryptedAppTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, uint pcbTicket)
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

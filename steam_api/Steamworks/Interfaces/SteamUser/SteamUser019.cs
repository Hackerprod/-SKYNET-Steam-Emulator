using Steamworks;
using System;

namespace SKYNET.Interface
{
    [Interface("SteamUser019")]
    public class SteamUser019 : ISteamInterface
    {
        public int GetHSteamUser(IntPtr _)
        {
            return SteamEmulator.SteamUser.GetHSteamUser(_);
        }

        public bool BLoggedOn(IntPtr _)
        {
            return SteamEmulator.SteamUser.BLoggedOn(_);
        }

        public ulong GetSteamID(IntPtr _)
        {
            return (ulong)SteamEmulator.SteamUser.GetSteamID(_);
        }

        public int InitiateGameConnection(IntPtr _, IntPtr pAuthBlob, int cbMaxAuthBlob, ulong steamIDGameServer, uint unIPServer, uint usPortServer, bool bSecure)
        {
            return SteamEmulator.SteamUser.InitiateGameConnection(_, pAuthBlob, cbMaxAuthBlob, steamIDGameServer, unIPServer, usPortServer, bSecure);
        }

        public void TerminateGameConnection(IntPtr _, uint unIPServer, uint usPortServer)
        {
            SteamEmulator.SteamUser.TerminateGameConnection(_, unIPServer, usPortServer);
        }

        public void TrackAppUsageEvent(IntPtr _, IntPtr gameID, int eAppUsageEvent, string pchExtraInfo = "")
        {
            SteamEmulator.SteamUser.TrackAppUsageEvent(_, gameID, eAppUsageEvent);
        }

        public bool GetUserDataFolder(IntPtr _, string pchBuffer, int cubBuffer)
        {
            return SteamEmulator.SteamUser.GetUserDataFolder(_, pchBuffer, cubBuffer);
        }

        public void StartVoiceRecording(IntPtr _)
        {
            SteamEmulator.SteamUser.StartVoiceRecording(_);
        }

        public void StopVoiceRecording(IntPtr _)
        {
            SteamEmulator.SteamUser.StopVoiceRecording(_);
        }

        public int GetAvailableVoice(IntPtr _, uint pcbCompressed, uint pcbUncompressed_Deprecated = 0, uint nUncompressedVoiceDesiredSampleRate_Deprecated = 0)
        {
            return SteamEmulator.SteamUser.GetAvailableVoice(_, pcbCompressed, pcbUncompressed_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
        }

        public int GetVoice(IntPtr _, bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, uint nBytesWritten, bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint cbUncompressedDestBufferSize_Deprecated, uint nUncompressBytesWritten_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
        {
            return SteamEmulator.SteamUser.GetVoice(_, bWantCompressed, pDestBuffer, cbDestBufferSize, nBytesWritten, bWantUncompressed_Deprecated, pUncompressedDestBuffer_Deprecated, cbUncompressedDestBufferSize_Deprecated, nUncompressBytesWritten_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
        }

        public int DecompressVoice(IntPtr _, IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, uint nBytesWritten, uint nDesiredSampleRate)
        {
            return SteamEmulator.SteamUser.DecompressVoice(_, pCompressed, cbCompressed, pDestBuffer, cbDestBufferSize, nBytesWritten, nDesiredSampleRate);
        }

        public uint GetVoiceOptimalSampleRate(IntPtr _)
        {
            return SteamEmulator.SteamUser.GetVoiceOptimalSampleRate(_);
        }

        public uint GetAuthSessionTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, uint pcbTicket)
        {
            return SteamEmulator.SteamUser.GetAuthSessionTicket(_, pTicket, cbMaxTicket, pcbTicket);
        }

        public int BeginAuthSession(IntPtr _, IntPtr pAuthTicket, int cbAuthTicket, ulong steamID)
        {
            return SteamEmulator.SteamUser.BeginAuthSession(_, pAuthTicket, cbAuthTicket, steamID);
        }

        public void EndAuthSession(IntPtr _, ulong steamID)
        {
            SteamEmulator.SteamUser.EndAuthSession(_, steamID);
        }

        public void CancelAuthTicket(IntPtr _, uint hAuthTicket)
        {
            SteamEmulator.SteamUser.CancelAuthTicket(_, hAuthTicket);
        }

        public int UserHasLicenseForApp(IntPtr _, ulong steamID, uint appID)
        {
            return SteamEmulator.SteamUser.UserHasLicenseForApp(_, steamID, appID);
        }

        public bool BIsBehindNAT(IntPtr _)
        {
            return SteamEmulator.SteamUser.BIsBehindNAT(_);
        }

        public void AdvertiseGame(IntPtr _, ulong steamIDGameServer, uint unIPServer, uint usPortServer)
        {
            SteamEmulator.SteamUser.AdvertiseGame(_, steamIDGameServer, unIPServer, usPortServer);
        }

        public ulong RequestEncryptedAppTicket(IntPtr _, IntPtr pDataToInclude, int cbDataToInclude)
        {
            return SteamEmulator.SteamUser.RequestEncryptedAppTicket(_, pDataToInclude, cbDataToInclude);
        }

        public bool GetEncryptedAppTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, uint pcbTicket)
        {
            return SteamEmulator.SteamUser.GetEncryptedAppTicket(_, pTicket, cbMaxTicket, pcbTicket);
        }

        public int GetGameBadgeLevel(IntPtr _, int nSeries, bool bFoil)
        {
            return SteamEmulator.SteamUser.GetGameBadgeLevel(_, nSeries, bFoil);
        }

        public int GetPlayerSteamLevel(IntPtr _)
        {
            return SteamEmulator.SteamUser.GetPlayerSteamLevel(_);
        }

        public ulong RequestStoreAuthURL(IntPtr _, string pchRedirectURL)
        {
            return SteamEmulator.SteamUser.RequestStoreAuthURL(_, pchRedirectURL);
        }

        public bool BIsPhoneVerified(IntPtr _)
        {
            return SteamEmulator.SteamUser.BIsPhoneVerified(_);
        }

        public bool BIsTwoFactorEnabled(IntPtr _)
        {
            return SteamEmulator.SteamUser.BIsTwoFactorEnabled(_);
        }

        public bool BIsPhoneIdentifying(IntPtr _)
        {
            return SteamEmulator.SteamUser.BIsPhoneIdentifying(_);
        }

        public bool BIsPhoneRequiringVerification(IntPtr _)
        {
            return SteamEmulator.SteamUser.BIsPhoneRequiringVerification(_);
        }


    }
}

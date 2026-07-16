using System;
using SKYNET.Helpers;

using SteamAPICall_t = System.UInt64;
using HSteamUser = System.UInt32;
using CGameID = System.UInt64;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamUser020")]
    public class SteamUser020 : ISteamInterface
    {
        public HSteamUser GetHSteamUser(IntPtr _) => SteamEmulator.SteamUser.GetHSteamUser();
        public bool BLoggedOn(IntPtr _) => SteamEmulator.SteamUser.BLoggedOn();
        public IntPtr GetSteamID(IntPtr _, IntPtr pSteamID) => NativeSteamId.Write(pSteamID, SteamEmulator.SteamUser.GetSteamID());
        public int InitiateGameConnection(IntPtr _, IntPtr pAuthBlob, int cbMaxAuthBlob, ulong steamIDGameServer, uint unIPServer, ushort usPortServer, bool bSecure) => SteamEmulator.SteamUser.InitiateGameConnection(pAuthBlob, cbMaxAuthBlob, steamIDGameServer, unIPServer, usPortServer, bSecure);
        public void TerminateGameConnection(IntPtr _, uint unIPServer, ushort usPortServer) => SteamEmulator.SteamUser.TerminateGameConnection(unIPServer, usPortServer);
        public void TrackAppUsageEvent(IntPtr _, CGameID gameID, int eAppUsageEvent, string pchExtraInfo) => SteamEmulator.SteamUser.TrackAppUsageEvent(gameID, eAppUsageEvent, pchExtraInfo);
        public bool GetUserDataFolder(IntPtr _, IntPtr pchBuffer, int cubBuffer) => SteamEmulator.SteamUser.GetUserDataFolder(pchBuffer, cubBuffer);
        public void StartVoiceRecording(IntPtr _) => SteamEmulator.SteamUser.StartVoiceRecording();
        public void StopVoiceRecording(IntPtr _) => SteamEmulator.SteamUser.StopVoiceRecording();
        public EVoiceResult GetAvailableVoice(IntPtr _, IntPtr pcbCompressed, IntPtr pcbUncompressed_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated) => SteamEmulator.SteamUser.GetAvailableVoice(pcbCompressed, pcbUncompressed_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
        public EVoiceResult GetVoice(IntPtr _, bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, IntPtr nBytesWritten, bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint cbUncompressedDestBufferSize_Deprecated, IntPtr nUncompressBytesWritten_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated) => SteamEmulator.SteamUser.GetVoice(bWantCompressed, pDestBuffer, cbDestBufferSize, nBytesWritten, bWantUncompressed_Deprecated, pUncompressedDestBuffer_Deprecated, cbUncompressedDestBufferSize_Deprecated, nUncompressBytesWritten_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
        public EVoiceResult DecompressVoice(IntPtr _, IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, IntPtr nBytesWritten, uint nDesiredSampleRate) => SteamEmulator.SteamUser.DecompressVoice(pCompressed, cbCompressed, pDestBuffer, cbDestBufferSize, nBytesWritten, nDesiredSampleRate);
        public uint GetVoiceOptimalSampleRate(IntPtr _) => SteamEmulator.SteamUser.GetVoiceOptimalSampleRate();
        public uint GetAuthSessionTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket) => SteamEmulator.SteamUser.GetAuthSessionTicket(pTicket, cbMaxTicket, out pcbTicket);
        public int BeginAuthSession(IntPtr _, IntPtr pAuthTicket, int cbAuthTicket, ulong steamID) => SteamEmulator.SteamUser.BeginAuthSession(pAuthTicket, cbAuthTicket, steamID);
        public void EndAuthSession(IntPtr _, ulong steamID) => SteamEmulator.SteamUser.EndAuthSession(steamID);
        public void CancelAuthTicket(IntPtr _, uint hAuthTicket) => SteamEmulator.SteamUser.CancelAuthTicket(hAuthTicket);
        public int UserHasLicenseForApp(IntPtr _, ulong steamID, uint appID) => SteamEmulator.SteamUser.UserHasLicenseForApp(steamID, appID);
        public bool BIsBehindNAT(IntPtr _) => SteamEmulator.SteamUser.BIsBehindNAT();
        public void AdvertiseGame(IntPtr _, ulong steamIDGameServer, uint unIPServer, ushort usPortServer) => SteamEmulator.SteamUser.AdvertiseGame(steamIDGameServer, unIPServer, usPortServer);
        public SteamAPICall_t RequestEncryptedAppTicket(IntPtr _, IntPtr pDataToInclude, int cbDataToInclude) => SteamEmulator.SteamUser.RequestEncryptedAppTicket(pDataToInclude, cbDataToInclude);
        public bool GetEncryptedAppTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, IntPtr pcbTicket) => SteamEmulator.SteamUser.GetEncryptedAppTicket(pTicket, cbMaxTicket, pcbTicket);
        public int GetGameBadgeLevel(IntPtr _, int nSeries, bool bFoil) => SteamEmulator.SteamUser.GetGameBadgeLevel(nSeries, bFoil);
        public int GetPlayerSteamLevel(IntPtr _) => SteamEmulator.SteamUser.GetPlayerSteamLevel();
        public SteamAPICall_t RequestStoreAuthURL(IntPtr _, string pchRedirectURL) => SteamEmulator.SteamUser.RequestStoreAuthURL(pchRedirectURL);
        public bool BIsPhoneVerified(IntPtr _) => SteamEmulator.SteamUser.BIsPhoneVerified();
        public bool BIsTwoFactorEnabled(IntPtr _) => SteamEmulator.SteamUser.BIsTwoFactorEnabled();
        public bool BIsPhoneIdentifying(IntPtr _) => SteamEmulator.SteamUser.BIsPhoneIdentifying();
        public bool BIsPhoneRequiringVerification(IntPtr _) => SteamEmulator.SteamUser.BIsPhoneRequiringVerification();
        public SteamAPICall_t GetMarketEligibility(IntPtr _) => SteamEmulator.SteamUser.GetMarketEligibility();
        public SteamAPICall_t GetDurationControl(IntPtr _) => SteamEmulator.SteamUser.GetDurationControl();
    }
}

using System;
using SKYNET.Helpers;

using CGameID = System.UInt64;
using HSteamUser = System.UInt32;

namespace SKYNET.Steamworks.Interfaces
{
    /// <summary>
    /// Legacy ISteamUser ABI used by the original Left 4 Dead Steamworks SDK.
    /// SteamUser013 predates the configurable voice sample-rate arguments and
    /// contains exactly 18 vtable slots, ending at UserHasLicenseForApp.
    /// </summary>
    [Interface("SteamUser013")]
    public class SteamUser013 : ISteamInterface
    {
        private const uint LegacyVoiceSampleRate = 11025;

        public HSteamUser GetHSteamUser(IntPtr _) => SteamEmulator.SteamUser.GetHSteamUser();
        public bool BLoggedOn(IntPtr _) => SteamEmulator.SteamUser.BLoggedOn();
        public IntPtr GetSteamID(IntPtr _, IntPtr pSteamID) => NativeSteamId.Write(pSteamID, SteamEmulator.SteamUser.GetSteamID());

        public int InitiateGameConnection(
            IntPtr _,
            IntPtr pAuthBlob,
            int cbMaxAuthBlob,
            ulong steamIDGameServer,
            uint unIPServer,
            ushort usPortServer,
            bool bSecure) =>
            SteamEmulator.SteamUser.InitiateGameConnection(
                pAuthBlob,
                cbMaxAuthBlob,
                steamIDGameServer,
                unIPServer,
                usPortServer,
                bSecure);

        public void TerminateGameConnection(IntPtr _, uint unIPServer, ushort usPortServer) =>
            SteamEmulator.SteamUser.TerminateGameConnection(unIPServer, usPortServer);

        public void TrackAppUsageEvent(IntPtr _, CGameID gameID, int eAppUsageEvent, string pchExtraInfo) =>
            SteamEmulator.SteamUser.TrackAppUsageEvent(gameID, eAppUsageEvent, pchExtraInfo);

        public bool GetUserDataFolder(IntPtr _, IntPtr pchBuffer, int cubBuffer) =>
            SteamEmulator.SteamUser.GetUserDataFolder(pchBuffer, cubBuffer);

        public void StartVoiceRecording(IntPtr _) => SteamEmulator.SteamUser.StartVoiceRecording();
        public void StopVoiceRecording(IntPtr _) => SteamEmulator.SteamUser.StopVoiceRecording();

        public EVoiceResult GetAvailableVoice(IntPtr _, IntPtr pcbCompressed, IntPtr pcbUncompressed) =>
            SteamEmulator.SteamUser.GetAvailableVoice(
                pcbCompressed,
                pcbUncompressed,
                LegacyVoiceSampleRate);

        public EVoiceResult GetVoice(
            IntPtr _,
            bool bWantCompressed,
            IntPtr pDestBuffer,
            uint cbDestBufferSize,
            IntPtr nBytesWritten,
            bool bWantUncompressed,
            IntPtr pUncompressedDestBuffer,
            uint cbUncompressedDestBufferSize,
            IntPtr nUncompressBytesWritten) =>
            SteamEmulator.SteamUser.GetVoice(
                bWantCompressed,
                pDestBuffer,
                cbDestBufferSize,
                nBytesWritten,
                bWantUncompressed,
                pUncompressedDestBuffer,
                cbUncompressedDestBufferSize,
                nUncompressBytesWritten,
                LegacyVoiceSampleRate);

        public EVoiceResult DecompressVoice(
            IntPtr _,
            IntPtr pCompressed,
            uint cbCompressed,
            IntPtr pDestBuffer,
            uint cbDestBufferSize,
            IntPtr nBytesWritten) =>
            SteamEmulator.SteamUser.DecompressVoice(
                pCompressed,
                cbCompressed,
                pDestBuffer,
                cbDestBufferSize,
                nBytesWritten,
                LegacyVoiceSampleRate);

        public uint GetAuthSessionTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket) =>
            SteamEmulator.SteamUser.GetAuthSessionTicket(pTicket, cbMaxTicket, out pcbTicket);

        public int BeginAuthSession(IntPtr _, IntPtr pAuthTicket, int cbAuthTicket, ulong steamID) =>
            SteamEmulator.SteamUser.BeginAuthSession(pAuthTicket, cbAuthTicket, steamID);

        public void EndAuthSession(IntPtr _, ulong steamID) => SteamEmulator.SteamUser.EndAuthSession(steamID);
        public void CancelAuthTicket(IntPtr _, uint hAuthTicket) => SteamEmulator.SteamUser.CancelAuthTicket(hAuthTicket);

        public int UserHasLicenseForApp(IntPtr _, ulong steamID, uint appID) =>
            SteamEmulator.SteamUser.UserHasLicenseForApp(steamID, appID);
    }
}

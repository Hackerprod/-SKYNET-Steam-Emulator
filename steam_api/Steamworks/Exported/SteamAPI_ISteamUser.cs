using System;
using System.Runtime.InteropServices;
using SKYNET.Managers;

namespace SKYNET.Steamworks.Exported
{
    using SteamAPICall_t = System.UInt64;
    using HSteamUser = System.UInt32;
    public class SteamAPI_ISteamUser
    {
        static SteamAPI_ISteamUser()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HSteamUser SteamAPI_ISteamUser_GetHSteamUser(IntPtr _)
        {
            Write("SteamAPI_ISteamUser_Get");
            return SteamEmulator.SteamUser.GetHSteamUser();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUser_BLoggedOn(IntPtr _)
        {
            Write("SteamAPI_ISteamUser_BLoggedOn");
            return SteamEmulator.SteamUser.BLoggedOn();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamUser_GetSteamID(IntPtr _)
        {
            Write("SteamAPI_ISteamUser_GetSteamID");
            return SteamEmulator.SteamUser.GetSteamID().SteamID;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUser_InitiateGameConnection(IntPtr _, IntPtr pAuthBlob, int cbMaxAuthBlob, ulong steamIDGameServer, uint unIPServer, uint usPortServer, bool bSecure)
        {
            Write("SteamAPI_ISteamUser_InitiateGameConnection");
            return SteamEmulator.SteamUser.InitiateGameConnection(pAuthBlob, cbMaxAuthBlob, steamIDGameServer, unIPServer, usPortServer, bSecure);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUser_TerminateGameConnection(IntPtr _, uint unIPServer, uint usPortServer)
        {
            Write("SteamAPI_ISteamUser_TerminateGameConnection");
            SteamEmulator.SteamUser.TerminateGameConnection(unIPServer, usPortServer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUser_TrackAppUsageEvent(IntPtr _, IntPtr gameID, int eAppUsageEvent, string pchExtraInfo)
        {
            Write("SteamAPI_ISteamUser_TrackAppUsageEvent");
            SteamEmulator.SteamUser.TrackAppUsageEvent(gameID, eAppUsageEvent, pchExtraInfo);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUser_GetUserDataFolder(IntPtr _, ref string pchBuffer, int cubBuffer)
        {
            Write("SteamAPI_ISteamUser_GetUserDataFolder");
            return SteamEmulator.SteamUser.GetUserDataFolder(ref pchBuffer, cubBuffer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUser_StartVoiceRecording(IntPtr _)
        {
            Write("SteamAPI_ISteamUser_StartVoiceRecording");
            SteamEmulator.SteamUser.StartVoiceRecording();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUser_StopVoiceRecording(IntPtr _)
        {
            //Write("SteamAPI_ISteamUser_StopVoiceRecording");
            SteamEmulator.SteamUser.StopVoiceRecording();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUser_GetAvailableVoice(IntPtr _, ref uint pcbCompressed, ref uint pcbUncompressed_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
        {
            //Write("SteamAPI_ISteamUser_GetAvailableVoice");
            return SteamEmulator.SteamUser.GetAvailableVoice(ref pcbCompressed, ref pcbUncompressed_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUser_GetVoice(IntPtr _, bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, ref uint nBytesWritten, bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint cbUncompressedDestBufferSize_Deprecated, ref uint nUncompressBytesWritten_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
        {
            Write("SteamAPI_ISteamUser_GetVoice");
            return SteamEmulator.SteamUser.GetVoice(bWantCompressed, pDestBuffer, cbDestBufferSize, ref nBytesWritten, bWantUncompressed_Deprecated, pUncompressedDestBuffer_Deprecated, cbUncompressedDestBufferSize_Deprecated, ref nUncompressBytesWritten_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUser_DecompressVoice(IntPtr _, IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, ref uint nBytesWritten, uint nDesiredSampleRate)
        {
            Write("SteamAPI_ISteamUser_DecompressVoice");
            return SteamEmulator.SteamUser.DecompressVoice(pCompressed, cbCompressed, pDestBuffer, cbDestBufferSize, ref nBytesWritten, nDesiredSampleRate);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamUser_GetVoiceOptimalSampleRate(IntPtr _)
        {
            //Write("SteamAPI_ISteamUser_GetVoiceOptimalSampleRate");
            return SteamEmulator.SteamUser.GetVoiceOptimalSampleRate();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamUser_GetAuthSessionTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
        {
            Write("SteamAPI_ISteamUser_GetAuthSessionTicket");
            return SteamEmulator.SteamUser.GetAuthSessionTicket(pTicket, cbMaxTicket, ref pcbTicket);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUser_BeginAuthSession(IntPtr _, IntPtr pAuthTicket, int cbAuthTicket, ulong steamID)
        {
            Write("SteamAPI_ISteamUser_BeginAuthSession");
            return SteamEmulator.SteamUser.BeginAuthSession(pAuthTicket, cbAuthTicket, steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUser_EndAuthSession(IntPtr _, ulong steamID)
        {
            Write("SteamAPI_ISteamUser_EndAuthSession");
            SteamEmulator.SteamUser.EndAuthSession(steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUser_CancelAuthTicket(IntPtr _, uint hAuthTicket)
        {
            Write("SteamAPI_ISteamUser_CancelAuthTicket");
            SteamEmulator.SteamUser.CancelAuthTicket(hAuthTicket);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUser_UserHasLicenseForApp(IntPtr _, ulong steamID, uint appID)
        {
            Write("SteamAPI_ISteamUser_UserHasLicenseForApp");
            return SteamEmulator.SteamUser.UserHasLicenseForApp(steamID, appID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUser_BIsBehindNAT(IntPtr _)
        {
            Write("SteamAPI_ISteamUser_BIsBehindNAT");
            return SteamEmulator.SteamUser.BIsBehindNAT();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUser_AdvertiseGame(IntPtr _, ulong steamIDGameServer, uint unIPServer, uint usPortServer)
        {
            Write("SteamAPI_ISteamUser_AdvertiseGame");
            SteamEmulator.SteamUser.AdvertiseGame(steamIDGameServer, unIPServer, usPortServer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUser_RequestEncryptedAppTicket(IntPtr _, IntPtr pDataToInclude, int cbDataToInclude)
        {
            Write("SteamAPI_ISteamUser_RequestEncryptedAppTicket");
            return SteamEmulator.SteamUser.RequestEncryptedAppTicket(pDataToInclude, cbDataToInclude);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUser_GetEncryptedAppTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, uint pcbTicket)
        {
            Write("SteamAPI_ISteamUser_GetEncryptedAppTicket");
            return SteamEmulator.SteamUser.GetEncryptedAppTicket(pTicket, cbMaxTicket, pcbTicket);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUser_GetGameBadgeLevel(IntPtr _, int nSeries, bool bFoil)
        {
            Write("SteamAPI_ISteamUser_GetGameBadgeLevel");
            return SteamEmulator.SteamUser.GetGameBadgeLevel(nSeries, bFoil);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUser_GetPlayerSteamLevel(IntPtr _)
        {
            Write("SteamAPI_ISteamUser_GetPlayerSteamLevel");
            return SteamEmulator.SteamUser.GetPlayerSteamLevel();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUser_RequestStoreAuthURL(IntPtr _, string pchRedirectURL)
        {
            Write("SteamAPI_ISteamUser_RequestStoreAuthURL");
            return SteamEmulator.SteamUser.RequestStoreAuthURL(pchRedirectURL);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUser_BIsPhoneVerified(IntPtr _)
        {
            Write("SteamAPI_ISteamUser_BIsPhoneVerified");
            return SteamEmulator.SteamUser.BIsPhoneVerified();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUser_BIsTwoFactorEnabled(IntPtr _)
        {
            Write("SteamAPI_ISteamUser_BIsTwoFactorEnabled");
            return SteamEmulator.SteamUser.BIsTwoFactorEnabled();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUser_BIsPhoneIdentifying(IntPtr _)
        {
            Write("SteamAPI_ISteamUser_BIsPhoneIdentifying");
            return SteamEmulator.SteamUser.BIsPhoneIdentifying();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUser_BIsPhoneRequiringVerification(IntPtr _)
        {
            Write("SteamAPI_ISteamUser_BIsPhoneRequiringVerification");
            return SteamEmulator.SteamUser.BIsPhoneRequiringVerification();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUser_GetMarketEligibility(IntPtr _)
        {
            Write("SteamAPI_ISteamUser_GetMarketEligibility");
            return SteamEmulator.SteamUser.GetMarketEligibility();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUser_GetDurationControl(IntPtr _)
        {
            Write("SteamAPI_ISteamUser_GetDurationControl");
            return SteamEmulator.SteamUser.GetDurationControl();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUser_BSetDurationControlOnlineState(IntPtr _, int eNewState)
        {
            Write("SteamAPI_ISteamUser_BSetDurationControlOnlineState");
            return SteamEmulator.SteamUser.BSetDurationControlOnlineState(eNewState);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamUser_v020()
        {
            Write("SteamAPI_SteamUser_v020");
            return InterfaceManager.FindOrCreateInterface("SteamUser020");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamUser_v021()
        {
            Write("SteamAPI_SteamUser_v020");
            return InterfaceManager.FindOrCreateInterface("SteamUser021");
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}

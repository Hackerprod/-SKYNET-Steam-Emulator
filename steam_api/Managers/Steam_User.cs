using System;
using System.Runtime.InteropServices;
using SKYNET.Interface;
using Steamworks;

namespace SKYNET.Managers
{
    public class Steam_User : SteamInterface, ISteamUser
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ISteamUser SteamAPI_SteamUser_v021()
        {
            Write($"{"SteamAPI_SteamUser_v021"}");
            return SteamClient.GetISteamUser(SteamClient.LocalUser, SteamClient.LocalPipe, "SteamUser021");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ISteamUser SteamAPI_SteamUser_v020()
        {
            Write($"{"SteamAPI_SteamUser_v020"}");
            return SteamClient.GetISteamUser(SteamClient.LocalUser, SteamClient.LocalPipe, "SteamUser020");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HSteamUser SteamAPI_ISteamUser_GetHSteamUser(ISteamUser self)
        {
            Write($"{"SteamAPI_ISteamUser_GetHSteamUser"}");
            return SteamClient.LocalUser;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUser_BLoggedOn(ISteamUser self)
        {
            Write($"{"SteamAPI_ISteamUser_BLoggedOn"}");
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamUser_GetSteamID(ISteamUser self)
        {
            Write($"{"SteamAPI_ISteamUser_GetSteamID"}");
            return SteamClient.SteamId;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUser_InitiateGameConnection(ISteamUser self, IntPtr pAuthBlob, int cbMaxAuthBlob, ulong steamIDGameServer, uint unIPServer, int usPortServer, bool bSecure)
        {
            Write($"{"SteamAPI_ISteamUser_InitiateGameConnection"}");
            return 1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUser_TerminateGameConnection(ISteamUser self, uint unIPServer, int usPortServer)
        {
            Write($"{"SteamAPI_ISteamUser_TerminateGameConnection"}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUser_TrackAppUsageEvent(ISteamUser self, ulong gameID, int eAppUsageEvent, IntPtr pchExtraInfo)
        {
            Write($"{"SteamAPI_ISteamUser_TrackAppUsageEvent"}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUser_GetUserDataFolder(ISteamUser self, IntPtr pchBuffer, int cubBuffer)
        {
            Write($"{"SteamAPI_ISteamUser_GetUserDataFolder"}");
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUser_StartVoiceRecording(ISteamUser self)
        {
            Write($"{"SteamAPI_ISteamUser_StartVoiceRecording"}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUser_StopVoiceRecording(ISteamUser self)
        {
            Write($"{"SteamAPI_ISteamUser_StopVoiceRecording"}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static VoiceResult SteamAPI_ISteamUser_GetAvailableVoice(ISteamUser self, uint pcbCompressed, uint pcbUncompressed_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
        {
            Write($"{"SteamAPI_ISteamUser_GetAvailableVoice"}");
            return VoiceResult.NotInitialized;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static VoiceResult SteamAPI_ISteamUser_GetVoice(ISteamUser self, bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, uint nBytesWritten, bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint cbUncompressedDestBufferSize_Deprecated, uint nUncompressBytesWritten_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
        {
            Write($"{"SteamAPI_ISteamUser_GetVoice"}");
            return VoiceResult.NotInitialized;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static VoiceResult SteamAPI_ISteamUser_DecompressVoice(ISteamUser self, IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, uint nBytesWritten, uint nDesiredSampleRate)
        {
            Write($"{"SteamAPI_ISteamUser_DecompressVoice"}");
            return VoiceResult.NotInitialized;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamUser_GetVoiceOptimalSampleRate(ISteamUser self)
        {
            Write($"{"SteamAPI_ISteamUser_GetVoiceOptimalSampleRate"}");
            return 10;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HAuthTicket SteamAPI_ISteamUser_GetAuthSessionTicket(ISteamUser self, IntPtr pTicket, int cbMaxTicket, uint pcbTicket)
        {
            Write($"{"SteamAPI_ISteamUser_GetAuthSessionTicket"}");
            return (HAuthTicket)1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static BeginAuthResult SteamAPI_ISteamUser_BeginAuthSession(ISteamUser self, IntPtr pAuthTicket, int cbAuthTicket, ulong steamID)
        {
            Write($"{"SteamAPI_ISteamUser_BeginAuthSession"}");
            return BeginAuthResult.OK;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUser_EndAuthSession(ISteamUser self, ulong steamID)
        {
            Write($"{"SteamAPI_ISteamUser_EndAuthSession"}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUser_CancelAuthTicket(ISteamUser self, HAuthTicket hAuthTicket)
        {
            Write($"{"SteamAPI_ISteamUser_CancelAuthTicket"}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static UserHasLicenseForAppResult SteamAPI_ISteamUser_UserHasLicenseForApp(ISteamUser self, ulong steamID, AppId_t appID)
        {
            Write($"{"SteamAPI_ISteamUser_UserHasLicenseForApp"}");
            return UserHasLicenseForAppResult.HasLicense;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUser_BIsBehindNAT(ISteamUser self)
        {
            Write($"{"SteamAPI_ISteamUser_BIsBehindNAT"}");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUser_AdvertiseGame(ISteamUser self, ulong steamIDGameServer, uint unIPServer, int usPortServer)
        {
            Write($"{"SteamAPI_ISteamUser_AdvertiseGame"}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUser_RequestEncryptedAppTicket(ISteamUser self, IntPtr pDataToInclude, int cbDataToInclude)
        {
            Write($"{"SteamAPI_ISteamUser_RequestEncryptedAppTicket"}");
            return (SteamAPICall_t)1; 
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUser_GetEncryptedAppTicket(ISteamUser self, IntPtr pTicket, int cbMaxTicket, uint pcbTicket)
        {
            Write($"{"SteamAPI_ISteamUser_GetEncryptedAppTicket"}");
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUser_GetGameBadgeLevel(ISteamUser self, int nSeries, bool bFoil)
        {
            Write($"{"SteamAPI_ISteamUser_GetGameBadgeLevel"}");
            return 90;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUser_GetPlayerSteamLevel(ISteamUser self)
        {
            Write($"{"SteamAPI_ISteamUser_GetPlayerSteamLevel"}");
            return 100;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUser_RequestStoreAuthURL(ISteamUser self, IntPtr pchRedirectURL)
        {
            Write($"{"SteamAPI_ISteamUser_RequestStoreAuthURL"}");
            return (SteamAPICall_t)1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUser_BIsPhoneVerified(ISteamUser self)
        {
            Write($"{"SteamAPI_ISteamUser_BIsPhoneVerified"}");
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUser_BIsTwoFactorEnabled(ISteamUser self)
        {
            Write($"{"SteamAPI_ISteamUser_BIsTwoFactorEnabled"}");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUser_BIsPhoneIdentifying(ISteamUser self)
        {
            Write($"{"SteamAPI_ISteamUser_BIsPhoneIdentifying"}");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUser_BIsPhoneRequiringVerification(ISteamUser self)
        {
            Write($"{"SteamAPI_ISteamUser_BIsPhoneRequiringVerification"}");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUser_GetMarketEligibility(ISteamUser self)
        {
            Write($"{"SteamAPI_ISteamUser_GetMarketEligibility"}");
            return (SteamAPICall_t)1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUser_GetDurationControl(ISteamUser self)
        {
            Write($"{"SteamAPI_ISteamUser_GetDurationControl"}");
            return (SteamAPICall_t)1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUser_BSetDurationControlOnlineState(ISteamUser self, DurationControlOnlineState eNewState)
        {
            Write($"{"SteamAPI_ISteamUser_BSetDurationControlOnlineState"}");
            return true;
        }

    }
}
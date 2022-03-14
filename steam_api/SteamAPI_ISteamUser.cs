using System;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SKYNET.Helper;
using SKYNET.Interface;
using Steamworks;

public class SteamAPI_ISteamUser : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static HSteamUser SteamAPI_ISteamUser_GetHSteamUser()
    {
        Write("SteamAPI_ISteamUser_Get");
        return SteamClient.SteamUser.GetHSteamUser();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUser_BLoggedOn()
    {
        Write("SteamAPI_ISteamUser_BLoggedOn");
        return SteamClient.SteamUser.BLoggedOn();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamUser_GetSteamID()
    {
        Write("SteamAPI_ISteamUser_GetSteamID");
        return SteamClient.SteamUser.GetSteamID();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamUser_InitiateGameConnection(IntPtr pAuthBlob, int cbMaxAuthBlob, IntPtr steamIDGameServer, uint unIPServer, uint usPortServer, bool bSecure)
    {
        Write("SteamAPI_ISteamUser_InitiateGameConnection");
        return SteamClient.SteamUser.InitiateGameConnection(pAuthBlob, cbMaxAuthBlob, steamIDGameServer, unIPServer, usPortServer, bSecure);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamUser_TerminateGameConnection(uint unIPServer, uint usPortServer)
    {
        Write("SteamAPI_ISteamUser_TerminateGameConnection");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamUser_TrackAppUsageEvent(IntPtr gameID, int eAppUsageEvent, string pchExtraInfo = "")
    {
        Write("SteamAPI_ISteamUser_TrackAppUsageEvent");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUser_GetUserDataFolder(string pchBuffer, int cubBuffer)
    {
        Write("SteamAPI_ISteamUser_GetUserDataFolder");
        return SteamClient.SteamUser.GetUserDataFolder(pchBuffer, cubBuffer);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamUser_StartVoiceRecording()
    {
        Write("SteamAPI_ISteamUser_StartVoiceRecording");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamUser_StopVoiceRecording()
    {
        Write("SteamAPI_ISteamUser_StopVoiceRecording");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static EVoiceResult SteamAPI_ISteamUser_GetAvailableVoice(uint pcbCompressed, uint pcbUncompressed_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
    {
        Write("SteamAPI_ISteamUser_GetAvailableVoice");
        return SteamClient.SteamUser.GetAvailableVoice(pcbCompressed, pcbUncompressed_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static EVoiceResult SteamAPI_ISteamUser_GetVoice(bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, uint nBytesWritten, bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint cbUncompressedDestBufferSize_Deprecated, uint nUncompressBytesWritten_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
    {
        Write("SteamAPI_ISteamUser_GetVoice");
        return SteamClient.SteamUser.GetVoice(bWantCompressed, pDestBuffer, cbDestBufferSize, nBytesWritten, bWantUncompressed_Deprecated, pUncompressedDestBuffer_Deprecated, cbUncompressedDestBufferSize_Deprecated, nUncompressBytesWritten_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static EVoiceResult SteamAPI_ISteamUser_DecompressVoice(IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, uint nBytesWritten, uint nDesiredSampleRate)
    {
        Write("SteamAPI_ISteamUser_DecompressVoice");
        return SteamClient.SteamUser.DecompressVoice(pCompressed, cbCompressed, pDestBuffer, cbDestBufferSize, nBytesWritten, nDesiredSampleRate);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamUser_GetVoiceOptimalSampleRate()
    {
        Write("SteamAPI_ISteamUser_GetVoiceOptimalSampleRate");
        return SteamClient.SteamUser.GetVoiceOptimalSampleRate();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static HAuthTicket SteamAPI_ISteamUser_GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, uint pcbTicket)
    {
        Write("SteamAPI_ISteamUser_GetAuthSessionTicket");
        return SteamClient.SteamUser.GetAuthSessionTicket(pTicket, cbMaxTicket, pcbTicket);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static EBeginAuthSessionResult SteamAPI_ISteamUser_BeginAuthSession(IntPtr pAuthTicket, int cbAuthTicket, IntPtr steamID)
    {
        Write("SteamAPI_ISteamUser_BeginAuthSession");
        return SteamClient.SteamUser.BeginAuthSession(pAuthTicket, cbAuthTicket, steamID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamUser_EndAuthSession(IntPtr steamID)
    {
        Write("SteamAPI_ISteamUser_EndAuthSession");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamUser_CancelAuthTicket(HAuthTicket hAuthTicket)
    {
        Write("SteamAPI_ISteamUser_CancelAuthTicket");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static EUserHasLicenseForAppResult SteamAPI_ISteamUser_UserHasLicenseForApp(IntPtr steamID, AppId_t appID)
    {
        Write("SteamAPI_ISteamUser_UserHasLicenseForApp");
        return SteamClient.SteamUser.UserHasLicenseForApp(steamID, appID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUser_BIsBehindNAT()
    {
        Write("SteamAPI_ISteamUser_BIsBehindNAT");
        return SteamClient.SteamUser.BIsBehindNAT();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamUser_AdvertiseGame(IntPtr steamIDGameServer, uint unIPServer, uint usPortServer)
    {
        Write("SteamAPI_ISteamUser_AdvertiseGame");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamUser_RequestEncryptedAppTicket(IntPtr pDataToInclude, int cbDataToInclude)
    {
        Write("SteamAPI_ISteamUser_RequestEncryptedAppTicket");
        return SteamClient.SteamUser.RequestEncryptedAppTicket(pDataToInclude, cbDataToInclude);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUser_GetEncryptedAppTicket(IntPtr pTicket, int cbMaxTicket, uint pcbTicket)
    {
        Write("SteamAPI_ISteamUser_GetEncryptedAppTicket");
        return SteamClient.SteamUser.GetEncryptedAppTicket(pTicket, cbMaxTicket, pcbTicket);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamUser_GetGameBadgeLevel(int nSeries, bool bFoil)
    {
        Write("SteamAPI_ISteamUser_GetGameBadgeLevel");
        return SteamClient.SteamUser.GetGameBadgeLevel(nSeries, bFoil);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamUser_GetPlayerSteamLevel()
    {
        Write("SteamAPI_ISteamUser_GetPlayerSteamLevel");
        return SteamClient.SteamUser.GetPlayerSteamLevel();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamUser_RequestStoreAuthURL(string pchRedirectURL)
    {
        Write("SteamAPI_ISteamUser_RequestStoreAuthURL");
        return SteamClient.SteamUser.RequestStoreAuthURL(pchRedirectURL);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUser_BIsPhoneVerified()
    {
        Write("SteamAPI_ISteamUser_BIsPhoneVerified");
        return SteamClient.SteamUser.BIsPhoneVerified();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUser_BIsTwoFactorEnabled()
    {
        Write("SteamAPI_ISteamUser_BIsTwoFactorEnabled");
        return SteamClient.SteamUser.BIsTwoFactorEnabled();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUser_BIsPhoneIdentifying()
    {
        Write("SteamAPI_ISteamUser_BIsPhoneIdentifying");
        return SteamClient.SteamUser.BIsPhoneIdentifying();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUser_BIsPhoneRequiringVerification()
    {
        Write("SteamAPI_ISteamUser_BIsPhoneRequiringVerification");
        return SteamClient.SteamUser.BIsPhoneRequiringVerification();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamUser_GetMarketEligibility()
    {
        Write("SteamAPI_ISteamUser_GetMarketEligibility");
        return SteamClient.SteamUser.GetMarketEligibility();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamUser_GetDurationControl()
    {
        Write("SteamAPI_ISteamUser_GetDurationControl");
        return SteamClient.SteamUser.GetDurationControl();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUser_BSetDurationControlOnlineState(EDurationControlOnlineState eNewState)
    {
        Write("SteamAPI_ISteamUser_BSetDurationControlOnlineState");
        return SteamClient.SteamUser.BSetDurationControlOnlineState(eNewState);
    }

}

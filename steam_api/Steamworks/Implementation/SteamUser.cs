using System;
using System.Runtime.InteropServices;
using Core.Interface;
using SKYNET;
using SKYNET.Interface;
using Steamworks;

//[Map("SteamUser")]
public class SteamUser : IBaseInterface, ISteamUser
{
    public HSteamUser GetHSteamUser(IntPtr _)
    {
        return default;
    }

    public bool BLoggedOn(IntPtr _)
    {
        return false;
    }

    public IntPtr GetSteamID(IntPtr _)
    {
        return default;
    }

    public int InitiateGameConnection(IntPtr _, IntPtr pAuthBlob, int cbMaxAuthBlob, IntPtr steamIDGameServer, uint unIPServer, uint usPortServer, bool bSecure)
    {
        return 0;
    }

    public void TerminateGameConnection(IntPtr _, uint unIPServer, uint usPortServer)
    {
        //
    }

    public void TrackAppUsageEvent(IntPtr _, IntPtr gameID, int eAppUsageEvent, string pchExtraInfo = "")
    {
        //
    }

    public bool GetUserDataFolder(IntPtr _, string pchBuffer, int cubBuffer)
    {
        return false;
    }

    public void StartVoiceRecording(IntPtr _)
    {
        //
    }

    public void StopVoiceRecording(IntPtr _)
    {
        //
    }

    public EVoiceResult GetAvailableVoice(IntPtr _, uint pcbCompressed, uint pcbUncompressed_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
    {
        return default;
    }

    public EVoiceResult GetVoice(IntPtr _, bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, uint nBytesWritten, bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint cbUncompressedDestBufferSize_Deprecated, uint nUncompressBytesWritten_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
    {
        return default;
    }

    public EVoiceResult DecompressVoice(IntPtr _, IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, uint nBytesWritten, uint nDesiredSampleRate)
    {
        return default;
    }

    public uint GetVoiceOptimalSampleRate(IntPtr _)
    {
        return 0;
    }

    public HAuthTicket GetAuthSessionTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, uint pcbTicket)
    {
        return default;
    }

    public EBeginAuthSessionResult BeginAuthSession(IntPtr _, IntPtr pAuthTicket, int cbAuthTicket, IntPtr steamID)
    {
        return default;
    }

    public void EndAuthSession(IntPtr _, IntPtr steamID)
    {
        //
    }

    public void CancelAuthTicket(IntPtr _, HAuthTicket hAuthTicket)
    {
        //
    }

    public EUserHasLicenseForAppResult UserHasLicenseForApp(IntPtr _, IntPtr steamID, AppId_t appID)
    {
        return default;
    }

    public bool BIsBehindNAT(IntPtr _)
    {
        return false;
    }

    public void AdvertiseGame(IntPtr _, IntPtr steamIDGameServer, uint unIPServer, uint usPortServer)
    {
        //
    }

    public SteamAPICall_t RequestEncryptedAppTicket(IntPtr _, IntPtr pDataToInclude, int cbDataToInclude)
    {
        return default;
    }

    public bool GetEncryptedAppTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, uint pcbTicket)
    {
        return false;
    }

    public int GetGameBadgeLevel(IntPtr _, int nSeries, bool bFoil)
    {
        return 0;
    }

    public int GetPlayerSteamLevel(IntPtr _)
    {
        return 0;
    }

    public SteamAPICall_t RequestStoreAuthURL(IntPtr _, string pchRedirectURL)
    {
        return default;
    }

    public bool BIsPhoneVerified(IntPtr _)
    {
        return false;
    }

    public bool BIsTwoFactorEnabled(IntPtr _)
    {
        return false;
    }

    public bool BIsPhoneIdentifying(IntPtr _)
    {
        return false;
    }

    public bool BIsPhoneRequiringVerification(IntPtr _)
    {
        return false;
    }

    public SteamAPICall_t GetMarketEligibility(IntPtr _)
    {
        return default;
    }

    public SteamAPICall_t GetDurationControl(IntPtr _)
    {
        return default;
    }

    public bool BSetDurationControlOnlineState(IntPtr _, EDurationControlOnlineState eNewState)
    {
        return false;
    }
    private void Write(string v)
    {
        Main.Write(InterfaceVersion, v);
    }

}
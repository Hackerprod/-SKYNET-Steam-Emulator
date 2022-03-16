using SKYNET.Interface;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate("SteamUser")]
    public class DSteamUser : SteamDelegate
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate HSteamUser GetHSteamUser();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BLoggedOn();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetSteamID();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int InitiateGameConnection(IntPtr pAuthBlob, int cbMaxAuthBlob, IntPtr steamIDGameServer, uint unIPServer, uint usPortServer, bool bSecure);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void TerminateGameConnection(uint unIPServer, uint usPortServer);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void TrackAppUsageEvent(IntPtr gameID, int eAppUsageEvent, string pchExtraInfo = "");

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetUserDataFolder(string pchBuffer, int cubBuffer);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void StartVoiceRecording();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void StopVoiceRecording();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EVoiceResult GetAvailableVoice(uint pcbCompressed, uint pcbUncompressed_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EVoiceResult GetVoice(bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, uint nBytesWritten, bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint cbUncompressedDestBufferSize_Deprecated, uint nUncompressBytesWritten_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EVoiceResult DecompressVoice(IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, uint nBytesWritten, uint nDesiredSampleRate);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetVoiceOptimalSampleRate();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate HAuthTicket GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, uint pcbTicket);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EBeginAuthSessionResult BeginAuthSession(IntPtr pAuthTicket, int cbAuthTicket, IntPtr steamID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void EndAuthSession(IntPtr steamID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void CancelAuthTicket(HAuthTicket hAuthTicket);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EUserHasLicenseForAppResult UserHasLicenseForApp(IntPtr steamID, AppId_t appID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsBehindNAT();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void AdvertiseGame(IntPtr steamIDGameServer, uint unIPServer, uint usPortServer);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t RequestEncryptedAppTicket(IntPtr pDataToInclude, int cbDataToInclude);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetEncryptedAppTicket(IntPtr pTicket, int cbMaxTicket, uint pcbTicket);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetGameBadgeLevel(int nSeries, bool bFoil);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetPlayerSteamLevel();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t RequestStoreAuthURL(string pchRedirectURL);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsPhoneVerified();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsTwoFactorEnabled();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsPhoneIdentifying();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsPhoneRequiringVerification();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t GetMarketEligibility();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t GetDurationControl();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BSetDurationControlOnlineState(EDurationControlOnlineState eNewState);

    }
}

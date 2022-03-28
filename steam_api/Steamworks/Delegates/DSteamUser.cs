
using SKYNET.Steamworks;
using SKYNET.Types;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate(Name = "SteamUser")]
    public class DSteamUser
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate HSteamUser GetHSteamUser(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BLoggedOn(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamId GetSteamID(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int InitiateGameConnection(IntPtr _, IntPtr pAuthBlob, int cbMaxAuthBlob, IntPtr steamIDGameServer, uint unIPServer, uint usPortServer, bool bSecure);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void TerminateGameConnection(IntPtr _, uint unIPServer, uint usPortServer);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void TrackAppUsageEvent(IntPtr _, IntPtr gameID, int eAppUsageEvent, string pchExtraInfo = "");

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetUserDataFolder(IntPtr _, string pchBuffer, int cubBuffer);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void StartVoiceRecording(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void StopVoiceRecording(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EVoiceResult GetAvailableVoice(IntPtr _, uint pcbCompressed, uint pcbUncompressed_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EVoiceResult GetVoice(IntPtr _, bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, uint nBytesWritten, bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint cbUncompressedDestBufferSize_Deprecated, uint nUncompressBytesWritten_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EVoiceResult DecompressVoice(IntPtr _, IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, uint nBytesWritten, uint nDesiredSampleRate);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetVoiceOptimalSampleRate(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate HAuthTicket GetAuthSessionTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, uint pcbTicket);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EBeginAuthSessionResult BeginAuthSession(IntPtr _, IntPtr pAuthTicket, int cbAuthTicket, IntPtr steamID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void EndAuthSession(IntPtr _, IntPtr steamID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void CancelAuthTicket(IntPtr _, HAuthTicket hAuthTicket);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EUserHasLicenseForAppResult UserHasLicenseForApp(IntPtr _, IntPtr steamID, AppId_t appID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsBehindNAT(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void AdvertiseGame(IntPtr _, IntPtr steamIDGameServer, uint unIPServer, uint usPortServer);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t RequestEncryptedAppTicket(IntPtr _, IntPtr pDataToInclude, int cbDataToInclude);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetEncryptedAppTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, uint pcbTicket);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetGameBadgeLevel(IntPtr _, int nSeries, bool bFoil);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetPlayerSteamLevel(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t RequestStoreAuthURL(IntPtr _, string pchRedirectURL);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsPhoneVerified(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsTwoFactorEnabled(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsPhoneIdentifying(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsPhoneRequiringVerification(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t GetMarketEligibility(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t GetDurationControl(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BSetDurationControlOnlineState(IntPtr _, EDurationControlOnlineState eNewState);

    }
}

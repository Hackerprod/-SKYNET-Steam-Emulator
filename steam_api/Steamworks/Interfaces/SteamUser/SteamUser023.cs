using System;
using System.Runtime.InteropServices;

using SteamAPICall_t = System.UInt64;
using HSteamUser = System.UInt32;
using HAuthTicket = System.UInt32;
using AppId_t = System.UInt32;
using SKYNET.Helpers;
using SKYNET.Steamworks.Types;

namespace SKYNET.Steamworks.Interfaces
{
	[Interface("SteamUser023")]
	public class SteamUser023 : ISteamInterface
	{
		public HSteamUser GetHSteamUser(IntPtr _)
		{
			return SteamEmulator.SteamUser.GetHSteamUser();
		}

		public bool BLoggedOn(IntPtr _)
		{
			return SteamEmulator.SteamUser.BLoggedOn();
		}

		public IntPtr GetSteamID(IntPtr _, IntPtr pSteamID)
		{
			return NativeSteamId.Write(pSteamID, SteamEmulator.SteamUser.GetSteamID());
		}

		public int InitiateGameConnection_DEPRECATED(IntPtr _, IntPtr pAuthBlob, int cbMaxAuthBlob, ulong steamIDGameServer, uint unIPServer, ushort usPortServer, bool bSecure)
		{
			return SteamEmulator.SteamUser.InitiateGameConnection_DEPRECATED(pAuthBlob, cbMaxAuthBlob, steamIDGameServer, unIPServer, usPortServer, bSecure);
		}

		public void TerminateGameConnection_DEPRECATED(IntPtr _, uint unIPServer, ushort usPortServer)
		{
			SteamEmulator.SteamUser.TerminateGameConnection_DEPRECATED(unIPServer, usPortServer);
		}

		public void TrackAppUsageEvent(IntPtr _, ulong gameID, int eAppUsageEvent, string pchExtraInfo)
		{
			SteamEmulator.SteamUser.TrackAppUsageEvent(gameID, eAppUsageEvent, pchExtraInfo);
		}

		public bool GetUserDataFolder(IntPtr _, IntPtr pchBuffer, int cubBuffer)
		{
			return SteamEmulator.SteamUser.GetUserDataFolder(pchBuffer, cubBuffer);
		}

		public void StartVoiceRecording(IntPtr _)
		{
			SteamEmulator.SteamUser.StartVoiceRecording();
		}

		public void StopVoiceRecording(IntPtr _)
		{
			SteamEmulator.SteamUser.StopVoiceRecording();
		}

		public EVoiceResult GetAvailableVoice(IntPtr _, IntPtr pcbCompressed, IntPtr pcbUncompressed_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
		{
			return SteamEmulator.SteamUser.GetAvailableVoice(pcbCompressed, pcbUncompressed_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
		}

		public EVoiceResult GetVoice(IntPtr _, bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, IntPtr nBytesWritten, bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint cbUncompressedDestBufferSize_Deprecated, IntPtr nUncompressBytesWritten_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
		{
			return SteamEmulator.SteamUser.GetVoice(bWantCompressed, pDestBuffer, cbDestBufferSize, nBytesWritten, bWantUncompressed_Deprecated, pUncompressedDestBuffer_Deprecated, cbUncompressedDestBufferSize_Deprecated, nUncompressBytesWritten_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
		}

		public EVoiceResult DecompressVoice(IntPtr _, IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, IntPtr nBytesWritten, uint nDesiredSampleRate)
		{
			return SteamEmulator.SteamUser.DecompressVoice(pCompressed, cbCompressed, pDestBuffer, cbDestBufferSize, nBytesWritten, nDesiredSampleRate);
		}

		public uint GetVoiceOptimalSampleRate(IntPtr _)
		{
			return SteamEmulator.SteamUser.GetVoiceOptimalSampleRate();
		}

		public HAuthTicket GetAuthSessionTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, out uint pcbTicket, IntPtr pSteamNetworkingIdentity)
		{
			return SteamEmulator.SteamUser.GetAuthSessionTicket(pTicket, cbMaxTicket, out pcbTicket);
		}

		public HAuthTicket GetAuthTicketForWebApi(IntPtr _, string pchIdentity)
		{
			return SteamEmulator.SteamUser.GetAuthTicketForWebApi(pchIdentity);
		}

		public int BeginAuthSession(IntPtr _, IntPtr pAuthTicket, int cbAuthTicket, ulong steamID)
		{
			return SteamEmulator.SteamUser.BeginAuthSession(pAuthTicket, cbAuthTicket, steamID);
		}

		public void EndAuthSession(IntPtr _, ulong steamID)
		{
			SteamEmulator.SteamUser.EndAuthSession(steamID);
		}

		public void CancelAuthTicket(IntPtr _, HAuthTicket hAuthTicket)
		{
			SteamEmulator.SteamUser.CancelAuthTicket(hAuthTicket);
		}

		public int UserHasLicenseForApp(IntPtr _, ulong steamID, AppId_t appID)
		{
			return SteamEmulator.SteamUser.UserHasLicenseForApp(steamID, appID);
		}

		public bool BIsBehindNAT(IntPtr _)
		{
			return SteamEmulator.SteamUser.BIsBehindNAT();
		}

		public void AdvertiseGame(IntPtr _, ulong steamIDGameServer, uint unIPServer, ushort usPortServer)
		{
			SteamEmulator.SteamUser.AdvertiseGame(steamIDGameServer, unIPServer, usPortServer);
		}

		public SteamAPICall_t RequestEncryptedAppTicket(IntPtr _, IntPtr pDataToInclude, int cbDataToInclude)
		{
			return SteamEmulator.SteamUser.RequestEncryptedAppTicket(pDataToInclude, cbDataToInclude);
		}

		public bool GetEncryptedAppTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, IntPtr pcbTicket)
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

		public SteamAPICall_t GetMarketEligibility(IntPtr _)
		{
			return SteamEmulator.SteamUser.GetMarketEligibility();
		}

		public SteamAPICall_t GetDurationControl(IntPtr _)
		{
			return SteamEmulator.SteamUser.GetDurationControl();
		}

		public bool BSetDurationControlOnlineState(IntPtr _, int eNewState)
		{
			return SteamEmulator.SteamUser.BSetDurationControlOnlineState(eNewState);
		}
	}

}

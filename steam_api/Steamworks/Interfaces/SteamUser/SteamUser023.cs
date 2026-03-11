using System;
using System.Runtime.InteropServices;

using SteamAPICall_t = System.UInt64;
using HSteamUser = System.UInt32;
using HAuthTicket = System.UInt32;
using AppId_t = System.UInt32;
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

		public CSteamID2 GetSteamID(IntPtr _)
		{
			return new CSteamID2(1000, CSteamID2.EUniverse.Public, CSteamID2.EAccountType.Individual);
		}

		public int InitiateGameConnection_DEPRECATED(IntPtr _, IntPtr pAuthBlob, int cbMaxAuthBlob, ulong steamIDGameServer, uint unIPServer, ushort usPortServer, bool bSecure)
		{
			return SteamEmulator.SteamUser.InitiateGameConnection_DEPRECATED(pAuthBlob, cbMaxAuthBlob, steamIDGameServer, unIPServer, usPortServer, bSecure);
		}

		public void TerminateGameConnection_DEPRECATED(IntPtr _, uint unIPServer, ushort usPortServer)
		{
			SteamEmulator.SteamUser.TerminateGameConnection_DEPRECATED(unIPServer, usPortServer);
		}

		public void TrackAppUsageEvent(IntPtr _, UInt32 gameID, int eAppUsageEvent, string pchExtraInfo)
		{
			SteamEmulator.SteamUser.TrackAppUsageEvent(gameID, eAppUsageEvent, pchExtraInfo);
		}

		public bool GetUserDataFolder(IntPtr _, out string pchBuffer, int cubBuffer)
		{
			return SteamEmulator.SteamUser.GetUserDataFolder(out pchBuffer, cubBuffer);
		}

		public void StartVoiceRecording(IntPtr _)
		{
			SteamEmulator.SteamUser.StartVoiceRecording();
		}

		public void StopVoiceRecording(IntPtr _)
		{
			SteamEmulator.SteamUser.StopVoiceRecording();
		}

		public EVoiceResult GetAvailableVoice(IntPtr _, out uint pcbCompressed, out uint pcbUncompressed_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
		{
			return SteamEmulator.SteamUser.GetAvailableVoice(out pcbCompressed, out pcbUncompressed_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
		}

		public EVoiceResult GetVoice(IntPtr _, bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, out uint nBytesWritten, bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint cbUncompressedDestBufferSize_Deprecated, out uint nUncompressBytesWritten_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
		{
			return SteamEmulator.SteamUser.GetVoice(bWantCompressed, pDestBuffer, cbDestBufferSize, out nBytesWritten, bWantUncompressed_Deprecated, pUncompressedDestBuffer_Deprecated, cbUncompressedDestBufferSize_Deprecated, out nUncompressBytesWritten_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
		}

		public EVoiceResult DecompressVoice(IntPtr _, IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, out uint nBytesWritten, uint nDesiredSampleRate)
		{
			return SteamEmulator.SteamUser.DecompressVoice(pCompressed, cbCompressed, pDestBuffer, cbDestBufferSize, out nBytesWritten, nDesiredSampleRate);
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

		public bool BIsBehindNAT(IntPtr _)
		{
			return SteamEmulator.SteamUser.BIsBehindNAT();
		}

		public void AdvertiseGame(IntPtr _, ulong steamIDGameServer, uint unIPServer, ushort usPortServer)
		{
			SteamEmulator.SteamUser.AdvertiseGame(steamIDGameServer, unIPServer, usPortServer);
		}
	}

}


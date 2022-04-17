using SKYNET.Types;
using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Explicit, Pack = 8)]
	public struct UserStatsReceived_t
	{
		public const int k_iCallback = 1101;

		[FieldOffset(0)]
		public ulong m_nGameID;

		[FieldOffset(8)]
		public EResult m_eResult;

		[FieldOffset(12)]
		public SteamID m_steamIDUser;
	}
}

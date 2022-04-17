using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SteamUGCQueryCompleted_t
	{
		public const int k_iCallback = 3401;

		public UGCQueryHandle_t m_handle;

		public EResult m_eResult;

		public uint m_unNumResultsReturned;

		public uint m_unTotalMatchingResults;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bCachedData;
	}
}

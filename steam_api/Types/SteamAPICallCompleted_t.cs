
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Steamworks
{
	//[CallbackIdentity] 
	public struct SteamAPICallCompleted_t 
	{
		public const int k_iCallback = 703; 
		public SteamAPICall_t m_hAsyncCall;
		public int m_iCallback; 
		public uint m_cubParam;
	}
}

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;


namespace Steamworks
{
	public struct CallbackMsg_t 
	{
		public int m_hSteamUser; 
		public int m_iCallback; 
		public IntPtr m_pubParam; 
		public int m_cubParam; 
	}
}

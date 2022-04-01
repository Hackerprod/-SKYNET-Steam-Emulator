using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

// Image 110: Assembly-CSharp-firstpass.dll - Assembly: Assembly-CSharp-firstpass, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null

namespace Steamworks
{
	public struct FriendGameInfo_t
	{
		// Fields
		public uint GameID;
		public uint GameIP;
		public ushort GamePort;
		public ushort QueryPort;
		public IntPtr steamIDLobby;
	}
}

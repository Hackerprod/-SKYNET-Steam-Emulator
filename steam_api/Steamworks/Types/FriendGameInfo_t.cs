using System;

namespace SKYNET.Steamworks
{
	public struct FriendGameInfo_t
	{
		public uint GameID;
		public uint GameIP;
		public ushort GamePort;
		public ushort QueryPort;
		public IntPtr steamIDLobby;
	}
}

using System;

using System.Runtime.InteropServices;

using Core.Interface;
using SKYNET;

namespace InterfaceFriends
{
    [Impl(Name = "SteamFriends001", ServerMapped = true)]
    public class SteamFriends001 : IBaseInterface
    {
        public SteamFriends001()
        {
        }

        public string GetPersonaName()
        {
            Write("GetPersonaName");
            return "Federiko";
        }

        public uint GetPersonaState()
        {
            Write("GetPersonaState");
            return (uint)1;
        }

        public void SetPersonaState(uint state)
        {
            Write("SetPersonaState");
        }

        public bool AddFriend(ulong steam_id)
        {
            Write("AddFriend");
            return true;
        }

        public bool RemoveFriend(ulong steam_id)
        {
            Write("RemoveFriend");
            return true;
        }

        public bool HasFriend(ulong steam_id)
        {
            Write("HasFriend");
            return true;
        }

        public uint GetFriendRelationship(ulong steam_id)
        {
            Write("GetFriendRelationship");
            return (uint)0;
        }

        public uint GetFriendPersonaState(ulong steam_id)
        {
            Write("GetFriendPersonaState");
            return (uint)1;
        }

        public bool Deprecated_GetFriendGamePlayed(ulong steam_id, ref uint app_id, ref uint game_ip, ref uint game_port)
        {
            Write("Deprecated_GetFriendGamePlayed");
            return false;
        }

        public string GetFriendPersonaName(ulong steam_id)
        {
            Write("GetFriendPersonaName");
            return "Fede";
        }

        // TODO: this returns a HSteamCall, but that isnt a thing anymore??
        // For now we just perform the action and then return 0
        public int AddFriendByName(string name)
        {
            Write("AddFriendByName");
            return 0;
        }

        public int GetFriendCount()
        {
            Write("GetFriendCount");
            return 100;
        }

        public ulong GetFriendByIndex(int index)
        {
            Write("GetFriendByIndex");
            return 0;
        }

        public void SendMsgToFriend(ulong dest_steam_id, uint msg_type, string message_body)
        {
            Write("SendMsgToFriend");
        }

        public void SetFriendRegValue(ulong steam_id, string key, string value)
        {
            // TODO: hook up to configstore
        }

        public string GetFriendRegValue(ulong steam_id, string key)
        {
            // TODO: hook up to configstore
            return "";
        }

        public string GetFriendPersonaNameHistory(ulong steam_id, int index)
        {
            return "";
        }

        // Real sig is int GetChatMessage(u64, i32, void *, i32, u32 *)
        [Buffer(Index = 2, NewPointerIndex = 2, NewSizeIndex = 3)]
        public int GetChatMessage(ulong steam_id, int msg_index, ref IntPtr b, ref uint msg_type)
        {
            Write("GetChatMessage");
            return 0;
        }

        public bool SendMsgToFriend2(ulong steam_id, uint type, string message, int length)
        {
            Write("SendMsgToFriend2");
            return true;
        }

        public int GetChatIDOfChatHistoryStart(ulong steam_id)
        {
            Console.WriteLine("GetChatIDOfChatHistoryStart should never be called!");
            return 0;
        }

        public void SetChatHistoryStart(ulong steam_id, int index)
        {
            Console.WriteLine("SetChatHistoryStart should never be called!");
        }

        public void ClearChatHistory(ulong steam_id)
        {
        }

        public int InviteFriendByEmail(string email)
        {
            Write("InviteFriendByEmail");

            return 0;
        }

        public int GetBlockedFriendCount()
        {
            Write("GetBlockedFriendCount");
            return 0;
        }

        public bool GetFriendGamePlayed(ulong steam_id, ref ulong game_id, ref uint server_ip, ref ushort server_port)
        {
            Write("GetFriendGamePlayed");
            return true;
        }
        public bool GetFriendGamePlayed2(ulong steam_id, ref ulong game_id, ref uint server_ip, ref ushort server_port)
        {
            Write("GetFriendGamePlayed2");
            return GetFriendGamePlayed(steam_id, ref game_id, ref server_ip, ref server_port);
        }

        private void Write(string v)
        {
            Main.Write(InterfaceVersion, v);
        }
    }
}

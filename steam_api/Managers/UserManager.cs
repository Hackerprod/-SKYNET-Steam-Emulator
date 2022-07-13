using SKYNET.Helper;
using SKYNET.Steamworks;
using SKYNET.Types;
using System.Collections.Generic;

namespace SKYNET.Managers
{
    public class UserManager
    {
        public static List<SteamPlayer> Users;

        static UserManager()
        {
            Users = new List<SteamPlayer>();
        }

        public static void UpdateUsers(List<SteamPlayer> UpdatedList)
        {
            MutexHelper.Wait("Users", delegate
            {
                Users.Clear();
                Users.AddRange(UpdatedList);
            });
        }

        public static SteamPlayer GetUser(ulong steamID)
        {
            SteamPlayer user = default;
            MutexHelper.Wait("Users", delegate
            {
                user = Users.Find(u => u.SteamID == steamID);
            });
            return user;
        }

        public static SteamPlayer GetUser(uint accountID)
        {
            SteamPlayer user = default;
            MutexHelper.Wait("Users", delegate
            {
                user = Users.Find(u => u.AccountID == accountID);
            });
            return user;
        }
        public static SteamPlayer GetUser(CSteamID steamID)
        {
            return GetUser((ulong)steamID);
        }

        public static SteamPlayer GetUserByAddress(string iPAddress)
        {
            SteamPlayer player = null;
            MutexHelper.Wait("Users", delegate
            {
                player = Users.Find(u => u.IPAddress == iPAddress);
            });
            return player;
        }

        public static List<SteamPlayer> GetFriends()
        {
            return Users.FindAll(f => f.SteamID != SteamEmulator.SteamID);
        }

        private static void Write(object msg)
        {
            SteamEmulator.Write("UserManager", msg);
        }
    }
}

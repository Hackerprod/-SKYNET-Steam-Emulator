using SKYNET.Client;
using SKYNET.Common;
using SKYNET.Helper;
using SKYNET.Steamworks;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Managers
{
    public class UserManager
    {
        public static event EventHandler<SteamPlayer> OnUserAdded;
        public static event EventHandler<SteamPlayer> OnUserUpdated;
        public static event EventHandler<SteamPlayer> OnUserRemoved;
        public static EventHandler<AvatarReceivedEventArgs> OnAvatarReceived;
        public static List<SteamPlayer> Users;
        public static Dictionary<ulong, Bitmap> UserAvatars;

        static UserManager()
        {
            Users = new List<SteamPlayer>();
            UserAvatars = new Dictionary<ulong, Bitmap>();
        }

        public static void Initialize(CSteamID SteamID, string PersonaName)
        {
            Users.Add(new SteamPlayer()
            {
                SteamID = (ulong)SteamID,
                PersonaName = PersonaName,
                AccountID = SteamID.AccountID,
                IPAddress = NetworkHelper.GetIPAddress().ToString(),
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

        public static void AddOrUpdateUser(uint accountID, string personaName, uint appID, string IPAddress)
        {
            MutexHelper.Wait("Users", delegate
            {
                var user = GetUser((ulong)new CSteamID(accountID));
                if (user == null)
                {
                    CSteamID steamID = new CSteamID(accountID);
                    user = new SteamPlayer()
                    {
                        PersonaName = personaName,
                        AccountID = accountID,
                        SteamID = (ulong)steamID,
                        GameID = appID,
                        IPAddress = IPAddress,
                        HasFriend = true
                    };
                    Users.Add(user);
                    OnUserAdded?.Invoke(null, user);
                    Write($"Added user {personaName} {steamID}, from {user.IPAddress}");
                }
                else
                {
                    user.PersonaName = personaName;
                    user.IPAddress = IPAddress;
                    user.GameID = appID;
                    OnUserUpdated?.Invoke(null, user);
                }
            });
            IPCManager.SendUpdatedUsers();
        }

        public static void RemoveUser(uint accountID)
        {
            MutexHelper.Wait("Users", delegate
            {
                var user = GetUser((ulong)new CSteamID(accountID));
                if (user != null)
                {
                    OnUserRemoved?.Invoke(null, user);
                    Write($"Removed user {user.PersonaName} {user.SteamID}");
                    Users.Remove(user);
                }
            });
            IPCManager.SendUpdatedUsers();
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

        public static void AvatarReceived(uint accountID, Bitmap avatar)
        {
            var User = GetUser(accountID);
            if (User != null)
            {
                if (UserAvatars.ContainsKey(User.SteamID))
                {
                    UserAvatars[User.SteamID] = avatar;
                }
                else
                {
                    UserAvatars.Add(User.SteamID, avatar);
                }
            }
            OnAvatarReceived?.Invoke(null, new AvatarReceivedEventArgs(accountID, avatar));
        }

        public static List<SteamPlayer> GetFriends()
        {
            return Users.FindAll(f => f.SteamID != SteamClient.SteamID);
        }

        public static void UserDataUpdated(uint accountID, string personaName, uint lobbyID)
        {
            var User = GetUser(accountID);
            if (User != null)
            {
                User.PersonaName = personaName;
                User.LobbyID = new CSteamID(lobbyID).AccountID;
            }
            IPCManager.SendUpdatedUsers();
        }

        public static void SetRichPresence(uint accountID, string key, string value)
        {
            var user = GetUser(accountID);
            if (user == null) return;
            if (user.RichPresence == null) user.RichPresence = new Dictionary<string, string>();
            if (user.RichPresence.ContainsKey(key))
                user.RichPresence[key] = value;
            else
                user.RichPresence.Add(key, value);
            IPCManager.SendUpdatedUsers();
        }

        public static Bitmap GetAvatar(ulong steamID)
        {
            var User = GetUser(steamID);
            if (User != null)
            {
                if (UserAvatars.ContainsKey(User.SteamID))
                {
                    return UserAvatars[User.SteamID];
                }
            }
            return SteamClient.DefaultAvatar;
        }

        private static void Write(object msg)
        {
            Log.Write("UserManager", msg);
        }

        public class AvatarReceivedEventArgs : EventArgs
        {
            public Bitmap Avatar;
            public uint AccountID;
            public AvatarReceivedEventArgs(uint accountID, Bitmap avatar)
            {
                AccountID = accountID;
                Avatar = avatar;
            }
        }
    }
}

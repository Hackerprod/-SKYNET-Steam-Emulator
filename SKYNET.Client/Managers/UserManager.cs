using SKYNET.Client;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using SKYNET.Types;
using SKYNET.WEB.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SKYNET.Managers
{
    public class UserManager
    {
        public static event EventHandler<SteamPlayer> OnUserUpdated;
        public static event EventHandler<SteamPlayer> OnUserRemoved;
        public static EventHandler<AvatarReceivedEventArgs> OnAvatarReceived;
        public static List<SteamPlayer> Users;
        public static Dictionary<ulong, Bitmap> UserAvatars;
        private static WebClient WebClient;

        static UserManager()
        {
            Users = new List<SteamPlayer>();
            UserAvatars = new Dictionary<ulong, Bitmap>();
            WebClient = new WebClient();
        }

        public static void Initialize()
        {
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

        public static void AddOrUpdateUser(uint accountID, string personaName)
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
                    };
                    Users.Add(user);
                    Write($"Added user {personaName} {steamID}");
                }
                else
                {
                    user.PersonaName = personaName;
                    OnUserUpdated?.Invoke(null, user);
                }
            });
            IPCManager.SendUpdatedUsers();
        }

        public static bool RequestAvatar(uint accountID, out Bitmap avatar)
        {
            string Url = $"http://{Settings.ServerIP}:27080/Images/AvatarCache/{accountID}.jpg";
            return RequestAvatar(Url, out avatar); 
        }

        public static bool RequestAvatar(string Url, out Bitmap avatar)
        {
            avatar = null;
            try
            {
                var WebClient = new WebClient();
                var Data = WebClient.DownloadData(Url);
                avatar = (Bitmap)ImageHelper.ImageFromBytes(Data);
                return true;
            }
            catch
            {
                return false;
            }
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

        public static object GetUser(object accountID)
        {
            throw new NotImplementedException();
        }

        internal static bool UpdateAvatar(Bitmap Image)
        {
            try
            {
                if (UserAvatars.ContainsKey((ulong)SteamClient.SteamID))
                {
                    UserAvatars[(ulong)SteamClient.SteamID] = Image;
                }
                else
                {
                    UserAvatars.Add((ulong)SteamClient.SteamID, Image);
                }

                SteamClient.Avatar = Image;

                string ImagePath = Path.Combine(Common.GetPath(), "Data", "Images", "AvatarCache", "Avatar.jpg");
                ImageHelper.ToFile(ImagePath, Image);

                OnAvatarReceived?.Invoke(null, new AvatarReceivedEventArgs(SteamClient.AccountID,Image));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
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

        public static List<SteamPlayer> GetUsersPlaying(uint appID, bool IncludeMe = true)
        {
            List<SteamPlayer> users = default;

            if (IncludeMe)
            {
                users = Users.FindAll(u => u.GameID == appID);
            }
            else
            {
                users = Users.FindAll(u => u.GameID == appID && u.SteamID != SteamClient.SteamID);
            }
            return users == null ? new List<SteamPlayer>() : users;
        }

        public static bool GetAvatar(ulong steamID, out Bitmap bitmap)
        {
            if (UserAvatars.ContainsKey(steamID))
            {
                bitmap = UserAvatars[steamID];
                return true;
            }
            else if (RequestAvatar(steamID.GetAccountID(), out var requestedBitmap))
            {
                bitmap = requestedBitmap;
                UserAvatars.Add(steamID, requestedBitmap);
                return true;
            }
            bitmap = SteamClient.DefaultAvatar;
            return false;
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

        public static void DownloadAvatar(uint accountID, string iPAddress)
        {
            ThreadPool.QueueUserWorkItem(DownloadAvatarTask, new object[] { accountID, iPAddress });
        }

        private static async void DownloadAvatarTask(object state)
        {
            try
            {
                object[] objects = (object[])state;
                uint accountID = (uint)objects[0];
                string iPAddress = (string)objects[1];
                string Url = $"http://{iPAddress}/Images/AvatarCache/Avatar.jpg";
                var Data = await WebClient.DownloadDataTaskAsync(Url);
                var Avatar = (Bitmap)ImageHelper.ImageFromBytes(Data);
                string AvatarCachePath = Path.Combine(Common.GetPath(), "Data", "Images", "AvatarCache", accountID + ".jpg");
                Common.EnsureDirectoryExists(AvatarCachePath, true);
                ImageHelper.ToFile(AvatarCachePath, Avatar);
                AvatarReceived(accountID, Avatar);
            }
            catch 
            {
            }
        }

        public static void UpdateUserPlaying(uint accountID, uint appID)
        {
            var user = GetUser(accountID);
            if (user != null)
            {
                user.GameID = appID;
                IPCManager.SendUpdatedUsers();
            }
        }

        public static void UpdateUserLobby(ulong userSteamID, ulong lobbySteamID)
        {
            var user = GetUser(userSteamID);
            if (user != null)
            {
                user.LobbyID = lobbySteamID;
                IPCManager.SendUpdatedUsers();
            }
        }

        internal static bool UserExists(uint accountID)
        {
            return GetUser(accountID) != null;
        }
    }
}

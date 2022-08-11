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
        private static WebClient WebClient;

        static UserManager()
        {
            Users = new List<SteamPlayer>();
            UserAvatars = new Dictionary<ulong, Bitmap>();
            WebClient = new WebClient();
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
             return;
            // Create TESTS Users
            Users.Add(new SteamPlayer()
            {
                SteamID = (ulong)new CSteamID(2000),
                PersonaName = "Alejandro",
                AccountID = 2000,
                IPAddress = NetworkHelper.GetIPAddress().ToString(),
                GameID = 570,
                HasFriend = true,
            });
            Users.Add(new SteamPlayer()
            {
                SteamID = (ulong)new CSteamID(3000),
                PersonaName = "Elier",
                AccountID = 3000,
                IPAddress = NetworkHelper.GetIPAddress().ToString(),
                GameID = 570,
                HasFriend = true,
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

        internal static bool UpdateUser(WEB_UpdateUser info, out bool AvatarUpdated)
        {
            var Result = false;
            AvatarUpdated = false;
            var User = GetUser(SteamClient.AccountID);
            if (User == null)
            {
                Write($"Warn... User {SteamClient.AccountID} not fount to edit.");
                return Result;
            }
            if (info.AccountID != 0 && info.AccountID != SteamClient.AccountID)
            {
                SteamClient.AccountID = User.AccountID = info.AccountID;
                SteamClient.SteamID = new CSteamID(info.AccountID);
                Settings.AccountID = info.AccountID;
                User.SteamID = (ulong)SteamClient.SteamID;
                Result = true;
            }
            if (!string.IsNullOrEmpty(info.PersonaName) && info.PersonaName != SteamClient.PersonaName)
            {
                SteamClient.PersonaName = User.PersonaName = info.PersonaName;
                Settings.PersonaName = info.PersonaName;
                Result = true;
            }
            if (!string.IsNullOrEmpty(info.Language) && info.Language != SteamClient.Language)
            {
                SteamClient.Language = info.Language;
                Settings.Language = info.Language;
                Result = true;
            }
            if (info.AllowRemoteAccess != Settings.AllowRemoteAccess)
            {
                Settings.AllowRemoteAccess = info.AllowRemoteAccess;
                Result = true;
            }
            if (info.ShowDebugConsole != Settings.ShowDebugConsole)
            {
                Settings.ShowDebugConsole = info.ShowDebugConsole;
                Result = true;
            }
            if (!string.IsNullOrEmpty(info.AvatarBase64) && (ImageHelper.ImageToBase64(SteamClient.Avatar) != info.AvatarBase64))
            {
                UpdateAvatar(ImageHelper.ImageFromBase64(info.AvatarBase64));
                AvatarUpdated = true;
                Result = true;
            }

            OnUserUpdated?.Invoke(null, User);

            return Result;
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

        public static bool GetAvatar(ulong steamID, out Bitmap bitmap)
        {
            if (UserAvatars.ContainsKey(steamID))
            {
                bitmap = UserAvatars[steamID];
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

using MongoDB.Driver;
using SKYNET.Steamworks;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SKYNET.DB
{
    public static class UserDB
    {
        public  static event EventHandler<SteamPlayer> OnNewAccount;
        private static MongoDbCollection<SteamPlayer> DB;

        public static void Initialize()
        {
            DB = new MongoDbCollection<SteamPlayer>("SKYNET_users");

            DB.CreateIndex(Builders<SteamPlayer>.IndexKeys.Ascending((SteamPlayer i) => i.SteamID));
            DB.CreateIndex(Builders<SteamPlayer>.IndexKeys.Ascending((SteamPlayer i) => i.AccountID));
            DB.CreateIndex(Builders<SteamPlayer>.IndexKeys.Ascending((SteamPlayer i) => i.AccountName));
            DB.CreateIndex(Builders<SteamPlayer>.IndexKeys.Ascending((SteamPlayer i) => i.PersonaName));
        }

        internal static List<SteamPlayer> GetUsers(ulong ExceptID = 0)
        {
            return DB.Collection.Find(u => u.SteamID != ExceptID, null).ToList();
        }

        public static void CreateAccount(SteamPlayer user)
        {
            uint AccountID = DB.Collection.Find(FilterDefinition<SteamPlayer>.Empty, null).SortByDescending((SteamPlayer u) => (object)u.AccountID).Project((SteamPlayer u) => u.AccountID).FirstOrDefault(default(CancellationToken));
            AccountID = AccountID <= 0U ? 1000 : AccountID + 1;
            user.SteamID = new CSteamID((uint)AccountID, EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeInvalid).SteamID;
            user.AccountID = AccountID;
            DB.Collection.InsertOne(user, null, default(CancellationToken));
            OnNewAccount?.Invoke(null, user);
        }

        public static void SetPlayingState(uint AccountID, uint appID)
        {
            if (appID != 0)
            {
                DB.Collection.FindOneAndUpdate((SteamPlayer user) => user.AccountID == AccountID, DB.Ub.Set((SteamPlayer user) => user.PlayingAppID, appID).Set((SteamPlayer user) => user.LastPlayedTime, (uint)DateTime.Now.ToTimestamp()), null, default(CancellationToken));
            }
            else
            {
                DB.Collection.FindOneAndUpdate((SteamPlayer user) => user.AccountID == AccountID, DB.Ub.Set((SteamPlayer user) => user.PlayingAppID, appID), null, default(CancellationToken));
            }
        }

        public static void SetPlayingState(ulong SteamId, bool playing)
        {
            if (playing)
            {
                DB.Collection.FindOneAndUpdate((SteamPlayer user) => user.SteamID == SteamId, DB.Ub.Set((SteamPlayer user) => user.IsPlaying, playing).Set((SteamPlayer user) => user.LastPlayedTime, (uint)DateTime.Now.ToTimestamp()), null, default(CancellationToken));
            }
            else
            {
                DB.Collection.FindOneAndUpdate((SteamPlayer user) => user.SteamID == SteamId, DB.Ub.Set((SteamPlayer user) => user.IsPlaying, playing), null, default(CancellationToken));
            }
        }

        public static int Count(ulong ExceptSteamId = 0)
        {
            return (int)DB.Collection.CountDocuments((SteamPlayer user) => user.SteamID != ExceptSteamId, null, default(CancellationToken));
        }

        public static void SetPlayingState(SteamPlayer User, bool playing)
        {
            SetPlayingState(User.SteamID, playing);
        }

        public static void SetLastLogOff(ulong steamID, DateTime lastLogOff)
        {
            DB.Collection.FindOneAndUpdate((SteamPlayer user) => user.SteamID == steamID, DB.Ub.Set<ulong>((SteamPlayer user) => user.LastLogoff, lastLogOff.ToTimestamp()), null, default(CancellationToken));
        }

        public static void SetLastLogOn(uint accountId, ulong lastLogOn)
        {
            DB.Collection.FindOneAndUpdate((SteamPlayer user) => user.AccountID == accountId, DB.Ub.Set<ulong>((SteamPlayer user) => user.LastLogon, lastLogOn), null, default(CancellationToken));
        }

        public static List<ulong> GetFriends(ulong steamID)
        {
            var player = DB.Collection.Find((SteamPlayer usr) => usr.SteamID == steamID, null).FirstOrDefault(default(CancellationToken));
            if (player != null)
            {
                return player.Friends == null ? new List<ulong>() : player.Friends;
            }
            return new List<ulong>();
        }

        public static bool IsValidPersonaName(string AccountName)
        {
            if (DB.Collection.CountDocuments((SteamPlayer usr) => usr.AccountName.ToLower() == AccountName.ToLower(), null, default(CancellationToken)) != 0L)
            {
                return false;
            }
            return true;
        }

        public static SteamPlayer Get(ulong SteamID)
        {
            return DB.Collection.Find((SteamPlayer usr) => usr.SteamID == SteamID, null).FirstOrDefault(default(CancellationToken));
        }

        public static SteamPlayer Get(uint accountID)
        {
            return DB.Collection.Find((SteamPlayer usr) => usr.AccountID == accountID, null).FirstOrDefault(default(CancellationToken));
        }

        public static SteamPlayer GetByAccountName(string AccountName)
        {
            return DB.Collection.Find((SteamPlayer usr) => usr.AccountName == AccountName, null).FirstOrDefault(default(CancellationToken));
        }

        public static SteamPlayer GetByPersonaName(string AccountName)
        {
            return DB.Collection.Find((SteamPlayer usr) => usr.AccountName == AccountName, null).FirstOrDefault(default(CancellationToken));
        }
    }
}

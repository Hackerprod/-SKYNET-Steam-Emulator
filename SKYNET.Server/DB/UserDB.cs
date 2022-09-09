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
        private static MongoDbCollection<SteamPlayer> DbUser;

        public static async void Initialize()
        {
            DbUser = new MongoDbCollection<SteamPlayer>("SKYNET_users");

            DbUser.CreateIndex(Builders<SteamPlayer>.IndexKeys.Ascending((SteamPlayer i) => i.SteamID));
            DbUser.CreateIndex(Builders<SteamPlayer>.IndexKeys.Ascending((SteamPlayer i) => i.AccountID));
            DbUser.CreateIndex(Builders<SteamPlayer>.IndexKeys.Ascending((SteamPlayer i) => i.AccountName));
            DbUser.CreateIndex(Builders<SteamPlayer>.IndexKeys.Ascending((SteamPlayer i) => i.PersonaName));
        }

        public static void CreateAccount(SteamPlayer user)
        {
            uint AccountID = DbUser.Collection.Find(FilterDefinition<SteamPlayer>.Empty, null).SortByDescending((SteamPlayer u) => (object)u.AccountID).Project((SteamPlayer u) => u.AccountID).FirstOrDefault(default(CancellationToken));
            AccountID = AccountID <= 0U ? 1000 : AccountID + 1;
            user.AccountID = AccountID;
            DbUser.Collection.InsertOne(user, null, default(CancellationToken));
            OnNewAccount.Invoke(null, user);
        }

        public static void SetPlayingState(uint AccountID, bool playing)
        {
            if (playing)
            {
                DbUser.Collection.FindOneAndUpdate((SteamPlayer user) => user.AccountID == AccountID, DbUser.Ub.Set((SteamPlayer user) => user.IsPlaying, playing).Set((SteamPlayer user) => user.LastPlayedTime, (uint)DateTime.Now.ToTimestamp()), null, default(CancellationToken));
            }
            else
            {
                DbUser.Collection.FindOneAndUpdate((SteamPlayer user) => user.AccountID == AccountID, DbUser.Ub.Set((SteamPlayer user) => user.IsPlaying, playing), null, default(CancellationToken));
            }
        }

        public static void SetPlayingState(ulong SteamId, bool playing)
        {
            if (playing)
            {
                DbUser.Collection.FindOneAndUpdate((SteamPlayer user) => user.SteamID == SteamId, DbUser.Ub.Set((SteamPlayer user) => user.IsPlaying, playing).Set((SteamPlayer user) => user.LastPlayedTime, (uint)DateTime.Now.ToTimestamp()), null, default(CancellationToken));
            }
            else
            {
                DbUser.Collection.FindOneAndUpdate((SteamPlayer user) => user.SteamID == SteamId, DbUser.Ub.Set((SteamPlayer user) => user.IsPlaying, playing), null, default(CancellationToken));
            }
        }

        public static int Count(ulong ExceptSteamId = 0)
        {
            return (int)DbUser.Collection.CountDocuments((SteamPlayer user) => user.SteamID != ExceptSteamId, null, default(CancellationToken));
        }

        public static void SetPlayingState(SteamPlayer User, bool playing)
        {
            SetPlayingState(User.SteamID, playing);
        }

        public static void SetLastLogOff(ulong steamID, DateTime lastLogOff)
        {
            DbUser.Collection.FindOneAndUpdate((SteamPlayer user) => user.SteamID == steamID, DbUser.Ub.Set<ulong>((SteamPlayer user) => user.LastLogoff, lastLogOff.ToTimestamp()), null, default(CancellationToken));
        }

        public static void SetLastLogOn(uint accountId, ulong lastLogOn)
        {
            DbUser.Collection.FindOneAndUpdate((SteamPlayer user) => user.AccountID == accountId, DbUser.Ub.Set<ulong>((SteamPlayer user) => user.LastLogon, lastLogOn), null, default(CancellationToken));
        }

        public static bool IsValidPersonaName(string AccountName)
        {
            if (DbUser.Collection.CountDocuments((SteamPlayer usr) => usr.AccountName.ToLower() == AccountName.ToLower(), null, default(CancellationToken)) != 0L)
            {
                return false;
            }
            return true;
        }

        public static SteamPlayer Get(ulong SteamID)
        {
            return DbUser.Collection.Find((SteamPlayer usr) => usr.SteamID == SteamID, null).FirstOrDefault(default(CancellationToken));
        }

        public static SteamPlayer Get(uint accountID)
        {
            return DbUser.Collection.Find((SteamPlayer usr) => usr.AccountID == accountID, null).FirstOrDefault(default(CancellationToken));
        }

        public static SteamPlayer GetByAccountName(string AccountName)
        {
            return DbUser.Collection.Find((SteamPlayer usr) => usr.AccountName == AccountName, null).FirstOrDefault(default(CancellationToken));
        }

        public static SteamPlayer GetByPersonaName(string AccountName)
        {
            return DbUser.Collection.Find((SteamPlayer usr) => usr.AccountName == AccountName, null).FirstOrDefault(default(CancellationToken));
        }
    }
}

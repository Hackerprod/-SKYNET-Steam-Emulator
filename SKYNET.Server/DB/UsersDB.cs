using SKYNET.Steamworks;
using SKYNET.Types;
using SQLite;
using System;
using System.Collections.Generic;

namespace SKYNET.DB
{
    public class UsersDB
    {
        private SQLiteAsyncConnection DB;
        public List<SteamPlayer> Users;

        public UsersDB(SQLiteAsyncConnection db)
        {
            DB = db;

            DB.CreateTableAsync<SteamPlayer>();

            DB.CreateIndexAsync("SteamPlayer", "AccountID");
            DB.CreateIndexAsync("SteamPlayer", "SteamID");
            DB.CreateIndexAsync("SteamPlayer", "AccountName");

            Reload();
        }

        public async void Reload()
        {
            Users = await DB.Table<SteamPlayer>().ToListAsync();
        }



        public void CreateAccount(SteamPlayer user)
        {
            Users.Sort((u1, u2) => u1.AccountID.CompareTo(u2.AccountID));
            uint AccountID = Users.Count == 0 ? 1000 : Users[Users.Count - 1].AccountID + 1;
            user.AccountID = AccountID;
            DB.InsertAsync(user);
            Reload();
        }

        public void SetPlayingState(uint accountID, bool playing)
        {
            var User = Get(accountID);
            if (User == null) return;

            User.IsPlaying = playing;

            if (playing)
            {
                User.LastPlayedTime = (uint)DateTime.Now.ToTimestamp();
            }
            DB.UpdateAsync(User);
        }

        public void SetPlayingState(SteamPlayer User, bool playing)
        {
            if (User == null) return;

            User.IsPlaying = playing;

            if (playing)
            {
                User.LastPlayedTime = (uint)DateTime.Now.ToTimestamp();
            }
            DB.UpdateAsync(User);
        }

        internal void SetLastLogOn(uint accountID, uint setLastLogOn)
        {
            var User = Get(accountID);
            if (User == null) return;

            User.LastLogon = setLastLogOn;

            DB.UpdateAsync(User);
        }

        internal bool ValidPersonaName(string personaName)
        {
            return Users.Find(u => u.PersonaName == personaName) == null;
        }

        public SteamPlayer Get(ulong steamId)
        {
            return Users.Find(u => ((ulong)new CSteamID(u.AccountID, EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeIndividual)) == steamId);
        }

        public SteamPlayer Get(uint accountID)
        {
            return Users.Find(u => u.AccountID == accountID);
        }

        internal void SetLastLogoff(uint accountID, uint lastLogoff)
        {
            var User = Get(accountID);
            if (User == null) return;

            User.LastLogoff = lastLogoff;
            DB.UpdateAsync(User);
        }
    }
}

using SKYNET.Steamworks;
using SKYNET.Types;
using SQLite;
using System;
using System.Collections.Generic;

namespace SKYNET.DB
{
    public class UsersDB
    {
        private SQLiteDatabase DB;
        public List<SteamPlayer> Users;

        public UsersDB(SQLiteDatabase db)
        {
            DB = db;

            DB.CreateTable<SteamPlayer>();

            DB.DBConnection.CreateIndex("SteamPlayer", "AccountID");
            DB.DBConnection.CreateIndex("SteamPlayer", "SteamID");
            DB.DBConnection.CreateIndex("SteamPlayer", "AccountName");

            Users = DB.GetTables<SteamPlayer>();
        }

        public void Reload()
        {
            Users = DB.GetTables<SteamPlayer>();
        }



        public void CreateAccount(SteamPlayer user)
        {
            Users.Sort((u1, u2) => u1.AccountID.CompareTo(u2.AccountID));
            uint AccountID = Users.Count == 0 ? 1000 : Users[Users.Count - 1].AccountID + 1;
            user.AccountID = AccountID;
            DB.Insert(user);
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
            DB.Update(User);
        }

        public void SetPlayingState(SteamPlayer User, bool playing)
        {
            if (User == null) return;

            User.IsPlaying = playing;

            if (playing)
            {
                User.LastPlayedTime = (uint)DateTime.Now.ToTimestamp();
            }
            DB.Update(User);
        }

        internal void SetLastLogOn(uint accountID, uint setLastLogOn)
        {
            var User = Get(accountID);
            if (User == null) return;

            User.LastLogon = setLastLogOn;

            DB.Update(User);
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
            DB.Update(User);
        }
    }
}

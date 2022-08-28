using SQLite;
using System;
using System.Collections.Generic;

namespace SKYNET.Types
{
    public class SteamPlayer
    {
        [PrimaryKey]
        public uint AccountID { get; set; }
        public string AccountName { get; set; }
        public string PersonaName { get; set; }
        public string Password { get; set; }
        public double Wallet { get; set; }
        public Dictionary<string, string> RichPresence { get; set; }
        public bool IsPlaying { get; set; }
        public uint LastPlayedTime { get; set; }
        public uint LastLogon { get; set; }
        public uint LastLogoff { get; set; }

        public SteamPlayer()
        {
            RichPresence = new Dictionary<string, string>();
        }

        public static SteamPlayer CreateOne(string accountName, string password)
        {
            var player = new SteamPlayer()
            {
                AccountName = accountName,
                PersonaName = accountName,
                Password = password,
                RichPresence = new Dictionary<string, string>(),
                Wallet = 0,
            };
            return player;
        }
    }
}

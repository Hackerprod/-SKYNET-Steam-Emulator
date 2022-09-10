using System.Collections.Generic;

namespace SKYNET.Types
{
    public class SteamPlayer
    {
        public ulong SteamID { get; set; }
        public uint AccountID { get; set; }
        public string AccountName { get; set; }
        public string PersonaName { get; set; }
        public string Password { get; set; }
        public double Wallet { get; set; }
        public uint PlayingAppID { get; set; }
        public uint LastPlayedTime { get; set; }
        public uint LastLogon { get; set; }
        public uint LastLogoff { get; set; }
        public List<ulong> Friends { get; set; }

        public bool IsPlaying => PlayingAppID != 0;

        public SteamPlayer()
        {
            Friends = new List<ulong>();
        }

        public static SteamPlayer CreateOne(string accountName, string password)
        {
            var player = new SteamPlayer()
            {
                AccountName = accountName,
                PersonaName = accountName,
                Password = password,
                Friends = new List<ulong>(),
                Wallet = 0,
                PlayingAppID = 0,
            };
            return player;
        }
    }
}

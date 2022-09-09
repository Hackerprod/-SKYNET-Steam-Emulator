using System.Collections.Generic;

namespace SKYNET.Types
{
    public class SteamPlayer
    {
        public uint SteamID { get; set; }
        public uint AccountID { get; set; }
        public string AccountName { get; set; }
        public string PersonaName { get; set; }
        public string Password { get; set; }
        public double Wallet { get; set; }
        public bool IsPlaying { get; set; }
        public uint LastPlayedTime { get; set; }
        public uint LastLogon { get; set; }
        public uint LastLogoff { get; set; }

        public static SteamPlayer CreateOne(string accountName, string password)
        {
            var player = new SteamPlayer()
            {
                AccountName = accountName,
                PersonaName = accountName,
                Password = password,
                Wallet = 0,
            };
            return player;
        }
    }
}

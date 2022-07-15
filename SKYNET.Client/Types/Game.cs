using System.Collections.Generic;

namespace SKYNET.Types
{
    public class Game 
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string AvatarHex { get; set; }
        public string ExecutablePath { get; set; }
        public uint AppID { get; set; }
        public string Parameters { get; set; }
        public List<DLC> GameDLC { get; set; }
        public bool LogToFile { get; set; }
        public bool LogToConsole { get; set; }
        public bool RunCallbacks { get; set; }
        public bool ISteamHTTP { get; set; }
        public bool LaunchWithoutEmu { get; set; }
        public bool GameOverlay { get; set; }
        public bool CSteamworks { get; set; }

        public Game()
        {
            GameDLC = new List<DLC>();
        }

        public class DLC
        {
            public string Name { get; set; }
            public uint AppId { get; set; }
        }
    }
}
using System;
using System.Collections.Generic;

namespace SKYNET
{
    [Serializable]
    public class Game : MarshalByRefObject
    {
        public string Name { get; set; }
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
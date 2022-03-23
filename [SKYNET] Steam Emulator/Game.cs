﻿using System;
using System.Collections.Generic;

namespace SKYNET
{
    [Serializable]
    public class Game
    {
        public string Name { get; set; }
        public string ExecutablePath { get; set; }
        public string SteamApiPath { get; set; }
        public uint AppId { get; set; }
        public string Parameters { get; set; }
        public List<DLC> GameDLC { get; set; }
        public bool LaunchWithoutEmu { get; set; }

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
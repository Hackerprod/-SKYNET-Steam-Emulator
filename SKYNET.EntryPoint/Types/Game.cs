using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace SKYNET.Types
{
    [Serializable]
    public class Game : MarshalByRefObject
    {
        public string Name { get; set; }
        public string ExecutablePath { get; set; }
        public string SteamApiPath { get; set; }
        public uint AppId { get; set; }
        public string Parameters { get; set; }
        public List<DLC> GameDLC { get; set; }
        public bool LaunchWithoutEmu { get; set; }
        public bool SaveLog { get; set; }

        public Game()
        {
            GameDLC = new List<DLC>();
        }
        public class DLC
        {
            public string Name { get; set; }
            public uint AppId { get; set; }
        }

        public static string Serialize(Game serializedGame)
        {
            return new JavaScriptSerializer().Serialize(serializedGame);
        }

        public static Game Deserialize(string serializedGame)
        {
            return new JavaScriptSerializer().Deserialize<Game>(serializedGame);
        }
    }
}
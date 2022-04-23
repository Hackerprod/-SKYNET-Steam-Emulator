using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Types
{
    public class GameServerData
    {
        public GameServerData()
        {
            KeyValues = new ConcurrentDictionary<string, string>();
        }

        public uint IP { get; set; }
        public int Port { get; set; }
        public int QueryPort { get; set; }
        public uint Flags { get; set; }
        public uint AppId { get; set; }
        public string VersionString { get; set; }
        public string Product { get; set; }
        public string Description { get; set; }
        public string ModDir { get; set; }
        public bool Dedicated { get; set; }
        public int MaxPlayers { get; set; }
        public int BotPlayers { get; set; }
        public string ServerName { get; set; }
        public string MapName { get; set; }
        public bool PasswordProtected { get; set; }
        public uint SpectatorPort { get; set; }
        public string SpectatorServerName { get; set; }
        public string GameTags { get; set; }
        public string GameData { get; set; }
        public string Region { get; set; }
        public ConcurrentDictionary<string, string> KeyValues { get; set; }
    }
}

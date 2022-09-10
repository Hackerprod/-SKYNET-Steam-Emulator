using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Types
{
    public class Leaderboard
    {
        public ulong SteamID { get; set; }
        public uint AppID { get; set; }
        public string Name { get; set; }
        public int ShortMethod { get; set; }
        public int DisplayType { get; set; }
        public int SteamLeaderboard { get; set; }
    }

}

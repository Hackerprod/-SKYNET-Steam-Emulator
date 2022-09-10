using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Types
{
    public class PlayerStat
    {
        public ulong SteamID { get; set; }
        public uint AppID { get; set; }
        public string Name { get; set; }
        public uint Data { get; set; }
    }

}

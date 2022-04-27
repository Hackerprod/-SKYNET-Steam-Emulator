using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Types
{
    public class SteamUser
    {
        public uint AccountId { get; set; }
        public ulong SteamId { get; set; }
        public string PersonaName { get; set; }
        public uint GameId { get; set; }
        public bool HasFriend { get; set; }
    }
}

using System.Collections.Generic;

namespace SKYNET.Types
{
    public class SteamPlayer
    {
        public uint AccountID { get; set; }
        public ulong SteamID { get; set; }
        public string PersonaName { get; set; }
        public uint GameID { get; set; }
        public ulong LobbyID { get; set; }
        public bool HasFriend { get; set; }
        public string IPAddress { get; set; }
        public List<RichPresence> RichPresence { get; set; }
        public SteamPlayer()
        {
            RichPresence = new List<RichPresence>();
        }
    }
}

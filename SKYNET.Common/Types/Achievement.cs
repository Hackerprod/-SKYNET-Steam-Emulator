using System;

namespace SKYNET.Types
{
    public class Achievement
    {
        public ulong SteamID { get; set; }
        public uint AppID { get; set; }
        public string Name { get; set; }
        public bool Earned { get; set; }
        public DateTime Date { get; set; }
        public uint Progress { get; set; }
        public uint MaxProgress { get; set; }
    }
}

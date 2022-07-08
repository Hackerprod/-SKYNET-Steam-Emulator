using System;

namespace SKYNET.Types
{
    public class Achievement
    {
        public string Name { get; set; }
        public bool Earned { get; set; }
        public DateTime Date { get; set; }
        public uint Progress { get; set; }
        public uint MaxProgress { get; set; }
    }
}

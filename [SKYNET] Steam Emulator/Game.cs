using System;

namespace SKYNET
{
    [Serializable]
    public class Game
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public uint AppId { get; set; }
        public string Parametters { get; set; }
    }
}
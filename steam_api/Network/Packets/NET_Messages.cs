using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Network.Packets
{
    public class NET_Base
    {

    }

    public class NET_Announce : NET_Base
    {
        public string PersonaName { get; set; }
        public uint AccountID { get; set; }
        public uint AppID { get; set; }
    }

    public enum MessageType : int
    {
        NET_Announce,
        NET_Avatar
    }
}

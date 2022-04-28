using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Network.Packets
{
    public class NetworkMessage
    {
        public int MessageType { get; set; }
        public string ParsedBody { get; set; }

    }
}

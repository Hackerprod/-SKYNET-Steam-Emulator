using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Network
{
    public class BroadcastPacket
    {
        public IPEndPoint RemoteEndPoint;
        public byte[] Data;
    }
}

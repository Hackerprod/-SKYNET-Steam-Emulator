using SKYNET.Network;
using SKYNET.Network.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interfaces
{
    public interface IConnection
    {
        bool Connected { get; }
        IPEndPoint RemoteEndPoint { get; }
        //void Send(IClientGCMsg msg, ulong steamId);
        void Send(NETMessage msg);
        void Disconnect();
    }
}

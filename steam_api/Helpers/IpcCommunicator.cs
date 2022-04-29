using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Helpers
{
    public class IpcCommunicator : MarshalByRefObject
    {
        public event EventHandler<object> OnMessage;
    }
}

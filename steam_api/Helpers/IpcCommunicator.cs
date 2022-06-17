using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Helper
{
    public class IpcCommunicator : MarshalByRefObject
    {
        public event EventHandler<object> OnMessage;
        public int ProcessID;

        public void InvokeMessage(object msg)
        {
            OnMessage?.Invoke(this, msg);
        }

        public void Ping(string v)
        {

        }
    }
}

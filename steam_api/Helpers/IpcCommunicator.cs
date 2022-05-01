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

        public void InvokeMessage(object msg)
        {
            OnMessage?.Invoke(this, msg);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Types
{
    public class MsgBase 
    {

    }

    public class MsgClientHello : MsgBase
    {
        public string UserName;
        public string Password;
    }

    public class MsgClientWelcome : MsgBase
    {
        public bool Success;
        public string ErrorMessage;
    }
}

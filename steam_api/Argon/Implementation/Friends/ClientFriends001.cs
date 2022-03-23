using System;

using System.Runtime.InteropServices;

using Core.Interface;
using SKYNET;

namespace InterfaceFriends
{
    [Impl(Name = "CLIENTFRIENDS_INTERFACE_VERSION001", ServerMapped = true)]
    class ClientFriends001 : IBaseInterface
    {
        public string GetPersonaName()
        {
            Write("GetPersonaName");
            //f.GetLocalName();
            return "";
        }

        public void SetPersonaName(string name)
        {
            Write("SetPersonaName");
        }

        public int SetPersonaNameEx(string name, bool send_cb)
        {
            Write("SetPersonaNameEx");
            return 1;
        }


        public bool IsPersonaNameSet()
        {
            Write("IsPersonaNameSet");
            return true;
        }


        public int GetPersonaState()
        {
            Write("GetPersonaState");
            return 1;
        }

        public void SetPersonaState(int state)
        {
            Write("SetPersonaState");
        }

        private void Write(string v)
        {
            Main.Write(InterfaceVersion, v);
        }
    }
}
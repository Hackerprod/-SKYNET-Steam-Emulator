using System;

using System.Runtime.InteropServices;

using Core.Interface;

namespace InterfaceFriends
{
    [Impl(Name = "CLIENTFRIENDS_INTERFACE_VERSION001", ServerMapped = true)]
    class ClientFriends001 : IBaseInterface
    {
        public string GetPersonaName()
        {
            Write("ClientFriends001", "GetPersonaName");
            //f.GetLocalName();
            return "";
        }

        public void SetPersonaName(string name)
        {
            Write("ClientFriends001", "SetPersonaName");
        }

        public int SetPersonaNameEx(string name, bool send_cb)
        {
            Write("ClientFriends001", "SetPersonaNameEx");
            return 1;
        }


        public bool IsPersonaNameSet()
        {
            Write("ClientFriends001", "IsPersonaNameSet");
            return true;
        }


        public int GetPersonaState()
        {
            Write("ClientFriends001", "GetPersonaState");
            return 1;
        }

        public void SetPersonaState(int state)
        {
            Write("ClientFriends001", "SetPersonaState");
        }

    }
}
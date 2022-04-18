using SKYNET;
using SKYNET.Callback;
using System;
using System.Collections.Generic;
using System.Text;

namespace SKYNET
{
    public class ISteamInterface
    {
        public string InterfaceVersion { get; set; }

        public void PostCallback(object data, CallbackType callback_id, Callback.Buffer b)
        {
            CallbackHandler.PostCallback(1, 1, (int)callback_id, b);
        }

        public void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }
    }
}

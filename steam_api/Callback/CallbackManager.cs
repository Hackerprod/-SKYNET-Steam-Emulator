using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steamworks;

namespace SKYNET.Callback
{
    public class CallbackManager
    {
        List<IntPtr> Callbacks;
        public CallbackManager()
        {
            Callbacks = new List<IntPtr>();
        }
        public void RegisterCallback(IntPtr pCallback, int iCallback)
        {
            SteamAPICall_t t = new SteamAPICall_t((ulong)iCallback);
            Callbacks.Add(pCallback);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Steamworks;

namespace SKYNET.Managers
{
    public class CallbackManager
    {
        //private Dictionary<int, CCallbackBase> Callbacks { get; set; } = new Dictionary<int, CCallbackBase>();

        public void RegisterCallback(IntPtr pCallback, int iCallback)
        {
           // SteamAPICall_t t = new SteamAPICall_t((ulong)iCallback);
            //Callbacks.Add(iCallback, pCallback);
        }

        public void UnregisterCallResult(IntPtr pCallback, SteamAPICall_t hAPICall)
        {

        }

        public void UnregisterCallback(IntPtr pCallback)
        {

        }

        public void RegisterCallResult(CCallbackBase pCallback, SteamAPICall_t hAPICall)
        {

        }

        public void RunCallbacks()
        {
            //for (auto & c : callbacks)
            //{
            //    c.second.results.clear();
            //}
        }

        public void FreeCallback(int pipe_id)
        {
            //bool found = Callbacks.TryGetValue(pipe_id, out CCallbackBase value);

            //if (found)
            //{
            //    IntPtr ptr = IntPtr.Zero;
            //    Marshal.StructureToPtr<CCallbackBase>(value, ptr, true);
            //    Marshal.FreeHGlobal(ptr);
            //    Callbacks.Remove(pipe_id);
            //}
        }
    }
}

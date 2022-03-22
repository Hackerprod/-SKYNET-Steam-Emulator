using Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate(Name = "SteamGameServer")]
    public class DSteamGameServer 
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetHSteamUser(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetHSteamPipe(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void RunCallbacks(IntPtr _);
    }
}

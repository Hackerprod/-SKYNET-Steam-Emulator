using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate("SteamVideo")]
    public class DSteamVideo
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void GetVideoURL(IntPtr unVideoAppID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsBroadcasting(int pnNumViewers);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void GetOPFSettings(IntPtr unVideoAppID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetOPFStringForApp(IntPtr unVideoAppID, string pchBuffer, uint pnBufferSize);

    }
}

using Core.Interface;
using SKYNET.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate(Name = "SteamRemotePlay")]
    public class DSteamRemotePlay 
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetSessionCount();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetSessionID(int iSessionIndex);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetSessionSteamID(uint unSessionID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetSessionClientName(uint unSessionID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ESteamDeviceFormFactor GetSessionClientFormFactor(uint unSessionID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BGetSessionClientResolution(uint unSessionID, int pnResolutionX, int pnResolutionY);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BSendRemotePlayTogetherInvite(IntPtr steamIDFriend);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Callback
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void RunCallbackDelegate(IntPtr _, IntPtr pvParam);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void RunCallResultDelegate(IntPtr _, IntPtr pvParam, [MarshalAs(UnmanagedType.I1)] bool bIOFailure, ulong hSteamAPICall);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int GetCallbackSizeBytesDelegate(IntPtr _);
}

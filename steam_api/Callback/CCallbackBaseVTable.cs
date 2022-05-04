using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Callback
{
    [StructLayout(LayoutKind.Sequential)]
    internal class CCallbackBaseVTable
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RunCBDel(IntPtr pvParam);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RunCRDel(IntPtr pvParam, [MarshalAs(UnmanagedType.I1)] bool bIOFailure, ulong hSteamAPICall);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int GetCallbackSizeBytesDel(IntPtr thisptr);

        private const CallingConvention cc = CallingConvention.Cdecl;

        [NonSerialized]
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public RunCRDel m_RunCallResult;

        [NonSerialized]
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public RunCBDel m_RunCallback;

        [NonSerialized]
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public GetCallbackSizeBytesDel m_GetCallbackSizeBytes;
    }

}

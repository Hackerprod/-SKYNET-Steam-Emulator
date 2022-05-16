using SKYNET;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public class ContextInitData
    {
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public pFnDelegate pFn;
    }

    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    public delegate void pFnDelegate(IntPtr ctx);

}

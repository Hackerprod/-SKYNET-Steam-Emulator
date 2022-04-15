using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Callback
{
    public interface ICallbackData
    {
        CallbackType CallbackType { get; }

        int DataSize { get; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct SetPersonaNameResponse_t : ICallbackData
    {
        [MarshalAs(UnmanagedType.I1)]
        internal bool Success;

        [MarshalAs(UnmanagedType.I1)]
        internal bool LocalSuccess;

        internal Result Result;

        public static int _datasize = Marshal.SizeOf(typeof(SetPersonaNameResponse_t));

        public int DataSize => _datasize;

        public CallbackType CallbackType => CallbackType.k_iSetPersonaNameResponse;
    }
}

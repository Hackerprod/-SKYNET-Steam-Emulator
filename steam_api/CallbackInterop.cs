using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TEST
{
    [StructLayout(LayoutKind.Sequential)]
    public class CCallbackBase
    {
        public byte m_nCallbackFlags;
        public int m_iCallback;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CCallResult //: CCallbackBase
    {
        public ulong m_hAPICall;
        public IntPtr m_pObj; // T *
        public IntPtr m_Func;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CCallback //: CCallbackBase
    {
        public IntPtr m_pObj; // T *
        public IntPtr m_Func;
    }
}

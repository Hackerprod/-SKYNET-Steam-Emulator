using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Callback
{
    [StructLayout(LayoutKind.Sequential)]
    public class CCallResult
    {
        public RunCallResultDelegate m_RunCallResult;
        public RunCallbackDelegate m_RunCallback;
        public GetCallbackSizeBytesDelegate m_GetCallbackSizeBytes;

        //public RunCallbackDelegate m_RunCallback;
        //public RunCallResultDelegate m_RunCallResult;
        //public GetCallbackSizeBytesDelegate m_GetCallbackSizeBytes;
    }
}

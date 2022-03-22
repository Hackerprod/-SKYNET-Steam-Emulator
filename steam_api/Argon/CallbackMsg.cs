using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Core.IPC
{

    [Serializable]
    public class InternalCallbackMsg
    {
        public int user_id;
        public uint callback_id;
        public byte[] data;
    }

    /// <summary>
    /// Similar to CallbackMsg_t from steamclient
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CallbackMsg
    {
        public int user_id;
        public uint callback_id;
        public IntPtr data;
        public int data_size;
    }
}

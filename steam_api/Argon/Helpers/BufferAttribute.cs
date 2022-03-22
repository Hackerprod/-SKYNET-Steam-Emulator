using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interface
{
    /// <summary>
    /// Used to signal to <see cref="Loader"/> that this function contains a buffer that needs to be mapped
    /// across from client to server and visa versa
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class BufferAttribute : Attribute
    {
        // Index of the buffer
        public int Index { get; set; }

        // Where the new buffer pointer should be placed
        public int NewPointerIndex { get; set; }

        // Where the new buffer size should be placed
        public int NewSizeIndex { get; set; }

    }
}

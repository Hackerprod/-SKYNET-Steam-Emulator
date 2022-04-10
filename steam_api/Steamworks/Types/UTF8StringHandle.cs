using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Steamworks.Types
{
    public class UTF8StringHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public UTF8StringHandle(string str) : base(ownsHandle: true)
        {
            if (str == null)
            {
                SetHandle(IntPtr.Zero);
                return;
            }
            byte[] array = new byte[Encoding.UTF8.GetByteCount(str) + 1];
            Encoding.UTF8.GetBytes(str, 0, str.Length, array, 0);
            IntPtr destination = Marshal.AllocHGlobal(array.Length);
            Marshal.Copy(array, 0, destination, array.Length);
            SetHandle(destination);
        }

        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                Marshal.FreeHGlobal(handle);
            }
            return true;
        }
    }
}

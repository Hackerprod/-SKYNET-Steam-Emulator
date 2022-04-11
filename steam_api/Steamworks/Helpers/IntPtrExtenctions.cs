using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Steamworks.Helpers
{
    public static class IntPtrExtenctions
    {
        public static AppId_t GetAppId_t(this IntPtr instance)
        {
            AppId_t result = default;
            try
            {
                result = Marshal.PtrToStructure<AppId_t>(instance);
            }
            catch (Exception)
            {
                result = new AppId_t();
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET
{
    public class DllInjector
    {
        [DllImport("ManualMapInjector.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static int _InjectFileHex(IntPtr szDll, int pId, int flags);

        public static Process Inject(string executablePath, string parameters, string x64Dll, string x86Dll)
        {
            var process  = Process.Start(executablePath, parameters);
            var nProcess = NativeSharp.NativeProcess.Open((uint)process.Id);
            var dllInjector = "";
            var dllPath = "";
            if (nProcess.Is64Bit)
            {
                dllInjector = Path.Combine(modCommon.GetPath(), "ncloaderx64.exe");
                dllPath = x64Dll;
            }
            else
            {
                dllInjector = Path.Combine(modCommon.GetPath(), "ncloaderx86.exe");
                dllPath = x86Dll;
            }
            var injector = Process.Start(dllInjector, $"{process.Id}" + "\"" + dllPath + "\"");

            return process;
        }
    }
}

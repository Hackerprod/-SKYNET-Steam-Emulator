using EasyHook;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Hook.Handles
{
    public class LdrLoadDll : IHook
    {
        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate uint LdrLoadDllDelegate(IntPtr pathToFile, IntPtr flags, IntPtr moduleFileName, IntPtr moduleHandle);

        private LdrLoadDllDelegate _LdrLoadDll;

        public override string Library => "ntdll.dll";

        public override string Method => "LdrLoadDll";

        public override LocalHook Hook { get; set; }

        public override System.Delegate Delegate
        {
            get
            {
                _LdrLoadDll = Marshal.GetDelegateForFunctionPointer<LdrLoadDllDelegate>(base.ProcAddress);
                return new LdrLoadDllDelegate(Callback);
            }
        }

        private uint Callback(IntPtr pathToFile, IntPtr flags, IntPtr moduleFileName, IntPtr moduleHandle)
        {
            uint result = _LdrLoadDll(pathToFile, flags, moduleFileName, moduleHandle);
            try
            {
                string unicodeString = GetUnicodeString(moduleFileName);
                unicodeString = Path.GetFileNameWithoutExtension(unicodeString).ToLower();
                Main.Write(unicodeString);
                Main.HookManager.Install(unicodeString.ToUpper());
                return result;
            }
            catch
            {
                return result;
            }
        }
        public static string GetUnicodeString(IntPtr ptr)
        {
            return Marshal.PtrToStructure<STRING>(ptr).Content;
        }
        internal struct STRING
        {
            public ushort Length;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string Content;
        }
    }
}

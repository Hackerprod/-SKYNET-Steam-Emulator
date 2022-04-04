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
    public class steamnetworkingsockets : IHook
    {
        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate bool LdrLoadDllDelegate();

        private LdrLoadDllDelegate _LdrLoadDll;

        public override string Library => "steamnetworkingsockets.dll";

        public override string Method => "SteamDatagramClient_Init_InternalV9";

        public override LocalHook Hook { get; set; }

        public override System.Delegate Delegate
        {
            get
            {
                _LdrLoadDll = Marshal.GetDelegateForFunctionPointer<LdrLoadDllDelegate>(base.ProcAddress);
                return new LdrLoadDllDelegate(Callback);
            }
        }

        private bool Callback()
        {
            Main.Write("SteamDatagramClient_Init");
            return true;
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

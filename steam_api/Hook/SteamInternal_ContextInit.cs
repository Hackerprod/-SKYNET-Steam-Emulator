using EasyHook;
using SKYNET.Helper;
using SKYNET.Types;
using Steamworks.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKYNET.Hook
{
    public class SteamInternal_ContextInit : IHook
    {
        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate IntPtr SteamInternal_ContextInitDelegate(IntPtr pContextInitData);

        private SteamInternal_ContextInitDelegate _SteamInternal_ContextInit;

        public override string Library => "steam_api64.dll";

        public override string Method => "SteamInternal_ContextInit";

        public override LocalHook Hook { get; set; }

        public override System.Delegate Delegate
        {
            get
            {
                new SKYNET.Hook.SteamAPI();
                _SteamInternal_ContextInit = Marshal.GetDelegateForFunctionPointer<SteamInternal_ContextInitDelegate>(base.ProcAddress);
                return new SteamInternal_ContextInitDelegate(Callback);
            }
        }

        private IntPtr Callback(IntPtr pContextInitData)
        {

            return MemoryHelper.MemoryAddress(SteamEmulator.Context);
        }

        public unsafe struct ContextInitData
        {
            public IntPtr Context;
            public uint counter;
        }
    }
}

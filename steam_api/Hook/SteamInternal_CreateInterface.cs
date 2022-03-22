using EasyHook;
using SKYNET.Types;
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
    public class SteamInternal_CreateInterface : IHook
    {
        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate IntPtr SteamInternal_CreateInterfaceDelegate([MarshalAs(UnmanagedType.LPStr)] string version);

        private SteamInternal_CreateInterfaceDelegate _SteamInternal_CreateInterface;

        public override string Library => "steam_api64.dll";

        public override string Method => "SteamInternal_CreateInterface";

        public override LocalHook Hook { get; set; }

        public override System.Delegate Delegate
        {
            get
            {
                _SteamInternal_CreateInterface = Marshal.GetDelegateForFunctionPointer<SteamInternal_CreateInterfaceDelegate>(base.ProcAddress);
                return new SteamInternal_CreateInterfaceDelegate(Callback);
            }
        }

        private IntPtr Callback([MarshalAs(UnmanagedType.LPStr)] string version)
        {
            Main.Write($"SteamInternal_CreateInterface {version}");
            return InterfaceManager.CreateInterface(version);
        }
    }
}

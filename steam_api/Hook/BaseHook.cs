using EasyHook;
using SKYNET.Helper;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Hook
{
    public abstract class BaseHook
    {
        public abstract bool Installed { get; set; }
        public string Library => "steam_api64.dll";

        internal void Install<T>(string Method, T Delegate1, T Delegate2) where T : System.Delegate
        {
            try
            {
                var ProcAddress = NativeMethods.GetProcAddress("steam_api64.dll", Method);
                if (ProcAddress == IntPtr.Zero)
                {
                    Main.Write("HookManager", "Method " + Method + " not found");
                    return;
                }
                try
                {
                    var Hook = LocalHook.Create(ProcAddress, Delegate2, Main.Instance);

                    Hook.ThreadACL.SetExclusiveACL(new int[0]);

                    Delegate1 = Marshal.GetDelegateForFunctionPointer<T>(ProcAddress);
                }
                catch
                {
                    Main.Write("HookManager", "Failed injecting " + Method + " in steam_api64.dll");
                }
            }
            catch
            {
            }
        }
        public abstract void Install();

        public abstract void Write(object v);
    }
}
using EasyHook;
using SKYNET.Helpers;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SKYNET.Hook
{
    public abstract class BaseHook
    {
        public abstract bool Installed { get; set; }
        public string Library => SteamEmulator.SteamApiPath;

        internal void Install<T>(string Method, T Delegate1, T Delegate2) where T : System.Delegate
        {
            try
            {
                var ProcAddress = NativeMethods.GetProcAddress(Library, Method);
                if (ProcAddress == IntPtr.Zero)
                {
                    //Main.Write("HookManager", "Method " + Method + " not found");
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
                    Main.Write("HookManager", $"Failed injecting {Method} in {Path.GetFileName(Library)}");
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
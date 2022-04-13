using EasyHook;
using SKYNET.Helpers;
using SKYNET.Managers;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SKYNET.Hook
{
    public abstract class BaseHook
    {
        public abstract bool Installed { get; set; }
        public string Library => SteamEmulator.SteamApiPath;

        internal void Install<T>(string Method, T Delegate1, T Delegate2, BaseHook Instance = null) where T : System.Delegate
        {
            try
            {
                var ProcAddress = NativeMethods.GetProcAddress(Library, Method);
                if (ProcAddress == IntPtr.Zero)
                {
                    //Main.Write("HookManager", "Method " + Method + " not found, Trying to create in memory");
                    //MethodInfo info = MemoryManager.GetMethodInfo(Method);
                    //if (info == null)
                    //{
                    //    Main.Write("HookManager", "Not found Method " + Method + " in Assembly");
                    //    retuGetFileSize
                    // return;
                    //}
                    //ProcAddress = MemoryManager.CreateMethod(Instance, info);
                    //if (ProcAddress == IntPtr.Zero)
                    //{
                    //    Main.Write("HookManager", "Error creating method " + Method + " in memory");
                    return;
                    //}
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
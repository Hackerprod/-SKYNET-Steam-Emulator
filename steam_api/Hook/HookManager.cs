using EasyHook;
using SKYNET.Helper;
using SKYNET.Hook;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SKYNET
{
    public class HookManager
    {
        public List<BaseHook> Hooks;

        public List<IHook> PluginHooks;

        public HookManager()
        {
            Hooks = new List<BaseHook>();
            PluginHooks = new List<IHook>();

            InstallHook(new LdrLoadDll());
            //InstallHook(new SteamInternal_ContextInit());

            Hooks.Add(new SKYNET.Hook.SteamInternal());
            //Hooks.Add(new SKYNET.Hook.SteamAPI());

            if (!Modules.Contains("steam_api.dll") || !Modules.Contains("steam_api64.dll"))
            {
                Main.ForceSteamAPILoad();
            }
        }

        public List<string> Modules
        {
            get
            {
                List<string> list = new List<string>();
                foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
                {
                    list.Add(module.ModuleName.ToUpper());
                }
                return list;
            }
        }

        internal void Install()
        {
            foreach (var hook in Hooks)
            {
                if (!hook.Installed && Modules.Contains(hook.Library.ToUpper()))
                {
                    hook.Install();
                }
            }

        }
        public void Install(string UpperDllName)
        {
            if (!string.IsNullOrEmpty(UpperDllName))
            {
                var hooks = Hooks.FindAll((BaseHook h) => !h.Installed && Path.GetFileNameWithoutExtension(h.Library).ToUpper() == UpperDllName);
                if (hooks.Any())
                {
                    foreach (var hook in hooks)
                    {
                        hook.Install();
                    }
                }

                var ihooks = PluginHooks.FindAll((IHook h) => !h.Installed && Path.GetFileNameWithoutExtension(h.Library).ToUpper() == UpperDllName);
                if (ihooks.Any())
                {
                    foreach (var hook in ihooks)
                    {
                        InstallHook(hook);
                    }
                }
            }
        }
        private void InstallHook(IHook hook)
        {
            try
            {
                hook.ProcAddress = NativeMethods.GetProcAddress(hook.Library, hook.Method);
                if (hook.ProcAddress == IntPtr.Zero)
                {
                    hook.Installed = false;
                    return;
                }
                try
                {
                    hook.Hook = LocalHook.Create(hook.ProcAddress, hook.Delegate, Main.Instance);
                    hook.Hook.ThreadACL.SetExclusiveACL(new int[0]);
                    hook.Installed = true;
                }
                catch
                {
                    Main.Write("HookManager", "Failed injecting " + hook.Method + " in " + hook.Library);
                    hook.Installed = false;
                }
            }
            catch
            {
            }
        }
        public void UninstallHooks()
        {
            foreach (var hook in Hooks)
            {
                try
                {
                    //hook.Hook.Dispose();
                    hook.Installed = false;
                }
                catch
                {
                }
            }
            foreach (IHook trafficHook in PluginHooks)
            {
                try
                {
                    trafficHook.Hook.Dispose();
                    trafficHook.Installed = false;
                }
                catch
                {
                }
            }
        }
        public void Uninstall(string Method)
        {
            //if (!string.IsNullOrEmpty(Method))
            //{
            //    foreach (IHook hook in Hooks)
            //    {
            //        if (hook.Method == Method)
            //        {
            //            try
            //            {
            //                hook.Hook.Dispose();
            //                hook.Installed = false;
            //            }
            //            catch
            //            {
            //            }
            //        }
            //    }
            //    return;
            //}
            //foreach (IHook hook2 in Hooks)
            //{
            //    try
            //    {
            //        hook2.Hook.Dispose();
            //        hook2.Installed = false;
            //    }
            //    catch
            //    {
            //    }
            //}
        }
    }
}
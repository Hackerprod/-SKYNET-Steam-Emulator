using EasyHook;
using SKYNET.Helpers;
using SKYNET.Hook;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SKYNET.Manager
{
    public class HookManager
    {
        public List<BaseHook> Hooks;
        public List<IHook> PluginHooks;

        public HookManager()
        {
            Hooks = new List<BaseHook>();
            PluginHooks = new List<IHook>();

            InstallHook(new SKYNET.Hook.Handles.LdrLoadDll());
            InstallHook(new SKYNET.Hook.Handles.steamnetworkingsockets());
            

            Hooks.Add(new SKYNET.Hook.Handles.SteamInternal());
            Hooks.Add(new SKYNET.Hook.Handles.SteamAPI());
            //Hooks.Add(new SKYNET.Hook.Handles.SteamAPI_ISteamAppList()); 
            //Hooks.Add(new SKYNET.Hook.Handles.SteamAPI_ISteamApps());


            if (!Modules.Contains("steam_api.dll") && !Modules.Contains("steam_api64.dll"))
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
                    Main.Write("HookManager", $"Failed injecting {hook.Method} in {hook.Library}");
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
    }
}
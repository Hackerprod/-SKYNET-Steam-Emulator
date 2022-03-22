using EasyHook;
using SKYNET.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SKYNET
{
    public class Main : IEntryPoint
    {
        public static Main Instance;

        public static HookManager HookManager;

        //public static HookCallback HookCallback;

        public static HookInterface HookInterface;

        private string callbackChannel;

        //private List<IPlugin> Plugins;

        public Main(RemoteHooking.IContext inContext, string inChannelName)
        {
            callbackChannel = null;
            Instance = this;
            HookInterface = (HookInterface)Activator.GetObject(typeof(HookInterface), "ipc://" + inChannelName + "/" + inChannelName);

            //HookCallback = new HookCallback();
            //HookCallback.ReleaseHooks += HookCallback_ReleaseHooks;
            //HookCallback.ReleaseHook += HookCallback_ReleaseHook;
            //HookCallback.DumpToConsoleChanged += HookCallback_DumpToConsoleChanged;
            //HookCallback.DumpToFileChanged += HookCallback_DumpToFileChanged;
            //HookCallback.DnsRedirectionChanged += HookCallback_DnsRedirectionChanged;
            //HookCallback.IpRedirectionChanged += HookCallback_IpRedirectionChanged;
            //HookCallback.PortRedirectionChanged += HookCallback_PortRedirectionChanged;
            //RemoteHooking.IpcCreateServer(ref callbackChannel, WellKnownObjectMode.SingleCall, HookCallback);

            HookInterface.Ping(callbackChannel);
            Config.HelperLibraryLocation = Path.GetDirectoryName(HookInterface.DllPath);
            Config.DependencyPath = Path.GetDirectoryName(HookInterface.DllPath);
            HookManager = new HookManager();
            //Plugins = new List<IPlugin>();
        }

        internal static void ForceSteamAPILoad()
        {
            string dllPath = modCommon.GetPath();

            if (File.Exists(Path.Combine(dllPath, "steam_api.dll")))
            {
                dllPath = Path.Combine(dllPath, "steam_api.dll");
            }
            else if (File.Exists(Path.Combine(dllPath, "steam_api64.dll")))
            {
                dllPath = Path.Combine(dllPath, "steam_api64.dll");
            }
            else
            {
                Write("Error forcing steam_api load");
                return;
            }

            Memory.CreateInMemoryInterface(dllPath);
        }

        internal static void ModuleLoaded(string v)
        {
            //if (!Instance.Plugins.Any())
            //{
            //    return;
            //}
            //foreach (IPlugin plugin in Instance.Plugins)
            //{
            //    plugin.ModuleLoaded(v);
            //}
        }

        public void Run(RemoteHooking.IContext inContext, string inChannelName)
        {
            try
            {
                InstallPlugins();


                if (!SteamEmulator.Initialized)
                {
                    new SteamEmulator().Initialize();
                }

                HookManager.Install();
            }
            catch (Exception msg)
            {
                Write(msg);
            }
            RemoteHooking.WakeUpProcess();
            try
            {
                while (true)
                {
                    Thread.Sleep(7000);
                    HookInterface.Ping("");
                }
            }
            catch
            {
            }
            HookManager.UninstallHooks();
            LocalHook.Release();
        }

        private void InstallPlugins()
        {
            //string path = Path.GetDirectoryName(HookInterface.DllPath) + "/Plugins";
            //if (!Directory.Exists(path))
            //{
            //    return;
            //}
            //string[] files = Directory.GetFiles(path, "*.dll");
            //foreach (string path2 in files)
            //{
            //    try
            //    {
            //        Assembly assembly = Assembly.LoadFile(path2);
            //        Type type = assembly.GetType("SKYNET.Plugin");
            //        if (type != null)
            //        {
            //            IPlugin plugin = (IPlugin)Activator.CreateInstance(type);
            //            if (plugin == null)
            //            {
            //                Write("PLUGINS", "Failed to load plugin " + Path.GetFileNameWithoutExtension(path2), Color.Red);
            //                continue;
            //            }
            //            plugin.Initialize(this, HookInterface);
            //            HookManager.PluginHooks.AddRange(plugin.Hooks);
            //            Write("PLUGINS", "Loaded " + Path.GetFileNameWithoutExtension(path2), ColorTranslator.FromHtml("#27B8EF"));
            //            Plugins.Add(plugin);
            //        }
            //    }
            //    catch (Exception)
            //    {
            //        Write("PLUGINS", "Failed to load plugin " + Path.GetFileNameWithoutExtension(path2) + " \n", Color.Red);
            //    }
            //}
        }

        public static void InjectToProcess(uint ProcessId, string name)
        {
            try
            {
                RemoteHooking.Inject((int)ProcessId, HookInterface.InjectionOptions, HookInterface.DllPath, HookInterface.DllPath, HookInterface.ChannelName);
            }
            catch (Exception ex)
            {
                Write("NET REDIRECTOR", "Error injecting process in " + name + " " + Environment.NewLine + " " + new string(' ', 17) + ex.Message);
            }
        }

        public static void Write(object msg)
        {
            Write(Process.GetCurrentProcess().ProcessName.ToUpper(), msg);
                }
        public static void Write(object sender, object msg)
        {
            HookInterface.InvokeMessage(sender.ToString(), msg);
        }

        private void HookCallback_ReleaseHooks(object sender, EventArgs e)
        {
            HookManager.UninstallHooks();
        }

        private void HookCallback_ReleaseHook(object sender, string hook)
        {
            HookManager.Uninstall(hook);
        }

        private void HookCallback_DumpToFileChanged(object sender, bool DumpToFile)
        {
            HookInterface.DumpToFile = DumpToFile;
        }

    }
}

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

        public static Game Game;

        //private List<IPlugin> Plugins;

        public Main(RemoteHooking.IContext inContext, string inChannelName)
        {
            Write("xd");
            callbackChannel = null;
            Instance = this;
            HookInterface = (HookInterface)Activator.GetObject(typeof(HookInterface), "ipc://" + inChannelName + "/" + inChannelName);
            Game = HookInterface.Game;

            Config.HelperLibraryLocation = Path.GetDirectoryName(HookInterface.DllPath);
            Config.DependencyPath = Path.GetDirectoryName(HookInterface.DllPath);

            HookManager = new HookManager();
            HookInterface.Ping(callbackChannel);
        }

        internal static void ForceSteamAPILoad()
        {
            string dllPath = modCommon.GetPath();

            if (File.Exists(Game.SteamApiPath))
            {
                dllPath = Game.SteamApiPath;
            }
            else if (File.Exists(Path.Combine(dllPath, "steam_api.dll")))
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
            SteamEmulator.SteamApiPath = dllPath;

            Memory.CreateInMemoryInterface(dllPath);
        }

        public void Run(RemoteHooking.IContext inContext, string inChannelName)
        {
            try
            {
                if (!SteamEmulator.Initialized)
                {
                    new SteamEmulator(true).Initialize();
                    SteamEmulator.AppId = HookInterface.Game.AppId;
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

        public static void Write(object msg)
        {
            Write(Process.GetCurrentProcess().ProcessName.ToUpper(), msg);
        }

        public static void Write(object sender, object msg)
        {
            HookInterface.InvokeMessage(sender.ToString(), msg);
        }

        public static void OnShowMessage(object msg)
        {
            HookInterface.InvokeShowMessage(msg);
        }
    }
}

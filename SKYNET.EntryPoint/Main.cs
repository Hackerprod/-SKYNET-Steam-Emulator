using EasyHook;
using SKYNET.Helpers;
using SKYNET.Manager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SKYNET
{
    public class Main : IEntryPoint
    {
        public static Main Instance;

        public static HookManager HookManager;

        public static HookInterface HookInterface;

        private string callbackChannel;

        public static Game Game;

        public static SteamEmulator SteamEmulator;

        public Main(RemoteHooking.IContext inContext, string inChannelName)
        {
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
                Write("Error forcing steam_api injection");
                return;
            }

            SteamEmulator.SteamApiPath = dllPath;

            Memory.CreateInMemoryModule(dllPath);
        }

        public void Run(RemoteHooking.IContext inContext, string inChannelName)
        {
            try
            {
                if (!SteamEmulator.Initialized)
                {
                    SteamEmulator = new SteamEmulator(true);
                    SteamEmulator.OnMessage += SteamEmulator_OnMessage;
                    SteamEmulator.Initialize();
                    SteamEmulator.AppId = HookInterface.Game.AppId;
                    SteamEmulator.EmulatorPath = HookInterface.EmulatorPath;
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

        private void SteamEmulator_OnMessage(object sender, object msg)
        {
            Write(msg);
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

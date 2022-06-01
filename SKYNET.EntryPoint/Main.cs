using EasyHook;
using SKYNET.Helpers;
using SKYNET.Hook.Handles;
using SKYNET.Manager;
using SKYNET.Managers;
using SKYNET.Steamworks;
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

        public void Run(RemoteHooking.IContext inContext, string inChannelName)
        {
            try
            {
                if (!SteamEmulator.Initialized)
                {
                    string app = Process.GetCurrentProcess().ProcessName;
                    Write($"Initializing steam API in {app}");
                    SteamEmulator = new SteamEmulator();
                    SteamEmulator.Initialize(true);
                    SteamEmulator.OnMessage += SteamEmulator_OnMessage;
                    SteamEmulator.FileLog = Game.SendLog;
                    SteamEmulator.EmulatorPath = HookInterface.EmulatorPath;
                    SteamEmulator.AppID = HookInterface.Game.AppId;
                    SteamEmulator.EmulatorPath = HookInterface.EmulatorPath;
                    SteamEmulator.PersonaName = HookInterface.PersonaName;
                    SteamEmulator.SteamID = (CSteamID)HookInterface.SteamId;
                    SteamEmulator.EmulatorPath = HookInterface.Language;
                    SteamEmulator.FileLog = true;
                    SteamEmulator.ConsoleLog = true;
                    bool ConsoleOutput = HookInterface.ConsoleOutput;

                    if (ConsoleOutput)
                    {
                        modCommon.ActiveConsoleOutput();
                    }
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

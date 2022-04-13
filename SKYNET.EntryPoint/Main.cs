using EasyHook;
using SKYNET.Helpers;
using SKYNET.Manager;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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

            Config.HelperLibraryLocation = Path.GetDirectoryName(HookInterface.DllPath);
            Config.DependencyPath = Path.GetDirectoryName(HookInterface.DllPath);

            HookInterface.Ping(callbackChannel);
        }

        public void Run(RemoteHooking.IContext inContext, string inChannelName)
        {
            try
            {
                if (!SteamEmulator.Initialized)
                {
                    SteamEmulator = new SteamEmulator(true);
                    SteamEmulator.OnMessage += SteamEmulator_OnMessage;
                    SteamEmulator.EmulatorPath = HookInterface.EmulatorPath;
                    SteamEmulator.Initialize();

                    Game = SKYNET.Types.Game.Deserialize(HookInterface.SerializedGame);
                    SettingsEmu settings = SettingsEmu.Deserialize(HookInterface.SerializedGame);

                    CLoadLibrary.LoadLibraryA(Game.SteamApiPath, out string Msg);

                    SteamEmulator.AppId = Game.AppId;
                    SteamEmulator.SteamId = settings.SteamId;
                    SteamEmulator.PersonaName = settings.PersonaName;
                    SteamEmulator.Language = settings.Language;
                    SteamEmulator.SendLog = HookInterface.SendLog;
                    SteamEmulator.SteamApiPath = Game.SteamApiPath;

                    if (settings.ConsoleOutput)
                    {
                        modCommon.ActiveConsoleOutput();
                    }

                    HookManager = new HookManager();
                }

                HookManager.Install();
            }
            catch (Exception msg)
            {
                MessageBox.Show(msg.ToString());
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

        public static void Write(object sender, object msg)
        {
            Write(Process.GetCurrentProcess().ProcessName.ToUpper(), msg);
        }

        public static void Write(object msg)
        {
            uint appId = Game == null ? 0 : Game.AppId;
            Write(appId, "Main", msg);
        }

        public static void Write(uint appId, object sender, object msg)
        {
            HookInterface.InvokeMessage(appId, sender.ToString(), msg);
        }

        public static void OnShowMessage(object msg)
        {
            HookInterface.InvokeShowMessage(msg);
        }
    }
}

using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using SKYNET.Managers;
using SKYNET.Plugin;
using SKYNET.Steamworks;
using SKYNET.Types;
using SKYNET.Wave;
using AppID = System.UInt32;

namespace SKYNET.Client
{
    public class SteamClient
    {
        public static CSteamID SteamID { get; set; }
        public static uint AccountID { get; set; }
        public static string AccountName { get; set; }
        public static string PersonaName { get; set; }
        public static string Language { get; set; }
        public static Bitmap Avatar { get; set; }
        public static Bitmap DefaultAvatar { get; set; }
        public static CSteamID SteamID_GS { get; set; }
        public static bool Debug { get; set; }
        public static int InputDeviceID { get; set; }
        public static double Wallet { get; set; }

        public SteamClient()
        {
            Write("SteamClient", "Initializing SteamClient");
            SteamID = new CSteamID(Settings.AccountID);
            AccountID = Settings.AccountID;
            AccountName = Settings.AccountName;
            PersonaName = Settings.PersonaName;
            Language = Settings.Language;
            SteamID_GS = CSteamID.GenerateGameServer();
            Debug = Settings.ShowDebugConsole;
            InputDeviceID = Settings.InputDeviceID;
        }

        public void Initialize()
        {
            Task.Run(delegate { UserManager.Initialize(SteamID, PersonaName); });
            Task.Run(delegate { NetworkManager.Initialize(); }); 
            Task.Run(delegate { StatsManager.Initialize(); });
            Task.Run(delegate { IPCManager.Initialize(); });
            Task.Run(delegate { OverlayManager.Initialize(); });
            
        }

        private static void InitializePlugins()
        {
            string PluginsDirectory = Common.GetPath();
            if (Directory.Exists(PluginsDirectory))
            {
                foreach (var file in Directory.GetFiles(PluginsDirectory, "*.dll"))
                {
                    if (Path.GetFileNameWithoutExtension(file).StartsWith("SKYNET."))
                    {
                        try
                        {
                            var plugin = Assembly.LoadFile(file);
                            Type type = plugin.GetType("SKYNET.GameCoordinator");
                            if (type != null)
                            {
                                IGameCoordinatorPlugin iPlugin = (IGameCoordinatorPlugin)Activator.CreateInstance(type);
                                if (iPlugin == null)
                                {
                                    Write("PLUGINS", $"Failed to load plugin {Path.GetFileNameWithoutExtension(file)}");
                                }
                                else
                                {
                                    AppID appID = iPlugin.Initialize();
                                    //if (appID == AppID)
                                    //{
                                    //    GameCoordinatorPlugin = iPlugin;
                                    //    GameCoordinatorPlugin.IsMessageAvailable = IsMessageAvailable;
                                    //    Write("PLUGINS", $"Loaded GameCoordinator plugin {Path.GetFileNameWithoutExtension(file)} for AppID {appID}");
                                    //}
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            Write("PLUGINGS", $"Failed to load plugin {Path.GetFileNameWithoutExtension(file)} {"\n"}");
                        }
                    }
                }
            }
        }


        public static void Write(string sender, object msg)
        {
            if (!Debug) return;
            Log.Write(sender, msg);
        }
    }
}

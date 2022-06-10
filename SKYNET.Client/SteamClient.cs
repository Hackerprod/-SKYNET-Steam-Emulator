using SKYNET.Helper;
using SKYNET.Managers;
using SKYNET.Plugin;
using SKYNET.Steamworks;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using AppID = System.UInt32;

namespace SKYNET.Client
{
    public class SteamClient
    {
        public static CSteamID SteamID { get; set; }
        public static uint AccountID { get; set; }
        public static string PersonaName { get; set; }
        public static string Language { get; set; }
        public static Bitmap Avatar { get; set; }
        public static CSteamID SteamID_GS { get; set; }

        public SteamClient(Settings settings)
        {
            SteamID = new CSteamID(settings.AccountID);
            AccountID = settings.AccountID;
            PersonaName = settings.PersonaName;
            Language = settings.Language;
            SteamID_GS = CSteamID.GenerateGameServer();

            var AvatarPath = Path.Combine(modCommon.GetPath(), "Data", "Images", "Avatar.jpg");
            if (File.Exists(AvatarPath))
                Avatar = (Bitmap)Image.FromFile(AvatarPath);
            else
                Avatar = ImageHelper.GetDesktopWallpaper(true);
        }

        public void Initialize()
        {
            UserManager.Initialize();
            NetworkManager.Initialize();
            IPCManager.Initialize();
        }

        private static void InitializePlugins()
        {
            string PluginsDirectory = modCommon.GetPath();
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
                            Write("PLUGINS", $"Failed to load plugin {Path.GetFileNameWithoutExtension(file)} {"\n"}");
                        }
                    }
                }
            }
        }


        internal static void Write(string v1, string v2)
        {

        }

    }
}

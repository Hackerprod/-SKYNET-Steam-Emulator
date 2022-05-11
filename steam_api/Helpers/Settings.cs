using SKYNET;
using SKYNET.Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SKYNET.Helper
{
    public class Settings
    {
        private static INIParser IniParser;

        public static void Load()
        {
            // Verify Paths
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "SKYNET"));
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "SKYNET", "Storage"));
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "SKYNET", "Storage", "Remote"));

            try
            {
                string fileName = Path.Combine(modCommon.GetPath(), "SKYNET", "[SKYNET] steam_api.ini");

                if (!File.Exists(fileName))
                {
                    string steam_appid = Path.Combine(modCommon.GetPath(), "steam_appid.txt");
                    string appid = File.Exists(steam_appid) ? File.ReadAllText(steam_appid) : "0";
                    StringBuilder config = new StringBuilder();

                    // User Configuration

                    config.AppendLine("[User Settings]");
                    config.AppendLine($"PersonaName = {Environment.UserName}");
                    config.AppendLine($"AccountId = {new Random().Next(1000, 9999)}");
                    config.AppendLine();

                    config.AppendLine("[Game Settings]");
                    config.AppendLine($"Languaje = english");
                    config.AppendLine($"AppId = {appid}");
                    config.AppendLine();

                    // Network Configuration

                    config.AppendLine("[Network Settings]");
                    config.AppendLine("# When the emulator is in LAN mode (without dedicated server) it sends and receives data through broadcast ");
                    config.AppendLine("ServerIP = 127.0.0.1");
                    config.AppendLine("BroadCastPort = 28025");
                    config.AppendLine();

                    // Log Configuration

                    config.AppendLine("[Log Settings]");
                    config.AppendLine("File = false");
                    config.AppendLine("Console = false");
                    config.AppendLine();

                    File.WriteAllText(fileName, config.ToString());
                }

                IniParser = new INIParser();
                IniParser.Load(fileName);

                SteamEmulator.PersonaName = (string)IniParser["User Settings"]["PersonaName"];
                SteamEmulator.Language = (string)IniParser["Game Settings"]["Languaje"];

                foreach (var item in IniParser["User Settings"].Settings)
                    if (item.Key == "AccountId")
                        if (uint.TryParse((string)item.Value, out uint accountId))
                            SteamEmulator.SteamId = new CSteamID(accountId, Steamworks.EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeIndividual);

                foreach (var item in IniParser["Game Settings"].Settings)
                    if (item.Key == "AppId")
                        if (uint.TryParse((string)item.Value, out uint appId))
                            SteamEmulator.AppId = appId;

                SteamEmulator.SendLog = (bool)IniParser["Log Settings"]["File"];
                SteamEmulator.ConsoleLog = (bool)IniParser["Log Settings"]["Console"];

                if (SteamEmulator.ConsoleLog)
                {
                    modCommon.ActiveConsoleOutput();
                }

                SteamEmulator.BroadCastPort = (int)IniParser["Network Settings"]["BroadCastPort"];

                string data = $"Loaded user data from file \n PersonaName: {SteamEmulator.PersonaName} \n SteamId:  {SteamEmulator.SteamId} \n Languaje: {SteamEmulator.Language} \n";
                SteamEmulator.Write("Settings", data);
            }
            catch (Exception)
            {
                MessageBox.Show("xd");
            }

        }
    }
}
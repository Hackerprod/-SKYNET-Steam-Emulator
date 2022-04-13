using SKYNET.Steamworks.Types;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SKYNET.Types
{
    [Serializable]
    public class SettingsEmu : MarshalByRefObject
    {
        public ulong SteamId { get; set; }
        public string PersonaName { get; set; }
        public string Language { get; set; }
        public bool LogToFile { get; set; }
        public bool ConsoleOutput { get; set; }

        public void Load()
        {
            try
            {
                string _file = Path.Combine(modCommon.GetPath(), "Data", "[SKYNET] Steam Emulator.ini");

                if (!File.Exists(_file))
                {
                    StringBuilder config = new StringBuilder();

                    // User Configuration

                    SteamID createdSteamID = new SteamID();
                    createdSteamID.Set((uint)(new Random().Next(1000, 9999)), SKYNET.Steamworks.EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeIndividual);

                    config.AppendLine("[STEAM USER]");
                    config.AppendLine($"Nickname = {Environment.UserName}");
                    config.AppendLine($"SteamID = {createdSteamID.ConvertToUInt64()}");
                    config.AppendLine($"Languaje = English");
                    config.AppendLine();

                    // Network Configuration

                    config.AppendLine("[NETWORK]");
                    config.AppendLine("# When the emulator is in LAN mode (without dedicated server) it sends and receives data through broadcast ");
                    config.AppendLine("ServerIP = 127.0.0.1");
                    config.AppendLine("BroadCastPort = 28025");
                    config.AppendLine();

                    // Log Configuration

                    config.AppendLine("[LOG]");
                    config.AppendLine("Console = false");
                    config.AppendLine("File = false");
                    config.AppendLine();

                    File.WriteAllText(_file, config.ToString());
                }

                INIParser IniParser = new INIParser();
                IniParser.Load(_file);

                SteamId = (ulong)IniParser["STEAM USER"]["SteamID"];
                PersonaName = (string)IniParser["STEAM USER"]["Nickname"];
                Language = (string)IniParser["STEAM USER"]["Languaje"];
                LogToFile = (bool)IniParser["LOG"]["File"];
                ConsoleOutput = (bool)IniParser["LOG"]["Console"];

                string data = $"Loaded user data from file \nNickName: {SteamEmulator.PersonaName} \nSteamId:  {SteamEmulator.SteamId} \nLanguaje: {SteamEmulator.Language} \n";
            }
            catch (Exception e)
            {
                string errorMessage = e.Message + " " + e.StackTrace;
            }
        }

        public void Save()
        {
            try
            {
                string _file = Path.Combine(modCommon.GetPath(), "Data", "[SKYNET] Steam Emulator.ini");

                INIParser IniParser = new INIParser();
                IniParser.Load(_file);

                IniParser["STEAM USER"]["SteamID"] = SteamId;
                IniParser["STEAM USER"]["Nickname"] = PersonaName;
                IniParser["STEAM USER"]["Languaje"] = Language;
                IniParser["LOG"]["File"] = LogToFile;
                IniParser["LOG"]["Console"] = ConsoleOutput;

                IniParser.Save();
            }
            catch (Exception e)
            {
                string errorMessage = e.Message + " " + e.StackTrace;
            }
        }

        public static string Serialize(SettingsEmu serializedGame)
        {
            return new JavaScriptSerializer().Serialize(serializedGame);
        }

        public static SettingsEmu Deserialize(string serializedGame)
        {
            return new JavaScriptSerializer().Deserialize<SettingsEmu>(serializedGame);
        }
    }
}

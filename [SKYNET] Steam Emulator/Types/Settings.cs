
using SKYNET.Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SKYNET.Types
{
    public class Settings
    {
        public ulong SteamId { get; set; }
        public string PersonaName { get; set; }
        public string Language { get; set; }
        public bool LogToFile { get; set; }
        public bool ConsoleOutput { get; set; }

        public static Settings Load()
        {
            Settings settings;
            string fileName = Path.Combine(modCommon.GetPath(), "Data", "[SKYNET] Steam Emulator.json");
            if (!File.Exists(fileName))
            {
                CSteamID steamID = CSteamID.CreateOne();
                settings = new Settings()
                {
                    SteamId = steamID.SteamID,
                    Language = "English",
                    PersonaName = Environment.UserName,
                    LogToFile = false,
                    ConsoleOutput = false
                };
                return settings;
            }
            else
            {
                string json = File.ReadAllText(fileName);
                settings = new JavaScriptSerializer().Deserialize<Settings>(json);
            }
            return settings;
        }

        public static void Save(Settings settings)
        {
            string fileName = Path.Combine(modCommon.GetPath(), "Data", "[SKYNET] Steam Emulator.json");

            string json = new JavaScriptSerializer().Serialize(settings);
            File.WriteAllText(fileName, json);

        }
    }
}

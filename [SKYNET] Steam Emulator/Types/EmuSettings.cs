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
    public class EmuSettings
    {
        public ulong SteamId { get; set; }
        public string PersonaName { get; set; }
        public string Language { get; set; }
        public bool LogToFile { get; set; }
        public bool ConsoleOutput { get; set; }

        public static EmuSettings Load()
        {
            EmuSettings settings;
            string fileName = Path.Combine(modCommon.GetPath(), "Data", "[SKYNET] Steam Emulator.json");
            if (!File.Exists(fileName))
            {
                SteamID steamId = new SteamID();
                steamId.Set((uint)(new Random().Next(1000, 9999)), Steamworks.EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeIndividual);

                settings = new EmuSettings()
                {
                    SteamId = steamId.ConvertToUInt64(),
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
                settings = new JavaScriptSerializer().Deserialize<EmuSettings>(json);
            }
            return settings;
        }

        public static void Save(EmuSettings settings)
        {
            string fileName = Path.Combine(modCommon.GetPath(), "Data", "[SKYNET] Steam Emulator.json");

            string json = new JavaScriptSerializer().Serialize(settings);
            File.WriteAllText(fileName, json);

        }
    }
}

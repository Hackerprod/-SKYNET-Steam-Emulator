
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SKYNET.Types
{
    public class Settings
    {
        public string AccountName { get; set; }
        public string PersonaName { get; set; }
        public uint AccountID { get; set; }
        public string Language { get; set; }
        public IPAddress ServerIP { get; set; }


        public static Settings Load()
        {
            Settings settings;
            string fileName = Path.Combine(modCommon.GetPath(), "Data", "[SKYNET] Steam Emulator.bin");
            if (!File.Exists(fileName))
            {
                settings = new Settings()
                {
                    PersonaName = Environment.UserName,
                    AccountName = Environment.UserName,
                    AccountID = (uint)new Random().Next(1000, 9999),
                    Language = "english",
                    ServerIP = IPAddress.Loopback,
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
            string fileName = Path.Combine(modCommon.GetPath(), "Data", "[SKYNET] Steam Emulator.bin");
            modCommon.EnsureDirectoryExists(fileName, true);
            string json = new JavaScriptSerializer().Serialize(settings);
            File.WriteAllText(fileName, json);
        }
    }
}

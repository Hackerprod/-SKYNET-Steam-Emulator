
using SKYNET.INI;
using SKYNET.INI.Attributes;
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
        [INISection("User Info")]
        public string AccountName { get; set; }

        [INISection("User Info")]
        public string PersonaName { get; set; }

        [INISection("User Info")]
        public uint AccountID { get; set; }

        [INISection("User Info")]
        public string Language { get; set; }

        [INISection("Network")]
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
                settings = INISerializer.DeserializeFromFile<Settings>(fileName);
            }
            return settings;
        }

        public static void Save(Settings settings)
        {
            string fileName = Path.Combine(modCommon.GetPath(), "Data", "[SKYNET] Steam Emulator.bin");
            modCommon.EnsureDirectoryExists(fileName, true);
            INISerializer.SerializeToFile(settings, fileName);
        }
    }
}

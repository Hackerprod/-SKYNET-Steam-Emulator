
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
        public string PersonaName { get; set; }

        public uint AccountID { get; set; }

        public string Language { get; set; }
        
        public IPAddress ServerIP { get; set; }

        public int BroadcastPort { get; set; }

        public bool LogToFile { get; set; }

        public bool LogToConsole { get; set; }


        public bool RunCallbacks { get; set; }

        public bool ISteamHTTP { get; set; }



        public static Settings Load()
        {
            Settings settings;
            string fileName = Path.Combine(modCommon.GetPath(), "Data", "[SKYNET] Steam Emulator.bin");
            if (!File.Exists(fileName))
            {
                settings = new Settings()
                {
                    PersonaName = Environment.UserName,
                    AccountID = (uint)new Random().Next(1000, 9999),
                    Language = "english",
                    ServerIP = IPAddress.Loopback,
                    BroadcastPort = 28025,
                    LogToFile = false,
                    LogToConsole = false,
                    RunCallbacks = true,
                    ISteamHTTP = true
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

        public void Save()
        {
            string fileName = Path.Combine(modCommon.GetPath(), "Data", "[SKYNET] Steam Emulator.bin");
            string json = new JavaScriptSerializer().Serialize(this);
            File.WriteAllText(fileName, json);
        }

    }
}

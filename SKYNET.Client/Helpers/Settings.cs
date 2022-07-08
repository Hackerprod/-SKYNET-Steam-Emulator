using SKYNET.Helper;
using System;
using System.Net;

namespace SKYNET.Types
{
    public class Settings
    {
        public string AccountName { get; set; }
        public string PersonaName { get; set; }
        public uint AccountID { get; set; }
        public string Language { get; set; }
        public bool AllowRemoteAccess { get; set; }
        public IPAddress ServerIP { get; set; }


        private static RegistrySettings Registry;

        static Settings()
        {
            Registry = new RegistrySettings(@"SOFTWARE\SKYNET\[SKYNET] Steam Emulator\");
        }

        public static Settings Load()
        {
            Settings settings = new Settings()
            {
                PersonaName = Registry.Get<string>("PersonaName", Environment.UserName),
                AccountName = Registry.Get<string>("AccountName", Environment.UserName),
                AccountID = Registry.Get<uint>("AccountID", (uint)new Random().Next(1000, 9999)),
                Language = Registry.Get<string>("Language", "english"),
                ServerIP = Registry.Get<IPAddress>("ServerIP", IPAddress.Loopback),
            };
            return settings;
        }

        public static void Save(Settings settings)
        {
            Registry.Set("PersonaName", settings.PersonaName);
            Registry.Set("AccountName", settings.AccountName);
            Registry.Set("AccountID", settings.AccountID);
            Registry.Set("Language", settings.Language);
            Registry.Set("ServerIP", settings.ServerIP);
        }
    }
}

using SKYNET.Helpers;
using System;
using System.Net;

namespace SKYNET.Types
{
    public class Settings
    {
        public static string AccountName { get; set; }
        public static string PersonaName { get; set; }
        public static uint AccountID { get; set; }
        public static string Language { get; set; }
        public static bool AllowRemoteAccess { get; set; }
        public static bool ShowDebugConsole { get; set; }
        public static IPAddress ServerIP { get; set; }
        public static int InputDeviceID { get; set; }



        private static RegistrySettings Registry;

        static Settings()
        {
            Registry = new RegistrySettings(@"SOFTWARE\SKYNET\[SKYNET] Steam Emulator\");
        }

        public static void Load()
        {
            PersonaName = Registry.Get<string>("PersonaName", Environment.UserName);
            AccountName = Registry.Get<string>("AccountName", Environment.UserName);
            AccountID = Registry.Get<uint>("AccountID", (uint)new Random().Next(1000, 9999));
            Language = Registry.Get<string>("Language", "english");
            ServerIP = Registry.Get<IPAddress>("ServerIP", IPAddress.Loopback);
            AllowRemoteAccess = Registry.Get<bool>("AllowRemoteAccess", false);
            ShowDebugConsole = Registry.Get<bool>("ShowDebugConsole", false);
            InputDeviceID = Registry.Get<int>("InputDeviceID", 0);
        }

        public static void Save()
        {
            Registry.Set("PersonaName", PersonaName);
            Registry.Set("AccountName", AccountName);
            Registry.Set("AccountID", AccountID);
            Registry.Set("Language", Language);
            Registry.Set("ServerIP", ServerIP);
            Registry.Set("AllowRemoteAccess", AllowRemoteAccess);
            Registry.Set("ShowDebugConsole", ShowDebugConsole);
            Registry.Set("InputDeviceID", InputDeviceID);
            
        }
    }
}

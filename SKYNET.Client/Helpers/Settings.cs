using SKYNET.Helpers;
using System;
using System.Net;

namespace SKYNET.Types
{
    public class Settings
    {
        public static string Username { get; set; }
        public static string Password { get; set; }
        public static bool Remember { get; set; }
        public static IPAddress ServerIP { get; set; }
        public static string Language { get; set; }
        public static bool AllowRemoteAccess { get; set; }
        public static bool ShowDebugConsole { get; set; }
        public static int InputDeviceID { get; set; }



        private static RegistrySettings Registry;

        static Settings()
        {
            Registry = new RegistrySettings(@"SOFTWARE\SKYNET\[SKYNET] Steam Emulator\");
        }

        public static void Load()
        {
            Username = Registry.Get<string>("Username", "");
            Password = Registry.Get<string>("Password", "");
            Remember = Registry.Get<bool>("Remember", false);
            ServerIP = Registry.Get<IPAddress>("ServerIP", IPAddress.Loopback);
            Language = Registry.Get<string>("Language", "english");
            AllowRemoteAccess = Registry.Get<bool>("AllowRemoteAccess", false);
            ShowDebugConsole = Registry.Get<bool>("ShowDebugConsole", false);
            InputDeviceID = Registry.Get<int>("InputDeviceID", 0);
        }

        public static void Save()
        {
            Registry.Set("Username", Username);
            Registry.Set("Password", Password);
            Registry.Set("Remember", Remember);
            Registry.Set("ServerIP", ServerIP);
            Registry.Set("Language", Language);
            Registry.Set("AllowRemoteAccess", AllowRemoteAccess);
            Registry.Set("ShowDebugConsole", ShowDebugConsole);
            Registry.Set("InputDeviceID", InputDeviceID);
            
        }
    }
}

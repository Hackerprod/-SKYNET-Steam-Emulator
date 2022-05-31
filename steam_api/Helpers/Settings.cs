using SKYNET;
using SKYNET.INI;
using SKYNET.Managers;
using SKYNET.Steamworks;
using SKYNET.Steamworks.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static SKYNET.INI.INISerializer;

namespace SKYNET.Helper
{
    public class Settings
    {
        [Section("User Info")]
        public string PersonaName { get; set; }

        [Section("User Info")]
        public uint AccountID { get; set; }


        [Section("Game Info")]
        public string Language { get; set; }

        [Section("Game Info")]
        public uint AppId { get; set; }


        [Comment("IP address of the dedicated steam server")]
        public IPAddress ServerIP { get; set; }

        [Section("Network Settings")]
        public int BroadcastPort { get; set; }

        

        [Section("Log Settings")]
        public bool File { get; set; }

        [Section("Log Settings")]
        public bool Console { get; set; }


        [Section("Debug Info")]
        public bool RunCallbacks { get; set; }

        [Section("Debug Info")]
        public bool ISteamHTTP { get; set; }

        public static void Load()
        {
            // Verify Paths
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "SKYNET"));
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "SKYNET", "Storage"));
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "SKYNET", "Storage", "Remote"));

            string fileName = Path.Combine(modCommon.GetPath(), "SKYNET", "[SKYNET] steam_api.ini");
            if (!System.IO.File.Exists(fileName))
            {
                CreateNew(fileName);
            }

            try
            {
                Settings settings = INISerializer.DeserializeFromFile<Settings>(fileName);

                SteamEmulator.PersonaName   = settings.PersonaName;
                SteamEmulator.SteamID       = new CSteamID(settings.AccountID, Steamworks.EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeIndividual);
                SteamEmulator.Language      = settings.Language;
                SteamEmulator.AppID         = settings.AppId;
                SteamEmulator.BroadcastPort = settings.BroadcastPort;
                SteamEmulator.FileLog       = settings.File;
                SteamEmulator.ConsoleLog    = settings.Console;
                SteamEmulator.RunCallbacks  = settings.RunCallbacks;
                SteamEmulator.ISteamHTTP    = settings.ISteamHTTP;
                if (settings.Console)
                {
                    modCommon.ActiveConsoleOutput();
                }
                string data = $"Loaded user data from file \n PersonaName: {SteamEmulator.PersonaName} \n SteamId:  {SteamEmulator.SteamID} \n Languaje: {SteamEmulator.Language} \n";
                SteamEmulator.Write("Settings", data);
                //INISerializer.SerializeToFile(settings, fileName);
            }
            catch
            {
                CreateNew(fileName);
            }
        }

        private static void CreateNew(string fileName)
        {
            string steam_appid = Path.Combine(modCommon.GetPath(), "steam_appid.txt");
            int appid = 0;
            if (System.IO.File.Exists(steam_appid))
            {
                try { appid = int.Parse(System.IO.File.ReadAllText(steam_appid)); } catch { }
            }

            Settings settings = new Settings()
            {
                PersonaName = Environment.UserName,
                AccountID = (uint)new Random().Next(1000, 9999),
                Language = "english",
                AppId = (uint)appid,
                ServerIP = IPAddress.Loopback,
                BroadcastPort = 28025,
                File = false,
                Console = false,
                RunCallbacks = true,
                ISteamHTTP = true
            };
            INISerializer.SerializeToFile(settings, fileName);
        }

        private static void DetourGameDebug(object sendLog)
        {
            if (SteamEmulator.FileLog)
            {
                try
                {
                    foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
                    {
                        //Console.WriteLine(DateTime.Now);
                    }
                }
                catch 
                {
                }
            }
            //var Assemblies = AppDomain.CurrentDomain;
            //SteamEmulator.Write("Assemblies", Assemblies == null);

            //var Assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //foreach (var Assemblie in Assemblies)
            //{
            //    SteamEmulator.Write("Assemblies", Assemblie.FullName);
            //}
            //foreach (var type in currentAssembly.GetTypes())
            //{
            //    if (type.IsDefined(typeof(InterfaceAttribute)))
            //    {
            //        foreach (var attribute in type.GetCustomAttributes<InterfaceAttribute>())
            //        {
            //            interfaceTypes.TryAdd(attribute.Name, type);
            //        }
            //    }
            //}
        }

        public static IntPtr GetProcAddress(string InModule, string InSymbolName)
        {
            IntPtr moduleHandle = GetModuleHandle(InModule);
            if (moduleHandle == IntPtr.Zero)
            {
                SteamEmulator.Write("HookManager", "Null Module Handle");
                return IntPtr.Zero;
            }
            IntPtr procAddress = GetProcAddress(moduleHandle, InSymbolName);
            if (procAddress == IntPtr.Zero)
            {
                SteamEmulator.Write("HookManager", InSymbolName + " does not exist.");
                return IntPtr.Zero;
            }
            return procAddress;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
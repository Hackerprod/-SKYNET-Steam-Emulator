using SKYNET;
using SKYNET.Managers;
using SKYNET.Steamworks;
using SKYNET.Steamworks.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SKYNET.Helper
{
    public class Settings
    {
        private static INIParser IniParser;

        public static void Load()
        {
            // Verify Paths
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "SKYNET"));
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "SKYNET", "Storage"));
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "SKYNET", "Storage", "Remote"));

            try
            {
                string fileName = Path.Combine(modCommon.GetPath(), "SKYNET", "[SKYNET] steam_api.ini");

                if (!File.Exists(fileName))
                {
                    string steam_appid = Path.Combine(modCommon.GetPath(), "steam_appid.txt");
                    string appid = File.Exists(steam_appid) ? File.ReadAllText(steam_appid) : "0";
                    StringBuilder config = new StringBuilder();

                    // User Configuration

                    config.AppendLine("[User Settings]");
                    config.AppendLine($"PersonaName = {Environment.UserName}");
                    config.AppendLine($"AccountId = {new Random().Next(1000, 9999)}");
                    config.AppendLine();

                    config.AppendLine("[Game Settings]");
                    config.AppendLine($"Languaje = english");
                    config.AppendLine($"AppId = {appid}");
                    config.AppendLine();

                    // Network Configuration

                    config.AppendLine("[Network Settings]");
                    config.AppendLine("# When the emulator is in LAN mode (without dedicated server) it sends and receives data through broadcast ");
                    config.AppendLine("ServerIP = 127.0.0.1");
                    config.AppendLine("BroadCastPort = 28025");
                    config.AppendLine();

                    // Network Configuration

                    config.AppendLine("[Debug Settings]");
                    config.AppendLine("RunCallbacks = true");
                    config.AppendLine("ISteamHTTP = true");
                    config.AppendLine();

                    // Log Configuration

                    config.AppendLine("[Log Settings]");
                    config.AppendLine("File = false");
                    config.AppendLine("Console = false");
                    config.AppendLine();

                    File.WriteAllText(fileName, config.ToString());
                }

                IniParser = new INIParser();
                IniParser.Load(fileName);

                SteamEmulator.PersonaName = (string)IniParser["User Settings"]["PersonaName"];
                SteamEmulator.Language = (string)IniParser["Game Settings"]["Languaje"];

                foreach (var item in IniParser["User Settings"].Settings)
                    if (item.Key == "AccountId")
                        if (uint.TryParse((string)item.Value, out uint accountId))
                            SteamEmulator.SteamID = new CSteamID(accountId, Steamworks.EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeIndividual);

                foreach (var item in IniParser["Game Settings"].Settings)
                    if (item.Key == "AppId")
                        if (uint.TryParse((string)item.Value, out uint appId))
                            SteamEmulator.AppID = appId;

                SteamEmulator.SendLog = (bool)IniParser["Log Settings"]["File"];
                SteamEmulator.ConsoleLog = (bool)IniParser["Log Settings"]["Console"];

                ThreadPool.QueueUserWorkItem(DetourGameDebug);

                try { SteamEmulator.RunCallbacks = (bool)IniParser["Debug Settings"]["RunCallbacks"]; } catch { }
                try { SteamEmulator.ISteamHTTP = (bool)IniParser["Debug Settings"]["ISteamHTTP"]; } catch { }

                if (SteamEmulator.ConsoleLog)
                {
                    modCommon.ActiveConsoleOutput();
                }

                SteamEmulator.BroadCastPort = (int)IniParser["Network Settings"]["BroadCastPort"];

                string data = $"Loaded user data from file \n PersonaName: {SteamEmulator.PersonaName} \n SteamId:  {SteamEmulator.SteamID} \n Languaje: {SteamEmulator.Language} \n";
                SteamEmulator.Write("Settings", data);
            }
            catch (Exception)
            {
                MessageBox.Show("xd");
            }

        }

        private static void DetourGameDebug(object sendLog)
        {
            if (SteamEmulator.SendLog)
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
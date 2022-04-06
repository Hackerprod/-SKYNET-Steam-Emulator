using SKYNET;
using SKYNET.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class modCommon
{
    public static bool LogToFile { get; set; }

    private static bool ConsoleEnabled;
    private static bool SettingsLoaded;
    private static INIParser IniParser;


    public static IntPtr GetObjectPtr(object Obj)
    {
        IntPtr Ptr = IntPtr.Zero;
        GCHandle gcHandle = GCHandle.Alloc(Obj, GCHandleType.WeakTrackResurrection);
        Ptr = Marshal.ReadIntPtr(GCHandle.ToIntPtr(gcHandle));
        gcHandle.Free();
        return Ptr;
    }
    public static void EnsureDirectoryExists(string filePath, bool isFile = false)
    {
        if (!string.IsNullOrEmpty(filePath))
        {
            filePath = filePath.Trim().Replace("\0", string.Empty);
            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    string text = isFile ? Path.GetDirectoryName(filePath) : filePath;
                    if (Path.IsPathRooted(filePath))
                    {
                        text = text.Trim();
                        if (!Directory.Exists(text))
                        {
                            Directory.CreateDirectory(text);
                        }
                    }
                }
                catch (Exception exception)
                {

                }
            }
        }
    }

    public static void Show(object msg)
    {
        MessageBox.Show(msg.ToString());
    }

    public static void ActiveConsoleOutput()
    {
        if (!ConsoleEnabled)
        {
            ConsoleEnabled = true;
            AllocConsole();
        }
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool AllocConsole();

    public static void LoadSettings()
    {
        if (SettingsLoaded)
        {
            return;
        }

        try
        {
            string _file = Path.Combine(GetPath(), "[SKYNET] steam_api.ini");

            if (!File.Exists(_file))
            {
                StringBuilder config = new StringBuilder();

                // User Configuration

                config.AppendLine("[STEAM USER]");
                config.AppendLine($"Nickname = {Environment.UserName}");
                config.AppendLine($"SteamID = {modCommon.GenerateSteamID()}");
                config.AppendLine($"Languaje = English");
                config.AppendLine();

                // Network Configuration

                config.AppendLine("[NETWORK]");
                config.AppendLine("# When the emulator is in LAN mode (without dedicated server) it sends and receives data through broadcast ");
                config.AppendLine("ServerIP = 127.0.0.1");
                config.AppendLine("BroadCastPort = 28025");
                config.AppendLine();

                // Log Configuration

                config.AppendLine("[LOG]");
                config.AppendLine("Console = false");
                config.AppendLine("File = false");
                config.AppendLine();

                File.WriteAllText(_file, config.ToString());
            }

            IniParser = new INIParser();
            IniParser.Load(_file);

            SteamEmulator.SteamId = (ulong)IniParser["STEAM USER"]["SteamID"];
            SteamEmulator.PersonaName = (string)IniParser["STEAM USER"]["Nickname"];
            SteamEmulator.Language = (string)IniParser["STEAM USER"]["Languaje"];

            LogToFile = (bool)IniParser["LOG"]["File"];

            bool ConsoleOutput = (bool)IniParser["LOG"]["Console"];

            if (ConsoleOutput)
            {
                ActiveConsoleOutput();
            }

            string data = $"Loaded user data from file \nNickName: {SteamEmulator.PersonaName} \nSteamId:  {SteamEmulator.SteamId} \nLanguaje: {SteamEmulator.Language} \n";
            SteamEmulator.Write(data);

            SettingsLoaded = true;
        }
        catch (Exception e)
        {
            string errorMessage = e.Message + " " + e.StackTrace;
            SteamEmulator.Write(errorMessage);
        }

    }

    public static string GenerateSteamID()
    {
        return "76561" + new Random().Next(100000, 999999) + new Random().Next(100000, 999999);
    }
    public static ulong CreateSteamID()
    {
        return ulong.Parse(GenerateSteamID());
    }

    public static string GetPath()
    {
        Process currentProcess;
        try
        {
            currentProcess = Process.GetCurrentProcess();
            return new FileInfo(currentProcess.MainModule.FileName).Directory?.FullName;
        }
        finally { currentProcess = null; }
    }
    public static bool Is64Bit()
    {
        return IntPtr.Size == 8;
    }
}


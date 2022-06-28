using NativeSharp;
using System;
using System.Diagnostics;
using System.IO;

namespace SKYNET
{
    public class DllInjector
    {
        public static Process Inject(Game game)
        {
            string x86Dll = Path.Combine(modCommon.GetPath(), "x86", "steam_api.dll");
            string x64Dll = Path.Combine(modCommon.GetPath(), "x64", "steam_api64.dll");
            string x86CSteamworksDll = Path.Combine(modCommon.GetPath(), "x86", "CSteamworks.dll");
            string x64CSteamworksDll = Path.Combine(modCommon.GetPath(), "x64", "CSteamworks.dll");

            Preparex64Dll();
            PrepareCsteamworks();

            string pName = "";
            var pInfo = new ProcessStartInfo();
            pInfo.FileName = game.ExecutablePath;
            pInfo.CreateNoWindow = true;
            pInfo.RedirectStandardOutput = false;
            pInfo.UseShellExecute = false;
            var tProcess = Process.Start(pInfo);
            pName = tProcess.ProcessName;

            var nProcess = NativeProcess.Open((uint)tProcess.Id);
            var Is64Bit = nProcess.Is64Bit;
            tProcess.Kill();

            string DllPath = "";
            if (game.CSteamworks)
                DllPath = Is64Bit ? x64CSteamworksDll : x86CSteamworksDll;
            else
                DllPath = Is64Bit ? x64Dll : x86Dll;

            string modifiedConfig = InjectConfig;

            modifiedConfig = modifiedConfig.Replace("$ExecutablePath$", game.ExecutablePath);
            modifiedConfig = modifiedConfig.Replace("$Parameters$", game.Parameters);
            modifiedConfig = modifiedConfig.Replace("$Dll$", DllPath);

            string tempConfig = Path.Combine(modCommon.GetPath(), "Data", "Injector", $"{game.AppID}.xpr");
            modCommon.EnsureDirectoryExists(tempConfig, true);
            File.WriteAllText(tempConfig, modifiedConfig);

            string Xenos = Path.Combine(modCommon.GetPath(), "Data", "Injector", "Xenos.exe");

            if (File.Exists(Xenos))
            {
                Process.Start(Xenos, "--run " + "\"" + tempConfig + "\"");
            }

            Process process = null;
            foreach (var item in Process.GetProcesses())
            {
                if (item.ProcessName == pName)
                {
                    process = item;
                }
            }

            return process;
        }

        private static void PrepareCsteamworks()
        {
            string x86Dll = Path.Combine(modCommon.GetPath(), "x86", "steam_api.dll");
            string x64Dll = Path.Combine(modCommon.GetPath(), "x64", "steam_api64.dll");

            string x86CSteamworksDll = Path.Combine(modCommon.GetPath(), "x86", "CSteamworks.dll");
            string x64CSteamworksDll = Path.Combine(modCommon.GetPath(), "x64", "CSteamworks.dll");

            try { File.Copy(x86Dll, x86CSteamworksDll, true); } catch { }
            try { File.Copy(x64Dll, x64CSteamworksDll, true); } catch { }
        }

        private static void Preparex64Dll()
        {
            string x64DllCompiled = Path.Combine(modCommon.GetPath(), "x64", "steam_api.dll");
            string x64Dll = Path.Combine(modCommon.GetPath(), "x64", "steam_api64.dll");
            try { File.Copy(x64DllCompiled, x64Dll, true); } catch { }
        }

        private static string InjectConfig = @"
        <XenosConfig>
	    <imagePath>$Dll$</imagePath>
	    <manualMapFlags>0</manualMapFlags>
	    <procName>$ExecutablePath$</procName>
	    <hijack>0</hijack>
	    <unlink>0</unlink>
	    <erasePE>0</erasePE>
	    <close>0</close>
	    <krnHandle>0</krnHandle>
	    <injIndef>0</injIndef>
	    <processMode>1</processMode>
	    <injectMode>0</injectMode>
	    <delay>0</delay>
	    <period>0</period>
	    <skip>0</skip>
	    <procCmdLine>$Parameters$</procCmdLine>
	    <initRoutine/>
	    <initArgs/>
        </XenosConfig>";
    }
}

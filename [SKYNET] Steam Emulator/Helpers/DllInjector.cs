using NativeSharp;
using System.Diagnostics;
using System.IO;

namespace SKYNET
{
    public class DllInjector
    {
        public static Process Inject(string executablePath, string parameters, object AppID)
        {
            string x86Dll = Path.Combine(modCommon.GetPath(), "x86", "steam_api.dll");
            string x64Dll = Path.Combine(modCommon.GetPath(), "x64", "steam_api64.dll");

            string pName = "";
            var pInfo = new ProcessStartInfo();
            pInfo.FileName = executablePath;
            pInfo.CreateNoWindow = true;
            pInfo.RedirectStandardOutput = false;
            pInfo.UseShellExecute = false;
            var tProcess = Process.Start(pInfo);
            pName = tProcess.ProcessName;

            var nProcess = NativeProcess.Open((uint)tProcess.Id);
            string DllPath = nProcess.Is64Bit ? x64Dll : x86Dll;
            tProcess.Kill();

            string modifiedConfig = InjectConfig;

            modifiedConfig = modifiedConfig.Replace("$ExecutablePath$", executablePath);
            modifiedConfig = modifiedConfig.Replace("$Parameters$", parameters);
            modifiedConfig = modifiedConfig.Replace("$Dll$", DllPath);

            string tempConfig = Path.Combine(modCommon.GetPath(), "Data", "Injector", $"{AppID}.xpr");
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

using NativeSharp;
using SKYNET.Types;
using System.Diagnostics;
using System.IO;

namespace SKYNET.Helpers
{
    public class DllInjector
    {
        public static void Inject(Game game)
        {
            string x86Dll = Path.Combine(modCommon.GetPath(), "x86", "steam_api.dll");
            string x64Dll = Path.Combine(modCommon.GetPath(), "x64", "steam_api64.dll");
            string x86CSteamworksDll = Path.Combine(modCommon.GetPath(), "x86", "CSteamworks.dll");
            string x64CSteamworksDll = Path.Combine(modCommon.GetPath(), "x64", "CSteamworks.dll");

            PrepareDlls();

            if (!File.Exists(game.ExecutablePath))
            {
                modCommon.Show("The game not exists");
                return;
            }

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

            string pVersion = Is64Bit ? "SKYNET.Injectorx64.exe" : "SKYNET.Injectorx86.exe";
            string Injector = Path.Combine(modCommon.GetPath(), "Data", "Injector", pVersion);
            string args = "\"" + game.ExecutablePath + "\" \"" + game.Parameters + "\" \"" + DllPath + "\"";

            var iInfo = new ProcessStartInfo();
            iInfo.FileName = Injector;
            iInfo.Arguments = args;
            iInfo.CreateNoWindow = true;
            iInfo.WindowStyle = ProcessWindowStyle.Hidden;
            iInfo.UseShellExecute = true;
            Process.Start(iInfo);
        }

        private static void PrepareDlls()
        {
            // Prepare x64 Dll
            string x86Dll = Path.Combine(modCommon.GetPath(), "x86", "steam_api.dll");
            string x64Dll = Path.Combine(modCommon.GetPath(), "x64", "steam_api64.dll");
            string x64DllCompiled = Path.Combine(modCommon.GetPath(), "x64", "steam_api.dll");
            try { File.Copy(x64DllCompiled, x64Dll, true); } catch { }

            // Prepare x64 Dll
            string x86CSteamworksDll = Path.Combine(modCommon.GetPath(), "x86", "CSteamworks.dll");
            string x64CSteamworksDll = Path.Combine(modCommon.GetPath(), "x64", "CSteamworks.dll");
            try { File.Copy(x86Dll, x86CSteamworksDll, true); } catch { }
            try { File.Copy(x64Dll, x64CSteamworksDll, true); } catch { }
        }
    }
}


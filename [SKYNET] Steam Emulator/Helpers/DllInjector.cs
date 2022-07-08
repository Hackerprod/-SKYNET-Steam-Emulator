using NativeSharp;
using SKYNET.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

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

            if (!File.Exists(game.ExecutablePath))
            {
                modCommon.Show("The game not exists");
                return null;
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

            //return TestOtherMethod(pName, game, DllPath);

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

        private static Process TestOtherMethod(string pName, Game game, string dllPath)
        {
            string Injector = Path.Combine(modCommon.GetPath(), "Data", "Injector", "Injector.exe");

            Process process = Process.Start(game.ExecutablePath, game.Parameters);
            new Injector().InjectIntoProcess(process, new FileInfo[] { new FileInfo(dllPath) }, 200);
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


        internal class Injector
        {
            private const int PROCESS_CREATE_THREAD = 0x0002;
            private const int PROCESS_QUERY_INFORMATION = 0x0400;
            private const int PROCESS_VM_OPERATION = 0x0008;
            private const int PROCESS_VM_WRITE = 0x0020;
            private const int PROCESS_VM_READ = 0x0010;

            private const uint MEM_COMMIT = 0x00001000;
            private const uint MEM_RESERVE = 0x00002000;
            private const uint MEM_RELEASE = 0x00008000;
            private const uint PAGE_READWRITE = 4;

            private const int OPEN_PROCESS = PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION |
                                             PROCESS_VM_WRITE | PROCESS_VM_READ;

            private const uint MEM_CREATE = MEM_COMMIT | MEM_RESERVE;


            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern int CloseHandle(IntPtr hObject);

            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            private static extern IntPtr GetModuleHandle(string lpModuleName);

            [DllImport("kernel32", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Ansi)]
            private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

            [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
            private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
                uint dwSize, uint flAllocationType, uint flProtect);

            [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
            private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint dwFreeType);

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize,
                out UIntPtr lpNumberOfBytesWritten);

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr CreateRemoteThread(IntPtr hProcess,
                IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter,
                uint dwCreationFlags, IntPtr lpThreadId);

            //public void Inject(string Dll, string Executable)
            //{
            //    Process process = null;
            //    int ProcessId = 0;
            //    if (ProcessId != null)
            //    {
            //        // find process by pid
            //        var pid = opts.ProcessId.Value;
            //        try
            //        {
            //            Write("Attempting to find running process by id...");
            //            process = Process.GetProcessById((int)opts.ProcessId.Value);
            //        }
            //        catch (ArgumentException)
            //        {
            //            Write($"Could not find process id {pid}");
            //        }
            //    }
            //    else if (Executable != null)
            //    {
            //        Write("Checking if process has already started");
            //        if (process == null)
            //        {
            //            var app = Executable;
            //            var app_args = "";

            //            Write($"Starting {Executable}");
            //            process = Process.Start(app, app_args);

            //            if (opts.ProcessName != null)
            //            {
            //                // if the exe to inject to is different than the one started
            //                Write("Waiting for real process to start...");
            //                process = WaitForProcess(opts.ProcessName, opts.Timeout);
            //            }

            //            if (opts.ProcessRestarts)
            //            {
            //                Write("Waiting for original process to exit");
            //                process = WaitForProcessRestart(process, opts.Timeout);
            //                opts.ProcessRestarts = false;
            //            }
            //            else
            //            {
            //                // set this to true, so we can attempt to wait for a restart even if the option is not set
            //                opts.ProcessRestarts = true;
            //            }
            //        }
            //        else
            //        {
            //            Write("Process already started.");
            //        }
            //    }
            //    else if (opts.ProcessName != null)
            //    {
            //        Write("Attempting to find running process by name...");
            //        process = WaitForProcess(opts.ProcessName, opts.Timeout);
            //    }

            //    if (process == null)
            //    {
            //        Write("No process to inject.");
            //    }

            //    var dlls = BuildDllInfos(opts.Dlls);
            //    var wait_dlls = BuildDllInfos(opts.WaitDlls);

            //    if (Executable == null)
            //    {
            //        Write("Not delaying before injection. Process already started.");
            //    }
            //    else
            //    {
            //        Write("Waiting for process to fully load");
            //        try
            //        {
            //            WaitForDlls(process, wait_dlls, opts.InjectionDelay);
            //        }
            //        catch (Exception) when (process.HasExited && opts.ProcessRestarts)
            //        {
            //            Write("It seems the process exited while waiting for it to initialize");
            //            process = WaitForProcessRestart(process, opts.Timeout);
            //            //if we fail here, then injector exits with error
            //            WaitForDlls(process, wait_dlls, opts.InjectionDelay);
            //        }
            //    }

            //    if (process.WaitForInputIdle())
            //    {
            //        Write($"Injecting {dlls.Count} DLL(s) into {process.ProcessName} ({process.Id})");
            //        InjectIntoProcess(process, dlls.ToArray(), opts.InjectLoopDelay);
            //    }
            //}

            public void InjectIntoProcess(Process process, FileInfo[] dlls, uint delay = InjectorOptions.DEFAULT_INJECTION_LOOP_DELAY)
            {
                Write("Opening handle to process");
                var procHandle = OpenProcess(OPEN_PROCESS, false, process.Id);
                if (procHandle == IntPtr.Zero)
                {
                    Write($"Unable to open {process.ProcessName}. Make sure to start the tool with Administrator privileges");
                }

                Write("Retrieving the memory address to LoadLibraryA");
                var loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                if (loadLibraryAddr == IntPtr.Zero)
                {
                    Write("Unable not retrieve the address for LoadLibraryA");
                }

                var dllIndex = 1;
                foreach (var dll in dlls)
                {
                    Write($"Attempting to inject DLL, {dllIndex} of {dlls.Length}, {dll.Name}...");
                    var size = (uint)(dll.FullName.Length + 1);
                    Write("Allocating memory in the process to write the DLL path");
                    var allocMemAddress = VirtualAllocEx(procHandle, IntPtr.Zero, size, MEM_CREATE, PAGE_READWRITE);
                    if (allocMemAddress == IntPtr.Zero)
                    {
                        Write("Unable to allocate memory in the process. Make sure to start the tool with Administrator privileges");
                    }

                    Write("Writing the DLL path in the process memory");
                    var result = WriteProcessMemory(
                        procHandle,
                        allocMemAddress,
                        Encoding.Default.GetBytes(dll.FullName),
                        size,
                        out var bytesWritten
                    );
                    if (!result)
                    {
                        Write("Failed to write the DLL path into the memory of the process");
                    }

                    Write("Creating remote thread. This is where the magic happens!");
                    var threadHandle = CreateRemoteThread(
                        procHandle,
                        IntPtr.Zero,
                        0,
                        loadLibraryAddr,
                        allocMemAddress,
                        0,
                        IntPtr.Zero
                    );
                    if (procHandle == IntPtr.Zero)
                    {
                        Write("Unable to create a remote thread in the process. Failed to inject the dll");
                    }


                    if (process.WaitForInputIdle())
                    {
                        Write("Closing remote thread");
                        CloseHandle(threadHandle);
                        Write("Freeing memory");
                        VirtualFreeEx(procHandle, allocMemAddress, UIntPtr.Zero, MEM_RELEASE);
                    }

                    if (dllIndex < dlls.Length)
                    {
                        if (delay == 0)
                        {
                            Write("No delay between next DLL injection");
                        }
                        else
                        {
                            Write($"Delaying next DLL injection by {delay} ms");
                            Thread.Sleep((int)delay);
                        }
                    }

                    dllIndex++;

                    Write("Injected!");
                }

                Write("Closing handle to process");
                CloseHandle(procHandle);
            }

            private void Write(string v)
            {
                Log.Write("Injector", v);
            }


            public class InjectorOptions
            {
                public const uint DEFAULT_INJECTION_DELAY = 5000;
                public const uint DEFAULT_INJECTION_LOOP_DELAY = 1000;
                public const uint DEFAULT_TIMEOUT = 30000;

                public uint? ProcessId { get; set; }

                public string ProcessName { get; set; }

                public string StartProcess { get; set; }

                public bool ProcessRestarts { get; set; }

                public bool IsWindowsApp { get; set; }

                public uint InjectionDelay { get; set; }

                public IEnumerable<string> WaitDlls { get; set; }

                public uint InjectLoopDelay { get; set; }

                public uint Timeout { get; set; }

                public string ConfigFile { get; set; }

                public bool Quiet { get; set; }

                public bool Verbose { get; set; }

                public bool Interactive { get; set; }

                public bool NoPauseOnError { get; set; }

                public IEnumerable<string> Dlls { get; set; }

            }
        }
    }
}


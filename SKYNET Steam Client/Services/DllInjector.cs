using System.Diagnostics;
using EasyHook;

namespace SKYNET.Client.Services;

/// <summary>
/// Launches a game and loads the emulator payload into it via EasyHook's
/// RemoteHooking.CreateAndInject - the game process is created suspended and
/// stays that way until our injected bootstrap (SKYNET.HookBootstrap, a small,
/// dependency-free assembly built separately for x86 and x64) explicitly resumes
/// it, so nothing in the game can run - including the Windows loader's own
/// static-import resolution - before the payload is loaded and the LdrLoadDll
/// redirect hook (see SKYNET Hook Bootstrap/EntryPoint.cs) is active.
///
/// That single hook covers every way a game ends up asking for the payload's
/// file name: a dynamic LoadLibrary(Ex)W call with just the bare file name, one
/// with a full path (what Unity's native plugin loader does - it resolves and
/// passes an absolute path under its own Plugins folder, which a bare-name
/// reuse trick never sees), and a game that statically imports steam_api64.dll
/// directly - all three ultimately resolve through ntdll's LdrLoadDll.
///
/// Nothing on the game's disk is touched: the payload loads from the path
/// GameLauncher prepared, normally a per-build shadow copy of the launcher's
/// own payload file.
/// </summary>
public static class DllInjector
{
    public static Process LaunchAndInject(string exePath, string dllPath, string arguments, string workingDir)
    {
        if (!File.Exists(exePath)) throw new FileNotFoundException("Executable not found", exePath);
        if (!File.Exists(dllPath)) throw new FileNotFoundException("Injection DLL not found", dllPath);

        var bootstrapX86 = BootstrapPath("x86");
        var bootstrapX64 = BootstrapPath("x64");
        if (!File.Exists(bootstrapX86)) throw new FileNotFoundException("The x86 hook bootstrap is missing", bootstrapX86);
        if (!File.Exists(bootstrapX64)) throw new FileNotFoundException("The x64 hook bootstrap is missing", bootstrapX64);

        RemoteHooking.CreateAndInject(
            InEXEPath: exePath,
            InCommandLine: arguments,
            InProcessCreationFlags: 0,
            InOptions: InjectionOptions.Default,
            InLibraryPath_x86: bootstrapX86,
            InLibraryPath_x64: bootstrapX64,
            OutProcessId: out var processId,
            InPassThruArgs: [dllPath]);

        var process = Process.GetProcessById(processId);
        AllowSetForegroundWindow(processId);
        return process;
    }

    private static string BootstrapPath(string arch) =>
        Path.Combine(AppContext.BaseDirectory, "helpers", arch, "SKYNET.HookBootstrap.dll");

    [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    private static extern bool AllowSetForegroundWindow(int dwProcessId);
}

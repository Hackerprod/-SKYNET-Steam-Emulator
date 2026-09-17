using System.Runtime.InteropServices;
using System.Threading;
using EasyHook;

namespace SKYNET.HookBootstrap;

/// <summary>
/// EasyHook's injection entry point (constructor + Run are found by name/signature
/// convention, not by an interface member - IEntryPoint itself declares nothing).
/// Loads the real payload directly, then hooks ntdll!LdrLoadDll so a later load
/// request for a file with the SAME NAME - regardless of which directory it's
/// requested from - resolves to the copy already loaded here instead of reading a
/// second, real copy from disk. This is what a Unity game's native plugin loader
/// does: it resolves and passes a full absolute path
/// ("&lt;Product&gt;_Data\Plugins\x86_64\steam_api64.dll"), which the ordinary
/// bare-name LoadLibrary reuse trick never sees.
/// </summary>
public sealed class EntryPoint : IEntryPoint
{
    private readonly string _payloadPath;
    private nint _payloadModule;
    private LocalHook? _hook;

    public EntryPoint(RemoteHooking.IContext context, string payloadPath)
    {
        _payloadPath = payloadPath;
    }

    public void Run(RemoteHooking.IContext context, string payloadPath)
    {
        try
        {
            _payloadModule = LoadLibraryW(_payloadPath);
            if (_payloadModule != 0)
            {
                InstallRedirectHook();
            }
        }
        catch
        {
            // Best-effort. The payload is already loaded above either way - a
            // loader that resolves by bare file name still reuses it via the
            // normal Windows loader behavior even without this hook; this only
            // widens coverage to loaders that resolve a full path first.
        }

        // CreateAndInject launches the target suspended and leaves it that way
        // until this call - resume only now that the payload is loaded and the
        // hook (if it installed) is active, so nothing in the game can race
        // ahead of either.
        RemoteHooking.WakeUpProcess();

        // If Run() returns, EasyHook tears the hook down immediately. Keep this
        // thread parked for the game's entire lifetime instead - it does not
        // block the game's own main thread, which already resumed above.
        while (true)
        {
            Thread.Sleep(60000);
        }
    }

    private void InstallRedirectHook()
    {
        var ntdll = GetModuleHandle("ntdll.dll");
        if (ntdll == 0)
            return;

        var procAddress = GetProcAddress(ntdll, "LdrLoadDll");
        if (procAddress == 0)
            return;

        var targetFileName = Path.GetFileName(_payloadPath);
        var payloadModule = _payloadModule;

        LdrLoadDllDelegate original = null!;
        LdrLoadDllDelegate detour = (pathToFile, flags, moduleFileName, moduleHandle) =>
        {
            var requested = ReadUnicodeString(moduleFileName);
            if (!string.IsNullOrEmpty(requested) &&
                string.Equals(Path.GetFileName(requested), targetFileName, StringComparison.OrdinalIgnoreCase))
            {
                Marshal.WriteIntPtr(moduleHandle, payloadModule);
                return 0; // STATUS_SUCCESS - skips the real LdrLoadDll entirely,
                          // no path rewriting or extra allocation needed since we
                          // already know the module handle to hand back.
            }

            return original(pathToFile, flags, moduleFileName, moduleHandle);
        };

        original = Marshal.GetDelegateForFunctionPointer<LdrLoadDllDelegate>(procAddress);
        _hook = LocalHook.Create(procAddress, detour, this);
        // Enable the hook for every thread in the process (an empty exclusion
        // list - EasyHook hooks install inactive until a thread ACL admits them).
        _hook.ThreadACL.SetExclusiveACL([]);
    }

    /// <summary>
    /// LdrLoadDll's moduleFileName argument points at an UNICODE_STRING, not a
    /// plain string. This struct shape (a Length field immediately followed by an
    /// LPWStr-marshaled field, no explicit MaximumLength) relies on
    /// Marshal.PtrToStructure's own field alignment padding landing the marshaled
    /// pointer field at the same offset UNICODE_STRING's real Buffer field would
    /// occupy, on both x86 and x64 - it is not a coincidence, it is the standard
    /// interop trick for reading this specific NT structure without hand-rolling
    /// the padding.
    /// </summary>
    private static string? ReadUnicodeString(nint ptr)
    {
        var value = Marshal.PtrToStructure<NtUnicodeString>(ptr);
        return value.Content;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NtUnicodeString
    {
        public ushort Length;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string? Content;
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
    private delegate uint LdrLoadDllDelegate(nint pathToFile, nint flags, nint moduleFileName, nint moduleHandle);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "LoadLibraryW")]
    private static extern nint LoadLibraryW(string path);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern nint GetModuleHandle(string name);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
    private static extern nint GetProcAddress(nint module, string name);
}

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using SKYNET.Client.Models;

namespace SKYNET.Client.Services;

/// <summary>
/// Rebinds a direct Steam API import before the initial process thread runs.
/// Windows then resolves the import to the payload's absolute shadow path while
/// loading the image, so static consumers use the launcher payload without any
/// game-folder replacement or post-start loader race.
/// </summary>
internal static class SteamStaticImportRebinder
{
    private const uint MemCommitReserve = 0x3000;
    private const uint MemRelease = 0x8000;
    private const uint PageReadWrite = 0x04;

    public static void Rebind(
        IntPtr processHandle,
        string executablePath,
        string payloadPath,
        string importedModuleName)
    {
        var nameFieldRvas = PeImports.FindImportNameFieldRvas(executablePath, importedModuleName);
        if (nameFieldRvas.Count == 0)
            throw new InvalidOperationException($"No direct import of '{importedModuleName}' was found in '{executablePath}'.");

        var imageBase = ReadImageBase(processHandle, PeArch.Detect(executablePath));
        var pathBytes = Encoding.ASCII.GetBytes(Path.GetFullPath(payloadPath) + "\0");
        var remotePath = AllocateWithinImageRva(processHandle, imageBase, pathBytes.Length);
        try
        {
            WriteBytes(processHandle, remotePath, pathBytes, "payload import path");
            var pathRva = checked((uint)(remotePath.ToInt64() - imageBase));
            var pathRvaBytes = BitConverter.GetBytes(pathRva);
            foreach (var nameFieldRva in nameFieldRvas)
            {
                var nameFieldAddress = new IntPtr(checked(imageBase + nameFieldRva));
                WriteProtectedBytes(processHandle, nameFieldAddress, pathRvaBytes, "import descriptor");
            }
        }
        catch
        {
            VirtualFreeEx(processHandle, remotePath, UIntPtr.Zero, MemRelease);
            throw;
        }
    }

    private static IntPtr AllocateWithinImageRva(IntPtr processHandle, long imageBase, int byteCount)
    {
        // IMAGE_IMPORT_DESCRIPTOR stores an RVA as uint32. Requesting memory above
        // the image makes the rebinding valid for both PE32 and PE32+ images.
        const long InitialOffset = 0x01000000;
        const long RetryStride = 0x01000000;
        for (var attempt = 0; attempt < 64; attempt++)
        {
            var hint = new IntPtr(checked(imageBase + InitialOffset + RetryStride * attempt));
            var allocation = VirtualAllocEx(processHandle, hint, (UIntPtr)byteCount, MemCommitReserve, PageReadWrite);
            if (allocation == IntPtr.Zero)
                continue;

            var rva = allocation.ToInt64() - imageBase;
            if (rva >= 0 && rva <= uint.MaxValue)
                return allocation;

            VirtualFreeEx(processHandle, allocation, UIntPtr.Zero, MemRelease);
        }

        throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not allocate a valid RVA for the static Steam API import.");
    }

    private static long ReadImageBase(IntPtr processHandle, GameArch targetArch)
    {
        IntPtr pebAddress;
        if (targetArch == GameArch.X86 && IntPtr.Size == 8)
        {
            var status = NtQueryInformationProcess(processHandle, ProcessWow64Information, out pebAddress, IntPtr.Size, out _);
            if (status != 0 || pebAddress == IntPtr.Zero)
                throw new InvalidOperationException($"NtQueryInformationProcess(ProcessWow64Information) failed with status 0x{status:X8}.");
        }
        else
        {
            var status = NtQueryInformationProcess(
                processHandle,
                ProcessBasicInformation,
                out ProcessBasicInformationResult basic,
                Marshal.SizeOf<ProcessBasicInformationResult>(),
                out _);
            if (status != 0 || basic.PebBaseAddress == IntPtr.Zero)
                throw new InvalidOperationException($"NtQueryInformationProcess(ProcessBasicInformation) failed with status 0x{status:X8}.");
            pebAddress = basic.PebBaseAddress;
        }

        var pointerSize = targetArch == GameArch.X86 ? 4 : 8;
        var imageBaseField = new IntPtr(checked(pebAddress.ToInt64() + (pointerSize == 8 ? 0x10 : 0x08)));
        var bytes = new byte[pointerSize];
        if (!ReadProcessMemory(processHandle, imageBaseField, bytes, bytes.Length, out var read) || read.ToInt64() != bytes.Length)
            throw new Win32Exception(Marshal.GetLastWin32Error(), "ReadProcessMemory failed while reading the target image base from its PEB.");

        return pointerSize == 8 ? BitConverter.ToInt64(bytes, 0) : BitConverter.ToUInt32(bytes, 0);
    }

    private static void WriteProtectedBytes(IntPtr processHandle, IntPtr address, byte[] bytes, string operation)
    {
        if (!VirtualProtectEx(processHandle, address, (UIntPtr)bytes.Length, PageReadWrite, out var previousProtect))
            throw new Win32Exception(Marshal.GetLastWin32Error(), $"VirtualProtectEx failed for {operation}.");

        try
        {
            WriteBytes(processHandle, address, bytes, operation);
        }
        finally
        {
            VirtualProtectEx(processHandle, address, (UIntPtr)bytes.Length, previousProtect, out _);
        }
    }

    private static void WriteBytes(IntPtr processHandle, IntPtr address, byte[] bytes, string operation)
    {
        if (!WriteProcessMemory(processHandle, address, bytes, bytes.Length, out var written) || written.ToInt64() != bytes.Length)
            throw new Win32Exception(Marshal.GetLastWin32Error(), $"WriteProcessMemory failed for {operation}.");
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flAllocationType, uint flProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint dwFreeType);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesRead);

    private const int ProcessBasicInformation = 0;
    private const int ProcessWow64Information = 26;

    [DllImport("ntdll.dll")]
    private static extern int NtQueryInformationProcess(
        IntPtr processHandle,
        int processInformationClass,
        out ProcessBasicInformationResult processInformation,
        int processInformationLength,
        out int returnLength);

    [DllImport("ntdll.dll")]
    private static extern int NtQueryInformationProcess(
        IntPtr processHandle,
        int processInformationClass,
        out IntPtr processInformation,
        int processInformationLength,
        out int returnLength);

    [StructLayout(LayoutKind.Sequential)]
    private struct ProcessBasicInformationResult
    {
        public IntPtr Reserved1;
        public IntPtr PebBaseAddress;
        public IntPtr Reserved2_0;
        public IntPtr Reserved2_1;
        public IntPtr UniqueProcessId;
        public IntPtr Reserved3;
    }
}

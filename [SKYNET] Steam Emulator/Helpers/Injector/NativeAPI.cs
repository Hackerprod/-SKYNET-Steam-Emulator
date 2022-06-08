using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Helpers
{
    internal static class NativeAPI_x86
    {
        private const string DllName = "EasyHook32.dll";

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern string RtlGetLastErrorStringCopy();

        public static string RtlGetLastErrorString()
        {
            return RtlGetLastErrorStringCopy();
        }

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int RtlGetLastError();

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void LhUninstallAllHooks();

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhInstallHook(IntPtr InEntryPoint, IntPtr InHookProc, IntPtr InCallback, IntPtr OutHandle);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhUninstallHook(IntPtr RefHandle);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhWaitForPendingRemovals();

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhSetInclusiveACL([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] int[] InThreadIdList, int InThreadCount, IntPtr InHandle);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhSetExclusiveACL([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] int[] InThreadIdList, int InThreadCount, IntPtr InHandle);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhSetGlobalInclusiveACL([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] int[] InThreadIdList, int InThreadCount);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhSetGlobalExclusiveACL([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] int[] InThreadIdList, int InThreadCount);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhIsThreadIntercepted(IntPtr InHandle, int InThreadID, out bool OutResult);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhBarrierGetCallback(out IntPtr OutValue);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhBarrierGetReturnAddress(out IntPtr OutValue);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhBarrierGetAddressOfReturnAddress(out IntPtr OutValue);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhBarrierBeginStackTrace(out IntPtr OutBackup);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhBarrierEndStackTrace(IntPtr OutBackup);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhBarrierGetCallingModule(out IntPtr OutValue);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int DbgAttachDebugger();

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int DbgGetThreadIdByHandle(IntPtr InThreadHandle, out int OutThreadId);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int DbgGetProcessIdByHandle(IntPtr InProcessHandle, out int OutProcessId);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int DbgHandleToObjectName(IntPtr InNamedHandle, IntPtr OutNameBuffer, int InBufferSize, out int OutRequiredSize);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern int RhInjectLibrary(int InTargetPID, int InWakeUpTID, int InInjectionOptions, string InLibraryPath_x86, string InLibraryPath_x64, IntPtr InPassThruBuffer, int InPassThruSize);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int RhIsX64Process(int InProcessId, out bool OutResult);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool RhIsAdministrator();

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int RhGetProcessToken(int InProcessId, out IntPtr OutToken);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern int RtlInstallService(string InServiceName, string InExePath, string InChannelName);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int RhWakeUpProcess();

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern int RtlCreateSuspendedProcess(string InEXEPath, string InCommandLine, int InProcessCreationFlags, out int OutProcessId, out int OutThreadId);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern int RhInstallDriver(string InDriverPath, string InDriverName);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int RhInstallSupportDriver();

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool RhIsX64System();

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GacCreateContext();

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void GacReleaseContext(ref IntPtr RefContext);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern bool GacInstallAssembly(IntPtr InContext, string InAssemblyPath, string InDescription, string InUniqueID);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern bool GacUninstallAssembly(IntPtr InContext, string InAssemblyName, string InDescription, string InUniqueID);

        [DllImport("EasyHook32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern int LhGetHookBypassAddress(IntPtr handle, out IntPtr address);
    }

    internal static class NativeAPI_x64
    {
        private const string DllName = "EasyHook64.dll";

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern string RtlGetLastErrorStringCopy();

        public static string RtlGetLastErrorString()
        {
            return RtlGetLastErrorStringCopy();
        }

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int RtlGetLastError();

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void LhUninstallAllHooks();

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhInstallHook(IntPtr InEntryPoint, IntPtr InHookProc, IntPtr InCallback, IntPtr OutHandle);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhUninstallHook(IntPtr RefHandle);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhWaitForPendingRemovals();

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhSetInclusiveACL([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] int[] InThreadIdList, int InThreadCount, IntPtr InHandle);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhSetExclusiveACL([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] int[] InThreadIdList, int InThreadCount, IntPtr InHandle);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhSetGlobalInclusiveACL([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] int[] InThreadIdList, int InThreadCount);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhSetGlobalExclusiveACL([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] int[] InThreadIdList, int InThreadCount);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhIsThreadIntercepted(IntPtr InHandle, int InThreadID, out bool OutResult);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhBarrierGetCallback(out IntPtr OutValue);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhBarrierGetReturnAddress(out IntPtr OutValue);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhBarrierGetAddressOfReturnAddress(out IntPtr OutValue);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhBarrierBeginStackTrace(out IntPtr OutBackup);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhBarrierEndStackTrace(IntPtr OutBackup);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int LhBarrierGetCallingModule(out IntPtr OutValue);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int DbgAttachDebugger();

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int DbgGetThreadIdByHandle(IntPtr InThreadHandle, out int OutThreadId);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int DbgGetProcessIdByHandle(IntPtr InProcessHandle, out int OutProcessId);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int DbgHandleToObjectName(IntPtr InNamedHandle, IntPtr OutNameBuffer, int InBufferSize, out int OutRequiredSize);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern int RhInjectLibrary(int InTargetPID, int InWakeUpTID, int InInjectionOptions, string InLibraryPath_x86, string InLibraryPath_x64, IntPtr InPassThruBuffer, int InPassThruSize);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int RhIsX64Process(int InProcessId, out bool OutResult);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool RhIsAdministrator();

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int RhGetProcessToken(int InProcessId, out IntPtr OutToken);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern int RtlInstallService(string InServiceName, string InExePath, string InChannelName);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern int RtlCreateSuspendedProcess(string InEXEPath, string InCommandLine, int InProcessCreationFlags, out int OutProcessId, out int OutThreadId);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int RhWakeUpProcess();

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern int RhInstallDriver(string InDriverPath, string InDriverName);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int RhInstallSupportDriver();

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool RhIsX64System();

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GacCreateContext();

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void GacReleaseContext(ref IntPtr RefContext);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern bool GacInstallAssembly(IntPtr InContext, string InAssemblyPath, string InDescription, string InUniqueID);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern bool GacUninstallAssembly(IntPtr InContext, string InAssemblyName, string InDescription, string InUniqueID);

        [DllImport("EasyHook64.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern int LhGetHookBypassAddress(IntPtr handle, out IntPtr address);
    }


    [Serializable]
    internal class ManagedRemoteInfo
    {
        public string ChannelName;

        public object[] UserParams;

        public string UserLibrary;

        public string UserLibraryName;

        public int HostPID;

        public bool RequireStrongName;
    }
}

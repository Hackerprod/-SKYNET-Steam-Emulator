using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Helpers
{
    public class Injector
    {
        public static readonly bool Is64Bit = IntPtr.Size == 8;
        public static void CreateAndInject(string InEXEPath, string InCommandLine, int InProcessCreationFlags, string InLibraryPath_x86, string InLibraryPath_x64, out int OutProcessId, params object[] InPassThruArgs)
        {
            RtlCreateSuspendedProcess(InEXEPath, InCommandLine, InProcessCreationFlags, out var OutProcessId2, out var OutThreadId);
            try
            {
                InjectEx(GetCurrentProcessId(), OutProcessId2, OutThreadId, 536870912, InLibraryPath_x86, InLibraryPath_x64, false, false, false, InPassThruArgs);
                OutProcessId = OutProcessId2;
            }
            catch (Exception ex2)
            {
                try
                {
                    Process.GetProcessById(OutProcessId2).Kill();
                }
                catch (Exception)
                {
                }
                throw ex2;
            }
        }

        internal static void InjectEx(int InHostPID, int InTargetPID, int InWakeUpTID, int InNativeOptions, string InLibraryPath_x86, string InLibraryPath_x64, bool InCanBypassWOW64, bool InCanCreateService, bool InRequireStrongName, params object[] InPassThruArgs)
        {
            MemoryStream memoryStream = new MemoryStream();
            HelperServiceInterface.BeginInjection(InTargetPID);
            try
            {
                ManagedRemoteInfo managedRemoteInfo = new ManagedRemoteInfo();
                managedRemoteInfo.HostPID = InHostPID;
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                List<object> list = new List<object>();
                if (InPassThruArgs != null)
                {
                    foreach (object graph in InPassThruArgs)
                    {
                        using MemoryStream memoryStream2 = new MemoryStream();
                        binaryFormatter.Serialize(memoryStream2, graph);
                        list.Add(memoryStream2.ToArray());
                    }
                }
                managedRemoteInfo.UserParams = list.ToArray();
                managedRemoteInfo.RequireStrongName = InRequireStrongName;
                //    GCHandle gCHandle = PrepareInjection(managedRemoteInfo, ref InLibraryPath_x86, ref InLibraryPath_x64, memoryStream);
                //    try
                //    {
                //        int inErrorCode;
                //        switch (inErrorCode = RhInjectLibraryEx(InTargetPID, InWakeUpTID, 1 | InNativeOptions, Path.Combine(Config.HelperLibraryLocation, "EasyLoad32.dll"), Path.Combine(Config.HelperLibraryLocation, "EasyLoad64.dll"), gCHandle.AddrOfPinnedObject(), (int)memoryStream.Length))
                //        {
                //            case -1073702760:
                //                if (InCanBypassWOW64)
                //                {
                //                    WOW64Bypass.Inject(InHostPID, InTargetPID, InWakeUpTID, InNativeOptions, InLibraryPath_x86, InLibraryPath_x64, InRequireStrongName, InPassThruArgs);
                //                    break;
                //                }
                //                throw new AccessViolationException("Unable to inject library into target process.");
                //            case -1073741790:
                //                if (InCanCreateService)
                //                {
                //                    ServiceMgmt.Inject(InHostPID, InTargetPID, InWakeUpTID, InNativeOptions, InLibraryPath_x86, InLibraryPath_x64, InRequireStrongName, InPassThruArgs);
                //                }
                //                else
                //                {
                //                    NativeAPI.Force(inErrorCode);
                //                }
                //                break;
                //            case 0:
                //                HelperServiceInterface.WaitForInjection(InTargetPID);
                //                break;
                //            default:
                //                NativeAPI.Force(inErrorCode);
                //                break;
                //        }
                //    }
                //    finally
                //    {
                //        gCHandle.Free();
                //    }
            }
            finally
            {
                HelperServiceInterface.EndInjection(InTargetPID);
            }
        }

        public static void RtlCreateSuspendedProcess(string InEXEPath, string InCommandLine, int InProcessCreationFlags, out int OutProcessId, out int OutThreadId)
        {
            if (Is64Bit)
            {
                NativeAPI_x64.RtlCreateSuspendedProcess(InEXEPath, InCommandLine, InProcessCreationFlags, out OutProcessId, out OutThreadId);
            }
            else
            {
                NativeAPI_x86.RtlCreateSuspendedProcess(InEXEPath, InCommandLine, InProcessCreationFlags, out OutProcessId, out OutThreadId);
            }
        }

        public static void WakeUpProcess()
        {
            if (Is64Bit)
            {
                NativeAPI_x64.RhWakeUpProcess();
            }
            else
            {
                NativeAPI_x86.RhWakeUpProcess();
            }
        }

        public static int RhInjectLibraryEx(int InTargetPID, int InWakeUpTID, int InInjectionOptions, string InLibraryPath_x86, string InLibraryPath_x64, IntPtr InPassThruBuffer, int InPassThruSize)
        {
            if (Is64Bit)
            {
                return NativeAPI_x64.RhInjectLibrary(InTargetPID, InWakeUpTID, InInjectionOptions, InLibraryPath_x86, InLibraryPath_x64, InPassThruBuffer, InPassThruSize);
            }
            return NativeAPI_x86.RhInjectLibrary(InTargetPID, InWakeUpTID, InInjectionOptions, InLibraryPath_x86, InLibraryPath_x64, InPassThruBuffer, InPassThruSize);
        }

        [DllImport("kernel32.dll")]
        public static extern int GetCurrentProcessId();
    }
}

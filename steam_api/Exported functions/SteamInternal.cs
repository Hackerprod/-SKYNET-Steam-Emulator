using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks.Implementation;
using SKYNET.Types;

namespace SKYNET.Steamworks.Exported
{
    public class SteamInternal : BaseCalls
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamInternal_FindOrCreateUserInterface(IntPtr hSteamUser, [MarshalAs(UnmanagedType.LPStr)] string pszVersion)
        {
            Write($"SteamInternal_FindOrCreateUserInterface {pszVersion}");
            return SteamEmulator.SteamClient.GetISteamGenericInterface((int)SteamEmulator.HSteamUser, (int)SteamEmulator.HSteamPipe, pszVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamInternal_FindOrCreateGameServerInterface(int hSteamUser, [MarshalAs(UnmanagedType.LPStr)] string pszVersion)
        {
            Write($"SteamInternal_FindOrCreateGameServerInterface {pszVersion}");
            return InterfaceManager.FindOrCreateInterface(pszVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamInternal_CreateInterface([MarshalAs(UnmanagedType.LPStr)] string pszVersion)
        {
            Write($"SteamInternal_CreateInterface {pszVersion}");
            return InterfaceManager.FindOrCreateInterface(pszVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamInternal_GameServer_Init(IntPtr unIP, IntPtr usPort, IntPtr usGamePort, IntPtr usQueryPort, IntPtr eServerMode, [MarshalAs(UnmanagedType.LPStr)] string pchVersionString)
        {
            Write($"SteamInternal_GameServer_Init {pchVersionString}");
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public unsafe static IntPtr SteamInternal_ContextInit(IntPtr contextInitData_ptr)
        {
            long counter = Marshal.ReadInt64(contextInitData_ptr + 8);

            Write($"SteamInternal_ContextInit Counter: {counter}");

            IntPtr steamApiContext_ptr = (IntPtr)contextInitData_ptr + 16;

            try
            {
                //ContextInitData_x64 context = Marshal.GetDelegateForFunctionPointer<ContextInitData_x64>(contextInitData_ptr);
                //Write(context == null);
            }
            catch (Exception ex)
            {
                Write($"Error in SteamInternal_ContextInit: {ex.Message} \n{ex.StackTrace}");
            }

            //CSteamInterfaceContext context = Interface.Bind<CSteamInterfaceContext>(steamApiContext_ptr);
           

            if (counter != 1)
            {
                Marshal.StructureToPtr(counter + 1, contextInitData_ptr + 8, false);

                //Marshal.StructureToPtr(SteamEmulator.Context, steamApiContext_ptr, false);
            }

            return steamApiContext_ptr;
        }

        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        //public unsafe static void* SteamInternal_ContextInit_NotWork(void* contextInitData_ptr)
        //{
        //    ContextInitData_x64* contextInitData = (ContextInitData_x64*)contextInitData_ptr;
        //    Write($"SteamInternal_ContextInit Counter: {contextInitData->counter}");

        //    if (contextInitData->counter != 1)
        //    {
        //        CSteamApiContext steamApiContext = contextInitData->Context;
        //        contextInitData->counter = 1;
        //        steamApiContext.Init();
        //        return &contextInitData->Context;
        //    }

        //    return &contextInitData->Context;
        //}

        public struct ContextInitData_x64
        {
            private long pFn;                   //64 bites space 
            public long counter;
            public CSteamApiContext Context;
        }

        public struct ContextInitData_x86
        {
            private uint pFn;                   //32 bites space
            public long counter;
            public CSteamApiContext Context;
        }
    }
}

/*
    This method implementation register SteamMasterServerUpdater and Crash

    SteamInternal_ContextInit Counter: 0, Context: -1823544080
    SteamInternal_ContextInit Counter: 0, Context: -1823544080
    SteamInternal_ContextInit Counter: 0, Context: -1823544080
    SteamMasterServerUpdater: ClearAllKeyValues                         <---
    SteamAPI: SteamAPI_SetMiniDumpComment
     
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static unsafe void* SteamInternal_ContextInit(IntPtr contextInitData_ptr)
    {
        ContextInitData_x64 contextInitData = Marshal.PtrToStructure<ContextInitData_x64>(contextInitData_ptr);
        Write($"SteamInternal_ContextInit Counter: {contextInitData.counter}, Context: {(int)&contextInitData.Context}");

        if (contextInitData.counter != 1)
        {
            contextInitData.Context.Init();
            return &contextInitData.Context;
        }

        return &contextInitData.Context;

    }
   
     *//// <summary>
       ///     Class representing a block of memory allocated in the local process.
       /// </summary>



using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using SKYNET.Managers;
using SKYNET.Steamworks.Implementation;
using SKYNET.Types;

using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;

namespace SKYNET.Steamworks.Exported
{
    public unsafe class SteamInternal
    {
        public static pFnDelegate pFn;

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamInternal_FindOrCreateUserInterface(int hSteamUser, [MarshalAs(UnmanagedType.LPStr)] string pszVersion)
        {
            Write($"SteamInternal_FindOrCreateUserInterface {pszVersion}");
            return InterfaceManager.FindOrCreateInterface(pszVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamInternal_FindOrCreateGameServerInterface(HSteamUser hSteamUser, [MarshalAs(UnmanagedType.LPStr)] string pszVersion)
        {
            Write($"SteamInternal_FindOrCreateGameServerInterface {pszVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, 1, pszVersion, true);
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
        public static IntPtr SteamInternal_ContextInit(IntPtr contextInitData_ptr)
        {
            IntPtr apiContext_ptr = ByPassFunction(contextInitData_ptr);

            if (apiContext_ptr != IntPtr.Zero)
            {
                return apiContext_ptr;
            }
            
            try
            {
                if (modCommon.Is64Bit())
                {
                    ContextInitData_x64 Context = Marshal.PtrToStructure<ContextInitData_x64>(contextInitData_ptr);
                    apiContext_ptr = contextInitData_ptr + 16;
                    if (Context.counter != 1)
                    {
                        Write($"SteamInternal_ContextInit");
                        Marshal.WriteInt64(contextInitData_ptr, 8, 1);
                        pFn = Marshal.GetDelegateForFunctionPointer<pFnDelegate>(Context.pFn);
                        pFn(apiContext_ptr);
                    }
                }
                else
                {
                    var Context = Marshal.PtrToStructure<ContextInitData_x86>(contextInitData_ptr);
                    apiContext_ptr = contextInitData_ptr + 8;
                    if (Context.counter != 1)
                    {
                        Write($"SteamInternal_ContextInit");
                        Marshal.WriteInt32(contextInitData_ptr, 4, 1);
                        pFn = Marshal.GetDelegateForFunctionPointer<pFnDelegate>(Context.pFn);
                        pFn(apiContext_ptr);
                    }
                }
            }
            catch
            {
            }
            return apiContext_ptr;
        }

        #region bypass functions

        private static IntPtr ByPassFunction(IntPtr contextInitData_ptr)
        {
            if (File.Exists(Path.Combine(modCommon.GetPath(), "steam_api_.dll")))
            {
                return SteamInternal_ContextInit86(contextInitData_ptr);
            }
            if (File.Exists(Path.Combine(modCommon.GetPath(), "steam_api64_.dll")))
            {
                return SteamInternal_ContextInit64(contextInitData_ptr);
            }
            return IntPtr.Zero;
        }

        [DllImport("steam_api_.dll",   EntryPoint = "SteamInternal_ContextInit", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SteamInternal_ContextInit86(IntPtr context);

        [DllImport("steam_api64_.dll", EntryPoint = "SteamInternal_ContextInit", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SteamInternal_ContextInit64(IntPtr context);

        #endregion

        private static void Write(string v)
        {
            SteamEmulator.Write("", v);
        }
    }
}


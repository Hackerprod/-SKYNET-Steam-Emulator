﻿using System;
using System.Runtime.InteropServices;
using SKYNET.Managers;
using SKYNET.Types;

using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;

namespace SKYNET.Steamworks.Exported
{
    public class SteamInternal
    {
        static SteamInternal()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamInternal_SteamAPI_Init()
        {
            Write($"SteamInternal_SteamAPI_Init");
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamInternal_GameServer_Init_V2()
        {
            Write($"SteamInternal_GameServer_Init_V2");
            return true;
        }

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
        public static bool SteamInternal_GameServer_Init(uint unIP, int usPort, int usGamePort, uint usQueryPort, uint eServerMode, string pchVersionString)
        {
            var unFlags = eServerMode == (int)EServerMode.AuthenticationAndSecure ? Constants.k_unServerFlagSecure : 0;
            var result = SteamEmulator.SteamGameServer.InitGameServer(unIP, usPort, (int)usQueryPort, unFlags, SteamEmulator.AppID, pchVersionString);
            Write($"SteamInternal_GameServer_Init = {result}");
            return result;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamInternal_ContextInit(IntPtr contextInitData_ptr)
        {
            var contextData = Marshal.PtrToStructure<ContextInitData>(contextInitData_ptr);
            var apiContext_ptr = contextInitData_ptr + (IntPtr.Size * 2);                       // 16 for x64 process, 8 for x86 process
            var counter = Marshal.ReadInt32(contextInitData_ptr, IntPtr.Size);

            if (counter != 1)
            {
                Write($"SteamInternal_ContextInit");
                Marshal.WriteInt32(contextInitData_ptr, IntPtr.Size, 1);
                contextData.pFn(apiContext_ptr);
            }

            return apiContext_ptr;
        }

        private static void Write(object msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}


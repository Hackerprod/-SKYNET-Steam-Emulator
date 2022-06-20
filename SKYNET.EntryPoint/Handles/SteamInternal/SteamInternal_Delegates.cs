using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

using HSteamUser = System.UInt32;

namespace SKYNET.Hook.Handles
{
    public partial class SteamInternal 
    {
        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamInternal_FindOrCreateUserInterfaceDelegate(int hSteamUser, [MarshalAs(UnmanagedType.LPStr)] string pszVersion);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamInternal_FindOrCreateGameServerInterfaceDelegate(HSteamUser hSteamUser, [MarshalAs(UnmanagedType.LPStr)] string pszVersion);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamInternal_CreateInterfaceDelegate([MarshalAs(UnmanagedType.LPStr)] string version);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamInternal_GameServer_InitDelegate(uint unIP, int usPort, int usGamePort, uint usQueryPort, uint eServerMode, string pchVersionString);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamInternal_ContextInitDelegate(IntPtr pContextInitData);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void pFn(IntPtr ctx);

        public SteamInternal_FindOrCreateUserInterfaceDelegate _SteamInternal_FindOrCreateUserInterface;
        public SteamInternal_FindOrCreateGameServerInterfaceDelegate _SteamInternal_FindOrCreateGameServerInterfaceDelegate;
        public SteamInternal_CreateInterfaceDelegate _SteamInternal_CreateInterfaceDelegate;
        public SteamInternal_GameServer_InitDelegate _SteamInternal_GameServer_InitDelegate;
        public unsafe SteamInternal_ContextInitDelegate _SteamInternal_ContextInitDelegate;
        public static pFn _pFn;

    }
}

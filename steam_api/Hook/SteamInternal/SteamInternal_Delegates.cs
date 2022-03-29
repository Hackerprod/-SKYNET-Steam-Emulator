using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SKYNET;
using SKYNET.Helper;
using SKYNET.Types;

namespace SKYNET.Hook.Handles
{
    public partial class SteamInternal 
    {
        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamInternal_FindOrCreateUserInterfaceDelegate(int hSteamUser, [MarshalAs(UnmanagedType.LPStr)] string pszVersion);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamInternal_FindOrCreateGameServerInterfaceDelegate(int hSteamUser, [MarshalAs(UnmanagedType.LPStr)] string pszVersion);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamInternal_CreateInterfaceDelegate([MarshalAs(UnmanagedType.LPStr)] string version);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamInternal_GameServer_InitDelegate(IntPtr unIP, IntPtr usPort, IntPtr usGamePort, IntPtr usQueryPort, IntPtr eServerMode, IntPtr pchVersionString);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamInternal_ContextInitDelegate(IntPtr pContextInitData);


        public SteamInternal_FindOrCreateUserInterfaceDelegate _SteamInternal_FindOrCreateUserInterface;
        public SteamInternal_FindOrCreateGameServerInterfaceDelegate _SteamInternal_FindOrCreateGameServerInterfaceDelegate;
        public SteamInternal_CreateInterfaceDelegate _SteamInternal_CreateInterfaceDelegate;
        public SteamInternal_GameServer_InitDelegate _SteamInternal_GameServer_InitDelegate;
        public SteamInternal_ContextInitDelegate _SteamInternal_ContextInitDelegate;
    }
}

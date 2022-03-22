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
using SKYNET.Interface;
using SKYNET.Types;
using Steamworks.Core;

namespace SKYNET.Hook
{
    public partial class SteamInternal : BaseHook
    {
        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate IntPtr SteamInternal_FindOrCreateUserInterfaceDelegate(IntPtr hSteamUser, [MarshalAs(UnmanagedType.LPStr)] string pszVersion);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate IntPtr SteamInternal_FindOrCreateGameServerInterfaceDelegate(int hSteamUser, [MarshalAs(UnmanagedType.LPStr)] string pszVersion);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate IntPtr SteamInternal_CreateInterfaceDelegate([MarshalAs(UnmanagedType.LPStr)] string version);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate bool SteamInternal_GameServer_InitDelegate(IntPtr unIP, IntPtr usPort, IntPtr usGamePort, IntPtr usQueryPort, IntPtr eServerMode, IntPtr pchVersionString);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        private delegate IntPtr SteamInternal_ContextInitDelegate(IntPtr pContextInitData);


        private SteamInternal_FindOrCreateUserInterfaceDelegate _SteamInternal_FindOrCreateUserInterface;
        private SteamInternal_FindOrCreateGameServerInterfaceDelegate _SteamInternal_FindOrCreateGameServerInterfaceDelegate;
        private SteamInternal_CreateInterfaceDelegate _SteamInternal_CreateInterfaceDelegate;
        private SteamInternal_GameServer_InitDelegate _SteamInternal_GameServer_InitDelegate;
        private SteamInternal_ContextInitDelegate _SteamInternal_ContextInitDelegate;
    }
}

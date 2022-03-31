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
    public static unsafe void* SteamInternal_ContextInit(void* c_contextPointer)
    {
        ContextInitData* CreatedContext = (ContextInitData*)c_contextPointer;

        if (CreatedContext->Context.SteamClient() != SteamEmulator.SteamClient.BaseAddress)
        {
            Main.Write("SteamInternal_ContextInit initializing");
            CreatedContext->Context.Init();
            CreatedContext->counter = 1;
        }

        return &CreatedContext->Context;
    }

    public struct ContextInitData
    {
        public CSteamApiContext Context;
        public uint counter;
    }
}

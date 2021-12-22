using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SKYNET.Types;

public class SteamInternal : BaseCalls
{
    public static uint global_counter { get; set; }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamInternal_FindOrCreateUserInterface(IntPtr hSteamUser, [MarshalAs(UnmanagedType.FunctionPtr)] string pszVersion)
    {
        DEBUG($"SteamInternal_FindOrCreateUserInterface {pszVersion}");
        return IntPtr.Zero;
    }


    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamInternal_FindOrCreateGameServerInterface(IntPtr hSteamUser, [MarshalAs(UnmanagedType.FunctionPtr)] string pszVersion)
    {
        DEBUG($"SteamInternal_FindOrCreateGameServerInterface {pszVersion}");
        return IntPtr.Zero;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamInternal_ContextInit(ContextInitData pContextInitData)
    {
        DEBUG("SteamInternal_ContextInit");

        if (pContextInitData.counter != global_counter)
        {
            pContextInitData.counter = global_counter;
        }

        return pContextInitData.ctx;
    }
    public struct ContextInitData
    {
        public uint counter;
        public IntPtr ctx;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamInternal_CreateInterface([MarshalAs(UnmanagedType.FunctionPtr)] string ver)
    {
        DEBUG($"SteamInternal_CreateInterface {ver}");
        return (IntPtr)4516351; //Testing
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamInternal_GameServer_Init(IntPtr unIP, IntPtr usPort, IntPtr usGamePort, IntPtr usQueryPort, IntPtr eServerMode, IntPtr pchVersionString)
    {
        DEBUG($"SteamInternal_GameServer_Init");
        return true;
    }
}


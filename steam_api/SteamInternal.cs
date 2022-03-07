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
    public static uint global_counter;

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
    public static IntPtr SteamInternal_ContextInit(IntPtr pContextInitData)
    {
        ContextInitData contextInitData = Marshal.PtrToStructure<ContextInitData>(pContextInitData);
        if (contextInitData.counter != global_counter)
        {
            DEBUG("SteamInternal_ContextInit initializing\n");
            contextInitData.counter = global_counter;
        }

        return contextInitData.ctx;
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


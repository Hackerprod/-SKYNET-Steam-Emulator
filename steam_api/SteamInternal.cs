using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SKYNET;
using SKYNET.Types;

public class SteamInternal : BaseCalls
{
    public static uint global_counter;

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamInternal_FindOrCreateUserInterface(IntPtr hSteamUser, [MarshalAs(UnmanagedType.FunctionPtr)] string pszVersion)
    {
        Write($"SteamInternal_FindOrCreateUserInterface {pszVersion}");
        return IntPtr.Zero;
    }


    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamInternal_FindOrCreateGameServerInterface(IntPtr hSteamUser, [MarshalAs(UnmanagedType.FunctionPtr)] string pszVersion)
    {
        Write($"SteamInternal_FindOrCreateGameServerInterface {pszVersion}");
        return IntPtr.Zero;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamInternal_ContextInit(IntPtr pContextInitData)
    {
        //var LUser = SteamClient.LocalUser;
        //SteamContext contextInitData = Marshal.PtrToStructure<SteamContext>(pContextInitData);

        //Write($"SteamInternal_ContextInit LocalUser:{(IntPtr)LUser.m_HSteamUser}, Flag:{contextInitData.Flag}, InitContext:{contextInitData.InitContext}, Out:{contextInitData.Out}");

        //if (contextInitData.Flag != (IntPtr)LUser.m_HSteamUser)
        //{
        //    contextInitData.Flag = (IntPtr)LUser.m_HSteamUser;
        //    //Initializar Context
        //}

        //return contextInitData.InitContext;

        ContextInitData contextInitData = Marshal.PtrToStructure<ContextInitData>(pContextInitData);

        Write($"SteamInternal_ContextInit GlobalCounter:{global_counter}, Counter:{contextInitData.counter}, Context:{contextInitData.ctx}");

        if (contextInitData.counter != global_counter)
        {
            contextInitData.counter = global_counter;
        }

        return contextInitData.ctx;
    }
    struct SteamContext
    {
        public IntPtr InitContext;
        public IntPtr Flag;
        public IntPtr Out;
    };

    public struct ContextInitData
    {
        public uint counter;
        public IntPtr ctx;
    }


    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamInternal_CreateInterface([MarshalAs(UnmanagedType.FunctionPtr)] string ver)
    {
        Write($"SteamInternal_CreateInterface {ver}");
        return (IntPtr)4516351; //Testing
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamInternal_GameServer_Init(IntPtr unIP, IntPtr usPort, IntPtr usGamePort, IntPtr usQueryPort, IntPtr eServerMode, IntPtr pchVersionString)
    {
        Write($"SteamInternal_GameServer_Init");
        return true;
    }
}


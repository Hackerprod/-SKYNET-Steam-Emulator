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
    public static IntPtr SteamInternal_CreateInterface([MarshalAs(UnmanagedType.LPStr)] string version)
    {
        Write($"SteamInternal_CreateInterface {version}");
        return InterfaceManager.FindOrCreateInterface(version);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamInternal_GameServer_Init(IntPtr unIP, IntPtr usPort, IntPtr usGamePort, IntPtr usQueryPort, IntPtr eServerMode, IntPtr pchVersionString)
    {
        Write($"SteamInternal_GameServer_Init");
        return true;
    }

    #region For test purposes

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamInternal_ContextInit(IntPtr pContextInitData)
    {
        ContextInitData contextInitData = Marshal.PtrToStructure<ContextInitData>(pContextInitData);

        if (contextInitData.counter != 1)
        {
            Write($"SteamInternal_ContextInit");

            IntPtr MemoryAddress = MemoryHelper.MemoryAddress(SteamEmulator.Context);

            return MemoryAddress == IntPtr.Zero ? contextInitData.Context : MemoryAddress;
        }
        return contextInitData.Context;
    }

    public unsafe struct ContextInitData
    {
        public IntPtr Context;
        public uint counter;
    }

    #endregion

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public unsafe static CSteamApiContext* SteamInternal_ContextInit(ContextInitData* pContextInitData)
    //{
    //    Write($"SteamInternal_ContextInit Counter: {pContextInitData->counter}, SteamUGC {pContextInitData->Context->SteamClient()}");

    //    ////var sa = pContextInitData->Context->m_pSteamClient.ToInt32();
    //    ////Write($"wuassa");

    //    return steamInternal_ContextInit(pContextInitData);

    //    //pContextInitData->Context->Clear();

    //    //if (pContextInitData->counter == 0)
    //    //{
    //    //    pContextInitData->counter = 1;
    //    //    pContextInitData->Context->Init();
    //    //}

    //    return pContextInitData->Context;
    //}

    //public unsafe struct ContextInitData
    //{
    //    public CSteamApiContext* Context;
    //    public uint counter;
    //}
}

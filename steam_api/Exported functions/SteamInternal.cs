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

    public static IntPtr SteamInternal_ContextInit(IntPtr c_contextPointer)
    {
        ContextInitData CreatedContext = Marshal.PtrToStructure<ContextInitData>(c_contextPointer);
        Write($"SteamInternal_ContextInit Counter: {CreatedContext.ctx}");

        //CSteamApiContext* context = &CreatedContext.ctx;

        //if (CreatedContext->counter == 0)
        //{
        //    CreatedContext->counter = 1;
        //}

        //context->m_pSteamClient = SteamEmulator.SteamClient.BaseAddress;
        //context->m_pSteamUser = SteamEmulator.SteamUser.BaseAddress;
        //context->m_pSteamFriends = SteamEmulator.SteamFriends.BaseAddress;
        //context->m_pSteamUtils = SteamEmulator.SteamUtils.BaseAddress;
        //context->m_pSteamMatchmaking = SteamEmulator.SteamMatchmaking.BaseAddress;
        //context->m_pSteamMatchmakingServers = SteamEmulator.SteamMatchMakingServers.BaseAddress;
        //context->m_pSteamUserStats = SteamEmulator.SteamUserStats.BaseAddress;
        //context->m_pSteamApps = SteamEmulator.SteamApps.BaseAddress;
        //context->m_pSteamNetworking = SteamEmulator.SteamNetworking.BaseAddress;
        //context->m_pSteamRemoteStorage = SteamEmulator.SteamMusicRemote.BaseAddress;
        //context->m_pSteamScreenshots = SteamEmulator.SteamScreenshots.BaseAddress;
        //context->m_pSteamHTTP = SteamEmulator.SteamHTTP.BaseAddress;
        //context->m_pSteamController = SteamEmulator.SteamController.BaseAddress;
        //context->m_pSteamUGC = SteamEmulator.SteamUGC.BaseAddress;
        //context->m_pSteamAppList = SteamEmulator.SteamAppList.BaseAddress;
        //context->m_pSteamMusic = SteamEmulator.SteamMusic.BaseAddress;
        //context->m_pSteamMusicRemote = SteamEmulator.SteamMusicRemote.BaseAddress;
        //context->m_pSteamHTMLSurface = SteamEmulator.SteamHTMLSurface.BaseAddress;
        //context->m_pSteamInventory = SteamEmulator.SteamInventory.BaseAddress;
        //context->m_pSteamVideo = SteamEmulator.SteamVideo.BaseAddress;

        return CreatedContext.ctx;
    }

    public struct ContextInitData
    {
        public IntPtr ctx;
        public uint counter;
    }
}

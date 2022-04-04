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

    public static uint global_counter = 1;

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static unsafe void* SteamInternal_ContextInit(void* c_contextPointer)
    {
        ContextInitData* CreatedContext = (ContextInitData*)c_contextPointer;
        Log.Write($"SteamInternal_ContextInit Counter: {CreatedContext->counter}");

        CSteamApiContext* context = &CreatedContext->Context;


        if (CreatedContext->counter != 1)
        {
            CreatedContext->counter = 1;

            //context->m_pSteamClient = SteamEmulator.SteamClient.MemoryAddress;
            //context->m_pSteamUser = SteamEmulator.SteamUser.MemoryAddress;
            //context->m_pSteamFriends = SteamEmulator.SteamFriends.MemoryAddress;
            //context->m_pSteamUtils = SteamEmulator.SteamUtils.MemoryAddress;
            //context->m_pSteamMatchmaking = SteamEmulator.SteamMatchmaking.MemoryAddress;
            //context->m_pSteamMatchmakingServers = SteamEmulator.SteamMatchMakingServers.MemoryAddress;
            //context->m_pSteamUserStats = SteamEmulator.SteamUserStats.MemoryAddress;
            //context->m_pSteamApps = SteamEmulator.SteamApps.MemoryAddress;
            //context->m_pSteamNetworking = SteamEmulator.SteamNetworking.MemoryAddress;
            //context->m_pSteamRemoteStorage = SteamEmulator.SteamMusicRemote.MemoryAddress;
            //context->m_pSteamScreenshots = SteamEmulator.SteamScreenshots.MemoryAddress;
            //context->m_pSteamHTTP = SteamEmulator.SteamHTTP.MemoryAddress;
            //context->m_pSteamController = SteamEmulator.SteamController.MemoryAddress;
            //context->m_pSteamUGC = SteamEmulator.SteamUGC.MemoryAddress;
            //context->m_pSteamAppList = SteamEmulator.SteamAppList.MemoryAddress;
            //context->m_pSteamMusic = SteamEmulator.SteamMusic.MemoryAddress;
            //context->m_pSteamMusicRemote = SteamEmulator.SteamMusicRemote.MemoryAddress;
            //context->m_pSteamHTMLSurface = SteamEmulator.SteamHTMLSurface.MemoryAddress;
            //context->m_pSteamInventory = SteamEmulator.SteamInventory.MemoryAddress;
            //context->m_pSteamVideo = SteamEmulator.SteamVideo.MemoryAddress;

            //context->Init();
        }

        return context;
    }

    public struct ContextInitData
    {
        public uint counter;
        public CSteamApiContext Context;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SKYNET;
using SKYNET.Interface;
using SKYNET.Types;

public class SteamInternal : BaseCalls
{
    public static uint global_counter;

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamInternal_FindOrCreateUserInterface(IntPtr hSteamUser, IntPtr pszVersion)
    {
        Write($"SteamInternal_FindOrCreateUserInterface {pszVersion}");
        return (IntPtr)Activator.CreateInstance(typeof(SteamInterface));
    }


    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamInternal_FindOrCreateGameServerInterface(IntPtr hSteamUser, IntPtr pszVersion)
    {
        Write($"SteamInternal_FindOrCreateGameServerInterface {pszVersion}");
        return IntPtr.Zero;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamInternal_ContextInit(IntPtr pContextInitData)
    {
        ContextInitData contextInitData = Marshal.PtrToStructure<ContextInitData>(pContextInitData);

        Write($"SteamInternal_ContextInit Counter: {contextInitData.counter}, ctx: {contextInitData.Context}");

        var LUser = SteamClient.LocalUser;
        if (contextInitData.counter != LUser.m_HSteamUser)
        {
            contextInitData.counter = (uint)LUser.m_HSteamUser;
            //Initializar Context
        }

        //CSteamAPIContext c = Marshal.PtrToStructure<CSteamAPIContext>(contextInitData.Context);
        //Write($"looooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooool");

        return contextInitData.Context;

    }

    public struct ContextInitData
    {
        public IntPtr Context;
        public uint counter;
    }


    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamInternal_CreateInterface(IntPtr version)
    {
        Write($"SteamInternal_CreateInterface {version}");
        return (IntPtr)4516351; //Testing
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamInternal_GameServer_Init(IntPtr unIP, IntPtr usPort, IntPtr usGamePort, IntPtr usQueryPort, IntPtr eServerMode, IntPtr pchVersionString)
    {
        Write($"SteamInternal_GameServer_Init");
        return true;
    }
}



[StructLayout(LayoutKind.Sequential)]
public struct CSteamApiContext
{
    public IntPtr m_pSteamClient; // class ISteamClient *
    public IntPtr m_pSteamUser; // class ISteamUser *
    public IntPtr m_pSteamFriends; // class ISteamFriends *
    public IntPtr m_pSteamUtils; // class ISteamUtils *
    public IntPtr m_pSteamMatchmaking; // class ISteamMatchmaking *
    public IntPtr m_pSteamUserStats; // class ISteamUserStats *
    public IntPtr m_pSteamApps; // class ISteamApps *
    public IntPtr m_pSteamMatchmakingServers; // class ISteamMatchmakingServers *
    public IntPtr m_pSteamNetworking; // class ISteamNetworking *
    public IntPtr m_pSteamRemoteStorage; // class ISteamRemoteStorage *
    public IntPtr m_pSteamScreenshots; // class ISteamScreenshots *
    public IntPtr m_pSteamHTTP; // class ISteamHTTP *
    public IntPtr m_pSteamUnifiedMessages; // class ISteamUnifiedMessages *
    public IntPtr m_pController; // class ISteamController *
    public IntPtr m_pSteamUGC; // class ISteamUgc *
    public IntPtr m_pSteamAppList; // class ISteamAppList *
    public IntPtr m_pSteamMusic; // class ISteamMusic *
    public IntPtr m_pSteamMusicRemote; // class ISteamMusicRemote *
    public IntPtr m_pSteamHTMLSurface; // class ISteamHTMLSurface *
    public IntPtr m_pSteamInventory; // class ISteamInventory *
    public IntPtr m_pSteamVideo; // class ISteamVideo *

    public void Clear()
    {
        m_pSteamClient = IntPtr.Zero;
        m_pSteamUser = IntPtr.Zero;
        m_pSteamFriends = IntPtr.Zero;
        m_pSteamUtils = IntPtr.Zero;
        m_pSteamMatchmaking = IntPtr.Zero;
        m_pSteamUserStats = IntPtr.Zero;
        m_pSteamApps = IntPtr.Zero;
        m_pSteamMatchmakingServers = IntPtr.Zero;
        m_pSteamNetworking = IntPtr.Zero;
        m_pSteamRemoteStorage = IntPtr.Zero;
        m_pSteamScreenshots = IntPtr.Zero;
        m_pSteamHTTP = IntPtr.Zero;
        m_pSteamUnifiedMessages = IntPtr.Zero;
        m_pController = IntPtr.Zero;
        m_pSteamUGC = IntPtr.Zero;
        m_pSteamAppList = IntPtr.Zero;
        m_pSteamMusic = IntPtr.Zero;
        m_pSteamMusicRemote = IntPtr.Zero;
        m_pSteamHTMLSurface = IntPtr.Zero;
        m_pSteamInventory = IntPtr.Zero;
        m_pSteamVideo = IntPtr.Zero;
    }

    public bool Init()
    {
        modCommon.Show("Init()");
        var a_steamUser = SteamAPI.GetHSteamUser();
        var a_steamPipe = SteamAPI.GetHSteamPipe();



        return true;
    }
}



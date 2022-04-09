using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NativeSharp;
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks.Implementation;
using SKYNET.Types;
using Steam4NET.Attributes;
using Steam4NET.Core;

namespace SKYNET.Steamworks.Exported
{
    public unsafe class SteamInternal : BaseCalls
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamInternal_FindOrCreateUserInterface(IntPtr hSteamUser, [MarshalAs(UnmanagedType.LPStr)] string pszVersion)
        {
            Write($"SteamInternal_FindOrCreateUserInterface {pszVersion}");
            return InterfaceManager.FindOrCreateInterface(pszVersion);
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
        public unsafe static IntPtr SteamInternal_ContextInit(IntPtr contextInitData_ptr)
        {
            long counter = Marshal.ReadInt64(contextInitData_ptr, 8);
            Write($"{Marshal.SizeOf(contextInitData_ptr)}");
            Write($"SteamInternal_ContextInit");

            IntPtr steamApiContext_ptr = (IntPtr)contextInitData_ptr + 16;

            if (counter != 1)
            {
                Marshal.WriteInt64(contextInitData_ptr, 8, 1);

                CSteamApiContext context = new CSteamApiContext();
                context.Init();

                //var ptr_size = Marshal.SizeOf(context);
                //var vtable = Marshal.AllocHGlobal(ptr_size);
                //Marshal.WriteIntPtr(steamApiContext_ptr, vtable);

                //CSteamApiContext v = Marshal.PtrToStructure<CSteamApiContext>(steamApiContext_ptr);    // Replace pointer Context for new
                //v.Init();
                //Marshal.StructureToPtr(v, steamApiContext_ptr, true);    // Replace pointer Context for new


                //Marshal.StructureToPtr(context, steamApiContext_ptr, false);    // Replace pointer Context for new

                //var bytes = MemoryManager.StructToBytes(SteamEmulator.Context);
                //Marshal.Copy(bytes, 0, steamApiContext_ptr, bytes.Length);
                //Memcpy(bytes, contextInitData_ptr + 16);

                var addr = Original_ContextInit(contextInitData_ptr);

                Marshal.StructureToPtr(context, steamApiContext_ptr, true);    // Replace pointer Context for new

                return addr;
                //Write($"Context Address: Custom implementation : {steamApiContext_ptr}  |  Original call : {addr}");
            }
            return steamApiContext_ptr;
        }

        [DllImport("steam_api64_Original.dll", EntryPoint = "SteamInternal_ContextInit")]
        private unsafe static extern IntPtr Original_ContextInit(IntPtr dst);


        unsafe public static void Memcpy(byte[] bytes, IntPtr dest) // faster than Marshal.Copy
        {
            for (int i = 0; i < bytes.Length; ++i)
                *(byte*)(dest + i) = bytes[i];
        }

        public struct ContextInitData_x64
        {
            private long pFn;                   //64 bites space 
            public long counter;
            public CSteamApiContext Context;
        }

        public struct ContextInitData_x86
        {
            private uint pFn;                   //32 bites space
            public long counter;
            public CSteamApiContext Context;
        }
    }
}

/*
    This method implementation register some interface calls and Crash

    SteamInternal_ContextInit Counter: 0
    SteamInternal_ContextInit Counter: 0
    SteamClient: GetISteamScreenshots 
    SteamClient: BReleaseSteamPipe -703862320                      <---
    SteamAPI: SteamAPI_SetMiniDumpComment
     
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static unsafe void* SteamInternal_ContextInit(IntPtr contextInitData_ptr)
    {
        long counter = Marshal.ReadInt64(contextInitData_ptr, 8);

        Write($"SteamInternal_ContextInit Counter: {counter}");

        IntPtr steamApiContext_ptr = (IntPtr)contextInitData_ptr + 16;

        if (counter != 1)
        {
            Marshal.WriteInt64(contextInitData_ptr, 8, 1);
            Marshal.StructureToPtr(SteamEmulator.Context, steamApiContext_ptr, true);    // Replace pointer Context for new
        }

        return steamApiContext_ptr;
    }
*/

public enum Win32Error
{
    SUCCESS,
    INVALID_FUNCTION,
    FILE_NOT_FOUND,
    PATH_NOT_FOUND,
    TOO_MANY_OPEN_FILES,
    ACCESS_DENIED,
    INVALID_HANDLE,
    ARENA_TRASHED,
    NOT_ENOUGH_MEMORY,
    INVALID_BLOCK,
    BAD_ENVIRONMENT
}

public interface ICSteamApiContext
{
    [VTableSlot(0)]
    IntPtr SteamClient();
    [VTableSlot(1)]
    IntPtr SteamUser();
    [VTableSlot(2)]
    IntPtr SteamFriends();
    [VTableSlot(3)]
    IntPtr SteamUtils();
    [VTableSlot(4)]
    IntPtr SteamMatchmaking();
    [VTableSlot(5)]
    IntPtr SteamGameSearch();
    [VTableSlot(6)]
    IntPtr SteamUserStats();
    [VTableSlot(7)]
    IntPtr SteamApps();
    [VTableSlot(8)]
    IntPtr SteamMatchmakingServers();
    [VTableSlot(9)]
    IntPtr SteamNetworking();
    [VTableSlot(10)]
    IntPtr SteamRemoteStorage();
    [VTableSlot(11)]
    IntPtr SteamScreenshots();
    [VTableSlot(12)]
    IntPtr SteamHTTP();
    [VTableSlot(13)]
    IntPtr SteamController();
    [VTableSlot(14)]
    IntPtr SteamUGC();
    [VTableSlot(15)]
    IntPtr SteamAppList();
    [VTableSlot(16)]
    IntPtr SteamMusic();
    [VTableSlot(17)]
    IntPtr SteamMusicRemote();
    [VTableSlot(18)]
    IntPtr SteamHTMLSurface();
    [VTableSlot(19)]
    IntPtr SteamInventory();
    [VTableSlot(20)]
    IntPtr SteamVideo();
    [VTableSlot(21)]
    IntPtr SteamParentalSettings();
    [VTableSlot(22)]
    IntPtr SteamInput();

    [VTableSlot(23)]
    bool Init();

    [VTableSlot(24)]
    void Clear();

}

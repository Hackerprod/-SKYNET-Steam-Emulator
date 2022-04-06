using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks.Implementation;
using SKYNET.Types;

namespace SKYNET.Steamworks.Exported
{
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
        public unsafe static IntPtr SteamInternal_ContextInit(IntPtr contextInitData_ptr)
        {
            long counter = Marshal.ReadInt64(contextInitData_ptr + 8);

            Write($"SteamInternal_ContextInit Counter: {counter}");

            IntPtr steamApiContext_ptr = (IntPtr)contextInitData_ptr + 16;

            CSteamInterfaceContext context = Interface.Bind<CSteamInterfaceContext>(steamApiContext_ptr);
            Write(context == null);

            if (counter != 1)
            {
                Marshal.StructureToPtr(counter + 1, contextInitData_ptr + 8, false);
                //Marshal.StructureToPtr(SteamEmulator.Context, steamApiContext_ptr, false);
            }

            return steamApiContext_ptr;
        }

        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        //public unsafe static void* SteamInternal_ContextInit_NotWork(void* contextInitData_ptr)
        //{
        //    ContextInitData_x64* contextInitData = (ContextInitData_x64*)contextInitData_ptr;
        //    Write($"SteamInternal_ContextInit Counter: {contextInitData->counter}");

        //    if (contextInitData->counter != 1)
        //    {
        //        CSteamApiContext steamApiContext = contextInitData->Context;
        //        contextInitData->counter = 1;
        //        steamApiContext.Init();
        //        return &contextInitData->Context;
        //    }

        //    return &contextInitData->Context;
        //}

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
    This method implementation register SteamMasterServerUpdater and Crash

    SteamInternal_ContextInit Counter: 0, Context: -1823544080
    SteamInternal_ContextInit Counter: 0, Context: -1823544080
    SteamInternal_ContextInit Counter: 0, Context: -1823544080
    SteamMasterServerUpdater: ClearAllKeyValues                         <---
    SteamAPI: SteamAPI_SetMiniDumpComment
     
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static unsafe void* SteamInternal_ContextInit(IntPtr contextInitData_ptr)
    {
        ContextInitData_x64 contextInitData = Marshal.PtrToStructure<ContextInitData_x64>(contextInitData_ptr);
        Write($"SteamInternal_ContextInit Counter: {contextInitData.counter}, Context: {(int)&contextInitData.Context}");

        if (contextInitData.counter != 1)
        {
            contextInitData.Context.Init();
            return &contextInitData.Context;
        }

        return &contextInitData.Context;

    }
   
     *//// <summary>
       ///     Class representing a block of memory allocated in the local process.
       /// </summary>
public class LocalUnmanagedMemory : IDisposable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LocalUnmanagedMemory" /> class, allocating a block of memory in the
    ///     local process.
    /// </summary>
    /// <param name="size">The size to allocate.</param>
    public LocalUnmanagedMemory(int size)
    {
        // Allocate the memory
        Size = size;
        Address = Marshal.AllocHGlobal(Size);
    }

    /// <summary>
    ///     The address where the data is allocated.
    /// </summary>
    public IntPtr Address { get; private set; }

    /// <summary>
    ///     The size of the allocated memory.
    /// </summary>
    public int Size { get; }

    /// <summary>
    ///     Releases the memory held by the <see cref="LocalUnmanagedMemory" /> object.
    /// </summary>
    public virtual void Dispose()
    {
        // Free the allocated memory
        Marshal.FreeHGlobal(Address);
        // Remove the pointer
        Address = IntPtr.Zero;
        // Avoid the finalizer
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Frees resources and perform other cleanup operations before it is reclaimed by garbage collection.
    /// </summary>
    ~LocalUnmanagedMemory()
    {
        Dispose();
    }

    /// <summary>
    ///     Reads data from the unmanaged block of memory.
    /// </summary>
    /// <typeparam name="T">The type of data to return.</typeparam>
    /// <returns>The return value is the block of memory casted in the specified type.</returns>
    public T Read<T>()
    {
        // Marshal data from the block of memory to a new allocated managed object
        return (T)Marshal.PtrToStructure(Address, typeof(T));
    }

    /// <summary>
    ///     Reads an array of bytes from the unmanaged block of memory.
    /// </summary>
    /// <returns>The return value is the block of memory.</returns>
    public byte[] Read()
    {
        // Allocate an array to store data
        var bytes = new byte[Size];
        // Copy the block of memory to the array
        Marshal.Copy(Address, bytes, 0, Size);
        // Return the array
        return bytes;
    }

    /// <summary>
    ///     Returns a string that represents the current object.
    /// </summary>
    public override string ToString()
    {
        return $"Size = {Size:X}";
    }

    /// <summary>
    ///     Writes an array of bytes to the unmanaged block of memory.
    /// </summary>
    /// <param name="byteArray">The array of bytes to write.</param>
    public void Write(byte[] byteArray)
    {
        // Copy the array of bytes into the block of memory
        Marshal.Copy(byteArray, 0, Address, Size);
    }

    /// <summary>
    ///     Write data to the unmanaged block of memory.
    /// </summary>
    /// <typeparam name="T">The type of data to write.</typeparam>
    /// <param name="data">The data to write.</param>
    public void Write<T>(T data)
    {
        // Marshal data from the managed object to the block of memory
        Marshal.StructureToPtr(data, Address, false);
    }
}


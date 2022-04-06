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
using Reloaded.Memory;

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
        Reloaded.Memory.Sources.Memory Memory = new Reloaded.Memory.Sources.Memory();

        ContextInitData_x64 contextInitData = Marshal.PtrToStructure<ContextInitData_x64>(contextInitData_ptr);
        Write($"SteamInternal_ContextInit Counter: {contextInitData.counter}");

        IntPtr steamApiContext_ptr = (IntPtr)contextInitData_ptr + 16 ;

        if (contextInitData.counter != 1)
        {
            Memory.ChangePermission(steamApiContext_ptr, (int)sizeof(CSteamApiContext), Reloaded.Memory.Kernel32.Kernel32.MEM_PROTECTION.PAGE_READWRITE);
            Memory.Write(steamApiContext_ptr, ref SteamEmulator.Context, false);
            //Marshal.StructureToPtr(SteamEmulator.Context, steamApiContext_ptr, false);
        }

        return steamApiContext_ptr;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public unsafe static void* SteamInternal_ContextInit_NotWork(void* contextInitData_ptr)
    {
        ContextInitData_x64* contextInitData = (ContextInitData_x64*)contextInitData_ptr;
        Write($"SteamInternal_ContextInit Counter: {contextInitData->counter}");

        if (contextInitData->counter != 1)
        {
            CSteamApiContext steamApiContext = contextInitData->Context;
            contextInitData->counter = 1;
            steamApiContext.Init();
            return &contextInitData->Context;
        }

        return &contextInitData->Context;
    }

    public struct ContextInitData_x64
    {
        private long pFn;                   //64 bit space 
        public  long counter;
        public CSteamApiContext Context;
    }

    public struct ContextInitData_x86
    {
        private uint pFn;                   //32 bit space
        public  long counter;
        public CSteamApiContext Context;
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
   
     */

public static class IntPtrExtensions
{
    #region Methods: Arithmetics
    public static IntPtr Decrement(this IntPtr pointer, Int32 value)
    {
        return Increment(pointer, -value);
    }

    public static IntPtr Decrement(this IntPtr pointer, Int64 value)
    {
        return Increment(pointer, -value);
    }

    public static IntPtr Decrement(this IntPtr pointer, IntPtr value)
    {
        switch (IntPtr.Size)
        {
            case sizeof(Int32):
                return (new IntPtr(pointer.ToInt32() - value.ToInt32()));

            default:
                return (new IntPtr(pointer.ToInt64() - value.ToInt64()));
        }
    }

    public static IntPtr Increment(this IntPtr pointer, Int32 value)
    {
        unchecked
        {
            switch (IntPtr.Size)
            {
                case sizeof(Int32):
                    return (new IntPtr(pointer.ToInt32() + value));

                default:
                    return (new IntPtr(pointer.ToInt64() + value));
            }
        }
    }

    public static IntPtr Increment(this IntPtr pointer, Int64 value)
    {
        unchecked
        {
            switch (IntPtr.Size)
            {
                case sizeof(Int32):
                    return (new IntPtr((Int32)(pointer.ToInt32() + value)));

                default:
                    return (new IntPtr(pointer.ToInt64() + value));
            }
        }
    }

    public static IntPtr Increment(this IntPtr pointer, IntPtr value)
    {
        unchecked
        {
            switch (IntPtr.Size)
            {
                case sizeof(int):
                    return new IntPtr(pointer.ToInt32() + value.ToInt32());
                default:
                    return new IntPtr(pointer.ToInt64() + value.ToInt64());
            }
        }
    }
    #endregion

    #region Methods: Comparison
    public static Int32 CompareTo(this IntPtr left, Int32 right)
    {
        return left.CompareTo((UInt32)right);
    }

    public static Int32 CompareTo(this IntPtr left, IntPtr right)
    {
        if (left.ToUInt64() > right.ToUInt64())
            return 1;

        if (left.ToUInt64() < right.ToUInt64())
            return -1;

        return 0;
    }

    public static Int32 CompareTo(this IntPtr left, UInt32 right)
    {
        if (left.ToUInt64() > right)
            return 1;

        if (left.ToUInt64() < right)
            return -1;

        return 0;
    }
    #endregion

    #region Methods: Conversion
    public unsafe static UInt32 ToUInt32(this IntPtr pointer)
    {
        return (UInt32)((void*)pointer);
    }

    public unsafe static UInt64 ToUInt64(this IntPtr pointer)
    {
        return (UInt64)((void*)pointer);
    }
    #endregion

    #region Methods: Equality
    public static Boolean Equals(this IntPtr pointer, Int32 value)
    {
        return (pointer.ToInt32() == value);
    }

    public static Boolean Equals(this IntPtr pointer, Int64 value)
    {
        return (pointer.ToInt64() == value);
    }

    public static Boolean Equals(this IntPtr left, IntPtr ptr2)
    {
        return (left == ptr2);
    }

    public static Boolean Equals(this IntPtr pointer, UInt32 value)
    {
        return (pointer.ToUInt32() == value);
    }

    public static Boolean Equals(this IntPtr pointer, UInt64 value)
    {
        return (pointer.ToUInt64() == value);
    }

    public static Boolean GreaterThanOrEqualTo(this IntPtr left, IntPtr right)
    {
        return (left.CompareTo(right) >= 0);
    }

    public static Boolean LessThanOrEqualTo(this IntPtr left, IntPtr right)
    {
        return (left.CompareTo(right) <= 0);
    }
    #endregion

    #region Methods: Logic
    public static IntPtr And(this IntPtr pointer, IntPtr value)
    {
        switch (IntPtr.Size)
        {
            case sizeof(Int32):
                return (new IntPtr(pointer.ToInt32() & value.ToInt32()));

            default:
                return (new IntPtr(pointer.ToInt64() & value.ToInt64()));
        }
    }

    public static IntPtr Not(this IntPtr pointer)
    {
        switch (IntPtr.Size)
        {
            case sizeof(Int32):
                return (new IntPtr(~pointer.ToInt32()));

            default:
                return (new IntPtr(~pointer.ToInt64()));
        }
    }

    public static IntPtr Or(this IntPtr pointer, IntPtr value)
    {
        switch (IntPtr.Size)
        {
            case sizeof(Int32):
                return (new IntPtr(pointer.ToInt32() | value.ToInt32()));

            default:
                return (new IntPtr(pointer.ToInt64() | value.ToInt64()));
        }
    }

    public static IntPtr Xor(this IntPtr pointer, IntPtr value)
    {
        switch (IntPtr.Size)
        {
            case sizeof(Int32):
                return (new IntPtr(pointer.ToInt32() ^ value.ToInt32()));

            default:
                return (new IntPtr(pointer.ToInt64() ^ value.ToInt64()));
        }
    }
    #endregion
}
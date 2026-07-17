using System;
using System.Runtime.InteropServices;
using System.Threading;
using SKYNET.Managers;

using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;

namespace SKYNET.Steamworks.Exported
{
    public class SteamInternal
    {
        private static readonly object ContextInitLock = new object();
        private static long contextCounter = 1;

        static SteamInternal()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamInternal_SteamAPI_Init(IntPtr pszInternalCheckInterfaceVersions, IntPtr pOutErrMsg)
        {
            Write($"SteamInternal_SteamAPI_Init");
            BumpContextCounter("SteamInternal_SteamAPI_Init");
            if (SteamEmulator.SecureNetworking)
            {
                Write("SecureNetworking requested; SDR CA memory patching is enabled");
            }
            return 0; // k_ESteamAPIInitResult_OK
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamInternal_GameServer_Init_V2(uint unIP, ushort usGamePort, ushort usQueryPort, uint eServerMode, [MarshalAs(UnmanagedType.LPStr)] string pchVersionString, IntPtr pszInternalCheckInterfaceVersions, IntPtr pOutErrMsg)
        {
            uint unFlags = 0; // Always insecure; InitGameServer enforces this too.
            var result = SteamEmulator.SteamGameServer.InitGameServer(unIP, usGamePort, usQueryPort, unFlags, SteamEmulator.AppID, pchVersionString);
            if (result)
            {
                BumpContextCounter("SteamInternal_GameServer_Init_V2");
            }
            Write($"SteamInternal_GameServer_Init_V2 = {result} (IP = {unIP}, GamePort = {usGamePort}, QueryPort = {usQueryPort}, ServerMode = {eServerMode}, Flags = {unFlags}, VersionString = {pchVersionString})");
            return result ? 0 : 1; // k_ESteamAPIInitResult_OK / generic failure
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamInternal_FindOrCreateUserInterface(int hSteamUser, [MarshalAs(UnmanagedType.LPStr)] string pszVersion)
        {
            Write($"SteamInternal_FindOrCreateUserInterface {pszVersion}");
            return InterfaceManager.FindOrCreateInterface(pszVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamInternal_FindOrCreateGameServerInterface(HSteamUser hSteamUser, [MarshalAs(UnmanagedType.LPStr)] string pszVersion)
        {
            Write($"SteamInternal_FindOrCreateGameServerInterface {pszVersion}");
            return InterfaceManager.FindOrCreateInterface(hSteamUser, 1, pszVersion, true);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamInternal_CreateInterface([MarshalAs(UnmanagedType.LPStr)] string pszVersion)
        {
            Write($"SteamInternal_CreateInterface {pszVersion}");
            return InterfaceManager.FindOrCreateInterface(pszVersion);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamInternal_GameServer_Init(uint unIP, int usPort, int usGamePort, uint usQueryPort, uint eServerMode, string pchVersionString)
        {
            uint unFlags = 0; // Always insecure; InitGameServer enforces this too.
            var result = SteamEmulator.SteamGameServer.InitGameServer(unIP, usPort, (int)usQueryPort, unFlags, SteamEmulator.AppID, pchVersionString);
            if (result)
            {
                BumpContextCounter("SteamInternal_GameServer_Init");
            }
            Write($"SteamInternal_GameServer_Init = {result}");
            return result;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamInternal_ContextInit(IntPtr pContextInitData)
        {
            if (pContextInitData == IntPtr.Zero)
            {
                Write("SteamInternal_ContextInit failed: null data");
                return IntPtr.Zero;
            }

            try
            {
                IntPtr initFnPtr = Marshal.ReadIntPtr(pContextInitData);
                if (initFnPtr == IntPtr.Zero)
                {
                    Write("SteamInternal_ContextInit failed: null init function");
                    return IntPtr.Zero;
                }

                IntPtr counterPtr = IntPtr.Add(pContextInitData, IntPtr.Size);
                IntPtr contextSlotPtr = IntPtr.Add(pContextInitData, IntPtr.Size * 2);

                lock (ContextInitLock)
                {
                    long currentCounter = Interlocked.Read(ref contextCounter);
                    long storedCounter = Marshal.ReadIntPtr(counterPtr).ToInt64();

                    if (storedCounter != currentCounter)
                    {
                        var initFn = Marshal.GetDelegateForFunctionPointer<SteamContextInitDelegate>(initFnPtr);
                        initFn(contextSlotPtr);
                        Marshal.WriteIntPtr(counterPtr, new IntPtr(currentCounter));

                        IntPtr contextValue = Marshal.ReadIntPtr(contextSlotPtr);
                        Write($"SteamInternal_ContextInit initialized counter={currentCounter} context=0x{contextValue.ToInt64():X}");
                    }
                }

                return contextSlotPtr;
            }
            catch (Exception ex)
            {
                Write($"SteamInternal_ContextInit failed: {ex.Message}");
                return IntPtr.Zero;
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SteamContextInitDelegate(IntPtr contextSlotPtr);

        private static void BumpContextCounter(string reason)
        {
            long value = Interlocked.Increment(ref contextCounter);
            Write($"SteamInternal context counter {value} ({reason})");
        }

        private static void Write(object msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}

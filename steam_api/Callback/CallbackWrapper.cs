using SKYNET.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using SteamAPICall_t = System.UInt64;

namespace SKYNET.Callback
{
    public class CallbackWrapper
    {
        internal const string libraryPath = "Callbacks.dll";

        public static SteamAPICall_t AddCallbackResult(int iCallback, IntPtr result, int size)
        {
            ThreadPool.QueueUserWorkItem(VerifyExists);
            return SKYNET_AddCallbackResult(iCallback, result, size);
        }

        public static void RegisterCallback(IntPtr pCallback, int iCallback)
        {
            SteamEmulator.Write("Unju", "RegisterCallback");
            ThreadPool.QueueUserWorkItem(VerifyExists);
            object obj = (object)new object[] { pCallback, iCallback };
            ThreadPool.QueueUserWorkItem(Register, obj);
        }

        private static void Register(object state)
        {
            object[] obj = (object[])state;
            SKYNET_RegisterCallback((IntPtr)obj[0], (int)obj[1]);
        }

        public static void RegisterCallResult(IntPtr pCallback, SteamAPICall_t hAPICall)
        {
            ThreadPool.QueueUserWorkItem(VerifyExists);
            SKYNET_RegisterCallResult(pCallback, hAPICall);
        }

        public static void RunCallbacks()
        {
            ThreadPool.QueueUserWorkItem(VerifyExists);
            SKYNET_RunCallbacks();
        }

        public static void UnregisterCallback(IntPtr pCallback)
        {
            ThreadPool.QueueUserWorkItem(VerifyExists);
            SKYNET_UnregisterCallback(pCallback);
        }

        public static void UnregisterCallResult(IntPtr pCallback, SteamAPICall_t hAPICall)
        {
            ThreadPool.QueueUserWorkItem(VerifyExists);
            SKYNET_UnregisterCallResult(pCallback, hAPICall);
        }

        public static IntPtr ContextInit(IntPtr contextInitData_ptr)
        {
            ThreadPool.QueueUserWorkItem(VerifyExists);
            return SKYNET_ContextInit(contextInitData_ptr);
        }

        private static void VerifyExists(object state)
        {
            if (!File.Exists(libraryPath))
            {
                File.WriteAllBytes(libraryPath, Resources.Callbacks);
            }
        }

        #region Importable functions

        [DllImport(libraryPath, EntryPoint = "SKYNET_AddCallbackResult")]
        public static extern SteamAPICall_t SKYNET_AddCallbackResult(int iCallback, IntPtr result, int size);

        [DllImport(libraryPath, EntryPoint = "SKYNET_RegisterCallback")]
        public static extern void SKYNET_RegisterCallback(IntPtr pCallback, int iCallback);

        [DllImport(libraryPath, EntryPoint = "SKYNET_RegisterCallResult")]
        public static extern void SKYNET_RegisterCallResult(IntPtr pCallback, SteamAPICall_t hAPICall);

        [DllImport(libraryPath, EntryPoint = "SKYNET_RunCallbacks")]
        public static extern void SKYNET_RunCallbacks();

        [DllImport(libraryPath, EntryPoint = "SKYNET_UnregisterCallback")]
        public static extern void SKYNET_UnregisterCallback(IntPtr pCallback);

        [DllImport(libraryPath, EntryPoint = "SKYNET_UnregisterCallResult")]
        public static extern void SKYNET_UnregisterCallResult(IntPtr pCallback, SteamAPICall_t hAPICall);

        [DllImport(libraryPath, EntryPoint = "SKYNET_UnregisterCallResult")]
        public static extern IntPtr SKYNET_ContextInit(IntPtr contextInitData_ptr);

        #endregion

    }
}

using System;
using System.Runtime.InteropServices;

namespace SKYNET.Hook.Handles
{
    internal class ContextInitTest
    {
        //

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool InitFunc();

        /// <summary>Steam's actual init, just a little christmas wrapping for managed access.</summary>
        private static InitFunc NativeInit { get; set; }

        //
        private const string kSteamworksWin64ModuleName = "steam_api64.dll";

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public unsafe delegate CSteamApiContext* ContextInitFunc(IntPtr c_callbackStruc);

        private static IntPtr m_callbackCounterAndContext;
        private static IntPtr m_libProcAddress;

        /// <summary>Pretending to be steam_api.h file</summary>
        private static ContextInitFunc ContextInit { get; set; }

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public unsafe delegate void OnContextInitFunc(CSteamApiContext* c_contextPtr);

        public static unsafe OnContextInitFunc OnContextInitPtr = OnContextInit;

        private static unsafe CSteamApiContext* m_steamApiContext;

        private static unsafe CSteamApiContext* GetSteamApiContext()
        {
            Write($"GetSteamApiContext");

            if (m_steamApiContext != null)
            {
                return m_steamApiContext;
            }

            var a_callbackCounterAndContext = CallbackCounterAndContext();

            m_steamApiContext = ContextInit(a_callbackCounterAndContext);

            return m_steamApiContext;
        }

        private static unsafe IntPtr CallbackCounterAndContext()
        {
            Write($"CallbackCounterAndContext");

            if (m_callbackCounterAndContext != IntPtr.Zero)
            {
                return m_callbackCounterAndContext;
            }

            var a_size = 2 + Marshal.SizeOf<CSteamApiContext>() / Marshal.SizeOf<IntPtr>();

            var a_callbackCounterAndContext = new IntPtr[a_size];

            a_callbackCounterAndContext[0] = Marshal.GetFunctionPointerForDelegate(OnContextInitPtr);

            m_callbackCounterAndContext = Marshal.UnsafeAddrOfPinnedArrayElement(a_callbackCounterAndContext, 0);

            return m_callbackCounterAndContext;
        }

        private static unsafe void OnContextInit(CSteamApiContext* c_contextPointer)
        {
            Write($"OnContextInit");
            c_contextPointer->Clear();

            //if (GetHSteamPipe() != 0)
            {
                c_contextPointer->Init();
            }
        }



        internal static void Init()
        {
            Write($"Init Context test");

            NativeInit = LoadSteamworksFunction<InitFunc>("SteamAPI_Init");

            var a = new InitFunc(InitF);

            ContextInit = LoadSteamworksFunction<ContextInitFunc>("SteamInternal_ContextInit");

            bool m_isInitialized = NativeInit();

            if (!m_isInitialized)
            {
                Write("SteamAPI could not be initialized!");
            }
        }

        private static bool InitF()
        {
            Write($"Init");
            return true;
        }

        private static TDelegate LoadSteamworksFunction<TDelegate>(string c_functionName)
        {
            Write($"LoadSteamworksFunction");

            if (m_libProcAddress == IntPtr.Zero)
            {
                m_libProcAddress = LoadLibrary(kSteamworksWin64ModuleName);

                if (m_libProcAddress == IntPtr.Zero)
                {
                    Write(Marshal.GetLastWin32Error().ToString());
                }
            }

            var a_procAddress = GetProcAddress(m_libProcAddress, c_functionName);

            if (a_procAddress == IntPtr.Zero)
            {
                Write($"{c_functionName} failed to load");
            }

            return Marshal.GetDelegateForFunctionPointer<TDelegate>(a_procAddress);
        }




        #region Windows Interop

        private const string kKernel32ModuleName = "kernel32.dll";

        [DllImport(kKernel32ModuleName, SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string c_lpFileName);

        [DllImport(kKernel32ModuleName, CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr c_hModule, string c_procName);

        #endregion

        private static void Write(string v)
        {
            Main.Write(v);
        }

    }
}
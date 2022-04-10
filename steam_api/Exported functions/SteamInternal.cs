using System;
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
            Write($"SteamInternal_ContextInit");
            IntPtr apiContext_ptr = IntPtr.Zero;
            if (modCommon.Is64Bit())
            {
                ContextInitData_64 Context = Marshal.PtrToStructure<ContextInitData_64>(contextInitData_ptr);
                apiContext_ptr = contextInitData_ptr + 16;
                if (Context.counter != 1)
                {
                    Marshal.WriteInt64(contextInitData_ptr, 8, 1);
                    _pFn = Marshal.GetDelegateForFunctionPointer<pFn>(Context.pFn);
                    _pFn.Invoke(apiContext_ptr);
                }
            }
            else
            {
                var Context = Marshal.PtrToStructure<ContextInitData_x86>(contextInitData_ptr);
                apiContext_ptr = contextInitData_ptr + 8;
                if (Context.counter != 1)
                {
                    Marshal.WriteInt32(contextInitData_ptr, 4, 1);
                    _pFn = Marshal.GetDelegateForFunctionPointer<pFn>(Context.pFn);
                    _pFn.Invoke(apiContext_ptr);
                }
            }

            return apiContext_ptr;
        }

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void pFn(IntPtr ctx);
        public static pFn _pFn;

        public struct ContextInitData_64
        {
            public IntPtr pFn;
            public long counter;
            public CSteamApiContext Context;
        }

        public struct ContextInitData_x86
        {
            public IntPtr pFn;
            public uint counter;
            public CSteamApiContext Context;
        }
    }
}


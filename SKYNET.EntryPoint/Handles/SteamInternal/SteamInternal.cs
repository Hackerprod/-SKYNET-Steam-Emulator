using System;
using System.Runtime.InteropServices;
using SKYNET.Managers;
using SKYNET.Types;

namespace SKYNET.Hook.Handles
{
    public partial class SteamInternal : BaseHook
    {
        public override bool Installed { get; set; }

        public unsafe override void Install()
        {
            // SteamInternal Handles

            base.Install<SteamInternal_FindOrCreateUserInterfaceDelegate>("SteamInternal_FindOrCreateUserInterface", _SteamInternal_FindOrCreateUserInterface, new SteamInternal_FindOrCreateUserInterfaceDelegate(SteamInternal_FindOrCreateUserInterface));
            base.Install<SteamInternal_FindOrCreateGameServerInterfaceDelegate>("SteamInternal_FindOrCreateGameServerInterface", _SteamInternal_FindOrCreateGameServerInterfaceDelegate, new SteamInternal_FindOrCreateGameServerInterfaceDelegate(SteamInternal_FindOrCreateGameServerInterface));
            base.Install<SteamInternal_CreateInterfaceDelegate>("SteamInternal_CreateInterface", _SteamInternal_CreateInterfaceDelegate, new SteamInternal_CreateInterfaceDelegate(SteamInternal_CreateInterface));
            base.Install<SteamInternal_GameServer_InitDelegate>("SteamInternal_GameServer_Init", _SteamInternal_GameServer_InitDelegate, new SteamInternal_GameServer_InitDelegate(SteamInternal_GameServer_Init));
            base.Install<SteamInternal_ContextInitDelegate>("SteamInternal_ContextInit", _SteamInternal_ContextInitDelegate, new SteamInternal_ContextInitDelegate(SteamInternal_ContextInit));
        }

        public IntPtr SteamInternal_FindOrCreateUserInterface(int hSteamUser, [MarshalAs(UnmanagedType.LPStr)] string pszVersion)
        {
            Write($"SteamInternal_FindOrCreateUserInterface {pszVersion}");
            return InterfaceManager.FindOrCreateInterface(pszVersion); 
        }

        public IntPtr SteamInternal_FindOrCreateGameServerInterface(int hSteamUser, [MarshalAs(UnmanagedType.LPStr)] string pszVersion)
        {
            Write($"SteamInternal_FindOrCreateGameServerInterface {pszVersion}");
            return InterfaceManager.FindOrCreateInterface(pszVersion);
        }

        public IntPtr SteamInternal_CreateInterface([MarshalAs(UnmanagedType.LPStr)] string pszVersion)
        {
            Write($"SteamInternal_CreateInterface {pszVersion}");
            return InterfaceManager.FindOrCreateInterface(pszVersion);
        }

        public bool SteamInternal_GameServer_Init(IntPtr unIP, IntPtr usPort, IntPtr usGamePort, IntPtr usQueryPort, IntPtr eServerMode, [MarshalAs(UnmanagedType.LPStr)] string pchVersionString)
        {
            Write($"SteamInternal_GameServer_Init {pchVersionString}");
            return true;
        }


        public IntPtr SteamInternal_ContextInit(IntPtr contextInitData_ptr)
        {
            Write($"SteamInternal_ContextInit");
            IntPtr apiContext_ptr = IntPtr.Zero;

            if (modCommon.Is64Bit())
            {
                var Context = Marshal.PtrToStructure<ContextInitData_64>(contextInitData_ptr);
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

        public override void Write(object v)
        {
            Main.Write("SteamInternal", v);
        }
    }

}


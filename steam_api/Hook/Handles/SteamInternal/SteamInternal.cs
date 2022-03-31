using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotNetCross.Memory;
using EasyHook;
using SKYNET;
using SKYNET.Helper;
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

        public static unsafe void* SteamInternal_ContextInit(void* c_contextPointer)
        {
            ContextInitData* CreatedContext = (ContextInitData*)c_contextPointer;

            if (CreatedContext->Context.SteamClient() != SteamEmulator.SteamClient.BaseAddress)
            {
                Main.Write("SteamInternal_ContextInit initializing");
                CreatedContext->Context.Init();
                CreatedContext->counter = 1;
            }

            if (m_steamApiContext != null)
            {
                return m_steamApiContext;
            }

            //var a_callbackCounterAndContext = CallbackCounterAndContext();
            //Main.Write($"Step1");
            //m_steamApiContext = ContextInit(a_callbackCounterAndContext);
            //Main.Write($"Step2");
            //return m_steamApiContext;


            return &CreatedContext->Context;
        }

        #region Testing

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public unsafe delegate CSteamApiContext* ContextInitFunc(IntPtr c_callbackStruc);
        private static ContextInitFunc ContextInit { get; set; }
        private static unsafe CSteamApiContext* m_steamApiContext;
        private static unsafe IntPtr CallbackCounterAndContext()
        {
            Main.Write($"CallbackCounterAndContext");

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
        private static IntPtr m_callbackCounterAndContext;
        public static unsafe OnContextInitFunc OnContextInitPtr = OnContextInit;
        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public unsafe delegate void OnContextInitFunc(CSteamApiContext* c_contextPtr);
        private static unsafe void OnContextInit(CSteamApiContext* c_contextPointer)
        {
            Main.Write($"OnContextInit");
            c_contextPointer->Clear();

            //if (GetHSteamPipe() != 0)
            {
                c_contextPointer->Init();
            }
        }

        #endregion

        public struct ContextInitData
        {
            public CSteamApiContext Context;
            public uint counter;
        }

        public override void Write(object v)
        {
            Main.Write("SteamInternal", v);
        }
    }

}


﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyHook;
using SKYNET;
using SKYNET.Helpers;
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
            base.Install<OnContextInitFunc>("SteamInternal_ContextInit", OnContextInitPtr, new OnContextInitFunc(SteamInternal_ContextInit));
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


        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public unsafe delegate void* OnContextInitFunc(void* c_contextPtr);

        public static unsafe OnContextInitFunc OnContextInitPtr = SteamInternal_ContextInit;

        public static unsafe void* SteamInternal_ContextInit(void* c_contextPointer)
        {
            ContextInitData* CreatedContext = (ContextInitData*)c_contextPointer;
            CSteamApiContext* context = &CreatedContext->Context;

            //if (CreatedContext->counter == 0)
            //{
            //    CreatedContext->counter = 1;
            //}

            Main.Write($"SteamInternal_ContextInit initializing ");


            return context;
        }

        public unsafe struct ContextInitData
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


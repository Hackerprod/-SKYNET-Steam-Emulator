using SKYNET.Callback;
using SKYNET.Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Steamworks.Net
{
    public sealed class SteamCallback<T> : IDisposable
    {
        public delegate void DispatchDelegate(T param);

        private CCallbackBaseVTable VTable;

        private IntPtr m_pVTable = IntPtr.Zero;

        private CCallbackBase m_CCallbackBase;

        private GCHandle m_pCCallbackBase;

        private bool m_bGameServer;

        private readonly int m_size = Marshal.SizeOf(typeof(T));

        private bool m_bDisposed;

        private event DispatchDelegate m_Func;

        public static SteamCallback<T> Create(DispatchDelegate func)
        {
            return new SteamCallback<T>(func);
        }

        public static SteamCallback<T> CreateGameServer(DispatchDelegate func)
        {
            return new SteamCallback<T>(func, bGameServer: true);
        }

        public SteamCallback(DispatchDelegate func, bool bGameServer = false)
        {
            m_bGameServer = bGameServer;
            BuildCCallbackBase();
            Register(func);
        }

        ~SteamCallback()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!m_bDisposed)
            {
                GC.SuppressFinalize(this);
                Unregister();
                if (m_pVTable != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(m_pVTable);
                }
                if (m_pCCallbackBase.IsAllocated)
                {
                    m_pCCallbackBase.Free();
                }
                m_bDisposed = true;
            }
        }

        public void Register(DispatchDelegate func)
        {
            if (func == null)
            {
                throw new Exception("Callback function must not be null.");
            }
            if ((m_CCallbackBase.m_nCallbackFlags & 1) == 1)
            {
                Unregister();
            }
            if (m_bGameServer)
            {
                SetGameserverFlag();
            }
            this.m_Func = func;
            NativeMethods.SteamAPI_RegisterCallback(m_pCCallbackBase.AddrOfPinnedObject(), 0 /*CallbackIdentities.GetCallbackIdentity(typeof(T))*/);
        }

        public void Unregister()
        {
            NativeMethods.SteamAPI_UnregisterCallback(m_pCCallbackBase.AddrOfPinnedObject());
        }

        public void SetGameserverFlag()
        {
            m_CCallbackBase.m_nCallbackFlags |= 2;
        }

        private void OnRunCallback(IntPtr thisptr, IntPtr pvParam)
        {
            try
            {
                this.m_Func((T)Marshal.PtrToStructure(pvParam, typeof(T)));
            }
            catch (Exception e)
            {
                CallbackDispatcher.ExceptionHandler(e);
            }
        }

        private void OnRunCallResult(IntPtr thisptr, IntPtr pvParam, bool bFailed, ulong hSteamAPICall)
        {
            try
            {
                this.m_Func((T)Marshal.PtrToStructure(pvParam, typeof(T)));
            }
            catch (Exception e)
            {
                CallbackDispatcher.ExceptionHandler(e);
            }
        }

        private int OnGetCallbackSizeBytes(IntPtr thisptr)
        {
            return m_size;
        }

        private void BuildCCallbackBase()
        {
            //VTable = new CCallbackBaseVTable
            //{
            //    m_RunCallResult = OnRunCallResult,
            //    m_RunCallback = OnRunCallback,
            //    m_GetCallbackSizeBytes = OnGetCallbackSizeBytes
            //};
            m_pVTable = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(CCallbackBaseVTable)));
            Marshal.StructureToPtr(VTable, m_pVTable, fDeleteOld: false);
            m_CCallbackBase = new CCallbackBase
            {
                m_vfptr = m_pVTable,
                m_nCallbackFlags = 0,
                m_iCallback = 0 // CallbackIdentities.GetCallbackIdentity(typeof(T))
            };
            m_pCCallbackBase = GCHandle.Alloc(m_CCallbackBase, GCHandleType.Pinned);
        }
    }

}

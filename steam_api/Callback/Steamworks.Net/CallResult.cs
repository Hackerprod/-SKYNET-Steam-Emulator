using SKYNET.Callback;
using SKYNET.Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using SteamAPICall_t = System.UInt64;

namespace Steamworks.Net
{
    public sealed class SteamCallResult<T> : IDisposable
    {
        public delegate void APIDispatchDelegate(T param, bool bIOFailure);

        private CCallbackBaseVTable VTable;

        private IntPtr m_pVTable = IntPtr.Zero;

        private CCallbackBase m_CCallbackBase;

        private GCHandle m_pCCallbackBase;

        private SteamAPICall_t m_hAPICall = 0;

        private readonly int m_size = Marshal.SizeOf(typeof(T));

        private bool m_bDisposed;

        public SteamAPICall_t Handle => m_hAPICall;

        private event APIDispatchDelegate m_Func;

        public static SteamCallResult<T> Create(APIDispatchDelegate func = null)
        {
            return new SteamCallResult<T>(func);
        }

        public SteamCallResult(APIDispatchDelegate func = null)
        {
            this.m_Func = func;
            BuildCCallbackBase();
        }

        ~SteamCallResult()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!m_bDisposed)
            {
                GC.SuppressFinalize(this);
                Cancel();
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

        public void Set(SteamAPICall_t hAPICall, APIDispatchDelegate func = null)
        {
            if (func != null)
            {
                this.m_Func = func;
            }
            if (this.m_Func == null)
            {
                throw new Exception("CallResult function was null, you must either set it in the CallResult Constructor or in Set()");
            }
            if (m_hAPICall != 0)
            {
                NativeMethods.SteamAPI_UnregisterCallResult(m_pCCallbackBase.AddrOfPinnedObject(), (ulong)m_hAPICall);
            }
            m_hAPICall = hAPICall;
            if (hAPICall != 0)
            {
                NativeMethods.SteamAPI_RegisterCallResult(m_pCCallbackBase.AddrOfPinnedObject(), (ulong)hAPICall);
            }
        }

        public bool IsActive()
        {
            return m_hAPICall != 0;
        }

        public void Cancel()
        {
            if (m_hAPICall != 0)
            {
                NativeMethods.SteamAPI_UnregisterCallResult(m_pCCallbackBase.AddrOfPinnedObject(), (ulong)m_hAPICall);
                m_hAPICall = 0;
            }
        }

        public void SetGameserverFlag()
        {
            m_CCallbackBase.m_nCallbackFlags |= 2;
        }

        private void OnRunCallback(IntPtr pvParam)
        {
            m_hAPICall = 0;
            try
            {
                this.m_Func((T)Marshal.PtrToStructure(pvParam, typeof(T)), bIOFailure: false);
            }
            catch (Exception e)
            {
                CallbackDispatcher.ExceptionHandler(e);
            }
        }

        private void OnRunCallResult(IntPtr pvParam, bool bFailed, ulong hSteamAPICall_)
        {
            if ((SteamAPICall_t)hSteamAPICall_ == m_hAPICall)
            {
                m_hAPICall = 0;
                try
                {
                    this.m_Func((T)Marshal.PtrToStructure(pvParam, typeof(T)), bFailed);
                }
                catch (Exception e)
                {
                    CallbackDispatcher.ExceptionHandler(e);
                }
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
            //    m_RunCallback = OnRunCallback,
            //    m_RunCallResult = OnRunCallResult,
            //    m_GetCallbackSizeBytes = OnGetCallbackSizeBytes
            //};
            //m_pVTable = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(CCallbackBaseVTable)));
            //Marshal.StructureToPtr(VTable, m_pVTable, fDeleteOld: false);
            //m_CCallbackBase = new CCallbackBase
            //{
            //    m_vfptr = m_pVTable,
            //    m_nCallbackFlags = 0,
            //    m_iCallback = 0 // CallbackIdentities.GetCallbackIdentity(typeof(T))
            //};
            //m_pCCallbackBase = GCHandle.Alloc(m_CCallbackBase, GCHandleType.Pinned);
        }
    }
}

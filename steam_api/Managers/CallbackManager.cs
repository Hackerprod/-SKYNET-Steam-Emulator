using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SKYNET.Callback;
using SKYNET.Helper;
using Steamworks;

namespace SKYNET.Managers
{
    public unsafe class CallbackManager
    {
        public const byte k_ECallbackFlagsRegistered = 1;
        public const byte k_ECallbackFlagsGameServer = 2;
        public static List<SteamAPICall_t> SteamAPICallsCompleted;
        public static ConcurrentDictionary<int, Type> CallbackTypes;

        private static ConcurrentDictionary<int, SteamCallback> Client_Callbacks;
        private static ConcurrentDictionary<int, SteamCallback> Server_Callbacks;


        private static Dictionary<ulong, CCallbackBase> CallbackResult;
        private static List<SteamAPICall_t> SteamAPICalls;

        private static object call_id_lock = new object();

        static CallbackManager()
        {
            if (Client_Callbacks == null)
            {
                Client_Callbacks = new ConcurrentDictionary<int, SteamCallback>();
            }
            if (Server_Callbacks == null)
            {
                Server_Callbacks = new ConcurrentDictionary<int, SteamCallback>();
            }
            if (SteamAPICalls == null)
            {
                SteamAPICalls = new List<SteamAPICall_t>();
            }
            if (SteamAPICallsCompleted == null)
            {
                SteamAPICallsCompleted = new List<SteamAPICall_t>();
            }
            if (CallbackResult == null)
            {
                CallbackResult = new Dictionary<ulong, CCallbackBase>();
            }
            if (CallbackTypes == null)
            {
                CallbackTypes = new ConcurrentDictionary<int, Type>();
            }
        }

        public unsafe static void RegisterCallback(int iCallback, IntPtr ptrCallback, bool Server = false)
        {
            if (iCallback == new SteamAPICallCompleted_t().Callback)
            {
                var CBase = ptrCallback.ToType<CCallbackBase>().m_iCallback;
            }

            SteamCallback sCallback = new SteamCallback(ptrCallback);
            sCallback.CallbackBase.m_nCallbackFlags |= k_ECallbackFlagsRegistered;
            sCallback.CallbackBase.m_iCallback = iCallback;
            sCallback.Update();


            if (Server)
            {
                if (Server_Callbacks.ContainsKey(iCallback))
                    Server_Callbacks[iCallback] = sCallback;
                else
                    Server_Callbacks.TryAdd(iCallback, sCallback);
            }
            else
            {
                if (Client_Callbacks.ContainsKey(iCallback))
                    Client_Callbacks[iCallback] = sCallback;
                else
                    Client_Callbacks.TryAdd(iCallback, sCallback);
            }
        }

        public static void UnregisterCallResult(IntPtr pCallback, SteamAPICall_t hAPICall)
        {
        }

        public static void UnregisterCallback(IntPtr ptrCallback)
        {
            CCallbackBase pCallback = ptrCallback.ToType<CCallbackBase>();
            if (pCallback.m_iCallback == new SteamAPICallCompleted_t().Callback)
            {
                pCallback.m_nCallbackFlags &= k_ECallbackFlagsRegistered;
                Marshal.StructureToPtr(pCallback, ptrCallback, false);
            }

            if (pCallback.IsGameServer())
            {
                if (Server_Callbacks.ContainsKey(pCallback.m_iCallback))
                    Server_Callbacks.TryRemove(pCallback.m_iCallback, out _);
            }
            else
            {
                if (Client_Callbacks.ContainsKey(pCallback.m_iCallback))
                    Client_Callbacks.TryRemove(pCallback.m_iCallback, out _);
            }
        }

        public static void RegisterCallResult(SteamAPICall_t hAPICall, CCallbackBase pCallback)
        {
            if (CallbackResult.ContainsKey(hAPICall))
            {
                CallbackResult[hAPICall] = pCallback;
            }
            else
            {
                CallbackResult.Add(hAPICall, pCallback);
            }
        }

        public static void RunCallbacks()
        {
            //for (auto & c : callbacks)
            //{
            //    c.second.results.clear();
            //}
        }

        public static void FreeCallback(int pipe_id)
        {
            //bool found = Callbacks.TryGetValue(pipe_id, out CCallbackBase value);

            //if (found)
            //{
            //    IntPtr ptr = IntPtr.Zero;
            //    Marshal.StructureToPtr<CCallbackBase>(value, ptr, true);
            //    Marshal.FreeHGlobal(ptr);
            //    Callbacks.Remove(pipe_id);
            //}
        }

        internal static void addCBResult(int k_iCallback, GCMessageAvailable_t data, uint msgSize)
        {
            
        }

        public static SteamAPICall_t AddCallbackResult(object callbackData, int iCallback)
        {

            //TODO
            //int iCallback = callbackData.k_iCallback;

            //lock (call_id_lock)
            //{
            //    last_call_id += 1;
            //    registered_calls[last_call_id] = new AsyncJobResult()
            //    {
            //        job_id = last_call_id,
            //        internal_job_id = job.JobID,
            //        finished = false,
            //        result = null,
            //    };
            //}

            //return last_call_id;

            return 0;
        }

        internal static byte[] GetCallResult(ulong handle, IntPtr callback, int callback_size, int callback_expected)
        {
            if (CallbackResult.ContainsKey(handle))
            {
                return CallbackResult[handle].m_vfptr.GetBytes(callback_size);
            }
            return default;
        }

        public static bool Contains(SteamAPICall_t hSteamAPICall)
        {
            return SteamAPICalls.Contains(hSteamAPICall);
        }
    }

    public class SteamCallback
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void RunDelegate(IntPtr pvParam);
        public static RunDelegate _Run;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void RunFullDelegate(IntPtr pvParam, bool bIOFailure, SteamAPICall_t hSteamAPICall);
        public static RunFullDelegate _RunFull;

        public bool Completed { get; set; }
        public IntPtr Pointer { get; }
        public CCallbackBase CallbackBase { get; }
        public bool IsGameserver => (CallbackBase.m_nCallbackFlags & CCallbackBase.k_ECallbackFlagsGameServer) != 0;

        public SteamCallback(IntPtr _pointer)
        {
            Pointer = _pointer;
            CallbackBase = _pointer.ToType<CCallbackBase>();

            //_Run = Marshal.GetDelegateForFunctionPointer<RunDelegate>(CallbackBase.Run);
            //_RunFull = Marshal.GetDelegateForFunctionPointer<RunFullDelegate>(CallbackBase.RunFull);
        }

        public void Run(IntPtr pvParam)
        {
            _Run(pvParam);
        }

        public void Run(IntPtr pvParam, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            _RunFull(pvParam, bIOFailure, hSteamAPICall);
        }

        public void Update()
        {
            Marshal.StructureToPtr(CallbackBase, Pointer, false);
        }
    }

}


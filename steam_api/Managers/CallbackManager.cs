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

        private static Dictionary<int, CCallbackBase> Client_Callbacks;
        private static Dictionary<int, CCallbackBase> Server_Callbacks;
        private static Dictionary<ulong, CCallbackBase> CallbackResult;
        private static List<SteamAPICall_t> SteamAPICalls;

        private static object call_id_lock = new object();

        static CallbackManager()
        {
            if (Client_Callbacks == null)
            {
                Client_Callbacks = new Dictionary<int, CCallbackBase>();
            }
            if (Server_Callbacks == null)
            {
                Server_Callbacks = new Dictionary<int, CCallbackBase>();
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

        public unsafe static void RegisterCallback(int iCallback, CCallbackBase pCallback, bool Server = false)
        {
            pCallback.m_nCallbackFlags |= k_ECallbackFlagsRegistered;
            pCallback.m_iCallback = iCallback;

            if (Server)
            {
                if (Server_Callbacks.ContainsKey(iCallback))
                    Server_Callbacks[iCallback] = pCallback;
                else
                    Server_Callbacks.Add(iCallback, pCallback);
            }
            else
            {
                if (Client_Callbacks.ContainsKey(iCallback))
                    Client_Callbacks[iCallback] = pCallback;
                else
                    Client_Callbacks.Add(iCallback, pCallback);
            }
        }

        public static void UnregisterCallResult(IntPtr pCallback, SteamAPICall_t hAPICall)
        {

        }

        public static void UnregisterCallback(IntPtr pCallback)
        {

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
}

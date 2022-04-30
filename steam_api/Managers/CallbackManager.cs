using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SKYNET.Callback;
using SKYNET.Helper;
using SKYNET.Steamworks;
using Steamworks;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Managers
{
    public unsafe class CallbackManager
    {
        private static ConcurrentDictionary<int, SteamCallback> Client_Callbacks;
        private static ConcurrentDictionary<int, SteamCallback> Server_Callbacks;
        public static SteamAPICall_t CurrentCall;

        private static Dictionary<ulong, CCallbackBase> CallbackResult;
        private static Dictionary<SteamAPICall_t, ICallbackData> SteamAPICalls;

        private static object call_id_lock = new object();

        static CallbackManager()
        {
            CurrentCall = 0;

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
                SteamAPICalls = new Dictionary<SteamAPICall_t, ICallbackData>();
            }
            if (CallbackResult == null)
            {
                CallbackResult = new Dictionary<ulong, CCallbackBase>();
            }
        }

        public unsafe static void RegisterCallback(int iCallback, IntPtr ptrCallback, bool Server = false)
        {
            if (iCallback == new SteamAPICallCompleted_t().Callback)
            {
                var CBase = ptrCallback.ToType<CCallbackBase>().m_iCallback;
                Write("Completeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeed");
            }

            SteamCallback sCallback = new SteamCallback(ptrCallback, iCallback);
            sCallback.Register(iCallback);


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

        public static void RegisterCallResult(IntPtr ptrCallback, SteamAPICall_t hAPICall)
        {
            //SteamCallback sCallback = new SteamCallback(ptrCallback);
            //sCallback.SteamAPICall = hAPICall;

            //HTTPRequestCompleted_t data = new HTTPRequestCompleted_t();
            //data.ContextValue = 0;
            //data.RequestSuccessful = false;
            //data.StatusCode = HTTPStatusCode.Code404NotFound;
            //data.BodySize = 0;

            //sCallback.Run(data, false, hAPICall);
        }

        public static void UnregisterCallResult(IntPtr pCallback, SteamAPICall_t hAPICall)
        {
            // TODO
        }

        public static void UnregisterCallback(IntPtr ptrCallback)
        {
            CCallbackBase pCallback = ptrCallback.ToType<CCallbackBase>();

            if (pCallback.m_iCallback == new SteamAPICallCompleted_t().Callback)
            {
                pCallback.m_nCallbackFlags &= SteamCallback.k_ECallbackFlagsRegistered;
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


        public static void RunCallbacks()
        {

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


        public static SteamAPICall_t AddCallbackResult(ICallbackData data)
        {
            CurrentCall++;

            SteamAPICalls.Add(CurrentCall, data);

            SteamEmulator.Debug($"Registered CallbackResult {CurrentCall}");

            return CurrentCall;
        }

        public static bool Contains(SteamAPICall_t hSteamAPICall)
        {
            return SteamAPICalls.ContainsKey(hSteamAPICall);
        }

        private static void Write(string v)
        {
            SteamEmulator.Write("Callback Manager", v);
        }
    }

    public class SteamCallback
    {
        public const byte k_ECallbackFlagsRegistered = 1;
        public const byte k_ECallbackFlagsGameServer = 2;

        public CallbackType MainType { get; set; }
        public CallbackType CallType { get; set; }
        public bool Completed { get; set; }
        public IntPtr Pointer { get; }
        public CCallbackBase CallbackBase { get; }
        public SteamAPICall_t SteamAPICall { get; set; }
        public bool IsGameserver => (CallbackBase.m_nCallbackFlags & CCallbackBase.k_ECallbackFlagsGameServer) != 0;


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void RunCallbackDelegate(IntPtr pvParam);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void RunCallResultDelegate(IntPtr pvParam, [MarshalAs(UnmanagedType.I1)] bool bIOFailure, SteamAPICall_t hSteamAPICall);

        [MarshalAs(UnmanagedType.FunctionPtr)]
        private RunCallbackDelegate RunCallback;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        private RunCallResultDelegate RunCallResult;


        public SteamCallback(IntPtr _pointer)
        {
            Pointer = _pointer;
            CallbackBase = _pointer.ToType<CCallbackBase>();
        }

        public SteamCallback(IntPtr _pointer, int iCallback)
        {
            Pointer = _pointer;
            CallbackBase = _pointer.ToType<CCallbackBase>();
            MainType = iCallback.GetCallbackType();
            CallType = (CallbackType)iCallback;

            try
            {
                //IntPtr p_RunCallback = CallbackBase.m_vfptr;
                //RunCallback = Marshal.GetDelegateForFunctionPointer<RunCallbackDelegate>(p_RunCallback);

                IntPtr p_RunCallResult = CallbackBase.m_vfptr + 8;
                RunCallResult = Marshal.GetDelegateForFunctionPointer<RunCallResultDelegate>(p_RunCallResult);

            //    SteamEmulator.Write("SteamCallback", "************************************* RunCallResult NULL:    " + RunCallResult == null);
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("SteamCallback", ex);
            }
        }

        public void Run(IntPtr pvParam)
        {
            //_Run(pvParam);
        }

        public void Run(ICallbackData data, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            IntPtr pvParam = Marshal.AllocHGlobal(data.DataSize);
            Marshal.StructureToPtr(data, pvParam, false);
            Run(pvParam, bIOFailure, hSteamAPICall);
        }

        public void Run(IntPtr pvParam, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            RunCallResult(pvParam, bIOFailure, hSteamAPICall);
        }

        public void Update()
        {
            Marshal.StructureToPtr(CallbackBase, Pointer, false);
        }

        public void Register(int iCallback)
        {
            CallbackBase.m_nCallbackFlags |= k_ECallbackFlagsRegistered;
            CallbackBase.m_iCallback = iCallback;
            Update();
        }
        public void Unregister()
        {
            CallbackBase.m_nCallbackFlags &= k_ECallbackFlagsRegistered;
            Update();
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    internal class CCallbackBaseVTable
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RunCBDel(IntPtr thisptr, IntPtr pvParam);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RunCRDel(IntPtr thisptr, IntPtr pvParam, [MarshalAs(UnmanagedType.I1)] bool bIOFailure, ulong hSteamAPICall);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int GetCallbackSizeBytesDel(IntPtr thisptr);

        private const CallingConvention cc = CallingConvention.Cdecl;

        [NonSerialized]
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public RunCRDel m_RunCallResult;

        [NonSerialized]
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public RunCBDel m_RunCallback;

        [NonSerialized]
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public GetCallbackSizeBytesDel m_GetCallbackSizeBytes;
    }
}


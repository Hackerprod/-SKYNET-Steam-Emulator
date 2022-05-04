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
        private static ConcurrentDictionary<CallbackType, SteamCallback> SteamCallbacks;
        public static Dictionary<SteamAPICall_t, ICallbackData> SteamAPICalls { get; set; }

        public static SteamAPICall_t CurrentCall;

        static CallbackManager()
        {
            CurrentCall = 0;

            SteamCallbacks = new ConcurrentDictionary<CallbackType, SteamCallback>();
            SteamAPICalls = new Dictionary<SteamAPICall_t, ICallbackData>();
        }

        public static void RegisterCallback(SteamCallback sCallback)
        {
            if (sCallback.SteamAPICall == (ulong)new SteamAPICallCompleted_t().CallbackType)
            {
                Write("Completeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeed");
            }

            sCallback.Register();
            CallbackType callType = (CallbackType)sCallback.CallbackType;

            MutexHelper.Wait("SteamCallbacks", delegate
            {
                if (SteamCallbacks.ContainsKey(callType))
                {
                    SteamCallbacks[callType] = sCallback;
                }
                else
                {
                    SteamCallbacks.TryAdd(callType, sCallback);
                }
            });
        }

        public static void RegisterCallResult(SteamCallback steamCallback)
        {
            Write($"RegisterCallResult: Processing callback {steamCallback.SteamAPICall}");

            MutexHelper.Wait("RegisterCallResult", delegate
            {
                try
                {
                    CallbackType callType = (CallbackType)steamCallback.CallbackBase.m_iCallback;

                    MutexHelper.Wait("SteamCallbacks", delegate
                    {
                        if (SteamCallbacks.ContainsKey(callType))
                        {
                            Write($"RegisterCallResult: SteamCallbacks contains key {callType}");
                            SteamCallbacks[callType] = steamCallback;
                        }
                        else
                        {
                            Write($"RegisterCallResult: Creating new SteamCallbacks with key {callType}");
                            SteamCallbacks.TryAdd(callType, steamCallback);
                        }

                        RunCallbacks();
                    });

                }
                catch (Exception ex)
                {
                    Write($"Error in RegisterCallResult, {ex}");
                }
            });
        }

        public static void RunCallbacks()
        {
            MutexHelper.Wait("SteamAPICalls", delegate
            {
                foreach (var KV in SteamAPICalls)
                {
                    try
                    {
                        SteamAPICall_t APICall = KV.Key;
                        ICallbackData callbackData = KV.Value;

                        MutexHelper.Wait("SteamCallbacks", delegate
                        {
                            if (SteamCallbacks.ContainsKey(callbackData.CallbackType))
                            {
                                SteamEmulator.Debug($"Foooooooooooooooooooooooooooound in RunCallbacks, {callbackData.CallbackType}");
                                SteamEmulator.Debug($"Found callback {callbackData.CallbackType}");

                                SteamCallback Callback = SteamCallbacks[callbackData.CallbackType];
                                Callback.Run(callbackData, false, APICall);
                                SteamAPICalls.Remove(APICall);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Write("" + ex);
                    }
                }
            });
        }

        public static SteamAPICall_t AddCallbackResult(ICallbackData data)
        {
            CurrentCall++;

            MutexHelper.Wait("SteamAPICalls", delegate
            {
                SteamAPICalls.Add(CurrentCall, data);
            });

            Write($"Registered CallbackResult {CurrentCall} {((int)data.CallbackType).GetCallbackType()} {data.CallbackType} ");

            return CurrentCall;
        }

        internal static void UnregisterCallback(IntPtr pCallback)
        {
            CCallbackBase Callback = pCallback.ToType<CCallbackBase>();
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

        private static Dictionary<SteamAPICall_t, ICallbackData> results;

        public CallbackType CallbackType { get; set; }
        public CallbackType BaseType { get { return ((int)CallbackType).GetCallbackType(); } }
        public bool Completed { get; set; }
        public IntPtr Pointer { get; }
        public CCallbackBase CallbackBase { get; }
        public SteamAPICall_t SteamAPICall { get; set; }
        public bool HasGameserver => (CallbackBase.m_nCallbackFlags & CCallbackBase.k_ECallbackFlagsGameServer) != 0;
        public bool HasResult { get; set; }
        public DateTime Created { get; set; }

        #region Delegates 

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RunCBDel(IntPtr pvParam);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RunCRDel(IntPtr pvParam, [MarshalAs(UnmanagedType.I1)] bool bIOFailure, ulong hSteamAPICall);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int GetCallbackSizeBytesDel();

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

        #endregion

        public SteamCallback(IntPtr _pointer, bool hasResult = false)
        {
            Pointer = _pointer;
            CallbackBase = _pointer.ToType<CCallbackBase>();
            Created = DateTime.Now;
            results = new Dictionary<SteamAPICall_t, ICallbackData>();
            HasResult = hasResult;

            CCallResult cResult = CallbackBase.m_vfptr.ToType<CCallResult>();
            m_RunCallback = Marshal.GetDelegateForFunctionPointer<RunCBDel>(cResult.m_RunCallback);
            m_RunCallResult = Marshal.GetDelegateForFunctionPointer<RunCRDel>(cResult.m_RunCallResult);
        }

        public SteamCallback(IntPtr _pointer, int iCallback, bool hasResult = false)
        {
            Pointer = _pointer;
            CallbackBase = _pointer.ToType<CCallbackBase>();
            CallbackBase.m_iCallback = iCallback;
            CallbackType = (CallbackType)iCallback;
            Created = DateTime.Now;
            results = new Dictionary<SteamAPICall_t, ICallbackData>();
            HasResult = hasResult;

            CCallResult cResult = CallbackBase.m_vfptr.ToType<CCallResult>();
            m_RunCallback = Marshal.GetDelegateForFunctionPointer<RunCBDel>(cResult.m_RunCallback);
            m_RunCallResult = Marshal.GetDelegateForFunctionPointer<RunCRDel>(cResult.m_RunCallResult);
        }

        public void Run(IntPtr pvParam)
        {
            m_RunCallback.Invoke(pvParam);
            SteamEmulator.Debug($"Called Run(IntPtr) function in callback {SteamAPICall}");
        }

        public void Run(ICallbackData data, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            IntPtr pvParam = Marshal.AllocHGlobal(data.DataSize);
            Marshal.StructureToPtr(data, pvParam, false);
            Run(pvParam, bIOFailure, hSteamAPICall);

            SteamAPICallCompleted_t completedData = new SteamAPICallCompleted_t()
            {
                m_hAsyncCall = hSteamAPICall,
                m_iCallback = (int)data.CallbackType,
                m_cubParam = (uint)data.DataSize
            };

            IntPtr pvParam2 = Marshal.AllocHGlobal(completedData.DataSize);
            Marshal.StructureToPtr(completedData, pvParam2, false);
            //Run(pvParam2, bIOFailure, hSteamAPICall);
            //Run(pvParam2);
        }

        public void Run(IntPtr pvParam, bool bIOFailure, SteamAPICall_t hSteamAPICall)
        {
            m_RunCallResult.Invoke(pvParam, bIOFailure, hSteamAPICall);
            SteamEmulator.Debug($"Called Run(IntPtr, bool, SteamAPICall_t) function in callback {hSteamAPICall}");
        }

        public void Update()
        {
            Marshal.StructureToPtr(CallbackBase, Pointer, false);
        }

        public void Register()
        {
            CallbackBase.m_nCallbackFlags |= k_ECallbackFlagsRegistered;
            CallbackBase.m_iCallback = (int)CallbackType;
            Update();
        }
        public void Unregister()
        {
            CallbackBase.m_nCallbackFlags &= k_ECallbackFlagsRegistered;
            Update();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CCallResult
        {
            public IntPtr m_RunCallResult;
            public IntPtr m_RunCallback;
            public IntPtr m_GetCallbackSizeBytes;
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CallbackMsg_t
    {
        public int m_hSteamUser; // Specific user to whom this callback applies.
        public int m_iCallback; // Callback identifier.  (Corresponds to the k_iCallback enum in the callback structure.)
        public IntPtr m_pubParam; // Points to the callback structure
        public int m_cubParam; // Size of the data pointed to by m_pubParam
    }
}



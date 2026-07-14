using SKYNET.Callback;
using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using SteamAPICall_t = System.UInt64;

public static class CallbackManager_
{
    private static readonly ConcurrentQueue<CallbackMsg_t> CallbackQueue = new ConcurrentQueue<CallbackMsg_t>();
    private static readonly ConcurrentDictionary<int, IntPtr> Callbacks = new ConcurrentDictionary<int, IntPtr>();
    private static readonly ConcurrentDictionary<SteamAPICall_t, IntPtr> CallResults = new ConcurrentDictionary<SteamAPICall_t, IntPtr>();
    private static SteamAPICall_t NextAPICallID = 1;

    // Register a normal callback
    public static int RegisterCallback(IntPtr pCallback, int callbackID)
    {
        if (!Callbacks.ContainsKey(callbackID))
        {
            Callbacks[callbackID] = pCallback;
            SteamEmulator.Write("CallbackManager", $"Callback registered: ID {callbackID}");
            return callbackID;
        }

        SteamEmulator.Write("CallbackManager", $"Error: Callback ID {callbackID} is already registered.");
        return -1;
    }

    public static void UnregisterCallback(int callbackID)
    {
        if (Callbacks.TryRemove(callbackID, out _))
        {
            SteamEmulator.Write("CallbackManager", $"Callback unregistered: ID {callbackID}");
        }
        else
        {
            SteamEmulator.Write("CallbackManager", $"Error: No callback found with ID {callbackID} to unregister.");
        }
    }

    // Register a call result
    public static bool RegisterCallResult(IntPtr pCallback, SteamAPICall_t hAPICall)
    {
        if (CallResults.TryAdd(hAPICall, pCallback))
        {
            SteamEmulator.Write("CallbackManager", $"Call result registered: APICallID {hAPICall}");
            return true;
        }

        SteamEmulator.Write("CallbackManager", $"Error registering call result: APICallID {hAPICall}");
        return false;
    }

    // Add a Callback Result associated with a specific structure
    public static SteamAPICall_t AddCallbackResult<T>(T result, bool ready = false) where T : struct
    {
        var apiCallID = GetNextAPICallID();

        //// Assign the callback result
        //var callbackResult = new CallbackResult
        //{
        //    APICallID = apiCallID,
        //    Data = result,
        //    Size = Marshal.SizeOf<T>()
        //};

        //if (CallResults.TryAdd(apiCallID, callbackResult))
        //{
        //    SteamEmulator.Write("CallbackManager", $"Callback result registered for APICallID {apiCallID}, structure {typeof(T).Name}");
        //}
        //else
        //{
        //    SteamEmulator.Write("CallbackManager", $"Error registering callback result for APICallID {apiCallID}");
        //}

        return apiCallID;
    }

    public static void UnregisterCallResult(IntPtr pCallback, SteamAPICall_t hAPICall)
    {
        if (CallResults.TryRemove(hAPICall, out _))
        {
            SteamEmulator.Write("CallbackManager", $"Call result unregistered: APICallID {hAPICall}");
        }
        else
        {
            SteamEmulator.Write("CallbackManager", $"Error unregistering call result: APICallID {hAPICall}");
        }
    }

    // Trigger a callback
    public static void TriggerCallback<T>(int callbackID, T data) where T : struct
    {
        if (Callbacks.TryGetValue(callbackID, out var pCallback))
        {
            var size = Marshal.SizeOf<T>();
            var dataPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(data, dataPtr, false);

            try
            {
                var callbackBase = Marshal.PtrToStructure<CCallbackBase>(pCallback);
                if (callbackBase.Run != IntPtr.Zero)
                {
                    var runDelegate = Marshal.GetDelegateForFunctionPointer<RunCallbackDelegate>(callbackBase.Run);
                    runDelegate(dataPtr);
                    SteamEmulator.Write("CallbackManager", $"Callback executed for ID {callbackID}");
                }
            }
            finally
            {
                Marshal.FreeHGlobal(dataPtr);
            }
        }
        else
        {
            SteamEmulator.Write("CallbackManager", $"No callback found with ID {callbackID}");
        }
    }

    // Trigger a call result
    public static void TriggerCallResult<T>(SteamAPICall_t hAPICall, T data, bool ioFailure = false) where T : struct
    {
        if (CallResults.TryRemove(hAPICall, out var pCallback))
        {
            var size = Marshal.SizeOf<T>();
            var dataPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(data, dataPtr, false);

            try
            {
                var callbackBase = Marshal.PtrToStructure<CCallbackBase>(pCallback);
                if (callbackBase.Run != IntPtr.Zero)
                {
                    var runDelegate = Marshal.GetDelegateForFunctionPointer<RunCallResultDelegate>(callbackBase.Run);
                    runDelegate(dataPtr, ioFailure, hAPICall);
                    SteamEmulator.Write("CallbackManager", $"Call result executed for APICallID {hAPICall}");
                }
            }
            finally
            {
                Marshal.FreeHGlobal(dataPtr);
            }
        }
        else
        {
            SteamEmulator.Write("CallbackManager", $"No call result found for APICallID {hAPICall}");
        }
    }

    // Run all pending callbacks
    public static void RunCallbacks()
    {
        while (CallbackQueue.TryDequeue(out var callbackMsg))
        {
            if (Callbacks.TryGetValue(callbackMsg.m_iCallback, out var pCallback))
            {
                var callbackBase = Marshal.PtrToStructure<CCallbackBase>(pCallback);
                if (callbackBase.Run != IntPtr.Zero)
                {
                    var runDelegate = Marshal.GetDelegateForFunctionPointer<RunCallbackDelegate>(callbackBase.Run);
                    runDelegate(callbackMsg.m_pubParam);
                    SteamEmulator.Write("CallbackManager", $"Callback executed: ID {callbackMsg.m_iCallback}");
                }
            }
        }
    }

    // Add a callback message to the queue
    public static void AddCallbackMessage<T>(int callbackID, T data) where T : struct
    {
        var size = Marshal.SizeOf<T>();
        var dataPtr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(data, dataPtr, false);

        var callbackMsg = new CallbackMsg_t
        {
            m_hSteamUser = (int)SteamEmulator.HSteamUser,
            m_iCallback = callbackID,
            m_pubParam = dataPtr,
            m_cubParam = size
        };

        CallbackQueue.Enqueue(callbackMsg);
        SteamEmulator.Write("CallbackManager", $"Callback message added: ID {callbackID}");
    }

    internal static void AddCallback(ICallbackData data)
    {
        
    }

    // Generate a new ID for API Calls
    private static SteamAPICall_t GetNextAPICallID()
    {
        return NextAPICallID++;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CCallbackBase
    {
        public IntPtr Run;
        public IntPtr RunExtra;
        public IntPtr Destructor;
        public int CallbackFlags;
        public int CallbackID;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CallbackMsg_t
    {
        public int m_hSteamUser;
        public int m_iCallback;
        public IntPtr m_pubParam;
        public int m_cubParam;
    }

    // Structure to store callback results
    internal class CallbackResult
    {
        public SteamAPICall_t APICallID { get; set; }
        public object Data { get; set; }
        public int Size { get; set; }
    } 

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void RunCallbackDelegate(IntPtr pvParam);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void RunCallResultDelegate(IntPtr pvParam, bool ioFailure, SteamAPICall_t hAPICall);
}

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
            SteamEmulator.Write("CallbackManager", $"Callback registrado: ID {callbackID}");
            return callbackID;
        }

        SteamEmulator.Write("CallbackManager", $"Error: Callback ID {callbackID} ya está registrado.");
        return -1;
    }

    public static void UnregisterCallback(int callbackID)
    {
        if (Callbacks.TryRemove(callbackID, out _))
        {
            SteamEmulator.Write("CallbackManager", $"Callback desregistrado: ID {callbackID}");
        }
        else
        {
            SteamEmulator.Write("CallbackManager", $"Error: No se encontró callback con ID {callbackID} para desregistrar.");
        }
    }

    // Register a call result
    public static bool RegisterCallResult(IntPtr pCallback, SteamAPICall_t hAPICall)
    {
        if (CallResults.TryAdd(hAPICall, pCallback))
        {
            SteamEmulator.Write("CallbackManager", $"Call result registrado: APICallID {hAPICall}");
            return true;
        }

        SteamEmulator.Write("CallbackManager", $"Error al registrar call result: APICallID {hAPICall}");
        return false;
    }

    // Agregar un Callback Result asociado a una estructura específica
    public static SteamAPICall_t AddCallbackResult<T>(T result, bool ready = false) where T : struct
    {
        var apiCallID = GetNextAPICallID();

        //// Asignar el resultado del callback
        //var callbackResult = new CallbackResult
        //{
        //    APICallID = apiCallID,
        //    Data = result,
        //    Size = Marshal.SizeOf<T>()
        //};

        //if (CallResults.TryAdd(apiCallID, callbackResult))
        //{
        //    SteamEmulator.Write("CallbackManager", $"Callback result registrado para APICallID {apiCallID}, estructura {typeof(T).Name}");
        //}
        //else
        //{
        //    SteamEmulator.Write("CallbackManager", $"Error al registrar callback result para APICallID {apiCallID}");
        //}

        return apiCallID;
    }

    public static void UnregisterCallResult(IntPtr pCallback, SteamAPICall_t hAPICall)
    {
        if (CallResults.TryRemove(hAPICall, out _))
        {
            SteamEmulator.Write("CallbackManager", $"Call result desregistrado: APICallID {hAPICall}");
        }
        else
        {
            SteamEmulator.Write("CallbackManager", $"Error al desregistrar call result: APICallID {hAPICall}");
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
                    SteamEmulator.Write("CallbackManager", $"Callback ejecutado para ID {callbackID}");
                }
            }
            finally
            {
                Marshal.FreeHGlobal(dataPtr);
            }
        }
        else
        {
            SteamEmulator.Write("CallbackManager", $"No se encontró callback con ID {callbackID}");
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
                    SteamEmulator.Write("CallbackManager", $"Call result ejecutado para APICallID {hAPICall}");
                }
            }
            finally
            {
                Marshal.FreeHGlobal(dataPtr);
            }
        }
        else
        {
            SteamEmulator.Write("CallbackManager", $"No se encontró call result para APICallID {hAPICall}");
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
                    SteamEmulator.Write("CallbackManager", $"Callback ejecutado: ID {callbackMsg.m_iCallback}");
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
        SteamEmulator.Write("CallbackManager", $"Mensaje de callback agregado: ID {callbackID}");
    }

    internal static void AddCallback(ICallbackData data)
    {
        
    }

    // Generar un nuevo ID para API Calls
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

    // Estructura para almacenar resultados de callbacks
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

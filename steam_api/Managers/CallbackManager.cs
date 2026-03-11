using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SKYNET.Callback;
using SKYNET.Steamworks;

using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;
using SteamAPICall_t = System.UInt64;

namespace SKYNET.Managers
{
    public static class CallbackManager
    {
        private static readonly object RegistrationLock = new object();
        private static readonly object AsyncLock = new object();

        private static readonly ConcurrentDictionary<IntPtr, SteamCallback> RegisteredCallbacks = new ConcurrentDictionary<IntPtr, SteamCallback>();
        private static readonly ConcurrentDictionary<SteamAPICall_t, ConcurrentDictionary<IntPtr, SteamCallback>> RegisteredCallResults = new ConcurrentDictionary<SteamAPICall_t, ConcurrentDictionary<IntPtr, SteamCallback>>();
        private static readonly ConcurrentDictionary<SteamAPICall_t, AsyncCallState> AsyncCalls = new ConcurrentDictionary<SteamAPICall_t, AsyncCallState>();

        private static readonly ConcurrentQueue<QueuedCallback> ClientCallbacks = new ConcurrentQueue<QueuedCallback>();
        private static readonly ConcurrentQueue<QueuedCallback> GameServerCallbacks = new ConcurrentQueue<QueuedCallback>();
        private static readonly ConcurrentQueue<ManualDispatchItem> ClientManualDispatch = new ConcurrentQueue<ManualDispatchItem>();
        private static readonly ConcurrentQueue<ManualDispatchItem> GameServerManualDispatch = new ConcurrentQueue<ManualDispatchItem>();

        private static readonly ConcurrentDictionary<HSteamPipe, ManualDispatchLease> ManualDispatchLeases = new ConcurrentDictionary<HSteamPipe, ManualDispatchLease>();

        private static SteamAPICall_t currentCall = 1000;

        public static void RegisterCallback(SteamCallback callback)
        {
            callback.Register();
            RegisteredCallbacks[callback.Pointer] = callback;
        }

        public static void RegisterCallResult(SteamCallback callback)
        {
            callback.Register();
            var registrations = RegisteredCallResults.GetOrAdd(callback.SteamAPICall, _ => new ConcurrentDictionary<IntPtr, SteamCallback>());
            registrations[callback.Pointer] = callback;
        }

        public static SteamAPICall_t AddCallbackResult(ICallbackData data, bool readyToCall = true)
        {
            var handle = CreateAsyncCall(data, false, readyToCall);
            Write($"Added CallbackResult {handle} {((int)data.CallbackType).GetCallbackType()} {data.CallbackType}");
            return handle;
        }

        public static SteamAPICall_t AddCallbackResultGameServer(ICallbackData data, bool readyToCall = true)
        {
            var handle = CreateAsyncCall(data, true, readyToCall);
            Write($"Added GameServer CallbackResult {handle} {((int)data.CallbackType).GetCallbackType()} {data.CallbackType}");
            return handle;
        }

        public static void AddCallback(ICallbackData data, bool readyToCall = true)
        {
            if (!readyToCall)
            {
                readyToCall = true;
            }

            EnqueueBroadcast(data, false);
            Write($"Added Callback {((int)data.CallbackType).GetCallbackType()} {data.CallbackType}");
        }

        public static void AddCallbackGameServer(ICallbackData data, bool readyToCall = true)
        {
            if (!readyToCall)
            {
                readyToCall = true;
            }

            EnqueueBroadcast(data, true);
            Write($"Added GameServer Callback {((int)data.CallbackType).GetCallbackType()} {data.CallbackType}");
        }

        public static void RunCallbacks(bool gameServer = false)
        {
            SkyNetEventPump.RunFrame(gameServer);
            DispatchQueuedCallbacks(gameServer);
            DispatchReadyCallResults(gameServer);
        }

        public static void ManualDispatchInit()
        {
        }

        public static void ManualDispatchRunFrame(HSteamPipe hSteamPipe)
        {
        }

        public static bool ManualDispatchGetNextCallback(HSteamPipe hSteamPipe, ref CallbackMsg_t callbackMsg)
        {
            if (ManualDispatchLeases.ContainsKey(hSteamPipe))
            {
                return false;
            }

            if (!TryGetManualQueue(hSteamPipe, out var queue))
            {
                return false;
            }

            if (!queue.TryDequeue(out var next))
            {
                return false;
            }

            IntPtr payload = IntPtr.Zero;
            if (next.Payload.Length > 0)
            {
                payload = Marshal.AllocHGlobal(next.Payload.Length);
                Marshal.Copy(next.Payload, 0, payload, next.Payload.Length);
            }

            callbackMsg.m_hSteamUser = next.SteamUser;
            callbackMsg.m_iCallback = next.CallbackId;
            callbackMsg.m_pubParam = payload;
            callbackMsg.m_cubParam = next.Payload.Length;

            ManualDispatchLeases[hSteamPipe] = new ManualDispatchLease
            {
                Payload = payload
            };

            return true;
        }

        public static void ManualDispatchFreeLastCallback(HSteamPipe hSteamPipe)
        {
            if (ManualDispatchLeases.TryRemove(hSteamPipe, out var lease) && lease.Payload != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(lease.Payload);
            }
        }

        public static bool ManualDispatchGetAPICallResult(HSteamPipe hSteamPipe, SteamAPICall_t apiCall, IntPtr callback, int callbackSize, int callbackExpected, ref bool failed)
        {
            failed = false;

            if (!AsyncCalls.TryGetValue(apiCall, out var state))
            {
                return false;
            }

            if (state.SteamPipe != hSteamPipe)
            {
                return false;
            }

            return GetAPICallResult(apiCall, callback, callbackSize, callbackExpected, ref failed);
        }

        public static bool GetCallResult(ulong handle, out CallbackMessage callback)
        {
            callback = default;
            if (AsyncCalls.TryGetValue(handle, out var state))
            {
                callback = state.Result;
            }
            return callback != null;
        }

        public static bool IsCompleted(ulong apiCall)
        {
            return AsyncCalls.TryGetValue(apiCall, out var state) && state.Result.ReadyToCall;
        }

        public static int GetAPICallFailureReason(ulong apiCall)
        {
            if (!AsyncCalls.TryGetValue(apiCall, out var state))
            {
                return (int)ESteamAPICallFailure.k_ESteamAPICallFailureInvalidHandle;
            }

            if (!state.Result.ReadyToCall)
            {
                return (int)ESteamAPICallFailure.k_ESteamAPICallFailureNone;
            }

            if (state.Result.IOFailure)
            {
                return (int)ESteamAPICallFailure.k_ESteamAPICallFailureNetworkFailure;
            }

            if (state.LastFailureReason != ESteamAPICallFailure.k_ESteamAPICallFailureNone)
            {
                return (int)state.LastFailureReason;
            }

            return (int)ESteamAPICallFailure.k_ESteamAPICallFailureNone;
        }

        public static bool GetAPICallResult(ulong apiCall, IntPtr callback, int callbackSize, int callbackExpected, ref bool failed)
        {
            failed = false;

            if (!AsyncCalls.TryGetValue(apiCall, out var state))
            {
                return false;
            }

            if (!state.Result.ReadyToCall)
            {
                return false;
            }

            if (callbackExpected != (int)state.Result.Data.CallbackType)
            {
                state.LastFailureReason = ESteamAPICallFailure.k_ESteamAPICallFailureMismatchedCallback;
                return false;
            }

            if (callbackSize < state.Result.Data.DataSize)
            {
                return false;
            }

            Marshal.StructureToPtr(state.Result.Data, callback, false);
            failed = state.Result.IOFailure;
            state.LastFailureReason = state.Result.IOFailure
                ? ESteamAPICallFailure.k_ESteamAPICallFailureNetworkFailure
                : ESteamAPICallFailure.k_ESteamAPICallFailureNone;

            AsyncCalls.TryRemove(apiCall, out _);
            RegisteredCallResults.TryRemove(apiCall, out _);

            return true;
        }

        public static bool UnregisterCallResult(SteamCallback callback, ulong apiCall)
        {
            if (!RegisteredCallResults.TryGetValue(apiCall, out var registrations))
            {
                return false;
            }

            var removed = registrations.TryRemove(callback.Pointer, out var removedCallback);
            if (removedCallback != null)
            {
                removedCallback.Unregister();
            }

            if (registrations.IsEmpty)
            {
                RegisteredCallResults.TryRemove(apiCall, out _);
            }

            return removed;
        }

        public static bool UnregisterCallback(SteamCallback callback)
        {
            var removed = RegisteredCallbacks.TryRemove(callback.Pointer, out var removedCallback);
            if (removedCallback != null)
            {
                removedCallback.Unregister();
            }
            return removed;
        }

        private static SteamAPICall_t CreateAsyncCall(ICallbackData data, bool gameServer, bool readyToCall, bool ioFailure = false)
        {
            lock (AsyncLock)
            {
                currentCall++;

                var state = new AsyncCallState
                {
                    Handle = currentCall,
                    IsGameServer = gameServer,
                    SteamPipe = gameServer ? SteamEmulator.HSteamPipe_GS : SteamEmulator.HSteamPipe,
                    SteamUser = gameServer ? SteamEmulator.HSteamUser_GS : SteamEmulator.HSteamUser,
                    Result = new CallbackMessage(data, readyToCall, ioFailure),
                    LastFailureReason = ESteamAPICallFailure.k_ESteamAPICallFailureNone
                };

                AsyncCalls[currentCall] = state;

                if (readyToCall)
                {
                    QueueAPICallCompleted(state);
                }

                return currentCall;
            }
        }

        private static void QueueAPICallCompleted(AsyncCallState state)
        {
            var completed = new SteamAPICallCompleted_t
            {
                m_hAsyncCall = state.Handle,
                m_iCallback = (int)state.Result.Data.CallbackType,
                m_cubParam = (uint)state.Result.Data.DataSize
            };

            EnqueueBroadcast(completed, state.IsGameServer, state.SteamUser);
        }

        private static void EnqueueBroadcast(ICallbackData data, bool gameServer, HSteamUser? steamUser = null)
        {
            var queued = new QueuedCallback
            {
                Data = data,
                GameServer = gameServer,
                SteamUser = steamUser ?? (gameServer ? SteamEmulator.HSteamUser_GS : SteamEmulator.HSteamUser)
            };

            var callbackQueue = gameServer ? GameServerCallbacks : ClientCallbacks;
            var manualQueue = gameServer ? GameServerManualDispatch : ClientManualDispatch;

            callbackQueue.Enqueue(queued);
            manualQueue.Enqueue(new ManualDispatchItem
            {
                SteamUser = queued.SteamUser,
                CallbackId = (int)data.CallbackType,
                Payload = Serialize(data)
            });
        }

        private static void DispatchQueuedCallbacks(bool gameServer)
        {
            var queue = gameServer ? GameServerCallbacks : ClientCallbacks;
            while (queue.TryDequeue(out var queued))
            {
                List<SteamCallback> callbacks;
                lock (RegistrationLock)
                {
                    callbacks = RegisteredCallbacks.Values
                        .Where(c => c.CallbackType == queued.Data.CallbackType && c.HasGameserver == gameServer)
                        .ToList();
                }

                foreach (var callback in callbacks)
                {
                    callback.Run(queued.Data);
                }
            }
        }

        private static void DispatchReadyCallResults(bool gameServer)
        {
            foreach (var state in AsyncCalls.Values.Where(s => s.IsGameServer == gameServer && s.Result.ReadyToCall).ToList())
            {
                if (!RegisteredCallResults.TryGetValue(state.Handle, out var registrations))
                {
                    continue;
                }

                foreach (var registration in registrations.Values.Where(c => c.HasGameserver == gameServer && c.CallbackType == state.Result.Data.CallbackType).ToList())
                {
                    registration.Run(state.Result.Data, state.Result.IOFailure, state.Handle);
                    registrations.TryRemove(registration.Pointer, out _);
                    registration.Unregister();
                }

                if (registrations.IsEmpty)
                {
                    RegisteredCallResults.TryRemove(state.Handle, out _);
                }
            }
        }

        private static bool TryGetManualQueue(HSteamPipe hSteamPipe, out ConcurrentQueue<ManualDispatchItem> queue)
        {
            if (hSteamPipe == SteamEmulator.HSteamPipe_GS)
            {
                queue = GameServerManualDispatch;
                return true;
            }

            if (hSteamPipe == SteamEmulator.HSteamPipe)
            {
                queue = ClientManualDispatch;
                return true;
            }

            queue = null;
            return false;
        }

        private static byte[] Serialize(ICallbackData data)
        {
            IntPtr buffer = IntPtr.Zero;
            try
            {
                buffer = Marshal.AllocHGlobal(data.DataSize);
                Marshal.StructureToPtr(data, buffer, false);
                var bytes = new byte[data.DataSize];
                Marshal.Copy(buffer, bytes, 0, data.DataSize);
                return bytes;
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }
        }

        private static void Write(string message)
        {
            SteamEmulator.Write("CallbackManager", message);
        }

        private sealed class AsyncCallState
        {
            public SteamAPICall_t Handle;
            public CallbackMessage Result;
            public bool IsGameServer;
            public HSteamPipe SteamPipe;
            public HSteamUser SteamUser;
            public ESteamAPICallFailure LastFailureReason;
        }

        private sealed class QueuedCallback
        {
            public ICallbackData Data;
            public bool GameServer;
            public HSteamUser SteamUser;
        }

        private sealed class ManualDispatchItem
        {
            public HSteamUser SteamUser;
            public int CallbackId;
            public byte[] Payload;
        }

        private sealed class ManualDispatchLease
        {
            public IntPtr Payload;
        }
    }
}

using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;
using SteamAPICall_t = System.UInt64;

namespace SKYNET.Managers
{
    public static class CallbackManager
    {
        private static readonly object Gate = new object();
        private static readonly TimeSpan DefaultCallbackDelay = TimeSpan.FromMilliseconds(2);
        private static readonly TimeSpan WaitForCallResultCallback = TimeSpan.FromMilliseconds(10);
        private static readonly TimeSpan CallResultLifetime = TimeSpan.FromSeconds(120);

        private static readonly Dictionary<CallbackKey, List<SteamCallback>> RegisteredCallbacks = new Dictionary<CallbackKey, List<SteamCallback>>();
        private static readonly Dictionary<CallbackKey, List<PendingDirectCallback>> PendingDirectCallbacks = new Dictionary<CallbackKey, List<PendingDirectCallback>>();
        private static readonly List<SteamCallback> CompletedCallbacks = new List<SteamCallback>();
        private static readonly List<CallbackRecord> Records = new List<CallbackRecord>();
        private static readonly Dictionary<HSteamPipe, Queue<ManualDispatchItem>> ManualDispatchQueues = new Dictionary<HSteamPipe, Queue<ManualDispatchItem>>();
        private static readonly Dictionary<HSteamPipe, ManualDispatchLease> ManualDispatchLeases = new Dictionary<HSteamPipe, ManualDispatchLease>();

        private static SteamAPICall_t currentCall = 1000;
        private static int gameServerConnectionReplayIssued;

        public static void RegisterCallback(SteamCallback callback)
        {
            if (callback == null || callback.Pointer == IntPtr.Zero)
            {
                return;
            }

            bool replayConnectionState = false;
            lock (Gate)
            {
                if (callback.CallbackType == CallbackType.SteamAPICallCompleted)
                {
                    AddUnique(CompletedCallbacks, callback);
                    callback.Register();
                    Write($"Registered SteamAPICallCompleted callback ptr=0x{callback.Pointer.ToInt64():X} gs={callback.HasGameserver}");
                    return;
                }

                var key = new CallbackKey((int)callback.CallbackType, callback.HasGameserver);
                var callbacks = GetCallbackList(key);
                AddUnique(callbacks, callback);
                callback.Register();

                if (PendingDirectCallbacks.TryGetValue(key, out var pendingCallbacks))
                {
                    foreach (var pending in pendingCallbacks)
                    {
                        AddDirectRecord(key, pending, new[] { callback });
                    }
                }

                replayConnectionState = callback.HasGameserver &&
                    (callback.CallbackType == CallbackType.SteamServersConnected ||
                     callback.CallbackType == CallbackType.SteamGameCoordinator);

                Write($"Registered callback {(int)callback.CallbackType} {callback.CallbackType} ptr=0x{callback.Pointer.ToInt64():X} gs={callback.HasGameserver}");
            }

            if (replayConnectionState)
            {
                ReplayGameServerConnectionStateIfNeeded(callback);
            }
        }

        public static void RegisterCallResult(SteamCallback callback)
        {
            if (callback == null || callback.Pointer == IntPtr.Zero || callback.SteamAPICall == 0)
            {
                return;
            }

            lock (Gate)
            {
                var record = FindRecord(callback.SteamAPICall);
                if (record == null)
                {
                    Write($"RegisterCallResult ignored missing handle={callback.SteamAPICall} ptr=0x{callback.Pointer.ToInt64():X}");
                    return;
                }

                if ((int)callback.CallbackType == 0)
                {
                    callback.CallbackType = (CallbackType)record.CallbackId;
                }

                AddUnique(record.Callbacks, callback);
                callback.Register();
                Write($"Registered call result handle={callback.SteamAPICall} callback={record.CallbackId} ptr=0x{callback.Pointer.ToInt64():X} gs={record.GameServer}");
            }
        }

        public static SteamAPICall_t AddCallbackResult(ICallbackData data, bool readyToCall = true)
        {
            return AddCallResult(data, false, readyToCall, false);
        }

        public static SteamAPICall_t AddCallbackResultGameServer(ICallbackData data, bool readyToCall = true)
        {
            return AddCallResult(data, true, readyToCall, false);
        }

        public static bool CompleteCallbackResult(SteamAPICall_t handle, ICallbackData data, bool ioFailure = false)
        {
            if (data == null)
            {
                return false;
            }

            lock (Gate)
            {
                var record = FindRecord(handle);
                if (record == null)
                {
                    return false;
                }

                var now = DateTime.UtcNow;
                record.CallbackId = (int)data.CallbackType;
                record.Payload = Serialize(data);
                record.Data = data;
                record.Ready = true;
                record.DueUtc = now;
                record.ReadyUtc = now;
                record.IOFailure = ioFailure;
                record.LastFailureReason = ioFailure
                    ? ESteamAPICallFailure.k_ESteamAPICallFailureNetworkFailure
                    : ESteamAPICallFailure.k_ESteamAPICallFailureNone;
                record.ToDelete = false;
                record.CompletedNotificationSent = false;

                Write($"Completed CallbackResult {handle} callback={record.CallbackId} size={record.Payload.Length} failed={ioFailure}");
                return true;
            }
        }

        public static void AddCallback(ICallbackData data, bool readyToCall = true)
        {
            AddDirectCallback(data, false, readyToCall);
        }

        public static void AddCallbackGameServer(ICallbackData data, bool readyToCall = true)
        {
            AddDirectCallback(data, true, readyToCall);
        }

        public static void ResetGameServerConnectionReplay()
        {
            Interlocked.Exchange(ref gameServerConnectionReplayIssued, 0);
        }

        public static void RunCallbacks(bool gameServer = false)
        {
            EventPump.RunFrame(gameServer);
            var invocations = DrainForFrame(gameServer, SteamEmulator.HSteamPipe, SteamEmulator.HSteamUser);
            InvokeCallbacks(invocations);
        }

        public static void ManualDispatchInit()
        {
        }

        public static void ManualDispatchRunFrame(HSteamPipe hSteamPipe)
        {
            bool gameServer = hSteamPipe == SteamEmulator.HSteamPipe_GS;
            EventPump.RunFrame(gameServer);
            var invocations = DrainForFrame(gameServer, hSteamPipe, gameServer ? SteamEmulator.HSteamUser_GS : SteamEmulator.HSteamUser);
            InvokeCallbacks(invocations);
        }

        public static bool ManualDispatchGetNextCallback(HSteamPipe hSteamPipe, ref CallbackMsg_t callbackMsg)
        {
            lock (Gate)
            {
                if (ManualDispatchLeases.ContainsKey(hSteamPipe))
                {
                    return false;
                }

                if (!ManualDispatchQueues.TryGetValue(hSteamPipe, out var queue) || queue.Count == 0)
                {
                    return false;
                }

                var next = queue.Dequeue();
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
                ManualDispatchLeases[hSteamPipe] = new ManualDispatchLease { Payload = payload };
                return true;
            }
        }

        public static void ManualDispatchFreeLastCallback(HSteamPipe hSteamPipe)
        {
            lock (Gate)
            {
                if (!ManualDispatchLeases.TryGetValue(hSteamPipe, out var lease))
                {
                    return;
                }

                ManualDispatchLeases.Remove(hSteamPipe);
                if (lease.Payload != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(lease.Payload);
                }
            }
        }

        public static bool ManualDispatchGetAPICallResult(HSteamPipe hSteamPipe, SteamAPICall_t apiCall, IntPtr callback, int callbackSize, int callbackExpected, ref bool failed)
        {
            lock (Gate)
            {
                var record = FindRecord(apiCall);
                if (record == null || record.SteamPipe != hSteamPipe)
                {
                    failed = false;
                    return false;
                }
            }

            return GetAPICallResult(apiCall, callback, callbackSize, callbackExpected, ref failed);
        }

        public static bool GetCallResult(ulong handle, out CallbackMessage callback)
        {
            lock (Gate)
            {
                var record = FindRecord(handle);
                if (record == null)
                {
                    callback = default;
                    return false;
                }

                callback = new CallbackMessage(record.Data, record.Ready, record.IOFailure);
                return true;
            }
        }

        public static bool IsCompleted(ulong apiCall)
        {
            lock (Gate)
            {
                var record = FindRecord(apiCall);
                return record != null && record.Ready && DateTime.UtcNow >= record.DueUtc;
            }
        }

        public static int GetAPICallFailureReason(ulong apiCall)
        {
            lock (Gate)
            {
                var record = FindRecord(apiCall);
                if (record == null)
                {
                    return (int)ESteamAPICallFailure.k_ESteamAPICallFailureInvalidHandle;
                }

                if (!record.Ready)
                {
                    return (int)ESteamAPICallFailure.k_ESteamAPICallFailureNone;
                }

                return (int)record.LastFailureReason;
            }
        }

        public static bool GetAPICallResult(ulong apiCall, IntPtr callback, int callbackSize, int callbackExpected, ref bool failed)
        {
            lock (Gate)
            {
                var record = FindRecord(apiCall);
                if (record == null || !record.Ready || DateTime.UtcNow < record.DueUtc)
                {
                    failed = false;
                    return false;
                }

                if (callbackExpected != 0 && callbackExpected != record.CallbackId)
                {
                    record.LastFailureReason = ESteamAPICallFailure.k_ESteamAPICallFailureMismatchedCallback;
                    Write($"GetAPICallResult mismatched callback handle={apiCall} expected={callbackExpected} actual={record.CallbackId}");
                    failed = false;
                    return false;
                }

                if (callback != IntPtr.Zero && callbackSize > 0)
                {
                    ZeroMemory(callback, callbackSize);
                    Marshal.Copy(record.Payload, 0, callback, Math.Min(record.Payload.Length, callbackSize));
                }

                failed = record.IOFailure;
                record.LastFailureReason = record.IOFailure
                    ? ESteamAPICallFailure.k_ESteamAPICallFailureNetworkFailure
                    : ESteamAPICallFailure.k_ESteamAPICallFailureNone;
                record.ToDelete = true;

                Write($"GetAPICallResult handle={apiCall} callback={record.CallbackId} requested={callbackSize} copied={Math.Min(record.Payload.Length, Math.Max(callbackSize, 0))} failed={failed}");
                return true;
            }
        }

        public static bool UnregisterCallResult(SteamCallback callback, ulong apiCall)
        {
            if (callback == null)
            {
                return false;
            }

            lock (Gate)
            {
                var record = FindRecord(apiCall);
                if (record == null)
                {
                    return false;
                }

                bool removed = RemoveByPointer(record.Callbacks, callback.Pointer);
                if (removed)
                {
                    callback.Unregister();
                }

                return removed;
            }
        }

        public static bool UnregisterCallback(SteamCallback callback)
        {
            if (callback == null)
            {
                return false;
            }

            lock (Gate)
            {
                bool removed = RemoveByPointer(CompletedCallbacks, callback.Pointer);

                foreach (var callbacks in RegisteredCallbacks.Values)
                {
                    removed |= RemoveByPointer(callbacks, callback.Pointer);
                }

                foreach (var record in Records)
                {
                    removed |= RemoveByPointer(record.Callbacks, callback.Pointer);
                }

                if (removed)
                {
                    callback.Unregister();
                }

                return removed;
            }
        }

        private static SteamAPICall_t AddCallResult(ICallbackData data, bool gameServer, bool readyToCall, bool ioFailure)
        {
            if (data == null)
            {
                return 0;
            }

            lock (Gate)
            {
                currentCall++;
                var now = DateTime.UtcNow;
                var record = new CallbackRecord
                {
                    ApiCall = currentCall,
                    CallbackId = (int)data.CallbackType,
                    Data = data,
                    Payload = Serialize(data),
                    GameServer = gameServer,
                    SteamPipe = gameServer ? SteamEmulator.HSteamPipe_GS : SteamEmulator.HSteamPipe,
                    SteamUser = gameServer ? SteamEmulator.HSteamUser_GS : SteamEmulator.HSteamUser,
                    CreatedUtc = now,
                    Ready = readyToCall,
                    DueUtc = readyToCall ? now + DefaultCallbackDelay : DateTime.MaxValue,
                    ReadyUtc = readyToCall ? now : DateTime.MaxValue,
                    RunCallCompletedCallback = true,
                    IOFailure = ioFailure,
                    LastFailureReason = ioFailure
                        ? ESteamAPICallFailure.k_ESteamAPICallFailureNetworkFailure
                        : ESteamAPICallFailure.k_ESteamAPICallFailureNone
                };

                Records.Add(record);
                Write($"Added CallbackResult {record.ApiCall} callback={record.CallbackId} size={record.Payload.Length} ready={readyToCall} gs={gameServer}");
                return record.ApiCall;
            }
        }

        private static void AddDirectCallback(ICallbackData data, bool gameServer, bool readyToCall)
        {
            if (data == null)
            {
                return;
            }

            lock (Gate)
            {
                var key = new CallbackKey((int)data.CallbackType, gameServer);
                var now = DateTime.UtcNow;
                var pending = new PendingDirectCallback
                {
                    CallbackId = key.CallbackId,
                    Data = data,
                    Payload = Serialize(data),
                    GameServer = gameServer,
                    SteamPipe = gameServer ? SteamEmulator.HSteamPipe_GS : SteamEmulator.HSteamPipe,
                    SteamUser = gameServer ? SteamEmulator.HSteamUser_GS : SteamEmulator.HSteamUser,
                    CreatedUtc = now,
                    DueUtc = now + (readyToCall ? DefaultCallbackDelay : TimeSpan.Zero)
                };

                if (!PendingDirectCallbacks.TryGetValue(key, out var pendingList))
                {
                    pendingList = new List<PendingDirectCallback>();
                    PendingDirectCallbacks[key] = pendingList;
                }

                pendingList.Add(pending);
                AddDirectRecord(key, pending, GetCallbackList(key).ToArray());
                Write($"Added Callback {pending.CallbackId} size={pending.Payload.Length} gs={gameServer} registered={GetCallbackList(key).Count}");
            }
        }

        private static List<CallbackInvocation> DrainForFrame(bool gameServer, HSteamPipe steamPipe, HSteamUser steamUser)
        {
            var now = DateTime.UtcNow;
            lock (Gate)
            {
                RemoveDeletedAndExpired(now);
                var invocations = DrainReadyRecords(gameServer, now);
                ClearPendingDirectCallbacks(gameServer);
                return invocations;
            }
        }

        private static void AddDirectRecord(CallbackKey key, PendingDirectCallback pending, IEnumerable<SteamCallback> callbacks)
        {
            var record = new CallbackRecord
            {
                ApiCall = 0,
                CallbackId = key.CallbackId,
                Data = pending.Data,
                Payload = pending.Payload,
                GameServer = key.GameServer,
                SteamPipe = pending.SteamPipe,
                SteamUser = pending.SteamUser,
                CreatedUtc = pending.CreatedUtc,
                Ready = true,
                DueUtc = pending.DueUtc,
                ReadyUtc = pending.CreatedUtc,
                RunCallCompletedCallback = false,
                IOFailure = false,
                LastFailureReason = ESteamAPICallFailure.k_ESteamAPICallFailureNone
            };

            record.Callbacks.AddRange(callbacks.Where(c => c.Pointer != IntPtr.Zero));
            Records.Add(record);
        }

        private static List<CallbackInvocation> DrainReadyRecords(bool gameServer, DateTime now)
        {
            var invocations = new List<CallbackInvocation>();
            foreach (var record in Records.Where(r => r.GameServer == gameServer && !r.ToDelete).ToList())
            {
                if (!CanExecute(record, now))
                {
                    continue;
                }

                if (record.RunCallCompletedCallback)
                {
                    DispatchCallResultRecord(record, invocations);
                }
                else
                {
                    DispatchDirectCallbackRecord(record, invocations);
                }

                record.ToDelete = true;
            }

            return invocations;
        }

        private static bool CanExecute(CallbackRecord record, DateTime now)
        {
            if (!record.Ready || now < record.DueUtc)
            {
                return false;
            }

            if (!record.RunCallCompletedCallback)
            {
                return true;
            }

            if (record.Callbacks.Count > 0)
            {
                return true;
            }

            return now - record.ReadyUtc >= WaitForCallResultCallback;
        }

        private static void DispatchCallResultRecord(CallbackRecord record, List<CallbackInvocation> invocations)
        {
            if (record.Data is SteamNetworkingSocketsCert_t cert)
            {
                Write($"Dispatch Cert CallResult {record.ApiCall} callback={record.CallbackId} size={record.Payload.Length} result={(int)cert.m_eResult} cert={cert.m_cbCert} sig={cert.m_cbSignature} key={cert.m_cbPrivKey} ca={cert.m_caKeyID:X16} callbacks={record.Callbacks.Count}");
            }
            else if (record.Data is HTTPRequestCompleted_t http)
            {
                Write($"Dispatch HTTP CallResult {record.ApiCall} callback={record.CallbackId} request={http.Request} success={http.RequestSuccessful} status={(int)http.StatusCode} body={http.BodySize} callbacks={record.Callbacks.Count}");
            }
            else
            {
                Write($"Dispatch CallResult {record.ApiCall} callback={record.CallbackId} size={record.Payload.Length} callbacks={record.Callbacks.Count}");
            }

            foreach (var callback in record.Callbacks.ToArray())
            {
                invocations.Add(CallbackInvocation.CallResult(callback, record.Payload, record.IOFailure, record.ApiCall));
            }

            if (record.CompletedNotificationSent)
            {
                return;
            }

            var completed = new SteamAPICallCompleted_t
            {
                m_hAsyncCall = record.ApiCall,
                m_iCallback = record.CallbackId,
                m_cubParam = (uint)record.Payload.Length
            };
            var completedBytes = Serialize(completed);
            QueueManualDispatch(record.SteamPipe, record.SteamUser, (int)completed.CallbackType, completedBytes);

            foreach (var callback in CompletedCallbacks.Where(c => c.HasGameserver == record.GameServer).ToArray())
            {
                invocations.Add(CallbackInvocation.Direct(callback, completedBytes));
            }

            record.CompletedNotificationSent = true;
        }

        private static void DispatchDirectCallbackRecord(CallbackRecord record, List<CallbackInvocation> invocations)
        {
            QueueManualDispatch(record.SteamPipe, record.SteamUser, record.CallbackId, record.Payload);
            foreach (var callback in record.Callbacks.ToArray())
            {
                invocations.Add(CallbackInvocation.Direct(callback, record.Payload));
            }
        }

        private static void InvokeCallbacks(List<CallbackInvocation> invocations)
        {
            foreach (var invocation in invocations)
            {
                IntPtr buffer = IntPtr.Zero;
                try
                {
                    buffer = Marshal.AllocHGlobal(invocation.Payload.Length);
                    if (invocation.Payload.Length > 0)
                    {
                        Marshal.Copy(invocation.Payload, 0, buffer, invocation.Payload.Length);
                    }

                    bool invoked = invocation.IsCallResult
                        ? invocation.Callback.Run(buffer, invocation.IOFailure, invocation.Handle)
                        : invocation.Callback.Run(buffer);

                    if (!invoked)
                    {
                        Write($"Callback invocation skipped callback={(int)invocation.Callback.CallbackType} ptr=0x{invocation.Callback.Pointer.ToInt64():X}");
                    }
                }
                catch (Exception ex)
                {
                    Write($"Callback invocation failed callback={(int)invocation.Callback.CallbackType} ptr=0x{invocation.Callback.Pointer.ToInt64():X}: {ex}");
                }
                finally
                {
                    if (buffer != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(buffer);
                    }
                }
            }
        }

        private static void QueueManualDispatch(HSteamPipe pipe, HSteamUser steamUser, int callbackId, byte[] payload)
        {
            if (!ManualDispatchQueues.TryGetValue(pipe, out var queue))
            {
                queue = new Queue<ManualDispatchItem>();
                ManualDispatchQueues[pipe] = queue;
            }

            queue.Enqueue(new ManualDispatchItem
            {
                SteamUser = steamUser,
                CallbackId = callbackId,
                Payload = payload.ToArray()
            });
        }

        private static void ClearPendingDirectCallbacks(bool gameServer)
        {
            foreach (var key in PendingDirectCallbacks.Keys.Where(k => k.GameServer == gameServer).ToArray())
            {
                PendingDirectCallbacks.Remove(key);
            }
        }

        private static void RemoveDeletedAndExpired(DateTime now)
        {
            Records.RemoveAll(r =>
                r.ToDelete ||
                now - r.CreatedUtc > CallResultLifetime);
        }

        private static List<SteamCallback> GetCallbackList(CallbackKey key)
        {
            if (!RegisteredCallbacks.TryGetValue(key, out var callbacks))
            {
                callbacks = new List<SteamCallback>();
                RegisteredCallbacks[key] = callbacks;
            }

            return callbacks;
        }

        private static CallbackRecord FindRecord(ulong apiCall)
        {
            return Records.LastOrDefault(r => r.ApiCall == apiCall);
        }

        private static void AddUnique(List<SteamCallback> callbacks, SteamCallback callback)
        {
            if (callbacks.All(c => c.Pointer != callback.Pointer))
            {
                callbacks.Add(callback);
            }
        }

        private static bool RemoveByPointer(List<SteamCallback> callbacks, IntPtr pointer)
        {
            return callbacks.RemoveAll(c => c.Pointer == pointer) > 0;
        }

        private static byte[] Serialize(ICallbackData data)
        {
            IntPtr buffer = IntPtr.Zero;
            try
            {
                buffer = Marshal.AllocHGlobal(data.DataSize);
                ZeroMemory(buffer, data.DataSize);
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

        private static void ZeroMemory(IntPtr ptr, int size)
        {
            if (ptr == IntPtr.Zero || size <= 0)
            {
                return;
            }

            var zero = new byte[Math.Min(size, 4096)];
            var remaining = size;
            var offset = 0;
            while (remaining > 0)
            {
                var count = Math.Min(remaining, zero.Length);
                Marshal.Copy(zero, 0, IntPtr.Add(ptr, offset), count);
                remaining -= count;
                offset += count;
            }
        }

        private static void ReplayGameServerConnectionStateIfNeeded(SteamCallback callback)
        {
            if (SteamEmulator.SteamGameServer == null || !SteamEmulator.SteamGameServer.LoggedIn)
            {
                return;
            }

            if (Interlocked.Exchange(ref gameServerConnectionReplayIssued, 1) != 0)
            {
                return;
            }

            ThreadPool.QueueUserWorkItem(_ =>
            {
                Thread.Sleep(250);
                AddCallbackGameServer(new SteamServersConnected_t());
                AddCallbackGameServer(new GSPolicyResponse_t
                {
                    Secure = SteamEmulator.SteamGameServer.BSecure() ? (byte)1 : (byte)0
                });
                Write($"Replayed GameServer connected state for late {callback.CallbackType} registration");
            });
        }

        private static void Write(string message)
        {
            SteamEmulator.Write("CallbackManager", message);
        }

        private struct CallbackKey : IEquatable<CallbackKey>
        {
            public readonly int CallbackId;
            public readonly bool GameServer;

            public CallbackKey(int callbackId, bool gameServer)
            {
                CallbackId = callbackId;
                GameServer = gameServer;
            }

            public bool Equals(CallbackKey other)
            {
                return CallbackId == other.CallbackId && GameServer == other.GameServer;
            }

            public override bool Equals(object obj)
            {
                return obj is CallbackKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (CallbackId * 397) ^ GameServer.GetHashCode();
                }
            }
        }

        private sealed class PendingDirectCallback
        {
            public int CallbackId;
            public ICallbackData Data;
            public byte[] Payload;
            public bool GameServer;
            public HSteamPipe SteamPipe;
            public HSteamUser SteamUser;
            public DateTime CreatedUtc;
            public DateTime DueUtc;
        }

        private sealed class CallbackRecord
        {
            public SteamAPICall_t ApiCall;
            public int CallbackId;
            public ICallbackData Data;
            public byte[] Payload;
            public bool GameServer;
            public HSteamPipe SteamPipe;
            public HSteamUser SteamUser;
            public DateTime CreatedUtc;
            public DateTime DueUtc;
            public DateTime ReadyUtc;
            public bool Ready;
            public bool ToDelete;
            public bool RunCallCompletedCallback;
            public bool CompletedNotificationSent;
            public bool IOFailure;
            public ESteamAPICallFailure LastFailureReason;
            public readonly List<SteamCallback> Callbacks = new List<SteamCallback>();
        }

        private sealed class CallbackInvocation
        {
            public SteamCallback Callback;
            public byte[] Payload;
            public bool IsCallResult;
            public bool IOFailure;
            public SteamAPICall_t Handle;

            public static CallbackInvocation Direct(SteamCallback callback, byte[] payload)
            {
                return new CallbackInvocation
                {
                    Callback = callback,
                    Payload = payload,
                    IsCallResult = false
                };
            }

            public static CallbackInvocation CallResult(SteamCallback callback, byte[] payload, bool ioFailure, SteamAPICall_t handle)
            {
                return new CallbackInvocation
                {
                    Callback = callback,
                    Payload = payload,
                    IsCallResult = true,
                    IOFailure = ioFailure,
                    Handle = handle
                };
            }
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

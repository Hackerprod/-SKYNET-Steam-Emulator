using SKYNET.Callback;
using SKYNET.Steamworks;
using System;
using System.Collections.Concurrent;
using System.Threading;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Managers
{
    /// <summary>
    /// Central background queue for server/API work that must not block Dota's
    /// Steamworks caller thread. Keep game-facing state changes synchronous and
    /// put only HTTP, disk-heavy sync, and slow retries here.
    /// </summary>
    public static class WorkQueue
    {
        private const int NormalWorkerCount = 3;
        private const int HighPriorityWorkerCount = 2;
        private const int MaxQueuedItems = 4096;

        private static readonly ConcurrentQueue<WorkItem> HighPriority = new ConcurrentQueue<WorkItem>();
        private static readonly ConcurrentQueue<WorkItem> NormalPriority = new ConcurrentQueue<WorkItem>();
        private static readonly ConcurrentDictionary<string, byte> CoalescedKeys = new ConcurrentDictionary<string, byte>(StringComparer.Ordinal);
        private static readonly SemaphoreSlim HighPrioritySignal = new SemaphoreSlim(0);
        private static readonly SemaphoreSlim NormalPrioritySignal = new SemaphoreSlim(0);

        private static int started;
        private static int queuedCount;
        private static long nextItemId;

        public static bool Enqueue(string name, Action work, string coalesceKey = null, bool highPriority = false)
        {
            if (work == null)
            {
                return false;
            }

            EnsureStarted();

            if (!string.IsNullOrEmpty(coalesceKey) && !CoalescedKeys.TryAdd(coalesceKey, 0))
            {
                return true;
            }

            var nextCount = Interlocked.Increment(ref queuedCount);
            if (nextCount > MaxQueuedItems && !highPriority)
            {
                Interlocked.Decrement(ref queuedCount);
                if (!string.IsNullOrEmpty(coalesceKey))
                {
                    CoalescedKeys.TryRemove(coalesceKey, out _);
                }

                SteamEmulator.Write("WorkQueue", $"Dropped work item because queue is saturated (Name = {name}, Queue = {nextCount})");
                return false;
            }

            var item = new WorkItem
            {
                Id = Interlocked.Increment(ref nextItemId),
                Name = string.IsNullOrWhiteSpace(name) ? "work" : name,
                Work = work,
                CoalesceKey = coalesceKey
            };

            if (highPriority)
            {
                HighPriority.Enqueue(item);
                HighPrioritySignal.Release();
            }
            else
            {
                NormalPriority.Enqueue(item);
                NormalPrioritySignal.Release();
            }

            return true;
        }

        public static SteamAPICall_t EnqueueCallbackResult(
            ICallbackData pendingResult,
            Func<ICallbackData> work,
            bool gameServer = false,
            string name = null,
            string coalesceKey = null,
            bool highPriority = true)
        {
            var handle = gameServer
                ? CallbackManager.AddCallbackResultGameServer(pendingResult, false)
                : CallbackManager.AddCallbackResult(pendingResult, false);

            var queued = Enqueue(name, () =>
            {
                ICallbackData result = pendingResult;
                var failed = false;
                try
                {
                    result = work?.Invoke() ?? pendingResult;
                }
                catch (Exception ex)
                {
                    failed = true;
                    SteamEmulator.Write("WorkQueue", $"{name ?? "callback work"} failed: {ex.Message}");
                }

                CallbackManager.CompleteCallbackResult(handle, result, failed);
            }, coalesceKey, highPriority);

            if (!queued)
            {
                CallbackManager.CompleteCallbackResult(handle, pendingResult, true);
            }

            return handle;
        }

        private static void EnsureStarted()
        {
            if (Interlocked.Exchange(ref started, 1) == 1)
            {
                return;
            }

            for (var i = 0; i < HighPriorityWorkerCount; i++)
            {
                var thread = new Thread(HighPriorityWorkerLoop)
                {
                    IsBackground = true,
                    Name = "SKYNET WorkQueue High " + (i + 1)
                };
                thread.Start();
            }

            for (var i = 0; i < NormalWorkerCount; i++)
            {
                var thread = new Thread(WorkerLoop)
                {
                    IsBackground = true,
                    Name = "SKYNET WorkQueue " + (i + 1)
                };
                thread.Start();
            }
        }

        private static void HighPriorityWorkerLoop()
        {
            while (true)
            {
                HighPrioritySignal.Wait();

                while (HighPriority.TryDequeue(out var item))
                {
                    RunItem(item);
                }
            }
        }

        private static void WorkerLoop()
        {
            while (true)
            {
                NormalPrioritySignal.Wait();

                while (NormalPriority.TryDequeue(out var item))
                {
                    RunItem(item);
                }
            }
        }

        private static void RunItem(WorkItem item)
        {
            try
            {
                item.Work();
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("WorkQueue", $"{item.Name} failed: {ex.Message}");
            }
            finally
            {
                Interlocked.Decrement(ref queuedCount);
                if (!string.IsNullOrEmpty(item.CoalesceKey))
                {
                    CoalescedKeys.TryRemove(item.CoalesceKey, out _);
                }
            }
        }

        private sealed class WorkItem
        {
            public long Id;
            public string Name;
            public string CoalesceKey;
            public Action Work;
        }
    }
}

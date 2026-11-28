using SKYNET.Helpers;
using SKYNET.Steamworks;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace SKYNET.Managers
{
    // Friend dialogs (add / accept / ignore / remove) executed without the profile overlay.
    //
    // GLOBAL LOCK ORDER (acquire left to right only):
    //   SnapshotGate -> SyncRoot
    //   SnapshotGate -> MirrorLock -> "Users"
    // SyncRoot and MirrorLock (both StateCache) are never nested; startLock/rtLock are leaves and never held together
    // with another lock. R1: none of {SnapshotGate, SyncRoot, MirrorLock, startLock} may be taken while holding "Users"
    // or inside any SteamFriends getter (Dota's thread runs those under "Users"): getters only use lock-free reads
    // (_snap, Interlocked, Volatile) and thread starts. R3: callbacks and the UserManager mirror run only after
    // SyncRoot is released. R5: only the executor thread sweeps the pending watchdog.

    internal enum FriendKind { Add, Remove, Accept, Ignore }

    internal enum Outcome { Ok, OkAlreadyGone, Failed, Skipped }

    internal sealed class FriendItem
    {
        public readonly ulong Id;
        public readonly FriendKind Kind;
        public readonly int Desired;   // relationship the optimistic update set (0 for unoptimistic items)
        public readonly int Epoch;     // 0 = unoptimistic: no Pending entry behind it
        public readonly int Gen;       // identity generation at press time
        public int Resolved;           // Interlocked: ResolveAction runs once per item

        public FriendItem(ulong id, FriendKind kind, int desired, int epoch, int gen)
        {
            Id = id;
            Kind = kind;
            Desired = desired;
            Epoch = epoch;
            Gen = gen;
        }
    }

    internal static class FriendActions
    {
        // ini [Log Settings] FriendsTrace (default true) and [Network Settings] FriendNotifyExtra (default false); read once at startup.
        // Bool ini values must be written as true / false (Settings.GetBool uses bool.TryParse; 0/1 fall back to the default).
        internal static bool Trace = true;
        internal static bool NotifyExtra;
        // ini [Network Settings] FriendOutgoingAsFriend (default true; write true / false): GetFriendRelationship reports 3 for a real 4.
        internal static bool OutgoingAsFriend = true;

        internal static void Fr(string message)
        {
            if (Trace)
            {
                SteamEmulator.Write("Friends", "[FR] " + message);
            }
        }

        internal static string DialogName(FriendKind kind)
        {
            switch (kind)
            {
                case FriendKind.Add: return "friendadd";
                case FriendKind.Accept: return "friendrequestaccept";
                case FriendKind.Ignore: return "friendrequestignore";
                default: return "friendremove";
            }
        }

        // Repeats the game-server / dedicated-role / disabled guards of EventPump.RunFrame inside the helper, so they
        // hold for every caller (the handshake worker runs in every role): Started is only set past those guards.
        internal static bool RoleAllowsThreads()
        {
            return APIClient.IsEnabled && EventPump.HasStarted;
        }

        internal static void Init()
        {
            if (RoleAllowsThreads())
            {
                FriendActionExecutor.EnsureAlive();
                FriendResync.EnsureAlive();
            }
        }

        // Dota's thread (ActivateGameOverlayToUser), no "Users" held. Never throws into the exported call.
        internal static void Run(FriendKind kind, ulong id)
        {
            try
            {
                if (id == 0 || id == (ulong)SteamEmulator.SteamID)
                {
                    Fr($"dialog={DialogName(kind)} id={id} ignored (invalid target)");
                    return;
                }

                // Before any cache change (R4: startLock is taken and released here, never inside SyncRoot).
                if (!FriendActionExecutor.EnsureAlive())
                {
                    Fr("executor unavailable");
                    return;
                }

                var begin = StateCache.BeginLocal(id, kind);
                StateCache.Emit(begin.Diffs);   // cache first, callback second, outside the locks

                if (begin.Item == null)
                {
                    if (begin.NeedResync)
                    {
                        FriendResync.RequestResync(StateCache.NextStamp());   // redundant add: the cache may be stale
                    }

                    return;
                }

                PreWarmProfile(id);

                try
                {
                    FriendActionExecutor.Enqueue(begin.Item);
                }
                catch (Exception ex)
                {
                    Fr("enqueue failed: " + ex.Message);
                    StateCache.ResolveAction(begin.Item, Outcome.Failed);
                }
            }
            catch (Exception ex)
            {
                Fr("Run failed: " + ex);
            }
        }

        // Lock-free (R1): the placeholder entry has no name yet, so fetch the profile now.
        private static void PreWarmProfile(ulong id)
        {
            if (StateCache.TryGetFriend(id, out var known) && !string.IsNullOrWhiteSpace(known.PersonaName))
            {
                return;
            }

            APIClient.QueueUserProfileRefresh(id, false, true);
        }
    }

    // One FIFO thread for every friend POST: strict ordering per id without keys or coalescing.
    internal static class FriendActionExecutor
    {
        private static readonly BlockingCollection<FriendItem> Queue = new BlockingCollection<FriendItem>();
        private static Thread t;
        private static readonly object startLock = new object();

        public static bool EnsureAlive()
        {
            if (!FriendActions.RoleAllowsThreads())
            {
                return false;
            }

            var th = Volatile.Read(ref t);
            if (th != null && th.IsAlive)
            {
                return true;   // lock-free fast path
            }

            lock (startLock)
            {
                if (t == null || !t.IsAlive)
                {
                    var restart = t != null;
                    t = new Thread(Loop) { IsBackground = true, Name = "SKYNET friend actions" };
                    t.Start();
                    if (restart)
                    {
                        FriendActions.Fr("executor restarted");
                    }
                }
            }

            return true;
        }

        public static void Enqueue(FriendItem item)
        {
            Queue.Add(item);
        }

        private static void Loop()
        {
            while (true)
            {
                // Outer loop: the thread never ends because of an exception.
                try
                {
                    while (true)
                    {
                        FriendItem item = null;
                        var outcome = Outcome.Failed;
                        try
                        {
                            if (!Queue.TryTake(out item, 5000))
                            {
                                StateCache.SweepPending();   // the only watchdog hook
                                continue;
                            }

                            // Old identity, or an epoch the watchdog already force-resolved: no POST.
                            outcome = item.Gen != StateCache.IdentityGen || StateCache.IsItemStale(item)
                                ? Outcome.Skipped
                                : RunPost(item);
                        }
                        catch (Exception ex)
                        {
                            FriendActions.Fr("executor item failed: " + ex.Message);
                            outcome = Outcome.Failed;
                        }
                        finally
                        {
                            try
                            {
                                if (item != null)
                                {
                                    StateCache.ResolveAction(item, outcome);
                                }
                            }
                            catch (Exception ex)
                            {
                                FriendActions.Fr("resolve failed: " + ex.Message);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    FriendActions.Fr("executor loop error: " + ex.Message);
                    Thread.Sleep(100);
                }
            }
        }

        private static Outcome RunPost(FriendItem item)
        {
            if (!APIClient.EnsureSession())
            {
                return Outcome.Failed;
            }

            if (item.Gen != StateCache.IdentityGen)
            {
                return Outcome.Skipped;
            }

            return APIClient.PostFriendAction(item.Kind, item.Id);
        }
    }

    // Re-reads the server state after local actions. Never coalesces with a fetch that started earlier: every attempt
    // waits for the snapshot gate and issues its own request with a fresh ticket (stamp greater than the request).
    internal static class FriendResync
    {
        private static long _needAfter;   // Interlocked/Volatile only
        private static readonly SemaphoreSlim Signal = new SemaphoreSlim(0);
        private static Thread rt;
        private static readonly object rtLock = new object();

        public static void EnsureAlive()
        {
            if (!FriendActions.RoleAllowsThreads())
            {
                return;
            }

            var th = Volatile.Read(ref rt);
            if (th != null && th.IsAlive)
            {
                return;
            }

            lock (rtLock)
            {
                if (rt == null || !rt.IsAlive)
                {
                    var restart = rt != null;
                    rt = new Thread(Loop) { IsBackground = true, Name = "SKYNET friend resync" };
                    rt.Start();
                    if (restart)
                    {
                        FriendActions.Fr("resync worker restarted");
                    }
                }
            }
        }

        // Callable from any thread: the CAS max and the semaphore release are leaves, the restart check is lock-free.
        public static void RequestResync(long afterStamp)
        {
            long cur;
            while ((cur = Volatile.Read(ref _needAfter)) < afterStamp &&
                   Interlocked.CompareExchange(ref _needAfter, afterStamp, cur) != cur)
            {
            }

            FriendActions.Fr("resync request");
            EnsureAlive();
            if (Signal.CurrentCount == 0)
            {
                Signal.Release();
            }
        }

        // Identity reset. Accepted race: a request already issued for the NEW identity can be wiped by this call; the
        // only cost is one delayed resync (the next action, the periodic refresh or ForceNextRefresh recovers it).
        public static void Reset()
        {
            Volatile.Write(ref _needAfter, 0);
        }

        private static void Loop()
        {
            while (true)
            {
                try
                {
                    Signal.Wait();
                    Thread.Sleep(300);   // debounce bursts
                    var target = Volatile.Read(ref _needAfter);
                    if (target == 0)
                    {
                        continue;
                    }

                    var ok = false;
                    var attempt = 0;
                    foreach (var delayMs in new[] { 0, 3000 })
                    {
                        if (delayMs > 0)
                        {
                            Thread.Sleep(delayMs);
                        }

                        attempt++;
                        ok = APIClient.FetchSnapshot(APIClient.SnapshotKind.Forced);
                        FriendActions.Fr($"resync attempt={attempt} result={(ok ? "ok" : "fail")}");
                        if (ok)
                        {
                            break;
                        }
                    }

                    if (!ok)
                    {
                        // Ids whose actions failed were restored at resolution and successful ones keep the server-accepted
                        // value; the next game-triggered refresh runs regardless of the 15 s window.
                        StateCache.ForceNextRefresh = true;
                        FriendActions.Fr("resync result=giveup");
                    }
                    else if (Volatile.Read(ref _needAfter) != target)
                    {
                        Signal.Release();
                    }
                }
                catch (Exception ex)
                {
                    FriendActions.Fr("resync loop error: " + ex.Message);
                }
            }
        }
    }
}

using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using SKYNET.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace SKYNET.Managers
{
    /// <summary>One id whose persona/relationship changed; <see cref="Flags"/> is an EPersonaChange mask.</summary>
    internal readonly struct PersonaDiff
    {
        public readonly ulong Id;
        public readonly int Flags;
        // Only the server's avatar flag (a web avatar upload) asks for a re-download. A new cache entry also carries
        // 0x40 so Dota loads the image through GetSmallFriendAvatar, which RequestAvatar already guards.
        public readonly bool RefreshAvatar;

        public PersonaDiff(ulong id, int flags, bool refreshAvatar = false)
        {
            Id = id;
            Flags = flags;
            RefreshAvatar = refreshAvatar;
        }
    }

    internal sealed class MutateResult
    {
        public bool Changed;
        public readonly List<PersonaDiff> Diffs = new List<PersonaDiff>();
    }

    internal struct FetchTicket
    {
        public long Stamp;
        public long StartMs;
        public int IdentityGen;
    }

    internal sealed class BeginResult
    {
        public FriendItem Item;          // null when nothing has to be sent
        public bool NeedResync;          // the press was redundant: only re-read the server state
        public List<PersonaDiff> Diffs = new List<PersonaDiff>();
    }

    public static class StateCache
    {
        // Lock order (see FriendActions.cs): SnapshotGate -> SyncRoot, SnapshotGate -> MirrorLock -> "Users".
        // SyncRoot and MirrorLock are never nested; nothing that holds "Users" may take either.
        private static readonly object SyncRoot = new object();
        private static readonly object MirrorLock = new object();
        private static readonly ConcurrentDictionary<ulong, List<PlayerStat>> PlayerStats = new ConcurrentDictionary<ulong, List<PlayerStat>>();
        private static readonly ConcurrentDictionary<ulong, List<Achievement>> PlayerAchievements = new ConcurrentDictionary<ulong, List<Achievement>>();
        private static readonly ConcurrentDictionary<ulong, DateTime> StatsRefreshedAt = new ConcurrentDictionary<ulong, DateTime>();

        private static SteamPlayer Self;
        private static DateTime LastSelfRefresh = DateTime.MinValue;
        private static bool SelfStatsDirty;
        private static bool SelfAchievementsDirty;
        private static string EventCursor;
        private static int CurrentPlayers;
        private static int SelfPlayerLevel;

        #region Snapshot

        // The friend/persona cache is an immutable snapshot swapped atomically, so readers never see a half-built list
        // (the old Clear()-then-refill made relationships flap through None while Dota was reading them).
        private sealed class Snapshot
        {
            public readonly Dictionary<ulong, SteamPlayer> ById;
            public readonly SteamPlayer[] Sorted;
            public readonly long Gen;

            private Snapshot(Dictionary<ulong, SteamPlayer> byId, SteamPlayer[] sorted, long gen)
            {
                ById = byId;
                Sorted = sorted;
                Gen = gen;
            }

            public static Snapshot From(Dictionary<ulong, SteamPlayer> byId, long gen)
            {
                var sorted = byId.Values
                    .OrderBy(p => p.PersonaName ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(p => p.SteamID)
                    .ToArray();
                return new Snapshot(byId, sorted, gen);
            }

            // Always a NEW instance with the generation the caller just took: Mirror() skips generations it has already
            // mirrored, so a shared "empty" instance would leave the previous identity's friends in the "Users" mirror.
            public static Snapshot CreateEmpty(long gen)
            {
                return new Snapshot(new Dictionary<ulong, SteamPlayer>(), new SteamPlayer[0], gen);
            }
        }

        private static volatile Snapshot _snap = Snapshot.CreateEmpty(0);
        private static long _snapGen;       // Interlocked
        private static long _mirroredGen;   // guarded by MirrorLock

        // The ONLY way to change the cache. `body` runs inside SyncRoot on a copy of the dictionary; entries are never
        // modified in place, only replaced. Changed = the body added/removed/replaced anything (even fields that produce
        // no callback flag); Diffs = ids with a non-zero callback mask. The caller emits the diffs after we return.
        private static List<PersonaDiff> Mutate(Action<Dictionary<ulong, SteamPlayer>, MutateResult> body)
        {
            var res = new MutateResult();
            Snapshot pub = null;
            lock (SyncRoot)
            {
                var copy = new Dictionary<ulong, SteamPlayer>(_snap.ById);
                body(copy, res);
                if (res.Changed)
                {
                    pub = Snapshot.From(copy, Interlocked.Increment(ref _snapGen));
                    _snap = pub;
                }
            }

            // After SyncRoot is released: the mirror takes the "Users" gate, which Dota's thread holds in the getters.
            if (pub != null)
            {
                Mirror(pub);
            }

            return res.Diffs;
        }

        private static void Mirror(Snapshot s)
        {
            lock (MirrorLock)
            {
                if (s.Gen <= _mirroredGen)
                {
                    return;
                }

                _mirroredGen = s.Gen;
                UserManager.ReplaceUsers(s.Sorted);
            }
        }

        // Callbacks go out after the snapshot is published, outside every lock (AddCallback only takes its own gate).
        internal static void Emit(List<PersonaDiff> diffs)
        {
            if (diffs == null)
            {
                return;
            }

            foreach (var diff in diffs)
            {
                if (diff.Flags == 0)
                {
                    continue;
                }

                var flags = diff.Flags;
                if (FriendActions.NotifyExtra && (flags & (int)EPersonaChange.k_EPersonaChangeRelationshipChanged) != 0)
                {
                    flags |= (int)EPersonaChange.k_EPersonaChangeName | (int)EPersonaChange.k_EPersonaChangeStatus;
                }

                FriendActions.Fr($"notify id={diff.Id} flags=0x{flags:X}");
                CallbackManager.AddCallback(new PersonaStateChange_t
                {
                    m_ulSteamID = diff.Id,
                    m_nChangeFlags = flags
                });

                if (diff.RefreshAvatar)
                {
                    var steamId = diff.Id;
                    ThreadPool.QueueUserWorkItem(_ => APIClient.RefreshAvatar(steamId));
                }
            }
        }

        // Readers never lock: one read of _snap gives a consistent view.
        public static bool TryGetFriend(ulong steamId, out SteamPlayer friend)
        {
            friend = null;

            if (_snap.ById.TryGetValue(steamId, out var cached))
            {
                friend = ClonePlayer(cached);
            }

            return friend != null;
        }

        public static List<SteamPlayer> GetFriends()
        {
            return _snap.Sorted.Select(ClonePlayer).ToList();
        }

        #endregion

        #region Clock, stamps, refresh timing

        // Single monotonic time source (net472 has no 64-bit tick counter). Double math avoids ticks * 1000 overflow.
        internal static long NowMs()
        {
            return (long)(Stopwatch.GetTimestamp() * (1000.0 / Stopwatch.Frequency));
        }

        // 64-bit shared fields are plain longs touched only through Interlocked/Volatile (atomic on 32-bit processes too).
        private static long _clock;
        private static long _lastFriendsRefreshMs;
        private static long _lastRefreshAttemptMs;
        private static int _identityGen;
        private static int _epochSeq;
        internal static volatile bool ForceNextRefresh;

        internal static int IdentityGen => Volatile.Read(ref _identityGen);

        internal static long NextStamp()
        {
            return Interlocked.Increment(ref _clock);
        }

        private struct TouchInfo
        {
            public long Stamp;
            public long Ms;
        }

        // Last local change per id. A fetch that started before the touch must not overwrite that id.
        private static readonly ConcurrentDictionary<ulong, TouchInfo> Touched = new ConcurrentDictionary<ulong, TouchInfo>();
        private const long MaxFetchAgeMs = 45000;
        private const long TouchTtlMs = 60000;

        private static void Touch(ulong id)
        {
            Touched[id] = new TouchInfo { Stamp = NextStamp(), Ms = NowMs() };
            if (Touched.Count > 256)
            {
                PruneTouched();
            }
        }

        private static void PruneTouched()
        {
            var now = NowMs();
            foreach (var kv in Touched)
            {
                if (now - kv.Value.Ms > TouchTtlMs)
                {
                    Touched.TryRemove(kv.Key, out _);
                }
            }
        }

        // Taken BEFORE the HTTP request: everything applied before this ticket was enqueued server-side before the
        // request, so the response reflects it; anything applied after has a larger stamp and wins over this fetch.
        internal static FetchTicket BeginFetch()
        {
            return new FetchTicket { Stamp = NextStamp(), StartMs = NowMs(), IdentityGen = IdentityGen };
        }

        // Both run inside SyncRoot at apply time.
        private static bool Applicable(FetchTicket t)
        {
            return t.IdentityGen == IdentityGen && NowMs() - t.StartMs <= MaxFetchAgeMs;
        }

        private static bool ShouldSkip(ulong id, FetchTicket t)
        {
            return Pending.ContainsKey(id) || (Touched.TryGetValue(id, out var x) && x.Stamp > t.Stamp);
        }

        internal static void NoteRefreshAttempt()
        {
            Volatile.Write(ref _lastRefreshAttemptMs, NowMs());
        }

        public static bool NeedsSelfRefresh(TimeSpan maxAge)
        {
            return Self == null || DateTime.UtcNow - LastSelfRefresh > maxAge;
        }

        // Lock-free: runs on Dota's thread, possibly inside the "Users" gate.
        public static bool NeedsFriendsRefresh(TimeSpan maxAge)
        {
            var now = NowMs();
            if (now - Volatile.Read(ref _lastRefreshAttemptMs) < 5000)
            {
                return false;   // back-off for every attempt kind: GetFriendCount is polled constantly
            }

            return ForceNextRefresh || now - Volatile.Read(ref _lastFriendsRefreshMs) > (long)maxAge.TotalMilliseconds;
        }

        public static bool NeedsStatsRefresh(ulong steamId, TimeSpan maxAge)
        {
            if (!StatsRefreshedAt.TryGetValue(steamId, out var refreshedAt))
            {
                return true;
            }

            return DateTime.UtcNow - refreshedAt > maxAge;
        }

        #endregion

        #region Identity

        public static void ResetForIdentityChange()
        {
            Snapshot empty;
            lock (SyncRoot)
            {
                Self = null;
                empty = Snapshot.CreateEmpty(Interlocked.Increment(ref _snapGen));
                _snap = empty;
                Pending.Clear();
                Touched.Clear();
                // Neutralises in-flight fetches, in-flight PollEvents cursor writes and queued POSTs (items carry the gen).
                Interlocked.Increment(ref _identityGen);
                LastSelfRefresh = DateTime.MinValue;
                Volatile.Write(ref _lastFriendsRefreshMs, 0);
                Volatile.Write(ref _lastRefreshAttemptMs, 0);
                ForceNextRefresh = false;
                SelfStatsDirty = false;
                SelfAchievementsDirty = false;
                EventCursor = string.Empty;
                CurrentPlayers = 0;
                SelfPlayerLevel = 0;
            }

            Mirror(empty);
            FriendResync.Reset();
            FriendActions.Fr("identity reset");
        }

        public static void ApplySelf(APIClient.ApiUser user)
        {
            if (user == null)
            {
                return;
            }

            lock (SyncRoot)
            {
                Self = MapUser(user);
                LastSelfRefresh = DateTime.UtcNow;
                SelfPlayerLevel = user.PlayerLevel;

                if (user.AccountId != 0)
                {
                    SteamEmulator.SteamID = new CSteamID(user.AccountId, EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeIndividual);
                }
                else if (user.SteamId != 0)
                {
                    SteamEmulator.SteamID = new CSteamID(user.SteamId);
                }

                if (!string.IsNullOrWhiteSpace(user.PersonaName))
                {
                    SteamEmulator.PersonaName = user.PersonaName;
                }
            }
        }

        #endregion

        #region Server snapshots and profiles

        private static int RelOf(SteamPlayer p)
        {
            if (p == null)
            {
                return 0;
            }

            return p.FriendRelationship != 0
                ? p.FriendRelationship
                : (p.HasFriend ? (int)EFriendRelationship.k_EFriendRelationshipFriend : 0);
        }

        private static bool IsSocial(int rel)
        {
            return rel == (int)EFriendRelationship.k_EFriendRelationshipRequestRecipient ||
                   rel == (int)EFriendRelationship.k_EFriendRelationshipFriend ||
                   rel == (int)EFriendRelationship.k_EFriendRelationshipRequestInitiator;
        }

        private static string Ids(IEnumerable<ulong> ids)
        {
            var list = ids.ToList();
            return "[" + string.Join(",", list.Take(20)) + (list.Count > 20 ? ",+" + (list.Count - 20) : "") + "]";
        }

        // Full replacement of the server-known social set (confirmed friends + pending requests in both directions).
        // Returns null when the response was discarded (identity changed or the fetch took too long).
        internal static List<PersonaDiff> ApplyFriends(List<APIClient.ApiUser> payload, FetchTicket t, string kind)
        {
            if (payload == null)
            {
                return null;
            }

            var applicable = false;
            var upsert = 0;
            var demote = 0;
            var skippedTouched = new List<ulong>();
            var skippedPending = new List<ulong>();

            var diffs = Mutate((copy, r) =>
            {
                if (!Applicable(t))
                {
                    return;
                }

                applicable = true;
                var seen = new HashSet<ulong>();
                var self = (ulong)SteamEmulator.SteamID;

                foreach (var user in payload)
                {
                    var mapped = MapUser(user);
                    if (mapped.SteamID == self || !IsSocial(RelOf(mapped)))
                    {
                        continue;
                    }

                    seen.Add(mapped.SteamID);
                    if (Pending.ContainsKey(mapped.SteamID))
                    {
                        skippedPending.Add(mapped.SteamID);
                        continue;
                    }

                    if (ShouldSkip(mapped.SteamID, t))
                    {
                        skippedTouched.Add(mapped.SteamID);
                        continue;
                    }

                    if (Put(copy, r, mapped))
                    {
                        upsert++;
                        Touch(mapped.SteamID);
                    }
                }

                // Entries the server no longer lists as a friend/request are demoted (persona kept), never dropped.
                foreach (var entry in copy.Values.Where(p => IsSocial(RelOf(p)) && !seen.Contains(p.SteamID)).ToList())
                {
                    if (Pending.ContainsKey(entry.SteamID))
                    {
                        skippedPending.Add(entry.SteamID);
                        continue;
                    }

                    if (ShouldSkip(entry.SteamID, t))
                    {
                        skippedTouched.Add(entry.SteamID);
                        continue;
                    }

                    Put(copy, r, WithRelationship(entry, entry.SteamID, 0));
                    demote++;
                    Touch(entry.SteamID);
                }

                PruneTouched();
                TrimNoneEntries(copy, r);
            });

            if (!applicable)
            {
                FriendActions.Fr(t.IdentityGen != IdentityGen ? "identity-mismatch" : "fetch-too-old");
                return null;
            }

            Volatile.Write(ref _lastFriendsRefreshMs, NowMs());
            ForceNextRefresh = false;

            if (upsert > 0 || demote > 0 || skippedTouched.Count > 0 || skippedPending.Count > 0)
            {
                FriendActions.Fr($"snapshot kind={kind} start={t.Stamp} n={payload.Count} upsert={upsert} demote={demote} " +
                                 $"skippedTouched={Ids(skippedTouched)} skippedPending={Ids(skippedPending)} " +
                                 $"diff=[{string.Join(",", diffs.Take(20).Select(d => d.Id + ":0x" + d.Flags.ToString("X")))}]");
            }

            return diffs;
        }

        // One user fetched on demand (api/users/<id>); same arbitration as a snapshot. Null when discarded.
        internal static List<PersonaDiff> ApplyProfile(APIClient.ApiUser user, FetchTicket t)
        {
            if (user == null)
            {
                return null;
            }

            var applicable = false;
            var result = "applied";
            var mapped = MapUser(user);
            var diffs = Mutate((copy, r) =>
            {
                if (!Applicable(t))
                {
                    return;
                }

                applicable = true;
                if (mapped.SteamID == 0 || mapped.SteamID == (ulong)SteamEmulator.SteamID)
                {
                    result = "noop";
                    return;
                }

                if (Pending.ContainsKey(mapped.SteamID))
                {
                    result = "skipped-pending";
                    return;
                }

                if (ShouldSkip(mapped.SteamID, t))
                {
                    result = "skipped-touched";
                    return;
                }

                if (Put(copy, r, mapped))
                {
                    Touch(mapped.SteamID);
                }

                TrimNoneEntries(copy, r);
            });

            if (!applicable)
            {
                FriendActions.Fr("profile id=" + mapped.SteamID + " result=" + (t.IdentityGen != IdentityGen ? "identity-mismatch" : "fetch-too-old"));
                return null;
            }

            if (result != "applied" || diffs.Count > 0)
            {
                FriendActions.Fr($"profile id={mapped.SteamID} result={result} diff=0x{(diffs.Count > 0 ? diffs[0].Flags : 0):X}");
            }

            return diffs;
        }

        private const int MaxNoneEntries = 2000;
        private const int KeepNoneEntries = 1500;

        // None entries (removed friends, looked-up strangers) are kept for their names; bound their growth by evicting
        // the least recently touched ones. Never evicts a non-None relationship or an id with a Pending action.
        // Runs inside SyncRoot (the Touched/Pending reads are lock-free or SyncRoot-guarded).
        private static void TrimNoneEntries(Dictionary<ulong, SteamPlayer> copy, MutateResult r)
        {
            var none = copy.Values.Where(p => RelOf(p) == 0 && !Pending.ContainsKey(p.SteamID)).ToList();
            if (none.Count <= MaxNoneEntries)
            {
                return;
            }

            foreach (var p in none
                .OrderBy(p => Touched.TryGetValue(p.SteamID, out var x) ? x.Stamp : 0)
                .Take(none.Count - KeepNoneEntries))
            {
                copy.Remove(p.SteamID);
            }

            r.Changed = true;
        }

        public static void ClearFriends()
        {
            Mutate((copy, r) =>
            {
                if (copy.Count > 0)
                {
                    copy.Clear();
                    r.Changed = true;
                }
            });
            Volatile.Write(ref _lastFriendsRefreshMs, NowMs());
        }

        #endregion

        #region Diffs

        // Adds the entry (or replaces it) and records the callback mask; false when nothing changed.
        private static bool Put(Dictionary<ulong, SteamPlayer> copy, MutateResult r, SteamPlayer np)
        {
            copy.TryGetValue(np.SteamID, out var old);
            if (old != null && SamePlayer(old, np))
            {
                return false;
            }

            copy[np.SteamID] = np;
            r.Changed = true;
            AddDiff(r, np.SteamID, DiffFlags(old, np));
            return true;
        }

        private static void AddDiff(MutateResult r, ulong id, int flags)
        {
            if (flags == 0)
            {
                return;
            }

            for (var i = 0; i < r.Diffs.Count; i++)
            {
                if (r.Diffs[i].Id == id)
                {
                    r.Diffs[i] = new PersonaDiff(id, r.Diffs[i].Flags | flags, r.Diffs[i].RefreshAvatar);
                    return;
                }
            }

            r.Diffs.Add(new PersonaDiff(id, flags));
        }

        // One mask per id per apply (see design section 6).
        private static int DiffFlags(SteamPlayer o, SteamPlayer n)
        {
            var f = 0;
            if (RelOf(o) != RelOf(n))
            {
                f |= (int)EPersonaChange.k_EPersonaChangeRelationshipChanged;
            }

            if (!string.Equals(o?.PersonaName ?? string.Empty, n.PersonaName ?? string.Empty, StringComparison.Ordinal))
            {
                f |= (int)EPersonaChange.k_EPersonaChangeName;
            }

            var oldState = o?.PersonaState ?? 0;
            if (oldState != n.PersonaState)
            {
                f |= (int)EPersonaChange.k_EPersonaChangeStatus;
                if (oldState == 0)
                {
                    f |= (int)EPersonaChange.k_EPersonaChangeComeOnline;
                }
                else if (n.PersonaState == 0)
                {
                    f |= (int)EPersonaChange.k_EPersonaChangeGoneOffline;
                }
            }

            // Rich presence is lumped under GamePlayed, as before.
            if ((o?.GameID ?? 0) != n.GameID || (o?.LobbyID ?? 0) != n.LobbyID || !SameRichPresence(o?.RichPresence, n.RichPresence))
            {
                f |= (int)EPersonaChange.k_EPersonaChangeGamePlayed;
            }

            if ((o?.GameServerSteamID ?? 0) != n.GameServerSteamID || (o?.GameServerIP ?? 0) != n.GameServerIP || (o?.GameServerPort ?? 0) != n.GameServerPort)
            {
                f |= (int)EPersonaChange.k_EPersonaChangeGameServer;
            }

            if (o == null)
            {
                f |= (int)EPersonaChange.k_EPersonaChangeAvatar;   // no avatar hash exists: Dota loads it for new entries
            }

            return f;
        }

        private static bool SameRichPresence(Dictionary<string, string> a, Dictionary<string, string> b)
        {
            var ac = a?.Count ?? 0;
            var bc = b?.Count ?? 0;
            if (ac != bc)
            {
                return false;
            }

            if (ac == 0)
            {
                return true;
            }

            foreach (var kv in a)
            {
                if (!b.TryGetValue(kv.Key, out var value) || !string.Equals(kv.Value, value, StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool SamePlayer(SteamPlayer a, SteamPlayer b)
        {
            return a.AccountID == b.AccountID &&
                   a.SteamID == b.SteamID &&
                   string.Equals(a.PersonaName ?? string.Empty, b.PersonaName ?? string.Empty, StringComparison.Ordinal) &&
                   a.GameID == b.GameID &&
                   a.LobbyID == b.LobbyID &&
                   a.GameServerSteamID == b.GameServerSteamID &&
                   a.GameServerIP == b.GameServerIP &&
                   a.GameServerPort == b.GameServerPort &&
                   a.PersonaState == b.PersonaState &&
                   a.PlayerLevel == b.PlayerLevel &&
                   a.HasFriend == b.HasFriend &&
                   a.FriendRelationship == b.FriendRelationship &&
                   string.Equals(a.IPAddress ?? string.Empty, b.IPAddress ?? string.Empty, StringComparison.Ordinal) &&
                   SameRichPresence(a.RichPresence, b.RichPresence);
        }

        // Copy of `source` (or a placeholder when there is none) with the given relationship.
        private static SteamPlayer WithRelationship(SteamPlayer source, ulong id, int rel)
        {
            var np = source != null
                ? ClonePlayer(source)
                : new SteamPlayer
                {
                    AccountID = id.GetAccountID(),
                    SteamID = id,
                    PersonaName = string.Empty,
                    IPAddress = string.Empty
                };
            np.FriendRelationship = rel;
            np.HasFriend = rel == (int)EFriendRelationship.k_EFriendRelationshipFriend;
            return np;
        }

        #endregion

        #region Local actions

        private sealed class PendingEntry
        {
            public int Epoch;
            public int Count;
            public int LocalRel;
            public bool AnyFailure;
            public SteamPlayer Confirmed;   // last server-confirmed entry (null when none existed)
            public long CreatedMs;
        }

        // Guarded by SyncRoot.
        private static readonly Dictionary<ulong, PendingEntry> Pending = new Dictionary<ulong, PendingEntry>();

        private enum Plan { Optimistic, Unoptimistic, SkipResync }

        // A stale cache must never make a button silently dead: when the cache contradicts the pressed dialog the POST
        // still goes out (the server decides) as an unoptimistic item and a resync repairs the cache afterwards.
        private static Plan PlanFor(FriendKind kind, int rel, out int desired)
        {
            const int none = (int)EFriendRelationship.k_EFriendRelationshipNone;
            const int recipient = (int)EFriendRelationship.k_EFriendRelationshipRequestRecipient;
            const int friend = (int)EFriendRelationship.k_EFriendRelationshipFriend;
            const int initiator = (int)EFriendRelationship.k_EFriendRelationshipRequestInitiator;
            desired = 0;

            switch (kind)
            {
                case FriendKind.Add:
                    if (rel == none) { desired = initiator; return Plan.Optimistic; }
                    if (rel == recipient) { desired = friend; return Plan.Optimistic; }   // the server auto-accepts the reverse request
                    return Plan.SkipResync;
                case FriendKind.Accept:
                    if (rel == recipient) { desired = friend; return Plan.Optimistic; }
                    return Plan.Unoptimistic;
                case FriendKind.Ignore:
                    if (rel == recipient) { desired = none; return Plan.Optimistic; }
                    return Plan.Unoptimistic;
                default:   // Remove: unfriend / cancel / decline
                    if (rel == friend || rel == initiator || rel == recipient) { desired = none; return Plan.Optimistic; }
                    return Plan.Unoptimistic;
            }
        }

        // Dota's thread, no "Users" held. Epoch and identity generation are captured inside the lock.
        internal static BeginResult BeginLocal(ulong id, FriendKind kind)
        {
            var result = new BeginResult();
            result.Diffs = Mutate((copy, r) =>
            {
                copy.TryGetValue(id, out var cached);
                Pending.TryGetValue(id, out var p);
                var rel = p != null ? p.LocalRel : RelOf(cached);
                var plan = PlanFor(kind, rel, out var desired);
                var planText = plan == Plan.Optimistic ? "optimistic(" + desired + ")" : plan == Plan.Unoptimistic ? "unoptimistic" : "skip-resync";
                FriendActions.Fr($"dialog={FriendActions.DialogName(kind)} id={id} R={rel} plan={planText}");

                if (plan == Plan.SkipResync)
                {
                    result.NeedResync = true;
                    return;
                }

                if (plan == Plan.Unoptimistic)
                {
                    result.Item = new FriendItem(id, kind, 0, 0, IdentityGen);
                    return;
                }

                if (p == null)
                {
                    p = new PendingEntry
                    {
                        Epoch = ++_epochSeq,
                        Confirmed = ClonePlayer(cached),
                        CreatedMs = NowMs()
                    };
                    Pending[id] = p;
                }

                p.Count++;
                p.LocalRel = desired;
                Put(copy, r, WithRelationship(cached, id, desired));
                Touch(id);
                r.Changed = true;
                result.Item = new FriendItem(id, kind, desired, p.Epoch, IdentityGen);
            });
            return result;
        }

        // Executor thread, exactly once per item.
        internal static void ResolveAction(FriendItem item, Outcome outcome)
        {
            if (Interlocked.Exchange(ref item.Resolved, 1) == 1)
            {
                return;
            }

            long resyncStamp = 0;
            var stale = false;
            var diffs = Mutate((copy, r) =>
            {
                if (item.Gen != IdentityGen)
                {
                    stale = true;
                    return;
                }

                if (item.Epoch == 0)
                {
                    resyncStamp = NextStamp();   // unoptimistic press: nothing local to settle, just re-read the server
                    return;
                }

                if (!Pending.TryGetValue(item.Id, out var p) || p.Epoch != item.Epoch)
                {
                    stale = true;
                    return;
                }

                if (outcome == Outcome.Ok || outcome == Outcome.OkAlreadyGone)
                {
                    if (p.Confirmed == null)
                    {
                        copy.TryGetValue(item.Id, out var current);
                        p.Confirmed = WithRelationship(current, item.Id, item.Desired);
                    }
                    else
                    {
                        p.Confirmed.FriendRelationship = item.Desired;
                        p.Confirmed.HasFriend = item.Desired == (int)EFriendRelationship.k_EFriendRelationshipFriend;
                    }
                }
                else
                {
                    p.AnyFailure = true;
                }

                p.Count--;
                FriendActions.Fr($"action={FriendActions.DialogName(item.Kind)} id={item.Id} outcome={outcome} count={p.Count} epoch={p.Epoch}");

                // Failures with Count > 0 wait: the remaining actions were computed on top of the failed one.
                if (p.Count > 0)
                {
                    return;
                }

                Pending.Remove(item.Id);
                Touch(item.Id);
                r.Changed = true;
                if (p.AnyFailure)
                {
                    FriendActions.Fr($"restore id={item.Id} rel={RelOf(p.Confirmed)}");
                    RestoreFrom(copy, r, item.Id, p.Confirmed);
                }

                // Otherwise keep the local value: the server accepted every action.
                resyncStamp = NextStamp();
            });

            if (stale)
            {
                FriendActions.Fr($"action={FriendActions.DialogName(item.Kind)} id={item.Id} outcome={outcome} ignored-stale");
                return;
            }

            if (item.Epoch == 0)
            {
                FriendActions.Fr($"action={FriendActions.DialogName(item.Kind)} id={item.Id} outcome={outcome} unoptimistic");
            }

            Emit(diffs);
            if (resyncStamp != 0)
            {
                FriendResync.RequestResync(resyncStamp);
            }
        }

        private static void RestoreFrom(Dictionary<ulong, SteamPlayer> copy, MutateResult r, ulong id, SteamPlayer confirmed)
        {
            if (confirmed != null)
            {
                Put(copy, r, ClonePlayer(confirmed));
            }
            else if (copy.TryGetValue(id, out var current))
            {
                copy.Remove(id);
                r.Changed = true;
                if (RelOf(current) != 0)
                {
                    AddDiff(r, id, (int)EPersonaChange.k_EPersonaChangeRelationshipChanged);
                }
            }
        }

        // Executor idle tick (the only caller): a Pending that never resolved is force-resolved as a failure.
        internal static void SweepPending()
        {
            long resyncStamp = 0;
            var diffs = Mutate((copy, r) =>
            {
                List<ulong> expired = null;
                var now = NowMs();
                foreach (var kv in Pending)
                {
                    if (now - kv.Value.CreatedMs > 120000)
                    {
                        if (expired == null)
                        {
                            expired = new List<ulong>();
                        }

                        expired.Add(kv.Key);
                    }
                }

                if (expired == null)
                {
                    return;
                }

                foreach (var id in expired)
                {
                    var p = Pending[id];
                    Pending.Remove(id);
                    FriendActions.Fr($"pending-watchdog id={id}");
                    RestoreFrom(copy, r, id, p.Confirmed);
                    Touch(id);
                    r.Changed = true;
                }

                resyncStamp = NextStamp();
            });

            Emit(diffs);
            if (resyncStamp != 0)
            {
                FriendResync.RequestResync(resyncStamp);
            }
        }

        // An item of a force-resolved epoch (or an old identity) must not be POSTed.
        internal static bool IsItemStale(FriendItem item)
        {
            if (item.Epoch == 0)
            {
                return false;
            }

            lock (SyncRoot)
            {
                return !Pending.TryGetValue(item.Id, out var p) || p.Epoch != item.Epoch;
            }
        }

        #endregion

        #region Events

        private static ulong EventSteamId(APIClient.ApiEvent serverEvent)
        {
            return serverEvent.SteamId != 0
                ? serverEvent.SteamId
                : (ulong)new CSteamID(serverEvent.AccountId);
        }

        internal static bool IsSocialEvent(string type)
        {
            switch (type)
            {
                case "persona_state_changed":
                case "friend_added":
                case "friend_request_received":
                case "friend_request_sent":
                case "friend_presence_changed":
                case "friend_removed":
                    return true;
                default:
                    return false;
            }
        }

        // Applies one social event from the ordered server log. Returns the diffs to Emit.
        internal static List<PersonaDiff> ApplySocialEvent(APIClient.ApiEvent serverEvent)
        {
            var id = EventSteamId(serverEvent);
            if (id == 0 || id == (ulong)SteamEmulator.SteamID)
            {
                return new List<PersonaDiff>();
            }

            var type = serverEvent.Type;
            var relationshipEvent = type == "friend_added" || type == "friend_request_received" ||
                                    type == "friend_request_sent" || type == "friend_removed";
            var rel = 0;
            if (relationshipEvent)
            {
                switch (type)
                {
                    case "friend_added":
                        rel = serverEvent.FriendRelationship != 0 ? serverEvent.FriendRelationship : (int)EFriendRelationship.k_EFriendRelationshipFriend;
                        break;
                    case "friend_request_received":
                        rel = serverEvent.FriendRelationship != 0 ? serverEvent.FriendRelationship : (int)EFriendRelationship.k_EFriendRelationshipRequestRecipient;
                        break;
                    case "friend_request_sent":
                        rel = serverEvent.FriendRelationship != 0 ? serverEvent.FriendRelationship : (int)EFriendRelationship.k_EFriendRelationshipRequestInitiator;
                        break;
                    default:   // friend_removed
                        rel = 0;
                        break;
                }
            }

            var avatarFlag = relationshipEvent ? 0 : serverEvent.ChangeFlags & (int)EPersonaChange.k_EPersonaChangeAvatar;
            var result = "applied";
            var cachedAndKnown = false;

            var diffs = Mutate((copy, r) =>
            {
                copy.TryGetValue(id, out var existing);
                cachedAndKnown = existing != null;
                var pending = Pending.ContainsKey(id);

                if (relationshipEvent)
                {
                    if (pending)
                    {
                        result = "ignored-pending";
                        return;
                    }

                    if (existing == null && rel == 0)
                    {
                        result = "dropped-absent";
                        return;
                    }

                    var np = WithRelationship(existing, id, rel);
                    ApplyEventPersona(np, existing, serverEvent, false);
                    Touch(id);
                    if (!Put(copy, r, np))
                    {
                        result = "noop";
                    }

                    return;
                }

                // Presence events never create entries and never change the relationship.
                if (existing == null)
                {
                    result = "dropped-absent";
                    return;
                }

                if (pending)
                {
                    result = avatarFlag != 0 ? "avatar-only" : "ignored-pending";
                    return;
                }

                var updated = ClonePlayer(existing);
                ApplyEventPersona(updated, existing, serverEvent, true);
                Touch(id);
                if (!Put(copy, r, updated))
                {
                    result = "noop";
                }
            });

            // The avatar bit is not relationship data: honoured for cached ids even while Pending or when nothing else
            // changed, so a web avatar upload still reaches the game. It alone asks for a re-download.
            if (avatarFlag != 0 && cachedAndKnown)
            {
                var index = diffs.FindIndex(d => d.Id == id);
                if (index >= 0)
                {
                    diffs[index] = new PersonaDiff(id, diffs[index].Flags | avatarFlag, true);
                }
                else
                {
                    diffs.Add(new PersonaDiff(id, avatarFlag, true));
                    if (result == "noop")
                    {
                        result = "avatar-only";
                    }
                }
            }

            var total = diffs.Where(d => d.Id == id).Select(d => d.Flags).FirstOrDefault();
            FriendActions.Fr($"event type={type} id={id} rel={rel} result={result} flags=0x{total:X}");
            return diffs;
        }

        // Persona fields an event carries; GameServer* and the level stay as cached unless it is a presence event.
        private static void ApplyEventPersona(SteamPlayer target, SteamPlayer existing, APIClient.ApiEvent serverEvent, bool presence)
        {
            if (existing == null)
            {
                target.AccountID = serverEvent.AccountId != 0 ? serverEvent.AccountId : target.SteamID.GetAccountID();
            }
            else if (serverEvent.AccountId != 0)
            {
                target.AccountID = serverEvent.AccountId;
            }

            if (!string.IsNullOrWhiteSpace(serverEvent.PersonaName))
            {
                target.PersonaName = serverEvent.PersonaName;
            }

            target.GameID = serverEvent.AppId;
            target.LobbyID = serverEvent.LobbyId;
            target.PersonaState = serverEvent.PersonaState;
            if (serverEvent.RichPresence != null)
            {
                target.RichPresence = new Dictionary<string, string>(serverEvent.RichPresence);
            }

            if (presence)
            {
                target.GameServerSteamID = serverEvent.GameServerSteamId;
                target.GameServerIP = serverEvent.GameServerIp;
                target.GameServerPort = serverEvent.GameServerPort;
            }
        }

        #endregion

        public static void ApplyStats(ulong steamId, IEnumerable<APIClient.ApiStat> stats)
        {
            var mapped = stats == null
                ? new List<PlayerStat>()
                : stats.Select(s => new PlayerStat
                {
                    Name = s.Name,
                    Data = s.Data
                }).ToList();

            PlayerStats[steamId] = mapped;
            StatsRefreshedAt[steamId] = DateTime.UtcNow;

            if (steamId == (ulong)SteamEmulator.SteamID)
            {
                SelfStatsDirty = false;
            }
        }

        public static void ApplyAchievements(ulong steamId, IEnumerable<APIClient.ApiAchievement> achievements)
        {
            var mapped = achievements == null
                ? new List<Achievement>()
                : achievements.Select(a => new Achievement
                {
                    Name = a.Name,
                    Earned = a.Earned,
                    Date = a.Date,
                    Progress = a.Progress,
                    MaxProgress = a.MaxProgress
                }).ToList();

            PlayerAchievements[steamId] = mapped;
            StatsRefreshedAt[steamId] = DateTime.UtcNow;

            if (steamId == (ulong)SteamEmulator.SteamID)
            {
                SelfAchievementsDirty = false;
            }
        }

        public static void SetCurrentPlayers(int currentPlayers)
        {
            CurrentPlayers = currentPlayers;
        }

        public static int GetCurrentPlayers()
        {
            return CurrentPlayers;
        }

        public static int GetSelfPlayerLevel()
        {
            return SelfPlayerLevel;
        }

        public static bool TryGetSelf(out SteamPlayer self)
        {
            self = Self == null ? null : ClonePlayer(Self);
            return self != null;
        }

        public static List<PlayerStat> GetStats(ulong steamId)
        {
            if (PlayerStats.TryGetValue(steamId, out var stats))
            {
                return stats.Select(s => new PlayerStat
                {
                    Name = s.Name,
                    Data = s.Data
                }).ToList();
            }

            return new List<PlayerStat>();
        }

        public static List<Achievement> GetAchievements(ulong steamId)
        {
            if (PlayerAchievements.TryGetValue(steamId, out var achievements))
            {
                return achievements.Select(a => new Achievement
                {
                    Name = a.Name,
                    Earned = a.Earned,
                    Date = a.Date,
                    Progress = a.Progress,
                    MaxProgress = a.MaxProgress
                }).ToList();
            }

            return new List<Achievement>();
        }

        public static void SetLocalStat(string name, uint data)
        {
            UpsertStat((ulong)SteamEmulator.SteamID, name, data, true);
        }

        public static void UpsertStat(ulong steamId, string name, uint data, bool markDirty)
        {
            var stats = PlayerStats.GetOrAdd(steamId, _ => new List<PlayerStat>());
            var stat = stats.Find(s => s.Name == name);

            if (stat == null)
            {
                stats.Add(new PlayerStat
                {
                    Name = name,
                    Data = data
                });
            }
            else
            {
                stat.Data = data;
            }

            if (markDirty && steamId == (ulong)SteamEmulator.SteamID)
            {
                SelfStatsDirty = true;
            }

            StatsRefreshedAt[steamId] = DateTime.UtcNow;
        }

        public static void SetLocalAchievement(string name, bool earned, uint progress = 0, uint maxProgress = 0)
        {
            UpsertAchievement((ulong)SteamEmulator.SteamID, name, earned, progress, maxProgress, true);
        }

        public static void UpsertAchievement(ulong steamId, string name, bool earned, uint progress, uint maxProgress, bool markDirty)
        {
            var achievements = PlayerAchievements.GetOrAdd(steamId, _ => new List<Achievement>());
            var achievement = achievements.Find(a => a.Name == name);

            if (achievement == null)
            {
                achievements.Add(new Achievement
                {
                    Name = name,
                    Earned = earned,
                    Date = DateTime.UtcNow,
                    Progress = progress,
                    MaxProgress = maxProgress
                });
            }
            else
            {
                achievement.Earned = earned;
                achievement.Date = DateTime.UtcNow;
                achievement.Progress = progress;
                achievement.MaxProgress = maxProgress;
            }

            if (markDirty && steamId == (ulong)SteamEmulator.SteamID)
            {
                SelfAchievementsDirty = true;
            }

            StatsRefreshedAt[steamId] = DateTime.UtcNow;
        }

        public static bool HasPendingStatsChanges()
        {
            return SelfStatsDirty || SelfAchievementsDirty;
        }

        public static void MarkStatsStored()
        {
            SelfStatsDirty = false;
            SelfAchievementsDirty = false;
        }

        public static string GetEventCursor()
        {
            return EventCursor ?? string.Empty;
        }

        public static void SetEventCursor(string cursor)
        {
            EventCursor = cursor ?? string.Empty;
        }

        private static SteamPlayer MapUser(APIClient.ApiUser user)
        {
            var steamId = user.SteamId != 0
                ? user.SteamId
                : (ulong)new CSteamID(user.AccountId);

            return new SteamPlayer
            {
                AccountID = user.AccountId != 0 ? user.AccountId : steamId.GetAccountID(),
                SteamID = steamId,
                PersonaName = user.PersonaName ?? string.Empty,
                GameID = user.AppId,
                LobbyID = user.LobbyId,
                GameServerSteamID = user.GameServerSteamId,
                GameServerIP = user.GameServerIp,
                GameServerPort = user.GameServerPort,
                PersonaState = user.PersonaState,
                PlayerLevel = user.PlayerLevel,
                FriendRelationship = user.FriendRelationship != 0
                    ? user.FriendRelationship
                    : (user.HasFriend || steamId == (ulong)SteamEmulator.SteamID
                        ? (int)EFriendRelationship.k_EFriendRelationshipFriend
                        : (int)EFriendRelationship.k_EFriendRelationshipNone),
                HasFriend = user.HasFriend || user.FriendRelationship == (int)EFriendRelationship.k_EFriendRelationshipFriend,
                IPAddress = string.Empty,
                RichPresence = user.RichPresence ?? new Dictionary<string, string>()
            };
        }

        private static SteamPlayer ClonePlayer(SteamPlayer player)
        {
            if (player == null)
            {
                return null;
            }

            return new SteamPlayer
            {
                AccountID = player.AccountID,
                SteamID = player.SteamID,
                PersonaName = player.PersonaName,
                GameID = player.GameID,
                LobbyID = player.LobbyID,
                GameServerSteamID = player.GameServerSteamID,
                GameServerIP = player.GameServerIP,
                GameServerPort = player.GameServerPort,
                PersonaState = player.PersonaState,
                PlayerLevel = player.PlayerLevel,
                HasFriend = player.HasFriend,
                FriendRelationship = player.FriendRelationship,
                IPAddress = player.IPAddress,
                RichPresence = new Dictionary<string, string>(player.RichPresence ?? new Dictionary<string, string>())
            };
        }
    }
}


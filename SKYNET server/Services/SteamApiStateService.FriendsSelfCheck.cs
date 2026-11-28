using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;
using SKYNET_server.Models;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    /// <summary>
    /// Self-check for the social data the server hands to clients: api/friends carries confirmed friends AND
    /// pending requests in both directions with an explicit relationship, every change produces exactly one
    /// event per side, and deleting a user tells its counterparts. Run with `--verify-friends`.
    /// It calls the *Locked methods directly (no HTTP, no sessions), so it cannot prove routing, auth or timing.
    /// </summary>
    public static bool RunFriendsSelfCheck(Action<string>? write = null)
    {
        write ??= _ => { };
        var failures = 0;
        void Check(bool condition, string name)
        {
            write($"{(condition ? "PASS" : "FAIL")} {name}");
            if (!condition)
            {
                failures++;
            }
        }

        // Field initializers do not run under GetUninitializedObject: set every field the exercised methods touch.
        var service = (SteamApiStateService)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(SteamApiStateService));
        void Set(string field, object value) =>
            typeof(SteamApiStateService).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(service, value);
        Set("_sync", new object());
        Set("_sessions", new Dictionary<string, ApiSession>(StringComparer.Ordinal));
        Set("_events", new List<ApiQueuedEvent>());
        Set("_flushSignal", new ManualResetEventSlim(false));
        Set("_presenceTimeout", TimeSpan.FromSeconds(90));
        Set("_sessionTimeout", TimeSpan.FromMinutes(30));
        Set("_gameServerLeaseTimeout", TimeSpan.FromSeconds(90));
        Set("_gameServerLeases", new Dictionary<ulong, DateTime>());
        Set("_logger", NullLogger<SteamApiStateService>.Instance);
        Set("_state", new ApiState());
        Set("_nextEventSequence", InitialEventSequence());

        ulong Id(uint account) => SteamIdAllocator.IndividualSteamId(account);
        var a = Id(1000);
        var b = Id(1001);
        var c = Id(1002);
        var d = Id(1003);
        var e = Id(1004);
        var g = Id(1005);
        lock (service._sync)
        {
            service.CreateUserLocked(a, 1000, 0, "Alice");
            service.CreateUserLocked(b, 1001, 0, "Bob");
            service.CreateUserLocked(c, 1002, 0, "Carol");
            service.CreateUserLocked(d, 1003, 0, "Dave");
            service.CreateUserLocked(e, 1004, 0, "Erin");
            service.CreateUserLocked(g, 1005, 0, "Gina");
        }

        List<ApiUser> ListOf(ulong viewer)
        {
            lock (service._sync)
            {
                return service.GetKnownSocialUsersLocked(viewer);
            }
        }

        int RelOf(ulong viewer, ulong other) => ListOf(viewer).FirstOrDefault(u => u.SteamId == other)?.FriendRelationship ?? -1;
        bool Sane(ulong viewer) => ListOf(viewer).All(u => u.SteamId != viewer && u.FriendRelationship != FriendRelationshipNone);
        var sane = true;
        bool Send(ulong from, ulong to) { lock (service._sync) { return service.SendFriendRequestLocked(from, to); } }
        bool Remove(ulong who, ulong other) { lock (service._sync) { return service.RemoveFriendOrRequestLocked(who, other); } }
        bool Accept(ulong from, ulong to)
        {
            lock (service._sync)
            {
                var request = service.FindPendingRequestLocked(from, to);
                return request != null && service.AcceptFriendRequestLocked(request);
            }
        }
        List<ApiQueuedEvent> EventsAfter(long sequence) => service._events.Where(x => x.Sequence > sequence).OrderBy(x => x.Sequence).ToList();
        long LastSequence() => service._events.Count == 0 ? 0 : service._events.Max(x => x.Sequence);
        int Count(List<ApiQueuedEvent> events, ulong recipient, string type, ulong about, int relationship) =>
            events.Count(x => x.RecipientSteamId == recipient && x.Event.Type == type && x.Event.SteamId == about && x.Event.FriendRelationship == relationship);
        void AllSane() => sane &= new[] { a, b, c, d, e, g }.All(Sane);

        // ---- 1/2: A -> B request ---------------------------------------------------------------
        var mark = LastSequence();
        Check(Send(a, b), "A -> B request accepted by the server");
        var ev = EventsAfter(mark);
        Check(RelOf(a, b) == 4 && ListOf(a).First(u => u.SteamId == b).HasFriend == false, "sender sees the target as RequestInitiator (4), not a friend");
        Check(RelOf(b, a) == 2 && ListOf(b).First(u => u.SteamId == a).HasFriend == false, "target sees the sender as RequestRecipient (2), not a friend");
        Check(ListOf(c).Count == 0, "an uninvolved user sees neither");
        Check(ev.Count == 2 && Count(ev, a, "friend_request_sent", b, 4) == 1 && Count(ev, b, "friend_request_received", a, 2) == 1,
            "exactly one friend_request_sent (4) to A and one friend_request_received (2) to B");
        Check(ev.Count == 2 && ev[0].Sequence < ev[1].Sequence, "event sequences are strictly increasing");
        AllSane();

        // ---- 3: accept -----------------------------------------------------------------------------
        mark = LastSequence();
        Check(Accept(a, b), "B accepts the request");
        ev = EventsAfter(mark);
        Check(ListOf(a).Count == 1 && RelOf(a, b) == 3 && ListOf(b).Count == 1 && RelOf(b, a) == 3, "both lists hold the other once as Friend (3)");
        Check(ListOf(a).First().HasFriend && ListOf(b).First().HasFriend, "HasFriend is true for confirmed friends");
        Check(!service._state.FriendRequests.Any(IsPending), "no pending request is left after accepting");
        Check(ev.Count == 2 && Count(ev, a, "friend_added", b, 3) == 1 && Count(ev, b, "friend_added", a, 3) == 1, "both sides get one friend_added");
        AllSane();

        // ---- 4: remove -----------------------------------------------------------------------------
        mark = LastSequence();
        Check(Remove(a, b), "removing a friend succeeds");
        ev = EventsAfter(mark);
        Check(ListOf(a).Count == 0 && ListOf(b).Count == 0, "both lists are empty after the removal");
        Check(ev.Count == 2 && Count(ev, a, "friend_removed", b, 0) == 1 && Count(ev, b, "friend_removed", a, 0) == 1, "both sides get friend_removed with relationship 0");
        mark = LastSequence();
        Check(!Remove(a, b) && EventsAfter(mark).Count == 0, "a second removal returns false and emits nothing");
        AllSane();

        // ---- 5: cancel (sender) and decline (recipient) -------------------------------------------
        mark = LastSequence();
        Send(a, b);
        Check(Remove(a, b), "the sender cancels the pending request");
        Check(service._state.FriendRequests.Last().Status == "cancelled", "cancel by the sender closes it as 'cancelled'");
        ev = EventsAfter(mark).Where(x => x.Event.Type == "friend_removed").ToList();
        Check(Count(ev, a, "friend_removed", b, 0) == 1 && Count(ev, b, "friend_removed", a, 0) == 1 && ListOf(a).Count == 0 && ListOf(b).Count == 0,
            "cancel notifies both sides and clears both lists");
        mark = LastSequence();
        Send(a, b);
        Check(Remove(b, a), "the recipient declines the pending request");
        Check(service._state.FriendRequests.Last().Status == "declined", "decline by the recipient closes it as 'declined'");
        ev = EventsAfter(mark).Where(x => x.Event.Type == "friend_removed").ToList();
        Check(Count(ev, a, "friend_removed", b, 0) == 1 && Count(ev, b, "friend_removed", a, 0) == 1, "decline notifies both sides");
        AllSane();

        // ---- 6: B -> A while A -> B is pending auto-accepts ------------------------------------------
        Send(a, b);
        Check(Send(b, a), "the reverse request while one is pending is accepted");
        Check(service._state.FriendLinks[a].Count == 1 && service._state.FriendLinks[b].Count == 1 && RelOf(a, b) == 3 && RelOf(b, a) == 3 &&
              service._state.FriendRequests.Last().Status == "accepted" && !service._state.FriendRequests.Any(IsPending),
            "crossing requests produce one friendship and no pending leftovers");
        Remove(a, b);

        // ---- 7: never self, never relationship 0 ---------------------------------------------------
        AllSane();
        Check(sane, "no list ever contained the viewer itself or a relationship-0 entry");
        Check(!Send(a, a) && ListOf(a).Count == 0, "a request to oneself is refused");

        // ---- 8: deleting a user ------------------------------------------------------------------
        // B: friend of A; pending B -> C; pending G -> B; accepted history in both directions with D; declined B -> E.
        // A -> C stays pending and is unrelated to B.
        Send(a, b); Accept(a, b);
        Send(b, c);
        Send(g, b);
        Send(a, c);
        Send(b, d); Send(d, b);          // accepted, request from B
        Remove(b, d);
        Send(d, b); Send(b, d);          // accepted again, request from D
        Remove(b, d);
        Send(b, e); Remove(e, b);        // declined, request from B
        var historyBefore = service._state.FriendRequests
            .Where(r => !IsPending(r))
            .Select(r => (r.Id, r.Status, r.RespondedAt))
            .ToList();
        Check(historyBefore.Count(h => h.Status == "accepted") == 5 && historyBefore.Count(h => h.Status == "declined") == 2,
            "setup: accepted/declined history in both directions exists before the deletion");
        mark = LastSequence();
        lock (service._sync)
        {
            service.AdminDeleteUserLocked(b);
        }
        ev = EventsAfter(mark);
        Check(Count(ev, a, "friend_removed", b, 0) == 1 && Count(ev, c, "friend_removed", b, 0) == 1 && Count(ev, g, "friend_removed", b, 0) == 1,
            "the former friend and both pending counterparts get friend_removed about the deleted user");
        Check(ev.All(x => x.RecipientSteamId != b) && ev.Count == 3, "nothing is sent to the deleted user");
        var removed = ev.FirstOrDefault(x => x.RecipientSteamId == a)?.Event;
        Check(removed != null && removed.PersonaName == "Bob" && removed.AccountId == 1001 && removed.ChangeFlags == 512,
            "the event is built from the captured record (name, account, flags 512)");
        var bPending = service._state.FriendRequests.Where(r => r.FromSteamId == b || r.ToSteamId == b).ToList();
        Check(!bPending.Any(IsPending), "no pending request involving the deleted user remains");
        Check(service._state.FriendRequests.Count(r => r.Status == "cancelled" && r.RespondedAt != null &&
                  ((r.FromSteamId == b && r.ToSteamId == c) || (r.FromSteamId == g && r.ToSteamId == b))) == 2,
            "both pending requests (B -> C, G -> B) became 'cancelled' with RespondedAt");
        var historyAfter = service._state.FriendRequests.Where(r => historyBefore.Any(h => h.Id == r.Id)).Select(r => (r.Id, r.Status, r.RespondedAt)).ToList();
        Check(historyAfter.Count == historyBefore.Count && historyBefore.All(h => historyAfter.Contains(h)),
            "accepted/declined requests keep their status and RespondedAt in both directions");
        Check(service._state.FriendRequests.Any(r => r.FromSteamId == a && r.ToSteamId == c && IsPending(r)), "an unrelated pending request is untouched");
        Check(ListOf(a).All(u => u.SteamId != b) && ListOf(c).All(u => u.SteamId != b) && !service._state.Users.ContainsKey(b),
            "the deleted user is gone from the users and from the counterparts' lists");
        AllSane();

        // ---- 9: event sequence seed --------------------------------------------------------------
        var seed1 = InitialEventSequence();
        var seed2 = InitialEventSequence();
        Check(seed1 > 10_000_000_000L, "the sequence seed is far above any small counter value");
        Check(Math.Abs(seed1 - DateTime.UtcNow.Ticks) < TimeSpan.FromSeconds(5).Ticks, "the seed follows the UTC clock");
        Check(seed2 >= seed1, "successive seeds do not decrease");

        // ---- 10: web actions report the resulting relationship and a truthful outcome -------------------
        // D and E are untouched by the sections above.
        ApiState Snapshot() => service._state;
        SteamApiStateService.WebFriendActionResult Web(ulong viewer, ulong target, string action)
        {
            lock (service._sync)
            {
                return service.WebFriendActionLocked(viewer, target, action);
            }
        }
        Check(Snapshot().FriendRequests.All(r => !IsPending(r) || r.FromSteamId != d), "web section starts with no pending request of D");
        var w = Web(d, e, "request");
        Check(w.Success && w.Relationship == 4 && w.Message == "Request sent." && w.Status == 200, "web request: success, relationship 4 (outgoing)");
        w = Web(d, e, "request");
        Check(!w.Success && w.Relationship == 4 && w.Status == 409 && w.Message != "Request sent.", "web request again: refused, not reported as sent, relationship still 4");
        w = Web(e, d, "remove");
        Check(w.Success && w.Relationship == 0 && w.Message == "Request declined.", "web remove by the recipient is a decline, relationship 0");
        Web(d, e, "request");
        w = Web(d, e, "remove");
        Check(w.Success && w.Relationship == 0 && w.Message == "Request cancelled.", "web remove by the sender is a cancel, relationship 0");
        w = Web(d, e, "remove");
        Check(!w.Success && w.Relationship == 0 && w.Status == 409, "web remove with nothing to remove fails and says 0");
        w = Web(d, e, "accept");
        Check(!w.Success && w.Relationship == 0, "web accept without a pending request fails");
        Web(d, e, "request");
        w = Web(e, d, "accept");
        Check(w.Success && w.Relationship == 3 && w.Message == "Request accepted.", "web accept: relationship 3 (friend)");
        w = Web(e, d, "request");
        Check(!w.Success && w.Relationship == 3, "web request to a friend is refused with relationship 3");
        w = Web(e, d, "remove");
        Check(w.Success && w.Relationship == 0 && w.Message == "Friend removed.", "web remove of a friend: relationship 0");
        Web(d, e, "request");
        w = Web(e, d, "request");
        Check(w.Success && w.Relationship == 3 && w.Message == "Request accepted.", "web crossing request ends as friend (3), not 4");
        Web(d, e, "remove");
        Check(!Web(d, d, "request").Success && Web(d, d, "request").Relationship == null, "web action on oneself is refused without a relationship");
        Check(Web(d, 123456789UL, "request").Status == 404 && Web(d, e, "bogus").Status == 400, "web action on an unknown user is 404, an unknown action 400");
        Check(Web(d, e, "accept").Relationship == 0 && ListOf(d).Count == 0 && ListOf(e).Count == 0, "web section leaves no relationship behind");
        // Token-based wrappers: no session means 401 / no map; a real session sees the other users' relationships.
        Check(service.WebFriendAction("nope", e, "request").Status == 401 && service.GetWebRelationships("nope") == null, "no session: 401 and no relationship map");
        const string testToken = "selfcheck-token";
        lock (service._sync)
        {
            service._sessions[testToken] = new ApiSession { AccessToken = testToken, SteamId = d, WebSession = true, LastSeenUtc = DateTime.UtcNow };
        }
        var sentByToken = service.WebFriendAction(testToken, e, "request");
        var map = service.GetWebRelationships(testToken);
        Check(sentByToken.Success && sentByToken.Relationship == 4 && map != null && map[e.ToString()] == 4 && !map.ContainsKey(d.ToString()),
            "poll map carries the viewer's relationship per other user (and not the viewer)");
        Web(d, e, "remove");

        write(failures == 0 ? "friends self-check: all passed" : $"friends self-check: {failures} FAILED");
        return failures == 0;
    }
}

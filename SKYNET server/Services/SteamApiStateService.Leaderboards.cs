using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SKYNET_server.Models;
using SKYNET_server.Persistence;
using SKYNET_server.Persistence.Entities;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    private const int LeaderboardSortAscending = 1;
    private const int LeaderboardSortDescending = 2;
    private const int LeaderboardUploadKeepBest = 1;
    private const int LeaderboardUploadForceUpdate = 2;
    private const int MaxLeaderboardDetails = 64;
    private const int MaxLeaderboardQueryEntries = 1000;

    public ApiLeaderboard? FindOrCreateLeaderboard(string token, ApiLeaderboardFindRequest request)
    {
        if (!TryResolveLeaderboardCaller(token, out _, out var appId) ||
            !TryNormalizeLeaderboardName(request?.Name, out var name) ||
            !IsValidLeaderboardSort(request!.SortMethod) ||
            request.DisplayType is < 0 or > 3)
        {
            return null;
        }

        using var db = _steamDbFactory.CreateDbContext();
        var existing = db.Leaderboards.AsNoTracking()
            .SingleOrDefault(item => item.AppId == appId && item.Name == name);
        if (existing != null)
        {
            return MapLeaderboard(db, existing);
        }

        var record = new LeaderboardRecord
        {
            AppId = appId,
            Name = name,
            SortMethod = request.SortMethod,
            DisplayType = request.DisplayType,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.Leaderboards.Add(record);
        try
        {
            db.SaveChanges();
            return MapLeaderboard(db, record);
        }
        catch (DbUpdateException)
        {
            // Concurrent find-or-create calls converge on the unique AppId/name row.
            db.ChangeTracker.Clear();
            existing = db.Leaderboards.AsNoTracking()
                .SingleOrDefault(item => item.AppId == appId && item.Name == name);
            return existing == null ? null : MapLeaderboard(db, existing);
        }
    }

    public ApiLeaderboard? GetLeaderboard(string token, ulong leaderboardId)
    {
        if (!TryResolveLeaderboardCaller(token, out _, out var appId) ||
            !TryLeaderboardDatabaseId(leaderboardId, out var databaseId))
        {
            return null;
        }

        using var db = _steamDbFactory.CreateDbContext();
        var record = db.Leaderboards.AsNoTracking()
            .SingleOrDefault(item => item.Id == databaseId && item.AppId == appId);
        return record == null ? null : MapLeaderboard(db, record);
    }

    public ApiLeaderboardEntries? QueryLeaderboardEntries(
        string token,
        ulong leaderboardId,
        ApiLeaderboardEntriesRequest request)
    {
        if (!TryResolveLeaderboardCaller(token, out var steamId, out var appId) ||
            !TryLeaderboardDatabaseId(leaderboardId, out var databaseId) ||
            request == null ||
            request.DataRequest is < 0 or > 3)
        {
            return null;
        }

        HashSet<ulong> friendIds;
        lock (_sync)
        {
            friendIds = _state.FriendLinks.TryGetValue(steamId, out var linked)
                ? new HashSet<ulong>(linked)
                : new HashSet<ulong>();
        }
        friendIds.Add(steamId);

        using var db = _steamDbFactory.CreateDbContext();
        var leaderboard = db.Leaderboards.AsNoTracking()
            .SingleOrDefault(item => item.Id == databaseId && item.AppId == appId);
        if (leaderboard == null)
        {
            return null;
        }

        var ordered = OrderLeaderboardScores(
                db.LeaderboardScores.AsNoTracking().Where(item => item.LeaderboardId == databaseId),
                leaderboard.SortMethod)
            .ToList()
            .Select((score, index) => new RankedLeaderboardScore(score, index + 1))
            .ToList();

        IEnumerable<RankedLeaderboardScore> selected = request.DataRequest switch
        {
            0 => SelectGlobalLeaderboardRange(ordered, request.RangeStart, request.RangeEnd),
            1 => SelectAroundUserLeaderboardRange(ordered, steamId, request.RangeStart, request.RangeEnd),
            2 => ordered.Where(item => friendIds.Contains(item.Score.SteamId)),
            3 => SelectRequestedLeaderboardUsers(ordered, request.Users),
            _ => Array.Empty<RankedLeaderboardScore>()
        };

        return new ApiLeaderboardEntries
        {
            Leaderboard = MapLeaderboard(leaderboard, ordered.Count),
            Entries = selected
                .Take(MaxLeaderboardQueryEntries)
                .Select(MapLeaderboardEntry)
                .ToList()
        };
    }

    public ApiLeaderboardScoreUploadResult? UploadLeaderboardScore(
        string token,
        ulong leaderboardId,
        ApiLeaderboardScoreUploadRequest request)
    {
        if (!TryResolveLeaderboardCaller(token, out var steamId, out var appId) ||
            !TryLeaderboardDatabaseId(leaderboardId, out var databaseId) ||
            request == null ||
            request.UploadMethod is not (LeaderboardUploadKeepBest or LeaderboardUploadForceUpdate))
        {
            return null;
        }

        var details = (request.Details ?? new List<int>())
            .Take(MaxLeaderboardDetails)
            .ToArray();

        using var db = _steamDbFactory.CreateDbContext();
        using var transaction = db.Database.BeginTransaction();
        var leaderboard = db.Leaderboards
            .SingleOrDefault(item => item.Id == databaseId && item.AppId == appId);
        if (leaderboard == null)
        {
            return null;
        }

        var before = OrderLeaderboardScores(
                db.LeaderboardScores.AsNoTracking().Where(item => item.LeaderboardId == databaseId),
                leaderboard.SortMethod)
            .ToList();
        var previousRank = FindLeaderboardRank(before, steamId);
        var existing = db.LeaderboardScores
            .SingleOrDefault(item => item.LeaderboardId == databaseId && item.SteamId == steamId);

        var changed = existing == null ||
            request.UploadMethod == LeaderboardUploadForceUpdate ||
            IsLeaderboardScoreBetter(request.Score, existing.Score, leaderboard.SortMethod);
        if (changed)
        {
            if (existing == null)
            {
                existing = new LeaderboardScoreRecord
                {
                    LeaderboardId = databaseId,
                    SteamId = steamId
                };
                db.LeaderboardScores.Add(existing);
            }

            existing.Score = request.Score;
            existing.DetailsJson = JsonSerializer.Serialize(details);
            existing.UpdatedAtUtc = DateTime.UtcNow;
            db.SaveChanges();
        }

        var after = OrderLeaderboardScores(
                db.LeaderboardScores.AsNoTracking().Where(item => item.LeaderboardId == databaseId),
                leaderboard.SortMethod)
            .ToList();
        transaction.Commit();

        return new ApiLeaderboardScoreUploadResult
        {
            Success = true,
            ScoreChanged = changed,
            Score = changed ? request.Score : existing!.Score,
            GlobalRankPrevious = previousRank,
            GlobalRankNew = FindLeaderboardRank(after, steamId)
        };
    }

    private bool TryResolveLeaderboardCaller(string token, out ulong steamId, out uint appId)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session) || session == null || session.AppId == 0)
            {
                steamId = 0;
                appId = 0;
                return false;
            }

            steamId = session.SteamId;
            appId = session.AppId;
            return steamId != 0;
        }
    }

    private static bool TryNormalizeLeaderboardName(string? candidate, out string name)
    {
        name = (candidate ?? string.Empty).Trim();
        return name.Length is > 0 and <= 128 && name.All(character => !char.IsControl(character));
    }

    private static bool IsValidLeaderboardSort(int sortMethod) =>
        sortMethod is LeaderboardSortAscending or LeaderboardSortDescending;

    private static bool TryLeaderboardDatabaseId(ulong value, out long databaseId)
    {
        databaseId = value <= long.MaxValue ? (long)value : 0;
        return databaseId > 0;
    }

    private static IOrderedQueryable<LeaderboardScoreRecord> OrderLeaderboardScores(
        IQueryable<LeaderboardScoreRecord> scores,
        int sortMethod)
    {
        return sortMethod == LeaderboardSortAscending
            ? scores.OrderBy(item => item.Score).ThenBy(item => item.UpdatedAtUtc).ThenBy(item => item.SteamId)
            : scores.OrderByDescending(item => item.Score).ThenBy(item => item.UpdatedAtUtc).ThenBy(item => item.SteamId);
    }

    private static IEnumerable<RankedLeaderboardScore> SelectGlobalLeaderboardRange(
        IReadOnlyList<RankedLeaderboardScore> scores,
        int rangeStart,
        int rangeEnd)
    {
        var start = Math.Max(0, rangeStart);
        var end = Math.Max(start - 1, rangeEnd);
        return scores.Skip(start).Take(Math.Min(MaxLeaderboardQueryEntries, end - start + 1));
    }

    private static IEnumerable<RankedLeaderboardScore> SelectAroundUserLeaderboardRange(
        IReadOnlyList<RankedLeaderboardScore> scores,
        ulong steamId,
        int rangeStart,
        int rangeEnd)
    {
        var userIndex = -1;
        for (var index = 0; index < scores.Count; index++)
        {
            if (scores[index].Score.SteamId == steamId)
            {
                userIndex = index;
                break;
            }
        }

        if (userIndex < 0)
        {
            return Array.Empty<RankedLeaderboardScore>();
        }

        var start = Math.Max(0, userIndex + rangeStart);
        var end = Math.Min(scores.Count - 1, userIndex + rangeEnd);
        return end < start
            ? Array.Empty<RankedLeaderboardScore>()
            : scores.Skip(start).Take(Math.Min(MaxLeaderboardQueryEntries, end - start + 1));
    }

    private static IEnumerable<RankedLeaderboardScore> SelectRequestedLeaderboardUsers(
        IReadOnlyList<RankedLeaderboardScore> scores,
        IEnumerable<ulong>? requestedUsers)
    {
        var requested = new HashSet<ulong>((requestedUsers ?? Array.Empty<ulong>()).Where(id => id != 0));
        return scores.Where(item => requested.Contains(item.Score.SteamId));
    }

    private static bool IsLeaderboardScoreBetter(int candidate, int current, int sortMethod) =>
        sortMethod == LeaderboardSortAscending ? candidate < current : candidate > current;

    private static int FindLeaderboardRank(IReadOnlyList<LeaderboardScoreRecord> scores, ulong steamId)
    {
        for (var index = 0; index < scores.Count; index++)
        {
            if (scores[index].SteamId == steamId)
            {
                return index + 1;
            }
        }

        return 0;
    }

    private static ApiLeaderboard MapLeaderboard(SteamDbContext db, LeaderboardRecord record) =>
        MapLeaderboard(
            record,
            db.LeaderboardScores.Count(item => item.LeaderboardId == record.Id));

    private static ApiLeaderboard MapLeaderboard(LeaderboardRecord record, int entryCount) => new()
    {
        Id = unchecked((ulong)record.Id),
        AppId = record.AppId,
        Name = record.Name,
        SortMethod = record.SortMethod,
        DisplayType = record.DisplayType,
        EntryCount = entryCount
    };

    private static ApiLeaderboardEntry MapLeaderboardEntry(RankedLeaderboardScore ranked)
    {
        List<int> details;
        try
        {
            details = JsonSerializer.Deserialize<List<int>>(ranked.Score.DetailsJson) ?? new List<int>();
        }
        catch (JsonException)
        {
            details = new List<int>();
        }

        return new ApiLeaderboardEntry
        {
            SteamId = ranked.Score.SteamId,
            GlobalRank = ranked.Rank,
            Score = ranked.Score.Score,
            Details = details.Take(MaxLeaderboardDetails).ToList(),
            UgcHandle = ranked.Score.UgcHandle
        };
    }

    private sealed record RankedLeaderboardScore(LeaderboardScoreRecord Score, int Rank);
}

using Microsoft.EntityFrameworkCore;
using SKYNET_server.Models;
using SKYNET_server.Persistence.Entities;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    private const int MaxWorkshopTitleLength = 128;
    private const int MaxWorkshopDescriptionLength = 7999;
    private const int MaxWorkshopTagsLength = 1024;
    private const int MaxWorkshopFileNameLength = 259;
    private const int MaxWorkshopMetadataLength = 4096;
    private const int MaxWorkshopUrlLength = 255;

    public List<ApiWorkshopSubscription>? GetWorkshopSubscriptions(string token)
    {
        if (!TryResolveCurrentSessionIdentity(token, out var steamId, out var appId))
        {
            return null;
        }

        return GetWorkshopSubscriptionsLocked(steamId, appId);
    }

    public ApiWorkshopItem? GetWorkshopItem(string token, ulong publishedFileId)
    {
        if (publishedFileId == 0 ||
            !TryResolveCurrentSessionIdentity(token, out _, out var appId))
        {
            return null;
        }

        using var db = _steamDbFactory.CreateDbContext();
        var item = db.WorkshopItems.AsNoTracking().SingleOrDefault(record =>
            record.PublishedFileId == publishedFileId &&
            record.ConsumerAppId == appId);
        return item == null ? null : MapWorkshopItem(item);
    }

    public ApiWorkshopItem? PutWorkshopItem(string token, ulong publishedFileId, ApiWorkshopItem payload)
    {
        if (publishedFileId == 0 ||
            payload == null ||
            !TryResolveCurrentSessionIdentity(token, out var steamId, out var appId))
        {
            return null;
        }

        using var db = _steamDbFactory.CreateDbContext();
        var record = db.WorkshopItems.SingleOrDefault(item => item.PublishedFileId == publishedFileId);
        if (record != null &&
            (record.OwnerSteamId != steamId || record.ConsumerAppId != appId))
        {
            return null;
        }

        var now = checked((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        if (record == null)
        {
            record = new WorkshopItemRecord
            {
                PublishedFileId = publishedFileId,
                ConsumerAppId = appId,
                OwnerSteamId = steamId,
                TimeCreated = payload.TimeCreated != 0 ? payload.TimeCreated : now
            };
            db.WorkshopItems.Add(record);
        }

        record.CreatorAppId = payload.CreatorAppId != 0 ? payload.CreatorAppId : appId;
        record.ConsumerAppId = appId;
        record.OwnerSteamId = steamId;
        record.FileType = payload.FileType;
        record.Title = NormalizeWorkshopText(payload.Title, MaxWorkshopTitleLength);
        record.Description = NormalizeWorkshopText(payload.Description, MaxWorkshopDescriptionLength);
        record.Tags = NormalizeWorkshopText(payload.Tags, MaxWorkshopTagsLength);
        record.FileName = NormalizeWorkshopText(payload.FileName, MaxWorkshopFileNameLength);
        record.Metadata = NormalizeWorkshopText(payload.Metadata, MaxWorkshopMetadataLength);
        record.PreviewUrl = NormalizeWorkshopText(payload.PreviewUrl, MaxWorkshopUrlLength);
        record.Visibility = Math.Clamp(payload.Visibility, 0, 3);
        record.Banned = payload.Banned;
        record.AcceptedForUse = payload.AcceptedForUse;
        record.TimeUpdated = payload.TimeUpdated != 0 ? payload.TimeUpdated : now;
        record.FileSize = Math.Max(0, payload.FileSize);
        record.TotalFilesSize = Math.Max(record.FileSize, payload.TotalFilesSize);
        record.VotesUp = payload.VotesUp;
        record.VotesDown = payload.VotesDown;
        record.Score = float.IsFinite(payload.Score) ? Math.Clamp(payload.Score, 0f, 1f) : 0f;
        db.SaveChanges();
        return MapWorkshopItem(record);
    }

    public ApiWorkshopMutationResult SubscribeWorkshopItem(string token, ulong publishedFileId)
    {
        if (publishedFileId == 0 ||
            !TryResolveCurrentSessionIdentity(token, out var steamId, out var appId))
        {
            return new ApiWorkshopMutationResult();
        }

        using var db = _steamDbFactory.CreateDbContext();
        var item = db.WorkshopItems.SingleOrDefault(record =>
            record.PublishedFileId == publishedFileId &&
            record.ConsumerAppId == appId);
        if (item == null || item.Banned)
        {
            return new ApiWorkshopMutationResult();
        }

        var subscription = db.WorkshopSubscriptions.SingleOrDefault(record =>
            record.SteamId == steamId &&
            record.AppId == appId &&
            record.PublishedFileId == publishedFileId);
        if (subscription == null)
        {
            subscription = new WorkshopSubscriptionRecord
            {
                SteamId = steamId,
                AppId = appId,
                PublishedFileId = publishedFileId,
                SubscribedAtUtc = DateTime.UtcNow
            };
            db.WorkshopSubscriptions.Add(subscription);
            db.SaveChanges();
        }

        return new ApiWorkshopMutationResult
        {
            Success = true,
            Subscription = MapWorkshopSubscription(subscription, item)
        };
    }

    public ApiWorkshopMutationResult UnsubscribeWorkshopItem(string token, ulong publishedFileId)
    {
        if (publishedFileId == 0 ||
            !TryResolveCurrentSessionIdentity(token, out var steamId, out var appId))
        {
            return new ApiWorkshopMutationResult();
        }

        using var db = _steamDbFactory.CreateDbContext();
        var subscription = db.WorkshopSubscriptions.SingleOrDefault(record =>
            record.SteamId == steamId &&
            record.AppId == appId &&
            record.PublishedFileId == publishedFileId);
        if (subscription != null)
        {
            db.WorkshopSubscriptions.Remove(subscription);
            db.SaveChanges();
        }

        return new ApiWorkshopMutationResult { Success = true };
    }

    private List<ApiWorkshopSubscription> GetWorkshopSubscriptionsLocked(ulong steamId, uint appId)
    {
        using var db = _steamDbFactory.CreateDbContext();
        var query =
            from subscription in db.WorkshopSubscriptions.AsNoTracking()
            join item in db.WorkshopItems.AsNoTracking()
                on subscription.PublishedFileId equals item.PublishedFileId
            where subscription.SteamId == steamId &&
                  subscription.AppId == appId &&
                  item.ConsumerAppId == appId
            orderby subscription.SubscribedAtUtc, subscription.PublishedFileId
            select new { Subscription = subscription, Item = item };

        return query
            .AsEnumerable()
            .Select(row => MapWorkshopSubscription(row.Subscription, row.Item))
            .ToList();
    }

    private static ApiWorkshopSubscription MapWorkshopSubscription(
        WorkshopSubscriptionRecord subscription,
        WorkshopItemRecord item) => new()
    {
        PublishedFileId = subscription.PublishedFileId,
        SubscribedAtUtc = subscription.SubscribedAtUtc,
        DisabledLocally = subscription.DisabledLocally,
        Item = MapWorkshopItem(item)
    };

    private static ApiWorkshopItem MapWorkshopItem(WorkshopItemRecord record) => new()
    {
        PublishedFileId = record.PublishedFileId,
        CreatorAppId = record.CreatorAppId,
        ConsumerAppId = record.ConsumerAppId,
        OwnerSteamId = record.OwnerSteamId,
        FileType = record.FileType,
        Title = record.Title,
        Description = record.Description,
        Tags = record.Tags,
        FileName = record.FileName,
        Metadata = record.Metadata,
        PreviewUrl = record.PreviewUrl,
        Visibility = record.Visibility,
        Banned = record.Banned,
        AcceptedForUse = record.AcceptedForUse,
        TimeCreated = record.TimeCreated,
        TimeUpdated = record.TimeUpdated,
        FileSize = record.FileSize,
        TotalFilesSize = record.TotalFilesSize,
        VotesUp = record.VotesUp,
        VotesDown = record.VotesDown,
        Score = record.Score
    };

    private static string NormalizeWorkshopText(string? value, int maxLength)
    {
        var normalized = (value ?? string.Empty).Replace("\0", string.Empty).Trim();
        return normalized.Length <= maxLength ? normalized : normalized[..maxLength];
    }
}

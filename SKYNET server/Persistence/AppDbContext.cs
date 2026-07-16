using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SKYNET_server.Persistence.Entities;

namespace SKYNET_server.Persistence;

/// <summary>
/// Single embedded SQLite store for all durable server state. Replaces the
/// monolithic api-state.json and the per-feature SQLite stores. Ephemeral
/// data (presence, in-flight game sessions, queues) is not persisted here.
/// </summary>
public sealed class AppDbContext : DbContext
{
    private static readonly JsonSerializerOptions JsonOpts = new();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UserRecord> Users => Set<UserRecord>();
    public DbSet<FriendEdge> Friends => Set<FriendEdge>();
    public DbSet<FriendRequestRecord> FriendRequests => Set<FriendRequestRecord>();
    public DbSet<AvatarRecord> Avatars => Set<AvatarRecord>();
    public DbSet<StatRecord> Stats => Set<StatRecord>();
    public DbSet<AchievementRecord> Achievements => Set<AchievementRecord>();
    public DbSet<WebAccountRecord> WebAccounts => Set<WebAccountRecord>();
    public DbSet<WebSessionRecord> WebSessions => Set<WebSessionRecord>();
    public DbSet<GameServerRecord> GameServers => Set<GameServerRecord>();
    public DbSet<LobbyRecord> Lobbies => Set<LobbyRecord>();
    public DbSet<RemoteFileRecord> RemoteFiles => Set<RemoteFileRecord>();
    public DbSet<RemoteFileShareRecord> RemoteFileShares => Set<RemoteFileShareRecord>();
    public DbSet<DotaItemRecord> DotaItems => Set<DotaItemRecord>();
    public DbSet<DotaEquipmentRecord> DotaEquipment => Set<DotaEquipmentRecord>();
    public DbSet<DotaMatchRecord> DotaMatches => Set<DotaMatchRecord>();
    public DbSet<DotaMatchPlayerRecord> DotaMatchPlayers => Set<DotaMatchPlayerRecord>();
    public DbSet<CosmeticSettingsRecord> CosmeticSettings => Set<CosmeticSettingsRecord>();
    public DbSet<DotaHeroIdRecord> DotaHeroIds => Set<DotaHeroIdRecord>();
    public DbSet<DotaHeroSlotRecord> DotaHeroSlots => Set<DotaHeroSlotRecord>();
    public DbSet<AppStateRecord> AppState => Set<AppStateRecord>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // SQLite stores INTEGER as signed 64-bit; map ulong through long so Steam
        // IDs and other 64-bit unsigned values round-trip without EF warnings.
        configurationBuilder.Properties<ulong>().HaveConversion<UlongToLongConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRecord>().HasKey(x => x.SteamId);
        modelBuilder.Entity<FriendEdge>().HasKey(x => new { x.SteamId, x.FriendSteamId });
        modelBuilder.Entity<FriendRequestRecord>().HasKey(x => x.Id);
        modelBuilder.Entity<AvatarRecord>().HasKey(x => x.SteamId);
        modelBuilder.Entity<StatRecord>().HasKey(x => new { x.SteamId, x.Name });
        modelBuilder.Entity<AchievementRecord>().HasKey(x => new { x.SteamId, x.Name });
        modelBuilder.Entity<WebAccountRecord>().HasKey(x => x.Username);
        modelBuilder.Entity<WebSessionRecord>().HasKey(x => x.AccessToken);
        modelBuilder.Entity<RemoteFileRecord>().HasKey(x => new { x.OwnerSteamId, x.AppId, x.NormalizedName });
        modelBuilder.Entity<RemoteFileShareRecord>().HasKey(x => x.Handle);
        modelBuilder.Entity<RemoteFileShareRecord>().Property(x => x.Handle).ValueGeneratedNever();
        modelBuilder.Entity<DotaItemRecord>().HasKey(x => x.DefIndex);
        modelBuilder.Entity<DotaEquipmentRecord>().HasKey(x => new { x.SteamId, x.HeroId, x.SlotId });
        modelBuilder.Entity<CosmeticSettingsRecord>().HasKey(x => x.Id);
        modelBuilder.Entity<DotaHeroIdRecord>().HasKey(x => x.Name);
        modelBuilder.Entity<DotaHeroSlotRecord>().HasKey(x => new { x.HeroName, x.SlotName });
        modelBuilder.Entity<AppStateRecord>().HasKey(x => x.Id);

        // Secondary indexes for the non-key columns we look rows up by.
        modelBuilder.Entity<WebAccountRecord>().HasIndex(x => x.SteamId);
        modelBuilder.Entity<WebSessionRecord>().HasIndex(x => x.SteamId);
        modelBuilder.Entity<FriendRequestRecord>().HasIndex(x => x.ToSteamId);
        modelBuilder.Entity<FriendRequestRecord>().HasIndex(x => x.FromSteamId);
        modelBuilder.Entity<DotaMatchPlayerRecord>().HasIndex(x => x.SteamId);

        modelBuilder.Entity<GameServerRecord>(b =>
        {
            b.HasKey(x => x.SteamId);
            b.Property(x => x.KeyValues).HasJsonConversion();
        });

        modelBuilder.Entity<LobbyRecord>(b =>
        {
            b.HasKey(x => x.SteamId);
            b.Property(x => x.LobbyData).HasJsonConversion();
            b.Property(x => x.Members).HasJsonConversion();
            b.Property(x => x.GameServer).HasJsonConversion();
        });

        modelBuilder.Entity<DotaItemRecord>(b =>
        {
            b.Property(x => x.HeroNames).HasJsonConversion();
            b.Property(x => x.HeroIds).HasJsonConversion();
        });

        modelBuilder.Entity<DotaMatchRecord>(b =>
        {
            b.HasKey(x => x.LobbyId);
            b.HasMany(x => x.Players)
                .WithOne(x => x.Match!)
                .HasForeignKey(x => x.LobbyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DotaMatchPlayerRecord>(b =>
        {
            b.HasKey(x => new { x.LobbyId, x.SteamId });
            b.Property(x => x.Equipment).HasJsonConversion();
        });
    }

    private sealed class UlongToLongConverter : ValueConverter<ulong, long>
    {
        public UlongToLongConverter()
            : base(v => unchecked((long)v), v => unchecked((ulong)v))
        {
        }
    }

    internal static ValueConverter<T, string> JsonConverter<T>() => new(
        v => JsonSerializer.Serialize(v, JsonOpts),
        v => JsonSerializer.Deserialize<T>(v, JsonOpts)!);

    internal static ValueComparer<T> JsonComparer<T>() => new(
        (a, b) => JsonSerializer.Serialize(a, JsonOpts) == JsonSerializer.Serialize(b, JsonOpts),
        v => v == null ? 0 : JsonSerializer.Serialize(v, JsonOpts).GetHashCode(),
        v => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(v, JsonOpts), JsonOpts)!);
}

internal static class PropertyBuilderJsonExtensions
{
    /// <summary>Stores a value object/collection as a JSON text column.</summary>
    public static Microsoft.EntityFrameworkCore.Metadata.Builders.PropertyBuilder<T> HasJsonConversion<T>(
        this Microsoft.EntityFrameworkCore.Metadata.Builders.PropertyBuilder<T> builder)
    {
        builder.HasConversion(AppDbContext.JsonConverter<T>(), AppDbContext.JsonComparer<T>());
        return builder;
    }
}

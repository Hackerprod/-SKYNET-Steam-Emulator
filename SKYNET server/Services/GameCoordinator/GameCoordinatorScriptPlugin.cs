using SKYNET_server.Models;
using SKYNET_server.GC.Dota2;
using TypeSharp.Hosting;
using TypeSharp.VM.Memory;

namespace SKYNET_server.Services;

public sealed class GameCoordinatorScriptPlugin : IGameCoordinatorPlugin, IGameCoordinatorTicker
{
    private readonly ILogger<GameCoordinatorScriptPlugin> _logger;
    private readonly GameCoordinatorTraceService _trace;
    private readonly string _gcRoot;
    private readonly GameCoordinatorProtoCodec _codec = new();
    private readonly object _cacheSync = new();
    private readonly Dictionary<uint, CachedScript> _cache = new();

    public GameCoordinatorScriptPlugin(
        IHostEnvironment hostEnvironment,
        ILogger<GameCoordinatorScriptPlugin> logger,
        GameCoordinatorTraceService trace)
    {
        _logger = logger;
        _trace = trace;
        _gcRoot = ResolveGcRoot(hostEnvironment.ContentRootPath);
        _logger.LogInformation("GC script root resolved to {GCRoot}", _gcRoot);
    }

    public bool CanHandle(uint appId)
    {
        return File.Exists(GetMainScriptPath(appId));
    }

    public ApiGCExchangeResponse Exchange(GameCoordinatorContext context, ApiGCExchangeRequest request)
    {
        var scriptRoot = GetScriptRoot(context.AppId);
        var scriptPath = Path.Combine(scriptRoot, "main.ts");
        if (!File.Exists(scriptPath))
        {
            return new ApiGCExchangeResponse { Handled = false };
        }

        _trace.Record("in", context.AppId, context.SteamId, request.MessageType,
            GameCoordinatorTraceService.EstimatePayloadSize(request.BodyBase64), context.PersonaName);

        try
        {
            var cacheEntry = GetCachedScript(context.AppId, scriptRoot, scriptPath);
            lock (cacheEntry.Sync)
            {
                var host = new ScriptExchangeHost(context, request, _codec, _logger, _trace);
                cacheEntry.Dispatcher.Current = host;
                try
                {
                    var handled = cacheEntry.Runtime
                        .InvokeAsync<bool>(context.AppId.ToString(), "handle")
                        .GetAwaiter()
                        .GetResult();
                    if (!handled)
                    {
                        _trace.Record("unhandled", context.AppId, context.SteamId, request.MessageType, 0);
                        return new ApiGCExchangeResponse { Handled = false };
                    }

                    foreach (var message in host.Response.Messages)
                    {
                        _trace.Record("out", context.AppId, context.SteamId, message.MessageType,
                            GameCoordinatorTraceService.EstimatePayloadSize(message.PayloadBase64),
                            message.TargetJobId == null ? string.Empty : $"job {message.TargetJobId}");
                    }

                    return host.Response;
                }
                finally
                {
                    cacheEntry.Dispatcher.Current = null;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GC script failed for app {AppId}, message {MessageType}",
                context.AppId, request.MessageType);
            _trace.Record("error", context.AppId, context.SteamId, request.MessageType, 0, ex.Message);
            return new ApiGCExchangeResponse { Handled = false };
        }
    }

    public ApiGCExchangeResponse Poll(GameCoordinatorContext context)
    {
        var response = new ApiGCExchangeResponse { Handled = true };
        response.Messages.AddRange(GameCoordinatorPendingMessages.Drain(context.AppId, context.SteamId));
        return response;
    }

    public void Tick()
    {
        List<uint> appIds;
        lock (_cacheSync)
        {
            appIds = _cache.Keys.ToList();
        }

        foreach (var appId in appIds)
        {
            var scriptRoot = GetScriptRoot(appId);
            var scriptPath = Path.Combine(scriptRoot, "main.ts");
            if (!File.Exists(scriptPath))
            {
                continue;
            }

            try
            {
                var cacheEntry = GetCachedScript(appId, scriptRoot, scriptPath);
                lock (cacheEntry.Sync)
                {
                    var host = new ScriptExchangeHost(
                        new GameCoordinatorContext { AppId = appId },
                        new ApiGCExchangeRequest { AppId = appId },
                        _codec,
                        _logger,
                        _trace);
                    cacheEntry.Dispatcher.Current = host;
                    try
                    {
                        cacheEntry.Runtime
                            .InvokeAsync<object?>(appId.ToString(), "tick")
                            .GetAwaiter()
                            .GetResult();
                    }
                    catch (InvalidOperationException ex) when (ex.Message.Contains("Function 'tick' not found", StringComparison.Ordinal))
                    {
                    }
                    finally
                    {
                        cacheEntry.Dispatcher.Current = null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GC script tick failed for app {AppId}", appId);
                _trace.Record("error", appId, 0, 0, 0, $"tick: {ex.Message}");
            }
        }
    }

    private CachedScript GetCachedScript(uint appId, string scriptRoot, string scriptPath)
    {
        var latestWriteUtc = GetScriptTreeStamp(scriptRoot);
        lock (_cacheSync)
        {
            if (_cache.TryGetValue(appId, out var cached) && cached.LastWriteUtc == latestWriteUtc)
            {
                return cached;
            }

            var dispatcher = new ScriptHostDispatcher();
            var builder = new TypeSharpRuntimeBuilder()
                .ConfigureLimits(options =>
                {
                    options.MaximumInstructions = 25_000_000;
                    options.MaximumMemoryBytes = 256 * 1024 * 1024;
                })
                .RegisterHostFunction("gc", "messageType", _ => dispatcher.RequireCurrent().MessageType())
                .RegisterHostFunction("gc", "body", _ => dispatcher.RequireCurrent().Body())
                .RegisterHostFunction("gc", "appId", _ => dispatcher.RequireCurrent().AppId())
                .RegisterHostFunction("gc", "steamId", _ => dispatcher.RequireCurrent().SteamId())
                .RegisterHostFunction("gc", "accountId", _ => dispatcher.RequireCurrent().AccountId())
                .RegisterHostFunction("gc", "personaName", _ => dispatcher.RequireCurrent().PersonaName())
                .RegisterHostFunction("gc", "now", _ => dispatcher.RequireCurrent().Now())
                .RegisterHostFunction("gc", "log", dispatcher.Log)
                .RegisterHostFunction("gc", "decode", dispatcher.Decode)
                .RegisterHostFunction("gc", "encode", dispatcher.Encode)
                .RegisterHostFunction("gc", "send", dispatcher.Send)
                .RegisterHostFunction("gc", "dotaInventory", _ => dispatcher.RequireCurrent().DotaInventory())
                .RegisterHostFunction("gc", "dotaCatalogItem", dispatcher.DotaCatalogItem)
                .RegisterHostFunction("gc", "dotaEquipItem", dispatcher.DotaEquipItem)
                .RegisterHostFunction("gc", "dotaSetItemStyle", dispatcher.DotaSetItemStyle)
                .RegisterHostFunction("gc", "dotaQueueCurrentLobbyServer", dispatcher.DotaQueueCurrentLobbyServer)
                .RegisterHostFunction("gc", "dotaProfile", dispatcher.DotaProfile)
                .RegisterHostFunction("gc", "dotaSaveProfileSlots", dispatcher.DotaSaveProfileSlots)
                .RegisterHostFunction("gc", "dotaSaveProfileUpdate", dispatcher.DotaSaveProfileUpdate);

            foreach (var sourceFile in EnumerateRuntimeScriptFiles(scriptRoot))
            {
                builder.AddSourceFile(sourceFile);
            }

            var runtime = builder.BuildAsync().GetAwaiter().GetResult();
            if (_cache.TryGetValue(appId, out var old))
            {
                old.Runtime.DisposeAsync().AsTask().GetAwaiter().GetResult();
            }

            cached = new CachedScript(runtime, dispatcher, latestWriteUtc);
            _cache[appId] = cached;
            _logger.LogInformation("GC script loaded for app {AppId} from {ScriptRoot}", appId, scriptRoot);
            return cached;
        }
    }

    private static DateTime GetScriptTreeStamp(string scriptRoot)
    {
        return EnumerateRuntimeScriptFiles(scriptRoot)
            .Select(File.GetLastWriteTimeUtc)
            .DefaultIfEmpty(DateTime.MinValue)
            .Max();
    }

    private static IEnumerable<string> EnumerateRuntimeScriptFiles(string scriptRoot)
    {
        var main = Path.Combine(scriptRoot, "main.ts");
        if (File.Exists(main))
        {
            yield return main;
        }

        foreach (var directoryName in new[] { "framework", "generated", "modules" })
        {
            var directory = Path.Combine(scriptRoot, directoryName);
            if (!Directory.Exists(directory))
            {
                continue;
            }

            foreach (var file in Directory.EnumerateFiles(directory, "*.ts", SearchOption.AllDirectories)
                         .Where(path => !path.EndsWith(".d.ts", StringComparison.OrdinalIgnoreCase)))
            {
                yield return file;
            }
        }
    }

    private string GetMainScriptPath(uint appId)
    {
        return Path.Combine(GetScriptRoot(appId), "main.ts");
    }

    private string GetScriptRoot(uint appId)
    {
        return Path.Combine(_gcRoot, appId.ToString());
    }

    private static string ResolveGcRoot(string contentRootPath)
    {
        var configuredRoot = Environment.GetEnvironmentVariable("SKYNET_GC_ROOT");
        if (IsValidGcRoot(configuredRoot))
        {
            return Path.GetFullPath(configuredRoot!);
        }

        var current = new DirectoryInfo(contentRootPath);
        while (current != null)
        {
            var candidate = Path.Combine(current.FullName, "GC");
            if (IsValidGcRoot(candidate))
            {
                return candidate;
            }

            current = current.Parent;
        }

        return Path.Combine(contentRootPath, "GC");
    }

    private static bool IsValidGcRoot(string? path)
    {
        return !string.IsNullOrWhiteSpace(path)
            && File.Exists(Path.Combine(path, "570", "main.ts"));
    }

    private sealed class CachedScript
    {
        public CachedScript(TypeSharpRuntime runtime, ScriptHostDispatcher dispatcher, DateTime lastWriteUtc)
        {
            Runtime = runtime;
            Dispatcher = dispatcher;
            LastWriteUtc = lastWriteUtc;
        }

        public TypeSharpRuntime Runtime { get; }
        public ScriptHostDispatcher Dispatcher { get; }
        public DateTime LastWriteUtc { get; }
        public object Sync { get; } = new();
    }
}

internal sealed class ScriptHostDispatcher
{
    public ScriptExchangeHost? Current { get; set; }

    public ScriptExchangeHost RequireCurrent()
    {
        return Current ?? throw new InvalidOperationException("GC script host is not bound to an exchange");
    }

    public TsValue? Log(TsValue[] args)
    {
        return RequireCurrent().Log(args);
    }

    public TsValue? Decode(TsValue[] args)
    {
        return RequireCurrent().Decode(args);
    }

    public TsValue? Encode(TsValue[] args)
    {
        return RequireCurrent().Encode(args);
    }

    public TsValue? Send(TsValue[] args)
    {
        return RequireCurrent().Send(args);
    }

    public TsValue? DotaEquipItem(TsValue[] args)
    {
        return RequireCurrent().DotaEquipItem(args);
    }

    public TsValue? DotaCatalogItem(TsValue[] args)
    {
        return RequireCurrent().DotaCatalogItem(args);
    }

    public TsValue? DotaSetItemStyle(TsValue[] args)
    {
        return RequireCurrent().DotaSetItemStyle(args);
    }

    public TsValue? DotaQueueCurrentLobbyServer(TsValue[] args)
    {
        return RequireCurrent().DotaQueueCurrentLobbyServer(args);
    }

    public TsValue? DotaProfile(TsValue[] args)
    {
        return RequireCurrent().DotaProfile(args);
    }

    public TsValue? DotaSaveProfileSlots(TsValue[] args)
    {
        return RequireCurrent().DotaSaveProfileSlots(args);
    }

    public TsValue? DotaSaveProfileUpdate(TsValue[] args)
    {
        return RequireCurrent().DotaSaveProfileUpdate(args);
    }
}

internal sealed class ScriptExchangeHost
{
    private readonly GameCoordinatorContext _context;
    private readonly ApiGCExchangeRequest _request;
    private readonly GameCoordinatorProtoCodec _codec;
    private readonly ILogger _logger;

    public ScriptExchangeHost(
        GameCoordinatorContext context,
        ApiGCExchangeRequest request,
        GameCoordinatorProtoCodec codec,
        ILogger logger,
        GameCoordinatorTraceService trace)
    {
        _context = context;
        _request = request;
        _codec = codec;
        _logger = logger;
        Response = new ApiGCExchangeResponse { Handled = true };
    }

    public ApiGCExchangeResponse Response { get; }

    public TsValue MessageType() => TsValue.FromInt32(unchecked((int)_request.MessageType));

    public TsValue Body()
    {
        return ToArray(Convert.FromBase64String(_request.BodyBase64 ?? string.Empty));
    }

    public TsValue AppId() => TsValue.FromInt32(unchecked((int)_context.AppId));

    public TsValue SteamId() => TsValue.FromUInt64(_context.SteamId);

    public TsValue AccountId() => TsValue.FromInt32(unchecked((int)_context.AccountId));

    public TsValue PersonaName() => TsValue.FromString(_context.PersonaName);

    public TsValue Now() => TsValue.FromInt64(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

    public TsValue? Log(TsValue[] args)
    {
        var message = args.Length > 0 ? ToString(args[0]) : string.Empty;
        _logger.LogInformation("GC script {AppId}: {Message}", _context.AppId, message);
        return TsValue.Void;
    }

    public TsValue? Decode(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("decode(typeName, payload) requires two arguments");
        }

        return _codec.Decode(ToString(args[0]), ToBytes(args[1]));
    }

    public TsValue? Encode(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("encode(typeName, value) requires two arguments");
        }

        return ToArray(_codec.Encode(ToString(args[0]), args[1]));
    }

    public TsValue? Send(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("send(messageType, payload, protobuf?) requires at least two arguments");
        }

        var messageType = Convert.ToUInt32(ToNumber(args[0], "send.messageType"));
        var payload = ToBytes(args[1], "send.payload");
        var protobuf = args.Length < 3 || args[2] is not TsBoolValue boolValue || boolValue.Value;
        Response.Messages.Add(new ApiGCMessage
        {
            AppId = _context.AppId,
            MessageType = messageType,
            PayloadBase64 = Convert.ToBase64String(payload),
            Protobuf = protobuf
        });
        return TsValue.FromBool(true);
    }

    public TsValue DotaInventory()
    {
        return ToTsInventory(DotaGcBackend.InventoryProvider?.Invoke(_context.SteamId));
    }

    public TsValue DotaCatalogItem(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaCatalogItem(defIndex) requires one argument");
        }

        var defIndex = Convert.ToUInt32(ToNumber(args[0], "dotaCatalogItem.defIndex"));
        var catalog = DotaGcBackend.InventoryProvider?.Invoke(_context.SteamId);
        var item = catalog?.Items.FirstOrDefault(candidate => candidate.DefIndex == defIndex);
        return item == null ? TsValue.Null : ToTsCatalogItem(item);
    }

    public TsValue DotaEquipItem(TsValue[] args)
    {
        if (args.Length < 4)
        {
            throw new InvalidOperationException("dotaEquipItem(itemId, heroId, slotId, style) requires four arguments");
        }

        var itemId = Convert.ToUInt64(ToInteger(args[0], "dotaEquipItem.itemId").ToString());
        var heroId = Convert.ToUInt32(ToNumber(args[1], "dotaEquipItem.heroId"));
        var slotId = Convert.ToUInt32(ToNumber(args[2], "dotaEquipItem.slotId"));
        var style = Convert.ToUInt32(ToNumber(args[3], "dotaEquipItem.style"));
        var changed = DotaGcBackend.EquipItemSink?.Invoke(_context.SteamId, itemId, heroId, slotId, style)
            ?? new List<ApiDotaEquipment>();
        return ToTsEquipmentList(changed);
    }

    public TsValue DotaSetItemStyle(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaSetItemStyle(itemId, style) requires two arguments");
        }

        var itemId = Convert.ToUInt64(ToInteger(args[0], "dotaSetItemStyle.itemId").ToString());
        var style = Convert.ToUInt32(ToNumber(args[1], "dotaSetItemStyle.style"));
        var changed = DotaGcBackend.SetItemStyleSink?.Invoke(_context.SteamId, itemId, style)
            ?? new List<ApiDotaEquipment>();
        return ToTsEquipmentList(changed);
    }

    public TsValue DotaQueueCurrentLobbyServer(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaQueueCurrentLobbyServer(messageType, payload) requires two arguments");
        }

        var messageType = Convert.ToUInt32(ToNumber(args[0], "dotaQueueCurrentLobbyServer.messageType"));
        var payload = ToBytes(args[1], "dotaQueueCurrentLobbyServer.payload");
        return TsValue.FromBool(DotaGcBackend.QueueCurrentLobbyServerProto(_context.SteamId, _context.AppId, messageType, payload));
    }

    public TsValue DotaProfile(TsValue[] args)
    {
        var accountId = args.Length > 0
            ? Convert.ToUInt32(ToNumber(args[0], "dotaProfile.accountId"))
            : _context.AccountId;
        if (accountId == 0)
        {
            accountId = _context.AccountId;
        }

        return ToTsProfileSnapshot(accountId);
    }

    public TsValue DotaSaveProfileSlots(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaSaveProfileSlots(slots) requires one argument");
        }

        var slots = new List<DotaStatsProfileSlot>();
        if (args[0] is not TsArrayValue arrayValue)
        {
            throw new InvalidOperationException("dotaSaveProfileSlots.slots: expected array");
        }

        for (var i = 0; i < arrayValue.Value.Count; i++)
        {
            if (arrayValue.Value.Get(i) is not TsObjectValue slotValue)
            {
                throw new InvalidOperationException($"dotaSaveProfileSlots.slots[{i}]: expected object");
            }

            var slot = slotValue.Value;
            var slotId = Convert.ToUInt32(ToNumber(slot.GetField("slotId"), $"dotaSaveProfileSlots.slots[{i}].slotId"));
            if (slotId == 0)
            {
                continue;
            }

            slots.Add(new DotaStatsProfileSlot
            {
                AccountId = _context.AccountId,
                SlotId = slotId,
                SlotType = Convert.ToUInt32(ToNumber(slot.GetField("slotType"), $"dotaSaveProfileSlots.slots[{i}].slotType")),
                SlotValue = Convert.ToUInt64(ToInteger(slot.GetField("slotValue"), $"dotaSaveProfileSlots.slots[{i}].slotValue").ToString())
            });
        }

        DotaGcBackend.StatsStore?.SaveProfileSlots(_context.AccountId, slots);
        return TsValue.FromBool(true);
    }

    public TsValue DotaSaveProfileUpdate(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaSaveProfileUpdate(backgroundItemId, featuredHeroIds) requires two arguments");
        }

        var backgroundItemId = Convert.ToUInt64(ToInteger(args[0], "dotaSaveProfileUpdate.backgroundItemId").ToString());
        if (args[1] is not TsArrayValue arrayValue)
        {
            throw new InvalidOperationException("dotaSaveProfileUpdate.featuredHeroIds: expected array");
        }

        var featuredHeroIds = new List<uint>();
        for (var i = 0; i < arrayValue.Value.Count; i++)
        {
            var heroId = Convert.ToUInt32(ToNumber(arrayValue.Value.Get(i), $"dotaSaveProfileUpdate.featuredHeroIds[{i}]"));
            if (heroId != 0)
            {
                featuredHeroIds.Add(heroId);
            }
        }

        DotaGcBackend.StatsStore?.SaveProfileUpdate(_context.AccountId, backgroundItemId, featuredHeroIds);
        return TsValue.FromBool(true);
    }

    private TsValue ToTsProfileSnapshot(uint requestedAccountId)
    {
        var store = DotaGcBackend.StatsStore;
        var profile = store == null
            ? DefaultProfile(requestedAccountId)
            : requestedAccountId == _context.AccountId
                ? store.EnsureProfile(_context.SteamId, _context.AccountId, _context.PersonaName)
                : store.GetProfile(requestedAccountId);

        var global = store?.GetGlobalStats(profile.AccountId) ?? new DotaStatsGlobalStats { AccountId = profile.AccountId };
        var conduct = store?.GetConduct(profile.AccountId) ?? new DotaStatsConduct { AccountId = profile.AccountId, RawBehaviorScore = 10000 };
        var value = new TsObject("DotaProfileSnapshot");
        value.SetField("accountId", TsValue.FromInt32(unchecked((int)profile.AccountId)));
        value.SetField("steamId", TsValue.FromUInt64(profile.SteamId));
        value.SetField("personaName", TsValue.FromString(profile.PersonaName ?? string.Empty));
        value.SetField("rankTier", TsValue.FromInt32(unchecked((int)profile.RankTier)));
        value.SetField("rankTierScore", TsValue.FromInt32(unchecked((int)profile.RankTierScore)));
        value.SetField("leaderboardRank", TsValue.FromInt32(unchecked((int)profile.LeaderboardRank)));
        value.SetField("badgePoints", TsValue.FromInt32(unchecked((int)profile.BadgePoints)));
        value.SetField("eventPoints", TsValue.FromInt32(unchecked((int)profile.EventPoints)));
        value.SetField("activeEventId", TsValue.FromInt32(57));
        value.SetField("lifetimeGames", TsValue.FromInt32(unchecked((int)profile.LifetimeGames)));
        value.SetField("level", TsValue.FromInt32(unchecked((int)profile.Level)));
        value.SetField("isPlusSubscriber", TsValue.FromBool(false));
        value.SetField("plusOriginalStartDate", TsValue.FromInt32(0));
        value.SetField("firstMatchTime", TsValue.FromInt32(unchecked((int)(store?.GetFirstMatchTime(profile.AccountId) ?? 0))));
        value.SetField("mvpCount", TsValue.FromInt32(unchecked((int)(store?.GetMvpCount(profile.AccountId) ?? 0))));
        value.SetField("globalStats", ToTsGlobalStats(global));
        value.SetField("conduct", ToTsConduct(conduct));
        value.SetField("slots", ToTsProfileSlots(store?.GetProfileSlots(profile.AccountId) ?? Array.Empty<DotaStatsProfileSlot>()));
        value.SetField("heroes", ToTsHeroStats(store?.GetHeroStandings(profile.AccountId) ?? Array.Empty<DotaStatsHeroStats>()));
        value.SetField("trophies", ToTsTrophies(store?.GetTrophies(profile.AccountId) ?? Array.Empty<DotaStatsTrophy>()));
        value.SetField("featuredHeroIds", ToTsUInt32Array(store?.GetFeaturedHeroes(profile.AccountId) ?? Array.Empty<uint>()));
        value.SetField("recentMatches", ToTsRecentMatches(store?.GetRecentMatches(profile.AccountId, 8) ?? Array.Empty<DotaStatsMatchPlayer>()));
        value.SetField("allHeroProgress", ToTsAllHeroProgress(store?.GetAllHeroProgress(profile.AccountId) ?? DefaultAllHeroProgress(profile.AccountId, profile.PersonaName ?? string.Empty)));
        return new TsObjectValue(value);
    }

    private static DotaStatsProfile DefaultProfile(uint accountId) => new()
    {
        AccountId = accountId,
        PersonaName = accountId == 0 ? string.Empty : $"User{accountId}",
        Level = 1
    };

    private static DotaStatsAllHeroProgress DefaultAllHeroProgress(uint accountId, string personaName) => new()
    {
        AccountId = accountId,
        HeroIds = new List<uint>(),
        ProfileName = personaName ?? string.Empty
    };

    private static TsValue ToTsGlobalStats(DotaStatsGlobalStats stats)
    {
        var value = new TsObject("DotaGlobalStats");
        value.SetField("gamesWon", TsValue.FromInt32(unchecked((int)stats.GamesWon)));
        value.SetField("gamesLost", TsValue.FromInt32(unchecked((int)stats.GamesLost)));
        value.SetField("matchCount", TsValue.FromInt32(unchecked((int)stats.MatchCount)));
        return new TsObjectValue(value);
    }

    private static TsValue ToTsConduct(DotaStatsConduct conduct)
    {
        var value = new TsObject("DotaConduct");
        value.SetField("commendCount", TsValue.FromInt32(unchecked((int)conduct.CommendCount)));
        value.SetField("rawBehaviorScore", TsValue.FromInt32(unchecked((int)conduct.RawBehaviorScore)));
        return new TsObjectValue(value);
    }

    private static TsValue ToTsProfileSlots(IEnumerable<DotaStatsProfileSlot> slots)
    {
        var array = new TsArray();
        foreach (var slot in slots)
        {
            var value = new TsObject("DotaProfileSlot");
            value.SetField("slotId", TsValue.FromInt32(unchecked((int)slot.SlotId)));
            value.SetField("slotType", TsValue.FromInt32(unchecked((int)slot.SlotType)));
            value.SetField("slotValue", TsValue.FromUInt64(slot.SlotValue));
            array.Add(new TsObjectValue(value));
        }

        return new TsArrayValue(array);
    }

    private static TsValue ToTsHeroStats(IEnumerable<DotaStatsHeroStats> heroes)
    {
        var array = new TsArray();
        foreach (var hero in heroes)
        {
            var value = new TsObject("DotaHeroStats");
            value.SetField("heroId", TsValue.FromInt32(unchecked((int)hero.HeroId)));
            value.SetField("wins", TsValue.FromInt32(unchecked((int)hero.Wins)));
            value.SetField("losses", TsValue.FromInt32(unchecked((int)hero.Losses)));
            value.SetField("bestWinStreak", TsValue.FromInt32(unchecked((int)hero.BestWinStreak)));
            array.Add(new TsObjectValue(value));
        }

        return new TsArrayValue(array);
    }

    private static TsValue ToTsTrophies(IEnumerable<DotaStatsTrophy> trophies)
    {
        var array = new TsArray();
        foreach (var trophy in trophies)
        {
            var value = new TsObject("DotaTrophy");
            value.SetField("trophyId", TsValue.FromInt32(unchecked((int)trophy.TrophyId)));
            value.SetField("trophyScore", TsValue.FromInt32(unchecked((int)trophy.TrophyScore)));
            value.SetField("lastUpdated", TsValue.FromInt32(unchecked((int)trophy.LastUpdated)));
            array.Add(new TsObjectValue(value));
        }

        return new TsArrayValue(array);
    }

    private static TsValue ToTsRecentMatches(IEnumerable<DotaStatsMatchPlayer> matches)
    {
        var array = new TsArray();
        foreach (var match in matches)
        {
            var value = new TsObject("DotaRecentMatch");
            value.SetField("matchId", TsValue.FromUInt64(match.MatchId));
            value.SetField("startTime", TsValue.FromInt32(unchecked((int)match.StartTime)));
            value.SetField("duration", TsValue.FromInt32(unchecked((int)match.Duration)));
            value.SetField("heroId", TsValue.FromInt32(unchecked((int)match.HeroId)));
            value.SetField("kills", TsValue.FromInt32(unchecked((int)match.Kills)));
            value.SetField("deaths", TsValue.FromInt32(unchecked((int)match.Deaths)));
            value.SetField("assists", TsValue.FromInt32(unchecked((int)match.Assists)));
            value.SetField("winner", TsValue.FromBool(match.Winner));
            value.SetField("gameMode", TsValue.FromInt32(unchecked((int)match.GameMode)));
            value.SetField("lobbyType", TsValue.FromInt32(unchecked((int)match.LobbyType)));
            value.SetField("playerSlot", TsValue.FromInt32(unchecked((int)match.PlayerSlot)));
            value.SetField("team", TsValue.FromInt32(unchecked((int)match.Team)));
            array.Add(new TsObjectValue(value));
        }

        return new TsArrayValue(array);
    }

    private static TsValue ToTsAllHeroProgress(DotaStatsAllHeroProgress progress)
    {
        var value = new TsObject("DotaAllHeroProgress");
        value.SetField("accountId", TsValue.FromInt32(unchecked((int)progress.AccountId)));
        value.SetField("heroIds", ToTsUInt32Array(progress.HeroIds));
        value.SetField("currentHeroId", TsValue.FromInt32(unchecked((int)progress.CurrentHeroId)));
        value.SetField("nextHeroId", TsValue.FromInt32(unchecked((int)progress.NextHeroId)));
        value.SetField("previousHeroId", TsValue.FromInt32(unchecked((int)progress.PreviousHeroId)));
        value.SetField("startHeroId", TsValue.FromInt32(unchecked((int)progress.StartHeroId)));
        value.SetField("lapsCompleted", TsValue.FromInt32(unchecked((int)progress.LapsCompleted)));
        value.SetField("currentHeroGames", TsValue.FromInt32(unchecked((int)progress.CurrentHeroGames)));
        value.SetField("currentLapStarted", TsValue.FromInt32(unchecked((int)progress.CurrentLapStarted)));
        value.SetField("currentLapGames", TsValue.FromInt32(unchecked((int)progress.CurrentLapGames)));
        value.SetField("bestLapGames", TsValue.FromInt32(unchecked((int)progress.BestLapGames)));
        value.SetField("bestLapTime", TsValue.FromInt32(unchecked((int)progress.BestLapTime)));
        value.SetField("lapHeroesCompleted", TsValue.FromInt32(unchecked((int)progress.LapHeroesCompleted)));
        value.SetField("lapHeroesRemaining", TsValue.FromInt32(unchecked((int)progress.LapHeroesRemaining)));
        value.SetField("previousHeroGames", TsValue.FromInt32(unchecked((int)progress.PreviousHeroGames)));
        value.SetField("profileName", TsValue.FromString(progress.ProfileName ?? string.Empty));
        return new TsObjectValue(value);
    }

    private static TsValue ToTsUInt32Array(IEnumerable<uint> values)
    {
        var array = new TsArray();
        foreach (var value in values)
        {
            array.Add(TsValue.FromInt32(unchecked((int)value)));
        }

        return new TsArrayValue(array);
    }

    private static TsValue ToArray(byte[] bytes)
    {
        var copy = new byte[bytes.Length];
        Array.Copy(bytes, copy, copy.Length);
        return new TsUint8ArrayValue(copy);
    }

    private static byte[] ToBytes(TsValue value, string path = "value")
    {
        if (value is TsStringValue stringValue)
        {
            return Convert.FromBase64String(stringValue.Value);
        }

        if (value is TsUint8ArrayValue bytesValue)
        {
            var copy = new byte[bytesValue.Length];
            Array.Copy(bytesValue.Value, copy, copy.Length);
            return copy;
        }

        if (value is not TsArrayValue arrayValue)
        {
            throw new InvalidOperationException($"{path}: expected Uint8Array, byte array, or base64 string; got {value.ValueType}");
        }

        var bytes = new byte[arrayValue.Value.Count];
        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(ToNumber(arrayValue.Value.Get(i), $"{path}[{i}]"));
        }

        return bytes;
    }

    private static string ToString(TsValue value)
    {
        return value is TsStringValue stringValue ? stringValue.Value : value.ToString() ?? string.Empty;
    }

    private static double ToNumber(TsValue value, string path = "value")
    {
        return value switch
        {
            TsInt32Value int32Value => int32Value.Value,
            TsInt64Value int64Value => int64Value.Value,
            TsUInt64Value uint64Value => uint64Value.Value,
            TsFloat32Value float32Value => float32Value.Value,
            TsFloat64Value float64Value => float64Value.Value,
            TsDecimalValue decimalValue => (double)decimalValue.Value,
            TsStringValue stringValue when double.TryParse(stringValue.Value, out var parsed) => parsed,
            _ => throw new InvalidOperationException($"{path}: expected numeric value, got {value.ValueType}")
        };
    }

    private static System.Numerics.BigInteger ToInteger(TsValue value, string path)
    {
        return value switch
        {
            TsInt32Value int32Value => int32Value.Value,
            TsInt64Value int64Value => int64Value.Value,
            TsUInt64Value uint64Value => uint64Value.Value,
            TsBigIntValue bigIntValue => bigIntValue.Value,
            TsFloat32Value float32Value => new System.Numerics.BigInteger(float32Value.Value),
            TsFloat64Value float64Value => new System.Numerics.BigInteger(float64Value.Value),
            TsDecimalValue decimalValue => new System.Numerics.BigInteger(decimalValue.Value),
            TsStringValue stringValue when System.Numerics.BigInteger.TryParse(stringValue.Value, out var parsed) => parsed,
            _ => throw new InvalidOperationException($"{path}: expected integer value, got {value.ValueType}")
        };
    }

    private static TsValue ToTsInventory(ApiDotaRuntimeInventory? inventory)
    {
        inventory ??= new ApiDotaRuntimeInventory();

        var value = new TsObject("DotaRuntimeInventory");
        value.SetField("steamId", TsValue.FromUInt64(inventory.SteamId));
        value.SetField("version", TsValue.FromUInt64(inventory.Version));
        value.SetField("ownedItems", ToTsCatalogItems(inventory.OwnedItems));
        value.SetField("equipment", ToTsEquipmentList(inventory.Equipment));
        return new TsObjectValue(value);
    }

    private static TsValue ToTsCatalogItems(IEnumerable<ApiDotaItem> items)
    {
        var array = new TsArray();
        foreach (var item in items)
        {
            array.Add(ToTsCatalogItem(item));
        }

        return new TsArrayValue(array);
    }

    private static TsObjectValue ToTsCatalogItem(ApiDotaItem item)
    {
        var value = new TsObject("DotaCatalogItem");
        value.SetField("defIndex", TsValue.FromInt32(unchecked((int)item.DefIndex)));
        value.SetField("name", TsValue.FromString(item.Name ?? string.Empty));
        value.SetField("qualityId", TsValue.FromInt32(unchecked((int)item.QualityId)));
        return new TsObjectValue(value);
    }

    private static TsValue ToTsEquipmentList(IEnumerable<ApiDotaEquipment> equipment)
    {
        var array = new TsArray();
        foreach (var item in equipment)
        {
            var value = new TsObject("DotaEquipment");
            value.SetField("steamId", TsValue.FromUInt64(item.SteamId));
            value.SetField("heroId", TsValue.FromInt32(unchecked((int)item.HeroId)));
            value.SetField("slotId", TsValue.FromInt32(unchecked((int)item.SlotId)));
            value.SetField("defIndex", TsValue.FromInt32(unchecked((int)item.DefIndex)));
            value.SetField("itemId", TsValue.FromUInt64(item.ItemId));
            value.SetField("style", TsValue.FromInt32(unchecked((int)item.Style)));
            array.Add(new TsObjectValue(value));
        }

        return new TsArrayValue(array);
    }
}

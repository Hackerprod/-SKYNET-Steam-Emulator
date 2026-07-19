using SKYNET_server.Models;
using SKYNET_server.GC.Dota2;
using TypeSharp.Hosting;
using TypeSharp.VM.Memory;

namespace SKYNET_server.Services;

public sealed class GameCoordinatorScriptPlugin : IGameCoordinatorPlugin, IGameCoordinatorTicker
{
    private const long ScriptMemoryLimitBytes = 768L * 1024 * 1024;
    private const long ScriptMaximumInstructions = 100_000_000;
    private static readonly TimeSpan ScriptExecutionTimeout = TimeSpan.FromSeconds(120);
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
                    options.ExecutionTimeout = ScriptExecutionTimeout;
                    options.MaximumInstructions = ScriptMaximumInstructions;
                    options.MaximumMemoryBytes = ScriptMemoryLimitBytes;
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
                .RegisterHostFunction("gc", "dotaSaveProfileUpdate", dispatcher.DotaSaveProfileUpdate)
                .RegisterHostFunction("gc", "dotaSocialFeed", dispatcher.DotaSocialFeed)
                .RegisterHostFunction("gc", "dotaSocialFeedComments", dispatcher.DotaSocialFeedComments)
                .RegisterHostFunction("gc", "dotaSocialFeedPostComment", dispatcher.DotaSocialFeedPostComment)
                .RegisterHostFunction("gc", "dotaLookupAccountName", dispatcher.DotaLookupAccountName)
                .RegisterHostFunction("gc", "dotaEventPoints", dispatcher.DotaEventPoints)
                .RegisterHostFunction("gc", "dotaHeroStandings", dispatcher.DotaHeroStandings)
                .RegisterHostFunction("gc", "dotaHeroGlobalData", dispatcher.DotaHeroGlobalData)
                .RegisterHostFunction("gc", "dotaPlayerStats", dispatcher.DotaPlayerStats)
                .RegisterHostFunction("gc", "dotaRank", dispatcher.DotaRank)
                .RegisterHostFunction("gc", "dotaTeammateStats", dispatcher.DotaTeammateStats)
                .RegisterHostFunction("gc", "dotaMatchHistory", dispatcher.DotaMatchHistory)
                .RegisterHostFunction("gc", "dotaMatchDetails", dispatcher.DotaMatchDetails)
                .RegisterHostFunction("gc", "dotaHeroStatsHistory", dispatcher.DotaHeroStatsHistory)
                .RegisterHostFunction("gc", "dotaShowcaseStats", dispatcher.DotaShowcaseStats)
                .RegisterHostFunction("gc", "dotaRecentAccomplishments", dispatcher.DotaRecentAccomplishments)
                .RegisterHostFunction("gc", "dotaHeroRecentAccomplishments", dispatcher.DotaHeroRecentAccomplishments)
                .RegisterHostFunction("gc", "dotaHasMvpVote", dispatcher.DotaHasMvpVote)
                .RegisterHostFunction("gc", "dotaVoteForMvp", dispatcher.DotaVoteForMvp)
                .RegisterHostFunction("gc", "dotaFinalizeMvpVote", dispatcher.DotaFinalizeMvpVote)
                .RegisterHostFunction("gc", "dotaSubmitLobbyMvpVote", dispatcher.DotaSubmitLobbyMvpVote)
                .RegisterHostFunction("gc", "dotaRecordSignOutMvpStats", dispatcher.DotaRecordSignOutMvpStats)
                .RegisterHostFunction("gc", "dotaRerollPlayerChallenge", dispatcher.DotaRerollPlayerChallenge)
                .RegisterHostFunction("gc", "dotaRecordMatchSignOutPermission", dispatcher.DotaRecordMatchSignOutPermission)
                .RegisterHostFunction("gc", "dotaSetMatchHistoryAccess", dispatcher.DotaSetMatchHistoryAccess)
                .RegisterHostFunction("gc", "dotaRecordServerStatus", dispatcher.DotaRecordServerStatus)
                .RegisterHostFunction("gc", "dotaRecordLeaver", dispatcher.DotaRecordLeaver)
                .RegisterHostFunction("gc", "dotaRecordRealtimeStats", dispatcher.DotaRecordRealtimeStats)
                .RegisterHostFunction("gc", "dotaRecordMatchStateHistory", dispatcher.DotaRecordMatchStateHistory)
                .RegisterHostFunction("gc", "dotaRecordSpectatorCount", dispatcher.DotaRecordSpectatorCount)
                .RegisterHostFunction("gc", "dotaRecordLiveScoreboard", dispatcher.DotaRecordLiveScoreboard)
                .RegisterHostFunction("gc", "dotaSavePlayerReport", dispatcher.DotaSavePlayerReport);

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

    public TsValue? DotaSocialFeed(TsValue[] args)
    {
        return RequireCurrent().DotaSocialFeed(args);
    }

    public TsValue? DotaSocialFeedComments(TsValue[] args)
    {
        return RequireCurrent().DotaSocialFeedComments(args);
    }

    public TsValue? DotaSocialFeedPostComment(TsValue[] args)
    {
        return RequireCurrent().DotaSocialFeedPostComment(args);
    }

    public TsValue? DotaLookupAccountName(TsValue[] args)
    {
        return RequireCurrent().DotaLookupAccountName(args);
    }

    public TsValue? DotaEventPoints(TsValue[] args)
    {
        return RequireCurrent().DotaEventPoints(args);
    }

    public TsValue? DotaHeroStandings(TsValue[] args)
    {
        return RequireCurrent().DotaHeroStandings(args);
    }

    public TsValue? DotaHeroGlobalData(TsValue[] args)
    {
        return RequireCurrent().DotaHeroGlobalData(args);
    }

    public TsValue? DotaPlayerStats(TsValue[] args)
    {
        return RequireCurrent().DotaPlayerStats(args);
    }

    public TsValue? DotaRank(TsValue[] args)
    {
        return RequireCurrent().DotaRank(args);
    }

    public TsValue? DotaTeammateStats(TsValue[] args)
    {
        return RequireCurrent().DotaTeammateStats(args);
    }

    public TsValue? DotaMatchHistory(TsValue[] args)
    {
        return RequireCurrent().DotaMatchHistory(args);
    }

    public TsValue? DotaMatchDetails(TsValue[] args)
    {
        return RequireCurrent().DotaMatchDetails(args);
    }

    public TsValue? DotaHeroStatsHistory(TsValue[] args)
    {
        return RequireCurrent().DotaHeroStatsHistory(args);
    }

    public TsValue? DotaShowcaseStats(TsValue[] args)
    {
        return RequireCurrent().DotaShowcaseStats(args);
    }

    public TsValue? DotaRecentAccomplishments(TsValue[] args)
    {
        return RequireCurrent().DotaRecentAccomplishments(args);
    }

    public TsValue? DotaHeroRecentAccomplishments(TsValue[] args)
    {
        return RequireCurrent().DotaHeroRecentAccomplishments(args);
    }

    public TsValue? DotaHasMvpVote(TsValue[] args)
    {
        return RequireCurrent().DotaHasMvpVote(args);
    }

    public TsValue? DotaVoteForMvp(TsValue[] args)
    {
        return RequireCurrent().DotaVoteForMvp(args);
    }

    public TsValue? DotaFinalizeMvpVote(TsValue[] args)
    {
        return RequireCurrent().DotaFinalizeMvpVote(args);
    }

    public TsValue? DotaSubmitLobbyMvpVote(TsValue[] args)
    {
        return RequireCurrent().DotaSubmitLobbyMvpVote(args);
    }

    public TsValue? DotaRecordSignOutMvpStats(TsValue[] args)
    {
        return RequireCurrent().DotaRecordSignOutMvpStats(args);
    }

    public TsValue? DotaRerollPlayerChallenge(TsValue[] args)
    {
        return RequireCurrent().DotaRerollPlayerChallenge();
    }

    public TsValue? DotaRecordMatchSignOutPermission(TsValue[] args)
    {
        return RequireCurrent().DotaRecordMatchSignOutPermission(args);
    }

    public TsValue? DotaSetMatchHistoryAccess(TsValue[] args)
    {
        return RequireCurrent().DotaSetMatchHistoryAccess(args);
    }

    public TsValue? DotaRecordServerStatus(TsValue[] args)
    {
        return RequireCurrent().DotaRecordServerStatus(args);
    }

    public TsValue? DotaRecordLeaver(TsValue[] args)
    {
        return RequireCurrent().DotaRecordLeaver(args);
    }

    public TsValue? DotaRecordRealtimeStats(TsValue[] args)
    {
        return RequireCurrent().DotaRecordRealtimeStats(args);
    }

    public TsValue? DotaRecordMatchStateHistory(TsValue[] args)
    {
        return RequireCurrent().DotaRecordMatchStateHistory(args);
    }

    public TsValue? DotaRecordSpectatorCount(TsValue[] args)
    {
        return RequireCurrent().DotaRecordSpectatorCount(args);
    }

    public TsValue? DotaRecordLiveScoreboard(TsValue[] args)
    {
        return RequireCurrent().DotaRecordLiveScoreboard(args);
    }

    public TsValue? DotaSavePlayerReport(TsValue[] args)
    {
        return RequireCurrent().DotaSavePlayerReport(args);
    }
}

internal sealed class ScriptExchangeHost
{
    private const uint DotaActiveEventId = 57;
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

    public TsValue DotaSocialFeed(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaSocialFeed(accountId, selfOnly) requires two arguments");
        }

        var accountId = Convert.ToUInt32(ToNumber(args[0], "dotaSocialFeed.accountId"));
        if (accountId == 0)
        {
            accountId = _context.AccountId;
        }

        var selfOnly = ToBool(args[1], "dotaSocialFeed.selfOnly");
        return ToTsSocialFeed(DotaGcBackend.StatsStore?.GetSocialFeed(accountId, selfOnly) ?? Array.Empty<DotaStatsSocialFeedEvent>());
    }

    public TsValue DotaSocialFeedComments(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaSocialFeedComments(feedEventId) requires one argument");
        }

        var feedEventId = Convert.ToUInt64(ToInteger(args[0], "dotaSocialFeedComments.feedEventId").ToString());
        return ToTsSocialFeedComments(feedEventId, DotaGcBackend.StatsStore?.GetSocialFeedComments(feedEventId) ?? Array.Empty<DotaStatsComment>());
    }

    public TsValue DotaSocialFeedPostComment(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaSocialFeedPostComment(feedEventId, comment) requires two arguments");
        }

        var feedEventId = Convert.ToUInt64(ToInteger(args[0], "dotaSocialFeedPostComment.feedEventId").ToString());
        var comment = ToString(args[1]).Trim();
        if (feedEventId == 0 || comment.Length == 0)
        {
            return TsValue.FromBool(false);
        }

        return TsValue.FromBool(DotaGcBackend.StatsStore?.SaveSocialFeedComment(feedEventId, _context.AccountId, comment) ?? false);
    }

    public TsValue DotaLookupAccountName(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaLookupAccountName(accountId) requires one argument");
        }

        var accountId = Convert.ToUInt32(ToNumber(args[0], "dotaLookupAccountName.accountId"));
        if (accountId == 0)
        {
            accountId = _context.AccountId;
        }

        var profile = GetStatsProfile(accountId);
        var value = new TsObject("DotaAccountName");
        value.SetField("accountId", TsValue.FromInt32(unchecked((int)profile.AccountId)));
        value.SetField("accountName", TsValue.FromString(profile.PersonaName ?? string.Empty));
        return new TsObjectValue(value);
    }

    public TsValue DotaEventPoints(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaEventPoints(accountId, eventId) requires two arguments");
        }

        var accountId = Convert.ToUInt32(ToNumber(args[0], "dotaEventPoints.accountId"));
        if (accountId == 0)
        {
            accountId = _context.AccountId;
        }

        var eventId = Convert.ToUInt32(ToNumber(args[1], "dotaEventPoints.eventId"));
        if (eventId == 0)
        {
            eventId = DotaActiveEventId;
        }

        var profile = GetStatsProfile(accountId);
        var value = new TsObject("DotaEventPoints");
        value.SetField("accountId", TsValue.FromInt32(unchecked((int)profile.AccountId)));
        value.SetField("eventId", TsValue.FromInt32(unchecked((int)eventId)));
        value.SetField("totalPoints", TsValue.FromInt32(unchecked((int)profile.EventPoints)));
        value.SetField("totalPremiumPoints", TsValue.FromInt32(0));
        value.SetField("points", TsValue.FromInt32(unchecked((int)profile.EventPoints)));
        value.SetField("premiumPoints", TsValue.FromInt32(0));
        value.SetField("owned", TsValue.FromBool(true));
        value.SetField("auditAction", TsValue.FromInt32(35));
        value.SetField("activeSeasonId", TsValue.FromInt32(unchecked((int)DotaActiveEventId)));
        return new TsObjectValue(value);
    }

    public TsValue DotaHeroStandings(TsValue[] args)
    {
        var accountId = args.Length > 0
            ? Convert.ToUInt32(ToNumber(args[0], "dotaHeroStandings.accountId"))
            : _context.AccountId;
        if (accountId == 0)
        {
            accountId = _context.AccountId;
        }

        return ToTsHeroStandings(DotaGcBackend.StatsStore?.GetHeroStandings(accountId) ?? Array.Empty<DotaStatsHeroStats>());
    }

    public TsValue DotaHeroGlobalData(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaHeroGlobalData(accountId, heroId) requires two arguments");
        }

        var accountId = Convert.ToUInt32(ToNumber(args[0], "dotaHeroGlobalData.accountId"));
        if (accountId == 0)
        {
            accountId = _context.AccountId;
        }

        var heroId = Convert.ToUInt32(ToNumber(args[1], "dotaHeroGlobalData.heroId"));
        var standings = DotaGcBackend.StatsStore?.GetHeroStandings(accountId) ?? Array.Empty<DotaStatsHeroStats>();
        var hero = standings.FirstOrDefault(item => item.HeroId == heroId);
        return ToTsHeroGlobalData(heroId, hero, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    }

    public TsValue DotaPlayerStats(TsValue[] args)
    {
        var accountId = args.Length > 0
            ? Convert.ToUInt32(ToNumber(args[0], "dotaPlayerStats.accountId"))
            : _context.AccountId;
        if (accountId == 0)
        {
            accountId = _context.AccountId;
        }

        var stats = DotaGcBackend.StatsStore?.GetGlobalStats(accountId) ?? new DotaStatsGlobalStats { AccountId = accountId };
        return ToTsPlayerStats(stats);
    }

    public TsValue DotaRank(TsValue[] args)
    {
        var accountId = args.Length > 0
            ? Convert.ToUInt32(ToNumber(args[0], "dotaRank.accountId"))
            : _context.AccountId;
        if (accountId == 0)
        {
            accountId = _context.AccountId;
        }

        var profile = GetStatsProfile(accountId);
        var value = new TsObject("DotaRank");
        value.SetField("result", TsValue.FromInt32(0));
        value.SetField("rankValue", TsValue.FromInt32(unchecked((int)profile.RankTier)));
        value.SetField("rankData1", TsValue.FromInt32(unchecked((int)profile.RankScore)));
        value.SetField("rankData2", TsValue.FromInt32(unchecked((int)profile.RankTierScore)));
        value.SetField("rankData3", TsValue.FromInt32(unchecked((int)profile.LeaderboardRank)));
        return new TsObjectValue(value);
    }

    public TsValue DotaTeammateStats(TsValue[] args)
    {
        var accountId = args.Length > 0
            ? Convert.ToUInt32(ToNumber(args[0], "dotaTeammateStats.accountId"))
            : _context.AccountId;
        if (accountId == 0)
        {
            accountId = _context.AccountId;
        }

        return ToTsTeammateStats(DotaGcBackend.StatsStore?.GetTeammateStats(accountId) ?? Array.Empty<DotaStatsTeammate>());
    }

    public TsValue DotaMatchHistory(TsValue[] args)
    {
        if (args.Length < 5)
        {
            throw new InvalidOperationException("dotaMatchHistory(accountId, startAtMatchId, requested, heroId, includePractice) requires five arguments");
        }

        var accountId = Convert.ToUInt32(ToNumber(args[0], "dotaMatchHistory.accountId"));
        if (accountId == 0)
        {
            accountId = _context.AccountId;
        }

        var startAtMatchId = Convert.ToUInt64(ToInteger(args[1], "dotaMatchHistory.startAtMatchId").ToString());
        var requested = Convert.ToUInt32(ToNumber(args[2], "dotaMatchHistory.requested"));
        var heroId = Convert.ToUInt32(ToNumber(args[3], "dotaMatchHistory.heroId"));
        var includePractice = ToBool(args[4], "dotaMatchHistory.includePractice");
        return ToTsMatchPlayers(DotaGcBackend.StatsStore?.GetMatchHistory(accountId, startAtMatchId, requested, heroId, includePractice) ?? Array.Empty<DotaStatsMatchPlayer>());
    }

    public TsValue DotaMatchDetails(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaMatchDetails(matchId) requires one argument");
        }

        var matchId = Convert.ToUInt64(ToInteger(args[0], "dotaMatchDetails.matchId").ToString());
        var match = DotaGcBackend.StatsStore?.GetMatch(matchId);
        return match == null ? TsValue.Null : ToTsMatch(match);
    }

    public TsValue DotaHeroStatsHistory(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaHeroStatsHistory(accountId, heroId) requires two arguments");
        }

        var accountId = Convert.ToUInt32(ToNumber(args[0], "dotaHeroStatsHistory.accountId"));
        if (accountId == 0)
        {
            accountId = _context.AccountId;
        }

        var heroId = Convert.ToUInt32(ToNumber(args[1], "dotaHeroStatsHistory.heroId"));
        return ToTsMatchPlayers(DotaGcBackend.StatsStore?.GetRecentMatches(accountId, 20, heroId) ?? Array.Empty<DotaStatsMatchPlayer>());
    }

    public TsValue DotaShowcaseStats(TsValue[] args)
    {
        var accountId = args.Length > 0
            ? Convert.ToUInt32(ToNumber(args[0], "dotaShowcaseStats.accountId"))
            : _context.AccountId;
        if (accountId == 0)
        {
            accountId = _context.AccountId;
        }

        var store = DotaGcBackend.StatsStore;
        var global = store?.GetGlobalStats(accountId) ?? new DotaStatsGlobalStats { AccountId = accountId };
        var conduct = store?.GetConduct(accountId) ?? new DotaStatsConduct { AccountId = accountId };
        var value = new TsObject("DotaShowcaseStats");
        value.SetField("gamesWon", TsValue.FromInt32(unchecked((int)global.GamesWon)));
        value.SetField("commendCount", TsValue.FromInt32(unchecked((int)conduct.CommendCount)));
        value.SetField("mvpCount", TsValue.FromInt32(unchecked((int)(store?.GetMvpCount(accountId) ?? 0))));
        return new TsObjectValue(value);
    }

    public TsValue DotaRecentAccomplishments(TsValue[] args)
    {
        var accountId = args.Length > 0
            ? Convert.ToUInt32(ToNumber(args[0], "dotaRecentAccomplishments.accountId"))
            : _context.AccountId;
        if (accountId == 0)
        {
            accountId = _context.AccountId;
        }

        return ToTsPlayerRecentAccomplishments(accountId);
    }

    public TsValue DotaHeroRecentAccomplishments(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaHeroRecentAccomplishments(accountId, heroId) requires two arguments");
        }

        var accountId = Convert.ToUInt32(ToNumber(args[0], "dotaHeroRecentAccomplishments.accountId"));
        if (accountId == 0)
        {
            accountId = _context.AccountId;
        }

        var heroId = Convert.ToUInt32(ToNumber(args[1], "dotaHeroRecentAccomplishments.heroId"));
        return ToTsHeroRecentAccomplishments(accountId, heroId);
    }

    public TsValue DotaHasMvpVote(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaHasMvpVote(matchId) requires one argument");
        }

        var matchId = Convert.ToUInt64(ToInteger(args[0], "dotaHasMvpVote.matchId").ToString());
        return TsValue.FromBool(DotaGcBackend.StatsStore?.HasMvpVote(matchId, _context.AccountId) ?? false);
    }

    public TsValue DotaVoteForMvp(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaVoteForMvp(matchId, votedAccountId) requires two arguments");
        }

        var matchId = Convert.ToUInt64(ToInteger(args[0], "dotaVoteForMvp.matchId").ToString());
        var votedAccountId = Convert.ToUInt32(ToNumber(args[1], "dotaVoteForMvp.votedAccountId"));
        return TsValue.FromBool(DotaGcBackend.StatsStore?.SaveMvpVote(matchId, _context.AccountId, votedAccountId) ?? false);
    }

    public TsValue DotaFinalizeMvpVote(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaFinalizeMvpVote(matchId) requires one argument");
        }

        var matchId = Convert.ToUInt64(ToInteger(args[0], "dotaFinalizeMvpVote.matchId").ToString());
        return TsValue.FromBool(DotaGcBackend.StatsStore?.FinalizeMvpVotes(matchId) ?? false);
    }

    public TsValue DotaSubmitLobbyMvpVote(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaSubmitLobbyMvpVote(targetAccountId) requires one argument");
        }

        var targetAccountId = Convert.ToUInt32(ToNumber(args[0], "dotaSubmitLobbyMvpVote.targetAccountId"));
        var latestMatchId = DotaGcBackend.StatsStore?.GetRecentMatches(_context.AccountId, 1).FirstOrDefault()?.MatchId ?? 0;
        var ok = latestMatchId != 0 && (DotaGcBackend.StatsStore?.SaveMvpVote(latestMatchId, _context.AccountId, targetAccountId) ?? false);
        if (ok)
        {
            DotaGcBackend.StatsStore?.FinalizeMvpVotes(latestMatchId);
        }

        var value = new TsObject("DotaLobbyMvpVoteResult");
        value.SetField("targetAccountId", TsValue.FromInt32(unchecked((int)targetAccountId)));
        value.SetField("result", TsValue.FromInt32(ok ? 1 : 2));
        return new TsObjectValue(value);
    }

    public TsValue DotaRecordSignOutMvpStats(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaRecordSignOutMvpStats(matchId, players) requires two arguments");
        }

        var matchId = Convert.ToUInt64(ToInteger(args[0], "dotaRecordSignOutMvpStats.matchId").ToString());
        if (args[1] is not TsArrayValue players)
        {
            throw new InvalidOperationException("dotaRecordSignOutMvpStats.players: expected array");
        }

        var winners = new List<uint>();
        for (var i = 0; i < players.Value.Count; i++)
        {
            if (players.Value.Get(i) is not TsObjectValue playerValue)
            {
                continue;
            }

            var player = playerValue.Value;
            var accountId = Convert.ToUInt32(ToNumber(player.GetField("accountId"), $"dotaRecordSignOutMvpStats.players[{i}].accountId"));
            var rank = Convert.ToUInt32(ToNumber(player.GetField("rank"), $"dotaRecordSignOutMvpStats.players[{i}].rank"));
            if (accountId != 0 && rank == 1)
            {
                winners.Add(accountId);
            }
        }

        return TsValue.FromBool(DotaGcBackend.StatsStore?.SetMatchMvps(matchId, winners) ?? false);
    }

    public TsValue DotaRerollPlayerChallenge()
    {
        var progress = DotaGcBackend.StatsStore?.RerollAllHeroChallenge(_context.AccountId);
        return TsValue.FromBool(progress != null);
    }

    public TsValue DotaRecordMatchSignOutPermission(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaRecordMatchSignOutPermission(request) requires one argument");
        }

        var request = RequireObject(args[0], "dotaRecordMatchSignOutPermission.request");
        return TsValue.FromBool(DotaGcBackend.StatsStore?.RecordMatchSignOutPermission(new DotaStatsMatchSignOutPermissionAudit
        {
            ServerSteamId = _context.SteamId,
            ServerVersion = U32Field(request, "serverVersion", "dotaRecordMatchSignOutPermission.request"),
            LocalAttempt = U32Field(request, "localAttempt", "dotaRecordMatchSignOutPermission.request"),
            TotalAttempt = U32Field(request, "totalAttempt", "dotaRecordMatchSignOutPermission.request"),
            SecondsWaited = U32Field(request, "secondsWaited", "dotaRecordMatchSignOutPermission.request"),
            PermissionGranted = BoolField(request, "permissionGranted", "dotaRecordMatchSignOutPermission.request"),
            AbandonSignout = BoolField(request, "abandonSignout", "dotaRecordMatchSignOutPermission.request"),
            RetryDelaySeconds = U32Field(request, "retryDelaySeconds", "dotaRecordMatchSignOutPermission.request")
        }) ?? false);
    }

    public TsValue DotaSetMatchHistoryAccess(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaSetMatchHistoryAccess(allow) requires one argument");
        }

        var allow = ToBool(args[0], "dotaSetMatchHistoryAccess.allow");
        return TsValue.FromBool(DotaGcBackend.StatsStore?.SetMatchHistoryAccess(_context.SteamId, _context.AccountId, allow) ?? false);
    }

    public TsValue DotaRecordServerStatus(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaRecordServerStatus(response) requires one argument");
        }

        var response = Convert.ToUInt32(ToNumber(args[0], "dotaRecordServerStatus.response"));
        return TsValue.FromBool(DotaGcBackend.StatsStore?.RecordServerStatusRequest(_context.SteamId, response) ?? false);
    }

    public TsValue DotaRecordLeaver(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaRecordLeaver(event) requires one argument");
        }

        var eventValue = RequireObject(args[0], "dotaRecordLeaver.event");
        var leaverSteamId = U64Field(eventValue, "leaverSteamId", "dotaRecordLeaver.event");
        return TsValue.FromBool(DotaGcBackend.StatsStore?.RecordLeaverDetected(new DotaStatsLeaverEvent
        {
            ServerSteamId = _context.SteamId,
            LeaverSteamId = leaverSteamId,
            LeaverAccountId = SteamIdToAccountId(leaverSteamId),
            LeaverStatus = U32Field(eventValue, "leaverStatus", "dotaRecordLeaver.event"),
            LobbyState = U32Field(eventValue, "lobbyState", "dotaRecordLeaver.event"),
            GameState = U32Field(eventValue, "gameState", "dotaRecordLeaver.event"),
            LeaverDetected = BoolField(eventValue, "leaverDetected", "dotaRecordLeaver.event"),
            FirstBloodHappened = BoolField(eventValue, "firstBloodHappened", "dotaRecordLeaver.event"),
            DiscardMatchResults = BoolField(eventValue, "discardMatchResults", "dotaRecordLeaver.event"),
            MassDisconnect = BoolField(eventValue, "massDisconnect", "dotaRecordLeaver.event"),
            ServerCluster = U32Field(eventValue, "serverCluster", "dotaRecordLeaver.event"),
            DisconnectReason = U32Field(eventValue, "disconnectReason", "dotaRecordLeaver.event")
        }) ?? false);
    }

    public TsValue DotaRecordRealtimeStats(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaRecordRealtimeStats(snapshot) requires one argument");
        }

        var snapshot = RequireObject(args[0], "dotaRecordRealtimeStats.snapshot");
        return TsValue.FromBool(DotaGcBackend.StatsStore?.RecordRealtimeStats(new DotaStatsRealtimeStatsSnapshot
        {
            ServerSteamId = U64Field(snapshot, "serverSteamId", "dotaRecordRealtimeStats.snapshot", _context.SteamId),
            MatchId = U64Field(snapshot, "matchId", "dotaRecordRealtimeStats.snapshot"),
            Timestamp = U32Field(snapshot, "timestamp", "dotaRecordRealtimeStats.snapshot"),
            GameTime = U32Field(snapshot, "gameTime", "dotaRecordRealtimeStats.snapshot"),
            GameState = U32Field(snapshot, "gameState", "dotaRecordRealtimeStats.snapshot"),
            GameMode = U32Field(snapshot, "gameMode", "dotaRecordRealtimeStats.snapshot"),
            LobbyType = U32Field(snapshot, "lobbyType", "dotaRecordRealtimeStats.snapshot"),
            LeagueId = U32Field(snapshot, "leagueId", "dotaRecordRealtimeStats.snapshot"),
            RadiantScore = U32Field(snapshot, "radiantScore", "dotaRecordRealtimeStats.snapshot"),
            DireScore = U32Field(snapshot, "direScore", "dotaRecordRealtimeStats.snapshot"),
            PlayerCount = U32Field(snapshot, "playerCount", "dotaRecordRealtimeStats.snapshot"),
            BuildingCount = U32Field(snapshot, "buildingCount", "dotaRecordRealtimeStats.snapshot"),
            DeltaFrame = BoolField(snapshot, "deltaFrame", "dotaRecordRealtimeStats.snapshot"),
            PayloadSize = U32Field(snapshot, "payloadSize", "dotaRecordRealtimeStats.snapshot")
        }) ?? false);
    }

    public TsValue DotaRecordMatchStateHistory(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaRecordMatchStateHistory(history) requires one argument");
        }

        var history = RequireObject(args[0], "dotaRecordMatchStateHistory.history");
        return TsValue.FromBool(DotaGcBackend.StatsStore?.RecordMatchStateHistory(new DotaStatsMatchStateHistorySnapshot
        {
            MatchId = U64Field(history, "matchId", "dotaRecordMatchStateHistory.history"),
            RadiantWon = BoolField(history, "radiantWon", "dotaRecordMatchStateHistory.history"),
            Mmr = U32Field(history, "mmr", "dotaRecordMatchStateHistory.history"),
            StateCount = U32Field(history, "stateCount", "dotaRecordMatchStateHistory.history"),
            LastGameTime = U32Field(history, "lastGameTime", "dotaRecordMatchStateHistory.history"),
            RadiantKills = U32Field(history, "radiantKills", "dotaRecordMatchStateHistory.history"),
            DireKills = U32Field(history, "direKills", "dotaRecordMatchStateHistory.history"),
            PayloadSize = U32Field(history, "payloadSize", "dotaRecordMatchStateHistory.history")
        }) ?? false);
    }

    public TsValue DotaRecordSpectatorCount(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaRecordSpectatorCount(spectatorCount) requires one argument");
        }

        var spectatorCount = Convert.ToUInt32(ToNumber(args[0], "dotaRecordSpectatorCount.spectatorCount"));
        return TsValue.FromBool(DotaGcBackend.StatsStore?.RecordSpectatorCount(_context.SteamId, spectatorCount) ?? false);
    }

    public TsValue DotaRecordLiveScoreboard(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaRecordLiveScoreboard(snapshot) requires one argument");
        }

        var snapshot = RequireObject(args[0], "dotaRecordLiveScoreboard.snapshot");
        return TsValue.FromBool(DotaGcBackend.StatsStore?.RecordLiveScoreboard(new DotaStatsLiveScoreboardSnapshot
        {
            ServerSteamId = _context.SteamId,
            MatchId = U64Field(snapshot, "matchId", "dotaRecordLiveScoreboard.snapshot"),
            TournamentId = U32Field(snapshot, "tournamentId", "dotaRecordLiveScoreboard.snapshot"),
            TournamentGameId = U32Field(snapshot, "tournamentGameId", "dotaRecordLiveScoreboard.snapshot"),
            Duration = U32Field(snapshot, "duration", "dotaRecordLiveScoreboard.snapshot"),
            HltvDelay = U32Field(snapshot, "hltvDelay", "dotaRecordLiveScoreboard.snapshot"),
            LeagueId = U32Field(snapshot, "leagueId", "dotaRecordLiveScoreboard.snapshot"),
            RadiantScore = U32Field(snapshot, "radiantScore", "dotaRecordLiveScoreboard.snapshot"),
            DireScore = U32Field(snapshot, "direScore", "dotaRecordLiveScoreboard.snapshot"),
            PlayerCount = U32Field(snapshot, "playerCount", "dotaRecordLiveScoreboard.snapshot"),
            RoshanRespawnTimer = U32Field(snapshot, "roshanRespawnTimer", "dotaRecordLiveScoreboard.snapshot"),
            PayloadSize = U32Field(snapshot, "payloadSize", "dotaRecordLiveScoreboard.snapshot")
        }) ?? false);
    }

    public TsValue DotaSavePlayerReport(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaSavePlayerReport(report) requires one argument");
        }

        var report = RequireObject(args[0], "dotaSavePlayerReport.report");
        return TsValue.FromBool(DotaGcBackend.StatsStore?.TrySavePlayerReport(new DotaStatsPlayerReport
        {
            ReporterSteamId = _context.SteamId,
            ReporterAccountId = _context.AccountId,
            TargetAccountId = U32Field(report, "targetAccountId", "dotaSavePlayerReport.report"),
            LobbyId = U64Field(report, "lobbyId", "dotaSavePlayerReport.report"),
            ReportFlags = U32Field(report, "reportFlags", "dotaSavePlayerReport.report"),
            ReportReasons = UInt32ArrayField(report, "reportReasons", "dotaSavePlayerReport.report"),
            Comment = StringField(report, "comment", "dotaSavePlayerReport.report"),
            GameTime = U32Field(report, "gameTime", "dotaSavePlayerReport.report"),
            DebugSlot = U32Field(report, "debugSlot", "dotaSavePlayerReport.report"),
            DebugMatchId = U64Field(report, "debugMatchId", "dotaSavePlayerReport.report")
        }) ?? false);
    }

    private DotaStatsProfile GetStatsProfile(uint accountId)
    {
        var store = DotaGcBackend.StatsStore;
        return store == null
            ? DefaultProfile(accountId)
            : accountId == _context.AccountId
                ? store.EnsureProfile(_context.SteamId, _context.AccountId, _context.PersonaName)
                : store.GetProfile(accountId);
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
        value.SetField("activeEventId", TsValue.FromInt32(unchecked((int)DotaActiveEventId)));
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

    private static TsValue ToTsPlayerStats(DotaStatsGlobalStats stats)
    {
        static double Score(double value, double divisor = 1.0) => 0.5 + Math.Min(1.0, value / divisor) / 2.0;

        var value = new TsObject("DotaPlayerStats");
        value.SetField("accountId", TsValue.FromInt32(unchecked((int)stats.AccountId)));
        value.SetField("matchCount", TsValue.FromInt32(unchecked((int)stats.MatchCount)));
        value.SetField("meanGpm", TsValue.FromFloat64(stats.MediaGpm));
        value.SetField("meanXppm", TsValue.FromFloat64(stats.MediaXpm));
        value.SetField("meanLasthits", TsValue.FromFloat64(stats.MediaLastHits));
        value.SetField("rampages", TsValue.FromInt32(unchecked((int)stats.Rampages)));
        value.SetField("tripleKills", TsValue.FromInt32(unchecked((int)stats.TripleKills)));
        value.SetField("firstBloodClaimed", TsValue.FromInt32(unchecked((int)stats.FirstBloodsReceived)));
        value.SetField("firstBloodGiven", TsValue.FromInt32(unchecked((int)stats.FirstBloodsGiven)));
        value.SetField("couriersKilled", TsValue.FromInt32(unchecked((int)stats.CouriersKilled)));
        value.SetField("aegisesSnatched", TsValue.FromInt32(unchecked((int)stats.AegisesSnatched)));
        value.SetField("cheesesEaten", TsValue.FromInt32(unchecked((int)stats.CheesesEaten)));
        value.SetField("creepsStacked", TsValue.FromInt32(unchecked((int)stats.CreepsStacked)));
        value.SetField("fightScore", TsValue.FromFloat64(Score(stats.AvgFightScore)));
        value.SetField("farmScore", TsValue.FromFloat64(Score(stats.AvgFarmScore, 500.0)));
        value.SetField("supportScore", TsValue.FromFloat64(Score(stats.AvgSupportScore, 10000.0)));
        value.SetField("pushScore", TsValue.FromFloat64(Score(stats.AvgPushScore, 2500.0)));
        value.SetField("versatilityScore", TsValue.FromFloat64(Score(stats.PlayedHeroCount, 123.0)));
        value.SetField("meanNetworth", TsValue.FromFloat64(stats.MeanNetworth));
        value.SetField("meanDamage", TsValue.FromFloat64(stats.MeanDamage));
        value.SetField("meanHeals", TsValue.FromFloat64(stats.MeanHeals));
        value.SetField("rapiersPurchased", TsValue.FromInt32(unchecked((int)stats.RapiersPurchased)));
        return new TsObjectValue(value);
    }

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

    private static TsValue ToTsMatchPlayers(IEnumerable<DotaStatsMatchPlayer> players)
    {
        var array = new TsArray();
        foreach (var player in players)
        {
            array.Add(ToTsMatchPlayer(player));
        }

        return new TsArrayValue(array);
    }

    private static TsValue ToTsMatch(DotaStatsMatch match)
    {
        var value = new TsObject("DotaMatch");
        value.SetField("matchId", TsValue.FromUInt64(match.MatchId));
        value.SetField("ownerSteamId", TsValue.FromUInt64(match.OwnerSteamId));
        value.SetField("serverSteamId", TsValue.FromUInt64(match.ServerSteamId));
        value.SetField("startTime", TsValue.FromInt32(unchecked((int)match.StartTime)));
        value.SetField("duration", TsValue.FromInt32(unchecked((int)match.Duration)));
        value.SetField("gameMode", TsValue.FromInt32(unchecked((int)match.GameMode)));
        value.SetField("lobbyType", TsValue.FromInt32(unchecked((int)match.LobbyType)));
        value.SetField("goodGuysWin", TsValue.FromBool(match.GoodGuysWin));
        value.SetField("matchFlags", TsValue.FromInt32(unchecked((int)match.MatchFlags)));
        value.SetField("radiantScore", TsValue.FromInt32(unchecked((int)match.RadiantScore)));
        value.SetField("direScore", TsValue.FromInt32(unchecked((int)match.DireScore)));
        value.SetField("cluster", TsValue.FromInt32(unchecked((int)match.Cluster)));
        value.SetField("firstBloodTime", TsValue.FromInt32(unchecked((int)match.FirstBloodTime)));
        value.SetField("players", ToTsMatchPlayers(match.Players));
        return new TsObjectValue(value);
    }

    private static TsValue ToTsMatchPlayer(DotaStatsMatchPlayer player)
    {
        var value = new TsObject("DotaMatchPlayer");
        value.SetField("matchId", TsValue.FromUInt64(player.MatchId));
        value.SetField("accountId", TsValue.FromInt32(unchecked((int)player.AccountId)));
        value.SetField("steamId", TsValue.FromUInt64(player.SteamId));
        value.SetField("personaName", TsValue.FromString(player.PersonaName ?? string.Empty));
        value.SetField("team", TsValue.FromInt32(unchecked((int)player.Team)));
        value.SetField("playerSlot", TsValue.FromInt32(unchecked((int)player.PlayerSlot)));
        value.SetField("heroId", TsValue.FromInt32(unchecked((int)player.HeroId)));
        value.SetField("kills", TsValue.FromInt32(unchecked((int)player.Kills)));
        value.SetField("deaths", TsValue.FromInt32(unchecked((int)player.Deaths)));
        value.SetField("assists", TsValue.FromInt32(unchecked((int)player.Assists)));
        value.SetField("winner", TsValue.FromBool(player.Winner));
        value.SetField("goodGuys", TsValue.FromBool(player.GoodGuys));
        value.SetField("gold", TsValue.FromInt32(unchecked((int)player.Gold)));
        value.SetField("goldSpent", TsValue.FromInt32(unchecked((int)player.GoldSpent)));
        value.SetField("gpm", TsValue.FromInt32(unchecked((int)player.Gpm)));
        value.SetField("xpm", TsValue.FromInt32(unchecked((int)player.Xpm)));
        value.SetField("lastHits", TsValue.FromInt32(unchecked((int)player.LastHits)));
        value.SetField("denies", TsValue.FromInt32(unchecked((int)player.Denies)));
        value.SetField("heroDamage", TsValue.FromInt32(unchecked((int)player.HeroDamage)));
        value.SetField("towerDamage", TsValue.FromInt32(unchecked((int)player.TowerDamage)));
        value.SetField("heroHealing", TsValue.FromInt32(unchecked((int)player.HeroHealing)));
        value.SetField("level", TsValue.FromInt32(unchecked((int)player.Level)));
        value.SetField("netWorth", TsValue.FromFloat64(player.NetWorth));
        value.SetField("supportGold", TsValue.FromInt32(unchecked((int)player.SupportGold)));
        value.SetField("claimedFarmGold", TsValue.FromInt32(unchecked((int)player.ClaimedFarmGold)));
        value.SetField("bountyRunes", TsValue.FromInt32(unchecked((int)player.BountyRunes)));
        value.SetField("outpostsCaptured", TsValue.FromInt32(unchecked((int)player.OutpostsCaptured)));
        value.SetField("selectedFacet", TsValue.FromInt32(unchecked((int)player.SelectedFacet)));
        value.SetField("leaverStatus", TsValue.FromInt32(unchecked((int)player.LeaverStatus)));
        value.SetField("items", ToTsUInt32Array(player.Items));
        value.SetField("startTime", TsValue.FromInt32(unchecked((int)player.StartTime)));
        value.SetField("duration", TsValue.FromInt32(unchecked((int)player.Duration)));
        value.SetField("gameMode", TsValue.FromInt32(unchecked((int)player.GameMode)));
        value.SetField("lobbyType", TsValue.FromInt32(unchecked((int)player.LobbyType)));
        value.SetField("goodGuysWin", TsValue.FromBool(player.GoodGuysWin));
        value.SetField("matchFlags", TsValue.FromInt32(unchecked((int)player.MatchFlags)));
        value.SetField("radiantScore", TsValue.FromInt32(unchecked((int)player.RadiantScore)));
        value.SetField("direScore", TsValue.FromInt32(unchecked((int)player.DireScore)));
        value.SetField("cluster", TsValue.FromInt32(unchecked((int)player.Cluster)));
        value.SetField("firstBloodTime", TsValue.FromInt32(unchecked((int)player.FirstBloodTime)));
        return new TsObjectValue(value);
    }

    private TsValue ToTsPlayerRecentAccomplishments(uint accountId)
    {
        var store = DotaGcBackend.StatsStore;
        var stats = store?.GetGlobalStats(accountId) ?? new DotaStatsGlobalStats { AccountId = accountId };
        var conduct = store?.GetConduct(accountId) ?? new DotaStatsConduct { AccountId = accountId };
        var matchCount = store?.GetMatchCount(accountId) ?? 0;
        var recent = store?.GetRecentMatches(accountId, 1).FirstOrDefault();

        var value = new TsObject("DotaPlayerRecentAccomplishments");
        value.SetField("recentOutcomes", ToTsRecentOutcomes(0, matchCount));
        value.SetField("totalRecord", ToTsRecentRecord(stats.GamesWon, stats.GamesLost));
        value.SetField("predictionStreak", TsValue.FromInt32(0));
        value.SetField("plusPredictionStreak", TsValue.FromInt32(0));
        value.SetField("recentCommends", ToTsRecentCommends(conduct.CommendCount, matchCount));
        value.SetField("firstMatchTimestamp", TsValue.FromInt32(unchecked((int)(store?.GetFirstMatchTime(accountId) ?? 0))));
        value.SetField("lastMatch", recent == null ? TsValue.Null : ToTsPlayerRecentMatchInfo(recent));
        value.SetField("recentMvps", ToTsRecentOutcomes(store?.GetMvpCount(accountId) ?? 0, matchCount));
        return new TsObjectValue(value);
    }

    private TsValue ToTsHeroRecentAccomplishments(uint accountId, uint heroId)
    {
        var store = DotaGcBackend.StatsStore;
        var hero = store?.GetHeroStandings(accountId).FirstOrDefault(item => item.HeroId == heroId);
        var recent = store?.GetRecentMatches(accountId, 1, heroId).FirstOrDefault();
        var matchCount = store?.GetMatchCount(accountId, heroId) ?? 0;

        var value = new TsObject("DotaHeroRecentAccomplishments");
        value.SetField("recentOutcomes", ToTsRecentOutcomes(0, matchCount));
        value.SetField("totalRecord", ToTsRecentRecord(hero?.Wins ?? 0, hero?.Losses ?? 0));
        value.SetField("lastMatch", recent == null ? TsValue.Null : ToTsPlayerRecentMatchInfo(recent));
        return new TsObjectValue(value);
    }

    private static TsValue ToTsPlayerRecentMatchInfo(DotaStatsMatchPlayer match)
    {
        var value = new TsObject("DotaPlayerRecentMatchInfo");
        value.SetField("matchId", TsValue.FromUInt64(match.MatchId));
        value.SetField("timestamp", TsValue.FromInt32(unchecked((int)match.StartTime)));
        value.SetField("duration", TsValue.FromInt32(unchecked((int)match.Duration)));
        value.SetField("win", TsValue.FromBool(match.Winner));
        value.SetField("heroId", TsValue.FromInt32(unchecked((int)match.HeroId)));
        value.SetField("kills", TsValue.FromInt32(unchecked((int)match.Kills)));
        value.SetField("deaths", TsValue.FromInt32(unchecked((int)match.Deaths)));
        value.SetField("assists", TsValue.FromInt32(unchecked((int)match.Assists)));
        return new TsObjectValue(value);
    }

    private static TsValue ToTsRecentRecord(uint wins, uint losses)
    {
        var value = new TsObject("DotaRecentRecord");
        value.SetField("wins", TsValue.FromInt32(unchecked((int)wins)));
        value.SetField("losses", TsValue.FromInt32(unchecked((int)losses)));
        return new TsObjectValue(value);
    }

    private static TsValue ToTsRecentOutcomes(uint outcomes, uint matchCount)
    {
        var value = new TsObject("DotaRecentOutcomes");
        value.SetField("outcomes", TsValue.FromInt32(unchecked((int)outcomes)));
        value.SetField("matchCount", TsValue.FromInt32(unchecked((int)matchCount)));
        return new TsObjectValue(value);
    }

    private static TsValue ToTsRecentCommends(uint commends, uint matchCount)
    {
        var value = new TsObject("DotaRecentCommends");
        value.SetField("commends", TsValue.FromInt32(unchecked((int)commends)));
        value.SetField("matchCount", TsValue.FromInt32(unchecked((int)matchCount)));
        return new TsObjectValue(value);
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

    private static TsValue ToTsSocialFeed(IEnumerable<DotaStatsSocialFeedEvent> events)
    {
        var array = new TsArray();
        foreach (var item in events)
        {
            var value = new TsObject("DotaSocialFeedEvent");
            value.SetField("feedEventId", TsValue.FromUInt64(item.FeedEventId));
            value.SetField("accountId", TsValue.FromInt32(unchecked((int)item.AccountId)));
            value.SetField("timestamp", TsValue.FromInt32(unchecked((int)item.Timestamp)));
            value.SetField("commentCount", TsValue.FromInt32(unchecked((int)item.CommentCount)));
            value.SetField("eventType", TsValue.FromInt32(unchecked((int)item.EventType)));
            value.SetField("eventSubType", TsValue.FromInt32(unchecked((int)item.EventSubType)));
            value.SetField("paramBigInt1", TsValue.FromUInt64(item.ParamBigInt1));
            value.SetField("paramInt1", TsValue.FromInt32(unchecked((int)item.ParamInt1)));
            value.SetField("paramInt2", TsValue.FromInt32(unchecked((int)item.ParamInt2)));
            value.SetField("paramInt3", TsValue.FromInt32(unchecked((int)item.ParamInt3)));
            value.SetField("paramString", TsValue.FromString(item.ParamString ?? string.Empty));
            array.Add(new TsObjectValue(value));
        }

        return new TsArrayValue(array);
    }

    private static TsValue ToTsSocialFeedComments(ulong feedEventId, IEnumerable<DotaStatsComment> comments)
    {
        var array = new TsArray();
        foreach (var comment in comments)
        {
            var value = new TsObject("DotaSocialFeedComment");
            value.SetField("feedEventId", TsValue.FromUInt64(feedEventId));
            value.SetField("commenterAccountId", TsValue.FromInt32(unchecked((int)comment.AccountId)));
            value.SetField("personaName", TsValue.FromString(comment.PersonaName ?? string.Empty));
            value.SetField("commentText", TsValue.FromString(comment.Comment ?? string.Empty));
            value.SetField("timestamp", TsValue.FromInt32(unchecked((int)comment.Timestamp)));
            array.Add(new TsObjectValue(value));
        }

        return new TsArrayValue(array);
    }

    private static TsValue ToTsHeroStandings(IEnumerable<DotaStatsHeroStats> heroes)
    {
        var array = new TsArray();
        foreach (var hero in heroes)
        {
            var value = new TsObject("DotaHeroStanding");
            value.SetField("heroId", TsValue.FromInt32(unchecked((int)hero.HeroId)));
            value.SetField("wins", TsValue.FromInt32(unchecked((int)hero.Wins)));
            value.SetField("losses", TsValue.FromInt32(unchecked((int)hero.Losses)));
            value.SetField("winStreak", TsValue.FromInt32(unchecked((int)hero.WinStreak)));
            value.SetField("bestWinStreak", TsValue.FromInt32(unchecked((int)hero.BestWinStreak)));
            value.SetField("avgKills", TsValue.FromFloat64(hero.AvgKills));
            value.SetField("avgDeaths", TsValue.FromFloat64(hero.AvgDeaths));
            value.SetField("avgAssists", TsValue.FromFloat64(hero.AvgAssists));
            value.SetField("avgGpm", TsValue.FromFloat64(hero.AvgGpm));
            value.SetField("avgXpm", TsValue.FromFloat64(hero.AvgXpm));
            value.SetField("bestKills", TsValue.FromInt32(unchecked((int)hero.BestKills)));
            value.SetField("bestAssists", TsValue.FromInt32(unchecked((int)hero.BestAssists)));
            value.SetField("bestGpm", TsValue.FromInt32(unchecked((int)hero.BestGpm)));
            value.SetField("bestXpm", TsValue.FromInt32(unchecked((int)hero.BestXpm)));
            value.SetField("performance", TsValue.FromFloat64(hero.Performance));
            value.SetField("networthPeak", TsValue.FromInt32(unchecked((int)hero.NetworthPeak)));
            value.SetField("lasthitPeak", TsValue.FromInt32(unchecked((int)hero.LasthitPeak)));
            value.SetField("denyPeak", TsValue.FromInt32(unchecked((int)hero.DenyPeak)));
            value.SetField("damagePeak", TsValue.FromInt32(unchecked((int)hero.DamagePeak)));
            value.SetField("longestGamePeak", TsValue.FromInt32(unchecked((int)hero.LongestGamePeak)));
            value.SetField("healingPeak", TsValue.FromInt32(unchecked((int)hero.HealingPeak)));
            value.SetField("avgLasthits", TsValue.FromFloat64(hero.AvgLastHits));
            value.SetField("avgDenies", TsValue.FromFloat64(hero.AvgDenies));
            array.Add(new TsObjectValue(value));
        }

        return new TsArrayValue(array);
    }

    private static TsValue ToTsHeroGlobalData(uint heroId, DotaStatsHeroStats? hero, long timestamp)
    {
        var value = new TsObject("DotaHeroGlobalData");
        value.SetField("heroId", TsValue.FromInt32(unchecked((int)heroId)));

        var chunk = new TsObject("DotaHeroGlobalDataChunk");
        chunk.SetField("rankChunk", TsValue.FromInt32(0));

        var averages = new TsObject("DotaGlobalHeroAverages");
        averages.SetField("lastRun", TsValue.FromInt32(unchecked((int)timestamp)));
        averages.SetField("avgGoldPerMin", TsValue.FromInt32(hero == null ? 0 : unchecked((int)Math.Round(hero.AvgGpm))));
        averages.SetField("avgXpPerMin", TsValue.FromInt32(hero == null ? 0 : unchecked((int)Math.Round(hero.AvgXpm))));
        averages.SetField("avgKills", TsValue.FromInt32(hero == null ? 0 : unchecked((int)Math.Round(hero.AvgKills))));
        averages.SetField("avgDeaths", TsValue.FromInt32(hero == null ? 0 : unchecked((int)Math.Round(hero.AvgDeaths))));
        averages.SetField("avgAssists", TsValue.FromInt32(hero == null ? 0 : unchecked((int)Math.Round(hero.AvgAssists))));
        averages.SetField("avgLastHits", TsValue.FromInt32(hero == null ? 0 : unchecked((int)Math.Round(hero.AvgLastHits))));
        averages.SetField("avgDenies", TsValue.FromInt32(hero == null ? 0 : unchecked((int)Math.Round(hero.AvgDenies))));
        averages.SetField("avgNetWorth", TsValue.FromInt32(hero == null ? 0 : unchecked((int)hero.NetworthPeak)));
        chunk.SetField("heroAverages", new TsObjectValue(averages));

        var chunks = new TsArray();
        chunks.Add(new TsObjectValue(chunk));
        value.SetField("heroDataPerChunk", new TsArrayValue(chunks));
        return new TsObjectValue(value);
    }

    private static TsValue ToTsTeammateStats(IEnumerable<DotaStatsTeammate> teammates)
    {
        var array = new TsArray();
        foreach (var teammate in teammates)
        {
            var value = new TsObject("DotaTeammateStats");
            value.SetField("accountId", TsValue.FromInt32(unchecked((int)teammate.AccountId)));
            value.SetField("games", TsValue.FromInt32(unchecked((int)teammate.Games)));
            value.SetField("wins", TsValue.FromInt32(unchecked((int)teammate.Wins)));
            value.SetField("mostRecentGameTimestamp", TsValue.FromInt32(unchecked((int)teammate.MostRecentGameTimestamp)));
            value.SetField("mostRecentGameMatchId", TsValue.FromUInt64(teammate.MostRecentGameMatchId));
            var performance = teammate.Games == 0 ? 0.0 : teammate.Wins * 100.0 / teammate.Games;
            value.SetField("performance", TsValue.FromFloat64(performance));
            array.Add(new TsObjectValue(value));
        }

        return new TsArrayValue(array);
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

    private static TsObject RequireObject(TsValue value, string path)
    {
        if (value is TsObjectValue objectValue)
        {
            return objectValue.Value;
        }

        throw new InvalidOperationException($"{path}: expected object, got {value.ValueType}");
    }

    private static uint U32Field(TsObject value, string fieldName, string path, uint defaultValue = 0)
    {
        var field = value.GetField(fieldName);
        return field is TsNull or TsVoid
            ? defaultValue
            : Convert.ToUInt32(ToNumber(field, $"{path}.{fieldName}"));
    }

    private static ulong U64Field(TsObject value, string fieldName, string path, ulong defaultValue = 0)
    {
        var field = value.GetField(fieldName);
        return field is TsNull or TsVoid
            ? defaultValue
            : Convert.ToUInt64(ToInteger(field, $"{path}.{fieldName}").ToString());
    }

    private static bool BoolField(TsObject value, string fieldName, string path, bool defaultValue = false)
    {
        var field = value.GetField(fieldName);
        return field is TsNull or TsVoid
            ? defaultValue
            : ToBool(field, $"{path}.{fieldName}");
    }

    private static string StringField(TsObject value, string fieldName, string path, string defaultValue = "")
    {
        var field = value.GetField(fieldName);
        return field is TsNull or TsVoid
            ? defaultValue
            : ToString(field);
    }

    private static List<uint> UInt32ArrayField(TsObject value, string fieldName, string path)
    {
        var field = value.GetField(fieldName);
        if (field is TsNull or TsVoid)
        {
            return new List<uint>();
        }

        if (field is not TsArrayValue arrayValue)
        {
            throw new InvalidOperationException($"{path}.{fieldName}: expected array, got {field.ValueType}");
        }

        var result = new List<uint>(arrayValue.Value.Count);
        for (var i = 0; i < arrayValue.Value.Count; i++)
        {
            result.Add(Convert.ToUInt32(ToNumber(arrayValue.Value.Get(i), $"{path}.{fieldName}[{i}]")));
        }

        return result;
    }

    private static uint SteamIdToAccountId(ulong steamId)
    {
        const ulong accountIdBase = 76561197960265728UL;
        return steamId >= accountIdBase ? unchecked((uint)(steamId - accountIdBase)) : unchecked((uint)steamId);
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

    private static bool ToBool(TsValue value, string path)
    {
        return value switch
        {
            TsBoolValue boolValue => boolValue.Value,
            TsInt32Value int32Value => int32Value.Value != 0,
            TsInt64Value int64Value => int64Value.Value != 0,
            TsUInt64Value uint64Value => uint64Value.Value != 0,
            _ => throw new InvalidOperationException($"{path}: expected boolean value, got {value.ValueType}")
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

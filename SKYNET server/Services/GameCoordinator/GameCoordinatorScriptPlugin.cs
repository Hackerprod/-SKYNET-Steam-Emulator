using SKYNET_server.Models;
using System.Text.Json;
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
            _trace.Record("error", context.AppId, context.SteamId, request.MessageType, 0, ex.ToString());
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
                .RegisterHostFunction("gc", "reply", dispatcher.Reply)
                .RegisterHostFunction("gc", "dotaInventory", dispatcher.DotaInventory)
                .RegisterHostFunction("gc", "dotaCatalogItem", dispatcher.DotaCatalogItem)
                .RegisterHostFunction("gc", "dotaEquipItem", dispatcher.DotaEquipItem)
                .RegisterHostFunction("gc", "dotaSetItemStyle", dispatcher.DotaSetItemStyle)
                .RegisterHostFunction("gc", "dotaProfile", dispatcher.DotaProfile)
                .RegisterHostFunction("gc", "dotaSaveProfileSlots", dispatcher.DotaSaveProfileSlots)
                .RegisterHostFunction("gc", "dotaSaveProfileUpdate", dispatcher.DotaSaveProfileUpdate)
                .RegisterHostFunction("gc", "dotaProfileConductScorecard", dispatcher.DotaProfileConductScorecard)
                .RegisterHostFunction("gc", "dotaProfileQuestProgress", dispatcher.DotaProfileQuestProgress)
                .RegisterHostFunction("gc", "dotaProfilePeriodicResource", dispatcher.DotaProfilePeriodicResource)
                .RegisterHostFunction("gc", "dotaProfileHeroStickers", dispatcher.DotaProfileHeroStickers)
                .RegisterHostFunction("gc", "dotaProfileSetHeroSticker", dispatcher.DotaProfileSetHeroSticker)
                .RegisterHostFunction("gc", "dotaProfileOverworldState", dispatcher.DotaProfileOverworldState)
                .RegisterHostFunction("gc", "dotaProfileMonsterHunterState", dispatcher.DotaProfileMonsterHunterState)
                .RegisterHostFunction("gc", "dotaSocialEmoticonAccess", _ => dispatcher.RequireCurrent().DotaSocialEmoticonAccess())
                .RegisterHostFunction("gc", "dotaSocialFeed", dispatcher.DotaSocialFeed)
                .RegisterHostFunction("gc", "dotaSocialFeedComments", dispatcher.DotaSocialFeedComments)
                .RegisterHostFunction("gc", "dotaSocialFeedPostComment", dispatcher.DotaSocialFeedPostComment)
                .RegisterHostFunction("gc", "dotaSocialMatchComments", dispatcher.DotaSocialMatchComments)
                .RegisterHostFunction("gc", "dotaSocialMatchPostComment", dispatcher.DotaSocialMatchPostComment)
                .RegisterHostFunction("gc", "dotaChatChannels", dispatcher.DotaChatChannels)
                .RegisterHostFunction("gc", "dotaChatJoinChannel", dispatcher.DotaChatJoinChannel)
                .RegisterHostFunction("gc", "dotaChatChannel", dispatcher.DotaChatChannel)
                .RegisterHostFunction("gc", "dotaChatLeaveChannel", dispatcher.DotaChatLeaveChannel)
                .RegisterHostFunction("gc", "dotaChatBroadcast", dispatcher.DotaChatBroadcast)
                .RegisterHostFunction("gc", "dotaGuildEnsureCurrent", dispatcher.DotaGuildEnsureCurrent)
                .RegisterHostFunction("gc", "dotaGuildMembership", dispatcher.DotaGuildMembership)
                .RegisterHostFunction("gc", "dotaGuild", dispatcher.DotaGuild)
                .RegisterHostFunction("gc", "dotaGuildPersonaInfo", dispatcher.DotaGuildPersonaInfo)
                .RegisterHostFunction("gc", "dotaGuildEventData", dispatcher.DotaGuildEventData)
                .RegisterHostFunction("gc", "dotaGuildInvite", dispatcher.DotaGuildInvite)
                .RegisterHostFunction("gc", "dotaGuildDeclineInvite", dispatcher.DotaGuildDeclineInvite)
                .RegisterHostFunction("gc", "dotaGuildCancelInvite", dispatcher.DotaGuildCancelInvite)
                .RegisterHostFunction("gc", "dotaGuildAcceptInvite", dispatcher.DotaGuildAcceptInvite)
                .RegisterHostFunction("gc", "dotaGuildLeave", dispatcher.DotaGuildLeave)
                .RegisterHostFunction("gc", "dotaReporterUpdates", dispatcher.DotaReporterUpdates)
                .RegisterHostFunction("gc", "dotaAcknowledgeReporterUpdates", dispatcher.DotaAcknowledgeReporterUpdates)
                .RegisterHostFunction("gc", "dotaTeam", dispatcher.DotaTeam)
                .RegisterHostFunction("gc", "dotaTeamsForAccount", dispatcher.DotaTeamsForAccount)
                .RegisterHostFunction("gc", "dotaNextTeamId", dispatcher.DotaNextTeamId)
                .RegisterHostFunction("gc", "dotaUpsertTeam", dispatcher.DotaUpsertTeam)
                .RegisterHostFunction("gc", "dotaAddTeamMember", dispatcher.DotaAddTeamMember)
                .RegisterHostFunction("gc", "dotaRemoveTeamMember", dispatcher.DotaRemoveTeamMember)
                .RegisterHostFunction("gc", "dotaRemoveTeam", dispatcher.DotaRemoveTeam)
                .RegisterHostFunction("gc", "dotaTeamNameAvailable", dispatcher.DotaTeamNameAvailable)
                .RegisterHostFunction("gc", "dotaTeamTagAvailable", dispatcher.DotaTeamTagAvailable)
                .RegisterHostFunction("gc", "dotaTeamPlayerInfo", dispatcher.DotaTeamPlayerInfo)
                .RegisterHostFunction("gc", "dotaUpsertTeamPlayerInfo", dispatcher.DotaUpsertTeamPlayerInfo)
                .RegisterHostFunction("gc", "dotaDeleteTeamPlayerInfo", dispatcher.DotaDeleteTeamPlayerInfo)
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
                .RegisterHostFunction("gc", "dotaMatchVotes", dispatcher.DotaMatchVotes)
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
                .RegisterHostFunction("gc", "dotaSavePlayerReport", dispatcher.DotaSavePlayerReport)
                .RegisterHostFunction("gc", "dotaPartyCurrent", _ => dispatcher.RequireCurrent().DotaPartyCurrent())
                .RegisterHostFunction("gc", "dotaPartyById", dispatcher.DotaPartyById)
                .RegisterHostFunction("gc", "dotaPartyEnsureCurrent", dispatcher.DotaPartyEnsureCurrent)
                .RegisterHostFunction("gc", "dotaPartyAddMember", dispatcher.DotaPartyAddMember)
                .RegisterHostFunction("gc", "dotaPartyRemoveMember", dispatcher.DotaPartyRemoveMember)
                .RegisterHostFunction("gc", "dotaPartyDelete", dispatcher.DotaPartyDelete)
                .RegisterHostFunction("gc", "dotaPartySetLeader", dispatcher.DotaPartySetLeader)
                .RegisterHostFunction("gc", "dotaPartySetCoach", dispatcher.DotaPartySetCoach)
                .RegisterHostFunction("gc", "dotaPartySetPing", dispatcher.DotaPartySetPing)
                .RegisterHostFunction("gc", "dotaPartyStartReadyCheck", dispatcher.DotaPartyStartReadyCheck)
                .RegisterHostFunction("gc", "dotaPartyAcknowledgeReadyCheck", dispatcher.DotaPartyAcknowledgeReadyCheck)
                .RegisterHostFunction("gc", "dotaPartyCreateInvite", dispatcher.DotaPartyCreateInvite)
                .RegisterHostFunction("gc", "dotaPartyTakeInvite", dispatcher.DotaPartyTakeInvite)
                .RegisterHostFunction("gc", "dotaPartyInvitesForTarget", dispatcher.DotaPartyInvitesForTarget)
                .RegisterHostFunction("gc", "dotaPartyDeleteInvitesForTarget", dispatcher.DotaPartyDeleteInvitesForTarget)
                .RegisterHostFunction("gc", "dotaPartyDeleteInvitesForParty", dispatcher.DotaPartyDeleteInvitesForParty)
                .RegisterHostFunction("gc", "dotaPartyPruneInvitesCreatedBefore", dispatcher.DotaPartyPruneInvitesCreatedBefore)
                .RegisterHostFunction("gc", "dotaPartyUserExists", dispatcher.DotaPartyUserExists)
                .RegisterHostFunction("gc", "dotaPartyUserOnline", dispatcher.DotaPartyUserOnline)
                .RegisterHostFunction("gc", "dotaQueueGcMessage", dispatcher.DotaQueueGcMessage)
                .RegisterHostFunction("gc", "dotaPublishMatchSnapshot", dispatcher.DotaPublishMatchSnapshot)
                .RegisterHostFunction("gc", "dotaListMatchSnapshots", dispatcher.DotaListMatchSnapshots)
                .RegisterHostFunction("gc", "dotaRemoveMatchSnapshot", dispatcher.DotaRemoveMatchSnapshot)
                .RegisterHostFunction("gc", "dotaStartDedicatedServer", dispatcher.DotaStartDedicatedServer)
                .RegisterHostFunction("gc", "dotaReleaseDedicatedServer", dispatcher.DotaReleaseDedicatedServer)
                .RegisterHostFunction("gc", "dotaResolveGameServerConnectIp", dispatcher.DotaResolveGameServerConnectIp)
                .RegisterHostFunction("gc", "dotaResolveGameServerConnectIps", dispatcher.DotaResolveGameServerConnectIps);

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

    public TsValue? Reply(TsValue[] args)
    {
        return RequireCurrent().Reply(args);
    }

    public TsValue? DotaEquipItem(TsValue[] args)
    {
        return RequireCurrent().DotaEquipItem(args);
    }

    public TsValue? DotaCatalogItem(TsValue[] args)
    {
        return RequireCurrent().DotaCatalogItem(args);
    }

    public TsValue? DotaInventory(TsValue[] args)
    {
        return RequireCurrent().DotaInventory(args);
    }

    public TsValue? DotaSetItemStyle(TsValue[] args)
    {
        return RequireCurrent().DotaSetItemStyle(args);
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

    public TsValue? DotaProfileConductScorecard(TsValue[] args)
    {
        return RequireCurrent().DotaProfileConductScorecard();
    }

    public TsValue? DotaProfileQuestProgress(TsValue[] args)
    {
        return RequireCurrent().DotaProfileQuestProgress(args);
    }

    public TsValue? DotaProfilePeriodicResource(TsValue[] args)
    {
        return RequireCurrent().DotaProfilePeriodicResource(args);
    }

    public TsValue? DotaProfileHeroStickers(TsValue[] args)
    {
        return RequireCurrent().DotaProfileHeroStickers();
    }

    public TsValue? DotaProfileSetHeroSticker(TsValue[] args)
    {
        return RequireCurrent().DotaProfileSetHeroSticker(args);
    }

    public TsValue? DotaProfileOverworldState(TsValue[] args)
    {
        return RequireCurrent().DotaProfileOverworldState(args);
    }

    public TsValue? DotaProfileMonsterHunterState(TsValue[] args)
    {
        return RequireCurrent().DotaProfileMonsterHunterState();
    }

    public TsValue? DotaSocialFeed(TsValue[] args)
    {
        return RequireCurrent().DotaSocialFeed(args);
    }

    public TsValue? DotaSocialMatchPostComment(TsValue[] args)
    {
        return RequireCurrent().DotaSocialMatchPostComment(args);
    }

    public TsValue? DotaSocialFeedComments(TsValue[] args)
    {
        return RequireCurrent().DotaSocialFeedComments(args);
    }

    public TsValue? DotaSocialMatchComments(TsValue[] args)
    {
        return RequireCurrent().DotaSocialMatchComments(args);
    }

    public TsValue? DotaSocialFeedPostComment(TsValue[] args)
    {
        return RequireCurrent().DotaSocialFeedPostComment(args);
    }

    public TsValue? DotaChatJoinChannel(TsValue[] args)
    {
        return RequireCurrent().DotaChatJoinChannel(args);
    }

    public TsValue? DotaChatChannels(TsValue[] args)
    {
        return RequireCurrent().DotaChatChannels();
    }

    public TsValue? DotaChatChannel(TsValue[] args)
    {
        return RequireCurrent().DotaChatChannel(args);
    }

    public TsValue? DotaChatLeaveChannel(TsValue[] args)
    {
        return RequireCurrent().DotaChatLeaveChannel(args);
    }

    public TsValue? DotaChatBroadcast(TsValue[] args)
    {
        return RequireCurrent().DotaChatBroadcast(args);
    }

    public TsValue? DotaGuildEnsureCurrent(TsValue[] args)
    {
        return RequireCurrent().DotaGuildEnsureCurrent();
    }

    public TsValue? DotaGuildMembership(TsValue[] args)
    {
        return RequireCurrent().DotaGuildMembership(args);
    }

    public TsValue? DotaGuild(TsValue[] args)
    {
        return RequireCurrent().DotaGuild(args);
    }

    public TsValue? DotaGuildPersonaInfo(TsValue[] args)
    {
        return RequireCurrent().DotaGuildPersonaInfo(args);
    }

    public TsValue? DotaGuildEventData(TsValue[] args)
    {
        return RequireCurrent().DotaGuildEventData(args);
    }

    public TsValue? DotaGuildInvite(TsValue[] args)
    {
        return RequireCurrent().DotaGuildInvite(args);
    }

    public TsValue? DotaGuildDeclineInvite(TsValue[] args)
    {
        return RequireCurrent().DotaGuildDeclineInvite(args);
    }

    public TsValue? DotaGuildCancelInvite(TsValue[] args)
    {
        return RequireCurrent().DotaGuildCancelInvite(args);
    }

    public TsValue? DotaGuildAcceptInvite(TsValue[] args)
    {
        return RequireCurrent().DotaGuildAcceptInvite(args);
    }

    public TsValue? DotaGuildLeave(TsValue[] args)
    {
        return RequireCurrent().DotaGuildLeave(args);
    }

    public TsValue? DotaReporterUpdates(TsValue[] args)
    {
        return RequireCurrent().DotaReporterUpdates();
    }

    public TsValue? DotaAcknowledgeReporterUpdates(TsValue[] args)
    {
        return RequireCurrent().DotaAcknowledgeReporterUpdates(args);
    }

    public TsValue? DotaTeam(TsValue[] args)
    {
        return RequireCurrent().DotaTeam(args);
    }

    public TsValue? DotaTeamsForAccount(TsValue[] args)
    {
        return RequireCurrent().DotaTeamsForAccount(args);
    }

    public TsValue? DotaNextTeamId(TsValue[] args)
    {
        return RequireCurrent().DotaNextTeamId();
    }

    public TsValue? DotaUpsertTeam(TsValue[] args)
    {
        return RequireCurrent().DotaUpsertTeam(args);
    }

    public TsValue? DotaAddTeamMember(TsValue[] args)
    {
        return RequireCurrent().DotaAddTeamMember(args);
    }

    public TsValue? DotaRemoveTeamMember(TsValue[] args)
    {
        return RequireCurrent().DotaRemoveTeamMember(args);
    }

    public TsValue? DotaRemoveTeam(TsValue[] args)
    {
        return RequireCurrent().DotaRemoveTeam(args);
    }

    public TsValue? DotaTeamNameAvailable(TsValue[] args)
    {
        return RequireCurrent().DotaTeamNameAvailable(args);
    }

    public TsValue? DotaTeamTagAvailable(TsValue[] args)
    {
        return RequireCurrent().DotaTeamTagAvailable(args);
    }

    public TsValue? DotaTeamPlayerInfo(TsValue[] args)
    {
        return RequireCurrent().DotaTeamPlayerInfo(args);
    }

    public TsValue? DotaUpsertTeamPlayerInfo(TsValue[] args)
    {
        return RequireCurrent().DotaUpsertTeamPlayerInfo(args);
    }

    public TsValue? DotaDeleteTeamPlayerInfo(TsValue[] args)
    {
        return RequireCurrent().DotaDeleteTeamPlayerInfo(args);
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

    public TsValue? DotaMatchVotes(TsValue[] args)
    {
        return RequireCurrent().DotaMatchVotes(args);
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

    public TsValue? DotaPartyById(TsValue[] args)
    {
        return RequireCurrent().DotaPartyById(args);
    }

    public TsValue? DotaPartyEnsureCurrent(TsValue[] args)
    {
        return RequireCurrent().DotaPartyEnsureCurrent(args);
    }

    public TsValue? DotaPartyAddMember(TsValue[] args)
    {
        return RequireCurrent().DotaPartyAddMember(args);
    }

    public TsValue? DotaPartyRemoveMember(TsValue[] args)
    {
        return RequireCurrent().DotaPartyRemoveMember(args);
    }

    public TsValue? DotaPartyDelete(TsValue[] args)
    {
        return RequireCurrent().DotaPartyDelete(args);
    }

    public TsValue? DotaPartySetLeader(TsValue[] args)
    {
        return RequireCurrent().DotaPartySetLeader(args);
    }

    public TsValue? DotaPartySetCoach(TsValue[] args)
    {
        return RequireCurrent().DotaPartySetCoach(args);
    }

    public TsValue? DotaPartySetPing(TsValue[] args)
    {
        return RequireCurrent().DotaPartySetPing(args);
    }

    public TsValue? DotaPartyStartReadyCheck(TsValue[] args)
    {
        return RequireCurrent().DotaPartyStartReadyCheck(args);
    }

    public TsValue? DotaPartyAcknowledgeReadyCheck(TsValue[] args)
    {
        return RequireCurrent().DotaPartyAcknowledgeReadyCheck(args);
    }

    public TsValue? DotaPartyCreateInvite(TsValue[] args)
    {
        return RequireCurrent().DotaPartyCreateInvite(args);
    }

    public TsValue? DotaPartyTakeInvite(TsValue[] args)
    {
        return RequireCurrent().DotaPartyTakeInvite(args);
    }

    public TsValue? DotaPartyInvitesForTarget(TsValue[] args)
    {
        return RequireCurrent().DotaPartyInvitesForTarget(args);
    }

    public TsValue? DotaPartyDeleteInvitesForTarget(TsValue[] args)
    {
        return RequireCurrent().DotaPartyDeleteInvitesForTarget(args);
    }

    public TsValue? DotaPartyDeleteInvitesForParty(TsValue[] args)
    {
        return RequireCurrent().DotaPartyDeleteInvitesForParty(args);
    }

    public TsValue? DotaPartyPruneInvitesCreatedBefore(TsValue[] args)
    {
        return RequireCurrent().DotaPartyPruneInvitesCreatedBefore(args);
    }

    public TsValue? DotaPartyUserExists(TsValue[] args)
    {
        return RequireCurrent().DotaPartyUserExists(args);
    }

    public TsValue? DotaPartyUserOnline(TsValue[] args)
    {
        return RequireCurrent().DotaPartyUserOnline(args);
    }

    public TsValue? DotaQueueGcMessage(TsValue[] args)
    {
        return RequireCurrent().DotaQueueGcMessage(args);
    }

    public TsValue? DotaPublishMatchSnapshot(TsValue[] args)
    {
        return RequireCurrent().DotaPublishMatchSnapshot(args);
    }

    public TsValue? DotaListMatchSnapshots(TsValue[] args)
    {
        return RequireCurrent().DotaListMatchSnapshots();
    }

    public TsValue? DotaRemoveMatchSnapshot(TsValue[] args)
    {
        return RequireCurrent().DotaRemoveMatchSnapshot(args);
    }

    public TsValue? DotaStartDedicatedServer(TsValue[] args)
    {
        return RequireCurrent().DotaStartDedicatedServer(args);
    }

    public TsValue? DotaReleaseDedicatedServer(TsValue[] args)
    {
        return RequireCurrent().DotaReleaseDedicatedServer(args);
    }

    public TsValue? DotaResolveGameServerConnectIp(TsValue[] args)
    {
        return RequireCurrent().DotaResolveGameServerConnectIp(args);
    }

    public TsValue? DotaResolveGameServerConnectIps(TsValue[] args)
    {
        return RequireCurrent().DotaResolveGameServerConnectIps(args);
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

        AddMessage(args, replyToCurrentJob: false, "send");
        return TsValue.FromBool(true);
    }

    public TsValue? Reply(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("reply(messageType, payload, protobuf?) requires at least two arguments");
        }

        AddMessage(args, replyToCurrentJob: true, "reply");
        return TsValue.FromBool(true);
    }

    private void AddMessage(TsValue[] args, bool replyToCurrentJob, string functionName)
    {
        var messageType = Convert.ToUInt32(ToNumber(args[0], $"{functionName}.messageType"));
        var payload = ToBytes(args[1], $"{functionName}.payload");
        var protobuf = args.Length < 3 || args[2] is not TsBoolValue boolValue || boolValue.Value;
        var sourceJobId = _request.SourceJobId;
        var targetJobId = replyToCurrentJob
            && sourceJobId.HasValue
            && sourceJobId.Value != 0
            && sourceJobId.Value != ulong.MaxValue
                ? sourceJobId.Value
                : (ulong?)null;

        Response.Messages.Add(new ApiGCMessage
        {
            AppId = _context.AppId,
            MessageType = messageType,
            PayloadBase64 = Convert.ToBase64String(payload),
            Protobuf = protobuf,
            TargetJobId = targetJobId
        });
    }

    public TsValue DotaPartyCurrent()
    {
        return ToTsPartyState(DotaGcRuntimeServices.PartyStore?.GetPartyByMember(_context.SteamId));
    }

    public TsValue DotaPartyById(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaPartyById(partyId) requires one argument");
        }

        var partyId = Convert.ToUInt64(ToInteger(args[0], "dotaPartyById.partyId").ToString());
        return ToTsPartyState(DotaGcRuntimeServices.PartyStore?.GetParty(partyId));
    }

    public TsValue DotaPartyEnsureCurrent(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaPartyEnsureCurrent(ping) requires one argument");
        }

        var store = DotaGcRuntimeServices.PartyStore ?? throw new InvalidOperationException("Dota party store is not initialized.");
        return ToTsPartyState(store.EnsureParty(_context.SteamId, _context.AccountId, _context.PersonaName, ToPartyPingData(args[0], "dotaPartyEnsureCurrent.ping")));
    }

    public TsValue DotaPartyAddMember(TsValue[] args)
    {
        if (args.Length < 3)
        {
            throw new InvalidOperationException("dotaPartyAddMember(partyId, ping, isCoach) requires three arguments");
        }

        var partyId = Convert.ToUInt64(ToInteger(args[0], "dotaPartyAddMember.partyId").ToString());
        var ping = ToPartyPingData(args[1], "dotaPartyAddMember.ping");
        var isCoach = ToBool(args[2], "dotaPartyAddMember.isCoach");
        return ToTsPartyState(DotaGcRuntimeServices.PartyStore?.AddMember(partyId, _context.SteamId, _context.AccountId, _context.PersonaName, ping, isCoach));
    }

    public TsValue DotaPartyRemoveMember(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaPartyRemoveMember(partyId, steamId) requires two arguments");
        }

        var partyId = Convert.ToUInt64(ToInteger(args[0], "dotaPartyRemoveMember.partyId").ToString());
        var steamId = Convert.ToUInt64(ToInteger(args[1], "dotaPartyRemoveMember.steamId").ToString());
        return ToTsPartyState(DotaGcRuntimeServices.PartyStore?.RemoveMember(partyId, steamId));
    }

    public TsValue DotaPartyDelete(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaPartyDelete(partyId) requires one argument");
        }

        var partyId = Convert.ToUInt64(ToInteger(args[0], "dotaPartyDelete.partyId").ToString());
        DotaGcRuntimeServices.PartyStore?.DeleteParty(partyId);
        return TsValue.FromBool(DotaGcRuntimeServices.PartyStore != null);
    }

    public TsValue DotaPartySetLeader(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaPartySetLeader(partyId, leaderSteamId) requires two arguments");
        }

        var partyId = Convert.ToUInt64(ToInteger(args[0], "dotaPartySetLeader.partyId").ToString());
        var leaderSteamId = Convert.ToUInt64(ToInteger(args[1], "dotaPartySetLeader.leaderSteamId").ToString());
        return ToTsPartyState(DotaGcRuntimeServices.PartyStore?.SetLeader(partyId, leaderSteamId));
    }

    public TsValue DotaPartySetCoach(TsValue[] args)
    {
        if (args.Length < 3)
        {
            throw new InvalidOperationException("dotaPartySetCoach(partyId, steamId, isCoach) requires three arguments");
        }

        var partyId = Convert.ToUInt64(ToInteger(args[0], "dotaPartySetCoach.partyId").ToString());
        var steamId = Convert.ToUInt64(ToInteger(args[1], "dotaPartySetCoach.steamId").ToString());
        var isCoach = ToBool(args[2], "dotaPartySetCoach.isCoach");
        return ToTsPartyState(DotaGcRuntimeServices.PartyStore?.SetMemberCoach(partyId, steamId, isCoach));
    }

    public TsValue DotaPartySetPing(TsValue[] args)
    {
        if (args.Length < 3)
        {
            throw new InvalidOperationException("dotaPartySetPing(partyId, steamId, ping) requires three arguments");
        }

        var partyId = Convert.ToUInt64(ToInteger(args[0], "dotaPartySetPing.partyId").ToString());
        var steamId = Convert.ToUInt64(ToInteger(args[1], "dotaPartySetPing.steamId").ToString());
        var ping = ToPartyPingData(args[2], "dotaPartySetPing.ping");
        return ToTsPartyState(DotaGcRuntimeServices.PartyStore?.SetMemberPing(partyId, steamId, ping));
    }

    public TsValue DotaPartyStartReadyCheck(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaPartyStartReadyCheck(partyId, durationSeconds) requires two arguments");
        }

        var partyId = Convert.ToUInt64(ToInteger(args[0], "dotaPartyStartReadyCheck.partyId").ToString());
        var durationSeconds = Convert.ToUInt32(ToNumber(args[1], "dotaPartyStartReadyCheck.durationSeconds"));
        return ToTsPartyState(DotaGcRuntimeServices.PartyStore?.StartReadyCheck(partyId, _context.AccountId, durationSeconds));
    }

    public TsValue DotaPartyAcknowledgeReadyCheck(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaPartyAcknowledgeReadyCheck(partyId, readyStatus) requires two arguments");
        }

        var partyId = Convert.ToUInt64(ToInteger(args[0], "dotaPartyAcknowledgeReadyCheck.partyId").ToString());
        var readyStatus = Convert.ToUInt32(ToNumber(args[1], "dotaPartyAcknowledgeReadyCheck.readyStatus"));
        return ToTsPartyState(DotaGcRuntimeServices.PartyStore?.AcknowledgeReadyCheck(partyId, _context.SteamId, readyStatus));
    }

    public TsValue DotaPartyCreateInvite(TsValue[] args)
    {
        if (args.Length < 4)
        {
            throw new InvalidOperationException("dotaPartyCreateInvite(partyId, targetSteamId, teamId, asCoach) requires four arguments");
        }

        var store = DotaGcRuntimeServices.PartyStore;
        if (store == null)
        {
            return TsValue.Null;
        }

        var partyId = Convert.ToUInt64(ToInteger(args[0], "dotaPartyCreateInvite.partyId").ToString());
        var targetSteamId = Convert.ToUInt64(ToInteger(args[1], "dotaPartyCreateInvite.targetSteamId").ToString());
        var teamId = Convert.ToUInt32(ToNumber(args[2], "dotaPartyCreateInvite.teamId"));
        var asCoach = ToBool(args[3], "dotaPartyCreateInvite.asCoach");
        var invite = store.CreateInvite(partyId, targetSteamId, _context.SteamId, _context.PersonaName, teamId, asCoach, out var replaced);

        var value = new TsObject("DotaPartyInviteMutation");
        value.SetField("invite", ToTsPartyInvite(invite));
        value.SetField("replaced", ToTsPartyInvite(replaced));
        return new TsObjectValue(value);
    }

    public TsValue DotaPartyTakeInvite(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaPartyTakeInvite(partyId) requires one argument");
        }

        var partyId = Convert.ToUInt64(ToInteger(args[0], "dotaPartyTakeInvite.partyId").ToString());
        return ToTsPartyInvite(DotaGcRuntimeServices.PartyStore?.TakeInvite(partyId, _context.SteamId, PartyInviteCutoff()));
    }

    public TsValue DotaPartyInvitesForTarget(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaPartyInvitesForTarget(targetSteamId) requires one argument");
        }

        var targetSteamId = Convert.ToUInt64(ToInteger(args[0], "dotaPartyInvitesForTarget.targetSteamId").ToString());
        return ToTsPartyInvites(DotaGcRuntimeServices.PartyStore?.GetInvitesForTarget(targetSteamId, PartyInviteCutoff()) ?? Array.Empty<DotaPartyInvite>());
    }

    public TsValue DotaPartyDeleteInvitesForTarget(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaPartyDeleteInvitesForTarget(targetSteamId) requires one argument");
        }

        var targetSteamId = Convert.ToUInt64(ToInteger(args[0], "dotaPartyDeleteInvitesForTarget.targetSteamId").ToString());
        return ToTsPartyInvites(DotaGcRuntimeServices.PartyStore?.DeleteInvitesForTarget(targetSteamId) ?? Array.Empty<DotaPartyInvite>());
    }

    public TsValue DotaPartyDeleteInvitesForParty(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaPartyDeleteInvitesForParty(partyId) requires one argument");
        }

        var partyId = Convert.ToUInt64(ToInteger(args[0], "dotaPartyDeleteInvitesForParty.partyId").ToString());
        return ToTsPartyInvites(DotaGcRuntimeServices.PartyStore?.DeleteInvitesForParty(partyId) ?? Array.Empty<DotaPartyInvite>());
    }

    public TsValue DotaPartyPruneInvitesCreatedBefore(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaPartyPruneInvitesCreatedBefore(cutoff) requires one argument");
        }

        var cutoff = Convert.ToUInt32(ToNumber(args[0], "dotaPartyPruneInvitesCreatedBefore.cutoff"));
        return ToTsPartyInvites(DotaGcRuntimeServices.PartyStore?.PruneInvitesCreatedBefore(cutoff) ?? Array.Empty<DotaPartyInvite>());
    }

    public TsValue DotaPartyUserExists(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaPartyUserExists(steamId) requires one argument");
        }

        var steamId = Convert.ToUInt64(ToInteger(args[0], "dotaPartyUserExists.steamId").ToString());
        return TsValue.FromBool(DotaGcRuntimeServices.UserExistsProvider?.Invoke(steamId) ?? true);
    }

    public TsValue DotaPartyUserOnline(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaPartyUserOnline(steamId) requires one argument");
        }

        var steamId = Convert.ToUInt64(ToInteger(args[0], "dotaPartyUserOnline.steamId").ToString());
        return TsValue.FromBool(DotaGcRuntimeServices.UserOnlineProvider?.Invoke(steamId) ?? true);
    }

    public TsValue DotaQueueGcMessage(TsValue[] args)
    {
        if (args.Length < 3)
        {
            throw new InvalidOperationException("dotaQueueGcMessage(steamId, messageType, payload, protobuf?) requires at least three arguments");
        }

        var steamId = Convert.ToUInt64(ToInteger(args[0], "dotaQueueGcMessage.steamId").ToString());
        var messageType = Convert.ToUInt32(ToNumber(args[1], "dotaQueueGcMessage.messageType"));
        var payload = ToBytes(args[2], "dotaQueueGcMessage.payload");
        var protobuf = args.Length < 4 || ToBool(args[3], "dotaQueueGcMessage.protobuf");
        var message = new ApiGCMessage
        {
            AppId = _context.AppId,
            MessageType = messageType,
            PayloadBase64 = Convert.ToBase64String(payload),
            Protobuf = protobuf
        };

        if (steamId == 0 || steamId == _context.SteamId)
        {
            Response.Messages.Add(message);
            return TsValue.FromBool(true);
        }

        DotaGcRuntimeServices.PendingMessageQueued?.Invoke(steamId, message);
        return TsValue.FromBool(DotaGcRuntimeServices.PendingMessageQueued != null);
    }

    public TsValue DotaPublishMatchSnapshot(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaPublishMatchSnapshot(snapshot) requires one argument");
        }

        if (DotaGcRuntimeServices.MatchSnapshotSink == null)
        {
            return TsValue.FromBool(false);
        }

        DotaGcRuntimeServices.MatchSnapshotSink(ToDotaMatchSnapshot(args[0], "dotaPublishMatchSnapshot.snapshot"));
        return TsValue.FromBool(true);
    }

    public TsValue DotaListMatchSnapshots()
    {
        var array = new TsArray();
        foreach (var snapshot in DotaGcRuntimeServices.MatchSnapshotsProvider?.Invoke() ?? Array.Empty<ApiDotaMatch>())
        {
            array.Add(ToTsDotaMatchSnapshot(snapshot));
        }

        return new TsArrayValue(array);
    }

    public TsValue DotaRemoveMatchSnapshot(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaRemoveMatchSnapshot(lobbyId) requires one argument");
        }

        var lobbyId = Convert.ToUInt64(ToInteger(args[0], "dotaRemoveMatchSnapshot.lobbyId").ToString());
        return TsValue.FromBool(DotaGcRuntimeServices.MatchSnapshotDeleteSink?.Invoke(lobbyId) ?? false);
    }

    public TsValue DotaStartDedicatedServer(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaStartDedicatedServer(lobbyId, map) requires two arguments");
        }

        var lobbyId = Convert.ToUInt64(ToInteger(args[0], "dotaStartDedicatedServer.lobbyId").ToString());
        var map = ToString(args[1]);
        var result = DotaGcRuntimeServices.DedicatedServerStart?.Invoke(lobbyId, map);
        if (result == null)
        {
            return TsValue.Null;
        }

        var value = new TsObject("DotaDedicatedLaunchResult");
        value.SetField("started", TsValue.FromBool(result.Started));
        value.SetField("port", TsValue.FromInt32(result.Port));
        value.SetField("state", TsValue.FromString(result.State.ToString()));
        value.SetField("error", TsValue.FromString(result.Error ?? string.Empty));
        return new TsObjectValue(value);
    }

    public TsValue DotaReleaseDedicatedServer(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaReleaseDedicatedServer(lobbyId, reason) requires two arguments");
        }

        var lobbyId = Convert.ToUInt64(ToInteger(args[0], "dotaReleaseDedicatedServer.lobbyId").ToString());
        var reason = ToString(args[1]);
        return TsValue.FromBool(DotaGcRuntimeServices.DedicatedServerRelease?.Invoke(lobbyId, reason) ?? false);
    }

    public TsValue DotaResolveGameServerConnectIp(TsValue[] args)
    {
        if (args.Length < 3)
        {
            throw new InvalidOperationException("dotaResolveGameServerConnectIp(publicIp, privateIp, fallbackIp) requires three arguments");
        }

        var publicIp = ToString(args[0]);
        var privateIp = ToString(args[1]);
        var fallbackIp = ToString(args[2]);
        var resolved = DotaGcRuntimeServices.GameServerConnectIpResolver?.Invoke(
            _context.ClientIp,
            publicIp,
            privateIp,
            fallbackIp) ?? fallbackIp;
        return TsValue.FromString(resolved);
    }

    public TsValue DotaResolveGameServerConnectIps(TsValue[] args)
    {
        if (args.Length < 3)
        {
            throw new InvalidOperationException("dotaResolveGameServerConnectIps(publicIp, privateIp, fallbackIp) requires three arguments");
        }

        var publicIp = ToString(args[0]);
        var privateIp = ToString(args[1]);
        var fallbackIp = ToString(args[2]);
        var resolved = DotaGcRuntimeServices.GameServerConnectIpsResolver?.Invoke(
            _context.ClientIp,
            publicIp,
            privateIp,
            fallbackIp);
        if (string.IsNullOrWhiteSpace(resolved))
        {
            resolved = BuildDefaultConnectIps(publicIp, privateIp, fallbackIp);
        }

        return TsValue.FromString(resolved);
    }

    private static string BuildDefaultConnectIps(string publicIp, string privateIp, string fallbackIp)
    {
        var ordered = new List<string>();
        void Add(string value)
        {
            var trimmed = (value ?? string.Empty).Trim();
            if (trimmed.Length == 0 || ordered.Contains(trimmed, StringComparer.Ordinal))
            {
                return;
            }

            ordered.Add(trimmed);
        }

        Add(publicIp);
        Add(privateIp);
        Add(fallbackIp);
        if (ordered.Count == 0)
        {
            ordered.Add("127.0.0.1");
        }

        return string.Join(' ', ordered.Take(2));
    }

    public TsValue DotaInventory(TsValue[] args)
    {
        var steamId = args.Length > 0
            ? Convert.ToUInt64(ToInteger(args[0], "dotaInventory.steamId").ToString())
            : _context.SteamId;
        if (steamId == 0)
        {
            steamId = _context.SteamId;
        }

        return ToTsInventory(DotaGcRuntimeServices.InventoryProvider?.Invoke(steamId));
    }

    public TsValue DotaCatalogItem(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaCatalogItem(defIndex) requires one argument");
        }

        var defIndex = Convert.ToUInt32(ToNumber(args[0], "dotaCatalogItem.defIndex"));
        var catalog = DotaGcRuntimeServices.InventoryProvider?.Invoke(_context.SteamId);
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
        var changed = DotaGcRuntimeServices.EquipItemSink?.Invoke(_context.SteamId, itemId, heroId, slotId, style)
            ?? new List<ApiDotaEquipment>();
        return ToTsEquipmentList(changed);
    }

    private static ulong ResolveStoredDotaItemDefIndex(ulong steamId, ulong itemId)
    {
        if (itemId == 0)
        {
            return 0;
        }

        var inventory = DotaGcRuntimeServices.InventoryProvider?.Invoke(steamId);
        if (inventory == null)
        {
            return itemId;
        }

        foreach (var item in inventory.Items.Concat(inventory.OwnedItems))
        {
            if (item.DefIndex == itemId)
            {
                return item.DefIndex;
            }

            if (BuildDotaItemInstanceId(steamId, item.DefIndex) == itemId)
            {
                return item.DefIndex;
            }
        }

        return itemId <= uint.MaxValue ? itemId : 0;
    }

    private static ulong? ResolveOwnedDotaItemDefIndex(ulong steamId, ulong itemId)
    {
        if (itemId == 0)
        {
            return null;
        }

        var inventory = DotaGcRuntimeServices.InventoryProvider?.Invoke(steamId);
        if (inventory == null)
        {
            return itemId <= uint.MaxValue ? itemId : null;
        }

        // The profile update protobuf carries an econ item instance id. The
        // legacy GC accepts it only if it belongs to the caller, then persists
        // the item's defIndex so future profile responses can rebuild the full
        // CSOEconItem from the current inventory.
        foreach (var item in inventory.Items.Concat(inventory.OwnedItems))
        {
            if (item.DefIndex == itemId || BuildDotaItemInstanceId(steamId, item.DefIndex) == itemId)
            {
                return item.DefIndex;
            }
        }

        return null;
    }

    private static ulong BuildDotaItemInstanceId(ulong steamId, uint defIndex)
    {
        var accountBits = steamId & 0xffffffffUL;
        return 0x7000000000000000UL | (accountBits << 20) | defIndex;
    }

    public TsValue DotaSetItemStyle(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaSetItemStyle(itemId, style) requires two arguments");
        }

        var itemId = Convert.ToUInt64(ToInteger(args[0], "dotaSetItemStyle.itemId").ToString());
        var style = Convert.ToUInt32(ToNumber(args[1], "dotaSetItemStyle.style"));
        var changed = DotaGcRuntimeServices.SetItemStyleSink?.Invoke(_context.SteamId, itemId, style)
            ?? new List<ApiDotaEquipment>();
        return ToTsEquipmentList(changed);
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

        DotaGcRuntimeServices.StatsStore?.SaveProfileSlots(_context.AccountId, slots);
        return TsValue.FromBool(true);
    }

    public TsValue DotaSaveProfileUpdate(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaSaveProfileUpdate(backgroundItemId, featuredHeroIds) requires two arguments");
        }

        var backgroundItemId = Convert.ToUInt64(ToInteger(args[0], "dotaSaveProfileUpdate.backgroundItemId").ToString());
        var backgroundItemDefIndex = ResolveOwnedDotaItemDefIndex(_context.SteamId, backgroundItemId);
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

        DotaGcRuntimeServices.StatsStore?.SaveProfileUpdate(_context.AccountId, backgroundItemDefIndex, featuredHeroIds);
        return TsValue.FromBool(true);
    }

    public TsValue DotaProfileConductScorecard()
    {
        return ToTsConductScorecard(
            DotaGcRuntimeServices.StatsStore?.GetConduct(_context.AccountId)
                ?? new DotaStatsConduct { AccountId = _context.AccountId });
    }

    public TsValue DotaProfileQuestProgress(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaProfileQuestProgress(questIds) requires one argument");
        }

        return ToTsQuestProgress(
            DotaGcRuntimeServices.StatsStore?.GetQuestProgress(_context.AccountId, UInt32Array(args[0], "dotaProfileQuestProgress.questIds"))
                ?? Array.Empty<DotaStatsQuestProgress>());
    }

    public TsValue DotaProfilePeriodicResource(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaProfilePeriodicResource(accountId, resourceId) requires two arguments");
        }

        var accountId = Convert.ToUInt32(ToNumber(args[0], "dotaProfilePeriodicResource.accountId"));
        if (accountId == 0)
        {
            accountId = _context.AccountId;
        }

        var resourceId = Convert.ToUInt32(ToNumber(args[1], "dotaProfilePeriodicResource.resourceId"));
        return ToTsPeriodicResource(
            DotaGcRuntimeServices.StatsStore?.GetPeriodicResource(accountId, resourceId)
                ?? new DotaStatsPeriodicResource { AccountId = accountId, ResourceId = resourceId });
    }

    public TsValue DotaProfileHeroStickers()
    {
        return ToTsHeroStickers(DotaGcRuntimeServices.StatsStore?.GetHeroStickers(_context.AccountId) ?? Array.Empty<DotaStatsHeroSticker>());
    }

    public TsValue DotaProfileSetHeroSticker(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaProfileSetHeroSticker(heroId, itemId) requires two arguments");
        }

        var heroId = Convert.ToUInt32(ToNumber(args[0], "dotaProfileSetHeroSticker.heroId"));
        var itemId = Convert.ToUInt64(ToInteger(args[1], "dotaProfileSetHeroSticker.itemId").ToString());
        return TsValue.FromBool(DotaGcRuntimeServices.StatsStore?.SetHeroSticker(_context.AccountId, heroId, itemId) ?? false);
    }

    public TsValue DotaProfileOverworldState(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaProfileOverworldState(overworldId) requires one argument");
        }

        var overworldId = Convert.ToUInt32(ToNumber(args[0], "dotaProfileOverworldState.overworldId"));
        return ToTsOverworldState(
            DotaGcRuntimeServices.StatsStore?.GetOverworldState(_context.AccountId, overworldId)
                ?? new DotaStatsOverworldState { OverworldId = overworldId });
    }

    public TsValue DotaProfileMonsterHunterState()
    {
        return ToTsMonsterHunterState(DotaGcRuntimeServices.StatsStore?.GetMonsterHunterState(_context.AccountId) ?? new DotaStatsMonsterHunterState());
    }

    public TsValue DotaSocialEmoticonAccess()
    {
        var access = DotaGcRuntimeServices.StatsStore?.GetEmoticonAccess(_context.AccountId) ?? new DotaStatsEmoticonAccess
        {
            AccountId = _context.AccountId,
            UnlockedMask = Array.Empty<byte>()
        };
        return ToTsEmoticonAccess(access);
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
        return ToTsSocialFeed(DotaGcRuntimeServices.StatsStore?.GetSocialFeed(accountId, selfOnly) ?? Array.Empty<DotaStatsSocialFeedEvent>());
    }

    public TsValue DotaSocialFeedComments(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaSocialFeedComments(feedEventId) requires one argument");
        }

        var feedEventId = Convert.ToUInt64(ToInteger(args[0], "dotaSocialFeedComments.feedEventId").ToString());
        return ToTsSocialFeedComments(feedEventId, DotaGcRuntimeServices.StatsStore?.GetSocialFeedComments(feedEventId) ?? Array.Empty<DotaStatsComment>());
    }

    public TsValue DotaSocialMatchComments(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaSocialMatchComments(matchId) requires one argument");
        }

        var matchId = Convert.ToUInt64(ToInteger(args[0], "dotaSocialMatchComments.matchId").ToString());
        return ToTsSocialFeedComments(matchId, DotaGcRuntimeServices.StatsStore?.GetSocialMatchComments(matchId) ?? Array.Empty<DotaStatsComment>());
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

        return TsValue.FromBool(DotaGcRuntimeServices.StatsStore?.SaveSocialFeedComment(feedEventId, _context.AccountId, comment) ?? false);
    }

    public TsValue DotaSocialMatchPostComment(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaSocialMatchPostComment(matchId, comment) requires two arguments");
        }

        var matchId = Convert.ToUInt64(ToInteger(args[0], "dotaSocialMatchPostComment.matchId").ToString());
        var comment = ToString(args[1]).Trim();
        if (matchId == 0 || comment.Length == 0)
        {
            return TsValue.FromBool(false);
        }

        return TsValue.FromBool(DotaGcRuntimeServices.StatsStore?.SaveSocialMatchComment(matchId, _context.AccountId, _context.PersonaName, comment) ?? false);
    }

    public TsValue DotaChatJoinChannel(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaChatJoinChannel(channelName, channelType) requires two arguments");
        }

        var channelName = ToString(args[0]);
        var channelType = Convert.ToUInt32(ToNumber(args[1], "dotaChatJoinChannel.channelType"));
        var snapshot = DotaGcRuntimeServices.ChatStore.Join(channelName, channelType, _context.SteamId, _context.AccountId, _context.PersonaName);
        return snapshot == null ? TsValue.Null : ToTsChatChannel(snapshot);
    }

    public TsValue DotaChatChannels()
    {
        var channels = new TsArray();
        foreach (var channel in DotaGcRuntimeServices.ChatStore.All())
        {
            var value = new TsObject("DotaChatChannelSummary");
            value.SetField("channelId", TsValue.FromUInt64(channel.ChannelId));
            value.SetField("channelName", TsValue.FromString(channel.ChannelName));
            value.SetField("channelType", TsValue.FromInt32(unchecked((int)channel.ChannelType)));
            value.SetField("maxMembers", TsValue.FromInt32(unchecked((int)channel.MaxMembers)));
            value.SetField("numMembers", TsValue.FromInt32(unchecked((int)channel.NumMembers)));
            channels.Add(new TsObjectValue(value));
        }

        return new TsArrayValue(channels);
    }

    public TsValue DotaChatChannel(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaChatChannel(channelId) requires one argument");
        }

        var channelId = Convert.ToUInt64(ToInteger(args[0], "dotaChatChannel.channelId").ToString());
        var snapshot = DotaGcRuntimeServices.ChatStore.Get(channelId, _context.SteamId);
        return snapshot == null ? TsValue.Null : ToTsChatChannel(snapshot);
    }

    public TsValue DotaChatLeaveChannel(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaChatLeaveChannel(channelId) requires one argument");
        }

        var channelId = Convert.ToUInt64(ToInteger(args[0], "dotaChatLeaveChannel.channelId").ToString());
        return TsValue.FromBool(DotaGcRuntimeServices.ChatStore.Leave(channelId, _context.SteamId));
    }

    public TsValue DotaChatBroadcast(TsValue[] args)
    {
        if (args.Length < 3)
        {
            throw new InvalidOperationException("dotaChatBroadcast(channelId, messageType, payload) requires three arguments");
        }

        var channelId = Convert.ToUInt64(ToInteger(args[0], "dotaChatBroadcast.channelId").ToString());
        var messageType = Convert.ToUInt32(ToNumber(args[1], "dotaChatBroadcast.messageType"));
        var payloadBase64 = Convert.ToBase64String(ToBytes(args[2], "dotaChatBroadcast.payload"));
        var includeSelf = args.Length < 4 || ToBool(args[3], "dotaChatBroadcast.includeSelf");
        var delivered = 0;
        foreach (var memberSteamId in DotaGcRuntimeServices.ChatStore.GetMemberSteamIds(channelId))
        {
            var message = new ApiGCMessage
            {
                AppId = _context.AppId,
                MessageType = messageType,
                PayloadBase64 = payloadBase64,
                Protobuf = true
            };
            if (memberSteamId == _context.SteamId)
            {
                if (includeSelf)
                {
                    Response.Messages.Add(message);
                    delivered++;
                }
            }
            else if (DotaGcRuntimeServices.PendingMessageQueued != null)
            {
                DotaGcRuntimeServices.PendingMessageQueued.Invoke(memberSteamId, message);
                delivered++;
            }
        }

        return TsValue.FromInt32(delivered);
    }

    public TsValue DotaGuildEnsureCurrent()
    {
        var store = DotaGcRuntimeServices.GuildStore ?? throw new InvalidOperationException("Dota guild store is not initialized.");
        return ToTsGuild(store.EnsureCurrentMembership(_context.SteamId, _context.AccountId, _context.PersonaName));
    }

    public TsValue DotaGuildMembership(TsValue[] args)
    {
        var accountId = args.Length > 0
            ? Convert.ToUInt32(ToNumber(args[0], "dotaGuildMembership.accountId"))
            : _context.AccountId;
        if (accountId == 0)
        {
            accountId = _context.AccountId;
        }

        var store = DotaGcRuntimeServices.GuildStore ?? throw new InvalidOperationException("Dota guild store is not initialized.");
        return ToTsGuildMembership(store.GetMembership(accountId));
    }

    public TsValue DotaGuild(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaGuild(guildId) requires one argument");
        }

        var guildId = Convert.ToUInt32(ToNumber(args[0], "dotaGuild.guildId"));
        if (guildId == 0)
        {
            return TsValue.Null;
        }

        var store = DotaGcRuntimeServices.GuildStore ?? throw new InvalidOperationException("Dota guild store is not initialized.");
        return ToTsGuild(store.GetGuild(guildId));
    }

    public TsValue DotaGuildPersonaInfo(TsValue[] args)
    {
        var accountId = args.Length > 0
            ? Convert.ToUInt32(ToNumber(args[0], "dotaGuildPersonaInfo.accountId"))
            : _context.AccountId;
        if (accountId == 0)
        {
            accountId = _context.AccountId;
        }

        var store = DotaGcRuntimeServices.GuildStore ?? throw new InvalidOperationException("Dota guild store is not initialized.");
        return ToTsGuildPersonaInfos(store.GetPersonaInfo(accountId));
    }

    public TsValue DotaGuildEventData(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaGuildEventData(guildId, eventId) requires two arguments");
        }

        var guildId = Convert.ToUInt32(ToNumber(args[0], "dotaGuildEventData.guildId"));
        var eventId = Convert.ToUInt32(ToNumber(args[1], "dotaGuildEventData.eventId"));
        var store = DotaGcRuntimeServices.GuildStore ?? throw new InvalidOperationException("Dota guild store is not initialized.");
        return ToTsGuildEventData(store.GetEventData(guildId, eventId, _context.AccountId));
    }

    public TsValue DotaGuildInvite(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaGuildInvite(guildId, targetAccountId) requires two arguments");
        }

        var guildId = Convert.ToUInt32(ToNumber(args[0], "dotaGuildInvite.guildId"));
        var targetAccountId = Convert.ToUInt32(ToNumber(args[1], "dotaGuildInvite.targetAccountId"));
        var store = DotaGcRuntimeServices.GuildStore ?? throw new InvalidOperationException("Dota guild store is not initialized.");
        return TsValue.FromInt32((int)store.Invite(guildId, _context.AccountId, targetAccountId));
    }

    public TsValue DotaGuildDeclineInvite(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaGuildDeclineInvite(guildId) requires one argument");
        }

        var guildId = Convert.ToUInt32(ToNumber(args[0], "dotaGuildDeclineInvite.guildId"));
        var store = DotaGcRuntimeServices.GuildStore ?? throw new InvalidOperationException("Dota guild store is not initialized.");
        return TsValue.FromInt32((int)store.DeclineInvite(guildId, _context.AccountId));
    }

    public TsValue DotaGuildCancelInvite(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaGuildCancelInvite(guildId, targetAccountId) requires two arguments");
        }

        var guildId = Convert.ToUInt32(ToNumber(args[0], "dotaGuildCancelInvite.guildId"));
        var targetAccountId = Convert.ToUInt32(ToNumber(args[1], "dotaGuildCancelInvite.targetAccountId"));
        var store = DotaGcRuntimeServices.GuildStore ?? throw new InvalidOperationException("Dota guild store is not initialized.");
        return TsValue.FromInt32((int)store.CancelInvite(guildId, _context.AccountId, targetAccountId));
    }

    public TsValue DotaGuildAcceptInvite(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaGuildAcceptInvite(guildId) requires one argument");
        }

        var guildId = Convert.ToUInt32(ToNumber(args[0], "dotaGuildAcceptInvite.guildId"));
        var store = DotaGcRuntimeServices.GuildStore ?? throw new InvalidOperationException("Dota guild store is not initialized.");
        return TsValue.FromInt32((int)store.AcceptInvite(guildId, _context.AccountId));
    }

    public TsValue DotaGuildLeave(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaGuildLeave(guildId) requires one argument");
        }

        var guildId = Convert.ToUInt32(ToNumber(args[0], "dotaGuildLeave.guildId"));
        var store = DotaGcRuntimeServices.GuildStore ?? throw new InvalidOperationException("Dota guild store is not initialized.");
        return TsValue.FromInt32((int)store.Leave(guildId, _context.AccountId));
    }

    public TsValue DotaReporterUpdates()
    {
        var summary = DotaGcRuntimeServices.StatsStore?.GetReporterUpdates(_context.AccountId) ?? new DotaStatsReporterUpdateSummary();
        return ToTsReporterUpdates(summary);
    }

    public TsValue DotaAcknowledgeReporterUpdates(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaAcknowledgeReporterUpdates(matchIds) requires one argument");
        }

        return TsValue.FromBool(DotaGcRuntimeServices.StatsStore?.AcknowledgeReporterUpdates(_context.AccountId, UInt64Array(args[0], "dotaAcknowledgeReporterUpdates.matchIds")) ?? false);
    }

    public TsValue DotaTeam(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaTeam(teamId) requires one argument");
        }

        var teamId = Convert.ToUInt32(ToNumber(args[0], "dotaTeam.teamId"));
        var json = DotaGcRuntimeServices.TeamJsonProvider?.Invoke(teamId) ?? "{}";
        return ToTsDotaTeam(json, null);
    }

    public TsValue DotaTeamsForAccount(TsValue[] args)
    {
        var accountId = args.Length > 0
            ? Convert.ToUInt32(ToNumber(args[0], "dotaTeamsForAccount.accountId"))
            : _context.AccountId;
        var json = DotaGcRuntimeServices.TeamsForAccountJsonProvider?.Invoke(accountId) ?? "[]";
        return ToTsDotaTeams(json);
    }

    public TsValue DotaNextTeamId()
    {
        var raw = DotaGcRuntimeServices.NextTeamIdProvider?.Invoke() ?? "1000000";
        return TsValue.FromInt32(unchecked((int)Convert.ToUInt32(raw)));
    }

    public TsValue DotaUpsertTeam(TsValue[] args)
    {
        if (args.Length < 4)
        {
            throw new InvalidOperationException("dotaUpsertTeam(teamId, name, tag, teamJson) requires four arguments");
        }

        var teamId = Convert.ToUInt32(ToNumber(args[0], "dotaUpsertTeam.teamId")).ToString(System.Globalization.CultureInfo.InvariantCulture);
        var name = ToString(args[1]);
        var tag = ToString(args[2]);
        var teamJson = ToString(args[3]);
        var json = DotaGcRuntimeServices.TeamUpsertSink?.Invoke(teamId, name, tag, teamJson) ?? "{}";
        return ToTsDotaTeam(json, null);
    }

    public TsValue DotaAddTeamMember(TsValue[] args)
    {
        if (args.Length < 3)
        {
            throw new InvalidOperationException("dotaAddTeamMember(teamId, accountId, role) requires three arguments");
        }

        var teamId = Convert.ToUInt32(ToNumber(args[0], "dotaAddTeamMember.teamId")).ToString(System.Globalization.CultureInfo.InvariantCulture);
        var accountId = Convert.ToUInt32(ToNumber(args[1], "dotaAddTeamMember.accountId"));
        var role = Convert.ToUInt32(ToNumber(args[2], "dotaAddTeamMember.role"));
        return TsValue.FromBool(DotaGcRuntimeServices.TeamMemberUpsertSink?.Invoke(teamId, accountId, role) ?? false);
    }

    public TsValue DotaRemoveTeamMember(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaRemoveTeamMember(teamId, accountId) requires two arguments");
        }

        var teamId = Convert.ToUInt32(ToNumber(args[0], "dotaRemoveTeamMember.teamId")).ToString(System.Globalization.CultureInfo.InvariantCulture);
        var accountId = Convert.ToUInt32(ToNumber(args[1], "dotaRemoveTeamMember.accountId"));
        return TsValue.FromBool(DotaGcRuntimeServices.TeamMemberRemoveSink?.Invoke(teamId, accountId) ?? false);
    }

    public TsValue DotaRemoveTeam(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaRemoveTeam(teamId) requires one argument");
        }

        var teamId = Convert.ToUInt32(ToNumber(args[0], "dotaRemoveTeam.teamId")).ToString(System.Globalization.CultureInfo.InvariantCulture);
        return TsValue.FromBool(DotaGcRuntimeServices.TeamRemoveSink?.Invoke(teamId) ?? false);
    }

    public TsValue DotaTeamNameAvailable(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaTeamNameAvailable(name, exceptTeamId) requires two arguments");
        }

        return TsValue.FromBool(DotaGcRuntimeServices.TeamNameAvailableProvider?.Invoke(ToString(args[0]), ToString(args[1])) ?? false);
    }

    public TsValue DotaTeamTagAvailable(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaTeamTagAvailable(tag, exceptTeamId) requires two arguments");
        }

        return TsValue.FromBool(DotaGcRuntimeServices.TeamTagAvailableProvider?.Invoke(ToString(args[0]), ToString(args[1])) ?? false);
    }

    public TsValue DotaTeamPlayerInfo(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaTeamPlayerInfo(accountId) requires one argument");
        }

        var accountId = Convert.ToUInt32(ToNumber(args[0], "dotaTeamPlayerInfo.accountId"));
        var json = DotaGcRuntimeServices.TeamPlayerInfoProvider?.Invoke(accountId) ?? "{}";
        return ToTsDotaPlayerInfo(json);
    }

    public TsValue DotaUpsertTeamPlayerInfo(TsValue[] args)
    {
        if (args.Length < 7)
        {
            throw new InvalidOperationException("dotaUpsertTeamPlayerInfo(accountId, name, countryCode, fantasyRole, teamId, sponsor, realName) requires seven arguments");
        }

        var accountId = Convert.ToUInt32(ToNumber(args[0], "dotaUpsertTeamPlayerInfo.accountId"));
        var fantasyRole = Convert.ToUInt32(ToNumber(args[3], "dotaUpsertTeamPlayerInfo.fantasyRole"));
        var teamId = Convert.ToUInt32(ToNumber(args[4], "dotaUpsertTeamPlayerInfo.teamId")).ToString(System.Globalization.CultureInfo.InvariantCulture);
        var json = DotaGcRuntimeServices.TeamPlayerInfoUpsertSink?.Invoke(
            accountId,
            ToString(args[1]),
            ToString(args[2]),
            fantasyRole,
            teamId,
            ToString(args[5]),
            ToString(args[6])) ?? "{}";
        return ToTsDotaPlayerInfo(json);
    }

    public TsValue DotaDeleteTeamPlayerInfo(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaDeleteTeamPlayerInfo(accountId) requires one argument");
        }

        var accountId = Convert.ToUInt32(ToNumber(args[0], "dotaDeleteTeamPlayerInfo.accountId"));
        return TsValue.FromBool(DotaGcRuntimeServices.TeamPlayerInfoDeleteSink?.Invoke(accountId) ?? false);
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

        return ToTsHeroStandings(DotaGcRuntimeServices.StatsStore?.GetHeroStandings(accountId) ?? Array.Empty<DotaStatsHeroStats>());
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
        var standings = DotaGcRuntimeServices.StatsStore?.GetHeroStandings(accountId) ?? Array.Empty<DotaStatsHeroStats>();
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

        var stats = DotaGcRuntimeServices.StatsStore?.GetGlobalStats(accountId) ?? new DotaStatsGlobalStats { AccountId = accountId };
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

        return ToTsTeammateStats(DotaGcRuntimeServices.StatsStore?.GetTeammateStats(accountId) ?? Array.Empty<DotaStatsTeammate>());
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
        return ToTsMatchPlayers(DotaGcRuntimeServices.StatsStore?.GetMatchHistory(accountId, startAtMatchId, requested, heroId, includePractice) ?? Array.Empty<DotaStatsMatchPlayer>());
    }

    public TsValue DotaMatchDetails(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaMatchDetails(matchId) requires one argument");
        }

        var matchId = Convert.ToUInt64(ToInteger(args[0], "dotaMatchDetails.matchId").ToString());
        var match = DotaGcRuntimeServices.StatsStore?.GetMatch(matchId);
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
        return ToTsMatchPlayers(DotaGcRuntimeServices.StatsStore?.GetRecentMatches(accountId, 20, heroId) ?? Array.Empty<DotaStatsMatchPlayer>());
    }

    public TsValue DotaMatchVotes(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaMatchVotes(matchId) requires one argument");
        }

        var matchId = Convert.ToUInt64(ToInteger(args[0], "dotaMatchVotes.matchId").ToString());
        var summary = DotaGcRuntimeServices.StatsStore?.GetMatchVotes(matchId, _context.AccountId) ?? new DotaStatsVoteSummary();
        var result = new TsObject("DotaMatchVoteSummary");
        result.SetField("success", TsValue.FromBool(summary.Success));
        result.SetField("vote", ToTsUInt32(summary.Vote));
        result.SetField("positiveVotes", ToTsUInt32(summary.PositiveVotes));
        result.SetField("negativeVotes", ToTsUInt32(summary.NegativeVotes));
        return new TsObjectValue(result);
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

        var store = DotaGcRuntimeServices.StatsStore;
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
        return TsValue.FromBool(DotaGcRuntimeServices.StatsStore?.HasMvpVote(matchId, _context.AccountId) ?? false);
    }

    public TsValue DotaVoteForMvp(TsValue[] args)
    {
        if (args.Length < 2)
        {
            throw new InvalidOperationException("dotaVoteForMvp(matchId, votedAccountId) requires two arguments");
        }

        var matchId = Convert.ToUInt64(ToInteger(args[0], "dotaVoteForMvp.matchId").ToString());
        var votedAccountId = Convert.ToUInt32(ToNumber(args[1], "dotaVoteForMvp.votedAccountId"));
        return TsValue.FromBool(DotaGcRuntimeServices.StatsStore?.SaveMvpVote(matchId, _context.AccountId, votedAccountId) ?? false);
    }

    public TsValue DotaFinalizeMvpVote(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaFinalizeMvpVote(matchId) requires one argument");
        }

        var matchId = Convert.ToUInt64(ToInteger(args[0], "dotaFinalizeMvpVote.matchId").ToString());
        return TsValue.FromBool(DotaGcRuntimeServices.StatsStore?.FinalizeMvpVotes(matchId) ?? false);
    }

    public TsValue DotaSubmitLobbyMvpVote(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaSubmitLobbyMvpVote(targetAccountId) requires one argument");
        }

        var targetAccountId = Convert.ToUInt32(ToNumber(args[0], "dotaSubmitLobbyMvpVote.targetAccountId"));
        var latestMatchId = DotaGcRuntimeServices.StatsStore?.GetRecentMatches(_context.AccountId, 1).FirstOrDefault()?.MatchId ?? 0;
        var ok = latestMatchId != 0 && (DotaGcRuntimeServices.StatsStore?.SaveMvpVote(latestMatchId, _context.AccountId, targetAccountId) ?? false);
        if (ok)
        {
            DotaGcRuntimeServices.StatsStore?.FinalizeMvpVotes(latestMatchId);
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

        return TsValue.FromBool(DotaGcRuntimeServices.StatsStore?.SetMatchMvps(matchId, winners) ?? false);
    }

    public TsValue DotaRerollPlayerChallenge()
    {
        var progress = DotaGcRuntimeServices.StatsStore?.RerollAllHeroChallenge(_context.AccountId);
        return TsValue.FromBool(progress != null);
    }

    public TsValue DotaRecordMatchSignOutPermission(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaRecordMatchSignOutPermission(request) requires one argument");
        }

        var request = RequireObject(args[0], "dotaRecordMatchSignOutPermission.request");
        return TsValue.FromBool(DotaGcRuntimeServices.StatsStore?.RecordMatchSignOutPermission(new DotaStatsMatchSignOutPermissionAudit
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
        return TsValue.FromBool(DotaGcRuntimeServices.StatsStore?.SetMatchHistoryAccess(_context.SteamId, _context.AccountId, allow) ?? false);
    }

    public TsValue DotaRecordServerStatus(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaRecordServerStatus(response) requires one argument");
        }

        var response = Convert.ToUInt32(ToNumber(args[0], "dotaRecordServerStatus.response"));
        return TsValue.FromBool(DotaGcRuntimeServices.StatsStore?.RecordServerStatusRequest(_context.SteamId, response) ?? false);
    }

    public TsValue DotaRecordLeaver(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaRecordLeaver(event) requires one argument");
        }

        var eventValue = RequireObject(args[0], "dotaRecordLeaver.event");
        var leaverSteamId = U64Field(eventValue, "leaverSteamId", "dotaRecordLeaver.event");
        return TsValue.FromBool(DotaGcRuntimeServices.StatsStore?.RecordLeaverDetected(new DotaStatsLeaverEvent
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
        return TsValue.FromBool(DotaGcRuntimeServices.StatsStore?.RecordRealtimeStats(new DotaStatsRealtimeStatsSnapshot
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
        return TsValue.FromBool(DotaGcRuntimeServices.StatsStore?.RecordMatchStateHistory(new DotaStatsMatchStateHistorySnapshot
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
        return TsValue.FromBool(DotaGcRuntimeServices.StatsStore?.RecordSpectatorCount(_context.SteamId, spectatorCount) ?? false);
    }

    public TsValue DotaRecordLiveScoreboard(TsValue[] args)
    {
        if (args.Length < 1)
        {
            throw new InvalidOperationException("dotaRecordLiveScoreboard(snapshot) requires one argument");
        }

        var snapshot = RequireObject(args[0], "dotaRecordLiveScoreboard.snapshot");
        return TsValue.FromBool(DotaGcRuntimeServices.StatsStore?.RecordLiveScoreboard(new DotaStatsLiveScoreboardSnapshot
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
        return TsValue.FromBool(DotaGcRuntimeServices.StatsStore?.TrySavePlayerReport(new DotaStatsPlayerReport
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
        var store = DotaGcRuntimeServices.StatsStore;
        return store == null
            ? DefaultProfile(accountId)
            : accountId == _context.AccountId
                ? store.EnsureProfile(_context.SteamId, _context.AccountId, _context.PersonaName)
                : store.GetProfile(accountId);
    }

    private TsValue ToTsProfileSnapshot(uint requestedAccountId)
    {
        var store = DotaGcRuntimeServices.StatsStore;
        var profile = store == null
            ? DefaultProfile(requestedAccountId)
            : requestedAccountId == _context.AccountId
                ? store.EnsureProfile(_context.SteamId, _context.AccountId, _context.PersonaName)
                : store.GetProfile(requestedAccountId);

        var global = store?.GetGlobalStats(profile.AccountId) ?? new DotaStatsGlobalStats { AccountId = profile.AccountId };
        var conduct = store?.GetConduct(profile.AccountId) ?? new DotaStatsConduct { AccountId = profile.AccountId };
        var value = new TsObject("DotaProfileSnapshot");
        value.SetField("accountId", TsValue.FromInt32(unchecked((int)profile.AccountId)));
        value.SetField("steamId", TsValue.FromUInt64(profile.SteamId));
        value.SetField("personaName", TsValue.FromString(profile.PersonaName ?? string.Empty));
        value.SetField(
            "backgroundItemDefIndex",
            TsValue.FromInt32(unchecked((int)ResolveStoredDotaItemDefIndex(profile.SteamId, profile.BackgroundItemId))));
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
        value.SetField("reportsCount", TsValue.FromInt32(unchecked((int)conduct.ReportsCount)));
        value.SetField("matchesAbandoned", TsValue.FromInt32(unchecked((int)conduct.MatchesAbandoned)));
        value.SetField("commsReports", TsValue.FromInt32(unchecked((int)conduct.CommsReports)));
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
        var store = DotaGcRuntimeServices.StatsStore;
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
        var store = DotaGcRuntimeServices.StatsStore;
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

    private static TsValue ToTsChatChannel(DotaChatChannelSnapshot snapshot)
    {
        var members = new TsArray();
        foreach (var member in snapshot.Members)
        {
            var memberValue = new TsObject("DotaChatMember");
            memberValue.SetField("steamId", TsValue.FromUInt64(member.SteamId));
            memberValue.SetField("accountId", TsValue.FromInt32(unchecked((int)member.AccountId)));
            memberValue.SetField("personaName", TsValue.FromString(member.PersonaName ?? string.Empty));
            memberValue.SetField("channelUserId", TsValue.FromInt32(unchecked((int)member.ChannelUserId)));
            members.Add(new TsObjectValue(memberValue));
        }

        var value = new TsObject("DotaChatChannel");
        value.SetField("channelId", TsValue.FromUInt64(snapshot.ChannelId));
        value.SetField("channelName", TsValue.FromString(snapshot.ChannelName ?? string.Empty));
        value.SetField("channelType", TsValue.FromInt32(unchecked((int)snapshot.ChannelType)));
        value.SetField("maxMembers", TsValue.FromInt32(unchecked((int)snapshot.MaxMembers)));
        value.SetField("isMember", TsValue.FromBool(snapshot.SelfIsMember));
        value.SetField("channelUserId", TsValue.FromInt32(unchecked((int)snapshot.SelfChannelUserId)));
        value.SetField("justJoined", TsValue.FromBool(snapshot.SelfJustJoined));
        value.SetField("members", new TsArrayValue(members));
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

    private static TsValue ToTsEmoticonAccess(DotaStatsEmoticonAccess access)
    {
        var value = new TsObject("DotaEmoticonAccess");
        value.SetField("accountId", TsValue.FromInt32(unchecked((int)access.AccountId)));
        value.SetField("unlockedEmoticons", TsValue.FromUint8Array(access.UnlockedMask));
        value.SetField("updatedAt", TsValue.FromInt32(unchecked((int)access.UpdatedAt)));
        return new TsObjectValue(value);
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

    private static TsValue ToTsPartyState(DotaPartyState? party)
    {
        if (party == null)
        {
            return TsValue.Null;
        }

        var value = new TsObject("DotaPartyState");
        value.SetField("partyId", TsValue.FromUInt64(party.PartyId));
        value.SetField("leaderSteamId", TsValue.FromUInt64(party.LeaderSteamId));
        value.SetField("state", TsValue.FromInt32(unchecked((int)party.State)));
        value.SetField("permanent", TsValue.FromBool(party.Permanent));
        value.SetField("readyStartTimestamp", TsValue.FromInt32(unchecked((int)party.ReadyStartTimestamp)));
        value.SetField("readyFinishTimestamp", TsValue.FromInt32(unchecked((int)party.ReadyFinishTimestamp)));
        value.SetField("readyInitiatorAccountId", TsValue.FromInt32(unchecked((int)party.ReadyInitiatorAccountId)));
        value.SetField("members", ToTsPartyMembers(party.Members));
        return new TsObjectValue(value);
    }

    private static TsValue ToTsPartyMembers(IEnumerable<DotaPartyMember> members)
    {
        var array = new TsArray();
        foreach (var member in members)
        {
            var value = new TsObject("DotaPartyMember");
            value.SetField("steamId", TsValue.FromUInt64(member.SteamId));
            value.SetField("accountId", TsValue.FromInt32(unchecked((int)member.AccountId)));
            value.SetField("personaName", TsValue.FromString(member.PersonaName ?? string.Empty));
            value.SetField("position", TsValue.FromInt32(member.Position));
            value.SetField("isCoach", TsValue.FromBool(member.IsCoach));
            value.SetField("isPlusSubscriber", TsValue.FromBool(member.IsPlusSubscriber));
            value.SetField("regionCodes", ToTsUInt32Array(member.RegionCodes));
            value.SetField("regionPings", ToTsUInt32Array(member.RegionPings));
            value.SetField("regionPingFailedBitmask", TsValue.FromInt32(unchecked((int)member.RegionPingFailedBitmask)));
            value.SetField("readyStatus", TsValue.FromInt32(unchecked((int)member.ReadyStatus)));
            array.Add(new TsObjectValue(value));
        }

        return new TsArrayValue(array);
    }

    private static TsValue ToTsPartyInvite(DotaPartyInvite? invite)
    {
        if (invite == null)
        {
            return TsValue.Null;
        }

        var value = new TsObject("DotaPartyInvite");
        value.SetField("inviteGid", TsValue.FromUInt64(invite.InviteGid));
        value.SetField("partyId", TsValue.FromUInt64(invite.PartyId));
        value.SetField("targetSteamId", TsValue.FromUInt64(invite.TargetSteamId));
        value.SetField("senderSteamId", TsValue.FromUInt64(invite.SenderSteamId));
        value.SetField("senderName", TsValue.FromString(invite.SenderName ?? string.Empty));
        value.SetField("teamId", TsValue.FromInt32(unchecked((int)invite.TeamId)));
        value.SetField("asCoach", TsValue.FromBool(invite.AsCoach));
        value.SetField("lowPriorityStatus", TsValue.FromBool(invite.LowPriorityStatus));
        value.SetField("createdAt", TsValue.FromInt32(unchecked((int)invite.CreatedAt)));
        return new TsObjectValue(value);
    }

    private static TsValue ToTsPartyInvites(IEnumerable<DotaPartyInvite> invites)
    {
        var array = new TsArray();
        foreach (var invite in invites)
        {
            array.Add(ToTsPartyInvite(invite));
        }

        return new TsArrayValue(array);
    }

    private static TsValue ToTsGuild(DotaGuildSnapshot? guild)
    {
        if (guild == null)
        {
            return TsValue.Null;
        }

        var value = new TsObject("DotaGuild");
        value.SetField("guildId", ToTsUInt32(guild.GuildId));
        value.SetField("info", ToTsGuildInfo(guild.Info));
        value.SetField("roles", ToTsGuildRoles(guild.Roles));
        value.SetField("members", ToTsGuildMembers(guild.Members));
        value.SetField("invites", ToTsGuildInvites(guild.Invites));
        return new TsObjectValue(value);
    }

    private static TsValue ToTsConductScorecard(DotaStatsConduct conduct)
    {
        var value = new TsObject("DotaConductScorecard");
        value.SetField("accountId", ToTsUInt32(conduct.AccountId));
        value.SetField("matchId", TsValue.FromUInt64(conduct.MatchId));
        value.SetField("seqNum", TsValue.FromInt32(0));
        value.SetField("reasons", ToTsUInt32(conduct.ReportsCount == 0 ? 0u : 1u));
        value.SetField("matchesInReport", ToTsUInt32(conduct.MatchesInReport));
        value.SetField("matchesClean", ToTsUInt32(conduct.MatchesClean));
        value.SetField("matchesReported", ToTsUInt32(conduct.MatchesReported));
        value.SetField("matchesAbandoned", ToTsUInt32(conduct.MatchesAbandoned));
        value.SetField("reportsCount", ToTsUInt32(conduct.ReportsCount));
        value.SetField("reportsParties", ToTsUInt32(conduct.ReportsParties));
        value.SetField("commendCount", ToTsUInt32(conduct.CommendCount));
        value.SetField("date", ToTsUInt32(conduct.Date));
        value.SetField("rawBehaviorScore", ToTsUInt32(conduct.RawBehaviorScore));
        value.SetField("oldRawBehaviorScore", ToTsUInt32(conduct.OldRawBehaviorScore));
        value.SetField("commsReports", ToTsUInt32(conduct.CommsReports));
        value.SetField("commsParties", ToTsUInt32(conduct.CommsReports));
        value.SetField("behaviorRating", ToTsUInt32(conduct.BehaviorRating));
        return new TsObjectValue(value);
    }

    private static TsValue ToTsQuestProgress(IEnumerable<DotaStatsQuestProgress> quests)
    {
        var array = new TsArray();
        foreach (var quest in quests)
        {
            var value = new TsObject("DotaQuestProgress");
            value.SetField("questId", ToTsUInt32(quest.QuestId));
            value.SetField("completedChallenges", ToTsQuestChallenges(quest.CompletedChallenges));
            array.Add(new TsObjectValue(value));
        }

        return new TsArrayValue(array);
    }

    private static TsValue ToTsQuestChallenges(IEnumerable<DotaStatsQuestChallenge> challenges)
    {
        var array = new TsArray();
        foreach (var challenge in challenges)
        {
            var value = new TsObject("DotaQuestChallenge");
            value.SetField("challengeId", ToTsUInt32(challenge.ChallengeId));
            value.SetField("timeCompleted", ToTsUInt32(challenge.TimeCompleted));
            value.SetField("attempts", ToTsUInt32(challenge.Attempts));
            value.SetField("heroId", ToTsUInt32(challenge.HeroId));
            value.SetField("templateId", ToTsUInt32(challenge.TemplateId));
            value.SetField("questRank", ToTsUInt32(challenge.QuestRank));
            array.Add(new TsObjectValue(value));
        }

        return new TsArrayValue(array);
    }

    private static TsValue ToTsPeriodicResource(DotaStatsPeriodicResource resource)
    {
        var value = new TsObject("DotaPeriodicResource");
        value.SetField("accountId", ToTsUInt32(resource.AccountId));
        value.SetField("resourceId", ToTsUInt32(resource.ResourceId));
        value.SetField("resourceMax", ToTsUInt32(resource.ResourceMax));
        value.SetField("resourceUsed", ToTsUInt32(resource.ResourceUsed));
        return new TsObjectValue(value);
    }

    private static TsValue ToTsHeroStickers(IEnumerable<DotaStatsHeroSticker> stickers)
    {
        var array = new TsArray();
        foreach (var sticker in stickers)
        {
            var value = new TsObject("DotaHeroSticker");
            value.SetField("heroId", ToTsUInt32(sticker.HeroId));
            value.SetField("itemDefId", ToTsUInt32(sticker.ItemDefId));
            value.SetField("quality", ToTsUInt32(sticker.Quality));
            value.SetField("sourceItemId", TsValue.FromUInt64(sticker.SourceItemId));
            array.Add(new TsObjectValue(value));
        }

        return new TsArrayValue(array);
    }

    private static TsValue ToTsOverworldState(DotaStatsOverworldState state)
    {
        var value = new TsObject("DotaOverworldState");
        value.SetField("overworldId", ToTsUInt32(state.OverworldId));
        value.SetField("currentNodeId", ToTsUInt32(state.CurrentNodeId));
        value.SetField("lastRelatedHeroId", ToTsUInt32(state.LastRelatedHeroId));
        value.SetField("overworldVersion", ToTsUInt32(state.OverworldVersion));
        return new TsObjectValue(value);
    }

    private static TsValue ToTsMonsterHunterState(DotaStatsMonsterHunterState state)
    {
        var value = new TsObject("DotaMonsterHunterState");
        value.SetField("unlockedCount", ToTsUInt32(state.UnlockedCount));
        return new TsObjectValue(value);
    }

    private static TsValue ToTsGuildInfo(DotaGuildInfoSnapshot info)
    {
        var value = new TsObject("DotaGuildInfo");
        value.SetField("guildName", TsValue.FromString(info.GuildName));
        value.SetField("guildTag", TsValue.FromString(info.GuildTag));
        value.SetField("createdTimestamp", ToTsUInt32(info.CreatedTimestamp));
        value.SetField("guildLanguage", ToTsUInt32(info.GuildLanguage));
        value.SetField("guildFlags", ToTsUInt32(info.GuildFlags));
        value.SetField("guildLogo", TsValue.FromUInt64(info.GuildLogo));
        value.SetField("guildRegion", ToTsUInt32(info.GuildRegion));
        value.SetField("guildChatGroupId", TsValue.FromUInt64(info.GuildChatGroupId));
        value.SetField("guildDescription", TsValue.FromString(info.GuildDescription));
        value.SetField("defaultChatChannelId", TsValue.FromUInt64(info.DefaultChatChannelId));
        value.SetField("guildPrimaryColor", ToTsUInt32(info.GuildPrimaryColor));
        value.SetField("guildSecondaryColor", ToTsUInt32(info.GuildSecondaryColor));
        value.SetField("guildPattern", ToTsUInt32(info.GuildPattern));
        value.SetField("guildRefreshTimeOffset", ToTsUInt32(info.GuildRefreshTimeOffset));
        value.SetField("guildRequiredRankTier", ToTsUInt32(info.GuildRequiredRankTier));
        value.SetField("guildMotdTimestamp", ToTsUInt32(info.GuildMotdTimestamp));
        value.SetField("guildMotd", TsValue.FromString(info.GuildMotd));
        return new TsObjectValue(value);
    }

    private static TsValue ToTsGuildRoles(IEnumerable<DotaGuildRoleSnapshot> roles)
    {
        var array = new TsArray();
        foreach (var role in roles)
        {
            var value = new TsObject("DotaGuildRole");
            value.SetField("roleId", ToTsUInt32(role.RoleId));
            value.SetField("roleName", TsValue.FromString(role.RoleName));
            value.SetField("roleFlags", ToTsUInt32(role.RoleFlags));
            value.SetField("roleOrder", ToTsUInt32(role.RoleOrder));
            array.Add(new TsObjectValue(value));
        }

        return new TsArrayValue(array);
    }

    private static TsValue ToTsGuildMembers(IEnumerable<DotaGuildMemberSnapshot> members)
    {
        var array = new TsArray();
        foreach (var member in members)
        {
            var value = new TsObject("DotaGuildMember");
            value.SetField("accountId", ToTsUInt32(member.AccountId));
            value.SetField("roleId", ToTsUInt32(member.RoleId));
            value.SetField("joinedTimestamp", ToTsUInt32(member.JoinedTimestamp));
            value.SetField("lastActiveTimestamp", ToTsUInt32(member.LastActiveTimestamp));
            array.Add(new TsObjectValue(value));
        }

        return new TsArrayValue(array);
    }

    private static TsValue ToTsGuildInvites(IEnumerable<DotaGuildInviteSnapshot> invites)
    {
        var array = new TsArray();
        foreach (var invite in invites)
        {
            var value = new TsObject("DotaGuildInvite");
            value.SetField("guildId", ToTsUInt32(invite.GuildId));
            value.SetField("requesterAccountId", ToTsUInt32(invite.RequesterAccountId));
            value.SetField("targetAccountId", ToTsUInt32(invite.TargetAccountId));
            value.SetField("timestampSent", ToTsUInt32(invite.TimestampSent));
            array.Add(new TsObjectValue(value));
        }

        return new TsArrayValue(array);
    }

    private static TsValue ToTsGuildMembership(DotaGuildMembershipSnapshot membership)
    {
        var value = new TsObject("DotaGuildMembership");
        value.SetField("guildIds", ToTsUInt32Array(membership.GuildIds));
        value.SetField("invites", ToTsGuildInvites(membership.Invites));
        return new TsObjectValue(value);
    }

    private static TsValue ToTsGuildPersonaInfos(IEnumerable<DotaGuildPersonaSnapshot> personaInfos)
    {
        var array = new TsArray();
        foreach (var info in personaInfos)
        {
            var value = new TsObject("DotaGuildPersona");
            value.SetField("guildId", ToTsUInt32(info.GuildId));
            value.SetField("guildTag", TsValue.FromString(info.GuildTag));
            value.SetField("guildFlags", ToTsUInt32(info.GuildFlags));
            array.Add(new TsObjectValue(value));
        }

        return new TsArrayValue(array);
    }

    private static TsValue ToTsGuildEventData(DotaGuildEventDataSnapshot eventData)
    {
        var value = new TsObject("DotaGuildEventData");
        value.SetField("guildId", ToTsUInt32(eventData.GuildId));
        value.SetField("eventId", ToTsUInt32(eventData.EventId));
        value.SetField("isMember", TsValue.FromBool(eventData.IsMember));
        value.SetField("guildPoints", ToTsUInt32(eventData.GuildPoints));
        value.SetField("contractsRefreshedTimestamp", ToTsUInt32(eventData.ContractsRefreshedTimestamp));
        value.SetField("completedChallengeCount", ToTsUInt32(eventData.CompletedChallengeCount));
        value.SetField("challengesRefreshTimestamp", ToTsUInt32(eventData.ChallengesRefreshTimestamp));
        value.SetField("guildWeeklyPercentile", ToTsUInt32(eventData.GuildWeeklyPercentile));
        value.SetField("guildWeeklyLastTimestamp", ToTsUInt32(eventData.GuildWeeklyLastTimestamp));
        value.SetField("lastWeeklyClaimTime", ToTsUInt32(eventData.LastWeeklyClaimTime));
        value.SetField("guildCurrentPercentile", ToTsUInt32(eventData.GuildCurrentPercentile));
        return new TsObjectValue(value);
    }

    private static TsValue ToTsReporterUpdates(DotaStatsReporterUpdateSummary summary)
    {
        var updates = new TsArray();
        foreach (var item in summary.Updates)
        {
            var update = new TsObject("DotaReporterUpdate");
            update.SetField("matchId", TsValue.FromUInt64(item.MatchId));
            update.SetField("heroId", ToTsUInt32(item.HeroId));
            update.SetField("reportReason", ToTsUInt32(item.ReportReason));
            update.SetField("timestamp", ToTsUInt32(item.Timestamp));
            updates.Add(new TsObjectValue(update));
        }

        var value = new TsObject("DotaReporterUpdates");
        value.SetField("updates", new TsArrayValue(updates));
        value.SetField("numReported", ToTsUInt32(summary.NumReported));
        value.SetField("numNoActionTaken", ToTsUInt32(summary.NumNoActionTaken));
        return new TsObjectValue(value);
    }

    private static TsValue ToTsDotaTeams(string teamsJson)
    {
        var teams = new TsArray();
        using var document = JsonDocument.Parse(string.IsNullOrWhiteSpace(teamsJson) ? "[]" : teamsJson);
        if (document.RootElement.ValueKind != JsonValueKind.Array)
        {
            return new TsArrayValue(teams);
        }

        foreach (var row in document.RootElement.EnumerateArray())
        {
            var role = JsonUInt32(row, "role", 0);
            teams.Add(ToTsDotaTeam(row, role));
        }

        return new TsArrayValue(teams);
    }

    private static TsValue ToTsDotaTeam(string teamJson, uint? role)
    {
        using var document = JsonDocument.Parse(string.IsNullOrWhiteSpace(teamJson) ? "{}" : teamJson);
        return ToTsDotaTeam(document.RootElement, role);
    }

    private static TsValue ToTsDotaTeam(JsonElement row, uint? role)
    {
        var teamId = JsonUInt32(row, "teamId", 0);
        if (teamId == 0)
        {
            return TsValue.Null;
        }

        var details = ParseTeamDetails(row);
        var value = new TsObject("DotaTeam");
        value.SetField("teamId", ToTsUInt32(teamId));
        value.SetField("name", TsValue.FromString(JsonString(row, "name", details.Name)));
        value.SetField("tag", TsValue.FromString(JsonString(row, "tag", details.Tag)));
        value.SetField("role", role.HasValue ? ToTsUInt32(role.Value) : TsValue.Null);
        value.SetField("logo", TsValue.FromUInt64(details.Logo));
        value.SetField("baseLogo", TsValue.FromUInt64(details.BaseLogo));
        value.SetField("bannerLogo", TsValue.FromUInt64(details.BannerLogo));
        value.SetField("logoUrl", TsValue.FromString(details.LogoUrl));
        value.SetField("abbreviation", TsValue.FromString(details.Abbreviation));
        value.SetField("countryCode", TsValue.FromString(details.CountryCode));
        value.SetField("url", TsValue.FromString(details.Url));
        value.SetField("wins", ToTsUInt32(details.Wins));
        value.SetField("losses", ToTsUInt32(details.Losses));
        value.SetField("gamesPlayedTotal", ToTsUInt32(details.GamesPlayedTotal));
        value.SetField("gamesPlayedMatchmaking", ToTsUInt32(details.GamesPlayedMatchmaking));
        value.SetField("region", ToTsUInt32(details.Region));
        value.SetField("members", ToTsDotaTeamMembers(row));
        return new TsObjectValue(value);
    }

    private static TsValue ToTsDotaTeamMembers(JsonElement row)
    {
        var result = new TsArray();
        if (!row.TryGetProperty("members", out var members))
        {
            return new TsArrayValue(result);
        }

        if (members.ValueKind == JsonValueKind.String)
        {
            using var document = JsonDocument.Parse(string.IsNullOrWhiteSpace(members.GetString()) ? "[]" : members.GetString()!);
            AddDotaTeamMembers(result, document.RootElement);
            return new TsArrayValue(result);
        }

        AddDotaTeamMembers(result, members);
        return new TsArrayValue(result);
    }

    private static void AddDotaTeamMembers(TsArray result, JsonElement members)
    {
        if (members.ValueKind != JsonValueKind.Array)
        {
            return;
        }

        foreach (var member in members.EnumerateArray())
        {
            var value = new TsObject("DotaTeamMember");
            value.SetField("accountId", ToTsUInt32(JsonUInt32(member, "accountId", 0)));
            value.SetField("role", ToTsUInt32(JsonUInt32(member, "role", 0)));
            result.Add(new TsObjectValue(value));
        }
    }

    private static TsValue ToTsDotaPlayerInfo(string playerInfoJson)
    {
        using var document = JsonDocument.Parse(string.IsNullOrWhiteSpace(playerInfoJson) ? "{}" : playerInfoJson);
        var row = document.RootElement;
        if (row.ValueKind != JsonValueKind.Object || JsonUInt32(row, "accountId", 0) == 0)
        {
            return TsValue.Null;
        }

        var value = new TsObject("DotaTeamPlayerInfo");
        value.SetField("accountId", ToTsUInt32(JsonUInt32(row, "accountId", 0)));
        value.SetField("name", TsValue.FromString(JsonString(row, "name", string.Empty)));
        value.SetField("countryCode", TsValue.FromString(JsonString(row, "countryCode", string.Empty)));
        value.SetField("fantasyRole", ToTsUInt32(JsonUInt32(row, "fantasyRole", 0)));
        value.SetField("teamId", ToTsUInt32(JsonUInt32(row, "teamId", 0)));
        value.SetField("sponsor", TsValue.FromString(JsonString(row, "sponsor", string.Empty)));
        value.SetField("realName", TsValue.FromString(JsonString(row, "realName", string.Empty)));
        return new TsObjectValue(value);
    }

    private static DotaTeamDetails ParseTeamDetails(JsonElement row)
    {
        var raw = JsonString(row, "teamJson", "{}");
        using var document = JsonDocument.Parse(string.IsNullOrWhiteSpace(raw) ? "{}" : raw);
        var details = document.RootElement.ValueKind == JsonValueKind.Object ? document.RootElement : default;
        return new DotaTeamDetails
        {
            Name = JsonString(details, "name", JsonString(row, "name", string.Empty)),
            Tag = JsonString(details, "tag", JsonString(row, "tag", string.Empty)),
            Logo = JsonUInt64(details, "teamLogo", JsonUInt64(details, "logo", JsonUInt64(details, "ugcLogo", 0))),
            BaseLogo = JsonUInt64(details, "teamBaseLogo", JsonUInt64(details, "baseLogo", JsonUInt64(details, "ugcBaseLogo", 0))),
            BannerLogo = JsonUInt64(details, "teamBannerLogo", JsonUInt64(details, "bannerLogo", JsonUInt64(details, "ugcBannerLogo", 0))),
            LogoUrl = JsonString(details, "teamLogoUrl", JsonString(details, "urlLogo", string.Empty)),
            Abbreviation = JsonString(details, "teamAbbreviation", JsonString(details, "abbreviation", string.Empty)),
            CountryCode = JsonString(details, "countryCode", string.Empty),
            Url = JsonString(details, "url", string.Empty),
            Wins = JsonUInt32(details, "wins", 0),
            Losses = JsonUInt32(details, "losses", 0),
            GamesPlayedTotal = JsonUInt32(details, "gamesPlayedTotal", 0),
            GamesPlayedMatchmaking = JsonUInt32(details, "gamesPlayedMatchmaking", 0),
            Region = JsonUInt32(details, "region", 0)
        };
    }

    private readonly record struct DotaTeamDetails(
        string Name,
        string Tag,
        ulong Logo,
        ulong BaseLogo,
        ulong BannerLogo,
        string LogoUrl,
        string Abbreviation,
        string CountryCode,
        string Url,
        uint Wins,
        uint Losses,
        uint GamesPlayedTotal,
        uint GamesPlayedMatchmaking,
        uint Region);

    private static string JsonString(JsonElement element, string propertyName, string defaultValue)
    {
        return element.ValueKind == JsonValueKind.Object &&
               element.TryGetProperty(propertyName, out var property) &&
               property.ValueKind == JsonValueKind.String
            ? property.GetString() ?? defaultValue
            : defaultValue;
    }

    private static uint JsonUInt32(JsonElement element, string propertyName, uint defaultValue)
    {
        if (element.ValueKind != JsonValueKind.Object || !element.TryGetProperty(propertyName, out var property))
        {
            return defaultValue;
        }

        return property.ValueKind switch
        {
            JsonValueKind.Number when property.TryGetUInt32(out var number) => number,
            JsonValueKind.String when uint.TryParse(property.GetString(), out var number) => number,
            _ => defaultValue
        };
    }

    private static ulong JsonUInt64(JsonElement element, string propertyName, ulong defaultValue)
    {
        if (element.ValueKind != JsonValueKind.Object || !element.TryGetProperty(propertyName, out var property))
        {
            return defaultValue;
        }

        return property.ValueKind switch
        {
            JsonValueKind.Number when property.TryGetUInt64(out var number) => number,
            JsonValueKind.String when ulong.TryParse(property.GetString(), out var number) => number,
            _ => defaultValue
        };
    }

    private static DotaPartyPingData ToPartyPingData(TsValue value, string path)
    {
        if (value is TsNull or TsVoid)
        {
            return new DotaPartyPingData();
        }

        var obj = RequireObject(value, path);
        return new DotaPartyPingData
        {
            RegionCodes = UInt32ArrayField(obj, "regionCodes", path),
            RegionPings = UInt32ArrayField(obj, "regionPings", path),
            RegionPingFailedBitmask = U32Field(obj, "regionPingFailedBitmask", path)
        };
    }

    private static uint PartyInviteCutoff()
    {
        const uint partyInviteLifetimeSeconds = 600;
        var now = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return now > partyInviteLifetimeSeconds ? now - partyInviteLifetimeSeconds : 0;
    }

    private static TsValue ToTsUInt32Array(IEnumerable<uint> values)
    {
        var array = new TsArray();
        foreach (var value in values)
        {
            array.Add(ToTsUInt32(value));
        }

        return new TsArrayValue(array);
    }

    private static TsValue ToTsUInt32(uint value)
    {
        return value <= int.MaxValue ? TsValue.FromInt32(unchecked((int)value)) : TsValue.FromInt64(value);
    }

    private static List<uint> UInt32Array(TsValue value, string path)
    {
        if (value is TsNull or TsVoid)
        {
            return new List<uint>();
        }

        if (value is not TsArrayValue arrayValue)
        {
            throw new InvalidOperationException($"{path}: expected array, got {value.ValueType}");
        }

        var result = new List<uint>();
        for (var i = 0; i < arrayValue.Value.Count; i++)
        {
            result.Add(Convert.ToUInt32(ToNumber(arrayValue.Value.Get(i), $"{path}[{i}]")));
        }

        return result;
    }

    private static List<ulong> UInt64Array(TsValue value, string path)
    {
        if (value is TsNull or TsVoid)
        {
            return new List<ulong>();
        }

        if (value is not TsArrayValue arrayValue)
        {
            throw new InvalidOperationException($"{path}: expected array, got {value.ValueType}");
        }

        var result = new List<ulong>();
        for (var i = 0; i < arrayValue.Value.Count; i++)
        {
            result.Add(Convert.ToUInt64(ToInteger(arrayValue.Value.Get(i), $"{path}[{i}]").ToString()));
        }

        return result;
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

    private static ApiDotaMatch ToDotaMatchSnapshot(TsValue source, string path)
    {
        var value = RequireObject(source, path);
        var snapshot = new ApiDotaMatch
        {
            LobbyId = U64Field(value, "lobbyId", path),
            MatchId = U64Field(value, "matchId", path),
            ServerSteamId = U64Field(value, "serverSteamId", path),
            Connect = StringField(value, "connect", path),
            State = U32Field(value, "state", path),
            GameState = U32Field(value, "gameState", path),
            GameStartTime = U32Field(value, "gameStartTime", path),
            Dedicated = BoolField(value, "dedicated", path),
            UpdatedAt = DateTime.UtcNow
        };

        var playersValue = value.GetField("players");
        if (playersValue is TsNull or TsVoid)
        {
            return snapshot;
        }

        if (playersValue is not TsArrayValue players)
        {
            throw new InvalidOperationException($"{path}.players: expected array, got {playersValue.ValueType}");
        }

        for (var i = 0; i < players.Value.Count; i++)
        {
            var playerPath = $"{path}.players[{i}]";
            var player = RequireObject(players.Value.Get(i), playerPath);
            snapshot.Players.Add(new ApiDotaMatchPlayer
            {
                SteamId = U64Field(player, "steamId", playerPath),
                AccountId = U32Field(player, "accountId", playerPath),
                PersonaName = StringField(player, "personaName", playerPath),
                Team = U32Field(player, "team", playerPath),
                Slot = U32Field(player, "slot", playerPath),
                CoachTeam = U32Field(player, "coachTeam", playerPath),
                HeroId = U32Field(player, "heroId", playerPath)
            });
        }

        return snapshot;
    }

    private static TsValue ToTsDotaMatchSnapshot(ApiDotaMatch snapshot)
    {
        var value = new TsObject("DotaLobbyMatchSnapshot");
        value.SetField("lobbyId", TsValue.FromUInt64(snapshot.LobbyId));
        value.SetField("matchId", TsValue.FromUInt64(snapshot.MatchId));
        value.SetField("serverSteamId", TsValue.FromUInt64(snapshot.ServerSteamId));
        value.SetField("connect", TsValue.FromString(snapshot.Connect ?? string.Empty));
        value.SetField("state", TsValue.FromInt32(unchecked((int)snapshot.State)));
        value.SetField("gameState", TsValue.FromInt32(unchecked((int)snapshot.GameState)));
        value.SetField("gameStartTime", TsValue.FromInt32(unchecked((int)snapshot.GameStartTime)));
        value.SetField("dedicated", TsValue.FromBool(snapshot.Dedicated));
        value.SetField("updatedAtUnix", TsValue.FromInt32(unchecked((int)new DateTimeOffset(snapshot.UpdatedAt).ToUnixTimeSeconds())));

        var players = new TsArray();
        foreach (var player in snapshot.Players)
        {
            var item = new TsObject("DotaLobbyMatchPlayer");
            item.SetField("steamId", TsValue.FromUInt64(player.SteamId));
            item.SetField("accountId", TsValue.FromInt32(unchecked((int)player.AccountId)));
            item.SetField("personaName", TsValue.FromString(player.PersonaName ?? string.Empty));
            item.SetField("team", TsValue.FromInt32(unchecked((int)player.Team)));
            item.SetField("slot", TsValue.FromInt32(unchecked((int)player.Slot)));
            item.SetField("coachTeam", TsValue.FromInt32(unchecked((int)player.CoachTeam)));
            item.SetField("heroId", TsValue.FromInt32(unchecked((int)player.HeroId)));
            players.Add(new TsObjectValue(item));
        }

        value.SetField("players", new TsArrayValue(players));
        return new TsObjectValue(value);
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
        value.SetField("catalogItems", ToTsCatalogItems(inventory.Items));
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

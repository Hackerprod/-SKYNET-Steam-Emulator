using System.ComponentModel;
using ModelContextProtocol.Server;
using SKYNET_server.Services.Diagnostics;

namespace SKYNET_server.Services.Mcp;

/// <summary>
/// Read-only diagnostic tools for SKYNET server, exposed over MCP so an operator's
/// AI/dev tooling can inspect live server state without a separate service or a
/// duplicated copy of SteamApiStateService's in-memory state (see discussion #36).
///
/// Every tool takes the same admin bearer token used by the existing /api/admin/*
/// endpoints and is gated the same way (SteamApiStateService.IsWebAdmin) - MCP is
/// a new transport for the existing admin-only read surface, not a new trust
/// boundary. No tool here mutates state.
/// </summary>
[McpServerToolType]
public sealed class SkynetDiagnosticsMcpTools
{
    private const string Unauthorized = "Unauthorized: provide a valid admin session token.";

    [McpServerTool, Description("Lists the games configured for the Game Coordinator bridge (AppID, display name, entry point, host services).")]
    public static object ListGames(SteamApiStateService state, GameCoordinatorScriptPlugin gc, GameCatalogService catalog,
        [Description("Admin session/bearer token")] string token)
    {
        if (!state.IsWebAdmin(token)) return Unauthorized;

        return gc.ListApps().Select(app => new
        {
            appId = app.AppId,
            name = catalog.GetName(app.AppId) is var catalogName && catalogName != "a game" ? catalogName : app.Name,
            entryPoint = app.EntryPoint,
            hostServices = app.HostServices
        }).OrderBy(g => g.appId).ToList();
    }

    [McpServerTool, Description("Looks up a registered user by SteamID64.")]
    public static object GetUser(SteamApiStateService state,
        [Description("Admin session/bearer token")] string token,
        [Description("SteamID64 of the user to look up")] ulong steamId)
    {
        var overview = state.GetAdminOverviewForSession(token);
        if (overview == null) return Unauthorized;

        var user = overview.Users.FirstOrDefault(u => u.SteamId == steamId);
        return user == null ? $"No user found with SteamID {steamId}." : user;
    }

    [McpServerTool, Description("Reports whether a given AppID has a valid Game Coordinator app configured, plus recent trace activity for it.")]
    public static object GetGcStatus(SteamApiStateService state, GameCoordinatorScriptPlugin gc, GameCoordinatorTraceService trace,
        [Description("Admin session/bearer token")] string token,
        [Description("Steam AppID")] uint appId)
    {
        if (!state.IsWebAdmin(token)) return Unauthorized;

        var configured = gc.TryGetApp(appId, out var app);
        var recent = trace.GetSince(0).Where(e => e.AppId == appId).ToList();
        return new
        {
            appId,
            configured,
            entryPoint = configured ? app.EntryPoint : null,
            hostServices = configured ? app.HostServices : null,
            traceEntryCount = recent.Count,
            unhandledCount = recent.Count(e => e.Kind == "unhandled"),
            errorCount = recent.Count(e => e.Kind == "error"),
            lastActivityUtc = recent.Count > 0 ? recent.Max(e => e.TimestampUtc) : (DateTime?)null
        };
    }

    [McpServerTool, Description("Lists the Game Coordinator routing table: which AppIDs are wired up, their entry script, host services, and proto contract sources.")]
    public static object GetGcRoutes(SteamApiStateService state, GameCoordinatorScriptPlugin gc,
        [Description("Admin session/bearer token")] string token)
    {
        if (!state.IsWebAdmin(token)) return Unauthorized;

        return gc.ListApps().Select(app => new
        {
            appId = app.AppId,
            entryPoint = app.EntryPoint,
            hostServices = app.HostServices,
            protoContractSources = app.ProtoContracts.Sources.Select(s => s.Assembly ?? "server").Distinct().ToList(),
            typeScriptRoutes = app.TypeScript.Routes
        }).OrderBy(r => r.appId).ToList();
    }

    [McpServerTool, Description("Returns the most recent Game Coordinator messages that no plugin handled, optionally filtered by AppID.")]
    public static object GetGcUnhandledMessages(SteamApiStateService state, GameCoordinatorTraceService trace,
        [Description("Admin session/bearer token")] string token,
        [Description("Optional AppID filter")] uint? appId = null,
        [Description("Maximum entries to return (default 50)")] int limit = 50)
    {
        if (!state.IsWebAdmin(token)) return Unauthorized;

        return trace.GetSince(0)
            .Where(e => e.Kind == "unhandled" && (appId == null || e.AppId == appId))
            .OrderByDescending(e => e.Seq)
            .Take(Math.Clamp(limit, 1, 500))
            .ToList();
    }

    [McpServerTool, Description("Returns the most recent Game Coordinator trace entries (in/out/error/unhandled), optionally filtered by AppID.")]
    public static object GetRecentGcMessages(SteamApiStateService state, GameCoordinatorTraceService trace,
        [Description("Admin session/bearer token")] string token,
        [Description("Optional AppID filter")] uint? appId = null,
        [Description("Maximum entries to return (default 50)")] int limit = 50)
    {
        if (!state.IsWebAdmin(token)) return Unauthorized;

        return trace.GetSince(0)
            .Where(e => appId == null || e.AppId == appId)
            .OrderByDescending(e => e.Seq)
            .Take(Math.Clamp(limit, 1, 500))
            .ToList();
    }

    [McpServerTool, Description("Lists currently registered game servers, optionally filtered by AppID.")]
    public static object ListGameServers(SteamApiStateService state,
        [Description("Admin session/bearer token")] string token,
        [Description("Optional AppID filter")] uint? appId = null)
    {
        var overview = state.GetAdminOverviewForSession(token);
        if (overview == null) return Unauthorized;

        return overview.GameServers
            .Where(server => appId == null || server.AppId == appId)
            .ToList();
    }

    [McpServerTool, Description("Returns the most recent SKYNET server application log lines.")]
    public static object GetServerLogs(SteamApiStateService state, InMemoryLogBufferProvider logs,
        [Description("Admin session/bearer token")] string token,
        [Description("Maximum lines to return (default 100)")] int count = 100)
    {
        if (!state.IsWebAdmin(token)) return Unauthorized;

        return logs.GetRecent(Math.Clamp(count, 1, 500));
    }
}

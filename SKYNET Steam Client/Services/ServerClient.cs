using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using SKYNET.Client.Models;

namespace SKYNET.Client.Services;

/// <summary>
/// Talks to the SKYNET server exactly like the emulator DLL: it POSTs
/// /api/auth/steam/session with UseActiveWebUser=true, and the server resolves the
/// web user logged in from this machine's IP (GetActiveWebUserForIp). No web user =>
/// send the user to the web login page.
/// </summary>
public sealed class ServerClient
{
    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(8) };

    // Long-poll requests are held open by the server for up to the requested wait, so
    // they get their own client with no global timeout (each call sets its own deadline).
    private readonly HttpClient _pollHttp = new() { Timeout = System.Threading.Timeout.InfiniteTimeSpan };

    // Avatars rarely change and the Users list is refreshed on every presence event.
    private readonly System.Collections.Concurrent.ConcurrentDictionary<uint, (byte[]? Png, DateTime FetchedUtc)> _avatarCache = new();
    private static readonly TimeSpan AvatarCacheTtl = TimeSpan.FromMinutes(5);
    private string _baseUrl = "http://127.0.0.1:27080/";

    public void Configure(string baseUrl)
    {
        _baseUrl = baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
    }

    public string LoginUrl => _baseUrl + "login";

    public string BaseUrl => _baseUrl;

    /// <summary>True if the given base URL answers an HTTP request quickly.</summary>
    public async Task<bool> IsReachableAsync(string baseUrl, int timeoutMs = 700)
    {
        try
        {
            var url = baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
            using var cts = new System.Threading.CancellationTokenSource(timeoutMs);
            using var resp = await _http.GetAsync(url, cts.Token).ConfigureAwait(false);
            return true; // any HTTP status means the server is there
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Makes sure we point at a live server: keeps the configured URL if it answers,
    /// otherwise (when enabled) auto-discovers by broadcast and persists the result.
    /// Returns the reachable base URL, or null if none was found.
    /// </summary>
    public async Task<string?> EnsureServerAsync(AppConfig app)
    {
        Configure(app.ServerUrl);
        if (await IsReachableAsync(app.ServerUrl).ConfigureAwait(false))
            return _baseUrl;

        if (!app.AutoDiscoverServer)
            return null;

        var discovered = await Task.Run(() => ServerDiscovery.Discover(app.DiscoveryPort)).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(discovered))
            return null;

        if (!await IsReachableAsync(discovered!).ConfigureAwait(false))
            return null;

        app.ServerUrl = discovered!;
        Configure(discovered!);
        return _baseUrl;
    }

    /// <summary>Token of the launcher's current session; empty when there is none.</summary>
    public string? CurrentToken { get; private set; }

    /// <summary>
    /// Resolves the active web user and opens a session. With presence=true the session
    /// is a "launcher" session (heartbeating, shows the user ONLINE); with presence=false
    /// it is a "probe" (identity lookup only, never counts as presence).
    /// </summary>
    public async Task<SessionResult> ResolveSessionAsync(AppConfig app, bool presence = true, bool discover = true)
    {
        if (discover)
            await EnsureServerAsync(app).ConfigureAwait(false);

        var request = new SessionRequestDto
        {
            ClientInstanceId = app.ClientInstanceId,
            ProcessRole = presence ? "launcher" : "probe",
            UseActiveWebUser = true
        };

        try
        {
            using var resp = await _http.PostAsJsonAsync(_baseUrl + "api/auth/steam/session", request, Json).ConfigureAwait(false);
            if (resp.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (presence) CurrentToken = null;
                return SessionResult.NotLoggedIn();
            }
            if (!resp.IsSuccessStatusCode)
                return SessionResult.Unavailable($"Server returned {(int)resp.StatusCode}");

            var dto = await resp.Content.ReadFromJsonAsync<SessionResultDto>(Json).ConfigureAwait(false);
            if (dto?.User == null)
            {
                if (presence) CurrentToken = null;
                return SessionResult.NotLoggedIn();
            }

            var user = new WebUser
            {
                AccountId = dto.User.AccountId,
                SteamId = dto.User.SteamId,
                DisplayName = string.IsNullOrWhiteSpace(dto.User.PersonaName) ? "Player" : dto.User.PersonaName,
                // Having a resolved session means this machine is signed in and
                // present -> show online. (The server's GameState stays "offline"
                // until a game reports rich presence, which would read as offline here.)
                Online = true,
                Level = PresenceLevel.Online,
                IsAdmin = dto.IsAdmin
            };
            user.AvatarPng = await FetchAvatarAsync(user.AccountId).ConfigureAwait(false);
            if (presence) CurrentToken = dto.AccessToken;
            return SessionResult.Ok(user, dto.AccessToken);
        }
        catch (Exception ex)
        {
            return SessionResult.Unavailable(ex.Message);
        }
    }

    /// <summary>
    /// One long-poll on the server's event stream (the same GET /api/events the emulator
    /// uses). The server holds the request until an event arrives or <paramref name="waitMs"/>
    /// elapses, so a pending poll is both the launcher's keep-alive (every authenticated call
    /// refreshes the session) and the push channel for friend / presence changes.
    /// A null <paramref name="cursor"/> is the first poll: it only learns the current cursor.
    /// </summary>
    public async Task<EventPollResult> PollEventsAsync(string? token, string? cursor, int waitMs, System.Threading.CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(token)) return new EventPollResult { Status = HeartbeatResult.Unauthorized };
        try
        {
            var url = _baseUrl + "api/events?since=" + Uri.EscapeDataString(cursor ?? "") + "&waitMs=" + Math.Max(0, waitMs);
            using var linked = System.Threading.CancellationTokenSource.CreateLinkedTokenSource(ct);
            linked.CancelAfter(Math.Max(0, waitMs) + 10000);
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            using var resp = await _pollHttp.SendAsync(req, linked.Token).ConfigureAwait(false);
            if (resp.StatusCode == HttpStatusCode.Unauthorized) return new EventPollResult { Status = HeartbeatResult.Unauthorized };
            if (!resp.IsSuccessStatusCode) return new EventPollResult { Status = HeartbeatResult.Unavailable };

            var envelope = await resp.Content.ReadFromJsonAsync<EventEnvelopeDto>(Json, linked.Token).ConfigureAwait(false);
            // The server answers an unknown/expired token with 200 and an EMPTY cursor; a
            // live session always gets a non-empty one.
            if (envelope == null || string.IsNullOrEmpty(envelope.Cursor))
                return new EventPollResult { Status = HeartbeatResult.Unauthorized };

            var changed = envelope.Events != null && envelope.Events.Any(ev =>
                ev.Type != null &&
                (ev.Type.StartsWith("persona", StringComparison.OrdinalIgnoreCase) ||
                 ev.Type.StartsWith("friend", StringComparison.OrdinalIgnoreCase)));
            return new EventPollResult { Status = HeartbeatResult.Ok, Cursor = envelope.Cursor, PresenceChanged = changed };
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return new EventPollResult { Status = HeartbeatResult.Unavailable };
        }
    }

    /// <summary>
    /// Best-effort "launcher closed" notice so friends see us offline right away. Bounded to
    /// about 2 seconds and never throws; a crash falls back to the server's presence timeout.
    /// </summary>
    public void GoOffline()
    {
        var token = CurrentToken;
        CurrentToken = null;
        if (string.IsNullOrWhiteSpace(token)) return;
        try
        {
            var url = _baseUrl + "api/presence/offline";
            Task.Run(async () =>
            {
                try
                {
                    using var cts = new System.Threading.CancellationTokenSource(1500);
                    using var req = new HttpRequestMessage(HttpMethod.Post, url);
                    req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    using var resp = await _http.SendAsync(req, cts.Token).ConfigureAwait(false);
                }
                catch { }
            }).Wait(2000);
        }
        catch { }
    }

    /// <summary>Community users from the server (requires a session token).</summary>
    public async Task<List<WebUser>> GetUsersAsync(string? token)
    {
        var result = new List<WebUser>();
        if (string.IsNullOrWhiteSpace(token)) return result;
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, _baseUrl + "api/users");
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            using var resp = await _http.SendAsync(req).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode) return result;

            var dtos = await resp.Content.ReadFromJsonAsync<List<ApiUserDto>>(Json).ConfigureAwait(false);
            if (dtos == null) return result;
            foreach (var d in dtos)
            {
                var level = ToLevel(d);
                result.Add(new WebUser
                {
                    AccountId = d.AccountId,
                    SteamId = d.SteamId,
                    DisplayName = string.IsNullOrWhiteSpace(d.PersonaName) ? "Player" : d.PersonaName,
                    Level = level,
                    Online = level != PresenceLevel.Offline,
                    AppId = d.AppId,
                    PresenceDetail = ToDetail(d)
                });
            }

            // Fetch avatars in parallel (loopback, cheap).
            await Task.WhenAll(result.Select(async u => u.AvatarPng = await FetchAvatarAsync(u.AccountId).ConfigureAwait(false)))
                      .ConfigureAwait(false);
        }
        catch { /* return whatever we have */ }
        return result;
    }

    /// <summary>Admin overview (server status, users, lobbies, game servers, settings).
    /// Returns null if unreachable or the token is not an admin session (server returns 403).</summary>
    public async Task<AdminOverview?> GetAdminOverviewAsync(string? token)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, _baseUrl + "api/admin/overview");
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            using var resp = await _http.SendAsync(req).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<AdminOverview>(Json).ConfigureAwait(false);
        }
        catch
        {
            return null;
        }
    }

    public async Task<byte[]?> FetchAvatarAsync(uint accountId)
    {
        if (_avatarCache.TryGetValue(accountId, out var cached) && DateTime.UtcNow - cached.FetchedUtc < AvatarCacheTtl)
            return cached.Png;

        try
        {
            using var resp = await _http.GetAsync(_baseUrl + $"Images/AvatarCache/{accountId}.jpg").ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode) return null;
            var png = await resp.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            _avatarCache[accountId] = (png, DateTime.UtcNow);
            return png;
        }
        catch
        {
            return null;
        }
    }

    // Older servers send no Presence: fall back to GameState.
    private static PresenceLevel ToLevel(ApiUserDto d)
    {
        switch ((d.Presence ?? "").ToLowerInvariant())
        {
            case "in_game": return PresenceLevel.InGame;
            case "online": return PresenceLevel.Online;
            case "offline": return PresenceLevel.Offline;
        }
        return string.Equals(d.GameState, "offline", StringComparison.OrdinalIgnoreCase)
            ? PresenceLevel.Offline
            : PresenceLevel.InGame;
    }

    private static string ToDetail(ApiUserDto d)
    {
        if (d.RichPresence != null)
        {
            if (d.RichPresence.TryGetValue("status", out var status) && !string.IsNullOrWhiteSpace(status)) return status;
            if (d.RichPresence.TryGetValue("steam_display", out var display) && !string.IsNullOrWhiteSpace(display)) return display;
        }
        if (string.Equals(d.GameState, "in_match", StringComparison.OrdinalIgnoreCase)) return "In match";
        if (string.Equals(d.GameState, "menu", StringComparison.OrdinalIgnoreCase)) return "In menu";
        return "";
    }

    // ---- wire DTOs ----
    private sealed class SessionRequestDto
    {
        public uint AccountId { get; set; }
        public ulong SteamId { get; set; }
        public uint AppId { get; set; }
        public string PersonaName { get; set; } = "";
        public string ClientInstanceId { get; set; } = "";
        public string ProcessRole { get; set; } = "client";
        public bool UseActiveWebUser { get; set; }
    }

    private sealed class SessionResultDto
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public ApiUserDto? User { get; set; }
        public bool IsAdmin { get; set; }
    }

    private sealed class EventEnvelopeDto
    {
        public string? Cursor { get; set; }
        public List<EventDto>? Events { get; set; }
    }

    private sealed class EventDto
    {
        public string? Type { get; set; }
    }

    private sealed class ApiUserDto
    {
        public uint AccountId { get; set; }
        public ulong SteamId { get; set; }
        public string PersonaName { get; set; } = "";
        public int PersonaState { get; set; }
        public string GameState { get; set; } = "offline";
        public string? Presence { get; set; }
        public uint AppId { get; set; }
        public Dictionary<string, string>? RichPresence { get; set; }
    }
}

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

    public async Task<SessionResult> ResolveSessionAsync(AppConfig app)
    {
        await EnsureServerAsync(app).ConfigureAwait(false);

        var request = new SessionRequestDto
        {
            ClientInstanceId = app.ClientInstanceId,
            ProcessRole = "client",
            UseActiveWebUser = true
        };

        try
        {
            using var resp = await _http.PostAsJsonAsync(_baseUrl + "api/auth/steam/session", request, Json).ConfigureAwait(false);
            if (resp.StatusCode == HttpStatusCode.Unauthorized)
                return SessionResult.NotLoggedIn();
            if (!resp.IsSuccessStatusCode)
                return SessionResult.Unavailable($"Server returned {(int)resp.StatusCode}");

            var dto = await resp.Content.ReadFromJsonAsync<SessionResultDto>(Json).ConfigureAwait(false);
            if (dto?.User == null)
                return SessionResult.NotLoggedIn();

            var user = new WebUser
            {
                AccountId = dto.User.AccountId,
                SteamId = dto.User.SteamId,
                DisplayName = string.IsNullOrWhiteSpace(dto.User.PersonaName) ? "Player" : dto.User.PersonaName,
                // Having a resolved session means this machine is signed in and
                // present -> show online. (The server's GameState stays "offline"
                // until a game reports rich presence, which would read as offline here.)
                Online = true
            };
            user.AvatarPng = await FetchAvatarAsync(user.AccountId).ConfigureAwait(false);
            return SessionResult.Ok(user, dto.AccessToken);
        }
        catch (Exception ex)
        {
            return SessionResult.Unavailable(ex.Message);
        }
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
                result.Add(new WebUser
                {
                    AccountId = d.AccountId,
                    SteamId = d.SteamId,
                    DisplayName = string.IsNullOrWhiteSpace(d.PersonaName) ? "Player" : d.PersonaName,
                    Online = !string.Equals(d.GameState, "offline", StringComparison.OrdinalIgnoreCase)
                });
            }

            // Fetch avatars in parallel (loopback, cheap).
            await Task.WhenAll(result.Select(async u => u.AvatarPng = await FetchAvatarAsync(u.AccountId).ConfigureAwait(false)))
                      .ConfigureAwait(false);
        }
        catch { /* return whatever we have */ }
        return result;
    }

    public async Task<byte[]?> FetchAvatarAsync(uint accountId)
    {
        try
        {
            using var resp = await _http.GetAsync(_baseUrl + $"Images/AvatarCache/{accountId}.jpg").ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }
        catch
        {
            return null;
        }
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
    }

    private sealed class ApiUserDto
    {
        public uint AccountId { get; set; }
        public ulong SteamId { get; set; }
        public string PersonaName { get; set; } = "";
        public int PersonaState { get; set; }
        public string GameState { get; set; } = "offline";
    }
}

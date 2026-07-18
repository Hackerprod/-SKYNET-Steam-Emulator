namespace SKYNET.Client.Models;

/// <summary>Result of resolving the active web user from the server.</summary>
public enum SessionStatus
{
    /// <summary>A web user is logged in and was returned.</summary>
    Authenticated,
    /// <summary>Server reachable but no web user logged in from this machine -> send to web login.</summary>
    NotLoggedIn,
    /// <summary>Server unreachable / error.</summary>
    ServerUnavailable
}

/// <summary>The logged-in user's public profile, as shown in the dashboard header.</summary>
public sealed class WebUser
{
    public ulong SteamId { get; set; }
    public uint AccountId { get; set; }
    public string DisplayName { get; set; } = "";
    public string? AvatarUrl { get; set; }
    public byte[]? AvatarPng { get; set; }
    public bool Online { get; set; }
}

public sealed class SessionResult
{
    public SessionStatus Status { get; set; }
    public WebUser? User { get; set; }
    public string? AccessToken { get; set; }
    public string? Error { get; set; }

    public static SessionResult Unavailable(string? error = null) =>
        new() { Status = SessionStatus.ServerUnavailable, Error = error };
    public static SessionResult NotLoggedIn() => new() { Status = SessionStatus.NotLoggedIn };
    public static SessionResult Ok(WebUser user, string? token) =>
        new() { Status = SessionStatus.Authenticated, User = user, AccessToken = token };
}

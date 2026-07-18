using System.Text.Json.Serialization;

namespace SKYNET.Client.Models;

/// <summary>Process bitness of a game executable.</summary>
public enum GameArch { Unknown, X86, X64 }

/// <summary>
/// A game the user added to the launcher. Persisted to the library JSON.
/// Per-game emulator settings live in <see cref="Ini"/> and are written into a
/// steam_api.ini next to the game when launched.
/// </summary>
public sealed class GameEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = "";
    public uint AppId { get; set; }
    public string ExecutablePath { get; set; } = "";

    /// <summary>Working directory; defaults to the exe folder when empty.</summary>
    public string WorkingDirectory { get; set; } = "";

    /// <summary>Extra command-line arguments passed to the game.</summary>
    public string LaunchArguments { get; set; } = "";

    public GameArch Arch { get; set; } = GameArch.Unknown;

    /// <summary>PNG bytes of the extracted exe icon (base64 in JSON). Optional.</summary>
    public byte[]? IconPng { get; set; }

    public DateTimeOffset AddedUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastPlayedUtc { get; set; }

    /// <summary>Per-game emulator settings, mirrored into steam_api.ini on launch.</summary>
    public GameIniSettings Ini { get; set; } = new();

    [JsonIgnore]
    public string ExeFolder =>
        string.IsNullOrWhiteSpace(ExecutablePath) ? "" : Path.GetDirectoryName(ExecutablePath) ?? "";

    [JsonIgnore]
    public bool ExeExists => !string.IsNullOrWhiteSpace(ExecutablePath) && File.Exists(ExecutablePath);
}

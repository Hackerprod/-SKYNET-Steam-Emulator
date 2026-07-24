using System.Globalization;

namespace SKYNET_server.Services;

/// <summary>
/// Resolves the Dota build number from the installation selected in Dota Cosmetics.
/// The reader accepts a Dota root, its game directory, or game\dota, so the server
/// owns path normalization and the GC only consumes the persisted build number.
/// </summary>
internal static class DotaSteamInfReader
{
    public static bool TryReadClientVersion(string? dotaPath, out uint clientVersion, out string steamInfPath, out string error)
    {
        clientVersion = 0;
        steamInfPath = string.Empty;
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(dotaPath))
        {
            error = "Dota path is not configured.";
            return false;
        }

        string root;
        try
        {
            root = Path.GetFullPath(dotaPath.Trim().Trim('"'));
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
        {
            error = $"Invalid Dota path: {ex.Message}";
            return false;
        }

        var candidates = new[]
        {
            Path.Combine(root, "game", "dota", "steam.inf"),
            Path.Combine(root, "dota", "steam.inf"),
            Path.Combine(root, "steam.inf")
        };
        steamInfPath = candidates.FirstOrDefault(File.Exists) ?? string.Empty;
        if (steamInfPath.Length == 0)
        {
            error = $"steam.inf was not found under '{root}'.";
            return false;
        }

        try
        {
            using var stream = new FileStream(steamInfPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            using var reader = new StreamReader(stream);
            while (reader.ReadLine() is { } line)
            {
                var entry = line.Trim();
                if (entry.Length == 0 || entry.StartsWith('#') || entry.StartsWith(';') || entry.StartsWith("//", StringComparison.Ordinal))
                {
                    continue;
                }

                var separator = entry.IndexOf('=');
                if (separator <= 0 || !string.Equals(entry[..separator].Trim(), "ClientVersion", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var value = entry[(separator + 1)..].Trim();
                if (uint.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out clientVersion) && clientVersion > 0)
                {
                    return true;
                }

                error = $"ClientVersion in '{steamInfPath}' is not a positive unsigned integer.";
                clientVersion = 0;
                return false;
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            error = $"Could not read '{steamInfPath}': {ex.Message}";
            return false;
        }

        error = $"ClientVersion is missing from '{steamInfPath}'.";
        return false;
    }
}

namespace SKYNET_server.Services;

/// <summary>
/// Shared resolution of the GC content root used by every GC engine
/// (Lua scripts, JS bundles, routing table). Honors SKYNET_GC_ROOT, otherwise
/// walks up from the content root looking for a valid GC folder.
/// </summary>
public static class GcPaths
{
    public static string ResolveGcRoot(string contentRootPath)
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
            && File.Exists(Path.Combine(path, "570", "main.lua"));
    }
}

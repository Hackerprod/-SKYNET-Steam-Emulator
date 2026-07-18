using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace SkynetServer.GcTests;

/// <summary>Minimal host plumbing pointing at the real repo GC tree.</summary>
public static class TestHost
{
    public static string RepoRoot { get; } = FindRepoRoot();
    public static string ServerRoot => Path.Combine(RepoRoot, "SKYNET server");

    public static IHostEnvironment Environment(string? contentRoot = null)
        => new FakeHostEnvironment(contentRoot ?? ServerRoot);

    public static IConfiguration EmptyConfiguration()
        => new ConfigurationBuilder().Build();

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, "SKYNET server", "GC", "570")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new InvalidOperationException("Repo root not found from " + AppContext.BaseDirectory);
    }

    private sealed class FakeHostEnvironment : IHostEnvironment
    {
        public FakeHostEnvironment(string contentRoot) => ContentRootPath = contentRoot;

        public string EnvironmentName { get; set; } = "Testing";
        public string ApplicationName { get; set; } = "SkynetServer.GcTests";
        public string ContentRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}

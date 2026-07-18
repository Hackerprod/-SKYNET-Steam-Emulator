using Microsoft.Extensions.Logging.Abstractions;
using SKYNET_server.Services;
using Xunit;

namespace SkynetServer.GcTests;

[Collection("gc-serial")]
public class GcRoutingTableTests : IDisposable
{
    private readonly string _tempRoot;

    public GcRoutingTableTests()
    {
        // Isolated GC root (must contain 570/main.lua to be considered valid).
        _tempRoot = Path.Combine(Path.GetTempPath(), "skynet-gc-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(_tempRoot, "570"));
        File.WriteAllText(Path.Combine(_tempRoot, "570", "main.lua"), "-- test stub");
        Environment.SetEnvironmentVariable("SKYNET_GC_ROOT", _tempRoot);
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable("SKYNET_GC_ROOT", null);
        try { Directory.Delete(_tempRoot, recursive: true); } catch { /* best effort */ }
    }

    private GcRoutingTable CreateTable()
        => new(TestHost.Environment(_tempRoot), NullLogger<GcRoutingTable>.Instance);

    private string RoutingPath => Path.Combine(_tempRoot, GcRoutingTable.FileName);

    private void WriteRouting(string json, DateTime? stampUtc = null)
    {
        File.WriteAllText(RoutingPath, json);
        if (stampUtc.HasValue)
        {
            File.SetLastWriteTimeUtc(RoutingPath, stampUtc.Value);
        }
    }

    [Fact]
    public void Missing_File_Means_Everything_Stays_On_Lua()
    {
        var table = CreateTable();
        Assert.False(table.IsMigratedToJs(570, 4006));
        Assert.Equal("lua", table.PollEngine(570));
    }

    [Fact]
    public void Migrated_Id_Routes_To_Js_And_Rollback_Flags_Work()
    {
        WriteRouting("""
        {
          "jintEnabled": true,
          "apps": { "570": { "pollEngine": "lua", "migratedMessageIds": [4006], "disabledMessageIds": [] } }
        }
        """);
        var table = CreateTable();
        Assert.True(table.IsMigratedToJs(570, 4006));
        Assert.False(table.IsMigratedToJs(570, 4007));
        Assert.False(table.IsMigratedToJs(730, 4006));

        // per-id rollback via disabledMessageIds (hot-reload on mtime change)
        WriteRouting("""
        {
          "jintEnabled": true,
          "apps": { "570": { "pollEngine": "lua", "migratedMessageIds": [4006], "disabledMessageIds": [4006] } }
        }
        """, DateTime.UtcNow.AddSeconds(3));
        Assert.False(table.IsMigratedToJs(570, 4006));
    }

    [Fact]
    public void Global_KillSwitch_Env_Var_Forces_Lua()
    {
        WriteRouting("""
        {
          "jintEnabled": true,
          "apps": { "570": { "pollEngine": "js", "migratedMessageIds": [4006], "disabledMessageIds": [] } }
        }
        """);
        var table = CreateTable();
        Assert.True(table.IsMigratedToJs(570, 4006));

        Environment.SetEnvironmentVariable("SKYNET_GC_JS_DISABLED", "1");
        try
        {
            Assert.False(table.IsMigratedToJs(570, 4006));
            Assert.Equal("lua", table.PollEngine(570));
        }
        finally
        {
            Environment.SetEnvironmentVariable("SKYNET_GC_JS_DISABLED", null);
        }
    }

    [Fact]
    public void Malformed_File_Keeps_Previous_Table()
    {
        WriteRouting("""
        {
          "jintEnabled": true,
          "apps": { "570": { "pollEngine": "lua", "migratedMessageIds": [4006], "disabledMessageIds": [] } }
        }
        """);
        var table = CreateTable();
        Assert.True(table.IsMigratedToJs(570, 4006));

        WriteRouting("{ this is not json", DateTime.UtcNow.AddSeconds(3));
        Assert.True(table.IsMigratedToJs(570, 4006));
    }
}

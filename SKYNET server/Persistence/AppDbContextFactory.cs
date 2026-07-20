using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SKYNET_server.Persistence;

public sealed class SteamDbContextFactory : IDesignTimeDbContextFactory<SteamDbContext>
{
    public SteamDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<SteamDbContext>()
            .UseSqlite("Data Source=Data/steam.db")
            .Options;
        return new SteamDbContext(options);
    }
}

public sealed class DotaDbContextFactory : IDesignTimeDbContextFactory<DotaDbContext>
{
    public DotaDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<DotaDbContext>()
            .UseSqlite("Data Source=Data/dota.db")
            .Options;
        return new DotaDbContext(options);
    }
}

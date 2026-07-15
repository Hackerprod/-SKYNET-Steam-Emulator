using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SKYNET_server.Persistence;

/// <summary>
/// Design-time factory so `dotnet ef` can build the model and generate
/// migrations without booting the whole web host. The runtime registration
/// (connection string, pooling, migrate-on-start) lives in Program.cs.
/// </summary>
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=Data/app.db")
            .Options;

        return new AppDbContext(options);
    }
}

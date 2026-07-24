using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace SKYNET_server.Persistence;

internal static class DatabaseSchemaMaintenance
{
    private const string SteamComponent = "steam-core";
    private const string DotaComponent = "dota-core";
    private const int SteamVersion = 1;
    private const int DotaVersion = 2;

    public static void EnsureCurrent(SteamDbContext steam, DotaDbContext dota)
    {
        EnsureSteamCurrent(steam);
        EnsureDotaCurrent(dota);
    }

    public static void EnsureSteamCurrent(SteamDbContext context)
    {
        EnsureSchemaVersionTable(context);
        if (GetVersion(context, SteamComponent) < SteamVersion)
        {
            SetVersion(context, SteamComponent, SteamVersion);
        }
    }

    public static void EnsureDotaCurrent(DotaDbContext context)
    {
        EnsureSchemaVersionTable(context);
        if (GetVersion(context, DotaComponent) < DotaVersion)
        {
            EnsureDotaEquipmentKey(context);
            EnsureDotaCosmeticClientVersion(context);
            SetVersion(context, DotaComponent, DotaVersion);
        }
    }

    private static void EnsureSchemaVersionTable(DbContext context)
    {
        context.Database.ExecuteSqlRaw("""
            CREATE TABLE IF NOT EXISTS SchemaVersions (
                Component TEXT NOT NULL PRIMARY KEY,
                Version INTEGER NOT NULL,
                AppliedAtUtc TEXT NOT NULL
            );
            """);
    }

    private static int GetVersion(DbContext context, string component)
    {
        var connection = (SqliteConnection)context.Database.GetDbConnection();
        var closeWhenDone = connection.State == System.Data.ConnectionState.Closed;
        if (closeWhenDone)
        {
            connection.Open();
        }

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Version FROM SchemaVersions WHERE Component = $component LIMIT 1;";
            command.Parameters.AddWithValue("$component", component);
            return Convert.ToInt32(command.ExecuteScalar() ?? 0);
        }
        finally
        {
            if (closeWhenDone)
            {
                connection.Close();
            }
        }
    }

    private static void SetVersion(DbContext context, string component, int version)
    {
        var now = DateTime.UtcNow.ToString("O", System.Globalization.CultureInfo.InvariantCulture);
        context.Database.ExecuteSqlRaw(
            """
            INSERT INTO SchemaVersions (Component, Version, AppliedAtUtc)
            VALUES ({0}, {1}, {2})
            ON CONFLICT(Component) DO UPDATE SET
                Version = excluded.Version,
                AppliedAtUtc = excluded.AppliedAtUtc;
            """,
            component,
            version,
            now);
    }

    private static void EnsureDotaEquipmentKey(DotaDbContext context)
    {
        var connection = (SqliteConnection)context.Database.GetDbConnection();
        var closeWhenDone = connection.State == System.Data.ConnectionState.Closed;
        if (closeWhenDone)
        {
            connection.Open();
        }

        try
        {
            if (!TableExists(connection, "DotaEquipment"))
            {
                return;
            }

            var primaryKey = GetPrimaryKeyColumns(connection, "DotaEquipment");
            if (primaryKey.SequenceEqual(new[] { "SteamId", "HeroId", "SlotId", "DefIndex" }, StringComparer.OrdinalIgnoreCase))
            {
                return;
            }

            // Global loadout items share HeroId/SlotId pairs, so DefIndex is part
            // of the durable identity. Older schemas collapsed those rows.
            Execute(connection, null, "PRAGMA foreign_keys=OFF;");
            using var transaction = connection.BeginTransaction();
            try
            {
                Execute(connection, transaction, """
                    CREATE TABLE DotaEquipment_new (
                        SteamId INTEGER NOT NULL,
                        HeroId INTEGER NOT NULL,
                        SlotId INTEGER NOT NULL,
                        HeroName TEXT NOT NULL,
                        Slot TEXT NOT NULL,
                        DefIndex INTEGER NOT NULL,
                        ItemId INTEGER NOT NULL,
                        Style INTEGER NOT NULL,
                        UpdatedAt TEXT NOT NULL,
                        CONSTRAINT PK_DotaEquipment PRIMARY KEY (SteamId, HeroId, SlotId, DefIndex)
                    );
                    """);
                Execute(connection, transaction, """
                    INSERT OR REPLACE INTO DotaEquipment_new
                        (SteamId, HeroId, SlotId, HeroName, Slot, DefIndex, ItemId, Style, UpdatedAt)
                    SELECT SteamId, HeroId, SlotId, HeroName, Slot, DefIndex, ItemId, Style, UpdatedAt
                    FROM DotaEquipment
                    ORDER BY UpdatedAt;
                    """);
                Execute(connection, transaction, "DROP TABLE DotaEquipment;");
                Execute(connection, transaction, "ALTER TABLE DotaEquipment_new RENAME TO DotaEquipment;");
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                Execute(connection, null, "PRAGMA foreign_keys=ON;");
            }
        }
        finally
        {
            if (closeWhenDone)
            {
                connection.Close();
            }
        }
    }

    private static void EnsureDotaCosmeticClientVersion(DotaDbContext context)
    {
        var connection = (SqliteConnection)context.Database.GetDbConnection();
        var closeWhenDone = connection.State == System.Data.ConnectionState.Closed;
        if (closeWhenDone)
        {
            connection.Open();
        }

        try
        {
            if (TableExists(connection, "CosmeticSettings") && !ColumnExists(connection, "CosmeticSettings", "ClientVersion"))
            {
                Execute(connection, null, "ALTER TABLE CosmeticSettings ADD COLUMN ClientVersion INTEGER NOT NULL DEFAULT 0;");
            }
        }
        finally
        {
            if (closeWhenDone)
            {
                connection.Close();
            }
        }
    }

    private static bool TableExists(SqliteConnection connection, string tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1 FROM sqlite_master WHERE type = 'table' AND name = $name LIMIT 1;";
        command.Parameters.AddWithValue("$name", tableName);
        return command.ExecuteScalar() != null;
    }

    private static List<string> GetPrimaryKeyColumns(SqliteConnection connection, string tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info({QuoteIdentifier(tableName)});";
        using var reader = command.ExecuteReader();
        var columns = new List<(int Order, string Name)>();
        while (reader.Read())
        {
            var pkOrder = reader.GetInt32(reader.GetOrdinal("pk"));
            if (pkOrder > 0)
            {
                columns.Add((pkOrder, reader.GetString(reader.GetOrdinal("name"))));
            }
        }

        return columns.OrderBy(column => column.Order).Select(column => column.Name).ToList();
    }

    private static bool ColumnExists(SqliteConnection connection, string tableName, string columnName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info({QuoteIdentifier(tableName)});";
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            if (string.Equals(reader.GetString(reader.GetOrdinal("name")), columnName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static void Execute(SqliteConnection connection, SqliteTransaction? transaction, string sql)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private static string QuoteIdentifier(string value) => "\"" + value.Replace("\"", "\"\"") + "\"";
}

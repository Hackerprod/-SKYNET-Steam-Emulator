using System.Text.Json;
using Microsoft.Data.Sqlite;
using SKYNET_server.Json;

namespace SKYNET_server.Services;

public abstract class ScriptDatabase
{
    protected static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    protected ScriptDatabase(IHostEnvironment environment, IConfiguration configuration, string fileName)
    {
        var dataRoot = ResolveDataRoot(environment.ContentRootPath, configuration);
        Directory.CreateDirectory(dataRoot);
        DatabasePath = Path.Combine(dataRoot, fileName);
    }

    public string DatabasePath { get; }

    protected abstract void Initialize();

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = SkynetJsonSerializerOptions.CreateCompatible();
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.WriteIndented = false;
        return options;
    }

    protected void InitializeDatabase()
    {
        Initialize();
    }

    protected SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection($"Data Source={DatabasePath}");
        connection.Open();
        Execute(connection, "PRAGMA journal_mode=WAL;");
        Execute(connection, "PRAGMA busy_timeout=5000;");
        return connection;
    }

    protected static void Execute(SqliteConnection connection, string sql, params (string Name, object? Value)[] parameters)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        AddParameters(command, parameters);
        command.ExecuteNonQuery();
    }

    protected static object? Scalar(SqliteConnection connection, string sql, params (string Name, object? Value)[] parameters)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        AddParameters(command, parameters);
        return command.ExecuteScalar();
    }

    protected static string Json(object value)
    {
        return JsonSerializer.Serialize(value, JsonOptions);
    }

    protected static string NormalizeJson(string? json, string fallback)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return fallback;
        }

        try
        {
            using var _ = JsonDocument.Parse(json);
            return json;
        }
        catch (JsonException)
        {
            return fallback;
        }
    }

    protected static string UtcNow()
    {
        return DateTimeOffset.UtcNow.ToString("O", System.Globalization.CultureInfo.InvariantCulture);
    }

    protected static string Id(ulong value)
    {
        return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    protected static ulong ParseUInt64(string? value)
    {
        return ulong.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : 0UL;
    }

    protected static uint ParseUInt32(string? value)
    {
        return uint.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : 0U;
    }

    private static void AddParameters(SqliteCommand command, params (string Name, object? Value)[] parameters)
    {
        foreach (var parameter in parameters)
        {
            command.Parameters.AddWithValue(parameter.Name, parameter.Value ?? DBNull.Value);
        }
    }

    private static string ResolveDataRoot(string contentRootPath, IConfiguration configuration)
    {
        var configuredRoot = configuration.GetValue<string>("Data:Root")?.Trim();
        if (string.IsNullOrWhiteSpace(configuredRoot))
        {
            configuredRoot = Environment.GetEnvironmentVariable("SKYNET_DATA_ROOT")?.Trim();
        }

        if (!string.IsNullOrWhiteSpace(configuredRoot))
        {
            return Path.GetFullPath(configuredRoot);
        }

        var root = new DirectoryInfo(Path.GetFullPath(contentRootPath));
        for (var current = root; current != null; current = current.Parent)
        {
            if (File.Exists(Path.Combine(current.FullName, "SKYNET server.csproj")))
            {
                return Path.Combine(current.FullName, "Data");
            }

            var nestedProject = Path.Combine(current.FullName, "SKYNET server", "SKYNET server.csproj");
            if (File.Exists(nestedProject))
            {
                return Path.Combine(current.FullName, "SKYNET server", "Data");
            }
        }

        return Path.Combine(root.FullName, "Data");
    }
}

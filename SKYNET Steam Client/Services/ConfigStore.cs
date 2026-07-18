using System.Text.Json;
using SKYNET.Client.Models;

namespace SKYNET.Client.Services;

/// <summary>Loads and saves <see cref="AppConfig"/> to %AppData%\SKYNET\client\config.json.</summary>
public sealed class ConfigStore
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static string RootDir { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SKYNET", "client");

    private static string ConfigPath => Path.Combine(RootDir, "config.json");

    public AppConfig Config { get; private set; } = new();

    public void Load()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                Config = JsonSerializer.Deserialize<AppConfig>(json, JsonOpts) ?? new AppConfig();
            }
            else
            {
                Config = new AppConfig();
                Save();
            }
        }
        catch
        {
            Config = new AppConfig();
        }
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(RootDir);
            var tmp = ConfigPath + ".tmp";
            File.WriteAllText(tmp, JsonSerializer.Serialize(Config, JsonOpts));
            if (File.Exists(ConfigPath)) File.Replace(tmp, ConfigPath, null);
            else File.Move(tmp, ConfigPath);
        }
        catch { /* best-effort persistence */ }
    }
}

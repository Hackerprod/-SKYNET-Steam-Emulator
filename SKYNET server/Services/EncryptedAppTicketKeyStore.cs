using SKYNET_server.Persistence;

namespace SKYNET_server.Services;

/// <summary>
/// Loads the 256-bit private key used to issue encrypted application tickets.
/// Keys stay on the server and are reloaded after operator edits without restart.
/// </summary>
public sealed class EncryptedAppTicketKeyStore
{
    private const int KeyLength = 32;

    private readonly object _sync = new();
    private readonly string _path;
    private readonly string _templatePath;
    private readonly ILogger<EncryptedAppTicketKeyStore> _logger;
    private DateTime _loadedWriteUtc = DateTime.MinValue;
    private IReadOnlyDictionary<uint, byte[]> _keys = new Dictionary<uint, byte[]>();

    public EncryptedAppTicketKeyStore(
        IHostEnvironment environment,
        IConfiguration configuration,
        ILogger<EncryptedAppTicketKeyStore> logger)
    {
        _logger = logger;
        var dataRoot = DatabaseSplitMigrator.ResolveDataRoot(environment.ContentRootPath, configuration);
        _path = Path.Combine(dataRoot, "encrypted-app-ticket-keys.ini");
        _templatePath = Path.Combine(environment.ContentRootPath, "Assets", "encrypted-app-ticket-keys.ini");
        EnsureFile();
    }

    public bool TryGetKey(uint appId, out byte[] key)
    {
        EnsureLoaded();
        if (_keys.TryGetValue(appId, out var configured))
        {
            key = configured.ToArray();
            return true;
        }

        key = Array.Empty<byte>();
        return false;
    }

    private void EnsureFile()
    {
        try
        {
            if (File.Exists(_path))
            {
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            if (File.Exists(_templatePath))
            {
                File.Copy(_templatePath, _path);
                return;
            }

            File.WriteAllText(
                _path,
                "; Steam AppID = 64 hexadecimal characters (32 bytes)." + Environment.NewLine +
                "[EncryptedAppTicketKeys]" + Environment.NewLine);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not initialize encrypted app ticket key store at {Path}", _path);
        }
    }

    private void EnsureLoaded()
    {
        var writeUtc = File.Exists(_path) ? File.GetLastWriteTimeUtc(_path) : DateTime.MinValue;
        if (writeUtc == _loadedWriteUtc)
        {
            return;
        }

        lock (_sync)
        {
            writeUtc = File.Exists(_path) ? File.GetLastWriteTimeUtc(_path) : DateTime.MinValue;
            if (writeUtc == _loadedWriteUtc)
            {
                return;
            }

            var loaded = new Dictionary<uint, byte[]>();
            try
            {
                foreach (var rawLine in File.ReadLines(_path))
                {
                    var line = rawLine.Trim();
                    if (line.Length == 0 || line.StartsWith(';') || line.StartsWith('#') || line.StartsWith('['))
                    {
                        continue;
                    }

                    var separator = line.IndexOf('=');
                    if (separator <= 0 || !uint.TryParse(line[..separator].Trim(), out var appId))
                    {
                        _logger.LogWarning("Ignoring malformed encrypted ticket key entry in {Path}", _path);
                        continue;
                    }

                    var value = line[(separator + 1)..].Trim();
                    if (value.Length != KeyLength * 2 || !TryDecodeHex(value, out var key))
                    {
                        _logger.LogWarning(
                            "Ignoring encrypted ticket key for AppID {AppId}: expected exactly {Characters} hexadecimal characters",
                            appId,
                            KeyLength * 2);
                        continue;
                    }

                    loaded[appId] = key;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not read encrypted app ticket key store at {Path}", _path);
                return;
            }

            _keys = loaded;
            _loadedWriteUtc = writeUtc;
        }
    }

    private static bool TryDecodeHex(string value, out byte[] key)
    {
        try
        {
            key = Convert.FromHexString(value);
            return key.Length == KeyLength;
        }
        catch (FormatException)
        {
            key = Array.Empty<byte>();
            return false;
        }
    }
}

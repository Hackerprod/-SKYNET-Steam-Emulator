using System.Globalization;
using System.Text;

namespace SKYNET_server.Services.Networking;

/// <summary>
/// Generates sdr-relays.ini, the SDR POP list SKYNET advertises to clients.
/// The format matches the reference companion project (micro-chief/-SDR-Relay):
/// an [SDR] revision stamp, one [Relay.&lt;popId&gt;] section per relay node, and
/// optional [TypicalPing.&lt;from&gt;-&lt;to&gt;] sections describing inter-POP latency.
///
/// Regenerated once at startup from configuration, so the file always reflects
/// the current deployment instead of requiring manual upkeep.
/// </summary>
public sealed class SdrRelayConfigService
{
    private readonly string _iniPath;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SdrRelayConfigService> _logger;

    public SdrRelayConfigService(IHostEnvironment hostEnvironment, IConfiguration configuration, ILogger<SdrRelayConfigService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        var dataRoot = Persistence.DatabaseSplitMigrator.ResolveDataRoot(hostEnvironment.ContentRootPath, configuration);
        _iniPath = Path.Combine(dataRoot, "sdr-relays.ini");
    }

    public void Generate()
    {
        var relays = LoadRelays();
        var pings = LoadTypicalPings(relays);
        var configuredRevision = _configuration.GetValue<long>("Sdr:Revision");
        var revision = configuredRevision > 0 ? configuredRevision : DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var ini = new StringBuilder();
        ini.AppendLine("[SDR]");
        ini.AppendLine(FormattableString.Invariant($"Revision={revision}"));

        foreach (var relay in relays)
        {
            ini.AppendLine();
            ini.AppendLine($"[Relay.{relay.PopId}]");
            ini.AppendLine($"Address={relay.Address}");
            ini.AppendLine(FormattableString.Invariant($"Port={relay.Port}"));
            ini.AppendLine($"Description={relay.Description}");
            ini.AppendLine(FormattableString.Invariant($"Longitude={relay.Longitude.ToString("0.##", CultureInfo.InvariantCulture)}"));
            ini.AppendLine(FormattableString.Invariant($"Latitude={relay.Latitude.ToString("0.##", CultureInfo.InvariantCulture)}"));
            ini.AppendLine(FormattableString.Invariant($"Partners={relay.Partners}"));
            ini.AppendLine(FormattableString.Invariant($"Tier={relay.Tier}"));
        }

        foreach (var ping in pings)
        {
            ini.AppendLine();
            ini.AppendLine($"[TypicalPing.{ping.From}-{ping.To}]");
            ini.AppendLine($"From={ping.From}");
            ini.AppendLine($"To={ping.To}");
            ini.AppendLine(FormattableString.Invariant($"Ping={ping.Ping}"));
        }

        var directory = Path.GetDirectoryName(_iniPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(_iniPath, ini.ToString());
        _logger.LogInformation("SDR relay config written to {Path} ({RelayCount} relay(s), revision {Revision})",
            _iniPath, relays.Count, revision);
    }

    private List<SdrRelayNode> LoadRelays()
    {
        var configured = _configuration.GetSection("Sdr:Relays").Get<List<SdrRelayNode>>();
        if (configured is { Count: > 0 })
        {
            return configured;
        }

        // No explicit relay list: advertise the single local relay this process
        // already runs (see SdrRelayService), so the common single-node
        // deployment works with zero extra configuration.
        var address = _configuration["Server:AdvertisedIp"];
        if (string.IsNullOrWhiteSpace(address))
        {
            _logger.LogWarning("Sdr:Relays not configured and Server:AdvertisedIp is empty; sdr-relays.ini will advertise no relays");
            return new List<SdrRelayNode>();
        }

        var port = _configuration.GetValue("Sdr:RelayPort", 28009);
        return new List<SdrRelayNode>
        {
            new()
            {
                PopId = _configuration["Sdr:PopId"] is { Length: > 0 } popId ? popId : "sky",
                Address = address,
                Port = port,
                Description = "SKYNET Primary",
                Partners = 1,
                Tier = 0
            }
        };
    }

    private List<SdrTypicalPing> LoadTypicalPings(IReadOnlyList<SdrRelayNode> relays)
    {
        var configured = _configuration.GetSection("Sdr:TypicalPings").Get<List<SdrTypicalPing>>();
        if (configured is { Count: > 0 })
        {
            return configured;
        }

        // Default: chain each configured relay to the next one with a nominal
        // 1ms hop, so a multi-relay sdr-relays.ini is still valid without
        // requiring the operator to hand-write every pair.
        var pings = new List<SdrTypicalPing>();
        for (var i = 0; i < relays.Count - 1; i++)
        {
            pings.Add(new SdrTypicalPing { From = relays[i].PopId, To = relays[i + 1].PopId, Ping = 1 });
        }

        return pings;
    }

    private sealed class SdrRelayNode
    {
        public string PopId { get; set; } = "sky";
        public string Address { get; set; } = string.Empty;
        public int Port { get; set; } = 28009;
        public string Description { get; set; } = string.Empty;
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public int Partners { get; set; } = 1;
        public int Tier { get; set; }
    }

    private sealed class SdrTypicalPing
    {
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public int Ping { get; set; } = 1;
    }
}

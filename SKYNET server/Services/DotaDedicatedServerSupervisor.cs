using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using SKYNET_server.Models;

namespace SKYNET_server.Services;

/// <summary>
/// Owns dedicated Dota processes requested by the Dota GC. Reservations are
/// keyed by lobby and port, so a late GC handshake cannot bind to another lobby.
/// </summary>
public sealed class DotaDedicatedServerSupervisor : BackgroundService
{
    private readonly object _sync = new();
    private readonly ILogger<DotaDedicatedServerSupervisor> _logger;
    private readonly GameServerSettingsService _settings;
    private readonly string _executablePath;
    private readonly string _workingDirectory;
    private readonly string _payloadDllPath;
    private readonly string _diagnosticsDirectory;
    private readonly int _tvPortOffset;
    private readonly TimeSpan _startupTimeout;
    private readonly bool _showWindow;
    private readonly Dictionary<ulong, DedicatedReservation> _reservations = new();

    public DotaDedicatedServerSupervisor(
        IHostEnvironment environment,
        IConfiguration configuration,
        GameServerSettingsService settings,
        ILogger<DotaDedicatedServerSupervisor> logger)
    {
        _logger = logger;
        _settings = settings;
        _executablePath = ResolvePath(
            configuration.GetValue<string>("GameCoordinator:Dota:Dedicated:ExecutablePath"),
            environment.ContentRootPath,
            Path.Combine("..", "..", "Steam", "steamapps", "common", "dota 2 beta", "game", "bin", "win64", "dota2.exe"));
        _workingDirectory = ResolvePath(
            configuration.GetValue<string>("GameCoordinator:Dota:Dedicated:WorkingDirectory"),
            environment.ContentRootPath,
            Path.GetDirectoryName(_executablePath) ?? environment.ContentRootPath);
        _payloadDllPath = ResolvePath(
            configuration.GetValue<string>("GameCoordinator:Dota:Dedicated:PayloadDllPath"),
            environment.ContentRootPath,
            Path.Combine("..", "SKYNET Steam Client", "payload", "x64", "steam_api64.dll"));
        _diagnosticsDirectory = Path.GetFullPath(Path.Combine(environment.ContentRootPath, "..", ".tmp"));
        _tvPortOffset = Math.Clamp(configuration.GetValue("GameCoordinator:Dota:Dedicated:TvPortOffset", 10000), 1, 30000);
        _startupTimeout = TimeSpan.FromSeconds(Math.Clamp(configuration.GetValue("GameCoordinator:Dota:Dedicated:StartupTimeoutSeconds", 120), 10, 180));
        _showWindow = configuration.GetValue("GameCoordinator:Dota:Dedicated:ShowWindow", true);
    }

    public DedicatedLaunchResult Start(ulong lobbyId, string? requestedMap)
    {
        if (lobbyId == 0)
        {
            return DedicatedLaunchResult.Failed("Lobby id is required.");
        }

        // Read the live settings once per launch so an admin edit takes effect on
        // the next dedicated server without a restart.
        var settings = _settings.Current;

        lock (_sync)
        {
            SweepLocked(DateTime.UtcNow);
            if (_reservations.TryGetValue(lobbyId, out var existing) && existing.IsLive)
            {
                return new DedicatedLaunchResult(true, existing.Port, existing.State, string.Empty);
            }

            if (!settings.DedicatedEnabled)
            {
                return DedicatedLaunchResult.Failed("Dedicated servers are disabled by configuration.");
            }

            if (!File.Exists(_executablePath))
            {
                return DedicatedLaunchResult.Failed($"Dota dedicated executable was not found at '{_executablePath}'.");
            }

            if (!Directory.Exists(_workingDirectory))
            {
                return DedicatedLaunchResult.Failed($"Dota dedicated working directory was not found at '{_workingDirectory}'.");
            }

            if (!File.Exists(_payloadDllPath))
            {
                return DedicatedLaunchResult.Failed($"Dota dedicated emulator payload was not found at '{_payloadDllPath}'.");
            }

            var port = AllocatePortLocked(settings.DedicatedPortStart);
            if (port == 0)
            {
                return DedicatedLaunchResult.Failed("No available UDP/TCP port was found for a dedicated server.");
            }

            var map = NormalizeMap(requestedMap);
            var arguments = new List<string>
            {
                "-dedicated"
            };
            // Always insecure: the emulator cannot produce a valid VAC session, so a
            // "secure" dedicated only ever yields the client VAC popup/block. Passing
            // -insecure keeps dota2 from loading the VAC module at all.
            arguments.Add("-insecure");
            // Bind explicitly. Recent Dota dedicated builds can otherwise pick a
            // single interface, leaving advertised endpoints black-holed.
            if (TryNormalizeIPv4(settings.DedicatedBindIp, out var bindIp))
            {
                arguments.Add("-ip");
                arguments.Add(bindIp);
            }
            arguments.Add("-port");
            arguments.Add(port.ToString(System.Globalization.CultureInfo.InvariantCulture));
            arguments.Add("+tv_port");
            arguments.Add((port + _tvPortOffset).ToString(System.Globalization.CultureInfo.InvariantCulture));
            arguments.Add("+sv_lan");
            arguments.Add("0");
            // Generous netchannel timeout. A peer with a weaker CPU or missing
            // cosmetic assets can stall the client for 10-20s during the hero-select
            // loadout (EconItemSearch / UGC image resolution), going silent on the
            // wire; the default ~20s server timeout then drops it mid-load. Real
            // matchmaking is lenient during loading, so widen the window.
            arguments.Add("+cl_timeout");
            arguments.Add("120");
            arguments.Add("+map");
            arguments.Add(map);
            arguments.Add("-console");
            arguments.Add("-vconsole");
            arguments.Add("-novid");

            var environmentVariables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["SKYNET_DEDICATED_LOBBY_ID"] = lobbyId.ToString(System.Globalization.CultureInfo.InvariantCulture),
                ["SKYNET_DEDICATED_PORT"] = port.ToString(System.Globalization.CultureInfo.InvariantCulture),
                ["SKYNET_PROCESS_ROLE"] = "dedicated",
                ["SKYNET_LOG_SUFFIX"] = $"dedicated_{lobbyId}_{port}",
                ["SKYNET_DEDICATED_INSECURE"] = "1"
            };

            try
            {
                Directory.CreateDirectory(_diagnosticsDirectory);
                var outputPath = Path.Combine(_diagnosticsDirectory, $"dota_dedicated_{lobbyId}_{port}_stdout.log");
                var errorPath = Path.Combine(_diagnosticsDirectory, $"dota_dedicated_{lobbyId}_{port}_stderr.log");
                // The dedicated process is injected before it runs so it uses the
                // launcher's payload DLL, not any stale steam_api64.dll beside Dota.
                // steam_api resolves its ini and SKYNET logs from Dota's exe folder,
                // while the executable bytes remain untouched on disk.
                var process = InjectedProcessLauncher.Launch(
                    _executablePath,
                    _payloadDllPath,
                    arguments,
                    _workingDirectory,
                    environmentVariables,
                    _showWindow);
                File.WriteAllText(outputPath, $"Launched through payload injection: {_payloadDllPath}{Environment.NewLine}");
                File.WriteAllText(errorPath, string.Empty);

                var reservation = new DedicatedReservation(lobbyId, port, map, process, DateTime.UtcNow);
                _reservations[lobbyId] = reservation;
                _logger.LogInformation(
                    "Started Dota dedicated server pid {ProcessId} for lobby {LobbyId} on port {Port} map {Map}. Payload: {Payload}. Args: {Arguments}. Stdout: {StdoutPath}. Stderr: {StderrPath}",
                    process.Id, lobbyId, port, map, _payloadDllPath, string.Join(" ", arguments), outputPath, errorPath);
                return new DedicatedLaunchResult(true, port, reservation.State, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not start Dota dedicated server for lobby {LobbyId}", lobbyId);
                return DedicatedLaunchResult.Failed(ex.Message);
            }
        }
    }

    // GCGameServerInfo is the authoritative point at which a server identity
    // becomes usable. Match on the reserved game port, never on arrival order.
    public string ClaimLobby(ulong gameServerSteamId, uint reportedPort)
    {
        if (gameServerSteamId == 0 || reportedPort == 0)
        {
            return "0";
        }

        lock (_sync)
        {
            SweepLocked(DateTime.UtcNow);
            foreach (var reservation in _reservations.Values)
            {
                if (reservation.GameServerSteamId == gameServerSteamId)
                {
                    return reservation.LobbyId.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }

                if ((reservation.State == DedicatedReservationState.Starting || reservation.State == DedicatedReservationState.Registered) &&
                    reservation.Port == reportedPort && reservation.IsLive)
                {
                    reservation.GameServerSteamId = gameServerSteamId;
                    reservation.State = DedicatedReservationState.Claimed;
                    _logger.LogInformation(
                        "Claimed dedicated server {SteamId} for lobby {LobbyId} on port {Port}",
                        gameServerSteamId, reservation.LobbyId, reportedPort);
                    return reservation.LobbyId.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            _logger.LogWarning(
                "Rejected unreserved dedicated GC server {SteamId} on port {Port}",
                gameServerSteamId, reportedPort);
            return "0";
        }
    }

    public bool HasReservationForPort(uint reportedPort)
    {
        if (reportedPort == 0)
        {
            return false;
        }

        lock (_sync)
        {
            SweepLocked(DateTime.UtcNow);
            return _reservations.Values.Any(reservation => reservation.IsLive && reservation.Port == reportedPort);
        }
    }

    public void ObserveRegistration(ulong gameServerSteamId, ApiGameServer server)
    {
        if (gameServerSteamId == 0 || server == null || server.Port <= 0 || !server.Dedicated)
        {
            return;
        }

        lock (_sync)
        {
            SweepLocked(DateTime.UtcNow);
            var reservation = _reservations.Values.FirstOrDefault(candidate =>
                candidate.State == DedicatedReservationState.Starting &&
                candidate.Port == server.Port && candidate.IsLive);
            if (reservation == null)
            {
                return;
            }

            reservation.GameServerSteamId = gameServerSteamId;
            reservation.State = DedicatedReservationState.Registered;
            _logger.LogInformation(
                "Dedicated server {SteamId} registered for lobby {LobbyId} on port {Port}",
                gameServerSteamId, reservation.LobbyId, reservation.Port);
        }
    }

    public string GetStatus(ulong lobbyId)
    {
        lock (_sync)
        {
            SweepLocked(DateTime.UtcNow);
            return _reservations.TryGetValue(lobbyId, out var reservation)
                ? reservation.State.ToString().ToLowerInvariant()
                : "not_found";
        }
    }

    public bool Release(ulong lobbyId, string reason)
    {
        lock (_sync)
        {
            if (!_reservations.Remove(lobbyId, out var reservation))
            {
                return false;
            }

            StopProcess(reservation, reason);
            return true;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                lock (_sync)
                {
                    SweepLocked(DateTime.UtcNow);
                }
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            foreach (var reservation in _reservations.Values)
            {
                StopProcess(reservation, "SKYNET server stopped");
            }
            _reservations.Clear();
        }

        return base.StopAsync(cancellationToken);
    }

    private void SweepLocked(DateTime now)
    {
        foreach (var reservation in _reservations.Values)
        {
            if (!reservation.IsLive && reservation.State is not DedicatedReservationState.Failed and not DedicatedReservationState.Stopped)
            {
                reservation.State = DedicatedReservationState.Failed;
                reservation.Error = "Dedicated Dota process exited before the match was ready.";
                _logger.LogWarning(
                    "Dedicated server for lobby {LobbyId} stopped before ready. Exit code: {ExitCode}",
                    reservation.LobbyId, TryGetExitCode(reservation.Process));
            }
            else if (reservation.State == DedicatedReservationState.Starting &&
                     now - reservation.StartedAt > _startupTimeout)
            {
                reservation.State = DedicatedReservationState.Failed;
                reservation.Error = "Timed out waiting for the dedicated server GC handshake.";
                StopProcess(reservation, reservation.Error);
                _logger.LogWarning("Dedicated server for lobby {LobbyId} timed out waiting for GC handshake", reservation.LobbyId);
            }
        }
    }

    private int AllocatePortLocked(int portStart)
    {
        for (var port = portStart; port <= Math.Min(65534, portStart + 256); port++)
        {
            if (_reservations.Values.Any(reservation => reservation.IsLive && reservation.Port == port))
            {
                continue;
            }

            if (CanBindPort(port))
            {
                return port;
            }
        }

        return 0;
    }

    private static bool CanBindPort(int port)
    {
        try
        {
            using var tcp = new TcpListener(IPAddress.Any, port);
            tcp.Start();
            using var udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udp.Bind(new IPEndPoint(IPAddress.Any, port));
            return true;
        }
        catch (SocketException)
        {
            return false;
        }
    }

    private void StopProcess(DedicatedReservation reservation, string reason)
    {
        if (reservation.State != DedicatedReservationState.Failed)
        {
            reservation.State = DedicatedReservationState.Stopped;
        }

        try
        {
            if (!reservation.Process.HasExited)
            {
                reservation.Process.Kill(entireProcessTree: true);
                _logger.LogInformation(
                    "Stopped dedicated Dota pid {ProcessId} for lobby {LobbyId}: {Reason}",
                    reservation.Process.Id, reservation.LobbyId, reason);
            }
        }
        catch (InvalidOperationException)
        {
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not stop dedicated Dota process for lobby {LobbyId}", reservation.LobbyId);
        }
    }

    private static string ResolvePath(string? configured, string contentRoot, string fallback)
    {
        var value = string.IsNullOrWhiteSpace(configured) ? fallback : configured.Trim();
        return Path.GetFullPath(Path.IsPathRooted(value) ? value : Path.Combine(contentRoot, value));
    }

    private static bool TryNormalizeIPv4(string value, out string normalized)
    {
        normalized = string.Empty;
        if (!IPAddress.TryParse(value, out var parsed) || parsed.AddressFamily != AddressFamily.InterNetwork ||
            IPAddress.IsLoopback(parsed))
        {
            return false;
        }

        normalized = parsed.ToString();
        return true;
    }

    private static string NormalizeMap(string? requestedMap)
    {
        var map = string.IsNullOrWhiteSpace(requestedMap) ? "dota" : requestedMap.Trim();
        return map.All(character => char.IsAsciiLetterOrDigit(character) || character is '_' or '-' or '/') &&
               !map.Contains("..", StringComparison.Ordinal)
            ? map
            : "dota";
    }

    private static int? TryGetExitCode(Process process)
    {
        try { return process.HasExited ? process.ExitCode : null; }
        catch (InvalidOperationException) { return null; }
    }

    public sealed record DedicatedLaunchResult(bool Started, int Port, DedicatedReservationState State, string Error)
    {
        public static DedicatedLaunchResult Failed(string error) => new(false, 0, DedicatedReservationState.Failed, error);
    }

    public enum DedicatedReservationState
    {
        Starting,
        Registered,
        Claimed,
        Failed,
        Stopped
    }

    private sealed class DedicatedReservation
    {
        public DedicatedReservation(ulong lobbyId, int port, string map, Process process, DateTime startedAt)
        {
            LobbyId = lobbyId;
            Port = port;
            Map = map;
            Process = process;
            StartedAt = startedAt;
        }

        public ulong LobbyId { get; }
        public int Port { get; }
        public string Map { get; }
        public Process Process { get; }
        public DateTime StartedAt { get; }
        public ulong GameServerSteamId { get; set; }
        public DedicatedReservationState State { get; set; } = DedicatedReservationState.Starting;
        public string Error { get; set; } = string.Empty;
        public bool IsLive
        {
            get
            {
                try { return !Process.HasExited; }
                catch (InvalidOperationException) { return false; }
            }
        }
    }
}

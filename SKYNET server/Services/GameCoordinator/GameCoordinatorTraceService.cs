namespace SKYNET_server.Services;

/// <summary>
/// In-memory ring buffer with recent GC activity (exchanges, pushes, script logs
/// and errors) consumed by the admin GC console page.
/// </summary>
public sealed class GameCoordinatorTraceService
{
    public sealed class TraceEntry
    {
        public long Seq { get; init; }
        public DateTime TimestampUtc { get; init; }
        public string Kind { get; init; } = string.Empty;
        public uint AppId { get; init; }
        public ulong SteamId { get; init; }
        public uint MessageType { get; init; }
        public int Size { get; init; }
        public string Detail { get; init; } = string.Empty;
        public string? PayloadBase64 { get; init; }
        public ulong? SourceJobId { get; init; }
        public ulong? TargetJobId { get; init; }
        public bool Protobuf { get; init; }
        public string? DecodedTypeName { get; init; }
        public string? DecodedJson { get; init; }
        public string? DecodeError { get; init; }
    }

    private const int MaxEntries = 1000;
    private const int MaxPayloadBytes = 16 * 1024;

    private readonly object _sync = new();
    private readonly Queue<TraceEntry> _entries = new();
    private long _nextSeq;

    public void Record(
        string kind,
        uint appId,
        ulong steamId,
        uint messageType,
        int size,
        string detail = "",
        string? payloadBase64 = null,
        ulong? sourceJobId = null,
        ulong? targetJobId = null,
        bool protobuf = false,
        string? decodedTypeName = null,
        string? decodedJson = null,
        string? decodeError = null)
    {
        var storedPayload = CapPayload(payloadBase64, out var truncationNote);
        if (!string.IsNullOrEmpty(truncationNote))
        {
            decodeError = string.IsNullOrEmpty(decodeError)
                ? truncationNote
                : $"{decodeError}; {truncationNote}";
        }

        lock (_sync)
        {
            _entries.Enqueue(new TraceEntry
            {
                Seq = ++_nextSeq,
                TimestampUtc = DateTime.UtcNow,
                Kind = kind,
                AppId = appId,
                SteamId = steamId,
                MessageType = messageType,
                Size = size,
                Detail = detail ?? string.Empty,
                PayloadBase64 = storedPayload,
                SourceJobId = sourceJobId,
                TargetJobId = targetJobId,
                Protobuf = protobuf,
                DecodedTypeName = decodedTypeName,
                DecodedJson = decodedJson,
                DecodeError = decodeError
            });

            while (_entries.Count > MaxEntries)
            {
                _entries.Dequeue();
            }
        }
    }

    public List<TraceEntry> GetSince(long sinceSeq)
    {
        lock (_sync)
        {
            return _entries.Where(entry => entry.Seq > sinceSeq).ToList();
        }
    }

    public bool TryGet(long seq, out TraceEntry entry)
    {
        lock (_sync)
        {
            entry = _entries.FirstOrDefault(candidate => candidate.Seq == seq)!;
            return entry != null;
        }
    }

    private static string? CapPayload(string? payloadBase64, out string? truncationNote)
    {
        truncationNote = null;
        if (string.IsNullOrEmpty(payloadBase64))
        {
            return payloadBase64;
        }

        try
        {
            var payload = Convert.FromBase64String(payloadBase64);
            if (payload.Length <= MaxPayloadBytes)
            {
                return payloadBase64;
            }

            truncationNote = $"payload truncated for inspector, {payload.Length} bytes total";
            return Convert.ToBase64String(payload, 0, MaxPayloadBytes);
        }
        catch (FormatException)
        {
            return payloadBase64.Length <= MaxPayloadBytes * 4 / 3
                ? payloadBase64
                : payloadBase64[..(MaxPayloadBytes * 4 / 3)];
        }
    }

    public static int EstimatePayloadSize(string? payloadBase64)
    {
        return string.IsNullOrEmpty(payloadBase64) ? 0 : payloadBase64.Length * 3 / 4;
    }
}

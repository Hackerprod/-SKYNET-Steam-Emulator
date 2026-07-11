namespace SKYNET_server.Services;

/// <summary>
/// In-memory ring buffer with recent GC activity (exchanges, pushes, Lua logs and
/// errors) consumed by the admin GC console page.
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
    }

    private const int MaxEntries = 1000;

    private readonly object _sync = new();
    private readonly Queue<TraceEntry> _entries = new();
    private long _nextSeq;

    public void Record(string kind, uint appId, ulong steamId, uint messageType, int size, string detail = "")
    {
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
                Detail = detail ?? string.Empty
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

    public static int EstimatePayloadSize(string? payloadBase64)
    {
        return string.IsNullOrEmpty(payloadBase64) ? 0 : payloadBase64.Length * 3 / 4;
    }
}

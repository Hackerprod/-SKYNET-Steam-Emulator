using Microsoft.Extensions.Logging;

namespace SKYNET_server.Services.Diagnostics;

/// <summary>
/// Captures the most recent application log lines in memory so read-only diagnostic
/// tools (e.g. the MCP server) can surface recent server activity without a log file
/// on disk. Mirrors the ring-buffer pattern already used by GameCoordinatorTraceService.
/// </summary>
public sealed class InMemoryLogBufferProvider : ILoggerProvider
{
    public sealed class LogEntry
    {
        public long Seq { get; init; }
        public DateTime TimestampUtc { get; init; }
        public LogLevel Level { get; init; }
        public string Category { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public string? Exception { get; init; }
    }

    private const int MaxEntries = 500;
    private readonly object _sync = new();
    private readonly Queue<LogEntry> _entries = new();
    private long _nextSeq;

    public void Record(string category, LogLevel level, string message, Exception? exception)
    {
        lock (_sync)
        {
            _entries.Enqueue(new LogEntry
            {
                Seq = ++_nextSeq,
                TimestampUtc = DateTime.UtcNow,
                Level = level,
                Category = category,
                Message = message,
                Exception = exception?.ToString()
            });

            while (_entries.Count > MaxEntries)
            {
                _entries.Dequeue();
            }
        }
    }

    public List<LogEntry> GetRecent(int count)
    {
        lock (_sync)
        {
            return _entries.Skip(Math.Max(0, _entries.Count - count)).ToList();
        }
    }

    public ILogger CreateLogger(string categoryName) => new BufferLogger(this, categoryName);

    public void Dispose() { }

    private sealed class BufferLogger : ILogger
    {
        private readonly InMemoryLogBufferProvider _owner;
        private readonly string _category;

        public BufferLogger(InMemoryLogBufferProvider owner, string category)
        {
            _owner = owner;
            _category = category;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            _owner.Record(_category, logLevel, formatter(state, exception), exception);
        }
    }
}

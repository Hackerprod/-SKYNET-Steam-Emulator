using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace SKYNET.Iface;

/// <summary>
/// Owns iface.jsonl. Detours only enqueue (bounded, never blocks the game); this thread numbers the records,
/// collapses consecutive identical calls into one with "rep", and flushes every 250 ms.
/// </summary>
internal sealed class TraceWriter
{
    private readonly FileStream _fs;
    private readonly object _lock = new object();
    private readonly BlockingCollection<Rec> _queue = new BlockingCollection<Rec>(8192);
    private readonly Thread _thread;
    private volatile bool _done;
    private Rec? _pending;
    private long _pendingAt, _seq, _lastDropReport;
    private int _dropUnreported;
    public int Dropped;
    public long Records;      // calls seen (collapsed repeats counted)

    public TraceWriter(string path)
    {
        _fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 64 * 1024);
        _thread = new Thread(Loop) { IsBackground = true, Name = "iface-writer" };
        _thread.Start();
    }

    public void Offer(Rec r)
    {
        if (_queue.TryAdd(r)) return;
        Interlocked.Increment(ref Dropped);
        Interlocked.Increment(ref _dropUnreported);
    }

    public void WriteLine(string line)
    {
        var b = Encoding.UTF8.GetBytes(line + "\n");
        lock (_lock) _fs.Write(b, 0, b.Length);
    }

    public void Flush() { lock (_lock) _fs.Flush(); }

    public void Close() { lock (_lock) _fs.Dispose(); }

    private void Add(Rec r)
    {
        Records += 1;
        if (_pending != null && _pending.Same(r)) { _pending.Rep++; _pending.QLast = r.Q; return; }
        EmitPending();
        r.Seq = ++_seq;
        _pending = r;
        _pendingAt = Stopwatch.GetTimestamp();
    }

    private void EmitPending()
    {
        if (_pending == null) return;
        WriteLine(_pending.ToJson());
        _pending = null;
    }

    private void Loop()
    {
        var lastFlush = Stopwatch.GetTimestamp();
        while (!(_done && _queue.Count == 0))
        {
            if (_queue.TryTake(out var r, 250)) Add(r);
            var now = Stopwatch.GetTimestamp();
            // A repeat run is closed after 1 s so the last record of a burst does not wait for the next call.
            if (_pending != null && now - _pendingAt > Stopwatch.Frequency) EmitPending();
            if (_dropUnreported > 0 && now - _lastDropReport > Stopwatch.Frequency)
            {
                WriteLine("{\"k\":\"drop\",\"n\":" + Interlocked.Exchange(ref _dropUnreported, 0) + "}");
                _lastDropReport = now;
            }
            if (now - lastFlush > Stopwatch.Frequency / 4) { Flush(); lastFlush = now; }
        }
        EmitPending();
        if (_dropUnreported > 0) WriteLine("{\"k\":\"drop\",\"n\":" + Interlocked.Exchange(ref _dropUnreported, 0) + "}");
        Flush();
    }

    /// <summary>Drains the queue and closes the repeat run; later Offer calls are ignored by the caller's stop flag.</summary>
    public void Drain()
    {
        _done = true;
        _thread.Join(5000);
    }
}

namespace SKYNET.Client.Services;

/// <summary>
/// Cross-process single-instance guard. The first process to run owns the
/// named mutex and listens on a named event; any later launch finds the
/// mutex already owned, signals the event instead of opening a second
/// window, and exits.
/// </summary>
public sealed class SingleInstanceGuard : IDisposable
{
    private const string MutexName = "SKYNET.SteamClient.SingleInstance";
    private const string ActivateEventName = "SKYNET.SteamClient.ActivateRequested";

    private readonly Mutex _mutex;
    private readonly EventWaitHandle _activateEvent;
    private Thread? _listenerThread;

    public SingleInstanceGuard()
    {
        _mutex = new Mutex(initiallyOwned: true, MutexName, out var createdNew);
        IsFirstInstance = createdNew;
        _activateEvent = new EventWaitHandle(false, EventResetMode.AutoReset, ActivateEventName);
    }

    public bool IsFirstInstance { get; }

    /// <summary>Called by a second launch to ask the running instance to show itself.</summary>
    public void SignalExistingInstance() => _activateEvent.Set();

    /// <summary>Owner-only: react whenever a later launch calls <see cref="SignalExistingInstance"/>.</summary>
    public void StartListening(Action onActivationRequested)
    {
        if (!IsFirstInstance) return;

        _listenerThread = new Thread(() =>
        {
            while (true)
            {
                _activateEvent.WaitOne();
                onActivationRequested();
            }
        })
        { IsBackground = true };
        _listenerThread.Start();
    }

    public void Dispose()
    {
        if (IsFirstInstance)
        {
            try { _mutex.ReleaseMutex(); } catch (ApplicationException) { /* not held */ }
        }
        _mutex.Dispose();
        _activateEvent.Dispose();
    }
}

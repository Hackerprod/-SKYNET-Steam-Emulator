using System.Windows.Forms;

namespace Overlay.Core;

public sealed class OverlayService
{
    public event EventHandler<OverlayActivatedEventArgs> OverlayActivated;

    public bool IsOverlayEnabled { get; private set; }
    public bool IsOverlayVisible { get; private set; }

    private OverlayOptions _options = new OverlayOptions();
    private Thread _uiThread;
    private OverlayWindow _window;
    private bool _shutdownRequested;
    private bool _suppressHiddenEvent;
    private readonly ManualResetEventSlim _ready = new ManualResetEventSlim(false);

    public void Initialize(OverlayOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));

        _uiThread = new Thread(UIEntryPoint)
        {
            IsBackground = true,
            Name = "Overlay.UI"
        };
        _uiThread.SetApartmentState(ApartmentState.STA);
        _uiThread.Start();

        if (!_ready.Wait(TimeSpan.FromSeconds(3)) || _window == null)
        {
            throw new InvalidOperationException("Overlay UI thread did not initialize.");
        }

        IsOverlayEnabled = true;
    }

    private void UIEntryPoint()
    {
        try
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }
        catch
        {
        }

        _window = new OverlayWindow(_options);
        _window.Hidden += OnWindowHidden;
        _ = _window.Handle;
        _ready.Set();
        Application.Run(_window);
    }

    public void Show(OverlayRequest request)
    {
        if (!IsOverlayEnabled) return;
        if (_window == null || _window.IsDisposed) return;

        _window.BeginInvoke((MethodInvoker)(() =>
        {
            bool wasVisible = IsOverlayVisible;
            _window.ShowOverlay(request);
            if (!wasVisible)
            {
                IsOverlayVisible = true;
                OverlayActivated?.Invoke(this, new OverlayActivatedEventArgs(true, true));
            }
        }));
    }

    public void Hide()
    {
        if (!IsOverlayVisible) return;
        if (_window == null || _window.IsDisposed) return;

        _suppressHiddenEvent = true;
        _window.BeginInvoke((MethodInvoker)(() =>
        {
            _window.HideOverlay();
            _suppressHiddenEvent = false;
            IsOverlayVisible = false;
            OverlayActivated?.Invoke(this, new OverlayActivatedEventArgs(false, true));
        }));
    }

    // Called when the window hides itself (Esc, close button, dim click)
    private void OnWindowHidden()
    {
        if (_suppressHiddenEvent) return;

        IsOverlayVisible = false;
        OverlayActivated?.Invoke(this, new OverlayActivatedEventArgs(false, true));
    }

    public void Shutdown()
    {
        if (_shutdownRequested) return;
        _shutdownRequested = true;

        if (_window != null && !_window.IsDisposed)
        {
            _window.BeginInvoke((MethodInvoker)(() =>
            {
                _window.Close();
                _window = null;
            }));
        }

        if (_uiThread != null && _uiThread.IsAlive)
        {
            _uiThread.Join(TimeSpan.FromSeconds(3));
        }

        IsOverlayEnabled = false;
        IsOverlayVisible = false;
    }
}

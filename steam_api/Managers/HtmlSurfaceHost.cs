using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SKYNET.Callback;

namespace SKYNET.Managers
{
    internal sealed class HtmlSurfaceHost : IDisposable
    {
        private const int DefaultWidth = 640;
        private const int DefaultHeight = 480;
        private const int MaxDimension = 8192;
        private const int PaintIntervalMilliseconds = 66;
        private const int WMKeyDown = 0x0100;
        private const int WMKeyUp = 0x0101;
        private const int WMChar = 0x0102;
        private const int WMMouseMove = 0x0200;
        private const int WMLButtonDown = 0x0201;
        private const int WMLButtonUp = 0x0202;
        private const int WMLButtonDoubleClick = 0x0203;
        private const int WRButtonDown = 0x0204;
        private const int WRButtonUp = 0x0205;
        private const int WRButtonDoubleClick = 0x0206;
        private const int WMMButtonDown = 0x0207;
        private const int WMMButtonUp = 0x0208;
        private const int WMMButtonDoubleClick = 0x0209;
        private const int WMMouseWheel = 0x020A;
        private const int MKLButton = 0x0001;
        private const int MKRButton = 0x0002;
        private const int MKMButton = 0x0010;
        private const int DvAspectContent = 1;

        private readonly ConcurrentQueue<Action> startupQueue = new ConcurrentQueue<Action>();
        private readonly ConcurrentDictionary<uint, PendingJsDialog> pendingJsDialogs =
            new ConcurrentDictionary<uint, PendingJsDialog>();
        private readonly ConcurrentDictionary<uint, PendingFileDialog> pendingFileDialogs =
            new ConcurrentDictionary<uint, PendingFileDialog>();
        private readonly ManualResetEventSlim ready = new ManualResetEventSlim(false);
        private readonly Dictionary<uint, BrowserSession> browsers = new Dictionary<uint, BrowserSession>();
        private readonly object lifecycleGate = new object();

        private Thread thread;
        private HostForm form;
        private System.Windows.Forms.Timer paintTimer;
        private int nextBrowserHandle;
        private bool disposed;
        private Exception startupFailure;

        public bool Start()
        {
            lock (lifecycleGate)
            {
                if (disposed)
                {
                    return false;
                }
                if (thread != null)
                {
                    return startupFailure == null;
                }

                thread = new Thread(ThreadMain)
                {
                    IsBackground = true,
                    Name = "SKYNET HTML Surface"
                };
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }

            ready.Wait(TimeSpan.FromSeconds(2));
            return ready.IsSet && startupFailure == null;
        }

        public ulong CreateBrowser(string userAgent, string userCss)
        {
            var call = CallbackManager.AddCallbackResult(new HTML_BrowserReady_t(), false);
            if (!Start())
            {
                CallbackManager.CompleteCallbackResult(call, new HTML_BrowserReady_t(), true);
                return call;
            }

            Post(() =>
            {
                try
                {
                    var handle = unchecked((uint)Interlocked.Increment(ref nextBrowserHandle));
                    if (handle == 0)
                    {
                        handle = unchecked((uint)Interlocked.Increment(ref nextBrowserHandle));
                    }

                    var browser = new SurfaceWebBrowser(
                        (message, confirm) => ShowJavaScriptDialog(handle, message, confirm))
                    {
                        ScriptErrorsSuppressed = true,
                        ScrollBarsEnabled = true,
                        IsWebBrowserContextMenuEnabled = false,
                        WebBrowserShortcutsEnabled = false,
                        AllowWebBrowserDrop = false,
                        Size = new Size(DefaultWidth, DefaultHeight),
                        Location = Point.Empty
                    };
                    browser.CreateControl();

                    var session = new BrowserSession(handle, browser, userAgent, userCss);
                    browsers[handle] = session;
                    form.Controls.Add(browser);
                    browser.BringToFront();
                    HookBrowserEvents(session);

                    CallbackManager.CompleteCallbackResult(call, new HTML_BrowserReady_t
                    {
                        UnBrowserHandle = handle
                    });
                }
                catch (Exception ex)
                {
                    SteamEmulator.Write("HTML browser creation", ex);
                    CallbackManager.CompleteCallbackResult(call, new HTML_BrowserReady_t(), true);
                }
            });
            return call;
        }

        public void RemoveBrowser(uint handle)
        {
            CancelPendingDialog(handle);
            Post(() =>
            {
                if (!browsers.TryGetValue(handle, out var session))
                {
                    return;
                }

                browsers.Remove(handle);
                form.Controls.Remove(session.Browser);
                session.Browser.Dispose();
                session.ReleasePixelBufferDeferred();
                QueueCallback(new HTML_CloseBrowser_t { UnBrowserHandle = handle });
            });
        }

        public void JavaScriptDialogResponse(uint handle, bool result)
        {
            if (pendingJsDialogs.TryGetValue(handle, out var dialog))
            {
                dialog.TrySetResult(result);
            }
        }

        public void FileDialogResponse(uint handle, IReadOnlyList<string> selectedFiles)
        {
            var normalized = (selectedFiles ?? Array.Empty<string>())
                .Where(path => !string.IsNullOrWhiteSpace(path))
                .Select(path =>
                {
                    try
                    {
                        return Path.GetFullPath(path);
                    }
                    catch
                    {
                        return string.Empty;
                    }
                })
                .Where(path => !string.IsNullOrEmpty(path) && File.Exists(path))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(256)
                .ToArray();
            if (pendingFileDialogs.TryGetValue(handle, out var dialog))
            {
                dialog.TrySetResult(normalized);
            }
        }

        public void LoadUrl(uint handle, string url, string postData)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return;
            }

            Post(() =>
            {
                if (!browsers.TryGetValue(handle, out var session))
                {
                    return;
                }
                RequestNavigation(session, uri.AbsoluteUri, postData, false);
            });
        }

        public void AllowStartRequest(uint handle, bool allowed)
        {
            Post(() =>
            {
                if (!browsers.TryGetValue(handle, out var session) || session.PendingNavigation == null)
                {
                    return;
                }

                var navigation = session.PendingNavigation;
                session.PendingNavigation = null;
                if (!allowed)
                {
                    return;
                }

                session.AllowNextNavigation = true;
                var headers = string.Join("\r\n", session.Headers.Select(pair => $"{pair.Key}: {pair.Value}"));
                var post = string.IsNullOrEmpty(navigation.PostData)
                    ? null
                    : Encoding.UTF8.GetBytes(navigation.PostData);
                session.Browser.Navigate(navigation.Url, null, post, headers);
            });
        }

        public void AddHeader(uint handle, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            Post(() =>
            {
                if (browsers.TryGetValue(handle, out var session))
                {
                    session.Headers[key.Trim()] = value ?? string.Empty;
                }
            });
        }

        public void SetSize(uint handle, uint width, uint height)
        {
            var safeWidth = (int)Math.Max(1, Math.Min(MaxDimension, width));
            var safeHeight = (int)Math.Max(1, Math.Min(MaxDimension, height));
            Post(() =>
            {
                if (!browsers.TryGetValue(handle, out var session))
                {
                    return;
                }
                session.Browser.Size = new Size(safeWidth, safeHeight);
                session.Dirty = true;
            });
        }

        public void SetDpiScaling(uint handle, float scale)
        {
            Post(() =>
            {
                if (browsers.TryGetValue(handle, out var session))
                {
                    session.DpiScale = ClampScale(scale);
                    ApplyZoom(session);
                }
            });
        }

        public void SetPageScale(uint handle, float scale)
        {
            Post(() =>
            {
                if (browsers.TryGetValue(handle, out var session))
                {
                    session.PageScale = ClampScale(scale);
                    ApplyZoom(session);
                }
            });
        }

        public void SetBackgroundMode(uint handle, bool background)
        {
            Post(() =>
            {
                if (browsers.TryGetValue(handle, out var session))
                {
                    session.Background = background;
                }
            });
        }

        public void ExecuteJavaScript(uint handle, string script)
        {
            Post(() =>
            {
                if (!browsers.TryGetValue(handle, out var session) || session.Browser.Document == null)
                {
                    return;
                }
                ExecuteScript(session, script);
                session.Dirty = true;
            });
        }

        public void Reload(uint handle)
        {
            WithBrowser(handle, session => session.Browser.Refresh(WebBrowserRefreshOption.Completely));
        }

        public void StopLoad(uint handle)
        {
            WithBrowser(handle, session => session.Browser.Stop());
        }

        public void GoBack(uint handle)
        {
            WithBrowser(handle, session =>
            {
                if (session.Browser.CanGoBack)
                {
                    session.Browser.GoBack();
                }
            });
        }

        public void GoForward(uint handle)
        {
            WithBrowser(handle, session =>
            {
                if (session.Browser.CanGoForward)
                {
                    session.Browser.GoForward();
                }
            });
        }

        public void SetHorizontalScroll(uint handle, uint position)
        {
            WithBrowser(handle, session =>
            {
                var window = session.Browser.Document?.Window;
                if (window != null)
                {
                    window.ScrollTo((int)Math.Min(int.MaxValue, position), window.Position.Y);
                    session.Dirty = true;
                }
            });
        }

        public void SetVerticalScroll(uint handle, uint position)
        {
            WithBrowser(handle, session =>
            {
                var window = session.Browser.Document?.Window;
                if (window != null)
                {
                    window.ScrollTo(window.Position.X, (int)Math.Min(int.MaxValue, position));
                    session.Dirty = true;
                }
            });
        }

        public void SetKeyFocus(uint handle, bool focused)
        {
            WithBrowser(handle, session =>
            {
                if (focused)
                {
                    session.Browser.Focus();
                }
                else
                {
                    form.Focus();
                }
            });
        }

        public void SendKey(uint handle, int message, uint keyCode, bool systemKey)
        {
            WithBrowser(handle, session => SendMessage(session.Browser.Handle, message, new IntPtr(keyCode), IntPtr.Zero));
        }

        public void SendMouseMove(uint handle, int x, int y)
        {
            WithBrowser(handle, session =>
            {
                session.MouseX = x;
                session.MouseY = y;
                SendMessage(session.Browser.Handle, WMMouseMove, IntPtr.Zero, MakeLParam(x, y));
            });
        }

        public void SendMouseButton(uint handle, int button, bool down, bool doubleClick)
        {
            WithBrowser(handle, session =>
            {
                var message = MouseMessage(button, down, doubleClick);
                var state = button == 0 ? MKLButton : button == 1 ? MKRButton : MKMButton;
                SendMessage(session.Browser.Handle, message, down ? new IntPtr(state) : IntPtr.Zero, MakeLParam(session.MouseX, session.MouseY));
            });
        }

        public void SendMouseWheel(uint handle, int delta)
        {
            WithBrowser(handle, session =>
            {
                var wheel = new IntPtr(unchecked(delta << 16));
                SendMessage(session.Browser.Handle, WMMouseWheel, wheel, MakeLParam(session.MouseX, session.MouseY));
            });
        }

        public void CopyToClipboard(uint handle)
        {
            WithBrowser(handle, session => session.Browser.Document?.ExecCommand("Copy", false, null));
        }

        public void PasteFromClipboard(uint handle)
        {
            WithBrowser(handle, session => session.Browser.Document?.ExecCommand("Paste", false, null));
        }

        public void Find(uint handle, string search, bool reverse)
        {
            WithBrowser(handle, session =>
            {
                if (session.Browser.Document?.Body == null || string.IsNullOrEmpty(search))
                {
                    QueueCallback(new HTML_SearchResults_t
                    {
                        UnBrowserHandle = handle,
                        UnResults = 0,
                        UnCurrentMatch = 0
                    });
                    return;
                }

                var escaped = JavaScriptString(search);
                ExecuteScript(session,
                    $"window.find('{escaped}',false,{(reverse ? "true" : "false")},true,false,true,false);");
                QueueCallback(new HTML_SearchResults_t
                {
                    UnBrowserHandle = handle,
                    UnResults = 1,
                    UnCurrentMatch = 1
                });
            });
        }

        public void GetLinkAtPosition(uint handle, int x, int y)
        {
            WithBrowser(handle, session =>
            {
                var element = session.Browser.Document?.GetElementFromPoint(new Point(x, y));
                var href = element?.GetAttribute("href") ?? string.Empty;
                QueueCallback(new HTML_LinkAtPosition_t
                {
                    UnBrowserHandle = handle,
                    X = unchecked((uint)Math.Max(0, x)),
                    Y = unchecked((uint)Math.Max(0, y)),
                    PchURL = href
                });
            });
        }

        public void Dispose()
        {
            HostForm currentForm;
            Thread currentThread;
            lock (lifecycleGate)
            {
                if (disposed)
                {
                    return;
                }
                disposed = true;
                currentForm = form;
                currentThread = thread;
            }

            foreach (var handle in pendingJsDialogs.Keys.Concat(pendingFileDialogs.Keys).Distinct())
            {
                CancelPendingDialog(handle);
            }

            if (currentForm != null && !currentForm.IsDisposed)
            {
                try
                {
                    currentForm.BeginInvoke(new Action(currentForm.Close));
                }
                catch (InvalidOperationException)
                {
                }
            }

            if (currentThread != null && currentThread.IsAlive && currentThread != Thread.CurrentThread)
            {
                currentThread.Join(TimeSpan.FromSeconds(2));
            }
            ready.Dispose();
        }

        private bool ShowJavaScriptDialog(uint handle, string message, bool confirm)
        {
            var dialog = new PendingJsDialog();
            if (!pendingJsDialogs.TryAdd(handle, dialog))
            {
                return false;
            }

            try
            {
                QueueCallback(confirm
                    ? (ICallbackData)new HTML_JSConfirm_t
                    {
                        UnBrowserHandle = handle,
                        PchMessage = message ?? string.Empty
                    }
                    : new HTML_JSAlert_t
                    {
                        UnBrowserHandle = handle,
                        PchMessage = message ?? string.Empty
                    });
                return dialog.Wait(TimeSpan.FromMinutes(5));
            }
            finally
            {
                if (pendingJsDialogs.TryGetValue(handle, out var current) &&
                    ReferenceEquals(current, dialog))
                {
                    pendingJsDialogs.TryRemove(handle, out _);
                }
                dialog.Dispose();
            }
        }

        private string[] ShowFileDialog(uint handle, string title, string initialFile)
        {
            var dialog = new PendingFileDialog();
            if (!pendingFileDialogs.TryAdd(handle, dialog))
            {
                return Array.Empty<string>();
            }

            try
            {
                QueueCallback(new HTML_FileOpenDialog_t
                {
                    UnBrowserHandle = handle,
                    PchTitle = title ?? string.Empty,
                    PchInitialFile = initialFile ?? string.Empty
                });
                return dialog.Wait(TimeSpan.FromMinutes(5));
            }
            finally
            {
                if (pendingFileDialogs.TryGetValue(handle, out var current) &&
                    ReferenceEquals(current, dialog))
                {
                    pendingFileDialogs.TryRemove(handle, out _);
                }
                dialog.Dispose();
            }
        }

        private void CancelPendingDialog(uint handle)
        {
            if (pendingJsDialogs.TryRemove(handle, out var dialog))
            {
                dialog.TrySetResult(false);
            }
            if (pendingFileDialogs.TryRemove(handle, out var fileDialog))
            {
                fileDialog.TrySetResult(Array.Empty<string>());
            }
        }

        private void ThreadMain()
        {
            try
            {
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += OnThreadException;
                form = new HostForm();
                paintTimer = new System.Windows.Forms.Timer { Interval = PaintIntervalMilliseconds };
                paintTimer.Tick += (_, __) => PaintBrowsers();
                paintTimer.Start();
                ready.Set();

                while (startupQueue.TryDequeue(out var work))
                {
                    form.BeginInvoke(work);
                }
                Application.Run(form);
            }
            catch (Exception ex)
            {
                startupFailure = ex;
                SteamEmulator.Write("HTML surface host", ex);
                ready.Set();
            }
            finally
            {
                foreach (var browser in browsers.Values.ToArray())
                {
                    browser.Dispose();
                }
                browsers.Clear();
                paintTimer?.Dispose();
                Application.ThreadException -= OnThreadException;
            }
        }

        private void Post(Action action)
        {
            if (action == null || disposed)
            {
                return;
            }

            var currentForm = form;
            if (currentForm == null || !currentForm.IsHandleCreated)
            {
                startupQueue.Enqueue(action);
                return;
            }

            try
            {
                currentForm.BeginInvoke(action);
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void WithBrowser(uint handle, Action<BrowserSession> action)
        {
            Post(() =>
            {
                if (browsers.TryGetValue(handle, out var session))
                {
                    action(session);
                }
            });
        }

        private void HookBrowserEvents(BrowserSession session)
        {
            session.Browser.Navigating += (_, args) =>
            {
                if (session.AllowNextNavigation)
                {
                    session.AllowNextNavigation = false;
                    return;
                }

                args.Cancel = true;
                RequestNavigation(session, args.Url?.AbsoluteUri, null, true);
            };
            session.Browser.DocumentCompleted += (_, args) =>
            {
                session.Dirty = true;
                ApplyCustomCss(session);
                ApplyZoom(session);
                HookFileInputs(session);
                var url = args.Url?.AbsoluteUri ?? session.Browser.Url?.AbsoluteUri ?? string.Empty;
                var title = session.Browser.DocumentTitle ?? string.Empty;
                QueueCallback(new HTML_URLChanged_t
                {
                    UnBrowserHandle = session.Handle,
                    PchURL = url,
                    PchPostData = string.Empty,
                    BIsRedirect = false,
                    PchPageTitle = title,
                    BNewNavigation = true
                });
                QueueCallback(new HTML_FinishedRequest_t
                {
                    UnBrowserHandle = session.Handle,
                    PchURL = url,
                    PchPageTitle = title
                });
                QueueCallback(new HTML_ChangedTitle_t
                {
                    UnBrowserHandle = session.Handle,
                    PchTitle = title
                });
                QueueNavigationState(session);
                PaintBrowser(session);
            };
            session.Browser.DocumentTitleChanged += (_, __) =>
            {
                QueueCallback(new HTML_ChangedTitle_t
                {
                    UnBrowserHandle = session.Handle,
                    PchTitle = session.Browser.DocumentTitle ?? string.Empty
                });
            };
            session.Browser.NewWindow += (_, args) =>
            {
                args.Cancel = true;
                var url = session.Browser.StatusText ?? string.Empty;
                QueueCallback(new HTML_OpenLinkInNewTab_t
                {
                    UnBrowserHandle = session.Handle,
                    PchURL = url
                });
            };
        }

        private void HookFileInputs(BrowserSession session)
        {
            HtmlDocument document = session.Browser.Document;
            if (document == null)
            {
                return;
            }

            foreach (HtmlElement element in document.GetElementsByTagName("input"))
            {
                if (!string.Equals(element.GetAttribute("type"), "file", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(element.GetAttribute("data-skynet-file-hooked"), "1", StringComparison.Ordinal))
                {
                    continue;
                }

                element.SetAttribute("data-skynet-file-hooked", "1");
                element.Click += (_, args) =>
                {
                    args.ReturnValue = false;
                    string title = element.GetAttribute("title");
                    if (string.IsNullOrWhiteSpace(title))
                    {
                        title = session.Browser.DocumentTitle ?? string.Empty;
                    }

                    string[] selectedFiles = ShowFileDialog(
                        session.Handle,
                        title,
                        element.GetAttribute("value"));
                    ApplySelectedFiles(element, selectedFiles);
                };
            }
        }

        private static void ApplySelectedFiles(HtmlElement element, IReadOnlyList<string> selectedFiles)
        {
            if (element == null || selectedFiles == null || selectedFiles.Count == 0)
            {
                return;
            }

            try
            {
                // MSHTML exposes a single legacy value property even when the
                // document requested multiple files.
                element.SetAttribute("value", selectedFiles[0]);
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("HTML file selection", ex);
            }
        }

        private static void RequestNavigation(BrowserSession session, string url, string postData, bool redirect)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }
            session.PendingNavigation = new PendingNavigation(url, postData);
            QueueCallback(new HTML_StartRequest_t
            {
                UnBrowserHandle = session.Handle,
                PchURL = url,
                PchTarget = string.Empty,
                PchPostData = postData ?? string.Empty,
                BIsRedirect = redirect
            });
        }

        private void PaintBrowsers()
        {
            foreach (var session in browsers.Values.ToArray())
            {
                if (!session.Background && session.Browser.Document != null)
                {
                    PaintBrowser(session);
                }
            }
        }

        private static void PaintBrowser(BrowserSession session)
        {
            var width = session.Browser.ClientSize.Width;
            var height = session.Browser.ClientSize.Height;
            if (width <= 0 || height <= 0)
            {
                return;
            }

            try
            {
                using (var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.Clear(Color.White);
                        var hdc = graphics.GetHdc();
                        try
                        {
                            var rect = new NativeRect(0, 0, width, height);
                            var unknown = Marshal.GetIUnknownForObject(session.Browser.ActiveX);
                            try
                            {
                                var result = OleDraw(unknown, DvAspectContent, hdc, ref rect);
                                if (result != 0)
                                {
                                    session.Browser.DrawToBitmap(bitmap, new Rectangle(0, 0, width, height));
                                }
                            }
                            finally
                            {
                                Marshal.Release(unknown);
                            }
                        }
                        finally
                        {
                            graphics.ReleaseHdc(hdc);
                        }
                    }

                    var data = bitmap.LockBits(
                        new Rectangle(0, 0, width, height),
                        ImageLockMode.ReadOnly,
                        PixelFormat.Format32bppArgb);
                    try
                    {
                        var rowBytes = checked(width * 4);
                        var totalBytes = checked(rowBytes * height);
                        session.EnsurePixelBuffer(totalBytes);
                        for (var row = 0; row < height; row++)
                        {
                            var sourceRow = data.Stride >= 0 ? row : height - row - 1;
                            var source = IntPtr.Add(data.Scan0, sourceRow * Math.Abs(data.Stride));
                            CopyMemory(IntPtr.Add(session.PixelBuffer, row * rowBytes), source, (UIntPtr)(uint)rowBytes);
                        }

                        session.PageSerial++;
                        QueueCallback(new HTML_NeedsPaint_t
                        {
                            UnBrowserHandle = session.Handle,
                            PBGRA = session.PixelBuffer,
                            UnWide = (uint)width,
                            UnTall = (uint)height,
                            UnUpdateX = 0,
                            UnUpdateY = 0,
                            UnUpdateWide = (uint)width,
                            UnUpdateTall = (uint)height,
                            UnScrollX = 0,
                            UnScrollY = 0,
                            FlPageScale = session.DpiScale * session.PageScale,
                            UnPageSerial = session.PageSerial
                        });
                    }
                    finally
                    {
                        bitmap.UnlockBits(data);
                    }
                }
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("HTML surface paint", ex);
            }
        }

        private static void ApplyCustomCss(BrowserSession session)
        {
            var document = session.Browser.Document;
            var head = document?.GetElementsByTagName("head")
                .Cast<HtmlElement>()
                .FirstOrDefault();
            if (string.IsNullOrWhiteSpace(session.UserCss) || head == null)
            {
                return;
            }
            var style = document.CreateElement("style");
            style.SetAttribute("type", "text/css");
            style.InnerText = session.UserCss;
            head.AppendChild(style);
        }

        private static void ApplyZoom(BrowserSession session)
        {
            if (session.Browser.Document?.Body == null)
            {
                return;
            }
            session.Browser.Document.Body.Style =
                (session.Browser.Document.Body.Style ?? string.Empty) +
                $";zoom:{session.DpiScale * session.PageScale:0.###};";
            session.Dirty = true;
        }

        private static void ExecuteScript(BrowserSession session, string script)
        {
            if (string.IsNullOrWhiteSpace(script) || session.Browser.Document == null)
            {
                return;
            }

            try
            {
                session.Browser.Document.InvokeScript("eval", new object[] { script });
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("HTML JavaScript", ex);
            }
        }

        private static void QueueNavigationState(BrowserSession session)
        {
            QueueCallback(new HTML_CanGoBackAndForward_t
            {
                UnBrowserHandle = session.Handle,
                BCanGoBack = session.Browser.CanGoBack,
                BCanGoForward = session.Browser.CanGoForward
            });
        }

        private static void QueueCallback(ICallbackData callback)
        {
            NativeCallbackQueue.Enqueue(() => CallbackManager.AddCallback(callback));
        }

        private static float ClampScale(float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                return 1f;
            }
            return Math.Max(0.25f, Math.Min(4f, value));
        }

        private static string JavaScriptString(string value)
        {
            return (value ?? string.Empty)
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n");
        }

        private static int MouseMessage(int button, bool down, bool doubleClick)
        {
            switch (button)
            {
                case 1:
                    return doubleClick ? WRButtonDoubleClick : down ? WRButtonDown : WRButtonUp;
                case 2:
                    return doubleClick ? WMMButtonDoubleClick : down ? WMMButtonDown : WMMButtonUp;
                default:
                    return doubleClick ? WMLButtonDoubleClick : down ? WMLButtonDown : WMLButtonUp;
            }
        }

        private static IntPtr MakeLParam(int low, int high)
        {
            return new IntPtr((high << 16) | (low & 0xFFFF));
        }

        private static void OnThreadException(object sender, ThreadExceptionEventArgs args)
        {
            SteamEmulator.Write("HTML surface UI", args.Exception);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("ole32.dll")]
        private static extern int OleDraw(IntPtr pUnk, int dwAspect, IntPtr hdcDraw, ref NativeRect lprcBounds);

        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr destination, IntPtr source, UIntPtr length);

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeRect
        {
            public NativeRect(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private sealed class HostForm : Form
        {
            public HostForm()
            {
                FormBorderStyle = FormBorderStyle.None;
                ShowInTaskbar = false;
                StartPosition = FormStartPosition.Manual;
                Location = new Point(-32000, -32000);
                Size = new Size(DefaultWidth, DefaultHeight);
                Opacity = 0;
            }

            protected override bool ShowWithoutActivation => true;
        }

        private sealed class SurfaceWebBrowser : WebBrowser
        {
            private readonly Func<string, bool, bool> showJavaScriptDialog;

            public SurfaceWebBrowser(Func<string, bool, bool> showJavaScriptDialog)
            {
                this.showJavaScriptDialog = showJavaScriptDialog;
            }

            public object ActiveX => ActiveXInstance;

            protected override WebBrowserSiteBase CreateWebBrowserSiteBase()
            {
                return new SurfaceWebBrowserSite(this, showJavaScriptDialog);
            }

            private sealed class SurfaceWebBrowserSite : WebBrowserSite, IDocHostShowUI
            {
            private const uint MessageBoxTypeMask = 0x0f;
            private const int IdOk = 1;
            private const int IdCancel = 2;
            private const int IdYes = 6;
            private const int IdNo = 7;
            private readonly Func<string, bool, bool> showJavaScriptDialog;

                public SurfaceWebBrowserSite(
                    WebBrowser host,
                    Func<string, bool, bool> showJavaScriptDialog)
                    : base(host)
                {
                    this.showJavaScriptDialog = showJavaScriptDialog;
                }

                public int ShowMessage(
                IntPtr hwnd,
                string text,
                string caption,
                uint type,
                string helpFile,
                uint helpContext,
                out int result)
            {
                var kind = type & MessageBoxTypeMask;
                var confirm = kind != 0;
                var accepted = showJavaScriptDialog?.Invoke(text ?? string.Empty, confirm) ?? false;
                if (!confirm)
                {
                    result = IdOk;
                }
                else if (kind == 4)
                {
                    result = accepted ? IdYes : IdNo;
                }
                else
                {
                    result = accepted ? IdOk : IdCancel;
                }
                return 0;
            }

                public int ShowHelp(
                IntPtr hwnd,
                string helpFile,
                uint command,
                uint data,
                NativePoint mousePosition,
                object dispatchObjectHit)
                {
                    return 1;
                }
            }
        }

        private sealed class BrowserSession : IDisposable
        {
            public BrowserSession(uint handle, SurfaceWebBrowser browser, string userAgent, string userCss)
            {
                Handle = handle;
                Browser = browser;
                UserAgent = userAgent ?? string.Empty;
                UserCss = userCss ?? string.Empty;
            }

            public readonly uint Handle;
            public readonly SurfaceWebBrowser Browser;
            public readonly string UserAgent;
            public readonly string UserCss;
            public readonly Dictionary<string, string> Headers =
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            public PendingNavigation PendingNavigation;
            public bool AllowNextNavigation;
            public bool Background;
            public bool Dirty;
            public float DpiScale = 1f;
            public float PageScale = 1f;
            public int MouseX;
            public int MouseY;
            public IntPtr PixelBuffer;
            public int PixelCapacity;
            public uint PageSerial;

            public void EnsurePixelBuffer(int size)
            {
                if (size <= PixelCapacity && PixelBuffer != IntPtr.Zero)
                {
                    return;
                }
                if (PixelBuffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(PixelBuffer);
                }
                PixelBuffer = Marshal.AllocHGlobal(size);
                PixelCapacity = size;
            }

            public void ReleasePixelBufferDeferred()
            {
                var buffer = PixelBuffer;
                PixelBuffer = IntPtr.Zero;
                PixelCapacity = 0;
                if (buffer == IntPtr.Zero)
                {
                    return;
                }
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    Thread.Sleep(5000);
                    Marshal.FreeHGlobal(buffer);
                });
            }

            public void Dispose()
            {
                Browser?.Dispose();
                if (PixelBuffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(PixelBuffer);
                    PixelBuffer = IntPtr.Zero;
                }
            }
        }

        private sealed class PendingNavigation
        {
            public PendingNavigation(string url, string postData)
            {
                Url = url;
                PostData = postData;
            }
            public string Url { get; }
            public string PostData { get; }
        }

        private sealed class PendingJsDialog : IDisposable
        {
            private readonly ManualResetEventSlim completed = new ManualResetEventSlim(false);
            private int result;

            public bool Wait(TimeSpan timeout)
            {
                return completed.Wait(timeout) && Volatile.Read(ref result) == 1;
            }

            public void TrySetResult(bool accepted)
            {
                Interlocked.Exchange(ref result, accepted ? 1 : -1);
                completed.Set();
            }

            public void Dispose()
            {
                completed.Dispose();
            }
        }

        private sealed class PendingFileDialog : IDisposable
        {
            private readonly ManualResetEventSlim completed = new ManualResetEventSlim(false);
            private string[] selectedFiles = Array.Empty<string>();

            public string[] Wait(TimeSpan timeout)
            {
                return completed.Wait(timeout)
                    ? Volatile.Read(ref selectedFiles)
                    : Array.Empty<string>();
            }

            public void TrySetResult(string[] files)
            {
                Interlocked.Exchange(ref selectedFiles, files ?? Array.Empty<string>());
                completed.Set();
            }

            public void Dispose()
            {
                completed.Dispose();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NativePoint
        {
            public int X;
            public int Y;
        }

        [ComImport]
        [Guid("C4D244B0-D43E-11CF-893B-00AA00BDCE1A")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IDocHostShowUI
        {
            [PreserveSig]
            int ShowMessage(
                IntPtr hwnd,
                [MarshalAs(UnmanagedType.LPWStr)] string text,
                [MarshalAs(UnmanagedType.LPWStr)] string caption,
                uint type,
                [MarshalAs(UnmanagedType.LPWStr)] string helpFile,
                uint helpContext,
                out int result);

            [PreserveSig]
            int ShowHelp(
                IntPtr hwnd,
                [MarshalAs(UnmanagedType.LPWStr)] string helpFile,
                uint command,
                uint data,
                NativePoint mousePosition,
                [MarshalAs(UnmanagedType.IDispatch)] object dispatchObjectHit);
        }
    }
}

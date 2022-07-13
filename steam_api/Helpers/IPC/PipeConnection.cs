using System;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SKYNET.IPC
{
    public sealed class PipeConnection<T>
    {
        public string PipeName { get; }
        public string ServerName { get; }
        public bool IsConnected => PipeStreamWrapper.IsConnected;
        public bool IsStarted => ReadWorker != null;
        public PipeStream PipeStream { get; }
        private PipeStreamWrapper PipeStreamWrapper { get; }
        private TaskWorker? ReadWorker { get; set; }


        #region Events

        public event EventHandler<ConnectionEventArgs<T>>? Disconnected;

        public event EventHandler<ConnectionMessageEventArgs<T>>? MessageReceived;

        public event EventHandler<ConnectionEventArgs<T>>? ExceptionOccurred;

        private void OnDisconnected()
        {
            Disconnected?.Invoke(this, new ConnectionEventArgs<T>(this));
        }

        private void OnMessageReceived(T message)
        {
            MessageReceived?.Invoke(this, new ConnectionMessageEventArgs<T>(this, message));
        }

        private void OnExceptionOccurred(Exception exception)
        {
            ExceptionOccurred?.Invoke(this, new ConnectionExceptionEventArgs<T>(this, exception));
        }

        #endregion

        #region Constructors

        internal PipeConnection(PipeStream stream, string pipeName, string serverName = "")
        {
            PipeName = pipeName;
            PipeStream = stream;
            PipeStreamWrapper = new PipeStreamWrapper(stream);
            ServerName = serverName;
        }

        #endregion

        #region Public methods

        public void Start()
        {
            if (IsStarted)
            {
                throw new InvalidOperationException("Connection already started");
            }

            ReadWorker = new TaskWorker(async cancellationToken =>
            {
                while (!cancellationToken.IsCancellationRequested && IsConnected)
                {
                    try
                    {
                        var bytes = await PipeStreamWrapper.ReadAsync(cancellationToken).ConfigureAwait(false);
                        if (bytes == null && !IsConnected)
                        {
                            break;
                        }

                        string JSON = Encoding.Default.GetString(bytes);
                        var obj = new JavaScriptSerializer().Deserialize<T>(JSON);

                        OnMessageReceived(obj);
                    }
                    catch (OperationCanceledException)
                    {
                        OnDisconnected();
                        throw;
                    }
                    catch (Exception exception)
                    {
                        OnExceptionOccurred(exception);
                    }
                }

                OnDisconnected();
            });
        }

        public async Task WriteAsync(T value, CancellationToken cancellationToken = default)
        {
            if (!IsConnected || !PipeStreamWrapper.CanWrite)
            {
                throw new InvalidOperationException("Client is not connected");
            }

            string JSON = new JavaScriptSerializer().Serialize(value);
            var bytes = Encoding.Default.GetBytes(JSON);

            await PipeStreamWrapper.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
        }

        public async Task StopAsync()
        {
            if (ReadWorker != null)
            {
                await ReadWorker.StopAsync().ConfigureAwait(false);

                ReadWorker = null;
            }

            await PipeStreamWrapper.StopAsync().ConfigureAwait(false);
        }

        public string GetImpersonationUserName()
        {
            if (!(PipeStream is NamedPipeServerStream serverStream))
            {
                throw new InvalidOperationException($"{nameof(PipeStream)} is not {nameof(NamedPipeServerStream)}.");
            }

            return serverStream.GetImpersonationUserName();
        }

        #endregion

        #region IDisposable

        public async Task Dispose()
        {
            await StopAsync().ConfigureAwait(false);
        }

        #endregion
    }
}


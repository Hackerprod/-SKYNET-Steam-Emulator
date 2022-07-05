using SKYNET.Common;
using System;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SKYNET.IPC
{
    /// <summary>
    /// Represents a connection between a named pipe client and server.
    /// </summary>
    /// <typeparam name="T">Reference type to read/write from the named pipe</typeparam>
    public sealed class PipeConnection<T>
    {
        /// <summary>
        /// Gets the connection's pipe name.
        /// </summary>
        public string PipeName { get; }

        /// <summary>
        /// Gets the connection's server name. Only for client connections.
        /// </summary>
        public string ServerName { get; }

        /// <summary>
        /// Gets a value indicating whether the pipe is connected or not.
        /// </summary>
        public bool IsConnected => PipeStreamWrapper.IsConnected;

        /// <summary>
        /// <see langword="true"/> if started and not disposed.
        /// </summary>
        public bool IsStarted => ReadWorker != null;

        /// <summary>
        /// Raw pipe stream. You can cast it to <see cref="NamedPipeClientStream"/> or <see cref="NamedPipeServerStream"/>.
        /// </summary>
        public PipeStream PipeStream { get; }

        private PipeStreamWrapper PipeStreamWrapper { get; }
        private TaskWorker? ReadWorker { get; set; }


        #region Events

        /// <summary>
        /// Invoked when the named pipe connection terminates.
        /// </summary>
        public event EventHandler<ConnectionEventArgs<T>>? Disconnected;

        /// <summary>
        /// Invoked whenever a message is received from the other end of the pipe.
        /// </summary>
        public event EventHandler<ConnectionMessageEventArgs<T>>? MessageReceived;

        /// <summary>
        /// Invoked when an exception is thrown during any read/write operation over the named pipe.
        /// </summary>
        public event EventHandler<ExceptionEventArgs>? ExceptionOccurred;

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
            ExceptionOccurred?.Invoke(this, new ExceptionEventArgs(exception));
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

        /// <summary>
        /// Begins reading from and writing to the named pipe on a background thread.
        /// This method returns immediately.
        /// </summary>
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

        /// <summary>
        /// Writes the specified <paramref name="value"/> and waits other end reading
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        public async Task WriteAsync(T value, CancellationToken cancellationToken = default)
        {
            if (!IsConnected || !PipeStreamWrapper.CanWrite)
            {
                Log.Write("PipeConnection", "Client is not connected to send data");
                return;
            }

            string JSON = new JavaScriptSerializer().Serialize(value);
            var bytes = Encoding.Default.GetBytes(JSON);

            await PipeStreamWrapper.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Dispose internal resources
        /// </summary>
        public async Task StopAsync()
        {
            if (ReadWorker != null)
            {
                await ReadWorker.StopAsync().ConfigureAwait(false);

                ReadWorker = null;
            }

            await PipeStreamWrapper.StopAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the user name of the client on the other end of the pipe.
        /// </summary>
        /// <returns>The user name of the client on the other end of the pipe.</returns>
        /// <exception cref="InvalidOperationException"><see cref="PipeStream"/> is not <see cref="NamedPipeServerStream"/>.</exception>
        /// <exception cref="InvalidOperationException">No pipe connections have been made yet.</exception>
        /// <exception cref="InvalidOperationException">The connected pipe has already disconnected.</exception>
        /// <exception cref="InvalidOperationException">The pipe handle has not been set.</exception>
        /// <exception cref="ObjectDisposedException">The pipe is closed.</exception>
        /// <exception cref="IOException">The pipe connection has been broken.</exception>
        /// <exception cref="IOException">The user name of the client is longer than 19 characters.</exception>
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

        /// <summary>
        /// Dispose internal resources
        /// </summary>
        public async Task Dispose()
        {
            await StopAsync().ConfigureAwait(false);
        }

        #endregion
    }
}


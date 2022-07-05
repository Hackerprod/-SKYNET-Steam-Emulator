using SKYNET.IPC.Types;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SKYNET.IPC
{
    public sealed class PipeClient
    {
        private volatile bool _isConnecting;
        private ulong _currentJob;

        /// <summary>
        /// Results that are waiting for collection from the pipe
        /// </summary>
        private List<IPCMessage> current_results;

        /// <summary>
        /// Semaphores for results that are being waited on
        /// </summary>
        private Dictionary<ulong, Semaphore> result_semaphores;


        /// <inheritdoc/>
        public bool AutoReconnect { get; set; } = true;

        /// <inheritdoc/>
        public TimeSpan ReconnectionInterval { get; }

        /// <inheritdoc/>
        public bool IsConnected => Connection != null;

        /// <inheritdoc/>
        public bool IsConnecting
        {
            get => _isConnecting;
            private set => _isConnecting = value;
        }


        /// <inheritdoc/>
        public string PipeName { get; }

        /// <inheritdoc/>
        public string ServerName { get; }

        /// <inheritdoc/>
        public PipeConnection<IPCMessage>? Connection { get; private set; }

        private System.Timers.Timer ReconnectionTimer { get; }
        public ulong NextJobID { get { _currentJob++; return _currentJob; } }


        /// <summary>
        /// Invoked whenever a message is received from the server.
        /// </summary>
        public event EventHandler<ConnectionMessageEventArgs<IPCMessage>>? MessageReceived;

        /// <summary>
        /// Invoked when the client disconnects from the server (e.g., the pipe is closed or broken).
        /// </summary>
        public event EventHandler<ConnectionEventArgs<IPCMessage>>? Disconnected;

        /// <summary>
        /// Invoked after each the client connect to the server (include reconnects).
        /// </summary>
        public event EventHandler<ConnectionEventArgs<IPCMessage>>? Connected;

        /// <summary>
        /// Invoked whenever an exception is thrown during a read or write operation on the named pipe.
        /// </summary>
        public event EventHandler<ExceptionEventArgs>? ExceptionOccurred;

        private void OnMessageReceived(ConnectionMessageEventArgs<IPCMessage> args)
        {
            if (result_semaphores.ContainsKey(args.Message.JobID))
            {
                current_results.Add(args.Message);
                var semaphore = result_semaphores[args.Message.JobID];
                semaphore?.Release();
            }
            else
            {
                MessageReceived?.Invoke(this, args);
            }
        }

        private void OnDisconnected(ConnectionEventArgs<IPCMessage> args)
        {
            Disconnected?.Invoke(this, args);
        }

        private void OnConnected(ConnectionEventArgs<IPCMessage> args)
        {
            Connected?.Invoke(this, args);
        }

        private void OnExceptionOccurred()
        {
            ExceptionOccurred?.Invoke(this, new ExceptionEventArgs());
        }


        /// <summary>
        /// Constructs a new <see cref="PipeClient{T}"/> to connect to the <see cref="PipeServer{T}"/> specified by <paramref name="pipeName"/>. <br/>
        /// Default reconnection interval - <see langword="100 ms"/>
        /// </summary>
        /// <param name="pipeName">Name of the server's pipe</param>
        /// <param name="serverName">the Name of the server, default is  local machine</param>
        /// <param name="reconnectionInterval">Default reconnection interval - <see langword="100 ms"/></param>
        /// <param name="formatter">Default formatter - <see cref="BinaryFormatter"/></param>
        public PipeClient(string pipeName, string serverName = ".", TimeSpan? reconnectionInterval = default)
        {
            PipeName = pipeName;
            ServerName = serverName;

            current_results = new List<IPCMessage>();
            result_semaphores = new Dictionary<ulong, Semaphore>();

            ReconnectionInterval = reconnectionInterval ?? TimeSpan.FromMilliseconds(100);
            ReconnectionTimer = new System.Timers.Timer(ReconnectionInterval.TotalMilliseconds);
            ReconnectionTimer.Elapsed += async (_, ob) =>
            {
                try
                {
                    if (!IsConnected && !IsConnecting)
                    {
                        using var cancellationTokenSource = new CancellationTokenSource(ReconnectionInterval);

                        try
                        {
                            await ConnectAsync(cancellationTokenSource.Token).ConfigureAwait(false);
                        }
                        catch (OperationCanceledException)
                        {
                        }
                    }
                }
                catch (Exception exception)
                {
                    ReconnectionTimer.Stop();

                    OnExceptionOccurred();
                }
            };
        }


        /// <summary>
        /// Connects to the named pipe server asynchronously.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            while (IsConnecting)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken).ConfigureAwait(false);
            }

            if (IsConnected)
            {
                return;
            }

            try
            {
                IsConnecting = true;

                if (AutoReconnect)
                {
                    ReconnectionTimer.Start();
                }

                var connectionPipeName = await GetConnectionPipeName(cancellationToken).ConfigureAwait(false);

                // Connect to the actual data pipe
                var dataPipe = CreateAndConnectAsync(connectionPipeName, ServerName, cancellationToken);

                Connection = new PipeConnection<IPCMessage>(dataPipe.Result, connectionPipeName, ServerName);
                Connection.Disconnected += async (_, args) =>
                {
                    await DisconnectInternalAsync().ConfigureAwait(false);

                    OnDisconnected(args);
                };
                Connection.MessageReceived += (_, args) => OnMessageReceived(args);
                Connection.ExceptionOccurred += (_, args) => OnExceptionOccurred();
                Connection.Start();

                OnConnected(new ConnectionEventArgs<IPCMessage>(Connection));
            }
            catch (Exception)
            {
                ReconnectionTimer.Stop();

                throw;
            }
            finally
            {
                IsConnecting = false;
            }
        }

        /// <summary>
        /// Disconnects from server
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public async Task DisconnectAsync(CancellationToken _ = default)
        {
            ReconnectionTimer.Stop();

            await DisconnectInternalAsync().ConfigureAwait(false);
        }

        private async Task DisconnectInternalAsync()
        {
            if (Connection == null)
            {
                return;
            }

            await Connection.Dispose();

            Connection = null;
        }

        /// <summary>
        /// Sends a message to the server over a named pipe. <br/>
        /// If client is not connected, <see cref="InvalidOperationException"/> is occurred
        /// </summary>
        /// <param name="value">Message to send to the server.</param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<IPCMessage> WriteAsync(IPCMessage value, bool WaitResult = false, CancellationToken cancellationToken = default)
        {
            if (!IsConnected && AutoReconnect)
            {
                await ConnectAsync(cancellationToken).ConfigureAwait(false);
            }
            if (Connection == null)
            {
                return null;
            }

            await Connection.WriteAsync(value, cancellationToken).ConfigureAwait(false);

            if (WaitResult)
            {
                var jobId = value.JobID;
                result_semaphores.Add(jobId, new Semaphore(0, 1));
                return WaitForResultForFunction(jobId);
            }
            return null;
        }

        public IPCMessage WaitForResultForFunction(ulong job_id)
        {
            // Wait for the semaphore and then remove it so gc collects it
            var this_semaphore = result_semaphores[job_id];
            this_semaphore.WaitOne();
            result_semaphores.Remove(job_id);

            var found = current_results.Find(x => x.JobID == job_id);
            current_results.Remove(found);

            return found;
        }

        /// <summary>
        /// Dispose internal resources
        /// </summary>
        public async void DisposeAsync()
        {
            ReconnectionTimer.Dispose();

            await DisconnectInternalAsync().ConfigureAwait(false);
        }



        /// <summary>
        /// Get the name of the data pipe that should be used from now on by this NamedPipeClient
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        private async Task<string> GetConnectionPipeName(CancellationToken cancellationToken = default)
        {
            using var handshake = await ConnectAsync(PipeName, ServerName, cancellationToken).ConfigureAwait(false);
            {
                var bytes = await handshake.ReadAsync(cancellationToken).ConfigureAwait(false);
                if (bytes == null)
                {
                    throw new InvalidOperationException("Connection failed: Returned by server pipeName is null");
                }

                return Encoding.UTF8.GetString(bytes);
            }
        }

        // Client Factory
        private static async Task<PipeStreamWrapper> ConnectAsync(string pipeName, string serverName, CancellationToken cancellationToken = default)
        {
            var pipe = await CreateAndConnectAsync(pipeName, serverName, cancellationToken).ConfigureAwait(false);

            return new PipeStreamWrapper(pipe);
        }

        private static async Task<NamedPipeClientStream> CreateAndConnectAsync(string pipeName, string serverName, CancellationToken cancellationToken = default)
        {
            var pipe = Create(pipeName, serverName);

            try
            {
                await pipe.ConnectAsync(cancellationToken).ConfigureAwait(false);

                return pipe;
            }
            catch
            {
                pipe.Dispose();
                throw;
            }
        }

        private static NamedPipeClientStream Create(string pipeName, string serverName)
        {
            return new NamedPipeClientStream(
                serverName,
                pipeName,
                direction: PipeDirection.InOut,
                options: PipeOptions.Asynchronous | PipeOptions.WriteThrough);
        }

    }
}


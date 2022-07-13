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

        private List<IPCMessage> CurrentResults;
        private List<ulong> RequestResults;

        public bool AutoReconnect { get; set; } = true;

        public TimeSpan ReconnectionInterval { get; }

        public bool IsConnected => Connection != null;

        public bool IsConnecting
        {
            get => _isConnecting;
            private set => _isConnecting = value;
        }

        public string PipeName { get; }

        public string ServerName { get; }

        public PipeConnection<IPCMessage>? Connection { get; private set; }

        private System.Timers.Timer ReconnectionTimer { get; }

        public event EventHandler<ConnectionMessageEventArgs<IPCMessage>>? MessageReceived;

        public event EventHandler<ConnectionEventArgs<IPCMessage>>? Disconnected;

        public event EventHandler<ConnectionEventArgs<IPCMessage>>? Connected;

        public event EventHandler<ExceptionEventArgs>? ExceptionOccurred;

        private void OnMessageReceived(ConnectionMessageEventArgs<IPCMessage> args)
        {
            if (RequestResults.Contains(args.Message.JobID))
            {
                CurrentResults.Add(args.Message);
                RequestResults.Remove(args.Message.JobID);
                Write("Founded message waiting response " + args.Message.JobID);
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

        public PipeClient(string pipeName, string serverName = ".", TimeSpan? reconnectionInterval = default)
        {
            PipeName = pipeName;
            ServerName = serverName;

            CurrentResults = new List<IPCMessage>();
            RequestResults = new List<ulong>();

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
                RequestResults.Add(jobId);
                return await WaitForResultForFunction(jobId);
            }
            return null;
        }

        public async Task<IPCMessage> WaitForResultForFunction(ulong job_id)
        {
            DateTime RequestTime = DateTime.Now;
            Label:;
            if ((DateTime.Now - RequestTime).Milliseconds > 500)
            {
                return null;
            }
            else
            {
                var result = CurrentResults.Find(x => x.JobID == job_id);
                if (result == null)
                {
                    await Task.Delay(10);
                    goto Label;
                }
                CurrentResults.Remove(result);
                return result;
            }
        }

        public async void DisposeAsync()
        {
            ReconnectionTimer.Dispose();

            await DisconnectInternalAsync().ConfigureAwait(false);
        }

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

        private void Write(string v)
        {
            SteamEmulator.Write("XXXXXXXXXXXXX", v);
        }
    }
}


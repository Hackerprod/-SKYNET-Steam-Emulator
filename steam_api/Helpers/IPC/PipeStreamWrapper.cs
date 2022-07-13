using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace SKYNET.IPC
{
    public sealed class PipeStreamWrapper : IDisposable
    {

        public bool IsConnected => BaseStream.IsConnected && Reader.IsConnected;

        public bool CanRead => BaseStream.CanRead;

        public bool CanWrite => BaseStream.CanWrite;

        private PipeStream BaseStream { get; }
        private PipeStreamReader Reader { get; }
        private PipeStreamWriter Writer { get; }

        public PipeStreamWrapper(PipeStream stream)
        {
            BaseStream = stream ?? throw new ArgumentNullException(nameof(stream));

            Reader = new PipeStreamReader(BaseStream);
            Writer = new PipeStreamWriter(BaseStream);
        }

        public async Task<byte[]?> ReadAsync(CancellationToken cancellationToken = default)
        {
            return await Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task WriteAsync(byte[] buffer, CancellationToken cancellationToken = default)
        {
            await Writer.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);

            Writer.WaitForPipeDrain();
        }

        public async Task StopAsync()
        {
            Dispose();

            await Task.CompletedTask.ConfigureAwait(false);
        }

        public void Dispose()
        {
            BaseStream.Dispose();

            // This is redundant, just to avoid mistakes and follow the general logic of Dispose
            Reader.Dispose();
            Writer.Dispose();
        }
    }
}


using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SKYNET.IPC
{
    public sealed class PipeStreamReader : IDisposable
    {

        public bool IsConnected { get; private set; }

        private PipeStream BaseStream { get; }

        public PipeStreamReader(PipeStream stream)
        {
            BaseStream = stream ?? throw new ArgumentNullException(nameof(stream));
            IsConnected = stream.IsConnected;
        }

        private async Task<int> ReadLengthAsync(CancellationToken cancellationToken = default)
        {
            var bytes = await ReadAsync(
                length: sizeof(int),
                throwIfReadLessThanLength: false,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            if (!bytes.Any())
            {
                IsConnected = false;
                return 0;
            }

            return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 0));
        }

        private async Task<byte[]> ReadAsync(
            int length,
            bool throwIfReadLessThanLength = true,
            CancellationToken cancellationToken = default)
        {
            var buffer = new byte[length];
            var read = await BaseStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
            if (read != buffer.Length)
            {
                return throwIfReadLessThanLength
                    ? throw new IOException($"Expected {buffer.Length} bytes but read {read}")
                    : Array.Empty<byte>();
            }

            return buffer;
        }

        public async Task<byte[]> ReadAsync(CancellationToken cancellationToken = default)
        {
            var length = await ReadLengthAsync(cancellationToken).ConfigureAwait(false);

            return length == 0
                ? default
                : await ReadAsync(
                    length: length,
                    throwIfReadLessThanLength: true,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public void Dispose()
        {
            BaseStream.Dispose();
        }

    }
}

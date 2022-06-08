using System;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SKYNET.IPC
{
    /// <summary>
    /// Wraps a <see cref="PipeStream"/> object and writes to it.
    /// </summary>
    public sealed class PipeStreamWriter : IDisposable
    {
        /// <summary>
        /// Gets the underlying <c>PipeStream</c> object.
        /// </summary>
        private PipeStream BaseStream { get; }
        private SemaphoreSlim SemaphoreSlim { get; } = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Constructs a new <c>PipeStreamWriter</c> object that writes to given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Pipe to write to</param>
        public PipeStreamWriter(PipeStream stream)
        {
            BaseStream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        private async Task WriteLengthAsync(int length, CancellationToken cancellationToken = default)
        {
            var buffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(length));

            await BaseStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Writes an object to the pipe.
        /// </summary>
        /// <param name="buffer">Object to write to the pipe</param>
        /// <param name="cancellationToken"></param>
        public async Task WriteAsync(byte[] buffer, CancellationToken cancellationToken = default)
        {
            buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));

            try
            {
                await SemaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(false);

                await WriteLengthAsync(buffer.Length, cancellationToken).ConfigureAwait(false);

                await BaseStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);

                try
                {
                    await BaseStream.FlushAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (IOException)
                {
                    // ignoring IOException according this issue: https://github.com/HavenDV/H.Pipes/issues/22#issuecomment-1144234017
                }
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        /// <summary>
        /// Waits for the other end of the pipe to read all sent bytes.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The pipe is closed.</exception>
        /// <exception cref="NotSupportedException">The pipe does not support write operations.</exception>
        /// <exception cref="IOException">The pipe is broken or another I/O error occurred.</exception>
        public void WaitForPipeDrain()
        {
            try
            {
                BaseStream.WaitForPipeDrain();
            }
            catch (IOException)
            {
                // ignoring IOException according this issue: https://github.com/HavenDV/H.Pipes/issues/22#issuecomment-1144234017
            }
        }

        /// <summary>
        /// Dispose internal <see cref="PipeStream"/>
        /// </summary>
        public void Dispose()
        {
            BaseStream.Dispose();
            SemaphoreSlim.Dispose();
        }
    }
}

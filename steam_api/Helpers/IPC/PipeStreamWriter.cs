using System;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SKYNET.IPC
{
    public sealed class PipeStreamWriter : IDisposable
    {
        private PipeStream BaseStream { get; }
        private SemaphoreSlim SemaphoreSlim { get; } = new SemaphoreSlim(1, 1);

        public PipeStreamWriter(PipeStream stream)
        {
            BaseStream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        private async Task WriteLengthAsync(int length, CancellationToken cancellationToken = default)
        {
            var buffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(length));

            await BaseStream.WriteAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
        }

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

        public void Dispose()
        {
            BaseStream.Dispose();
            SemaphoreSlim.Dispose();
        }
    }
}

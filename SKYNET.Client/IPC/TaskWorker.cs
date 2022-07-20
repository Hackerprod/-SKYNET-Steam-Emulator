using System;
using System.Threading;
using System.Threading.Tasks;

namespace SKYNET.IPC
{
    internal class TaskWorker
    {
        #region Fields

        private volatile bool _isDisposed;

        #endregion

        #region Properties

        /// <summary>
        /// Internal task
        /// </summary>
        public Task Task { get; set; }

        /// <summary>
        /// Internal task CancellationTokenSource
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates and starts <see cref="TaskWorker"/>
        /// </summary>
        /// <param name="action"></param>
        /// <param name="exceptionAction"></param>
        public TaskWorker(Func<CancellationToken, Task> action, Action<Exception>? exceptionAction = null)
        {
            Task = Task.Factory.StartNew(async () =>
            {
                try
                {
                    await action(CancellationTokenSource.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception exception)
                {
                    exceptionAction?.Invoke(exception);
                }
            }, CancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Cancel task(if it's not completed) and dispose internal resources <br/>
        /// </summary>
        public async Task StopAsync()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            CancellationTokenSource.Cancel();

            try
            {
                await Task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }

            // Some system code can still use CancellationToken, so we wait
            await Task.Delay(TimeSpan.FromMilliseconds(1)).ConfigureAwait(false);

            CancellationTokenSource.Dispose();
            Task.Dispose();
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Cancel task(if it's not completed) and dispose internal resources <br/>
        /// </summary>
        public async void DisposeAsync()
        {
            await StopAsync().ConfigureAwait(false);
        }

        #endregion
    }
}

using System;
using System.Collections.Concurrent;

namespace SKYNET.Managers
{
    internal static class NativeCallbackQueue
    {
        private const int MaxCallbacksPerFrame = 1024;
        private static readonly ConcurrentQueue<Action> ClientCallbacks = new ConcurrentQueue<Action>();
        private static readonly ConcurrentQueue<Action> GameServerCallbacks = new ConcurrentQueue<Action>();

        public static void Enqueue(Action callback, bool gameServer = false)
        {
            if (callback == null)
            {
                return;
            }

            (gameServer ? GameServerCallbacks : ClientCallbacks).Enqueue(callback);
        }

        public static void Drain(bool gameServer)
        {
            var queue = gameServer ? GameServerCallbacks : ClientCallbacks;
            var delivered = 0;
            while (delivered < MaxCallbacksPerFrame && queue.TryDequeue(out var callback))
            {
                try
                {
                    callback();
                }
                catch (Exception ex)
                {
                    SteamEmulator.Write("Native callbacks", ex);
                }

                delivered++;
            }
        }
    }
}

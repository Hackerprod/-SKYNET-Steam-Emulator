using System;
using System.Collections.Concurrent;

namespace SKYNET.Helpers
{
    public class MutexHelper
    {
        // Named in-process locks. The old implementation used a named kernel
        // Mutex per name: every WaitOne/ReleaseMutex was a syscall (~µs) and a
        // named kernel object needlessly serialized this data across processes.
        // Monitor over a plain gate object is a cheap in-process lock (no
        // syscall in the uncontended case) with the same "wait by name" API.
        private static readonly ConcurrentDictionary<string, object> Gates = new();

        private static object Gate(string name) => Gates.GetOrAdd(name, _ => new object());

        public static void Wait(string MutexName, Action code)
        {
            var gate = Gate(MutexName);
            lock (gate)
            {
                try
                {
                    code();
                }
                catch
                {
                }
            }
        }
    }
}

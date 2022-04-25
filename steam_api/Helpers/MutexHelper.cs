using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SKYNET.Helper
{
    public class MutexHelper
    {
        private static ConcurrentDictionary<string, Mutex> StoredMutex;

        static MutexHelper()
        {
            StoredMutex = new ConcurrentDictionary<string, Mutex>();
        }

        private static Mutex GetOrCreate(string MutexName)
        {
            if (StoredMutex.TryGetValue(MutexName, out Mutex mutex))
            {
                return mutex;
            }
            else
            {
                mutex = new Mutex(false, MutexName);
                StoredMutex.TryAdd(MutexName, mutex);
            }
            return mutex;
        }

        public static void Wait(string MutexName, Action code)
        {
            //Mutex mutex = GetOrCreate(MutexName);

            try
            {
                //mutex.WaitOne();
                code();
            }
            catch
            {
                //
            }
            finally
            {
                //mutex.ReleaseMutex();
            }
        }
    }
}

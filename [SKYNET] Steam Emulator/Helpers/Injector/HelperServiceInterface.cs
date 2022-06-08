using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SKYNET.Helpers
{
    public class HelperServiceInterface : MarshalByRefObject
    {
        private class InjectionWait
        {
            public Mutex ThreadLock = new Mutex(initiallyOwned: false);

            public ManualResetEvent Completion = new ManualResetEvent(initialState: false);

            public Exception Error;
        }

        private static SortedList<int, InjectionWait> InjectionList = new SortedList<int, InjectionWait>();


        public object ExecuteAsService<TClass>(string InMethodName, object[] InParams)
        {
            return typeof(TClass).InvokeMember(InMethodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, InParams);
        }

        public static void BeginInjection(int InTargetPID)
        {
            InjectionWait value;
            lock (InjectionList)
            {
                if (!InjectionList.TryGetValue(InTargetPID, out value))
                {
                    value = new InjectionWait();
                    InjectionList.Add(InTargetPID, value);
                }
            }
            value.ThreadLock.WaitOne();
            value.Error = null;
            value.Completion.Reset();
            lock (InjectionList)
            {
                if (!InjectionList.ContainsKey(InTargetPID))
                {
                    InjectionList.Add(InTargetPID, value);
                }
            }
        }

        public static void EndInjection(int InTargetPID)
        {
            lock (InjectionList)
            {
                InjectionList[InTargetPID].ThreadLock.ReleaseMutex();
                InjectionList.Remove(InTargetPID);
            }
        }

        public static void WaitForInjection(int InTargetPID)
        {
            InjectionWait injectionWait;
            lock (InjectionList)
            {
                injectionWait = InjectionList[InTargetPID];
            }
            if (!injectionWait.Completion.WaitOne(20000, exitContext: false))
            {
                throw new TimeoutException("Unable to wait for injection completion.");
            }
            if (injectionWait.Error != null)
            {
                throw injectionWait.Error;
            }
        }

        public void InjectionException(int InClientPID, Exception e)
        {
            InjectionWait injectionWait;
            lock (InjectionList)
            {
                injectionWait = InjectionList[InClientPID];
            }
            injectionWait.Error = e;
            injectionWait.Completion.Set();
        }

        public void InjectionCompleted(int InClientPID)
        {
            InjectionWait injectionWait;
            lock (InjectionList)
            {
                injectionWait = InjectionList[InClientPID];
            }
            injectionWait.Error = null;
            injectionWait.Completion.Set();
        }

        public void Ping()
        {
        }
    }
}

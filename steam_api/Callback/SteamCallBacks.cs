//using SKYNET.Helper;
//using SKYNET.Steamworks;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using SteamAPICall_t = System.UInt64;

//namespace SKYNET.Callback
//{
//    public class Steam_Call_Back
//    {
//        public List<IntPtr> callbacks;
//        public List<ICallbackData> results;

//        public Steam_Call_Back()
//        {
//            callbacks = new List<IntPtr>();
//            results = new List<ICallbackData>();
//        }
//    };

//    public class SteamCallBacks
//    {
//        public static ConcurrentDictionary<int, Steam_Call_Back> callbacks;
//        public static SteamCallResults steamCallResults;

//        static SteamCallBacks()
//        {
//            steamCallResults = new SteamCallResults();
//            callbacks = new ConcurrentDictionary<int, Steam_Call_Back>();
//        }

//        public static void addCallBack(int iCallback, IntPtr cb)
//        {
//            try
//            {
//                if (iCallback == (int)new SteamAPICallCompleted_t().CallbackType)
//                {
//                    steamCallResults.addCallCompleted(cb);
//                    CCallbackMgr.SetRegister(cb, iCallback);
//                    return;
//                }

//                if (callbacks.TryGetValue(iCallback, out Steam_Call_Back cb_result))
//                {
//                    callbacks[iCallback].callbacks.Add(cb);
//                    CCallbackMgr.SetRegister(cb, iCallback);
//                    //for (auto & res: callbacks[iCallback].results)
//                    {
//                        //TODO: timeout?
//                        SteamAPICall_t api_id = steamCallResults.addCallResult(iCallback, null, 0, 0.0, false);
//                        steamCallResults.addCallBack(api_id, cb);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                SteamEmulator.Write("cd", ex);
//            }
//        }
//        public static void addCBResult(int iCallback, ICallbackData result, int size)
//        {
//            addCBResult(iCallback, result, size, 60000, false);
//        }

//        public static void addCBResult(int iCallback, ICallbackData result, int size, double timeout, bool dont_post_if_already)
//        {
//            callbacks[iCallback].results.Add(result);
//            foreach (var cb in callbacks[iCallback].callbacks)
//            {
//                SteamAPICall_t api_id = steamCallResults.addCallResult(iCallback, result, size, timeout, false);
//                steamCallResults.addCallBack(api_id, cb);
//            }
//            if (!callbacks[iCallback].callbacks.Any())
//            {
//                steamCallResults.addCallResult(iCallback, result, size, timeout, false);
//            }
//        }
//    }

//    public class CCallbackMgr
//    {
//        public const byte k_ECallbackFlagsRegistered = 1;
//        public const byte k_ECallbackFlagsGameServer = 2;

//        internal static void SetRegister(IntPtr ptrCallback, int iCallback)
//        {
//            CCallbackBase pCallback = ptrCallback.ToType<CCallbackBase>();
//            pCallback.m_nCallbackFlags |= k_ECallbackFlagsRegistered;
//            pCallback.m_iCallback = iCallback;
//            Marshal.StructureToPtr(pCallback, ptrCallback, false);
//        }

//        internal static void SetUnregister(IntPtr ptrCallback)
//        {
//            CCallbackBase pCallback = ptrCallback.ToType<CCallbackBase>();
//            pCallback.m_nCallbackFlags |= k_ECallbackFlagsRegistered;
//            Marshal.StructureToPtr(pCallback, ptrCallback, false);
//        }

//        internal static bool isServer(IntPtr ptrCallback)
//        {
//            CCallbackBase pCallback = ptrCallback.ToType<CCallbackBase>();
//            return (pCallback.m_nCallbackFlags & k_ECallbackFlagsGameServer) != 0;
//        }
//    }

//    public class SteamCallResults
//    {
//        public List<Steam_Call_Result> callresults;
//        public List<IntPtr> completed_callbacks;
//        //void ( cb_all) (List<char> result, int callback) = nullptr;

//        public SteamCallResults()
//        {
//            callresults = new List<Steam_Call_Result>();
//            completed_callbacks = new List<IntPtr>();
//        }

//        public void addCallBack(SteamAPICall_t api_call, IntPtr cb)
//        {
//            var CallbackBase = cb.ToType<CCallbackBase>();
//            var cb_result = callresults.Find(cr => cr.api_call == api_call);
//            if (cb_result != null)
//            {
//                cb_result.callbacks.Add(cb);
//                CCallbackMgr.SetRegister(cb, CallbackBase.m_iCallback);
//            }
//        }

//        public void addCallCompleted(IntPtr cb)
//        {
//            completed_callbacks.Add(cb);
//        }


//        internal SteamAPICall_t addCallResult(int iCallback, ICallbackData result, int size, double timeout = 120, bool run_call_completed_cb = true)
//        {
//            try
//            {
//                CSteamID id = new CSteamID();
//                return addCallResult((ulong)id, iCallback, result, size, timeout, run_call_completed_cb);
//            }
//            catch (Exception ex)
//            {
//                SteamEmulator.Write("cd", ex);
//            }
//            return 0;
//        }

//        internal SteamAPICall_t addCallResult(SteamAPICall_t api_call, int iCallback, ICallbackData result, int size, double timeout = 120, bool run_call_completed_cb = true)
//        {
//            try
//            {
//                var cb_result = callresults.Find(scr => scr.api_call == api_call);
//                if (cb_result != null)
//                {
//                    // if recerved
//                }
//                else
//                {
//                    var res = new Steam_Call_Result(api_call, iCallback, result, 0, 0.0, false);
//                    callresults.Add(res);
//                    return res.api_call;
//                }
//            }
//            catch (Exception ex)
//            {
//                SteamEmulator.Write("cd", ex);
//            }
//            return 0;
//        }

//        public void runCallResults()
//        {
//            try
//            {
//                long current_size = callresults.Count();
//                for (int i = 0; i < current_size; ++i)
//                {
//                    int index = i;

//                    if (!callresults[index].to_delete)
//                    {
//                        if (callresults[index].can_execute())
//                        {
//                            var result = callresults[index].result;
//                            SteamAPICall_t api_call = callresults[index].api_call;
//                            bool run_call_completed_cb = callresults[index].run_call_completed_cb;
//                            int iCallback = callresults[index].iCallback;
//                            if (run_call_completed_cb)
//                            {
//                                callresults[index].run_call_completed_cb = false;
//                            }

//                            callresults[index].to_delete = true;
//                            if (callresults[index].callbacks.Any())
//                            {
//                                var temp_cbs = callresults[index].callbacks;
//                                foreach (var cb in temp_cbs)
//                                {
//                                    PRINT_DEBUG("Calling callresult %p %i\n");

//                                    //TODO: unlock relock doesn't work if mutex was locked more than once.
//                                    if (run_call_completed_cb)
//                                    { //run the right function depending on if it's a callback or a call result.
//                                        Run(cb, result[0], false, api_call);
//                                    }
//                                    else
//                                    {
//                                        Run(cb, result[0]);
//                                    }
//                                    PRINT_DEBUG("callresult done\n");
//                                }
//                            }

//                            if (run_call_completed_cb)
//                            {
//                                //can it happen that one is removed during the callback?
//                                List<IntPtr> callbacks = completed_callbacks;
//                                SteamAPICallCompleted_t data;
//                                data.m_hAsyncCall = api_call;
//                                data.m_iCallback = iCallback;
//                                data.m_cubParam = (uint)result.Count();

//                                foreach (var cb in callbacks)
//                                {
//                                    PRINT_DEBUG("Call complete cb %i %p %llu\n");
//                                    //TODO: check if this is a problem or not.
//                                    SteamAPICallCompleted_t temp = data;
//                                    Run(cb, temp);
//                                }
//                            }
//                        }
//                        else
//                        {
//                            if (callresults[index].timed_out())
//                            {
//                                callresults[index].to_delete = true;
//                            }
//                        }
//                    }
//                }

//                //PRINT_DEBUG("runCallResults erase to_delete\n");
//                for (int i = 0; i < callresults.Count; i++)
//                {
//                    var c = callresults[i];
//                    if (c.to_delete)
//                    {
//                        callresults.RemoveAt(i);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                SteamEmulator.Write("cd", ex);
//            }
//        }

//        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
//        private delegate void RunCallbackDelegate(IntPtr pvParam);

//        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
//        private delegate void RunCallResultDelegate(IntPtr pvParam, bool bIOFailure, SteamAPICall_t hSteamAPICall);

//        [MarshalAs(UnmanagedType.FunctionPtr)]
//        private RunCallbackDelegate RunCallback;

//        [MarshalAs(UnmanagedType.FunctionPtr)]
//        private RunCallResultDelegate RunCallResult;


//        private void Run(IntPtr cb, ICallbackData data)
//        {
//            CCallbackBase CallbackBase = cb.ToType<CCallbackBase>();
//            CCallResult cResult = CallbackBase.m_vfptr.ToType<CCallResult>();
//            RunCallback = Marshal.GetDelegateForFunctionPointer<RunCallbackDelegate>(cResult.m_RunCallback);
//            RunCallResult = Marshal.GetDelegateForFunctionPointer<RunCallResultDelegate>(cResult.m_RunCallResult);

//            IntPtr result = Marshal.AllocHGlobal(data.DataSize);
//            Marshal.StructureToPtr(data, result, false);

//            RunCallback(result);
//        }

//        private void Run(IntPtr cb, ICallbackData data, bool v, ulong api_call)
//        {
//            CCallbackBase CallbackBase = cb.ToType<CCallbackBase>();
//            CCallResult cResult = CallbackBase.m_vfptr.ToType<CCallResult>();
//            RunCallback = Marshal.GetDelegateForFunctionPointer<RunCallbackDelegate>(cResult.m_RunCallback);
//            RunCallResult = Marshal.GetDelegateForFunctionPointer<RunCallResultDelegate>(cResult.m_RunCallResult);

//            IntPtr result = Marshal.AllocHGlobal(data.DataSize);
//            Marshal.StructureToPtr(data, result, false);

//            RunCallResult(result, v, api_call);
//        }

//        private void PRINT_DEBUG(string v)
//        {
//            SteamEmulator.Write("sdsd", v);
//        }
//    }
//    public class Steam_Call_Result
//    {
//        public SteamAPICall_t api_call;
//        public List<IntPtr> callbacks;
//        public List<ICallbackData> result;
//        public bool to_delete = false;
//        public bool reserved = false;
//        public DateTime created;
//        public double run_in;
//        public bool run_call_completed_cb;
//        public int iCallback;

//        public Steam_Call_Result(SteamAPICall_t a, int icb, ICallbackData r, int s, double r_in, bool run_cc_cb)
//        {
//            callbacks = new List<IntPtr>();
//            api_call = a;
//            created = DateTime.Now;
//            run_in = r_in;
//            run_call_completed_cb = run_cc_cb;
//            iCallback = icb;
//            result = new List<ICallbackData>();
//            if (r != null)
//            {
//                result.Add(r);
//            }
//        }

//        internal bool can_execute()
//        {
//            return timed_out() && !to_delete && callbacks.Count > 0;
//        }

//        internal bool timed_out()
//        {
//            return (DateTime.Now - created).Seconds > 120;
//        }
//    }
//}

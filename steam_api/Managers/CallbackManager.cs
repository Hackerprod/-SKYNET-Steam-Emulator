using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SKYNET.Callback;
using SKYNET.Helper;
using SKYNET.Steamworks;
using Steamworks;
using SteamAPICall_t = System.UInt64;

namespace SKYNET.Managers
{
    public unsafe class CallbackManager
    {
        public static ConcurrentDictionary<CallbackType, SteamCallback> SteamCallbacks;
        public static Dictionary<SteamAPICall_t, ICallbackData> CallbackResults { get; set; }
        public static List<ICallbackData> Callbacks { get; set; }

        public static SteamAPICall_t CurrentCall;

        static CallbackManager()
        {
            CurrentCall = 1000000000;

            SteamCallbacks = new ConcurrentDictionary<CallbackType, SteamCallback>();
            CallbackResults = new Dictionary<SteamAPICall_t, ICallbackData>();
            Callbacks = new List<ICallbackData>();
        }

        public static void RegisterCallback(SteamCallback sCallback)
        {
            //if (sCallback.SteamAPICall == (ulong)new SteamAPICallCompleted_t().CallbackType)
            //{
            //    Write("Completeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeed");
            //}

            sCallback.Register();
            CallbackType callType = (CallbackType)sCallback.CallbackType;

            MutexHelper.Wait("SteamCallbacks", delegate
            {
                if (SteamCallbacks.ContainsKey(callType))
                {
                    Write($"RegisterCallback: SteamCallbacks contains key {callType}");
                    SteamCallbacks[callType] = sCallback;
                }
                else
                {
                    Write($"RegisterCallback: Creating new SteamCallbacks with key {callType}");
                    SteamCallbacks.TryAdd(callType, sCallback);
                }
            });
        }

        public static void RegisterCallResult(SteamCallback steamCallback)
        {
            Write($"RegisterCallResult: Processing callback {steamCallback.SteamAPICall}");

            MutexHelper.Wait("RegisterCallResult", delegate
            {
                try
                {
                    CallbackType callType = (CallbackType)steamCallback.CallbackBase.m_iCallback;

                    MutexHelper.Wait("SteamCallbacks", delegate
                    {
                        if (SteamCallbacks.ContainsKey(callType))
                        {
                            Write($"RegisterCallResult: SteamCallbacks contains key {callType}");
                            SteamCallbacks[callType] = steamCallback;
                        }
                        else
                        {
                            Write($"RegisterCallResult: Creating new SteamCallbacks with key {callType}");
                            SteamCallbacks.TryAdd(callType, steamCallback);
                        }
                    });

                }
                catch (Exception ex)
                {
                    Write($"Error in RegisterCallResult, {ex}");
                }
            });
        }

        public static SteamAPICall_t AddCallbackResult(ICallbackData data)
        {
            //return CallbackWrapper.AddCallbackResult(data);

            CurrentCall++;

            MutexHelper.Wait("CallbackResults", delegate
            {
                CallbackResults.Add(CurrentCall, data);
            });

            Write($"Added CallbackResult {CurrentCall} {((int)data.CallbackType).GetCallbackType()} {data.CallbackType} ");

            return CurrentCall;
        }

        public static void AddCallback(ICallbackData data)
        {
            //return CallbackWrapper.AddCallbackResult(data);

            MutexHelper.Wait("Callbacks", delegate
            {
                Callbacks.Add(data);
            });

            Write($"Added Callback {CurrentCall} {((int)data.CallbackType).GetCallbackType()} {data.CallbackType} ");
        }

        public static void RunCallbacks()
        {
            MutexHelper.Wait("CallbackResults", delegate
            {
                foreach (var KV in CallbackResults)
                {
                    try
                    {
                        SteamAPICall_t APICall = KV.Key;
                        ICallbackData callbackData = KV.Value;

                        MutexHelper.Wait("SteamCallbacks", delegate
                        {
                            if (SteamCallbacks.ContainsKey(callbackData.CallbackType))
                            {
                                SteamEmulator.Debug($"Foooooooooooooooooooooooooooound in RunCallbacks, {callbackData.CallbackType}");
                                SteamEmulator.Debug($"Found callback {callbackData.CallbackType}");

                                SteamCallback Callback = SteamCallbacks[callbackData.CallbackType];
                                Callback.Run(callbackData, false, APICall);
                                CallbackResults.Remove(APICall);
                            }
                            else
                            {
                                var Callback = SteamCallbacks.Where(c => c.Value.CallbackType == callbackData.CallbackType).Select(c => c.Value).FirstOrDefault();
                                if (Callback != null)
                                {
                                    SteamEmulator.Debug($"Yeeeeeeeeeeeeeeeeeeeeeeeeeeees in RunCallbacks, {callbackData.CallbackType}");
                                    Callback.Run(callbackData, false, APICall);
                                    CallbackResults.Remove(APICall);
                                }
                                else
                                {
                                    SteamEmulator.Debug($"not found callback for {callbackData.CallbackType}", ConsoleColor.Green);
                                }
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Write("" + ex);
                    }
                }
            });

            MutexHelper.Wait("Callbacks", delegate
            {
                foreach (var callbackData in Callbacks)
                {
                    SteamEmulator.Debug($"In Callbacks list {callbackData.CallbackType}", ConsoleColor.Yellow);

                    try
                    {
                        var Callback = SteamCallbacks.Where(c => c.Value.CallbackType == callbackData.CallbackType).Select(c => c.Value).FirstOrDefault();
                        if (Callback != null)
                        {
                            SteamEmulator.Debug($"Yeeeeeeeeeeeeeeeeeeeeeeeeeeees in RunCallbacks, {callbackData.CallbackType}");
                            Callback.Run(callbackData);
                        }
                        else
                        {
                            SteamEmulator.Debug($"not found callback for {callbackData.CallbackType}", ConsoleColor.Green);
                        }
                    }
                    catch (Exception ex)
                    {
                        Write("" + ex);
                    }
                }
            });
        }

        internal static bool IsCompleted(ulong hSteamAPICall)
        {
            bool Result = false;
            MutexHelper.Wait("SteamCallbacks", delegate
            {
                foreach (var callback in SteamCallbacks)
                {
                    if (callback.Value.SteamAPICall == hSteamAPICall)
                    {
                        Result = true;
                    }
                }
            });
            return Result;
        }

        internal static void UnregisterCallback(IntPtr pCallback)
        {
            CCallbackBase Callback = pCallback.ToType<CCallbackBase>();
        }

        private static void Write(string v)
        {
            SteamEmulator.Write("CallbackManager", v);
        }
    }
}



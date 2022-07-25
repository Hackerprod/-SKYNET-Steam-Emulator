using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SKYNET.Callback;
using SKYNET.Helpers;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Managers
{
    public unsafe class CallbackManager
    {
        public static ConcurrentDictionary<CallbackType, SteamCallback> SteamCallbacks;
        public static ConcurrentDictionary<SteamAPICall_t, CallbackMessage> CallbackResults { get; set; }
        public static List<CallbackMessage> Callbacks { get; set; }

        public static SteamAPICall_t CurrentCall;

        private const int CallbackTimeOut = 20;

        static CallbackManager()
        {
            CurrentCall = 1000;

            SteamCallbacks = new ConcurrentDictionary<CallbackType, SteamCallback>();
            CallbackResults = new ConcurrentDictionary<SteamAPICall_t, CallbackMessage>();
            Callbacks = new List<CallbackMessage>();
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
                    //Write($"RegisterCallback: SteamCallbacks contains key {callType}");
                    SteamCallbacks[callType] = sCallback;
                }
                else
                {
                    //Write($"RegisterCallback: Creating new SteamCallbacks with key {callType}");
                    SteamCallbacks.TryAdd(callType, sCallback);
                }
            });
        }

        public static void RegisterCallResult(SteamCallback steamCallback)
        {
            MutexHelper.Wait("RegisterCallResult", delegate
            {
                try
                {
                    CallbackType callType = (CallbackType)steamCallback.CallbackBase.m_iCallback;

                    MutexHelper.Wait("SteamCallbacks", delegate
                    {
                        if (SteamCallbacks.ContainsKey(callType))
                        {
                            //Write($"RegisterCallResult: SteamCallbacks contains key {callType}");
                            SteamCallbacks[callType] = steamCallback;
                        }
                        else
                        {
                            //Write($"RegisterCallResult: Creating new SteamCallbacks with key {callType}");
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

        public static SteamAPICall_t AddCallbackResult(ICallbackData data, bool ReadyToCall = true)
        {
            CurrentCall++;

            MutexHelper.Wait("CallbackResults", delegate
            {
                CallbackResults.TryAdd(CurrentCall, new CallbackMessage(data, ReadyToCall));
            });

            Write($"Added CallbackResult {CurrentCall} {((int)data.CallbackType).GetCallbackType()} {data.CallbackType} ");

            return CurrentCall;
        }

        public static void AddCallback(ICallbackData data, bool readyToCall = true)
        {
            MutexHelper.Wait("Callbacks", delegate
            {
                Callbacks.Add(new CallbackMessage(data, readyToCall));
            });

            Write($"Added Callback {CurrentCall} {((int)data.CallbackType).GetCallbackType()} {data.CallbackType} ");
        }

        public static void RunCallbacks()
        {
            if (!SteamEmulator.RunCallbacks) return;

            MutexHelper.Wait("CallbackResults", delegate
            {
                foreach (var KV in CallbackResults)
                {
                    try
                    {
                        SteamAPICall_t APICall = KV.Key;
                        var CallbackMessage = KV.Value;
                        ICallbackData callbackData = CallbackMessage.Data;
                        if (!CallbackMessage.ReadyToCall)
                        {
                            if (CallbackMessage.TimeSeconds > 5)
                            {
                                CallbackMessage.ReadyToCall = true;
                            }
                            goto Skip;
                        }
                        if (CallbackMessage.TimedOut())
                        {
                            CallbackResults.TryRemove(APICall, out _);
                            goto Skip;
                        }
                        if (CallbackMessage.Called)
                        {
                            CallbackResults.TryRemove(APICall, out _);
                        }
                        else
                        {
                            MutexHelper.Wait("SteamCallbacks", delegate
                            {
                                if (SteamCallbacks.ContainsKey(callbackData.CallbackType))
                                {
                                    SteamEmulator.Debug($"Found callback {callbackData.CallbackType}");

                                    SteamCallback Callback = SteamCallbacks[callbackData.CallbackType];
                                    Callback.Run(callbackData, false, APICall);
                                    CallbackMessage.Called = true;
                                    Write($"Called function (IntPtr, bool, SteamAPICall_t) in {callbackData.CallbackType} callback");
                                }
                                else
                                {
                                    var Callback = SteamCallbacks.Where(c => c.Value.CallbackType == callbackData.CallbackType).Select(c => c.Value).FirstOrDefault();
                                    if (Callback != null)
                                    {
                                        Callback.Run(callbackData, false, APICall);
                                        CallbackMessage.Called = true;
                                        Write($"Called function (IntPtr, bool, SteamAPICall_t) in {callbackData.CallbackType} callback");
                                    }
                                    else
                                    {
                                        //SteamEmulator.Debug($"not found callback for {callbackData.CallbackType}", ConsoleColor.Green);
                                    }
                                }
                            });
                        }

                        Skip:;
                    }
                    catch (Exception ex)
                    {
                        Write("" + ex);
                    }
                }
            });

            MutexHelper.Wait("Callbacks", delegate
            {
                for (int i = 0; i < Callbacks.Count; i++)
                {
                    var CallbackMessage = Callbacks[i];
                    if (CallbackMessage.Called)
                    {
                        Callbacks.RemoveAt(i);
                    }
                    else
                    {
                        if (!CallbackMessage.ReadyToCall)
                        {
                            if (CallbackMessage.TimeSeconds > 5)
                            {
                                CallbackMessage.ReadyToCall = true;
                            }
                            goto Skip;
                        }
                        if (CallbackMessage.TimedOut())
                        {
                            Callbacks.RemoveAt(i);
                            goto Skip;
                        }

                        var data = CallbackMessage.Data;
                        try
                        {
                            var Callback = SteamCallbacks.Where(c => c.Value.CallbackType == data.CallbackType).Select(c => c.Value).FirstOrDefault();
                            if (Callback != null)
                            {
                                Callback.Run(data);
                                CallbackMessage.Called = true;
                                Write($"Called function Run(IntPtr) in {data.CallbackType} callback");
                            }
                            else
                            {
                                //SteamEmulator.Debug($"not found callback for {data.CallbackType}", ConsoleColor.Green);
                            }
                        }
                        catch (Exception ex)
                        {
                            Write("" + ex);
                        }

                        Skip:;
                    }
                }
            });
        }

        internal static bool GetFirstCallResult(out CallbackMessage callResult, out SteamAPICall_t APICall_t)
        {
            CallbackMessage callback = default;
            SteamAPICall_t APICall = 0;
            MutexHelper.Wait("CallbackResults", delegate
            {
                if (CallbackResults.Any())
                {
                    var KV = CallbackResults.FirstOrDefault();
                    APICall = KV.Key;
                    callback = KV.Value;
                }
            });
            APICall_t = APICall;
            callResult = callback;
            return callback != null;
        }

        internal static bool GetCallResult(ulong handle, out CallbackMessage cCallback)
        {
            CallbackMessage callback = default;
            MutexHelper.Wait("CallbackResults", delegate
            {
                CallbackResults.TryGetValue(handle, out callback);
            });
            cCallback = callback;
            return cCallback != null;
        }

        public static bool UnregisterCallResult(SteamCallback pCallback, ulong hAPICall)
        {
            bool Result = false;
            MutexHelper.Wait("CallbackResults", delegate
            {
                Result = CallbackResults.TryRemove(hAPICall, out _);
            });
            return Result;
        }

        public static bool IsCompleted(ulong hSteamAPICall)
        {
            bool Result = false;
            MutexHelper.Wait("SteamCallbacks", delegate
            {
                if (CallbackResults.TryGetValue(hSteamAPICall, out var callback))
                {
                    //Result = callback.Called;
                    Result = callback.ReadyToCall;
                }
            });
            return Result;
        }

        public static bool UnregisterCallback(SteamCallback pCallback)
        {
            var Type = pCallback.CallbackType;
            return SteamCallbacks.TryRemove(Type, out _);
        }

        private static void Write(string v)
        {
            SteamEmulator.Write("CallbackManager", v);
        }
    }
}



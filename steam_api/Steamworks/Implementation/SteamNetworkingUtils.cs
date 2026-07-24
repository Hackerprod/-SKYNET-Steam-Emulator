using System;
using System.Runtime.InteropServices;
using System.Threading;
using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks.Interfaces;

using SteamNetworkingPOPID = System.UInt32;
using SteamNetworkingMicroseconds = System.Int64;
using HSteamNetConnection = System.UInt32;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamNetworkingUtils : ISteamInterface
    {
        public static SteamNetworkingUtils Instance;
        private IntPtr _connectionStatusCallback;
        private IntPtr _authenticationStatusCallback;
        private IntPtr _relayNetworkStatusCallback;
        private IntPtr _fakeIpCallback;
        private IntPtr _messagesSessionRequestCallback;
        private IntPtr _messagesSessionFailedCallback;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void NativeStatusCallback(IntPtr status);

        public SteamNetworkingUtils()
        {
            Instance = this;
            InterfaceName = "SteamNetworkingUtils";
            InterfaceVersion = "SteamNetworkingUtils004";
        }

        public IntPtr AllocateMessage(int cbAllocateBuffer)
        {
            return SteamNetworkingMessageStore.AllocateOutbound(cbAllocateBuffer);
        }

        public void InitRelayNetworkAccess()
        {
            Write("InitRelayNetworkAccess");
            if (SteamNetworkingSocketsSerialized.SecureCertMode)
            {
                SteamNetworkingSocketsSerialized.EnsureSecureCertMemoryPatched("InitRelayNetworkAccess");
            }
            else
            {
                SteamNetworkingSocketsSerialized.QueueCurrentNetworkStatusCallbacks();
            }
        }

        public int GetRelayNetworkStatus(IntPtr pDetails)
        {
            Write("GetRelayNetworkStatus");
            if (SteamNetworkingSocketsSerialized.SecureCertMode)
            {
                SteamNetworkingSocketsSerialized.EnsureSecureCertMemoryPatched("GetRelayNetworkStatus");
            }

            SteamRelayNetworkStatus_t data = SteamNetworkingSocketsSerialized.BuildRelayNetworkStatus();
            CallbackManager.AddCallbackResult(data);
            if (pDetails != IntPtr.Zero)
            {
                Marshal.StructureToPtr(data, pDetails, false);
            }
            return (int)ESteamNetworkingAvailability.k_ESteamNetworkingAvailability_Current;
        }

        public float GetLocalPingLocation(IntPtr result)
        {
            Write("GetLocalPingLocation");
            SteamNetworkPingLocation_t pingLocation = Marshal.PtrToStructure<SteamNetworkPingLocation_t>(result);
            pingLocation.m_data = new byte[512];
            pingLocation.m_data[0] = 20;
            Marshal.StructureToPtr(pingLocation, result, false);
            return 2;
        }

        public int EstimatePingTimeBetweenTwoLocations(IntPtr location1, IntPtr location2)
        {
            Write("EstimatePingTimeBetweenTwoLocations");
            return 15;
        }

        public int EstimatePingTimeFromLocalHost(IntPtr remoteLocation)
        {
            Write("EstimatePingTimeFromLocalHost");
            return 15;
        }

        public void ConvertPingLocationToString(IntPtr location, IntPtr pszBuf, int cchBufSize)
        {
            Write("ConvertPingLocationToString");
            NativeStringCache.WriteUtf8Buffer(pszBuf, cchBufSize, "us=8+5");
        }

        public bool ParsePingLocationString(string pszString, IntPtr result)
        {
            Write("ParsePingLocationString");
            return true;
        }

        public bool CheckPingDataUpToDate(float flMaxAgeSeconds)
        {
            Write("CheckPingDataUpToDate");
            if (SteamNetworkingSocketsSerialized.SecureCertMode)
            {
                SteamNetworkingSocketsSerialized.EnsureSecureCertMemoryPatched("CheckPingDataUpToDate");
            }
            else
            {
                SteamNetworkingSocketsSerialized.QueueCurrentNetworkStatusCallbacks();
            }
            return true;
        }

        public int GetPingToDataCenter(SteamNetworkingPOPID popID, IntPtr pViaRelayPoP)
        {
            Write("GetPingToDataCenter");
            if (pViaRelayPoP != IntPtr.Zero)
            {
                Marshal.WriteInt32(pViaRelayPoP, 0);
            }
            return 0;
        }

        //public int GetPingToDataCenter(IntPtr popID)
        //{
        //    Write("GetPingToDataCenter");
        //    return 0;
        //}

        public int GetPOPCount()
        {
            Write("GetPOPCount");
            return 0;
        }

        public int GetDirectPingToPOP(SteamNetworkingPOPID popID)
        {
            Write("GetDirectPingToPOP");
            return 0;
        }

        public int GetPOPList(IntPtr list, int nListSz)
        {
            Write("GetPOPList");
            return 0;
        }

        public SteamNetworkingMicroseconds GetLocalTimestamp()
        {
            Write("GetLocalTimestamp");
            return (long)(DateTime.Now - SteamEmulator.SteamUtils.ActiveTime).Seconds + (SteamNetworkingMicroseconds)24 * 3600 * 30 /* * 1e6 */; 
        }

        public void SetDebugOutputFunction(int eDetailLevel, IntPtr pfnFunc)
        {
            Write("SetDebugOutputFunction");
        }

        public bool SetGlobalConfigValueInt32(int eValue, Int32 val)
        {
            Write($"SetGlobalConfigValueInt32 (Value={eValue}, Data={val})");
            return SetConfigValue(eValue, (int)NetConfigScope.Global, IntPtr.Zero, (int)ESteamNetworkingConfigDataType.Int32, IntPtr.Zero);
        }

        public bool SetGlobalConfigValueFloat(int eValue, float val)
        {
            Write($"SetGlobalConfigValueFloat (Value={eValue}, Data={val})");
            return SetConfigValue(eValue, (int)NetConfigScope.Global, IntPtr.Zero, (int)ESteamNetworkingConfigDataType.Float, IntPtr.Zero);
        }

        public bool SetGlobalConfigValueString(int eValue, string val)
        {
            Write($"SetGlobalConfigValueString (Value={eValue}, Data={val})");
            return SetConfigValue(eValue, (int)NetConfigScope.Global, IntPtr.Zero, (int)ESteamNetworkingConfigDataType.String, IntPtr.Zero);
        }

        public bool SetConnectionConfigValueInt32(HSteamNetConnection hConn, int eValue, Int32 val)
        {
            Write($"SetConnectionConfigValueInt32 (Conn={hConn}, Value={eValue}, Data={val})");
            return SetConfigValue(eValue, (int)NetConfigScope.Connection, new IntPtr((int)hConn), (int)ESteamNetworkingConfigDataType.Int32, IntPtr.Zero);
        }

        public bool SetConnectionConfigValueFloat(HSteamNetConnection hConn, int eValue, float val)
        {
            Write($"SetConnectionConfigValueFloat (Conn={hConn}, Value={eValue}, Data={val})");
            return SetConfigValue(eValue, (int)NetConfigScope.Connection, new IntPtr((int)hConn), (int)ESteamNetworkingConfigDataType.Float, IntPtr.Zero);
        }

        public bool SetGlobalConfigValuePtr(int eValue, IntPtr val)
        {
            Write($"SetGlobalConfigValuePtr (Value={eValue}, Ptr=0x{val.ToInt64():X})");
            return SetPointerConfigValue(eValue, val);
        }

        public bool SetConnectionConfigValueString(HSteamNetConnection hConn, int eValue, string val)
        {
            Write($"SetConnectionConfigValueString (Conn={hConn}, Value={eValue}, Data={val})");
            return SetConfigValue(eValue, (int)NetConfigScope.Connection, new IntPtr((int)hConn), (int)ESteamNetworkingConfigDataType.String, IntPtr.Zero);
        }

        public bool SetConfigValue(int eValue, int eScopeType, IntPtr scopeObj, int eDataType, IntPtr pArg)
        {
            Write($"SetConfigValue (Value={eValue}, Scope={eScopeType}, DataType={eDataType}, Arg=0x{pArg.ToInt64():X})");
            if (eDataType == (int)ESteamNetworkingConfigDataType.Ptr && IsGlobalCallbackConfig(eValue))
            {
                IntPtr callback = IntPtr.Zero;
                if (pArg != IntPtr.Zero)
                {
                    try
                    {
                        // Steamworks passes a pointer to the function pointer for Ptr config values.
                        callback = Marshal.ReadIntPtr(pArg);
                    }
                    catch (Exception ex)
                    {
                        Write($"SetConfigValue pointer read failed: {ex.Message}");
                        return false;
                    }
                }

                return SetPointerConfigValue(eValue, callback);
            }

            if (eValue == (int)ESteamNetworkingConfigValue.Callback_AuthStatusChanged ||
                eValue == (int)ESteamNetworkingConfigValue.Callback_RelayNetworkStatusChanged ||
                eValue == (int)ESteamNetworkingConfigValue.IP_AllowWithoutAuth ||
                eValue == (int)ESteamNetworkingConfigValue.Unencrypted)
            {
                SteamNetworkingSocketsSerialized.QueueCurrentNetworkStatusCallbacks();
            }

            return true;
        }

        public bool SetConfigValueStruct(IntPtr opt, int eScopeType, IntPtr scopeObj)
        {
            Write("SetConfigValueStruct");
            return true;
        }

        internal bool SetGlobalCallback_SteamNetConnectionStatusChanged(IntPtr fnCallback)
        {
            Write($"SetGlobalCallback_SteamNetConnectionStatusChanged (Ptr=0x{fnCallback.ToInt64():X})");
            return SetPointerConfigValue((int)ESteamNetworkingConfigValue.Callback_ConnectionStatusChanged, fnCallback);
        }

        internal bool SetGlobalCallback_SteamNetAuthenticationStatusChanged(IntPtr fnCallback)
        {
            Write($"SetGlobalCallback_SteamNetAuthenticationStatusChanged (Ptr=0x{fnCallback.ToInt64():X})");
            return SetPointerConfigValue((int)ESteamNetworkingConfigValue.Callback_AuthStatusChanged, fnCallback);
        }

        public int GetConfigValue(int eValue, int eScopeType, IntPtr scopeObj, IntPtr pOutDataType, IntPtr pResult, IntPtr cbResult)
        {
            Write("GetConfigValue");
            if (pOutDataType != IntPtr.Zero)
            {
                Marshal.WriteInt32(pOutDataType, 0);
            }
            return default;
        }

        internal bool SetGlobalCallback_MessagesSessionRequest(IntPtr fnCallback)
        {
            Write($"SetGlobalCallback_MessagesSessionRequest (Ptr=0x{fnCallback.ToInt64():X})");
            return SetPointerConfigValue((int)ESteamNetworkingConfigValue.Callback_MessagesSessionRequest, fnCallback);
        }

        internal bool SetGlobalCallback_MessagesSessionFailed(IntPtr fnCallback)
        {
            Write($"SetGlobalCallback_MessagesSessionFailed (Ptr=0x{fnCallback.ToInt64():X})");
            return SetPointerConfigValue((int)ESteamNetworkingConfigValue.Callback_MessagesSessionFailed, fnCallback);
        }

        internal bool SetGlobalCallback_SteamRelayNetworkStatusChanged(IntPtr fnCallback)
        {
            Write($"SetGlobalCallback_SteamRelayNetworkStatusChanged (Ptr=0x{fnCallback.ToInt64():X})");
            return SetPointerConfigValue((int)ESteamNetworkingConfigValue.Callback_RelayNetworkStatusChanged, fnCallback);
        }

        internal void QueueNativeNetworkStatusCallbacks(int delayMs = 0)
        {
            var authStatus = SteamNetworkingSocketsSerialized.BuildAuthenticationStatus();
            var relayStatus = SteamNetworkingSocketsSerialized.BuildRelayNetworkStatus();

            ThreadPool.QueueUserWorkItem(_ =>
            {
                if (delayMs > 0)
                {
                    Thread.Sleep(delayMs);
                }

                InvokeNativeCallback(_authenticationStatusCallback, authStatus, nameof(SteamNetAuthenticationStatus_t));
                InvokeNativeCallback(_relayNetworkStatusCallback, relayStatus, nameof(SteamRelayNetworkStatus_t));
            });
        }

        internal void NotifyConnectionStateChange(uint connection, SteamNetConnectionInfo_t info, ConnectionState oldState)
        {
            var callback = new SteamNetConnectionStatusChangedCallback_t
            {
                m_hConn = connection,
                m_info = info,
                m_eOldState = oldState
            };
            CallbackManager.AddCallback(callback);
            ThreadPool.QueueUserWorkItem(_ => InvokeNativeCallback(_connectionStatusCallback, callback, nameof(SteamNetConnectionStatusChangedCallback_t)));
        }

        internal void NotifyMessagesSessionRequest(ulong remoteSteamId)
        {
            var callback = new SteamNetworkingMessagesSessionRequest_t
            {
                m_identityRemote = SteamNetworkingIdentity_t.FromSteamId(remoteSteamId)
            };
            CallbackManager.AddCallback(callback);
            ThreadPool.QueueUserWorkItem(_ => InvokeNativeCallback(_messagesSessionRequestCallback, callback, nameof(SteamNetworkingMessagesSessionRequest_t)));
        }

        private bool SetPointerConfigValue(int eValue, IntPtr callback)
        {
            switch ((ESteamNetworkingConfigValue)eValue)
            {
                case ESteamNetworkingConfigValue.Callback_ConnectionStatusChanged:
                    _connectionStatusCallback = callback;
                    return true;
                case ESteamNetworkingConfigValue.Callback_AuthStatusChanged:
                    _authenticationStatusCallback = callback;
                    QueueNativeNetworkStatusCallbacks();
                    return true;
                case ESteamNetworkingConfigValue.Callback_RelayNetworkStatusChanged:
                    _relayNetworkStatusCallback = callback;
                    QueueNativeNetworkStatusCallbacks();
                    return true;
                case ESteamNetworkingConfigValue.Callback_FakeIPResult:
                    _fakeIpCallback = callback;
                    return true;
                case ESteamNetworkingConfigValue.Callback_MessagesSessionRequest:
                    _messagesSessionRequestCallback = callback;
                    return true;
                case ESteamNetworkingConfigValue.Callback_MessagesSessionFailed:
                    _messagesSessionFailedCallback = callback;
                    return true;
                default:
                    return true;
            }
        }

        private static bool IsGlobalCallbackConfig(int eValue)
        {
            return eValue == (int)ESteamNetworkingConfigValue.Callback_ConnectionStatusChanged ||
                   eValue == (int)ESteamNetworkingConfigValue.Callback_AuthStatusChanged ||
                   eValue == (int)ESteamNetworkingConfigValue.Callback_RelayNetworkStatusChanged ||
                   eValue == (int)ESteamNetworkingConfigValue.Callback_FakeIPResult ||
                   eValue == (int)ESteamNetworkingConfigValue.Callback_MessagesSessionRequest ||
                   eValue == (int)ESteamNetworkingConfigValue.Callback_MessagesSessionFailed;
        }

        private void InvokeNativeCallback<T>(IntPtr callback, T data, string name) where T : struct
        {
            if (callback == IntPtr.Zero)
            {
                return;
            }

            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));
                Marshal.StructureToPtr(data, ptr, false);
                Marshal.GetDelegateForFunctionPointer<NativeStatusCallback>(callback)(ptr);
                Write($"Native {name} callback invoked");
            }
            catch (Exception ex)
            {
                Write($"Native {name} callback failed: {ex.Message}");
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                {
                    Marshal.DestroyStructure(ptr, typeof(T));
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        public int GetIPv4FakeIPType(uint nIPv4)
        {
            Write($"GetIPv4FakeIPType ({nIPv4})");
            return (int)SteamNetworkingFakeIPType.NotFake;
        }

        public int GetRealIdentityForFakeIP(IntPtr fakeIP, IntPtr pOutRealIdentity)
        {
            Write("GetRealIdentityForFakeIP");
            return (int)EResult.k_EResultNoMatch;
        }

        public IntPtr GetConfigValueInfo(int eValue, IntPtr pOutDataType, IntPtr pOutScope)
        {
            Write("GetConfigValueInfo");
            if (pOutDataType != IntPtr.Zero)
            {
                Marshal.WriteInt32(pOutDataType, 0);
            }
            if (pOutScope != IntPtr.Zero)
            {
                Marshal.WriteInt32(pOutScope, 0);
            }
            return NativeStringCache.ToUtf8Ptr(string.Empty);
        }

        public int GetFirstConfigValue()
        {
            Write("GetFirstConfigValue");
            return 0; //ESteamNetworkingConfigValue.k_ESteamNetworkingConfig_Invalid;
        }

        public void SteamNetworkingIPAddr_ToString(IntPtr addr, IntPtr buf, UIntPtr cbBuf, bool bWithPort)
        {
            Write("SteamNetworkingIPAddr_ToString");
            NativeStringCache.WriteUtf8Buffer(buf, checked((int)cbBuf.ToUInt64()), string.Empty);
        }

        public bool SteamNetworkingIPAddr_ParseString(IntPtr pAddr, string pszStr)
        {
            Write("SteamNetworkingIPAddr_ParseString");
            return false;
        }

        public void SteamNetworkingIdentity_ToString(IntPtr identity, IntPtr buf, UIntPtr cbBuf)
        {
            NativeStringCache.WriteUtf8Buffer(buf, checked((int)cbBuf.ToUInt64()), SteamNetworkingIdentityInterop.Format(identity));
        }

        public bool SteamNetworkingIdentity_ParseString(IntPtr pIdentity, string pszStr)
        {
            if (!SteamNetworkingIdentityInterop.TryParseSteamId(pszStr, out var steamId))
            {
                SteamNetworkingIdentityInterop.Clear(pIdentity);
                return false;
            }
            SteamNetworkingIdentityInterop.WriteSteamId(pIdentity, steamId);
            return true;
        }
    }
}

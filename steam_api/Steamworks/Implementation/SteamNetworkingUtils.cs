using System;
using System.Runtime.InteropServices;
using SKYNET.Callback;
using SKYNET.Managers;

using SteamNetworkingPOPID = System.UInt32;
using SteamNetworkingMicroseconds = System.Int64;
using HSteamNetConnection = System.UInt32;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamNetworkingUtils : ISteamInterface
    {
        public static SteamNetworkingUtils Instance;

        public SteamNetworkingUtils()
        {
            Instance = this;
            InterfaceName = "SteamNetworkingUtils";
            InterfaceVersion = "SteamNetworkingUtils003";
        }

        public IntPtr AllocateMessage(int cbAllocateBuffer)
        {
            Write("AllocateMessage");
            return IntPtr.Zero;
        }

        public void InitRelayNetworkAccess()
        {
            Write("InitRelayNetworkAccess");
        }

        public int GetRelayNetworkStatus(IntPtr pDetails)
        {
            Write("GetRelayNetworkStatus");
            SteamRelayNetworkStatus_t data = new SteamRelayNetworkStatus_t()
            {
                m_eAvail = ESteamNetworkingAvailability.k_ESteamNetworkingAvailability_Current,
                m_bPingMeasurementInProgress = 0,
                m_eAvailAnyRelay = ESteamNetworkingAvailability.k_ESteamNetworkingAvailability_Current,
                m_eAvailNetworkConfig = ESteamNetworkingAvailability.k_ESteamNetworkingAvailability_Current
            };
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
            pingLocation.m_data = 20;
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

        public void ConvertPingLocationToString(IntPtr location, ref string pszBuf, int cchBufSize)
        {
            Write("ConvertPingLocationToString");
            pszBuf = "us=8+5";
        }

        public bool ParsePingLocationString(string pszString, IntPtr result)
        {
            Write("ParsePingLocationString");
            return true;
        }

        public bool CheckPingDataUpToDate(float flMaxAgeSeconds)
        {
            Write("CheckPingDataUpToDate");
            return true;
        }

        public int GetPingToDataCenter(SteamNetworkingPOPID popID, SteamNetworkingPOPID pViaRelayPoP)
        {
            Write("GetPingToDataCenter");
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

        public int GetPOPList(SteamNetworkingPOPID list, int nListSz)
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
            Write("SetGlobalConfigValueInt32");
            return false;
        }

        public bool SetGlobalConfigValueFloat(int eValue, float val)
        {
            Write("SetGlobalConfigValueFloat");
            return false;
        }

        public bool SetGlobalConfigValueString(int eValue, string val)
        {
            Write("SetGlobalConfigValueString");
            return false;
        }

        public bool SetConnectionConfigValueInt32(HSteamNetConnection hConn, int eValue, Int32 val)
        {
            Write("SetConnectionConfigValueInt32");
            return false;
        }

        public bool SetConnectionConfigValueFloat(HSteamNetConnection hConn, int eValue, float val)
        {
            Write("SetConnectionConfigValueFloat");
            return false;
        }

        public bool SetGlobalConfigValuePtr(int eValue, IntPtr val)
        {
            Write("SetGlobalConfigValuePtr");
            return false;
        }

        public bool SetConnectionConfigValueString(HSteamNetConnection hConn, int eValue, string val)
        {
            Write("SetConnectionConfigValueString");
            return false;
        }

        public bool SetConfigValue(int eValue, int eScopeType, IntPtr scopeObj, int eDataType, IntPtr pArg)
        {
            Write("SetConfigValue");
            return false;
        }

        public bool SetConfigValueStruct(IntPtr opt, int eScopeType, IntPtr scopeObj)
        {
            Write("SetConfigValueStruct");
            return false;
        }

        internal bool SetGlobalCallback_SteamNetConnectionStatusChanged(IntPtr fnCallback)
        {
            Write("SetGlobalCallback_SteamNetConnectionStatusChanged");
            return true;
        }

        internal bool SetGlobalCallback_SteamNetAuthenticationStatusChanged(IntPtr fnCallback)
        {
            Write("SetGlobalCallback_SteamNetAuthenticationStatusChanged");
            return true;
        }

        public int GetConfigValue(int eValue, int eScopeType, IntPtr scopeObj, int pOutDataType, IntPtr pResult, IntPtr cbResult)
        {
            Write("GetConfigValue");
            return default;
        }

        internal bool SetGlobalCallback_MessagesSessionRequest(IntPtr fnCallback)
        {
            Write("SetGlobalCallback_MessagesSessionRequest");
            return true;
        }

        internal bool SetGlobalCallback_MessagesSessionFailed(IntPtr fnCallback)
        {
            Write("SetGlobalCallback_MessagesSessionFailed");
            return true;
        }

        internal bool SetGlobalCallback_SteamRelayNetworkStatusChanged(IntPtr fnCallback)
        {
            Write("SetGlobalCallback_SteamRelayNetworkStatusChanged");
            return true;
        }

        public bool GetConfigValueInfo(int eValue, string pOutName, int pOutDataType, int pOutScope, int pOutNextValue)
        {
            Write("GetConfigValueInfo");
            return false;
        }

        public int GetFirstConfigValue()
        {
            Write("GetFirstConfigValue");
            return 0; //ESteamNetworkingConfigValue.k_ESteamNetworkingConfig_Invalid;
        }

        public void SteamNetworkingIPAddr_ToString(IntPtr addr, string buf, IntPtr cbBuf, bool bWithPort)
        {
            Write("SteamNetworkingIPAddr_ToString");
            // TODO
        }

        public bool SteamNetworkingIPAddr_ParseString(IntPtr pAddr, string pszStr)
        {
            Write("SteamNetworkingIPAddr_ParseString");
            return false;
        }

        public void SteamNetworkingIdentity_ToString(IntPtr identity, string buf, IntPtr cbBuf)
        {
            Write("SteamNetworkingIdentity_ToString");
        }

        public bool SteamNetworkingIdentity_ParseString(IntPtr pIdentity, string pszStr)
        {
            Write("SteamNetworkingIdentity_ParseString");
            return false;
        }
    }
}
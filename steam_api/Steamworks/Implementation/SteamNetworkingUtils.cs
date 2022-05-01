using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using System;
using System.Runtime.InteropServices;

using SteamNetworkingPOPID = System.UInt32;
using SteamNetworkingMicroseconds = System.Int64;
using HSteamNetConnection = System.UInt32;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamNetworkingUtils : ISteamInterface
    {
        public SteamNetworkingUtils()
        {
            InterfaceVersion = "SteamNetworkingUtils";
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
            return 0;
        }

        public float GetLocalPingLocation(IntPtr result)
        {
            Write("GetLocalPingLocation");
            return 0;
        }

        public int EstimatePingTimeBetweenTwoLocations(IntPtr location1, IntPtr location2)
        {
            Write("EstimatePingTimeBetweenTwoLocations");
            return 0;
        }

        public int EstimatePingTimeFromLocalHost(IntPtr remoteLocation)
        {
            Write("EstimatePingTimeFromLocalHost");
            return 0;
        }

        public void ConvertPingLocationToString(IntPtr location, string pszBuf, int cchBufSize)
        {
            Write("ConvertPingLocationToString");
        }

        public bool ParsePingLocationString(string pszString, IntPtr result)
        {
            Write("ParsePingLocationString");
            return false;
        }

        public bool CheckPingDataUpToDate(float flMaxAgeSeconds)
        {
            Write("CheckPingDataUpToDate");
            return false;
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
            return 0;
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
            return 0;
        }

        public void SteamNetworkingIPAddr_ToString(IntPtr addr, string buf, IntPtr cbBuf, bool bWithPort)
        {
            Write("SteamNetworkingIPAddr_ToString");
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
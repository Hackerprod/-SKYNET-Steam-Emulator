using System;

using SteamNetworkingPOPID = System.UInt32;
using HSteamNetConnection = System.UInt32;
using SteamNetworkingMicroseconds = System.Int64;

namespace SKYNET.Interface
{
    [Interface("SteamNetworkingUtils003")]
    public class SteamNetworkingUtils003 : ISteamInterface
    {
        public IntPtr AllocateMessage(int cbAllocateBuffer)
        {
            return SteamEmulator.SteamNetworkingUtils.AllocateMessage(cbAllocateBuffer);
        }

        public void InitRelayNetworkAccess()
        {
            SteamEmulator.SteamNetworkingUtils.InitRelayNetworkAccess();
        }

        public int GetRelayNetworkStatus(IntPtr pDetails)
        {
            return SteamEmulator.SteamNetworkingUtils.GetRelayNetworkStatus(pDetails);
        }

        public float GetLocalPingLocation(IntPtr result)
        {
            return SteamEmulator.SteamNetworkingUtils.GetLocalPingLocation(result);
        }

        public int EstimatePingTimeBetweenTwoLocations(IntPtr location1, IntPtr location2)
        {
            return SteamEmulator.SteamNetworkingUtils.EstimatePingTimeBetweenTwoLocations(location1, location2);
        }

        public int EstimatePingTimeFromLocalHost(IntPtr remoteLocation)
        {
            return SteamEmulator.SteamNetworkingUtils.EstimatePingTimeFromLocalHost(remoteLocation);
        }

        public void ConvertPingLocationToString(IntPtr location, string pszBuf, int cchBufSize)
        {
            SteamEmulator.SteamNetworkingUtils.ConvertPingLocationToString(location, pszBuf, cchBufSize);
        }

        public bool ParsePingLocationString(string pszString, IntPtr result)
        {
            return SteamEmulator.SteamNetworkingUtils.ParsePingLocationString(pszString, result);
        }

        public bool CheckPingDataUpToDate(float flMaxAgeSeconds)
        {
            return SteamEmulator.SteamNetworkingUtils.CheckPingDataUpToDate(flMaxAgeSeconds);
        }

        public int GetPingToDataCenter(SteamNetworkingPOPID popID, SteamNetworkingPOPID pViaRelayPoP)
        {
            return SteamEmulator.SteamNetworkingUtils.GetPingToDataCenter(popID, pViaRelayPoP);
        }

        public int GetDirectPingToPOP(SteamNetworkingPOPID popID)
        {
            return SteamEmulator.SteamNetworkingUtils.GetDirectPingToPOP(popID);
        }

        public int GetPOPCount()
        {
            return SteamEmulator.SteamNetworkingUtils.GetPOPCount();
        }

        public int GetPOPList(SteamNetworkingPOPID list, int nListSz)
        {
            return SteamEmulator.SteamNetworkingUtils.GetPOPList(list, nListSz);
        }

        public SteamNetworkingMicroseconds GetLocalTimestamp()
        {
            return SteamEmulator.SteamNetworkingUtils.GetLocalTimestamp();
        }

        public void SetDebugOutputFunction(int eDetailLevel, IntPtr pfnFunc)
        {
            SteamEmulator.SteamNetworkingUtils.SetDebugOutputFunction(eDetailLevel, pfnFunc);
        }

        public bool SetGlobalConfigValueInt32(int eValue, Int32 val)
        {
            return SteamEmulator.SteamNetworkingUtils.SetGlobalConfigValueInt32(eValue, val);
        }

        public bool SetGlobalConfigValueFloat(int eValue, float val)
        {
            return SteamEmulator.SteamNetworkingUtils.SetGlobalConfigValueFloat(eValue, val);
        }

        public bool SetGlobalConfigValueString(int eValue, string val)
        {
            return SteamEmulator.SteamNetworkingUtils.SetGlobalConfigValueString(eValue, val);
        }

        public bool SetGlobalConfigValuePtr(int eValue, IntPtr val)
        {
            return SteamEmulator.SteamNetworkingUtils.SetGlobalConfigValuePtr(eValue, val);
        }

        public bool SetConnectionConfigValueInt32(HSteamNetConnection hConn, int eValue, Int32 val)
        {
            return SteamEmulator.SteamNetworkingUtils.SetConnectionConfigValueInt32(hConn, eValue, val);
        }

        public bool SetConnectionConfigValueFloat(HSteamNetConnection hConn, int eValue, float val)
        {
            return SteamEmulator.SteamNetworkingUtils.SetConnectionConfigValueFloat(hConn, eValue, val);
        }

        public bool SetConnectionConfigValueString(HSteamNetConnection hConn, int eValue, string val)
        {
            return SteamEmulator.SteamNetworkingUtils.SetConnectionConfigValueString(hConn, eValue, val);
        }

        public bool SetGlobalCallback_SteamNetConnectionStatusChanged(IntPtr fnCallback)
        {
            return SteamEmulator.SteamNetworkingUtils.SetGlobalCallback_SteamNetConnectionStatusChanged(fnCallback);
        }

        public bool SetGlobalCallback_SteamNetAuthenticationStatusChanged(IntPtr fnCallback)
        {
            return SteamEmulator.SteamNetworkingUtils.SetGlobalCallback_SteamNetAuthenticationStatusChanged(fnCallback);
        }

        public bool SetGlobalCallback_SteamRelayNetworkStatusChanged(IntPtr fnCallback)
        {
            return SteamEmulator.SteamNetworkingUtils.SetGlobalCallback_SteamRelayNetworkStatusChanged(fnCallback);
        }

        public bool SetGlobalCallback_MessagesSessionRequest(IntPtr fnCallback)
        {
            return SteamEmulator.SteamNetworkingUtils.SetGlobalCallback_MessagesSessionRequest(fnCallback);
        }

        public bool SetGlobalCallback_MessagesSessionFailed(IntPtr fnCallback)
        {
            return SteamEmulator.SteamNetworkingUtils.SetGlobalCallback_MessagesSessionFailed(fnCallback);
        }

        public bool SetConfigValue(int eValue, int eScopeType, IntPtr scopeObj, int eDataType, IntPtr pArg)
        {
            return SteamEmulator.SteamNetworkingUtils.SetConfigValue(eValue, eScopeType, scopeObj, eDataType, pArg);
        }

        public bool SetConfigValueStruct(IntPtr opt, int eScopeType, IntPtr scopeObj)
        {
            return SteamEmulator.SteamNetworkingUtils.SetConfigValueStruct(opt, eScopeType, scopeObj);
        }

        public int GetConfigValue(int eValue, int eScopeType, IntPtr scopeObj, int pOutDataType, IntPtr pResult, IntPtr cbResult)
        {
            return SteamEmulator.SteamNetworkingUtils.GetConfigValue(eValue, eScopeType, scopeObj, pOutDataType, pResult, cbResult);
        }

        public bool GetConfigValueInfo(int eValue, string pOutName, int pOutDataType, int pOutScope, int pOutNextValue)
        {
            return SteamEmulator.SteamNetworkingUtils.GetConfigValueInfo(eValue, pOutName, pOutDataType, pOutScope, pOutNextValue);
        }

        public int GetFirstConfigValue()
        {
            return SteamEmulator.SteamNetworkingUtils.GetFirstConfigValue();
        }

        public void SteamNetworkingIPAddr_ToString(IntPtr addr, string buf, IntPtr cbBuf, bool bWithPort)
        {
            SteamEmulator.SteamNetworkingUtils.SteamNetworkingIPAddr_ToString(addr, buf, cbBuf, bWithPort);
        }

        public bool SteamNetworkingIPAddr_ParseString(IntPtr pAddr, string pszStr)
        {
            return SteamEmulator.SteamNetworkingUtils.SteamNetworkingIPAddr_ParseString(pAddr, pszStr);
        }

        public void SteamNetworkingIdentity_ToString(IntPtr identity, string buf, IntPtr cbBuf)
        {
            SteamEmulator.SteamNetworkingUtils.SteamNetworkingIdentity_ToString(identity, buf, cbBuf);
        }

        public bool SteamNetworkingIdentity_ParseString(IntPtr pIdentity, string pszStr)
        {
            return SteamEmulator.SteamNetworkingUtils.SteamNetworkingIdentity_ParseString(pIdentity, pszStr);
        }
    }
}

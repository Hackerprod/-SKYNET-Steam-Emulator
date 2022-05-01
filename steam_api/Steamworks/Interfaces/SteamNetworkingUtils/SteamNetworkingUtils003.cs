using System;

using SteamNetworkingPOPID = System.UInt32;
using HSteamNetConnection = System.UInt32;
using SteamNetworkingMicroseconds = System.Int64;

namespace SKYNET.Interface
{
    [Interface("SteamNetworkingUtils003")]
    public class SteamNetworkingUtils003 : ISteamInterface
    {
        public IntPtr AllocateMessage(IntPtr _, int cbAllocateBuffer)
        {
            return SteamEmulator.SteamNetworkingUtils.AllocateMessage(cbAllocateBuffer);
        }

        public void InitRelayNetworkAccess(IntPtr _)
        {
            SteamEmulator.SteamNetworkingUtils.InitRelayNetworkAccess();
        }

        public int GetRelayNetworkStatus(IntPtr _, IntPtr pDetails)
        {
            return SteamEmulator.SteamNetworkingUtils.GetRelayNetworkStatus(pDetails);
        }

        public float GetLocalPingLocation(IntPtr _, IntPtr result)
        {
            return SteamEmulator.SteamNetworkingUtils.GetLocalPingLocation(result);
        }

        public int EstimatePingTimeBetweenTwoLocations(IntPtr _, IntPtr location1, IntPtr location2)
        {
            return SteamEmulator.SteamNetworkingUtils.EstimatePingTimeBetweenTwoLocations(location1, location2);
        }

        public int EstimatePingTimeFromLocalHost(IntPtr _, IntPtr remoteLocation)
        {
            return SteamEmulator.SteamNetworkingUtils.EstimatePingTimeFromLocalHost(remoteLocation);
        }

        public void ConvertPingLocationToString(IntPtr _, IntPtr location, string pszBuf, int cchBufSize)
        {
            SteamEmulator.SteamNetworkingUtils.ConvertPingLocationToString(location, pszBuf, cchBufSize);
        }

        public bool ParsePingLocationString(IntPtr _, string pszString, IntPtr result)
        {
            return SteamEmulator.SteamNetworkingUtils.ParsePingLocationString(pszString, result);
        }

        public bool CheckPingDataUpToDate(IntPtr _, float flMaxAgeSeconds)
        {
            return SteamEmulator.SteamNetworkingUtils.CheckPingDataUpToDate(flMaxAgeSeconds);
        }

        public int GetPingToDataCenter(IntPtr _, SteamNetworkingPOPID popID, SteamNetworkingPOPID pViaRelayPoP)
        {
            return SteamEmulator.SteamNetworkingUtils.GetPingToDataCenter(popID, pViaRelayPoP);
        }

        public int GetDirectPingToPOP(IntPtr _, SteamNetworkingPOPID popID)
        {
            return SteamEmulator.SteamNetworkingUtils.GetDirectPingToPOP(popID);
        }

        public int GetPOPCount(IntPtr _)
        {
            return SteamEmulator.SteamNetworkingUtils.GetPOPCount();
        }

        public int GetPOPList(IntPtr _, SteamNetworkingPOPID list, int nListSz)
        {
            return SteamEmulator.SteamNetworkingUtils.GetPOPList(list, nListSz);
        }

        public SteamNetworkingMicroseconds GetLocalTimestamp(IntPtr _)
        {
            return SteamEmulator.SteamNetworkingUtils.GetLocalTimestamp();
        }

        public void SetDebugOutputFunction(IntPtr _, int eDetailLevel, IntPtr pfnFunc)
        {
            SteamEmulator.SteamNetworkingUtils.SetDebugOutputFunction(eDetailLevel, pfnFunc);
        }

        public bool SetGlobalConfigValueInt32(IntPtr _, int eValue, Int32 val)
        {
            return SteamEmulator.SteamNetworkingUtils.SetGlobalConfigValueInt32(eValue, val);
        }

        public bool SetGlobalConfigValueFloat(IntPtr _, int eValue, float val)
        {
            return SteamEmulator.SteamNetworkingUtils.SetGlobalConfigValueFloat(eValue, val);
        }

        public bool SetGlobalConfigValueString(IntPtr _, int eValue, string val)
        {
            return SteamEmulator.SteamNetworkingUtils.SetGlobalConfigValueString(eValue, val);
        }

        public bool SetGlobalConfigValuePtr(IntPtr _, int eValue, IntPtr val)
        {
            return SteamEmulator.SteamNetworkingUtils.SetGlobalConfigValuePtr(eValue, val);
        }

        public bool SetConnectionConfigValueInt32(IntPtr _, HSteamNetConnection hConn, int eValue, Int32 val)
        {
            return SteamEmulator.SteamNetworkingUtils.SetConnectionConfigValueInt32(hConn, eValue, val);
        }

        public bool SetConnectionConfigValueFloat(IntPtr _, HSteamNetConnection hConn, int eValue, float val)
        {
            return SteamEmulator.SteamNetworkingUtils.SetConnectionConfigValueFloat(hConn, eValue, val);
        }

        public bool SetConnectionConfigValueString(IntPtr _, HSteamNetConnection hConn, int eValue, string val)
        {
            return SteamEmulator.SteamNetworkingUtils.SetConnectionConfigValueString(hConn, eValue, val);
        }

        public bool SetGlobalCallback_SteamNetConnectionStatusChanged(IntPtr _, IntPtr fnCallback)
        {
            return SteamEmulator.SteamNetworkingUtils.SetGlobalCallback_SteamNetConnectionStatusChanged(fnCallback);
        }

        public bool SetGlobalCallback_SteamNetAuthenticationStatusChanged(IntPtr _, IntPtr fnCallback)
        {
            return SteamEmulator.SteamNetworkingUtils.SetGlobalCallback_SteamNetAuthenticationStatusChanged(fnCallback);
        }

        public bool SetGlobalCallback_SteamRelayNetworkStatusChanged(IntPtr _, IntPtr fnCallback)
        {
            return SteamEmulator.SteamNetworkingUtils.SetGlobalCallback_SteamRelayNetworkStatusChanged(fnCallback);
        }

        public bool SetGlobalCallback_MessagesSessionRequest(IntPtr _, IntPtr fnCallback)
        {
            return SteamEmulator.SteamNetworkingUtils.SetGlobalCallback_MessagesSessionRequest(fnCallback);
        }

        public bool SetGlobalCallback_MessagesSessionFailed(IntPtr _, IntPtr fnCallback)
        {
            return SteamEmulator.SteamNetworkingUtils.SetGlobalCallback_MessagesSessionFailed(fnCallback);
        }

        public bool SetConfigValue(IntPtr _, int eValue, int eScopeType, IntPtr scopeObj, int eDataType, IntPtr pArg)
        {
            return SteamEmulator.SteamNetworkingUtils.SetConfigValue(eValue, eScopeType, scopeObj, eDataType, pArg);
        }

        public bool SetConfigValueStruct(IntPtr _, IntPtr opt, int eScopeType, IntPtr scopeObj)
        {
            return SteamEmulator.SteamNetworkingUtils.SetConfigValueStruct(opt, eScopeType, scopeObj);
        }

        public int GetConfigValue(IntPtr _, int eValue, int eScopeType, IntPtr scopeObj, int pOutDataType, IntPtr pResult, IntPtr cbResult)
        {
            return SteamEmulator.SteamNetworkingUtils.GetConfigValue(eValue, eScopeType, scopeObj, pOutDataType, pResult, cbResult);
        }

        public bool GetConfigValueInfo(IntPtr _, int eValue, string pOutName, int pOutDataType, int pOutScope, int pOutNextValue)
        {
            return SteamEmulator.SteamNetworkingUtils.GetConfigValueInfo(eValue, pOutName, pOutDataType, pOutScope, pOutNextValue);
        }

        public int GetFirstConfigValue(IntPtr _)
        {
            return SteamEmulator.SteamNetworkingUtils.GetFirstConfigValue();
        }

        public void SteamNetworkingIPAddr_ToString(IntPtr _, IntPtr addr, string buf, IntPtr cbBuf, bool bWithPort)
        {
            SteamEmulator.SteamNetworkingUtils.SteamNetworkingIPAddr_ToString(addr, buf, cbBuf, bWithPort);
        }

        public bool SteamNetworkingIPAddr_ParseString(IntPtr _, IntPtr pAddr, string pszStr)
        {
            return SteamEmulator.SteamNetworkingUtils.SteamNetworkingIPAddr_ParseString(pAddr, pszStr);
        }

        public void SteamNetworkingIdentity_ToString(IntPtr _, IntPtr identity, string buf, IntPtr cbBuf)
        {
            SteamEmulator.SteamNetworkingUtils.SteamNetworkingIdentity_ToString(identity, buf, cbBuf);
        }

        public bool SteamNetworkingIdentity_ParseString(IntPtr _, IntPtr pIdentity, string pszStr)
        {
            return SteamEmulator.SteamNetworkingUtils.SteamNetworkingIdentity_ParseString(pIdentity, pszStr);
        }
    }
}

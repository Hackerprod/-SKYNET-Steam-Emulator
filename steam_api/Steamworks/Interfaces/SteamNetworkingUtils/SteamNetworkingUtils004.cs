using System;
using SKYNET.Steamworks;

using SteamNetworkingPOPID = System.UInt32;
using SteamNetworkingMicroseconds = System.Int64;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamNetworkingUtils004")]
    public class SteamNetworkingUtils004 : ISteamInterface
    {
        public IntPtr AllocateMessage(IntPtr _, int cbAllocateBuffer)
        {
            return SteamEmulator.SteamNetworkingUtils.AllocateMessage(cbAllocateBuffer);
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

        public void ConvertPingLocationToString(IntPtr _, IntPtr location, IntPtr pszBuf, int cchBufSize)
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

        public int GetPingToDataCenter(IntPtr _, SteamNetworkingPOPID popID, IntPtr pViaRelayPoP)
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

        public int GetPOPList(IntPtr _, IntPtr list, int nListSz)
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

        public int GetIPv4FakeIPType(IntPtr _, uint nIPv4)
        {
            return SteamEmulator.SteamNetworkingUtils.GetIPv4FakeIPType(nIPv4);
        }

        public int GetRealIdentityForFakeIP(IntPtr _, IntPtr fakeIP, IntPtr pOutRealIdentity)
        {
            return SteamEmulator.SteamNetworkingUtils.GetRealIdentityForFakeIP(fakeIP, pOutRealIdentity);
        }

        public bool SetConfigValue(IntPtr _, int eValue, int eScopeType, IntPtr scopeObj, int eDataType, IntPtr pArg)
        {
            return SteamEmulator.SteamNetworkingUtils.SetConfigValue(eValue, eScopeType, scopeObj, eDataType, pArg);
        }

        public int GetConfigValue(IntPtr _, int eValue, int eScopeType, IntPtr scopeObj, IntPtr pOutDataType, IntPtr pResult, IntPtr cbResult)
        {
            return SteamEmulator.SteamNetworkingUtils.GetConfigValue(eValue, eScopeType, scopeObj, pOutDataType, pResult, cbResult);
        }

        public IntPtr GetConfigValueInfo(IntPtr _, int eValue, IntPtr pOutDataType, IntPtr pOutScope)
        {
            return SteamEmulator.SteamNetworkingUtils.GetConfigValueInfo(eValue, pOutDataType, pOutScope);
        }

        public int IterateGenericEditableConfigValues(IntPtr _, int eCurrent, bool bEnumerateDevVars)
        {
            return SteamEmulator.SteamNetworkingUtils.GetFirstConfigValue();
        }

        public void SteamNetworkingIPAddr_ToString(IntPtr _, IntPtr addr, IntPtr buf, UIntPtr cbBuf, bool bWithPort)
        {
            SteamEmulator.SteamNetworkingUtils.SteamNetworkingIPAddr_ToString(addr, buf, cbBuf, bWithPort);
        }

        public bool SteamNetworkingIPAddr_ParseString(IntPtr _, IntPtr pAddr, string pszStr)
        {
            return SteamEmulator.SteamNetworkingUtils.SteamNetworkingIPAddr_ParseString(pAddr, pszStr);
        }

        public int SteamNetworkingIPAddr_GetFakeIPType(IntPtr _, IntPtr addr)
        {
            return (int)SteamNetworkingFakeIPType.NotFake;
        }

        public void SteamNetworkingIdentity_ToString(IntPtr _, IntPtr identity, IntPtr buf, UIntPtr cbBuf)
        {
            SteamEmulator.SteamNetworkingUtils.SteamNetworkingIdentity_ToString(identity, buf, cbBuf);
        }

        public bool SteamNetworkingIdentity_ParseString(IntPtr _, IntPtr pIdentity, string pszStr)
        {
            return SteamEmulator.SteamNetworkingUtils.SteamNetworkingIdentity_ParseString(pIdentity, pszStr);
        }
    }
}

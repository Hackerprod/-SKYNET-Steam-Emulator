using Core.Interface;
using SKYNET.Interface;
using System;

//[Map("SteamNetworkingUtils")]
public class SteamNetworkingUtils : IBaseInterface, ISteamNetworkingUtils
{
    public IntPtr AllocateMessage(int cbAllocateBuffer)
    {
        return IntPtr.Zero;
    }

    public void InitRelayNetworkAccess()
    {
        //
    }

    public ESteamNetworkingAvailability GetRelayNetworkStatus(SteamRelayNetworkStatus_t pDetails)
    {
        return default;
    }

    public float GetLocalPingLocation(SteamNetworkPingLocation_t result)
    {
        return default;
    }

    public int EstimatePingTimeBetweenTwoLocations(SteamNetworkPingLocation_t location1, SteamNetworkPingLocation_t location2)
    {
        return 0;
    }

    public int EstimatePingTimeFromLocalHost(SteamNetworkPingLocation_t remoteLocation)
    {
        return 0;
    }

    public void ConvertPingLocationToString(SteamNetworkPingLocation_t location, char pszBuf, int cchBufSize)
    {
        //
    }

    public bool ParsePingLocationString(char pszString, SteamNetworkPingLocation_t result)
    {
        return false;
    }

    public bool CheckPingDataUpToDate(float flMaxAgeSeconds)
    {
        return false;
    }

    public int GetPingToDataCenter(IntPtr popID, IntPtr pViaRelayPoP)
    {
        return 0;
    }

    public int GetDirectPingToPOP(IntPtr popID)
    {
        return 0;
    }

    public int GetPOPCount()
    {
        return 0;
    }

    public int GetPOPList(IntPtr list, int nListSz)
    {
        return 0;
    }

    public uint GetLocalTimestamp()
    {
        return 0;
    }

    public void SetDebugOutputFunction(ESteamNetworkingSocketsDebugOutputType eDetailLevel, IntPtr pfnFunc)
    {
        //
    }

    public bool SetGlobalConfigValueInt32(int eValue, uint val)
    {
        return false;
    }

    public bool SetGlobalConfigValueFloat(int eValue, float val)
    {
        return false;
    }

    public bool SetGlobalConfigValueString(int eValue, char val)
    {
        return false;
    }

    public bool SetConnectionConfigValueInt32(uint hConn, int eValue, uint val)
    {
        return false;
    }

    public bool SetConnectionConfigValueFloat(uint hConn, int eValue, float val)
    {
        return false;
    }

    public bool SetConnectionConfigValueString(uint hConn, int eValue, char val)
    {
        return false;
    }

    public bool SetConfigValue(int eValue, IntPtr eScopeType, IntPtr scopeObj, IntPtr eDataType, IntPtr pArg)
    {
        return false;
    }

    public bool SetConfigValueStruct(IntPtr opt, IntPtr eScopeType, IntPtr scopeObj)
    {
        return false;
    }

    public ESteamNetworkingGetConfigValueResult GetConfigValue(int eValue, IntPtr eScopeType, IntPtr scopeObj, IntPtr pOutDataType, IntPtr pResult, IntPtr cbResult)
    {
        return default;
    }

    public bool GetConfigValueInfo(int eValue, char pOutName, IntPtr pOutDataType, IntPtr pOutScope, int pOutNextValue)
    {
        return false;
    }

    public int GetFirstConfigValue()
    {
        return 0;
    }

    public void SteamNetworkingIPAddr_ToString(IntPtr addr, char buf, IntPtr cbBuf, bool bWithPort)
    {
        //
    }

    public bool SteamNetworkingIPAddr_ParseString(IntPtr pAddr, char pszStr)
    {
        return false;
    }

    public void SteamNetworkingIdentity_ToString(IntPtr identity, char buf, IntPtr cbBuf)
    {
        //
    }

    public bool SteamNetworkingIdentity_ParseString(IntPtr pIdentity, char pszStr)
    {
        return false;
    }

}
using SKYNET;
using SKYNET.Helper;
using SKYNET.Steamworks;
using System;

public class SteamNetworkingUtils : ISteamInterface
{
    public IntPtr MemoryAddress { get; set; }
    public string InterfaceVersion { get; set; }

    public SteamNetworkingUtils()
    {
        InterfaceVersion = "SteamNetworkingUtils";
    }

    public IntPtr AllocateMessage(int cbAllocateBuffer)
    {
        Write("AllocateMessage");
        return IntPtr.Zero;
    }

    public void InitRelayNetworkAccess(IntPtr _)
    {
        Write("InitRelayNetworkAccess");
    }

    public ESteamNetworkingAvailability GetRelayNetworkStatus(SteamRelayNetworkStatus_t pDetails)
    {
        Write("GetRelayNetworkStatus");
        return default;
    }

    public float GetLocalPingLocation(SteamNetworkPingLocation_t result)
    {
        Write("GetLocalPingLocation");
        return default;
    }

    public int EstimatePingTimeBetweenTwoLocations(SteamNetworkPingLocation_t location1, SteamNetworkPingLocation_t location2)
    {
        Write("EstimatePingTimeBetweenTwoLocations");
        return 0;
    }

    public int EstimatePingTimeFromLocalHost(SteamNetworkPingLocation_t remoteLocation)
    {
        Write("EstimatePingTimeFromLocalHost");
        return 0;
    }

    public void ConvertPingLocationToString(SteamNetworkPingLocation_t location, char pszBuf, int cchBufSize)
    {
        Write("ConvertPingLocationToString");
    }

    public bool ParsePingLocationString(char pszString, SteamNetworkPingLocation_t result)
    {
        Write("ParsePingLocationString");
        return false;
    }

    public bool CheckPingDataUpToDate(float flMaxAgeSeconds)
    {
        Write("CheckPingDataUpToDate");
        return false;
    }

    public int GetPingToDataCenter(IntPtr popID, IntPtr pViaRelayPoP)
    {
        Write("GetPingToDataCenter");
        return 0;
    }

    //public int GetPingToDataCenter(IntPtr popID)
    //{
    //    Write("GetPingToDataCenter");
    //    return 0;
    //}

    public int GetPOPCount(IntPtr _)
    {
        Write("GetPOPCount");
        return 0;
    }

    public int GetPOPList(IntPtr list, int nListSz)
    {
        Write("GetPOPList");
        return 0;
    }

    public uint GetLocalTimestamp(IntPtr _)
    {
        Write("GetLocalTimestamp");
        return 0;
    }

    public void SetDebugOutputFunction(ESteamNetworkingSocketsDebugOutputType eDetailLevel, IntPtr pfnFunc)
    {
        Write("SetDebugOutputFunction");
    }

    public bool SetGlobalConfigValueInt32(int eValue, uint val)
    {
        Write("SetGlobalConfigValueInt32");
        return false;
    }

    public bool SetGlobalConfigValueFloat(int eValue, float val)
    {
        Write("SetGlobalConfigValueFloat");
        return false;
    }

    public bool SetGlobalConfigValueString(int eValue, char val)
    {
        Write("SetGlobalConfigValueString");
        return false;
    }

    public bool SetConnectionConfigValueInt32(uint hConn, int eValue, uint val)
    {
        Write("SetConnectionConfigValueInt32");
        return false;
    }

    public bool SetConnectionConfigValueFloat(uint hConn, int eValue, float val)
    {
        Write("SetConnectionConfigValueFloat");
        return false;
    }

    public bool SetConnectionConfigValueString(uint hConn, int eValue, char val)
    {
        Write("SetConnectionConfigValueString");
        return false;
    }

    public bool SetConfigValue(int eValue, IntPtr eScopeType, IntPtr scopeObj, IntPtr eDataType, IntPtr pArg)
    {
        Write("SetConfigValue");
        return false;
    }

    public bool SetConfigValueStruct(IntPtr opt, IntPtr eScopeType, IntPtr scopeObj)
    {
        Write("SetConfigValueStruct");
        return false;
    }

    public ESteamNetworkingGetConfigValueResult GetConfigValue(int eValue, IntPtr eScopeType, IntPtr scopeObj, IntPtr pOutDataType, IntPtr pResult, IntPtr cbResult)
    {
        Write("GetConfigValue");
        return default;
    }

    public bool GetConfigValueInfo(int eValue, char pOutName, IntPtr pOutDataType, IntPtr pOutScope, int pOutNextValue)
    {
        Write("GetConfigValueInfo");
        return false;
    }

    public int GetFirstConfigValue(IntPtr _)
    {
        Write("GetFirstConfigValue");
        return 0;
    }

    public void SteamNetworkingIPAddr_ToString(IntPtr addr, char buf, IntPtr cbBuf, bool bWithPort)
    {
        Write("SteamNetworkingIPAddr_ToString");
    }

    public bool SteamNetworkingIPAddr_ParseString(IntPtr pAddr, char pszStr)
    {
        Write("SteamNetworkingIPAddr_ParseString");
        return false;
    }

    public void SteamNetworkingIdentity_ToString(IntPtr identity, char buf, IntPtr cbBuf)
    {
        Write("SteamNetworkingIdentity_ToString");
    }

    public bool SteamNetworkingIdentity_ParseString(IntPtr pIdentity, char pszStr)
    {
        Write("SteamNetworkingIdentity_ParseString");
        return false;
    }

    private void Write(string v)
    {
        SteamEmulator.Write(InterfaceVersion, v);
    }
}
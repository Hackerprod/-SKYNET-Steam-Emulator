using SKYNET;
using SKYNET.Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class SteamAPI_ISteamNetworkingUtils : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamNetworkingUtils_AllocateMessage(int cbAllocateBuffer)
    {
        Write("SteamAPI_ISteamNetworkingUtils_AllocateMessage");
        return SteamEmulator.SteamNetworkingUtils.AllocateMessage(cbAllocateBuffer);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamNetworkingUtils_InitRelayNetworkAccess()
    {
        Write("SteamAPI_ISteamNetworkingUtils_InitRelayNetworkAccess");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ESteamNetworkingAvailability SteamAPI_ISteamNetworkingUtils_GetRelayNetworkStatus(SteamRelayNetworkStatus_t pDetails)
    {
        Write("SteamAPI_ISteamNetworkingUtils_GetRelayNetworkStatus");
        return SteamEmulator.SteamNetworkingUtils.GetRelayNetworkStatus(pDetails);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static float SteamAPI_ISteamNetworkingUtils_GetLocalPingLocation(SteamNetworkPingLocation_t result)
    {
        Write("SteamAPI_ISteamNetworkingUtils_GetLocalPingLocation");
        return SteamEmulator.SteamNetworkingUtils.GetLocalPingLocation(result);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamNetworkingUtils_EstimatePingTimeBetweenTwoLocations(SteamNetworkPingLocation_t location1, SteamNetworkPingLocation_t location2)
    {
        Write("SteamAPI_ISteamNetworkingUtils_EstimatePingTimeBetweenTwoLocations");
        return SteamEmulator.SteamNetworkingUtils.EstimatePingTimeBetweenTwoLocations(location1, location2);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamNetworkingUtils_EstimatePingTimeFromLocalHost(SteamNetworkPingLocation_t remoteLocation)
    {
        Write("SteamAPI_ISteamNetworkingUtils_EstimatePingTimeFromLocalHost");
        return SteamEmulator.SteamNetworkingUtils.EstimatePingTimeFromLocalHost(remoteLocation);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamNetworkingUtils_ConvertPingLocationToString(SteamNetworkPingLocation_t location, char pszBuf, int cchBufSize)
    {
        Write("SteamAPI_ISteamNetworkingUtils_ConvertPingLocationToString");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingUtils_ParsePingLocationString(char pszString, SteamNetworkPingLocation_t result)
    {
        Write("SteamAPI_ISteamNetworkingUtils_ParsePingLocationString");
        return SteamEmulator.SteamNetworkingUtils.ParsePingLocationString(pszString, result);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingUtils_CheckPingDataUpToDate(float flMaxAgeSeconds)
    {
        Write("SteamAPI_ISteamNetworkingUtils_CheckPingDataUpToDate");
        return SteamEmulator.SteamNetworkingUtils.CheckPingDataUpToDate(flMaxAgeSeconds);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamNetworkingUtils_GetPingToDataCenter(IntPtr popID, IntPtr pViaRelayPoP)
    {
        Write("SteamAPI_ISteamNetworkingUtils_GetPingToDataCenter");
        return SteamEmulator.SteamNetworkingUtils.GetPingToDataCenter(popID, pViaRelayPoP);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamNetworkingUtils_GetDirectPingToPOP(IntPtr popID)
    {
        Write("SteamAPI_ISteamNetworkingUtils_GetDirectPingToPOP");
        return SteamEmulator.SteamNetworkingUtils.GetDirectPingToPOP(popID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamNetworkingUtils_GetPOPCount(IntPtr _)
    {
        Write("SteamAPI_ISteamNetworkingUtils_GetPOPCount");
        return SteamEmulator.SteamNetworkingUtils.GetPOPCount(_);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamNetworkingUtils_GetPOPList(IntPtr list, int nListSz)
    {
        Write("SteamAPI_ISteamNetworkingUtils_GetPOPList");
        return SteamEmulator.SteamNetworkingUtils.GetPOPList(list, nListSz);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamNetworkingUtils_GetLocalTimestamp(IntPtr _)
    {
        Write("SteamAPI_ISteamNetworkingUtils_GetLocalTimestamp");
        return SteamEmulator.SteamNetworkingUtils.GetLocalTimestamp(_);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamNetworkingUtils_SetDebugOutputFunction(ESteamNetworkingSocketsDebugOutputType eDetailLevel, IntPtr pfnFunc)
    {
        Write("SteamAPI_ISteamNetworkingUtils_SetDebugOutputFunction");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingUtils_SetGlobalConfigValueInt32(int eValue, uint val)
    {
        Write("SteamAPI_ISteamNetworkingUtils_SetGlobalConfigValueInt32");
        return SteamEmulator.SteamNetworkingUtils.SetGlobalConfigValueInt32(eValue, val);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingUtils_SetGlobalConfigValueFloat(int eValue, float val)
    {
        Write("SteamAPI_ISteamNetworkingUtils_SetGlobalConfigValueFloat");
        return SteamEmulator.SteamNetworkingUtils.SetGlobalConfigValueFloat(eValue, val);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingUtils_SetGlobalConfigValueString(int eValue, char val)
    {
        Write("SteamAPI_ISteamNetworkingUtils_SetGlobalConfigValueString");
        return SteamEmulator.SteamNetworkingUtils.SetGlobalConfigValueString(eValue, val);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueInt32(uint hConn, int eValue, uint val)
    {
        Write("SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueInt32");
        return SteamEmulator.SteamNetworkingUtils.SetConnectionConfigValueInt32(hConn, eValue, val);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueFloat(uint hConn, int eValue, float val)
    {
        Write("SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueFloat");
        return SteamEmulator.SteamNetworkingUtils.SetConnectionConfigValueFloat(hConn, eValue, val);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueString(uint hConn, int eValue, char val)
    {
        Write("SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueString");
        return SteamEmulator.SteamNetworkingUtils.SetConnectionConfigValueString(hConn, eValue, val);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingUtils_SetConfigValue(int eValue, IntPtr eScopeType, IntPtr scopeObj, IntPtr eDataType, IntPtr pArg)
    {
        Write("SteamAPI_ISteamNetworkingUtils_SetConfigValue");
        return SteamEmulator.SteamNetworkingUtils.SetConfigValue(eValue, eScopeType, scopeObj, eDataType, pArg);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingUtils_SetConfigValueStruct(IntPtr opt, IntPtr eScopeType, IntPtr scopeObj)
    {
        Write("SteamAPI_ISteamNetworkingUtils_SetConfigValueStruct");
        return SteamEmulator.SteamNetworkingUtils.SetConfigValueStruct(opt, eScopeType, scopeObj);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ESteamNetworkingGetConfigValueResult SteamAPI_ISteamNetworkingUtils_GetConfigValue(int eValue, IntPtr eScopeType, IntPtr scopeObj, IntPtr pOutDataType, IntPtr pResult, IntPtr cbResult)
    {
        Write("SteamAPI_ISteamNetworkingUtils_GetConfigValue");
        return SteamEmulator.SteamNetworkingUtils.GetConfigValue(eValue, eScopeType, scopeObj, pOutDataType, pResult, cbResult);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingUtils_GetConfigValueInfo(int eValue, char pOutName, IntPtr pOutDataType, IntPtr pOutScope, int pOutNextValue)
    {
        Write("SteamAPI_ISteamNetworkingUtils_GetConfigValueInfo");
        return SteamEmulator.SteamNetworkingUtils.GetConfigValueInfo(eValue, pOutName, pOutDataType, pOutScope, pOutNextValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamNetworkingUtils_GetFirstConfigValue(IntPtr _)
    {
        Write("SteamAPI_ISteamNetworkingUtils_GetFirstConfigValue");
        return SteamEmulator.SteamNetworkingUtils.GetFirstConfigValue(_);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamNetworkingUtils_SteamNetworkingIPAddr_ToString(IntPtr addr, char buf, IntPtr cbBuf, bool bWithPort)
    {
        Write("SteamAPI_ISteamNetworkingUtils_SteamNetworkingIPAddr_ToString");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingUtils_SteamNetworkingIPAddr_ParseString(IntPtr pAddr, char pszStr)
    {
        Write("SteamAPI_ISteamNetworkingUtils_SteamNetworkingIPAddr_ParseString");
        return SteamEmulator.SteamNetworkingUtils.SteamNetworkingIPAddr_ParseString(pAddr, pszStr);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamNetworkingUtils_SteamNetworkingIdentity_ToString(IntPtr identity, char buf, IntPtr cbBuf)
    {
        Write("SteamAPI_ISteamNetworkingUtils_SteamNetworkingIdentity_ToString");
        //
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingUtils_SteamNetworkingIdentity_ParseString(IntPtr pIdentity, char pszStr)
    {
        Write("SteamAPI_ISteamNetworkingUtils_SteamNetworkingIdentity_ParseString");
        return SteamEmulator.SteamNetworkingUtils.SteamNetworkingIdentity_ParseString(pIdentity, pszStr);
    }

}


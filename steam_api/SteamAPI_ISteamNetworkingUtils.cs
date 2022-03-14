using SKYNET.Interface;
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
        return SteamClient.SteamNetworkingUtils.AllocateMessage(cbAllocateBuffer);
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
        return SteamClient.SteamNetworkingUtils.GetRelayNetworkStatus(pDetails);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static float SteamAPI_ISteamNetworkingUtils_GetLocalPingLocation(SteamNetworkPingLocation_t result)
    {
        Write("SteamAPI_ISteamNetworkingUtils_GetLocalPingLocation");
        return SteamClient.SteamNetworkingUtils.GetLocalPingLocation(result);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamNetworkingUtils_EstimatePingTimeBetweenTwoLocations(SteamNetworkPingLocation_t location1, SteamNetworkPingLocation_t location2)
    {
        Write("SteamAPI_ISteamNetworkingUtils_EstimatePingTimeBetweenTwoLocations");
        return SteamClient.SteamNetworkingUtils.EstimatePingTimeBetweenTwoLocations(location1, location2);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamNetworkingUtils_EstimatePingTimeFromLocalHost(SteamNetworkPingLocation_t remoteLocation)
    {
        Write("SteamAPI_ISteamNetworkingUtils_EstimatePingTimeFromLocalHost");
        return SteamClient.SteamNetworkingUtils.EstimatePingTimeFromLocalHost(remoteLocation);
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
        return SteamClient.SteamNetworkingUtils.ParsePingLocationString(pszString, result);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingUtils_CheckPingDataUpToDate(float flMaxAgeSeconds)
    {
        Write("SteamAPI_ISteamNetworkingUtils_CheckPingDataUpToDate");
        return SteamClient.SteamNetworkingUtils.CheckPingDataUpToDate(flMaxAgeSeconds);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamNetworkingUtils_GetPingToDataCenter(IntPtr popID, IntPtr pViaRelayPoP)
    {
        Write("SteamAPI_ISteamNetworkingUtils_GetPingToDataCenter");
        return SteamClient.SteamNetworkingUtils.GetPingToDataCenter(popID, pViaRelayPoP);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamNetworkingUtils_GetDirectPingToPOP(IntPtr popID)
    {
        Write("SteamAPI_ISteamNetworkingUtils_GetDirectPingToPOP");
        return SteamClient.SteamNetworkingUtils.GetDirectPingToPOP(popID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamNetworkingUtils_GetPOPCount()
    {
        Write("SteamAPI_ISteamNetworkingUtils_GetPOPCount");
        return SteamClient.SteamNetworkingUtils.GetPOPCount();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamNetworkingUtils_GetPOPList(IntPtr list, int nListSz)
    {
        Write("SteamAPI_ISteamNetworkingUtils_GetPOPList");
        return SteamClient.SteamNetworkingUtils.GetPOPList(list, nListSz);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamNetworkingUtils_GetLocalTimestamp()
    {
        Write("SteamAPI_ISteamNetworkingUtils_GetLocalTimestamp");
        return SteamClient.SteamNetworkingUtils.GetLocalTimestamp();
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
        return SteamClient.SteamNetworkingUtils.SetGlobalConfigValueInt32(eValue, val);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingUtils_SetGlobalConfigValueFloat(int eValue, float val)
    {
        Write("SteamAPI_ISteamNetworkingUtils_SetGlobalConfigValueFloat");
        return SteamClient.SteamNetworkingUtils.SetGlobalConfigValueFloat(eValue, val);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingUtils_SetGlobalConfigValueString(int eValue, char val)
    {
        Write("SteamAPI_ISteamNetworkingUtils_SetGlobalConfigValueString");
        return SteamClient.SteamNetworkingUtils.SetGlobalConfigValueString(eValue, val);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueInt32(uint hConn, int eValue, uint val)
    {
        Write("SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueInt32");
        return SteamClient.SteamNetworkingUtils.SetConnectionConfigValueInt32(hConn, eValue, val);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueFloat(uint hConn, int eValue, float val)
    {
        Write("SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueFloat");
        return SteamClient.SteamNetworkingUtils.SetConnectionConfigValueFloat(hConn, eValue, val);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueString(uint hConn, int eValue, char val)
    {
        Write("SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueString");
        return SteamClient.SteamNetworkingUtils.SetConnectionConfigValueString(hConn, eValue, val);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingUtils_SetConfigValue(int eValue, IntPtr eScopeType, IntPtr scopeObj, IntPtr eDataType, IntPtr pArg)
    {
        Write("SteamAPI_ISteamNetworkingUtils_SetConfigValue");
        return SteamClient.SteamNetworkingUtils.SetConfigValue(eValue, eScopeType, scopeObj, eDataType, pArg);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingUtils_SetConfigValueStruct(IntPtr opt, IntPtr eScopeType, IntPtr scopeObj)
    {
        Write("SteamAPI_ISteamNetworkingUtils_SetConfigValueStruct");
        return SteamClient.SteamNetworkingUtils.SetConfigValueStruct(opt, eScopeType, scopeObj);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ESteamNetworkingGetConfigValueResult SteamAPI_ISteamNetworkingUtils_GetConfigValue(int eValue, IntPtr eScopeType, IntPtr scopeObj, IntPtr pOutDataType, IntPtr pResult, IntPtr cbResult)
    {
        Write("SteamAPI_ISteamNetworkingUtils_GetConfigValue");
        return SteamClient.SteamNetworkingUtils.GetConfigValue(eValue, eScopeType, scopeObj, pOutDataType, pResult, cbResult);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamNetworkingUtils_GetConfigValueInfo(int eValue, char pOutName, IntPtr pOutDataType, IntPtr pOutScope, int pOutNextValue)
    {
        Write("SteamAPI_ISteamNetworkingUtils_GetConfigValueInfo");
        return SteamClient.SteamNetworkingUtils.GetConfigValueInfo(eValue, pOutName, pOutDataType, pOutScope, pOutNextValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamNetworkingUtils_GetFirstConfigValue()
    {
        Write("SteamAPI_ISteamNetworkingUtils_GetFirstConfigValue");
        return SteamClient.SteamNetworkingUtils.GetFirstConfigValue();
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
        return SteamClient.SteamNetworkingUtils.SteamNetworkingIPAddr_ParseString(pAddr, pszStr);
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
        return SteamClient.SteamNetworkingUtils.SteamNetworkingIdentity_ParseString(pIdentity, pszStr);
    }

}


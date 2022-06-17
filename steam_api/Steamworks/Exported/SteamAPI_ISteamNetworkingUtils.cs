using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Exported
{
    using SteamNetworkingMicroseconds = System.Int64;
    public class SteamAPI_ISteamNetworkingUtils
    {
        static SteamAPI_ISteamNetworkingUtils()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamNetworkingUtils_AllocateMessage(IntPtr _, int cbAllocateBuffer)
        {
            Write("SteamAPI_ISteamNetworkingUtils_AllocateMessage");
            return SteamEmulator.SteamNetworkingUtils.AllocateMessage(cbAllocateBuffer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamNetworkingUtils_InitRelayNetworkAccess(IntPtr _)
        {
            Write("SteamAPI_ISteamNetworkingUtils_InitRelayNetworkAccess");
            //
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamNetworkingUtils_GetRelayNetworkStatus(IntPtr _, IntPtr pDetails)
        {
            Write("SteamAPI_ISteamNetworkingUtils_GetRelayNetworkStatus");
            return SteamEmulator.SteamNetworkingUtils.GetRelayNetworkStatus(pDetails);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static float SteamAPI_ISteamNetworkingUtils_GetLocalPingLocation(IntPtr _, IntPtr result)
        {
            Write("SteamAPI_ISteamNetworkingUtils_GetLocalPingLocation");
            return SteamEmulator.SteamNetworkingUtils.GetLocalPingLocation(result);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamNetworkingUtils_EstimatePingTimeBetweenTwoLocations(IntPtr _, IntPtr location1, IntPtr location2)
        {
            Write("SteamAPI_ISteamNetworkingUtils_EstimatePingTimeBetweenTwoLocations");
            return SteamEmulator.SteamNetworkingUtils.EstimatePingTimeBetweenTwoLocations(location1, location2);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamNetworkingUtils_EstimatePingTimeFromLocalHost(IntPtr _, IntPtr remoteLocation)
        {
            Write("SteamAPI_ISteamNetworkingUtils_EstimatePingTimeFromLocalHost");
            return SteamEmulator.SteamNetworkingUtils.EstimatePingTimeFromLocalHost(remoteLocation);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamNetworkingUtils_ConvertPingLocationToString(IntPtr _, IntPtr location, string pszBuf, int cchBufSize)
        {
            Write("SteamAPI_ISteamNetworkingUtils_ConvertPingLocationToString");
            //
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworkingUtils_ParsePingLocationString(IntPtr _, string pszString, IntPtr result)
        {
            Write("SteamAPI_ISteamNetworkingUtils_ParsePingLocationString");
            return SteamEmulator.SteamNetworkingUtils.ParsePingLocationString(pszString, result);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworkingUtils_CheckPingDataUpToDate(IntPtr _, float flMaxAgeSeconds)
        {
            Write("SteamAPI_ISteamNetworkingUtils_CheckPingDataUpToDate");
            return SteamEmulator.SteamNetworkingUtils.CheckPingDataUpToDate(flMaxAgeSeconds);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamNetworkingUtils_GetPingToDataCenter(IntPtr _, uint popID, uint pViaRelayPoP)
        {
            Write("SteamAPI_ISteamNetworkingUtils_GetPingToDataCenter");
            return SteamEmulator.SteamNetworkingUtils.GetPingToDataCenter(popID, pViaRelayPoP);
        }

        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        //public static int SteamAPI_ISteamNetworkingUtils_GetDirectPingToPOP(IntPtr popID)
        //{
        //    Write("SteamAPI_ISteamNetworkingUtils_GetDirectPingToPOP");
        //    return SteamEmulator.SteamNetworkingUtils.GetDirectPingToPOP(popID);
        //}

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamNetworkingUtils_GetPOPCount(IntPtr _)
        {
            Write("SteamAPI_ISteamNetworkingUtils_GetPOPCount");
            return SteamEmulator.SteamNetworkingUtils.GetPOPCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamNetworkingUtils_GetPOPList(IntPtr _, uint list, int nListSz)
        {
            Write("SteamAPI_ISteamNetworkingUtils_GetPOPList");
            return SteamEmulator.SteamNetworkingUtils.GetPOPList(list, nListSz);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamNetworkingMicroseconds SteamAPI_ISteamNetworkingUtils_GetLocalTimestamp(IntPtr _)
        {
            Write("SteamAPI_ISteamNetworkingUtils_GetLocalTimestamp");
            return SteamEmulator.SteamNetworkingUtils.GetLocalTimestamp();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamNetworkingUtils_SetDebugOutputFunction(IntPtr _, int eDetailLevel, IntPtr pfnFunc)
        {
            Write("SteamAPI_ISteamNetworkingUtils_SetDebugOutputFunction");
            //
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworkingUtils_SetGlobalConfigValueInt32(IntPtr _, int eValue, int val)
        {
            Write("SteamAPI_ISteamNetworkingUtils_SetGlobalConfigValueInt32");
            return SteamEmulator.SteamNetworkingUtils.SetGlobalConfigValueInt32(eValue, val);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworkingUtils_SetGlobalConfigValueFloat(IntPtr _, int eValue, float val)
        {
            Write("SteamAPI_ISteamNetworkingUtils_SetGlobalConfigValueFloat");
            return SteamEmulator.SteamNetworkingUtils.SetGlobalConfigValueFloat(eValue, val);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworkingUtils_SetGlobalConfigValueString(IntPtr _, int eValue, string val)
        {
            Write("SteamAPI_ISteamNetworkingUtils_SetGlobalConfigValueString");
            return SteamEmulator.SteamNetworkingUtils.SetGlobalConfigValueString(eValue, val);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueInt32(IntPtr _, uint hConn, int eValue, int val)
        {
            Write("SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueInt32");
            return SteamEmulator.SteamNetworkingUtils.SetConnectionConfigValueInt32(hConn, eValue, val);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueFloat(IntPtr _, uint hConn, int eValue, float val)
        {
            Write("SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueFloat");
            return SteamEmulator.SteamNetworkingUtils.SetConnectionConfigValueFloat(hConn, eValue, val);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueString(IntPtr _, uint hConn, int eValue, string val)
        {
            Write("SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueString");
            return SteamEmulator.SteamNetworkingUtils.SetConnectionConfigValueString(hConn, eValue, val);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworkingUtils_SetConfigValue(IntPtr _, int eValue, int eScopeType, IntPtr scopeObj, int eDataType, IntPtr pArg)
        {
            Write("SteamAPI_ISteamNetworkingUtils_SetConfigValue");
            return SteamEmulator.SteamNetworkingUtils.SetConfigValue(eValue, eScopeType, scopeObj, eDataType, pArg);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworkingUtils_SetConfigValueStruct(IntPtr _, IntPtr opt, int eScopeType, IntPtr scopeObj)
        {
            Write("SteamAPI_ISteamNetworkingUtils_SetConfigValueStruct");
            return SteamEmulator.SteamNetworkingUtils.SetConfigValueStruct(opt, eScopeType, scopeObj);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamNetworkingUtils_GetConfigValue(IntPtr _, int eValue, int eScopeType, IntPtr scopeObj, int pOutDataType, IntPtr pResult, IntPtr cbResult)
        {
            Write("SteamAPI_ISteamNetworkingUtils_GetConfigValue");
            return SteamEmulator.SteamNetworkingUtils.GetConfigValue(eValue, eScopeType, scopeObj, pOutDataType, pResult, cbResult);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworkingUtils_GetConfigValueInfo(IntPtr _, int eValue, string pOutName, int pOutDataType, int pOutScope, int pOutNextValue)
        {
            Write("SteamAPI_ISteamNetworkingUtils_GetConfigValueInfo");
            return SteamEmulator.SteamNetworkingUtils.GetConfigValueInfo(eValue, pOutName, pOutDataType, pOutScope, pOutNextValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamNetworkingUtils_GetFirstConfigValue(IntPtr _)
        {
            Write("SteamAPI_ISteamNetworkingUtils_GetFirstConfigValue");
            return SteamEmulator.SteamNetworkingUtils.GetFirstConfigValue();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamNetworkingUtils_SteamNetworkingIPAddr_ToString(IntPtr _, IntPtr addr, string buf, IntPtr cbBuf, bool bWithPort)
        {
            Write("SteamAPI_ISteamNetworkingUtils_SteamNetworkingIPAddr_ToString");
            //
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworkingUtils_SteamNetworkingIPAddr_ParseString(IntPtr _, IntPtr pAddr, string pszStr)
        {
            Write("SteamAPI_ISteamNetworkingUtils_SteamNetworkingIPAddr_ParseString");
            return SteamEmulator.SteamNetworkingUtils.SteamNetworkingIPAddr_ParseString(pAddr, pszStr);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamNetworkingUtils_SteamNetworkingIdentity_ToString(IntPtr _, IntPtr identity, string buf, IntPtr cbBuf)
        {
            Write("SteamAPI_ISteamNetworkingUtils_SteamNetworkingIdentity_ToString");
            //
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworkingUtils_SteamNetworkingIdentity_ParseString(IntPtr _, IntPtr pIdentity, string pszStr)
        {
            Write("SteamAPI_ISteamNetworkingUtils_SteamNetworkingIdentity_ParseString");
            return SteamEmulator.SteamNetworkingUtils.SteamNetworkingIdentity_ParseString(pIdentity, pszStr);
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}


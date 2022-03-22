using Core.Interface;
using SKYNET.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate(Name = "SteamNetworkingUtils")]
    public class DSteamNetworkingUtils 
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr AllocateMessage(int cbAllocateBuffer);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void InitRelayNetworkAccess(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ESteamNetworkingAvailability GetRelayNetworkStatus(SteamRelayNetworkStatus_t pDetails);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate float GetLocalPingLocation(SteamNetworkPingLocation_t result);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int EstimatePingTimeBetweenTwoLocations(SteamNetworkPingLocation_t location1, SteamNetworkPingLocation_t location2);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int EstimatePingTimeFromLocalHost(SteamNetworkPingLocation_t remoteLocation);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ConvertPingLocationToString(SteamNetworkPingLocation_t location, char pszBuf, int cchBufSize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool ParsePingLocationString(char pszString, SteamNetworkPingLocation_t result);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool CheckPingDataUpToDate(float flMaxAgeSeconds);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetPingToDataCenter(IntPtr popID, IntPtr pViaRelayPoP);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetDirectPingToPOP(IntPtr popID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetPOPCount(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetPOPList(IntPtr list, int nListSz);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetLocalTimestamp(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetDebugOutputFunction(ESteamNetworkingSocketsDebugOutputType eDetailLevel, IntPtr pfnFunc);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetGlobalConfigValueInt32(int eValue, uint val);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetGlobalConfigValueFloat(int eValue, float val);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetGlobalConfigValueString(int eValue, char val);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetConnectionConfigValueInt32(uint hConn, int eValue, uint val);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetConnectionConfigValueFloat(uint hConn, int eValue, float val);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetConnectionConfigValueString(uint hConn, int eValue, char val);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetConfigValue(int eValue, IntPtr eScopeType, IntPtr scopeObj, IntPtr eDataType, IntPtr pArg);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetConfigValueStruct(IntPtr opt, IntPtr eScopeType, IntPtr scopeObj);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ESteamNetworkingGetConfigValueResult GetConfigValue(int eValue, IntPtr eScopeType, IntPtr scopeObj, IntPtr pOutDataType, IntPtr pResult, IntPtr cbResult);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetConfigValueInfo(int eValue, char pOutName, IntPtr pOutDataType, IntPtr pOutScope, int pOutNextValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetFirstConfigValue(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SteamNetworkingIPAddr_ToString(IntPtr addr, char buf, IntPtr cbBuf, bool bWithPort);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SteamNetworkingIPAddr_ParseString(IntPtr pAddr, char pszStr);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SteamNetworkingIdentity_ToString(IntPtr identity, char buf, IntPtr cbBuf);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SteamNetworkingIdentity_ParseString(IntPtr pIdentity, char pszStr);
    }
}

using System;
using System.Runtime.InteropServices;
using SKYNET.Helpers;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_SteamNetworkingIdentity
    {
        static SteamAPI_SteamNetworkingIdentity()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIdentity_Clear(IntPtr _)
        {
            SteamNetworkingIdentityInterop.Clear(_);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamNetworkingIdentity_GetGenericBytes(IntPtr _, IntPtr cbLen)
        {
            return SteamNetworkingIdentityInterop.GetDataPointer(_, NetIdentityType.GenericBytes, cbLen);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamNetworkingIdentity_GetGenericString(IntPtr _)
        {
            return SteamNetworkingIdentityInterop.GetDataPointer(_, NetIdentityType.GenericString);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamNetworkingIdentity_GetIPAddr(IntPtr _)
        {
            return SteamNetworkingIdentityInterop.GetDataPointer(_, NetIdentityType.IPAddress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_SteamNetworkingIdentity_GetSteamID(IntPtr _)
        {
            return SteamNetworkingIdentityInterop.TryReadSteamId(_, out var steamId) ? steamId : 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_SteamNetworkingIdentity_GetSteamID64(IntPtr _)
        {
            return SteamNetworkingIdentityInterop.TryReadSteamId(_, out var steamId) ? steamId : 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamNetworkingIdentity_GetXboxPairwiseID(IntPtr _)
        {
            return SteamNetworkingIdentityInterop.GetDataPointer(_, NetIdentityType.XboxPairwiseID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIdentity_IsEqualTo(IntPtr _, IntPtr x)
        {
            return SteamNetworkingIdentityInterop.Equals(_, x);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIdentity_IsInvalid(IntPtr _)
        {
            return SteamNetworkingIdentityInterop.IsInvalid(_);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIdentity_IsLocalHost(IntPtr _)
        {
            return SteamNetworkingIdentityInterop.IsLocalHost(_);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIdentity_ParseString(IntPtr _, string pszStr)
        {
            if (!SteamNetworkingIdentityInterop.TryParse(pszStr, (UIntPtr)SteamNetworkingIdentityInterop.Size, out var identity))
            {
                SteamNetworkingIdentityInterop.Clear(_);
                return false;
            }

            SteamNetworkingIdentityInterop.Write(_, identity);
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIdentity_SetGenericBytes(IntPtr _, IntPtr data, uint cbLen)
        {
            if (cbLen > SteamNetworkingIdentityInterop.DataCapacity || (cbLen > 0 && data == IntPtr.Zero))
            {
                return false;
            }

            var bytes = new byte[cbLen];
            if (bytes.Length > 0)
            {
                Marshal.Copy(data, bytes, 0, bytes.Length);
            }
            return SteamNetworkingIdentityInterop.SetBytes(_, NetIdentityType.GenericBytes, bytes, nullTerminate: false, requireContent: false);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIdentity_SetGenericString(IntPtr _, string pszString)
        {
            return SteamNetworkingIdentityInterop.SetBytes(
                _,
                NetIdentityType.GenericString,
                System.Text.Encoding.UTF8.GetBytes(pszString ?? string.Empty),
                nullTerminate: true,
                requireContent: false);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIdentity_SetIPAddr(IntPtr _, IntPtr addr)
        {
            if (_ == IntPtr.Zero || addr == IntPtr.Zero)
            {
                return;
            }

            SteamNetworkingIdentityInterop.Write(_, SteamNetworkingIdentityInterop.FromIpAddress(SteamNetworkingIPAddrInterop.Read(addr)));
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIdentity_SetLocalHost(IntPtr _)
        {
            SteamNetworkingIdentityInterop.Write(_, SteamNetworkingIdentityInterop.LocalHost());
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIdentity_SetSteamID(IntPtr _, ulong steamID)
        {
            SteamNetworkingIdentityInterop.WriteSteamId(_, steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIdentity_SetSteamID64(IntPtr _, ulong steamID)
        {
            SteamNetworkingIdentityInterop.WriteSteamId(_, steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIdentity_SetXboxPairwiseID(IntPtr _, string pszString)
        {
            return SteamNetworkingIdentityInterop.SetBytes(
                _,
                NetIdentityType.XboxPairwiseID,
                System.Text.Encoding.UTF8.GetBytes(pszString ?? string.Empty),
                nullTerminate: true,
                requireContent: true);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIdentity_ToString(IntPtr _, IntPtr buf, UIntPtr cbBuf)
        {
            NativeStringCache.WriteUtf8Buffer(buf, checked((int)cbBuf.ToUInt64()), SteamNetworkingIdentityInterop.Format(_));
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamNetworkingIdentityRender_c_str(IntPtr _)
        {
            return _;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIdentity_SetIPv4Addr(IntPtr _, uint nIPv4, ushort nPort)
        {
            SteamNetworkingIdentityInterop.Write(_, SteamNetworkingIdentityInterop.FromIpAddress(SteamNetworkingIPAddrInterop.FromIPv4(nIPv4, nPort)));
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_SteamNetworkingIdentity_GetIPv4(IntPtr _)
        {
            return ReadIPv4(_);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_SteamNetworkingIdentity_GetFakeIPType(IntPtr _)
        {
            return ResolveFakeIPType(_);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_SteamNetworkingIdentity_IsFakeIP(IntPtr _)
        {
            return ResolveFakeIPType(_) > (int)SteamNetworkingFakeIPType.NotFake;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIdentity_SetPSNID(IntPtr _, ulong id)
        {
            SteamNetworkingIdentityInterop.SetBytes(_, NetIdentityType.SonyPSN, BitConverter.GetBytes(id), nullTerminate: false, requireContent: false);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_SteamNetworkingIdentity_GetPSNID(IntPtr _)
        {
            return ReadId(_, NetIdentityType.SonyPSN);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_SteamNetworkingIdentity_SetStadiaID(IntPtr _, ulong id)
        {
            SteamNetworkingIdentityInterop.SetBytes(_, NetIdentityType.GoogleStadia, BitConverter.GetBytes(id), nullTerminate: false, requireContent: false);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_SteamNetworkingIdentity_GetStadiaID(IntPtr _)
        {
            return ReadId(_, NetIdentityType.GoogleStadia);
        }

        private static uint ReadIPv4(IntPtr identity)
        {
            var addr = SteamNetworkingIdentityInterop.GetDataPointer(identity, NetIdentityType.IPAddress);
            return addr == IntPtr.Zero ? 0 : SteamNetworkingIPAddrInterop.GetIPv4(SteamNetworkingIPAddrInterop.Read(addr));
        }

        private static int ResolveFakeIPType(IntPtr identity)
        {
            return SteamNetworkingIdentityInterop.GetDataPointer(identity, NetIdentityType.IPAddress) == IntPtr.Zero
                ? (int)SteamNetworkingFakeIPType.Invalid
                : SteamEmulator.SteamNetworkingUtils.GetIPv4FakeIPType(ReadIPv4(identity));
        }

        private static ulong ReadId(IntPtr identity, NetIdentityType type)
        {
            var data = SteamNetworkingIdentityInterop.GetDataPointer(identity, type);
            return data == IntPtr.Zero ? 0UL : unchecked((ulong)Marshal.ReadInt64(data));
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}

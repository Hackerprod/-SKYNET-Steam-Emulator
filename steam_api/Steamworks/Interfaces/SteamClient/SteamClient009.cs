using System;
using System.Runtime.InteropServices;

using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;

namespace SKYNET.Steamworks.Interfaces
{
    /// <summary>Legacy ISteamClient ABI used by Left 4 Dead (AppID 500).</summary>
    [Interface("SteamClient009")]
    public class SteamClient009 : ISteamInterface
    {
        public HSteamPipe CreateSteamPipe(IntPtr _) => SteamEmulator.SteamClient.CreateSteamPipe();

        public bool BReleaseSteamPipe(IntPtr _, HSteamPipe hSteamPipe) =>
            SteamEmulator.SteamClient.BReleaseSteamPipe(hSteamPipe);

        public HSteamUser ConnectToGlobalUser(IntPtr _, HSteamPipe hSteamPipe) =>
            SteamEmulator.SteamClient.ConnectToGlobalUser(hSteamPipe);

        public HSteamUser CreateLocalUser(IntPtr _, IntPtr phSteamPipe, int eAccountType)
        {
            HSteamPipe pipe = SteamEmulator.CreateSteamPipe();
            if (phSteamPipe != IntPtr.Zero)
            {
                Marshal.WriteInt32(phSteamPipe, unchecked((int)pipe));
            }
            return SteamEmulator.SteamClient.CreateLocalUser(pipe, eAccountType);
        }

        public void ReleaseUser(IntPtr _, HSteamPipe hSteamPipe, HSteamUser hUser) =>
            SteamEmulator.SteamClient.ReleaseUser(hSteamPipe, hUser);

        public IntPtr GetISteamUser(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) =>
            SteamEmulator.SteamClient.GetISteamUser(hSteamUser, hSteamPipe, pchVersion);

        public IntPtr GetISteamGameServer(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) =>
            SteamEmulator.SteamClient.GetISteamGameServer(hSteamUser, hSteamPipe, pchVersion);

        public void SetLocalIPBinding(IntPtr _, uint unIP, ushort usPort) =>
            SteamEmulator.SteamClient.SetLocalIPBinding(unIP, usPort);

        public IntPtr GetISteamFriends(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) =>
            SteamEmulator.SteamClient.GetISteamFriends(hSteamUser, hSteamPipe, pchVersion);

        public IntPtr GetISteamUtils(IntPtr _, HSteamPipe hSteamPipe, string pchVersion) =>
            SteamEmulator.SteamClient.GetISteamUtils(hSteamPipe, pchVersion);

        public IntPtr GetISteamMatchmaking(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) =>
            SteamEmulator.SteamClient.GetISteamMatchmaking(hSteamUser, hSteamPipe, pchVersion);

        public IntPtr GetISteamMasterServerUpdater(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) =>
            SteamEmulator.SteamClient.GetISteamMasterServerUpdater(hSteamUser, hSteamPipe, pchVersion);

        public IntPtr GetISteamMatchmakingServers(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) =>
            SteamEmulator.SteamClient.GetISteamMatchmakingServers(hSteamUser, hSteamPipe, pchVersion);

        public IntPtr GetISteamGenericInterface(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) =>
            SteamEmulator.SteamClient.GetISteamGenericInterface(hSteamUser, hSteamPipe, pchVersion);

        public IntPtr GetISteamUserStats(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) =>
            SteamEmulator.SteamClient.GetISteamUserStats(hSteamUser, hSteamPipe, pchVersion);

        public IntPtr GetISteamGameServerStats(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) =>
            SteamEmulator.SteamClient.GetISteamGameServerStats(hSteamUser, hSteamPipe, pchVersion);

        public IntPtr GetISteamApps(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) =>
            SteamEmulator.SteamClient.GetISteamApps(hSteamUser, hSteamPipe, pchVersion);

        public IntPtr GetISteamNetworking(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) =>
            SteamEmulator.SteamClient.GetISteamNetworking(hSteamUser, hSteamPipe, pchVersion);

        public IntPtr GetISteamRemoteStorage(IntPtr _, HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion) =>
            SteamEmulator.SteamClient.GetISteamRemoteStorage(hSteamUser, hSteamPipe, pchVersion);

        public void RunFrame(IntPtr _) => SteamEmulator.SteamClient.RunFrame();
        public uint GetIPCCallCount(IntPtr _) => SteamEmulator.SteamClient.GetIPCCallCount();

        public void SetWarningMessageHook(IntPtr _, IntPtr pFunction) =>
            SteamEmulator.SteamClient.SetWarningMessageHook(pFunction);
    }
}

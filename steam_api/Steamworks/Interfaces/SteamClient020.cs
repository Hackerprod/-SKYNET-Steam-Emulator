using SKYNET.Interface;
using SKYNET.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    [Interface("SteamClient020")]
    public class SteamClient020 : ISteamInterface
    {
        public int CreateSteamPipe(IntPtr _)
        {
            return SteamEmulator.SteamClient.CreateSteamPipe(_);
        }

        public bool ReleaseSteamPipe(IntPtr _, int pipe)
        {
            return SteamEmulator.SteamClient.BReleaseSteamPipe(pipe);
        }

        public int ConnectToGlobalUser(IntPtr _, int pipe)
        {
            return SteamEmulator.SteamClient.ConnectToGlobalUser(pipe);
        }

        public int CreateLocalUser(IntPtr _, ref int pipe, uint account_type)
        {
            return SteamEmulator.SteamClient.CreateLocalUser(pipe, (int)account_type);
        }


        public void ReleaseUser(IntPtr _, int user, int pipe)
        {
            SteamEmulator.SteamClient.ReleaseUser(user, pipe);
        }


        public IntPtr GetSteamUser(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public IntPtr GetSteamGameServer(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public void SetLocalIPBinding(IntPtr _, uint ip, uint port)
        {
            SteamEmulator.SteamClient.SetLocalIPBinding(ip, port);
        }


        public IntPtr GetSteamFriends(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public IntPtr GetSteamUtils(IntPtr _, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(version);
        }


        public IntPtr GetSteamMatchmaking(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public IntPtr GetSteamMatchmakingServers(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public IntPtr GetSteamGenericInterface(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public IntPtr GetSteamUserStats(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public IntPtr GetSteamGameServerStats(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public IntPtr GetSteamApps(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public IntPtr GetSteamNetworking(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public IntPtr GetSteamRemoteStorage(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public IntPtr GetSteamScreenshots(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public IntPtr GetISteamGameSearch(IntPtr _, int user, int pipe, string version) // Remove from here
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public void RunFrame(IntPtr _)
        {
            SteamEmulator.SteamClient.RunFrame(_);
        }


        public uint GetIPCCallCount(IntPtr _)
        {
            return SteamEmulator.SteamClient.GetIPCCallCount(_);
        }


        public void SetWarningMessageHook(IntPtr _, IntPtr function)
        {
            SteamEmulator.SteamClient.SetWarningMessageHook(function);
        }


        public bool ShutdownIfAllPipesClosed(IntPtr _)
        {
            return SteamEmulator.SteamClient.BShutdownIfAllPipesClosed(_);
        }


        public IntPtr GetSteamHTTP(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public IntPtr GetSteamUnifiedMessages(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public IntPtr GetSteamController(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public IntPtr GetSteamUGC(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public IntPtr GetSteamAppList(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public IntPtr GetSteamMusic(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public IntPtr GetSteamMusicRemote(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public IntPtr GetSteamHTMLSurface(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public void Set_SteamAPI_CPostAPIResultInProcess(IntPtr _, IntPtr function)
        {
            SteamEmulator.SteamClient.Set_SteamAPI_CPostAPIResultInProcess(function);
        }


        public void DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess(IntPtr _, IntPtr function)
        {
            SteamEmulator.SteamClient.DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess(function);
        }


        public void Set_SteamAPI_CCheckCallbackRegisteredInProcess(IntPtr _, IntPtr function)
        {
            SteamEmulator.SteamClient.DEPRECATED_Remove_SteamAPI_CPostAPIResultInProcess(function);
        }


        public IntPtr GetSteamInventory(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public IntPtr GetSteamVideo(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public IntPtr GetISteamParentalSettings(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public IntPtr GetISteamInput(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public IntPtr GetISteamParties(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public IntPtr GetISteamRemotePlay(IntPtr _, int user, int pipe, string version)
        {
            return InterfaceManager.FindOrCreateInterface(user, pipe, version);
        }


        public void DestroyAllInterfaces(IntPtr _)
        {
            SteamEmulator.SteamClient.DestroyAllInterfaces(_);
        }
    }
}

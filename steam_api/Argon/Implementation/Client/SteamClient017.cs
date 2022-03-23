using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Core;
using SKYNET;

namespace InterfaceClient
{
    [Core.Interface.Impl(Name = "SteamClient017", ServerMapped = false)]
    public class SteamClient017 : Core.Interface.IBaseInterface
    {
        public int CreateSteamPipe(IntPtr _)
        {
            Write("CreateSteamPipe");
            return (int)SteamEmulator.CreateSteamPipe();
        }

        public bool ReleaseSteamPipe(IntPtr _, int pipe)
        {
            Write("WARNING: ReleaseSteamPipe NOT IMPLEMENTED");
            return true;
        }

        // TODO: Deal with global users
        // In theory as a result of our user model any process can connect
        // to any user that they wish too, they just need to know which one
        // is the "global user". 

        // (This can be implemented as a handle = global user)
        // or we could provide a method by which a user can choose a user to connect too...
        // TODO: Deal with global users
        public int ConnectToGlobalUser(IntPtr _, int pipe)
        {
            Write("ConnectToGlobalUser");
            return 0;
        }

        public int CreateLocalUser(IntPtr _, ref int pipe, uint account_type)
        {
            Write(string.Format("CreateLocalUser {0} {1}", pipe, account_type));

            if (pipe == 0) pipe = CreateSteamPipe(IntPtr.Zero);

            return (int)SteamEmulator.CreateSteamUser(); ;
        }

        public void ReleaseUser(IntPtr _, int user, int pipe)
        {
            Write("ReleaseUser");
            return;
        }

        IntPtr CreateInterface(int pipe, int user, string version)
        {
            try
            {
                return InterfaceManager.FindOrCreateInterface(version);
            }
            catch (Exception e)
            {
                Write(string.Format("Exception in CreateInterface \"{0}\"", e.Message));
                return IntPtr.Zero;
            }
        }

        public IntPtr GetSteamUser(IntPtr _, int user, int pipe, string version)
        {
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetSteamGameServer(IntPtr _, int user, int pipe, string version)
        {
            return CreateInterface(pipe, user, version);
        }

        public void SetLocalIPBinding(IntPtr _, uint ip, uint port)
        {
            Write("SetLocalIPBinding");
            return;
        }

        public IntPtr GetSteamFriends(IntPtr _, int user, int pipe, string version)
        {
            Write("GetSteamFriends");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetSteamUtils(IntPtr _, int pipe, string version)
        {
            Write("GetSteamUtils");
            return InterfaceManager.CreateInterfaceNoUser(pipe, version);
        }

        public IntPtr GetSteamMatchmaking(IntPtr _, int user, int pipe, string version)
        {
            Write("GetSteamMatchmaking");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetSteamMatchmakingServers(IntPtr _, int user, int pipe, string version)
        {
            Write("GetSteamMatchmakingServers");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetSteamGenericInterface(IntPtr _, int user, int pipe, string version)
        {
            Write("GetSteamGenericInterface");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetSteamUserStats(IntPtr _, int user, int pipe, string version)
        {
            Write("GetSteamUserStats");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetSteamGameServerStats(IntPtr _, int user, int pipe, string version)
        {
            Write("GetSteamGameServerStats");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetSteamApps(IntPtr _, int user, int pipe, string version)
        {
            Write("GetSteamApps");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetSteamNetworking(IntPtr _, int user, int pipe, string version)
        {
            Write("GetSteamNetworking");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetSteamRemoteStorage(IntPtr _, int user, int pipe, string version)
        {
            Write("GetSteamRemoteStorage");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetSteamScreenshots(IntPtr _, int user, int pipe, string version)
        {
            Write("GetSteamScreenshots");
            return CreateInterface(pipe, user, version);
        }

        public void RunFrame(IntPtr _)
        {
            // Pipes automatically run ipc threads so...
            // TODO: is this type of behaviour allowable?
            Write("RunFrame");
        }

        public uint GetIPCCallCount(IntPtr _)
        {
            Write("GetIPCCallCount");
            return 0;
        }

        public void SetWarningMessageHook(IntPtr _, IntPtr function)
        {
            Write("SetWarningMessageHook");
            return;
        }

        public bool ShutdownIfAllPipesClosed(IntPtr _)
        {
            // TODO: what does this actually do?
            Write("ShutdownIfAllPipesClosed");
            return false;
        }

        public IntPtr GetSteamHTTP(IntPtr _, int user, int pipe, string version)
        {
            Write("GetSteamHTTP");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetSteamUnifiedMessages(IntPtr _, int user, int pipe, string version)
        {
            Write("GetSteamUnifiedMessages");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetSteamController(IntPtr _, int user, int pipe, string version)
        {
            Write("GetSteamController");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetSteamUGC(IntPtr _, int user, int pipe, string version)
        {
            Write("GetSteamUGC");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetSteamAppList(IntPtr _, int user, int pipe, string version)
        {
            Write("GetSteamAppList");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetSteamMusic(IntPtr _, int user, int pipe, string version)
        {
            Write("GetSteamMusic");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetSteamMusicRemote(IntPtr _, int user, int pipe, string version)
        {
            Write("GetSteamMusicRemote");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetSteamHTMLSurface(IntPtr _, int user, int pipe, string version)
        {
            Write("GetSteamHTMLSurface");
            return CreateInterface(pipe, user, version);
        }

        public void Set_SteamAPI_CPostAPIResultInProcess(IntPtr _, IntPtr function)
        {
            Write("Set_SteamAPI_CPostAPIResultInProcess");
        }

        public void Remove_SteamAPI_CPostAPIResultInProcess(IntPtr _, IntPtr function)
        {
            Write("Remove_SteamAPI_CPostAPIResultInProcess");
        }

        public void Set_SteamAPI_CCheckCallbackRegisteredInProcess(IntPtr _, IntPtr function)
        {
            Write("Set_SteamAPI_CCheckCallbackRegisteredInProcess");
        }

        public IntPtr GetSteamInventory(IntPtr _, int user, int pipe, string version)
        {
            Write("GetSteamInventory");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetSteamVideo(IntPtr _, int user, int pipe, string version)
        {
            Write("GetSteamVideo");
            return CreateInterface(pipe, user, version);
        }

        private void Write(string v)
        {
            Main.Write(InterfaceVersion, v);
        }
    }
}

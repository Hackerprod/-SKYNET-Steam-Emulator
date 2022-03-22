using System;
using System.Collections.Generic;
using System.Text;
using Core;
using Core.Interface;

namespace InterfaceClient
{
    [Impl(Name = "CLIENTENGINE_INTERFACE_VERSION005", ServerMapped = false)]
    class ClientEngine005 : IBaseInterface
    {
        public int CreateClientPipe(IntPtr _)
        {
            Write("CreateSteamPipe");
            return 1;
        }

        public bool ReleaseClientPipe(IntPtr _, int pipe)
        {
            Write("WARNING: ReleaseClientPipe NOT IMPLEMENTED");
            return true;
        }

        public int CreateGlobalUser(IntPtr _, ref int pipe)
        {
            Write("CreateGlobalUser");
            return 0;
        }

        // TODO: Deal with global users (See ISteamClient017)
        public int ConnectToGlobalUser(IntPtr _, int pipe)
        {
            Write("ConnectToGlobalUser");
            return 0;
        }

        public int CreateLocalUser(IntPtr _, ref int pipe, uint account_type)
        {
            Write(string.Format("CreateLocalUser {0} {1}", pipe, account_type));

            return 1;
        }

        public void CreatePipeToLocalUser(IntPtr _, int user, ref int pipe)
        {
            Write("CreatePipeToLocalUser");
        }

        public void ReleaseUser(IntPtr _, int user, int pipe)
        {
            // TODO: remove users
            Write("ReleaseUser");
            return;
        }

        public bool IsValidHSteamUserPipe(IntPtr _, int pipe, int user)
        {
            Write("IsValidHSteamUserPipe not implemented...");
            return true;
        }

        IntPtr CreateInterface(int pipe, int user, string version)
        {
            Write(string.Format("CreateInterface {0}", version));
            try
            {
                return InterfaceManager.CreateInterface(pipe, version);
            }
            catch (Exception e)
            {
                Write(string.Format("Exception in CreateInterface \"{0}\"", e.Message));
                return IntPtr.Zero;
            }
        }

        public IntPtr GetClientUser(IntPtr _, int user, int pipe, string version)
        {
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetClientGameServer(IntPtr _, int user, int pipe, string version)
        {
            return CreateInterface(pipe, user, version);
        }

        public void SetLocalIPBinding(IntPtr _, uint ip, uint port)
        {
            Write("SetLocalIPBinding");
            return;
        }

        public string GetUniverseName(IntPtr _, uint universe)
        {
            return ""; //Enum.GetName(typeof(SteamKit2.EUniverse), universe);
        }

        public IntPtr GetClientFriends(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientFriends");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetClientUtils(IntPtr _, int pipe, string version)
        {
            Write("GetClientUtils");
            return InterfaceManager.CreateInterfaceNoUser(pipe, version);
        }

        public IntPtr GetClientBilling(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientBilling");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetClientMatchmaking(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientMatchmaking");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetClientApps(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientApps");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetClientMatchmakingServers(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientMatchmakingServers");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetClientGameSearch(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientGameSearch");
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
            // TODO: actually keep track of these in the future
            Write("GetIPCCallCount");
            return 1;
        }

        public IntPtr GetClientUserStats(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientUserStats");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetClientGameServerStats(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientGameServerStats");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetClientNetworking(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientNetworking");
            return CreateInterface(pipe, user, version);
        }
        public IntPtr GetClientRemoteStorage(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientRemoteStorage");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetClientScreenshots(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientScreenshots");
            return CreateInterface(pipe, user, version);
        }

        public void SetWarningMessageHook(IntPtr _, IntPtr function)
        {
            Write("SetWarningMessageHook");
            return;
        }

        public IntPtr GetClientGameCoordinator(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientGameCoordinator");
            return CreateInterface(pipe, user, version);
        }

        public void SetOverlayNotificationPosition(IntPtr _, uint position)
        {

        }

        public void SetOverlayNotificationInsert(IntPtr _, uint position)
        {

        }

        public bool HookScreenshots(IntPtr _, bool hook)
        {
            return false;
        }

        public bool IsOverlayEnabled(IntPtr _)
        {
            return false;
        }

        public bool GetAPICallResult(IntPtr _, int pipe, uint handle, ref IntPtr callback_buffer, int callback_size, int expected_callback, ref bool failed)
        {
            failed = true;
            return false;
        }

        public IntPtr GetClientProductBuilder(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientProductBuilder");
            return CreateInterface(pipe, user, version);
        }
        public IntPtr GetClientDepotBuilder(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientDepotBuilder");
            return CreateInterface(pipe, user, version);
        }
        public IntPtr GetClientNetworkDeviceManager(IntPtr _, int pipe, string version)
        {
            Write("GetClientNetworkDeviceManager");
            return InterfaceManager.CreateInterfaceNoUser(pipe, version);
        }

        public void ConCommandInit(IntPtr _, IntPtr __)
        {
        }

        public IntPtr GetClientAppManager(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientAppManager");
            return CreateInterface(pipe, user, version);
        }
        public IntPtr GetClientConfigStore(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientConfigStore");
            return CreateInterface(pipe, user, version);
        }

        public bool BOverlayNeedsPresent()
        {
            return false;
        }

        public IntPtr GetClientGameStats(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientGameStats");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetClientHTTP(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientHTTP");
            return CreateInterface(pipe, user, version);
        }

        public bool BShutdownIfAllPipesClosed()
        {
            return false;
        }

        public IntPtr GetClientAudio(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientAudio");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetClientMusic(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientMusic");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetClientUnifiedMessages(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientUnifiedMessages");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetClientController(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientController");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetClientParentalSettings(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientParentalSettings");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetClientStreamLauncher(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientStreamLauncher");
            return CreateInterface(pipe, user, version);
        }
        public IntPtr GetClientDeviceAuth(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientDeviceAuth");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetClientRemoteClientManager(IntPtr _, int pipe, string version)
        {
            Write("GetClientRemoteClientManager");
            return InterfaceManager.CreateInterfaceNoUser(pipe, version);
        }
        public IntPtr GetClientStreamClient(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientStreamClient");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetClientShortcuts(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientShortcuts");
            return CreateInterface(pipe, user, version);
        }
        public IntPtr GetClientUGC(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientUGC");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetClientInventory(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientInventory");
            return CreateInterface(pipe, user, version);
        }
        public IntPtr GetClientVR(IntPtr _, int pipe, string version)
        {
            Write("GetClientVR");
            return InterfaceManager.CreateInterfaceNoUser(pipe, version);
        }

        public IntPtr GetClientGameNotifications(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientGameNotifications");
            return CreateInterface(pipe, user, version);
        }
        public IntPtr GetClientHTMLSurface(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientHTMLSurface");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetClientVideo(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientVideo");
            return CreateInterface(pipe, user, version);
        }
        public IntPtr GetClientControllerSerialized(IntPtr _, int pipe, string version)
        {
            Write("GetClientControllerSerialized");
            return IntPtr.Zero;
        }

        public IntPtr GetClientAppDisableUpdate(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientAppDisableUpdate");
            return CreateInterface(pipe, user, version);
        }

        public int Set_Client_API_CCheckCallbackRegisteredInProcess(IntPtr _, IntPtr callback)
        {
            return 0;
        }

        public IntPtr GetClientBluetoothManager(IntPtr _, int pipe, string version)
        {
            Write("GetClientAppDisableUpdate");
            return IntPtr.Zero;
        }

        public IntPtr GetClientSharedConnection(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientSharedConnection");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetClientShader(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientShader");
            return CreateInterface(pipe, user, version);
        }

        public IntPtr GetClientNetworkingSocketsSerialized(IntPtr _, int user, int pipe, string version)
        {
            Write("GetClientNetworkingSocketsSerialized");
            return CreateInterface(pipe, user, version);
        }

        // TODO: if we are on unix there are 2 of these...
        public void Destructor_CSteamClient1(IntPtr _)
        {

        }

        public void GetIPCServerMap(IntPtr _)
        {

        }

        public void OnDebugTextArrived(IntPtr _, string text)
        {

        }

        public void OnThreadLocalRegistration(IntPtr _)
        {

        }

        public void OnThreadBuffersOverLimit(IntPtr _)
        {

        }

    }
}

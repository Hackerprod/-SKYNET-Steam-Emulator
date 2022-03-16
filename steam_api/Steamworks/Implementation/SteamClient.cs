using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Interface;
using SKYNET;
using SKYNET.Callback;
using SKYNET.GUI;
using SKYNET.Helper;
using SKYNET.Interface;
using SKYNET.Managers;
using Steamworks;

namespace SKYNET.Managers
{
    [Interface.MapAttribute("SteamClient")]
    public class SteamClient : IBaseInterface, ISteamClient
    {
        public void SetAppId(uint appId)
        {
            SteamEmulator.AppId = appId;
        }

        public HSteamPipe CreateSteamPipe()
        {
            Write("CreateSteamPipe");
            return SteamEmulator.CreateSteamPipe();
        }

        public bool BReleaseSteamPipe(HSteamPipe hSteamPipe)
        {
            Write("BReleaseSteamPipe");
            return false;
        }

        public HSteamUser ConnectToGlobalUser(HSteamPipe hSteamPipe)
        {
            Write("ConnectToGlobalUser");

            if (SteamEmulator.steam_pipes.ContainsKey(hSteamPipe))
            {
                return (HSteamUser)0;
            }

            SteamEmulator.steam_pipes[hSteamPipe] = Steam_Pipe.CLIENT;

            return (HSteamUser)(int)Steam_Pipe.CLIENT;
        }

        public HSteamUser CreateLocalUser(out HSteamPipe phSteamPipe, EAccountType eAccountType)
        {
            Write("CreateLocalUser");
            phSteamPipe = (HSteamPipe)1;
            return SteamEmulator.CreateSteamUser();
        }

        public void ReleaseUser(HSteamPipe hSteamPipe, HSteamUser hUser)
        {
            Write("ReleaseUser");
        }

        public ISteamUser GetISteamUser(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamUser");
            return SteamEmulator.SteamUser;
        }

        public ISteamGameServer GetISteamGameServer(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamGameServer");
            return SteamEmulator.SteamGameServer;
        }

        public void SetLocalIPBinding(uint unIP, ushort usPort)
        {
            Write("SetLocalIPBinding");
        }

        public ISteamFriends GetISteamFriends(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamFriends");
            return SteamEmulator.SteamFriends;
        }

        public ISteamGameSearch GetISteamGameSearch(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamFriends");
            return SteamEmulator.SteamGameSearch;
        }

        public ISteamUtils GetISteamUtils(HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamUtils");
            return SteamEmulator.SteamUtils;
        }

        public ISteamMatchmaking GetISteamMatchmaking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamMatchmaking");
            return SteamEmulator.SteamMatchmaking;
        }

        public ISteamMatchmakingServers GetISteamMatchmakingServers(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamMatchmakingServers");
            return SteamEmulator.SteamMatchmakingServers;
        }

        public IntPtr GetISteamGenericInterface(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamGenericInterface");
            return InterfaceManager.GetGenericInterface(hSteamUser, hSteamPipe, pchVersion);
        }

        public ISteamUserStats GetISteamUserStats(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamUserStats");
            return SteamEmulator.SteamUserStats;
        }

        public ISteamGameServerStats GetISteamGameServerStats(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamGameServerStats");
            return SteamEmulator.SteamGameServerStats;
        }

        public ISteamApps GetISteamApps(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamApps");
            return SteamEmulator.SteamApps;
        }

        public ISteamNetworking GetISteamNetworking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamNetworking");
            return SteamEmulator.SteamNetworking;
        }

        public ISteamRemoteStorage GetISteamRemoteStorage(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamRemoteStorage");
            return SteamEmulator.SteamRemoteStorage;
        }

        public ISteamScreenshots GetISteamScreenshots(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamScreenshots");
            return SteamEmulator.SteamScreenshots;
        }

        public uint GetIPCCallCount()
        {
            Write("GetIPCCallCount");
            return 0;
        }

        public bool BShutdownIfAllPipesClosed()
        {
            Write("BShutdownIfAllPipesClosed");
            return false;
        }

        public ISteamHTTP GetISteamHTTP(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamHTTP");
            return SteamEmulator.SteamHTTP;
        }

        public ISteamController GetISteamController(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamController");
            return SteamEmulator.SteamController;
        }

        public ISteamUGC GetISteamUGC(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamUGC");
            return SteamEmulator.SteamUGC;
        }

        public ISteamAppList GetISteamAppList(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamAppList");
            return SteamEmulator.SteamAppList;
        }

        public ISteamMusic GetISteamMusic(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamMusic");
            return SteamEmulator.SteamMusic;
        }

        public ISteamMusicRemote GetISteamMusicRemote(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamMusicRemote");
            return SteamEmulator.SteamMusicRemote;
        }

        public ISteamInput GetISteamInput(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamInput");
            return SteamEmulator.SteamInput;
        }

        public ISteamHTMLSurface GetISteamHTMLSurface(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamHTMLSurface");
            return SteamEmulator.SteamHTMLSurface;
        }

        public ISteamParties GetISteamParties(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamParties");
            return SteamEmulator.SteamParties;
        }

        public ISteamInventory GetISteamInventory(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamInventory");
            return SteamEmulator.SteamInventory;
        }

        public ISteamRemotePlay GetISteamRemotePlay(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamRemotePlay");
            return SteamEmulator.SteamRemotePlay;
        }

        public ISteamVideo GetISteamVideo(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamVideo");
            return SteamEmulator.SteamVideo;
        }

        public ISteamParentalSettings GetISteamParentalSettings(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
        {
            Write("GetISteamParentalSettings");
            return SteamEmulator.SteamParentalSettings;
        }

        public void SetPersonaName(string pchPersonaName)
        {
            // Save to registry
        }

    }
}

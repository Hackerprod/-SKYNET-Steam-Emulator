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
using Steamworks;

//[Interface.MapAttribute("SteamClient")]
public class SteamClient : IBaseInterface, ISteamClient
{
    public void SetAppId(uint appId)
    {
        SteamEmulator.AppId = appId;
    }

    public int CreateSteamPipe(IntPtr _)
    {
        Write("CreateSteamPipe");
        return (int)SteamEmulator.CreateSteamPipe();
    }

    public bool BReleaseSteamPipe(int hSteamPipe)
    {
        Write("BReleaseSteamPipe");
        return false;
    }

    public int ConnectToGlobalUser(int hSteamPipe)
    {
        if (SteamEmulator.steam_pipes.ContainsKey((HSteamPipe)hSteamPipe))
        {
            return 0;
        }

        SteamEmulator.steam_pipes.Add((HSteamPipe)hSteamPipe, Steam_Pipe.CLIENT);

        return (int)Steam_Pipe.CLIENT;
    }

    public int CreateLocalUser(out int phSteamPipe, EAccountType eAccountType)
    {
        Write("CreateLocalUser");
        phSteamPipe = 1;
        return (int)SteamEmulator.CreateSteamUser();
    }

    public void ReleaseUser(int hSteamPipe, int hUser)
    {
        Write("ReleaseUser");
    }

    public IntPtr GetISteamUser(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamUser");
        return SteamEmulator.SteamUser.BaseAddress;
    }

    public IntPtr GetISteamGameServer(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamGameServer");
        return SteamEmulator.SteamGameServer.BaseAddress;
    }

    public void SetLocalIPBinding(uint unIP, ushort usPort)
    {
        Write("SetLocalIPBinding");
    }

    public IntPtr GetISteamFriends(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamFriends");
        return SteamEmulator.SteamFriends.BaseAddress;
    }

    public IntPtr GetISteamGameSearch(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamFriends");
        return SteamEmulator.SteamGameSearch.BaseAddress;
    }

    public IntPtr GetISteamUtils(int hSteamPipe, string pchVersion)
    {
        Write("GetISteamUtils");
        return SteamEmulator.SteamUtils.BaseAddress;
    }

    public IntPtr GetISteamMatchmaking(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamMatchmaking");
        return SteamEmulator.SteamMatchmaking.BaseAddress;
    }

    public IntPtr GetISteamMatchmakingServers(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamMatchmakingServers");
        return SteamEmulator.SteamMatchMakingServers.BaseAddress;
    }

    public IntPtr GetISteamGenericInterface(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamGenericInterface");
        return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
    }

    public IntPtr GetISteamUserStats(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamUserStats");
        return SteamEmulator.SteamUserStats.BaseAddress;
    }

    public IntPtr GetISteamGameServerStats(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamGameServerStats");
        return SteamEmulator.SteamGameServerStats.BaseAddress;
    }

    public IntPtr GetISteamApps(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamApps");
        return SteamEmulator.SteamApps.BaseAddress;
    }

    public IntPtr GetISteamNetworking(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamNetworking");
        return SteamEmulator.SteamNetworking.BaseAddress;
    }

    public IntPtr GetISteamRemoteStorage(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamRemoteStorage");
        return SteamEmulator.SteamRemoteStorage.BaseAddress;
    }

    public IntPtr GetISteamScreenshots(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamScreenshots");
        return SteamEmulator.SteamScreenshots.BaseAddress;
    }

    public uint GetIPCCallCount(IntPtr _)
    {
        Write("GetIPCCallCount");
        return 0;
    }

    public bool BShutdownIfAllPipesClosed(IntPtr _)
    {
        Write("BShutdownIfAllPipesClosed");
        return false;
    }

    public IntPtr GetISteamHTTP(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamHTTP");
        return SteamEmulator.SteamHTTP.BaseAddress;
    }

    public IntPtr GetISteamController(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamController");
        return SteamEmulator.SteamController.BaseAddress;
    }

    public IntPtr GetISteamUGC(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamUGC");
        return SteamEmulator.SteamUGC.BaseAddress;
    }

    public IntPtr GetISteamAppList(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamAppList");
        return SteamEmulator.SteamAppList.BaseAddress;
    }

    public IntPtr GetISteamMusic(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamMusic");
        return SteamEmulator.SteamMusic.BaseAddress;
    }

    public IntPtr GetISteamMusicRemote(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamMusicRemote");
        return SteamEmulator.SteamMusicRemote.BaseAddress;
    }

    public IntPtr GetISteamInput(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamInput");
        return SteamEmulator.SteamInput.BaseAddress;
    }

    public IntPtr GetISteamHTMLSurface(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamHTMLSurface");
        return SteamEmulator.SteamHTMLSurface.BaseAddress;
    }

    public IntPtr GetISteamParties(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamParties");
        return SteamEmulator.SteamParties.BaseAddress;
    }

    public IntPtr GetISteamInventory(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamInventory");
        return SteamEmulator.SteamInventory.BaseAddress;
    }

    public IntPtr GetISteamRemotePlay(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamRemotePlay");
        return SteamEmulator.SteamRemotePlay.BaseAddress;
    }

    public IntPtr GetISteamVideo(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamVideo");
        return SteamEmulator.SteamVideo.BaseAddress;
    }

    public IntPtr GetISteamParentalSettings(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write("GetISteamParentalSettings");
        return SteamEmulator.SteamParentalSettings.BaseAddress;
    }

    public void SetPersonaName(string pchPersonaName)
    {
        modCommon.Show("SetPersonaName " + pchPersonaName);
    }

    private void Write(string v)
    {
        Main.Write(InterfaceVersion, v);
    }
}


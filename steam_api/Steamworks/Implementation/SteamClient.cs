using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helper;
using SKYNET.Steamworks;
using Steamworks;

public class SteamClient : ISteamInterface
{
    public IntPtr MemoryAddress { get; set; }
    public string InterfaceVersion { get; set; } = "SteamClient";

    public SteamClient()
    {
        InterfaceVersion = "SteamClient";
    }

    public void SetAppId(uint appId)
    {
        Write("SetAppId");
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
        //Write("ConnectToGlobalUser");
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

    public void SetLocalIPBinding(uint unIP, ushort usPort)
    {
        Write("SetLocalIPBinding");
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

    #region Interfaces

    public IntPtr GetISteamUser(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamUser {pchVersion}");
        return InterfaceManager.FindOrCreateInterface(pchVersion);
    }

    public IntPtr GetISteamGameServer(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamGameServer {pchVersion}");
        return SteamEmulator.SteamGameServer.MemoryAddress;
    }

    public IntPtr GetISteamFriends(int user, int pipe, string pchVersion)
    {
        Write($"GetISteamFriends {pchVersion}");
        return SteamEmulator.SteamFriends.MemoryAddress;
    }

    public IntPtr GetISteamGameSearch(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamFriends {pchVersion}");
        return SteamEmulator.SteamGameSearch.MemoryAddress;
    }

    public IntPtr GetISteamUtils(int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamUtils {pchVersion}");
        return SteamEmulator.SteamUtils.MemoryAddress;
    }

    public IntPtr GetISteamMatchmaking(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamMatchmaking {pchVersion}");
        return SteamEmulator.SteamMatchmaking.MemoryAddress;
    }

    public IntPtr GetISteamMatchmakingServers(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamMatchmakingServers {pchVersion}");
        return SteamEmulator.SteamMatchMakingServers.MemoryAddress;
    }

    public IntPtr GetISteamGenericInterface(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamGenericInterface {pchVersion}");
        return InterfaceManager.FindOrCreateInterface(hSteamUser, hSteamPipe, pchVersion);
    }

    public IntPtr GetISteamUserStats(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamUserStats {pchVersion}");
        return SteamEmulator.SteamUserStats.MemoryAddress;
    }

    public IntPtr GetISteamGameServerStats(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamGameServerStats {pchVersion}");
        return SteamEmulator.SteamGameServerStats.MemoryAddress;
    }

    public IntPtr GetISteamApps(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamApps {pchVersion}");
        return SteamEmulator.SteamApps.MemoryAddress;
    }

    public IntPtr GetISteamNetworking(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamNetworking {pchVersion}");
        return SteamEmulator.SteamNetworking.MemoryAddress;
    }

    public IntPtr GetISteamRemoteStorage(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamRemoteStorage {pchVersion}");
        return SteamEmulator.SteamRemoteStorage.MemoryAddress;
    }

    public IntPtr GetISteamScreenshots(int hSteamUser, int hSteamPipe, [MarshalAs(UnmanagedType.LPStr)] string pchVersion)
    {
        Write($"GetISteamScreenshots {pchVersion}");
        return SteamEmulator.SteamScreenshots.MemoryAddress;
    }

    public IntPtr GetISteamHTTP(int hSteamUser, int hSteamPipe, IntPtr pchVersion)
    {
        string version = Marshal.PtrToStringAnsi(pchVersion);
        Write($"GetISteamHTTP {version}");

        return SteamEmulator.SteamHTTP.MemoryAddress;
    }

    public IntPtr GetISteamController(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamController {pchVersion}");
        return SteamEmulator.SteamController.MemoryAddress;
    }

    public IntPtr GetISteamUGC(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamUGC {pchVersion}");
        return SteamEmulator.SteamUGC.MemoryAddress;
    }

    public IntPtr GetISteamAppList(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamAppList {pchVersion}");
        return SteamEmulator.SteamAppList.MemoryAddress;
    }

    public IntPtr GetISteamMusic(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamMusic {pchVersion}");
        return SteamEmulator.SteamMusic.MemoryAddress;
    }

    public IntPtr GetISteamMusicRemote(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamMusicRemote {pchVersion}");
        return SteamEmulator.SteamMusicRemote.MemoryAddress;
    }

    public IntPtr GetISteamInput(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamInput {pchVersion}");
        return SteamEmulator.SteamInput.MemoryAddress;
    }

    public IntPtr GetISteamHTMLSurface(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamHTMLSurface {pchVersion}");
        return SteamEmulator.SteamHTMLSurface.MemoryAddress;
    }

    public IntPtr GetISteamParties(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamParties {pchVersion}");
        return SteamEmulator.SteamParties.MemoryAddress;
    }

    public IntPtr GetISteamInventory(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamInventory {pchVersion}");
        return SteamEmulator.SteamInventory.MemoryAddress;
    }

    public IntPtr GetISteamRemotePlay(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamRemotePlay {pchVersion}");
        return SteamEmulator.SteamRemotePlay.MemoryAddress;
    }

    public IntPtr GetISteamVideo(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamVideo {pchVersion}");
        return SteamEmulator.SteamVideo.MemoryAddress;
    }

    public IntPtr GetISteamParentalSettings(int hSteamUser, int hSteamPipe, string pchVersion)
    {
        Write($"GetISteamParentalSettings {pchVersion}");
        return SteamEmulator.SteamParentalSettings.MemoryAddress;
    }

    #endregion

    public void SetPersonaName(string pchPersonaName)
    {
        Write("pchPersonaName");
    }

    public void SetWarningMessageHook(SteamAPIWarningMessageHook_t pFunction)
    {
        Write("SetWarningMessageHook");
    }

    private void Write(string v)
    {
        SteamEmulator.Write(InterfaceVersion, v);
    }
}


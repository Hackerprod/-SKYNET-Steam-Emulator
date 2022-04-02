using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Steamworks;

public class SteamGameSearch : SteamInterface
{ 
    public GameSearchErrorCode_t AcceptGame(IntPtr self)
    {
        Write("AcceptGame");
        return GameSearchErrorCode_t.OK;
    }

    public GameSearchErrorCode_t AddGameSearchParams(string pchKeyToFind, string pchValuesToFind)
    {
        Write("AddGameSearchParams");
        return GameSearchErrorCode_t.OK;
    }

    public GameSearchErrorCode_t CancelRequestPlayersForGame(IntPtr self)
    {
        Write("CancelRequestPlayersForGame");
        return GameSearchErrorCode_t.OK;
    }

    public GameSearchErrorCode_t DeclineGame(IntPtr self)
    {
        Write("DeclineGame");
        return GameSearchErrorCode_t.OK;
    }

    public GameSearchErrorCode_t EndGame(ulong ullUniqueGameID)
    {
        Write("EndGame");
        return GameSearchErrorCode_t.OK;
    }

    public GameSearchErrorCode_t EndGameSearch(IntPtr self)
    {
        Write("EndGameSearch");
        return GameSearchErrorCode_t.OK;
    }

    public GameSearchErrorCode_t HostConfirmGameStart(ulong ullUniqueGameID)
    {
        Write("HostConfirmGameStart");
        return GameSearchErrorCode_t.OK;
    }

    public GameSearchErrorCode_t RequestPlayersForGame(int nPlayerMin, int nPlayerMax, int nMaxTeamSize)
    {
        Write("RequestPlayersForGame");
        return GameSearchErrorCode_t.OK;
    }

    public GameSearchErrorCode_t RetrieveConnectionDetails(IntPtr steamIDHost, IntPtr pchConnectionDetails, int cubConnectionDetails)
    {
        Write("RetrieveConnectionDetails");
        return GameSearchErrorCode_t.OK;
    }

    public GameSearchErrorCode_t SearchForGameSolo(int nPlayerMin, int nPlayerMax)
    {
        Write("SearchForGameSolo");
        return GameSearchErrorCode_t.OK;
    }

    public GameSearchErrorCode_t SearchForGameWithLobby(IntPtr steamIDLobby, int nPlayerMin, int nPlayerMax)
    {
        Write("SearchForGameWithLobby");
        return GameSearchErrorCode_t.OK;
    }

    public GameSearchErrorCode_t SetConnectionDetails(string pchConnectionDetails, int cubConnectionDetails)
    {
        Write("SetConnectionDetails");
        return GameSearchErrorCode_t.OK;
    }

    public GameSearchErrorCode_t SetGameHostParams(string pchKey, string pchValue)
    {
        Write("SetGameHostParams");
        return GameSearchErrorCode_t.OK;
    }

    public GameSearchErrorCode_t SubmitPlayerResult(ulong ullUniqueGameID, IntPtr steamIDPlayer, PlayerResult_t EPlayerResult)
    {
        Write("SubmitPlayerResult");
        return GameSearchErrorCode_t.OK;
    }

    private void Write(string v)
    {
        Main.Write(InterfaceVersion, v);
    }
}
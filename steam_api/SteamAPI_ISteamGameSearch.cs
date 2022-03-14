using System;
using System.Runtime.InteropServices;
using SKYNET.Helper;
using SKYNET.Interface;

public class SteamAPI_ISteamGameSearch : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_AcceptGame(IntPtr self)
    {
        Log.Write("SteamAPI_ISteamGameSearch_AcceptGame");
        return SteamClient.SteamGameSearch.AcceptGame(self);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_AddGameSearchParams(string pchKeyToFind, string pchValuesToFind)
    {
        Log.Write("SteamAPI_ISteamGameSearch_AddGameSearchParams");
        return SteamClient.SteamGameSearch.AddGameSearchParams(pchKeyToFind, pchValuesToFind);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_CancelRequestPlayersForGame(IntPtr self)
    {
        Log.Write("SteamAPI_ISteamGameSearch_CancelRequestPlayersForGame");
        return SteamClient.SteamGameSearch.CancelRequestPlayersForGame(self);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_DeclineGame(IntPtr self)
    {
        Log.Write("SteamAPI_ISteamGameSearch_DeclineGame");
        return SteamClient.SteamGameSearch.DeclineGame(self);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_EndGame(ulong ullUniqueGameID)
    {
        Log.Write("SteamAPI_ISteamGameSearch_EndGame");
        return SteamClient.SteamGameSearch.HostConfirmGameStart(ullUniqueGameID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_EndGameSearch(IntPtr self)
    {
        Log.Write("SteamAPI_ISteamGameSearch_EndGameSearch");
        return SteamClient.SteamGameSearch.EndGameSearch(self);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_HostConfirmGameStart(ulong ullUniqueGameID)
    {
        Log.Write("SteamAPI_ISteamGameSearch_HostConfirmGameStart");
        return SteamClient.SteamGameSearch.HostConfirmGameStart(ullUniqueGameID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_RequestPlayersForGame(int nPlayerMin, int nPlayerMax, int nMaxTeamSize)
    {
        Log.Write("SteamAPI_ISteamGameSearch_RequestPlayersForGame");
        return SteamClient.SteamGameSearch.RequestPlayersForGame(nPlayerMin, nPlayerMax, nMaxTeamSize);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_RetrieveConnectionDetails(IntPtr steamIDHost, IntPtr pchConnectionDetails, int cubConnectionDetails)
    {
        Log.Write("SteamAPI_ISteamGameSearch_RetrieveConnectionDetails");
        return SteamClient.SteamGameSearch.RetrieveConnectionDetails(steamIDHost, pchConnectionDetails, cubConnectionDetails);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_SearchForGameSolo(int nPlayerMin, int nPlayerMax)
    {
        Log.Write("SteamAPI_ISteamGameSearch_SearchForGameSolo");
        return SteamClient.SteamGameSearch.SearchForGameSolo(nPlayerMin, nPlayerMax);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_SearchForGameWithLobby(IntPtr steamIDLobby, int nPlayerMin, int nPlayerMax)
    {
        Log.Write("SteamAPI_ISteamGameSearch_SearchForGameWithLobby");
        return SteamClient.SteamGameSearch.SearchForGameWithLobby(steamIDLobby, nPlayerMin, nPlayerMax);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_SetConnectionDetails(string pchConnectionDetails, int cubConnectionDetails)
    {
        Log.Write("SteamAPI_ISteamGameSearch_SetConnectionDetails");
        return SteamClient.SteamGameSearch.SetConnectionDetails(pchConnectionDetails, cubConnectionDetails);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_SetGameHostParams(string pchKey, string pchValue)
    {
        Log.Write("SteamAPI_ISteamGameSearch_SetGameHostParams");
        return SteamClient.SteamGameSearch.SetGameHostParams(pchKey, pchValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_SubmitPlayerResult(ulong ullUniqueGameID, IntPtr steamIDPlayer, PlayerResult_t EPlayerResult)
    {
        Log.Write("SteamAPI_ISteamGameSearch_SubmitPlayerResult");
        return SteamClient.SteamGameSearch.SubmitPlayerResult(ullUniqueGameID, steamIDPlayer, EPlayerResult);
    }
}

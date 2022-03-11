using System;
using System.Runtime.InteropServices;
using SKYNET.Helper;
using SKYNET.Interface;

namespace SKYNET.Managers
{
    public class SteamAPI_ISteamGameSearch : BaseCalls
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public  static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_AcceptGame(IntPtr self)
        {
            Log.Write("SteamAPI_ISteamGameSearch_AcceptGame");
            return SteamClient.steam_GameSearch.AcceptGame(self);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public  static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_AddGameSearchParams(string pchKeyToFind, string pchValuesToFind)
        {
            Log.Write("SteamAPI_ISteamGameSearch_AddGameSearchParams");
            return SteamClient.steam_GameSearch.AddGameSearchParams(pchKeyToFind, pchValuesToFind);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public  static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_CancelRequestPlayersForGame(IntPtr self)
        {
            Log.Write("SteamAPI_ISteamGameSearch_CancelRequestPlayersForGame");
            return SteamClient.steam_GameSearch.CancelRequestPlayersForGame(self);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public  static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_DeclineGame(IntPtr self)
        {
            Log.Write("SteamAPI_ISteamGameSearch_DeclineGame");
            return SteamClient.steam_GameSearch.DeclineGame(self);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public  static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_EndGame(ulong ullUniqueGameID)
        {
            Log.Write("SteamAPI_ISteamGameSearch_EndGame");
            return SteamClient.steam_GameSearch.HostConfirmGameStart(ullUniqueGameID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public  static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_EndGameSearch(IntPtr self)
        {
            Log.Write("SteamAPI_ISteamGameSearch_EndGameSearch");
            return SteamClient.steam_GameSearch.EndGameSearch(self);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public  static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_HostConfirmGameStart(ulong ullUniqueGameID)
        {
            Log.Write("SteamAPI_ISteamGameSearch_HostConfirmGameStart");
            return SteamClient.steam_GameSearch.HostConfirmGameStart(ullUniqueGameID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public  static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_RequestPlayersForGame(int nPlayerMin, int nPlayerMax, int nMaxTeamSize)
        {
            Log.Write("SteamAPI_ISteamGameSearch_RequestPlayersForGame");
            return SteamClient.steam_GameSearch.RequestPlayersForGame(nPlayerMin, nPlayerMax, nMaxTeamSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public  static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_RetrieveConnectionDetails(IntPtr steamIDHost, IntPtr pchConnectionDetails, int cubConnectionDetails)
        {
            Log.Write("SteamAPI_ISteamGameSearch_RetrieveConnectionDetails");
            return SteamClient.steam_GameSearch.RetrieveConnectionDetails(steamIDHost, pchConnectionDetails, cubConnectionDetails);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public  static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_SearchForGameSolo(int nPlayerMin, int nPlayerMax)
        {
            Log.Write("SteamAPI_ISteamGameSearch_SearchForGameSolo");
            return SteamClient.steam_GameSearch.SearchForGameSolo(nPlayerMin, nPlayerMax);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public  static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_SearchForGameWithLobby(IntPtr steamIDLobby, int nPlayerMin, int nPlayerMax)
        {
            Log.Write("SteamAPI_ISteamGameSearch_SearchForGameWithLobby");
            return SteamClient.steam_GameSearch.SearchForGameWithLobby(steamIDLobby, nPlayerMin, nPlayerMax);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public  static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_SetConnectionDetails(string pchConnectionDetails, int cubConnectionDetails)
        {
            Log.Write("SteamAPI_ISteamGameSearch_SetConnectionDetails");
            return SteamClient.steam_GameSearch.SetConnectionDetails(pchConnectionDetails, cubConnectionDetails);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public  static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_SetGameHostParams(string pchKey, string pchValue)
        {
            Log.Write("SteamAPI_ISteamGameSearch_SetGameHostParams");
            return SteamClient.steam_GameSearch.SetGameHostParams(pchKey, pchValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public  static GameSearchErrorCode_t SteamAPI_ISteamGameSearch_SubmitPlayerResult(ulong ullUniqueGameID, IntPtr steamIDPlayer, PlayerResult_t EPlayerResult)
        {
            Log.Write("SteamAPI_ISteamGameSearch_SubmitPlayerResult");
            return SteamClient.steam_GameSearch.SubmitPlayerResult(ullUniqueGameID, steamIDPlayer, EPlayerResult);
        }
    }
}
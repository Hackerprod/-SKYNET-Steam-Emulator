using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamGameSearch
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamGameSearch_AcceptGame(IntPtr _)
        {
            Write("SteamAPI_ISteamGameSearch_AcceptGame");
            return SteamEmulator.SteamGameSearch.AcceptGame();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamGameSearch_AddGameSearchParams(IntPtr _, string pchKeyToFind, string pchValuesToFind)
        {
            Write("SteamAPI_ISteamGameSearch_AddGameSearchParams");
            return SteamEmulator.SteamGameSearch.AddGameSearchParams(pchKeyToFind, pchValuesToFind);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamGameSearch_CancelRequestPlayersForGame(IntPtr _)
        {
            Write("SteamAPI_ISteamGameSearch_CancelRequestPlayersForGame");
            return SteamEmulator.SteamGameSearch.CancelRequestPlayersForGame();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamGameSearch_DeclineGame(IntPtr _)
        {
            Write("SteamAPI_ISteamGameSearch_DeclineGame");
            return SteamEmulator.SteamGameSearch.DeclineGame();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamGameSearch_EndGame(IntPtr _, ulong ullUniqueGameID)
        {
            Write("SteamAPI_ISteamGameSearch_EndGame");
            return SteamEmulator.SteamGameSearch.HostConfirmGameStart(ullUniqueGameID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamGameSearch_EndGameSearch(IntPtr _)
        {
            Write("SteamAPI_ISteamGameSearch_EndGameSearch");
            return SteamEmulator.SteamGameSearch.EndGameSearch();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamGameSearch_HostConfirmGameStart(IntPtr _, ulong ullUniqueGameID)
        {
            Write("SteamAPI_ISteamGameSearch_HostConfirmGameStart");
            return SteamEmulator.SteamGameSearch.HostConfirmGameStart(ullUniqueGameID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamGameSearch_RequestPlayersForGame(IntPtr _, int nPlayerMin, int nPlayerMax, int nMaxTeamSize)
        {
            Write("SteamAPI_ISteamGameSearch_RequestPlayersForGame");
            return SteamEmulator.SteamGameSearch.RequestPlayersForGame(nPlayerMin, nPlayerMax, nMaxTeamSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamGameSearch_RetrieveConnectionDetails(IntPtr _, ulong steamIDHost, IntPtr pchConnectionDetails, int cubConnectionDetails)
        {
            Write("SteamAPI_ISteamGameSearch_RetrieveConnectionDetails");
            return SteamEmulator.SteamGameSearch.RetrieveConnectionDetails(steamIDHost, pchConnectionDetails, cubConnectionDetails);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamGameSearch_SearchForGameSolo(IntPtr _, int nPlayerMin, int nPlayerMax)
        {
            Write("SteamAPI_ISteamGameSearch_SearchForGameSolo");
            return SteamEmulator.SteamGameSearch.SearchForGameSolo(nPlayerMin, nPlayerMax);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamGameSearch_SearchForGameWithLobby(IntPtr _, ulong steamIDLobby, int nPlayerMin, int nPlayerMax)
        {
            Write("SteamAPI_ISteamGameSearch_SearchForGameWithLobby");
            return SteamEmulator.SteamGameSearch.SearchForGameWithLobby(steamIDLobby, nPlayerMin, nPlayerMax);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamGameSearch_SetConnectionDetails(IntPtr _, string pchConnectionDetails, int cubConnectionDetails)
        {
            Write("SteamAPI_ISteamGameSearch_SetConnectionDetails");
            return SteamEmulator.SteamGameSearch.SetConnectionDetails(pchConnectionDetails, cubConnectionDetails);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamGameSearch_SetGameHostParams(IntPtr _, string pchKey, string pchValue)
        {
            Write("SteamAPI_ISteamGameSearch_SetGameHostParams");
            return SteamEmulator.SteamGameSearch.SetGameHostParams(pchKey, pchValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamGameSearch_SubmitPlayerResult(IntPtr _, ulong ullUniqueGameID, ulong steamIDPlayer, int EPlayerResult)
        {
            Write("SteamAPI_ISteamGameSearch_SubmitPlayerResult");
            return SteamEmulator.SteamGameSearch.SubmitPlayerResult(ullUniqueGameID, steamIDPlayer, EPlayerResult);
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}

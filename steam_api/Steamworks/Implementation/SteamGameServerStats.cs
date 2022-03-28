using SKYNET;
using Steamworks;
using System;

public class SteamGameServerStats : SteamInterface
{
    public bool ClearUserAchievement(IntPtr steamIDUser, string pchName)
    {
        Write("ClearUserAchievement");
        return false;
    }

    public bool GetUserAchievement(IntPtr steamIDUser, string pchName, bool pbAchieved)
    {
        Write("GetUserAchievement");
        return false;
    }

    public bool GetUserStat(IntPtr steamIDUser, string pchName, int pData)
    {
        Write("GetUserStat");
        return false;
    }

    public SteamAPICall_t RequestUserStats(IntPtr steamIDUser)
    {
        Write("RequestUserStats");
        //GSStatsReceived_t data;
        //data.m_eResult = 0;//k_EResultOK;
        //data.m_steamIDUser = steamIDUser;
        return SteamAPICall_t.Invalid;
    }

    public bool SetUserAchievement(IntPtr steamIDUser, string pchName)
    {
        Write("SetUserAchievement");
        return false;
    }

    public bool SetUserStat(IntPtr steamIDUser, string pchName, int nData)
    {
        Write("SetUserStat");
        return false;
    }

    public SteamAPICall_t StoreUserStats(IntPtr steamIDUser)
    {
        Write("StoreUserStats");
        return SteamAPICall_t.Invalid;
    }

    public bool UpdateUserAvgRateStat(IntPtr steamIDUser, string pchName, float flCountThisSession, double dSessionLength)
    {
        Write("UpdateUserAvgRateStat");
        return false;
    }

    private void Write(string v)
    {
        Main.Write(InterfaceVersion, v);
    }
}
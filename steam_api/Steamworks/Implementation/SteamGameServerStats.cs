using SKYNET.Callback;
using SKYNET.Managers;
using SKYNET.Steamworks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamGameServerStats : ISteamInterface
    {
        public static SteamGameServerStats Instance;

        public SteamGameServerStats()
        {
            Instance = this;
            InterfaceName = "SteamGameServerStats";
            InterfaceVersion = "SteamGameServerStats001";
        }

        public bool ClearUserAchievement(ulong steamIDUser, string pchName)
        {
            Write($"ClearUserAchievement ({steamIDUser}, {pchName})");
            SkyNetStateCache.UpsertAchievement(steamIDUser, pchName, false, 0, 0, false);
            return true;
        }

        public bool GetUserAchievement(ulong steamIDUser, string pchName, bool pbAchieved)
        {
            Write($"GetUserAchievement (SteamID: {steamIDUser}, Name: {pchName})");
            return SkyNetStateCache.GetAchievements(steamIDUser).Any(a => a.Name == pchName && a.Earned);
        }

        public bool GetUserStat(ulong steamIDUser, string pchName, float pData)
        {
            Write($"GetUserStat ({steamIDUser}, {pchName})");
            return SkyNetStateCache.GetStats(steamIDUser).Any(s => s.Name == pchName);
        }

        public bool GetUserStat(ulong steamIDUser, string pchName, int pData)
        {
            Write($"GetUserStat ({steamIDUser}, {pchName})");
            return SkyNetStateCache.GetStats(steamIDUser).Any(s => s.Name == pchName);
        }

        public SteamAPICall_t RequestUserStats(ulong steamIDUser)
        {
            Write($"RequestUserStats {steamIDUser}");

            var result = EResult.k_EResultFail;
            if (SkyNetApiClient.IsEnabled)
            {
                var envelope = SkyNetApiClient.RequestGameServerUserStats(steamIDUser);
                if (envelope != null)
                {
                    SkyNetStateCache.ApplyStats(steamIDUser, envelope.Stats);
                    SkyNetStateCache.ApplyAchievements(steamIDUser, envelope.Achievements);
                    result = EResult.k_EResultOK;
                }
            }

            return CallbackManager.AddCallbackResultGameServer(new GSStatsReceived_t
            {
                Result = result,
                SteamIDUser = steamIDUser
            });
        }

        public bool SetUserAchievement(ulong steamIDUser, string pchName)
        {
            Write($"SetUserAchievement ({steamIDUser}, {pchName})");
            SkyNetStateCache.UpsertAchievement(steamIDUser, pchName, true, 0, 0, false);
            return true;
        }

        public bool SetUserStat(ulong steamIDUser, string pchName, float nData)
        {
            Write($"SetUserStat ({steamIDUser}, {pchName}, {nData})");
            SkyNetStateCache.UpsertStat(steamIDUser, pchName, (uint)Math.Max(0, (int)nData), false);
            return true;
        }

        public bool SetUserStat(ulong steamIDUser, string pchName, int nData)
        {
            Write($"SetUserStat ({steamIDUser}, {pchName}, {nData})");
            SkyNetStateCache.UpsertStat(steamIDUser, pchName, (uint)Math.Max(0, nData), false);
            return true;
        }

        public SteamAPICall_t StoreUserStats(ulong steamIDUser)
        {
            Write($"StoreUserStats {steamIDUser}");

            var result = EResult.k_EResultFail;
            if (SkyNetApiClient.IsEnabled)
            {
                result = SkyNetApiClient.StoreGameServerUserStats(
                    steamIDUser,
                    SkyNetStateCache.GetStats(steamIDUser).ConvertAll(s => new SkyNetApiClient.SkyNetStatDto
                    {
                        Name = s.Name,
                        Data = s.Data
                    }),
                    SkyNetStateCache.GetAchievements(steamIDUser).ConvertAll(a => new SkyNetApiClient.SkyNetAchievementDto
                    {
                        Name = a.Name,
                        Earned = a.Earned,
                        Date = a.Date,
                        Progress = a.Progress,
                        MaxProgress = a.MaxProgress
                    }))
                    ? EResult.k_EResultOK
                    : EResult.k_EResultFail;
            }

            return CallbackManager.AddCallbackResultGameServer(new GSStatsStored_t
            {
                Result = result,
                SteamIDUser = steamIDUser
            });
        }

        public bool UpdateUserAvgRateStat(ulong steamIDUser, string pchName, float flCountThisSession, double dSessionLength)
        {
            Write($"UpdateUserAvgRateStat ({steamIDUser}, {pchName})");
            if (dSessionLength <= 0)
            {
                return false;
            }

            var average = (uint)Math.Max(0, (int)(flCountThisSession / dSessionLength));
            SkyNetStateCache.UpsertStat(steamIDUser, pchName, average, false);
            return true;
        }
    }
}

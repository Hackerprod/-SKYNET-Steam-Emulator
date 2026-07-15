using SKYNET.Callback;
using SKYNET.Managers;
using SKYNET.Steamworks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

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
            StateCache.UpsertAchievement(steamIDUser, pchName, false, 0, 0, false);
            return true;
        }

        public bool GetUserAchievement(ulong steamIDUser, string pchName, bool pbAchieved)
        {
            Write($"GetUserAchievement (SteamID: {steamIDUser}, Name: {pchName})");
            return StateCache.GetAchievements(steamIDUser).Any(a => a.Name == pchName && a.Earned);
        }

        public bool GetUserAchievement(ulong steamIDUser, string pchName, IntPtr pbAchieved)
        {
            Write($"GetUserAchievement (SteamID: {steamIDUser}, Name: {pchName})");
            bool achieved = StateCache.GetAchievements(steamIDUser).Any(a => a.Name == pchName && a.Earned);
            WriteBool(pbAchieved, achieved);
            return true;
        }

        public bool GetUserStat(ulong steamIDUser, string pchName, float pData)
        {
            Write($"GetUserStat ({steamIDUser}, {pchName})");
            return StateCache.GetStats(steamIDUser).Any(s => s.Name == pchName);
        }

        public bool GetUserStatFloat(ulong steamIDUser, string pchName, IntPtr pData)
        {
            Write($"GetUserStatFloat ({steamIDUser}, {pchName})");
            var stat = StateCache.GetStats(steamIDUser).FirstOrDefault(s => s.Name == pchName);
            WriteSingle(pData, stat?.Data ?? 0);
            return stat != null;
        }

        public bool GetUserStat(ulong steamIDUser, string pchName, int pData)
        {
            Write($"GetUserStat ({steamIDUser}, {pchName})");
            return StateCache.GetStats(steamIDUser).Any(s => s.Name == pchName);
        }

        public bool GetUserStatInt32(ulong steamIDUser, string pchName, IntPtr pData)
        {
            Write($"GetUserStatInt32 ({steamIDUser}, {pchName})");
            var stat = StateCache.GetStats(steamIDUser).FirstOrDefault(s => s.Name == pchName);
            WriteInt32(pData, unchecked((int)(stat?.Data ?? 0)));
            return stat != null;
        }

        public SteamAPICall_t RequestUserStats(ulong steamIDUser)
        {
            Write($"RequestUserStats {steamIDUser}");

            return WorkQueue.EnqueueCallbackResult(
                new GSStatsReceived_t
                {
                    Result = EResult.k_EResultFail,
                    SteamIDUser = steamIDUser
                },
                () =>
                {
                    var result = EResult.k_EResultFail;
                    if (SkyNetApiClient.IsEnabled)
                    {
                        var envelope = SkyNetApiClient.RequestGameServerUserStats(steamIDUser);
                        if (envelope != null)
                        {
                            StateCache.ApplyStats(steamIDUser, envelope.Stats);
                            StateCache.ApplyAchievements(steamIDUser, envelope.Achievements);
                            result = EResult.k_EResultOK;
                        }
                    }

                    return new GSStatsReceived_t
                    {
                        Result = result,
                        SteamIDUser = steamIDUser
                    };
                },
                true,
                "GameServer request user stats",
                "gsstats:request:" + steamIDUser,
                true);
        }

        public bool SetUserAchievement(ulong steamIDUser, string pchName)
        {
            Write($"SetUserAchievement ({steamIDUser}, {pchName})");
            StateCache.UpsertAchievement(steamIDUser, pchName, true, 0, 0, false);
            return true;
        }

        public bool SetUserStat(ulong steamIDUser, string pchName, float nData)
        {
            Write($"SetUserStat ({steamIDUser}, {pchName}, {nData})");
            StateCache.UpsertStat(steamIDUser, pchName, (uint)Math.Max(0, (int)nData), false);
            return true;
        }

        public bool SetUserStat(ulong steamIDUser, string pchName, int nData)
        {
            Write($"SetUserStat ({steamIDUser}, {pchName}, {nData})");
            StateCache.UpsertStat(steamIDUser, pchName, (uint)Math.Max(0, nData), false);
            return true;
        }

        public SteamAPICall_t StoreUserStats(ulong steamIDUser)
        {
            Write($"StoreUserStats {steamIDUser}");

            return WorkQueue.EnqueueCallbackResult(
                new GSStatsStored_t
                {
                    Result = EResult.k_EResultFail,
                    SteamIDUser = steamIDUser
                },
                () =>
                {
                    var result = EResult.k_EResultFail;
                    if (SkyNetApiClient.IsEnabled)
                    {
                        result = SkyNetApiClient.StoreGameServerUserStats(
                            steamIDUser,
                            StateCache.GetStats(steamIDUser).ConvertAll(s => new SkyNetApiClient.ApiStat
                            {
                                Name = s.Name,
                                Data = s.Data
                            }),
                            StateCache.GetAchievements(steamIDUser).ConvertAll(a => new SkyNetApiClient.ApiAchievement
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

                    return new GSStatsStored_t
                    {
                        Result = result,
                        SteamIDUser = steamIDUser
                    };
                },
                true,
                "GameServer store user stats",
                "gsstats:store:" + steamIDUser,
                true);
        }

        public bool UpdateUserAvgRateStat(ulong steamIDUser, string pchName, float flCountThisSession, double dSessionLength)
        {
            Write($"UpdateUserAvgRateStat ({steamIDUser}, {pchName})");
            if (dSessionLength <= 0)
            {
                return false;
            }

            var average = (uint)Math.Max(0, (int)(flCountThisSession / dSessionLength));
            StateCache.UpsertStat(steamIDUser, pchName, average, false);
            return true;
        }

        private static void WriteBool(IntPtr destination, bool value)
        {
            if (destination != IntPtr.Zero)
            {
                Marshal.WriteByte(destination, value ? (byte)1 : (byte)0);
            }
        }

        private static void WriteInt32(IntPtr destination, int value)
        {
            if (destination != IntPtr.Zero)
            {
                Marshal.WriteInt32(destination, value);
            }
        }

        private static void WriteSingle(IntPtr destination, float value)
        {
            if (destination != IntPtr.Zero)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                Marshal.Copy(bytes, 0, destination, bytes.Length);
            }
        }
    }
}

using SKYNET.Common;
using SKYNET.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Managers
{
    public class StatsManager
    {
        private static ConcurrentDictionary<uint, List<Leaderboard>> Leaderboards;
        private static ConcurrentDictionary<uint, List<Achievement>> Achievements;
        private static ConcurrentDictionary<uint, List<PlayerStat>> PlayerStats;

        static StatsManager()
        {
            Leaderboards = new ConcurrentDictionary<uint, List<Leaderboard>>();
            Achievements = new ConcurrentDictionary<uint, List<Achievement>>();
            PlayerStats = new ConcurrentDictionary<uint, List<PlayerStat>>();
        }

        public static void Initialize()
        {
            string achievementsPath = Path.Combine(modCommon.GetPath(), "SKYNET", "Storage", "Achievements.json");
            //if (File.Exists(achievementsPath))
            //{
            //    string fileContent = File.ReadAllText(achievementsPath);
            //    Achievements = fileContent.FromJson<List<Achievement>>();
            //}

            //string UserStatsPath = Path.Combine(modCommon.GetPath(), "SKYNET", "Storage", "UserStats.json");
            //if (File.Exists(UserStatsPath))
            //{
            //    string fileContent = File.ReadAllText(UserStatsPath);
            //    var StatsList = fileContent.FromJson<List<PlayerStat>>();
            //    UserStats.TryAdd((ulong)SteamEmulator.SteamID, StatsList);
            //}
            //else
            //{
            //    UserStats.TryAdd((ulong)SteamEmulator.SteamID, new List<PlayerStat>());
            //}

        }
    }
}

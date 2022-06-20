using SKYNET.Common;
using SKYNET.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Managers
{
    public class StatsManager
    {
        private static ConcurrentDictionary<uint, List<Leaderboard>> Leaderboards;
        private static ConcurrentDictionary<uint, List<Achievement>> Achievements;
        private static ConcurrentDictionary<uint, List<PlayerStat>>  PlayerStats;

        static StatsManager()
        {
            Leaderboards = new ConcurrentDictionary<uint, List<Leaderboard>>();
            Achievements = new ConcurrentDictionary<uint, List<Achievement>>();
            PlayerStats =  new ConcurrentDictionary<uint, List<PlayerStat>>();
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

        public static void GenerateAchievements(uint app_id, string steam_apikey)
        {
            string URL = $"https://api.steampowered.com/ISteamUserStats/GetSchemaForGame/v2/?key={steam_apikey}&appid={app_id}";
            string achievementsPath = Path.Combine(modCommon.GetPath(), "Data", "Storage", app_id.ToString(), "Achievements.json");
            modCommon.EnsureDirectoryExists(achievementsPath, true);

            try
            {
                WebRequest webrequest = HttpWebRequest.Create(URL);
                webrequest.Method = "GET";
                HttpWebResponse response = (HttpWebResponse)webrequest.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string content = reader.ReadToEnd();
                File.WriteAllText(achievementsPath, content);
            }
            catch 
            {

            }
        }

        public static void GenerateItems(uint app_id)
        {
            //string URL = $"https://api.steampowered.com/IInventoryService/GetItemDefMeta/v1?key={steam_apikey}&appid={app_id}";
            string URL = $"https://api.steampowered.com/IGameInventory/GetItemDefArchive/v0001?appid={app_id}";
            string achievementsPath = Path.Combine(modCommon.GetPath(), "Data", "Storage", app_id.ToString(), "Items.json");
            modCommon.EnsureDirectoryExists(achievementsPath, true);

            try
            {
                WebRequest webrequest = HttpWebRequest.Create(URL);
                webrequest.Method = "GET";
                HttpWebResponse response = (HttpWebResponse)webrequest.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string content = reader.ReadToEnd();
                File.WriteAllText(achievementsPath, content);
            }
            catch
            {

            }
        }

        public static void GenerateDLCs(uint app_id)
        {
            string URL = $"https://store.steampowered.com/api/appdetails/?appids={app_id}";
            string achievementsPath = Path.Combine(modCommon.GetPath(), "Data", "Storage", app_id.ToString(), "DLCs.json");
            modCommon.EnsureDirectoryExists(achievementsPath, true);

            try
            {
                WebRequest webrequest = HttpWebRequest.Create(URL);
                webrequest.Method = "GET";
                HttpWebResponse response = (HttpWebResponse)webrequest.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string content = reader.ReadToEnd();
                File.WriteAllText(achievementsPath, content);
            }
            catch
            {

            }
        }

        public static void GenerateAppDetails(uint app_id)
        {
            string URL = $"https://store.steampowered.com/api/appdetails/?appids={app_id}";
            string achievementsPath = Path.Combine(modCommon.GetPath(), "Data", "Storage", app_id.ToString(), "AppDetails.json");
            modCommon.EnsureDirectoryExists(achievementsPath, true);

            try
            {
                WebRequest webrequest = HttpWebRequest.Create(URL);
                webrequest.Method = "GET";
                HttpWebResponse response = (HttpWebResponse)webrequest.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string content = reader.ReadToEnd();
                File.WriteAllText(achievementsPath, content);
            }
            catch
            {

            }
        }
    }
}

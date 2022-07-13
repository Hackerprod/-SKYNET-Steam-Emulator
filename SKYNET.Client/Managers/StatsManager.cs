using SKYNET.Helper;
using SKYNET.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SKYNET.Managers
{
    public class StatsManager
    {
        private static ConcurrentDictionary<uint, List<Leaderboard>> Leaderboards;
        private static ConcurrentDictionary<uint, List<Achievement>> Achievements;
        private static ConcurrentDictionary<uint, List<PlayerStat>> PlayerStats;
        private const string SteamAPIKey = "96BA415C674509039C4C058B99E30F9D";

        private static ConcurrentDictionary<uint, AppDetails> AppDetails;
        private static ConcurrentDictionary<uint, GameSchema> GameSchemas;

        private static WebClient WebClient;
        private static string StoragePath;
        private static string AppCachePath;
        private static int DownloadID;

        static StatsManager()
        {
            Leaderboards = new ConcurrentDictionary<uint, List<Leaderboard>>();
            Achievements = new ConcurrentDictionary<uint, List<Achievement>>();
            PlayerStats = new ConcurrentDictionary<uint, List<PlayerStat>>();
            AppDetails = new ConcurrentDictionary<uint, AppDetails>();
            GameSchemas = new ConcurrentDictionary<uint, GameSchema>();
            WebClient = new WebClient();
        }

        public static void Initialize()
        {
            StoragePath = Path.Combine(modCommon.GetPath(), "Data", "Storage");
            AppCachePath = Path.Combine(modCommon.GetPath(), "Data", "Images", "AppCache");

            modCommon.EnsureDirectoryExists(StoragePath);
            modCommon.EnsureDirectoryExists(AppCachePath);

            foreach (var directory in Directory.GetDirectories(StoragePath))
            {
                var appID = new DirectoryInfo(directory).Name; 
                foreach (var file in Directory.GetFiles(directory))
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    try
                    {
                        switch (fileName)
                        {
                            case "Achievements":
                                {
                                    if (uint.TryParse(appID, out var AppID))
                                    {
                                        string fileContent = File.ReadAllText(file);
                                        var achievements = new JavaScriptSerializer().Deserialize<List<Achievement>>(fileContent);
                                        if (achievements != null)
                                        {
                                            Achievements.TryAdd(AppID, achievements);
                                        }
                                    }
                                }
                                break;
                            case "Leaderboards":
                                {
                                    if (uint.TryParse(appID, out var AppID))
                                    {
                                        string fileContent = File.ReadAllText(file);
                                        var leaderboards = new JavaScriptSerializer().Deserialize<List<Leaderboard>>(fileContent);
                                        if (leaderboards != null)
                                        {
                                            Leaderboards.TryAdd(AppID, leaderboards);
                                        }
                                    }
                                }
                                break;
                            case "PlayerStats":
                                {
                                    if (uint.TryParse(appID, out var AppID))
                                    {
                                        string fileContent = File.ReadAllText(file);
                                        var playerStats = new JavaScriptSerializer().Deserialize<List<PlayerStat>>(fileContent);
                                        if (playerStats != null)
                                        {
                                            PlayerStats.TryAdd(AppID, playerStats);
                                        }
                                    }
                                }
                                break;
                            case "AppDetails":
                                {
                                    if (uint.TryParse(appID, out var AppID))
                                    {
                                        string fileContent = File.ReadAllText(file);
                                        var shema = new JavaScriptSerializer().Deserialize<dynamic>(fileContent);
                                        AppDetails appDetails = new JavaScriptSerializer().ConvertToType<AppDetails>(shema[appID]);
                                        if (appDetails != null)
                                        {
                                            AppDetails.TryAdd(AppID, appDetails);
                                        }
                                    }
                                }
                                break;
                            case "GameSchema":
                                {
                                    if (uint.TryParse(appID, out var AppID))
                                    {
                                        string fileContent = File.ReadAllText(file);
                                        var gameSchema = new JavaScriptSerializer().Deserialize<GameSchema>(fileContent);
                                        if (gameSchema != null)
                                        {
                                            GameSchemas.TryAdd(AppID, gameSchema);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    catch 
                    {

                    }
                }
            }
        }

        public static void SetAchievement(uint appID, Achievement achievement)
        {
            MutexHelper.Wait("Achievements", delegate
            {
                if (Achievements.TryGetValue(appID, out var achievements))
                {
                    achievements.Add(achievement);
                }
                else
                {
                    Achievements.TryAdd(appID, new List<Achievement>() { achievement });
                }
            });

            SaveAchievements();
        }

        public static void SetLeaderboard(uint appID, Leaderboard leaderboard)
        {
            MutexHelper.Wait("Leaderboards", delegate
            {
                if (Leaderboards.TryGetValue(appID, out var leaderboards))
                {
                    leaderboards.Add(leaderboard);
                }
                else
                {
                    Leaderboards.TryAdd(appID, new List<Leaderboard>() { leaderboard });
                }
            });

            SaveLeaderboards();
        }

        public static void SetPlayerStat(uint appID, PlayerStat playerStat)
        {
            MutexHelper.Wait("PlayerStats", delegate
            {
                if (PlayerStats.TryGetValue(appID, out var achievements))
                {
                    achievements.Add(playerStat);
                }
                else
                {
                    PlayerStats.TryAdd(appID, new List<PlayerStat>() { playerStat });
                }
            });

            SavePlayerStats();
        }

        private static void SaveAchievements()
        {
            MutexHelper.Wait("Achievements", delegate
            {
                foreach (var KV in Achievements)
                {
                    try
                    {
                        var AppID = KV.Key.ToString();
                        var achievements = KV.Value;
                        var filePath = Path.Combine(modCommon.GetPath(), "Data", "Storage", AppID, "Achievements.json");
                        modCommon.EnsureDirectoryExists(filePath, true);
                        var JSON = new JavaScriptSerializer().Serialize(achievements);
                        File.WriteAllText(filePath, JSON);
                    }
                    catch
                    {
                    }
                }
            });
        }

        private static void SaveLeaderboards()
        {
            MutexHelper.Wait("Leaderboards", delegate
            {
                foreach (var KV in Leaderboards)
                {
                    try
                    {
                        var AppID = KV.Key.ToString();
                        var leaderboards = KV.Value;
                        var filePath = Path.Combine(modCommon.GetPath(), "Data", "Storage", AppID, "Leaderboards.json");
                        modCommon.EnsureDirectoryExists(filePath, true);
                        var JSON = new JavaScriptSerializer().Serialize(leaderboards);
                        File.WriteAllText(filePath, JSON);
                    }
                    catch
                    {
                    }
                }
            });
        }

        private static void SavePlayerStats()
        {
            MutexHelper.Wait("PlayerStats", delegate
            {
                foreach (var KV in PlayerStats)
                {
                    try
                    {
                        var AppID = KV.Key.ToString();
                        var playerStats = KV.Value;
                        var filePath = Path.Combine(modCommon.GetPath(), "Data", "Storage", AppID, "PlayerStats.json");
                        modCommon.EnsureDirectoryExists(filePath, true);
                        var JSON = new JavaScriptSerializer().Serialize(playerStats);
                        File.WriteAllText(filePath, JSON);
                    }
                    catch
                    {
                    }
                }
            });
        }

        public static List<Achievement> GetAchievements(uint appID)
        {
            if (Achievements.TryGetValue(appID, out var achievements))
            {
                return achievements;
            }
            return new List<Achievement>();
        }

        public static List<Leaderboard> GetLeaderboards(uint appID)
        {
            if (Leaderboards.TryGetValue(appID, out var leaderboards))
            {
                return leaderboards;
            }
            return new List<Leaderboard>();
        }

        public static List<PlayerStat> GetPlayerStats(uint appID)
        {
            if (PlayerStats.TryGetValue(appID, out var playerStats))
            {
                return playerStats;
            }
            return new List<PlayerStat>();
        }

        public static void UpdateAchievement(uint appID, Achievement achievement)
        {
            MutexHelper.Wait("Achievements", delegate
            {
                if (Achievements.TryGetValue(appID, out var achievements))
                {
                    var toUpdate = achievements.Find(a => a.Name == achievement.Name);
                    if (toUpdate != null)
                    {
                        toUpdate.Date = achievement.Date;
                        toUpdate.Earned = achievement.Earned;
                        toUpdate.MaxProgress = achievement.MaxProgress;
                        toUpdate.Name = achievement.Name;
                        toUpdate.Progress = achievement.Progress;
                    }
                }
                else
                {
                    Achievements.TryAdd(appID, new List<Achievement>() { achievement });
                }
            });

            SaveAchievements();
        }

        public static void ResetAllStats(uint appID, bool achievementsToo)
        {
            if (PlayerStats.TryGetValue(appID, out var playerStats))
            {
                playerStats.Clear();
            }
            if (achievementsToo)
            {
                if (Achievements.TryGetValue(appID, out var achievements))
                {
                    achievements.Clear();
                }
            }
        }

        #region Generate Data online

        public static async void DownloadAppCache(uint AppId)
        {
            WebClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            DownloadID = new Random().Next(100, 200);

            try
            {
                string Url = $"https://steamcdn-a.akamaihd.net/steam/apps/{AppId}/library_hero.jpg";
                var Data = await WebClient.DownloadDataTaskAsync(Url);
                File.WriteAllBytes(Path.Combine(AppCachePath, $"{AppId}_library_hero.jpg"), Data);
            }
            catch { }

            try
            {
                string Url = $"https://steamcdn-a.akamaihd.net/steam/apps/{AppId}/header.jpg";
                var Data = await WebClient.DownloadDataTaskAsync(Url);
                File.WriteAllBytes(Path.Combine(AppCachePath, $"{AppId}_header.jpg"), Data);
            }
            catch { }
            try
            {
                GenerateAchievements(AppId);
            }
            catch { }

            try
            {
                GenerateAppDetails(AppId);
            }
            catch { }
            try
            {
                GenerateItems(AppId);
            }
            catch { }
        }

        private static void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // TODO: Notify web Client
        }

        public static async void GenerateAchievements(uint app_id)
        {
            string URL = $"https://api.steampowered.com/ISteamUserStats/GetSchemaForGame/v2/?key={SteamAPIKey}&appid={app_id}";
            string achievementsPath = Path.Combine(modCommon.GetPath(), "Data", "Storage", app_id.ToString(), "GameSchema.json");
            modCommon.EnsureDirectoryExists(achievementsPath, true);

            try
            {
                string RequestResponse = await GetResponseString(URL);
                if (string.IsNullOrEmpty(RequestResponse))
                {
                    File.WriteAllText(achievementsPath, RequestResponse);
                }
            }
            catch 
            {

            }
        }

        private static async Task<string> GetResponseString(string URL)
        {
            string content = "";
            try
            {
                WebRequest webrequest = HttpWebRequest.Create(URL);
                webrequest.Method = "GET";
                var ResponseTask = await webrequest.GetResponseAsync();
                HttpWebResponse response = (HttpWebResponse)ResponseTask;
                StreamReader reader = new StreamReader(response.GetResponseStream());
                content = reader.ReadToEnd();
            }
            catch 
            {
            }
            return content;
        }

        public static string GetGameDescription(uint appID)
        {
            string Description = "";
            try
            {
                if (AppDetails.ContainsKey(appID))
                {
                    var Details = AppDetails[appID];
                    Description = Details.success ? Details.data.detailed_description : "";
                }
            }
            catch { }
            return Description;
        }

        public static async void GenerateItems(uint app_id)
        {
            string URL = $"https://api.steampowered.com/IInventoryService/GetItemDefMeta/v1?key={SteamAPIKey}&appid={app_id}";
            string itemsPath = Path.Combine(modCommon.GetPath(), "Data", "Storage", app_id.ToString(), "Items.json");
            modCommon.EnsureDirectoryExists(itemsPath, true);

            try
            {
                string RequestResponse = await GetResponseString(URL);
                if (string.IsNullOrEmpty(RequestResponse))
                {
                    File.WriteAllText(itemsPath, RequestResponse);
                }
            }
            catch
            {

            }
        }

        public static async void GenerateAppDetails(uint app_id)
        {
            string URL = $"https://store.steampowered.com/api/appdetails/?appids={app_id}";
            string appDetailsPath = Path.Combine(modCommon.GetPath(), "Data", "Storage", app_id.ToString(), "AppDetails.json");
            modCommon.EnsureDirectoryExists(appDetailsPath, true);

            try
            {
                string RequestResponse = await GetResponseString(URL);
                if (string.IsNullOrEmpty(RequestResponse))
                {
                    File.WriteAllText(appDetailsPath, RequestResponse);
                }
            }
            catch
            {

            }
        }
        #endregion
    }
}

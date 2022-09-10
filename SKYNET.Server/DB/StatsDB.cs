using MongoDB.Driver;
using SKYNET.Steamworks;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SKYNET.DB
{
    public static class StatsDB
    {
        private static MongoDbCollection<Achievement> AchievementDB;
        private static MongoDbCollection<Leaderboard> LeaderboardDB;
        private static MongoDbCollection<PlayerStat> PlayerStatDB;

        public static async void Initialize()
        {
            AchievementDB = new MongoDbCollection<Achievement>("SKYNET_Achievement");
            AchievementDB.CreateIndex(Builders<Achievement>.IndexKeys.Ascending((Achievement i) => i.SteamID));
            AchievementDB.CreateIndex(Builders<Achievement>.IndexKeys.Ascending((Achievement i) => i.Name));
            AchievementDB.CreateIndex(Builders<Achievement>.IndexKeys.Ascending((Achievement i) => i.Earned));

            LeaderboardDB = new MongoDbCollection<Leaderboard>("SKYNET_Leaderboard");
            LeaderboardDB.CreateIndex(Builders<Leaderboard>.IndexKeys.Ascending((Leaderboard i) => i.SteamID));
            LeaderboardDB.CreateIndex(Builders<Leaderboard>.IndexKeys.Ascending((Leaderboard i) => i.Name));

            PlayerStatDB = new MongoDbCollection<PlayerStat>("SKYNET_PlayerStat");
            PlayerStatDB.CreateIndex(Builders<PlayerStat>.IndexKeys.Ascending((PlayerStat i) => i.SteamID));
            PlayerStatDB.CreateIndex(Builders<PlayerStat>.IndexKeys.Ascending((PlayerStat i) => i.Name));
        }

        public static void SetAchievement(ulong steamID, Achievement achievement)
        {
            achievement.SteamID = steamID;

            AchievementDB.Collection.InsertOne(achievement, null, default(CancellationToken));
        }

        public static void SetLeaderboard(ulong steamID, Leaderboard leaderboard)
        {
            leaderboard.SteamID = steamID;

            LeaderboardDB.Collection.InsertOne(leaderboard, null, default(CancellationToken));
        }

        public static void SetPlayerStat(ulong steamID, PlayerStat playerStat)
        {
            playerStat.SteamID = steamID;

            PlayerStatDB.Collection.InsertOne(playerStat, null, default(CancellationToken));
        }

        public static void UpdateAchievement(ulong steamID, Achievement achievement)
        {
            AchievementDB.Collection.FindOneAndUpdate((Achievement user) => user.SteamID == steamID, AchievementDB.Ub
                .Set((Achievement a) => a.Name, achievement.Name)
                .Set((Achievement a) => a.Date, achievement.Date)
                .Set((Achievement a) => a.Earned, achievement.Earned)
                .Set((Achievement a) => a.Progress, achievement.Progress)
                .Set((Achievement a) => a.MaxProgress, achievement.MaxProgress), null, default(CancellationToken));
        }
    }
}

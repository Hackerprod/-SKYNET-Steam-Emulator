using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web.Script.Serialization;
using SKYNET.Helpers;
using SKYNET.Network.Types;
using SKYNET.Types;

namespace SKYNET.Managers
{
    public class GameManager
    {
        public static EventHandler<Game> OnGameAdded;
        public static EventHandler<Game> OnGameUpdated;
        public static EventHandler<Game> OnGameRemoved;
        public static EventHandler<GameLaunchedEventArgs> OnGameLaunched;
        public static EventHandler<NET_GameOpened> OnUserGameOpened;
        public static EventHandler<string> OnGameClosed;

        public static List<RunningGame> RunningGames;
        private static List<Game> Games;

        static GameManager()
        {
            Games = new List<Game>();
            RunningGames = new List<RunningGame>();
        }

        public static void Initialize()
        {
            List<Game> games = new List<Game>();
            string gamePath = Path.Combine(Common.GetPath(), "Data", "Games.bin");
            if (File.Exists(gamePath))
            {
                try
                {
                    string json = File.ReadAllText(gamePath);
                    games = new JavaScriptSerializer().Deserialize<List<Game>>(json);
                }
                catch (Exception)
                {
                    games = new List<Game>();
                    Common.Show("Error loading Game stored data");
                }
            }

            foreach (var game in games)
            {
                AddGame(game);
            }

        }

        public static Game GetGame(uint appID)
        {
            return Games.Find(g => g.AppID == appID);
        }

        public static Game GetGame(string Guid)
        {
            return Games.Find(g => g.Guid == Guid);
        }

        internal static Game GetGameByPath(string executablePath)
        {
            return Games.Find(g => g.ExecutablePath == executablePath);
        }

        internal static Game GetGameByRootPath(string executablePath)
        {
            return Games.Find(g => Common.GetRootPath(g.ExecutablePath) == Common.GetRootPath(executablePath));
        }

        public static void AddGame(Game game)
        {
            Games.Add(game);
            Save();
            OnGameAdded?.Invoke(null, game);
        }

        public static void Remove(string Guid)
        {
            var Game = Games.Find(g => g.Guid == Guid);
            if (Game != null)
            {
                OnGameRemoved?.Invoke(null, Game);
                Games.RemoveAll(g => g.Guid == Guid);
            }
            Save();
        }

        public static List<Game> GetGames()
        {
            return Games;
        }

        public static void Update(Game game)
        {
            var Game = Games.Find(g => g.Guid == game.Guid);
            if (Game != null)
            {
                Game.AppID = game.AppID;
                Game.ExecutablePath = game.ExecutablePath;
                Game.GameDLC = game.GameDLC;
                Game.GameOverlay = game.GameOverlay;
                Game.ISteamHTTP = game.ISteamHTTP;
                Game.LaunchWithoutEmu = game.LaunchWithoutEmu;
                Game.LogToConsole = game.LogToConsole;
                Game.LogToFile = game.LogToFile;
                Game.Name = game.Name;
                Game.Parameters = game.Parameters;
                Game.RunCallbacks = game.RunCallbacks;
                OnGameUpdated?.Invoke(null, game);
            }
            Save();
        }

        public static void InvokeGameLaunched(Game game, int processID, string gameClientID)
        {
            OnGameLaunched?.Invoke(null, new GameLaunchedEventArgs(processID, game, gameClientID));
        }

        public static void InvokeUserGameOpened(NET_GameOpened gameOpened)
        {
            OnUserGameOpened?.Invoke(null, gameOpened);
        }

        public static void InvokeGameClosed(string gameClientID)
        {
            OnGameClosed?.Invoke(null, gameClientID);
        }

        internal static uint GetLastPlayed(string guid)
        {
            var Game = GetGame(guid);
            if (Game != null)
            {
                return Game.LastPlayed;
            }
            return 0;
        }

        internal static long GetTimePlayed(string guid)
        {
            var Game = GetGame(guid);
            if (Game != null)
            {
                return Game.TimePlayed;
            }
            return 0;
        }

        internal static uint GetUsersPlaying(string guid)
        {
            var Game = GetGame(guid);
            if (Game == null) return 0;
            return (uint)UserManager.GetUsersPlaying(Game.AppID).Count;
        }

        internal static bool IsPlaying(string guid)
        {
            return RunningGames.Find(r => r.Game.Guid == guid) != null;
        }

        public static void SetLastPlayedTime(string guid)
        {
            var Game = GetGame(guid);
            if (Game != null)
            {
                Game.LastPlayed = DateTime.Now.ToTimestamp();
            }
            Save();
        }

        public static void SetTimePlayed(string guid, DateTime oppenedTime)
        {
            var Game = GetGame(guid);
            if (Game != null)
            {
                var timeSpan = DateTime.Now - oppenedTime;
                Game.TimePlayed = (Game.TimePlayed + (long)timeSpan.TotalSeconds);
            }
            Save();
        }

        public static RunningGame GetRunningGame(string ID, bool FromGuid = true)
        {
            if (FromGuid)
            {
                return RunningGames.Find(g => g.Game.Guid == ID); 
            }
            return RunningGames.Find(g => g.GameClientID == ID); 
        }

        public static bool IsRunningGame(string ID, bool FromGuid = true)
        {
            return GetRunningGame(ID, FromGuid) != null;
        }

        public static bool AddRunningGame(int processID, Game game, string gameClientID, out RunningGame runninGame)
        {
            runninGame = new RunningGame(processID, game, gameClientID);
            if (runninGame.Process == null) return false;
            RunningGames.Add(runninGame);
            return true;
        }

        public static void RemoveRunningGame(string gameClientID)
        {
            RunningGames.RemoveAll(g => g.GameClientID == gameClientID);
        }

        public static void Save()
        {
            try
            {
                string path = Path.Combine(Common.GetPath(), "Data", "Games.bin");
                Common.EnsureDirectoryExists(path, true);
                string json = new JavaScriptSerializer().Serialize(Games);
                File.WriteAllText(path, json);
            }
            catch { }
        }

        public class GameLaunchedEventArgs : EventArgs
        {
            public Game Game;
            public int ProcessID;
            public string GameClientID;
            public GameLaunchedEventArgs(int processID, Game game, string gameClientID)
            {
                ProcessID = processID;
                Game = game;
                GameClientID = gameClientID;
            }
        }

    }
}

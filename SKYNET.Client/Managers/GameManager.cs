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
        public static event EventHandler<Game> OnGameAdded;
        public static event EventHandler<Game> OnGameUpdated;
        public static event EventHandler<Game> OnGameRemoved;
        public static EventHandler<GameLaunchedEventArgs> OnGameLaunched;
        public static event EventHandler<NET_GameOpened> OnUserGameOpened;

        public static EventHandler<string> OnGameClosed;
        private static List<Game> Games;

        static GameManager()
        {
            Games = new List<Game>();
        }

        public static void Initialize()
        {
            List<Game> games = new List<Game>();
            string gamePath = Path.Combine(modCommon.GetPath(), "Data", "Games.bin");
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
                    modCommon.Show("Error loading Game stored data");
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
            return Games.Find(g => modCommon.GetRootPath(g.ExecutablePath) == modCommon.GetRootPath(executablePath));
        }

        public static void AddGame(Game game)
        {
            Games.Add(game);
            OnGameAdded?.Invoke(null, game);
        }

        public static void Remove(uint appID)
        {
            var Game = Games.Find(g => g.AppID == appID);
            if (Game != null)
            {
                OnGameRemoved?.Invoke(null, Game);
                Games.RemoveAll(g => g.AppID == appID);
            }
        }

        public static void Remove(string Guid)
        {
            var Game = Games.Find(g => g.Guid == Guid);
            if (Game != null)
            {
                OnGameRemoved?.Invoke(null, Game);
                Games.RemoveAll(g => g.Guid == Guid);
            }
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

        internal static uint GetLastPlayed(string guid)
        {
            return 0;
        }

        internal static uint GetTimePlayed(string guid)
        {
            return 0;
        }

        internal static uint GetUsersPlaying(string guid)
        {
            return 0;
        }

        internal static bool IsPlaying(string guid)
        {
            return true;
        }

        internal static Bitmap GetGameImage(uint appID, bool Header = false)
        {
            string imageName = Header ? appID + "_header.jpg" : appID + "_library_hero.jpg";
            Bitmap bitmap = new Bitmap(200, 200);
            try
            {
                string ImagePath = Path.Combine(modCommon.GetPath(), "Data", "Images", "AppCache", imageName);
                if (File.Exists(ImagePath))
                {
                    bitmap = (Bitmap)ImageHelper.FromFile(ImagePath);
                }
            }
            catch
            {
            }
            return bitmap;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SKYNET.Managers
{
    public class GameManager
    {
        public static event EventHandler<Game> OnGameAdded;
        public static event EventHandler<Game> OnGameUpdated;
        public static event EventHandler<Game> OnGameRemoved;
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

        internal static Game GetGame(uint appID)
        {
            return Games.Find(g => g.AppID == appID);
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

        public static void Update(Game game)
        {
            var Game = Games.Find(g => g.AppID == game.AppID);
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
                Game.SteamApiPath = game.SteamApiPath;

                OnGameUpdated?.Invoke(null, game);
            }
        }
    }
}

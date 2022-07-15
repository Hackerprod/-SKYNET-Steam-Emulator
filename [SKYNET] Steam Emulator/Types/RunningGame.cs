using System.Diagnostics;

namespace SKYNET.Types
{
    public class RunningGame
    {
        public Game Game;
        public Process Process;
        public string GameClientID;

        public RunningGame(int processID, Game game, string gameClientID)
        {
            Game = game;
            GameClientID = gameClientID;
            try
            {
                Process = Process.GetProcessById(processID);
            }
            catch { }
        }
    }
}

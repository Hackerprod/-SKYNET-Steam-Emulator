using System;
using System.Diagnostics;

namespace SKYNET.Types
{
    public class RunningGame
    {
        public Game Game;
        public Process Process;
        public string GameClientID;
        public DateTime OppenedTime;

        public RunningGame(int processID, Game game, string gameClientID)
        {
            OppenedTime = DateTime.Now;
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

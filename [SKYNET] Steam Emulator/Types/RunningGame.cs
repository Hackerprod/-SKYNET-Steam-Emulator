using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

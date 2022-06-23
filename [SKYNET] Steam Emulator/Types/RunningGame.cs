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
        public event EventHandler<Game> OnGameClosed;
        public Game Game;
        public Process Process;
        public string GameClientID;

        private int ProcessId;

        public RunningGame(int processID, Game game, string gameClientID)
        {
            Game = game;
            ProcessId = processID;
            GameClientID = gameClientID;
            Process = Process.GetProcessById(processID);
            Initialize();
        }

        public void Initialize()
        {
            //Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        if (Process.GetProcessById(ProcessId) == null)
            //        {
            //            OnGameClosed?.Invoke(null, Game);
            //            break;
            //        }
            //    }
            //    OnGameClosed?.Invoke(null, Game);
            //});
        }
    }
}

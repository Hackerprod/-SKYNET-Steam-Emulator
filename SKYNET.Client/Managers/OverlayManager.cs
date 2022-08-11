using SKYNET.Client;
using SKYNET.Helpers;
using SKYNET.Types;
using System.Collections.Generic;

namespace SKYNET.Managers
{
    public class OverlayManager
    {
        private static List<OverlayGameData> OverlayData;
        public static void Initialize()
        {
            OverlayData = new List<OverlayGameData>();
        }

        public static void SetDirect3DVersion(Direct3DVersion version, RunningGame game)
        {
            var Game = GetGame(game.GameClientID);
            if (Game == null)
            {
                Game = new OverlayGameData()
                {
                    RunningGame = game,
                    Versions = new List<Direct3DVersion>() { version }
                };
                OverlayData.Add(Game);
            }
            else
            {
                Game.Versions.Add(version);
            }
        }

        public static void RemoveGameData(string GameClientID)
        {
            OverlayData.RemoveAll(o => o.RunningGame.GameClientID == GameClientID);
        }

        private static OverlayGameData GetGame(string gameClientID)
        {
            return OverlayData.Find(o => o.RunningGame.GameClientID == gameClientID);
        }

        private static void Write(object msg)
        {
            SteamClient.Write("OverlayManager", msg);
        }

        internal class OverlayGameData
        {
            internal RunningGame RunningGame;
            internal List<Direct3DVersion> Versions;
            public OverlayGameData()
            {
                Versions = new List<Direct3DVersion>();
            }
        }
    }

    public enum Direct3DVersion
    {
        Unknown,
        AutoDetect,
        Direct3D9,
        Direct3D10,
        Direct3D10_1,
        Direct3D11,
        Direct3D11_1,
    }
}

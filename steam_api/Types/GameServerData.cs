using System.Collections.Concurrent;

namespace SKYNET.Types
{
    public class GameServerData
    {
        public GameServerData()
        {
            KeyValues = new ConcurrentDictionary<string, string>();
            Players = new ConcurrentDictionary<ulong, GameServerPlayerData>();
        }

        public ulong SteamId { get; set; }
        public uint IP { get; set; }
        public int Port { get; set; }
        public int QueryPort { get; set; }
        public uint Flags { get; set; }
        public byte Secure { get; set; }
        public uint AppId { get; set; }
        public string VersionString { get; set; }
        public string Product { get; set; }
        public string Description { get; set; }
        public string ModDir { get; set; }
        public bool Dedicated { get; set; }
        public int MaxPlayers { get; set; }
        public int BotPlayers { get; set; }
        public string ServerName { get; set; }
        public string MapName { get; set; }
        public bool PasswordProtected { get; set; }
        public uint SpectatorPort { get; set; }
        public string SpectatorServerName { get; set; }
        public string GameTags { get; set; }
        public string GameData { get; set; }
        public string Region { get; set; }
        public bool LoggedOn { get; set; }
        public bool AdvertiseActive { get; set; } = true;
        public ConcurrentDictionary<string, string> KeyValues { get; set; }
        public ConcurrentDictionary<ulong, GameServerPlayerData> Players { get; set; }
    }

    public sealed class GameServerPlayerData
    {
        public ulong SteamId { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public float TimePlayedSeconds { get; set; }
        public long ConnectedAtUtcTicks { get; set; }

        public float GetTimePlayedSeconds()
        {
            if (ConnectedAtUtcTicks <= 0)
            {
                return TimePlayedSeconds;
            }

            var connectedAt = new System.DateTime(ConnectedAtUtcTicks, System.DateTimeKind.Utc);
            var elapsed = (float)(System.DateTime.UtcNow - connectedAt).TotalSeconds;
            return System.Math.Max(TimePlayedSeconds, elapsed);
        }
    }
}

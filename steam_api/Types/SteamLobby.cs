using SKYNET.Steamworks;
using System.Collections.Generic;

namespace SKYNET.Types
{
    public class SteamLobby
    {
        public ulong SteamID { get; set; }
        public uint AppID { get; set; }
        public ulong Owner { get; set; }
        public ELobbyType Type { get; set; }
        public List<LobbyMember> Members { get; set; }
        public Dictionary<string, string> LobbyData { get; set; }
        public int MaxMembers { get; set; }
        public bool Joinable { get; set; }
        public LobbyGameserver Gameserver { get; set; }

        public SteamLobby()
        {
            Members = new List<LobbyMember>();
            LobbyData = new Dictionary<string, string>();
            Type = ELobbyType.k_ELobbyTypePublic;
            Gameserver = new LobbyGameserver()
            {
                SteamID = (ulong)SteamEmulator.SteamID_GS
            };
        }

        public (string Key, string Value) GetDataByIndex(int iLobbyData)
        {
            string key = "";
            string value = "";
            int index = 0;
            foreach (var item in LobbyData)
            {
                if (index == iLobbyData)
                {
                    key = item.Key;
                    value = item.Value;
                    break;
                }
                index++;
            }
            return (key, value);
        }

        public class LobbyGameserver
        {
            public ulong SteamID { get; set; }
            public uint IP { get; set; }
            public uint Port { get; set; }
            public bool Filled { get; set; }
        }

        public class LobbyMember
        {
            public ulong m_SteamID;
            public List<LobbyMetaData> m_Data;

            public LobbyMember()
            {
                m_Data = new List<LobbyMetaData>();
            }
        }

        public class LobbyMetaData
        {
            public string m_Key;
            public string m_Value;
        }
    }
}

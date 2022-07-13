using SKYNET.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                SteamID = (ulong)SteamClient.SteamID_GS
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

    public enum ELobbyType
    {
        k_ELobbyTypePrivate = 0,        // only way to join the lobby is to invite to someone else
        k_ELobbyTypeFriendsOnly = 1,    // shows for friends or invitees, but not in lobby list
        k_ELobbyTypePublic = 2,         // visible for friends and in lobby list
        k_ELobbyTypeInvisible = 3,      // returned by search, but not visible to other friends 
                                        //    useful if you want a user in two lobbies, for example matching groups together
                                        //	  a user can be in only one regular lobby, and up to two invisible lobbies
        k_ELobbyTypePrivateUnique = 4,  // private, unique and does not delete when empty - only one of these may exist per unique keypair set
                                        // can only create from webapi
    };
}

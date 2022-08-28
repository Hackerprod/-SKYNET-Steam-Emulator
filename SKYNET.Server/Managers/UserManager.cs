using SKYNET.Steamworks;
using SKYNET.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Managers
{
    public class UserManager
    {
        public static List<SteamPlayer> Users => DBManager.Users.Users;
        public static ConcurrentDictionary<uint, List<RichPresence>> RichPresence;
        public static ConcurrentDictionary<uint, List<uint>> RegisteredRichPresence;

        public static void Initialize()
        {
            RichPresence = new ConcurrentDictionary<uint, List<RichPresence>>();
            RegisteredRichPresence = new ConcurrentDictionary<uint, List<uint>>();
        }

        public static List<RichPresence> GetRichPresence(uint accountID)
        {
            if (RichPresence.TryGetValue(accountID, out var richPrecense))
            {
                return richPrecense;
            }
            return new List<RichPresence>();
        }

        public static void SetRichPresence(uint accountID, string key, string value)
        {
            if (RichPresence.TryGetValue(accountID, out var richPrecense))
            {
                var rich = richPrecense.Find(r => r.Key == key);
                if (rich == null)
                {
                    richPrecense.Add(new RichPresence(key, value));
                }
                else
                {
                    rich.Value = value;
                }
            }
            else
            {
                RichPresence.TryAdd(accountID, new List<RichPresence>() { new RichPresence(key, value) });
            }
        }

        public static bool ValidPersonaName(string personaName)
        {
            return Users.Find(u => u.PersonaName == personaName) == null;
        }

        public static SteamPlayer GetByAccountName(string username)
        {
            return Users.Find(u => !string.IsNullOrEmpty(u.AccountName) && u.AccountName.ToLower() == username.ToLower());
        }

        public static SteamPlayer Get(uint accountID)
        {
            return Users.Find(u => u.AccountID == accountID);
        }

        public static SteamPlayer Get(ulong steamId)
        {
            return Users.Find(u => ((ulong)new CSteamID(u.AccountID, EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeIndividual)) == steamId);
        }

        public static void CreateAccount(SteamPlayer user)
        {
            DBManager.Users.CreateAccount(user);
        }

        public static void RegisterToRichPresence(uint accountID, uint subscribedAccountID)
        {
            if (RegisteredRichPresence.TryGetValue(accountID, out var registeredClients))
            {
                if (!registeredClients.Contains(subscribedAccountID))
                {
                    registeredClients.Add(subscribedAccountID);
                }
            }
            else
            {
                RegisteredRichPresence.TryAdd(accountID, new List<uint>() { subscribedAccountID });
            }
        }

        public static List<uint> GetRegisteredRichPresence(uint accountID)
        {
            List<uint> clients = new List<uint>();
            if (RegisteredRichPresence.TryGetValue(accountID, out var registeredClients))
            {
                clients.AddRange(registeredClients);
            }
            return clients;
        }

        public static void SetPlayingState(uint accountID, bool playing)
        {
            DBManager.Users.SetPlayingState(accountID, playing);
        }

        public static void SetPlayingState(SteamPlayer User, bool playing)
        {
            DBManager.Users.SetPlayingState(User, playing);
        }

        public static void SetLastLogOn(uint accountID, uint setLastLogOn)
        {
            DBManager.Users.SetLastLogOn(accountID, setLastLogOn);
        }

    }
}

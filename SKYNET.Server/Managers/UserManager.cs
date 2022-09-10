using SKYNET.DB;
using SKYNET.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SKYNET.Managers
{
    public class UserManager
    {
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
                RichPresence rich = richPrecense.Find((RichPresence r) => r.Key == key);
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
                RichPresence.TryAdd(accountID, new List<RichPresence>
            {
                new RichPresence(key, value)
            });
            }
        }

        public static bool IsValidPersonaName(string personaName)
        {
            return UserDB.GetByPersonaName(personaName) == null;
        }

        public static SteamPlayer GetByAccountName(string AccountName)
        {
            return UserDB.GetByAccountName(AccountName);
        }

        public static SteamPlayer Get(uint accountID)
        {
            return UserDB.Get(accountID);
        }

        public static SteamPlayer Get(ulong steamId)
        {
            return UserDB.Get(steamId);
        }

        public static void CreateAccount(SteamPlayer user)
        {
            UserDB.CreateAccount(user);
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
                RegisteredRichPresence.TryAdd(accountID, new List<uint> { subscribedAccountID });
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

        public static void SetPlayingState(uint accountID, uint appID)
        {
            UserDB.SetPlayingState(accountID, appID);
        }

        internal static List<ulong> GetFriends(ulong steamID)
        {
            return UserDB.GetFriends(steamID);
        }

        public static void SetPlayingState(SteamPlayer User, bool playing)
        {
            UserDB.SetPlayingState(User, playing);
        }

        public static void SetLastLogOn(uint accountID, uint setLastLogOn)
        {
            UserDB.SetLastLogOn(accountID, setLastLogOn);
        }
    }
}

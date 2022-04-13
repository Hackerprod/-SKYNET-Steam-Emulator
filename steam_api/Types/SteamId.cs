using SKYNET.Steamworks;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SKYNET.Types
{
    public class SteamID
    {
        [Flags]
        public enum ChatInstanceFlags : uint
        {
            Clan = 0x80000u,
            Lobby = 0x40000u,
            MMSLobby = 0x20000u
        }

        private BitVector64 steamid;

        private static Regex Steam2Regex = new Regex("STEAM_(?<universe>[0-4]):(?<authserver>[0-1]):(?<accountid>\\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Regex Steam3Regex = new Regex("\\[(?<type>[AGMPCgcLTIUai]):(?<universe>[0-4]):(?<account>\\d+)(:(?<instance>\\d+))?\\]", RegexOptions.Compiled);

        private static Regex Steam3FallbackRegex = new Regex("\\[(?<type>[AGMPCgcLTIUai]):(?<universe>[0-4]):(?<account>\\d+)(\\((?<instance>\\d+)\\))?\\]", RegexOptions.Compiled);

        private static Dictionary<EAccountType, char> AccountTypeChars = new Dictionary<EAccountType, char>
    {
        {
            EAccountType.k_EAccountTypeAnonGameServer,
            'A'
        },
        {
            EAccountType.k_EAccountTypeGameServer,
            'G'
        },
        {
            EAccountType.k_EAccountTypeMultiseat,
            'M'
        },
        {
            EAccountType.k_EAccountTypePending,
            'P'
        },
        {
            EAccountType.k_EAccountTypeContentServer,
            'C'
        },
        {
            EAccountType.k_EAccountTypeClan,
            'g'
        },
        {
            EAccountType.k_EAccountTypeChat,
            'T'
        },
        {
            EAccountType.k_EAccountTypeInvalid,
            'I'
        },
        {
            EAccountType.k_EAccountTypeIndividual,
            'U'
        },
        {
            EAccountType.k_EAccountTypeAnonUser,
            'a'
        }
    };

        private const char UnknownAccountTypeChar = 'i';

        public const uint AllInstances = 0u;

        public const uint DesktopInstance = 1u;

        public const uint ConsoleInstance = 2u;

        public const uint WebInstance = 4u;

        public const uint AccountIDMask = uint.MaxValue;

        public const uint AccountInstanceMask = 1048575u;

        public bool IsBlankAnonAccount
        {
            get
            {
                if (AccountID == 0 && IsAnonAccount)
                {
                    return AccountInstance == 0;
                }
                return false;
            }
        }

        public bool IsGameServerAccount
        {
            get
            {
                if (AccountType != EAccountType.k_EAccountTypeGameServer)
                {
                    return AccountType == EAccountType.k_EAccountTypeAnonGameServer;
                }
                return true;
            }
        }

        public bool IsPersistentGameServerAccount => AccountType == EAccountType.k_EAccountTypeGameServer;

        public bool IsAnonGameServerAccount => AccountType == EAccountType.k_EAccountTypeAnonGameServer;

        public bool IsContentServerAccount => AccountType == EAccountType.k_EAccountTypeContentServer;

        public bool IsClanAccount => AccountType == EAccountType.k_EAccountTypeClan;

        public bool IsChatAccount => AccountType == EAccountType.k_EAccountTypeChat;

        public bool IsLobby
        {
            get
            {
                if (AccountType == EAccountType.k_EAccountTypeChat)
                {
                    return (AccountInstance & 0x40000) != 0;
                }
                return false;
            }
        }

        public bool IsIndividualAccount
        {
            get
            {
                if (AccountType != EAccountType.k_EAccountTypeIndividual)
                {
                    return AccountType == EAccountType.k_EAccountTypeConsoleUser;
                }
                return true;
            }
        }

        public bool IsAnonAccount
        {
            get
            {
                if (AccountType != EAccountType.k_EAccountTypeAnonUser)
                {
                    return AccountType == EAccountType.k_EAccountTypeAnonGameServer;
                }
                return true;
            }
        }

        public bool IsAnonUserAccount => AccountType == EAccountType.k_EAccountTypeAnonUser;

        public bool IsConsoleUserAccount => AccountType == EAccountType.k_EAccountTypeConsoleUser;

        public bool IsValid
        {
            get
            {
                if (AccountType <= EAccountType.k_EAccountTypeInvalid || AccountType >= EAccountType.k_EAccountTypeMax)
                {
                    return false;
                }
                if (AccountUniverse <= EUniverse.k_EUniverseInvalid || AccountUniverse >= EUniverse.k_EUniverseMax)
                {
                    return false;
                }
                if (AccountType == EAccountType.k_EAccountTypeIndividual && (AccountID == 0 || AccountInstance > 4))
                {
                    return false;
                }
                if (AccountType == EAccountType.k_EAccountTypeClan && (AccountID == 0 || AccountInstance != 0))
                {
                    return false;
                }
                if (AccountType == EAccountType.k_EAccountTypeGameServer && AccountID == 0)
                {
                    return false;
                }
                return true;
            }
        }

        public uint AccountID
        {
            get
            {
                return (uint)steamid[0u, 4294967295uL];
            }
            set
            {
                steamid[0u, 4294967295uL] = value;
            }
        }

        public uint AccountInstance
        {
            get
            {
                return (uint)steamid[32u, 1048575uL];
            }
            set
            {
                steamid[32u, 1048575uL] = value;
            }
        }

        public EAccountType AccountType
        {
            get
            {
                return (EAccountType)steamid[52u, 15uL];
            }
            set
            {
                steamid[52u, 15uL] = (ulong)value;
            }
        }

        public EUniverse AccountUniverse
        {
            get
            {
                return (EUniverse)steamid[56u, 255uL];
            }
            set
            {
                steamid[56u, 255uL] = (ulong)value;
            }
        }

        public SteamID()
            : this(0uL)
        {
        }

        public SteamID(uint unAccountID, EUniverse eUniverse, EAccountType eAccountType)
            : this()
        {
            Set(unAccountID, eUniverse, eAccountType);
        }

        public SteamID(uint unAccountID, uint unInstance, EUniverse eUniverse, EAccountType eAccountType)
            : this()
        {
            InstancedSet(unAccountID, unInstance, eUniverse, eAccountType);
        }

        public SteamID(ulong id)
        {
            steamid = new BitVector64(id);
        }

        public SteamID(string steamId)
            : this(steamId, EUniverse.k_EUniversePublic)
        {
        }

        public SteamID(string steamId, EUniverse eUniverse)
            : this()
        {
            SetFromString(steamId, eUniverse);
        }

        public void Set(uint unAccountID, EUniverse eUniverse, EAccountType eAccountType)
        {
            AccountID = unAccountID;
            AccountUniverse = eUniverse;
            AccountType = eAccountType;
            if (eAccountType == EAccountType.k_EAccountTypeClan || eAccountType == EAccountType.k_EAccountTypeGameServer)
            {
                AccountInstance = 0u;
            }
            else
            {
                AccountInstance = 1u;
            }
        }

        public void InstancedSet(uint unAccountID, uint unInstance, EUniverse eUniverse, EAccountType eAccountType)
        {
            AccountID = unAccountID;
            AccountUniverse = eUniverse;
            AccountType = eAccountType;
            AccountInstance = unInstance;
        }

        public bool SetFromString(string steamId, EUniverse eUniverse)
        {
            if (string.IsNullOrEmpty(steamId))
            {
                return false;
            }
            Match match = Steam2Regex.Match(steamId);
            if (!match.Success)
            {
                return false;
            }
            if (!uint.TryParse(match.Groups["accountid"].Value, out var result) || !uint.TryParse(match.Groups["authserver"].Value, out var result2))
            {
                return false;
            }
            AccountUniverse = eUniverse;
            AccountInstance = 1u;
            AccountType = EAccountType.k_EAccountTypeIndividual;
            AccountID = (result << 1) | result2;
            return true;
        }

        public bool SetFromSteam3String(string steamId)
        {
            if (string.IsNullOrEmpty(steamId))
            {
                return false;
            }
            Match match = Steam3Regex.Match(steamId);
            if (!match.Success)
            {
                match = Steam3FallbackRegex.Match(steamId);
                if (!match.Success)
                {
                    return false;
                }
            }
            if (!uint.TryParse(match.Groups["account"].Value, out var result))
            {
                return false;
            }
            if (!uint.TryParse(match.Groups["universe"].Value, out var result2))
            {
                return false;
            }
            string value = match.Groups["type"].Value;
            if (value.Length != 1)
            {
                return false;
            }
            char type = value[0];
            Group group = match.Groups["instance"];
            uint num;
            if (group != null && !string.IsNullOrEmpty(group.Value))
            {
                num = uint.Parse(group.Value);
            }
            else
            {
                switch (type)
                {
                    case 'L':
                    case 'T':
                    case 'c':
                    case 'g':
                        num = 0u;
                        break;
                    default:
                        num = 1u;
                        break;
                }
            }
            if (type == 'c')
            {
                num |= 0x80000u;
                AccountType = EAccountType.k_EAccountTypeChat;
            }
            else if (type == 'L')
            {
                num |= 0x40000u;
                AccountType = EAccountType.k_EAccountTypeChat;
            }
            else if (type == 'i')
            {
                AccountType = EAccountType.k_EAccountTypeInvalid;
            }
            else
            {
                AccountType = AccountTypeChars.First((KeyValuePair<EAccountType, char> x) => x.Value == type).Key;
            }
            AccountUniverse = (EUniverse)result2;
            AccountInstance = num;
            AccountID = result;
            return true;
        }

        public void SetFromUInt64(ulong ulSteamID)
        {
            steamid.Data = ulSteamID;
        }

        public ulong ConvertToUInt64()
        {
            return steamid.Data;
        }

        public ulong GetStaticAccountKey()
        {
            return (ulong)(((long)AccountUniverse << 56) + ((long)AccountType << 52) + AccountID);
        }

        public string Render(bool steam3 = false)
        {
            if (steam3)
            {
                return RenderSteam3();
            }
            return RenderSteam2();
        }

        private string RenderSteam2()
        {
            EAccountType accountType = AccountType;
            if ((uint)accountType <= 1u)
            {
                string arg = ((AccountUniverse <= EUniverse.k_EUniversePublic) ? "0" : Enum.Format(typeof(EUniverse), AccountUniverse, "D"));
                return $"STEAM_{arg}:{AccountID & 1u}:{AccountID >> 1}";
            }
            return Convert.ToString(this);
        }

        private string RenderSteam3()
        {
            if (!AccountTypeChars.TryGetValue(AccountType, out var value))
            {
                value = 'i';
            }
            if (AccountType == EAccountType.k_EAccountTypeChat)
            {
                if (((ChatInstanceFlags)AccountInstance).HasFlag(ChatInstanceFlags.Clan))
                {
                    value = 'c';
                }
                else if (((ChatInstanceFlags)AccountInstance).HasFlag(ChatInstanceFlags.Lobby))
                {
                    value = 'L';
                }
            }
            bool flag = false;
            switch (AccountType)
            {
                case EAccountType.k_EAccountTypeMultiseat:
                case EAccountType.k_EAccountTypeAnonGameServer:
                    flag = true;
                    break;
                case EAccountType.k_EAccountTypeIndividual:
                    flag = AccountInstance != 1;
                    break;
            }
            if (flag)
            {
                return $"[{value}:{(uint)AccountUniverse}:{AccountID}:{AccountInstance}]";
            }
            return $"[{value}:{(uint)AccountUniverse}:{AccountID}]";
        }

        public override string ToString()
        {
            return Render();
        }

        public static implicit operator ulong(SteamID sid)
        {
            return sid.steamid.Data;
        }

        public static implicit operator SteamID(ulong id)
        {
            return new SteamID(id);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            SteamID steamID = obj as SteamID;
            if ((object)steamID == null)
            {
                return false;
            }
            return steamid.Data == steamID.steamid.Data;
        }

        public bool Equals(SteamID sid)
        {
            if ((object)sid == null)
            {
                return false;
            }
            return steamid.Data == sid.steamid.Data;
        }

        public static bool operator ==(SteamID a, SteamID b)
        {
            if ((object)a == b)
            {
                return true;
            }
            if ((object)a == null || (object)b == null)
            {
                return false;
            }
            return a.steamid.Data == b.steamid.Data;
        }

        public static bool operator !=(SteamID a, SteamID b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return steamid.Data.GetHashCode();
        }
    }
    internal class BitVector64
    {
        private ulong data;

        public ulong Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        public ulong this[uint bitoffset, ulong valuemask]
        {
            get
            {
                return (data >> (int)(ushort)bitoffset) & valuemask;
            }
            set
            {
                data = (data & ~(valuemask << (int)(ushort)bitoffset)) | ((value & valuemask) << (int)(ushort)bitoffset);
            }
        }

        public BitVector64()
        {
        }

        public BitVector64(ulong value)
        {
            data = value;
        }
    }
}

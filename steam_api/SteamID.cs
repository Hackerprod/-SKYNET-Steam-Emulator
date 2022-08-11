using System;
using System.Runtime.InteropServices;
using AccountID_t = System.UInt32;

namespace SteamworksNET
{
    [StructLayout(LayoutKind.Sequential)]
    // Steam ID structure (64 bits total)
    public class CSteamID : IEquatable<CSteamID>, IComparable<CSteamID>, IEquatable<ulong>, IComparable<ulong>
    {
        //-----------------------------------------------------------------------------
        // Purpose: ructor
        //-----------------------------------------------------------------------------
        public CSteamID()
        {
            m_steamid.m_comp.m_unAccountID = 0;
            m_steamid.m_comp.m_EAccountType = (uint)EAccountType.k_EAccountTypeInvalid;
            m_steamid.m_comp.m_EUniverse = EUniverse.k_EUniverseInvalid;
            m_steamid.m_comp.m_unAccountInstance = 0;
        }


        //-----------------------------------------------------------------------------
        // Purpose: ructor
        // Input  : unAccountID -	32-bit account ID
        //			eUniverse -		Universe this account belongs to
        //			eAccountType -	Type of account
        //-----------------------------------------------------------------------------
        public CSteamID(UInt32 unAccountID, EUniverse eUniverse, EAccountType eAccountType)
        {
            Set(unAccountID, eUniverse, eAccountType);
        }


        //-----------------------------------------------------------------------------
        // Purpose: ructor
        // Input  : unAccountID -	32-bit account ID
        //			unAccountInstance - instance 
        //			eUniverse -		Universe this account belongs to
        //			eAccountType -	Type of account
        //-----------------------------------------------------------------------------
        public CSteamID(UInt32 unAccountID, uint unAccountInstance, EUniverse eUniverse, EAccountType eAccountType)
        {
            InstancedSet(unAccountID, unAccountInstance, eUniverse, eAccountType);
        }


        //-----------------------------------------------------------------------------
        // Purpose: ructor
        // Input  : ulSteamID -		64-bit representation of a Steam ID
        // Note:	Will not accept a UInt32 or int32 as input, as that is a probable mistake.
        //			See the stubbed out overloads in the private: section for more info.
        //-----------------------------------------------------------------------------
        public CSteamID(UInt64 ulSteamID)
        {
            SetFromUInt64(ulSteamID);
        }

        //-----------------------------------------------------------------------------
        // Purpose: Sets parameters for steam ID
        // Input  : unAccountID -	32-bit account ID
        //			eUniverse -		Universe this account belongs to
        //			eAccountType -	Type of account
        //-----------------------------------------------------------------------------
        public void Set(UInt32 unAccountID, EUniverse eUniverse, EAccountType eAccountType)
        {
            m_steamid.m_comp.m_unAccountID = unAccountID;
            m_steamid.m_comp.m_EUniverse = eUniverse;
            m_steamid.m_comp.m_EAccountType = (uint)eAccountType;

            if (eAccountType == EAccountType.k_EAccountTypeClan || eAccountType == EAccountType.k_EAccountTypeGameServer)
            {
                m_steamid.m_comp.m_unAccountInstance = 0;
            }
            else
            {
                m_steamid.m_comp.m_unAccountInstance = k_unSteamUserDefaultInstance;
            }

            uint instance = m_steamid.m_comp.m_unAccountInstance;

            this.m_steamid.m_unAll64Bits = 0;
            this.m_steamid.m_unAll64Bits = (m_steamid.m_unAll64Bits & ~(0xFFFFFFFFul << (ushort)0)) | (((ulong)(unAccountID) & 0xFFFFFFFFul) << (ushort)0);
            this.m_steamid.m_unAll64Bits = (m_steamid.m_unAll64Bits & ~(0xFFul << (ushort)56)) | (((ulong)(eUniverse) & 0xFFul) << (ushort)56);
            this.m_steamid.m_unAll64Bits = (m_steamid.m_unAll64Bits & ~(0xFul << (ushort)52)) | (((ulong)(eAccountType) & 0xFul) << (ushort)52);
            this.m_steamid.m_unAll64Bits = (m_steamid.m_unAll64Bits & ~(0xFFFFFul << (ushort)32)) | (((ulong)(instance) & 0xFFFFFul) << (ushort)32);

        }


        //-----------------------------------------------------------------------------
        // Purpose: Sets parameters for steam ID
        // Input  : unAccountID -	32-bit account ID
        //			eUniverse -		Universe this account belongs to
        //			eAccountType -	Type of account
        //-----------------------------------------------------------------------------
        public void InstancedSet(UInt32 unAccountID, UInt32 unInstance, EUniverse eUniverse, EAccountType eAccountType)
        {
            m_steamid.m_comp.m_unAccountID = unAccountID;
            m_steamid.m_comp.m_EUniverse = eUniverse;
            m_steamid.m_comp.m_EAccountType = (uint)eAccountType;
            m_steamid.m_comp.m_unAccountInstance = unInstance;
        }


        //-----------------------------------------------------------------------------
        // Purpose: Initializes a steam ID from its 52 bit parts and universe/type
        // Input  : ulIdentifier - 52 bits of goodness
        //-----------------------------------------------------------------------------
        public void FullSet(UInt64 ulIdentifier, EUniverse eUniverse, EAccountType eAccountType)
        {
            m_steamid.m_comp.m_unAccountID = (uint)(ulIdentifier & k_unSteamAccountIDMask);                       // account ID is low 32 bits
            m_steamid.m_comp.m_unAccountInstance = (uint)((ulIdentifier >> 32) & k_unSteamAccountInstanceMask);           // account instance is next 20 bits
            m_steamid.m_comp.m_EUniverse = eUniverse;
            m_steamid.m_comp.m_EAccountType = (uint)eAccountType;
        }


        //-----------------------------------------------------------------------------
        // Purpose: Initializes a steam ID from its 64-bit representation
        // Input  : ulSteamID -		64-bit representation of a Steam ID
        //-----------------------------------------------------------------------------
        public void SetFromUInt64(UInt64 ulSteamID)
        {
            m_steamid.m_unAll64Bits = ulSteamID;
        }


        //-----------------------------------------------------------------------------
        // Purpose: Clear all fields, leaving an invalid ID.
        //-----------------------------------------------------------------------------
        public void Clear()
        {
            m_steamid.m_comp.m_unAccountID = 0;
            m_steamid.m_comp.m_EAccountType = (uint)EAccountType.k_EAccountTypeInvalid;
            m_steamid.m_comp.m_EUniverse = EUniverse.k_EUniverseInvalid;
            m_steamid.m_comp.m_unAccountInstance = 0;
        }

        //-----------------------------------------------------------------------------
        // Purpose: Converts steam ID to its 64-bit representation
        // Output : 64-bit representation of a Steam ID
        //-----------------------------------------------------------------------------
        public UInt64 ConvertToUInt64()
        {
            return m_steamid.m_unAll64Bits;
        }


        //-----------------------------------------------------------------------------
        // Purpose: Converts the static parts of a steam ID to a 64-bit representation.
        //			For multiseat accounts, all instances of that account will have the
        //			same static account key, so they can be grouped together by the static
        //			account key.
        // Output : 64-bit static account key
        //-----------------------------------------------------------------------------
        public UInt64 GetStaticAccountKey()
        {
            // note we do NOT include the account instance (which is a dynamic property) in the static account key
            return (UInt64)((((UInt64)m_steamid.m_comp.m_EUniverse) << 56) + ((UInt64)m_steamid.m_comp.m_EAccountType << 52) + m_steamid.m_comp.m_unAccountID);
        }


        //-----------------------------------------------------------------------------
        // Purpose: create an anonymous game server login to be filled in by the AM
        //-----------------------------------------------------------------------------
        public void CreateBlankAnonLogon(EUniverse eUniverse)
        {
            m_steamid.m_comp.m_unAccountID = 0;
            m_steamid.m_comp.m_EAccountType = (uint)EAccountType.k_EAccountTypeAnonGameServer;
            m_steamid.m_comp.m_EUniverse = eUniverse;
            m_steamid.m_comp.m_unAccountInstance = 0;
        }


        //-----------------------------------------------------------------------------
        // Purpose: create an anonymous game server login to be filled in by the AM
        //-----------------------------------------------------------------------------
        public void CreateBlankAnonUserLogon(EUniverse eUniverse)
        {
            m_steamid.m_comp.m_unAccountID = 0;
            m_steamid.m_comp.m_EAccountType = (uint)EAccountType.k_EAccountTypeAnonUser;
            m_steamid.m_comp.m_EUniverse = eUniverse;
            m_steamid.m_comp.m_unAccountInstance = 0;
        }

        //-----------------------------------------------------------------------------
        // Purpose: Is this an anonymous game server login that will be filled in?
        //-----------------------------------------------------------------------------
        public bool BBlankAnonAccount()
        {
            return m_steamid.m_comp.m_unAccountID == 0 && BAnonAccount() && m_steamid.m_comp.m_unAccountInstance == 0;
        }

        //-----------------------------------------------------------------------------
        // Purpose: Is this a game server account id?  (Either persistent or anonymous)
        //-----------------------------------------------------------------------------
        public bool BGameServerAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeGameServer || m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeAnonGameServer;
        }

        //-----------------------------------------------------------------------------
        // Purpose: Is this a persistent (not anonymous) game server account id?
        //-----------------------------------------------------------------------------
        public bool BPersistentGameServerAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeGameServer;
        }

        //-----------------------------------------------------------------------------
        // Purpose: Is this an anonymous game server account id?
        //-----------------------------------------------------------------------------
        public bool BAnonGameServerAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeAnonGameServer;
        }

        //-----------------------------------------------------------------------------
        // Purpose: Is this a content server account id?
        //-----------------------------------------------------------------------------
        public bool BContentServerAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeContentServer;
        }


        //-----------------------------------------------------------------------------
        // Purpose: Is this a clan account id?
        //-----------------------------------------------------------------------------
        public bool BClanAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeClan;
        }


        //-----------------------------------------------------------------------------
        // Purpose: Is this a chat account id?
        //-----------------------------------------------------------------------------
        public bool BChatAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeChat;
        }

        //-----------------------------------------------------------------------------
        // Purpose: Is this a chat account id?
        //-----------------------------------------------------------------------------
        public bool IsLobby()
        {
            return (m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeChat)
             && ((m_steamid.m_comp.m_unAccountInstance & (0x000FFFFF + 1) >> 2) != 0);
        }


        //-----------------------------------------------------------------------------
        // Purpose: Is this an individual user account id?
        //-----------------------------------------------------------------------------
        public bool BIndividualAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeIndividual || m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeConsoleUser;
        }


        //-----------------------------------------------------------------------------
        // Purpose: Is this an anonymous account?
        //-----------------------------------------------------------------------------
        public bool BAnonAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeAnonUser || m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeAnonGameServer;
        }

        //-----------------------------------------------------------------------------
        // Purpose: Is this an anonymous user account? ( used to create an account or reset a password )
        //-----------------------------------------------------------------------------
        public bool BAnonUserAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeAnonUser;
        }

        //-----------------------------------------------------------------------------
        // Purpose: Is this a faked up Steam ID for a PSN friend account?
        //-----------------------------------------------------------------------------
        public bool BConsoleUserAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeConsoleUser;
        }

        // simple accessors
        public void SetAccountID(UInt32 unAccountID) { m_steamid.m_comp.m_unAccountID = unAccountID; }
        public void SetAccountInstance(UInt32 unInstance) { m_steamid.m_comp.m_unAccountInstance = unInstance; }

        public AccountID_t GetAccountID() { return m_steamid.m_comp.m_unAccountID; }
        public UInt32 GetUnAccountInstance() { return m_steamid.m_comp.m_unAccountInstance; }
        public EAccountType GetEAccountType() { return (EAccountType)m_steamid.m_comp.m_EAccountType; }
        public EUniverse GetEUniverse() { return m_steamid.m_comp.m_EUniverse; }
        public void SetEUniverse(EUniverse eUniverse) { m_steamid.m_comp.m_EUniverse = eUniverse; }

        public string Render()
        {
            switch ((EAccountType)m_steamid.m_comp.m_EAccountType)
            {
                case EAccountType.k_EAccountTypeInvalid:
                case EAccountType.k_EAccountTypeIndividual:
                    if (m_steamid.m_comp.m_EUniverse <= EUniverse.k_EUniversePublic)
                        return String.Format("STEAM_0:{0}:{1}", m_steamid.m_comp.m_unAccountID & 1, m_steamid.m_comp.m_unAccountID >> 1);
                    else
                        return String.Format("STEAM_{2}:{0}:{1}", m_steamid.m_comp.m_unAccountID & 1, m_steamid.m_comp.m_unAccountID >> 1, (int)m_steamid.m_comp.m_EUniverse);
                default:
                    return Convert.ToString(this);
            }
        }            // renders this steam ID to string

        //public void SetFromString(string pchSteamID, EUniverse eDefaultUniverse);
        // SetFromString allows many partially-correct strings, raining how
        // we might be able to change things in the future.
        // SetFromStringStrict requires the exact string forms that we support
        // and is preferred when the caller knows it's safe to be strict.
        // Returns whether the string parsed correctly.
        //public bool SetFromStringStrict(string pchSteamID, EUniverse eDefaultUniverse);
        //public bool SetFromSteam2String(string pchSteam2ID, EUniverse eUniverse);

        public static bool operator ==(CSteamID x, ulong y) { return x.m_steamid.m_unAll64Bits == y; }
        public static bool operator !=(CSteamID x, ulong y) { return !(x.m_steamid.m_unAll64Bits == y); }
        public static bool operator ==(ulong y, CSteamID x) { return x.m_steamid.m_unAll64Bits == y; }
        public static bool operator !=(ulong y, CSteamID x) { return !(x.m_steamid.m_unAll64Bits == y); }
        public static bool operator <(ulong y, CSteamID x) { return y < x.m_steamid.m_unAll64Bits; }
        public static bool operator >(ulong y, CSteamID x) { return y > x.m_steamid.m_unAll64Bits; }
        public bool Equals(ulong other) { return this.m_steamid.m_unAll64Bits == other; }
        public int CompareTo(ulong other) { return this.m_steamid.m_unAll64Bits.CompareTo(other); }
        public int CompareTo(CSteamID other) { return this.m_steamid.m_unAll64Bits.CompareTo(other.m_steamid.m_unAll64Bits); }
        public bool Equals(CSteamID other) {  return this.m_steamid.m_unAll64Bits == other.m_steamid.m_unAll64Bits; }

        public static explicit operator ulong(CSteamID that) { return that.m_steamid.m_unAll64Bits; }
        public static explicit operator string(CSteamID that) { return (string)that; }

        // DEBUG function
        //bool BValidExternalSteamID();

        // These are defined here to prevent accidental implicit conversion of a u32AccountID to a CSteamID.
        // If you get a compiler error about an ambiguous ructor/function then it may be because you're
        // passing a 32-bit int to a function that takes a CSteamID. You should explicitly create the SteamID
        // using the correct Universe and account Type/Instance values.
        //CSteamID(UInt32 );
        //CSteamID(int32 );

        public SteamID_t m_steamid = new SteamID_t();

        public bool IsValid()
        {
            if (m_steamid.m_comp.m_EAccountType <= (uint)EAccountType.k_EAccountTypeInvalid || m_steamid.m_comp.m_EAccountType >= (uint)EAccountType.k_EAccountTypeMax)
                return false;

            if (m_steamid.m_comp.m_EUniverse <= EUniverse.k_EUniverseInvalid || m_steamid.m_comp.m_EUniverse >= EUniverse.k_EUniverseMax)
                return false;

            if (m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeIndividual)
            {
                if (m_steamid.m_comp.m_unAccountID == 0 || m_steamid.m_comp.m_unAccountInstance != k_unSteamUserDefaultInstance)
                    return false;
            }

            if (m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeClan)
            {
                if (m_steamid.m_comp.m_unAccountID == 0 || m_steamid.m_comp.m_unAccountInstance != 0)
                    return false;
            }

            if (m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeGameServer)
            {
                if (m_steamid.m_comp.m_unAccountID == 0)
                    return false;
                // Any limit on instances?  We use them for local users and bots
            }
            return true;
        }

        // generic invalid CSteamID
        //public const CSteamID k_steamIDNil = new CSteamID();

        // This steamID comes from a user game connection to an out of date GS that hasnt implemented the protocol
        // to provide its steamID
        //public const CSteamID k_steamIDOutofDateGS = new CSteamID(0, 0, EUniverse.k_EUniverseInvalid, EAccountType.k_EAccountTypeInvalid);
        // This steamID comes from a user game connection to an sv_lan GS
        //public const CSteamID k_steamIDLanModeGS = new CSteamID(0, 0, EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeInvalid);
        // This steamID can come from a user game connection to a GS that has just booted but hasnt yet even initialized
        // its steam3 component and started logging on.
        //public const CSteamID k_steamIDNotInitYetGS = new CSteamID(1, 0, EUniverse.k_EUniverseInvalid, EAccountType.k_EAccountTypeInvalid);
        // This steamID can come from a user game connection to a GS that isn't using the steam authentication system but still
        // wants to support the "Join Game" option in the friends list
        //public const CSteamID k_steamIDNonSteamGS = new CSteamID(2, 0, EUniverse.k_EUniverseInvalid, EAccountType.k_EAccountTypeInvalid);

        public const uint k_unSteamAccountIDMask = 0xFFFFFFFF;
        public const uint k_unSteamAccountInstanceMask = 0x000FFFFF;
        public const uint k_unSteamUserDefaultInstance = 1; // fixed instance for all individual users

        //# ifdef STEAM
        //    // Returns the matching chat steamID, with the default instance of 0
        //    // If the steamID passed in is already of type k_EAccountTypeChat it will be returned with the same instance
        //    CSteamID ChatIDFromSteamID(  CSteamID &steamID );
        //// Returns the matching clan steamID, with the default instance of 0
        //// If the steamID passed in is already of type k_EAccountTypeClan it will be returned with the same instance
        //CSteamID ClanIDFromSteamID(  CSteamID &steamID );
        //// Asserts steamID type before conversion
        //CSteamID ChatIDFromClanID(  CSteamID &steamIDClan );
        //// Asserts steamID type before conversion
        //CSteamID ClanIDFromChatID(  CSteamID &steamIDChat );

        //#endif // _STEAM
    }

    [StructLayout(LayoutKind.Sequential)]
    // 64 bits total
    public class SteamID_t
    {
        public SteamIDComponent_t m_comp = new SteamIDComponent_t();
        public UInt64 m_unAll64Bits;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class SteamIDComponent_t
    {
        public UInt32 m_unAccountID; //: 32;          // unique account identifier
        public uint m_unAccountInstance; //: 20;  // dynamic instance ID
        public uint m_EAccountType; //: 4;            // type of account - can't show as EAccountType, due to signed / unsigned difference
        public EUniverse m_EUniverse; //: 8;  // universe this account belongs to
    }

    // Steam universes.  Each universe is a self-contained Steam instance.
    public enum EUniverse
    {
        k_EUniverseInvalid = 0,
        k_EUniversePublic = 1,
        k_EUniverseBeta = 2,
        k_EUniverseInternal = 3,
        k_EUniverseDev = 4,
        // k_EUniverseRC = 5,				// no such universe anymore
        k_EUniverseMax
    };

    // Steam account types
    public enum EAccountType
    {
        k_EAccountTypeInvalid = 0,
        k_EAccountTypeIndividual = 1,       // single user account
        k_EAccountTypeMultiseat = 2,        // multiseat (e.g. cybercafe) account
        k_EAccountTypeGameServer = 3,       // game server account
        k_EAccountTypeAnonGameServer = 4,   // anonymous game server account
        k_EAccountTypePending = 5,          // pending
        k_EAccountTypeContentServer = 6,    // content server
        k_EAccountTypeClan = 7,
        k_EAccountTypeChat = 8,
        k_EAccountTypeConsoleUser = 9,      // Fake SteamID for local PSN account on PS3 or Live account on 360, etc.
        k_EAccountTypeAnonUser = 10,

        // Max of 16 items in this field
        k_EAccountTypeMax
    };

    // Special flags for Chat accounts - they go in the top 8 bits
    // of the steam ID's "instance", leaving 12 for the actual instances
    enum EChatSteamIDInstanceFlags
    {
        k_EChatAccountInstanceMask = 0x00000FFF, // top 8 bits are flags

        k_EChatInstanceFlagClan = (0x000FFFFF + 1) >> 1,  // top bit
        k_EChatInstanceFlagLobby = (0x000FFFFF + 1) >> 2, // next one down, etc
        k_EChatInstanceFlagMMSLobby = (0x000FFFFF + 1) >> 3,  // next one down, etc

        // Max of 8 flags
    };
}
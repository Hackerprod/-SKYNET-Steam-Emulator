using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public class CSteamID2 : IEquatable<CSteamID2>, IComparable<CSteamID2>, IEquatable<ulong>, IComparable<ulong>
    {
        public SteamID_t m_steamid;

        #region Extentions

        const uint k_unSteamAccountIDMask = 0xFFFFFFFF;
        const uint k_unSteamAccountInstanceMask = 0x000FFFFF;
        const uint k_unSteamUserDefaultInstance = 1; // fixed instance for all individual users

        //-----------------------------------------------------------------------------
        // Purpose: Constructor
        //-----------------------------------------------------------------------------
        public CSteamID2()
        {
            m_steamid = new SteamID_t();
            m_steamid.m_comp.m_unAccountID = 0;
            m_steamid.m_comp.m_EAccountType = (uint)EAccountType.k_EAccountTypeInvalid;
            m_steamid.m_comp.m_EUniverse = EUniverse.k_EUniverseInvalid;
            m_steamid.m_comp.m_unAccountInstance = 0;
        }


        //-----------------------------------------------------------------------------
        // Purpose: Constructor
        // Input  : unAccountID -	32-bit account ID
        //			eUniverse -		Universe this account belongs to
        //			eAccountType -	Type of account
        //-----------------------------------------------------------------------------
        public CSteamID2(UInt32 unAccountID, EUniverse eUniverse, EAccountType eAccountType)
        {
            Set(unAccountID, eUniverse, eAccountType);
        }


        //-----------------------------------------------------------------------------
        // Purpose: Constructor
        // Input  : unAccountID -	32-bit account ID
        //			unAccountInstance - instance
        //			eUniverse -		Universe this account belongs to
        //			eAccountType -	Type of account
        //-----------------------------------------------------------------------------
        public CSteamID2(UInt32 unAccountID, uint unAccountInstance, EUniverse eUniverse, EAccountType eAccountType)
        {
            InstancedSet(unAccountID, unAccountInstance, eUniverse, eAccountType);
        }


        //-----------------------------------------------------------------------------
        // Purpose: Constructor
        // Input  : ulSteamID -		64-bit representation of a Steam ID
        // Note:	Will not accept a uint32 or int32 as input, as that is a probable mistake.
        //			See the stubbed out overloads in the private: section for more info.
        //-----------------------------------------------------------------------------
        public CSteamID2(UInt64 ulSteamID)
        {
            SetFromUint64(ulSteamID);
        }


        //-----------------------------------------------------------------------------
        // Purpose: Sets parameters for steam ID
        // Input  : unAccountID -	32-bit account ID
        //			eUniverse -		Universe this account belongs to
        //			eAccountType -	Type of account
        //-----------------------------------------------------------------------------
        public void Set(UInt32 unAccountID, EUniverse eUniverse, EAccountType eAccountType)
        {
            m_steamid = new SteamID_t();
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

            // 
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
            m_steamid = new SteamID_t();
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
            m_steamid = new SteamID_t();
            m_steamid.m_comp.m_unAccountID = (uint)(ulIdentifier & k_unSteamAccountIDMask);                       // account ID is low 32 bits
            m_steamid.m_comp.m_unAccountInstance = (uint)((ulIdentifier >> 32) & k_unSteamAccountInstanceMask);           // account instance is next 20 bits
            m_steamid.m_comp.m_EUniverse = eUniverse;
            m_steamid.m_comp.m_EAccountType = (uint)eAccountType;
        }

        //-----------------------------------------------------------------------------
        // Purpose: Initializes a steam ID from its 64-bit representation
        // Input  : ulSteamID -		64-bit representation of a Steam ID
        //-----------------------------------------------------------------------------
        public void SetFromUint64(UInt64 ulSteamID)
        {
            m_steamid = new SteamID_t();
            m_steamid.m_unAll64Bits = ulSteamID;
        }

        //-----------------------------------------------------------------------------
        // Purpose: Converts steam ID to its 64-bit representation
        // Output : 64-bit representation of a Steam ID
        //-----------------------------------------------------------------------------
        public UInt64 ConvertToUint64()
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
        void CreateBlankAnonLogon(EUniverse eUniverse)
        {
            m_steamid.m_comp.m_unAccountID = 0;
            m_steamid.m_comp.m_EAccountType = (uint)EAccountType.k_EAccountTypeAnonGameServer;
            m_steamid.m_comp.m_EUniverse = eUniverse;
            m_steamid.m_comp.m_unAccountInstance = 0;
        }

        //-----------------------------------------------------------------------------
        // Purpose: create an anonymous game server login to be filled in by the AM
        //-----------------------------------------------------------------------------
        void CreateBlankAnonUserLogon(EUniverse eUniverse)
        {
            m_steamid.m_comp.m_unAccountID = 0;
            m_steamid.m_comp.m_EAccountType = (uint)EAccountType.k_EAccountTypeAnonUser;
            m_steamid.m_comp.m_EUniverse = eUniverse;
            m_steamid.m_comp.m_unAccountInstance = 0;
        }

        //-----------------------------------------------------------------------------
        // Purpose: Is this an anonymous game server login that will be filled in?
        //-----------------------------------------------------------------------------
        bool BBlankAnonAccount()
        {
            return m_steamid.m_comp.m_unAccountID == 0 && BAnonAccount() && m_steamid.m_comp.m_unAccountInstance == 0;
        }

        //-----------------------------------------------------------------------------
        // Purpose: Is this a game server account id?
        //-----------------------------------------------------------------------------
        bool BGameServerAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeGameServer || m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeAnonGameServer;
        }

        //-----------------------------------------------------------------------------
        // Purpose: Is this a content server account id?
        //-----------------------------------------------------------------------------
        bool BContentServerAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeContentServer;
        }


        //-----------------------------------------------------------------------------
        // Purpose: Is this a clan account id?
        //-----------------------------------------------------------------------------
        bool BClanAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeClan;
        }


        //-----------------------------------------------------------------------------
        // Purpose: Is this a chat account id?
        //-----------------------------------------------------------------------------
        bool BChatAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeChat;
        }

        //-----------------------------------------------------------------------------
        // Purpose: Is this a chat account id?
        //-----------------------------------------------------------------------------
        //bool IsLobby()
        //{
        //    return (m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeChat)
        //                && (m_steamid.m_comp.m_unAccountInstance & (uint)EChatSteamIDInstanceFlags.k_EChatInstanceFlagLobby);
        //}

        //-----------------------------------------------------------------------------
        // Purpose: Is this an individual user account id?
        //-----------------------------------------------------------------------------
        bool BIndividualAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeIndividual || m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeConsoleUser;
        }

        //-----------------------------------------------------------------------------
        // Purpose: Is this an anonymous account?
        //-----------------------------------------------------------------------------
        bool BAnonAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeAnonUser || m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeAnonGameServer;
        }

        //-----------------------------------------------------------------------------
        // Purpose: Is this an anonymous user account? ( used to create an account or reset a password )
        //-----------------------------------------------------------------------------
        bool BAnonUserAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeAnonUser;
        }

        //-----------------------------------------------------------------------------
        // Purpose: Is this a faked up Steam ID for a PSN friend account?
        //-----------------------------------------------------------------------------
        bool BConsoleUserAccount()
        {
            return m_steamid.m_comp.m_EAccountType == (uint)EAccountType.k_EAccountTypeConsoleUser;
        }

        public int CompareTo(CSteamID2 other)
        {
            return this.m_steamid.m_unAll64Bits.CompareTo(other.m_steamid.m_unAll64Bits);
        }

        public bool Equals(CSteamID2 other)
        {
            return this.m_steamid.m_unAll64Bits == other.m_steamid.m_unAll64Bits;
        }

        public static bool operator ==(CSteamID2 x, CSteamID2 y)
        {
            return x.m_steamid.m_unAll64Bits == y.m_steamid.m_unAll64Bits;
        }

        public static bool operator !=(CSteamID2 x, CSteamID2 y)
        {
            return !(x.m_steamid.m_unAll64Bits == y.m_steamid.m_unAll64Bits);
        }

        //public static explicit operator CSteamID2(ulong value)
        //{
        //    return new CSteamID2(value);
        //}

        public static explicit operator ulong(CSteamID2 that)
        {
            return that.m_steamid.m_unAll64Bits;
        }

        public override string ToString()
        {
            return $"[U:{m_steamid.m_comp.m_EUniverse}:{m_steamid.m_comp.m_unAccountID}] ({m_steamid.m_unAll64Bits})";
        }

        public static explicit operator string(CSteamID2 that)
        {
            return (string)that;
        }

        public bool Equals(ulong other)
        {
            return this.m_steamid.m_unAll64Bits == other;
        }

        public int CompareTo(ulong other)
        {
            return this.m_steamid.m_unAll64Bits.CompareTo(other);
        }

        public static bool operator ==(CSteamID2 x, ulong y)
        {
            return x.m_steamid.m_unAll64Bits == y;
        }

        public static bool operator !=(CSteamID2 x, ulong y)
        {
            return !(x.m_steamid.m_unAll64Bits == y);
        }

        public static bool operator ==(ulong y, CSteamID2 x)
        {
            return x.m_steamid.m_unAll64Bits == y;
        }

        public static bool operator !=(ulong y, CSteamID2 x)
        {
            return !(x.m_steamid.m_unAll64Bits == y);
        }

        //internal static CSteamID2 GenerateGameServer()
        //{
        //    return new CSteamID((uint)new Random().Next(1000, 9999), EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeGameServer);
        //}

        //internal static CSteamID2 CreateUnauthenticatedUser()
        //{
        //    return new CSteamID((uint)new Random().Next(1000, 9999), EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeAnonUser);
        //}

        // Special flags for Chat accounts - they go in the top 8 bits
        // of the steam ID's "instance", leaving 12 for the actual instances
        public enum EChatSteamIDInstanceFlags : uint
        {
            k_EChatAccountInstanceMask = 0x00000FFF, // top 8 bits are flags

            k_EChatInstanceFlagClan = (k_unSteamAccountInstanceMask + 1) >> 1,  // top bit
            k_EChatInstanceFlagLobby = (k_unSteamAccountInstanceMask + 1) >> 2, // next one down, etc
            k_EChatInstanceFlagMMSLobby = (k_unSteamAccountInstanceMask + 1) >> 3,  // next one down, etc

            // Max of 8 flags
        };

        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public class SteamID_t
    {
        public SteamIDComponent_t m_comp;
        public ulong m_unAll64Bits;
        public SteamID_t()
        {
            m_comp = new SteamIDComponent_t();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class SteamIDComponent_t
    {
        public uint m_unAccountID;
        public uint m_unAccountInstance;
        public uint m_EAccountType;
        public EUniverse m_EUniverse;
    }
}

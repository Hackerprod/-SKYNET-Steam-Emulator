using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CSteamID : IEquatable<CSteamID>, IComparable<CSteamID>, IEquatable<ulong>, IComparable<ulong>
    {
        public ulong SteamID;
        public uint AccountID;
        public byte Universe;
        public byte AccountType;

        public static CSteamID Invalid = (CSteamID)0;

        public CSteamID(uint accountId)
        {
            this.AccountID = accountId;
            this.Universe = (byte)EUniverse.k_EUniversePublic;
            this.AccountType = (byte)EAccountType.k_EAccountTypeIndividual;

            int instance = (AccountType == (byte)EAccountType.k_EAccountTypeClan || AccountType == (byte)EAccountType.k_EAccountTypeGameServer) ? 0 : 1;

            this.SteamID = 0;
            this.SteamID = (SteamID & ~(0xFFFFFFFFul << (ushort)0)) | (((ulong)(AccountID) & 0xFFFFFFFFul) << (ushort)0);
            this.SteamID = (SteamID & ~(0xFFul << (ushort)56)) | (((ulong)(Universe) & 0xFFul) << (ushort)56);
            this.SteamID = (SteamID & ~(0xFul << (ushort)52)) | (((ulong)(AccountType) & 0xFul) << (ushort)52);
            this.SteamID = (SteamID & ~(0xFFFFFul << (ushort)32)) | (((ulong)(instance) & 0xFFFFFul) << (ushort)32);
        }

        public CSteamID(ulong _steamID)
        {
            this.SteamID = _steamID;
            this.AccountID = (uint)(_steamID & 0xFFFFFFFFul);
            this.Universe = (byte)(EUniverse)((_steamID >> 56) & 0xFFul);
            this.AccountType = (byte)(EAccountType)((_steamID >> 52) & 0xFul);
        }

        public CSteamID(uint _accountId, EUniverse _Universe, EAccountType _AccountType)
        {
            this.AccountID = _accountId;
            this.Universe = (byte)_Universe;
            this.AccountType = (byte)_AccountType;

            int instance = (_AccountType == EAccountType.k_EAccountTypeClan || _AccountType == EAccountType.k_EAccountTypeGameServer) ? 0 : 1;

            this.SteamID = 0;
            this.SteamID = (SteamID & ~(0xFFFFFFFFul << (ushort)0)) | (((ulong)(_accountId) & 0xFFFFFFFFul) << (ushort)0);
            this.SteamID = (SteamID & ~(0xFFul << (ushort)56)) | (((ulong)(_Universe) & 0xFFul) << (ushort)56);
            this.SteamID = (SteamID & ~(0xFul << (ushort)52)) | (((ulong)(_AccountType) & 0xFul) << (ushort)52);
            this.SteamID = (SteamID & ~(0xFFFFFul << (ushort)32)) | (((ulong)(instance) & 0xFFFFFul) << (ushort)32);
        }

        public static CSteamID CreateOne(bool GameServer = false)
        {
            if (GameServer)
            {
                return new CSteamID((uint)new Random().Next(1000, 9999), EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeGameServer);
            }
            return new CSteamID((uint)new Random().Next(1000, 9999), EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeIndividual);
        }

        public override int GetHashCode()
        {
            return this.SteamID.GetHashCode();
        }

        public int CompareTo(CSteamID other)
        {
            return this.SteamID.CompareTo(other.SteamID);
        }

        public bool Equals(CSteamID other)
        {
            return this.SteamID == other.SteamID;
        }

        public static bool operator ==(CSteamID x, CSteamID y)
        {
            return x.SteamID == y.SteamID;
        }

        public static bool operator !=(CSteamID x, CSteamID y)
        {
            return !(x.SteamID == y.SteamID);
        }

        public static explicit operator CSteamID(ulong value)
        {
            return new CSteamID(value);
        }

        public static explicit operator ulong(CSteamID that)
        {
            return that.SteamID;
        }

        public override string ToString()
        {
            return $"[U:{Universe}:{AccountID}] ({SteamID})";
        }

        public static explicit operator string(CSteamID that)
        {
            return (string)that;
        }

        public bool Equals(ulong other)
        {
            return this.SteamID == other;
        }

        public int CompareTo(ulong other)
        {
            return this.SteamID.CompareTo(other);
        }

        public static bool operator ==(CSteamID x, ulong y)
        {
            return x.SteamID == y;
        }

        public static bool operator !=(CSteamID x, ulong y)
        {
            return !(x.SteamID == y);
        }

        public static bool operator ==(ulong y, CSteamID x)
        {
            return x.SteamID == y;
        }

        public static bool operator !=(ulong y, CSteamID x)
        {
            return !(x.SteamID == y);
        }

        internal static CSteamID GenerateGameServer()
        {
            return new CSteamID((uint)new Random().Next(1000, 9999), EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeGameServer);
        }

        internal static CSteamID CreateUnauthenticatedUser()
        {
            return new CSteamID((uint)new Random().Next(1000, 9999), EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeAnonUser);
        }
    }
}


public class CSteamID2
{
    private ulong steamID;

    private const uint k_unSteamUserDefaultInstance = 1;

    public enum EAccountType
    {
        Invalid = 0,
        Individual = 1,
        Multiseat = 2,
        GameServer = 3,
        AnonGameServer = 4,
        Pending = 5,
        ContentServer = 6,
        Clan = 7,
        Chat = 8,
        ConsoleUser = 9,
        AnonUser = 10,
        Max
    }

    public enum EUniverse
    {
        Invalid = 0,
        Public = 1,
        Beta = 2,
        Internal = 3,
        Dev = 4,
        Max
    }

    public CSteamID2()
    {
        Set(0, EUniverse.Invalid, EAccountType.Invalid);
    }

    public CSteamID2(uint accountId, EUniverse universe, EAccountType accountType)
    {
        Set(accountId, universe, accountType);
    }

    public void Set(uint accountId, EUniverse universe, EAccountType accountType)
    {
        uint instance = k_unSteamUserDefaultInstance;
        if (accountType == EAccountType.Clan || accountType == EAccountType.GameServer)
        {
            instance = 0;
        }

        SetInternal(accountId, instance, universe, accountType);
    }

    private void SetInternal(uint accountId, uint instance, EUniverse universe, EAccountType accountType)
    {
        steamID = 0;
        steamID |= (ulong)accountId;
        steamID |= (ulong)instance << 32;
        steamID |= (ulong)accountType << 52;
        steamID |= (ulong)universe << 56;
    }

    public ulong ConvertToUint64()
    {
        return steamID;
    }

    public override string ToString()
    {
        uint accountId = (uint)(steamID & 0xFFFFFFFF);
        uint instance = (uint)((steamID >> 32) & 0xFFFFF);
        EAccountType accountType = (EAccountType)((steamID >> 52) & 0xF);
        EUniverse universe = (EUniverse)((steamID >> 56) & 0xFF);

        return $"[U:{(int)universe}:{accountId}] (Type: {accountType}, Instance: {instance})";
    }

    public static CSteamID2 CreateDefault()
    {
        return new CSteamID2(12345, EUniverse.Public, EAccountType.Individual); // Cambiar a valores válidos.
    }
}

using SKYNET.Steamworks;
using SKYNET.Types;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Steamworks
{
    [StructLayout(LayoutKind.Sequential)]
    public class CSteamID : IEquatable<CSteamID>, IComparable<CSteamID>
    {
        public ulong SteamID;
        public uint AccountId;
        public byte Universe;
        public byte AccountType;

        public CSteamID()
        {
            this.AccountId = (uint)new Random().Next(1000, 9999);
            this.Universe = (byte)EUniverse.k_EUniversePublic;
            this.AccountType = (byte)EAccountType.k_EAccountTypeIndividual;

            int instance = (AccountType == (byte)EAccountType.k_EAccountTypeClan || AccountType == (byte)EAccountType.k_EAccountTypeGameServer) ? 0 : 1;

            this.SteamID = (SteamID & ~(0xFFFFFFFFul << (ushort)0)) | (((ulong)(AccountId) & 0xFFFFFFFFul) << (ushort)0);
            this.SteamID = (SteamID & ~(0xFFul << (ushort)56)) | (((ulong)(Universe) & 0xFFul) << (ushort)56);
            this.SteamID = (SteamID & ~(0xFul << (ushort)52)) | (((ulong)(AccountType) & 0xFul) << (ushort)52);
            this.SteamID = (SteamID & ~(0xFFFFFul << (ushort)32)) | (((ulong)(instance) & 0xFFFFFul) << (ushort)32);
        }

        public CSteamID(ulong _steamID)
        {
            this.SteamID = _steamID;
            this.AccountId = (uint)(_steamID & 0xFFFFFFFFul);
            this.Universe = (byte)(EUniverse)((_steamID >> 56) & 0xFFul);
            this.AccountType = (byte)(EAccountType)((_steamID >> 52) & 0xFul);
        }

        public CSteamID(uint _accountId, EUniverse _Universe, EAccountType _AccountType)
        {
            this.AccountId = _accountId;
            this.Universe = (byte)_Universe;
            this.AccountType = (byte)_AccountType;

            int instance = (_AccountType == EAccountType.k_EAccountTypeClan || _AccountType == EAccountType.k_EAccountTypeGameServer) ? 0 : 1;

            this.SteamID = (SteamID & ~(0xFFFFFFFFul << (ushort)0)) | (((ulong)(_accountId) & 0xFFFFFFFFul) << (ushort)0);
            this.SteamID = (SteamID & ~(0xFFul << (ushort)56)) | (((ulong)(_Universe) & 0xFFul) << (ushort)56);
            this.SteamID = (SteamID & ~(0xFul << (ushort)52)) | (((ulong)(_AccountType) & 0xFul) << (ushort)52);
            this.SteamID = (SteamID & ~(0xFFFFFul << (ushort)32)) | (((ulong)(instance) & 0xFFFFFul) << (ushort)32);
        }

        public override int GetHashCode()
        {
            return this.SteamID.GetHashCode();
        }

        public bool Equals(uint other)
        {
            return this.AccountId == other;
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
            return !(x == y);
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
            return $"[U:{Universe}:{AccountId}] ({SteamID})";
        }

        public static explicit operator string(CSteamID that)
        {
            return that.ToString();
        }
    }
}


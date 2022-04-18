using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Steamworks.Types
{
    [Serializable]
    public struct AccountID_t : IEquatable<AccountID_t>, IComparable<AccountID_t>
    {
        public uint m_AccountID;

        public AccountID_t(uint value)
        {
            this.m_AccountID = value;
        }

        public override string ToString()
        {
            return this.m_AccountID.ToString();
        }

        public override bool Equals(object other)
        {
            return other is AccountID_t && this == (AccountID_t)other;
        }

        public override int GetHashCode()
        {
            return this.m_AccountID.GetHashCode();
        }

        public bool Equals(AccountID_t other)
        {
            return this.m_AccountID == other.m_AccountID;
        }

        public int CompareTo(AccountID_t other)
        {
            return this.m_AccountID.CompareTo(other.m_AccountID);
        }

        public static bool operator ==(AccountID_t x, AccountID_t y)
        {
            return x.m_AccountID == y.m_AccountID;
        }

        public static bool operator !=(AccountID_t x, AccountID_t y)
        {
            return !(x == y);
        }

        public static explicit operator AccountID_t(uint value)
        {
            return new AccountID_t(value);
        }

        public static explicit operator uint(AccountID_t that)
        {
            return that.m_AccountID;
        }
    }
}

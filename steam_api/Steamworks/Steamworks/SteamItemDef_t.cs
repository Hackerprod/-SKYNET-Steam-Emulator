using System;

namespace Steamworks
{
	public struct SteamItemDef_t : IEquatable<SteamItemDef_t>, IComparable<SteamItemDef_t>
	{
		public int m_SteamItemDef_t;

		public SteamItemDef_t(int value)
		{
			this.m_SteamItemDef_t = value;
		}

		public override string ToString()
		{
			return this.m_SteamItemDef_t.ToString();
		}

		public override bool Equals(object other)
		{
			return other is SteamItemDef_t && this == (SteamItemDef_t)other;
		}

		public override int GetHashCode()
		{
			return this.m_SteamItemDef_t.GetHashCode();
		}

		public bool Equals(SteamItemDef_t other)
		{
			return this.m_SteamItemDef_t == other.m_SteamItemDef_t;
		}

		public int CompareTo(SteamItemDef_t other)
		{
			return this.m_SteamItemDef_t.CompareTo(other.m_SteamItemDef_t);
		}

		public static bool operator ==(SteamItemDef_t x, SteamItemDef_t y)
		{
			return x.m_SteamItemDef_t == y.m_SteamItemDef_t;
		}

		public static bool operator !=(SteamItemDef_t x, SteamItemDef_t y)
		{
			return !(x == y);
		}

		public static explicit operator SteamItemDef_t(int value)
		{
			return new SteamItemDef_t(value);
		}

		public static explicit operator int(SteamItemDef_t that)
		{
			return that.m_SteamItemDef_t;
		}
	}
}

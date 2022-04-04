/*
 * Generated code file by Il2CppInspector - http://www.djkaty.com - https://github.com/djkaty
 */

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

// Image 110: Assembly-CSharp-firstpass.dll - Assembly: Assembly-CSharp-firstpass, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null

namespace Steamworks
{
	[Serializable]
	public struct SteamAPICall_t : IEquatable<SteamAPICall_t>, IComparable<SteamAPICall_t>
	{
		public static readonly SteamAPICall_t Invalid = new SteamAPICall_t(0uL);

		public ulong m_SteamAPICall;

		public SteamAPICall_t(ulong value)
		{
			m_SteamAPICall = value;
		}

		public override string ToString()
		{
			return m_SteamAPICall.ToString();
		}

		public override bool Equals(object other)
		{
			if (other is SteamAPICall_t)
			{
				return this == (SteamAPICall_t)other;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return m_SteamAPICall.GetHashCode();
		}

		public static bool operator ==(SteamAPICall_t steamAPICall_t_0, SteamAPICall_t steamAPICall_t_1)
		{
			return steamAPICall_t_0.m_SteamAPICall == steamAPICall_t_1.m_SteamAPICall;
		}

		public static bool operator !=(SteamAPICall_t steamAPICall_t_0, SteamAPICall_t steamAPICall_t_1)
		{
			return !(steamAPICall_t_0 == steamAPICall_t_1);
		}

		public static explicit operator SteamAPICall_t(ulong ulong_0)
		{
			return new SteamAPICall_t(ulong_0);
		}

		public static explicit operator ulong(SteamAPICall_t steamAPICall_t_0)
		{
			return steamAPICall_t_0.m_SteamAPICall;
		}

		public bool Equals(SteamAPICall_t other)
		{
			return m_SteamAPICall == other.m_SteamAPICall;
		}

		public int CompareTo(SteamAPICall_t other)
		{
			return m_SteamAPICall.CompareTo(other.m_SteamAPICall);
		}
	}
}


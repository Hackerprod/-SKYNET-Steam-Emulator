using SKYNET.Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET
{
    public class CSteamID2
    {
        public SteamID_t m_steamid;
    }
    public struct SteamIDComponent_t
    {
        public UInt32 m_unAccountID;               // : 32; // unique account identifier
        public int m_unAccountInstance;            // : 20; // dynamic instance ID
        public int m_EAccountType;                 // : 4;  // type of account - can't show as EAccountType, due to signed / unsigned difference
        public EUniverse m_EUniverse;              // : 8;  // universe this account belongs to
    }

    public struct SteamID_t
    {
        public SteamIDComponent_t m_comp;
        public UInt64 m_unAll64Bits;
    }
}

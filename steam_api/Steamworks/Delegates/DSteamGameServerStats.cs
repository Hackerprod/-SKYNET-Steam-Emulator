
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate(Name = "SteamGameServerStats")]
    public class DSteamGameServerStats
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t RequestUserStats(IntPtr steamIDUser);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetUserStat(IntPtr steamIDUser, string pchName, int pData);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetUserAchievement(IntPtr steamIDUser, string pchName, bool pbAchieved);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetUserStat(IntPtr steamIDUser, string pchName, int nData);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool UpdateUserAvgRateStat(IntPtr steamIDUser, string pchName, float flCountThisSession, double dSessionLength);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetUserAchievement(IntPtr steamIDUser, string pchName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool ClearUserAchievement(IntPtr steamIDUser, string pchName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t StoreUserStats(IntPtr steamIDUser);

    }
}

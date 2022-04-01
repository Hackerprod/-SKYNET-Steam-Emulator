using SKYNET.Delegate.Helper;
using SKYNET.Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate(Name = "SteamParentalSettings")]
    public class DSteamParentalSettings 
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsParentalLockEnabled(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsParentalLockLocked(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsAppBlocked(IntPtr nAppID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsAppInBlockList(IntPtr nAppID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsFeatureBlocked(EParentalFeature eFeature);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsFeatureInBlockList(EParentalFeature eFeature);
    }
}

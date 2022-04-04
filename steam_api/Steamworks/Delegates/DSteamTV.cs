
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate(Name = "SteamTV")]
    public class DSteamTV 
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsBroadcasting(int pnNumViewers);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void AddBroadcastGameData(string pchKey, string pchValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void RemoveBroadcastGameData(string pchKey);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void AddTimelineMarker(string pchTemplateName, bool bPersistent, uint nColorR, uint nColorG, uint nColorB);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void RemoveTimelineMarker(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint AddRegion(string pchElementName, string pchTimelineDataSection, IntPtr pSteamTVRegion, int eSteamTVRegionBehavior);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void RemoveRegion(uint unRegionHandle);
    }
}

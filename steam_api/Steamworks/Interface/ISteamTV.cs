using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    public interface ISteamTV
    {
        bool IsBroadcasting(int pnNumViewers);
        void AddBroadcastGameData(string pchKey, string pchValue);
        void RemoveBroadcastGameData(string pchKey);
        void AddTimelineMarker(string pchTemplateName, bool bPersistent, uint nColorR, uint nColorG, uint nColorB);
        void RemoveTimelineMarker();
        uint AddRegion(string pchElementName, string pchTimelineDataSection, IntPtr pSteamTVRegion, int eSteamTVRegionBehavior);
        void RemoveRegion(uint unRegionHandle);
    }
}

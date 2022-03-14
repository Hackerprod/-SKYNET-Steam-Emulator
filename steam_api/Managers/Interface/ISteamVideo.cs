using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    public interface ISteamVideo
    {
        // Get a URL suitable for streaming the given Video app ID's video
        void GetVideoURL(IntPtr unVideoAppID);

        // returns true if user is uploading a live broadcast
        bool IsBroadcasting(int pnNumViewers);

        // Get the OPF Details for 360 Video Playback
        void GetOPFSettings(IntPtr unVideoAppID);
        bool GetOPFStringForApp(IntPtr unVideoAppID, string pchBuffer, uint pnBufferSize);

    }
}

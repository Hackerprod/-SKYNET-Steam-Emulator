using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    public interface ISteamGameServer
    {
        int GetHSteamUser(IntPtr _);

        int GetHSteamPipe(IntPtr _);

        void RunCallbacks(IntPtr _);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steamworks;

namespace SKYNET.Interface
{
    public interface ISteamAppList 
    {
        uint GetNumInstalledApps();
        uint GetInstalledApps(AppId_t pvecAppID, UInt32 unMaxAppIDs);

        int GetAppName(AppId_t nAppID, IntPtr pchName, int cchNameMax); // returns -1 if no name was found
        int GetAppInstallDir(AppId_t nAppID, IntPtr pchDirectory, int cchNameMax); // returns -1 if no dir was found

        int GetAppBuildId(AppId_t nAppID); // return the buildid of this app, may change at any time based on backend updates to the game
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Core.Interface;
using SKYNET.Delegate;
using Steamworks;

namespace SKYNET.Delegate
{
    [Delegate(Name = "SteamAppList")]
    public class DSteamAppList 
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetNumInstalledApps();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetInstalledApps(AppId_t pvecAppID, UInt32 unMaxAppIDs);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetAppName(AppId_t nAppID, IntPtr pchName, int cchNameMax);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetAppInstallDir(AppId_t nAppID, IntPtr pchDirectory, int cchNameMax);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetAppBuildId(AppId_t nAppID);
    }

}

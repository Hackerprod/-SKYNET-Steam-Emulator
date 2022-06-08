using SKYNET.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Client
{
    public class SteamClient
    {

        public SteamClient()
        {

        }

        public void Initialize()
        {
            //NetworkManager.Initialize();
            IPCManager.Initialize();
        }

        internal static void Write(string v1, string v2)
        {

        }
    }
}

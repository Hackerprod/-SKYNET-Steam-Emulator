using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    [Interface("SteamAppDisableUpdate001")]
    public class ISteamAppDisableUpdate001 : ISteamInterface
    {
        public void SetAppUpdateDisabledSecondsRemaining(int seconds)
        {
            SteamEmulator.Write("ISteamAppDisableUpdate001", "SetAppUpdateDisabledSecondsRemaining");
        }
    }
}

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SKYNET;
using SKYNET.Helpers;
using Steamworks;

namespace SKYNET.Steamworks.Implementation
{
    [StructLayout(LayoutKind.Sequential)]
    public class SteamGameServer : ISteamInterface
    {
        public int GetHSteamUser(IntPtr _)
        {
            Write($"GetHSteamUser");
            return 1;
        }

        public int GetHSteamPipe(IntPtr _)
        {
            Write($"GetHSteamPipe");
            return 1;
        }

        public void RunCallbacks(IntPtr _)
        {
            Write($"RunCallbacks");
        }

        public IntPtr MemoryAddress { get; set; }
        public string InterfaceVersion { get; set; }

        public SteamGameServer()
        {
            InterfaceVersion = "SteamGameServer";
        }

        private void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }
    }
}

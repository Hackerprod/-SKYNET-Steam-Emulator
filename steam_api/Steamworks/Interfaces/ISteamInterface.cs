using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Interfaces
{
    public class ISteamInterface
    {
        public const SteamAPICall_t k_uAPICallInvalid = 0x0;
        public CSteamID k_steamIDNil = (CSteamID)0;

        public string InterfaceVersion { get; set; }
        public string InterfaceName { get; set; }

        public void Write(object msg)
        {
            SteamEmulator.Write(InterfaceName, msg);
        }
    }
}

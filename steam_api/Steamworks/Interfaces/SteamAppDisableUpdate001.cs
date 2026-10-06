namespace SKYNET.Steamworks.Interfaces
{
    [InterfaceLayout("SteamAppDisableUpdate001",
        "SetAppUpdateDisabledSecondsRemaining")]
    public class SteamAppDisableUpdate001 : ISteamInterface
    {
        public void SetAppUpdateDisabledSecondsRemaining(System.IntPtr _, int seconds)
        {
            SteamEmulator.Write("ISteamAppDisableUpdate001", $"SetAppUpdateDisabledSecondsRemaining {seconds}");
        }
    }
}

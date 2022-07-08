namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamAppDisableUpdate001")]
    public class SteamAppDisableUpdate001 : ISteamInterface
    {
        public void SetAppUpdateDisabledSecondsRemaining(int seconds)
        {
            SteamEmulator.Write("ISteamAppDisableUpdate001", $"SetAppUpdateDisabledSecondsRemaining {seconds}");
        }
    }
}

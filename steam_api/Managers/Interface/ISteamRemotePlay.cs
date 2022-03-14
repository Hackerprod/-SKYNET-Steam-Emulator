using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    public interface ISteamRemotePlay
    {
        uint GetSessionCount();

        // Get the currently connected Steam Remote Play session ID at the specified index. Returns zero if index is out of bounds.
        uint GetSessionID(int iSessionIndex);

        // Get the SteamID of the connected user
        IntPtr GetSessionSteamID(uint unSessionID);

        // Get the name of the session client device
        // This returns NULL if the sessionID is not valid
        string GetSessionClientName(uint unSessionID);

        // Get the form factor of the session client device
        ESteamDeviceFormFactor GetSessionClientFormFactor(uint unSessionID);

        // Get the resolution, in pixels, of the session client device
        // This is set to 0x0 if the resolution is not available
        bool BGetSessionClientResolution(uint unSessionID, int pnResolutionX, int pnResolutionY);

        // Invite a friend to Remote Play Together
        // This returns false if the invite can't be sent
        bool BSendRemotePlayTogetherInvite(IntPtr steamIDFriend);
    }
    //-----------------------------------------------------------------------------
    // Purpose: The form factor of a device
    //-----------------------------------------------------------------------------
    public enum ESteamDeviceFormFactor : int
    {
        k_ESteamDeviceFormFactorUnknown = 0,
        k_ESteamDeviceFormFactorPhone = 1,
        k_ESteamDeviceFormFactorTablet = 2,
        k_ESteamDeviceFormFactorComputer = 3,
        k_ESteamDeviceFormFactorTV = 4,
    };
}

using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Steamworks
{
    public struct ControllerDigitalActionData_t
    {
        // The current state of this action; will be true if currently pressed
        public bool bState;

        // Whether or not this action is currently available to be bound in the active action set
        public bool bActive;
    };
    public struct ControllerAnalogActionData_t
    {
        // Type of data coming from this action, this will match what got specified in the action set
        public int eMode;

        // The current state of this action; will be delta updates for mouse actions
        public float x, y;

        // Whether or not this action is currently available to be bound in the active action set
        public bool bActive;
    };

    public struct ControllerMotionData_t
    {
        // Sensor-fused absolute rotation; will drift in heading
        float rotQuatX;
        float rotQuatY;
        float rotQuatZ;
        float rotQuatW;

        // Positional acceleration
        float posAccelX;
        float posAccelY;
        float posAccelZ;

        // Angular velocity
        float rotVelX;
        float rotVelY;
        float rotVelZ;
    };

    public struct InputDigitalActionData_t
    {
        // The current state of this action; will be true if currently pressed
        bool bState;

        // Whether or not this action is currently available to be bound in the active action set
        bool bActive;
    };
    public struct InputAnalogActionData_t
    {
        // Type of data coming from this action, this will match what got specified in the action set
        int eMode;

        // The current state of this action; will be delta updates for mouse actions
        float x, y;

        // Whether or not this action is currently available to be bound in the active action set
        bool bActive;
    };
    public struct InputMotionData_t
    {
        // Sensor-fused absolute rotation; will drift in heading
        float rotQuatX;
        float rotQuatY;
        float rotQuatZ;
        float rotQuatW;

        // Positional acceleration
        float posAccelX;
        float posAccelY;
        float posAccelZ;

        // Angular velocity
        float rotVelX;
        float rotVelY;
        float rotVelZ;
    };

    public struct SteamItemDetails_t
    {
        SteamItemInstanceID_t m_itemId;
        SteamItemDef_t m_iDefinition;
        uint m_unQuantity;
        uint m_unFlags; // see ESteamItemFlags
    };

    public struct P2PSessionState_t
    {
        uint m_bConnectionActive;      // true if we've got an active open connection
        uint m_bConnecting;            // true if we're currently trying to establish a connection
        uint m_eP2PSessionError;       // last error recorded (see enum above)
        uint m_bUsingRelay;            // true if it's going through a relay server (TURN)
        uint m_nBytesQueuedForSend;
        uint m_nPacketsQueuedForSend;
        uint m_nRemoteIP;             // potential IP:Port of remote host. Could be TURN server. 
        uint m_nRemotePort;           // Only exists for compatibility with older authentication api's
    };

    public struct SteamDatagramRelayAuthTicket { };
    public struct SteamDatagramHostedAddress { };
    public struct SteamDatagramGameCoordinatorServerLogin { };
    public struct SteamRelayNetworkStatus_t { };

    //
    // Ping location / measurement
    //

    /// Object that describes a "location" on the Internet with sufficient
    /// detail that we can reasonably estimate an upper bound on the ping between
    /// the two hosts, even if a direct route between the hosts is not possible,
    /// and the connection must be routed through the Steam Datagram Relay network.
    /// This does not contain any information that identifies the host.  Indeed,
    /// if two hosts are in the same building or otherwise have nearly identical
    /// networking characteristics, then it's valid to use the same location
    /// object for both of them.
    ///
    /// NOTE: This object should only be used in the same process!  Do not serialize it,
    /// send it over the wire, or persist it in a file or database!  If you need
    /// to do that, convert it to a string representation using the methods in
    /// ISteamNetworkingUtils().
    public struct SteamNetworkPingLocation_t
    {
        uint m_data;
    };

    public struct SteamPartyBeaconLocation_t
    {
        uint m_eType;
        uint m_ulLocationID;
    };

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public delegate void SteamAPIWarningMessageHook_t(int nSeverity, System.Text.StringBuilder pchDebugText);
}

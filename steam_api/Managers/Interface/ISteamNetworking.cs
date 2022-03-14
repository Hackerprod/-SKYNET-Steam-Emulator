using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    public interface ISteamNetworking
    {
        ////////////////////////////////////////////////////////////////////////////////////////////
        //
        // UDP-style (connectionless) networking interface.  These functions send messages using
        // an API organized around the destination.  Reliable and unreliable messages are supported.
        //
        // For a more TCP-style interface (meaning you have a connection handle), see the functions below.
        // Both interface styles can send both reliable and unreliable messages.
        //
        // Automatically establishes NAT-traversing or Relay server connections

        // Sends a P2P packet to the specified user
        // UDP-like, unreliable and a max packet size of 1200 bytes
        // the first packet send may be delayed as the NAT-traversal code runs
        // if we can't get through to the user, an error will be posted via the callback P2PSessionConnectFail_t
        // see EP2PSend enum above for the descriptions of the different ways of sending packets
        //
        // nChannel is a routing number you can use to help route message to different systems 	- you'll have to call ReadP2PPacket() 
        // with the same channel number in order to retrieve the data on the other end
        // using different channels to talk to the same user will still use the same underlying p2p connection, saving on resources
        bool SendP2PPacket(IntPtr steamIDRemote, IntPtr pubData, uint cubData, EP2PSend eP2PSendType, int nChannel);

        // returns true if any data is available for read, and the amount of data that will need to be read
        bool IsP2PPacketAvailable(uint pcubMsgSize, int nChannel);

        // reads in a packet that has been sent from another user via SendP2PPacket()
        // returns the size of the message and the steamID of the user who sent it in the last two parameters
        // if the buffer passed in is too small, the message will be truncated
        // this call is not blocking, and will return false if no data is available
        bool ReadP2PPacket(IntPtr pubDest, uint cubDest, uint pcubMsgSize, IntPtr psteamIDRemote, int nChannel);

        // AcceptP2PSessionWithUser() should only be called in response to a P2PSessionRequest_t callback
        // P2PSessionRequest_t will be posted if another user tries to send you a packet that you haven't talked to yet
        // if you don't want to talk to the user, just ignore the request
        // if the user continues to send you packets, another P2PSessionRequest_t will be posted periodically
        // this may be called multiple times for a single user
        // (if you've called SendP2PPacket() on the other user, this implicitly accepts the session request)
        bool AcceptP2PSessionWithUser(IntPtr steamIDRemote);

        // call CloseP2PSessionWithUser() when you're done talking to a user, will free up resources under-the-hood
        // if the remote user tries to send data to you again, another P2PSessionRequest_t callback will be posted
        bool CloseP2PSessionWithUser(IntPtr steamIDRemote);

        // call CloseP2PChannelWithUser() when you're done talking to a user on a specific channel. Once all channels
        // open channels to a user have been closed, the open session to the user will be closed and new data from this
        // user will trigger a P2PSessionRequest_t callback
        bool CloseP2PChannelWithUser(IntPtr steamIDRemote, int nChannel);

        // fills out P2PSessionState_t structure with details about the underlying connection to the user
        // should only needed for debugging purposes
        // returns false if no connection exists to the specified user
        bool GetP2PSessionState(IntPtr steamIDRemote, P2PSessionState_t pConnectionState);

        // Allow P2P connections to fall back to being relayed through the Steam servers if a direct connection
        // or NAT-traversal cannot be established. Only applies to connections created after setting this value,
        // or to existing connections that need to automatically reconnect after this value is set.
        //
        // P2P packet relay is allowed by default
        bool AllowP2PPacketRelay(bool bAllow);


        ////////////////////////////////////////////////////////////////////////////////////////////
        //
        // LISTEN / CONNECT connection-oriented interface functions
        //
        // These functions are more like a client-server TCP API.  One side is the "server"
        // and "listens" for incoming connections, which then must be "accepted."  The "client"
        // initiates a connection by "connecting."  Sending and receiving is done through a
        // connection handle.
        //
        // For a more UDP-style interface, where you do not track connection handles but
        // simply send messages to a SteamID, use the UDP-style functions above.
        //
        // Both methods can send both reliable and unreliable methods.
        //
        ////////////////////////////////////////////////////////////////////////////////////////////


        // creates a socket and listens others to connect
        // will trigger a SocketStatusCallback_t callback on another client connecting
        // nVirtualP2PPort is the unique ID that the client will connect to, in case you have multiple ports
        //		this can usually just be 0 unless you want multiple sets of connections
        // unIP is the local IP address to bind to
        //		pass in 0 if you just want the default local IP
        // unPort is the port to use
        //		pass in 0 if you don't want users to be able to connect via IP/Port, but expect to be always peer-to-peer connections only
        uint CreateListenSocket(int nVirtualP2PPort, IntPtr nIP, uint nPort, bool bAllowUseOfPacketRelay);

        // creates a socket and begin connection to a remote destination
        // can connect via a known steamID (client or game server), or directly to an IP
        // on success will trigger a SocketStatusCallback_t callback
        // on failure or timeout will trigger a SocketStatusCallback_t callback with a failure code in m_eSNetSocketState
        uint CreateP2PConnectionSocket(IntPtr steamIDTarget, int nVirtualPort, int nTimeoutSec, bool bAllowUseOfPacketRelay);
        uint CreateConnectionSocket(IntPtr nIP, uint nPort, int nTimeoutSec);

        // disconnects the connection to the socket, if any, and invalidates the handle
        // any unread data on the socket will be thrown away
        // if bNotifyRemoteEnd is set, socket will not be completely destroyed until the remote end acknowledges the disconnect
        bool DestroySocket(uint hSocket, bool bNotifyRemoteEnd);
        // destroying a listen socket will automatically kill all the regular sockets generated from it
        bool DestroyListenSocket(uint hSocket, bool bNotifyRemoteEnd);

        // sending data
        // must be a handle to a connected socket
        // data is all sent via UDP, and thus send sizes are limited to 1200 bytes; after this, many routers will start dropping packets
        // use the reliable flag with caution; although the resend rate is pretty aggressive,
        // it can still cause stalls in receiving data (like TCP)
        bool SendDataOnSocket(uint hSocket, IntPtr pubData, uint cubData, bool bReliable);

        // receiving data
        // returns false if there is no data remaining
        // fills out pcubMsgSize with the size of the next message, in bytes
        bool IsDataAvailableOnSocket(uint hSocket, uint pcubMsgSize);

        // fills in pubDest with the contents of the message
        // messages are always complete, of the same size as was sent (i.e. packetized, not streaming)
        // if pcubMsgSize < cubDest, only partial data is written
        // returns false if no data is available
        bool RetrieveDataFromSocket(uint hSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize);

        // checks for data from any socket that has been connected off this listen socket
        // returns false if there is no data remaining
        // fills out pcubMsgSize with the size of the next message, in bytes
        // fills out phSocket with the socket that data is available on
        bool IsDataAvailable(uint hListenSocket, uint pcubMsgSize, uint phSocket);

        // retrieves data from any socket that has been connected off this listen socket
        // fills in pubDest with the contents of the message
        // messages are always complete, of the same size as was sent (i.e. packetized, not streaming)
        // if pcubMsgSize < cubDest, only partial data is written
        // returns false if no data is available
        // fills out phSocket with the socket that data is available on
        bool RetrieveData(uint hListenSocket, IntPtr pubDest, uint cubDest, uint pcubMsgSize, uint phSocket);

        // returns information about the specified socket, filling out the contents of the pointers
        bool GetSocketInfo(uint hSocket, IntPtr pSteamIDRemote, int peSocketStatus, IntPtr punIPRemote, uint punPortRemote);

        // returns which local port the listen socket is bound to
        // pnIP and pnPort will be 0 if the socket is set to listen for P2P connections only
        bool GetListenSocketInfo(uint hListenSocket, IntPtr pnIP, uint pnPort);

        // returns true to describe how the socket ended up connecting
        ESNetSocketConnectionType GetSocketConnectionType(uint hSocket);

        // max packet size, in bytes
        int GetMaxPacketSize(uint hSocket);

    }
    public enum EP2PSend
    {
        // Basic UDP send. Packets can't be bigger than 1200 bytes (your typical MTU size). Can be lost, or arrive out of order (rare).
        // The sending API does have some knowledge of the underlying connection, so if there is no NAT-traversal accomplished or
        // there is a recognized adjustment happening on the connection, the packet will be batched until the connection is open again.
        k_EP2PSendUnreliable = 0,

        // As above, but if the underlying p2p connection isn't yet established the packet will just be thrown away. Using this on the first
        // packet sent to a remote host almost guarantees the packet will be dropped.
        // This is only really useful for kinds of data that should never buffer up, i.e. voice payload packets
        k_EP2PSendUnreliableNoDelay = 1,

        // Reliable message send. Can send up to 1MB of data in a single message. 
        // Does fragmentation/re-assembly of messages under the hood, as well as a sliding window for efficient sends of large chunks of data. 
        k_EP2PSendReliable = 2,

        // As above, but applies the Nagle algorithm to the send - sends will accumulate 
        // until the current MTU size (typically ~1200 bytes, but can change) or ~200ms has passed (Nagle algorithm). 
        // Useful if you want to send a set of smaller messages but have the coalesced into a single packet
        // Since the reliable stream is all ordered, you can do several small message sends with k_EP2PSendReliableWithBuffering and then
        // do a normal k_EP2PSendReliable to force all the buffered data to be sent.
        k_EP2PSendReliableWithBuffering = 3,

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
    // describes how the socket is currently connected
    public enum ESNetSocketConnectionType
    {
        k_ESNetSocketConnectionTypeNotConnected = 0,
        k_ESNetSocketConnectionTypeUDP = 1,
        k_ESNetSocketConnectionTypeUDPRelay = 2,
    };
}

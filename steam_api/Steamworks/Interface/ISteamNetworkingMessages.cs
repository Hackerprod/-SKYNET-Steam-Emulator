using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    public interface ISteamNetworkingMessages
    {
        /// Sends a message to the specified host.  If we don't already have a session with that user,
        /// a session is implicitly created.  There might be some handshaking that needs to happen
        /// before we can actually begin sending message data.  If this handshaking fails and we can't
        /// get through, an error will be posted via the callback SteamNetworkingMessagesSessionFailed_t.
        /// There is no notification when the operation succeeds.  (You should have the peer send a reply
        /// for this purpose.)
        ///
        /// Sending a message to a host will also implicitly accept any incoming connection from that host.
        ///
        /// nSendFlags is a bitmask of k_nSteamNetworkingSend_xxx options
        ///
        /// nRemoteChannel is a routing number you can use to help route message to different systems.
        /// You'll have to call ReceiveMessagesOnChannel() with the same channel number in order to retrieve
        /// the data on the other end.
        ///
        /// Using different channels to talk to the same user will still use the same underlying
        /// connection, saving on resources.  If you don't need this feature, use 0.
        /// Otherwise, small integers are the most efficient.
        ///
        /// It is guaranteed that reliable messages to the same host on the same channel
        /// will be be received by the remote host (if they are received at all) exactly once,
        /// and in the same order that they were sent.
        ///
        /// NO other order guarantees exist!  In particular, unreliable messages may be dropped,
        /// received out of order with respect to each other and with respect to reliable data,
        /// or may be received multiple times.  Messages on different channels are not guaranteed
        /// to be received in the order they were sent.
        ///
        /// A note for those familiar with TCP/IP ports, or converting an existing codebase that
        /// opened multiple sockets:  You might notice that there is only one channel, and with
        /// TCP/IP each endpoint has a port number.  You can think of the channel number as the
        /// destination port.  If you need each message to also include a "source port" (so the
        /// recipient can route the reply), then just put that in your message.  That is essentially
        /// how UDP works!
        ///
        /// Returns:
        /// - k_EREsultOK on success.
        /// - k_EResultNoConnection will be returned if the session has failed or was closed by the peer,
        ///   and k_nSteamNetworkingSend_AutoRestartBrokenSession is not used.  (You can use
        ///   GetSessionConnectionInfo to get the details.)  In order to acknowledge the broken session
        ///   and start a new one, you must call CloseSessionWithUser
        /// - See ISteamNetworkingSockets::SendMessageToConnection for more possible return values
        int SendMessageToUser( IntPtr identityRemote, IntPtr pubData, uint cubData, int nSendFlags, int nRemoteChannel );

        /// Reads the next message that has been sent from another user via SendMessageToUser() on the given channel.
        /// Returns number of messages returned into your list.  (0 if no message are available on that channel.)
        ///
        /// When you're done with the message object(s), make sure and call SteamNetworkingMessage_t::Release()!
        int ReceiveMessagesOnChannel(int nLocalChannel, IntPtr ppOutMessages, int nMaxMessages);

	/// Call this in response to a SteamNetworkingMessagesSessionRequest_t callback.
	/// SteamNetworkingMessagesSessionRequest_t are posted when a user tries to send you a message,
	/// and you haven't tried to talk to them first.  If you don't want to talk to them, just ignore
	/// the request.  If the user continues to send you messages, SteamNetworkingMessagesSessionRequest_t
	/// callbacks will continue to be posted periodically.
	///
	/// Returns false if there is no session with the user pending or otherwise.  If there is an
	/// existing active session, this function will return true, even if it is not pending.
	///
	/// Calling SendMessageToUser() will implicitly accepts any pending session request to that user.
	bool AcceptSessionWithUser( IntPtr identityRemote );

	/// Call this when you're done talking to a user to immediately free up resources under-the-hood.
	/// If the remote user tries to send data to you again, another SteamNetworkingMessagesSessionRequest_t
	/// callback will be posted.
	///
	/// Note that sessions that go unused for a few minutes are automatically timed out.
	bool CloseSessionWithUser( IntPtr identityRemote );

	/// Call this  when you're done talking to a user on a specific channel.  Once all
	/// open channels to a user have been closed, the open session to the user will be
	/// closed, and any new data from this user will trigger a
	/// SteamSteamNetworkingMessagesSessionRequest_t callback
	bool CloseChannelWithUser( IntPtr identityRemote, int nLocalChannel );

        /// Returns information about the latest state of a connection, if any, with the given peer.
        /// Primarily intended for debugging purposes, but can also be used to get more detailed
        /// failure information.  (See SendMessageToUser and k_nSteamNetworkingSend_AutoRestartBrokenSession.)
        ///
        /// Returns the value of SteamNetConnectionInfo_t::m_eState, or k_ESteamNetworkingConnectionState_None
        /// if no connection exists with specified peer.  You may pass nullptr for either parameter if
        /// you do not need the corresponding details.  Note that sessions time out after a while,
        /// so if a connection fails, or SendMessageToUser returns k_EResultNoConnection, you cannot wait
        /// indefinitely to obtain the reason for failure.
        IntPtr GetSessionConnectionInfo( IntPtr identityRemote, IntPtr pConnectionInfo, IntPtr pQuickStatus );

    }
}

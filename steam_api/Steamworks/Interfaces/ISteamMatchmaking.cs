using Steamworks;
using System;

namespace SKYNET.Interface
{
    public interface ISteamMatchmaking
    {
        // game server favorites storage
        // saves basic details about a multiplayer game server locally

        // returns the number of favorites servers the user has stored
        int GetFavoriteGameCount(IntPtr _);

        // returns the details of the game server
        // iGame is of range [0,GetFavoriteGameCount())
        // pnIP, pnConnPort are filled in the with IP:port of the game server
        // punFlags specify whether the game server was stored as an explicit favorite or in the history of connections
        // pRTime32LastPlayedOnServer is filled in the with the Unix time the favorite was added
        bool GetFavoriteGame(int iGame, AppId_t pnAppID, uint pnIP, uint pnConnPort, uint pnQueryPort, uint punFlags, uint pRTime32LastPlayedOnServer);

        // adds the game server to the local list; updates the time played of the server if it already exists in the list
        int AddFavoriteGame(AppId_t nAppID, uint nIP, uint nConnPort, uint nQueryPort, uint unFlags, uint rTime32LastPlayedOnServer);

        // removes the game server from the local storage; returns true if one was removed
        bool RemoveFavoriteGame(AppId_t nAppID, uint nIP, uint nConnPort, uint nQueryPort, uint unFlags);

        ///////
        // Game lobby functions

        // Get a list of relevant lobbies
        // this is an asynchronous request
        // results will be returned by LobbyMatchList_t callback & call result, with the number of lobbies found
        // this will never return lobbies that are full
        // to add more filter, the filter calls below need to be call before each and every RequestLobbyList() call
        // use the CCallResult<> object in steam_api.h to match the SteamAPICall_t call result to a function in an object, e.g.
        /*
            class CMyLobbyListManager
            {
                CCallResult<CMyLobbyListManager, LobbyMatchList_t> m_CallResultLobbyMatchList;
                void FindLobbies()
                {
                    // SteamMatchmaking()->AddRequestLobbyListFilter() functions would be called here, before RequestLobbyList()
                    SteamAPICall_t hSteamAPICall = SteamMatchmaking()->RequestLobbyList();
                    m_CallResultLobbyMatchList.Set( hSteamAPICall, this, &CMyLobbyListManager::OnLobbyMatchList );
                }

                void OnLobbyMatchList( LobbyMatchList_t pLobbyMatchList, bool bIOFailure )
                {
                    // lobby list has be retrieved from Steam back-end, use results
                }
            }
        */
        // 

        SteamAPICall_t RequestLobbyList(IntPtr _);
        // filters for lobbies
        // this needs to be called before RequestLobbyList() to take effect
        // these are cleared on each call to RequestLobbyList()
        void AddRequestLobbyListStringFilter(string pchKeyToMatch, string pchValueToMatch, ELobbyComparison eComparisonType);
        // numerical comparison
        void AddRequestLobbyListNumericalFilter(string pchKeyToMatch, int nValueToMatch, ELobbyComparison eComparisonType);
        // returns results closest to the specified value. Multiple near filters can be added, with early filters taking precedence
        void AddRequestLobbyListNearValueFilter(string pchKeyToMatch, int nValueToBeCloseTo);
        // returns only lobbies with the specified number of slots available
        void AddRequestLobbyListFilterSlotsAvailable(int nSlotsAvailable);
        // sets the distance for which we should search for lobbies (based on users IP address to location map on the Steam backed)
        void AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter eLobbyDistanceFilter);
        // sets how many results to return, the lower the count the faster it is to download the lobby results & details to the client
        void AddRequestLobbyListResultCountFilter(int cMaxResults);

        void AddRequestLobbyListCompatibleMembersFilter(IntPtr steamIDLobby);

        // returns the CSteamID of a lobby, as retrieved by a RequestLobbyList call
        // should only be called after a LobbyMatchList_t callback is received
        // iLobby is of the range [0, LobbyMatchList_t::m_nLobbiesMatching)
        // the returned CSteamID::IsValid() will be false if iLobby is out of range
        IntPtr GetLobbyByIndex(int iLobby);

        // Create a lobby on the Steam servers.
        // If private, then the lobby will not be returned by any RequestLobbyList() call; the CSteamID
        // of the lobby will need to be communicated via game channels or via InviteUserToLobby()
        // this is an asynchronous request
        // results will be returned by LobbyCreated_t callback and call result; lobby is joined & ready to use at this point
        // a LobbyEnter_t callback will also be received (since the local user is joining their own lobby)

        SteamAPICall_t CreateLobby(ELobbyType eLobbyType, int cMaxMembers);

        // Joins an existing lobby
        // this is an asynchronous request
        // results will be returned by LobbyEnter_t callback & call result, check m_EChatRoomEnterResponse to see if was successful
        // lobby metadata is available to use immediately on this call completing

        SteamAPICall_t JoinLobby(IntPtr steamIDLobby);

        // Leave a lobby; this will take effect immediately on the client side
        // other users in the lobby will be notified by a LobbyChatUpdate_t callback
        void LeaveLobby(IntPtr steamIDLobby);

        // Invite another user to the lobby
        // the target user will receive a LobbyInvite_t callback
        // will return true if the invite is successfully sent, whether or not the target responds
        // returns false if the local user is not connected to the Steam servers
        // if the other user clicks the join link, a GameLobbyJoinRequested_t will be posted if the user is in-game,
        // or if the game isn't running yet the game will be launched with the parameter +connect_lobby <64-bit lobby id>
        bool InviteUserToLobby(IntPtr steamIDLobby, IntPtr steamIDInvitee);

        // Lobby iteration, for viewing details of users in a lobby
        // only accessible if the lobby user is a member of the specified lobby
        // persona information for other lobby members (name, avatar, etc.) will be asynchronously received
        // and accessible via ISteamFriends interface

        // returns the number of users in the specified lobby
        int GetNumLobbyMembers(IntPtr steamIDLobby);
        // returns the CSteamID of a user in the lobby
        // iMember is of range [0,GetNumLobbyMembers())
        // note that the current user must be in a lobby to retrieve CSteamIDs of other users in that lobby
        IntPtr GetLobbyMemberByIndex(IntPtr steamIDLobby, int iMember);

        // Get data associated with this lobby
        // takes a simple key, and returns the string associated with it
        // "" will be returned if no value is set, or if steamIDLobby is invalid
        string GetLobbyData(IntPtr steamIDLobby, string pchKey);
        // Sets a key/value pair in the lobby metadata
        // each user in the lobby will be broadcast this new value, and any new users joining will receive any existing data
        // this can be used to set lobby names, map, etc.
        // to reset a key, just set it to ""
        // other users in the lobby will receive notification of the lobby data change via a LobbyDataUpdate_t callback
        bool SetLobbyData(IntPtr steamIDLobby, string pchKey, string pchValue);

        // returns the number of metadata keys set on the specified lobby
        int GetLobbyDataCount(IntPtr steamIDLobby);

        // returns a lobby metadata key/values pair by index, of range [0, GetLobbyDataCount())
        bool GetLobbyDataByIndex(IntPtr steamIDLobby, int iLobbyData, string pchKey, int cchKeyBufferSize, string pchValue, int cchValueBufferSize);

        // removes a metadata key from the lobby
        bool DeleteLobbyData(IntPtr steamIDLobby, string pchKey);

        // Gets per-user metadata for someone in this lobby
        string GetLobbyMemberData(IntPtr steamIDLobby, IntPtr steamIDUser, string pchKey);
        // Sets per-user metadata (for the local user implicitly)
        void SetLobbyMemberData(IntPtr steamIDLobby, string pchKey, string pchValue);

        // Broadcasts a chat message to the all the users in the lobby
        // users in the lobby (including the local user) will receive a LobbyChatMsg_t callback
        // returns true if the message is successfully sent
        // pvMsgBody can be binary or text data, up to 4k
        // if pvMsgBody is text, cubMsgBody should be strlen( text ) + 1, to include the null terminator
        bool SendLobbyChatMsg(IntPtr steamIDLobby, IntPtr pvMsgBody, int cubMsgBody);
        // Get a chat message as specified in a LobbyChatMsg_t callback
        // iChatID is the LobbyChatMsg_t::m_iChatID value in the callback
        // pSteamIDUser is filled in with the CSteamID of the member
        // pvData is filled in with the message itself
        // return value is the number of bytes written into the buffer
        int GetLobbyChatEntry(IntPtr steamIDLobby, int iChatID, IntPtr pSteamIDUser, IntPtr pvData, int cubData, EChatEntryType peChatEntryType);

        // Refreshes metadata for a lobby you're not necessarily in right now
        // you never do this for lobbies you're a member of, only if your
        // this will send down all the metadata associated with a lobby
        // this is an asynchronous call
        // returns false if the local user is not connected to the Steam servers
        // results will be returned by a LobbyDataUpdate_t callback
        // if the specified lobby doesn't exist, LobbyDataUpdate_t::m_bSuccess will be set to false
        bool RequestLobbyData(IntPtr steamIDLobby);

        // sets the game server associated with the lobby
        // usually at this point, the users will join the specified game server
        // either the IP/Port or the steamID of the game server has to be valid, depending on how you want the clients to be able to connect
        void SetLobbyGameServer(IntPtr steamIDLobby, uint unGameServerIP, uint unGameServerPort, IntPtr steamIDGameServer);
        // returns the details of a game server set in a lobby - returns false if there is no game server set, or that lobby doesn't exist
        bool GetLobbyGameServer(IntPtr steamIDLobby, uint punGameServerIP, uint punGameServerPort, IntPtr psteamIDGameServer);

        // set the limit on the # of users who can join the lobby
        bool SetLobbyMemberLimit(IntPtr steamIDLobby, int cMaxMembers);
        // returns the current limit on the # of users who can join the lobby; returns 0 if no limit is defined
        int GetLobbyMemberLimit(IntPtr steamIDLobby);

        // updates which type of lobby it is
        // only lobbies that are k_ELobbyTypePublic or k_ELobbyTypeInvisible, and are set to joinable, will be returned by RequestLobbyList() calls
        bool SetLobbyType(IntPtr steamIDLobby, ELobbyType eLobbyType);

        // sets whether or not a lobby is joinable - defaults to true for a new lobby
        // if set to false, no user can join, even if they are a friend or have been invited
        bool SetLobbyJoinable(IntPtr steamIDLobby, bool bLobbyJoinable);

        // returns the current lobby owner
        // you must be a member of the lobby to access this
        // there always one lobby owner - if the current owner leaves, another user will become the owner
        // it is possible (bur rare) to join a lobby just as the owner is leaving, thus entering a lobby with self as the owner
        IntPtr GetLobbyOwner(IntPtr steamIDLobby);

        // changes who the lobby owner is
        // you must be the lobby owner for this to succeed, and steamIDNewOwner must be in the lobby
        // after completion, the local user will no longer be the owner
        bool SetLobbyOwner(IntPtr steamIDLobby, IntPtr steamIDNewOwner);

        // link two lobbies for the purposes of checking player compatibility
        // you must be the lobby owner of both lobbies
        bool SetLinkedLobby(IntPtr steamIDLobby, IntPtr steamIDLobbyDependent);
    }
    // lobby search distance. Lobby results are sorted from closest to farthest.
    public enum ELobbyDistanceFilter
    {
        k_ELobbyDistanceFilterClose,        // only lobbies in the same immediate region will be returned
        k_ELobbyDistanceFilterDefault,      // only lobbies in the same region or near by regions
        k_ELobbyDistanceFilterFar,          // for games that don't have many latency requirements, will return lobbies about half-way around the globe
        k_ELobbyDistanceFilterWorldwide,    // no filtering, will match lobbies as far as India to NY (not recommended, expect multiple seconds of latency between the clients)
    };
    // lobby type description
    public enum ELobbyType
    {
        k_ELobbyTypePrivate = 0,        // only way to join the lobby is to invite to someone else
        k_ELobbyTypeFriendsOnly = 1,    // shows for friends or invitees, but not in lobby list
        k_ELobbyTypePublic = 2,         // visible for friends and in lobby list
        k_ELobbyTypeInvisible = 3,      // returned by search, but not visible to other friends 
                                        //    useful if you want a user in two lobbies, for example matching groups together
                                        //	  a user can be in only one regular lobby, and up to two invisible lobbies
        k_ELobbyTypePrivateUnique = 4,  // private, unique and does not delete when empty - only one of these may exist per unique keypair set
                                        // can only create from webapi
    };

    // lobby search filter tools
    public enum ELobbyComparison
    {
        k_ELobbyComparisonEqualToOrLessThan = -2,
        k_ELobbyComparisonLessThan = -1,
        k_ELobbyComparisonEqual = 0,
        k_ELobbyComparisonGreaterThan = 1,
        k_ELobbyComparisonEqualToOrGreaterThan = 2,
        k_ELobbyComparisonNotEqual = 3,
    };
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Types
{
    public enum OverlayType
    {
        UsersList,              // Own type             Show list of users to send game invitation or message
        SteamProfile,           // steamid              Opens the overlay web browser to the specified user or groups profile. 
        Chat,                   // chat                 Opens a chat window to the specified user, or joins the group chat.
        JoinTrade,              // jointrade            Opens a window to a Steam Trading session that was started with the ISteamEconomy / StartTrade Web API.
        Stats,                  // stats                Opens the overlay web browser to the specified user's stats.
        Achievements,           // achievements         Opens the overlay web browser to the specified user's achievements.
        FriendAdd,              // friendadd            Opens the overlay in minimal mode prompting the user to add the target user as a friend.
        FriendRemove,           // friendremove         Opens the overlay in minimal mode prompting the user to remove the target friend.
        FriendRequestAccept,    // friendrequestaccept  Opens the overlay in minimal mode prompting the user to accept an incoming friend invite.
        FriendRequestIgnore,    // friendrequestignore  Opens the overlay in minimal mode prompting the user to ignore an incoming friend invite.
        LobbyInvite,
    }
}

using System;
using SKYNET.Helpers;
using SKYNET.Steamworks.Implementation;

namespace SKYNET.Steamworks.Interfaces
{
    /// <summary>
    /// Legacy ISteamFriends ABI used by the original Left 4 Dead Steamworks SDK.
    /// The interface contains exactly 24 vtable slots and predates the async
    /// SetPersonaName return value and the overlay-to-store flag argument.
    /// </summary>
    [Interface("SteamFriends005")]
    public class SteamFriends005 : ISteamInterface
    {
        public string GetPersonaName(IntPtr _) => SteamFriends.Instance.GetPersonaName();

        public void SetPersonaNameOld(IntPtr _, string pchPersonaName)
        {
            SteamFriends.Instance.SetPersonaName(pchPersonaName);
        }

        public EPersonaState GetPersonaState(IntPtr _) =>
            (EPersonaState)SteamFriends.Instance.GetPersonaState();

        public int GetFriendCount(IntPtr _, int eFriendFlags) =>
            SteamFriends.Instance.GetFriendCount(eFriendFlags);

        public IntPtr GetFriendByIndex(IntPtr _, IntPtr ret, int iFriend, int iFriendFlags) =>
            NativeSteamId.Write(ret, SteamFriends.Instance.GetFriendByIndex(iFriend, iFriendFlags));

        public EFriendRelationship GetFriendRelationship(IntPtr _, ulong steamIDFriend) =>
            (EFriendRelationship)SteamFriends.Instance.GetFriendRelationship(steamIDFriend);

        public EPersonaState GetFriendPersonaState(IntPtr _, ulong steamIDFriend) =>
            (EPersonaState)SteamFriends.Instance.GetFriendPersonaState(steamIDFriend);

        public string GetFriendPersonaName(IntPtr _, ulong steamIDFriend) =>
            SteamFriends.Instance.GetFriendPersonaName(steamIDFriend);

        public int GetFriendAvatar(IntPtr _, ulong steamIDFriend, int eAvatarSize)
        {
            switch (eAvatarSize)
            {
                case 0:
                    return SteamFriends.Instance.GetSmallFriendAvatar(steamIDFriend);
                case 1:
                    return SteamFriends.Instance.GetMediumFriendAvatar(steamIDFriend);
                case 2:
                    return SteamFriends.Instance.GetLargeFriendAvatar(steamIDFriend);
                default:
                    return 0;
            }
        }

        public bool GetFriendGamePlayed(IntPtr _, ulong steamIDFriend, ref FriendGameInfo_t pFriendGameInfo) =>
            SteamFriends.Instance.GetFriendGamePlayed(steamIDFriend, ref pFriendGameInfo);

        public string GetFriendPersonaNameHistory(IntPtr _, ulong steamIDFriend, int iPersonaName) =>
            SteamFriends.Instance.GetFriendPersonaNameHistory(steamIDFriend, iPersonaName);

        public bool HasFriend(IntPtr _, ulong steamIDFriend, int eFriendFlags) =>
            SteamFriends.Instance.HasFriend(steamIDFriend, eFriendFlags);

        public int GetClanCount(IntPtr _) => SteamFriends.Instance.GetClanCount();

        public IntPtr GetClanByIndex(IntPtr _, IntPtr ret, int iClan) =>
            NativeSteamId.Write(ret, SteamFriends.Instance.GetClanByIndex(iClan));

        public string GetClanName(IntPtr _, ulong steamIDClan) =>
            SteamFriends.Instance.GetClanName(steamIDClan);

        public int GetFriendCountFromSource(IntPtr _, ulong steamIDSource) =>
            SteamFriends.Instance.GetFriendCountFromSource(steamIDSource);

        public IntPtr GetFriendFromSourceByIndex(IntPtr _, IntPtr ret, ulong steamIDSource, int iFriend) =>
            NativeSteamId.Write(ret, SteamFriends.Instance.GetFriendFromSourceByIndex(steamIDSource, iFriend));

        public bool IsUserInSource(IntPtr _, ulong steamIDUser, ulong steamIDSource) =>
            SteamFriends.Instance.IsUserInSource(steamIDUser, steamIDSource);

        public void SetInGameVoiceSpeaking(IntPtr _, ulong steamIDUser, bool bSpeaking) =>
            SteamFriends.Instance.SetInGameVoiceSpeaking(steamIDUser, bSpeaking);

        public void ActivateGameOverlay(IntPtr _, string pchDialog) =>
            SteamFriends.Instance.ActivateGameOverlay(pchDialog);

        public void ActivateGameOverlayToUser(IntPtr _, string pchDialog, ulong steamID) =>
            SteamFriends.Instance.ActivateGameOverlayToUser(pchDialog, steamID);

        public void ActivateGameOverlayToWebPage(IntPtr _, string pchURL) =>
            SteamFriends.Instance.ActivateGameOverlayToWebPage(pchURL, 0);

        public void ActivateGameOverlayToStore(IntPtr _, uint nAppID) =>
            SteamFriends.Instance.ActivateGameOverlayToStore(nAppID, 0);

        public void SetPlayedWith(IntPtr _, ulong steamIDUserPlayedWith) =>
            SteamFriends.Instance.SetPlayedWith(steamIDUserPlayedWith);
    }
}

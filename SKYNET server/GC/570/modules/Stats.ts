import { Messages } from "../Messages";

export class Stats {
    msg: Messages;

    constructor() {
        this.msg = new Messages();
    }

    handle(type: int32): boolean {
        if (type == this.msg.ClientToGCLookupAccountName()) return this.lookupAccountName();
        if (type == this.msg.DOTAGetEventPoints()) return this.getEventPoints();
        if (type == this.msg.GCMatchmakingStatsRequest()) return this.matchmakingStats();
        if (type == this.msg.GCGetHeroStandings()) return this.getHeroStandings();
        if (type == this.msg.GCGetHeroStatsHistory()) return this.getHeroStatsHistory();
        if (type == this.msg.DOTAGetPlayerMatchHistory()) return this.getPlayerMatchHistory();
        if (type == this.msg.GCMatchDetailsRequest()) return this.matchDetails();
        if (type == this.msg.GCRetrieveMatchVote()) return this.retrieveMatchVote();
        if (type == this.msg.ClientToGCPlayerStatsRequest()) return this.playerStats();
        if (type == this.msg.GCGetHeroTimedStats()) return this.heroTimedStats();
        if (type == this.msg.HeroGlobalDataRequest()) return this.heroGlobalData();
        if (type == this.msg.ClientToGCTeammateStatsRequest()) return this.teammateStats();
        if (type == this.msg.ClientToGCRankRequest()) return this.rankRequest();
        if (type == this.msg.ClientToGCShowcaseGetUserData()) return this.showcaseGetUserData();
        if (type == this.msg.ClientToGCRequestPlayerRecentAccomplishments()) return this.clientRecentAccomplishments();
        if (type == this.msg.ClientToGCRequestPlayerHeroRecentAccomplishments()) return this.clientHeroRecentAccomplishments();
        if (type == this.msg.ClientToGCHasPlayerVotedForMVP()) return this.hasPlayerVotedForMvp();
        if (type == this.msg.ClientToGCVoteForMVP()) return this.voteForMvp();
        if (type == this.msg.ClientToGCMVPVoteTimeout()) return this.mvpVoteTimeout();
        if (type == this.msg.GCSubmitLobbyMVPVote()) return this.submitLobbyMvpVote();
        if (type == this.msg.SignOutMVPStats()) return this.signOutMvpStats();
        if (type == this.msg.ClientToGCRerollPlayerChallenge()) return this.rerollPlayerChallenge();
        return false;
    }

    lookupAccountName(): boolean { return false; }
    getEventPoints(): boolean { return false; }
    matchmakingStats(): boolean { return false; }
    getHeroStandings(): boolean { return false; }
    getHeroStatsHistory(): boolean { return false; }
    getPlayerMatchHistory(): boolean { return false; }
    matchDetails(): boolean { return false; }
    retrieveMatchVote(): boolean { return false; }
    playerStats(): boolean { return false; }
    heroTimedStats(): boolean { return false; }
    heroGlobalData(): boolean { return false; }
    teammateStats(): boolean { return false; }
    rankRequest(): boolean { return false; }
    showcaseGetUserData(): boolean { return false; }
    clientRecentAccomplishments(): boolean { return false; }
    clientHeroRecentAccomplishments(): boolean { return false; }
    hasPlayerVotedForMvp(): boolean { return false; }
    voteForMvp(): boolean { return false; }
    mvpVoteTimeout(): boolean { return false; }
    submitLobbyMvpVote(): boolean { return false; }
    signOutMvpStats(): boolean { return false; }
    rerollPlayerChallenge(): boolean { return false; }
}

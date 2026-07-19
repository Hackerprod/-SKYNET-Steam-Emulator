import { gc } from "../framework/gc";
import { Msg } from "../generated/dota";

export function registerStats(): void {
    const stats = new Stats();
    stats.register();
}

export class Stats {
    register(): void {
        gc.onMessage(Msg.ClientToGCLookupAccountName, () => this.lookupAccountName());
        gc.onMessage(Msg.DOTAGetEventPoints, () => this.getEventPoints());
        gc.onMessage(Msg.GCMatchmakingStatsRequest, () => this.matchmakingStats());
        gc.onMessage(Msg.GCGetHeroStandings, () => this.getHeroStandings());
        gc.onMessage(Msg.GCGetHeroStatsHistory, () => this.getHeroStatsHistory());
        gc.onMessage(Msg.DOTAGetPlayerMatchHistory, () => this.getPlayerMatchHistory());
        gc.onMessage(Msg.GCMatchDetailsRequest, () => this.matchDetails());
        gc.onMessage(Msg.ClientToGCPlayerStatsRequest, () => this.playerStats());
        gc.onMessage(Msg.HeroGlobalDataRequest, () => this.heroGlobalData());
        gc.onMessage(Msg.ClientToGCTeammateStatsRequest, () => this.teammateStats());
        gc.onMessage(Msg.ClientToGCRankRequest, () => this.rankRequest());
        gc.onMessage(Msg.ClientToGCShowcaseGetUserData, () => this.showcaseGetUserData());
        gc.onMessage(Msg.ClientToGCRequestPlayerRecentAccomplishments, () => this.clientRecentAccomplishments());
        gc.onMessage(Msg.ClientToGCRequestPlayerHeroRecentAccomplishments, () =>
            this.clientHeroRecentAccomplishments()
        );
        gc.onMessage(Msg.ClientToGCHasPlayerVotedForMVP, () => this.hasPlayerVotedForMvp());
        gc.onMessage(Msg.ClientToGCVoteForMVP, () => this.voteForMvp());
        gc.onMessage(Msg.ClientToGCMVPVoteTimeout, () => this.mvpVoteTimeout());
        gc.onMessage(Msg.GCSubmitLobbyMVPVote, () => this.submitLobbyMvpVote());
        gc.onMessage(Msg.SignOutMVPStats, () => this.signOutMvpStats());
        gc.onMessage(Msg.ClientToGCRerollPlayerChallenge, () => this.rerollPlayerChallenge());
    }

    lookupAccountName(): boolean {
        return false;
    }
    getEventPoints(): boolean {
        return false;
    }
    matchmakingStats(): boolean {
        return false;
    }
    getHeroStandings(): boolean {
        return false;
    }
    getHeroStatsHistory(): boolean {
        return false;
    }
    getPlayerMatchHistory(): boolean {
        return false;
    }
    matchDetails(): boolean {
        return false;
    }
    retrieveMatchVote(): boolean {
        return false;
    }
    playerStats(): boolean {
        return false;
    }
    heroTimedStats(): boolean {
        return false;
    }
    heroGlobalData(): boolean {
        return false;
    }
    teammateStats(): boolean {
        return false;
    }
    rankRequest(): boolean {
        return false;
    }
    showcaseGetUserData(): boolean {
        return false;
    }
    clientRecentAccomplishments(): boolean {
        return false;
    }
    clientHeroRecentAccomplishments(): boolean {
        return false;
    }
    hasPlayerVotedForMvp(): boolean {
        return false;
    }
    voteForMvp(): boolean {
        return false;
    }
    mvpVoteTimeout(): boolean {
        return false;
    }
    submitLobbyMvpVote(): boolean {
        return false;
    }
    signOutMvpStats(): boolean {
        return false;
    }
    rerollPlayerChallenge(): boolean {
        return false;
    }
}

import { gc } from "../framework/gc";
import { Msg } from "../generated/dota";

export function registerStats(): void {
    const stats = new Stats();
    stats.register();
}

export class Stats {
    register(): void {
        gc.onMessage(Msg.ClientToGCLookupAccountName as number, () => this.lookupAccountName());
        gc.onMessage(Msg.DOTAGetEventPoints as number, () => this.getEventPoints());
        gc.onMessage(Msg.GCMatchmakingStatsRequest as number, () => this.matchmakingStats());
        gc.onMessage(Msg.GCGetHeroStandings as number, () => this.getHeroStandings());
        gc.onMessage(Msg.GCGetHeroStatsHistory as number, () => this.getHeroStatsHistory());
        gc.onMessage(Msg.DOTAGetPlayerMatchHistory as number, () => this.getPlayerMatchHistory());
        gc.onMessage(Msg.GCMatchDetailsRequest as number, () => this.matchDetails());
        gc.onMessage(Msg.ClientToGCPlayerStatsRequest as number, () => this.playerStats());
        gc.onMessage(Msg.HeroGlobalDataRequest as number, () => this.heroGlobalData());
        gc.onMessage(Msg.ClientToGCTeammateStatsRequest as number, () => this.teammateStats());
        gc.onMessage(Msg.ClientToGCRankRequest as number, () => this.rankRequest());
        gc.onMessage(Msg.ClientToGCShowcaseGetUserData as number, () => this.showcaseGetUserData());
        gc.onMessage(Msg.ClientToGCRequestPlayerRecentAccomplishments as number, () =>
            this.clientRecentAccomplishments()
        );
        gc.onMessage(Msg.ClientToGCRequestPlayerHeroRecentAccomplishments as number, () =>
            this.clientHeroRecentAccomplishments()
        );
        gc.onMessage(Msg.ClientToGCHasPlayerVotedForMVP as number, () => this.hasPlayerVotedForMvp());
        gc.onMessage(Msg.ClientToGCVoteForMVP as number, () => this.voteForMvp());
        gc.onMessage(Msg.ClientToGCMVPVoteTimeout as number, () => this.mvpVoteTimeout());
        gc.onMessage(Msg.GCSubmitLobbyMVPVote as number, () => this.submitLobbyMvpVote());
        gc.onMessage(Msg.SignOutMVPStats as number, () => this.signOutMvpStats());
        gc.onMessage(Msg.ClientToGCRerollPlayerChallenge as number, () => this.rerollPlayerChallenge());
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

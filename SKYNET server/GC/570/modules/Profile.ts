import { gc } from "../framework/gc";
import { Msg } from "../generated/dota";

export function registerProfile(): void {
    const profile = new Profile();
    profile.register();
}

export class Profile {
    register(): void {
        gc.onMessage(Msg.ClientToGCGetProfileCard, () => this.getProfileCard());
        gc.onMessage(Msg.ClientToGCGetProfileCardStats, () => this.getProfileCardStats());
        gc.onMessage(Msg.ClientToGCSetProfileCardSlots, () => this.setProfileCardSlots());
        gc.onMessage(Msg.ClientToGCGetProfileTickets, () => this.getProfileTickets());
        gc.onMessage(Msg.ClientToGCGetQuestProgress, () => this.getQuestProgress());
        gc.onMessage(Msg.ClientToGCLatestConductScorecardRequest, () => this.latestConductScorecard());
        gc.onMessage(Msg.ClientToGCMyTeamInfoRequest, () => this.myTeamInfo());
        gc.onMessage(Msg.DOTAGetPeriodicResource, () => this.getPeriodicResource());
        gc.onMessage(Msg.ProfileRequest, () => this.profileRequest());
        gc.onMessage(Msg.ProfileUpdate, () => this.profileUpdate());
        gc.onMessage(Msg.ClientToGCGetTrophyList, () => this.getTrophyList());
        gc.onMessage(Msg.ClientToGCGetAllHeroOrder, () => this.getAllHeroOrder());
        gc.onMessage(Msg.ClientToGCGetAllHeroProgress, () => this.getAllHeroProgress());
        gc.onMessage(Msg.ClientToGCGetBattleReportInfo, () => this.getBattleReportInfo());
        gc.onMessage(Msg.ClientToGCGetHeroStickers, () => this.getHeroStickers());
        gc.onMessage(Msg.ClientToGCOverworldGetUserData, () => this.overworldGetUserData());
        gc.onMessage(Msg.ClientToGCMonsterHunterGetUserData, () => this.monsterHunterGetUserData());
    }

    getProfileCard(): boolean {
        return false;
    }
    getProfileCardStats(): boolean {
        return false;
    }
    setProfileCardSlots(): boolean {
        return false;
    }
    getProfileTickets(): boolean {
        return false;
    }
    getQuestProgress(): boolean {
        return false;
    }
    latestConductScorecard(): boolean {
        return false;
    }
    myTeamInfo(): boolean {
        return false;
    }
    getPeriodicResource(): boolean {
        return false;
    }
    profileRequest(): boolean {
        return false;
    }
    profileUpdate(): boolean {
        return false;
    }
    getTrophyList(): boolean {
        return false;
    }
    getAllHeroOrder(): boolean {
        return false;
    }
    getAllHeroProgress(): boolean {
        return false;
    }
    getBattleReportInfo(): boolean {
        return false;
    }
    getHeroStickers(): boolean {
        return false;
    }
    overworldGetUserData(): boolean {
        return false;
    }
    monsterHunterGetUserData(): boolean {
        return false;
    }
}

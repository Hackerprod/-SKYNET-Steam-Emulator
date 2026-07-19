import { gc } from "../framework/gc";
import { Msg } from "../generated/dota";

export function registerProfile(): void {
    const profile = new Profile();
    profile.register();
}

export class Profile {
    register(): void {
        gc.onMessage(Msg.ClientToGCGetProfileCard as number, () => this.getProfileCard());
        gc.onMessage(Msg.ClientToGCGetProfileCardStats as number, () => this.getProfileCardStats());
        gc.onMessage(Msg.ClientToGCSetProfileCardSlots as number, () => this.setProfileCardSlots());
        gc.onMessage(Msg.ClientToGCGetProfileTickets as number, () => this.getProfileTickets());
        gc.onMessage(Msg.ClientToGCGetQuestProgress as number, () => this.getQuestProgress());
        gc.onMessage(Msg.ClientToGCLatestConductScorecardRequest as number, () => this.latestConductScorecard());
        gc.onMessage(Msg.ClientToGCMyTeamInfoRequest as number, () => this.myTeamInfo());
        gc.onMessage(Msg.DOTAGetPeriodicResource as number, () => this.getPeriodicResource());
        gc.onMessage(Msg.ProfileRequest as number, () => this.profileRequest());
        gc.onMessage(Msg.ProfileUpdate as number, () => this.profileUpdate());
        gc.onMessage(Msg.ClientToGCGetTrophyList as number, () => this.getTrophyList());
        gc.onMessage(Msg.ClientToGCGetAllHeroOrder as number, () => this.getAllHeroOrder());
        gc.onMessage(Msg.ClientToGCGetAllHeroProgress as number, () => this.getAllHeroProgress());
        gc.onMessage(Msg.ClientToGCGetBattleReportInfo as number, () => this.getBattleReportInfo());
        gc.onMessage(Msg.ClientToGCGetHeroStickers as number, () => this.getHeroStickers());
        gc.onMessage(Msg.ClientToGCOverworldGetUserData as number, () => this.overworldGetUserData());
        gc.onMessage(Msg.ClientToGCMonsterHunterGetUserData as number, () => this.monsterHunterGetUserData());
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

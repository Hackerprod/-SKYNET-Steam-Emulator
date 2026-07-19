import { Messages } from "../Messages";

export class Profile {
    msg: Messages;

    constructor() {
        this.msg = new Messages();
    }

    handle(type: int32): boolean {
        if (type == this.msg.ClientToGCGetProfileCard()) return this.getProfileCard();
        if (type == this.msg.ClientToGCGetProfileCardStats()) return this.getProfileCardStats();
        if (type == this.msg.ClientToGCSetProfileCardSlots()) return this.setProfileCardSlots();
        if (type == this.msg.ClientToGCGetProfileTickets()) return this.getProfileTickets();
        if (type == this.msg.ClientToGCGetQuestProgress()) return this.getQuestProgress();
        if (type == this.msg.ClientToGCLatestConductScorecardRequest()) return this.latestConductScorecard();
        if (type == this.msg.ClientToGCMyTeamInfoRequest()) return this.myTeamInfo();
        if (type == this.msg.DOTAGetPeriodicResource()) return this.getPeriodicResource();
        if (type == this.msg.ProfileRequest()) return this.profileRequest();
        if (type == this.msg.ProfileUpdate()) return this.profileUpdate();
        if (type == this.msg.ClientToGCGetTrophyList()) return this.getTrophyList();
        if (type == this.msg.ClientToGCGetAllHeroOrder()) return this.getAllHeroOrder();
        if (type == this.msg.ClientToGCGetAllHeroProgress()) return this.getAllHeroProgress();
        if (type == this.msg.ClientToGCGetBattleReportInfo()) return this.getBattleReportInfo();
        if (type == this.msg.ClientToGCGetHeroStickers()) return this.getHeroStickers();
        if (type == this.msg.ClientToGCOverworldGetUserData()) return this.overworldGetUserData();
        if (type == this.msg.ClientToGCMonsterHunterGetUserData()) return this.monsterHunterGetUserData();
        return false;
    }

    getProfileCard(): boolean { return false; }
    getProfileCardStats(): boolean { return false; }
    setProfileCardSlots(): boolean { return false; }
    getProfileTickets(): boolean { return false; }
    getQuestProgress(): boolean { return false; }
    latestConductScorecard(): boolean { return false; }
    myTeamInfo(): boolean { return false; }
    getPeriodicResource(): boolean { return false; }
    profileRequest(): boolean { return false; }
    profileUpdate(): boolean { return false; }
    getTrophyList(): boolean { return false; }
    getAllHeroOrder(): boolean { return false; }
    getAllHeroProgress(): boolean { return false; }
    getBattleReportInfo(): boolean { return false; }
    getHeroStickers(): boolean { return false; }
    overworldGetUserData(): boolean { return false; }
    monsterHunterGetUserData(): boolean { return false; }
}

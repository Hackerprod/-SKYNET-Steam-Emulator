import { Messages } from "../Messages";

export class Party {
    msg: Messages;

    constructor() {
        this.msg = new Messages();
    }

    handle(type: int32): boolean {
        if (type == this.msg.GCInviteToParty()) return this.inviteToParty();
        if (type == this.msg.GCPartyInviteResponse()) return this.inviteResponse();
        if (type == this.msg.ClientToGCPingData()) return this.pingData();
        if (type == this.msg.ClientToGCSetPartyLeader()) return this.setLeader();
        if (type == this.msg.GCLeaveParty()) return this.leave();
        if (type == this.msg.GCKickFromParty()) return this.kick();
        if (type == this.msg.GCPartyMemberSetCoach()) return this.setCoach();
        if (type == this.msg.PartyReadyCheckRequest()) return this.readyCheckRequest();
        if (type == this.msg.PartyReadyCheckAcknowledge()) return this.readyCheckAcknowledge();
        return false;
    }

    inviteToParty(): boolean { return false; }
    inviteResponse(): boolean { return false; }
    pingData(): boolean { return false; }
    setLeader(): boolean { return false; }
    leave(): boolean { return false; }
    kick(): boolean { return false; }
    setCoach(): boolean { return false; }
    readyCheckRequest(): boolean { return false; }
    readyCheckAcknowledge(): boolean { return false; }
    tick(): void { }
}

import { gc } from "../framework/gc";
import { Msg } from "../generated/dota";

export function registerParty(): Party {
    const party = new Party();
    party.register();
    return party;
}

export class Party {
    register(): void {
        gc.onMessage(Msg.GCInviteToParty, () => this.inviteToParty());
        gc.onMessage(Msg.GCPartyInviteResponse, () => this.inviteResponse());
        gc.onMessage(Msg.ClientToGCPingData, () => this.pingData());
        gc.onMessage(Msg.ClientToGCSetPartyLeader, () => this.setLeader());
        gc.onMessage(Msg.GCLeaveParty, () => this.leave());
        gc.onMessage(Msg.GCKickFromParty, () => this.kick());
        gc.onMessage(Msg.GCPartyMemberSetCoach, () => this.setCoach());
        gc.onMessage(Msg.PartyReadyCheckRequest, () => this.readyCheckRequest());
        gc.onMessage(Msg.PartyReadyCheckAcknowledge, () => this.readyCheckAcknowledge());
    }

    inviteToParty(): boolean {
        return false;
    }
    inviteResponse(): boolean {
        return false;
    }
    pingData(): boolean {
        return false;
    }
    setLeader(): boolean {
        return false;
    }
    leave(): boolean {
        return false;
    }
    kick(): boolean {
        return false;
    }
    setCoach(): boolean {
        return false;
    }
    readyCheckRequest(): boolean {
        return false;
    }
    readyCheckAcknowledge(): boolean {
        return false;
    }
    tick(): void {}
}

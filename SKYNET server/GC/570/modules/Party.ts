import { gc } from "../framework/gc";
import { Msg } from "../generated/dota";

export function registerParty(): Party {
    const party = new Party();
    party.register();
    return party;
}

export class Party {
    register(): void {
        gc.onMessage(Msg.GCInviteToParty as number, () => this.inviteToParty());
        gc.onMessage(Msg.GCPartyInviteResponse as number, () => this.inviteResponse());
        gc.onMessage(Msg.ClientToGCPingData as number, () => this.pingData());
        gc.onMessage(Msg.ClientToGCSetPartyLeader as number, () => this.setLeader());
        gc.onMessage(Msg.GCLeaveParty as number, () => this.leave());
        gc.onMessage(Msg.GCKickFromParty as number, () => this.kick());
        gc.onMessage(Msg.GCPartyMemberSetCoach as number, () => this.setCoach());
        gc.onMessage(Msg.PartyReadyCheckRequest as number, () => this.readyCheckRequest());
        gc.onMessage(Msg.PartyReadyCheckAcknowledge as number, () => this.readyCheckAcknowledge());
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

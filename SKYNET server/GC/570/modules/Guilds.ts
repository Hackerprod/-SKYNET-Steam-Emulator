import { Messages } from "../Messages";

export class Guilds {
    msg: Messages;

    constructor() {
        this.msg = new Messages();
    }

    handle(type: int32): boolean {
        if (type == this.msg.ClientToGCRequestGuildData()) return this.requestGuildData();
        if (type == this.msg.ClientToGCRequestGuildMembership()) return this.requestGuildMembership();
        if (type == this.msg.ClientToGCUnknown8716()) return this.unknown8716();
        if (type == this.msg.ClientToGCRequestAccountGuildPersonaInfo()) return this.requestAccountGuildPersonaInfo();
        if (type == this.msg.ClientToGCRequestAccountGuildPersonaInfoBatch()) return this.requestAccountGuildPersonaInfoBatch();
        return false;
    }

    requestGuildData(): boolean { return false; }
    requestGuildMembership(): boolean { return false; }
    unknown8716(): boolean { return false; }
    requestAccountGuildPersonaInfo(): boolean { return false; }
    requestAccountGuildPersonaInfoBatch(): boolean { return false; }
}

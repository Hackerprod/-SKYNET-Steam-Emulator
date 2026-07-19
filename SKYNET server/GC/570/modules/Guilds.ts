import { gc } from "../framework/gc";
import { Msg } from "../generated/dota";

export function registerGuilds(): void {
    const guilds = new Guilds();
    guilds.register();
}

export class Guilds {
    register(): void {
        gc.onMessage(Msg.ClientToGCRequestGuildData, () => this.requestGuildData());
        gc.onMessage(Msg.ClientToGCRequestGuildMembership, () => this.requestGuildMembership());
        gc.onMessage(Msg.ClientToGCRequestReporterUpdates, () => this.unknown8716());
        gc.onMessage(Msg.ClientToGCRequestAccountGuildPersonaInfo, () => this.requestAccountGuildPersonaInfo());
        gc.onMessage(Msg.ClientToGCRequestAccountGuildPersonaInfoBatch, () =>
            this.requestAccountGuildPersonaInfoBatch()
        );
    }

    requestGuildData(): boolean {
        return false;
    }
    requestGuildMembership(): boolean {
        return false;
    }
    unknown8716(): boolean {
        return false;
    }
    requestAccountGuildPersonaInfo(): boolean {
        return false;
    }
    requestAccountGuildPersonaInfoBatch(): boolean {
        return false;
    }
}

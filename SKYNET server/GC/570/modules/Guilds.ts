import { gc } from "../framework/gc";
import { Msg } from "../generated/dota";

export function registerGuilds(): void {
    const guilds = new Guilds();
    guilds.register();
}

export class Guilds {
    register(): void {
        gc.onMessage(Msg.ClientToGCRequestGuildData as number, () => this.requestGuildData());
        gc.onMessage(Msg.ClientToGCRequestGuildMembership as number, () => this.requestGuildMembership());
        gc.onMessage(Msg.ClientToGCRequestReporterUpdates as number, () => this.unknown8716());
        gc.onMessage(Msg.ClientToGCRequestAccountGuildPersonaInfo as number, () =>
            this.requestAccountGuildPersonaInfo()
        );
        gc.onMessage(Msg.ClientToGCRequestAccountGuildPersonaInfoBatch as number, () =>
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

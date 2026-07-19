import { gc } from "./framework/gc";
import { registerAuth } from "./modules/Auth";
import { registerChat } from "./modules/Chat";
import { registerCoaching } from "./modules/Coaching";
import { registerEconomy } from "./modules/Economy";
import { registerGuilds } from "./modules/Guilds";
import { registerItems } from "./modules/Items";
import { registerLobby } from "./modules/Lobby";
import { registerMatch } from "./modules/Match";
import { registerParty } from "./modules/Party";
import { registerProfile } from "./modules/Profile";
import { registerSocial } from "./modules/Social";
import { registerStats } from "./modules/Stats";

const lobby = registerLobby();
const party = registerParty();

registerAuth();
registerItems();
registerStats();
registerProfile();
registerEconomy();
registerMatch();
registerGuilds();
registerCoaching();
registerSocial();
registerChat();

export async function handle(): Promise<boolean> {
    return await gc.dispatch();
}

export function tick(): void {
    lobby.tick();
    party.tick();
}

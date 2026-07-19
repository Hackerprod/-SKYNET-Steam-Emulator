import { Auth } from "./modules/Auth";
import { Coaching } from "./modules/Coaching";
import { Economy } from "./modules/Economy";
import { Guilds } from "./modules/Guilds";
import { Items } from "./modules/Items";
import { Lobby } from "./modules/Lobby";
import { Match } from "./modules/Match";
import { Party } from "./modules/Party";
import { Profile } from "./modules/Profile";
import { Social } from "./modules/Social";
import { Stats } from "./modules/Stats";

const auth = new Auth();
const lobby = new Lobby();
const party = new Party();
const items = new Items();
const stats = new Stats();
const profile = new Profile();
const economy = new Economy();
const match = new Match();
const guilds = new Guilds();
const coaching = new Coaching();
const social = new Social();

function handle(): boolean {
    const type = messageType();

    if (auth.handle(type)) return true;
    if (lobby.handle(type)) return true;
    if (party.handle(type)) return true;
    if (items.handle(type)) return true;
    if (stats.handle(type)) return true;
    if (profile.handle(type)) return true;
    if (economy.handle(type)) return true;
    if (match.handle(type)) return true;
    if (guilds.handle(type)) return true;
    if (coaching.handle(type)) return true;
    if (social.handle(type)) return true;

    return false;
}

function tick(): void {
    lobby.tick();
    party.tick();
}

import { gc, RawMessageContext } from "../framework/gc";
import { GCConnectionStatus, Msg, Proto } from "../generated/dota";
import { buildDotaGameAccount, buildEconSoCacheSubscribed } from "./InventorySos";
import { emitCurrentLobby } from "./Lobby";
import { emitCurrentParty } from "./Party";

const Welcome = {
    Version: 20,
    OwnerSteamId: 1,
    DotaServiceGame: 0,
    DotaServiceEcon: 1,
    TypeDotaAccount: 2002,
    TypeDotaPlus: 2012
} as const;

export function registerAuth(): void {
    const auth = new Auth();
    auth.register();
}

export class Auth {
    register(): void {
        gc.onMessage(Msg.GCClientHello, (ctx) => {
            this.handleClientHello(ctx);
        });
    }

    handleClientHello(ctx: RawMessageContext): void {
        const version = ctx.services.build.clientVersion();
        let sessionNeed: number = 0;

        // ClientHello opens the GC session in three packets: first we tell the
        // client it is out of the logon queue, then we send the welcome payload
        // with the SO caches owned by this SteamID, and finally we mark the
        // session as usable. Dota uses that sequence to unlock GC-backed UI and
        // to bind account/econ state to the current player.
        ctx.send(Msg.GCClientConnectionStatus, Proto.CMsgConnectionStatus, {
            status: GCConnectionStatus.NoSessionInLogonQueue,
            clientSessionNeed: sessionNeed
        });

        const gameAccount = ctx.encode(Proto.CSODOTAGameAccountClient, buildDotaGameAccount(ctx));

        const plusAccount = ctx.encode(Proto.CSODOTAGameAccountPlus, {
            accountId: ctx.accountId
        });

        const dotaWelcome = ctx.encode(Proto.CMsgDOTAWelcome, {
            allow3rdPartyMatchHistory: true,
            gcSocacheFileVersion: Welcome.Version,
            activeEvent: 0,
            activeEventForDisplay: 0
        });

        // The welcome message carries gameData plus the initial subscribed
        // object caches. The game-service cache contains account level state;
        // the econ cache contains inventory/equipped objects and global effects.
        // Both caches must keep their owner SOID and service fields populated so
        // the client accepts them as authoritative for this player.
        ctx.send(Msg.GCClientWelcome, Proto.CMsgClientWelcome, {
            version: version,
            gameData: dotaWelcome,
            outofdateSubscribedCaches: [
                {
                    objects: [
                        {
                            typeId: Welcome.TypeDotaAccount,
                            objectData: [gameAccount]
                        },
                        {
                            typeId: Welcome.TypeDotaPlus,
                            objectData: [plusAccount]
                        }
                    ],
                    version: 20n,
                    ownerSoid: {
                        type: Welcome.OwnerSteamId,
                        id: ctx.steamId
                    },
                    serviceId: Welcome.DotaServiceGame,
                    serviceList: [Welcome.DotaServiceEcon],
                    syncVersion: 1n
                },
                buildEconSoCacheSubscribed(ctx)
            ],
            gcSocacheFileVersion: Welcome.Version,
            rtime32GcWelcomeTimestamp: ctx.clock.now(),
            currency: 0
        });

        ctx.send(Msg.GCClientConnectionStatus, Proto.CMsgConnectionStatus, {
            status: GCConnectionStatus.HaveSession,
            clientSessionNeed: sessionNeed
        });

        emitCurrentParty(ctx);
        emitCurrentLobby(ctx);
    }
}

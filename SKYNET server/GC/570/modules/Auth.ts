import { gc, HandlerContext } from "../framework/gc";
import { CMsgClientHello, CMsgClientWelcome, GCConnectionStatus, Msg, Proto, Routes } from "../generated/dota";
import { buildEconSoCacheSubscribed } from "./InventorySos";
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
        gc.on(Routes.ClientHello, (ctx) => {
            this.handleClientHello(ctx);
        });
    }

    handleClientHello(ctx: HandlerContext<CMsgClientHello, CMsgClientWelcome>): void {
        let version: number = Welcome.Version;
        if (ctx.request.version) {
            version = ctx.request.version as number;
        }

        let sessionNeed: number = 0;
        if (ctx.request.clientSessionNeed) {
            sessionNeed = ctx.request.clientSessionNeed as number;
        }

        // ClientHello opens the GC session in three packets: first we tell the
        // client it is out of the logon queue, then we send the welcome payload
        // with the SO caches owned by this SteamID, and finally we mark the
        // session as usable. Dota uses that sequence to unlock GC-backed UI and
        // to bind account/econ state to the current player.
        ctx.send(Msg.GCClientConnectionStatus, Proto.CMsgConnectionStatus, {
            status: GCConnectionStatus.NoSessionInLogonQueue,
            clientSessionNeed: sessionNeed
        });

        const gameAccount = ctx.encode(Proto.CSODOTAGameAccountClient, {
            accountId: ctx.accountId
        });

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

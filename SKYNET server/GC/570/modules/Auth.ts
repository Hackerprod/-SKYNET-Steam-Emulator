import { gc, HandlerContext } from "../framework/gc";
import { CMsgClientHello, CMsgClientWelcome, GCConnectionStatus, Msg, Proto, Routes } from "../generated/dota";

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

        ctx.send(Msg.GCClientConnectionStatus as number, Proto.CMsgConnectionStatus, {
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

        ctx.send(Msg.GCClientWelcome as number, Proto.CMsgClientWelcome, {
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
                }
            ],
            gcSocacheFileVersion: Welcome.Version,
            rtime32GcWelcomeTimestamp: ctx.clock.now(),
            currency: 0
        });

        ctx.send(Msg.GCClientConnectionStatus as number, Proto.CMsgConnectionStatus, {
            status: GCConnectionStatus.HaveSession,
            clientSessionNeed: sessionNeed
        });
    }
}

import { Messages } from "../Messages";
import { gc } from "../framework/gc";
import { ConnectionStatus, Msg, Proto, Routes, Welcome } from "../generated/dota";

export class Auth {
    msg: Messages;

    constructor() {
        this.msg = new Messages();
    }

    handle(type: int32): boolean {
        if (type == this.msg.GCClientHello()) return this.handleClientHello();
        return false;
    }

    handleClientHello(): boolean {
        return gc.on(Routes.ClientHello, ctx => {
            let version: int32 = Welcome.Version;
            if (ctx.request.version) {
                version = ctx.request.version as int32;
            }

            let sessionNeed: int32 = 0;
            if (ctx.request.clientSessionNeed) {
                sessionNeed = ctx.request.clientSessionNeed as int32;
            }

            ctx.send(Msg.GCClientConnectionStatus as int32, Proto.CMsgConnectionStatus, {
                status: ConnectionStatus.NoSessionInLogonQueue,
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

            ctx.send(Msg.GCClientWelcome as int32, Proto.CMsgClientWelcome, {
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
                        version: Welcome.Version,
                        ownerSoid: {
                            type: Welcome.OwnerSteamId,
                            id: ctx.steamId
                        },
                        serviceId: Welcome.DotaServiceGame,
                        serviceList: [Welcome.DotaServiceEcon],
                        syncVersion: 1
                    }
                ],
                gcSocacheFileVersion: Welcome.Version,
                rtime32GcWelcomeTimestamp: now() as int32,
                currency: 0
            });

            ctx.send(Msg.GCClientConnectionStatus as int32, Proto.CMsgConnectionStatus, {
                status: ConnectionStatus.HaveSession,
                clientSessionNeed: sessionNeed
            });
        });
    }
}

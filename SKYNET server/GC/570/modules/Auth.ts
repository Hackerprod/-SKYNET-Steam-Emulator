import { Messages } from "../Messages";

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
        const GC_STATUS_HAVE_SESSION = 0;
        const GC_STATUS_NO_SESSION_IN_LOGON_QUEUE = 3;
        const WELCOME_VERSION = 20;
        const SO_OWNER_STEAM_ID = 1;
        const DOTA_SERVICE_GAME = 0;
        const DOTA_SERVICE_ECON = 1;
        const SO_TYPE_DOTA_ACCOUNT = 2002;
        const SO_TYPE_DOTA_PLUS = 2012;

        const hello = decode("CMsgClientHello", body());
        const version = hello.version == 0 ? WELCOME_VERSION : hello.version;
        const sessionNeed = hello.clientSessionNeed;

        send(this.msg.GCClientConnectionStatus(), encode("CMsgConnectionStatus", {
            status: GC_STATUS_NO_SESSION_IN_LOGON_QUEUE,
            clientSessionNeed: sessionNeed
        }), true);

        const gameAccount = encode("CSODOTAGameAccountClient", {
            accountId: accountId()
        });

        const plusAccount = encode("CSODOTAGameAccountPlus", {
            accountId: accountId()
        });

        const gameCache = {
            objects: [
                {
                    typeId: SO_TYPE_DOTA_ACCOUNT,
                    objectData: [gameAccount]
                },
                {
                    typeId: SO_TYPE_DOTA_PLUS,
                    objectData: [plusAccount]
                }
            ],
            version: WELCOME_VERSION,
            ownerSoid: {
                type: SO_OWNER_STEAM_ID,
                id: steamId()
            },
            serviceId: DOTA_SERVICE_GAME,
            serviceList: [DOTA_SERVICE_ECON],
            syncVersion: 1
        };

        const dotaWelcome = encode("CMsgDOTAWelcome", {
            allow3rdPartyMatchHistory: true,
            gcSocacheFileVersion: WELCOME_VERSION,
            activeEvent: 0,
            activeEventForDisplay: 0
        });

        send(this.msg.GCClientWelcome(), encode("CMsgClientWelcome", {
            version: version,
            gameData: dotaWelcome,
            outofdateSubscribedCaches: [gameCache],
            gcSocacheFileVersion: WELCOME_VERSION,
            rtime32GcWelcomeTimestamp: now(),
            currency: 0
        }), true);

        send(this.msg.GCClientConnectionStatus(), encode("CMsgConnectionStatus", {
            status: GC_STATUS_HAVE_SESSION,
            clientSessionNeed: sessionNeed
        }), true);

        return true;
    }
}

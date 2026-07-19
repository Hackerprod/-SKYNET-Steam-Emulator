import { GcRoute, ProtoDescriptor } from "../generated/dota";

export type HandlerResult = void | boolean | Promise<void | boolean>;
export type RouteHandler<TRequest, TResponse> = (ctx: HandlerContext<TRequest, TResponse>) => HandlerResult;
export type RawMessageHandler = (ctx: RawMessageContext) => HandlerResult;

export interface HandlerContext<TRequest, TResponse> {
    readonly route: GcRoute<TRequest, TResponse>;
    readonly request: TRequest;
    readonly steamId: bigint;
    readonly accountId: number;
    readonly personaName: string;
    readonly clock: Clock;
    readonly logger: Logger;
    reply(response: TResponse): void;
    send<TMessage>(messageType: number, proto: ProtoDescriptor<TMessage>, message: TMessage): void;
    encode<TMessage>(proto: ProtoDescriptor<TMessage>, message: TMessage): Uint8Array;
}

export interface RawMessageContext {
    readonly messageType: number;
    readonly payload: Uint8Array;
    readonly steamId: bigint;
    readonly accountId: number;
    readonly personaName: string;
    readonly clock: Clock;
    readonly logger: Logger;
    send<TMessage>(messageType: number, proto: ProtoDescriptor<TMessage>, message: TMessage): void;
    encode<TMessage>(proto: ProtoDescriptor<TMessage>, message: TMessage): Uint8Array;
    decode<TMessage>(proto: ProtoDescriptor<TMessage>): TMessage;
}

export interface Clock {
    now(): number;
}

export interface Logger {
    info(message: string): void;
}

function currentSteamId(): bigint {
    return steamId();
}

function currentAccountId(): number {
    return accountId();
}

function currentPersonaName(): string {
    return personaName();
}

class GcClock {
    now(): number {
        return now() as number;
    }
}

class GcLogger {
    info(message: string): void {
        log(message);
    }
}

class GcHandlerContext<TRequest, TResponse> implements HandlerContext<TRequest, TResponse> {
    route: GcRoute<TRequest, TResponse>;
    request: TRequest;
    steamId: bigint;
    accountId: number;
    personaName: string;
    clock: Clock;
    logger: Logger;

    constructor(route: GcRoute<TRequest, TResponse>) {
        this.route = route;
        this.request = decode(route.request.name, body()) as TRequest;
        this.steamId = currentSteamId();
        this.accountId = currentAccountId();
        this.personaName = currentPersonaName();
        this.clock = new GcClock();
        this.logger = new GcLogger();
    }

    reply(response: TResponse): void {
        send(this.route.responseId, encode(this.route.response.name, response), true);
    }

    send<TMessage>(messageType: number, proto: ProtoDescriptor<TMessage>, message: TMessage): void {
        send(messageType, encode(proto.name, message), true);
    }

    encode<TMessage>(proto: ProtoDescriptor<TMessage>, message: TMessage): Uint8Array {
        return encode(proto.name, message) as Uint8Array;
    }
}

class GcRawMessageContext implements RawMessageContext {
    messageType: number;
    payload: Uint8Array;
    steamId: bigint;
    accountId: number;
    personaName: string;
    clock: Clock;
    logger: Logger;

    constructor(currentMessageType: number) {
        this.messageType = currentMessageType;
        this.payload = body() as Uint8Array;
        this.steamId = currentSteamId();
        this.accountId = currentAccountId();
        this.personaName = currentPersonaName();
        this.clock = new GcClock();
        this.logger = new GcLogger();
    }

    send<TMessage>(messageType: number, proto: ProtoDescriptor<TMessage>, message: TMessage): void {
        send(messageType, encode(proto.name, message), true);
    }

    encode<TMessage>(proto: ProtoDescriptor<TMessage>, message: TMessage): Uint8Array {
        return encode(proto.name, message) as Uint8Array;
    }

    decode<TMessage>(proto: ProtoDescriptor<TMessage>): TMessage {
        return decode(proto.name, this.payload) as TMessage;
    }
}

const emptyProto: ProtoDescriptor<unknown> = { name: "" };
const emptyRoute: GcRoute<unknown, unknown> = {
    requestId: 0,
    responseId: 0,
    request: emptyProto,
    response: emptyProto
};
const unregisteredRouteHandler: RouteHandler<unknown, unknown> = () => false;
const unregisteredRawHandler: RawMessageHandler = () => false;

class GcRouter {
    messageIds: number[];
    routes: GcRoute<unknown, unknown>[];
    routeHandlers: RouteHandler<unknown, unknown>[];
    rawHandlers: RawMessageHandler[];
    isRawMessage: boolean[];
    count: number;

    constructor() {
        this.messageIds = [];
        this.routes = [];
        this.routeHandlers = [];
        this.rawHandlers = [];
        this.isRawMessage = [];
        this.count = 0;
    }

    on<TRequest, TResponse>(route: GcRoute<TRequest, TResponse>, handler: RouteHandler<TRequest, TResponse>): void {
        const index = this.count;
        this.messageIds[index] = route.requestId;
        this.routes[index] = route as GcRoute<unknown, unknown>;
        this.routeHandlers[index] = handler as RouteHandler<unknown, unknown>;
        this.rawHandlers[index] = unregisteredRawHandler;
        this.isRawMessage[index] = false;
        this.count = this.count + 1;
    }

    onMessage(messageId: number, handler: RawMessageHandler): void {
        const index = this.count;
        this.messageIds[index] = messageId;
        this.routes[index] = emptyRoute;
        this.routeHandlers[index] = unregisteredRouteHandler;
        this.rawHandlers[index] = handler;
        this.isRawMessage[index] = true;
        this.count = this.count + 1;
    }

    async dispatch(): Promise<boolean> {
        const current = messageType();

        for (let i = 0; i < this.count; i = i + 1) {
            if (this.messageIds[i] === current) {
                let result: HandlerResult = true;
                if (this.isRawMessage[i]) {
                    const handler = this.rawHandlers[i];
                    result = handler(this.createRawContext(current));
                } else {
                    const handler = this.routeHandlers[i];
                    result = handler(this.createContext(this.routes[i]));
                }

                const resolved = await result;
                if (resolved === false) {
                    return false;
                }
                return true;
            }
        }

        return false;
    }

    createContext<TRequest, TResponse>(route: GcRoute<TRequest, TResponse>): HandlerContext<TRequest, TResponse> {
        return new GcHandlerContext(route);
    }

    createRawContext(currentMessageType: number): RawMessageContext {
        return new GcRawMessageContext(currentMessageType);
    }
}

export const gc = new GcRouter();

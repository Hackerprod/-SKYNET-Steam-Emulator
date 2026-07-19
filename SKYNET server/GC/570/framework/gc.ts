import { GcRoute, ProtoDescriptor } from "../generated/dota";

export interface HandlerContext<TRequest, TResponse> {
    readonly route: GcRoute<TRequest, TResponse>;
    readonly request: TRequest;
    readonly steamId: uint64;
    readonly accountId: int32;
    readonly personaName: string;
    reply(response: TResponse): void;
    send<TMessage>(messageType: int32, proto: ProtoDescriptor<TMessage>, message: TMessage): void;
    encode<TMessage>(proto: ProtoDescriptor<TMessage>, message: TMessage): int32[];
}

function createContext<TRequest, TResponse>(route: GcRoute<TRequest, TResponse>): HandlerContext<TRequest, TResponse> {
    return {
        route: route,
        request: decode(route.request.name, body()) as TRequest,
        steamId: steamId(),
        accountId: accountId(),
        personaName: personaName(),
        reply: (response: TResponse): void => {
            send(route.responseId, encode(route.response.name, response), true);
        },
        send: <TMessage>(messageType: int32, proto: ProtoDescriptor<TMessage>, message: TMessage): void => {
            send(messageType, encode(proto.name, message), true);
        },
        encode: <TMessage>(proto: ProtoDescriptor<TMessage>, message: TMessage): int32[] => {
            return encode(proto.name, message) as int32[];
        }
    } as HandlerContext<TRequest, TResponse>;
}

export const gc = {
    on: <TRequest, TResponse>(
        route: GcRoute<TRequest, TResponse>,
        handler: (ctx: HandlerContext<TRequest, TResponse>) => void): boolean => {
        if (messageType() != route.requestId) {
            return false;
        }

        handler(createContext(route));
        return true;
    }
} as const;

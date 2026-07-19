declare global {
    type int32 = number;

    interface Clock {
        now(): number;
    }

    interface Logger {
        info(message: string): void;
    }

    function messageType(): number;
    function body(): Uint8Array;
    function now(): number;
    function steamId(): bigint;
    function accountId(): number;
    function personaName(): string;
    function decode<TMessage = unknown>(typeName: string, payload: Uint8Array): TMessage;
    function encode<TMessage = unknown>(typeName: string, value: TMessage): Uint8Array;
    function send(messageType: number, payload: Uint8Array, protobuf?: boolean): boolean;
    function log(message: string): void;
}

export {};

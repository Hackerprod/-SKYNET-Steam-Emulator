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
    function dotaInventory(): unknown;
    function dotaCatalogItem(defIndex: number): unknown;
    function dotaEquipItem(itemId: bigint, heroId: number, slotId: number, style: number): unknown;
    function dotaSetItemStyle(itemId: bigint, style: number): unknown;
    function dotaQueueCurrentLobbyServer(messageType: number, payload: Uint8Array): boolean;
    function dotaProfile(accountId: number): unknown;
    function dotaSaveProfileSlots(slots: unknown[]): boolean;
    function dotaSaveProfileUpdate(backgroundItemId: bigint, featuredHeroIds: number[]): boolean;
    function dotaSocialFeed(accountId: number, selfOnly: boolean): unknown;
    function dotaSocialFeedComments(feedEventId: bigint): unknown;
    function dotaSocialFeedPostComment(feedEventId: bigint, comment: string): boolean;
    function dotaEventPoints(accountId: number, eventId: number): unknown;
    function dotaHeroStandings(accountId: number): unknown;
    function dotaRank(accountId: number): unknown;
}

export {};

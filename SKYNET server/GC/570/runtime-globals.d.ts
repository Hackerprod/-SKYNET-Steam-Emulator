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
    function dotaLookupAccountName(accountId: number): unknown;
    function dotaEventPoints(accountId: number, eventId: number): unknown;
    function dotaHeroStandings(accountId: number): unknown;
    function dotaHeroGlobalData(accountId: number, heroId: number): unknown;
    function dotaPlayerStats(accountId: number): unknown;
    function dotaRank(accountId: number): unknown;
    function dotaTeammateStats(accountId: number): unknown;
    function dotaMatchHistory(
        accountId: number,
        startAtMatchId: bigint,
        requested: number,
        heroId: number,
        includePractice: boolean
    ): unknown;
    function dotaMatchDetails(matchId: bigint): unknown;
    function dotaHeroStatsHistory(accountId: number, heroId: number): unknown;
    function dotaShowcaseStats(accountId: number): unknown;
    function dotaRecentAccomplishments(accountId: number): unknown;
    function dotaHeroRecentAccomplishments(accountId: number, heroId: number): unknown;
    function dotaHasMvpVote(matchId: bigint): boolean;
    function dotaVoteForMvp(matchId: bigint, votedAccountId: number): boolean;
    function dotaFinalizeMvpVote(matchId: bigint): boolean;
    function dotaSubmitLobbyMvpVote(targetAccountId: number): unknown;
    function dotaRecordSignOutMvpStats(matchId: bigint, players: unknown[]): boolean;
    function dotaRerollPlayerChallenge(): boolean;
    function dotaRecordMatchSignOutPermission(request: unknown): boolean;
    function dotaSetMatchHistoryAccess(allow: boolean): boolean;
    function dotaRecordServerStatus(response: number): boolean;
    function dotaRecordLeaver(event: unknown): boolean;
    function dotaRecordRealtimeStats(snapshot: unknown): boolean;
    function dotaRecordMatchStateHistory(history: unknown): boolean;
    function dotaRecordSpectatorCount(spectatorCount: number): boolean;
    function dotaRecordLiveScoreboard(snapshot: unknown): boolean;
    function dotaSavePlayerReport(report: unknown): boolean;
}

export {};

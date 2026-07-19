export interface ProtoDescriptor<TMessage> {
    readonly name: string;
}

export interface GcRoute<TRequest, TResponse> {
    readonly requestId: int32;
    readonly request: ProtoDescriptor<TRequest>;
    readonly responseId: int32;
    readonly response: ProtoDescriptor<TResponse>;
}

export interface EmptyRequest {
}

export interface CMsgClientHello {
    readonly version?: int32;
    readonly clientSessionNeed?: int32;
}

export interface CMsgConnectionStatus {
    readonly status: int32;
    readonly clientSessionNeed: int32;
}

export interface CSODOTAGameAccountClient {
    readonly accountId: int32;
}

export interface CSODOTAGameAccountPlus {
    readonly accountId: int32;
}

export interface CMsgSOIDOwner {
    readonly type: int32;
    readonly id: uint64;
}

export interface CMsgSOCacheSubscribedObject {
    readonly typeId: int32;
    readonly objectData: int32[][];
}

export interface CMsgSOCacheSubscribed {
    readonly objects: CMsgSOCacheSubscribedObject[];
    readonly version: int32;
    readonly ownerSoid: CMsgSOIDOwner;
    readonly serviceId: int32;
    readonly serviceList: int32[];
    readonly syncVersion: uint64;
}

export interface CMsgDOTAWelcome {
    readonly allow3rdPartyMatchHistory: boolean;
    readonly gcSocacheFileVersion: int32;
    readonly activeEvent: int32;
    readonly activeEventForDisplay: int32;
}

export interface CMsgClientWelcome {
    readonly version: int32;
    readonly gameData: int32[];
    readonly outofdateSubscribedCaches: CMsgSOCacheSubscribed[];
    readonly gcSocacheFileVersion: int32;
    readonly rtime32GcWelcomeTimestamp: int32;
    readonly currency: int32;
}

export interface CMsgGCRequestStoreSalesData {
    readonly version?: int32;
}

export interface CMsgGCRequestStoreSalesDataResponse {
    readonly version: int32;
    readonly expirationTime: int32;
}

export interface CMsgGCNotificationsRequest {
}

export interface CMsgGCNotificationsUpdate {
    readonly result: int32;
    readonly notifications: any[];
}

export interface CMsgGCNotificationsResponse {
    readonly update: CMsgGCNotificationsUpdate;
}

export interface CMsgDOTAJoinChatChannel {
    readonly channelName?: string;
    readonly channelType?: int32;
}

export interface CMsgDOTAJoinChatChannelResponse {
    readonly channelName?: string;
    readonly channelType?: int32;
    readonly result: int32;
}

export interface CMsgGCToClientFindTopSourceTVGamesResponse {
    readonly searchKey?: string;
    readonly matches: any[];
}

export interface CMsgClientToGCGetCurrentPrivateCoachingSessionResponse {
    readonly result: int32;
    readonly currentSession?: any;
}

export interface CMsgAvailablePrivateCoachingSessionList {
    readonly availableCoachingSessions: any[];
}

export interface CMsgClientToGCGetAvailablePrivateCoachingSessionsResponse {
    readonly result: int32;
    readonly availableSessionsList: CMsgAvailablePrivateCoachingSessionList;
}

export interface CMsgAvailablePrivateCoachingSessionSummary {
    readonly coachingSessionCount: int32;
}

export interface CMsgClientToGCGetAvailablePrivateCoachingSessionsSummaryResponse {
    readonly result: int32;
    readonly coachingSessionSummary: CMsgAvailablePrivateCoachingSessionSummary;
}

export const Msg = {
    GCClientWelcome: 4004,
    GCClientHello: 4006,
    GCClientConnectionStatus: 4009,
    GCJoinChatChannel: 7009,
    GCJoinChatChannelResponse: 7010,
    GCRequestStoreSalesData: 2536,
    GCRequestStoreSalesDataResponse: 2537,
    ClientToGCCancelUnfinalizedTransactions: 2617,
    ClientToGCAggregateMetrics: 4523,
    GCNotificationsRequest: 7427,
    GCNotificationsResponse: 7428,
    ClientToGCEmoticonDataRequest: 7503,
    GCToClientEmoticonData: 7504,
    ClientToGCFindTopSourceTVGames: 8009,
    GCToClientFindTopSourceTVGamesResponse: 8010,
    ClientToGCGetCurrentPrivateCoachingSession: 8793,
    ClientToGCGetCurrentPrivateCoachingSessionResponse: 8794,
    ClientToGCGetAvailablePrivateCoachingSessions: 8798,
    ClientToGCGetAvailablePrivateCoachingSessionsResponse: 8799,
    ClientToGCGetAvailablePrivateCoachingSessionsSummary: 8800,
    ClientToGCGetAvailablePrivateCoachingSessionsSummaryResponse: 8801
} as const;

export const Proto = {
    EmptyRequest: { name: "EmptyRequest" } as ProtoDescriptor<EmptyRequest>,
    CMsgClientHello: { name: "CMsgClientHello" } as ProtoDescriptor<CMsgClientHello>,
    CMsgClientWelcome: { name: "CMsgClientWelcome" } as ProtoDescriptor<CMsgClientWelcome>,
    CMsgConnectionStatus: { name: "CMsgConnectionStatus" } as ProtoDescriptor<CMsgConnectionStatus>,
    CMsgDOTAWelcome: { name: "CMsgDOTAWelcome" } as ProtoDescriptor<CMsgDOTAWelcome>,
    CSODOTAGameAccountClient: { name: "CSODOTAGameAccountClient" } as ProtoDescriptor<CSODOTAGameAccountClient>,
    CSODOTAGameAccountPlus: { name: "CSODOTAGameAccountPlus" } as ProtoDescriptor<CSODOTAGameAccountPlus>,
    CMsgDOTAJoinChatChannel: { name: "CMsgDOTAJoinChatChannel" } as ProtoDescriptor<CMsgDOTAJoinChatChannel>,
    CMsgDOTAJoinChatChannelResponse: { name: "CMsgDOTAJoinChatChannelResponse" } as ProtoDescriptor<CMsgDOTAJoinChatChannelResponse>,
    CMsgGCRequestStoreSalesData: { name: "CMsgGCRequestStoreSalesData" } as ProtoDescriptor<CMsgGCRequestStoreSalesData>,
    CMsgGCRequestStoreSalesDataResponse: { name: "CMsgGCRequestStoreSalesDataResponse" } as ProtoDescriptor<CMsgGCRequestStoreSalesDataResponse>,
    CMsgGCNotificationsRequest: { name: "CMsgGCNotificationsRequest" } as ProtoDescriptor<CMsgGCNotificationsRequest>,
    CMsgGCNotificationsResponse: { name: "CMsgGCNotificationsResponse" } as ProtoDescriptor<CMsgGCNotificationsResponse>,
    CMsgClientToGCFindTopSourceTVGames: { name: "CMsgClientToGCFindTopSourceTVGames" } as ProtoDescriptor<EmptyRequest>,
    CMsgGCToClientFindTopSourceTVGamesResponse: { name: "CMsgGCToClientFindTopSourceTVGamesResponse" } as ProtoDescriptor<CMsgGCToClientFindTopSourceTVGamesResponse>,
    CMsgClientToGCGetCurrentPrivateCoachingSession: { name: "CMsgClientToGCGetCurrentPrivateCoachingSession" } as ProtoDescriptor<EmptyRequest>,
    CMsgClientToGCGetCurrentPrivateCoachingSessionResponse: { name: "CMsgClientToGCGetCurrentPrivateCoachingSessionResponse" } as ProtoDescriptor<CMsgClientToGCGetCurrentPrivateCoachingSessionResponse>,
    CMsgClientToGCGetAvailablePrivateCoachingSessions: { name: "CMsgClientToGCGetAvailablePrivateCoachingSessions" } as ProtoDescriptor<EmptyRequest>,
    CMsgClientToGCGetAvailablePrivateCoachingSessionsResponse: { name: "CMsgClientToGCGetAvailablePrivateCoachingSessionsResponse" } as ProtoDescriptor<CMsgClientToGCGetAvailablePrivateCoachingSessionsResponse>,
    CMsgClientToGCGetAvailablePrivateCoachingSessionsSummary: { name: "CMsgClientToGCGetAvailablePrivateCoachingSessionsSummary" } as ProtoDescriptor<EmptyRequest>,
    CMsgClientToGCGetAvailablePrivateCoachingSessionsSummaryResponse: { name: "CMsgClientToGCGetAvailablePrivateCoachingSessionsSummaryResponse" } as ProtoDescriptor<CMsgClientToGCGetAvailablePrivateCoachingSessionsSummaryResponse>
} as const;

export const CoachingResponse = {
    InternalError: 0,
    Success: 1,
    TooBusy: 2,
    Disabled: 3,
    Timeout: 4
} as const;

export const ConnectionStatus = {
    HaveSession: 0,
    NoSessionInLogonQueue: 3
} as const;

export const Welcome = {
    Version: 20,
    OwnerSteamId: 1,
    DotaServiceGame: 0,
    DotaServiceEcon: 1,
    TypeDotaAccount: 2002,
    TypeDotaPlus: 2012
} as const;

export const NotificationResult = {
    Success: 0,
    ErrorUnspecified: 1
} as const;

export const JoinChatResult = {
    JoinSuccess: 0,
    InvalidChannelType: 1,
    AccountNotFound: 2,
    UserInTooManyChannels: 4,
    RateLimitExceeded: 5,
    ChannelFull: 6,
    UserNotAllowed: 14,
    SilentError: 17
} as const;

export const Routes = {
    ClientHello: {
        requestId: Msg.GCClientHello,
        request: Proto.CMsgClientHello,
        responseId: Msg.GCClientWelcome,
        response: Proto.CMsgClientWelcome
    } as GcRoute<CMsgClientHello, CMsgClientWelcome>,
    RequestStoreSalesData: {
        requestId: Msg.GCRequestStoreSalesData,
        request: Proto.CMsgGCRequestStoreSalesData,
        responseId: Msg.GCRequestStoreSalesDataResponse,
        response: Proto.CMsgGCRequestStoreSalesDataResponse
    } as GcRoute<CMsgGCRequestStoreSalesData, CMsgGCRequestStoreSalesDataResponse>,
    Notifications: {
        requestId: Msg.GCNotificationsRequest,
        request: Proto.CMsgGCNotificationsRequest,
        responseId: Msg.GCNotificationsResponse,
        response: Proto.CMsgGCNotificationsResponse
    } as GcRoute<CMsgGCNotificationsRequest, CMsgGCNotificationsResponse>,
    JoinChatChannel: {
        requestId: Msg.GCJoinChatChannel,
        request: Proto.CMsgDOTAJoinChatChannel,
        responseId: Msg.GCJoinChatChannelResponse,
        response: Proto.CMsgDOTAJoinChatChannelResponse
    } as GcRoute<CMsgDOTAJoinChatChannel, CMsgDOTAJoinChatChannelResponse>,
    FindTopSourceTvGames: {
        requestId: Msg.ClientToGCFindTopSourceTVGames,
        request: Proto.CMsgClientToGCFindTopSourceTVGames,
        responseId: Msg.GCToClientFindTopSourceTVGamesResponse,
        response: Proto.CMsgGCToClientFindTopSourceTVGamesResponse
    } as GcRoute<EmptyRequest, CMsgGCToClientFindTopSourceTVGamesResponse>,
    GetCurrentPrivateCoachingSession: {
        requestId: Msg.ClientToGCGetCurrentPrivateCoachingSession,
        request: Proto.CMsgClientToGCGetCurrentPrivateCoachingSession,
        responseId: Msg.ClientToGCGetCurrentPrivateCoachingSessionResponse,
        response: Proto.CMsgClientToGCGetCurrentPrivateCoachingSessionResponse
    } as GcRoute<EmptyRequest, CMsgClientToGCGetCurrentPrivateCoachingSessionResponse>,
    GetAvailablePrivateCoachingSessions: {
        requestId: Msg.ClientToGCGetAvailablePrivateCoachingSessions,
        request: Proto.CMsgClientToGCGetAvailablePrivateCoachingSessions,
        responseId: Msg.ClientToGCGetAvailablePrivateCoachingSessionsResponse,
        response: Proto.CMsgClientToGCGetAvailablePrivateCoachingSessionsResponse
    } as GcRoute<EmptyRequest, CMsgClientToGCGetAvailablePrivateCoachingSessionsResponse>,
    GetAvailablePrivateCoachingSessionsSummary: {
        requestId: Msg.ClientToGCGetAvailablePrivateCoachingSessionsSummary,
        request: Proto.CMsgClientToGCGetAvailablePrivateCoachingSessionsSummary,
        responseId: Msg.ClientToGCGetAvailablePrivateCoachingSessionsSummaryResponse,
        response: Proto.CMsgClientToGCGetAvailablePrivateCoachingSessionsSummaryResponse
    } as GcRoute<EmptyRequest, CMsgClientToGCGetAvailablePrivateCoachingSessionsSummaryResponse>
} as const;

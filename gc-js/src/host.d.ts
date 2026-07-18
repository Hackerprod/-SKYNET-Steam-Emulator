// Production host->JS contract (Sec.3 of the migration roadmap).
//
// uint64 boundary rule: every 64-bit identifier crossing the host boundary is a
// decimal string (steamId, jobIds). Inside JS, protobuf 64-bit fields are
// Long.js objects (Fase 0 decision); convert with Long.fromString/toString.
// `runtime.fixture` does NOT exist in this contract: the production sandbox
// never has it (see host.dev.d.ts, dev/test builds only).

export interface GcEnvelope {
  appId: number;
  steamId: string; // uint64 as string
  accountId: number;
  personaName: string;
  clientIp: string;
  messageType: number;
  sourceJobId: string | null; // uint64 as string
  targetJobId: string | null;
  isGameServer: boolean;
  // Derived server-side from the last CMsgClientHello (field 1) seen for this
  // session; null before the first hello. Fase B decision: no steam_api change.
  clientVersion: number | null;
  payload: Uint8Array;
}

export interface GcEmitApi {
  reply(responseMsgId: number, bytes: Uint8Array): void;
  proto(msgId: number, bytes: Uint8Array): void;
  queueReplyTo(steamId: string, msgId: number, bytes: Uint8Array, targetJobId: string): void;
  queueToServer(serverSteamId: string, msgId: number, bytes: Uint8Array): void;
  notHandled(reason: string): void;
}

declare global {
  // Host-injected logger (routes to GameCoordinatorTraceService).
  function log(message: string): void;
}

export {};

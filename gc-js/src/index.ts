// Entry point of the GC JS bundle (dist/gc.js), executed by JintGcEngine.
//
// Boot protocol with the host:
//   1. Host sets globalThis.__PROTO_DESCRIPTOR__ = contents of proto-descriptor.json
//   2. Host evaluates this bundle (protobufjs/light + Long are bundled in).
//   3. Per exchange, host calls __dispatch(envelopeJson) -> result JSON.
//      Per tick, host calls __tick(ctxJson).
// The JSON boundary keeps every uint64 as a string (see src/proto.ts header).
//
// Handlers get the roadmap contract (env, gc.*, Proto, log). During Fase 1 the
// only handler is the noop gate: everything returns notHandled and the host
// falls back to the Lua/C# engine.

import { initProto, Proto } from "./proto";
import { b64decode, b64encode } from "./base64";
import type { GcEnvelope, GcEmitApi } from "./host";

declare function log(message: string): void;

interface EmittedMessage {
  kind: "reply" | "proto" | "queueReplyTo" | "queueToServer";
  msgId: number;
  payloadBase64: string;
  targetSteamId?: string;
  targetJobId?: string;
}

interface DispatchResult {
  handled: boolean;
  reason: string | null;
  messages: EmittedMessage[];
}

let emissions: EmittedMessage[] = [];
let handledFlag = true;
let notHandledReason: string | null = null;

const gc: GcEmitApi = {
  reply(responseMsgId, bytes) {
    emissions.push({ kind: "reply", msgId: responseMsgId, payloadBase64: b64encode(bytes) });
  },
  proto(msgId, bytes) {
    emissions.push({ kind: "proto", msgId, payloadBase64: b64encode(bytes) });
  },
  queueReplyTo(steamId, msgId, bytes, targetJobId) {
    emissions.push({
      kind: "queueReplyTo",
      msgId,
      payloadBase64: b64encode(bytes),
      targetSteamId: steamId,
      targetJobId,
    });
  },
  queueToServer(serverSteamId, msgId, bytes) {
    emissions.push({
      kind: "queueToServer",
      msgId,
      payloadBase64: b64encode(bytes),
      targetSteamId: serverSteamId,
    });
  },
  notHandled(reason) {
    handledFlag = false;
    notHandledReason = reason;
  },
};

// ---------------------------------------------------------------- handlers
// Message handlers registered by id. Empty during Fase 1 (noop gate): every
// exchange falls through to notHandled and the router uses the Lua/C# engine.
type Handler = (env: GcEnvelope) => void;
const handlers: Map<number, Handler> = new Map();

function handle(env: GcEnvelope): void {
  const handler = handlers.get(env.messageType);
  if (!handler) {
    gc.notHandled("noop: no JS handler registered for msg " + env.messageType);
    return;
  }
  handler(env);
}

function tick(_ctx: { appId: number }): void {
  // No JS-side timers during Fase 1; lobby/party state still lives in Lua/C#.
}

// ------------------------------------------------------------- host bridge
const g = globalThis as any;

g.__dispatch = function (envelopeJson: string): string {
  const raw = JSON.parse(envelopeJson);
  emissions = [];
  handledFlag = true;
  notHandledReason = null;
  const env: GcEnvelope = {
    appId: raw.appId,
    steamId: raw.steamId ?? "0",
    accountId: raw.accountId ?? 0,
    personaName: raw.personaName ?? "",
    clientIp: raw.clientIp ?? "",
    messageType: raw.messageType,
    sourceJobId: raw.sourceJobId ?? null,
    targetJobId: raw.targetJobId ?? null,
    isGameServer: raw.isGameServer === true,
    clientVersion: raw.clientVersion ?? null,
    payload: b64decode(raw.payloadBase64 ?? ""),
  };

  try {
    handle(env);
  } catch (error) {
    handledFlag = false;
    notHandledReason = "exception: " + String(error);
    emissions = [];
  }

  const result: DispatchResult = {
    handled: handledFlag,
    reason: notHandledReason,
    messages: handledFlag ? emissions : [],
  };
  return JSON.stringify(result);
};

g.__tick = function (ctxJson: string): string {
  try {
    tick(JSON.parse(ctxJson));
    return "{}";
  } catch (error) {
    return JSON.stringify({ error: String(error) });
  }
};

// Codec bridge used by the spike suite and by host-side oracle tooling; the
// same code paths the handlers use, exposed over the JSON/base64 boundary.
g.__proto_decode_json = function (typeName: string, payloadBase64: string): string {
  return JSON.stringify(Proto.decodeToObject(typeName, b64decode(payloadBase64)));
};

g.__proto_encode_b64 = function (typeName: string, objectJson: string): string {
  return b64encode(Proto.encode(typeName, JSON.parse(objectJson)));
};

// Decode->encode round-trips executed inside the engine so perf measurements
// are not dominated by host<->engine string marshalling.
g.__proto_roundtrip_bench = function (typeName: string, payloadBase64: string, iterations: number): string {
  const bytes = b64decode(payloadBase64);
  let lastLength = 0;
  for (let i = 0; i < iterations; i++) {
    const message = Proto.decode(typeName, bytes);
    lastLength = Proto.encode(typeName, message).length;
  }
  return JSON.stringify({ iterations, inputLength: bytes.length, outputLength: lastLength });
};

g.__gc_bundle_version = "0.1.0";

if (typeof g.__PROTO_DESCRIPTOR__ === "string") {
  initProto(g.__PROTO_DESCRIPTOR__);
  if (typeof log === "function") {
    log("gc.js bundle " + g.__gc_bundle_version + " loaded; proto descriptor initialised");
  }
}

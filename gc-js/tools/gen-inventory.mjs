// Fase A - Contract Inventory & Quarantine.
// Derives every artifact from the source files (messages.lua, routes.generated.lua,
// DotaGcBackend*.cs, the Lua dispatch tables). Nothing is hardcoded except the
// classification rules and explicit overrides, which are the reviewable part.
//
// Outputs (all versioned in git):
//   gc-js/registry/proto-message-registry.json
//   gc-js/observed-migration.csv
//   gc-js/quarantine.json
//   gc-js/inventory/dotagcbackend-inventory.json
//
// Run: node tools/gen-inventory.mjs   (cwd = gc-js/)

import fs from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const here = path.dirname(fileURLToPath(import.meta.url));
const repoRoot = path.resolve(here, "..", "..");
const gcDir = path.join(repoRoot, "SKYNET server", "GC", "570");
const outDir = path.resolve(here, "..");

// ---------------------------------------------------------------- messages.lua
const messagesLua = fs.readFileSync(path.join(gcDir, "messages.lua"), "utf8");
const msgByName = new Map();
const msgById = new Map();
for (const m of messagesLua.matchAll(/^\s*(\w+)\s*=\s*(\d+)\s*,?\s*$/gm)) {
  const name = m[1];
  const id = Number(m[2]);
  msgByName.set(name, id);
  if (!msgById.has(id)) msgById.set(id, name);
}

// ---------------------------------------------------- routes.generated.lua (OBSERVED)
const routesLua = fs.readFileSync(path.join(gcDir, "routes.generated.lua"), "utf8");
const observed = { client: new Map(), server: new Map() };
for (const m of routesLua.matchAll(/OBSERVED\.(client|server)\[(\d+)\]\s*=\s*\{([^}]*)\}/g)) {
  const fixtures = [...m[3].matchAll(/"([^"]+)"/g)].map((f) => f[1]);
  observed[m[1]].set(Number(m[2]), fixtures);
}

// -------------------------------------------- Lua dispatch tables (handled message names)
const luaHandlerFiles = [
  "main.lua",
  "dota_lobby.lua",
  "dota_party.lua",
  "dota_items.lua",
  "dota_stats.lua",
];
const handledNames = new Set();
for (const file of luaHandlerFiles) {
  const text = fs.readFileSync(path.join(gcDir, file), "utf8");
  for (const m of text.matchAll(/\[MSG\.(\w+)\]\s*=/g)) handledNames.add(m[1]);
  for (const m of text.matchAll(/==\s*MSG\.(\w+)/g)) handledNames.add(m[1]);
}
const handledIds = new Set([...handledNames].map((n) => msgByName.get(n)).filter((v) => v != null));

// -------------------------------------------------------------- proto type mapping
// Explicit overrides for messages whose proto type does not follow a naming rule.
// Everything else is left empty and gets filled during Fase 2 curation.
const protoTypeOverrides = {
  SOSingleObject: "CMsgSOSingleObject",
  SOSingleObjectDestroyed: "CMsgSOSingleObject",
  SOCacheSubscribed: "CMsgSOCacheSubscribed",
  SOCacheUnsubscribed: "CMsgSOCacheUnsubscribed",
  SOCacheUpdated: "CMsgSOMultipleObjects",
  SOCacheSubscriptionRefresh: "CMsgSOCacheSubscriptionRefresh",
  SOCacheSubscribedUpToDate: "CMsgSOCacheSubscribedUpToDate",
  GCClientHello: "CMsgClientHello",
  GCGameServerHello: "CMsgClientHello",
  GCClientWelcome: "CMsgClientWelcome",
  GCGameServerWelcome: "CMsgClientWelcome",
  GCClientConnectionStatus: "CMsgConnectionStatus",
  GCPracticeLobbyCreate: "CMsgPracticeLobbyCreate",
  GCPracticeLobbyResponse: "CMsgPracticeLobbyJoinResponse",
  GCPracticeLobbySetDetails: "CMsgPracticeLobbySetDetails",
  GCPracticeLobbySetTeamSlot: "CMsgPracticeLobbySetTeamSlot",
  GCPracticeLobbyLeave: "CMsgPracticeLobbyLeave",
  GCPracticeLobbyLaunch: "CMsgPracticeLobbyLaunch",
  GCPracticeLobbyJoin: "CMsgPracticeLobbyJoin",
  GCPracticeLobbyList: "CMsgPracticeLobbyList",
  GCPracticeLobbyListResponse: "CMsgPracticeLobbyListResponse",
  GCMatchmakingStatsRequest: "CMsgDOTAMatchmakingStatsRequest",
  GCMatchmakingStatsResponse: "CMsgDOTAMatchmakingStatsResponse",
  GCJoinChatChannel: "CMsgDOTAJoinChatChannel",
  GCJoinChatChannelResponse: "CMsgDOTAJoinChatChannelResponse",
  GCRequestStoreSalesData: "CMsgGCGetStoreSalesData",
  GCRequestStoreSalesDataResponse: "CMsgGCGetStoreSalesDataResponse",
  DOTAGetEventPoints: "CMsgDOTAGetEventPoints",
  DOTAGetEventPointsResponse: "CMsgDOTAGetEventPointsResponse",
  ClientToGCGetProfileCard: "CMsgClientToGCGetProfileCard",
  ClientToGCGetProfileCardResponse: "CMsgDOTAProfileCard",
  GCToClientProfileCardUpdated: "CMsgDOTAProfileCard",
  ClientToGCRankRequest: "CMsgClientToGCRankRequest",
  GCToClientRankResponse: "CMsgGCToClientRankResponse",
  ClientToGCEquipItems: "CMsgClientToGCEquipItems",
  ClientToGCEquipItemsResponse: "CMsgClientToGCEquipItemsResponse",
  ClientToGCSetItemStyle: "CMsgClientToGCSetItemStyle",
  ClientToGCSetItemStyleResponse: "CMsgClientToGCSetItemStyleResponse",
  GCGenericResult: "CMsgDOTAGenericResult",
};

function protoTypeFor(name) {
  if (!name) return "";
  if (protoTypeOverrides[name]) return protoTypeOverrides[name];
  if (name.startsWith("ClientToGC") || name.startsWith("GCToClient") || name.startsWith("ServerToGC")) {
    return `CMsg${name}`;
  }
  return "";
}

// ------------------------------------------------------------ observed bucket rules
// catalogo-global : same bytes for every player (catalog/config style responses)
// por-jugador     : identity/state derived from DB per account
// deprecado-vacio : empty/absent response tolerated by the client
// desconocido     : id not present in messages.lua; needs analysis before cutover
const catalogPatterns = /(StoreSalesData|MatchmakingStats|EmoticonData|HeroGlobalData|EventSchedule|TopCustomGames)/;
const perPlayerPatterns = /(ProfileCard|ProfileTickets|Profile\b|Rank|Guild|Quest|Conduct|TeamInfo|MyTeamInfo|PeriodicResource|Showcase|Overworld|MonsterHunter|BattleReport|LookupAccountName|EventPoints|HeroStickers|Notifications|CoachingSession|ChatChannel|Hello|Welcome|SOCache|SOSingle)/;
const emptyPatterns = /(AggregateMetrics|CancelUnfinalizedTransactions|FindTopSourceTVGames|HeroStandings)/;

function bucketFor(name) {
  if (!name) return "desconocido";
  if (emptyPatterns.test(name)) return "deprecado-vacio";
  if (catalogPatterns.test(name)) return "catalogo-global";
  if (perPlayerPatterns.test(name)) return "por-jugador";
  return "por-jugador";
}

function fixtureStats(fixtures) {
  let count = 0;
  let bytes = 0;
  for (const rel of fixtures) {
    const p = path.join(gcDir, rel.replace(/\//g, path.sep));
    try {
      bytes += fs.statSync(p).size;
      count++;
    } catch {
      // missing fixture file: keep count of listed entries anyway
      count++;
    }
  }
  return { count, avgBytes: count ? Math.round(bytes / count) : 0 };
}

// ------------------------------------------------------- observed-migration.csv
const csvRows = [
  "msgId,name,direction,protoType,bucket,sourceCapture,fixtureCount,avgBytes,handledToday,owner,status",
];
const allObserved = [];
for (const dir of ["client", "server"]) {
  for (const [id, fixtures] of [...observed[dir].entries()].sort((a, b) => a[0] - b[0])) {
    const name = msgById.get(id) ?? "";
    const stats = fixtureStats(fixtures);
    const bucket = bucketFor(name);
    // A message already served by a real Lua/C# builder only needs migration of the
    // builder, not a from-scratch implementation; the fixture stays as test oracle.
    const requestName = name;
    const handledToday =
      dir === "client"
        ? handledNames.has(requestName)
        : true; // server-direction ids are emissions; they exist because some handler produced them
    const status = handledToday ? "builder-existente" : "pendiente-analisis";
    allObserved.push({ id, dir, name, bucket, status });
    csvRows.push(
      [
        id,
        name,
        dir === "client" ? "c2s" : "s2c",
        protoTypeFor(name),
        bucket,
        fixtures[0] ?? "",
        stats.count,
        stats.avgBytes,
        handledToday ? "yes" : "no",
        "",
        status,
      ].join(","),
    );
  }
}
fs.writeFileSync(path.join(outDir, "observed-migration.csv"), csvRows.join("\n") + "\n");

// ------------------------------------------------- ProtoMessageRegistry (generated)
const registry = {
  $comment:
    "Generated by gc-js/tools/gen-inventory.mjs from messages.lua + routes.generated.lua + the Lua dispatch tables. Single source for messageId -> proto type -> direction -> engine. Regenerate, do not hand-edit.",
  appId: 570,
  messages: {},
};
const c2sHeuristic = /^(ClientToGC|ServerToGC|GC(?!To)(?!Client(Welcome|ConnectionStatus)))/;
for (const [name, id] of [...msgByName.entries()].sort((a, b) => a[1] - b[1])) {
  let direction = "unknown";
  if (observed.client.has(id)) direction = "c2s";
  else if (observed.server.has(id)) direction = "s2c";
  else if (name.startsWith("ClientToGC") || (name.endsWith("Request") && !name.startsWith("GCToClient"))) direction = "c2s";
  else if (name.startsWith("GCToClient") || name.endsWith("Response")) direction = "s2c";
  registry.messages[id] = {
    name,
    protoType: protoTypeFor(name),
    direction,
    handledByLua: handledIds.has(id),
    engine: "lua",
  };
}
fs.mkdirSync(path.join(outDir, "registry"), { recursive: true });
fs.writeFileSync(
  path.join(outDir, "registry", "proto-message-registry.json"),
  JSON.stringify(registry, null, 2) + "\n",
);

// ------------------------------------------- DotaGcBackend*.cs member classification
const backendFiles = fs
  .readdirSync(gcDir)
  .filter((f) => f.startsWith("DotaGcBackend") && f.endsWith(".cs"));

const methodRegex =
  /^\s*(?:\[[^\]]+\]\s*)*(?:public|internal|private|protected)(?:\s+(?:static|sealed|override|async|new|partial))*\s+[\w<>?\[\],.\s]+?\s(\w+)\s*\(/;
const keywords = new Set([
  "if", "for", "foreach", "while", "switch", "using", "lock", "return", "new",
  "catch", "get", "set", "DotaGcBackend",
]);

function classifyMethod(name) {
  if (/^Build/.test(name) || /Response$/.test(name)) return "response-builder";
  if (
    /^(Field|Write)/.test(name) ||
    ["Concat", "Encode", "Result", "Reply", "ReplyEmpty", "Proto", "Raw",
      "MessageWithTargetJob", "MessageWithTargetJobString",
      "ClientWelcomeResponse", "GameSoCacheSubscribed", "EconSoCacheSubscribed",
      "ProfileCardUpdate", "ConnectionStatus", "ConnectionStatusHaveSession",
    ].includes(name)
  ) {
    return "response-builder";
  }
  if (/^(Read|TryRead|Parse|Skip|Decode|ToX16)/.test(name) || name === "FieldCount") return "wire-codec";
  if (/Json$/.test(name) || /^DotaLobbyInvite/.test(name) ||
    ["DotaUserExists", "DotaUserOnline", "DotaResolveItemDef", "DotaInventoryVersionString",
      "DotaGetActiveMatchJson", "GetStatsProfile", "StatsStoreOrThrow", "PartyStoreOrThrow",
    ].includes(name)
  ) {
    return "data-access";
  }
  if (/^Queue/.test(name) ||
    ["Poll", "ExpireInactiveSessions", "ExpireInactiveSessionsLocked", "NextObjectIdString",
      "Ignore", "NotHandled", "Standard",
    ].includes(name)
  ) {
    return "host-plumbing";
  }
  if (/^(DotaStartDedicatedServer|DotaClaimDedicatedServer|DotaReleaseDedicatedServer|DotaDedicatedServer|DotaResolveGameServerConnectIp|DotaGetHostConnectIps|DotaRequestGameServerChange)/.test(name)) {
    return "data-access";
  }
  return "gc-logic";
}

const inventory = { $comment: "Generated by gc-js/tools/gen-inventory.mjs. Buckets: host-plumbing (stays C#), data-access (stays C#, re-exposed as typed DTOs), gc-logic (migrates to JS), response-builder (migrates to JS; forbidden for migrated ids), wire-codec (hand protobuf codec; deleted in Fase 5, replaced by Proto.*), fixture-replay (dev/test only).", files: {}, summary: {} };
const quarantine = new Set();
const counts = {};
for (const file of backendFiles) {
  const text = fs.readFileSync(path.join(gcDir, file), "utf8");
  const members = [];
  for (const line of text.split(/\r?\n/)) {
    const m = line.match(methodRegex);
    if (!m) continue;
    const name = m[1];
    if (keywords.has(name)) continue;
    const bucket = classifyMethod(name);
    members.push({ name, bucket });
    counts[bucket] = (counts[bucket] ?? 0) + 1;
    if (bucket === "response-builder" || bucket === "fixture-replay") quarantine.add(name);
  }
  inventory.files[file] = members;
}
inventory.summary = counts;

// The Lua-side fixture replay entry points are quarantined too.
quarantine.add("Fixture"); // LuaGameCoordinatorRuntime.Fixture / runtime.Fixture
quarantine.add("observed_fixture"); // routes.generated.lua helper

fs.mkdirSync(path.join(outDir, "inventory"), { recursive: true });
fs.writeFileSync(
  path.join(outDir, "inventory", "dotagcbackend-inventory.json"),
  JSON.stringify(inventory, null, 2) + "\n",
);
fs.writeFileSync(
  path.join(outDir, "quarantine.json"),
  JSON.stringify(
    {
      $comment:
        "Generated by gc-js/tools/gen-inventory.mjs. Names of C#/Lua response-builder and fixture-replay members that migrated message routes must never call. Enforced by gc-js/tools/check-gates.mjs.",
      forbidden: [...quarantine].sort(),
    },
    null,
    2,
  ) + "\n",
);

console.log(`messages.lua ids: ${msgByName.size}`);
console.log(`OBSERVED client ids: ${observed.client.size}, server ids: ${observed.server.size}, total: ${observed.client.size + observed.server.size}`);
console.log(`Lua-handled request ids: ${handledIds.size}`);
console.log(`DotaGcBackend member classification:`, counts);
console.log(`quarantine entries: ${quarantine.size}`);

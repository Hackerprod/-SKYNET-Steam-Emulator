# Game Coordinator Scripts

Game Coordinator behavior lives in this folder, separated by app id. The current
development model is TypeScript-first: game-specific message handling belongs in
`GC/<appid>/main.ts` and its modules, while the native DLL stays generic and only
forwards Game Coordinator traffic to the server.

```text
GC/
  570/
    main.ts
    framework/
      gc.ts
    modules/
      Auth.ts
      Items.ts
      Profile.ts
      Social.ts
      Stats.ts
      ...
    contracts/
      routes.json
      extra-message-ids.json
    generated/
      dota.ts
    runtime-globals.d.ts
    package.json
    tsconfig.json
```

`GC/570` is the Dota 2 coordinator. New work should target the TypeScript
pipeline unless a task explicitly says otherwise.

## Runtime Flow

At runtime the flow is:

1. The client or dedicated server sends a GC message through the emulator DLL.
2. The server receives it through the Game Coordinator exchange endpoint.
3. `GameCoordinatorScriptPlugin` locates the app script folder, compiles the
   TypeScript source with TypeSharp, and caches the runtime.
4. `main.ts` exports `handle()`, which calls `gc.dispatch()`.
5. `gc.dispatch()` looks up the registered handler by message id.
6. For typed routes, the framework decodes the request protobuf into
   `ctx.request`.
7. The handler reads data, updates state when needed, and replies with
   `ctx.reply(...)` or `ctx.send(...)`.
8. The server returns the queued response messages to the DLL.

The script cache is invalidated when files under the app folder change. That
keeps the development loop hot: edit TypeScript, send the next GC message, and
the server reloads the script.

## Entry Point

`GC/570/main.ts` is intentionally small. It imports module registration
functions, registers all handlers once, and exports `handle()`:

```ts
import { gc } from "./framework/gc";
import { registerAuth } from "./modules/Auth";
import { registerSocial } from "./modules/Social";

registerAuth();
registerSocial();

export async function handle(): Promise<boolean> {
    return await gc.dispatch();
}
```

A module should expose a `registerX()` function and keep the message logic in a
small class or focused functions:

```ts
import { gc, HandlerContext } from "../framework/gc";
import { CMsgSomeRequest, CMsgSomeResponse, Routes } from "../generated/dota";

export function registerExample(): void {
    const example = new Example();
    example.register();
}

class Example {
    register(): void {
        gc.on(Routes.SomeRoute, ctx => this.someRoute(ctx));
    }

    private someRoute(ctx: HandlerContext<CMsgSomeRequest, CMsgSomeResponse>): boolean {
        ctx.reply({
            result: 0
        });
        return true;
    }
}
```

## What `ctx` Is

`ctx` is the per-message handler context created by `framework/gc.ts`. It is the
main API a TypeScript developer should use inside business modules.

For `gc.on(Routes.X, handler)`, `ctx` is a typed
`HandlerContext<TRequest, TResponse>`:

- `ctx.route`: the generated route descriptor.
- `ctx.request`: decoded protobuf request, strongly typed from
  `generated/dota.ts`.
- `ctx.steamId`: current user SteamID as `bigint`.
- `ctx.accountId`: current Dota account id as `number`.
- `ctx.personaName`: current user display name.
- `ctx.services`: typed server-backed services for data access and state
  changes.
- `ctx.clock.now()`: current Unix timestamp in seconds.
- `ctx.logger.info(message)`: GC script logging.
- `ctx.signal`: cancellation surface for future async work.
- `ctx.reply(response)`: encode and send the route response type.
- `ctx.send(messageType, proto, message)`: send another protobuf message.
- `ctx.encode(proto, message)`: encode a nested protobuf payload to
  `Uint8Array`.

For messages that do not have a generated request/response route yet,
`gc.onMessage(id, handler)` creates a `RawMessageContext`. It has the same
identity, service, logging and sending helpers, plus:

- `ctx.messageType`: incoming message id.
- `ctx.payload`: raw request body as `Uint8Array`.
- `ctx.decode(proto)`: decode the payload manually with a generated protobuf
  descriptor.

Prefer `gc.on(Routes.X, ...)` over `gc.onMessage(...)`. Raw handlers are a
temporary bridge while a protobuf route is missing or being investigated.

## Services

Business modules should not call host globals such as `send`, `decode`, `body`,
`messageType`, or `now` directly. Use `ctx` and `ctx.services` instead.

Current services include:

- `ctx.services.items`
  - `getInventory()`
  - `getCatalogItem(defIndex)`
  - `equipItem(itemId, heroId, slotId, style)`
  - `setItemStyle(itemId, style)`
- `ctx.services.lobby`
  - `queueMessage(steamId, messageType, payload, protobuf)`
  - `publishSnapshot(snapshot)`
  - `removeSnapshot(lobbyId)`
  - `startDedicatedServer(lobbyId, map)`
  - `releaseDedicatedServer(lobbyId, reason)`
  - `resolveGameServerConnectIp(publicIp, privateIp, fallbackIp)`
  - `resolveGameServerConnectIps(publicIp, privateIp, fallbackIp)`
- `ctx.services.teams`
  - `get(teamId)`
  - `getForAccount(accountId)`
- `ctx.services.profiles`
  - `get(accountId)`
  - `saveCardSlots(slots)`
  - `saveProfileUpdate(backgroundItemId, featuredHeroIds)`
  - `getConductScorecard()`
  - `getQuestProgress(questIds)`
  - `getPeriodicResource(accountId, resourceId)`
  - `getHeroStickers()`
  - `setHeroSticker(heroId, itemId)`
  - `getOverworldState(overworldId)`
  - `getMonsterHunterState()`
- `ctx.services.social`
  - `emoticonAccess()`
  - `feed(accountId, selfOnly)`
  - `comments(feedEventId)`
  - `postComment(feedEventId, comment)`
  - `postMatchComment(matchId, comment)`
- `ctx.services.chat`
  - `join(channelName, channelType)`
  - `get(channelId)`
  - `leave(channelId)`
  - `broadcast(channelId, messageType, payload, includeSelf)`
- `ctx.services.guilds`
  - `ensureCurrent()`
  - `getMembership(accountId)`
  - `getGuild(guildId)`
  - `getPersonaInfo(accountId)`
  - `getEventData(guildId, eventId)`
  - `getReporterUpdates()`
  - `acknowledgeReporterUpdates(matchIds)`
- `ctx.services.match`
  - `recordSignOutPermission(request)`
  - `setHistoryAccess(allow)`
  - `recordServerStatus(response)`
  - `recordLeaver(event)`
  - `recordRealtimeStats(snapshot)`
  - `recordMatchStateHistory(history)`
  - `recordSpectatorCount(spectatorCount)`
  - `recordLiveScoreboard(snapshot)`
  - `savePlayerReport(report)`
- `ctx.services.party`
  - `getCurrent()`
  - `getById(partyId)`
  - `ensureCurrent(ping)`
  - `addMember(partyId, ping, isCoach)`
  - `removeMember(partyId, steamId)`
  - `deleteParty(partyId)`
  - `setLeader(partyId, leaderSteamId)`
  - `setMemberCoach(partyId, steamId, isCoach)`
  - `setMemberPing(partyId, steamId, ping)`
  - `startReadyCheck(partyId, durationSeconds)`
  - `acknowledgeReadyCheck(partyId, readyStatus)`
  - `createInvite(partyId, targetSteamId, teamId, asCoach)`
  - `takeInvite(partyId)`
  - `getInvitesForTarget(targetSteamId)`
  - `deleteInvitesForTarget(targetSteamId)`
  - `deleteInvitesForParty(partyId)`
  - `pruneInvitesCreatedBefore(cutoff)`
  - `userExists(steamId)`
  - `userOnline(steamId)`
  - `queueMessage(steamId, messageType, payload, protobuf)`
- `ctx.services.stats`
  - `lookupAccountName(accountId)`
  - `getEventPoints(accountId, eventId)`
  - `getHeroStandings(accountId)`
  - `getHeroGlobalData(accountId, heroId)`
  - `getPlayerStats(accountId)`
  - `getRank(accountId)`
  - `getTeammateStats(accountId)`
  - `getMatchHistory(accountId, startAtMatchId, requested, heroId, includePractice)`
  - `getMatchDetails(matchId)`
  - `getHeroStatsHistory(accountId, heroId)`
  - `getShowcaseStats(accountId)`
  - `getRecentAccomplishments(accountId)`
  - `getHeroRecentAccomplishments(accountId, heroId)`
  - `hasMvpVote(matchId)`
  - `voteForMvp(matchId, votedAccountId)`
  - `finalizeMvpVote(matchId)`
  - `submitLobbyMvpVote(targetAccountId)`
  - `recordSignOutMvpStats(matchId, players)`
  - `rerollPlayerChallenge()`

The services are implemented in the C# host, but TypeScript modules should see
plain TypeScript-friendly DTOs: `number`, `bigint`, `boolean`, `string`,
`Uint8Array`, arrays, and objects. Do not pass C# domain objects directly into
script code.

If a module needs new server data, add a typed service method to
`framework/gc.ts`, add the host global declaration in `runtime-globals.d.ts`,
and implement a C# host function in `GameCoordinatorScriptPlugin`. Keep the
state/data access in C# services and keep the message decision logic in
TypeScript.

## Responding To A Call

Use `ctx.reply(...)` for the normal route response:

```ts
gc.on(Routes.RequestSocialFeed, ctx => {
    const events = ctx.services.social.feed(ctx.request.accountId ?? ctx.accountId, ctx.request.selfOnly ?? false);

    ctx.reply({
        result: CMsgSocialFeedResponse_Result.Success,
        feedEvents: events.map(event => ({
            feedEventId: event.feedEventId,
            accountId: event.accountId,
            timestamp: event.timestamp,
            commentCount: event.commentCount
        }))
    });

    return true;
});
```

Use `ctx.send(...)` when the flow requires additional messages, such as a
connection status message before or after a main response:

```ts
ctx.send(Msg.GCClientConnectionStatus, Proto.CMsgConnectionStatus, {
    status: GCConnectionStatus.HaveSession,
    clientSessionNeed: sessionNeed
});
```

Return values matter:

- `true` or `void`: the message was handled.
- `false`: the message is intentionally unhandled and can fall back to another
  handler path or be logged as unhandled.
- `Promise<boolean | void>`: supported; `handle()` awaits `gc.dispatch()`.

## Adding A New Typed Handler

Use this workflow when migrating a message or adding a new response:

1. Confirm the protobuf request and response types exist under
   `SKYNET server/Services/GameCoordinator/Generated`.
2. Add a route to `GC/570/contracts/routes.json`:

   ```json
   {
       "name": "SomeRoute",
       "requestMessage": "ClientToGCSomeRequest",
       "requestProto": "CMsgSomeRequest",
       "responseMessage": "GCToClientSomeResponse",
       "responseProto": "CMsgSomeResponse"
   }
   ```

3. Regenerate TypeScript contracts:

   ```powershell
   powershell -ExecutionPolicy Bypass -File "SKYNET server\GC\570\tools\generate-dota-contracts.ps1"
   ```

4. Register the route in the appropriate module with `gc.on(Routes.SomeRoute,
   ...)`.
5. Use `ctx.request`, `ctx.services`, `ctx.reply`, `ctx.send`, and generated
   enums/types from `generated/dota.ts`.
6. Add the route to `GcScriptSelfCheck` if it should always be covered by the
   script runtime.
7. Run validation.

## Validation

For TypeScript-only GC changes:

```powershell
Push-Location "SKYNET server\GC\570"
npm test
Pop-Location

dotnet build "SKYNET server\SKYNET server.csproj" -c Debug --no-restore /nodeReuse:false
dotnet run --project "SKYNET server\SKYNET server.csproj" -c Debug --no-build -- --verify-gc-ts
```

`npm test` runs:

- TypeScript type checking.
- ESLint.
- Prettier check.
- Boundary checks that prevent business modules from bypassing the framework.

`--verify-gc-ts` performs an exchange-level smoke test against known Dota GC
messages and verifies that the expected protobuf responses are emitted.

## Local Tools

Tools currently used by the TypeScript GC workflow:

- `SKYNET server/GC/570/tools/generate-dota-contracts.ps1`: regenerates `generated/dota.ts` from route contracts and server protobufs.
- `SKYNET server/GC/570/tools/verify-gc-ts-boundaries.ps1`: checks that business modules go through `ctx`/services instead of bypassing the framework.
- `DeveloperTools/NetHookGcJson`: decodes NetHook GC captures into reports, timelines, message indexes, job correlations, and per-record JSON.
- `DeveloperTools/DecodeGcBody`: decodes an individual GC message body when focused inspection is faster than a whole-capture run.

`Tools/` may contain local binaries such as NetHook2, SDK drops, and helper bundles. It is ignored by git; document reusable commands here or in skills, not by relying on local binaries being committed.

## Rules For Good Handlers

- Do not hardcode raw protobuf bytes in TypeScript.
- Do not build protobuf payloads by hand when a generated type exists.
- Do not call host globals directly from business modules.
- Do not add C# bridge logic that owns GC business decisions just to make a TS
  handler pass.
- Keep C# host functions as typed data/service boundaries.
- Keep per-message logic readable in TypeScript.
- Keep request parsing and response construction close to the handler.
- Prefer generated enum constants over magic numbers.
- Add a real data service when data is dynamic; do not create fixtures for
  player profiles, social feed, inventory, stats, lobbies, or matches.

## Async And Tick

Handlers can be async:

```ts
gc.on(Routes.SomeRoute, async ctx => {
    const profile = ctx.services.profiles.get(ctx.accountId);
    ctx.reply({ profile });
    return true;
});
```

If `main.ts` exports `tick()`, the server calls it on a fixed interval
(`GameCoordinator:TickIntervalMs` in appsettings, default 1000ms, clamped to
100..60000). Use ticks for coordinator timers and proactive messages. During a
tick there is no request body, so do not read `ctx.request`; track recipients in
server state or explicit services.

## GC Console

`/Admin/GcConsole` shows live GC traffic:

- incoming messages
- replies
- queued async pushes
- script logs
- unhandled messages
- script errors

Keep it open while implementing a flow. It is the fastest way to confirm whether
Dota sent the message you expected and whether the TypeScript runtime handled it.

## Capture Workflow

Captures are evidence, not implementation. When a message flow is unclear:

1. Capture a clean Steam/Dota session with NetHook2.
2. Inspect it with NetHookAnalyzer.
3. Identify message ids, protobuf types, response order and required fields.
4. Update protobuf contracts if needed.
5. Implement a typed handler in TypeScript.

Keep raw captures outside the repository. They are research material only; do
not copy binary fixture behavior into the TypeScript runtime.

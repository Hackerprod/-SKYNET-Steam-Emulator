# Game Coordinator Scripts

Game Coordinator behavior is AppID-aware. Each app owns its own folder under
`GC/<appId>/` and declares its runtime contract in `gc.json`. The native server
bridge is generic: it discovers the app by AppID, loads that app manifest,
compiles that app's TypeScript entry point, and exposes only the host-service
surface requested by the manifest.

```text
GC/
  tools/
    generate-contracts.ps1
    GcTsContractGenerator/
  <appId>/
    gc.json
    main.ts
    framework/
    modules/
    contracts/
      routes.json
      extra-message-ids.json
    generated/
    runtime-globals.d.ts
    package.json
    tsconfig.json
```

`GC/570` is the Dota 2 coordinator implementation. Use it as an app-specific
example, not as a server-wide assumption.

## App Manifest

Every app may define `gc.json`:

```json
{
    "appId": 570,
    "name": "Dota 2",
    "entryPoint": "main.ts",
    "hostServices": ["dota"],
    "protoContracts": {
        "sources": [
            {
                "assembly": "server"
            }
        ]
    },
    "typeScript": {
        "generatedContracts": "generated/dota.ts",
        "extraMessageIds": "contracts/extra-message-ids.json",
        "routes": "contracts/routes.json"
    }
}
```

- `entryPoint` is relative to the app folder.
- `hostServices` opts into game-specific host APIs. Unknown services fail fast
  so a misconfigured app cannot silently run with missing globals.
- `protoContracts.sources` controls which protobuf contract assemblies/types are
  visible to that app. Runtime type names are resolved per AppID.
- `typeScript` tells the shared generator where to read route metadata and where
  to write generated TypeScript contracts.

If `gc.json` is absent, the catalog falls back to `main.ts` and no game-specific
host service. That keeps simple test apps possible without Dota coupling.

## Runtime Flow

1. The client or dedicated server sends a GC message through the emulator DLL.
2. The server receives it through the Game Coordinator exchange endpoint.
3. `GameCoordinatorScriptPlugin` resolves `GC/<appId>/gc.json` and loads only
   that app's TypeScript runtime.
4. The plugin configures a protobuf registry for that AppID. Duplicate simple
   protobuf names are treated as ambiguous; scripts should use generated
   canonical descriptors.
5. The app entry point exports `handle()`, usually delegating to `gc.dispatch()`.
6. The framework decodes typed routes into `ctx.request` and exposes identity,
   clock, logger, reply/send helpers, and app services.
7. The handler updates state and replies with `ctx.reply(...)` or emits extra
   messages with `ctx.send(...)`.
8. The server returns the queued response messages to the DLL.

The script cache is invalidated when the app source or manifest changes. For
TypeScript GC work, edit the app files and send the next GC message; the server
reloads the runtime without restarting the game or server unless state cleanup is
needed.

## Entry Point

An app entry point should be intentionally small:

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

A module should expose a `registerX()` function and keep message logic in focused
classes or functions.

## Handler Context

For generated routes, prefer `gc.on(Routes.X, handler)` over raw message
handlers. `HandlerContext<TRequest, TResponse>` provides:

- `ctx.route` and `ctx.request` for the typed route.
- `ctx.steamId`, `ctx.accountId`, `ctx.personaName`.
- `ctx.services` for app-specific server data and state changes.
- `ctx.clock.now()` and `ctx.logger.info(message)`.
- `ctx.reply(response)` for the route response.
- `ctx.send(messageType, proto, message)` for extra protobuf messages.
- `ctx.encode(proto, message)` for nested protobuf payloads.

Use `gc.onMessage(id, handler)` only while investigating a route that does not
have generated request/response descriptors yet.

## Services

Business modules should not call host globals such as `send`, `decode`, `body`,
`messageType`, or `now` directly. Use `ctx` and `ctx.services` instead.

Host globals are the native boundary. If a module needs new server data, add a
TypeScript-friendly service method to `framework/gc.ts`, declare the host global
in `runtime-globals.d.ts`, and implement the C# host function. Keep data access
in C# services and keep message decisions in TypeScript.

Dota currently exposes services for inventory, lobby, teams, profiles, social,
chat, guilds, match, party, and stats. Those services are Dota host-service
surface, not generic server globals.

## Adding A Typed Handler

1. Confirm the protobuf request and response types exist in the app's manifest
   contract sources.
2. Add a route to `GC/<appId>/contracts/routes.json`.
3. Regenerate TypeScript contracts:

   ```powershell
   powershell -ExecutionPolicy Bypass -File "SKYNET server/GC/tools/generate-contracts.ps1" -AppId <appId>
   ```

4. Register the route in the appropriate module with `gc.on(Routes.SomeRoute,
   ...)`.
5. Use `ctx.request`, `ctx.services`, `ctx.reply`, `ctx.send`, and generated
   enums/types.
6. Add the route to app-specific self-check coverage when it must always be
   supported.
7. Run validation.

## Validation

For TypeScript-only Dota GC changes:

```powershell
Push-Location "SKYNET server/GC/570"
npm test
Pop-Location

dotnet build "SKYNET server/SKYNET server.csproj" -c Debug --no-restore /nodeReuse:false
dotnet run --project "SKYNET server/SKYNET server.csproj" -c Debug --no-build -- --verify-gc-ts
```

`npm test` runs type checking, ESLint, Prettier, and boundary checks. The
exchange self-check validates known Dota GC messages against the TypeSharp
runtime.

## Local Tools

- `SKYNET server/GC/tools/generate-contracts.ps1`: regenerates an app's generated
  TypeScript contracts from its manifest, message ids, route definitions, and
  protobuf contract sources.
- `SKYNET server/GC/<appId>/tools/verify-gc-ts-boundaries.ps1`: app-specific
  boundary checks for TypeScript modules.
- `DeveloperTools/NetHookGcJson`: decodes NetHook GC captures into reports,
  timelines, message indexes, job correlations, and per-record JSON.
- `DeveloperTools/DecodeGcBody`: decodes an individual GC message body when
  focused inspection is faster than a whole-capture run.

Keep raw captures and local binary tools outside the repository unless they are
small, reusable source artifacts.

## Rules For Good Handlers

- Do not hardcode raw protobuf bytes in TypeScript.
- Do not build protobuf payloads by hand when a generated type exists.
- Do not call host globals directly from business modules.
- Do not add C# bridge logic that owns GC business decisions just to make a TS
  handler pass.
- Keep C# host functions as typed data/service boundaries.
- Keep per-message logic readable in TypeScript.
- Prefer generated enum constants over magic numbers.
- Add a real data service when data is dynamic; do not create fixtures for
  player profiles, social feed, inventory, stats, lobbies, or matches.

## Async And Tick

Handlers can be async. If `main.ts` exports `tick()`, the server calls it on the
configured GC tick interval. During a tick there is no request body, so do not
read `ctx.request`; track recipients in server state or explicit services.

## GC Console

`/Admin/GcConsole` shows incoming messages, replies, queued pushes, script logs,
unhandled messages, and script errors. Keep it open while implementing a flow.

## Capture Workflow

Captures are evidence, not implementation. When a message flow is unclear:

1. Capture a clean Steam/game session with NetHook2.
2. Inspect it with NetHookAnalyzer or the JSON dump tools.
3. Identify message ids, protobuf types, response order, and required fields.
4. Update protobuf contracts if needed.
5. Implement a typed handler in TypeScript.

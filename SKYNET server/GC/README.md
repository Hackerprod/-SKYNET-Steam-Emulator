# Game Coordinator plugins

The DLL must stay generic. Game-specific GC behavior lives here, under one
folder per Steam AppID:

```text
GC/
  570/
    main.lua
    messages.lua
    fixtures/
    captures/
```

`main.lua` (and every `.lua` it includes) is compiled once and cached; any edit
to a `.lua` file under the app folder triggers a reload on the next message, so
changes are hot-reloaded while the server and the game stay running.
The server exposes two globals to Lua:

- `gc`: current exchange context and reply helpers.
- `runtime`: fixture, protobuf, state and utility helpers.

For AppID `570`, `gc` is backed by `DotaGcBackend` while the updated protobuf
surface is still being migrated. For other AppIDs, `gc` is a generic backend
that can reply with captured fixtures or manually encoded payloads.

## Lua flow

```lua
include("messages.lua")

function handle()
    if gc.MessageType == MSG.SomeRequest then
        return gc.Reply(MSG.SomeResponse, runtime.Fixture("fixtures/server_1234_0001.bin"))
    end

    return false
end
```

Returning `false` makes the DLL log the unhandled message. Returning `true`
acknowledges it.

## Lua API surface

Request parsing (`gc`, operates on the incoming message body):

- `gc.ReadVarint(field, default)` / `gc.ReadVarintAt(field, occurrence, default)`
- `gc.ReadString(field)` / `gc.ReadStringAt(field, occurrence)`
- `gc.ReadBytes(field)` / `gc.ReadBytesAt(field, occurrence)` (base64)
- `gc.FieldCount(field)` counts repeated-field occurrences.

Payload building (`gc`, all return base64 you can nest with `Concat`):

- `gc.FieldVarint(field, value)`, `gc.FieldFixed64(field, value)`
- `gc.FieldString(field, text)`, `gc.FieldBytes(field, payloadBase64)`
- `gc.Concat(a, b, ...)`, `gc.Result(code)`

Nested payload parsing (`runtime`, operates on any base64 payload, e.g. the
result of `gc.ReadBytes` for an embedded message):

- `runtime.ProtoVarint(payload, field, occurrence, default)`
- `runtime.ProtoString(payload, field, occurrence)`
- `runtime.ProtoBytes(payload, field, occurrence)`
- `runtime.ProtoCount(payload, field)`

Persistent state (survives script reloads; values are strings — serialize
tables yourself):

- `runtime.SessionGet/SessionSet/SessionDelete(key)` scoped per (AppId, SteamId).
- `runtime.StoreGet/StoreSet/StoreDelete(scope, key)` for app-wide state.

Async delivery:

- `gc.QueueTo(steamId, msgType, payloadBase64, protobuf)` pushes to a client
  through the `/api/events` channel (`gc_message` events).
- `gc.QueueToPoll(steamId, msgType, payloadBase64, protobuf)` targets a game
  server; game servers do not run the event pump and drain
  `/gamecoordinator/poll` instead. Do not use both for the same recipient or the
  message is delivered twice.

## tick()

If the script defines a global `tick()` function, the server calls it on a
fixed interval (`GameCoordinator:TickIntervalMs` in appsettings, default
1000ms, clamped to 100..60000). Ticking starts after the app's first GC
exchange loads the script. During a tick `gc` has no request body
(`MessageType` is 0, `SteamId` is 0) — use it for timers and proactive pushes
via `gc.QueueTo`, tracking recipients yourself in `runtime.Store*`.

```lua
function tick()
    local ids = runtime.StoreGet("app", "known_players")
    for steamId in string.gmatch(ids, "[^,]+") do
        -- e.g. periodic GCClientConnectionStatus
    end
end
```

## GC Console

`/Admin/GcConsole` (admin login required) shows live GC traffic: incoming
messages, replies, async pushes, `runtime.Log` output, unhandled messages and
script errors. Keep it open next to the game while editing Lua to close the
edit-and-see loop.

## NetHook workflow

1. Capture a clean Steam/Dota session with NetHook2.
2. Export or inspect it with NetHookAnalyzer.
3. Import the capture into this plugin folder:

```powershell
.\SKYNET server\GC\tools\Import-NetHook.ps1 -InputPath "D:\Path\To\NetHookDump" -AppId 570
```

The importer creates:

- `GC/570/captures/nethook_*.json`: normalized capture index.
- `GC/570/fixtures/nethook/*.bin`: replayable payload bodies.
- `GC/570/routes.generated.lua`: observed request/response fixture index.

Review `routes.generated.lua` before including it from `main.lua`; captures are
evidence, not automatically trusted behavior.


---
name: skynet-dota-gc
description: Implement, maintain, reverse-engineer, compare, and validate Dota 2 Game Coordinator behavior in SKYNET server-side GC scripts/protobufs, especially lobby, inventory/SO caches, profile, welcome/login, matchmaking, dedicated/listen server routing, GC captures, NetHook comparisons, and TypeScript coordinator hot reload. Use for Dota GC protocol logic, not Steam API DLL internals or generic server hosting.
---

# SKYNET Dota GC

Use this skill for Dota 2 Game Coordinator behavior implemented in server-side TypeScript and related protobuf data.

## Environment

Before using any path, command, port, launch target, log location, capture file location, or tool location, read the shared editable environment file at `../skynet-environment.md`.

That file is the source of truth for local paths and commands. If the current machine differs, update that file instead of embedding paths in this skill.

If capture tooling details are needed, also read `references/tools-and-capture.md`.

## Responsibility

Own:

- Dota GC TypeScript modules and message handlers.
- Protobuf message shape, enum usage, request/response payloads, and GC routing.
- Welcome/login flow, profile/player data, behavior score, lobbies, lobby launch, matchmaking, practice/local/dedicated server handoff, inventory, SO caches, hero cosmetics, global items, and econ item equip state.
- NetHook capture comparison and old coordinator comparison when reconstructing behavior.

Do not own:

- Steam API DLL internals or generic Steamworks implementation. Use `skynet-steam-api-dll`.
- Generic server hosting, API routes, port binding, launcher config, or process lifecycle. Use `skynet-server-maintenance`.

## TypeScript Runtime

GC logic lives in TypeScript and is designed for hot iteration. For TypeScript-only GC edits, do not restart the server or Dota by default.

Use the runtime hot-reload behavior, then clear stale sessions, lobbies, cached SO state, or player data only when that state prevents a clean validation. Restart the server only when the host/runtime, dependency loading, generated code, or non-hot-reloadable server layer changes.

Use C# only when strictly necessary to improve the generic runtime or host contract. Do not implement Dota-specific GC behavior in C#.

## Workflow

1. Read `../skynet-environment.md`.
2. Inspect the active TypeScript GC module, protobuf definitions, generated bindings, and logs for the exact message flow.
3. Compare against known-good legacy coordinator behavior or real NetHook captures when message shape is unclear. Treat old Lua only as historical behavior evidence, not as active runtime guidance.
4. Preserve protocol completeness. Do not omit fields just because legacy code omitted them; populate required/defaultable fields coherently.
5. Route messages to the correct recipient and SteamID instance. Dota uses different recipients for client, lobby owner, members, game server, and dedicated server lifecycle messages.
6. Keep lobby/server selection state persistent and data-driven. Do not hardcode region/local behavior.
7. Inventory and cosmetics must publish the data Dota expects in-game:
   - ownership/econ data must exist before equip state is announced;
   - equipped hero items and global items are different concepts and both need their expected SO/update flow;
   - item definitions, styles, sockets, effects, and equip slots must be complete enough for the client to render and equip them;
   - in-game validation must check both hero equipment and global effects.
8. Behavior score starts from the configured baseline and changes through reports, recommendations, or stored player state. Do not invent fixed behavior values in handlers.
9. When a GC issue is resolved, document the working flow in code comments near the relevant handler or state builder. Describe how the valid flow works, not the broken symptom.

## Capture And Comparison

Use captures when local code and legacy behavior disagree or when Dota silently ignores a message:

- Restore the original Steam API DLL according to the environment file before capturing real Steam behavior.
- Capture the same user action in real Steam/Dota.
- Decode and compare message IDs, proto fields, recipient SteamIDs, routing job IDs, and ordering.
- Restore the emulator DLL before emulator validation.
- Keep captured evidence summarized in notes or comments when it explains the implemented flow.

## Validation

For GC changes, complete as much of this as applies:

- Run the configured TypeScript/protobuf validation commands.
- Exercise the exact GC flow through the emulator.
- Launch Dota through the launcher command from the environment file, with launch parameters, when gameplay validation is required.
- Verify logs show the expected request, response, SO update, lobby update, or server handoff.
- For item work, visually validate that items are listed, equipped, and visible in-game, including global effects when relevant.
- For lobby/server work, validate both local and selected-region paths when the change can affect either.

Final response must state what was changed, what validation ran, what was visually checked, and what remains unverified.

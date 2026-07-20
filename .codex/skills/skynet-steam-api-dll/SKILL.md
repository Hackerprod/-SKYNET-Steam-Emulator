---
name: skynet-steam-api-dll
description: Build, deploy, package, and validate the SKYNET Steam API emulator DLL (`steam_api`, `steam_api64.dll`, Steamworks interfaces, callbacks, auth/session transport, networking, remote storage, friends, avatars, and generic runtime DLL behavior). Use when changing or debugging DLL-side code, launcher payload DLLs, injected Steamworks behavior, or Dota-visible emulator DLL runtime issues.
---

# SKYNET Steam API DLL

Use this skill only for the native/managed Steam API emulator DLL layer and its packaged payloads.

## Environment

Before using any path, command, port, launch target, log location, or tool location, read the shared editable environment file at `../skynet-environment.md`.

That file is the source of truth for local paths and commands. If the current machine differs, update that file instead of embedding paths in this skill.

## Responsibility

Own:

- `steam_api` and `steam_api64.dll` implementation.
- Steamworks interface behavior exposed by the emulator DLL.
- DLL callbacks, call results, auth/session transport, sockets, remote storage, friends, avatars, and generic runtime plumbing.
- DLL build, launcher payload sync, direct-DLL fallback deployment, and DLL runtime validation.

Do not own:

- Dota-specific GC protocol behavior. Use `skynet-dota-gc`.
- Generic server/web/API behavior. Use `skynet-server-maintenance`.
- Dota gameplay data fixes inside the DLL unless the issue is proven to be transport or Steamworks surface behavior.

## Workflow

1. Read `../skynet-environment.md`.
2. Inspect the relevant DLL code and the latest DLL/game/server logs before editing.
3. Identify whether the bug belongs to a Steamworks interface, callback/call-result flow, auth/session transport, storage, networking, or packaging/deployment.
4. Keep DLL behavior generic. If a fix would hardcode Dota GC semantics, move it to the GC TypeScript layer.
5. Build the DLL with the command from the environment file.
6. Sync the correct architecture artifact into the launcher payload. The normal launcher flow injects from payload and does not replace the game-folder DLL.
7. Deploy to the configured game-folder DLL target only when explicitly debugging direct Dota startup or capturing/restoring original behavior.
8. Verify payload or direct-DLL identity with timestamp, size, and preferably hash before launching.
9. Launch test games through the launcher command from the environment file, with launch parameters. Do not validate by starting the game executable directly. Use UI automation only after launcher-based startup when visual inspection is needed.
10. Inspect logs after launch and verify the expected Steamworks/API calls reached the server or game.

## Quality Rules

- Runtime fixes must be robust and emulator-generic, not one-off patches for the current Dota symptom.
- Preserve original game DLL backups according to the environment file before any explicit direct-DLL overwrite.
- Do not hide failures behind silent fallbacks. Log enough context to explain which API call, SteamID, app, endpoint, or DLL path failed.
- Avoid hardcoded ports, local IPs, install paths, SteamIDs, app IDs, or build tool paths in code unless the project already models them as configurable defaults.
- If generated binaries changed, report exactly which artifacts were built and deployed.

## Validation

For DLL-side changes, complete as much of this as applies:

- Build succeeds for the required architecture.
- Launcher payload matches the freshly built artifact for normal validation.
- Direct game-folder DLL deployment is verified only when that fallback path was intentionally used.
- Server connectivity is verified when the DLL uses server APIs.
- Game is launched through the launcher with configured launch parameters.
- Logs show the expected Steamworks/API flow.
- Any visual validation is based on launcher-started gameplay, not a direct executable run.

Final response must state what was changed, what was built/deployed, what was validated, and what remains unverified.

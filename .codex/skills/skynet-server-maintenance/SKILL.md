---
name: skynet-server-maintenance
description: Inspect, edit, build, restart, and validate the SKYNET emulator server, web/API endpoints, launcher/server integration, persistence, discovery, hosted services, dedicated server supervision, and non-GC server-side runtime. Use when work affects the SKYNET server process, API routes, auth/session backend, launcher configuration, payload packaging, or server logs, excluding Dota GC protocol logic and DLL internals.
---

# SKYNET Server Maintenance

Use this skill for the emulator server as a host, API, process, launcher backend, and generic runtime.

## Environment

Before using any path, command, port, launch target, log location, or tool location, read the shared editable environment file at `../skynet-environment.md`.

That file is the source of truth for local paths and commands. If the current machine differs, update that file instead of embedding paths in this skill.

## Responsibility

Own:

- Server process startup, shutdown, build, restart, and health checks.
- Web/API routes, auth/session backend, persistence, hosted services, logging, discovery, and LAN binding.
- Launcher/server integration, launcher config generation, payload packaging orchestration, and server-side launcher support.
- Database ownership split: `steam.db` owns generic Steam/server state; `dota.db` owns Dota/GC/game state.
- Dedicated server supervision when the issue is process lifecycle, launch parameters, routing host state, or logs.

Do not own:

- Steamworks DLL implementation details. Use `skynet-steam-api-dll`.
- Dota GC message semantics, inventory, lobbies, matchmaking, profile, SO caches, or protobuf behavior. Use `skynet-dota-gc`.

## Workflow

1. Read `../skynet-environment.md`.
2. Inspect current server code, process state, port binding, and logs before editing.
3. Classify the issue as server host/API/runtime, DLL, or GC. Switch skills if it belongs elsewhere.
4. For C# server changes, build the server with the environment command and restart the server process only after a successful build.
5. For TypeScript GC-only changes, do not restart server or Dota by default. Use the GC skill hot-reload workflow; reset sessions, lobbies, or cached runtime data only when stale state affects validation.
6. After server restart, verify the configured port is listening on all required interfaces, including `0.0.0.0` when LAN clients must connect.
7. Curl the configured local and LAN URLs from the environment file.
8. For launcher/server issues, verify generated config, payload source, payload target, selected server persistence, and launcher startup command.
9. Launch games through the launcher command from the environment file, with launch parameters. Do not use direct executable startup as proof that the emulator flow works.

## Quality Rules

- Keep server fixes in the correct layer. Do not move Dota GC behavior into C# unless the TypeScript host/runtime genuinely lacks a required generic capability.
- Server runtime changes must be generic and reusable, not special cases for a single GC module.
- Preserve existing user changes in the repo. Do not reset or clean unrelated files.
- Avoid hardcoded local paths, ports, IPs, SteamIDs, app IDs, region names, or machine assumptions. Put environment-specific values in the environment file or project configuration.
- When a bug is resolved, document the working flow in the relevant code comments or skill guidance without describing the failed attempt as the canonical flow.
- Do not reintroduce active `app.db`, `SteamDB.db`, or per-feature legacy Dota databases. Migration code may mention legacy inputs, but runtime should use only `steam.db` and `dota.db`.

## Validation

For server-side changes, complete as much of this as applies:

- Server build succeeds.
- Server process starts without immediate exit.
- Configured port is listening on expected interfaces.
- Local and LAN HTTP checks return the expected response.
- Launcher obtains the expected server configuration.
- Relevant logs show the expected request, session, lobby, or process lifecycle.

Final response must state what was changed, what was built/restarted, what was validated, and what remains unverified.

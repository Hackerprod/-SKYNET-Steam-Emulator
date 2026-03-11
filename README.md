# [SKYNET] Steam Emulator

`steam_api` replacement focused on emulating the Steam DLL ABI while moving game-facing state to a local backend server.

This repository is no longer documented as a client emulator. The active architecture is:

- `steam_api/`: drop-in Steam DLL replacement and ABI bridge
- `SKYNET server/`: ASP.NET Core backend that owns session, friends, stats, lobbies, storage, tickets, game server state, P2P event relay, and game coordinator message relay

## Current Architecture

The DLL is now **server-first**. Instead of keeping authority in local LAN-only state, `steam_api` resolves core behavior through `SKYNET server` over HTTP plus event polling:

- user identity and session bootstrap
- friends and persona/rich presence
- stats and achievements
- matchmaking and lobby state
- remote storage metadata and file content
- auth tickets and validation
- game server registration and stats
- P2P packet relay
- game coordinator message relay
- Steam-style callbacks fed from remote events

## Main Improvements

- Reworked callback system to separate broadcast callbacks, call results, and manual dispatch behavior.
- `ISteamGameCoordinator` queue/callback flow now follows Steam semantics more closely.
- Added newer interface coverage used by this repo, including newer Friends, UGC, Remote Play, Game Server, and Timeline exposure.
- Matchmaking now uses backend-owned lobbies instead of only local socket state.
- `RemoteStorage`, ticket/auth flow, `SteamGameServer`, `SteamGameServerStats`, P2P, and GC were moved to backend-backed paths.
- `SKYNET server` now exposes a real `/api/...` surface instead of only Razor mock pages.

## Repository Layout

```text
steam_api/        Steam DLL replacement, exported interfaces, callback manager, backend client
SKYNET server/    ASP.NET Core backend and Razor admin UI
```

## Build Notes

### `steam_api`

Requires `.NET Framework 4.7.2 Developer Pack`. If it is missing, MSBuild fails with `MSB3644`.

### `SKYNET server`

```bash
dotnet build "SKYNET server/SKYNET server.csproj"
dotnet run --project "SKYNET server/SKYNET server.csproj"
```

The default DLL config points to:

```text
http://127.0.0.1:27080/
```

## Status

This project is still an emulator and compatibility layer, not a full Steamworks implementation. The core direction is now backend-backed behavior rather than standalone LAN emulation.

## Contributions

Code contributions are welcome.

Keep pull requests focused, document the behavior being changed, and include enough detail to reproduce or validate the result. For `steam_api`, preserve Steam ABI compatibility and prefer changes that follow documented Steam behavior instead of ad-hoc shortcuts. For `SKYNET server`, keep endpoints and payloads aligned with the DLL client contract.

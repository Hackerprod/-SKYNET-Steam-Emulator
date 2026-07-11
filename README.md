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

## Identity Source

The web/server account is the authoritative identity source. When `SkyNetUseActiveWebUser = true`, the DLL asks `SKYNET server` for the active web user during Steam session creation and uses that server response for `SteamID`, account id, persona name, avatar, friends, stats, lobbies and GC context.

The local INI should only contain bootstrap/fallback values:

- `ClientInstanceId`: stable machine/client discriminator.
- `FallbackPersonaName` and `FallbackAccountId`: used only when the server is unavailable or no active web account exists.
- network/server settings.

Do not persist server-issued identity or access tokens in the INI; they are runtime session state.

## LAN Ports

- `27080/TCP`: SKYNET server HTTP/API/Razor UI.
- `27081/UDP`: SKYNET server autodiscovery. Clients broadcast `SKYNET_DISCOVER`; the server replies with its HTTP URL.
- `3333/TCP`: legacy direct P2P socket path in `P2PNetworking`.
- `28032/TCP/UDP`: legacy LAN broadcast/config path used by `NetworkManager`.

The current default remains server-first for identity, friends, stats, lobbies, storage, tickets and GC. P2P relay sends are now queued and batched off the game thread. A verified hybrid mode can later use the server as a peer directory and the legacy direct socket path for LAN gameplay packets, with HTTP relay as fallback.

## Game Coordinator Maintenance

Dota-specific coordinator behavior lives under `SKYNET server/GC/570/`; the DLL should only forward GC transport and context. Lua scripts are cached and hot-reloaded when `.lua` files change. For future NetHook maintenance, keep fixtures templated by account/session before replaying captured payloads, and regenerate `messages.lua` from current Valve/SteamKit protobuf enums instead of hand-copying IDs.

## Status

This project is still an emulator and compatibility layer, not a full Steamworks implementation. The core direction is now backend-backed behavior rather than standalone LAN emulation.

## Contributions

Code contributions are welcome.

Keep pull requests focused, document the behavior being changed, and include enough detail to reproduce or validate the result. For `steam_api`, preserve Steam ABI compatibility and prefer changes that follow documented Steam behavior instead of ad-hoc shortcuts. For `SKYNET server`, keep endpoints and payloads aligned with the DLL client contract.

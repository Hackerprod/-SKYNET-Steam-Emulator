# AGENTS.md

Guide for agents working in `C:\SERVER\SKYNET Steam Emulator`.

## Project Context

This repo combines three surfaces that often affect each other:

- `SKYNET server`: .NET backend, lobby state, HTTP routes, dedicated-server supervisor, split SQLite persistence, and TypeScript GC runtime.
- `steam_api`: Steamworks emulator built as launcher payload DLLs. Normal validation injects the payload from the launcher; it does not replace Dota's DLL in the game folder.
- Dota runtime: client and dedicated server under `C:\SERVER\Steam\steamapps\common\dota 2 beta`.

Main rule: read the real code and logs before changing code. Do not assume a Dota failure comes from certificates, GC state, or patching without evidence.

## Required Skills

Use these skills when they apply:

- `skynet-steam-api-dll`: use for `steam_api`, launcher payload DLLs, Steamworks interfaces, callbacks, auth/session transport, networking, remote storage, friends, avatars, and generic DLL runtime behavior.
- `skynet-server-maintenance`: use for server process/API/persistence/discovery/launcher integration/dedicated-server supervision that is not GC protocol logic or DLL internals.
- `skynet-dota-gc`: use for Dota Game Coordinator work: `SKYNET server\GC\570`, TypeScript handlers, protobuf, NetHook, captures, lobby/profile/login/stats/items/SO cache messages, and GC routing.
- `dota-computer-use`: use only after launching Dota by shell through the SKYNET launcher command with launch parameters. Use it for Dota window inspection, UI interaction, screenshots, lobby creation/start, and popup handling.
- `skill-creator`: use when creating or maintaining a local repo skill.

If a task mentions skills or clearly matches one of these domains, read the relevant `SKILL.md` file completely before acting.

## Workflow

1. Check repo state with `git status --short --untracked-files=all`.
2. Read the affected code paths and relevant logs before editing.
3. Keep changes scoped and preserve user changes. Do not use `git reset --hard` or revert files you did not change unless explicitly instructed.
4. Use `apply_patch` for manual edits.
5. Build the surface you changed.
6. Deploy only what is needed.
7. Validate with logs and, when Dota is involved, launch through the launcher command first; use Computer Use only for subsequent UI inspection and screenshots.

Usual server build command:

```powershell
dotnet build "SKYNET server\SKYNET server.csproj" -c Debug --no-restore
```

Usual `steam_api` x64 build command:

```powershell
$msbuild = "C:\Program Files (x86)\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin\amd64\MSBuild.exe"
& $msbuild "steam_api\steam_api.csproj" /t:Restore /p:Configuration=Release /p:Platform="AnyCPU" /p:RuntimeIdentifier=win-x64 "/p:SolutionDir=C:\SERVER\SKYNET Steam Emulator\\"
& $msbuild "steam_api\steam_api.csproj" /t:Rebuild /p:Configuration=Release /p:Platform="AnyCPU" /p:RuntimeIdentifier=win-x64 /p:PlatformTarget=x64 /p:DllExportPlatform=x64 /p:DllExportOurILAsm=true "/p:SolutionDir=C:\SERVER\SKYNET Steam Emulator\\" /m:1 /v:minimal
```

When touching `steam_api`, sync the fresh artifact into the launcher payload and rebuild the launcher:

```powershell
Copy-Item -LiteralPath "steam_api\bin\Release\steam_api.dll" -Destination "SKYNET Steam Client\payload\x64\steam_api64.dll" -Force
dotnet build "SKYNET Steam Client\SKYNET Steam Client.csproj" -c Debug
```

## Dota Runtime

Dota client:

```text
C:\SERVER\Steam\steamapps\common\dota 2 beta\game\bin\win64\dota2.exe
```

Recommended launcher validation command:

```powershell
& "C:\SERVER\SKYNET Steam Emulator\SKYNET Steam Client\bin\Debug\net472\SKYNET Steam Client.exe" --launch 8f48e29f623a4248ab533c63b7b0c835
```

The expected non-local lobby dedicated server should launch with a line equivalent to:

```text
-dedicated -insecure -port 27025 +tv_port 37025 +sv_lan 0 +map dota -console -vconsole -condebug -conclearlog -novid
```

For non-local lobby validation, the UI should show a server such as `US West`, not `Local lobby`.

## Key Logs

Read these logs before concluding the cause:

```text
C:\SERVER\Steam\steamapps\common\dota 2 beta\game\dota\console.log
C:\SERVER\Steam\steamapps\common\dota 2 beta\game\dota\console.*.log
C:\SERVER\Steam\steamapps\common\dota 2 beta\game\bin\win64\SKYNET\steam_api.log
C:\SERVER\SKYNET Steam Emulator\.tmp\skynet_server_stdout_*.log
C:\SERVER\Lobby problem
```

Useful markers:

```text
[SteamNetSockets] Certificate expires in
[SteamNetSockets] AuthStatus
create_lobby
assigned_match_id
launch_lobby dedicated
game_server_info claimed dedicated
attach_server endpoint
connected_players
destroy_lobby
GCToClientMatchSignedOut
player_failed_to_connect
```

## Current Diagnostic Notes

- If the dedicated server prints `[SteamNetSockets] Certificate expires in ...`, the certificate is being accepted by SteamNetSockets on that path.
- If VAC appears while the certificate is OK and the server repeats `connected_players ... connected=0`, first investigate why the client did not enter the dedicated server or why lobby/match state did not advance.
- Do not destroy the lobby on `game_state == 7` without additional evidence. Current Dota versions may use that state during the active flow, not only as a disconnect.
- Public-key patching may already be handled on disk if the user patched it manually. Do not reintroduce in-memory patching without a concrete reason.

## Git And Local Files

The worktree may be dirty because of user work or previous agents. Do not revert changes you did not make.

`.codex/` is ignored by git in this repo. Local skills under `.codex\skills` are not uploaded unless the versioning strategy is explicitly changed.

`AGENTS.md` is also ignored by the current `.gitignore`. Keep that in mind if the user wants this guide committed or pushed.

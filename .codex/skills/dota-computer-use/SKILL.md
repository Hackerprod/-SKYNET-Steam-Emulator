---
name: dota-computer-use
description: Control the local Dota 2 client with Computer Use for SKYNET Steam Emulator validation. Use when Codex needs to launch Dota, inspect the Dota window, create or start a custom lobby, test non-local dedicated-server flow, close VAC/reconnect popups, or gather visual evidence from the Dota UI.
---

# Dota Computer Use

## Core Rules

Use this skill together with the `computer-use` skill. Read and follow `computer-use` before sending Windows input.

Do not automate Dota with PowerShell SendKeys, custom mouse scripts, or Windows Run. Use the Computer Use `sky` API for UI input and screenshots.

Use shell only for non-UI process work such as starting Dota with command-line arguments, checking processes, reading logs, building, or restarting the SKYNET server. The Computer Use `launch_app` API cannot pass Dota launch arguments.

After every UI step, verify with `sky.get_window_state({ window: targetWindow })` when the next click depends on visible state.

## Coordinate Maintenance

When a useful Dota UI coordinate is discovered, corrected, or becomes stale, update this skill before finishing the task. This is part of the validation work, not optional documentation.

For each saved coordinate, keep enough context for another agent to reuse it:

- Screenshot size, normally `1536x960`.
- UI state or precondition, such as which play panel is selected or whether a modal is open.
- Coordinate and intended action.
- Any known variant when the same control moves after another panel expands.

If a click misses because a coordinate is stale, fix or remove the stale entry instead of leaving both values without context.

## Recommended Launch

Primary validation launcher:

```text
C:\SERVER\SKYNET Steam Emulator\SKYNET Steam Client\bin\Debug\net472\SKYNET Steam Client.exe
```

Launch Dota through the SKYNET Steam Client launcher for emulator validation. This is the default path because it exercises the same startup flow, environment, payload injection path, and launch parameters that players use.

Current Dota library id in the local launcher config:

```text
8f48e29f623a4248ab533c63b7b0c835
```

Preferred automated validation command:

```powershell
$launcher = 'C:\SERVER\SKYNET Steam Emulator\SKYNET Steam Client\bin\Debug\net472\SKYNET Steam Client.exe'
& $launcher --launch 8f48e29f623a4248ab533c63b7b0c835
```

This preserves the per-game `LaunchArguments` stored in `%APPDATA%\SKYNET\client\config.json`, currently:

```text
-console -vconsole -condebug -conclearlog -novid -nojoy -dx11 +fps_max 60
```

Use the raw Dota executable only when explicitly debugging direct process startup, command-line parsing, or a launcher-independent Dota runtime problem:

```text
C:\SERVER\Steam\steamapps\common\dota 2 beta\game\bin\win64\dota2.exe
```

Recommended validation args for the client:

```text
-insecure -console -vconsole -condebug -conclearlog -novid -nojoy -dx11 -windowed +fps_max 60
```

If coordinates drift, relaunch or resize until `get_window_state` reports screenshot size:

```text
1536x960
```

The coordinates below assume a Dota screenshot of exactly `1536x960`. If a different same-aspect resolution is unavoidable, scale coordinates:

```text
x_scaled = round(x * width / 1536)
y_scaled = round(y * height / 960)
```

## Computer Use Bootstrap

Use the standard Computer Use bootstrap, then choose the Dota window from `list_apps`:

```js
globalThis.apps = await sky.list_apps();
globalThis.dotaCandidates = apps.filter(candidate =>
  /dota|dota2|570/i.test(`${candidate.id} ${candidate.displayName ?? ""}`),
);
globalThis.targetApp = dotaCandidates.find(candidate =>
  candidate.windows?.length && /dota2\.exe/i.test(candidate.id),
) ?? dotaCandidates.find(candidate => candidate.windows?.length);
globalThis.targetWindow = await sky.get_window(targetApp.windows[0]);
await sky.activate_window({ window: targetWindow });
globalThis.state = await sky.get_window_state({ window: targetWindow });
globalThis.targetWindow = state.window;
```

For normal validation, start the launcher process in headless launch mode by shell, then immediately switch back to Computer Use for Dota interaction:

```powershell
$launcher = 'C:\SERVER\SKYNET Steam Emulator\SKYNET Steam Client\bin\Debug\net472\SKYNET Steam Client.exe'
Start-Process -FilePath $launcher -ArgumentList '--launch 8f48e29f623a4248ab533c63b7b0c835' -WorkingDirectory 'C:\SERVER\SKYNET Steam Emulator\SKYNET Steam Client\bin\Debug\net472'
```

For direct Dota launch fallback with args:

```powershell
$exe = 'C:\SERVER\Steam\steamapps\common\dota 2 beta\game\bin\win64\dota2.exe'
$args = '-insecure -console -vconsole -condebug -conclearlog -novid -nojoy -dx11 -windowed +fps_max 60'
Start-Process -FilePath $exe -ArgumentList $args -WorkingDirectory 'C:\SERVER\Steam\steamapps\common\dota 2 beta\game\bin\win64'
```

## Main Coordinates

Main menu:

```text
JUGAR A DOTA: x=1340 y=920
Event COMENZAR button: x=965 y=758
Event reward error ACEPTAR: x=768 y=580
```

Play panel after clicking `JUGAR A DOTA`:

```text
SALAS PERSONALIZADAS header when another mode is selected: x=1338 y=362
SALAS PERSONALIZADAS header when already expanded or shifted down: x=1338 y=582
+ CREAR custom lobby after SALAS PERSONALIZADAS is expanded: x=1265 y=624
BUSCAR custom lobby after SALAS PERSONALIZADAS is expanded: x=1408 y=624
BUSCAR PARTIDA: x=1335 y=920
```

Custom lobby screen:

```text
INICIAR PARTIDA: x=1338 y=920
ABANDONAR SALA: x=1338 y=868
Lobby settings area: x=1218 y=708 to x=1485 y=795
Server label appears near: x=1218 y=734
Server value appears near: x=1325 y=734
```

Expected non-local validation signal:

```text
Lobby settings should show: SERVIDOR: EE. UU. OESTE
Do not validate non-local flow when it says: Partida local
```

Match/reconnect state:

```text
VAC popup CERRAR: x=895 y=555
VAC popup MAS INFORMACION: x=675 y=555
RECONECTAR: x=1335 y=905
ABANDONAR PARTIDA: x=1335 y=856
```

Observed `1229x768` window variant:

```text
Precondition: Dota launched windowed and get_window_state reports screenshot width=1229 height=768.
Main menu JUGAR A DOTA: x=1072 y=736
Conduct summary ACEPTAR modal: x=768 y=758
Play panel + CREAR under SALAS PERSONALIZADAS: x=1012 y=503
Custom lobby INICIAR PARTIDA: x=1071 y=736
Custom lobby settings server value appears near: x=1050 y=588
Hero select ALEATORIO: x=1122 y=590
VAC popup CERRAR, scaled from 1536x960 if needed: x=716 y=444
```

## Non-Local Lobby Test

1. Launch or focus Dota and wait until the main menu is visible.
2. Confirm screenshot size is `1536x960`.
3. Click `JUGAR A DOTA` at `1340,920`.
4. Click `+ CREAR` under `SALAS PERSONALIZADAS` at `1265,624`.
5. Verify the lobby settings show `SERVIDOR: EE. UU. OESTE`.
6. Click `INICIAR PARTIDA` at `1338,920`.
7. Expect a dedicated-server console window to open. It may cover the upper-left of Dota; do not close it during validation.
8. Observe the Dota UI and logs until the client connects, shows `RECONECTAR`, or shows the VAC popup.

Expected dedicated launch line:

```text
-dedicated -insecure -port 27025 +tv_port 37025 +sv_lan 0 +map dota -console -vconsole -condebug -conclearlog -novid
```

Expected certificate evidence in the dedicated log:

```text
[SteamNetSockets] AuthStatus (...): OK (OK)
[SteamNetSockets] Certificate expires in ...
```

If the VAC popup appears while the dedicated log has certificate OK, treat it as a client/lobby/connect-state problem, not a certificate generation failure.

## Useful Log Paths

```text
C:\SERVER\Steam\steamapps\common\dota 2 beta\game\dota\console.log
C:\SERVER\Steam\steamapps\common\dota 2 beta\game\bin\win64\SKYNET\steam_api.log
C:\SERVER\SKYNET Steam Emulator\.tmp\skynet_server_stdout_*.log
C:\SERVER\Lobby problem
```

Relevant server log markers:

```text
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

When Dota shows VAC but logs show `connected_players ... connected=0` repeatedly, inspect why the client did not enter the dedicated server before changing certificate code.

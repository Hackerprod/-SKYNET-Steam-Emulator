---
name: skynet-gc-maintenance
description: Use when implementing, maintaining, capturing, reverse-engineering, or validating Dota 2 Game Coordinator behavior in the SKYNET Steam Emulator server, especially GC Lua plugins under `/GC/570`, protobuf updates, NetHook2 captures, NetHookAnalyzer comparison, lobby/profile/login coordinator messages, or server-side GC routing.
---

# SKYNET GC Maintenance

## Scope

Use this skill for Dota 2 Game Coordinator work in the SKYNET emulator. The goal is to keep `steam_api64.dll` generic and put Dota-specific GC behavior in the server, preferably under a hot-editable coordinator such as:

```text
SKYNET server\GC\570\
```

The DLL should forward GC traffic and Steamworks context. The server/plugin should decide how to answer Dota GC messages.

For tool setup, capture commands, and new-workspace notes, read [references/tools-and-capture.md](references/tools-and-capture.md).

## Core Rule

Never implement Dota-specific GC behavior directly inside `steam_api64.dll` unless it is strictly transport plumbing. Put message handling, state machines, lobby responses, profile/welcome responses, and test fixtures in the server-side GC layer.

## Mandatory Capture Workflow

1. Stop Dota and the emulator server.

2. Restore the original Valve DLL before capturing real Steam behavior.
   Use the backup:

   ```text
   C:\SERVER\Steam\steamapps\common\dota 2 beta\game\bin\win64\steam_api64_original.dll
   ```

   Copy it over:

   ```powershell
   Copy-Item -LiteralPath "C:\SERVER\Steam\steamapps\common\dota 2 beta\game\bin\win64\steam_api64_original.dll" `
     -Destination "C:\SERVER\Steam\steamapps\common\dota 2 beta\game\bin\win64\steam_api64.dll" `
     -Force
   ```

   If `steam_api64_original.dll` does not exist, stop and ask for the original Valve DLL. Do not capture against the emulator DLL and call it ground truth.

3. Launch Steam normally and start NetHook2 capture.
   Use elevated permissions when required. Confirm NetHook2 attaches to the correct Steam process before launching Dota.

4. Launch Dota with the original DLL and perform the exact scenario to emulate.
   Examples:
   - GC welcome/login until coordinator connected.
   - Open profile and inspect account/player data.
   - Create custom lobby.
   - Change team, coach slot, leave team, rejoin, start match.
   - Accept invites or friend-dependent flows if relevant.

5. Stop capture and save the dumps with a scenario name and timestamp.
   Keep raw captures separate from decoded notes. Do not overwrite earlier captures.

6. Decode and compare with NetHookAnalyzer.
   Identify message IDs, protobuf types, request/response order, fields Dota cares about, and timing/ack patterns.

7. Restore the emulator DLL before implementing/testing SKYNET behavior.
   Build `steam_api`, copy `steam_api\bin\Release\x64\steam_api.dll` to Dota as `steam_api64.dll`, and restart the SKYNET server.

## Implementation Workflow

1. Locate the server-side GC routing first.
   Inspect:
   - `SKYNET server\Program.cs`
   - GC endpoints such as `/api/gamecoordinator/messages`, `/exchange`, `/poll`
   - `SKYNET server\GC\570\`
   - plugin registry/service code
   - current Lua/proto helpers

2. Update protobuf definitions before guessing fields.
   Use Valve/Dota protobuf files from current captures or current known schema sources. Regenerate any generated code/helpers if the project uses generated proto artifacts. Keep old compatibility only when Dota still sends old fields.

3. Implement handlers in the server coordinator.
   Prefer small handlers per message type:
   - parse request
   - update server state
   - emit one or more response messages
   - preserve unknown fields when possible
   - log message type, job ID, and key IDs

4. Keep coordinator state explicit.
   Store session/account/profile/lobby/team/coach data in server state. Avoid hardcoded friends, names, lobby users, or account data unless explicitly creating a test fixture.

5. Keep hot reload useful.
   If Lua is used, structure the plugin so common behavior can be changed without recompiling the DLL. Restart only the server/plugin when possible; rebuild the DLL only when transport structs or API forwarding changes.

6. Add logs that help compare to captures.
   Log request type, response type, key IDs, result codes, lobby IDs, party/team changes, and GC session phase. Avoid logging secrets or raw huge payloads unless debugging a specific parser issue.

## Validation Workflow

1. Start the SKYNET server in the background on `http://127.0.0.1:27080`.

2. Build and deploy the emulator DLL if transport changed.
   Follow `$skynet-emulator-implementation` for backup, build, deploy, Dota launch, screenshot inspection, and log review.

3. Launch Dota without Steam when testing the emulator.

4. Reproduce the same scenario from the capture.
   Compare:
   - message order
   - message IDs/types
   - job IDs
   - result codes
   - required fields
   - visible Dota UI state
   - server and `steam_api.log`

5. Capture a screenshot of the relevant Dota state.
   Do not claim a GC fix works from logs alone. For login/profile/lobby work, inspect the actual Dota UI image.

6. Iterate until Dota reaches the target state or the blocker is concrete.
   If the game hangs, inspect the last outbound/inbound GC messages and compare to the original capture.

## What To Report

Final responses for GC work must include:

- Which original capture was used, or state that no fresh capture was used.
- Whether the original Valve DLL was restored for capture.
- Which GC messages/protos were implemented or changed.
- Whether the change was server-only or DLL transport was rebuilt.
- Server status and PID if left running.
- Dota launch result.
- Screenshot observation.
- Relevant log findings.
- Remaining gaps against the original capture.

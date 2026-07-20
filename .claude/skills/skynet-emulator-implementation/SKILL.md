---
name: skynet-emulator-implementation
description: Use when modifying or validating the SKYNET Steam Emulator repo, especially changes in `SKYNET server`, `steam_api`, `steam_api64.dll`, Steamworks interfaces, GameCoordinator, lobbies, friends, avatars, auth, or Dota 2 runtime behavior. Enforces backup, build, deploy, server restart, Dota launch, screenshot inspection, and log analysis before claiming a fix works.
---

# SKYNET Emulator Implementation

## Scope

Use this skill for work in `C:\SERVER\SKYNET Steam Emulator` that affects either:

- `SKYNET server`
- `steam_api`
- `steam_api64.dll`
- Dota 2 behavior through the emulator
- server/client synchronization such as friends, avatars, lobbies, GameCoordinator, stats, auth, or callbacks

Do not treat a build-only result as complete when runtime behavior is involved. The expected close-out is implementation, build, deployment, game launch, screenshot inspection, and log review.

## Fixed Paths

- Repo: `C:\SERVER\SKYNET Steam Emulator`
- Dota win64 folder: `C:\SERVER\Steam\steamapps\common\dota 2 beta\game\bin\win64`
- Emulated DLL name required by Dota: `steam_api64.dll`
- Native exported x64 build artifact: `steam_api\bin\Release\x64\steam_api.dll`
- Original Valve DLL backup: `C:\SERVER\Steam\steamapps\common\dota 2 beta\game\bin\win64\steam_api64_original.dll`
- Server URL: `http://127.0.0.1:27080`
- Server project: `SKYNET server\SKYNET server.csproj`
- Dota/API log to inspect when relevant: `steam_api.log`

## Mandatory Workflow

1. Inspect the current implementation before editing.
   Read the relevant server and DLL files, plus recent logs if the user reports a crash, hang, missing UI, missing avatars, bad friends, lobby failure, or GC issue.

2. Determine the affected side.
   If the change affects the web/API/backend only, build and restart the server. If it affects Steamworks calls, callback structs, `steam_api`, avatars, friends, lobbies, GC, auth tickets, or Dota-visible behavior, follow the DLL workflow too.

3. Preserve the original Steam DLL before overwriting anything.
   Before copying a new emulator DLL to the Dota folder, check whether `steam_api64_original.dll` exists. If it does not exist, copy the current `steam_api64.dll` to `steam_api64_original.dll`. Never overwrite an existing original backup unless the user explicitly requests it.

4. Build the server when server code changed.
   Stop the process listening on port `27080`, then run:

   ```powershell
   dotnet build "SKYNET server\SKYNET server.csproj" -c Debug --no-restore
   ```

   Start the server in the background and verify `curl http://127.0.0.1:27080/` or `/login` returns a valid response. Keep the server running if Dota needs it.

5. Build the DLL when `steam_api` changed.
   Use MSBuild with DllExport's bundled IL tools:

   ```powershell
   $msbuild = "C:\Program Files (x86)\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
   $argsList = @(
     'steam_api\steam_api.csproj',
     '/restore',
     '/p:Configuration=Release',
     '/p:Platform=AnyCPU',
     '/p:SolutionDir=D:/Install/Dev/Projects/SKYNET Steam Emulator/',
     '/p:DllExportOurILAsm=true',
     '/m'
   )
   & $msbuild @argsList
   ```

   Treat `steam_api\bin\Release\steam_api.dll` as an intermediate x86/managed artifact. Deploy `steam_api\bin\Release\x64\steam_api.dll` for Dota.

6. Deploy the DLL.
   Copy the x64 artifact to the Dota folder with the exact name `steam_api64.dll`:

   ```powershell
   Copy-Item -LiteralPath "steam_api\bin\Release\x64\steam_api.dll" `
     -Destination "C:\SERVER\Steam\steamapps\common\dota 2 beta\game\bin\win64\steam_api64.dll" `
     -Force
   ```

   Verify timestamp, file size, and PE machine type when there is any doubt.

7. Launch Dota for runtime validation when DLL-visible behavior changed.
   Do not skip this after Steam API/DLL changes unless the user explicitly says to defer runtime testing. Launch the Dota executable from the win64 folder without requiring Steam when using the emulator. If Dota requires elevated permission, report that and continue only after the user grants/handles it.

8. Capture and inspect evidence.
   After launching Dota, capture a screenshot of the relevant in-game screen. Inspect the image, not only logs. For UI changes such as avatars, names, friends, profile panel, lobby, login, coordinator status, or dialogs, state what is visible and whether it matches the expected result.

9. Analyze logs after every runtime test.
   Read `steam_api.log` and server logs after the launch. If the game crashes, hangs, or shows wrong data, use those logs plus the screenshot to drive the next iteration.

10. Iterate until the behavior is verified or clearly blocked.
    Do not claim success from compilation alone. If Dota hangs, crashes, or cannot be launched, say exactly where validation stopped and what evidence was collected.

## Runtime Validation Rules

- For avatar work, verify that Dota calls `GetSmallFriendAvatar`, `GetMediumFriendAvatar`, or `GetLargeFriendAvatar`, that `GetImageSize` and `GetImageRGBA` return valid data, and that the in-game image is not the default/blank avatar.
- For friends work, verify relationship values, friend count, friend names, invite/request callbacks, and the in-game friends/profile panel.
- For lobby work, test create, join, team changes, coach slot, leave, and start-game flow when relevant.
- For GC work, keep the server running, verify `gamecoordinator` requests hit the backend, and inspect both server and DLL logs.
- For auth/session work, verify Dota reaches the expected connection/login state, not just that `/api/auth/steam/session` returns OK.

## Final Response Requirements

Include:

- What changed.
- What was built.
- Where the DLL was deployed.
- Whether the original backup existed or was created.
- Server PID/URL if left running.
- Dota launch result.
- Screenshot/image observation.
- Log observations.
- Any unverified piece or blocker.

Keep the final answer concise, but never omit a failed or skipped runtime validation.

# Tools And Capture

## Required Tools

- Visual Studio Build Tools with MSBuild.
- .NET SDK for the server.
- Valve Steamworks SDK used by the repo.
- NetHook2 for capturing Steam/GC traffic.
- NetHookAnalyzer for decoding captures.
- Protobuf tooling matching the server/plugin implementation.
- A real Steam install and Dota 2 install for original behavior captures.

## Protobuf Sources

Prefer sources in this order:

1. Current local Dota files or capture-derived schemas when available.
2. SteamTracking/GameTracking-Dota2 for Dota-specific protos:

   ```text
   https://github.com/SteamDatabase/GameTracking-Dota2
   ```

   Important folder:

   ```text
   GameTracking-Dota2\Protobufs
   ```

3. SteamTracking/Protobufs for shared Steam base protos:

   ```text
   https://github.com/SteamDatabase/Protobufs
   ```

   Important folders:

   ```text
   Protobufs\steam
   Protobufs\dota
   ```

Do not paste random `.proto` files from blogs or old sample projects into the active coordinator. If an old project is used as a guide, compare it against the current tracked proto files and the current NetHook capture before implementing.

## New Workspace Setup

1. Install Visual Studio Build Tools.
   Include .NET desktop build tools and MSBuild. The expected MSBuild path on this machine is:

   ```text
   C:\Program Files (x86)\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin\MSBuild.exe
   ```

2. Install .NET SDK.
   Verify:

   ```powershell
   dotnet --info
   ```

3. Restore/build the server:

   ```powershell
   dotnet restore "SKYNET server\SKYNET server.csproj"
   dotnet build "SKYNET server\SKYNET server.csproj" -c Debug
   ```

4. Build the DLL with DllExport:

   ```powershell
   $msbuild = "C:\Program Files (x86)\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
   $argsList = @(
     'steam_api\steam_api.csproj',
     '/restore',
     '/p:Configuration=Release',
     '/p:Platform=AnyCPU',
     '/p:SolutionDir=C:\SERVER\SKYNET Steam Emulator/',
     '/p:DllExportOurILAsm=true',
     '/m'
   )
   & $msbuild @argsList
   ```

5. Install NetHook2 and NetHookAnalyzer.
   Prefer their official GitHub releases. If the workspace already has a known copy, use that copy rather than downloading random forks. Keep the tools outside the repo unless the user explicitly wants tool binaries vendored.

6. Install protobuf tools only as needed.
   Use the language/runtime expected by the coordinator implementation. Keep generated files reproducible and note the exact command used.

## Preparing Protos In A New Workspace

Use a separate tooling folder so upstream schemas do not get mixed with edited server files:

```powershell
New-Item -ItemType Directory -Force -Path Tools\protos\upstream | Out-Null
git clone https://github.com/SteamDatabase/GameTracking-Dota2 Tools\protos\upstream\GameTracking-Dota2
git clone https://github.com/SteamDatabase/Protobufs Tools\protos\upstream\SteamTracking-Protobufs
```

Record the exact commits used:

```powershell
git -C Tools\protos\upstream\GameTracking-Dota2 rev-parse HEAD
git -C Tools\protos\upstream\SteamTracking-Protobufs rev-parse HEAD
```

Find imports before generating:

```powershell
rg '^import ' Tools\protos\upstream\GameTracking-Dota2\Protobufs -g "*.proto"
rg '^import ' Tools\protos\upstream\SteamTracking-Protobufs -g "*.proto"
```

Common Dota files to inspect first:

```text
dota_gcmessages_msgid.proto
dota_gcmessages_client.proto
dota_gcmessages_server.proto
dota_clientmessages.proto
dota_commonmessages.proto
```

The exact set changes with Dota updates. Let `protoc` import errors and NetHookAnalyzer decoded message names drive the final list.

## Generating Proto Artifacts

Use the repo's existing coordinator runtime first. Do not introduce a second proto runtime unless the existing one cannot parse the needed messages.

For descriptor-driven Lua or hot-reload systems, generate a descriptor set:

```powershell
protoc `
  -ITools\protos\upstream\GameTracking-Dota2\Protobufs `
  -ITools\protos\upstream\SteamTracking-Protobufs\steam `
  -ITools\protos\upstream\SteamTracking-Protobufs\dota `
  --include_imports `
  --descriptor_set_out "SKYNET server\GC\570\protos\dota.desc" `
  Tools\protos\upstream\GameTracking-Dota2\Protobufs\dota_gcmessages_msgid.proto `
  Tools\protos\upstream\GameTracking-Dota2\Protobufs\dota_gcmessages_client.proto `
  Tools\protos\upstream\GameTracking-Dota2\Protobufs\dota_gcmessages_server.proto
```

For generated C# helpers, prefer a checked-in generation script and output into a clear generated folder. Example shape:

```powershell
protoc `
  -ITools\protos\upstream\GameTracking-Dota2\Protobufs `
  -ITools\protos\upstream\SteamTracking-Protobufs\steam `
  --csharp_out "SKYNET server\Generated\DotaProtos" `
  Tools\protos\upstream\GameTracking-Dota2\Protobufs\dota_gcmessages_msgid.proto
```

After generation:

- Do not hand-edit generated code.
- Commit or document the upstream commit hashes.
- Keep descriptor/generated output versioned with the handler changes that require it.
- If a capture has fields missing from the generated type, check for outdated proto commits before adding ad hoc parsing.

## Original DLL Handling

Before capture:

```powershell
Copy-Item -LiteralPath "C:\SERVER\Steam\steamapps\common\dota 2 beta\game\bin\win64\steam_api64_original.dll" `
  -Destination "D:\SERVER\Steam\steamapps\common\dota 2 beta\game\bin\win64\steam_api64.dll" `
  -Force
```

Before emulator validation:

```powershell
Copy-Item -LiteralPath "steam_api\bin\Release\x64\steam_api.dll" `
  -Destination "D:\SERVER\Steam\steamapps\common\dota 2 beta\game\bin\win64\steam_api64.dll" `
  -Force
```

If `steam_api64_original.dll` is missing, do not overwrite the only available DLL. Ask the user for the original or restore it through Steam verification first.

## Capture Naming

Use scenario-focused folders:

```text
captures\
  2026-07-07_gc-login-profile_original\
  2026-07-07_lobby-create-team-coach_original\
```

Inside each folder, keep:

- raw NetHook dumps
- analyzer exports
- notes with scenario steps
- screenshots if available
- Dota/Steam version and date

## Capture Scenarios

For login/profile:

1. Start Steam.
2. Attach NetHook2.
3. Launch Dota.
4. Wait for GC connected/login complete.
5. Open profile panel.
6. Stop capture.

For lobbies:

1. Start from connected GC state.
2. Create custom lobby.
3. Change team.
4. Leave team.
5. Join coach slot.
6. Rejoin team.
7. Start match or attempt start.
8. Stop capture.

## Analyzer Notes

When decoding captures, extract:

- message ID
- message name/proto type
- direction
- job ID/source job ID
- account ID/steam ID
- lobby ID/party ID
- result code
- repeated fields that Dota expects
- unknown fields that appear consistently

Use the original sequence as the baseline. Implement the smallest server behavior that makes Dota progress to the next state, then rerun and compare.

## Runtime Verification

Always collect:

- server stdout/stderr tail
- `steam_api.log`
- screenshot of the Dota UI state
- the last GC request and response pair before a hang/crash

For UI-related GC fixes, screenshot inspection is mandatory.

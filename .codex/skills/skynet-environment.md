# SKYNET Environment

Edit this file when the repo, install path, tool path, server URL, Dota install, launcher command, or capture tooling differs in another environment. Skills should read this file before using paths or commands.

## Repository

- Repo root: `C:\SERVER\SKYNET Steam Emulator`
- Server project: `C:\SERVER\SKYNET Steam Emulator\SKYNET server\SKYNET server.csproj`
- Server executable: `C:\SERVER\SKYNET Steam Emulator\SKYNET server\bin\Debug\net8.0\SKYNET server.exe`
- GC TypeScript root: `C:\SERVER\SKYNET Steam Emulator\SKYNET server\GC\570`
- Steam API project: `C:\SERVER\SKYNET Steam Emulator\steam_api\steam_api.csproj`
- Launcher project: `C:\SERVER\SKYNET Steam Emulator\SKYNET Steam Client\SKYNET Steam Client.csproj`
- Launcher executable: `C:\SERVER\SKYNET Steam Emulator\SKYNET Steam Client\bin\Debug\net472\SKYNET Steam Client.exe`

## Dota And Payloads

- Dota win64 folder: `C:\SERVER\Steam\steamapps\common\dota 2 beta\game\bin\win64`
- Dota direct DLL target: `C:\SERVER\Steam\steamapps\common\dota 2 beta\game\bin\win64\steam_api64.dll`
- Dota original Valve DLL backup: `C:\SERVER\Steam\steamapps\common\dota 2 beta\game\bin\win64\steam_api64_original.dll`
- Built Steam API DLL output: `C:\SERVER\SKYNET Steam Emulator\steam_api\bin\Release\steam_api.dll`
- Launcher x64 payload: `C:\SERVER\SKYNET Steam Emulator\SKYNET Steam Client\payload\x64\steam_api64.dll`
- Launcher x86 payload: `C:\SERVER\SKYNET Steam Emulator\SKYNET Steam Client\payload\x86\steam_api.dll`

## Server

- HTTP local URL: `http://127.0.0.1:27080`
- HTTP LAN URL: `http://10.0.0.1:27080`
- Expected listen address for LAN tests: `0.0.0.0:27080`
- Login health endpoint behavior: `GET /` may redirect to `/login`; this is acceptable when checking that the server is reachable.

## Logs

- Server logs: inspect the active server console/output first, then configured log files if present.
- DLL/game log: inspect the configured SKYNET Steam API log beside the launched game when present.
- External copied logs: use only when the user provides or references a copied log directory for a specific issue.

## Build Commands

Build server:

```powershell
dotnet build "C:\SERVER\SKYNET Steam Emulator\SKYNET server\SKYNET server.csproj" -c Debug
```

Build launcher:

```powershell
dotnet build "C:\SERVER\SKYNET Steam Emulator\SKYNET Steam Client\SKYNET Steam Client.csproj" -c Debug
```

Build Steam API DLL x64:

```powershell
$msbuild = "C:\Program Files (x86)\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin\amd64\MSBuild.exe"
& $msbuild "C:\SERVER\SKYNET Steam Emulator\steam_api\steam_api.csproj" /t:Restore /p:Configuration=Release /p:Platform="AnyCPU" /p:RuntimeIdentifier=win-x64 "/p:SolutionDir=C:\SERVER\SKYNET Steam Emulator\\"
& $msbuild "C:\SERVER\SKYNET Steam Emulator\steam_api\steam_api.csproj" /t:Rebuild /p:Configuration=Release /p:Platform="AnyCPU" /p:RuntimeIdentifier=win-x64 /p:PlatformTarget=x64 /p:DllExportPlatform=x64 /p:DllExportOurILAsm=true "/p:SolutionDir=C:\SERVER\SKYNET Steam Emulator\\" /m:1 /v:minimal
```

Build Steam API DLL x86:

```powershell
$msbuild = "C:\Program Files (x86)\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin\amd64\MSBuild.exe"
& $msbuild "C:\SERVER\SKYNET Steam Emulator\steam_api\steam_api.csproj" /t:Restore /p:Configuration=Release /p:Platform="AnyCPU" /p:RuntimeIdentifier=win-x86 "/p:SolutionDir=C:\SERVER\SKYNET Steam Emulator\\"
& $msbuild "C:\SERVER\SKYNET Steam Emulator\steam_api\steam_api.csproj" /t:Rebuild /p:Configuration=Release /p:Platform="AnyCPU" /p:RuntimeIdentifier=win-x86 /p:PlatformTarget=x86 /p:DllExportPlatform=x86 /p:DllExportOurILAsm=true "/p:SolutionDir=C:\SERVER\SKYNET Steam Emulator\\" /m:1 /v:minimal
```

Sync launcher payloads:

```powershell
Copy-Item -LiteralPath "C:\SERVER\SKYNET Steam Emulator\steam_api\bin\Release\steam_api.dll" -Destination "C:\SERVER\SKYNET Steam Emulator\SKYNET Steam Client\payload\x64\steam_api64.dll" -Force
# Rebuild x86 first, then:
Copy-Item -LiteralPath "C:\SERVER\SKYNET Steam Emulator\steam_api\bin\Release\steam_api.dll" -Destination "C:\SERVER\SKYNET Steam Emulator\SKYNET Steam Client\payload\x86\steam_api.dll" -Force
dotnet build "C:\SERVER\SKYNET Steam Emulator\SKYNET Steam Client\SKYNET Steam Client.csproj" -c Debug
```

## Runtime Commands

Restart server:

```powershell
Get-Process | Where-Object { $_.Path -eq "C:\SERVER\SKYNET Steam Emulator\SKYNET server\bin\Debug\net8.0\SKYNET server.exe" } | Stop-Process -Force
Start-Process -FilePath "C:\SERVER\SKYNET Steam Emulator\SKYNET server\bin\Debug\net8.0\SKYNET server.exe" -WorkingDirectory "C:\SERVER\SKYNET Steam Emulator\SKYNET server\bin\Debug\net8.0" -WindowStyle Hidden
```

Verify server:

```powershell
Get-NetTCPConnection -LocalPort 27080 -State Listen
curl.exe -I http://127.0.0.1:27080
curl.exe -I http://10.0.0.1:27080
```

Launch Dota for emulator validation through the launcher with launch parameters. Do not start the game executable directly for emulator validation:

```powershell
& "C:\SERVER\SKYNET Steam Emulator\SKYNET Steam Client\bin\Debug\net472\SKYNET Steam Client.exe" --launch "8f48e29f623a4248ab533c63b7b0c835"
```

Expected Dota launch health for the current local setup: the `dota2` process should create a `Dota 2` window and grow well beyond the early 30-60 MB startup range. If it stays headless with `MainWindowHandle = 0`, inspect `SKYNET\steam_api.log` and the DLL init path before treating it as a UI foreground issue.

## GC Validation

- Prefer the repo's configured TypeScript/protobuf validation command when present.
- For TypeScript-only GC edits, rely on hot reload and reset stale sessions/lobbies/player data only when needed.
- Restart server or Dota only when the host/runtime, generated bindings, dependencies, or process state requires it.

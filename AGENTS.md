# Repository Guidelines

## Project Structure & Module Organization
`steam_api` is a .NET Framework 4.7.2 class library that emulates the Steam API DLL. Core entry points live at the project root in `SteamEmulator.cs`, `SteamID.cs`, and `CallbackInterop.cs`. Keep feature code in the existing folders:

- `Managers/` for lifecycle and subsystem coordinators such as networking, overlay, tickets, and callbacks.
- `Steamworks/Implementation/`, `Steamworks/Interfaces/`, and `Steamworks/Exported/` for Steam API surface area and exported bindings.
- `Network/`, `Callback/`, `Helpers/`, and `Types/` for transport, callback plumbing, shared utilities, and DTO-style models.
- `Overlay/` and `Properties/` for WinForms UI and project resources.

Do not commit generated output from `bin/`, `obj/`, `x64/`, `x86/`, or zip archives unless a release task explicitly requires it.

## Build, Test, and Development Commands
Build from the solution root (`..`), not just this folder, because the project imports `DllExport.bat` and shared `packages/` assets.

```powershell
msbuild "..\[SKYNET] Steam Emulator.sln" /p:Configuration=Debug
msbuild "..\[SKYNET] Steam Emulator.sln" /p:Configuration=Release
msbuild ".\steam_api.csproj" /t:Clean
```

`Debug` writes the DLL into the client output path configured in `steam_api.csproj`; `Release` emits to `bin\Release\`.

## Coding Style & Naming Conventions
Use 4-space indentation and standard C# brace style. Follow the existing naming pattern: `PascalCase` for types, methods, and public fields; `camelCase` only for local variables and parameters. Match the current folder naming when adding new Steam interface versions, for example `Steamworks/Interfaces/SteamUser/SteamUser024.cs`. Keep changes narrow and preserve public signatures used by exported bindings.

## Testing Guidelines
There is no dedicated test project in this repository. Validate changes with a clean build plus targeted manual smoke tests against the emulator flow you changed, such as login, lobby, overlay, or network messaging. When fixing protocol or callback behavior, document the reproduction steps in the pull request.

## Commit & Pull Request Guidelines
Recent history uses short imperative subjects such as `Update ImageHelper.cs` and `MongoDB server updated`. Prefer concise commit titles under 72 characters that name the affected area. Pull requests should include a clear summary, linked issue if applicable, manual verification steps, and screenshots only for `Overlay/` or other UI changes.

## Configuration Notes
Treat `app.config`, ports, storage paths, and injected DLL names as environment-sensitive. Avoid hardcoding machine-specific paths outside the existing project settings, and do not commit personal `.user` or Visual Studio cache files.

## Importante
- Este es un proyecto de estudio para emular la conexion a steam para juegos en local, ayudaras a la investigacion siendo fiel a la emulacion de la logica original de steam para este fchero 

# InterfaceLayoutCheck

Verifies the Steamworks interface vtables that `steam_api` exposes. A game calls an interface method by
slot number, so the order and count of methods registered for each interface version string
(`STEAMUSER_INTERFACE_VERSION019`, `SteamUser019`, ...) must match Valve's exactly.

The tool loads the managed `steam_api.dll`, initializes `SKYNET.Managers.InterfaceManager`, and reads the
methods declared with `[InterfaceLayout("<version>", "MethodName", ...)]` in
`steam_api/Steamworks/Interfaces/<Family>/`.

## Build

```powershell
dotnet build "DeveloperTools\InterfaceLayoutCheck\InterfaceLayoutCheck.csproj" -c Release
```

Targets .NET Framework 4.7.2, x64, to match `steam_api`.

## Usage

```
InterfaceLayoutCheck generate|verify        <steam_api.dll> <goldenDir> [versionPrefix]
InterfaceLayoutCheck generate-new           <steam_api.dll> <goldenDir> [versionPrefix]
InterfaceLayoutCheck verify-headers         <steam_api.dll> <header-spec-dir> [versionPrefix]
InterfaceLayoutCheck verify-vtables         <steam_api.dll> <anyDir> [versionPrefix]
```

`<steam_api.dll>` is the managed build output, `steam_api/obj/Release/steam_api.dll`.
`versionPrefix` limits the run to versions starting with it (for example `STEAMUSER`).

| Mode | What it does |
|------|--------------|
| `verify` | Compares the runtime slot order against `golden/<version>.txt`. Reports `DIFF`, `NEW` (registered, no golden) and `MISSING` (golden exists, no longer registered). |
| `verify-headers` | Compares the runtime slot order (names only) against `header-spec/<version>.txt`, generated from Valve's headers by [`InterfaceLayoutGen`](../InterfaceLayoutGen/README.md). Reports `HDRDIFF`; overload and variant slots the emulator names freely are only a `NOTE`. |
| `verify-vtables` | Builds every registered vtable for real and checks each slot got a native function pointer (`VTABLE` on failure). |
| `generate` | Overwrites `golden/` with the current layout. Only use when the change is intended. |
| `generate-new` | Writes goldens only for versions that have none. Run `verify-headers` first: a golden is a snapshot of the current layout, not proof it is right. |

The process exit code is non-zero when there are failures.

## Data folders

- `golden/` (about 240 files): the accepted runtime layout per interface version, one line per slot:
  `index|MethodName|returnType|paramTypes`. Regression guard.
- `header-spec/` (about 250 files): the layout derived from the real headers, one line per slot:
  `index|MethodName|paramCount|returnType|paramTypes`, preceded by `#` comments naming the source
  header and any overload groups. Written by `InterfaceLayoutGen`; do not edit by hand.

## Typical flow when adding or changing an interface

1. Declare the class and `[InterfaceLayout(...)]` in `steam_api`.
2. Build `steam_api` in Release.
3. `verify-headers` against the spec. Fix the order until there is no `HDRDIFF`.
4. `verify-vtables` to confirm every slot has a native pointer.
5. `generate-new` (or `generate` for a deliberate change) to record the golden.

# DeveloperTools

Tools for developing and validating SKYNET. None of them ships with the server or the launcher.

| Folder | Kind | Use it to |
|--------|------|-----------|
| [`SkynetGc`](SkynetGc/README.md) | .NET CLI | Record the real Dota GC flow, analyze it, decode GC bodies, drive the game window. |
| [`InterfaceLayoutCheck`](InterfaceLayoutCheck/README.md) | .NET checker | Verify the Steamworks interface vtables of `steam_api` (slot order, native pointers). |
| [`InterfaceLayoutGen`](InterfaceLayoutGen/README.md) | Python generator | Derive the vtable specs from Valve's real headers (feeds `InterfaceLayoutCheck`). |
| [`CallbackLayoutCheck`](CallbackLayoutCheck/README.md) | .NET checker + Python | Verify callback and struct layouts (ids, sizes, field offsets) of `steam_api` against the SDK headers. |
| [`SteamClientVtableProbe`](SteamClientVtableProbe/README.md) | Python scripts | Read vtable layouts out of Valve's real `steamclient64.dll` (ground truth). |

## Which one for which question

- "What does the real GC send for this action?" -> `SkynetGc`.
- "Did I break a Steamworks interface or callback in `steam_api`?" -> `InterfaceLayoutCheck` and
  `CallbackLayoutCheck`, run against the built `steam_api.dll`.
- "A new SDK version appeared / I need the layout of an interface" -> `InterfaceLayoutGen` (from
  headers), `SteamClientVtableProbe` (from the real client DLL).

## How the steam_api checks fit together

```
Valve SDK headers --InterfaceLayoutGen--> InterfaceLayoutCheck/header-spec/*.txt --+
                                                                                   +--> InterfaceLayoutCheck verify-headers
steam_api.dll (managed) --InterfaceLayoutCheck generate--> golden/*.txt -----------+
Valve SDK headers --CallbackLayoutCheck/gen_expected.py--> expected/callbacks_*.txt --> CallbackLayoutCheck verify
steamclient64.dll --SteamClientVtableProbe--> slot counts --> compare_proton.py (cross-check)
```

`steam_api` is the emulated Steamworks DLL (`steam_api/`). Games call its interfaces through C++
vtables, so a wrong slot order or a wrong struct size breaks them silently. These tools make that
mismatch visible at build time.

## Conventions

- Source reference material (SDK headers, Proton's `lsteamclient`, Goldberg sources) lives under
  `.tmp/` in the repository root. That folder is git-ignored; each tool's README says what it expects
  there.
- Generated output (`golden/`, `header-spec/`, `expected/`) is committed. Do not edit it by hand:
  regenerate it with the tool that owns it.
- `bin/` and `obj/` are build output and ignored.

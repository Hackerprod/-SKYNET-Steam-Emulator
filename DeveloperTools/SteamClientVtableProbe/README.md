# SteamClientVtableProbe

Ground truth for interface vtable layouts. Valve's real `steamclient64.dll` exposes one
`CAdapterSteam<Family><NNN>` class per interface version, and that class is exactly the vtable a game
calls. Reading it settles questions the headers cannot (slot counts, overload order, destructors).

Python 3 scripts. `scan_vtables.py` and `disasm_slots.py` need `pip install pefile capstone`.

## Get the DLL

From Valve's CDN: manifest `https://steamcdn-a.akamaihd.net/client/steam_cmd_win32`, package
`steamcmd_bins_win32.zip.<hash>`. It contains `steamclient64.dll` and `steamclient.dll`. The DLL is not
committed.

## Scripts

| Script | Use |
|--------|-----|
| `scan_vtables.py <steamclient64.dll> <CAdapterSteamXxxNNN ...>` | Finds the RTTI type descriptor of each class, walks its vtable and prints `slots=N`. Writes `<folder of the DLL>/../vt/out.json` (create that `vt` folder first). |
| `disasm_slots.py <steamclient64.dll> <Class> <slot ...>` | Disassembles the given slots of a class to see what a slot really does. It reads `steamcmd_bins/vt/out.json` relative to the current folder, so keep the DLL in a subfolder of `steamcmd_bins` and run it from the folder that contains `steamcmd_bins`. |
| `compare_proton.py [repoRoot]` | Cross-checks our `InterfaceLayoutCheck/golden/` against Valve's Proton `lsteamclient` Windows vtables (`.tmp/ProtonLsteamclient/lsteamclient/win*.c`) and against `steamclient64_slot_counts.txt`. Reports disagreements in method order or count. |

## Data

`steamclient64_slot_counts.txt`: `CAdapterSteam<Family><NNN> vtable_rva=<hex> slots=<N>` for every adapter
class of the build downloaded 2026-10-19. `compare_proton.py` reads it.

## What it established (2026-10-19, issue #42)

- Overload pairs are reversed in the Windows vtable: `SetStat` float before int32, `SetProperty` Bool at
  slot 34. (`InterfaceLayoutGen` applies the same MSVC rule.)
- `ISteamHTMLSurface004` slot 0 is a scalar-deleting destructor.
- `ISteamGameServer` versions 011 to 015 have 44 slots.
- UGC `KeyValueTag` / `CreateQueryAllUGCRequest` order matches Proton's Windows vtables.

## Related

[`InterfaceLayoutCheck`](../InterfaceLayoutCheck/README.md) holds the layouts being checked;
[`InterfaceLayoutGen`](../InterfaceLayoutGen/README.md) derives them from headers.


# SteamClientVtableProbe

Ground truth for interface vtable layouts: the real `steamclient64.dll` from Valve exposes one
`CAdapterSteam<Family><NNN>` class per interface version, which is exactly the vtable games call.

1. Get the DLL from Valve's CDN: manifest `https://steamcdn-a.akamaihd.net/client/steam_cmd_win32`,
   package `steamcmd_bins_win32.zip.<hash>` (contains `steamclient64.dll`, `steamclient.dll`).
2. `python scan_vtables.py steamclient64.dll <CAdapterSteamXxxNNN ...>` prints `slots=N` per class
   and writes `../vt/out.json` (create that dir first); `disasm_slots.py steamclient64.dll <Class> <slot...>`
   disassembles slots (needs `pip install pefile capstone`).
3. `steamclient64_slot_counts.txt` is the output for every CAdapterSteam class of the build
   downloaded 2026-10-19.

Findings 2026-10-19 (issue #42): overload pairs are reversed (SetStat float before int32, SetProperty
Bool at slot 34), ISteamHTMLSurface004 slot 0 is a scalar-deleting destructor, GameServer 011-015 have
44 slots, UGC KeyValueTag/CreateQueryAllUGCRequest order matches Proton's Windows vtables.

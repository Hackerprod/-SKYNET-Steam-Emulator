# SKYNET GC tool

Developer tool for the Dota 2 GameCoordinator (GC) flow: record what the real GC says to a real
Dota client, turn it into readable flows, decode single messages, and drive the game window to
reproduce flows. It is for developing and validating the GC scripts in `SKYNET server/GC/570`; it is
not part of the shipped server or launcher.

```powershell
dotnet build "DeveloperTools\SkynetGc\SkynetGc.csproj" -c Debug
$gc = "DeveloperTools\SkynetGc\bin\Debug\net8.0-windows\skynet-gc.exe"
& $gc          # lists the commands
```

## Layout

One project, two targets of the same sources:

| Target | Output | Purpose |
|--------|--------|---------|
| `net8.0-windows` | `skynet-gc.exe` | The command line: `capture`, `analyze`, `decode`, `ui`, `iface`. |
| `net472` | `hook\skynet-gc-hook.exe` | EasyHook payload. Run by `capture` to start Dota; it is also the assembly injected into `dota2.exe`. |

EasyHook needs .NET Framework (it uses Remoting), both to inject and to load the payload, same as the
launcher's own injector (`SKYNET Steam Client/Services/DllInjector.cs`). Building the tool builds the
hook and copies it, with the EasyHook native files, to `hook\` next to `skynet-gc.exe`. It also builds the interface-tracer payload (`..\SkynetIfaceHook`, net472, its own project so EasyHook finds one entry point per assembly) and copies `skynet-iface-hook.exe` and `meta\SteamFriends017.txt` there too.

## capture: record the real GC flow

```powershell
& $gc capture start [--out DIR] [--args "extra dota args"] [--dota <...\game\bin\win64>]
```

Steam must be running and logged in. Starts `dota2.exe` against the real Steam client and hooks
`SendMessage` and `RetrieveMessage` of `ISteamGameCoordinator` inside the game process. Only GC
traffic is recorded, already decrypted, as one JSON object per line in `gc.jsonl` (default folder:
`%TEMP%\skynet-capture\<timestamp>`):

```json
{"t":55613,"dir":"recv","type":4004,"proto":true,"size":809,"b64":"..."}
```

`b64` is the exact buffer exchanged with the interface (GC header + protobuf body). The Dota
directory is found through Steam's library list. The hook is read-only: it observes the calls and
passes them on unchanged.

Launching Dota against real Steam with an injected DLL carries VAC risk. Use it deliberately, with an
account you accept that risk for, and stop the capture if a VAC popup appears.

## iface: trace the calls of a Steam interface

```powershell
& $gc iface start  [--out DIR] [--args "..."] [--dota DIR] [--interfaces LIST] [--methods LIST]   # Dota on the real Steam, tracer injected
& $gc iface attach --pid N [--out DIR] [--emu-log PATH] [--interfaces LIST] [--methods LIST] [--via-emulator]   # into a running dota2.exe
& $gc iface mark "label" ; & $gc iface stop ; & $gc iface status [--list]
& $gc iface timeline [--iface RE] [--steamid ID] [--method RE] [--contains TEXT] [--between A B] [--limit 40] [--offset 0]
& $gc iface query --method RE [--steamid ID] [--between A B]   # grouped: distinct args -> result, count, callers
& $gc iface diff [--emu-log PATH]                              # tracer vs the emulator's steam_api.log
& $gc iface selftest                                           # offline checks, no game needed
```

Generic over any Steam interface: records every call of the selected methods with decoded arguments, result
and caller (`module+0xRVA`) to `iface.jsonl` in a capture folder (default `%TEMP%\skynet-iface\<timestamp>`;
commands without `--dir` use the newest). `--interfaces` takes spec file names or `interface:version`
(`SteamFriends017,ISteamUser:23`; any of the 247 files of `InterfaceLayoutCheck\header-spec`); a version is
required when several exist, because a wrong guess would silently record nothing. Without `--interfaces` the
tool says so and traces `SteamFriends017` (ISteamFriends v017, the first interface verified live) with 11
preset methods (`GetPersonaState`, `GetFriendCount`, `GetFriendByIndex`, `GetFriendRelationship`,
`GetFriendPersonaState`, `GetFriendPersonaName`, `GetFriendGamePlayed`, `GetFriendRichPresence`, `HasFriend`,
`ActivateGameOverlayToUser`, `RequestUserInformation`). `--methods` picks names; with `--interfaces` and no
`--methods` every method whose signature is fully known is traced, and each other method gets a `skip` record
with the reason (floats, by-value structs, ...). Method order and signatures come from the committed
header-spec at run time. The only
change made in the game is one 8-byte store per method in the interface's vtable (data, no code); `stop`
puts the originals back (only if the slot still holds the tracer's pointer). Nothing is changed in an
argument or a result. `mark` stamps the capture so `timeline`/`query` can slice between actions.

`diff` is the gate against the emulator: it reads the byte range of the emulator log written while the
tracer was active (lines `<Interface>: <Method> ...`, only for patched methods). Default rule for any
method: every value the trace recorded (non-pointer arguments, result) must appear in a log line of that
method, and every logged line must be explained by a trace record. Methods with an entry in the `Rules`
table of `Iface\IfaceDiff.cs` are compared exactly instead (the ISteamFriends v017 ones: exact for
relationship, count, by-index, persona state/name, rich presence and overlay dialog; bool only for
`GetFriendGamePlayed`; presence only for `HasFriend` and `GetPersonaState`; arguments only for
`RequestUserInformation`); adding a rule is adding a row. Exit 0 = clean pass. Dropped records
or an emulator log that did not grow make the result INCONCLUSIVE (exit 1).

Emulator games: a Dota started by the launcher already hosts EasyHook, and a second EasyHook injection fails (Code 15).
Use `--via-emulator` instead: set the environment variable `SKYNET_IFACE_TRACER` to the full path of
`skynet-iface-hook.exe` in the process that starts the game (`$env:SKYNET_IFACE_TRACER = "<hook exe>"` before running
`SKYNET Steam Client.exe --launch ...`; the launcher creates the game with the environment it inherited), start the game, then
`iface attach --pid N --via-emulator`. The ini key `[Debug] IfaceTracer` in `steam_api.ini` is honoured too, but the launcher
rewrites that ini on every launch and drops unknown keys, so the variable is the supported way. With neither set the feature is
off: no thread, no polling. The CLI writes `%TEMP%\skynet-iface\attach-request.json` (atomically); a
thread in the emulator DLL (client process only) polls it every ~2 s, loads the payload in-process and runs it. Its
log lines are prefixed `[IFACE]` in `steam_api.log`. Everything after attach (`mark`, `stop`, `status`, `timeline`,
`query`, `diff`) is unchanged, and a second request is served after `stop`. Real Steam uses `iface start`.

Extending to a new interface needs no code unless it uses types that do not fit an integer register: the
decoder tables in `SkynetIfaceHook\Types.cs` (handle typedef widths, enum names, flag parameters, out structs)
get rows; float/double (typed delegates), by-value structs and other hidden-return structs are the one place
that needs real work, and until then those methods are skipped with a reason.

Launching Dota against real Steam with an injected DLL carries VAC risk. Use it deliberately, with an
account you accept that risk for, and stop the capture if a VAC popup appears. (`attach` into a Dota that
runs on the emulator involves no Steam session.)
## analyze: turn a capture into a flow report

```powershell
& $gc analyze <capture-dir | gc.jsonl | decoded-json-dir> [out] [--after N --window N --baseline DIR]
```

Decodes every message with the protobuf contracts in `SKYNET server/GC/570/contracts` and writes
agent-readable artifacts to `out` (default `<input>\gc-json`):

| File | Content |
|------|---------|
| `summary.json` | Decode totals and compact record metadata. |
| `timeline.json` | Chronological message list. |
| `message-index.json` | Messages grouped by id and direction. |
| `jobs.json` | `source_job_id` to `target_job_id` request/response links. |
| `conversation-flows.json` | Each client request with its immediate and job-linked replies. |
| `transitions.json` | Counts of consecutive-message transitions. |
| `after-windows.json` | The next N messages after selected anchor messages. |
| `protocol-graph.json` | Causal graph of messages, SO cache nodes and edges. |
| `pattern-summary.json` | Compact flow patterns. |
| `entities.json` | Every id, `steam_id`, `account_id` and `lobby_id` value and the time-ordered messages carrying it. |
| `graph-signature.json` | Stable shape signatures, for comparing captures across proto versions. |
| `graph-diff.json` | Added, removed and changed shapes (only with `--baseline`). |
| `flow-report.html` | Interactive sequence diagram of the flow. |
| `forensic-report.md` | The same flow as a text report. |

Options:

- `--after N --window W`: the W records that follow GC message type N.
- `--baseline DIR`: compare against a previous output folder or its `graph-signature.json`.

A folder of per-record decoded JSON files (the per-message files `analyze` itself writes) is accepted
as input, to rebuild the aggregate artifacts without the original capture.

## decode: one message body

```powershell
& $gc decode <gcMessageType> <body.bin> [client|server]
```

## ui: drive the game window

```powershell
& $gc ui shot [file.png]       # client area of the Dota window
& $gc ui click X Y [--right]   # X, Y are fractions 0..1 of the client area
& $gc ui key "{ENTER}"         # SendKeys syntax
& $gc ui type "text"
& $gc ui console "disconnect"  # opens the console, runs a command, closes it
```

Primitives for an agent (or a person) to reproduce a flow. Coordinates are fractions of the window,
so nothing depends on resolution or Dota version. Pick a new file name for each `shot`.

Safety: the dedicated server's console window is never targeted (a click inside it enters selection
mode and freezes the server), and a click is refused when the game window is not what is under the
cursor.

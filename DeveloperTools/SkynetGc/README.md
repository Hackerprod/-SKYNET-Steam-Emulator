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
| `net8.0-windows` | `skynet-gc.exe` | The command line: `capture`, `analyze`, `decode`, `ui`. |
| `net472` | `hook\skynet-gc-hook.exe` | EasyHook payload. Run by `capture` to start Dota; it is also the assembly injected into `dota2.exe`. |

EasyHook needs .NET Framework (it uses Remoting), both to inject and to load the payload, same as the
launcher's own injector (`SKYNET Steam Client/Services/DllInjector.cs`). Building the tool builds the
hook and copies it, with the EasyHook native files, to `hook\` next to `skynet-gc.exe`.

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

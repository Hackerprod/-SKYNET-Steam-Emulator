# SKYNET CLI

One developer CLI for working with the Dota 2 GameCoordinator flow. It replaces the former
`NetHookGcJson` and `DecodeGcBody` tools.

```powershell
dotnet build "DeveloperTools\SkynetCli\SkynetCli.csproj" -c Debug
$cli = "DeveloperTools\SkynetCli\bin\Debug\net8.0-windows\skynet-cli.exe"
```

The project builds two targets of the same sources: `net8.0-windows` is the command line
(`skynet-cli.exe`); `net472` is the EasyHook payload (`hook\skynet-hook.dll`) that `capture` injects
into `dota2.exe`. EasyHook loads managed payloads through the .NET Framework CLR, same as the
launcher's own injector. Building the CLI builds and copies the hook.

## capture: record the real GC flow

```powershell
& $cli capture start [--out DIR] [--args "extra dota args"] [--dota <...\game\bin\win64>]
```

Steam must be running and logged in. Starts `dota2.exe` against the real Steam client and hooks the
three methods of `ISteamGameCoordinator` (`SendMessage`, `RetrieveMessage`) inside the game process.
Only GC traffic is recorded, already decrypted, one JSON object per line in `gc.jsonl`:

```json
{"t":55613,"dir":"recv","type":4004,"proto":true,"size":809,"b64":"..."}
```

`b64` is the exact buffer exchanged with the interface (GC header + protobuf body). Nothing else
Steam does is recorded, unlike NetHook. Real Steam launches carry VAC risk: use it deliberately, and
stop if the VAC popup appears.

## analyze: turn a capture into a flow report

```powershell
& $cli analyze <capture-dir | gc.jsonl | decoded-json-dir | nethook-dump-dir> [out] [--after N --window N --baseline DIR]
```

Decodes every message with the protobuf contracts in `SKYNET server/GC/570/contracts` and writes
agent-readable artifacts: `summary.json`, `timeline.json`, `message-index.json`, `jobs.json`
(`source_job_id` -> `target_job_id`), `conversation-flows.json`, `transitions.json`,
`after-windows.json`, `protocol-graph.json`, `pattern-summary.json`, `entities.json` (every
id/steam_id/account_id/lobby_id value and the messages that carry it), `graph-signature.json`,
`graph-diff.json` (with `--baseline`), `flow-report.html` and `forensic-report.md`.
Legacy NetHook `.bin` folders are still accepted.

- `--after N --window W`: the next W records after GC message type N.
- `--baseline DIR`: compare against a previous output folder or `graph-signature.json`.

## decode: one message body

```powershell
& $cli decode <gcMessageType> <body.bin> [client|server]
```

## ui: drive the game window (agent primitives)

```powershell
& $cli ui shot [file.png]       # client area of the Dota window
& $cli ui click X Y [--right]   # X, Y are fractions 0..1 of the client area
& $cli ui key "{ENTER}"         # SendKeys syntax
& $cli ui type "text"
& $cli ui console "disconnect"  # open console, run a command, close it
```

Coordinates are fractions of the window, so nothing depends on resolution or Dota version. The
dedicated server's console window is never targeted (a click inside it enters selection mode and
freezes the server); clicks are refused when the game window is not at the click position.

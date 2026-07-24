# NetHook GC JSON and Flow Graphs

`NetHookGcJson` decodes NetHook2 Steam GameCoordinator captures and builds
agent-readable protocol flow artifacts. It accepts either raw NetHook `.bin`
folders or folders that already contain decoded per-record JSON files.

## Build

```powershell
dotnet build "DeveloperTools\NetHookGcJson\NetHookGcJson.csproj" -c Debug /nodeReuse:false
```

## Raw `.bin` Capture

```powershell
dotnet run --project "DeveloperTools\NetHookGcJson\NetHookGcJson.csproj" -c Debug --no-build -- "<capture-folder>" "<output-folder>"
```

## Decoded JSON Folder

```powershell
dotnet run --project "DeveloperTools\NetHookGcJson\NetHookGcJson.csproj" -c Debug --no-build -- "<decoded-json-folder>" "<output-folder>"
```

The JSON mode ignores generated aggregate files such as `summary.json`,
`timeline.json`, `protocol-graph.json` and `pattern-summary.json`, then rebuilds
all aggregate artifacts from decoded per-record JSON.

## Focused Windows

Use `--after` and `--window` to inspect what follows a specific GC message.

```powershell
dotnet run --project "DeveloperTools\NetHookGcJson\NetHookGcJson.csproj" -c Debug --no-build -- "<capture-folder>" "<output-folder>" --after 7038 --window 20
```

## Baseline Comparison

Use `--baseline` with either a previous output folder or a direct
`graph-signature.json` path. The tool writes `graph-diff.json` and summarizes
the changes in `forensic-report.md`.

```powershell
dotnet run --project "DeveloperTools\NetHookGcJson\NetHookGcJson.csproj" -c Debug --no-build -- "<new-capture-folder>" "<new-output-folder>" --baseline "<old-output-folder>"
```

## Outputs

- `summary.json`: decode totals and compact record metadata.
- `timeline.json`: chronological GC message list.
- `message-index.json`: message groups by GC id/direction.
- `jobs.json`: `source_job_id` to `target_job_id` request/response links.
- `conversation-flows.json`: client request records with immediate and job-linked replies.
- `transitions.json`: consecutive-message transition counts.
- `after-windows.json`: next N records after selected anchor messages.
- `protocol-graph.json`: causal graph with message nodes, SO cache nodes and edges.
- `pattern-summary.json`: compact flow patterns for agent reasoning.
- `entities.json`: every concrete id/steam_id/account_id/lobby_id value and the time-ordered chain of messages (with field paths) that carry it, tagged strong/medium/weak so an agent can follow which request carries an id and which later message echoes it.
- `graph-signature.json`: stable message/edge/pattern shape signatures for comparing captures across proto versions.
- `graph-diff.json`: added/removed/count-changed message, edge and pattern shapes when a baseline is supplied.
- `flow-report.html`: self-contained interactive canvas. Renders the capture as a Client / Game Coordinator / SO Cache sequence diagram with job-id correlation arcs and SO-mutation branches, a "Top correlated IDs" panel, plus a written "mental map" and a click-to-inspect per-message panel. Selecting a message shows its payload digest (what it contains), its id values, job correlation, SO effects, and every other message carrying the same id (with the sequence gap) — the value-level data-flow map. Rows sharing the selected message's ids are highlighted.
- `forensic-report.md`: human-readable summary.

## Graph Semantics

Message nodes preserve direction, sequence, GC message id/name, proto type,
job ids, payload shape hash and entity hints such as `lobby_id`, `party_id`,
`match_id`, `owner_id`, `steam_id` and `account_id`.

SO cache nodes are extracted from decoded payloads when `type_id`,
`owner_type`, `owner_id`, `service_id` or `object_data` are available. Edges are
classified as exact, strong or inferred:

- `responds_to`: exact `source_job_id`/`target_job_id` correlation.
- `causes_immediate`: server messages emitted immediately after a client request.
- `creates_so` / `updates_so`: decoded SO cache mutation caused by a message.
- `same_entity`: messages sharing an entity hint.
- `temporal_next`: chronological adjacency.

Value-level correlation (`entities.json`) is separate from `same_entity`: it keys on the
concrete id value, keeps the exact field path in every message, and grades each value
strong (large/unique id), medium (small int under a single field name) or weak
(small int colliding across different field names, filtered out of the reports). This is
generic — it works on any GC proto with no per-message hardcoding.

Use `graph-signature.json` when comparing a known-good capture against a newer
proto version. It highlights structural changes in message shapes, edge shapes
and high-level request/response patterns without requiring byte-identical dumps.

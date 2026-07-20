---
name: nethook-gc-forensics
description: Decode and analyze NetHook2 Steam GameCoordinator captures for SKYNET Dota GC work. Use when Codex needs to inspect raw `*_5452_*` or `*_5453_*` dumps, export protobuf bodies to JSON, compare real Steam GC flows, find message order after an anchor message, correlate `source_job_id` and `target_job_id`, or inspect SO cache/lobby objects before implementing TS GC handlers.
---

# NetHook GC Forensics

## Core Rule

Use this skill after `skynet-dota-gc` when raw NetHook dumps are the source of truth. Do not infer Dota GC fields from legacy byte fixtures when a capture is available. Decode the capture, inspect the protobuf JSON, then update TS GC behavior from the observed flow and data.

## Tool

The repo contains a C# forensic decoder that reuses the server's generated protobuf-net contracts:

```powershell
dotnet build "DeveloperTools\NetHookGcJson\NetHookGcJson.csproj" -c Debug --no-restore /nodeReuse:false
```

Default command:

```powershell
dotnet run --project "DeveloperTools\NetHookGcJson\NetHookGcJson.csproj" -c Debug --no-build -- "<capture-nethook-folder>" "<output-folder>"
```

Focused "what came after this message" command:

```powershell
dotnet run --project "DeveloperTools\NetHookGcJson\NetHookGcJson.csproj" -c Debug --no-build -- "<capture-nethook-folder>" "<output-folder>" --after 7038 --window 20
```

For the current local lobby capture:

```powershell
dotnet run --project "DeveloperTools\NetHookGcJson\NetHookGcJson.csproj" -c Debug --no-build -- "C:\SERVER\SKYNET Steam Emulator\.tmp\Lobby\nethook" "C:\SERVER\SKYNET Steam Emulator\.tmp\Lobby\gc-json" --after 7038 --window 20
```

## Output Files

- `forensic-report.md`: human-readable summary, hot messages, job links, and hot transitions.
- `summary.json`: decoded/error totals and compact per-record metadata.
- `timeline.json`: every GC record in chronological order.
- `message-index.json`: records grouped by GC message type.
- `jobs.json`: `source_job_id` request to `target_job_id` response correlation.
- `conversation-flows.json`: each client request with immediate server replies and job-linked replies.
- `transitions.json`: global consecutive-message transition counts.
- `after-windows.json`: next N records after each anchor selected by `--after`.
- Per-record JSON files: full decoded payloads, raw packet base64, headers, job IDs, and nested SO object data.

## Workflow

1. Build the tool before relying on old binaries.
2. Run it against the raw NetHook folder, not against already exported JSON.
3. Open `forensic-report.md` first to understand coverage and decode gaps.
4. Use `after-windows.json` when the question is "after message X, what follows?"
5. Use `jobs.json` and `conversation-flows.json` when the question involves request/response matching.
6. Open per-record JSON files for implementation details. For lobby work, inspect SO cache records such as `SOCacheSubscribed` and `SOCacheUpdated`.
7. If a payload is undecoded, search generated contracts and routes before guessing. Add a real message/proto mapping or regenerate protos; do not create byte-level fixtures.
8. When SO object `type_id` is unknown, trust only `decodedType` or `candidateDecodedTypes` entries produced by exact protobuf roundtrip matching.

## Implementation Guidance

Use decoded payloads to implement TS handlers with typed generated descriptors. Preserve job IDs in responses when the captured flow uses them. Keep dynamic data in GC state/services; do not copy whole base64 payloads into handlers.

When comparing emulator behavior to a capture, check:

- message type and direction
- order relative to neighboring messages
- `source_job_id` and `target_job_id`
- response result fields
- SO cache owner, object type, version, and decoded object fields
- whether a message is immediate, job-linked, or asynchronous

If the current generated contracts cannot decode a real capture message, treat that as a schema/tooling gap to fix before implementing behavior.

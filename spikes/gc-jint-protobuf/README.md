# Fase 0 spike - Jint + protobuf.js (descriptor JSON)

De-risk spike for the GC migration (roadmap v3). Runs the **committed
production artifacts** (`SKYNET server/GC/570/js/dist/gc.js` +
`proto-descriptor.json`) under Jint 4.13.0 exactly like `JintGcEngine` does,
and validates every blocking assumption against real nethook captures.

Run: `dotnet test spikes/gc-jint-protobuf`

## Gate results (2026-07-17, 12/12 green)

### 1. Codegen under Jint: WORKS
protobuf.js generates its encoders/decoders with the `Function` constructor
(`@protobufjs/codegen`). Jint 4.13 executes them without issue, including
`Root.fromJSON` over the descriptor. The `pbjs -t static-module` fallback is
NOT needed.

### 2. 64-bit representation: Long.js (decision)
protobuf.js does not emit BigInt; its native representation is Long.js. The
bundle ships `long` 5.3.2 and sets `protobuf.util.Long` explicitly (without it,
protobuf.js silently degrades 64-bit fields to lossy JS numbers).

**Decision: Long.js end-to-end inside JS; decimal strings at every JSON/C#
boundary** (`toObject({ longs: String })`). No BigInt shim - it would add a
conversion on every 64-bit field in the hot path for no benefit. Verified
exact for `ulong.MaxValue`, `long.MaxValue` and real 64-bit steamids/version
numbers from the captures.

### 3. Unknown fields: dropped on decode (documented + bounded)
protobuf.js drops unknown fields when decoding, so decode->encode is NOT
byte-stable for messages with unmodeled fields (demonstrated with a real
`CMsgClientHello` capture: 103 -> 63 bytes; all modeled fields survive).

**Audited subset requiring preservation: empty today.** No current flow
decodes a client message and re-emits it with unmodeled fields expected to
survive: s2c responses are built from scratch, and pass-through/catalog blobs
are re-emitted from the original bytes without a decode->encode round-trip.
Rule going forward (enforced in Fase 4 review, id by id): a pass-through
handler must keep and re-emit the **original bytes**; never decode->encode a
message it does not fully model.

### 4. Oracle modes: both implemented
- **byte-exact**: only for pass-through / catalogo-global blobs re-emitted
  untouched.
- **semantic diff** (decode both sides, compare trees): for reconstructed
  messages. Protobuf has no canonical encoding - the spike proves two
  wire-equivalent encodings with reordered fields fail the byte oracle and
  pass the semantic one.

### 5. Perf under the Jint interpreter (no V8 JIT), worst realistic case
Real capture `server_24_0001.bin`: `CMsgSOCacheSubscribed`, 25,641 bytes,
**309 econ items**.

| Case | p50 | p99 |
|---|---|---|
| SOCacheSubscribed decode+encode (25.6 KB) | **35.5 ms** | 211 ms |
| CSOEconItem decode+encode x309 (full inventory) | **75.8 ms** | 93.9 ms (~0.245 ms/item) |

These messages fire at login frequency, not per-frame: acceptable. The single
engine + global lock model (Fase 1 decision (a)) is not a bottleneck at this
scale.

## Proto curation validated (Fase 2 loop started)
The curated `proto/` decodes the real captures; one community-proto error was
already caught by this loop: `CMsgSOIDOwner.id` is `uint64` (varint) on the
wire, not `fixed64`. The fixture round-trip is the proof of the `.proto`.

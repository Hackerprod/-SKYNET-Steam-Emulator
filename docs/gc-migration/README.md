# Migracion GC (Dota 570) a TypeScript + Jint + protobuf.js - estado

Implementacion del roadmap v3. Este documento registra que fase esta
completada, las decisiones tomadas en cada gate y lo que queda.

## Estado por fase

| Fase | Estado | Entregables |
|---|---|---|
| A - Inventario y cuarentena | **completada** | `gc-js/observed-migration.csv` (94 ids OBSERVED), `gc-js/quarantine.json` (200 builders prohibidos), `gc-js/registry/proto-message-registry.json` (201 ids), `gc-js/inventory/dotagcbackend-inventory.json` (385 metodos en 6 cubos). Todo generado por `gc-js/tools/gen-inventory.mjs`, nada hardcodeado. |
| B - Router por mensaje | **completada** | `GcEngineRouter` (Exchange, Poll y Tick enrutados), `GcRoutingTable` hot-reload (`GC/gc-routing.json`), flags de rollback por id/global/env, **decision de clientVersion** (abajo). Gate: con 0 ids migrados el comportamiento es identico (tests verdes). |
| 0 - Spike de de-risk | **completada** | `spikes/gc-jint-protobuf/` 12/12 verde. Ver su README: codegen OK bajo Jint, repr 64-bit = **Long.js + string en frontera**, unknown-fields acotado (subconjunto vacio hoy), ambos oraculos, perf SOCache 309 items p50=35.5ms. |
| 1 - Plomeria host JS | **completada** | `JintGcEngine` (sandbox sin CLR/fs/red, LimitMemory 128MB, timeout configurable `GameCoordinator:JsTimeoutMs`, LimitRecursion), **concurrencia: engine unico por app + lock global (opcion a)**, hot-reload con swap atomico, throw -> notHandled -> fallback a Lua. Toolchain `gc-js/` (TS + esbuild + pbjs), bundle commiteado. Gate verificado por `tests/SkynetServer.GcTests`. |
| 2 - Codec protobuf | **iniciada** | `proto/` curado inicial (gcsdk + econ) validado contra fixtures (ya cazo un error de la comunidad: `CMsgSOIDOwner.id` es varint, no fixed64). `Proto.encode/decode` via `Root.fromJSON` operativo bajo Jint. **Falta**: curar el resto de mensajes de `observed-migration.csv` iterando contra sus fixtures. |
| 3 - Golden tests por escenario | pendiente | El harness de oraculos (byte-exacto/semantico) existe en el spike; falta el runner de escenarios con estado (mensajes/orden/jobIds/DB/pending). |
| 4 - Migracion incremental | pendiente | Orden: login -> econ/SO cache -> lobby -> party -> stats -> cola larga (`observed-migration.csv` bucket por bucket). Por id: handler TS + golden verde -> activar en `gc-routing.json` -> borrar ruta Lua/C#. |
| 5 - Cutover y limpieza | pendiente | Apagar Lua, vaciar `DotaGcBackend`, quitar codec a mano y hacks MoonSharp. |

## Decisiones cerradas

- **uint64**: Long.js dentro de JS (protobuf.js no emite BigInt); string decimal
  en toda frontera JSON/C#; `ulong` en el host. Una sola representacion.
- **clientVersion (Fase B)**: NO es derivable de la sesion (verificado: el
  handshake no lo trae) y `steam_api` (DLL) no cambia. Se deriva server-side
  del `CMsgClientHello` (field 1) de cada sesion via `GcClientVersionCache`;
  `GcEnvelope.clientVersion` es null antes del primer hello. Sin excepcion a
  "el DLL no cambia".
- **Concurrencia del engine (Fase 1)**: opcion (a), engine unico por appId con
  lock global. Justificacion: engine stateless (regla 10) + perf del spike.
  El hot-reload construye el engine nuevo fuera del lock y hace swap atomico;
  las llamadas en vuelo terminan en el engine viejo.
- **Poll (regla 8)**: `GcEngineRouter.Poll` consulta `gc-routing.json`
  (`pollEngine` por app). El branch `DotaGcBackend.Poll` sigue siendo la
  implementacion del motor Lua/C# hasta que el estado de lobby/sesion migre
  (Fase 4/5); ya no es un atajo del plugin, es una ruta explicita del router.
- **Deploy runtime**: `dist/gc.js` (incluye protobufjs/light + long, bundled
  por esbuild) + `proto-descriptor.json`. Un archivo menos que el plan
  original (no hay `protobuf.min.js` separado); mismo principio: cero Node en
  el server.
- **Fixtures**: excluidas del output de build (`CopyToOutputDirectory=Never`),
  `main.lua` no incluye `routes.generated.lua`, y el gate de CI
  (`check-gates.mjs`) lo verifica en cada build.

## Como migrar un mensaje (Fase 4, por id)

1. Curar/validar su `.proto` contra los fixtures (`gc-js/tools/dev-decode-fixture.mjs`).
2. Escribir el handler TS en `gc-js/src/` y registrarlo en el mapa `handlers`.
3. `npm run build` (regenera descriptor + bundle) y commitear los artefactos.
4. Golden test que mencione el id (el gate de CI lo exige) con el oraculo
   correcto: byte-exacto si es pass-through/catalogo, diff semantico si es
   reconstruido.
5. Anadir el id a `migratedMessageIds` en `GC/gc-routing.json` (hot-reload).
6. Cuando este estable, borrar la ruta Lua/C# equivalente.

Rollback: quitar el id de `migratedMessageIds` o anadirlo a
`disabledMessageIds` (hot-reload), `jintEnabled=false`, o
`SKYNET_GC_JS_DISABLED=1`.

## Gates de CI (`.github/workflows/gc-ci.yml`)

- `tsc --noEmit` falla el pipeline.
- Bundle/descriptor commiteados != rebuild de las fuentes -> fail.
- `runtime.fixture`/`observed_fixture` en el bundle de produccion -> fail.
- C# productivo leyendo `GC/570/fixtures` -> fail.
- `routes.generated.lua` alcanzable desde `main.lua` -> fail.
- Id migrado sin golden test -> fail.
- Artefactos de inventario (Fase A) desincronizados -> fail.
- Spike (Fase 0) y tests de router/engine (Fase B/1) en cada push.

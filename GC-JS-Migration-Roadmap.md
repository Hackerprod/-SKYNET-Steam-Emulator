# Roadmap v2 - Migracion del Game Coordinator (Dota 570) a TypeScript + Jint + protobuf.js

> Para que otro agente lo implemente. El usuario revisa antes de arrancar.
> Decision: **TS en dev/CI** (se transpila a JS y se commitea el `.js`; el server NO lleva Node).
> v2 incorpora la revision de Fable: endurecer contrato, router y pruebas ANTES de migrar,
> para que Jint no termine escondiendo los mismos hardcodes con otra sintaxis.

---

## 0. Objetivo y no-negociables

**Objetivo:** mover TODA la logica GC a una capa de script hot-reloadable y capaz por si misma
(sin fugar logica a C#), con `.proto` como fuente de verdad y uint64 correcto.

**Reglas duras (su violacion es la razon de esta migracion):**
1. **Hot-reload obligatorio.** Editar reglas/mensajes GC (JS + `proto-descriptor.json` + datos) y probar sin
   recompilar C#, sin reiniciar Dota ni el server. (Cambios en el C# host SI pueden requerir restart; es la parte estable.)
2. **Cero Node en el server.** Runtime = .NET + Jint + `protobuf.min.js` (light) + `proto-descriptor.json` + `dist/gc.js`. Node solo en dev/CI.
3. **`.proto` = fuente de verdad.** Nada de wire bytes a mano.
4. **uint64 unico:** `BigInt` dentro de JS; **string** en la frontera JSON/C#.
5. **Strangler por mensaje, no big-bang.** GC-JS junto al Lua/C#; se migra por message id.
6. **Sin regresion de arranque** ni de partida.
7. **[Fable] Ningun response builder de GC en C# para mensajes migrados.** Prohibido por CI.
8. **[Fable] Poll/tick/pending pasan por el MISMO router** que Exchange. Nada de `DotaGcBackend.Poll` directo.
9. **[Fable] Fixtures solo en dev/test.** El host NO inyecta `runtime.fixture` en el sandbox de produccion
   (no existe como capacidad, no solo "lanza"). Ademas, NINGUN codigo productivo (C# ni JS) lee `GC/570/fixtures`.
10. **[Fable] Estado en host/DB, no en globals JS.** Recargar el engine no puede perder lobby/session/match.

---

## 1. Estado actual (mapa real + evidencia del atajo)

### 1.1 Entrada y contrato
- `IGameCoordinatorPlugin` (`Services/GameCoordinator/GameCoordinatorContext.cs`):
  `CanHandle(uint appId)`, `Exchange(context, request)`, `Poll(context)`.
  **Problema [Fable]:** `CanHandle` solo mira `appId` -> imposible enrutar "este mensaje a JS, este a Lua/C#".
- `GameCoordinatorContext` = `{ AppId, SteamId, AccountId, PersonaName, ClientIp }` (ya trae mas que appId/steamId).
- `ApiGCExchangeRequest` = `{ AppId, MessageType, BodyBase64, SourceJobId?, SteamId, GameServer }` (`Models/SteamApiModels.cs:516`).
- `ApiGCExchangeResponse` = `{ Handled, Messages: List<ApiGCMessage> }`; cola async `GameCoordinatorPendingMessages`.
- Plugin Lua (`LuaGameCoordinatorPlugin.cs`): carga/cachea `Script` MoonSharp (`Preset_SoftSandbox`), globals `gc/runtime/SteamDB/DotaDB/DedicatedServerService/include`, llama `handle` (Exchange) / `tick` (Poll). Hot-reload por mtime.
- **Atajo confirmado [Fable]:** `Poll(context)` hace `context.AppId == 570 ? DotaGcBackend.Poll(context) : ...` (`LuaGameCoordinatorPlugin.cs:106`). Estado vivo del GC escondido en C#.

### 1.2 El circulo vicioso (evidencia)
- `gc` = **`DotaGcBackend`** (god node; `GC/570/DotaGcBackend*.cs`, ~230 KB, ~474 miembros).
- Payloads cruzan como **base64** (`gc.Proto(msgId, base64)`, `gc.Reply(msgId, base64)`) porque Lua no maneja bytes.
- `gc.ProtoVarint / ProtoFixed64String / ProtoBytes / ProtoCount` = **parsers protobuf a mano en C#** (MoonSharp no parsea protobuf). `ProtoFixed64String` devuelve **string** para no corromper uint64 como double.
- `DotaGcBackend` construye econ/SO cache a mano (`BuildEconItemsForUser`, `BuildEconItem`, `BuildLobbySoCacheSubscribed`, `QueueEconItemChangesToServer`) = **response builders compilados** = violacion del hot-reload.
- `SteamDB`/`DotaDB` wrappers estilo Lua devuelven **JSON string** (`DotaDB.cs:490`) -> JS necesita DTOs tipados, no strings.
- Fixtures: `GC/570/fixtures/nethook/*.bin` (363) + `routes.generated.lua` (`OBSERVED.client[msgId]={bins}`) = replay de bytes (parche, no solucion).

### 1.3 Lua a migrar
`main.lua` (dispatch por `MSG.*`), `dota_lobby.lua` (94 KB, state machine), `dota_items.lua`, `dota_party.lua`, `dota_stats.lua`, `wire.lua`, `messages.lua`, `routes.generated.lua`.

---

## 2. Arquitectura objetivo

```
C# host (compilado, estable)
  IGcMessageHandler (router NUEVO por mensaje)
    - LuaGcEngine     (existente; se apaga por id)
    - JintGcEngine    (NUEVO; JS)
  GcEngineRouter: decide motor por (context, messageType, direction). Exchange Y Poll/tick.
  Host API tipada al sandbox JS:
    gc.*       emitir/encolar (reply, proto, queueReplyTo, queueToServer, notHandled)
    Proto.*    encode/decode desde proto-descriptor.json (Root.fromJSON) con BigInt + unknown fields
    SteamDB, DotaDB, DedicatedServerService  -> DTOs TIPADOS (no JSON string)
    log, runtime.fixture (SOLO build dev/test; NO existe en el sandbox de produccion)
  Transporte, sesiones/auth, DB, DedicatedServer, colas, storage, certs. SIN logica GC.
JS (hot-reload): toda la logica GC. Autoria TS -> esbuild -> dist/gc.js (commiteado).
.proto (fuente de verdad, dev/CI) -> pbjs -> proto-descriptor.json -> (runtime) protobuf.min.js light + Root.fromJSON -> Proto.encode/decode
Estado persistente: host/DB (NO globals JS).
```

---

## 3. GcEnvelope - el contrato interno completo [Fable, critico]

`msg: { type, payload }` es insuficiente. Definir un **GcEnvelope** que el host construye y pasa a JS:

```ts
interface GcEnvelope {
  appId: number;
  steamId: bigint;
  accountId: number;
  personaName: string;
  clientIp: string;
  messageType: number;
  sourceJobId: bigint | null;    // request.SourceJobId
  targetJobId: bigint | null;    // para respuestas dirigidas
  isGameServer: boolean;         // request.GameServer
  clientVersion: number | null;  // build/version del cliente (agregar a request si no existe aun)
  payload: Uint8Array;           // bytes (no base64)
}
```
- `clientVersion/build` no esta hoy en `ApiGCExchangeRequest`: **agregarlo** (o derivarlo) en la fase de contrato.
- El host mapea `GameCoordinatorContext` + `ApiGCExchangeRequest` -> `GcEnvelope` (BigInt para 64-bit, `Uint8Array` desde `BodyBase64`).

### Contrato host->JS (typings `gc-js/src/host.d.ts`)
```ts
declare const env: GcEnvelope;
declare const Proto: {
  decode(name: string, bytes: Uint8Array): any;   // 64-bit -> BigInt; unknown fields preservados
  encode(name: string, obj: any): Uint8Array;
};
declare const gc: {
  reply(responseMsgId: number, bytes: Uint8Array): void;
  proto(msgId: number, bytes: Uint8Array): void;
  queueReplyTo(steamId: bigint, msgId: number, bytes: Uint8Array, targetJobId: bigint): void;
  queueToServer(serverSteamId: bigint, msgId: number, bytes: Uint8Array): void;
  notHandled(reason: string): void;                // fallback explicito con motivo (loggeado)
};
declare const SteamDB: { /* DTOs tipados: user, session, friends, presence, auth */ };
declare const DotaDB:  { /* DTOs tipados: profile, stats, matches, teams, inventory, conduct, quests */ };
declare const DedicatedServerService: { /* reservar/lanzar/estado, tipado */ };
declare const log: (msg: string) => void;
declare function handle(env: GcEnvelope): void;
declare function tick(ctx: { appId: number; steamId: bigint }): void;
```
`runtime.fixture` NO va en el contrato de produccion. Vive en un typing aparte `gc-js/src/host.dev.d.ts`
(`interface DevRuntime { fixture(path: string): Uint8Array }`) que solo se incluye en el build dev/test.
En produccion el host no inyecta esa capacidad en el sandbox (no existe, no solo "lanza").

---

## 4. Toolchain TS (dev/CI) - cero Node en server

- `gc-js/` (proyecto TS): `typescript`, `esbuild`, `protobufjs` (+ `pbjs`/`pbts` para descriptor y `.d.ts`).
- Build: `esbuild src/index.ts --bundle --format=iife --outfile=dist/gc.js` (ms; watch en dev).
- **Codec protobuf [correccion Fable]:** `protobuf.js/light` NO parsea `.proto` texto (no trae parser).
  Ruta correcta (recomendada por Fable): **descriptor JSON**.
  - `.proto` sigue siendo la fuente de verdad, en `proto/`.
  - **Dev/CI** genera `proto-descriptor.json` desde `proto/` con `pbjs -t json`.
  - **Runtime** (Jint) carga `protobuf.min.js` (light) + `proto-descriptor.json` via `protobuf.Root.fromJSON(descriptor)`.
    Mas rapido y estable que parsear texto, y mantiene cero Node en el server.
  - `pbts` genera `.d.ts` para tipado en el editor (no va a runtime).
  - Cambiar un `.proto` -> regenerar `proto-descriptor.json` (paso dev/CI, rapido) -> deploy del JSON -> el server
    lo recarga (hot-reload del descriptor). NO es recompilar C# ni reiniciar el server.
  - Alternativa (si se quiere `.proto` texto en runtime): `protobuf.js` **full** dentro de Jint (trae parser),
    a costa de bundle mas pesado y el parser corriendo bajo Jint. Por defecto usar la ruta descriptor JSON.
- Deploy al server: `dist/gc.js` + `protobuf.min.js` (light) + `proto-descriptor.json`. Nada de Node/`node_modules`
  ni de los `.proto` crudos en runtime.
- **Los `.proto` SI se versionan en el repo** (`proto/`, fuente de verdad, editados en dev). Lo que NO se despliega al
  runtime del server son los `.proto` crudos: el server solo recibe el `proto-descriptor.json` derivado. Repo = tiene `.proto`; runtime server = no.
- **CI build-fail gates [Fable]:**
  - `tsc --noEmit` falla el pipeline.
  - `dist/gc.js` != salida de `esbuild` del TS actual -> **fail** (bundle desincronizado).
  - Handler migrado que referencia `DotaGcBackend.*Response` (o cualquier response builder C# prohibido) -> **fail** (lint/grep gate sobre la lista de cuarentena).
  - JS que llama `runtime.fixture` compilado en modo produccion -> **fail** (macro/flag de build).
  - **[Fable]** Cualquier codigo productivo (C# o JS/bundle) que referencie/lea `GC/570/fixtures` -> **fail** (grep gate sobre el arbol de prod).
  - Mensaje marcado "migrado" sin golden test asociado -> **fail**.

---

## 5. Fases

### Fase A - Contract Inventory & Quarantine [Fable, NUEVA, antes de todo]
Sin esto, la migracion repite el atajo.
1. **Inventario** de: `DotaGcBackend*` (474 miembros), `LuaGameCoordinatorBackend`, `messages.lua`, `main.lua` (tabla dispatch), `routes.generated.lua` (OBSERVED), `fixtures/nethook/*` y rutas actuales.
2. **Clasificar cada metodo de `DotaGcBackend`** en 5 cubos:
   - `host-plumbing` (transporte, cola, enqueue) -> se mantiene en C#.
   - `data-access` (DB reads/writes) -> se mantiene en C#, pero re-expuesto como DTO tipado.
   - `gc-logic` (reglas) -> **migra a JS**.
   - `response-builder` (construye protobuf) -> **migra a JS**; **prohibido** para ids migrados.
   - `fixture-replay` -> **dev/test only**; se elimina de prod.
3. **Lista de cuarentena** (archivo versionado, ej. `gc-js/quarantine.json`): nombres de metodos `response-builder`/`fixture-replay` que el CI prohibe llamar desde rutas migradas.
4. **ProtoMessageRegistry** generado: `messageId -> proto type name -> direction (c2s/s2c) -> handler`. Fuente unica, no ids dispersos. Deriva de `messages.lua`/`main.lua`/`.proto`.
5. **Enumerar y clasificar los N mensajes fixture-replay** [critico]. Un conjunto de message ids (generados
   desde `routes.generated.lua` `OBSERVED.client[...]`; ~94 al momento de escribir, pero el numero se deriva del
   archivo, no se hardcodea) existe HOY SOLO como replay de bytes capturados. Con "cero fixtures en prod",
   cada uno necesita implementacion real ANTES del cutover. Enumerar N desde `routes.generated.lua` y clasificar cada id en:
   - **catalogo-global**: mismos bytes para todos (store sales, emoticon defs, matchmaking stats, event schedule).
     -> builder desde tabla de datos/config + `Proto.encode`. El contenido puede sembrarse de la captura una vez,
     pero es un BUILDER, no replay runtime.
     - **por-jugador**: identidad/estado (ProfileCard, Rank, TeamInfo, quests, conduct). -> builder real desde DB.
   - **deprecado/vacio**: respuesta ausente o vacia explicita, verificando que el cliente lo tolera.
   Producir `gc-js/observed-migration.csv`: `msgId, protoType, bucket, sourceCapture, owner, status`.
   Ninguno queda como `runtime.fixture` en prod.
- **Gate:** inventario + clasificacion `DotaGcBackend` + clasificacion de los N OBSERVED + `quarantine.json` + `ProtoMessageRegistry` revisados y aprobados.

### Fase B - Router por mensaje [Fable, antes de Jint]
El registry actual escoge plugin por AppId; hay que cambiarlo ANTES de meter Jint.
- Introducir `GcEngineRouter` (o cambiar `IGameCoordinatorPlugin` a `CanHandle(context, request)`):
  decide motor por `(appId, messageType, direction, isGameServer)`.
- **Poll/tick tambien enrutados.** Eliminar el atajo `AppId==570 ? DotaGcBackend.Poll`. `Poll` consulta el router (para ids ya migrados, el estado vive en host/DB y lo sirve el motor JS; para no migrados, Lua/C#).
- Tabla explicita "ids migrados -> JS". Flag de rollback por id y global.
- **Gate:** router en produccion con 0 ids en JS -> comportamiento identico al actual (todo Lua/C#), incluido Poll.

### Fase 0 - Spike de de-risk [BLOQUEANTE]
- Jint + `protobuf.min.js` (light) + `proto-descriptor.json` (via `pbjs -t json`) en un test C# aislado.
- Cargar `.proto` real (empezar por `CMsgSOCacheSubscribed`/econ item con ids 64-bit).
- Probar `Proto.decode -> BigInt correcto -> Proto.encode -> bytes IDENTICOS` (round-trip contra fixture nethook).
- **[Fable] Cubrir tipos protobuf dificiles, no solo SOCache/econ:** `repeated fixed64`, `packed repeated`,
  `bytes`, `enum` con valor desconocido, y **mensajes anidados**. Cada uno con round-trip byte-exacto.
- **[Fable] Unknown fields como GATE:** round-trip binario de un mensaje Dota real **preservando campos desconocidos**. Si protobuf.js no los conserva como necesitamos -> implementar preservacion propia o estrategia hibrida (guardar bytes crudos de campos no reconocidos y re-emitir). No avanzar sin resolver esto.
- Medir perf por mensaje.
- **Gate:** round-trip byte-exacto + BigInt + **unknown fields preservados** + perf aceptable. Si falla -> replantear (Long.js vs BigInt bajo Jint; preservacion custom; o plan B C#+protobuf-net, que rompe hot-reload).
- Entregable: `spikes/gc-jint-protobuf/` + README de resultados.

### Fase 1 - Plomeria del host JS (sin logica GC)
- `JintGcEngine` integrado al `GcEngineRouter`. Sandbox: sin `fs`/`process`/red/timers salvo wrappers explicitos del contrato Sec.3.
- **Limites Jint [Fable]:** timeout por mensaje, limite de memoria/bytes, cancelacion. Logging de excepcion con: handler name, route decision, fallback reason.
- **Aislamiento:** throw en JS -> log + `notHandled(reason)` -> fallback, nunca tumba el server.
- **Hot-reload correcto [Fable]:** watch de `dist/gc.js` + `proto-descriptor.json`; recargar engine SIN perder estado (estado en host/DB, no en globals JS).
  **Alcance del hot-reload:** solo logica JS + `proto-descriptor.json` + datos GC se recargan sin reiniciar.
  Cambios en el **C# host** (router, contrato, plomeria, DTOs) SI requieren recompilar y reiniciar el server; eso es esperado y aceptable (el host es la parte estable). La regla es que iterar reglas/mensajes GC NO requiera restart.
- Implementar el contrato Sec.3 marshalando a la plomeria existente (`ApiGCExchangeResponse.Messages`, `GameCoordinatorPendingMessages`).
- **Gate:** handler JS trivial (`handle` -> `gc.notHandled("noop")`) carga, ejecuta, hot-reloadea; server sirve Dota igual (todo cae a fallback).

### Fase 2 - Codec protobuf (`.proto` fuente de verdad)
- Curar `.proto` de Dota GC (protobufs comunidad 570) -> `proto/`.
- Pipeline dev/CI: `pbjs -t json proto/*.proto -> proto-descriptor.json`; deploy del descriptor (no de los `.proto`).
- `Proto.encode/decode(name, ...)` via `Root.fromJSON(proto-descriptor.json)` con BigInt + unknown fields (segun resolucion de Fase 0).
- uint64 policy en el host (BigInt <-> string <-> ulong) documentada.
- **Gate:** N mensajes clave (login, SOCache, lobby) round-trip byte-exacto contra fixtures.

### Fase 3 - Golden tests por ESCENARIO con estado [Fable, ampliado]
No basta "input bin -> output bin"; el GC tiene secuencias con estado. Testear escenarios completos:
- login, SOCache, equip items, lobby create/update, party, dedicated claim, match signout, historial, remote player con cosmeticos.
- Cada test compara: **mensajes, orden, job IDs, payloads, estado de DB y mensajes diferidos (pending)**.
- Dataset base desde `routes.generated.lua OBSERVED` + `fixtures/nethook/*.bin`, mas escenarios con estado escritos a mano.
- Runner en CI; falla si algun id migrado difiere en cualquier dimension.
- **Gate:** harness verde con 0 handlers migrados (baseline), y capaz de validar escenarios con estado.

### Fase 4 - Migracion incremental (reclamar logica de `DotaGcBackend`)
Orden por dependencia:
1. **Login/handshake:** `GCClientHello -> ClientWelcome`, `SOCacheSubscribed` base. (Reclama `ClientHello` + SO cache building.)
2. **Econ/SO cache items:** `SOCacheSubscribed/Updated`, econ por jugador. (Reclama `BuildEconItemsForUser/BuildEconItem/QueueEconItemChangesToServer`.)
3. **Lobby:** create/update/leave, `PracticeLobbyResponse`, slots. (`dota_lobby.lua` + partes de `DotaGcBackend`.)
4. **Party:** `DotaGcBackend.Party.cs` + `dota_party.lua`.
5. **Stats/perfil:** ProfileCard, Rank, MatchStats. (`DotaGcBackend.Stats.cs`, 60 KB.)
6. **Cola larga (los N mensajes fixture-replay):** subfase propia, NO opcional. Trabajar la tabla
   `observed-migration.csv` de Fase A, bucket por bucket:
   - **catalogo-global** -> builder desde tabla de datos (`.proto` + config sembrada de la captura), `Proto.encode`.
   - **por-jugador** -> builder real desde DB (DTOs tipados).
   - **deprecado/vacio** -> respuesta vacia/ausente explicita.
   Golden test por id (bytes == captura para catalogo-global; escenario con estado para por-jugador).
   **Ninguno queda como `runtime.fixture` en prod.** El fixture pasa a ser solo oraculo de golden test.

Por handler: escribir `.ts` -> construir objeto -> `Proto.encode`; golden verde (todas las dimensiones) antes de activar el id en el router; borrar la ruta Lua/C# equivalente solo cuando el id esta estable; vaciar `DotaGcBackend` de esa logica.

### Fase 5 - Cutover y limpieza
- Todos los ids en JS y verdes -> apagar Lua, borrar `GC/570/*.lua`, `routes.generated.lua`.
- Adelgazar `DotaGcBackend` a solo host/data-access (o eliminarlo). Quitar `ProtoVarint/Fixed64String/Bytes/Count` y el hand-encoding. Quitar el `Poll` directo.
- Quitar hacks JSON de MoonSharp (`JsonToTable/TableToJson/LuaSafeJsonOptions`).
- Fixtures pasan a suite de regresion (dev/test).
- **Gate:** partida completa con cosmeticos, sin Lua ni GC-logic/response-builders en C#.

---

## 6. Cross-cutting

- **DTOs tipados [Fable]:** `SteamDB`/`DotaDB`/`DedicatedServerService` exponen objetos/DTOs a JS, NO JSON string. C# = acceso a datos; JS = logica.
- **Estado [Fable]:** lobby/session/match/party viven en host/DB. El engine JS es stateless entre recargas.
- **Versionado por parche Dota:** `proto/` y handlers versionables; hot-reload para parchear en vivo cuando Valve cambia el GC.
- **Sandbox/seguridad:** solo el contrato Sec.3; limites Jint (timeout, memoria, cancelacion).
- **Tracing:** reusar `GameCoordinatorTraceService`; log por (appId, steamId, msgId, engine, route decision, fallback reason).

---

## 7. Criterio "no repetir el atajo" [Fable] - la migracion es sana solo si:
1. JS maneja rutas GC reales, **no** fixtures.
2. `.proto` es la fuente de verdad.
3. `DotaGcBackend` ya **no** construye respuestas GC migradas.
4. Poll/tick/pending pasan por el **mismo router** JS.
5. Dota puede: login, crear sala no-local, lanzar dedicado, conectar remoto con cosmeticos, terminar partida y guardar historial.
6. Golden tests comparan mensajes, orden, job IDs, payloads, estado DB y mensajes diferidos.

## 8. Build-fail gates (resumen CI) [Fable]
- Handler migrado llama `DotaGcBackend.*Response` (o cualquier item de `quarantine.json`) -> fail.
- JS usa `runtime.fixture` en modo produccion -> fail.
- Codigo productivo (C#/JS) lee `GC/570/fixtures` -> fail.
- `dist/gc.js` != transpile del TS actual -> fail.
- Id marcado "migrado" sin golden test -> fail.
- `tsc --noEmit` con errores -> fail.

## 9. Riesgos y mitigaciones

| Riesgo | Mitigacion |
|---|---|
| protobuf.js no preserva unknown fields como se necesita | Gate en Fase 0; preservacion custom/hibrida si falla |
| BigInt de protobuf.js no funciona bajo Jint | Spike bloqueante; Long.js<->string o replantear |
| Router actual solo por AppId | Fase B: `GcEngineRouter` por (context, messageType), Poll incluido, ANTES de Jint |
| Estado GC escondido en C# (Poll, DotaGcBackend) | Fase A cuarentena + Fase B router + estado en host/DB |
| Migrar y esconder hardcodes en JS | quarantine.json + build-fail gates + golden por escenario con estado |
| Fixtures como logica en prod | Prohibido por flag de build; solo dev/test |
| DB como JSON string | DTOs tipados antes de migrar handlers que la usan |
| Perf Jint en SO cache masivo | Cachear proto compilado; medir en spike |

## 10. Entregables por fase (checklist)
- [ ] A: inventario + clasificacion de `DotaGcBackend` + clasificacion de los N OBSERVED (`observed-migration.csv`) + `quarantine.json` + `ProtoMessageRegistry`.
- [ ] B: `GcEngineRouter` por mensaje (+ Poll enrutado, sin atajo 570), flags de rollback.
- [ ] 0: `spikes/gc-jint-protobuf/` con round-trip byte-exacto + unknown fields + perf.
- [ ] 1: `JintGcEngine`, sandbox+limites, hot-reload sin perder estado, toolchain TS/esbuild, CI gates.
- [ ] 2: `proto/` + `proto-descriptor.json` (pbjs), `Proto.encode/decode` via Root.fromJSON (BigInt + unknown fields), uint64 policy.
- [ ] 3: golden por escenario con estado (mensajes/orden/jobIds/payloads/DB/pending).
- [ ] 4: handlers migrados (login->econ->lobby->party->stats), cada uno con golden verde y ruta vieja borrada.
- [ ] 4.6: los N mensajes fixture-replay implementados como builders (catalogo-global/por-jugador/vacio); 0 `runtime.fixture` en prod.
- [ ] 5: cutover, Lua + GC-logic C# eliminados, hacks MoonSharp fuera, partida completa validada.

---

## Notas para el implementador
- Grafo del repo en `.codex/graphify/` -> `DotaGcBackend` es god node (276 edges): el punto a vaciar.
- Espejar la plomeria del backend Lua (`ApiGCExchangeResponse.Messages`, `GameCoordinatorPendingMessages`), distinto motor.
- No romper el flujo `IGameCoordinatorPlugin`; el router nuevo convive con Lua/C# hasta el cutover.
- `steam_api` (DLL cliente) NO cambia: esto es puramente server-side GC.
- Encoding: este archivo esta en UTF-8 sin BOM y sin caracteres acentuados problematicos, para que otro agente no lo lea con mojibake.

## Importante
Al finalizar haces una revision y si pasa haces commit y subes los cambios a github
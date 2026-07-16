# SteamInventory — Rediseño completo (TODO v2)

> Revisión tras auditoría de ABI. El problema **no** está solo en `SteamInventory.cs`: está en
> marshalling de vtable, exports flat, tipos base y ausencia total de callbacks/persistencia.
> Esto es un rediseño de interfaz completo, no un "rellenar la impl".

## Estado real (auditado)

Cuatro capas, todas rotas de distinta forma:

1. **Impl** (`Steamworks/Implementation/SteamInventory.cs`) — stub: todo `false`/`0`. Sin store, sin
   handles, sin callbacks. `GetResultStatus` devuelve `k_EResultOK` para cualquier handle (peligroso).
2. **Vtable** (`Interfaces/SteamInventory002.cs`, `003.cs`) — firmas nativas OK (IntPtr) pero
   **descartan out-params**: `WriteUInt32(punSize,0)` + pasan `0` a la impl → two-call imposible.
3. **Exports flat DllExport** (`Exported/SteamAPI_ISteamInventory.cs`) — **rotos en x64**:
   - out-handle pasado por valor `SteamInventoryResult_t`(uint) en `GetAllItems`, `GenerateItems`,
     `GrantPromoItems`, `AddPromoItem(s)`, `ConsumeItem`, `TransferItemQuantity`, `TriggerItemDrop`,
     `InspectItem`, `SubmitUpdateProperties`, `DeserializeResult`, `TradeItems` → el juego pasa un
     `SteamInventoryResult_t*`; recibirlo como `uint` **trunca el puntero de 64 bits** y no puede escribir.
   - `GetItemPrice(..., ulong pCurrentPrice, ulong pBasePrice)` y `GetItemsWithPrices(..., ulong pBasePrices, ...)`
     → punteros de salida por valor. Rotos.
   - size in/out (`punOutItemsArraySize`, `punValueBufferSizeOut`, `punOutBufferSize`,
     `punItemDefIDsArraySize`) pasados como `uint` por valor → two-call imposible.
   - `TransferItemQuantity(uint itemIdSource, uint unQuantity, uint itemIdDest)` → **item IDs de 64 bits
     truncados a uint**. `RemoveProperty(ulong handle, uint nItemID,...)` → nItemID truncado.
   - `ExchangeItems(ref SteamItemDef_t[] ...)` → arrays *managed* por ref sobre ABI nativo. Roto.
   - `GetItemsByID(ref SteamItemInstanceID_t pInstanceIDs,...)` → `ref` a un solo valor donde es array.
   - `DestroyResult`/`SendItemDropHeartbeat` exports **ni llaman a la impl** (no-op vacío).
4. **Flat wrapper** (`Exported/CSteamworks.cs` `ISteamInventory_*` y `ISteamGameServerInventory_*`) —
   usa `ref` correcto en varios, pero arrastra `SteamInventoryResult_t = uint`, arrays managed por ref
   (variantes comentadas), y **duplica todo para GameServer** (hay que corregir ambos).

**Divergencia v002 vs v003 (confirmada):**
- v003 `GetItemsWithPrices(defs, pCurrentPrices, pBasePrices, len)` — 4 args.
- v002 `GetItemsWithPrices(defs, pPrices, len)` — 3 args (un solo array de precios).
- v003 `GetItemPrice(def, pCurrentPrice, pBasePrice)`; v002 `GetItemPrice(def, pPrice)`.
- La impl debe exponer variantes o normalizar; **no** asumir una sola firma.

---

## 1. Tipos base (compatibles con SDK) — corregir typedefs en TODAS las capas

Reemplazar los `using` en `SteamInventory.cs`, `002/003.cs`, `SteamAPI_ISteamInventory.cs`,
`CSteamworks.cs` (y variantes GameServer):

| Tipo | Correcto | Inválido |
|---|---|---|
| `SteamInventoryResult_t` | **`int`** | **`-1`** (`k_SteamInventoryResultInvalid`) |
| `SteamItemDef_t` | **`int`** | — |
| `SteamItemInstanceID_t` | `ulong` | `0xFFFFFFFFFFFFFFFF` |
| `SteamInventoryUpdateHandle_t` | `ulong` | `0xFFFFFFFFFFFFFFFF` |
| cantidad interna | `uint` | castear a `ushort` solo al llenar `SteamItemDetails_t` |

Impacto: `GetResultStatus`/handles nunca deben devolver 0 como inválido; usar `-1`. Handle 0 **es válido**
en el modelo SDK (no reservarlo como "vacío" — reservar `-1`).

---

## 2. `SteamItemDetails_t` — layout validado (no solo "público")

`Types/SteamTypes.cs`: campos hoy `private`. Cambiar a:

```csharp
[StructLayout(LayoutKind.Sequential)]   // secuencial; NO forzar Pack=1 a ciegas
public struct SteamItemDetails_t
{
    public SteamItemInstanceID_t m_itemId;   // ulong  (8)
    public SteamItemDef_t        m_iDefinition; // int (4)
    public ushort                m_unQuantity;  // (2)
    public ushort                m_unFlags;     // (2)  ESteamItemFlags
}
```
**Test de tamaño obligatorio**: `Marshal.SizeOf<SteamItemDetails_t>() == 16`. Si difiere, ajustar
(el orden ulong→int→ushort→ushort da 16 con alineación natural). El harness de §11 valida esto.

`ESteamItemFlags`: `NoTrade=1<<0`, `Removed=1<<8`, `Consumed=1<<9`.

---

## 3. Corrección de marshalling por capa (REQUERIDO antes de la impl)

Regla única: **todo puntero de salida = `IntPtr` o `ref` real; nunca por valor.** Aplicar a las 4 capas.

### 3a. Exports flat `SteamAPI_ISteamInventory.cs`
- out-handle: `SteamInventoryResult_t pResultHandle` → **`IntPtr pResultHandle`**; la impl escribe con
  `Marshal.WriteInt32`.
- size in/out: `uint punX` → **`IntPtr punX`**.
- precios: `ulong pCurrentPrice/pBasePrice` → **`IntPtr`**.
- item IDs: `uint itemIdSource/itemIdDest`, `uint nItemID` → **`ulong`** (`SteamItemInstanceID_t`).
- `ExchangeItems`: firmar con `IntPtr` para los 4 arrays (defs/qtys/destroy/qtys) + `IntPtr pResultHandle`;
  leer con `Marshal.Copy` a `int[]`/`ulong[]`/`uint[]`.
- `GetItemsByID`: `ref SteamItemInstanceID_t` → **`IntPtr pInstanceIDs`** + `uint unCount`.
- `DestroyResult`, `SendItemDropHeartbeat`: **llamar a la impl** (hoy son no-op vacíos).
- Mantener `[DllExport(Cdecl)]`; SetProperty0/1/2 mapean a las sobrecargas tipadas.

### 3b. Vtable `SteamInventory002.cs` / `003.cs`
- Dejar de escribir `0` y descartar. **Pasar los IntPtr crudos** a la impl:
  ```csharp
  public bool GetResultItems(IntPtr _, int h, IntPtr pOut, IntPtr punSize)
      => SteamEmulator.SteamInventory.GetResultItems(h, pOut, punSize);
  ```
  Igual: `GetResultItemProperty`, `SerializeResult`, `GetItemDefinitionIDs`,
  `GetItemDefinitionProperty`, `GetEligiblePromoItemDefinitionIDs`, `GetItemsWithPrices`, `GetItemPrice`.
- `pResultHandle` (out del handle) también se pasa a la impl para que escriba el handle real.
- **v002 vs v003**: dos wrappers distintos para `GetItemsWithPrices`/`GetItemPrice` que llaman a
  métodos impl separados (`GetItemsWithPrices_V2` de un solo array vs `_V3` current+base).

### 3c. Flat wrapper `CSteamworks.cs` — `ISteamInventory_*` **y** `ISteamGameServerInventory_*`
- Cambiar `SteamInventoryResult_t = uint` → `int`.
- Eliminar las variantes de array managed por `ref` (usar `IntPtr` + `Marshal.Copy`).
- Ambos conjuntos (cliente y GameServer) apuntan a la misma impl; GameServer usa el store del server-side owner.

---

## 4. Callbacks y call results (sección crítica que faltaba)

Inventory es async. Toda op que produce handle **debe** completarse por `CallbackManager`. Structs ya
existen en `Callback/CallbackTypes.cs`.

| Disparador | Callback / CallResult | Cuándo |
|---|---|---|
| `GetAllItems`, `GetItemsByID`, `GenerateItems`, `Grant/AddPromo`, `Consume`, `Exchange`, `Transfer`, `TriggerItemDrop`, `SubmitUpdateProperties`, `DeserializeResult`, `InspectItem` | `SteamInventoryResultReady_t{Handle,Result}` | al completar el result (via `CallbackManager.AddCallback`) |
| Cualquier mutación del store (full snapshot nuevo) | `SteamInventoryFullUpdate_t{Handle}` | tras persistir |
| `LoadItemDefinitions` (async) | `SteamInventoryDefinitionUpdate_t` | tras cargar manifest |
| `RequestEligiblePromoItemDefinitionsIDs(steamID)` | **retorna `SteamAPICall_t`** → `SteamInventoryEligiblePromoItemDefIDs_t{Result,SteamID,num,CachedData}` | via `CallbackManager.AddCallbackResult` |
| `RequestPrices()` | **retorna `SteamAPICall_t`** → `SteamInventoryRequestPricesResult_t{Result,Currency}` | idem |
| `StartPurchase(defs,qtys)` | **retorna `SteamAPICall_t`** → `SteamInventoryStartPurchaseResult_t{Result,OrderID,TransID}` | idem |

Reglas:
- `RequestPrices`/`StartPurchase`/`RequestEligiblePromo...` devuelven `SteamAPICall_t` (no bool/handle);
  `k_uAPICallInvalid` solo en error de argumentos.
- El result-handle se emite **de inmediato** (síncrono) para que `pResultHandle` sea válido; el
  `ResultReady_t` se postea **después** desde el `WorkQueue` (patrón async real). Un juego que sondea
  `GetResultStatus` verá `k_EResultPending` hasta que se complete.
- Usar `EnqueueCallbackResult` de `WorkQueue` donde aplique para atar el call result al trabajo.

---

## 5. Estado autoritativo — `Dictionary` bajo lock único (no mezclar)

`InventoryManager` (nuevo, `Managers/Inventory/InventoryManager.cs`), singleton estático.

```
// AUTORITATIVO: Dictionary + un solo lock. Nunca ConcurrentDictionary aquí.
private static readonly object StoreLock = new object();
private static readonly Dictionary<ulong, InventoryItem> Items = new();     // por ItemId
private static readonly Dictionary<int, ItemDefinition>  Definitions = new();

// TEMPORAL/efímero: concurrente OK.
private static readonly ConcurrentDictionary<int, InventoryResult> Results = new();
private static readonly ConcurrentDictionary<ulong, PropertyUpdate> Updates = new();
```
Toda operación compuesta (leer-validar-mutar-persistir) ocurre **entera** bajo `StoreLock`. Snapshots
copian a array inmutable bajo el lock y se sueltan. Los `Results` y update-handles, al ser efímeros y
por-handle, sí pueden ser concurrentes.

---

## 6. Modelo de datos

### 6a. `InventoryItem`
```
ulong  ItemId
int    DefId
uint   Quantity              // interno uint; cast a ushort al marshalar
ushort Flags                 // ESteamItemFlags
uint   AcquiredUnix
Dictionary<string,string> Properties
```

### 6b. `ItemDefinition` — flexible (no rígido)
Fuente de verdad = JSON raw / `Dictionary<string,string>`; campos tipados **derivados** on-demand:
```
int    DefId
Dictionary<string,string> Raw     // todas las props arbitrarias (fuente de verdad)
// derivados (lazy desde Raw):
string Type      => Raw["type"]
bool   Promo     => Raw["promo"] == "true"
ulong  PriceCents=> parse Raw["price"]
bool   Tradable, Marketable
List<Recipe> Exchange   // parse de Raw["exchange"]
List<Drop>   DropList
```
`GetItemDefinitionProperty(def, null, ...)` → devuelve **lista de nombres** de props (CSV) — comportamiento
requerido por el SDK cuando `pchPropertyName == null`.

### 6c. `InventoryResult`
```
int     Handle              // >=0 ; inválido = -1
EResult Status              // Pending -> OK/Fail
uint    TimestampUnix
ulong   OwnerSteamID
InventoryItem[] Items       // snapshot inmutable
byte[]  SerializedBlob      // cache lazy (§8)
```

---

## 7. Persistencia (incluye `NextItemId`)

Local-first, disco autoritativo. Directorio `SKYNET/Inventory/<appid>/`.

- `items.json` — array de `InventoryItem`.
- `meta.json` — **`{ NextItemId, NextResultHandle?, Version }`**. Persistir el contador de IDs: no
  derivarlo del steamID ni del max actual (tras borrar+reiniciar puede **colisionar** ids reusados).
  Cargar `NextItemId` de `meta.json`; `Interlocked`/lock al incrementar; persistir meta en cada bump
  (o batch al persistir items).
- `defs.json` — manifest de definiciones (o cargado del server).
- Escritura atómica: reusar `.tmp`+`File.Replace` (patrón de RemoteStorage).

---

## 8. Serialize/Deserialize con firma real (HMAC, no CRC)

`SerializeResult` produce blob **firmado**; `DeserializeResult` lo valida.

Formato:
```
[magic "SKIV"][u16 version][u32 appid][u64 owner][u64 timestamp][u32 count]
[ items: u64 itemId, i32 defId, u16 qty, u16 flags, propsLen+props ]...
[ HMACSHA256(todo lo anterior, secret) ]   // 32 bytes
```
- `secret`: por instalación, en `SerializeSecretPath` (generado aleatorio la 1ª vez, 32 bytes).
- **Validación** en Deserialize: magic, version soportada, appid coincide, HMAC válido, (opcional) TTL
  de `timestamp`. Si falla cualquiera → **crear un `InventoryResult` con `Status=k_EResultFail`** y
  escribir su handle, **no** simplemente `return false` (el SDK espera un handle consultable).
- `SerializeResult` two-call: buffer null → escribe tamaño; buffer → escribe blob.

---

## 9. Server sync — identidad por sesión (Bearer), no `{steamId}` en cliente

Solo si `SyncMode != Local`. Local-first sigue autoritativo si el server no está.

### 9a. Rutas cliente (server deriva `SteamId`+`AppId` del Bearer, como RemoteStorage)
```
GET /api/inventory/items      # inventario del owner del token
PUT /api/inventory/items      # commit
GET /api/inventory/defs       # manifest de definiciones del appid del token
GET /api/inventory/prices     # precios (currency por query)
```
Rutas `/{appId}/{steamId}` **solo** para admin/debug (autenticadas como admin).

### 9b. `APIClient` (nuevo, patrón del wrapper de RemoteStorage — enum de resultado, `quietStatusCode` 404,
sin tocar `Send<T>` global):
```
InventoryDownloadResult GetInventory(out ApiInventorySnapshot snap, int timeoutMs)
bool CommitInventory(items)
List<ItemDefinition> GetItemDefinitions()
List<ItemPrice> GetItemPrices(currency)
```

### 9c. SKYNET server: `InventoryController` + store EF (SQLite ya en uso). Deriva identidad del token.
Admin: ver/editar inventario por (appId, steamId), cargar manifest de defs. Ops sensibles
(Exchange/Consume/Grant) validadas server-side cuando `SyncMode=Server`.

---

## 10. Config real `[Inventory]` (cargar en `Settings.Load()` + defaults en `SteamEmulator` static ctor)

```
[Inventory]
Enabled = true
DefinitionsPath = SKYNET/Inventory/<appid>/defs.json
StorageMode = Local            # Local | Server | Mirror
SyncMode = Off                 # Off | Pull | PushPull
SerializeSecretPath = SKYNET/Inventory/serialize.key
AutoGrantPurchases = true      # StartPurchase concede sin pago real
AutoGrantPromos = true
Currency = USD
AllowGenerate = false          # GenerateItems solo sandbox
```
Añadir los campos a `SteamEmulator` (static fields + carga en `Settings`), igual que se hizo con
`EnableVoiceCapture`/networking.

---

## 11. Verificación — marshalling real, no solo compilar

Harness dedicado (test host que hace P/Invoke a la DLL exportada con buffers **unmanaged reales**):

1. **Tamaño de struct**: `SteamItemDetails_t == 16` bytes; round-trip StructureToPtr/PtrToStructure.
2. **Two-call sizing**: `GetResultItems`/`GetItemDefinitionIDs`/`GetResultItemProperty`/`SerializeResult`
   con buffer `null` → count/size correcto; luego con buffer del tamaño → contenido correcto.
3. **Punteros x64**: llamar los **flat exports** (`SteamAPI_ISteamInventory_*`) y **vtable** con
   `Marshal.AllocHGlobal` para cada out (handle/size/price) y verificar que se escribió (no truncado).
4. **Callbacks/call results**: `RequestPrices`/`StartPurchase`/`RequestEligiblePromo...` → llega el
   call result; ops de mutación → `ResultReady_t` + `FullUpdate_t`; `LoadItemDefinitions` → `DefinitionUpdate_t`.
5. **Persistencia tras reinicio**: generar items, reiniciar, `GetAllItems` los conserva; `NextItemId`
   no reusa ids (crear, borrar todo, crear → id nuevo distinto).
6. **Blob corrupto/expirado** en `DeserializeResult` → handle con `Status=Fail` (no crash, no `false` seco).
7. **v002 vs v003**: `GetItemsWithPrices`/`GetItemPrice` probados en **ambas** firmas.
8. **Handle inválido**: `GetResultStatus(-1)` y de un handle nunca emitido → `k_EResultInvalidParam`.
9. Sin residuos `.tmp`; sin crash con arrays/ buffers null; item IDs de 64 bits intactos en Transfer/Consume.

---

## 12. Orden de implementación

1. Tipos base (§1) + `SteamItemDetails_t` (§2) + test de tamaño.
2. Corrección de marshalling en las 4 capas (§3) — antes de cualquier lógica; deja la ABI sana.
3. `InventoryManager`: store `Dictionary`+lock (§5), modelo (§6), persistencia+`NextItemId` (§7).
4. Result lifecycle + callbacks/call results (§4).
5. Reescribir `SteamInventory.cs` método a método sobre la ABI corregida.
6. Serialize/Deserialize HMAC (§8).
7. Definiciones flexibles + LoadItemDefinitions + drops/promos/prices (§6b).
8. Config `[Inventory]` (§10).
9. (Opcional) Server sync identidad-por-token (§9).
10. Harness de marshalling (§11); build VS18 + deploy win64; commit.

---

## Nota Dota

Dota **no** usa `ISteamInventory` (cosméticos = GC econ). Esta implementación existe para que el
emulador sea genérico y potente para juegos que sí dependen de Inventory Service. Para Dota basta con
que no crashee. **Fix mínimo inmediato** si no se implementa todo ya: (a) `GetResultStatus` de handle
desconocido → `k_EResultInvalidParam`; (b) corregir los exports flat rotos en x64 (§3a) para que no
trunquen punteros aunque la impl siga siendo no-op — un puntero truncado escrito por el juego puede
corromper su stack.

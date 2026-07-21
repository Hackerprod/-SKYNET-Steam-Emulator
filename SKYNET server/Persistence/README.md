# Persistence layout

The server has two live SQLite databases:

- `steam.db` owns Steam/server state: users, credentials, friendships, Steam stats, achievements, remote storage, web sessions, avatars, and generic server state.
- `dota.db` owns app 570/Game Coordinator state: lobbies, game servers, Dota cosmetics, Dota equipment, Dota live-match cache, and Dota-specific user/game data.

Game Coordinator code resolves Steam identity and relationships from `steam.db`; it does not duplicate that ownership in `dota.db`.

## EF Core and SQL stores

Canonical durable aggregates are modeled through EF Core in `SteamDbContext` and `DotaDbContext`. JSON value objects stored by EF use the shared compatibility serializer so old persisted shapes can still be read when fields are added, removed, or represented as strings/nulls.

Some Dota GC stores still use explicit SQLite commands because their schemas are specialized and hot-path oriented. Those stores must remain behind typed service classes, use parameters for all values, and keep schema creation/migration inside the owning store or the shared schema maintenance layer. New durable state should prefer EF Core unless a domain-specific store has a clear reason to own raw SQL.

## Schema compatibility

`DatabaseSchemaMaintenance` runs after EF creates the base model and before state is loaded. Each database has a `SchemaVersions` table. Compatibility fixes should be added as idempotent versioned migrations there instead of scattered `CREATE TABLE` or `ALTER TABLE` calls.

This replaces the old Mongo behavior deliberately:

- extra JSON fields are ignored by the serializer;
- missing fields keep CLR defaults;
- numeric ids can be read from JSON numbers, strings, empty strings, or nulls;
- unsigned Steam/Dota ids remain explicit in the C# model and are converted at the EF boundary.

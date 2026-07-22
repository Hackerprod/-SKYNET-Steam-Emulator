// Shared Object owner ids. These identify the SO cache namespace
// (Steam account, party group, lobby group or invite-owned cache).
export const SO_OWNER_STEAM_ID = 1;
export const SO_OWNER_PARTY_GROUP = 2;
export const SO_OWNER_LOBBY_GROUP = 3;
export const SO_OWNER_INVITE = 4;

// Dota SO cache services. Service 0 carries game/lobby/account objects;
// service 1 carries econ inventory/account objects.
export const DOTA_SO_SERVICE_GAME = 0;
export const DOTA_SO_SERVICE_ECON = 1;

// Econ SO type ids. These are the type_id values used inside
// CMsgSOCacheSubscribed/CMsgSOMultipleObjects buckets.
export const DOTA_SO_ECON_ITEM = 1;
export const DOTA_SO_ECON_GAME_ACCOUNT_CLIENT = 7;

// Dota game SO type ids from the generated protobuf/SteamKit mapping.
export const DOTA_SO_GAME_ACCOUNT_CLIENT = 2002;
export const DOTA_SO_PARTY = 2003;
export const DOTA_SO_LOBBY = 2004;
export const DOTA_SO_PARTY_INVITE = 2006;
export const DOTA_SO_PLAYER_CHALLENGE = 2010;
export const DOTA_SO_LOBBY_INVITE_CANONICAL = 2011;
export const DOTA_SO_GAME_ACCOUNT_PLUS = 2012;
export const DOTA_SO_DYNAMIC_LOBBY = 2013;
export const DOTA_SO_STATIC_LOBBY = 2014;
export const DOTA_SO_SERVER_LOBBY = 2015;
export const DOTA_SO_SERVER_STATIC_LOBBY = 2016;

// Protocol versions used by the GC welcome/SO cache handshake.
export const DOTA_GC_SOCACHE_FILE_VERSION = 20;

// The working lobby invite flow currently uses the 2013 cache bucket. Keep this
// value separate from DOTA_SO_LOBBY_INVITE_CANONICAL so we can document the
// canonical mapping without changing the validated lobby wire flow.
//
// Keep this as a direct literal. The TypeScript runtime may evaluate imported
// const-to-const aliases before the referenced export is materialized, which can
// turn the SO type into 0 in the generated cache subscription.
export const DOTA_SO_WORKING_LOBBY_INVITE_CACHE = 2013;

import { DotaProfileSnapshot } from "../../framework/gc";

// Profile stat ids shared by the profile card slots (7535/8035) and the
// profile showcase stat items (8887). The client sends only the stat id; the
// score is always computed here from the account's persisted data.
export const PROFILE_STAT_WINS = 3;
export const PROFILE_STAT_COMMENDS = 4;
export const PROFILE_STAT_GAMES_PLAYED = 5;
export const PROFILE_STAT_FIRST_MATCH = 6;
export const PROFILE_STAT_MVPS = 8;

export function profileStatScore(snapshot: DotaProfileSnapshot, statId: number): number {
    if (statId === PROFILE_STAT_WINS) {
        return snapshot.globalStats.gamesWon;
    }

    if (statId === PROFILE_STAT_COMMENDS) {
        return snapshot.conduct.commendCount;
    }

    if (statId === PROFILE_STAT_GAMES_PLAYED) {
        return snapshot.globalStats.matchCount;
    }

    if (statId === PROFILE_STAT_FIRST_MATCH) {
        return snapshot.firstMatchTime;
    }

    if (statId === PROFILE_STAT_MVPS) {
        return snapshot.mvpCount;
    }

    return 0;
}

// Resolves the account a profile-style request targets: an explicit non-zero
// account id wins, otherwise the request is about the caller.
export function requestedAccountId(value: number | undefined, fallback: number): number {
    const accountId = value ?? 0;
    if (accountId === 0) {
        return fallback;
    }

    return accountId;
}

import { DotaConduct, DotaConductScorecard } from "../../framework/gc";

const BASE_BEHAVIOR_SCORE = 10000;
const MIN_BEHAVIOR_SCORE = 0;

export function behaviorScore(value: number | undefined, hasConductSignals = false): number {
    // New accounts enter the GC contract with full conduct. Stored reports,
    // abandons and commends are the only signals that should move it away from
    // 10000.
    if (value === undefined || (value === MIN_BEHAVIOR_SCORE && !hasConductSignals)) {
        return BASE_BEHAVIOR_SCORE;
    }

    return clamp(value, MIN_BEHAVIOR_SCORE, BASE_BEHAVIOR_SCORE);
}

export function normalizeConduct(conduct: DotaConduct | null | undefined): DotaConduct {
    if (conduct === null || conduct === undefined) {
        return {
            commendCount: 0,
            rawBehaviorScore: BASE_BEHAVIOR_SCORE,
            reportsCount: 0,
            matchesAbandoned: 0,
            commsReports: 0
        };
    }

    const hasConductSignals =
        optionalCount(conduct.reportsCount) > 0 ||
        optionalCount(conduct.matchesAbandoned) > 0 ||
        conduct.commendCount > 0 ||
        optionalCount(conduct.commsReports) > 0;
    return {
        commendCount: conduct.commendCount,
        rawBehaviorScore: behaviorScore(conduct.rawBehaviorScore, hasConductSignals),
        reportsCount: conduct.reportsCount,
        matchesAbandoned: conduct.matchesAbandoned,
        commsReports: conduct.commsReports
    };
}

export function normalizeConductScorecard(scorecard: DotaConductScorecard): DotaConductScorecard {
    const hasConductSignals =
        scorecard.reportsCount > 0 ||
        scorecard.matchesAbandoned > 0 ||
        scorecard.commendCount > 0 ||
        scorecard.commsReports > 0;
    const rawBehaviorScore = behaviorScore(scorecard.rawBehaviorScore, hasConductSignals);
    const oldRawBehaviorScore = behaviorScore(scorecard.oldRawBehaviorScore, hasConductSignals);
    return {
        accountId: scorecard.accountId,
        matchId: scorecard.matchId,
        seqNum: scorecard.seqNum,
        reasons: scorecard.reasons,
        matchesInReport: scorecard.matchesInReport,
        matchesClean: scorecard.matchesClean,
        matchesReported: scorecard.matchesReported,
        matchesAbandoned: scorecard.matchesAbandoned,
        reportsCount: scorecard.reportsCount,
        reportsParties: scorecard.reportsParties,
        commendCount: scorecard.commendCount,
        date: scorecard.date,
        rawBehaviorScore,
        oldRawBehaviorScore,
        commsReports: scorecard.commsReports,
        commsParties: scorecard.commsParties,
        behaviorRating: scorecard.behaviorRating
    };
}

function optionalCount(value: number | undefined): number {
    if (value === undefined) {
        return 0;
    }

    return value;
}

function clamp(value: number, min: number, max: number): number {
    if (value < min) {
        return min;
    }

    if (value > max) {
        return max;
    }

    return value;
}

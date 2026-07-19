import { DotaMatch, DotaMatchPlayer } from "../../framework/gc";
import { CMsgDOTAMatch, CMsgDOTAMatch_Player } from "../../generated/dota";

const MATCH_OUTCOME_RADIANT = 2;
const MATCH_OUTCOME_DIRE = 3;

export function mapMatchDetails(match: DotaMatch): CMsgDOTAMatch {
    const players = mapMatchPlayers(match.players);
    return {
        duration: match.duration,
        starttime: match.startTime,
        players,
        matchId: match.matchId,
        cluster: match.cluster,
        firstBloodTime: match.firstBloodTime,
        lobbyType: match.lobbyType,
        humanPlayers: countHumanPlayers(match.players),
        gameMode: match.gameMode === 0 ? 1 : match.gameMode,
        engine: 1,
        matchFlags: match.matchFlags,
        radiantTeamScore: match.radiantScore,
        direTeamScore: match.direScore,
        matchOutcome: match.goodGuysWin ? MATCH_OUTCOME_RADIANT : MATCH_OUTCOME_DIRE
    };
}

function mapMatchPlayers(players: DotaMatchPlayer[]): CMsgDOTAMatch_Player[] {
    const mapped: CMsgDOTAMatch_Player[] = [];
    for (let i = 0; i < players.length; i++) {
        const player = players[i];
        mapped.push({
            accountId: player.accountId,
            playerSlot: player.playerSlot,
            heroId: player.heroId,
            item0: matchItem(player, 0),
            item1: matchItem(player, 1),
            item2: matchItem(player, 2),
            item3: matchItem(player, 3),
            item4: matchItem(player, 4),
            item5: matchItem(player, 5),
            expectedTeamContribution: 0,
            scaledMetric: 0,
            previousRank: 0,
            rankChange: 0,
            kills: player.kills,
            deaths: player.deaths,
            assists: player.assists,
            leaverStatus: player.leaverStatus,
            gold: player.gold,
            lastHits: player.lastHits,
            denies: player.denies,
            goldPerMin: player.gpm,
            xpPerMin: player.xpm,
            goldSpent: player.goldSpent,
            heroDamage: player.heroDamage,
            towerDamage: player.towerDamage,
            heroHealing: player.heroHealing,
            level: player.level,
            playerName: player.personaName,
            claimedFarmGold: player.claimedFarmGold,
            supportGold: player.supportGold,
            activePlusSubscription: true,
            netWorth: Math.round(player.netWorth),
            item6: matchItem(player, 6),
            item7: matchItem(player, 7),
            item8: matchItem(player, 8),
            item9: matchItem(player, 9),
            bountyRunes: player.bountyRunes,
            outpostsCaptured: player.outpostsCaptured,
            teamNumber: player.goodGuys ? 0 : 1,
            teamSlot: player.playerSlot,
            selectedFacet: player.selectedFacet,
            item10: matchItem(player, 10)
        });
    }

    return mapped;
}

function countHumanPlayers(players: DotaMatchPlayer[]): number {
    let count = 0;
    for (let i = 0; i < players.length; i++) {
        if (players[i].steamId !== 0n) {
            count++;
        }
    }

    return count;
}

function matchItem(player: DotaMatchPlayer, index: number): number {
    if (index < player.items.length) {
        return player.items[index];
    }

    return 0;
}

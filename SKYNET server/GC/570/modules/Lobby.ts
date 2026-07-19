import { gc } from "../framework/gc";
import { Msg } from "../generated/dota";

export function registerLobby(): Lobby {
    const lobby = new Lobby();
    lobby.register();
    return lobby;
}

export class Lobby {
    register(): void {
        gc.onMessage(Msg.GCPracticeLobbyCreate as number, () => this.createLobby());
        gc.onMessage(Msg.GCAbandonCurrentGame as number, () => this.abandonCurrentGame());
        gc.onMessage(Msg.GCPracticeLobbyLeave as number, () => this.leaveLobby());
        gc.onMessage(Msg.GCPracticeLobbyJoin as number, () => this.joinLobby());
        gc.onMessage(Msg.GCInviteToLobby as number, () => this.inviteToLobby());
        gc.onMessage(Msg.GCLobbyInviteResponse as number, () => this.lobbyInviteResponse());
        gc.onMessage(Msg.GCPracticeLobbyList as number, () => this.practiceLobbyList());
        gc.onMessage(Msg.GCFriendPracticeLobbyListRequest as number, () => this.friendPracticeLobbyList());
        gc.onMessage(Msg.GCLobbyList as number, () => this.lobbyList());
        gc.onMessage(Msg.GCPracticeLobbySetDetails as number, () => this.setDetails());
        gc.onMessage(Msg.GCPracticeLobbySetTeamSlot as number, () => this.setTeamSlot());
        gc.onMessage(Msg.GCPracticeLobbySetCoach as number, () => this.setCoach());
        gc.onMessage(Msg.GCApplyTeamToPracticeLobby as number, () => this.applyTeam());
        gc.onMessage(Msg.GCPracticeLobbyLaunch as number, () => this.launchLobby());
        gc.onMessage(Msg.GCGameServerHello as number, () => this.gameServerHello());
        gc.onMessage(Msg.GCGameServerInfo as number, () => this.gameServerInfo());
        gc.onMessage(Msg.GCLANServerAvailable as number, () => this.lanServerAvailable());
        gc.onMessage(Msg.GCServerAvailable as number, () => this.serverAvailable());
        gc.onMessage(Msg.GCConnectedPlayers as number, () => this.connectedPlayers());
        gc.onMessage(Msg.GCPlayerFailedToConnect as number, () => this.playerFailedToConnect());
        gc.onMessage(Msg.GCGameMatchSignOut as number, () => this.signOut());
        gc.onMessage(Msg.GCGameBotMatchSignOut as number, () => this.signOut());
        gc.onMessage(Msg.ServerToGCRequestBatchPlayerResources as number, () => this.requestBatchPlayerResources());
        gc.onMessage(Msg.ServerToGCRequestPlayerRecentAccomplishments as number, () => this.recentAccomplishments());
        gc.onMessage(Msg.ServerToGCLobbyInitialized as number, () => this.lobbyInitialized());
    }

    createLobby(): boolean {
        return false;
    }
    abandonCurrentGame(): boolean {
        return false;
    }
    leaveLobby(): boolean {
        return false;
    }
    joinLobby(): boolean {
        return false;
    }
    inviteToLobby(): boolean {
        return false;
    }
    lobbyInviteResponse(): boolean {
        return false;
    }
    practiceLobbyList(): boolean {
        return false;
    }
    friendPracticeLobbyList(): boolean {
        return false;
    }
    lobbyList(): boolean {
        return false;
    }
    setDetails(): boolean {
        return false;
    }
    setTeamSlot(): boolean {
        return false;
    }
    setCoach(): boolean {
        return false;
    }
    applyTeam(): boolean {
        return false;
    }
    launchLobby(): boolean {
        return false;
    }
    gameServerHello(): boolean {
        return false;
    }
    gameServerInfo(): boolean {
        return false;
    }
    lanServerAvailable(): boolean {
        return false;
    }
    serverAvailable(): boolean {
        return false;
    }
    connectedPlayers(): boolean {
        return false;
    }
    playerFailedToConnect(): boolean {
        return false;
    }
    signOut(): boolean {
        return false;
    }
    requestBatchPlayerResources(): boolean {
        return false;
    }
    recentAccomplishments(): boolean {
        return false;
    }
    lobbyInitialized(): boolean {
        return false;
    }
    tick(): void {}
}

import { gc } from "../framework/gc";
import { Msg } from "../generated/dota";

export function registerLobby(): Lobby {
    const lobby = new Lobby();
    lobby.register();
    return lobby;
}

export class Lobby {
    register(): void {
        gc.onMessage(Msg.GCPracticeLobbyCreate, () => this.createLobby());
        gc.onMessage(Msg.GCAbandonCurrentGame, () => this.abandonCurrentGame());
        gc.onMessage(Msg.GCPracticeLobbyLeave, () => this.leaveLobby());
        gc.onMessage(Msg.GCPracticeLobbyJoin, () => this.joinLobby());
        gc.onMessage(Msg.GCInviteToLobby, () => this.inviteToLobby());
        gc.onMessage(Msg.GCLobbyInviteResponse, () => this.lobbyInviteResponse());
        gc.onMessage(Msg.GCPracticeLobbyList, () => this.practiceLobbyList());
        gc.onMessage(Msg.GCFriendPracticeLobbyListRequest, () => this.friendPracticeLobbyList());
        gc.onMessage(Msg.GCLobbyList, () => this.lobbyList());
        gc.onMessage(Msg.GCPracticeLobbySetDetails, () => this.setDetails());
        gc.onMessage(Msg.GCPracticeLobbySetTeamSlot, () => this.setTeamSlot());
        gc.onMessage(Msg.GCPracticeLobbySetCoach, () => this.setCoach());
        gc.onMessage(Msg.GCApplyTeamToPracticeLobby, () => this.applyTeam());
        gc.onMessage(Msg.GCPracticeLobbyLaunch, () => this.launchLobby());
        gc.onMessage(Msg.GCGameServerHello, () => this.gameServerHello());
        gc.onMessage(Msg.GCGameServerInfo, () => this.gameServerInfo());
        gc.onMessage(Msg.GCLANServerAvailable, () => this.lanServerAvailable());
        gc.onMessage(Msg.GCServerAvailable, () => this.serverAvailable());
        gc.onMessage(Msg.GCConnectedPlayers, () => this.connectedPlayers());
        gc.onMessage(Msg.GCPlayerFailedToConnect, () => this.playerFailedToConnect());
        gc.onMessage(Msg.GCGameMatchSignOut, () => this.signOut());
        gc.onMessage(Msg.GCGameBotMatchSignOut, () => this.signOut());
        gc.onMessage(Msg.ServerToGCRequestBatchPlayerResources, () => this.requestBatchPlayerResources());
        gc.onMessage(Msg.ServerToGCRequestPlayerRecentAccomplishments, () => this.recentAccomplishments());
        gc.onMessage(Msg.ServerToGCLobbyInitialized, () => this.lobbyInitialized());
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

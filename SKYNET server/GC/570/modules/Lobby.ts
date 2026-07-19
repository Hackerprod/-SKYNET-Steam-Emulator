import { Messages } from "../Messages";

export class Lobby {
    msg: Messages;

    constructor() {
        this.msg = new Messages();
    }

    handle(type: int32): boolean {
        if (type == this.msg.GCPracticeLobbyCreate()) return this.createLobby();
        if (type == this.msg.GCAbandonCurrentGame()) return this.abandonCurrentGame();
        if (type == this.msg.GCPracticeLobbyLeave()) return this.leaveLobby();
        if (type == this.msg.GCPracticeLobbyJoin()) return this.joinLobby();
        if (type == this.msg.GCInviteToLobby()) return this.inviteToLobby();
        if (type == this.msg.GCLobbyInviteResponse()) return this.lobbyInviteResponse();
        if (type == this.msg.GCPracticeLobbyList()) return this.practiceLobbyList();
        if (type == this.msg.GCFriendPracticeLobbyListRequest()) return this.friendPracticeLobbyList();
        if (type == this.msg.GCLobbyList()) return this.lobbyList();
        if (type == this.msg.GCPracticeLobbySetDetails()) return this.setDetails();
        if (type == this.msg.GCPracticeLobbySetTeamSlot()) return this.setTeamSlot();
        if (type == this.msg.GCPracticeLobbySetCoach()) return this.setCoach();
        if (type == this.msg.GCApplyTeamToPracticeLobby()) return this.applyTeam();
        if (type == this.msg.GCPracticeLobbyLaunch()) return this.launchLobby();
        if (type == this.msg.GCGameServerHello()) return this.gameServerHello();
        if (type == this.msg.GCGameServerInfo()) return this.gameServerInfo();
        if (type == this.msg.GCLANServerAvailable()) return this.lanServerAvailable();
        if (type == this.msg.GCServerAvailable()) return this.serverAvailable();
        if (type == this.msg.GCConnectedPlayers()) return this.connectedPlayers();
        if (type == this.msg.GCPlayerFailedToConnect()) return this.playerFailedToConnect();
        if (type == this.msg.GCGameMatchSignOut()) return this.signOut();
        if (type == this.msg.GCGameMatchSignOut2()) return this.signOut();
        if (type == this.msg.GCRequestBatchPlayerResources()) return this.requestBatchPlayerResources();
        if (type == this.msg.ServerToGCRequestPlayerRecentAccomplishments()) return this.recentAccomplishments();
        if (type == this.msg.ServerToGCLobbyInitialized()) return this.lobbyInitialized();
        return false;
    }

    createLobby(): boolean { return false; }
    abandonCurrentGame(): boolean { return false; }
    leaveLobby(): boolean { return false; }
    joinLobby(): boolean { return false; }
    inviteToLobby(): boolean { return false; }
    lobbyInviteResponse(): boolean { return false; }
    practiceLobbyList(): boolean { return false; }
    friendPracticeLobbyList(): boolean { return false; }
    lobbyList(): boolean { return false; }
    setDetails(): boolean { return false; }
    setTeamSlot(): boolean { return false; }
    setCoach(): boolean { return false; }
    applyTeam(): boolean { return false; }
    launchLobby(): boolean { return false; }
    gameServerHello(): boolean { return false; }
    gameServerInfo(): boolean { return false; }
    lanServerAvailable(): boolean { return false; }
    serverAvailable(): boolean { return false; }
    connectedPlayers(): boolean { return false; }
    playerFailedToConnect(): boolean { return false; }
    signOut(): boolean { return false; }
    requestBatchPlayerResources(): boolean { return false; }
    recentAccomplishments(): boolean { return false; }
    lobbyInitialized(): boolean { return false; }
    tick(): void { }
}

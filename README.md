 # [SKYNET] Steam Emulator
This project is created with the aim of replacing the original `steam_api.dll` from Steam with this one and thus emulating a connection to be able to play games in LAN mode.
This is not a steamworks wrapper like `Steamworks.Net` or `Facepunch`.
The project is in an initial stage, so it is not functional yet.

![Screenshot](Capture.png)

## Settings
This emulator reads settings from `[SKYNET] steam_api.ini` file, data like Nickname, SteamId, Language etc.

## Currently working on
Interface implementation.

## Log
When File log option si enabled in settings, a log file will be created in the root of the game executable with the following name `[SKYNET] steam_api.log`

## Implemented Interfaces
### Implementation of Interfaces
- [ ] Basic implementation
- [x] Advanced implementation
###
- [ ] ISteamAppList		
- [ ] ISteamApps
- [x] ISteamClient		
- [ ] ISteamController
- [x] ISteamFriends		
- [x] ISteamGameCoordinator
- [ ] ISteamGameSearch		
- [ ] ISteamGameServer
- [ ] ISteamGameServerStats	
- [ ] ISteamMasterServerUpdater
- [ ] ISteamMatchmaking		
- [ ] ISteamMatchmakingServers
- [ ] ISteamMusic		
- [ ] ISteamMusicRemote
- [ ] ISteamNetworking		
- [ ] ISteamNetworkingMessages
- [ ] ISteamNetworkingSockets	
- [ ] ISteamNetworkingSocketsSerialized
- [ ] ISteamNetworkingUtils	
- [ ] ISteamParentalSettings
- [ ] ISteamRemotePlay		
- [ ] ISteamRemoteStorage
- [ ] ISteamScreenshots		
- [ ] ISteamTV
- [ ] ISteamUGC			
- [ ] ISteamUser
- [ ] ISteamUserStats		
- [ ] ISteamUtils
- [ ] ISteamVideo			

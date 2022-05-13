 # [SKYNET] Steam Emulator
This project is created with the aim of replacing the original `steam_api.dll` from Steam with this one and thus emulating a connection to be able to play games in LAN mode.
This is not a steamworks wrapper like `Steamworks.Net` or `Facepunch`.
The project is in an initial stage, so it is not functional yet for some Games.

![Screenshot](Capture.png)

## 📁 Directury structure
```
📁 Game folder                     
└──📁 SKYNET
   ├──📁 AvatarCache                  // Contains avatars cache
   ├──📁 Storage                      // Contains stats and achievements files
   |  └──📁 Remote                    // Contains game files
   ├──📄 [SKYNET] steam_api.ini       
   └──📑 [SKYNET] steam_api.log       // If option is enabled
```

## ⚙️ Settings
This emulator reads settings from `[SKYNET] steam_api.ini` file, data like Nickname, SteamId, Language etc.

## 🔨 Currently working on
Callback system implementation.
Fucking SteamInternal_ContextInit in x86 Games

## 📝 Log
When File log option si enabled in settings, a log file will be created in the root of the game executable with the following name `[SKYNET] steam_api.log`

## Implemented Interfaces
###
- [x] ISteamAppDisableUpdate
- - [x] SteamAppDisableUpdate001
- [x] ISteamAppList		
- - [x] ISteamAppList001
- [x] ISteamAppDisableUpdate
- - [x] ISteamAppDisableUpdate001
- [x] ISteamApps
- - [x] ISteamApps008
- [x] ISteamClient		
- - [x] ISteamClient017
- - [x] ISteamClient018
- - [x] ISteamClient019
- - [x] ISteamClient020
- [x] ISteamController
- - [x] ISteamController005
- - [x] ISteamController006
- - [x] ISteamController007
- - [x] ISteamController008
- [x] ISteamFriends		
- - [x] ISteamFriends015
- - [x] ISteamFriends017 
- [x] ISteamGameCoordinator
- - [x] ISteamGameCoordinator001
- [ ] ISteamGameSearch		
- [x] ISteamGameServer
- - [x] ISteamGameServer012 
- - [x] ISteamGameServer014
- [x] ISteamGameServerStats	
- - [x] ISteamGameServerStats001 
- [x] ISteamGameStats	
- - [x] ISteamGameStats001 
- [x] ISteamHTMLSurface
- - [x] ISteamHTMLSurface003
- - [x] ISteamHTMLSurface004
- - [x] ISteamHTMLSurface005
- [x] ISteamHTTP
- - [x] ISteamHTTP002 
- - [x] ISteamHTTP003 
- [x] ISteamInput
- - [x] ISteamInput001
- - [x] ISteamInput002
- - [x] ISteamInput006
- [x] ISteamInventory
- - [x] ISteamInventory002
- - [x] ISteamInventory003
- [x] SteamMatchGameSearch
- - [x] SteamMatchGameSearch001 
- [ ] ISteamMasterServerUpdater
- [x] ISteamMatchmaking		
- - [x] ISteamMatchmaking008 
- - [x] ISteamMatchmaking009
- [x] ISteamMatchmakingServers
- - [x] ISteamMatchmakingServers002 
- [x] ISteamMusic		
- - [x] ISteamMusic001 
- [x] ISteamMusicRemote
- - [x] ISteamMusicRemote001 
- [x] ISteamNetworking		
- - [x] ISteamNetworking005
- - [x] ISteamNetworking006 
- [x] ISteamNetworkingMessages
- - [x] SteamNetworkingMessages002
- [x] ISteamNetworkingSockets	
- - [x] ISteamNetworkingSockets008
- - [x] ISteamNetworkingSockets009
- - [x] ISteamNetworkingSockets012
- [x] ISteamNetworkingSocketsSerialized
- - [x] ISteamNetworkingSocketsSerialized002 
- - [x] ISteamNetworkingSocketsSerialized003 
- - [x] ISteamNetworkingSocketsSerialized004 
- - [x] ISteamNetworkingSocketsSerialized005 
- [x] ISteamNetworkingUtils	
- - [x] ISteamNetworkingUtils003
- [x] ISteamParentalSettings
- - [x] ISteamParentalSettings001 
- [x] ISteamRemotePlay		
- - [x] ISteamRemotePlay001		
- [x] ISteamRemoteStorage
- - [x] ISteamRemoteStorage013 
- - [x] ISteamRemoteStorage014 
- - [x] ISteamRemoteStorage016 
- [x] ISteamScreenshots		
- - [x] ISteamScreenshots003 
- [ ] ISteamTV
- [x] ISteamUGC			
- - [x] ISteamUGC010
- - [x] ISteamUGC012
- - [x] ISteamUGC014
- - [x] ISteamUGC015
- - [x] ISteamUGC016
- [x] ISteamUnifiedMessages
- - [x] SteamUnifiedMessages001 
- [x] ISteamUser
- - [x] ISteamUser019 
- - [x] ISteamUser020 
- - [x] ISteamUser021 
- [x] ISteamUserStats		
- - [x] ISteamUserStats012 
- [x] ISteamUtils
- - [x] ISteamUtils009
- - [x] ISteamUtils010
- [x] ISteamVideo			
- - [x] ISteamVideo002 

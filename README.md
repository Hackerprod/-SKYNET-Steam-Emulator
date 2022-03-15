# [SKYNET] Steam Emulator
This project is created with the aim of replacing the original `steam_api.dll` from Steam with this one and thus emulating a connection to be able to play games in LAN mode.
This is not a steamworks wrapper like `Steamworks.Net` or `Facepunch`.
The project is in an initial stage, so it is not functional yet.

## Settings
This emulator reads settings from `[SKYNET] steam_api.ini` file, data like Nickname, SteamId, Language etc.

## LOG
When File log option si enabled in settings, a log file will be created in the root of the game executable with the following name `[SKYNET] steam_api.log`

## Implemented Interfaces
- [ ] ISteamClient017
- [x] ISteamFriends

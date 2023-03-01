 <img src="[SKYNET] Steam Emulator/game.ico" align="right" height="150" width="150" />

# [SKYNET] Steam Emulator 
This project is created with the aim of replacing the original `steam_api.dll` from Steam with this one and thus emulating a connection to be able to play games in LAN mode.
This is not a steamworks wrapper like `Steamworks.Net` or `Facepunch`.
The project is in an initial stage, so it is not functional yet for some Games.

</br>

Some time ago I have not been able to update the Repository due to personal problems so those who want to collaborate with the development are welcome

</br>
<p align="center">
    <img src="https://img.shields.io/github/contributors/Hackerprod/-SKYNET-Steam-Emulator?style=for-the-badge" />
    <img src="https://img.shields.io/github/forks/Hackerprod/-SKYNET-Steam-Emulator?style=for-the-badge" alt="Project forks">
    <img src="https://img.shields.io/github/stars/Hackerprod/-SKYNET-Steam-Emulator?label=Project%20Stars%21%21%21&style=for-the-badge" alt="Project stars">
    <img src="https://img.shields.io/github/issues/Hackerprod/-SKYNET-Steam-Emulator?style=for-the-badge" alt="Project issues">
</p>
</br>

![Screenshot](Capture.png)

## ❔ How to use
### Replacing the dll
When compiling the project, two folders are generated (x64 and x86) that contain the dll for different target platform, in the case of x64 you must rename the file to steam_api64.dll, to emulate the connection to Steam of a game you must replace the dll with the one that contains the game. In case the game engine is Unity you can rename the dll to CSteamworks.dll and replace it.

### Client Emulator
To use the client you simply need to add the game and configure the appid. The client is currently under development.

## 📁 Directory structure
```
📁 Root client folder                     
├──📁 x64                             // The x64 version of the SteamAPI dll that will be injected
├──📁 x86                             // The x64 version of the SteamAPI dll that will be injected
└──📁 Data
   ├──📁 Assemblies                   // Contains client libraries (Including cefsharp api or gecko) 
   ├──📁 Images                       // Contains app cache and avatar images 
   ├──📁 Injector                     // Contains the DLL injectors
   ├──📁 www                          // Contains the web files
   ├──📁 Storage                      // Contains stats and achievements files
   |  └──📁 Remote                    // Contains game files
   └──📑 Games.bin                    // Stored game list         
```

```
📁 Root server folder                     
└──📁 Data
   ├──📁 Assemblies                   // Contains server libraries 
   ├──📁 Images                       // Contains app cache and avatar images 
   ├──📁 MongoDB                      // Contains local MongoDB server
   └──📁 Storage                      // Contains some server files      
```

## 🔗 Features

```
User Stats manager         Save and Load user stats from local folder.
Achievements manager       Save and Load user achievements from local folder.
CSteamworks emulation      Rename the emu to CSteamworks.dll to emulate them.
Supported Game Engines     Works with multiple game engines like Source 2, Unity 3D etc.
Network communication      Network communication between clients through a configurable port.
Overlay                    External Overlay for steam and game messages.
DLC                        Unlock all downloaded DLCs.
Avatar support             Load avatar from file (Avatar.jpg) inside SKYNET folder and share it through the network.
Plugin system              Load external plugin to communicate with the emu.
In game voice              Fully functional voice system
```

## 🔨 Currently working on
Callback system implementation.<br />
SteamInternal_ContextInit in x86 Games

## 📝 Log
When File log option si enabled in settings, a log file will be created inside "root game folder/SKYNET" folder with the following name `[SKYNET] steam_api.log`

## 🔌 Plugin system
The plugin system is developed in order to establish a communication between the game and the game coordinator, the following example shows a basic plugin. <br /><br />
**Interface for Game coordinator plugin:**
```csharp
namespace SKYNET.Plugin
{
    public interface IGameCoordinatorPlugin
    {
        uint Initialize();
        void MessageFromGame(byte[] bytes);
        EventHandler<Dictionary <uint, byte[]>> IsMessageAvailable { get; set; }
    }
}
```
**Game coordinator plugin example:**
```csharp
namespace SKYNET.Plugin
{
    public class Dota2GameCoordinator : IGameCoordinatorPlugin
    {
        private uint AppID = 570;
        public EventHandler<Dictionary<uint, byte[]>> IsMessageAvailable { get; set; }

        public uint Initialize()
        {
            // TODO: Initialize all Game coordinator class
            return AppID;
        }

        public void MessageFromGame(byte[] bytes)
        {
            // Process message from game
            uint MsgType = MsgUtil.GetGCMsg(new MemoryStream(bytes).ReadUInt32L());
            IPacketGCMsg packetGCMsg = MsgUtil.GetPacketGcMsg(MsgType, bytes);
            // TODO: Process GC message
        }

        public void SendPacketToGame(uint msgType, byte[] packet)
        {
            Dictionary<uint, byte[]> message = new Dictionary<uint, byte[]>();
            message.Add(msgType, packet);
            IsMessageAvailable?.Invoke(this, message);
        }

        public void SendPacketToGame(Dictionary<uint, byte[]> messages)
        {
            IsMessageAvailable?.Invoke(this, messages);
        }
    }
}
```

## ⭐ Implemented Interfaces

<details><summary>Click to expand</summary><br />

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
</details>


# skynet-mcp

Stdio MCP server that gives AI agents the `SkynetGc` tools (interface tracer, GC capture and analysis,
game window control) as 14 compact tools. It has no logic of its own: each tool runs `skynet-gc.exe`, so
the CLI and the agent behave the same. Official `ModelContextProtocol` C# SDK 2.2.0.

```powershell
dotnet build "DeveloperTools\SkynetGc\SkynetGc.csproj" -c Debug     # also builds the tracer payload
dotnet build "DeveloperTools\SkynetMcp\SkynetMcp.csproj" -c Debug
powershell -File DeveloperTools\SkynetMcp\selftest.ps1              # offline JSON-RPC check, starts no game
```

Register it per machine and user (the paths are absolute on purpose, so agents in git worktrees find the
builds; nothing is committed):

```powershell
claude mcp add --scope local skynet-mcp `
  -e SKYNET_GC_EXE="<repo>\DeveloperTools\SkynetGc\bin\Debug\net8.0-windows\skynet-gc.exe" `
  -e SKYNET_LAUNCHER_EXE="<repo>\SKYNET Steam Client\bin\Debug\net472\SKYNET Steam Client.exe" `
  -- "<repo>\DeveloperTools\SkynetMcp\bin\Debug\net8.0\skynet-mcp.exe"
```

| Tool | Does |
|------|------|
| `iface_start` `iface_stop` `iface_mark` `iface_status` | Run and label a trace of any Steam interface (`interfaces`/`methods` parameters; default `SteamFriends017`; `attach` to a running Dota (`via_emulator=true` for a launcher-started game; needs the environment variable `SKYNET_IFACE_TRACER` = path of `skynet-iface-hook.exe` when the game was started), or `launch` on real Steam). |
| `iface_timeline` `iface_query` `iface_diff` | Read the trace; compare it with the emulator's `steam_api.log`. |
| `emu_launch` | Start Dota under the emulator (`SKYNET Steam Client.exe --launch`), return its pid. |
| `gc_capture_start` `gc_capture_stop` `gc_analyze` `gc_messages` `gc_decode` | The existing GC capture and analysis. |
| `game_ui` | `skynet-gc ui`: screenshot, click, key, type, console. |

`iface_start(mode=launch)` and `gc_capture_start` launch Dota against real Steam with an injected DLL, which
carries VAC risk. Use them deliberately, with an account you accept that risk for, and stop the capture if a
VAC popup appears.
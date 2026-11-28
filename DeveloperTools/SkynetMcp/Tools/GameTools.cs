using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace SKYNET.Mcp.Tools;

/// <summary>Starting Dota under the emulator and driving its window.</summary>
[McpServerToolType]
public static class GameTools
{
    [McpServerTool(Name = "emu_launch"), Description(
        "Start a game under the SKYNET emulator (SKYNET Steam Client.exe --launch). Waits until a NEW dota2.exe has a window (<=180 s) and returns {pid}. " +
        "target = path of dota2.exe, a library game name or an app id.")]
    public static async Task<string> EmuLaunch(string target, string? args = null)
    {
        var exe = Engine.LauncherExe;
        if (!File.Exists(exe)) return "launcher not found: " + exe + " (build it or set SKYNET_LAUNCHER_EXE)";
        var before = Process.GetProcessesByName("dota2").Select(p => p.Id).ToHashSet();   // other Dota clients may already run
        var a = new List<string> { "--launch", target };
        if (!string.IsNullOrWhiteSpace(args)) { a.Add("--"); a.AddRange(args.Split(' ', StringSplitOptions.RemoveEmptyEntries)); }
        Engine.StartBackground(exe, a, Path.Combine(Engine.IfaceRoot, "emu-launch.log"));   // stdio redirected: never inherit the MCP pipes
        for (var i = 0; i < 180; i++)
        {
            await Task.Delay(1000);
            foreach (var p in Process.GetProcessesByName("dota2"))
                if (!before.Contains(p.Id) && p.MainWindowHandle != IntPtr.Zero) return JsonSerializer.Serialize(new { pid = p.Id });
        }
        return "no new dota2.exe window after 180 s; see launcher.log in the launcher's data folder";
    }

    [McpServerTool(Name = "game_ui"), Description(
        "Drive the local Dota window (skynet-gc ui). action: shot (returns the PNG path and size) | click (x,y fractions 0..1 of the client area) | key (SendKeys syntax, text) | type (text) | console (text = command). " +
        "Refuses when the game window is obscured, and never targets the dedicated server's console.")]
    public static Task<string> GameUi(string action, double? x = null, double? y = null, string? text = null)
    {
        var a = new List<string> { "ui", action };
        switch (action)
        {
            case "shot": a.Add(Path.Combine(Path.GetTempPath(), "skynet-iface", "shot-" + DateTime.Now.ToString("HHmmss-fff") + ".png")); Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "skynet-iface")); break;
            case "click":
                if (x == null || y == null) return Task.FromResult("click needs x and y");
                a.Add(x.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)); a.Add(y.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)); break;
            case "key": case "type": case "console":
                if (text == null) return Task.FromResult(action + " needs text");
                a.Add(text); break;
            default: return Task.FromResult("action must be shot|click|key|type|console");
        }
        return Engine.Gc(a, 60_000);
    }
}

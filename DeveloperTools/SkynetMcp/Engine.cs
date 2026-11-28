using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace SKYNET.Mcp;

/// <summary>Runs skynet-gc.exe (the engine) and keeps tool output small.</summary>
internal static class Engine
{
    public const int DefaultMaxChars = 4000;
    private static readonly List<Process> Background = new();   // keep launched processes referenced

    // bin\<cfg>\<tfm>\ of this exe; the siblings are found by the same layout unless the env vars say otherwise.
    private static string Cfg => Path.GetFileName(Path.GetDirectoryName(AppContext.BaseDirectory.TrimEnd('\\', '/'))!);
    private static string Dev => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    public static string GcExe => Environment.GetEnvironmentVariable("SKYNET_GC_EXE")
        ?? Path.Combine(Dev, "SkynetGc", "bin", Cfg, "net8.0-windows", "skynet-gc.exe");

    public static string LauncherExe => Environment.GetEnvironmentVariable("SKYNET_LAUNCHER_EXE")
        ?? Path.GetFullPath(Path.Combine(Dev, "..", "SKYNET Steam Client", "bin", Cfg, "net472", "SKYNET Steam Client.exe"));

    public static string IfaceRoot => Path.Combine(Path.GetTempPath(), "skynet-iface");
    public static string GcRoot => Path.Combine(Path.GetTempPath(), "skynet-capture");

    /// <summary>Caps text for an agent; the tail says how much was cut.</summary>
    public static string Cap(string text, int max = DefaultMaxChars, int offset = 0)
    {
        text = text.TrimEnd();
        if (offset > 0) text = offset < text.Length ? text.Substring(offset) : "";
        if (text.Length <= max) return text;
        return text.Substring(0, max) + $"\n[truncated: {text.Length - max} more chars, use offset/limit]";
    }

    public static async Task<(int Code, string Text)> Run(string exe, IEnumerable<string> args, int timeoutMs = 120_000)
    {
        if (!File.Exists(exe)) return (2, "not found: " + exe + " (build it, or set the env var that points to it)");
        var psi = new ProcessStartInfo(exe) { UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true, StandardOutputEncoding = Encoding.UTF8, StandardErrorEncoding = Encoding.UTF8 };
        foreach (var a in args) psi.ArgumentList.Add(a);
        using var p = Process.Start(psi)!;
        p.StandardInput.Close();   // a child must never share the MCP protocol stdin
        var stdout = p.StandardOutput.ReadToEndAsync();
        var stderr = p.StandardError.ReadToEndAsync();
        using var cts = new CancellationTokenSource(timeoutMs);
        try { await p.WaitForExitAsync(cts.Token); }
        catch (OperationCanceledException) { try { p.Kill(true); } catch { } return (124, $"timed out after {timeoutMs / 1000}s"); }
        var err = (await stderr).Trim();
        return (p.ExitCode, (await stdout).TrimEnd() + (err.Length > 0 ? "\n[stderr] " + err : ""));
    }

    /// <summary>
    /// For paged tools (timeline/query: one item per line, optional last line next_offset=N). Cuts at whole lines and
    /// rewrites next_offset so that continuing from it loses nothing; says truncated=true when it had to cut.
    /// </summary>
    public static async Task<string> GcPaged(IEnumerable<string> args, int offset, int max = DefaultMaxChars, int timeoutMs = 60_000)
    {
        var (code, text) = await Run(GcExe, args, timeoutMs);
        if (code != 0) return Cap($"exit {code}\n{text}", max);
        return Page(text, offset, max);
    }

    public static string Page(string text, int offset, int max)
    {
        var lines = text.TrimEnd().Split('\n').Select(l => l.TrimEnd('\r')).ToList();
        var hadNext = lines.Count > 0 && lines[^1].StartsWith("next_offset=", StringComparison.Ordinal);
        if (hadNext) lines.RemoveAt(lines.Count - 1);
        var size = 0; var kept = 0;
        foreach (var l in lines) { if (kept > 0 && size + l.Length + 1 > max) break; size += l.Length + 1; kept++; }
        var sb = new StringBuilder();
        for (var i = 0; i < kept; i++) sb.AppendLine(lines[i]);
        if (kept < lines.Count) sb.AppendLine("truncated=true (output size cap)").Append("next_offset=" + (offset + kept));
        else if (hadNext) sb.Append(text.TrimEnd().Split('\n')[^1].TrimEnd('\r'));
        return sb.ToString().TrimEnd();
    }

    public static async Task<string> Gc(IEnumerable<string> args, int timeoutMs = 120_000, int max = DefaultMaxChars)
    {
        var (code, text) = await Run(GcExe, args, timeoutMs);
        return Cap(code == 0 ? text : $"exit {code}\n{text}", max);
    }

    /// <summary>Starts a long-running child (the injector stays alive while the game runs) and copies its output to a file,
    /// because stdout of this process is the protocol channel and must never be inherited.</summary>
    public static Process StartBackground(string exe, IEnumerable<string> args, string logFile)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(logFile)!);
        var psi = new ProcessStartInfo(exe) { UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
        foreach (var a in args) psi.ArgumentList.Add(a);
        var p = Process.Start(psi)!;
        p.StandardInput.Close();
        var gate = new object();
        void Pump(string? line) { if (line == null) return; lock (gate) { try { File.AppendAllText(logFile, line + "\n"); } catch { } } }
        p.OutputDataReceived += (_, e) => Pump(e.Data);
        p.ErrorDataReceived += (_, e) => Pump(e.Data);
        p.BeginOutputReadLine();
        p.BeginErrorReadLine();
        lock (Background) Background.Add(p);
        return p;
    }

    public static string NewDir(string root) => Path.Combine(root, DateTime.Now.ToString("yyyyMMdd-HHmmss"));

    public static string? Newest(string root) =>
        Directory.Exists(root) ? new DirectoryInfo(root).GetDirectories().OrderByDescending(d => d.CreationTimeUtc).FirstOrDefault()?.FullName : null;

    public static void WriteSession(string dir, string kind, int pid, int gamePid)
    {
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, "mcp-session.json"), JsonSerializer.Serialize(new { kind, pid, gamePid, dir }));
    }

    public static (int Pid, int GamePid)? ReadSession(string dir)
    {
        try
        {
            using var doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(dir, "mcp-session.json")));
            return (doc.RootElement.GetProperty("pid").GetInt32(), doc.RootElement.GetProperty("gamePid").GetInt32());
        }
        catch { return null; }
    }

    public static string? LastMetaStatus(string dir)
    {
        var path = Path.Combine(dir, "iface.jsonl");
        if (!File.Exists(path)) return null;
        string? status = null;
        try
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            using var r = new StreamReader(fs);
            string? line;
            while ((line = r.ReadLine()) != null)
            {
                if (!line.StartsWith("{\"k\":\"meta\"", StringComparison.Ordinal)) continue;
                try { using var d = JsonDocument.Parse(line); status = d.RootElement.GetProperty("status").GetString(); } catch (JsonException) { }
            }
        }
        catch (IOException) { }
        return status;
    }

    /// <summary>Polls until the payload's meta leaves "waiting" (it stays waiting until the game has logged in to Steam).</summary>
    public static async Task<string?> WaitMeta(string dir, int seconds)
    {
        for (var i = 0; i < seconds; i++)
        {
            var s = LastMetaStatus(dir);
            if (s != null && s != "waiting") return s;
            await Task.Delay(1000);
        }
        return LastMetaStatus(dir);
    }

    public static void CloseGame(int pid)
    {
        try
        {
            using var g = Process.GetProcessById(pid);
            if (g.HasExited) return;
            g.CloseMainWindow();
            if (!g.WaitForExit(10_000)) g.Kill();
        }
        catch { }
    }
}

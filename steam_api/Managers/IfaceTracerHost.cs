using SKYNET.Helpers;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace SKYNET.Managers
{
    // Developer interface tracer, loaded in-process. A second EasyHook injection into a game the launcher already
    // injected fails (Code 15), so for the emulator the tracer payload (skynet-iface-hook.exe) is loaded here instead:
    // `skynet-gc iface attach --via-emulator` drops %TEMP%\skynet-iface\attach-request.json, this thread picks it up and
    // runs IfaceEntryPoint.Run(attach) by reflection. Off unless the environment variable SKYNET_IFACE_TRACER or the ini key
    // [Debug] IfaceTracer holds the full path of the exe (env wins when non-empty); with neither set no thread exists and
    // nothing polls. Never throws into the game.
    //
    // The ini key is NOT reliable with the launcher: SKYNET Steam Client rewrites steam_api.ini from its per-game settings on
    // every launch and drops unknown keys. The environment variable is the supported way: the launcher creates the game with
    // RemoteHooking.CreateAndInject (CreateProcess with the launcher's own environment, like SteamPath/SKYNET_LOG_SUFFIX), so a
    // variable set in the process that starts the launcher reaches dota2.exe.
    internal static class IfaceTracerHost
    {
        internal const string EnvVariable = "SKYNET_IFACE_TRACER";

        // ini [Debug] IfaceTracer (string key; empty = feature off). Read once at startup. See the note above: the launcher
        // overwrites the ini, so SKYNET_IFACE_TRACER normally decides.
        internal static string ExePath = string.Empty;

        private static int started;

        internal static string RequestFile => Path.Combine(Path.GetTempPath(), "skynet-iface", "attach-request.json");

        // Same role guards as FriendActions.Init: never in game-server / dedicated / disabled roles.
        internal static void Init()
        {
            var fromEnv = (Environment.GetEnvironmentVariable(EnvVariable) ?? string.Empty).Trim();
            if (fromEnv.Length > 0)
            {
                ExePath = fromEnv;
            }

            if (string.IsNullOrWhiteSpace(ExePath) || !FriendActions.RoleAllowsThreads())
            {
                return;
            }

            if (Interlocked.Exchange(ref started, 1) == 1)
            {
                return;
            }

            try
            {
                new Thread(Loop) { IsBackground = true, Name = "SKYNET iface tracer host" }.Start();
                Log("armed, waiting for " + RequestFile);
            }
            catch (Exception ex)
            {
                Log("could not start: " + ex.Message);
            }
        }

        private static void Log(string message)
        {
            try
            {
                SteamEmulator.Write("IfaceTracer", "[IFACE] " + message);
            }
            catch
            {
            }
        }

        private static string Field(string json, string name)
        {
            var m = Regex.Match(json, "\"" + name + "\"\\s*:\\s*\"((?:[^\"\\\\]|\\\\.)*)\"");
            return m.Success ? Regex.Unescape(m.Groups[1].Value) : string.Empty;
        }

        private static void Loop()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(2000);
                    var file = RequestFile;
                    if (!File.Exists(file))
                    {
                        continue;
                    }

                    var json = File.ReadAllText(file);
                    // Several game clients may run with the feature on: a request naming another pid is not ours to take.
                    var pid = Regex.Match(json, "\"pid\"\\s*:\\s*(\\d+)");
                    if (pid.Success && int.Parse(pid.Groups[1].Value) != System.Diagnostics.Process.GetCurrentProcess().Id)
                    {
                        continue;
                    }

                    File.Delete(file);
                    Run(json);
                }
                catch (Exception ex)
                {
                    Log("request failed: " + ex.Message);
                }
            }
        }

        // Blocks until the capture is stopped (STOP file), like the payload's Run does under EasyHook; the next request
        // is served afterwards.
        private static void Run(string json)
        {
            try
            {
                var dir = Field(json, "dir");
                if (dir.Length == 0)
                {
                    Log("request without dir ignored");
                    return;
                }

                var assembly = Assembly.LoadFrom(ExePath);
                var type = assembly.GetType("SKYNET.Iface.IfaceEntryPoint", true);
                var specDir = Path.Combine(Path.GetDirectoryName(ExePath) ?? string.Empty, "meta");
                // Same arguments RemoteHooking.Inject passes; the context is unused in attach mode.
                var args = new object[] { null, dir, "attach", Field(json, "emuLog"), specDir, Field(json, "interfaces"), Field(json, "methods") };
                var entry = Activator.CreateInstance(type, args);
                Log("tracer attached: dir=" + dir + " interfaces=" + args[5]);
                type.GetMethod("Run").Invoke(entry, args);
                Log("tracer stopped: dir=" + dir);
            }
            catch (Exception ex)
            {
                Log("tracer failed: " + (ex.InnerException ?? ex).Message);
            }
        }
    }
}

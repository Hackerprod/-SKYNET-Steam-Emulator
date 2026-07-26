using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using SKYNET.Callback;
using SKYNET.Steamworks;

namespace SKYNET.Managers
{
    internal static class MusicPlayerManager
    {
        private const string Alias = "skynet_steam_music";
        private const int PollIntervalMs = 250;
        private static readonly HashSet<string> SupportedExtensions =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".aac", ".flac", ".m4a", ".mid", ".midi", ".mp3", ".wav", ".wma"
            };
        private static readonly object StateGate = new object();
        private static readonly ConcurrentQueue<Action> Commands = new ConcurrentQueue<Action>();
        private static readonly AutoResetEvent Signal = new AutoResetEvent(false);
        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();

        private static Thread worker;
        private static List<string> tracks = new List<string>();
        private static int currentIndex;
        private static float volume = 1f;
        private static AudioPlayback_Status playbackStatus = AudioPlayback_Status.AudioPlayback_Idle;
        private static bool aliasOpen;
        private static bool shutdown;

        public static bool IsEnabled => SteamEmulator.MusicEnabled;

        public static bool IsPlaying
        {
            get
            {
                lock (StateGate)
                {
                    return playbackStatus == AudioPlayback_Status.AudioPlayback_Playing ||
                           playbackStatus == AudioPlayback_Status.AudioPlayback_Paused;
                }
            }
        }

        public static AudioPlayback_Status PlaybackStatus
        {
            get
            {
                if (!IsEnabled)
                {
                    return AudioPlayback_Status.AudioPlayback_Undefined;
                }

                lock (StateGate)
                {
                    return playbackStatus;
                }
            }
        }

        public static float Volume
        {
            get
            {
                lock (StateGate)
                {
                    return volume;
                }
            }
        }

        public static void Play()
        {
            if (IsEnabled)
            {
                Enqueue(PlayCore);
            }
        }

        public static void Pause()
        {
            if (!IsEnabled)
            {
                return;
            }

            Enqueue(() =>
            {
                if (!aliasOpen || PlaybackStatus != AudioPlayback_Status.AudioPlayback_Playing)
                {
                    return;
                }

                if (SendCommand("pause " + Alias))
                {
                    SetPlaybackStatus(AudioPlayback_Status.AudioPlayback_Paused);
                }
            });
        }

        public static void PlayNext()
        {
            if (IsEnabled)
            {
                Enqueue(() => ChangeTrack(1));
            }
        }

        public static void PlayPrevious()
        {
            if (IsEnabled)
            {
                Enqueue(() => ChangeTrack(-1));
            }
        }

        public static void SetVolume(float value)
        {
            var normalized = float.IsNaN(value) || float.IsInfinity(value)
                ? 1f
                : Math.Max(0f, Math.Min(1f, value));
            bool changed;
            lock (StateGate)
            {
                changed = Math.Abs(volume - normalized) > 0.0001f;
                volume = normalized;
            }

            if (!changed)
            {
                return;
            }

            QueueCallback(new VolumeHasChanged_t { NewVolume = normalized });
            if (IsEnabled)
            {
                Enqueue(() =>
                {
                    ApplyVolume();
                    PersistState();
                });
            }
        }

        public static void Shutdown()
        {
            Thread currentWorker;
            lock (StateGate)
            {
                shutdown = true;
                currentWorker = worker;
            }
            Signal.Set();

            if (currentWorker != null &&
                currentWorker != Thread.CurrentThread &&
                currentWorker.IsAlive)
            {
                currentWorker.Join(TimeSpan.FromSeconds(2));
            }

            lock (StateGate)
            {
                if ((worker == null || ReferenceEquals(worker, currentWorker)) &&
                    (currentWorker == null || !currentWorker.IsAlive))
                {
                    worker = null;
                    shutdown = false;
                    aliasOpen = false;
                    playbackStatus = AudioPlayback_Status.AudioPlayback_Idle;
                    while (Commands.TryDequeue(out _))
                    {
                    }
                }
            }
        }

        private static void EnsureStarted()
        {
            lock (StateGate)
            {
                if (worker != null || shutdown)
                {
                    return;
                }

                worker = new Thread(WorkerLoop)
                {
                    IsBackground = true,
                    Name = "SKYNET Steam Music"
                };
                worker.SetApartmentState(ApartmentState.MTA);
                worker.Start();
            }
        }

        private static void Enqueue(Action command)
        {
            if (command == null)
            {
                return;
            }

            EnsureStarted();
            lock (StateGate)
            {
                if (shutdown)
                {
                    return;
                }
            }
            Commands.Enqueue(command);
            Signal.Set();
        }

        private static void WorkerLoop()
        {
            try
            {
                RefreshLibrary();
                RestoreState();
                while (true)
                {
                    lock (StateGate)
                    {
                        if (shutdown)
                        {
                            break;
                        }
                    }

                    Signal.WaitOne(PollIntervalMs);
                    while (Commands.TryDequeue(out var command))
                    {
                        try
                        {
                            command();
                        }
                        catch (Exception ex)
                        {
                            SteamEmulator.Write("Steam Music command", ex);
                        }
                    }

                    PollPlayback();
                }
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("Steam Music worker", ex);
            }
            finally
            {
                CloseCurrentTrack();
                lock (StateGate)
                {
                    if (ReferenceEquals(worker, Thread.CurrentThread))
                    {
                        worker = null;
                    }
                }
            }
        }

        private static void PlayCore()
        {
            RefreshLibraryIfEmpty();
            if (tracks.Count == 0)
            {
                SetPlaybackStatus(AudioPlayback_Status.AudioPlayback_Idle);
                return;
            }

            if (aliasOpen && PlaybackStatus == AudioPlayback_Status.AudioPlayback_Paused)
            {
                if (SendCommand("resume " + Alias))
                {
                    SetPlaybackStatus(AudioPlayback_Status.AudioPlayback_Playing);
                }
                return;
            }

            if (!aliasOpen && !OpenCurrentTrack())
            {
                return;
            }

            if (SendCommand("play " + Alias))
            {
                SetPlaybackStatus(AudioPlayback_Status.AudioPlayback_Playing);
                PersistState();
            }
        }

        private static void ChangeTrack(int delta)
        {
            RefreshLibraryIfEmpty();
            if (tracks.Count == 0)
            {
                SetPlaybackStatus(AudioPlayback_Status.AudioPlayback_Idle);
                return;
            }

            var wasActive = IsPlaying;
            CloseCurrentTrack();
            currentIndex = Mod(currentIndex + delta, tracks.Count);
            if (wasActive && OpenCurrentTrack() && SendCommand("play " + Alias))
            {
                SetPlaybackStatus(AudioPlayback_Status.AudioPlayback_Playing);
            }
            else
            {
                SetPlaybackStatus(AudioPlayback_Status.AudioPlayback_Idle);
            }
            PersistState();
        }

        private static bool OpenCurrentTrack()
        {
            if (tracks.Count == 0)
            {
                return false;
            }

            currentIndex = Mod(currentIndex, tracks.Count);
            var attempts = tracks.Count;
            while (attempts-- > 0)
            {
                var path = tracks[currentIndex];
                if (File.Exists(path) && SendCommand("open \"" + path.Replace("\"", "\"\"") + "\" alias " + Alias))
                {
                    aliasOpen = true;
                    ApplyVolume();
                    return true;
                }

                currentIndex = Mod(currentIndex + 1, tracks.Count);
            }

            aliasOpen = false;
            return false;
        }

        private static void CloseCurrentTrack()
        {
            if (!aliasOpen)
            {
                return;
            }

            SendCommand("close " + Alias, false);
            aliasOpen = false;
        }

        private static void PollPlayback()
        {
            if (!aliasOpen || PlaybackStatus != AudioPlayback_Status.AudioPlayback_Playing)
            {
                return;
            }

            var mode = SendCommandForText("status " + Alias + " mode");
            if (string.Equals(mode, "stopped", StringComparison.OrdinalIgnoreCase))
            {
                ChangeTrack(1);
            }
        }

        private static void ApplyVolume()
        {
            if (aliasOpen)
            {
                SendCommand("setaudio " + Alias + " volume to " + (int)Math.Round(Volume * 1000f), false);
            }
        }

        private static void RefreshLibraryIfEmpty()
        {
            if (tracks.Count == 0)
            {
                RefreshLibrary();
            }
        }

        private static void RefreshLibrary()
        {
            var root = GetLibraryRoot();
            try
            {
                Directory.CreateDirectory(root);
                tracks = Directory
                    .EnumerateFiles(root, "*", SearchOption.AllDirectories)
                    .Where(path => SupportedExtensions.Contains(Path.GetExtension(path)))
                    .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                    .ToList();
                currentIndex = tracks.Count == 0 ? 0 : Mod(currentIndex, tracks.Count);
            }
            catch (Exception ex)
            {
                tracks = new List<string>();
                SteamEmulator.Write("Steam Music library", ex);
            }
        }

        private static void RestoreState()
        {
            try
            {
                var path = GetStatePath();
                if (!File.Exists(path))
                {
                    return;
                }

                var state = Serializer.Deserialize<MusicState>(File.ReadAllText(path));
                if (state == null)
                {
                    return;
                }

                lock (StateGate)
                {
                    volume = Math.Max(0f, Math.Min(1f, state.Volume));
                }
                currentIndex = tracks.Count == 0 ? 0 : Mod(state.TrackIndex, tracks.Count);
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("Steam Music state restore", ex);
            }
        }

        private static void PersistState()
        {
            try
            {
                var path = GetStatePath();
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                var temporary = path + ".tmp";
                File.WriteAllText(temporary, Serializer.Serialize(new MusicState
                {
                    TrackIndex = currentIndex,
                    Volume = Volume
                }));

                if (File.Exists(path))
                {
                    try
                    {
                        File.Replace(temporary, path, null);
                    }
                    catch (PlatformNotSupportedException)
                    {
                        ReplaceByCopy(temporary, path);
                    }
                    catch (IOException)
                    {
                        ReplaceByCopy(temporary, path);
                    }
                }
                else
                {
                    File.Move(temporary, path);
                }
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("Steam Music state persistence", ex);
            }
        }

        private static void ReplaceByCopy(string temporary, string path)
        {
            File.Copy(temporary, path, true);
            File.Delete(temporary);
        }

        private static void SetPlaybackStatus(AudioPlayback_Status status)
        {
            bool changed;
            lock (StateGate)
            {
                changed = playbackStatus != status;
                playbackStatus = status;
            }
            if (changed)
            {
                QueueCallback(new PlaybackStatusHasChanged_t());
            }
        }

        private static void QueueCallback(ICallbackData callback)
        {
            NativeCallbackQueue.Enqueue(() => CallbackManager.AddCallback(callback));
        }

        private static bool SendCommand(string command, bool logFailure = true)
        {
            var error = mciSendString(command, null, 0, IntPtr.Zero);
            if (error == 0)
            {
                return true;
            }

            if (logFailure)
            {
                SteamEmulator.Write("Steam Music", command + " failed: " + GetMciError(error));
            }
            return false;
        }

        private static string SendCommandForText(string command)
        {
            var result = new StringBuilder(128);
            var error = mciSendString(command, result, result.Capacity, IntPtr.Zero);
            return error == 0 ? result.ToString().Trim() : string.Empty;
        }

        private static string GetMciError(uint error)
        {
            var message = new StringBuilder(256);
            return mciGetErrorString(error, message, message.Capacity)
                ? message.ToString()
                : "MCI error " + error;
        }

        private static string GetLibraryRoot()
        {
            return string.IsNullOrWhiteSpace(SteamEmulator.MusicLibraryRoot)
                ? Path.Combine(Common.GetPath(), "SKYNET", "Music")
                : SteamEmulator.MusicLibraryRoot;
        }

        private static string GetStatePath()
        {
            return Path.Combine(Common.GetPath(), "SKYNET", "Music", "player-state.json");
        }

        private static int Mod(int value, int modulus)
        {
            var result = value % modulus;
            return result < 0 ? result + modulus : result;
        }

        [DllImport("winmm.dll", CharSet = CharSet.Unicode, EntryPoint = "mciSendStringW")]
        private static extern uint mciSendString(
            string command,
            StringBuilder returnValue,
            int returnLength,
            IntPtr callbackWindow);

        [DllImport("winmm.dll", CharSet = CharSet.Unicode, EntryPoint = "mciGetErrorStringW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool mciGetErrorString(uint errorCode, StringBuilder errorText, int errorTextSize);

        private sealed class MusicState
        {
            public int TrackIndex { get; set; }
            public float Volume { get; set; } = 1f;
        }
    }
}

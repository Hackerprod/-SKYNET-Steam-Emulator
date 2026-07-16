using SKYNET.Wave;
using System;
using System.Collections.Generic;

namespace SKYNET.Managers
{
    public class AudioManager
    {
        public static bool Recording;
        public static int SampleRate = 48000;
        public static int InputDeviceID;
        internal static bool VoiceCaptureEnabled => SteamEmulator.EnableVoiceCapture;

        private static readonly object SyncRoot = new object();
        private static readonly List<byte> AvailableBuffer = new List<byte>();
        private static WaveIn recorder;
        private static bool disabledLogged;

        private static bool Initialized => recorder != null;

        public static bool Initialize()
        {
            if (!VoiceCaptureEnabled)
            {
                WriteDisabledOnce();
                return false;
            }

            lock (SyncRoot)
            {
                return EnsureInitialized();
            }
        }

        internal static bool StartVoiceRecording()
        {
            lock (SyncRoot)
            {
                if (!VoiceCaptureEnabled)
                {
                    Recording = false;
                    WriteDisabledOnce();
                    return false;
                }

                if (Recording)
                {
                    return true;
                }

                if (!EnsureInitialized())
                {
                    Recording = false;
                    return false;
                }

                try
                {
                    Recording = recorder.Start();
                    if (Recording)
                    {
                        Write("StartVoiceRecording");
                    }
                    return Recording;
                }
                catch (Exception ex)
                {
                    Recording = false;
                    Write($"Error starting voice recording: {ex.GetType().Name}: {ex.Message}");
                    return false;
                }
            }
        }

        internal static void StopVoiceRecording()
        {
            lock (SyncRoot)
            {
                if (!VoiceCaptureEnabled)
                {
                    Recording = false;
                    return;
                }

                if (Recording && Initialized)
                {
                    try
                    {
                        recorder.Stop();
                    }
                    catch (Exception ex)
                    {
                        Write($"Error stopping voice recording: {ex.GetType().Name}: {ex.Message}");
                    }
                }

                Recording = false;
                Write("StopVoiceRecording");
            }
        }

        internal static bool GetAvailableVoice(out uint compressed, out uint unCompressed)
        {
            compressed = 0;
            unCompressed = 0;

            lock (SyncRoot)
            {
                if (!VoiceCaptureEnabled || !Recording)
                {
                    return false;
                }

                DrainRecorder();

                compressed = (uint)AvailableBuffer.Count;
                unCompressed = (uint)AvailableBuffer.Count;
                return AvailableBuffer.Count != 0;
            }
        }

        internal static bool GetVoice(uint maxBytes, out byte[] buffer)
        {
            lock (SyncRoot)
            {
                if (!VoiceCaptureEnabled || !Recording || maxBytes == 0)
                {
                    buffer = Array.Empty<byte>();
                    return false;
                }

                DrainRecorder();

                if (AvailableBuffer.Count == 0)
                {
                    buffer = Array.Empty<byte>();
                    return false;
                }

                int bytesToRead = (int)Math.Min(maxBytes, (uint)AvailableBuffer.Count);
                buffer = AvailableBuffer.GetRange(0, bytesToRead).ToArray();
                AvailableBuffer.RemoveRange(0, bytesToRead);
                return true;
            }
        }

        private static bool EnsureInitialized()
        {
            if (Initialized)
            {
                return true;
            }

            try
            {
                var devices = WaveIn.Devices;
                if (devices.Length == 0)
                {
                    Write("No audio input devices available");
                    return false;
                }

                var deviceIndex = InputDeviceID;
                if (deviceIndex < 0 || deviceIndex >= devices.Length)
                {
                    Write($"Input device ID {InputDeviceID} unavailable, using device 0");
                    deviceIndex = 0;
                }

                var inDevice = devices[deviceIndex];
                recorder = new WaveIn(inDevice, SampleRate, 16, 1, GetDefaultBufferSize());
                AvailableBuffer.Clear();
                Write($"Audio system initialized in device \"{inDevice.Name}\", sample rate {SampleRate}");
                return true;
            }
            catch (Exception ex)
            {
                recorder = null;
                Write($"Error initializing audio in device ID {InputDeviceID}, sample rate {SampleRate}: {ex.GetType().Name}: {ex.Message}");
                return false;
            }
        }

        private static void DrainRecorder()
        {
            if (!Initialized)
            {
                return;
            }

            byte[] data = recorder.ReadAvailable();
            if (data.Length > 0)
            {
                AvailableBuffer.AddRange(data);
            }
        }

        private static int GetDefaultBufferSize()
        {
            return Math.Max(400, (SampleRate / 50) * 2);
        }

        private static void Write(string value)
        {
            SteamEmulator.Write("AudioManager", value);
        }

        private static void WriteDisabledOnce()
        {
            if (disabledLogged)
            {
                return;
            }

            disabledLogged = true;
            Write("Voice capture disabled by configuration");
        }
    }
}

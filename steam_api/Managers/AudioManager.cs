using SKYNET.Wave;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SKYNET.Managers
{
    public class AudioManager
    {
        public static bool Recording;
        public static int SampleRate = 48000; // 22050
        public static int InputDeviceID;

        private static WaveIn recorder;
        private static List<byte> Buffer;
        private static List<byte> AvailableBuffer;
        private static bool Initialized => recorder != null;


        public static void Initialize()
        {
            ThreadPool.QueueUserWorkItem(InitializeInternal);
        }

        private static void InitializeInternal(object state)
        {
            Buffer = new List<byte>();
            AvailableBuffer = new List<byte>();

            try
            {
                var InDevice = WaveIn.Devices[InputDeviceID];
                recorder = new WaveIn(InDevice, SampleRate, 16, 1, 400);
                recorder.BufferFull += OnDataAvailable;
                Write($"Audio system initialized in device \"{InDevice.Name}\", sample rate {SampleRate}");
            }
            catch (System.Exception)
            {
                Write($"Error initializing audio in device ID {InputDeviceID}, sample rate {SampleRate}");
            }
        }

        private static void OnDataAvailable(byte[] buffer)
        {
            try { Buffer.AddRange(buffer); } catch { }
        }

        internal static void StartVoiceRecording()
        {
            if (!Recording)
            {
                if (Initialized)
                {
                    try
                    {
                        recorder.Start();
                        Recording = true;
                    }
                    catch { }
                }
                else
                {
                    Initialize();
                }
                Write("StartVoiceRecording");
            }
        }

        internal static void StopVoiceRecording()
        {
            if (Recording)
            {
                if (Initialized)
                {
                    try
                    {
                        recorder.Stop();
                        Recording = false;
                    }
                    catch { }
                }
                else
                {
                    Initialize();
                }
                Write("StopVoiceRecording");
            }
        }

        internal static bool GetAvailableVoice(out uint compressed, out uint unCompressed)
        {
            compressed = 0;
            unCompressed = 0;

            try { AvailableBuffer.AddRange(Buffer); } catch { }

            try { Buffer.Clear(); } catch { }

            compressed = (uint)AvailableBuffer.Count();
            unCompressed = (uint)AvailableBuffer.Count();

            return AvailableBuffer.Count != 0;
        }

        internal static bool GetVoice(out byte[] buffer)
        {
            byte[] bytes = default;

            try { bytes = AvailableBuffer.ToArray(); } catch { }

            buffer = bytes != null ? bytes : new byte[0] ;

            try { AvailableBuffer.Clear(); } catch { }
            return buffer.Length != 0;
        }

        private static void Write(string v)
        {
            SteamEmulator.Write("AudioManager", v);
        }
    }
}

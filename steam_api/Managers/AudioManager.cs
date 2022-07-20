using SKYNET.Wave;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SKYNET.Managers
{
    public class AudioManager
    {
        private static WaveIn recorder;
        private static List<byte> Buffer;
        private static List<byte> AvailableBuffer;
        public static int SampleRate = 22050;

        public static bool Recording { get; private set; }

        public static void Initialize()
        {
            ThreadPool.QueueUserWorkItem(InitializeInternal);
        }

        private static void InitializeInternal(object state)
        {
            Buffer = new List<byte>();
            AvailableBuffer = new List<byte>();

            recorder = new WaveIn(new WavInDevice(0, "", 1), SampleRate, 16, 1, 400);
            recorder.BufferFull += OnDataAvailable;
        }

        private static void OnDataAvailable(byte[] buffer)
        {
            try { Buffer.AddRange(buffer); } catch { }
        }

        internal static void StartVoiceRecording()
        {
            if (!Recording)
            {
                try
                {
                    recorder.Start();
                    Recording = true;
                }
                catch { }
                Write("StartVoiceRecording");
            }
        }

        internal static void StopVoiceRecording()
        {
            if (Recording)
            {
                try
                {
                    recorder.Stop();
                    Recording = false;
                }
                catch { }
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

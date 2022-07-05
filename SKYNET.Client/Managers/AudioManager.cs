using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Managers
{
    public class AudioManager
    {
        public static void Initialize()
        {

        }

        internal static void StartVoiceRecording()
        {

        }

        internal static void StopVoiceRecording()
        {

        }

        internal static void GetAvailableVoice(out uint compressed, out uint unCompressed)
        {
            compressed = 0;
            unCompressed = 0;
        }

        internal static void GetVoice(out byte[] buffer)
        {
            buffer = new byte[0];
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;

namespace SKYNET.Helpers
{
    public class Log
    {
        public static bool Initialized;

        private static List<string> buffered = new List<string>();
        private static object file_lock = new object();
        private static string lastMsg = "";
        private static string fileName;


        public static void Initialize()
        {
            var LogPath = Path.Combine(Common.GetPath(), "SKYNET");
            var suffix = Environment.GetEnvironmentVariable("SKYNET_LOG_SUFFIX");
            var logName = "steam_api.log";
            if (!string.IsNullOrWhiteSpace(suffix))
            {
                foreach (var invalid in Path.GetInvalidFileNameChars())
                {
                    suffix = suffix.Replace(invalid, '_');
                }

                logName = $"steam_api.{suffix}.log";
            }

            fileName = Path.Combine(LogPath, logName);
            Common.EnsureDirectoryExists(LogPath);
            Clean();
            Initialized = true;
        }

        internal static void AppEnd(string formatted)
        {
            if (!Initialized) return;

            try
            {
                lock (file_lock)
                {
                    if (formatted == lastMsg)
                    {
                        return;
                    }

                    buffered.Add(formatted);
                    if (File.Exists(fileName))
                    {
                        File.AppendAllLines(fileName, buffered);
                    }
                    else
                    {
                        File.WriteAllLines(fileName, buffered);
                    }
                    buffered.Clear();
                    lastMsg = formatted;
                }
            }
            catch 
            {
            }
        }

        private static void EnsurePathExists()
        {
            Common.EnsureDirectoryExists(fileName, true);
        }

        public static void Clean()
        {
            try
            {
                if (File.Exists(fileName))
                {
                    File.WriteAllText(fileName, "");
                }
            }
            catch 
            {
            }
        }
    }
}

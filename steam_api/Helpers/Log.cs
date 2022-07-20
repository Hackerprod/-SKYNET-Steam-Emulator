using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace SKYNET.Helpers
{
    public class Log
    {
        private static List<string> buffered = new List<string>();
        private static object file_lock = new object();
        private static string lastMsg = "";
        private static string fileName;
        
        static Log()
        {
            fileName = Path.Combine(modCommon.GetPath(), "SKYNET", "[SKYNET] steam_api.log");
            modCommon.EnsureDirectoryExists(fileName, true);
        }

        internal static void AppEnd(string formatted)
        {
            try
            {
                if (formatted != lastMsg)
                {
                    var taken = false;
                    Monitor.TryEnter(file_lock, ref taken);

                    if (taken)
                    {
                        buffered.Add(formatted);
                        File.AppendAllLines(fileName, buffered);
                        buffered.Clear();

                        Monitor.Exit(file_lock);
                    }
                    lastMsg = formatted;
                }
            }
            catch 
            {
            }
        }

        public static void Clean()
        {
            try
            {
                File.WriteAllText(fileName, "");
            }
            catch 
            {
            }
        }
    }
}

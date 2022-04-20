using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SKYNET.Helper
{
    public class Log
    {
        private static List<string> buffered = new List<string>();
        private static object file_lock = new object();
        private static string lastMsg = "";
        private static string fileName;
        
        static Log()
        {
            fileName = modCommon.GetPath() + "/[SKYNET] steam_api.log";
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
    }
}

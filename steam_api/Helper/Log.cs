using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Helper
{
    public class Log
    {
        private static Stream outputStream;

        static Log()
        {
            outputStream = new FileStream($"[SKYNET] steam_api.log", FileMode.OpenOrCreate);
        }

        public static void Write(object msg)
        {
            Console.WriteLine(msg);

            if (!modCommon.LogToFile)
            {
                //return;
            }

            if (File.Exists("[SKYNET] steam_api.log"))
            {
                string _msg = "";
                if (msg != null)
                {
                    _msg = msg.ToString();
                }
                byte[] bytes = Encoding.Default.GetBytes(msg + Environment.NewLine);
                outputStream.Write(bytes, 0, bytes.Length);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Helpers
{
    public class Log
    {
        private static Stream outputStream;

        public static void Write(object sender, object msg)
        {
            Write(sender + ": " + msg);
        }

        public static void Write(object msg, bool force = false)
        {
            if (!modCommon.LogToFile && !force)
            {
                //modCommon.Show(msg);
                return;
            }


            if (outputStream == null)
            {
                string _file = Path.Combine(modCommon.GetPath(), "[SKYNET] steam_api.log");
                outputStream = new FileStream(_file, FileMode.OpenOrCreate);
            }

            if (msg == null)
            {
                msg = "Message log is Null";
            }

            Console.WriteLine(msg);

            if (File.Exists(Path.Combine(modCommon.GetPath(), "[SKYNET] steam_api.log")))
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

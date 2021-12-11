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
            outputStream = new FileStream($"c:/steamEmu.log", FileMode.OpenOrCreate);
        }

        public static void Write(object msg)
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

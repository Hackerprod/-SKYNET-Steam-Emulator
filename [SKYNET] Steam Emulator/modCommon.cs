using System;
using System.IO;
using System.Windows.Forms;

namespace _SKYNET__Steam_Emulator
{
    public class modCommon
    {
        public static void Show(object msg)
        {
            MessageBox.Show(msg.ToString());
        }
        public static void EnsureDirectoryExists(string filePath, bool isFile = false)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                filePath = filePath.Trim().Replace("\0", string.Empty);
                if (!string.IsNullOrEmpty(filePath))
                {
                    try
                    {
                        string text = isFile ? Path.GetDirectoryName(filePath) : filePath;
                        if (Path.IsPathRooted(filePath))
                        {
                            text = text.Trim();
                            if (!Directory.Exists(text))
                            {
                                Directory.CreateDirectory(text);
                            }
                        }
                    }
                    catch (Exception exception)
                    {

                    }
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SKYNET // & " = |
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += ResolveAssembly;
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;

            TestJSON();
            ClearTempFiles();

            Application.ThreadException += UIThreadExceptionHandler;
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new frmLogin());
            Application.Run(new frmMain());
        }

        private static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            var fullAssemblyName = new AssemblyName(args.Name);
            string assemblyPathRoot = Path.Combine(modCommon.GetPath(), "Data", "Assemblies", fullAssemblyName.Name + ".dll");
            if (File.Exists(assemblyPathRoot))
            {
                return Assembly.LoadFrom(assemblyPathRoot);
            }
            return null;
        }

        private static void ClearTempFiles()
        {
            try
            {
                foreach (var file in Directory.GetFiles(modCommon.GetPath(), "*.*", SearchOption.TopDirectoryOnly))
                {
                    if (Path.GetExtension(file) == ".config" || Path.GetExtension(file) == ".pdb" || Path.GetExtension(file) == ".xml" || Path.GetExtension(file) == ".txt" || Path.GetExtension(file) == ".log")
                    {
                        try { File.Delete(file); } catch { }
                    }
                }
            }
            catch 
            {
            }
        }

        #region Log system

        private static object file_lock = new object();
        private static List<string> buffered = new List<string>();

        public static void UIThreadExceptionHandler(object sender, ThreadExceptionEventArgs t)
        {
            WriteException(t.Exception);
        }

        public static void UnhandledExceptionHandler(object sender, System.UnhandledExceptionEventArgs t)
        {
            WriteException(t.ExceptionObject);
        }

        public static void WriteException(object msg)
        {
            if (msg is Exception)
            {
                Exception ex = (Exception)msg;

                string filePath = Path.Combine(modCommon.GetPath(), "Data", "[SKYNET] Steam Emulator.log");

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(msg);
                string formatted = string.Format(string.Format("{0}:", stringBuilder.ToString()), Array.Empty<object>());
                var taken = false;

                Monitor.TryEnter(file_lock, ref taken);

                if (taken)
                {
                    buffered.Add(formatted);
                    File.AppendAllLines(filePath, buffered);
                    buffered.Clear();

                    Monitor.Exit(file_lock);
                }
            }
        }

        #endregion

        private static void TestJSON()
        {
            //string JSON = File.ReadAllText(@"D:\Instaladores\Programación\Projects\[SKYNET] Steam Emulator\[SKYNET] Steam Emulator\bin\Debug\Data\Storage\570\AppDetails.json");
            //AppDetails details = new JavaScriptSerializer().Deserialize<AppDetails>(JSON);
            //modCommon.Show(details.lolo.data.about_the_game);
        }
    }
}





using SKYNET.GUI;
using SKYNET.Managers;
using SKYNET.Types;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace SKYNET.GUI
{
    public partial class frmMain : Form
    {
        dynamic ChromiumWebBrowser = null;
        dynamic GeckoWebBrowser = null;

        public frmMain()
        {
            Hide();
            InitializeComponent();
        }


        private void FrmBrowser_Load(object sender, EventArgs e)
        {
            if (LoadChromium())
            {
                return;
            }

            if (LoadGecko())
            {
                return;
            }

            Process.Start("http://127.0.0.1:27088");
        }

        private bool LoadGecko()
        {
            try
            {
                // Initialize Gecko
                string FireFoxPath = GetAssemblyPath("mozglue.dll", true);
                string assemblyPath = GetAssemblyPath("Geckofx-Core.dll");
                var assembly = Assembly.LoadFile(assemblyPath);
                var Xpcom = assembly.GetExportedTypes().First(t => t.Name == "Xpcom");
                Xpcom.InvokeMember("Initialize", BindingFlags.InvokeMethod, null, null, new object[] { FireFoxPath });

                // Initialize Control
                this.GeckoWebBrowser = CreateInstance("Geckofx-Winforms.dll", "Gecko.GeckoWebBrowser");
                if (this.GeckoWebBrowser == null) return false;
                WebContainer.Controls.Add(this.GeckoWebBrowser);
                this.GeckoWebBrowser.Dock = DockStyle.Fill;
                this.GeckoWebBrowser.Navigate("http://127.0.0.1:27088");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private dynamic CreateInstance(string AssemblyName, string TypeName)
        {
            try
            {
                string assemblyPath = GetAssemblyPath(AssemblyName);
                if (string.IsNullOrEmpty(assemblyPath)) return null;
                var assembly = Assembly.LoadFile(assemblyPath);
                var Type = assembly.GetType(TypeName);
                return Activator.CreateInstance(Type);
            }
            catch 
            {
                return null;
            }
        }

        private bool LoadChromium()
        {
            try
            {
                this.ChromiumWebBrowser = CreateInstance("CefSharp.WinForms.dll", "CefSharp.WinForms.ChromiumWebBrowser");
                if (this.ChromiumWebBrowser == null) return false;
                WebContainer.Controls.Add(this.ChromiumWebBrowser);
                this.ChromiumWebBrowser.Dock = DockStyle.Fill;
                this.ChromiumWebBrowser.Load("http://127.0.0.1:27088");
                return true;
            }
            catch 
            {
                return false;
            }
        }

        private string GetAssemblyPath(string fileName, bool RootPath = false)
        {
            string assembliesPath = Path.Combine(Common.GetPath(), "Data", "Assemblies");
            Common.EnsureDirectoryExists(assembliesPath);
            foreach (var file in Directory.GetFiles(assembliesPath, "*.dll", SearchOption.AllDirectories))
            {
                if (Path.GetFileName(file) == fileName)
                {
                    if (RootPath)
                    {
                        return new FileInfo(file).Directory?.FullName;
                    }
                    return file;
                }
            }
            return null;
        }

        private void MinimizeBox_Clicked(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void CloseBox_Clicked(object sender, EventArgs e)
        {
            GameManager.Save();
            Settings.Save();
            Process.GetCurrentProcess().Kill();
        }
    }
}


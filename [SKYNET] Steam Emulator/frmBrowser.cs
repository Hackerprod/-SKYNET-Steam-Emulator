using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKYNET
{
    public partial class frmBrowser : Form
    {
        public frmBrowser()
        {
            InitializeComponent();
        }

        private void FrmBrowser_Load(object sender, EventArgs e)
        {
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "Data", "cefsharp", "locales"));

            var settings = new CefSettings();
            settings.BrowserSubprocessPath = Path.Combine(modCommon.GetPath(), "Data", "cefsharp", "CefSharp.BrowserSubprocess.exe");
            settings.LocalesDirPath = Path.Combine(modCommon.GetPath(), "Data", "cefsharp", "locales");
            settings.ResourcesDirPath = Path.Combine(modCommon.GetPath(), "Data", "cefsharp");
            //settings.CachePath = Path.Combine(modCommon.GetPath(), @"resources\cefsharp\GPUCache\");
            //settings.RootCachePath = Path.Combine(modCommon.GetPath(), @"resources\cefsharp\GPUCache\");

            Cef.Initialize(settings, performDependencyCheck: false);
            Browser.Load("127.0.0.1");
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;
using mshtml;
using System.Runtime.InteropServices;
using SKYNET.Pages;
using System.IO;
using SKYNET.Properties;
using CefSharp;
using CefSharp.WinForms;
using System.Diagnostics;

namespace SKYNET.GUI.Controls
{
    [ComVisible(true)]
    public partial class WebControl : UserControl
    {
        [Category("SKYNET")]
        public Color LoggerBackColor
        {
            get
            {
                return _LoggerBackColor;
            }
            set
            {
                _LoggerBackColor = value;
            }
        }
        Color _LoggerBackColor;

        [Category("SKYNET")]
        public Color ScrollColors
        {
            get
            {
                return _scroll;
            }
            set
            {
                _scroll = value;
            }
        }
        Color _scroll;

        public WebControl()
        {
            InitializeComponent();
            InitializeWebBrowser();
        }
        public ChromiumWebBrowser chromeBrowser;
        private void InitializeWebBrowser()
        {
            InternetExplorerBrowserEmulation.SetBrowserEmulationVersion();
            //webBrowser1.ScriptErrorsSuppressed = true;

            LoadGamePage();

            CefSettings settings = new CefSettings();
            settings.BrowserSubprocessPath = Process.GetCurrentProcess().MainModule.FileName;
            // Initialize cef with the provided settings
            Cef.Initialize(settings);
            // Create a browser component
            chromeBrowser = new ChromiumWebBrowser(@"C:\steam-skynet\index.html");
            // Add it to the form and fill it to the form window.
            this.Controls.Add(chromeBrowser);
            chromeBrowser.Dock = DockStyle.Fill;


            BrowserSettings browserSettings = new BrowserSettings();
            browserSettings.FileAccessFromFileUrls = CefState.Enabled;
            browserSettings.UniversalAccessFromFileUrls = CefState.Enabled;
            chromeBrowser.BrowserSettings = browserSettings;
        }

        public void LoadGamePage()
        {
            //webBrowser1.ScriptErrorsSuppressed = true;
            //webBrowser1.Navigate("about:blank");

            //while (webBrowser1.Document == null || webBrowser1.Document.Body == null)
            //    Application.DoEvents();

            //webBrowser1.Document.OpenNew(true).Write(HtmlPage());
            ////AssignStyleSheet();
            

            //webBrowser1.ObjectForScripting = this;


        }

        private string HtmlPage()
        {
            return Index.html("", "");
        }
        public void AssignStyleSheet()
        {
            //IHTMLStyleSheet2 instance = (IHTMLStyleSheet2)((IHTMLDocument2)webBrowser1.Document.DomDocument).createStyleSheet("", 0);

            //NewLateBinding.LateSet(instance, null, "cssText", new object[1]
            //{
            //    GetStyles()
            //}, null, null);

            //HtmlElement head = webBrowser1.Document.GetElementsByTagName("head")[0];
            //HtmlElement scriptEl = webBrowser1.Document.CreateElement("script");
            //IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;

            //element.text = JavaScript();
            //head.AppendChild(scriptEl);

        }

        private string JavaScript()
        {
            return jquery.js() + Environment.NewLine + main.js();
        }

        private object GetStyles()
        {
            return helpers.css() + Environment.NewLine + style.css();
            //return Resources.helpers + Environment.NewLine + Resources.style;
        }
    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using mshtml;
using Microsoft.VisualBasic.CompilerServices;
using System.Threading;
using System.Runtime.InteropServices;
using SKYNET.Types;

namespace SKYNET.GUI.Controls
{
    [ComVisible(true)]
    public partial class SKYNET_WebLogger : UserControl
    {
        [Category("SKYNET")]
        public event EventHandler<WebBrowserDocumentCompletedEventArgs> DocumentCompleted;

        [Category("SKYNET")]
        public event EventHandler<WebBrowserNavigatingEventArgs> Navigating;

        public override ContextMenuStrip ContextMenuStrip
        {
            get { return webChat.ContextMenuStrip; }
            set { webChat.ContextMenuStrip = value; }
        }

        private Mutex mutex;
        private List<ConsoleMessage> CallBack;
        private Font _font;
        public override Font Font
        {
            get
            {
                return _font;
            }
            set
            {
                _font = value;
            }
        }
        public SKYNET_WebLogger()
        {
            InitializeComponent();
            InitializeWebBrowser();
            _font = new Font("Segoe UI", 10);
            CallBack = new List<ConsoleMessage>();
            mutex = new Mutex(false, "CallBack");
        }
        private void InitializeWebBrowser()
        {
            InternetExplorerBrowserEmulation.SetBrowserEmulationVersion();

            webChat.ScriptErrorsSuppressed = true;
            webChat.Navigate("about:blank");

            while (webChat.Document == null || webChat.Document.Body == null)
            Application.DoEvents();

            webChat.Document.OpenNew(true).Write($"<html><head>{GetHtmlIEVersion()}{ScrollBar(_scroll) + JavaScript()} <style>{GetStyles()}</style>  <title name = 'head'>SKYNET</title>" + $"</head><body class='body' bgcolor={ColorTranslator.ToHtml(Color.FromArgb(33, 43, 53))}><table style=\"padding: 0px\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" id='table'>");
            AssignStyleSheet();

            webChat.Navigating += new WebBrowserNavigatingEventHandler(webChat_Navigating);
            //webChat.ContextMenuStrip = base.ContextMenuStrip;    //! Set our ContextMenuStrip
            webChat.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(OnDocumentCompleted);
            webChat.ObjectForScripting = this;

        }
        public static string GetHtmlIEVersion()
        {
            string str = "";
            try
            {
                if (Conversions.ToInteger(Conversions.ToString(InternetExplorerBrowserEmulation.GetInternetExplorerMajorVersion())) == 11)
                {
                    str = string.Format("<meta http-equiv='X-UA-Compatible' content='IE=edge'>");
                }
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                ProjectData.ClearProjectError();
            }
            return str;
        }

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

        [Category("SKYNET")]
        public bool AutoScrollLines
        {
            get
            {
                return _autoscroll;
            }
            set
            {
                _autoscroll = value;
            }
        }
        public bool CancelNext { get; private set; }

        bool _autoscroll = true;


        public void WriteLine(ConsoleMessage msg)
        {
            try { mutex.WaitOne(); } catch { }
            CallBack.Add(msg);
            mutex.ReleaseMutex();
            VerifyCallback();
        }
        private void VerifyCallback()
        {
            try
            {
                mutex.WaitOne();
                for (int i = 0; i < CallBack.Count; i++)
                {
                    try
                    {
                        var callback = CallBack[i];
                        if (callback == null || callback.Msg == null)
                            return;

                        string htmlMessage = GetMessage(callback);
                        Write(htmlMessage);

                        CallBack.RemoveAt(i);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch
            {

            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        public void Write(string html)
        {
            try
            {
                int bodyHeight = 0;
                bool scroll = false;

                try { bodyHeight = Convert.ToInt32(webChat.Document.InvokeScript("GetPageHeight")); } catch { }
                try { scroll = bodyHeight - Height == webChat.Document?.Body?.ScrollTop; } catch { }

                webChat.Invoke(new Action(() =>
                {
                    webChat.Document.Write(html);
                    try
                    {
                        if (scroll)
                        {
                            webChat.Document.Window.ScrollTo(0, webChat.Document.Body.ScrollRectangle.Height);
                        }
                    }
                    catch { }
                }));

            }
            catch (Exception ex)
            {
                modCommon.Show(ex);
            }
        }

        public void AssignStyleSheet()
        {
            string name = webChat.Name;
            IHTMLStyleSheet2 instance = (IHTMLStyleSheet2)((IHTMLDocument2)webChat.Document.DomDocument).createStyleSheet("", 0);

            NewLateBinding.LateSet(instance, null, "cssText", new object[1]
            {
                GetStyles()
            }, null, null);

            HtmlElement htmlElement = webChat.Document.GetElementsByTagName("head")[0];
            HtmlElement htmlElement2 = webChat.Document.CreateElement("script");
            IHTMLScriptElement iHTMLScriptElement = (IHTMLScriptElement)htmlElement2.DomElement;

            iHTMLScriptElement.text = JavaScript(); //GetJavascript();
            htmlElement.AppendChild(htmlElement2);
        }
        public static string ScrollBar(Color scroll)
        {
            return @"<style type='text/css'> body { " +
            $"scrollbar-face-color:#52a1f2; " + //barra
            $"scrollbar-highligh-color:#1d2733; " +
            $"scrollbar-3dligh-color:#1d2733; " +
            $"scrollbar-darkshadow-color:#1d2733; " +
            $"scrollbar-shadow-color:#73b5f8; " + //Borde afuera
            $"scrollbar-track-color:#1d2733; " + //Fondo de la barra
             "scrollbar-arrow-color:#52a1f2;" + //Arrow
            "} </style>" +
            "";
        }

        internal static string JavaScript()
        {
            string  intHeigth = @"
                <script>
                    function GetPageHeight()
                    {
	                    var body = document.body;
 	                    var html = document.documentElement;
 	                    var height = Math.max(body.scrollHeight ,body.offsetHeight, html.clientHeight, html.scrollHeight, html.offsetHeight);
 	                    return height;
                     }
                    function getSelectionText() 
                    { 
                        var text = '';                        
                        if (window.getSelection) 
                        {                               
                            text = window.getSelection().toString();
                        } 
                        else if (document.selection && document.selection.type != 'Control') 
                        {
                            text = document.selection.createRange().text;                        
                        }                        
                        return text;                    
                    }    
                </script>
                    ";

            return intHeigth;
        }


        private void OnDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HtmlElementCollection body = webChat.Document.GetElementsByTagName("BODY");
            for (int i = 0; i < body.Count; i++)
            {
                HtmlElement el = body[i];
                el.AttachEventHandler("onclick", (Sender, args) => OnElementSelect(el, EventArgs.Empty));
            }
            DocumentCompleted?.Invoke(sender, e);
        }
        protected void OnElementSelect(object sender, EventArgs args)
        {
            //Hecho por mi
            string selection = webChat.Document.InvokeScript("getSelectionText").ToString();
            if (!string.IsNullOrEmpty(selection))
            {
                Clipboard.Clear();
                Clipboard.SetText(selection, TextDataFormat.UnicodeText);
            }
        }

        private void webChat_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            Navigating?.Invoke(sender, e);
        }

        public void ClearScreen()
        {
            webChat.Document.OpenNew(true);
            InitializeWebBrowser();
        }
        public static string GetStyles()
        {
            string clase = @"
            .message-Body 
            {
                color: #BFBFBF;
                font-size: 10pt;
                FONT-FAMILY: Segoe UI;
            }
            .message-type {
            color: #FFFFFF;
            font-size: 9pt;
            FONT-FAMILY: Segoe UI;
            }

            ";
            return clase;
        }

    }

    public partial class SKYNET_WebLogger
    {
        private string C_1 = "#212B35";
        private string C_2 = "#242F39";
        private string C_Last = "#212B35";
        public string GetMessage(ConsoleMessage m)
        {
            string msg = m.Msg.ToString();
            string ObjectId = "";

            ObjectId = "ObjectId";

            C_Last = C_Last == C_1 ? C_2 : C_1;
            StringBuilder code = new StringBuilder();

            //Sender
            code.AppendLine($"<tr bgcolor=\"{C_Last}\">");
            code.AppendLine("<td style=\"padding-left:5; width:115\">");
            code.AppendLine($"<h5 style='color:{ColorTranslator.ToHtml(Color.White)}; text-align:top' Class='message-type'>{m.Sender}</h5>");
            code.AppendLine("</td>");

            //Body
            code.AppendLine("<td Class=\"message-Body\" style=\"padding-left:5; max-width:650px; width:650px\">");

            var lines = SplitLines(msg);
            if (lines.Count > 1)
            {
                code.AppendLine(lines[0]);
                code.AppendLine("</td>");
                code.AppendLine("</tr>");
                code.AppendLine($"<tr bgcolor=\"{C_Last}\">");
                code.AppendLine("<td style=\"padding-left:5; width:115\">");
                code.AppendLine("</td>");
                code.AppendLine("<td Class=\"message-Body\" style=\"padding-left:5; max-width:650px; width:650px\">");
                for (int i = 0; i < lines.Count; i++)
                {
                    if (i != 0)
                    {
                        string line = (i == lines.Count - 1) ? lines[i] : lines[i] + "</br>";
                        code.AppendLine(line);
                    }
                }
                code.AppendLine("</td>");
            }
            else
            {
                if (ObjectId != "")
                {
                    code.Append(msg);
                }
                else
                {
                    msg = ReplaceNewSpace(msg);
                    code.AppendLine(msg);
                }

                code.AppendLine("</td>");
            }
            code.AppendLine("</tr>");
            string Code = code.ToString();
            return Code;
        }

        private string ReplaceNewSpace(string msg)
        {
            string result = "";
            string[] lines = msg.TrimEnd('\r', '\n').Split(new[] { "\\n", "\n", "\r\n" }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                result += line + "</br>";
            }
            return result;
        }

        private List<string> SplitLines(string raw)
        {
            List<string> result = new List<string>();
            string[] lines = raw.TrimEnd('\r', '\n').Split(new[] { "\\n", "\n", "\r\n" }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                result.Add(line);
            }
            return result;
        }
    }
}

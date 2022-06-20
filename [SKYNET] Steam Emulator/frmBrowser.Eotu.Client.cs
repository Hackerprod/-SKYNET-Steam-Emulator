using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace SKYNET
{
    public partial class frmBrowser 
    {
        /////////////////////////////////////////////////////////////////////////////////////////
        /// Eotu.Client Example
        /// 

        private void browser_OnInitCompleted(object sender, EventArgs e)
        {
            if (!browserInitCompleted)
            {
                browser.AddMessageEventListener("CreateWindow", ((string json) => CreateWindow(json)));
                browser.AddMessageEventListener("SendWindowMessage", ((string json) => SendWindowMessage(json)));
                browser.AddMessageEventListener("RecvWindowMessage", ((string json) => RecvWindowMessage(json)));
                browser.AddMessageEventListener("AjaxGet", ((string json) => AjaxGet(json)));
                browser.AddMessageEventListener("ShowMessage", ((string json) => ShowMessage(json)));
                browser.AddMessageEventListener("PlaySound", ((string json) => PlaySound(json)));
                browser.AddMessageEventListener("SetWindowActivate", ((string json) => SetWindowActivate(json)));
                browser.AddMessageEventListener("SetWindowTitle", ((string json) => SetWindowTitle(json)));
                browser.AddMessageEventListener("SetWindowStyle", ((string json) => SetWindowStyle(json)));
                browser.AddMessageEventListener("SetResizeMode", ((string json) => SetResizeMode(json)));
                browser.AddMessageEventListener("SetWindowSize", ((string json) => SetWindowSize(json)));
                browser.AddMessageEventListener("Navigate", ((string json) => Navigate(json)));
                browser.WebBrowserFocus.Activate();

                //if (page_json != null)
                //{
                //    Navigate(page_json);
                //}

                //JObject data = new JObject();
                //data["type"] = Message.TYPE_INITCOMPLETED;
                //OnSendMessage(MessageEventArgs.Create(SendMessageEvent, data.ToString()));
            }
            browserInitCompleted = true;
        }

        public void CreateWindow(string json)
        {
            Console.WriteLine("CreateWindow");
            BrowserMessage message = DeserializeObject<BrowserMessage>(json);

            //BlankWindow window = new BlankWindow(this);
            //window.Show();
        }

        public void ShowMessage(string json)
        {
            Console.WriteLine("ShowMessage");
            BrowserMessage message = DeserializeObject<BrowserMessage>(json);
            MessageBox.Show(message.message, message.title);
        }

        public void PlaySound(string json)
        {
            Console.WriteLine("PlaySound");
            BrowserMessage message = DeserializeObject<BrowserMessage>(json);
            string path = message.path;
            if (message.local)
            {
                path = Path.Combine(modCommon.GetPath(), path);
                path = Path.GetFullPath(path);
            }
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(path);
            player.Play();
        }

        public void SetWindowActivate(string json)
        {
            Console.WriteLine("SetWindowActivate");
            BrowserMessage message = DeserializeObject<BrowserMessage>(json);
            TopMost = message.topmost;
            this.Activate();
        }

        public void SetWindowTitle(string json)
        {
            Console.WriteLine("SetWindowTitle");
            BrowserMessage message = DeserializeObject<BrowserMessage>(json);
            this.Text = message.title;
        }

        public void SetWindowStyle(string json)
        {
            BrowserMessage message = DeserializeObject<BrowserMessage>(json);
            Console.WriteLine("SetWindowStyle");
            //switch (message.style)
            //{
            //    case "None":
            //        this.WindowStyle = WindowStyle.None;
            //        break;
            //    case "SingleBorderWindow":
            //        this.WindowStyle = WindowStyle.SingleBorderWindow;
            //        break;
            //    case "ThreeDBorderWindow":
            //        this.WindowStyle = WindowStyle.ThreeDBorderWindow;
            //        break;
            //    case "ToolWindow":
            //        this.WindowStyle = WindowStyle.ToolWindow;
            //        break;
            //}
        }

        public void SetResizeMode(string json)
        {
            Console.WriteLine("SetResizeMode");
            BrowserMessage message = DeserializeObject<BrowserMessage>(json);
            //switch (message.mode)
            //{
            //    case "CanMinimize":
            //        this.ResizeMode = ResizeMode.CanMinimize;
            //        break;
            //    case "CanResize":
            //        this.ResizeMode = ResizeMode.CanResize;
            //        break;
            //    case "CanResizeWithGrip":
            //        this.ResizeMode = ResizeMode.CanResizeWithGrip;
            //        break;
            //}
        }

        public void SetWindowSize(string json)
        {
            Console.WriteLine("SetWindowSize");
            BrowserMessage message = DeserializeObject<BrowserMessage>(json);
            //Size = new Size(message.width, message.height);
            browser.Width = message.width;
            browser.Height = message.height;
        }

        public void Navigate(string json)
        {
            Console.WriteLine("Navigate");
            BrowserMessage message = DeserializeObject<BrowserMessage>(json);

            string url = message.url;
            if (message.local)
            {
                url = Path.Combine(modCommon.GetPath(), url);
                url = Path.GetFullPath(url);
            }
            var uri = new Uri(url);
            browser.Navigate(uri.AbsoluteUri);
            browser.WebBrowserFocus.Activate();
        }

        public void AjaxGet(string json)
        {
            Console.WriteLine("AjaxGet");
            BrowserMessage message = DeserializeObject<BrowserMessage>(json);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(message.url);
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream receiveStream = response.GetResponseStream();

            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            string data = readStream.ReadToEnd().Replace("'", "\\'");
            JSCall("Eotu.Success('" + data + "');");
        }

        public void SendWindowMessage(string json)
        {
            Console.WriteLine("SendWindowMessage");
            //OnSendMessage(MessageEventArgs.Create(SendMessageEvent, json));
        }

        public void RecvWindowMessage(string json)
        {
            Console.WriteLine("RecvWindowMessage");
            //OnRecvMessage(MessageEventArgs.Create(RecvMessageEvent, json));
        }

        private T DeserializeObject<T>(string json)
        {
            return new JavaScriptSerializer().Deserialize<T>(json);
        }

        public string JSCall(string script)
        {
            Console.WriteLine("JSCall");
            string outString = "";
            using (var js = new Gecko.AutoJSContext(browser.Window))
            {
                js.EvaluateScript(script, (Gecko.nsISupports)browser.Window.DomWindow, out outString);
            }
            return outString;
        }

        class BrowserMessage
        {
            public string type = null;
            public string title = null;
            public string style = null;
            public string mode = null;
            public string message = null;
            public int width = -1;
            public int height = -1;
            public bool local = false;
            public string url = null;
            public string path = null;
            public string data = null;
            public bool topmost = false;
        }

    }
}

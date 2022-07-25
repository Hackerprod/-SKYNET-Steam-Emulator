using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using WebSocketSharp.Server;

namespace SKYNET.Network
{
    public class WebServer
    {
        private string _rootDirectory;
        private string _webDirectory;
        private HttpServer httpsv;
        private int ServerPort;
        private readonly string[] _indexFiles =
        {
            "index.html",
            "index.htm",
            "default.html",
            "default.htm"
        };
        private static Dictionary<string, string> _mimeTypeMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {
        #region extension to MIME type list
        { ".asf", "video/x-ms-asf" },
        { ".asx", "video/x-ms-asf" },
        { ".avi", "video/x-msvideo" },
        { ".bin", "application/octet-stream" },
        { ".cco", "application/x-cocoa" },
        { ".crt", "application/x-x509-ca-cert" },
        { ".css", "text/css" },
        { ".deb", "application/octet-stream" },
        { ".der", "application/x-x509-ca-cert" },
        { ".dll", "application/octet-stream" },
        { ".dmg", "application/octet-stream" },
        { ".ear", "application/java-archive" },
        { ".eot", "application/octet-stream" },
        { ".exe", "application/octet-stream" },
        { ".flv", "video/x-flv" },
        { ".gif", "image/gif" },
        { ".hqx", "application/mac-binhex40" },
        { ".htc", "text/x-component" },
        { ".htm", "text/html" },
        { ".html", "text/html" },
        { ".ico", "image/x-icon" },
        { ".img", "application/octet-stream" },
        { ".iso", "application/octet-stream" },
        { ".jar", "application/java-archive" },
        { ".jardiff", "application/x-java-archive-diff" },
        { ".jng", "image/x-jng" },
        { ".jnlp", "application/x-java-jnlp-file" },
        { ".jpeg", "image/jpeg" },
        { ".jpg", "image/jpeg" },
        { ".js", "application/x-javascript" },
        { ".mml", "text/mathml" },
        { ".mng", "video/x-mng" },
        { ".mov", "video/quicktime" },
        { ".mp3", "audio/mpeg" },
        { ".mpeg", "video/mpeg" },
        { ".mpg", "video/mpeg" },
        { ".msi", "application/octet-stream" },
        { ".msm", "application/octet-stream" },
        { ".msp", "application/octet-stream" },
        { ".pdb", "application/x-pilot" },
        { ".pdf", "application/pdf" },
        { ".pem", "application/x-x509-ca-cert" },
        { ".pl", "application/x-perl" },
        { ".pm", "application/x-perl" },
        { ".png", "image/png" },
        { ".prc", "application/x-pilot" },
        { ".ra", "audio/x-realaudio" },
        { ".rar", "application/x-rar-compressed" },
        { ".rpm", "application/x-redhat-package-manager" },
        { ".rss", "text/xml" },
        { ".run", "application/x-makeself" },
        { ".sea", "application/x-sea" },
        { ".shtml", "text/html" },
        { ".sit", "application/x-stuffit" },
        { ".swf", "application/x-shockwave-flash" },
        { ".tcl", "application/x-tcl" },
        { ".tk", "application/x-tcl" },
        { ".txt", "text/plain" },
        { ".war", "application/java-archive" },
        { ".wbmp", "image/vnd.wap.wbmp" },
        { ".wmv", "video/x-ms-wmv" },
        { ".xml", "text/xml" },
        { ".xpi", "application/x-xpinstall" },
        { ".zip", "application/zip" },
        { ".json", "application/json" },
        #endregion
    };

        public WebServer(int port)
        {
            httpsv = new HttpServer(port);
            httpsv.DocumentRootPath = Path.Combine(Common.GetPath(), "Data");
            httpsv.OnGet += OnGet;
            this._rootDirectory = Path.Combine(Common.GetPath(), "Data");
            _webDirectory = Path.Combine(Common.GetPath(), "Data", "www");
            ServerPort = port;
        }

        private void OnGet(object sender, HttpRequestEventArgs e)
        {
            string filename = e.Request.Url.AbsolutePath;
            filename = filename.Substring(1);
            //Write(e.Request.HttpMethod + " " + e.Request.Url.ToString());

            if (string.IsNullOrEmpty(filename))
            {
                foreach (string indexFile in _indexFiles)
                {
                    if (File.Exists(Path.Combine(_webDirectory, indexFile)))
                    {
                        filename = indexFile;
                        break;
                    }
                }
            }

            var isweb = File.Exists(Path.Combine(_webDirectory, filename));
            if (isweb)
                filename = Path.Combine(_webDirectory, filename);
            else
                filename = Path.Combine(_rootDirectory, filename);

            if (File.Exists(filename))
            {
                try
                {
                    Stream input = new FileStream(filename, FileMode.Open);

                    e.Response.ContentType = _mimeTypeMappings.TryGetValue(Path.GetExtension(filename), out string mime) ? mime : "application/octet-stream";
                    e.Response.ContentLength64 = input.Length;
                    
                    byte[] buffer = new byte[input.Length];
                    int nbytes;
                    while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        e.Response.OutputStream.Write(buffer, 0, nbytes);
                        e.Response.ContentLength64 = buffer.Length;
                    }
                    input.Close();
                   
                    e.Response.StatusCode = (int)HttpStatusCode.OK;
                    e.Response.OutputStream.Flush();
                }
                catch 
                {
                    e.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
            }
            else
            {
                e.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            e.Response.OutputStream.Close();
            e.Response.Close();
        }

        public bool Start()
        {
            try
            {
                httpsv.Start();
                //Log.Write("WebServer", $"Web server started on port {ServerPort}");
                return true;
            }
            catch
            {
                Write($"Error starting File server on port {ServerPort}");
                return false;
            }
        }

        internal void Stop()
        {
            httpsv.Stop();
        }

        private void Write(string v)
        {
            Log.Write("HTTPServer", v);
        }
    }
}

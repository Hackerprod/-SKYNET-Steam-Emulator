using SKYNET.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using WebSocketSharp.Server;

namespace SKYNET
{
    public class FileServer
    {
        private string _rootDirectory;
        private HttpServer httpsv;
        private int ServerPort;
        private readonly string[] _indexFiles =
            {
        "index.html",
        "index.htm",
        "default.html",
        "default.htm"
        };
        private static IDictionary<string, string> _mimeTypeMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {
        #region extension to MIME type list
        {".asf", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".avi", "video/x-msvideo"},
        {".bin", "application/octet-stream"},
        {".cco", "application/x-cocoa"},
        {".crt", "application/x-x509-ca-cert"},
        {".css", "text/css"},
        {".deb", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dll", "application/octet-stream"},
        {".dmg", "application/octet-stream"},
        {".ear", "application/java-archive"},
        {".eot", "application/octet-stream"},
        {".exe", "application/octet-stream"},
        {".flv", "video/x-flv"},
        {".gif", "image/gif"},
        {".hqx", "application/mac-binhex40"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".ico", "image/x-icon"},
        {".img", "application/octet-stream"},
        {".iso", "application/octet-stream"},
        {".jar", "application/java-archive"},
        {".jardiff", "application/x-java-archive-diff"},
        {".jng", "image/x-jng"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".mml", "text/mathml"},
        {".mng", "video/x-mng"},
        {".mov", "video/quicktime"},
        {".mp3", "audio/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpg", "video/mpeg"},
        {".msi", "application/octet-stream"},
        {".msm", "application/octet-stream"},
        {".msp", "application/octet-stream"},
        {".pdb", "application/x-pilot"},
        {".pdf", "application/pdf"},
        {".pem", "application/x-x509-ca-cert"},
        {".pl", "application/x-perl"},
        {".pm", "application/x-perl"},
        {".png", "image/png"},
        {".prc", "application/x-pilot"},
        {".ra", "audio/x-realaudio"},
        {".rar", "application/x-rar-compressed"},
        {".rpm", "application/x-redhat-package-manager"},
        {".rss", "text/xml"},
        {".run", "application/x-makeself"},
        {".sea", "application/x-sea"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".swf", "application/x-shockwave-flash"},
        {".tcl", "application/x-tcl"},
        {".tk", "application/x-tcl"},
        {".txt", "text/plain"},
        {".war", "application/java-archive"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wmv", "video/x-ms-wmv"},
        {".xml", "text/xml"},
        {".xpi", "application/x-xpinstall"},
        {".zip", "application/zip"},
        #endregion
    };

        public FileServer(int port)
        {
            httpsv = new HttpServer(port);
            httpsv.DocumentRootPath = Path.Combine(modCommon.GetPath(), "Data", "www");
            httpsv.OnGet += OnGet;
            this._rootDirectory = Path.Combine(modCommon.GetPath(), "Data", "www"); 
            ServerPort = port;
        }

        private void OnGet(object sender, HttpRequestEventArgs e)
        {
            string filename = e.Request.Url.AbsolutePath;
            filename = filename.Substring(1);

            if (string.IsNullOrEmpty(filename))
            {
                foreach (string indexFile in _indexFiles)
                {
                    if (File.Exists(Path.Combine(_rootDirectory, indexFile)))
                    {
                        filename = indexFile;
                        break;
                    }
                }
            }

            filename = Path.Combine(_rootDirectory, filename);
            if (File.Exists(filename))
            {
                try
                {

                    Stream input = new FileStream(filename, FileMode.Open);

                    //Adding permanent http response headers
                    string mime;
                    e.Response.ContentType = _mimeTypeMappings.TryGetValue(Path.GetExtension(filename), out mime) ? mime : "application/octet-stream";
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
                catch (Exception ex)
                {
                    e.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }

            }
            else
            {
                e.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            e.Response.OutputStream.Close();
        }

        private string GetFileName(string filePath, string filename)
        {
            string path = "";
            if (filePath[filePath.Length - 1] == '/')
                path = filePath + filename;
            else
                path = filePath + "/" + filename;

            return path;
        }

        private static string GetcontentType(string extension)
        {
            switch (extension)
            {
                case ".avi":
                    return "video/x-msvideo";
                case ".css":
                    return "text/css";
                case ".doc":
                    return "application/msword";
                case ".gif":
                    return "image/gif";
                case ".htm":
                case ".html":
                    return "text/html";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".js":
                    return "application/x-javascript";
                case ".mp3":
                    return "audio/mpeg";
                case ".png":
                    return "image/png";
                case ".pdf":
                    return "application/pdf";
                case ".ppt":
                    return "application/vnd.ms-powerpoint";
                case ".zip":
                    return "application/zip";
                case ".txt":
                    return "text/plain";
            }
            return "application/octet-stream";
        }
        private bool IsOtherBasePath(string RawUrl)
        {
            if (RawUrl == "")
            {
                return false;
            }
            try
            {
                if (RawUrl.Contains(@"\"))
                {
                    RawUrl = RawUrl.Replace(@"\", "/");
                    string[] Directories = RawUrl.Split('/');
                    if (Directories.Any())
                    {
                        if (Directory.GetDirectories(_rootDirectory, Directories[0], SearchOption.TopDirectoryOnly).ToList().Count > 0)
                            return false;
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool Start()
        {
            try
            {
                httpsv.Start();
                Log.Write("WebServer", $"Web server started on port {ServerPort}");
                return true;
            }
            catch
            {
                Log.Error("WebServer", $"Error starting File server on port {ServerPort}");
                return false;
            }
        }

        internal void Stop()
        {
            httpsv.Stop();
        }
    }
}

using Gecko;
using SKYNET.GUI;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SKYNET
{
    public partial class frmBrowser : frmBase
    {
        GeckoWebBrowser browser;
        AutoJSContext JavaScriptContext;
        private bool browserInitCompleted;

        public frmBrowser()
        {
            InitializeComponent();
            BlurEffect = true;
        }


        private void FrmBrowser_Load(object sender, EventArgs e)
        {
            string FireFoxPath = Path.Combine(modCommon.GetPath(), "Data", "Firefox");
            Xpcom.Initialize(FireFoxPath);

            InitializePref();
            InitializePreferences();

            browser = new GeckoWebBrowser()
            {
                Dock = DockStyle.Fill
            };

            browser.DocumentCompleted += _browser_DocumentCompleted;
            //browser.ConsoleMessage += Browser_ConsoleMessage;

            PN_WebContainer.Controls.Add(browser);
            Xpcom.InitChromeContext();
            //JavaScriptContext = new AutoJSContext(browser.Window);

            TB_Url.Text = "127.0.0.1/index.html";
            browser.Navigate(TB_Url.Text);
        }

        private void Browser_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            modCommon.Show(e.Message);
        }

        private void CallmeAction(string obj)
        {
            MessageBox.Show(obj, "c# Application");
        }

        private void _browser_DocumentCompleted(object sender, Gecko.Events.GeckoDocumentCompletedEventArgs e)
        {
            //Text = e.Uri.ToString();
            TB_Url.Text = e.Uri.ToString();
            UpdateJavaScriptContext();
            browser_OnInitCompleted(null, null);
        }

        private void UpdateJavaScriptContext()
        {
            //JavaScriptContext = new AutoJSContext(browser.Window);
            //browser.AddMessageEventListener("CallmeAction", CallmeAction);
        }

        private void InitializePref()
        {
            nsIPrefBranch pref = Xpcom.GetService<nsIPrefBranch>("@mozilla.org/preferences-service;1");
            // Show same page as firefox does for unsecure SSL/TLS connections ...
            pref.SetIntPref("browser.ssl_override_behavior", 1);
            pref.SetIntPref("security.OCSP.enabled", 0);
            pref.SetBoolPref("security.OCSP.require", false);
            pref.SetBoolPref("extensions.hotfix.cert.checkAttributes", true);
            pref.SetBoolPref("security.remember_cert_checkbox_default_setting", true);
            pref.SetBoolPref("services.sync.prefs.sync.security.default_personal_cert", true);
            pref.SetBoolPref("browser.xul.error_pages.enabled", true);
            pref.SetBoolPref("browser.xul.error_pages.expert_bad_cert", false);

            // disable caching of http documents
            pref.SetBoolPref("network.http.use-cache", false);

            // disalbe memory caching
            pref.SetBoolPref("browser.cache.memory.enable", false);

            // Desktop Notification
            pref.SetBoolPref("notification.feature.enabled", true);

            // WebSMS
            pref.SetBoolPref("dom.sms.enabled", true);
            pref.SetCharPref("dom.sms.whitelist", "");

            // WebContacts
            pref.SetBoolPref("dom.mozContacts.enabled", true);
            pref.SetCharPref("dom.mozContacts.whitelist", "");

            pref.SetBoolPref("social.enabled", false);

            // WebAlarms
            pref.SetBoolPref("dom.mozAlarms.enabled", true);

            // WebSettings
            pref.SetBoolPref("dom.mozSettings.enabled", true);

            pref.SetBoolPref("network.jar.open-unsafe-types", true);
            pref.SetBoolPref("security.warn_entering_secure", false);
            pref.SetBoolPref("security.warn_entering_weak", false);
            pref.SetBoolPref("security.warn_leaving_secure", false);
            pref.SetBoolPref("security.warn_viewing_mixed", false);
            pref.SetBoolPref("security.warn_submit_insecure", false);
            pref.SetIntPref("security.ssl.warn_missing_rfc5746", 1);
            pref.SetBoolPref("security.ssl.enable_false_start", false);
            pref.SetBoolPref("security.enable_ssl3", true);
            pref.SetBoolPref("security.enable_tls", true);
            pref.SetBoolPref("security.enable_tls_session_tickets", true);
            pref.SetIntPref("privacy.popups.disable_from_plugins", 2);

            // don't store passwords
            pref.SetIntPref("security.ask_for_password", 1);
            pref.SetIntPref("security.password_lifetime", 0);
            pref.SetBoolPref("signon.prefillForms", false);
            pref.SetBoolPref("signon.rememberSignons", false);
            pref.SetBoolPref("browser.fixup.hide_user_pass", false);
            pref.SetBoolPref("privacy.item.passwords", true);
        }

        private void InitializePreferences()
        {
            // In order to use Components.classes etc we need to enable certan privileges. 
            GeckoPreferences.User["capability.principal.codebase.p0.granted"] = "UniversalXPConnect";
            GeckoPreferences.User["capability.principal.codebase.p0.id"] = "file://";
            GeckoPreferences.User["capability.principal.codebase.p0.subjectName"] = "";
            GeckoPreferences.User["security.fileuri.strict_origin_policy"] = false;

            GeckoPreferences.User["network.proxy.http"] = string.Empty;
            GeckoPreferences.User["network.proxy.http_port"] = 80;
            GeckoPreferences.User["network.proxy.type"] = 1; // 0 = direct (uses system settings on Windows), 1 = manual configuration
                                                             // Try some settings to reduce memory consumption by the mozilla browser engine.
                                                             // Testing on Linux showed eventual substantial savings after several cycles of viewing
                                                             // all the pages and then going to the publish tab and producing PDF files for several
                                                             // books with embedded jpeg files.  (physical memory 1,153,864K, managed heap 37,789K
                                                             // instead of physical memory 1,952,380K, managed heap 37,723K for stepping through the
                                                             // same operations on the same books in the same order.  I don't know why managed heap
                                                             // changed although it didn't change much.)
                                                             // See http://kb.mozillazine.org/About:config_entries, http://www.davidtan.org/tips-reduce-firefox-memory-cache-usage
                                                             // and http://forums.macrumors.com/showthread.php?t=1838393.
            GeckoPreferences.User["memory.free_dirty_pages"] = true;
            // Do NOT set this to zero. Somehow that disables following hyperlinks within a document (e.g., the ReadMe
            // for the template starter, BL-5321).
            GeckoPreferences.User["browser.sessionhistory.max_entries"] = 1;
            GeckoPreferences.User["browser.sessionhistory.max_total_viewers"] = 0;
            GeckoPreferences.User["browser.cache.memory.enable"] = false;

            // Some more settings that can help to reduce memory consumption.
            // (Tested in switching pages in the Edit tool.  These definitely reduce consumption in that test.)
            // See http://www.instantfundas.com/2013/03/how-to-keep-firefox-from-using-too-much.html
            // and http://kb.mozillazine.org/Memory_Leak.
            // maximum amount of memory used to cache decoded images
            GeckoPreferences.User["image.mem.max_decoded_image_kb"] = 40960;        // 40MB (default = 256000 == 250MB)
                                                                                    // maximum amount of memory used by javascript
            GeckoPreferences.User["javascript.options.mem.max"] = 40960;            // 40MB (default = -1 == automatic)
                                                                                    // memory usage at which javascript starts garbage collecting
            GeckoPreferences.User["javascript.options.mem.high_water_mark"] = 20;   // 20MB (default = 128 == 128MB)
                                                                                    // SurfaceCache is an imagelib-global service that allows caching of temporary
                                                                                    // surfaces. Surfaces normally expire from the cache automatically if they go
                                                                                    // too long without being accessed.
                                                                                    // 40MB is not enough for pdfjs to work reliably with some (large?) jpeg images with some test data.
                                                                                    // (See https://silbloom.myjetbrains.com/youtrack/issue/BL-6247.)  That value was chosen arbitrarily
                                                                                    // a couple of years ago, possibly to match image.mem.max_decoded_image_kb and javascript.options.mem.max
                                                                                    // above.  It seemed to work okay until we stumbled across occasional books that refused to display their
                                                                                    // jpeg files.  70MB was enough in my testing of a couple of those books, but let's go with 100MB since
                                                                                    // other books may well need more.  (Mozilla seems to have settled on 1GB for the default surfacecache
                                                                                    // size, but that doesn't appear to be needed in the Bloom context.)  Most Linux systems are 64-bit and
                                                                                    // run a 64-bit version of of Bloom, while Bloom on Windows is still a 32-bit program regardless of the
                                                                                    // system.  Since Windows Bloom uses Adobe Acrobat code to display PDF files, it doesn't need the larger
                                                                                    // size for surfacecache, and that memory may be needed elsewhere.

            GeckoPreferences.User["image.mem.surfacecache.max_size_kb"] = 40960;    // 40MB
            GeckoPreferences.User["image.mem.surfacecache.min_expiration_ms"] = 500;    // 500ms (default = 60000 == 60sec)

            // maximum amount of memory for the browser cache (probably redundant with browser.cache.memory.enable above, but doesn't hurt)
            GeckoPreferences.User["browser.cache.memory.capacity"] = 0;             // 0 disables feature

            // do these do anything?
            //GeckoPreferences.User["javascript.options.mem.gc_frequency"] = 5;	// seconds?
            //GeckoPreferences.User["dom.caches.enabled"] = false;
            //GeckoPreferences.User["browser.sessionstore.max_tabs_undo"] = 0;	// (default = 10)
            //GeckoPreferences.User["network.http.use-cache"] = false;

            // These settings prevent a problem where the gecko instance running the add page dialog
            // would request several images at once, but we were not able to generate the image
            // because we could not make additional requests of the localhost server, since some limit
            // had been reached. I'm not sure all of them are needed, but since in this program we
            // only talk to our own local server, there is no reason to limit any requests to the server,
            // so increasing all the ones that look at all relevant seems like a good idea.
            GeckoPreferences.User["network.http.max-persistent-connections-per-server"] = 200;
            GeckoPreferences.User["network.http.pipelining.maxrequests"] = 200;
            GeckoPreferences.User["network.http.pipelining.max-optimistic-requests"] = 200;

            // Graphite support was turned off by default in Gecko45. Back on in 49, but we don't have that yet.
            // We always want it, so may as well keep this permanently.
            GeckoPreferences.User["gfx.font_rendering.graphite.enabled"] = true;

            // This suppresses the normal zoom-whole-window behavior that Gecko normally does when using the mouse while
            // while holding crtl. Code in bloomEditing.js provides a more controlled zoom of just the body.
            GeckoPreferences.User["mousewheel.with_control.action"] = 0;

            // These two allow the sign language toolbox to capture a camera without asking the user's permission...
            // which we have no way to do, so it otherwise just fails.
            GeckoPreferences.User["media.navigator.enabled"] = true;
            GeckoPreferences.User["media.navigator.permission.disabled"] = true;

            // (In Geckofx60) Video is being rendered with a different thread to the main page.
            // However for some paint operations, the main thread temporary changes the ImageFactory on the container
            // (shared by both threads) to a BasicImageFactory, which is incompatible with the video decoding.
            // So if BasicImageFactory is set while a video image is being decoded, the decoding fails, resulting in
            // an unhelpful "Out of Memory" error.  If HW composing is on, then the main thread doesn't switch to the
            // BasicImageFactory, as composing is cheap (since FF is now using LAYERS_OPENGL on Linux instead of
            // LAYERS_BASIC).  [analysis courtesy of Tom Hindle]
            // This setting is needed only on Linux as far as we can tell.


        }

        private void BT_JSFunction_Click(object sender, EventArgs e)
        {
            JavaScriptContext.EvaluateScript($"myFunction('{TB_Message.Text}');", out string Result);
            modCommon.Show(Result);

            JSCall("Eotu.Success('" + TB_Message.Text + "');");
        }

        #region CrossLanguageTests

        #region JavaScriptToCSharpCallBack
        public class MyCSharpComClassFactory : nsIFactory
        {
            public IntPtr CreateInstance(nsISupports aOuter, ref Guid iid)
            {
                var obj = new MyCSharpComClass();
                return Marshal.GetIUnknownForObject(obj);
            }

            public void LockFactory(bool @lock)
            {

            }
        }

        // if you want you use a custom com interface one has to register it with firefox
        // see interfaces in https://developer.mozilla.org/en/Chrome_Registration#manifest
        // to produce a xpt file one has to convert a idl file to xpt.
        public class MyCSharpComClass : nsICommandHandler
        {
            public static int _execCount = 0;
            public static string _aCommand;
            public static string _aParameters;

            public string Exec(string aCommand, string aParameters)
            {
                _execCount++;
                _aCommand = aCommand;
                _aParameters = aParameters;

                // TODO: work out how to return C# string.
                modCommon.Show(aCommand);
                return null;
            }

            public string Query(string aCommand, string aParameters)
            {
                return null;
            }
        }

        public void JavaScriptToCSharpCallBack()
        {
            // Note: Firefox 17 removed enablePrivilege #546848 - refactored test so that javascript to create "@mozillazine.org/example/priority;1" is now executated by AutoJsContext 

            // Register a C# COM Object

            const string ComponentManagerCID = "91775d60-d5dc-11d2-92fb-00e09805570f";
            nsIComponentRegistrar mgr = (nsIComponentRegistrar)Xpcom.GetObjectForIUnknown((IntPtr)Xpcom.GetService(new Guid(ComponentManagerCID)));
            Guid aClass = new Guid("a7139c0e-962c-44b6-bec3-aaaaaaaaaaab");
            mgr.RegisterFactory(ref aClass, "Example C sharp com component", "@geckofx/mysharpclass;1", new MyCSharpComClassFactory());

            // In order to use Components.classes etc we need to enable certan privileges. 
            GeckoPreferences.User["capability.principal.codebase.p0.granted"] = "UniversalXPConnect";
            GeckoPreferences.User["capability.principal.codebase.p0.id"] = "file://";
            GeckoPreferences.User["capability.principal.codebase.p0.subjectName"] = "";
            GeckoPreferences.User["security.fileuri.strict_origin_policy"] = false;

            //browser.JavascriptError += (x, w) => Console.WriteLine(w.Message);

            string inithtml = "<html><body></body></html>";

            string initialjavascript =
                "var myClassInstance = Components.classes['@geckofx/mysharpclass;1'].createInstance(Components.interfaces.nsICommandHandler); myClassInstance.exec('hello', 'world');";

            // Create temp file to load 
            var tempfilename = Path.GetTempFileName();
            tempfilename += ".html";
            using (TextWriter tw = new StreamWriter(tempfilename))
            {
                tw.WriteLine(inithtml);
                tw.Close();
            }

            browser.Navigate(tempfilename);
            browser.NavigateFinishedNotifier.BlockUntilNavigationFinished();
            File.Delete(tempfilename);

            using (var context = new AutoJSContext(browser.Window))
            {
                string result = String.Empty;
                bool success = context.EvaluateScript(initialjavascript, out result);
                Console.WriteLine("success = {1} result = {0}", result, success);
            }

            // Test the results
            //Assert.AreEqual(MyCSharpComClass._execCount, 1);
            //Assert.AreEqual(MyCSharpComClass._aCommand, "hello");
            //Assert.AreEqual(MyCSharpComClass._aParameters, "world");
        }
        #endregion

        #region CSharpInvokingJavascriptComObjects

        public class MyCSharpClassThatContainsXpComJavascriptObjectsFactory : nsIFactory
        {
            public IntPtr CreateInstance(nsISupports aOuter, ref Guid iid)
            {
                var obj = new MyCSharpClassThatContainsXpComJavascriptObjects();
                return Marshal.GetIUnknownForObject(obj);
            }

            public void LockFactory(bool @lock)
            {

            }
        }

        /// <summary>
        /// TODO: currenly I am abusing the nsIWebPageDescriptor interface just to make the CurrentDescriptor attribute return the nsIComponentRegistrar
        /// This allows my to dynamically register javascript xpcom factories.
        /// </summary>
        public class MyCSharpClassThatContainsXpComJavascriptObjects : nsIWebPageDescriptor
        {

            public void LoadPage(nsISupports aPageDescriptor, uint aDisplayType)
            {
                throw new NotImplementedException();
            }

            public nsISupports GetCurrentDescriptorAttribute()
            {
                const string ComponentManagerCID = "91775d60-d5dc-11d2-92fb-00e09805570f";
                nsIComponentRegistrar mgr = (nsIComponentRegistrar)Xpcom.GetObjectForIUnknown((IntPtr)Xpcom.GetService(new Guid(ComponentManagerCID)));
                return (nsISupports)mgr;
            }
        }

        public void CSharpInvokingJavascriptComObjects()
        {
            // Note: Firefox 17 removed enablePrivilege #546848 - refactored test so that javascript to create "@mozillazine.org/example/priority;1" is now executated by AutoJsContext 

            // Register a C# COM Object

            // TODO would be nice to get nsIComponentRegistrar the xpcom way with CreateInstance
            // ie Xpcom.CreateInstance<nsIComponentRegistrar>(...
            Guid aClass = new Guid("a7139c0e-962c-44b6-bec3-aaaaaaaaaaac");
            var factory = new MyCSharpClassThatContainsXpComJavascriptObjectsFactory();
            Xpcom.ComponentRegistrar.RegisterFactory(ref aClass, "Example C sharp com component", "@geckofx/myclass;1", factory);

            // In order to use Components.classes etc we need to enable certan privileges. 
            GeckoPreferences.User["capability.principal.codebase.p0.granted"] = "UniversalXPConnect";
            GeckoPreferences.User["capability.principal.codebase.p0.id"] = "file://";
            GeckoPreferences.User["capability.principal.codebase.p0.subjectName"] = "";
            GeckoPreferences.User["security.fileuri.strict_origin_policy"] = false;

            //browser.JavascriptError += (x, w) => Console.WriteLine("Message = {0}", w.Message);

            string intialPage = "<html><body></body></html>";

            string initialjavascript =
                "var myClassInstance = Components.classes['@geckofx/myclass;1'].createInstance(Components.interfaces.nsIWebPageDescriptor);" +
                "var reg = myClassInstance.currentDescriptor.QueryInterface(Components.interfaces.nsIComponentRegistrar);" +
                "Components.utils.import(\"resource://gre/modules/XPCOMUtils.jsm\"); " +
                "const nsISupportsPriority = Components.interfaces.nsISupportsPriority;" +
                "const nsISupports = Components.interfaces.nsISupports;" +
                "const CLASS_ID = Components.ID(\"{1C0E8D86-B661-40d0-AE3D-CA012FADF170}\");" +
                "const CLASS_NAME = \"My Supports Priority Component\";" +
                "const CONTRACT_ID = \"@mozillazine.org/example/priority;1\";" +
                "function MyPriority() {" +
                "	this._priority = nsISupportsPriority.PRIORITY_LOWEST;" +
                "};" +
                "MyPriority.prototype = {" +
                "  _priority: null," +

                "  get priority() { return this._priority; }," +
                "  set priority(aValue) { this._priority = aValue; }," +

                "  adjustPriority: function(aDelta) {" +
                "	this._priority += aDelta;" +
                "  }," +

                "  QueryInterface: function(aIID)" +
                "  { " +
                "	/*if (!aIID.equals(nsISupportsPriority) &&    " +
                "		!aIID.equals(nsISupports))" +
                "	  throw Components.results.NS_ERROR_NO_INTERFACE;*/" +
                "	return this;" +
                "  }" +
                "};" +
                "" +
                "var MyPriorityFactory = {" +
                "  createInstance: function (aOuter, aIID)" +
                "  { " +
                "	if (aOuter != null)" +
                "	  throw Components.results.NS_ERROR_NO_AGGREGATION; " +
                "	return (new MyPriority()).QueryInterface(aIID);" +
                "  }" +
                "};" +
                "" +
                "var MyPriorityModule = {" +
                "  _firstTime: true," +
                "  registerSelf: function(aCompMgr, aFileSpec, aLocation, aType)" +
                "  {" +
                "	aCompMgr = aCompMgr.QueryInterface(Components.interfaces.nsIComponentRegistrar);" +
                "	aCompMgr.registerFactory(CLASS_ID, CLASS_NAME, CONTRACT_ID, MyPriorityFactory);" +
                "  }," +
                "" +
                "  unregisterSelf: function(aCompMgr, aLocation, aType)" +
                "  {" +
                "	aCompMgr = aCompMgr.QueryInterface(Components.interfaces.nsIComponentRegistrar);" +
                "	aCompMgr.unregisterFactoryLocation(CLASS_ID, aLocation);        " +
                "  }," +
                "" +
                "  getClassObject: function(aCompMgr, aCID, aIID)" +
                "  {alert('hi');" +
                "	if (!aIID.equals(Components.interfaces.nsIFactory))" +
                "	  throw Components.results.NS_ERROR_NOT_IMPLEMENTED;" +
                "" +
                "	if (aCID.equals(CLASS_ID))" +
                "	  return MyPriorityFactory;" +
                "" +
                "	throw Components.results.NS_ERROR_NO_INTERFACE;" +
                "  }," +
                "" +
                "  canUnload: function(aCompMgr) { return true; }" +
                "};" +
                "MyPriorityModule.registerSelf(reg);" +
                "";

            // Create temp file to load 
            var tempfilename = Path.GetTempFileName();
            tempfilename += ".html";
            using (TextWriter tw = new StreamWriter(tempfilename))
            {
                tw.WriteLine(intialPage);
                tw.Close();
            }

            browser.Navigate(tempfilename);
            browser.NavigateFinishedNotifier.BlockUntilNavigationFinished();

            using (var context = new AutoJSContext(browser.Window))
            {
                string result = String.Empty;
                var success = context.EvaluateScript(initialjavascript, out result);
                Console.WriteLine("sucess = {0} result = {1}", success, result);
            }

            File.Delete(tempfilename);

            // Create instance of javascript xpcom objects
            var p = Xpcom.CreateInstance<nsISupportsPriority>("@mozillazine.org/example/priority;1");
            //Assert.NotNull(p);

            // test invoking method of javascript xpcom object.
            p.GetPriorityAttribute();

            Xpcom.ComponentRegistrar.UnregisterFactory(aClass, factory);
        }

        #endregion

        #endregion

        private void BT_Go_Click(object sender, EventArgs e)
        {
            browser.Navigate(TB_Url.Text);
        }

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("User32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            const int WM_NCPAINT = 0x85;
            if (m.Msg == WM_NCPAINT)
            {
                IntPtr hdc = GetWindowDC(m.HWnd);
                if ((int)hdc != 0)
                {
                    Graphics g = Graphics.FromHdc(hdc);
                    g.FillRectangle(Brushes.Red, new Rectangle(0, 0, 4800, 23));
                    g.Flush();
                    ReleaseDC(m.HWnd, hdc);
                }
            }
        }
    }
}


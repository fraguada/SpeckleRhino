using System;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using System.IO;
using System.Reflection;

namespace SpeckleRhino
{
  [System.Runtime.InteropServices.Guid("041397A5-BB8B-4FCA-B580-1E6A7F0B2137")]
  public partial class SpeckleRhinoPanelControl : UserControl
  {
    private ChromiumWebBrowser m_browser;

    /// <summary>
    /// Returns the ID of this panel.
    /// </summary>
    public static Guid PanelId
    {
      get
      {
        return typeof(SpeckleRhinoPanelControl).GUID;
      }
    }

    public SpeckleRhinoPanelControl()
    {
      InitializeComponent();
      InitializeBrowser();

      m_browser.RegisterJsObject("cefCustomObject", new CefCustomObject(m_browser, this));

      SpeckleRhinoPlugIn.Instance.UserControl = this;

      m_browser.LoadingStateChanged += OnLoadingStateChanged;
      m_browser.ConsoleMessage += OnBrowserConsoleMessage;
      m_browser.StatusMessage += OnBrowserStatusMessage;
      m_browser.TitleChanged += OnBrowserTitleChanged;
      m_browser.AddressChanged += OnBrowserAddressChanged;
      this.Disposed += new EventHandler(OnDisposed);
    }

    private void OnBrowserAddressChanged(object sender, AddressChangedEventArgs e)
    {
#if DEBUG
      Rhino.RhinoApp.WriteLine("\nSpeckle for Rhino: Browser Address Changed. \nBrowser: {0}, Address: {1}", e.Browser, e.Address);
#endif
    }

    private void OnBrowserTitleChanged(object sender, TitleChangedEventArgs e)
    {
#if DEBUG
      Rhino.RhinoApp.WriteLine("\nSpeckle for Rhino: Browser Title Changed. \nTitle: {0}", e.Title);
#endif
    }

    private void OnBrowserStatusMessage(object sender, StatusMessageEventArgs e)
    {
#if DEBUG
      Rhino.RhinoApp.WriteLine("\nSpeckle for Rhino: Browser Status Message Changed. \nBrowser: {0}, Value: {1}", e.Browser, e.Value);
#endif
    }

    private void OnBrowserConsoleMessage(object sender, ConsoleMessageEventArgs e)
    {
#if DEBUG
      Rhino.RhinoApp.WriteLine("\nSpeckle for Rhino: Browser Console Message Changed. \nLine: {0}, Message: {1}, Source: {2}", e.Line, e.Message, e.Source);
#endif
    }

    private void OnLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
    {
#if DEBUG
      Rhino.RhinoApp.WriteLine("\nSpeckle for Rhino: Loading State Changed. \nBrowser: {0}, Is Loading: {1}", e.Browser, e.IsLoading);
      if (!e.IsLoading)
        Rhino.RhinoApp.WriteLine("\nSpeckle for Rhino: Should probably connect to Speckle.");
#endif
    }

    private void InitializeBrowser()
    {

      if (!Cef.IsInitialized)
      {

        var settings = new CefSettings();

        // Increase the log severity so CEF outputs detailed information, useful for debugging
        settings.LogSeverity = LogSeverity.Verbose;
        // By default CEF uses an in memory cache, to save cached data e.g. passwords you need to specify a cache path
        // NOTE: The executing user must have sufficient privileges to write to this folder.
        settings.CachePath = "cache";
        settings.RemoteDebuggingPort = 7070;

        string assemblyLocation = Assembly.GetExecutingAssembly().Location;
        string assemblyPath = Path.GetDirectoryName(assemblyLocation);
        string pathSubprocess = Path.Combine(assemblyPath, "CefSharp.BrowserSubprocess.exe");

        settings.BrowserSubprocessPath = pathSubprocess;

        Cef.EnableHighDPISupport();
        Cef.Initialize(settings);

      }
      else
      {
        Rhino.RhinoApp.WriteLine("Speckle for Rhino: Warning.  CefSharp has already been initialized by another plugin.  This may cause issues.");
      }

      var path = Directory.GetParent(Assembly.GetExecutingAssembly().Location);

      string page = string.Format(@"{0}\app\index.html", path);

      if (!File.Exists(page))
        Rhino.RhinoApp.WriteLine("Speckle for Rhino: Error. The html file doesn't exists : {0}", page);

      m_browser = new ChromiumWebBrowser(@"http://10.211.55.2:8080/");
      //m_browser = new ChromiumWebBrowser("https://app.speckle.works/receiver/example/");
      toolStripContainer.ContentPanel.Controls.Add(m_browser);
      m_browser.Dock = DockStyle.Fill;

      // Allow the use of local resources in the browser
      BrowserSettings browserSettings = new BrowserSettings();
      browserSettings.FileAccessFromFileUrls = CefState.Enabled;
      browserSettings.UniversalAccessFromFileUrls = CefState.Enabled;
      m_browser.BrowserSettings = browserSettings;

      //m_browser.Enabled = true;
      //m_browser.Show();
      
    }

    /// <summary>
    /// Occurs when the component is disposed by a call to the
    /// System.ComponentModel.Component.Dispose() method.
    /// </summary>
    private void OnDisposed(object sender, EventArgs e)
    {
      m_browser.Dispose();
      Cef.Shutdown();
      SpeckleRhinoPlugIn.Instance.UserControl = null;
    }
  }
}

using System;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using System.IO;
using System.Reflection;

namespace SpeckleRhinoChromium
{
  [System.Runtime.InteropServices.Guid("B92C6D38-ED33-49EC-AFAA-BDF655A5C359")]
  public partial class SampleCsChromiumPanelControl : UserControl
  {
    private ChromiumWebBrowser m_browser;

    /// <summary>
    /// Returns the ID of this panel.
    /// </summary>
    public static Guid PanelId
    {
      get
      {
        return typeof(SampleCsChromiumPanelControl).GUID;
      }
    }

    public SampleCsChromiumPanelControl()
    {
      InitializeComponent();
      InitializeBrowser();
      m_browser.RegisterJsObject("cefCustomObject", new CefCustomObject(m_browser, this));
      SpeckleRhinoChromiumPlugIn.Instance.UserControl = this;
      this.Disposed += new EventHandler(OnDisposed);
    }

    private void InitializeBrowser()
    {

      var path = Directory.GetParent(Assembly.GetExecutingAssembly().Location);

      String page = string.Format(@"{0}\app\index.html", path);

      if (!File.Exists(page))
      {
        MessageBox.Show("Error The html file doesn't exists : " + page);
      }

      Cef.EnableHighDPISupport();
      Cef.Initialize(new CefSettings());
      m_browser = new ChromiumWebBrowser(page);
      Controls.Add(m_browser);
      m_browser.Dock = DockStyle.Fill;

      // Allow the use of local resources in the browser
      BrowserSettings browserSettings = new BrowserSettings();
      browserSettings.FileAccessFromFileUrls = CefState.Enabled;
      browserSettings.UniversalAccessFromFileUrls = CefState.Enabled;
      m_browser.BrowserSettings = browserSettings;

      m_browser.Enabled = true;
      m_browser.Show();
    }

    /// <summary>
    /// Occurs when the component is disposed by a call to the
    /// System.ComponentModel.Component.Dispose() method.
    /// </summary>
    private void OnDisposed(object sender, EventArgs e)
    {
      m_browser.Dispose();
      Cef.Shutdown();
      SpeckleRhinoChromiumPlugIn.Instance.UserControl = null;
    }
  }
}

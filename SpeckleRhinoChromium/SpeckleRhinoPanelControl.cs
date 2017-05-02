﻿using System;
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
      SpeckleRhinoPlugIn.Instance.UserControl = null;
    }
  }
}
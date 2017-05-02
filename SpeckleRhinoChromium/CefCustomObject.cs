using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SpeckleRhino
{
  class CefCustomObject
  {

    // Declare a local instance of chromium and the main form in order to execute things from here in the main thread
    private static ChromiumWebBrowser _instanceBrowser = null;
    // The form class needs to be changed according to yours
    private static SampleCsChromiumPanelControl _instanceMainForm = null;

    public CefCustomObject(ChromiumWebBrowser originalBrowser, SampleCsChromiumPanelControl mainForm)
    {
      _instanceBrowser = originalBrowser;
      _instanceMainForm = mainForm;
    }

    public void showDevTools()
    {
      _instanceBrowser.ShowDevTools();
    }

    public void opencmd()
    {
      ProcessStartInfo start = new ProcessStartInfo("cmd.exe", "/c pause");
      Process.Start(start);
    }

    public void openWeb(string url)
    {
      ProcessStartInfo start = new ProcessStartInfo(url);
      Process.Start(start);
    }
  }
}

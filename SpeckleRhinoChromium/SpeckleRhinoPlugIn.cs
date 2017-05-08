using CefSharp;

namespace SpeckleRhino
{
  ///<summary>
  /// <para>Every RhinoCommon .rhp assembly must have one and only one PlugIn-derived
  /// class. DO NOT create instances of this class yourself. It is the
  /// responsibility of Rhino to create an instance of this class.</para>
  /// <para>To complete plug-in information, please also see all PlugInDescription
  /// attributes in AssemblyInfo.cs (you might need to click "Project" ->
  /// "Show All Files" to see it in the "Solution Explorer" window).</para>
  ///</summary>
  public class SpeckleRhinoPlugIn : Rhino.PlugIns.PlugIn
  {
    public SpeckleRhinoPlugIn()
    {
      Instance = this;
    }

    ///<summary>Gets the only instance of the SampleCsChromiumPlugIn plug-in.</summary>
    public static SpeckleRhinoPlugIn Instance
    {
      get;
      private set;
    }

    // You can override methods here to change the plug-in behavior on
    // loading and shut down, add options pages to the Rhino _Option command
    // and mantain plug-in wide options in a document.

    //public override Rhino.PlugIns.PlugInLoadTime LoadTime => Rhino.PlugIns.PlugInLoadTime.AtStartup;

    /// <summary>
    /// Called when the plug-in is being loaded.
    /// </summary>
    protected override Rhino.PlugIns.LoadReturnCode OnLoad(ref string errorMessage)
    {

      // InitCEF();
      Rhino.RhinoApp.WriteLine("Cef Initialized: {0}", Cef.IsInitialized);

      var panel_type = typeof(SpeckleRhinoPanelControl);
      Rhino.UI.Panels.RegisterPanel(this, panel_type, "Speckle for Rhino", Properties.Resources.SpeckleLogo);

      Rhino.RhinoApp.WriteLine("Cef Initialized Post: {0}", Cef.IsInitialized);

      Rhino.RhinoApp.WriteLine("Controls: ");

      return Rhino.PlugIns.LoadReturnCode.Success;
    }

    public static void InitCEF()
    {

      if(Cef.IsInitialized)
        Cef.Shutdown();

      var settings = new CefSettings();

      // Increase the log severity so CEF outputs detailed information, useful for debugging
      settings.LogSeverity = LogSeverity.Verbose;
      // By default CEF uses an in memory cache, to save cached data e.g. passwords you need to specify a cache path
      // NOTE: The executing user must have sufficient privileges to write to this folder.
      settings.CachePath = "cache";
      settings.RemoteDebuggingPort = 7070;
      Cef.EnableHighDPISupport();

      if (!Cef.IsInitialized)
        Cef.Initialize(settings, true, true);

      Rhino.RhinoApp.WriteLine("Cef Initialized: {0}", Cef.IsInitialized);
    }

    /// <summary>
    /// Gets tabbed dockbar user control
    /// </summary>
    public SpeckleRhinoPanelControl UserControl
    {
      get;
      set;
    }
  }
}
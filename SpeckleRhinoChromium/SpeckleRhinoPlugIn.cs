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
            var panel_type = typeof(SpeckleRhinoPanelControl);
            Rhino.UI.Panels.RegisterPanel(this, panel_type, "Speckle for Rhino", Properties.Resources.SpeckleLogo);
            return Rhino.PlugIns.LoadReturnCode.Success;
        }

        /// <summary>
        /// Gets tabbed dockbar user control
        /// </summary>
        public SpeckleRhinoPanelControl UserControl
        {
            get;
            set;
        }
        public SpeckleRhinoViewModel ViewModel { get; set; }

    }
}
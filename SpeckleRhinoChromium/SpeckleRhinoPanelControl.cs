using System;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace SpeckleRhino
{
    [System.Runtime.InteropServices.Guid("041397A5-BB8B-4FCA-B580-1E6A7F0B2137")]
    public partial class SpeckleRhinoPanelControl : UserControl
    {

        private SpeckleRhinoViewModel m_viewModel;

        private WebBrowser m_webBrowser;

        private SpeckleRhinoPipeline m_pipeline;

        private bool m_indexLoaded = false;

        private string m_indexUrl;

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

            m_viewModel = new SpeckleRhinoViewModel();
            SpeckleRhinoPlugIn.Instance.ViewModel = m_viewModel;

            //m_browser.RegisterJsObject("speckleRhinoPipeline", new SpeckleRhinoPipeline(m_browser, this, m_viewModel));
            m_pipeline = new SpeckleRhinoPipeline(m_webBrowser, this, m_viewModel);

            SpeckleRhinoPlugIn.Instance.UserControl = this;

            this.Disposed += new EventHandler(OnDisposed);
        }

        private void InitializeBrowser()
        {
            m_webBrowser = new WebBrowser();

            m_webBrowser.DocumentCompleted += OnDocumentLoaded;
            m_webBrowser.Navigating += OnDocumentLoading;

            //do we need to listen fr any other events?

#if Dimitrie
            m_indexUrl = @"http://10.211.55.2:9090/";
            m_webBrowser.Url = new Uri(m_indexUrl);
#elif Luis
            m_indexUrl = @"http://localhost:9090/";
            m_webBrowser.Url = new Uri(m_indexUrl);
#else 
            var path = Directory.GetParent(Assembly.GetExecutingAssembly().Location);

            var indexPath = string.Format(@"{0}\app\index.html", path);

            if (!File.Exists(indexPath))
                Rhino.RhinoApp.WriteLine("Speckle for Rhino: Error. The html file doesn't exists : {0}", indexPath);

            m_indexUrl = indexPath.Replace("\\", "/");

            m_webBrowser.Url = new Uri(m_indexUrl);
#endif 
            toolStripContainer.ContentPanel.Controls.Add(m_webBrowser);
            m_webBrowser.Dock = DockStyle.Fill;

        }

        private void OnDocumentLoading(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (!m_indexLoaded) return;

            e.Cancel = true;

            m_pipeline.ParseUri(e.Url);

        }

        private void OnDocumentLoaded(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.OriginalString == m_indexUrl) m_indexLoaded = true;
        }

        /// <summary>
        /// Occurs when the component is disposed by a call to the
        /// System.ComponentModel.Component.Dispose() method.
        /// </summary>
        private void OnDisposed(object sender, EventArgs e)
        {
            m_webBrowser.Dispose();

            SpeckleRhinoPlugIn.Instance.UserControl = null;
        }
    }
}

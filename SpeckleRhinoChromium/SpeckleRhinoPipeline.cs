using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Rhino;
using SpeckleGhRhConverter;
using System.Windows.Forms;

namespace SpeckleRhino
{
    class SpeckleRhinoPipeline
    {

        // Declare a local instance of chromium and the main form in order to execute things from here in the main thread
        private static WebBrowser _instanceBrowser = null;
        // The form class needs to be changed according to yours
        private static SpeckleRhinoPanelControl _instanceMainForm = null;
        private static SpeckleRhinoViewModel _viewModel = null;

        public List<SpeckleAccount> accounts = new List<SpeckleAccount>();

        public SpeckleRhinoPipeline(WebBrowser originalBrowser, SpeckleRhinoPanelControl mainForm, SpeckleRhinoViewModel viewModel)
        {
            _instanceBrowser = originalBrowser;
            _instanceMainForm = mainForm;
            _viewModel = viewModel;

            string strPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            strPath = strPath + @"\SpeckleSettings";

            if (Directory.Exists(strPath) && Directory.EnumerateFiles(strPath, "*.txt").Count() > 0)
                foreach (string file in Directory.EnumerateFiles(strPath, "*.txt"))
                {
                    string content = File.ReadAllText(file);
                    string[] pieces = content.TrimEnd('\r', '\n').Split(',');

                    accounts.Add(new SpeckleAccount() { email = pieces[0], apiToken = pieces[1], serverName = pieces[2], restApi = pieces[3], rootUrl = pieces[4] });
                }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        public void ParseUri(Uri uri)
        {
            Debug.WriteLine("Uri: " + uri.ToString(), "SpeckleRhino");
            Debug.WriteLine("Action: " + uri.Host, "SpeckleRhino");

            var decodedParams = Convert.FromBase64String(uri.AbsolutePath.Substring(1, uri.AbsolutePath.Length - 1));
            string decodedString = Encoding.UTF8.GetString(decodedParams);
            string[] incomingParameters = decodedString.Split('/');

            string incomingAction = uri.Host;

            switch (incomingAction)
            {
                case "togglelayer":
                    var dataToggleLayer = new SpeckleLayerData() { StreamId = incomingParameters[0], Id = Guid.Parse(incomingParameters[1]), Visible = bool.Parse(incomingParameters[2]) };
                    layerVisibilityUpdate(dataToggleLayer);
                    break;

                case "layercolorupdate":
                    var dataLayerColorUpdate = new SpeckleLayerData() { StreamId = incomingParameters[0], Id = Guid.Parse(incomingParameters[1]), Color = new SpeckleColor() { Hex = incomingParameters[2] }, Opacity = float.Parse(incomingParameters[3]) };
                    layerColorUpdate(dataLayerColorUpdate);
                    break;

                case "liveupdate":
                    liveUpdate(incomingParameters[0]);
                    break;

                case "metadataupdate":
                    metadataUpdate(incomingParameters[0]);
                    break;

                case "receiverready":
                    receiverReady(incomingParameters);
                    break;

                default:
                    Debug.WriteLine("Action not implemented", "SpeckleRhino");
                    break;
            }

        }

        public string getAccounts()
        {
            return JsonConvert.SerializeObject(accounts);
        }

        public void saveNewAccount(string email, string apiToken, string serverName, string restApi, string rootUrl)
        {
            Debug.WriteLine("Hello world", "Speckle Rhino");
        }

        public void showDevTools()
        {
            //_instanceBrowser.ShowDevTools();
            //in case we want firebug lite: https://stackoverflow.com/questions/12693444/embedded-webbrowser-web-console-access
        }

        public void openWeb(string url)
        {
            //ProcessStartInfo start = new ProcessStartInfo(url);
            //Process.Start(start);
            
        }

        public void receiverReady(string[] args)
        {
            if (!_viewModel.Model.Receivers.Any(R => R.StreamId == args[0]))
                _viewModel.Model.Receivers.Add(new SpeckleRhinoReceiverWorker(args[0], args[3], args[1], args[2]));
        }

        public void liveUpdate(string streamId)
        {
            if (_viewModel.Model.Receivers.Any(R => R.StreamId == streamId))
            {
                _viewModel.Model.Receivers.First(R => R.StreamId == streamId).GetLiveUpdate();
            }
            else
            {
                _viewModel.Model.Receivers.Add(new SpeckleRhinoReceiverWorker(streamId));
                _viewModel.Model.Receivers.First(R => R.StreamId == streamId).GetLiveUpdate();
            }
        }

        public void metadataUpdate(string streamId, string name, string serialisedLayerList)
        {
            // todo: update layers (that's the only thing we're interested in in this update
        }

        private void metadataUpdate(string streamId)
        {
            if (_viewModel.Model.Receivers.Any(R => R.StreamId == streamId))
            {
                _viewModel.Model.Receivers.First(R => R.StreamId == streamId).GetMetadataUpdate();
            }
            else
            {
                _viewModel.Model.Receivers.Add(new SpeckleRhinoReceiverWorker(streamId));
                _viewModel.Model.Receivers.First(R => R.StreamId == streamId).GetMetadataUpdate();
            }
        }

        public void streamVisibilityUpdate(string streamId)
        {
            // toggles a whole layer off
        }

        public void layerVisibilityUpdate(SpeckleLayerData data)
        {

            if (_viewModel.Model.Receivers.Any(R => R.StreamId == data.StreamId))
            {
                _viewModel.Model.Receivers.First(R => R.StreamId == data.StreamId).LayerVisibilityUpdate(data);
            }
            else
            {
                Debug.WriteLine("Stream not found", "SpeckleRhino");
            }
        }

        public void layerColorUpdate(SpeckleLayerData data)
        {
            if (_viewModel.Model.Receivers.Any(R => R.StreamId == data.StreamId))
            {
                _viewModel.Model.Receivers.First(R => R.StreamId == data.StreamId).LayerColorUpdate(data);
            }
            else
            {
                Debug.WriteLine("Stream not found", "SpeckleRhino");
            }
        }
    }

    [Serializable]
    public class SpeckleAccount
    {
        public string email { get; set; }
        public string apiToken { get; set; }
        public string serverName { get; set; }
        public string restApi { get; set; }
        public string rootUrl { get; set; }
    }
}

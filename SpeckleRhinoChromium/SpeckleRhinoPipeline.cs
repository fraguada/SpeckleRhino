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

        public string getAccounts()
        {
            return JsonConvert.SerializeObject(accounts);
        }

        public void saveNewAccount(string email, string apiToken, string serverName, string restApi, string rootUrl)
        {
            Debug.WriteLine("Hello world");
        }

        public void showDevTools()
        {
            //_instanceBrowser.ShowDevTools();
        }

        public void openWeb(string url)
        {
            ProcessStartInfo start = new ProcessStartInfo(url);
            Process.Start(start);
        }

        public void liveUpdate(string streamId, string name, string serialisedObjectList, string serialisedPropertiesList, string serialsedLayersList, string serialisedLayerMaterialsList)
        {
            if(_viewModel.Model.Receivers.Any(R => R.StreamId == streamId))
            {
                _viewModel.Model.Receivers.First(R => R.StreamId == streamId).Update(serialisedObjectList, serialisedPropertiesList, serialsedLayersList, serialisedLayerMaterialsList);
            } else
            {
                _viewModel.Model.Receivers.Add(new SpeckleRhinoReceiverWorker(streamId, name, serialisedObjectList, serialisedPropertiesList, serialsedLayersList, serialisedLayerMaterialsList));
            }
        }

        public void ParseUri(Uri uri)
        {
            Debug.WriteLine("Uri: " + uri.ToString(), "SpeckleRhino");
            Debug.WriteLine("StreamId: " + uri.Scheme, "SpeckleRhino");
            Debug.WriteLine("Action: " + uri.Host, "SpeckleRhino");
            Debug.WriteLine("Parameters: " + uri.AbsolutePath, "SpeckleRhino");

            string incomingStringId = uri.Scheme;
            string incomingAction = uri.Host;
            string[] incomingParameters = uri.AbsolutePath.Substring(1, uri.AbsolutePath.Length-1).Split('/');

            switch (incomingAction)
            {
                case "togglelayer":
                    var data = new SpeckleLayerData() { StreamId = incomingStringId, Id = Guid.Parse(incomingParameters[0]), Visible = bool.Parse(incomingParameters[1]) };
                    layerVisibilityUpdate(data);
                    break;
            }
            
        }

        public void metadataUpdate(string streamId, string name, string serialisedLayerList)
        {
            // todo: update layers (that's the only thing we're interested in in this update
        }

        public void streamVisibilityUpdate(string streamId)
        {
            // toggles a whole layer off
        }

        public void layerVisibilityUpdate(string data)
        {
            var deserializedData = JsonConvert.DeserializeObject<SpeckleLayerData>(data);

            if (_viewModel.Model.Receivers.Any(R => R.StreamId == deserializedData.StreamId))
            {
                _viewModel.Model.Receivers.First(R => R.StreamId == deserializedData.StreamId).LayerVisibilityUpdate(deserializedData);
            }
            else
            {
                Debug.WriteLine("Stream not found", "SpeckleRhino");
            }
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


        public void layerColorUpdate(string data)
        {
            var deserializedData = JsonConvert.DeserializeObject<SpeckleLayerData>(data);
            //data = { streamId, layerGuid, color, opacity }

            if (_viewModel.Model.Receivers.Any(R => R.StreamId == deserializedData.StreamId))
            {
                _viewModel.Model.Receivers.First(R => R.StreamId == deserializedData.StreamId).LayerColorUpdate(deserializedData);
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

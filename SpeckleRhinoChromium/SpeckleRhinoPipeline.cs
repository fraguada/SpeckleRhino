﻿using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Rhino;
using SpeckleGhRhConverter;


namespace SpeckleRhino
{
    class SpeckleRhinoPipeline
    {

        // Declare a local instance of chromium and the main form in order to execute things from here in the main thread
        private static ChromiumWebBrowser _instanceBrowser = null;
        // The form class needs to be changed according to yours
        private static SpeckleRhinoPanelControl _instanceMainForm = null;
        private static SpeckleRhinoViewModel _viewModel = null;

        public List<SpeckleAccount> accounts = new List<SpeckleAccount>();

        public SpeckleRhinoPipeline(ChromiumWebBrowser originalBrowser, SpeckleRhinoPanelControl mainForm, SpeckleRhinoViewModel viewModel)
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

        /// <summary>
        /// Pushes updates to the Rhino Document.
        /// </summary>
        /// <param name="streamId"></param>
        /// <param name="name"></param>
        /// <param name="serialisedObjectList"></param>
        /// <param name="serialisedPropertiesList"></param>
        /// <param name="serialsedLayersList"></param>
        /// <param name="serialisedLayerMaterialsList"></param>
        public void liveUpdate(string streamId, string name, string serialisedObjectList, string serialisedPropertiesList, string serialsedLayersList, string serialisedLayerMaterialsList)
        {
            Debug.WriteLine(streamId);

            if(_viewModel.Model.Receivers.Any(R => R.Id == streamId))
            {
                _viewModel.Model.Receivers.First(R => R.Id == streamId).Update(serialisedObjectList, serialisedPropertiesList, serialsedLayersList, serialisedLayerMaterialsList);
            } else
            {
                _viewModel.Model.Receivers.Add(new SpeckleRhinoReceiverWorker(streamId, name, serialisedObjectList, serialisedPropertiesList, serialsedLayersList, serialisedLayerMaterialsList));
            }
        }

        public void layerVisibilityUpdate(string layerData)
        {
            var deserializedLayerData = JsonConvert.DeserializeObject<SpeckleLayerData>(layerData);

            if (_viewModel.Model.Receivers.Any(R => R.Id == deserializedLayerData.StreamId))
            {
                _viewModel.Model.Receivers.First(R => R.Id == deserializedLayerData.StreamId).LayerVisibilityUpdate(deserializedLayerData);
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
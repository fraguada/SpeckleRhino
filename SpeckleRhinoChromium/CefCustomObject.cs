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
    class CefCustomObject
    {

        // Declare a local instance of chromium and the main form in order to execute things from here in the main thread
        private static ChromiumWebBrowser _instanceBrowser = null;
        // The form class needs to be changed according to yours
        private static SpeckleRhinoPanelControl _instanceMainForm = null;

        public List<SpeckleAccount> accounts = new List<SpeckleAccount>();


        public CefCustomObject(ChromiumWebBrowser originalBrowser, SpeckleRhinoPanelControl mainForm)
        {
            _instanceBrowser = originalBrowser;
            _instanceMainForm = mainForm;

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

        public void addObjects(string serialisedObjectList)
        {
            Rhino.DocObjects.Tables.ObjectTable ot = Rhino.RhinoDoc.ActiveDoc.Objects;
 
            GhRhConveter c = new GhRhConveter();
            var objectList = JsonConvert.DeserializeObject<List<dynamic>>(serialisedObjectList);
            var copy = objectList;

            foreach(var obj in objectList)
            {
                string type = (string)obj.type;

                if (type == "Mesh")
                    ot.AddMesh(c.encodeObject(obj));

                if (type == "Polyline")
                    ot.AddPolyline(c.encodeObject(obj));
            }

            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();

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

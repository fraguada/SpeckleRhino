using Newtonsoft.Json;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using SpeckleCommon;
using SpeckleGhRhConverter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SpeckleRhino
{
    /// <summary>
    /// SpeckleRhinoReveiverWorker
    /// This is a class which acts on behalf of the js receiver.  
    /// It does the work of doing things to Rhino such as adding geometry to the document,
    /// keeping track of geometry, etc.
    /// </summary>
    public class SpeckleRhinoReceiverWorker : SpeckleRhinoWorker, IEquatable<SpeckleRhinoReceiverWorker>, INotifyPropertyChanged, IDisposable
    {

        #region Members

        /// <summary>
        /// The Display Conduit for showing stream objects and their status.
        /// </summary>
        private SpeckleRhinoDisplayConduit Display;

        /// <summary>
        /// The string of all serialzed objects.
        /// </summary>
        public string SerializedObjects { get; private set; }

        /// <summary>
        /// The string of all serialzed object properties.
        /// </summary>
        public string SerializedProperties { get; private set; }

        /// <summary>
        /// A collection of geometry to add to the document and send to the Display Conduit.
        /// </summary>
        public List<GeometryBase> Geometry { get; private set; }

        /// <summary>
        /// A collection of guid of objects added to the Rhino Document
        /// </summary>
        public List<Guid> Ids { get; private set; }

        /// <summary>
        /// The layer index of the parent layer.
        /// </summary>
        public int ParentLayerId { get; private set; }

        /// <summary>
        /// A collection of layer indices for the sublayers.
        /// </summary>
        public List<int> LayerIds { get; private set; }

        public List<SpeckleLayer> SpeckleLayers { get; private set; }

        public List<SpeckleLayerMaterial> SpeckleLayerMaterials { get; private set; }

        public List<dynamic> SpeckleObjects { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Color> LayerColors { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<bool> VisibleList { get; private set; }

        public SpeckleReceiver Receiver { get; private set; }

        public string ApiUrl { get; private set; }

        public string Token { get; private set; }

        public SpeckleConverter Converter { get; private set; }

        #endregion Members

        #region Constructors

        /// <summary>
        /// Base ctor.
        /// </summary>
        public SpeckleRhinoReceiverWorker()
        {
            Type = "Receiver";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="streamId"></param>
        public SpeckleRhinoReceiverWorker(string streamId)
        {
            StreamId = streamId;
        }

        public SpeckleRhinoReceiverWorker(string streamId, string name) : this(streamId)
        {
            Name = name;
        }

        public SpeckleRhinoReceiverWorker(string streamId, string name, string apiUrl, string token) : this(streamId, name)
        {
            ApiUrl = apiUrl;
            Token = token;
            Converter = new GhRhConveter();
            Geometry = new List<GeometryBase>();
            Ids = new List<Guid>();
            SpeckleLayers = new List<SpeckleCommon.SpeckleLayer>();
            SpeckleObjects = new List<dynamic>();

            Receiver = new SpeckleReceiver("https://" + ApiUrl + "/api/v1", Token, StreamId, Converter);

            registermyReceiverEvents();
        }

        #endregion Constructors

        #region Methods

        public bool Equals(SpeckleRhinoReceiverWorker other)
        {
            throw new NotImplementedException();
        }

        
        /// <summary>
        /// 
        /// </summary>
        public void ProcessLayers()
        {
            LayerIds = new List<int>();
            LayerColors = new List<Color>();
            VisibleList = new List<bool>();

            ParentLayerId = RhinoDoc.ActiveDoc.Layers.FindByFullPath(this.Name + "_[" + this.StreamId + "]", true);

            if (ParentLayerId == -1)
            {
                var layer = new Layer() { Name = this.Name + "_[" + this.StreamId + "]" };
                ParentLayerId = RhinoDoc.ActiveDoc.Layers.Add(layer);
            }
            else
            {
                //delete existing sublayers and their objects
                foreach (var layer in RhinoDoc.ActiveDoc.Layers[ParentLayerId].GetChildren())
                    RhinoDoc.ActiveDoc.Layers.Delete(layer.LayerIndex, true);
            }

            foreach (var speckleLayer in SpeckleLayers)
            {
                var layerId = RhinoDoc.ActiveDoc.Layers.FindByFullPath(this.Name + "_[" + this.StreamId + "]" + "::" + speckleLayer.name, true);
                //var layerMaterial = layerMaterialsList.Find(lm => lm.Id == speckleLayer.Id);
                if (layerId == -1)
                {
                    var layer = new Layer()
                    {
                        Name = speckleLayer.name,
                        Id = Guid.Parse(speckleLayer.guid),
                        ParentLayerId = RhinoDoc.ActiveDoc.Layers[ParentLayerId].Id,
                        //Color = layerMaterial.Color.ToColor()
                        Color = Color.Black,
                        //IsVisible = layerMaterial.Visible
                        IsVisible = true
                    };
                    layerId = RhinoDoc.ActiveDoc.Layers.Add(layer);
                }

                for (int i = speckleLayer.startIndex; i < speckleLayer.startIndex + speckleLayer.objectCount; i++)
                {
                    LayerIds.Add(layerId);
                    //LayerColors.Add(layerMaterial.Color.ToColor());
                    LayerColors.Add(Color.Black);
                    //VisibleList.Add(layerMaterial.Visible);
                    VisibleList.Add(true);
                }
            }


        }

        /// <summary>
        /// 
        /// </summary>
        public void ProcessObjects()
        {
            GhRhConveter converter = new GhRhConveter();

            Geometry.Clear();
            if (Ids != null)
            {
                RhinoDoc.ActiveDoc.Objects.Delete(Ids, true);
                Ids.Clear();
            }

            foreach (var obj in SpeckleObjects)
            {
                var type = obj.GetType();

                switch (type.Name)
                {
                    case "Mesh":
                    case "Brep":
                    case "Curve":
                    case "NurbsCurve":

                        //Geometry.Add(converter.encodeObject(obj));
                        Geometry.Add(obj);

                        break;

                    case "Point":
                    case "Point3d":

                        //Geometry.Add(new Rhino.Geometry.Point(converter.encodeObject(obj)));
                        Geometry.Add(new Rhino.Geometry.Point(obj));

                        break;

                    case "Polyline":

                        //Polyline polyline = converter.encodeObject(obj);
                        //Geometry.Add(polyline.ToNurbsCurve());
                        Geometry.Add(obj.ToNurbsCurve());

                        break;
                    case "Circle":

                        //Circle circle = converter.encodeObject(obj);
                        //Geometry.Add(circle.ToNurbsCurve());
                        Geometry.Add(obj.ToNurbsCurve());

                        break;

                    case "Rectangle":

                        //Rectangle3d rectangle = converter.encodeObject(obj);
                        //Geometry.Add(rectangle.ToNurbsCurve());
                        Geometry.Add(obj.ToNurbsCurve());

                        break;

                    case "Line":

                        //Line line = converter.encodeObject(obj);
                        //Geometry.Add(line.ToNurbsCurve());
                        Geometry.Add(obj.ToNurbsCurve());

                        break;

                    case "Box":

                        //Box box = converter.encodeObject(obj);
                        //Geometry.Add(box.ToBrep());
                        Geometry.Add(obj.ToBrep());

                        break;

                    default:

                        Debug.WriteLine("Type not supported: " + (string)type.Name, "SpeckleRhino");

                        break;
                }


            }

            for (int i = 0; i < Geometry.Count; i++)
                Ids.Add(RhinoDoc.ActiveDoc.Objects.Add(Geometry[i], new ObjectAttributes() { LayerIndex = LayerIds[i] }));

            DisplayContents();
        }

        /// <summary>
        /// 
        /// </summary>
        public void GetMetadataUpdate()
        {
            Debug.WriteLine("TODO: GetMetadataUpdate!", "SpeckleRhino");
        }

        /// <summary>
        /// 
        /// </summary>
        public void GetLiveUpdate()
        {
            Debug.WriteLine("TODO: GetLiveUpdate!", "SpeckleRhino");
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deserializedLayerData"></param>
        public void LayerVisibilityUpdate(SpeckleLayerData deserializedLayerData)
        {

            //Debug.WriteLine("SpeckleRhino: received LayerVis update.  Vis: {0}", deserializedLayerData.Visible);
            if (SpeckleLayers == null || SpeckleLayers.Count == 0) return;

            //SpeckleLayer updatedLayer = SpeckleRhinoLayers.Find(sl => sl.Id == deserializedLayerData.Id);
            SpeckleCommon.SpeckleLayer updatedLayer = SpeckleLayers.Find(sl => Guid.Parse(sl.guid) == deserializedLayerData.Id);

            for (int i = updatedLayer.startIndex; i < updatedLayer.startIndex + updatedLayer.objectCount; i++)
            {
                VisibleList[i] = deserializedLayerData.Visible;
            }

            DisplayContents();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deserializedLayerData"></param>
        public void LayerColorUpdate(SpeckleLayerData deserializedLayerData)
        {

            if (SpeckleLayers == null || SpeckleLayers.Count == 0) return;

            var updatedLayer = SpeckleLayers.Find(sl => Guid.Parse(sl.guid) == deserializedLayerData.Id);

            var layerIndex = Rhino.RhinoDoc.ActiveDoc.Layers.Find(updatedLayer.guid, true);

            var layer = Rhino.RhinoDoc.ActiveDoc.Layers[layerIndex];
            layer.Color = deserializedLayerData.Color.ToColor();
            layer.CommitChanges();

            for (int i = updatedLayer.startIndex; i < updatedLayer.startIndex + updatedLayer.objectCount; i++)
            {
                LayerColors[i] = deserializedLayerData.Color.ToColor();

            }

            DisplayContents();
        }

        /// <summary>
        /// Display the contents of the stream in a display conduit.
        /// </summary>
        public void DisplayContents()
        {
            //Debug.WriteLine("Geometry: {0}, Colors: {1}, Visible: {2}",Geometry.Count, LayerColors.Count, VisibleList.Count);

            if (Display != null)
            {

                Display.Geometry = Geometry;
                //Display.Colors = LayerColors;
                Display.VisibleList = VisibleList;

            }
            else
            {
                Display = new SpeckleRhinoDisplayConduit(Geometry, LayerColors, VisibleList) { Enabled = true };
            }

            RhinoDoc.ActiveDoc.Views.Redraw();
        }

        /// <summary>
        /// Dispose to ensure Display Conduit is disabled.
        /// </summary>
        public void Dispose()
        {
            if (Display != null)
                Display.Enabled = false;
        }

        private async Task<dynamic> RequestLayerMaterials()
        {

            string url = "https://server.speckle.works/api/v1/streams/" + StreamId;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));

            request.Method = "GET";

            using (WebResponse response = await request.GetResponseAsync())
            {

                StreamReader reader = new StreamReader(response.GetResponseStream());
                return reader.ReadToEnd();

            }
        }

        async void GetLayerMaterials()
        {
            SpeckleLayerMaterials = new List<SpeckleLayerMaterial>();
            //dynamic mats = await RequestLayerMaterials();
            dynamic data = JsonConvert.DeserializeObject<dynamic>(await RequestLayerMaterials());

           // SpeckleLayerMaterials = new List<SpeckleLayerMaterial>(JsonConvert.DeserializeObject<dynamic>(mats).data.layerMaterials);
            //RequestLayerMaterials().Wait();
            
        }

        #endregion Methods

        #region Events / Event Handlers

        public event PropertyChangedEventHandler PropertyChanged;

        void registermyReceiverEvents()
        {
            if (Receiver == null) return;

            Receiver.OnDataMessage += OnDataMessage;

            Receiver.OnError += OnError;

            Receiver.OnReady += OnReady;

            Receiver.OnMetadata += OnMetadata;

            Receiver.OnData += OnData;

            Receiver.OnHistory += OnHistory;

            Receiver.OnMessage += OnVolatileMessage;

            Receiver.OnBroadcast += OnBroadcast;
        }

        private void OnBroadcast(object source, SpeckleEventArgs e)
        {
            Debug.WriteLine("Receiver: OnBroadcast", "SpeckleRhino");
        }

        private void OnVolatileMessage(object source, SpeckleEventArgs e)
        {
            Debug.WriteLine("Receiver: OnVolatileMessage", "SpeckleRhino");
        }

        private void OnHistory(object source, SpeckleEventArgs e)
        {
            Debug.WriteLine("Receiver: OnHistory", "SpeckleRhino");
        }

        private void OnData(object source, SpeckleEventArgs e)
        {
            Debug.WriteLine("Receiver: OnData", "SpeckleRhino");
        }

        private void OnMetadata(object source, SpeckleEventArgs e)
        {
            Debug.WriteLine("Receiver: OnMetadataMessage", "SpeckleRhino");
        }

        private void OnReady(object source, SpeckleEventArgs e)
        {
            Debug.WriteLine("Receiver: OnReady", "SpeckleRhino");

            SpeckleLayers = e.Data.layers;
            SpeckleObjects = e.Data.objects;

           // GetLayerMaterials();

            ProcessLayers();
            ProcessObjects();
        }

        private void OnError(object source, SpeckleEventArgs e)
        {
            Debug.WriteLine("Receiver: OnError " + e.EventInfo, "SpeckleRhino");
        }

        private void OnDataMessage(object source, SpeckleEventArgs e)
        {
            Debug.WriteLine("Receiver: OnDataMessage", "SpeckleRhino");
        }

        #endregion Events / Event Handlers

    }

    /// <summary>
    /// Collection class for anything related to handling comparison of SpeckleRhinoReceiverWorkers
    /// </summary>
    public class SpeckleRhinoReceiverWorkerCollection : Collection<SpeckleRhinoReceiverWorker> { }
}

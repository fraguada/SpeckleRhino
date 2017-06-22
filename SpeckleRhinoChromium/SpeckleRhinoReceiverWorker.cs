using Newtonsoft.Json;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using SpeckleGhRhConverter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;


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
        /// The Id for the receiver.  This can be the stream id, or something else identifyable that
        /// connects this to the js receiver.
        /// </summary>
        public string Id { get; private set; }

        public string Name { get; private set; }

        private SpeckleRhinoDisplayConduit Display;
        public string SerializedObjects { get; private set; }
        public string SerializedProperties { get; private set; }
        public List<GeometryBase> Geometry { get; private set; }
        public List<Guid> Ids { get; private set; }

        //Will this class hold a collection of uuids to track geometry in the document?

        //Will this class do the conduit work as well?

        //Need to develop the events/handlers to communicate with Cef

        #endregion Members

        #region Constructors

        public SpeckleRhinoReceiverWorker()
        {
            Type = "Receiver";
        }

        public SpeckleRhinoReceiverWorker(string id) : this()
        {
            Id = id;
        }

        public SpeckleRhinoReceiverWorker(string id, string name, string serializedObjectList, string serializedPropertiesList, string serializedLayerList, string serializedLayerMaterialsList) : this()
        {
            Id = id;
            Name = name;
            Geometry = new List<GeometryBase>();
            Ids = new List<Guid>();
            CreateLayers(serializedLayerList, serializedLayerMaterialsList);
            Update(serializedObjectList, serializedPropertiesList, serializedLayerList, serializedLayerMaterialsList);
        }

        #endregion Constructors

        #region Methods

        public bool Equals(SpeckleRhinoReceiverWorker other)
        {
            throw new NotImplementedException();
        }

        public void CreateLayers (string serializedLayers, string serializedLayerMaterials)
        {
            var layersList = JsonConvert.DeserializeObject<List<SpeckleLayer>>(serializedLayers);
            var layerMaterialsList = JsonConvert.DeserializeObject<List<SpeckleLayerMaterial>>(serializedLayerMaterials);

            var parentLayerId = RhinoDoc.ActiveDoc.Layers.FindByFullPath(this.Name, true);
            if (parentLayerId == -1)
            {
                var layer = new Layer() { Name = this.Name };
                parentLayerId = RhinoDoc.ActiveDoc.Layers.Add(layer);
            }

            foreach(var speckleLayer in layersList)
            {
                var layerId = RhinoDoc.ActiveDoc.Layers.FindByFullPath(this.Name + "::" + speckleLayer.Name, true);
                if (layerId == -1)
                {
                    var layer = new Layer()
                    {   Name = speckleLayer.Name,
                        Id = speckleLayer.Id,
                        ParentLayerId = RhinoDoc.ActiveDoc.Layers[parentLayerId].Id                  
                     };
                    RhinoDoc.ActiveDoc.Layers.Add(layer);
                }
            }

        }

        public void Update(string serializedObjectList, string serializedPropertiesList, string serializedLayersList, string serializedLayerMaterialsList)
        {
            Debug.WriteLine("Should be Updating Objects");
            if (SerializedObjects != serializedObjectList)
            {
                SerializedObjects = serializedObjectList;
                
                var objectList = JsonConvert.DeserializeObject<List<dynamic>>(SerializedObjects);
                var propertiesList = JsonConvert.DeserializeObject<List<dynamic>>(serializedPropertiesList);
                var layersList = JsonConvert.DeserializeObject<List<SpeckleLayer>>(serializedLayersList);
                var layerMaterialsList = JsonConvert.DeserializeObject<List<dynamic>>(serializedLayerMaterialsList);

                GhRhConveter converter = new GhRhConveter();

                Geometry.Clear();
                if (Ids != null)
                {
                    RhinoDoc.ActiveDoc.Objects.Delete(Ids, true);
                    Ids.Clear();
                }

                foreach (var obj in objectList)
                {
                    string type = (string)obj.type;
                    
                    switch (type)
                    {
                        case "Mesh":
                        case "Brep":
                        case "Curve":

                            Geometry.Add(converter.encodeObject(obj));
                            
                            break;

                        case "Point":

                            Geometry.Add(new Rhino.Geometry.Point(converter.encodeObject(obj)));
                            
                            break;

                        case "Polyline":

                            Polyline polyline = converter.encodeObject(obj);
                            Geometry.Add(polyline.ToNurbsCurve());
                            
                            break;
                        case "Circle":

                            Circle circle = converter.encodeObject(obj);
                            Geometry.Add(circle.ToNurbsCurve());
                            
                            break;

                        case "Rectangle":

                            Rectangle3d rectangle = converter.encodeObject(obj);
                            Geometry.Add(rectangle.ToNurbsCurve());
                            
                            break;

                        case "Line":

                            Line line = converter.encodeObject(obj);
                            Geometry.Add(line.ToNurbsCurve());
                            
                            break;

                        case "Box":

                            Box box = converter.encodeObject(obj);
                            Geometry.Add(box.ToBrep());
                            
                            break;

                        default:

                            RhinoApp.WriteLine("{0}", obj);

                            break;
                    }


                }

                //ObjectAttributes oa = new ObjectAttributes() {  };

                foreach (var geo in Geometry)
                    Ids.Add(RhinoDoc.ActiveDoc.Objects.Add(geo));

                DisplayContents();

            }
           
        }

        public void DisplayContents()
        {
            if (Display != null)
            {

                Display.Geometry = Geometry;

            }
            else
            {
                Display = new SpeckleRhinoDisplayConduit(Geometry) { Enabled = true };
            }

            RhinoDoc.ActiveDoc.Views.Redraw();
        }

        public void Dispose()
        {
            if (Display != null)
                Display.Enabled = false;
        }

        #endregion Methods

        #region Events / Event Handlers

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events / Event Handlers

    }

    /// <summary>
    /// Collection class for anything related to handling comparison of SpeckleRhinoReceiverWorkers
    /// </summary>
    public class SpeckleRhinoReceiverWorkerCollection : Collection<SpeckleRhinoReceiverWorker> { }
}

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
using System.Drawing;

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
        /// The streamId for the receiver.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// The stream Name.
        /// </summary>
        public string Name { get; private set; }

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

        public List<Color> LayerColors { get; set; }

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
        /// ctor for the Receiver Worker.
        /// </summary>
        /// <param name="id">The streamId.</param>
        /// <param name="name">The name of the stream.</param>
        /// <param name="serializedObjectList">The objects coming from the stream.</param>
        /// <param name="serializedPropertiesList">The object properties coming from the stream.</param>
        /// <param name="serializedLayersList">The layer data coming from the stream.</param>
        /// <param name="serializedLayerMaterialsList">The layer material data coming from the stream.</param>
        public SpeckleRhinoReceiverWorker(string id, string name, string serializedObjectList, string serializedPropertiesList, string serializedLayersList, string serializedLayerMaterialsList) : this()
        {
            Id = id;
            Name = name;
            Geometry = new List<GeometryBase>();
            Ids = new List<Guid>();
            Update(serializedObjectList, serializedPropertiesList, serializedLayersList, serializedLayerMaterialsList);
        }

        #endregion Constructors

        #region Methods

        public bool Equals(SpeckleRhinoReceiverWorker other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create the layers in the Rhino Document associated with this stream.
        /// </summary>
        /// <param name="serializedLayers">The layer data coming from the stream.</param>
        /// <param name="serializedLayerMaterials">The layer material data coming from the stream.</param>
        public void CreateLayers (string serializedLayers, string serializedLayerMaterials)
        {
            LayerIds = new List<int>();
            LayerColors = new List<Color>();
            var layersList = JsonConvert.DeserializeObject<List<SpeckleLayer>>(serializedLayers);
            var layerMaterialsList = JsonConvert.DeserializeObject<List<SpeckleLayerMaterial>>(serializedLayerMaterials);

            ParentLayerId = RhinoDoc.ActiveDoc.Layers.FindByFullPath(this.Name + "_[" + Id + "]", true);

            if (ParentLayerId == -1)
            {
                var layer = new Layer() { Name = this.Name + "_[" + Id + "]" };
                ParentLayerId = RhinoDoc.ActiveDoc.Layers.Add(layer);
            }
            else
            {
                //delete existing sublayers and their objects
                foreach (var layer in RhinoDoc.ActiveDoc.Layers[ParentLayerId].GetChildren())
                    RhinoDoc.ActiveDoc.Layers.Delete(layer.LayerIndex, true);
            }

            foreach(var speckleLayer in layersList)
            {
                var layerId = RhinoDoc.ActiveDoc.Layers.FindByFullPath(this.Name + "_[" + Id + "]" + "::" + speckleLayer.Name, true);
                var layerMaterial = layerMaterialsList.Find(lm => lm.Id == speckleLayer.Id);
                if (layerId == -1)
                {
                    var layer = new Layer()
                    { Name = speckleLayer.Name,
                        Id = speckleLayer.Id,
                        ParentLayerId = RhinoDoc.ActiveDoc.Layers[ParentLayerId].Id,
                        Color = layerMaterial.Color.ToColor(),
                        IsVisible = layerMaterial.Visible
                     };
                    layerId = RhinoDoc.ActiveDoc.Layers.Add(layer);
                }

                for (int i = speckleLayer.StartIndex; i < speckleLayer.StartIndex + speckleLayer.ObjectCount; i++)
                {
                    LayerIds.Add(layerId);
                    LayerColors.Add(layerMaterial.Color.ToColor());
                }
            }

        }

        /// <summary>
        /// Updates the Rhino Document accorting to the stream contents.
        /// </summary>
        /// <param name="serializedObjectList"></param>
        /// <param name="serializedPropertiesList"></param>
        /// <param name="serializedLayersList"></param>
        /// <param name="serializedLayerMaterialsList"></param>
        public void Update(string serializedObjectList, string serializedPropertiesList, string serializedLayersList, string serializedLayerMaterialsList)
        {
            CreateLayers(serializedLayersList, serializedLayerMaterialsList);

            Debug.WriteLine("Should be Updating Objects");
            if (SerializedObjects != serializedObjectList)
            {
                SerializedObjects = serializedObjectList;
                
                var objectList = JsonConvert.DeserializeObject<List<dynamic>>(SerializedObjects);
                //var propertiesList = JsonConvert.DeserializeObject<List<dynamic>>(serializedPropertiesList);

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

                for (int i = 0; i < Geometry.Count; i++)
                    Ids.Add(RhinoDoc.ActiveDoc.Objects.Add(Geometry[i], new ObjectAttributes() { LayerIndex = LayerIds[i] } ));

                DisplayContents();

            }
           
        }

        /// <summary>
        /// Display the contents of the stream in a display conduit.
        /// </summary>
        public void DisplayContents()
        {
            if (Display != null)
            {

                Display.Geometry = Geometry;
                Display.Colors = LayerColors;

            }
            else
            {
                Display = new SpeckleRhinoDisplayConduit(Geometry, LayerColors) { Enabled = true };
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

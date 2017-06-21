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

        private SpeckleRhinoDisplayConduit Display;
        public string SerializedObjects { get; private set; }
        public string SerializedProperties { get; private set; }

        public List<GeometryBase> Geometry { get; private set; }

        public List<RhinoObject> Objects { get; private set; }

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

        public SpeckleRhinoReceiverWorker(string id, string serializedObjectList, string serializedPropertiesList) : this()
        {
            Id = id;
            Geometry = new List<GeometryBase>();
            Objects = new List<RhinoObject>();
            Update(serializedObjectList, serializedPropertiesList);
            
        }

        #endregion Constructors

        #region Methods

        public bool Equals(SpeckleRhinoReceiverWorker other)
        {
            throw new NotImplementedException();
        }
        public void Update(string serializedObjectList, string serializedPropertiesList)
        {
            Debug.WriteLine("Should be Updating Objects");
            if (SerializedObjects != serializedObjectList)
            {
                SerializedObjects = serializedObjectList;
                
                var objectList = JsonConvert.DeserializeObject<List<dynamic>>(SerializedObjects);
                var propertiesList = JsonConvert.DeserializeObject<List<dynamic>>(serializedPropertiesList);

                GhRhConveter c = new GhRhConveter();
                Geometry.Clear();
                foreach (var obj in objectList)
                {
                    string type = (string)obj.type;

                    //Objects.Add(c.encodeObject(obj));

                    switch (type)
                    {
                        case "Mesh":
                        case "Brep":
                        case "Curve":
                            Geometry.Add(c.encodeObject(obj));
                            break;
                        case "Point":
                            Geometry.Add(new Rhino.Geometry.Point(c.encodeObject(obj)));
                            break;
                        case "Polyline":
                            Polyline polyline = c.encodeObject(obj);
                            Geometry.Add(polyline.ToNurbsCurve());
                            break;
                        case "Circle":
                            Circle circle = c.encodeObject(obj);
                            Geometry.Add(circle.ToNurbsCurve());
                            break;
                        case "Rectangle":
                            Rectangle3d rectangle = c.encodeObject(obj);
                            Geometry.Add(rectangle.ToNurbsCurve());
                            break;
                        case "Line":
                            Line line = c.encodeObject(obj);
                            Geometry.Add(line.ToNurbsCurve());
                            break;
                        case "Box":
                            Box box = c.encodeObject(obj);
                            Geometry.Add(box.ToBrep());
                            break;
                        default:
                            RhinoApp.WriteLine("{0}", obj);
                            break;
                    }


                }

                DisplayContents();

            }
            //throw new NotImplementedException();
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

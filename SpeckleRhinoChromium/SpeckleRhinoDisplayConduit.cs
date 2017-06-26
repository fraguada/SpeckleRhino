using Rhino.Display;
using System.Collections.Generic;
using System.Drawing;

namespace SpeckleRhino
{
    public class SpeckleRhinoDisplayConduit : Rhino.Display.DisplayConduit
    {
        public List<Rhino.Geometry.GeometryBase> Geometry { get; set; }

        public List<Color> Colors { get; set; }

        public SpeckleRhinoDisplayConduit() { }

        public SpeckleRhinoDisplayConduit(List<Rhino.Geometry.GeometryBase> geometry):this()
        {
            Geometry = geometry;
        }

        public SpeckleRhinoDisplayConduit(List<Rhino.Geometry.GeometryBase> geometry, List<Color> colors) : this()
        {
            Geometry = geometry;
            Colors = colors;
        }

        protected override void CalculateBoundingBox(CalculateBoundingBoxEventArgs e)
        {
            Rhino.Geometry.BoundingBox bbox = Rhino.Geometry.BoundingBox.Unset;
            if (null != Geometry)
            {
                foreach (var obj in Geometry)
                    bbox.Union(obj.GetBoundingBox(false));
                e.IncludeBoundingBox(bbox);
            }

        }

        protected override void CalculateBoundingBoxZoomExtents(CalculateBoundingBoxEventArgs e)
        {
            Rhino.Geometry.BoundingBox bbox = Rhino.Geometry.BoundingBox.Unset;
            if (null != Geometry)
            {
                foreach (var obj in Geometry)
                    bbox.Union(obj.GetBoundingBox(false));
                e.IncludeBoundingBox(bbox);
            }
        }

        protected override void PostDrawObjects(DrawEventArgs e)
        {
            base.PostDrawObjects(e);
            var cnt = 0;
            if (null != Geometry)
                foreach (var obj in Geometry)
                {
                    switch (obj.ObjectType)
                    {
                        case Rhino.DocObjects.ObjectType.Point:
                            e.Display.DrawPoint((obj as Rhino.Geometry.Point).Location, Colors[cnt]);
                            break;
                        case Rhino.DocObjects.ObjectType.Curve:
                            e.Display.DrawCurve((obj as Rhino.Geometry.Curve), Colors[cnt]);
                            break;
                        case Rhino.DocObjects.ObjectType.Mesh:
                            Rhino.Display.DisplayMaterial material = new Rhino.Display.DisplayMaterial(Colors[cnt], 0.5);
                            e.Display.DrawMeshShaded((obj as Rhino.Geometry.Mesh), material);
                            e.Display.DrawMeshWires((obj as Rhino.Geometry.Mesh), Colors[cnt]);
                            break;
                        case Rhino.DocObjects.ObjectType.Brep:
                            Rhino.Display.DisplayMaterial materialBrep = new Rhino.Display.DisplayMaterial(Colors[cnt], 0.5);
                            e.Display.DrawBrepShaded((obj as Rhino.Geometry.Brep), materialBrep);
                            e.Display.DrawBrepWires((obj as Rhino.Geometry.Brep), Colors[cnt]);
                            break;
                        default:
                            Rhino.RhinoApp.WriteLine("SpeckleRhino: " + obj.ObjectType.ToString() + " is not supported");
                            break;
                    }

                    cnt++;

                }
        }
    }

   
}

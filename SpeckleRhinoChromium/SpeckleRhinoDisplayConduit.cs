﻿using Rhino.Display;
using System.Collections.Generic;

namespace SpeckleRhino
{
    public class SpeckleRhinoDisplayConduit : Rhino.Display.DisplayConduit
    {
        public List<Rhino.Geometry.GeometryBase> Objects { get; private set; }

        public SpeckleRhinoDisplayConduit() { }

        protected override void CalculateBoundingBox(CalculateBoundingBoxEventArgs e)
        {
            Rhino.Geometry.BoundingBox bbox = Rhino.Geometry.BoundingBox.Unset;
            if (null != Objects)
            {
                foreach (var obj in Objects)
                    bbox.Union(obj.GetBoundingBox(false));
                e.IncludeBoundingBox(bbox);
            }

        }

        protected override void CalculateBoundingBoxZoomExtents(CalculateBoundingBoxEventArgs e)
        {
            Rhino.Geometry.BoundingBox bbox = Rhino.Geometry.BoundingBox.Unset;
            if (null != Objects)
            {
                foreach (var obj in Objects)
                    bbox.Union(obj.GetBoundingBox(false));
                e.IncludeBoundingBox(bbox);
            }
        }

        protected override void PostDrawObjects(DrawEventArgs e)
        {
            if (null != Objects)
                foreach (var obj in Objects)
                {
                    switch (obj.ObjectType)
                    {
                        case Rhino.DocObjects.ObjectType.Point:
                            e.Display.DrawPoint((obj as Rhino.Geometry.Point).Location, System.Drawing.Color.Magenta);
                            break;
                        case Rhino.DocObjects.ObjectType.Curve:
                            e.Display.DrawCurve((obj as Rhino.Geometry.Curve), System.Drawing.Color.Magenta);
                            break;
                        case Rhino.DocObjects.ObjectType.Mesh:
                            Rhino.Display.DisplayMaterial material = new Rhino.Display.DisplayMaterial(System.Drawing.Color.Magenta, 0.5);
                            e.Display.DrawMeshShaded((obj as Rhino.Geometry.Mesh), material);
                            break;
                        case Rhino.DocObjects.ObjectType.Brep:
                            Rhino.Display.DisplayMaterial materialBrep = new Rhino.Display.DisplayMaterial(System.Drawing.Color.Magenta, 0.5);
                            e.Display.DrawMeshShaded((obj as Rhino.Geometry.Mesh), materialBrep);
                            break;
                        default:
                            Rhino.RhinoApp.WriteLine("SpeckleRhino: " + obj.ObjectType.ToString() + " is not supported");
                            break;
                    }
                }
        }
    }

   
}
using System;
using PaintingApp.Contracts;
using PaintingApp.Models;
using Windows.Foundation;

namespace PaintingApp.Services.Resizers;

public class OvalResizer : IShapeResizer
{
    public bool PreserveAspectRatio => false;

    public void Resize(ShapeModel shape, ResizeHandle handle, Point anchorPoint, Point currentPoint)
    {
        if (shape is not OvalModel oval) return;

        var deltaX = currentPoint.X - anchorPoint.X;
        var deltaY = currentPoint.Y - anchorPoint.Y;

        switch (handle)
        {
            case ResizeHandle.TopLeft:
                oval.Center = new Point(oval.Center.X + deltaX / 2, oval.Center.Y + deltaY / 2);
                oval.RadiusX = Math.Max(5, oval.RadiusX - deltaX / 2);
                oval.RadiusY = Math.Max(5, oval.RadiusY - deltaY / 2);
                break;

            case ResizeHandle.TopCenter:
                oval.Center = new Point(oval.Center.X, oval.Center.Y + deltaY / 2);
                oval.RadiusY = Math.Max(5, oval.RadiusY - deltaY / 2);
                break;

            case ResizeHandle.TopRight:
                oval.Center = new Point(oval.Center.X + deltaX / 2, oval.Center.Y + deltaY / 2);
                oval.RadiusX = Math.Max(5, oval.RadiusX + deltaX / 2);
                oval.RadiusY = Math.Max(5, oval.RadiusY - deltaY / 2);
                break;

            case ResizeHandle.MiddleLeft:
                oval.Center = new Point(oval.Center.X + deltaX / 2, oval.Center.Y);
                oval.RadiusX = Math.Max(5, oval.RadiusX - deltaX / 2);
                break;

            case ResizeHandle.MiddleRight:
                oval.Center = new Point(oval.Center.X + deltaX / 2, oval.Center.Y);
                oval.RadiusX = Math.Max(5, oval.RadiusX + deltaX / 2);
                break;

            case ResizeHandle.BottomLeft:
                oval.Center = new Point(oval.Center.X + deltaX / 2, oval.Center.Y + deltaY / 2);
                oval.RadiusX = Math.Max(5, oval.RadiusX - deltaX / 2);
                oval.RadiusY = Math.Max(5, oval.RadiusY + deltaY / 2);
                break;

            case ResizeHandle.BottomCenter:
                oval.Center = new Point(oval.Center.X, oval.Center.Y + deltaY / 2);
                oval.RadiusY = Math.Max(5, oval.RadiusY + deltaY / 2);
                break;

            case ResizeHandle.BottomRight:
                oval.Center = new Point(oval.Center.X + deltaX / 2, oval.Center.Y + deltaY / 2);
                oval.RadiusX = Math.Max(5, oval.RadiusX + deltaX / 2);
                oval.RadiusY = Math.Max(5, oval.RadiusY + deltaY / 2);
                break;
        }
    }
}

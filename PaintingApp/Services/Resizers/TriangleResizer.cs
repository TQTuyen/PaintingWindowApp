using System;
using System.Linq;
using PaintingApp.Contracts;
using PaintingApp.Models;
using Windows.Foundation;

namespace PaintingApp.Services.Resizers;

public class TriangleResizer : IShapeResizer
{
    public bool PreserveAspectRatio => false;

    public void Resize(ShapeModel shape, ResizeHandle handle, Point anchorPoint, Point currentPoint)
    {
        if (shape is not TriangleModel triangle) return;

        var bounds = triangle.GetBounds();
        var deltaX = currentPoint.X - anchorPoint.X;
        var deltaY = currentPoint.Y - anchorPoint.Y;

        var scaleX = 1.0;
        var scaleY = 1.0;
        var offsetX = 0.0;
        var offsetY = 0.0;

        switch (handle)
        {
            case ResizeHandle.TopLeft:
                scaleX = Math.Max(0.1, (bounds.Width - deltaX) / bounds.Width);
                scaleY = Math.Max(0.1, (bounds.Height - deltaY) / bounds.Height);
                offsetX = deltaX;
                offsetY = deltaY;
                break;

            case ResizeHandle.TopCenter:
                scaleY = Math.Max(0.1, (bounds.Height - deltaY) / bounds.Height);
                offsetY = deltaY;
                break;

            case ResizeHandle.TopRight:
                scaleX = Math.Max(0.1, (bounds.Width + deltaX) / bounds.Width);
                scaleY = Math.Max(0.1, (bounds.Height - deltaY) / bounds.Height);
                offsetY = deltaY;
                break;

            case ResizeHandle.MiddleLeft:
                scaleX = Math.Max(0.1, (bounds.Width - deltaX) / bounds.Width);
                offsetX = deltaX;
                break;

            case ResizeHandle.MiddleRight:
                scaleX = Math.Max(0.1, (bounds.Width + deltaX) / bounds.Width);
                break;

            case ResizeHandle.BottomLeft:
                scaleX = Math.Max(0.1, (bounds.Width - deltaX) / bounds.Width);
                scaleY = Math.Max(0.1, (bounds.Height + deltaY) / bounds.Height);
                offsetX = deltaX;
                break;

            case ResizeHandle.BottomCenter:
                scaleY = Math.Max(0.1, (bounds.Height + deltaY) / bounds.Height);
                break;

            case ResizeHandle.BottomRight:
                scaleX = Math.Max(0.1, (bounds.Width + deltaX) / bounds.Width);
                scaleY = Math.Max(0.1, (bounds.Height + deltaY) / bounds.Height);
                break;
        }

        triangle.Point1 = ScalePoint(triangle.Point1, bounds, scaleX, scaleY, offsetX, offsetY);
        triangle.Point2 = ScalePoint(triangle.Point2, bounds, scaleX, scaleY, offsetX, offsetY);
        triangle.Point3 = ScalePoint(triangle.Point3, bounds, scaleX, scaleY, offsetX, offsetY);
    }

    private static Point ScalePoint(Point point, Rect bounds, double scaleX, double scaleY, double offsetX, double offsetY)
    {
        var relX = (point.X - bounds.X) / bounds.Width;
        var relY = (point.Y - bounds.Y) / bounds.Height;

        var newWidth = bounds.Width * scaleX;
        var newHeight = bounds.Height * scaleY;

        return new Point(
            bounds.X + offsetX + relX * newWidth,
            bounds.Y + offsetY + relY * newHeight
        );
    }
}

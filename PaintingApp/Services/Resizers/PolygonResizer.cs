using System;
using System.Linq;
using PaintingApp.Contracts;
using PaintingApp.Models;
using Windows.Foundation;

namespace PaintingApp.Services.Resizers;

public class PolygonResizer : IShapeResizer
{
    public bool PreserveAspectRatio => false;

    public void Resize(ShapeModel shape, ResizeHandle handle, Point anchorPoint, Point currentPoint)
    {
        if (shape is not PolygonModel polygon || polygon.Points.Count == 0) return;

        var bounds = polygon.GetBounds();
        if (bounds.Width <= 0 || bounds.Height <= 0) return;

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

        for (int i = 0; i < polygon.Points.Count; i++)
        {
            polygon.Points[i] = ScalePoint(polygon.Points[i], bounds, scaleX, scaleY, offsetX, offsetY);
        }
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

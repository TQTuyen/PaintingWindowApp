using System;
using PaintingApp.Contracts;
using PaintingApp.Models;
using Windows.Foundation;

namespace PaintingApp.Services.Resizers;

public class RectangleResizer : IShapeResizer
{
    public bool PreserveAspectRatio => false;

    public void Resize(ShapeModel shape, ResizeHandle handle, Point anchorPoint, Point currentPoint)
    {
        if (shape is not RectangleModel rect) return;

        var deltaX = currentPoint.X - anchorPoint.X;
        var deltaY = currentPoint.Y - anchorPoint.Y;

        switch (handle)
        {
            case ResizeHandle.TopLeft:
                rect.TopLeft = new Point(rect.TopLeft.X + deltaX, rect.TopLeft.Y + deltaY);
                rect.Width = Math.Max(10, rect.Width - deltaX);
                rect.Height = Math.Max(10, rect.Height - deltaY);
                break;

            case ResizeHandle.TopCenter:
                rect.TopLeft = new Point(rect.TopLeft.X, rect.TopLeft.Y + deltaY);
                rect.Height = Math.Max(10, rect.Height - deltaY);
                break;

            case ResizeHandle.TopRight:
                rect.TopLeft = new Point(rect.TopLeft.X, rect.TopLeft.Y + deltaY);
                rect.Width = Math.Max(10, rect.Width + deltaX);
                rect.Height = Math.Max(10, rect.Height - deltaY);
                break;

            case ResizeHandle.MiddleLeft:
                rect.TopLeft = new Point(rect.TopLeft.X + deltaX, rect.TopLeft.Y);
                rect.Width = Math.Max(10, rect.Width - deltaX);
                break;

            case ResizeHandle.MiddleRight:
                rect.Width = Math.Max(10, rect.Width + deltaX);
                break;

            case ResizeHandle.BottomLeft:
                rect.TopLeft = new Point(rect.TopLeft.X + deltaX, rect.TopLeft.Y);
                rect.Width = Math.Max(10, rect.Width - deltaX);
                rect.Height = Math.Max(10, rect.Height + deltaY);
                break;

            case ResizeHandle.BottomCenter:
                rect.Height = Math.Max(10, rect.Height + deltaY);
                break;

            case ResizeHandle.BottomRight:
                rect.Width = Math.Max(10, rect.Width + deltaX);
                rect.Height = Math.Max(10, rect.Height + deltaY);
                break;
        }
    }
}

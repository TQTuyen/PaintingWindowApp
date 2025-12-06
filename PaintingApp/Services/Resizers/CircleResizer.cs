using System;
using PaintingApp.Contracts;
using PaintingApp.Models;
using Windows.Foundation;

namespace PaintingApp.Services.Resizers;

public class CircleResizer : IShapeResizer
{
    public bool PreserveAspectRatio => true;

    public void Resize(ShapeModel shape, ResizeHandle handle, Point anchorPoint, Point currentPoint)
    {
        if (shape is not CircleModel circle) return;

        var deltaX = currentPoint.X - anchorPoint.X;
        var deltaY = currentPoint.Y - anchorPoint.Y;

        switch (handle)
        {
            case ResizeHandle.TopLeft:
                var deltaTL = Math.Max(Math.Abs(deltaX), Math.Abs(deltaY));
                if (deltaX > 0 || deltaY > 0) deltaTL = -deltaTL;
                circle.Radius = Math.Max(5, circle.Radius - deltaTL / 2);
                break;

            case ResizeHandle.TopRight:
                var deltaTR = Math.Max(Math.Abs(deltaX), Math.Abs(deltaY));
                if (deltaX < 0 || deltaY > 0) deltaTR = -deltaTR;
                circle.Radius = Math.Max(5, circle.Radius + deltaTR / 2);
                break;

            case ResizeHandle.BottomLeft:
                var deltaBL = Math.Max(Math.Abs(deltaX), Math.Abs(deltaY));
                if (deltaX > 0 || deltaY < 0) deltaBL = -deltaBL;
                circle.Radius = Math.Max(5, circle.Radius - deltaBL / 2);
                break;

            case ResizeHandle.BottomRight:
                var deltaBR = Math.Max(Math.Abs(deltaX), Math.Abs(deltaY));
                if (deltaX < 0 || deltaY < 0) deltaBR = -deltaBR;
                circle.Radius = Math.Max(5, circle.Radius + deltaBR / 2);
                break;

            case ResizeHandle.TopCenter:
                circle.Center = new Point(circle.Center.X, circle.Center.Y + deltaY / 2);
                circle.Radius = Math.Max(5, circle.Radius - deltaY / 2);
                break;

            case ResizeHandle.MiddleLeft:
                circle.Center = new Point(circle.Center.X + deltaX / 2, circle.Center.Y);
                circle.Radius = Math.Max(5, circle.Radius - deltaX / 2);
                break;

            case ResizeHandle.MiddleRight:
                circle.Center = new Point(circle.Center.X + deltaX / 2, circle.Center.Y);
                circle.Radius = Math.Max(5, circle.Radius + deltaX / 2);
                break;

            case ResizeHandle.BottomCenter:
                circle.Center = new Point(circle.Center.X, circle.Center.Y + deltaY / 2);
                circle.Radius = Math.Max(5, circle.Radius + deltaY / 2);
                break;
        }
    }
}

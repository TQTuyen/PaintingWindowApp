using System;
using PaintingApp.Contracts;
using PaintingApp.Models;
using Windows.Foundation;
using Windows.UI;

namespace PaintingApp.Services.Factories;

public class OvalFactory : IShapeFactory
{
    public ShapeModel CreateShape(Point startPoint, Color strokeColor, double strokeThickness, Color fillColor)
    {
        return new OvalModel
        {
            Center = startPoint,
            RadiusX = 0,
            RadiusY = 0,
            StrokeColor = strokeColor,
            StrokeThickness = strokeThickness,
            FillColor = fillColor
        };
    }

    public void UpdateShape(ShapeModel shape, Point startPoint, Point currentPoint)
    {
        if (shape is OvalModel oval)
        {
            oval.Center = new Point(
                (startPoint.X + currentPoint.X) / 2,
                (startPoint.Y + currentPoint.Y) / 2
            );
            oval.RadiusX = Math.Abs(currentPoint.X - startPoint.X) / 2;
            oval.RadiusY = Math.Abs(currentPoint.Y - startPoint.Y) / 2;
        }
    }
}

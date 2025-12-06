using System;
using PaintingApp.Contracts;
using PaintingApp.Models;
using Windows.Foundation;
using Windows.UI;

namespace PaintingApp.Services.Factories;

public class CircleFactory : IShapeFactory
{
    public ShapeModel CreateShape(Point startPoint, Color strokeColor, double strokeThickness, Color fillColor)
    {
        return new CircleModel
        {
            Center = startPoint,
            Radius = 0,
            StrokeColor = strokeColor,
            StrokeThickness = strokeThickness,
            FillColor = fillColor
        };
    }

    public void UpdateShape(ShapeModel shape, Point startPoint, Point currentPoint)
    {
        if (shape is CircleModel circle)
        {
            var dx = currentPoint.X - startPoint.X;
            var dy = currentPoint.Y - startPoint.Y;
            circle.Radius = Math.Sqrt(dx * dx + dy * dy);
        }
    }
}

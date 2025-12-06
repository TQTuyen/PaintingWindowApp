using System;
using PaintingApp.Contracts;
using PaintingApp.Models;
using Windows.Foundation;
using Windows.UI;

namespace PaintingApp.Services.Factories;

public class RectangleFactory : IShapeFactory
{
    public ShapeModel CreateShape(Point startPoint, Color strokeColor, double strokeThickness, Color fillColor)
    {
        return new RectangleModel
        {
            TopLeft = startPoint,
            Width = 0,
            Height = 0,
            StrokeColor = strokeColor,
            StrokeThickness = strokeThickness,
            FillColor = fillColor
        };
    }

    public void UpdateShape(ShapeModel shape, Point startPoint, Point currentPoint)
    {
        if (shape is RectangleModel rect)
        {
            rect.TopLeft = new Point(
                Math.Min(startPoint.X, currentPoint.X),
                Math.Min(startPoint.Y, currentPoint.Y)
            );
            rect.Width = Math.Abs(currentPoint.X - startPoint.X);
            rect.Height = Math.Abs(currentPoint.Y - startPoint.Y);
        }
    }
}

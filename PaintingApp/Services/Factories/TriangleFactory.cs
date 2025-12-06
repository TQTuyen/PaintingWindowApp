using PaintingApp.Contracts;
using PaintingApp.Models;
using Windows.Foundation;
using Windows.UI;

namespace PaintingApp.Services.Factories;

public class TriangleFactory : IShapeFactory
{
    public ShapeModel CreateShape(Point startPoint, Color strokeColor, double strokeThickness, Color fillColor)
    {
        return new TriangleModel
        {
            Point1 = startPoint,
            Point2 = startPoint,
            Point3 = startPoint,
            StrokeColor = strokeColor,
            StrokeThickness = strokeThickness,
            FillColor = fillColor
        };
    }

    public void UpdateShape(ShapeModel shape, Point startPoint, Point currentPoint)
    {
        if (shape is TriangleModel triangle)
        {
            var width = currentPoint.X - startPoint.X;
            var height = currentPoint.Y - startPoint.Y;

            triangle.Point1 = new Point(startPoint.X + width / 2, startPoint.Y);
            triangle.Point2 = new Point(startPoint.X, startPoint.Y + height);
            triangle.Point3 = new Point(currentPoint.X, currentPoint.Y);
        }
    }
}

using System.Collections.Generic;
using PaintingApp.Contracts;
using PaintingApp.Models;
using Windows.Foundation;
using Windows.UI;

namespace PaintingApp.Services.Factories;

public class PolygonFactory : IShapeFactory
{
    public ShapeModel CreateShape(Point startPoint, Color strokeColor, double strokeThickness, Color fillColor)
    {
        return new PolygonModel
        {
            Points = [startPoint, startPoint, startPoint, startPoint, startPoint],
            StrokeColor = strokeColor,
            StrokeThickness = strokeThickness,
            FillColor = fillColor
        };
    }

    public void UpdateShape(ShapeModel shape, Point startPoint, Point currentPoint)
    {
        if (shape is PolygonModel polygon)
        {
            var centerX = (startPoint.X + currentPoint.X) / 2;
            var centerY = (startPoint.Y + currentPoint.Y) / 2;
            var radiusX = System.Math.Abs(currentPoint.X - startPoint.X) / 2;
            var radiusY = System.Math.Abs(currentPoint.Y - startPoint.Y) / 2;

            // Create a regular pentagon
            const int sides = 5;
            var points = new List<Point>();

            for (int i = 0; i < sides; i++)
            {
                var angle = 2 * System.Math.PI * i / sides - System.Math.PI / 2;
                points.Add(new Point(
                    centerX + radiusX * System.Math.Cos(angle),
                    centerY + radiusY * System.Math.Sin(angle)
                ));
            }

            polygon.Points = points;
        }
    }
}

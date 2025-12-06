using PaintingApp.Contracts;
using PaintingApp.Models;
using Windows.Foundation;
using Windows.UI;

namespace PaintingApp.Services.Factories;

public class LineFactory : IShapeFactory
{
    public ShapeModel CreateShape(Point startPoint, Color strokeColor, double strokeThickness, Color fillColor)
    {
        return new LineModel
        {
            StartPoint = startPoint,
            EndPoint = startPoint,
            StrokeColor = strokeColor,
            StrokeThickness = strokeThickness,
            FillColor = fillColor
        };
    }

    public void UpdateShape(ShapeModel shape, Point startPoint, Point currentPoint)
    {
        if (shape is LineModel line)
        {
            line.EndPoint = currentPoint;
        }
    }
}

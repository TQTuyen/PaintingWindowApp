using PaintingApp.Models;
using Windows.Foundation;
using Windows.UI;

namespace PaintingApp.Contracts;

public interface IShapeFactory
{
    ShapeModel CreateShape(Point startPoint, Color strokeColor, double strokeThickness, Color fillColor);

    void UpdateShape(ShapeModel shape, Point startPoint, Point currentPoint);
}

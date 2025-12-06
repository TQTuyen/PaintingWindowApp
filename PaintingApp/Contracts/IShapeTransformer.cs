using PaintingApp.Models;
using Windows.Foundation;

namespace PaintingApp.Contracts;

public interface IShapeTransformer
{
    void Translate(ShapeModel shape, double deltaX, double deltaY);
    Point GetReferencePoint(ShapeModel shape);
}

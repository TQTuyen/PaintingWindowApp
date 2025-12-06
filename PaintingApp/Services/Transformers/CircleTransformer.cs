using PaintingApp.Contracts;
using PaintingApp.Models;
using Windows.Foundation;

namespace PaintingApp.Services.Transformers;

public class CircleTransformer : IShapeTransformer
{
    public void Translate(ShapeModel shape, double deltaX, double deltaY)
    {
        if (shape is CircleModel circle)
        {
            circle.Center = new Point(
                circle.Center.X + deltaX,
                circle.Center.Y + deltaY
            );
        }
    }

    public Point GetReferencePoint(ShapeModel shape)
    {
        if (shape is CircleModel circle)
        {
            return circle.Center;
        }
        return new Point(0, 0);
    }
}

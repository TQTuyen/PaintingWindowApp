using PaintingApp.Contracts;
using PaintingApp.Models;
using Windows.Foundation;

namespace PaintingApp.Services.Transformers;

public class RectangleTransformer : IShapeTransformer
{
    public void Translate(ShapeModel shape, double deltaX, double deltaY)
    {
        if (shape is RectangleModel rect)
        {
            rect.TopLeft = new Point(
                rect.TopLeft.X + deltaX,
                rect.TopLeft.Y + deltaY
            );
        }
    }

    public Point GetReferencePoint(ShapeModel shape)
    {
        if (shape is RectangleModel rect)
        {
            return rect.TopLeft;
        }
        return new Point(0, 0);
    }
}

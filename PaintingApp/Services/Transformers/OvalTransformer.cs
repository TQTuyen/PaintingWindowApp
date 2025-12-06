using PaintingApp.Contracts;
using PaintingApp.Models;
using Windows.Foundation;

namespace PaintingApp.Services.Transformers;

public class OvalTransformer : IShapeTransformer
{
    public void Translate(ShapeModel shape, double deltaX, double deltaY)
    {
        if (shape is OvalModel oval)
        {
            oval.Center = new Point(
                oval.Center.X + deltaX,
                oval.Center.Y + deltaY
            );
        }
    }

    public Point GetReferencePoint(ShapeModel shape)
    {
        if (shape is OvalModel oval)
        {
            return oval.Center;
        }
        return new Point(0, 0);
    }
}

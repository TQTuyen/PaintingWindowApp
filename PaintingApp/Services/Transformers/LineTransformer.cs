using PaintingApp.Contracts;
using PaintingApp.Models;
using Windows.Foundation;

namespace PaintingApp.Services.Transformers;

public class LineTransformer : IShapeTransformer
{
    public void Translate(ShapeModel shape, double deltaX, double deltaY)
    {
        if (shape is LineModel line)
        {
            line.StartPoint = new Point(
                line.StartPoint.X + deltaX,
                line.StartPoint.Y + deltaY
            );
            line.EndPoint = new Point(
                line.EndPoint.X + deltaX,
                line.EndPoint.Y + deltaY
            );
        }
    }

    public Point GetReferencePoint(ShapeModel shape)
    {
        if (shape is LineModel line)
        {
            return line.StartPoint;
        }
        return new Point(0, 0);
    }
}

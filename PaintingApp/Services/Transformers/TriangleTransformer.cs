using PaintingApp.Contracts;
using PaintingApp.Models;
using Windows.Foundation;

namespace PaintingApp.Services.Transformers;

public class TriangleTransformer : IShapeTransformer
{
    public void Translate(ShapeModel shape, double deltaX, double deltaY)
    {
        if (shape is TriangleModel triangle)
        {
            triangle.Point1 = new Point(
                triangle.Point1.X + deltaX,
                triangle.Point1.Y + deltaY
            );
            triangle.Point2 = new Point(
                triangle.Point2.X + deltaX,
                triangle.Point2.Y + deltaY
            );
            triangle.Point3 = new Point(
                triangle.Point3.X + deltaX,
                triangle.Point3.Y + deltaY
            );
        }
    }

    public Point GetReferencePoint(ShapeModel shape)
    {
        if (shape is TriangleModel triangle)
        {
            return triangle.Point1;
        }
        return new Point(0, 0);
    }
}

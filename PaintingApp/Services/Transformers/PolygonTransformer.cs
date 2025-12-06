using PaintingApp.Contracts;
using PaintingApp.Models;
using Windows.Foundation;

namespace PaintingApp.Services.Transformers;

public class PolygonTransformer : IShapeTransformer
{
    public void Translate(ShapeModel shape, double deltaX, double deltaY)
    {
        if (shape is PolygonModel polygon)
        {
            for (int i = 0; i < polygon.Points.Count; i++)
            {
                polygon.Points[i] = new Point(
                    polygon.Points[i].X + deltaX,
                    polygon.Points[i].Y + deltaY
                );
            }
        }
    }

    public Point GetReferencePoint(ShapeModel shape)
    {
        if (shape is PolygonModel polygon && polygon.Points.Count > 0)
        {
            return polygon.Points[0];
        }
        return new Point(0, 0);
    }
}

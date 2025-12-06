using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using PaintingApp.Data.Entities;
using Windows.Foundation;

namespace PaintingApp.Models;

public partial class PolygonModel : ShapeModel
{
    [ObservableProperty]
    private List<Point> _points = [];

    public PolygonModel()
    {
        Type = ShapeType.Polygon;
    }

    public override List<Point> GetPoints()
    {
        return Points;
    }

    public override Rect GetBounds()
    {
        if (Points == null || Points.Count == 0)
            return Rect.Empty;

        var minX = Points.Min(p => p.X);
        var minY = Points.Min(p => p.Y);
        var maxX = Points.Max(p => p.X);
        var maxY = Points.Max(p => p.Y);

        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }
}

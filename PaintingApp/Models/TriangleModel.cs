using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using PaintingApp.Data.Entities;
using Windows.Foundation;

namespace PaintingApp.Models;

public partial class TriangleModel : ShapeModel
{
    [ObservableProperty]
    private Point _point1;

    [ObservableProperty]
    private Point _point2;

    [ObservableProperty]
    private Point _point3;

    public TriangleModel()
    {
        Type = ShapeType.Triangle;
    }

    public override List<Point> GetPoints()
    {
        return [Point1, Point2, Point3];
    }

    public override Rect GetBounds()
    {
        var points = GetPoints();
        var minX = points.Min(p => p.X);
        var minY = points.Min(p => p.Y);
        var maxX = points.Max(p => p.X);
        var maxY = points.Max(p => p.Y);

        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }
}

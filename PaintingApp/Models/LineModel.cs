using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using PaintingApp.Data.Entities;
using Windows.Foundation;

namespace PaintingApp.Models;

public partial class LineModel : ShapeModel
{
    [ObservableProperty]
    private Point _startPoint;

    [ObservableProperty]
    private Point _endPoint;

    public LineModel()
    {
        Type = ShapeType.Line;
    }

    public override List<Point> GetPoints()
    {
        return [StartPoint, EndPoint];
    }

    public override Rect GetBounds()
    {
        var minX = Math.Min(StartPoint.X, EndPoint.X);
        var minY = Math.Min(StartPoint.Y, EndPoint.Y);
        var maxX = Math.Max(StartPoint.X, EndPoint.X);
        var maxY = Math.Max(StartPoint.Y, EndPoint.Y);

        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }
}

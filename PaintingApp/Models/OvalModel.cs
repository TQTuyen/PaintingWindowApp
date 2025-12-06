using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using PaintingApp.Data.Entities;
using Windows.Foundation;

namespace PaintingApp.Models;

public partial class OvalModel : ShapeModel
{
    [ObservableProperty]
    private Point _center;

    [ObservableProperty]
    private double _radiusX;

    [ObservableProperty]
    private double _radiusY;

    public OvalModel()
    {
        Type = ShapeType.Oval;
    }

    public override List<Point> GetPoints()
    {
        var points = new List<Point>();
        const int segments = 36;

        for (int i = 0; i < segments; i++)
        {
            var angle = 2 * Math.PI * i / segments;
            points.Add(new Point(
                Center.X + RadiusX * Math.Cos(angle),
                Center.Y + RadiusY * Math.Sin(angle)
            ));
        }

        return points;
    }

    public override Rect GetBounds()
    {
        return new Rect(
            Center.X - RadiusX,
            Center.Y - RadiusY,
            RadiusX * 2,
            RadiusY * 2
        );
    }
}

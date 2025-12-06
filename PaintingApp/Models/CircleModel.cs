using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using PaintingApp.Data.Entities;
using Windows.Foundation;

namespace PaintingApp.Models;

public partial class CircleModel : ShapeModel
{
    [ObservableProperty]
    private Point _center;

    [ObservableProperty]
    private double _radius;

    public CircleModel()
    {
        Type = ShapeType.Circle;
    }

    public override List<Point> GetPoints()
    {
        var points = new List<Point>();
        const int segments = 36;

        for (int i = 0; i < segments; i++)
        {
            var angle = 2 * Math.PI * i / segments;
            points.Add(new Point(
                Center.X + Radius * Math.Cos(angle),
                Center.Y + Radius * Math.Sin(angle)
            ));
        }

        return points;
    }

    public override Rect GetBounds()
    {
        return new Rect(
            Center.X - Radius,
            Center.Y - Radius,
            Radius * 2,
            Radius * 2
        );
    }
}

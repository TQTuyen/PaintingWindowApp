using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using PaintingApp.Data.Entities;
using Windows.Foundation;

namespace PaintingApp.Models;

public partial class RectangleModel : ShapeModel
{
    [ObservableProperty]
    private Point _topLeft;

    [ObservableProperty]
    private double _width;

    [ObservableProperty]
    private double _height;

    public RectangleModel()
    {
        Type = ShapeType.Rectangle;
    }

    public override List<Point> GetPoints()
    {
        return
        [
            TopLeft,
            new Point(TopLeft.X + Width, TopLeft.Y),
            new Point(TopLeft.X + Width, TopLeft.Y + Height),
            new Point(TopLeft.X, TopLeft.Y + Height)
        ];
    }

    public override Rect GetBounds()
    {
        return new Rect(TopLeft.X, TopLeft.Y, Width, Height);
    }
}

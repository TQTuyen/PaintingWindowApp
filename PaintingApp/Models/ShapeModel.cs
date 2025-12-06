using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI;
using PaintingApp.Data.Entities;
using Windows.Foundation;
using Windows.UI;

namespace PaintingApp.Models;

public abstract partial class ShapeModel : ObservableObject
{
    [ObservableProperty]
    private ShapeType _type;

    [ObservableProperty]
    private Color _strokeColor = Colors.Black;

    [ObservableProperty]
    private double _strokeThickness = 2.0;

    [ObservableProperty]
    private Color _fillColor = Colors.Transparent;

    [ObservableProperty]
    private int _zIndex;

    [ObservableProperty]
    private bool _isSelected;

    public abstract List<Point> GetPoints();

    public abstract Rect GetBounds();
}

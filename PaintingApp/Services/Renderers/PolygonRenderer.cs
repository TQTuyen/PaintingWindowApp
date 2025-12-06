using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using PaintingApp.Contracts;
using PaintingApp.Models;

namespace PaintingApp.Services.Renderers;

public class PolygonRenderer : IShapeRenderer
{
    private readonly IStrokeDashProvider _dashProvider;

    public PolygonRenderer(IStrokeDashProvider dashProvider)
    {
        _dashProvider = dashProvider;
    }

    public void Render(Canvas canvas, ShapeModel shape)
    {
        var points = shape.GetPoints();
        if (points.Count < 3) return;

        var uiPolygon = new Polygon
        {
            Stroke = new SolidColorBrush(shape.StrokeColor),
            StrokeThickness = shape.StrokeThickness,
            StrokeDashArray = _dashProvider.GetDashArray(shape.StrokeDashStyle),
            Fill = new SolidColorBrush(shape.FillColor),
            Tag = shape
        };

        foreach (var point in points)
        {
            uiPolygon.Points.Add(point);
        }

        Canvas.SetZIndex(uiPolygon, shape.ZIndex);
        canvas.Children.Add(uiPolygon);
    }

    public void Update(Canvas canvas, ShapeModel shape)
    {
        Remove(canvas, shape);
        Render(canvas, shape);
    }

    public void Remove(Canvas canvas, ShapeModel shape)
    {
        var element = canvas.Children.FirstOrDefault(c => (c as FrameworkElement)?.Tag == shape);
        if (element != null)
        {
            canvas.Children.Remove(element);
        }
    }
}

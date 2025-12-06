using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using PaintingApp.Contracts;
using PaintingApp.Models;

namespace PaintingApp.Services.Renderers;

public class LineRenderer : IShapeRenderer
{
    private readonly IStrokeDashProvider _dashProvider;

    public LineRenderer(IStrokeDashProvider dashProvider)
    {
        _dashProvider = dashProvider;
    }

    public void Render(Canvas canvas, ShapeModel shape)
    {
        if (shape is not LineModel line) return;

        var uiLine = new Line
        {
            X1 = line.StartPoint.X,
            Y1 = line.StartPoint.Y,
            X2 = line.EndPoint.X,
            Y2 = line.EndPoint.Y,
            Stroke = new SolidColorBrush(line.StrokeColor),
            StrokeThickness = line.StrokeThickness,
            StrokeDashArray = _dashProvider.GetDashArray(line.StrokeDashStyle),
            Tag = shape
        };

        Canvas.SetZIndex(uiLine, line.ZIndex);
        canvas.Children.Add(uiLine);
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

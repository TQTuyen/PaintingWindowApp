using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using PaintingApp.Contracts;
using PaintingApp.Models;

namespace PaintingApp.Services.Renderers;

public class CircleRenderer : IShapeRenderer
{
    public void Render(Canvas canvas, ShapeModel shape)
    {
        if (shape is not CircleModel circle) return;

        var uiEllipse = new Ellipse
        {
            Width = circle.Radius * 2,
            Height = circle.Radius * 2,
            Stroke = new SolidColorBrush(circle.StrokeColor),
            StrokeThickness = circle.StrokeThickness,
            Fill = new SolidColorBrush(circle.FillColor),
            Tag = shape
        };

        Canvas.SetLeft(uiEllipse, circle.Center.X - circle.Radius);
        Canvas.SetTop(uiEllipse, circle.Center.Y - circle.Radius);
        Canvas.SetZIndex(uiEllipse, circle.ZIndex);

        canvas.Children.Add(uiEllipse);
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
